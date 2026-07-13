using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;
using System;

public class Match3RulesTests
{
    [Test]
    public void FindMatches_FindsHorizontalAndVerticalRuns()
    {
        var board = TestFixtures.NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Coral;
        board[4, 1].Kind = TileKind.Shell;
        board[4, 2].Kind = TileKind.Shell;
        board[4, 3].Kind = TileKind.Shell;

        var matches = Match3Rules.FindMatches(board);

        Assert.That(matches, Does.Contain(0));
        Assert.That(matches, Does.Contain(2));
        Assert.That(matches, Does.Contain(12));
        Assert.That(matches, Does.Contain(28));
    }

    [Test]
    public void IsAdjacent_OnlyAcceptsOrthogonalNeighbors()
    {
        Assert.That(Match3Rules.IsAdjacent(2, 2, 2, 3), Is.True);
        Assert.That(Match3Rules.IsAdjacent(2, 2, 3, 2), Is.True);
        Assert.That(Match3Rules.IsAdjacent(2, 2, 3, 3), Is.False);
        Assert.That(Match3Rules.IsAdjacent(2, 2, 4, 2), Is.False);
    }

    [Test]
    public void HasAvailableMove_FindsAProductiveSwap()
    {
        var board = TestFixtures.NewBoard();
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[3, 0].Kind = TileKind.Coral;
        board[2, 1].Kind = TileKind.Coral;

        Assert.That(Match3Rules.HasAvailableMove(board), Is.True);
    }

    [Test]
    public void HasAvailableMove_TreatsSpecialTileAsAValidMove()
    {
        var board = TestFixtures.NewNoMoveBoard();
        Assert.That(Match3Rules.HasAvailableMove(board), Is.False);

        board[1, 1].Special = SpecialKind.Beam;

        Assert.That(Match3Rules.HasAvailableMove(board), Is.True);
    }

    [Test]
    public void BoardShuffler_CreatesPlayableBoardWithoutStartingMatches()
    {
        var board = TestFixtures.NewNoMoveBoard();

        bool shuffled = BoardShuffler.TryShuffleToPlayable(board, new Random(17));

        Assert.That(shuffled, Is.True);
        Assert.That(Match3Rules.HasMatches(board), Is.False);
        Assert.That(Match3Rules.HasAvailableMove(board), Is.True);
    }

    [Test]
    public void BoardShuffler_PreservesSpecials()
    {
        var board = TestFixtures.NewNoMoveBoard();
        board[0, 0].Special = SpecialKind.Bomb;

        bool shuffled = BoardShuffler.TryShuffleToPlayable(board, new Random(31));

        Assert.That(shuffled, Is.True);
        Assert.That(CountSpecials(board, SpecialKind.Bomb), Is.EqualTo(1));
        Assert.That(Match3Rules.HasMatches(board), Is.False);
    }

    [Test]
    public void BlockerRules_CrateNeedsTwoHits()
    {
        var blocker = new BlockerState(BlockerKind.Crate);

        bool clearedOnFirstHit = BlockerRules.Damage(blocker, false);
        Assert.That(blocker.Kind, Is.EqualTo(BlockerKind.Crate));
        Assert.That(blocker.HitsRemaining, Is.EqualTo(1));

        bool clearedOnSecondHit = BlockerRules.Damage(blocker, false);

        Assert.That(clearedOnFirstHit, Is.False);
        Assert.That(blocker.Kind, Is.EqualTo(BlockerKind.None));
        Assert.That(clearedOnSecondHit, Is.True);
    }

    [Test]
    public void BlockerRules_SeaweedRequiresASpecialEffect()
    {
        var blocker = new BlockerState(BlockerKind.Seaweed);

        Assert.That(BlockerRules.Damage(blocker, false), Is.False);
        Assert.That(blocker.Kind, Is.EqualTo(BlockerKind.Seaweed));
        Assert.That(BlockerRules.Damage(blocker, true), Is.True);
        Assert.That(blocker.Kind, Is.EqualTo(BlockerKind.None));
    }

    [Test]
    public void BoardEngine_CollapseKeepsBlockersAtTheirCells()
    {
        var engine = new BoardEngine(3, 3);
        for (int y = 0; y < engine.Height; y++)
        for (int x = 0; x < engine.Width; x++)
            engine.Tiles[x, y] = new TileState(TileKind.Coral);
        engine.Tiles[1, 0] = null;
        engine.Blockers[1, 2] = new BlockerState(BlockerKind.Ice);

        engine.Collapse(() => new TileState(TileKind.Shell));

        Assert.That(engine.Blockers[1, 2].Kind, Is.EqualTo(BlockerKind.Ice));
        Assert.That(engine.Tiles[1, 2].Kind, Is.EqualTo(TileKind.Shell));
    }

    [Test]
    public void TileFallMoves_ComputesExistingAndSpawnedTiles()
    {
        int size = 4;
        var before = new TileState[size, size];
        var after = new TileState[size, size];
        var falling = new TileState(TileKind.Coral);
        var spawned = new TileState(TileKind.Shell);

        before[1, 3] = falling;
        after[1, 0] = falling;
        after[1, 3] = spawned;

        var moves = TileFallMoves.Compute(before, after, size);

        Assert.That(moves, Has.Count.EqualTo(2));
        Assert.That(moves, Has.Some.Matches<TileFallMove>(move => move.X == 1 && move.FromY == 3 && move.ToY == 0 && move.Tile == falling));
        Assert.That(moves, Has.Some.Matches<TileFallMove>(move => move.X == 1 && move.FromY == size && move.ToY == 3 && move.Tile == spawned));
    }

    private static int CountSpecials(TileState[,] board, SpecialKind kind)
    {
        int count = 0;
        foreach (TileState tile in board)
            if (tile.Special == kind) count++;
        return count;
    }
}
