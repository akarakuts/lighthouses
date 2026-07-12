using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public static class BoardShuffler
    {
        public static bool TryShuffleToPlayable(TileState[,] board, IGameRandom random, int maxAttempts = 128) =>
            TryShuffleToPlayable(board, new RandomAdapter(random), maxAttempts);

        public static bool TryShuffleToPlayable(TileState[,] board, Random random, int maxAttempts = 128)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            var tiles = new List<TileState>(width * height);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                tiles.Add(board[x, y].Clone());

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Shuffle(tiles, random);
                int index = 0;
                for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    board[x, y] = tiles[index++];

                if (!Match3Rules.HasMatches(board) && Match3Rules.HasAvailableMove(board)) return true;
            }
            return false;
        }

        private static void Shuffle(IList<TileState> tiles, Random random)
        {
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
            }
        }
    }
}
