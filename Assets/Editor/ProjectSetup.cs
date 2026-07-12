#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

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
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.archipelagostudio.lighthouses");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            if (PlayerSettings.Android.bundleVersionCode < 1) PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            EditorUserBuildSettings.buildAppBundle = true;
            ApplyAndroidIcon();

            if (System.Array.Exists(EditorBuildSettings.scenes, scene => scene.path == scenePath)) return;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
            Debug.Log("Lighthouse Match3 project settings initialized.");
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
