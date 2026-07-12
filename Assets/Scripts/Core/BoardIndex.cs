namespace LighthouseMatch3
{
    public static class BoardIndex
    {
        public static int ToIndex(int boardSize, int x, int y) => x + y * boardSize;

        public static void FromIndex(int boardSize, int index, out int x, out int y)
        {
            x = index % boardSize;
            y = index / boardSize;
        }

        public static bool InBounds(int boardSize, int x, int y) =>
            x >= 0 && y >= 0 && x < boardSize && y < boardSize;
    }
}
