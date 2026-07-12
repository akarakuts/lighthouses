using System;
using System.Collections.Generic;
using LighthouseMatch3;

namespace LighthouseMatch3.Tests
{
    public static class TestFixtures
    {
        public static TileState[,] NewBoard(int size = 8)
        {
            var board = new TileState[size, size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                board[x, y] = new TileState((TileKind)((x * 2 + y * 3) % 6));
            return board;
        }

        public static TileState[,] NewNoMoveBoard()
        {
            var board = new TileState[3, 3];
            for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                board[x, y] = new TileState((TileKind)((x + y) % 3));
            return board;
        }

        public static BlockerState[,] EmptyBlockers(int size) => new BlockerState[size, size];
    }

    public sealed class MemoryStore : IProgressStore, ITelemetryStore
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public string GetString(string key, string defaultValue) =>
            _values.TryGetValue(key, out string value) ? value : defaultValue;

        public void SetString(string key, string value) => _values[key] = value;
        public void Save() { }
    }

    public sealed class FixedClock : IUtcClock
    {
        public FixedClock(DateTime utcNow) => UtcNow = utcNow;
        public DateTime UtcNow { get; set; }
    }
}
