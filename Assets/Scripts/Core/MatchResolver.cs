using System;
using System.Collections.Generic;

namespace LighthouseMatch3
{
    public readonly struct MatchClearResult
    {
        public int ScoreDelta { get; init; }
        public int CollectedDelta { get; init; }
        public int BlockersCleared { get; init; }
        public bool SpecialWasTriggered { get; init; }
    }

    public static class MatchResolver
    {
        public static bool ShouldContinueResolution(
            HashSet<int> matches, TileState[,] board, int boardSize, ReadOnlySpan<BoardPoint> swapped, int chain)
        {
            if (matches.Count > 0) return true;
            if (chain != 0) return false;
            foreach (BoardPoint point in swapped)
            {
                TileState tile = board[point.X, point.Y];
                if (tile != null && tile.Special != SpecialKind.None) return true;
            }
            return false;
        }

        public static void PrepareResolutionStep(
            TileState[,] board, int boardSize, HashSet<int> matches, ReadOnlySpan<BoardPoint> swapped,
            out SpecialKind createdSpecial, out int keepIndex)
        {
            createdSpecial = SpecialTileRules.PickCreatedSpecial(board, boardSize, matches, swapped);
            keepIndex = SpecialTileRules.FindKeepIndex(boardSize, matches, swapped);
            if (createdSpecial != SpecialKind.None && keepIndex >= 0) matches.Remove(keepIndex);

            foreach (BoardPoint point in swapped)
            {
                TileState tile = board[point.X, point.Y];
                if (tile != null && tile.Special != SpecialKind.None)
                    matches.Add(BoardIndex.ToIndex(boardSize, point.X, point.Y));
            }

            SpecialTileRules.ExpandSpecials(board, boardSize, matches);

            if (createdSpecial != SpecialKind.None && keepIndex >= 0)
            {
                BoardIndex.FromIndex(boardSize, keepIndex, out int x, out int y);
                board[x, y].Special = createdSpecial;
            }
        }

        public static MatchClearResult ClearMatches(
            TileState[,] tiles, BlockerState[,] blockers, int boardSize,
            HashSet<int> matches, int chain, TileKind collectKind)
        {
            bool triggeredBySpecial = SpecialTileRules.HasSpecialInMatches(tiles, boardSize, matches);
            int collected = 0;
            int blockersCleared = 0;

            foreach (int index in matches)
            {
                BoardIndex.FromIndex(boardSize, index, out int x, out int y);
                TileState tile = tiles[x, y];
                if (tile == null) continue;
                if (BlockerRules.Damage(blockers[x, y], triggeredBySpecial)) blockersCleared++;
                if (tile.Kind == collectKind) collected++;
                tiles[x, y] = null;
            }

            return new MatchClearResult
            {
                ScoreDelta = matches.Count * 100 * chain,
                CollectedDelta = collected,
                BlockersCleared = blockersCleared,
                SpecialWasTriggered = triggeredBySpecial
            };
        }
    }
}
