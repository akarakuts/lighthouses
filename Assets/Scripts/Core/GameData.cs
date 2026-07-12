using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public enum TileKind { Coral, Shell, Drop, Sunstone, Starfish, Crystal }
    public enum SpecialKind { None, Beam, Bomb, Pearl }
    public enum BlockerKind { None, Ice, Crate, Seaweed }

    [Serializable]
    public sealed class TileState
    {
        public TileKind Kind;
        public SpecialKind Special;

        public TileState(TileKind kind)
        {
            Kind = kind;
        }

        public TileState Clone()
        {
            return new TileState(Kind) { Special = Special };
        }
    }

    [Serializable]
    public sealed class BlockerState
    {
        public BlockerKind Kind;
        public int HitsRemaining;

        public BlockerState(BlockerKind kind)
        {
            Kind = kind;
            HitsRemaining = BlockerRules.RequiredHits(kind);
        }

        public BlockerState Clone()
        {
            return new BlockerState(Kind) { HitsRemaining = HitsRemaining };
        }
    }

    [Serializable]
    public sealed class LevelDefinition
    {
        public int Id;
        public int Moves;
        public TileKind CollectKind;
        public int CollectTarget;
        public BlockerKind Blocker;
        public int BlockerCount;
        public int StarTwoScore;
        public int StarThreeScore;
    }

    public static class LevelCatalog
    {
        public const int BoardSize = 8;
        public const int LevelCount = 20;

        private static ILevelCatalog _source = ProceduralLevelCatalog.Instance;

        public static ILevelCatalog Source
        {
            get => _source;
            set => _source = value ?? ProceduralLevelCatalog.Instance;
        }

        public static LevelDefinition Get(int id) => _source.Get(id);

        public static void ResetSource() => _source = ProceduralLevelCatalog.Instance;
    }

    public interface ILevelCatalog
    {
        LevelDefinition Get(int id);
    }

    public sealed class ProceduralLevelCatalog : ILevelCatalog
    {
        public static ProceduralLevelCatalog Instance { get; } = new ProceduralLevelCatalog();

        public LevelDefinition Get(int id)
        {
            int chapter = (id - 1) / 5;
            BlockerKind blocker = id < 4 ? BlockerKind.None : (BlockerKind)((id - 4) % 3 + 1);
            return new LevelDefinition
            {
                Id = id,
                Moves = Math.Max(15, 24 - chapter - id / 7),
                CollectKind = (TileKind)((id - 1) % 6),
                CollectTarget = 18 + id * 2,
                Blocker = blocker,
                BlockerCount = blocker == BlockerKind.None ? 0 : Math.Min(18, 4 + id),
                StarTwoScore = 1300 + id * 300,
                StarThreeScore = 2200 + id * 450
            };
        }
    }

    [Serializable]
    public sealed class PlayerProgress
    {
        public int UnlockedLevel = 1;
        public int Coins = 120;
        public int Stars;
        public int LighthouseStage;
        public int Lives = 5;
        public long LastLifeUtcTicks;
        public string LastRewardUtcDate = string.Empty;
        public int DailyRewardStreak;
        public bool SoundEnabled = true;
        public bool HapticsEnabled = true;
        public string LanguageCode = string.Empty;
        public bool HasSeenTutorial;
        public List<int> CompletedLevels = new List<int>();
    }
}
