#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace LighthouseMatch3.Editor
{
    public static class BuildCommandLine
    {
        public static void BuildAndroid()
        {
            BuildProductionAndroid();
        }

        public static void BuildProductionAndroid()
        {
            ApplyReleaseVersion();
            BuildAndroidBundle($"Release/Lighthouses-{PlayerSettings.bundleVersion}.aab", true);
        }

        public static void BuildValidationAndroid()
        {
            BuildAndroidBundle("Release/Lighthouses-validation.aab", false);
        }

        public static void BuildValidationAndroidForEmulator()
        {
            BuildValidationAndroid();
        }

        private static void BuildAndroidBundle(string outputPath, bool requireUploadKey, BuildOptions options = BuildOptions.None)
        {
            Directory.CreateDirectory("Release");
            EditorUserBuildSettings.buildAppBundle = true;
            ProjectSetup.ConfigureReleasePlayerSettings();
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            ConfigureSigning(requireUploadKey);

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = options
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Android build failed: {report.summary.result}");

            UnityEngine.Debug.Log($"Android App Bundle created at {Path.GetFullPath(outputPath)}");
        }

        private static void ApplyReleaseVersion()
        {
            string versionName = Environment.GetEnvironmentVariable("LIGHTHOUSES_VERSION_NAME");
            string versionCodeText = Environment.GetEnvironmentVariable("LIGHTHOUSES_VERSION_CODE");
            if (string.IsNullOrWhiteSpace(versionName) || !int.TryParse(versionCodeText, out int versionCode) || versionCode < 1)
                throw new BuildFailedException("Production builds require LIGHTHOUSES_VERSION_NAME and a positive LIGHTHOUSES_VERSION_CODE.");

            PlayerSettings.bundleVersion = versionName;
            PlayerSettings.Android.bundleVersionCode = versionCode;
        }

        private static void ConfigureSigning(bool requireUploadKey)
        {
            string path = Environment.GetEnvironmentVariable("LIGHTHOUSES_KEYSTORE_PATH");
            string storePassword = Environment.GetEnvironmentVariable("LIGHTHOUSES_KEYSTORE_PASSWORD");
            string alias = Environment.GetEnvironmentVariable("LIGHTHOUSES_KEY_ALIAS");
            string keyPassword = Environment.GetEnvironmentVariable("LIGHTHOUSES_KEY_PASSWORD");
            bool hasUploadKey = !string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(storePassword)
                && !string.IsNullOrWhiteSpace(alias) && !string.IsNullOrWhiteSpace(keyPassword);

            if (!hasUploadKey)
            {
                PlayerSettings.Android.useCustomKeystore = false;
                if (requireUploadKey)
                    throw new BuildFailedException("Production builds require the four LIGHTHOUSES_KEYSTORE_* upload-key environment variables.");
                return;
            }

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = path;
            PlayerSettings.Android.keystorePass = storePassword;
            PlayerSettings.Android.keyaliasName = alias;
            PlayerSettings.Android.keyaliasPass = keyPassword;
        }
    }
}
#endif
