using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LighthouseMatch3;

internal static class Program
{
    private const string ScriptGuid = "7f4e8a2b1c3d4e5f67890123456789ab";
    private const string OutputFolder = "Assets/Resources/Levels";

    private static int Main()
    {
        try
        {
            Directory.CreateDirectory(OutputFolder);
            for (int id = 1; id <= LevelCatalog.LevelCount; id++)
            {
                LevelDefinition level = ProceduralLevelCatalog.Instance.Get(id);
                string assetPath = Path.Combine(OutputFolder, $"Level_{id:00}.asset");
                string metaPath = assetPath + ".meta";
                File.WriteAllText(assetPath, BuildAssetYaml(level));
                File.WriteAllText(metaPath, BuildMetaYaml(StableGuid($"level-{id:00}")));
            }
            File.WriteAllText(OutputFolder + ".meta", BuildFolderMetaYaml());
            Console.WriteLine($"Generated {LevelCatalog.LevelCount} level assets in {OutputFolder}.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static string BuildAssetYaml(LevelDefinition level) => $@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {ScriptGuid}, type: 3}}
  m_Name: Level_{level.Id:00}
  Id: {level.Id}
  Moves: {level.Moves}
  CollectKind: {(int)level.CollectKind}
  CollectTarget: {level.CollectTarget}
  Blocker: {(int)level.Blocker}
  BlockerCount: {level.BlockerCount}
  StarTwoScore: {level.StarTwoScore}
  StarThreeScore: {level.StarThreeScore}
";

    private static string BuildMetaYaml(string guid) => $@"fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";

    private static string BuildFolderMetaYaml() => @"fileFormatVersion: 2
guid: c4d8e1f2a3b4c5d6e7f8091a2b3c4d5e
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";

    private static string StableGuid(string seed)
    {
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"lighthouses::{seed}"));
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}
