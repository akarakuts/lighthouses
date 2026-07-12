using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public static class Match3Rules
    {
        public static HashSet<int> FindMatches(TileState[,] board)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            var matches = new HashSet<int>();
            for (int y = 0; y < height; y++)
            {
                int run = 1;
                for (int x = 1; x <= width; x++)
                {
                    bool same = x < width && board[x, y] != null && board[x - 1, y] != null && board[x, y].Kind == board[x - 1, y].Kind;
                    if (same) { run++; continue; }
                    if (run >= 3) for (int i = x - run; i < x; i++) matches.Add(i + y * width);
                    run = 1;
                }
            }
            for (int x = 0; x < width; x++)
            {
                int run = 1;
                for (int y = 1; y <= height; y++)
                {
                    bool same = y < height && board[x, y] != null && board[x, y - 1] != null && board[x, y].Kind == board[x, y - 1].Kind;
                    if (same) { run++; continue; }
                    if (run >= 3) for (int i = y - run; i < y; i++) matches.Add(x + i * width);
                    run = 1;
                }
            }
            return matches;
        }

        public static bool IsAdjacent(int ax, int ay, int bx, int by)
        {
            return Math.Abs(ax - bx) + Math.Abs(ay - by) == 1;
        }

        public static bool HasAvailableMove(TileState[,] board)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (board[x, y]?.Special != SpecialKind.None) return true;
                if (x + 1 < width && TrySwapForMatch(board, x, y, x + 1, y)) return true;
                if (y + 1 < height && TrySwapForMatch(board, x, y, x, y + 1)) return true;
            }
            return false;
        }

        public static bool HasMatches(TileState[,] board) => FindMatches(board).Count > 0;

        private static bool TrySwapForMatch(TileState[,] board, int ax, int ay, int bx, int by)
        {
            (board[ax, ay], board[bx, by]) = (board[bx, by], board[ax, ay]);
            bool result = FindMatches(board).Count > 0;
            (board[ax, ay], board[bx, by]) = (board[bx, by], board[ax, ay]);
            return result;
        }
    }
}
