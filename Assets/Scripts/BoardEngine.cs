using System;

namespace LighthouseMatch3
{
    public sealed class BoardEngine
    {
        public TileState[,] Tiles { get; }
        public BlockerState[,] Blockers { get; }

        public int Width => Tiles.GetLength(0);
        public int Height => Tiles.GetLength(1);

        public BoardEngine(int width, int height)
        {
            Tiles = new TileState[width, height];
            Blockers = new BlockerState[width, height];
        }

        public bool HasAvailableMove() => Match3Rules.HasAvailableMove(Tiles);

        public bool HasMatches() => Match3Rules.HasMatches(Tiles);

        public void Swap(int ax, int ay, int bx, int by)
        {
            (Tiles[ax, ay], Tiles[bx, by]) = (Tiles[bx, by], Tiles[ax, ay]);
        }

        public void Collapse(Func<TileState> nextTile)
        {
            for (int x = 0; x < Width; x++)
            {
                int write = 0;
                for (int y = 0; y < Height; y++)
                    if (Tiles[x, y] != null) Tiles[x, write++] = Tiles[x, y];
                for (int y = write; y < Height; y++) Tiles[x, y] = nextTile();
            }
        }

        public bool TryShuffleToPlayable(Random random)
        {
            return BoardShuffler.TryShuffleToPlayable(Tiles, random);
        }
    }
}
