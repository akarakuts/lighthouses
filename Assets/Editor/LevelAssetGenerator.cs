#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LighthouseMatch3.Editor
{
    public static class LevelAssetGenerator
    {
        private const string LevelsFolder = "Assets/Resources/Levels";

        [InitializeOnLoadMethod]
        private static void EnsureLevelAssetsOnLoad()
        {
            if (AssetDatabase.LoadAssetAtPath<LevelDefinitionAsset>($"{LevelsFolder}/Level_01.asset") != null) return;
            GenerateAllLevels();
        }

        [MenuItem("Lighthouses/Generate Level Assets")]
        public static void GenerateAllLevels()
        {
            Directory.CreateDirectory(LevelsFolder);
            for (int id = 1; id <= LevelCatalog.LevelCount; id++)
            {
                LevelDefinition procedural = ProceduralLevelCatalog.Instance.Get(id);
                string path = $"{LevelsFolder}/Level_{id:00}.asset";
                LevelDefinitionAsset asset = AssetDatabase.LoadAssetAtPath<LevelDefinitionAsset>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<LevelDefinitionAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                }
                asset.Id = procedural.Id;
                asset.Moves = procedural.Moves;
                asset.CollectKind = procedural.CollectKind;
                asset.CollectTarget = procedural.CollectTarget;
                asset.Blocker = procedural.Blocker;
                asset.BlockerCount = procedural.BlockerCount;
                asset.StarTwoScore = procedural.StarTwoScore;
                asset.StarThreeScore = procedural.StarThreeScore;
                EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated {LevelCatalog.LevelCount} level assets in {LevelsFolder}.");
        }
    }
}
#endif
