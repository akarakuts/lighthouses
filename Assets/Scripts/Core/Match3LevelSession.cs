using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public sealed class Match3LevelSession
    {
        private readonly IGameRandom _random;

        public BoardEngine Engine { get; }
        public LevelDefinition Level { get; }
        public int Moves { get; set; }
        public int Score { get; set; }
        public int Collected { get; set; }
        public int BlockersLeft { get; set; }

        public TileState[,] Tiles => Engine.Tiles;
        public BlockerState[,] Blockers => Engine.Blockers;

        public Match3LevelSession(LevelDefinition level, IGameRandom random)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            Engine = new BoardEngine(LevelCatalog.BoardSize, LevelCatalog.BoardSize);
            Moves = level.Moves;
            BlockersLeft = level.BlockerCount;
            ResetBoard();
        }

        public bool IsWin => Collected >= Level.CollectTarget && BlockersLeft <= 0;

        public int CalculateStars() =>
            1 + (Score >= Level.StarTwoScore ? 1 : 0) + (Score >= Level.StarThreeScore ? 1 : 0);

        public int CalculateCoins() => 15 + Level.Id * 2;

        public void ResetBoard()
        {
            BoardGenerator.FillWithoutMatches(Tiles, _random);
            if (Level.BlockerCount > 0)
                BoardGenerator.PlaceRandomBlockers(Blockers, Level.BlockerCount, Level.Blocker, _random);
            BoardGenerator.EnsurePlayable(Engine, _random);
        }

        public TileState CreateTile() => BoardGenerator.CreateRandomTile(_random);

        public bool SwapProducesMatch(int ax, int ay, int bx, int by)
        {
            bool usesSpecial = Tiles[ax, ay].Special != SpecialKind.None || Tiles[bx, by].Special != SpecialKind.None;
            Engine.Swap(ax, ay, bx, by);
            bool hasMatch = Match3Rules.FindMatches(Tiles).Count > 0 || usesSpecial;
            Engine.Swap(ax, ay, bx, by);
            return hasMatch;
        }

        public MatchClearResult ClearMatches(HashSet<int> matches, int chain)
        {
            MatchClearResult result = MatchResolver.ClearMatches(
                Tiles, Blockers, LevelCatalog.BoardSize, matches, chain, Level.CollectKind);
            Score += result.ScoreDelta;
            Collected += result.CollectedDelta;
            BlockersLeft = Math.Max(0, BlockersLeft - result.BlockersCleared);
            return result;
        }

        public void CollapseBoard() => Engine.Collapse(CreateTile);

        public void EnsurePlayable() => BoardGenerator.EnsurePlayable(Engine, _random);

        public void InstallBoard(TileState[,] tiles)
        {
            int size = LevelCatalog.BoardSize;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                Tiles[x, y] = tiles[x, y]?.Clone();
            BoardGenerator.EnsurePlayable(Engine, _random);
        }
    }
}
