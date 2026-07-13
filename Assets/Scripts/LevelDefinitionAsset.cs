using UnityEngine;

namespace LighthouseMatch3
{
    [CreateAssetMenu(menuName = "Lighthouses/Level Definition", fileName = "Level")]
    public sealed class LevelDefinitionAsset : ScriptableObject
    {
        public int Id = 1;
        public int Moves = 20;
        public TileKind CollectKind;
        public int CollectTarget = 20;
        public BlockerKind Blocker;
        public int BlockerCount;
        public int StarTwoScore = 1600;
        public int StarThreeScore = 2500;

        public LevelDefinition ToDefinition() => new LevelDefinition
        {
            Id = Id,
            Moves = Moves,
            CollectKind = CollectKind,
            CollectTarget = CollectTarget,
            Blocker = Blocker,
            BlockerCount = BlockerCount,
            StarTwoScore = StarTwoScore,
            StarThreeScore = StarThreeScore
        };
    }
}
