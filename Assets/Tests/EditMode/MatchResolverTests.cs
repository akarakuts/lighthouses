using System.Collections.Generic;
using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class MatchResolverTests
{
    private const int Size = 8;

    [Test]
    public void PickCreatedSpecial_CreatesBeamForFourInARow()
    {
        TileState[,] board = TestFixtures.NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Coral;
        board[3, 0].Kind = TileKind.Coral;
        var matches = Match3Rules.FindMatches(board);
        var swapped = new[] { new BoardPoint(2, 0), new BoardPoint(3, 0) };

        SpecialKind created = SpecialTileRules.PickCreatedSpecial(board, Size, matches, swapped);

        Assert.That(created, Is.EqualTo(SpecialKind.Beam));
    }

    [Test]
    public void PickCreatedSpecial_CreatesPearlForFiveInARow()
    {
        TileState[,] board = TestFixtures.NewBoard();
        for (int x = 0; x < 5; x++) board[x, 0].Kind = TileKind.Shell;
        var matches = Match3Rules.FindMatches(board);
        var swapped = new[] { new BoardPoint(3, 0), new BoardPoint(4, 0) };

        SpecialKind created = SpecialTileRules.PickCreatedSpecial(board, Size, matches, swapped);

        Assert.That(created, Is.EqualTo(SpecialKind.Pearl));
    }

    [Test]
    public void ExpandSpecials_BeamClearsEntireRow()
    {
        TileState[,] board = TestFixtures.NewBoard();
        board[2, 3].Special = SpecialKind.Beam;
        var matches = new HashSet<int> { BoardIndex.ToIndex(Size, 2, 3) };

        SpecialTileRules.ExpandSpecials(board, Size, matches);

        Assert.That(matches.Count, Is.EqualTo(Size));
        for (int x = 0; x < Size; x++)
            Assert.That(matches, Does.Contain(BoardIndex.ToIndex(Size, x, 3)));
    }

    [Test]
    public void ExpandSpecials_BombClearsThreeByThreeArea()
    {
        TileState[,] board = TestFixtures.NewBoard();
        board[4, 4].Special = SpecialKind.Bomb;
        var matches = new HashSet<int> { BoardIndex.ToIndex(Size, 4, 4) };

        SpecialTileRules.ExpandSpecials(board, Size, matches);

        Assert.That(matches.Count, Is.EqualTo(9));
        Assert.That(matches, Does.Contain(BoardIndex.ToIndex(Size, 3, 3)));
        Assert.That(matches, Does.Contain(BoardIndex.ToIndex(Size, 5, 5)));
    }

    [Test]
    public void ClearMatches_AppliesChainMultiplierAndCollectsGoalTiles()
    {
        TileState[,] board = TestFixtures.NewBoard();
        BlockerState[,] blockers = TestFixtures.EmptyBlockers(Size);
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Coral;
        var matches = Match3Rules.FindMatches(board);

        MatchClearResult result = MatchResolver.ClearMatches(board, blockers, Size, matches, chain: 2, TileKind.Coral);

        Assert.That(result.ScoreDelta, Is.EqualTo(900));
        Assert.That(result.CollectedDelta, Is.EqualTo(3));
        Assert.That(board[0, 0], Is.Null);
        Assert.That(board[1, 0], Is.Null);
        Assert.That(board[2, 0], Is.Null);
    }

    [Test]
    public void ClearMatches_ClearsBlockerWhenTriggeredBySpecial()
    {
        TileState[,] board = TestFixtures.NewBoard();
        BlockerState[,] blockers = TestFixtures.EmptyBlockers(Size);
        board[1, 1].Special = SpecialKind.Bomb;
        blockers[1, 1] = new BlockerState(BlockerKind.Seaweed);
        var matches = new HashSet<int> { BoardIndex.ToIndex(Size, 1, 1) };

        MatchClearResult result = MatchResolver.ClearMatches(board, blockers, Size, matches, chain: 1, TileKind.Coral);

        Assert.That(result.BlockersCleared, Is.EqualTo(1));
        Assert.That(result.SpecialWasTriggered, Is.True);
        Assert.That(blockers[1, 1].Kind, Is.EqualTo(BlockerKind.None));
    }

    [Test]
    public void ShouldContinueResolution_ContinuesWhenSwappedSpecialExistsOnFirstChain()
    {
        TileState[,] board = TestFixtures.NewBoard();
        board[2, 2].Special = SpecialKind.Beam;
        var swapped = new[] { new BoardPoint(2, 2), new BoardPoint(2, 3) };

        bool shouldContinue = MatchResolver.ShouldContinueResolution(new HashSet<int>(), board, Size, swapped, chain: 0);

        Assert.That(shouldContinue, Is.True);
    }

    [Test]
    public void PrepareResolutionStep_PlacesCreatedSpecialOnKeepCell()
    {
        TileState[,] board = TestFixtures.NewBoard();
        for (int x = 0; x < 4; x++) board[x, 0].Kind = TileKind.Drop;
        var matches = Match3Rules.FindMatches(board);
        var swapped = new[] { new BoardPoint(2, 0), new BoardPoint(3, 0) };

        MatchResolver.PrepareResolutionStep(board, Size, matches, swapped, out SpecialKind created, out int keepIndex);

        Assert.That(created, Is.EqualTo(SpecialKind.Beam));
        BoardIndex.FromIndex(Size, keepIndex, out int x, out int y);
        Assert.That(board[x, y].Special, Is.EqualTo(SpecialKind.Beam));
    }
}
