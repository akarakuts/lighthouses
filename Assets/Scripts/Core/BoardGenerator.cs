using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public static class BoardGenerator
    {
        public static TileState CreateRandomTile(IGameRandom random) =>
            new TileState((TileKind)random.Range(0, 6));

        public static bool CreatesStartingMatch(TileState[,] board, int x, int y)
        {
            if (x >= 2 && board[x - 1, y].Kind == board[x - 2, y].Kind && board[x, y].Kind == board[x - 1, y].Kind)
                return true;
            return y >= 2 && board[x, y - 1].Kind == board[x, y - 2].Kind && board[x, y].Kind == board[x, y - 1].Kind;
        }

        public static void FillWithoutMatches(TileState[,] board, IGameRandom random)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                do board[x, y] = CreateRandomTile(random);
                while (CreatesStartingMatch(board, x, y));
            }
        }

        public static void PlaceRandomBlockers(BlockerState[,] blockers, int count, BlockerKind kind, IGameRandom random)
        {
            int width = blockers.GetLength(0);
            int height = blockers.GetLength(1);
            var available = new List<(int x, int y)>(width * height);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) available.Add((x, y));

            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int pick = random.Range(0, available.Count);
                (int x, int y) = available[pick];
                available.RemoveAt(pick);
                blockers[x, y] = new BlockerState(kind);
            }
        }

        public static void EnsurePlayable(BoardEngine engine, IGameRandom random)
        {
            if (engine.HasAvailableMove()) return;
            if (engine.TryShuffleToPlayable(new RandomAdapter(random))) return;

            for (int attempt = 0; attempt < 256; attempt++)
            {
                FillWithoutMatches(engine.Tiles, random);
                if (engine.HasAvailableMove()) return;
            }

            throw new InvalidOperationException("Unable to generate a playable match-3 board.");
        }
    }
}
