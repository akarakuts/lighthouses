#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LighthouseMatch3.Editor
{
    public static class TmpResourceImporter
    {
        public static void ImportEssentialResourcesBatch()
        {
            if (AssetDatabase.FindAssets("t:TMP_Settings").Length > 0)
            {
                Debug.Log("TMP Essential Resources already present.");
                return;
            }

            string packagePath = Path.Combine(
                EditorApplication.applicationContentsPath,
                "Resources/PackageManager/BuiltInPackages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage");

            if (!File.Exists(packagePath))
                throw new FileNotFoundException("TMP Essential Resources package not found.", packagePath);

            AssetDatabase.ImportPackage(packagePath, false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("TMP Essential Resources imported.");
        }
    }
}
#endif
