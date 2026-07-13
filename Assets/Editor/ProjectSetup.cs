#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

namespace LighthouseMatch3.Editor
{
    internal static class ProjectSetup
    {
        [InitializeOnLoadMethod]
        private static void ConfigureProject()
        {
            const string scenePath = "Assets/Scenes/Main.unity";
            PlayerSettings.companyName = "Archipelago Studio";
            PlayerSettings.productName = "Lighthouses: Mystery of the Archipelago";
            if (string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion)) PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.karakuts.lighthouses");
            ConfigureReleasePlayerSettings();
            EditorUserBuildSettings.buildAppBundle = true;
            ApplyAndroidIcon();

            if (System.Array.Exists(EditorBuildSettings.scenes, scene => scene.path == scenePath)) return;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
            Debug.Log("Lighthouse Match3 project settings initialized.");
        }

        internal static void ConfigureAndroidGraphics()
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        }

        internal static void ConfigureReleasePlayerSettings()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.usePlayerLog = false;
            PlayerSettings.stripEngineCode = true;
            if (PlayerSettings.Android.bundleVersionCode < 1) PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minifyRelease = true;
            ConfigureAndroidGraphics();
        }

        private static void ApplyAndroidIcon()
        {
            const string iconPath = "Assets/Artwork/AppIcon-v1.png";
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon != null) PlayerSettings.SetIcons(NamedBuildTarget.Android, new[] { icon }, IconKind.Application);
        }
    }
}
#endif
