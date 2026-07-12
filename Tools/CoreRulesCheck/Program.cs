using System;
using LighthouseMatch3;

internal static class Program
{
    private static int Main()
    {
        try
        {
            FindsHorizontalAndVerticalRuns();
            ValidatesAdjacency();
            FindsProductiveSwap();
            Console.WriteLine("Core match-3 smoke tests passed.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static void FindsHorizontalAndVerticalRuns()
    {
        TileState[,] board = NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Coral;
        board[4, 1].Kind = TileKind.Shell;
        board[4, 2].Kind = TileKind.Shell;
        board[4, 3].Kind = TileKind.Shell;

        var matches = Match3Rules.FindMatches(board);
        Require(matches.Contains(0) && matches.Contains(2), "Horizontal match was not found.");
        Require(matches.Contains(12) && matches.Contains(28), "Vertical match was not found.");
    }

    private static void ValidatesAdjacency()
    {
        Require(Match3Rules.IsAdjacent(2, 2, 2, 3), "Vertical neighbor should be valid.");
        Require(Match3Rules.IsAdjacent(2, 2, 3, 2), "Horizontal neighbor should be valid.");
        Require(!Match3Rules.IsAdjacent(2, 2, 3, 3), "Diagonal neighbor should be invalid.");
        Require(!Match3Rules.IsAdjacent(2, 2, 4, 2), "Distant neighbor should be invalid.");
    }

    private static void FindsProductiveSwap()
    {
        TileState[,] board = NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[3, 0].Kind = TileKind.Coral;
        board[2, 1].Kind = TileKind.Coral;

        Require(Match3Rules.HasAvailableMove(board), "A productive swap should be available.");
    }

    private static TileState[,] NewBoard()
    {
        var board = new TileState[8, 8];
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
            board[x, y] = new TileState((TileKind)((x * 2 + y * 3) % 6));
        return board;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
