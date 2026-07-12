using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public static class SpecialTileRules
    {
        public static SpecialKind PickCreatedSpecial(
            TileState[,] board, int boardSize, HashSet<int> matches, ReadOnlySpan<BoardPoint> swapped)
        {
            if (matches.Count < 4) return SpecialKind.None;
            int longest = 0;
            foreach (BoardPoint point in swapped)
            {
                longest = Math.Max(longest, RunLengthAt(board, boardSize, point.X, point.Y, 1, 0));
                longest = Math.Max(longest, RunLengthAt(board, boardSize, point.X, point.Y, 0, 1));
            }
            if (longest >= 5) return SpecialKind.Pearl;
            if (longest >= 4) return SpecialKind.Beam;
            return matches.Count >= 5 ? SpecialKind.Bomb : SpecialKind.None;
        }

        public static int RunLengthAt(TileState[,] board, int boardSize, int x, int y, int dx, int dy)
        {
            TileKind kind = board[x, y].Kind;
            int count = 1;
            for (int d = 1; BoardIndex.InBounds(boardSize, x + d * dx, y + d * dy)
                 && board[x + d * dx, y + d * dy].Kind == kind; d++) count++;
            for (int d = 1; BoardIndex.InBounds(boardSize, x - d * dx, y - d * dy)
                 && board[x - d * dx, y - d * dy].Kind == kind; d++) count++;
            return count;
        }

        public static int FindKeepIndex(int boardSize, HashSet<int> matches, ReadOnlySpan<BoardPoint> swapped)
        {
            foreach (BoardPoint point in swapped)
            {
                int index = BoardIndex.ToIndex(boardSize, point.X, point.Y);
                if (matches.Contains(index)) return index;
            }
            if (matches.Count == 0) return -1;
            foreach (int index in matches) return index;
            return -1;
        }

        public static void ExpandSpecials(TileState[,] board, int boardSize, HashSet<int> matches)
        {
            var queue = new Queue<int>(matches);
            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                BoardIndex.FromIndex(boardSize, index, out int x, out int y);
                TileState tile = board[x, y];
                if (tile == null || tile.Special == SpecialKind.None) continue;
                foreach (int target in SpecialTargets(board, boardSize, x, y, tile))
                    if (matches.Add(target)) queue.Enqueue(target);
            }
        }

        public static bool HasSpecialInMatches(TileState[,] board, int boardSize, HashSet<int> matches)
        {
            foreach (int index in matches)
            {
                BoardIndex.FromIndex(boardSize, index, out int x, out int y);
                TileState tile = board[x, y];
                if (tile != null && tile.Special != SpecialKind.None) return true;
            }
            return false;
        }

        private static IEnumerable<int> SpecialTargets(TileState[,] board, int boardSize, int x, int y, TileState tile)
        {
            if (tile.Special == SpecialKind.Beam)
            {
                for (int i = 0; i < boardSize; i++) yield return BoardIndex.ToIndex(boardSize, i, y);
                yield break;
            }
            if (tile.Special == SpecialKind.Bomb)
            {
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                    if (BoardIndex.InBounds(boardSize, x + dx, y + dy))
                        yield return BoardIndex.ToIndex(boardSize, x + dx, y + dy);
                yield break;
            }
            if (tile.Special == SpecialKind.Pearl)
            {
                for (int yy = 0; yy < boardSize; yy++)
                for (int xx = 0; xx < boardSize; xx++)
                    if (board[xx, yy] != null && board[xx, yy].Kind == tile.Kind)
                        yield return BoardIndex.ToIndex(boardSize, xx, yy);
            }
        }
    }
}
