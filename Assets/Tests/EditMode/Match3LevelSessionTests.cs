using System.Collections.Generic;
using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class Match3LevelSessionTests
{
    [Test]
    public void SwapProducesMatch_DetectsValidSwap()
    {
        var session = new Match3LevelSession(LevelCatalog.Get(1), new SeededGameRandom(1));
        session.InstallBoard(CreateMatchSetup());

        Assert.That(session.SwapProducesMatch(2, 0, 3, 0), Is.True);
    }

    [Test]
    public void ClearMatches_UpdatesScoreAndCollection()
    {
        var session = new Match3LevelSession(LevelCatalog.Get(1), new SeededGameRandom(1));
        session.InstallBoard(CreateMatchSetup());
        session.Engine.Swap(2, 0, 3, 0);
        HashSet<int> matches = Match3Rules.FindMatches(session.Tiles);

        session.ClearMatches(matches, chain: 1);

        Assert.That(session.Score, Is.GreaterThan(0));
        Assert.That(session.Collected, Is.GreaterThan(0));
    }

    [Test]
    public void CalculateStars_ReturnsOneToThree()
    {
        var session = new Match3LevelSession(LevelCatalog.Get(10), new SeededGameRandom(2));
        session.Score = session.Level.StarThreeScore + 100;

        Assert.That(session.CalculateStars(), Is.EqualTo(3));
    }

    private static TileState[,] CreateMatchSetup()
    {
        var board = TestFixtures.NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Shell;
        board[3, 0].Kind = TileKind.Coral;
        board[4, 0].Kind = TileKind.Coral;
        return board;
    }
}
