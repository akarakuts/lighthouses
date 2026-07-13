using System.Collections.Generic;

namespace LighthouseMatch3
{
    public readonly struct TileFallMove
    {
        public int X { get; }
        public int FromY { get; }
        public int ToY { get; }
        public TileState Tile { get; }

        public TileFallMove(int x, int fromY, int toY, TileState tile)
        {
            X = x;
            FromY = fromY;
            ToY = toY;
            Tile = tile;
        }
    }

    public static class TileFallMoves
    {
        public static List<TileFallMove> Compute(TileState[,] before, TileState[,] after, int size)
        {
            var moves = new List<TileFallMove>();
            for (int x = 0; x < size; x++)
            {
                var beforeTiles = new HashSet<TileState>();
                for (int y = 0; y < size; y++)
                    if (before[x, y] != null) beforeTiles.Add(before[x, y]);

                for (int y = 0; y < size; y++)
                {
                    TileState tile = after[x, y];
                    if (tile == null) continue;

                    int fromY = FindSourceY(before, beforeTiles, after, x, y, tile, size);
                    if (fromY != y) moves.Add(new TileFallMove(x, fromY, y, tile));
                }
            }

            return moves;
        }

        private static int FindSourceY(
            TileState[,] before,
            HashSet<TileState> beforeTiles,
            TileState[,] after,
            int x,
            int toY,
            TileState tile,
            int size)
        {
            if (beforeTiles.Contains(tile))
            {
                for (int y = 0; y < size; y++)
                    if (before[x, y] == tile) return y;
                return toY;
            }

            int newAbove = 0;
            for (int y = toY + 1; y < size; y++)
            {
                TileState above = after[x, y];
                if (above != null && !beforeTiles.Contains(above)) newAbove++;
            }

            return size + newAbove;
        }
    }
}
