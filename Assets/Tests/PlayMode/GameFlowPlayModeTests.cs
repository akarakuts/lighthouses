using System.Collections;
using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class GameFlowPlayModeTests
{
    [SetUp]
    public void SetUp()
    {
        Match3GameController.ConfigureRandomForTests(new SeededGameRandom(42));
        SaveService.ConfigureForTests(new MemoryStore(), new FixedClock(new System.DateTime(2026, 7, 12, 10, 0, 0, System.DateTimeKind.Utc)));
        SaveService.Load();
        SaveService.Progress.Lives = 5;
        SaveService.Progress.UnlockedLevel = 20;
        TelemetryService.ConfigureForTests(new MemoryStore());
        TelemetryService.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        Match3GameController.ResetRandomForTests();
        SaveService.ResetDependencies();
        TelemetryService.ResetDependencies();
        LevelCatalog.ResetSource();
    }

    [UnityTest]
    public IEnumerator Bootstrap_CreatesInteractiveGameRoot()
    {
        if (Object.FindFirstObjectByType<GameBootstrap>() == null)
            new GameObject("Lighthouse Match3 Test").AddComponent<GameBootstrap>();

        yield return null;

        Assert.That(Object.FindFirstObjectByType<GameBootstrap>(), Is.Not.Null);
        Assert.That(Object.FindFirstObjectByType<Match3GameController>(), Is.Not.Null);
        Assert.That(Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>(), Is.Not.Null);
        Assert.That(GameObject.Find("Game Canvas"), Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator Swap_ClearsMatchAndIncreasesScore()
    {
        if (Object.FindFirstObjectByType<GameBootstrap>() == null)
            new GameObject("Lighthouse Match3 Test").AddComponent<GameBootstrap>();

        yield return null;

        var controller = Object.FindFirstObjectByType<Match3GameController>();
        var board = new TileState[LevelCatalog.BoardSize, LevelCatalog.BoardSize];
        for (int y = 0; y < LevelCatalog.BoardSize; y++)
        for (int x = 0; x < LevelCatalog.BoardSize; x++)
            board[x, y] = new TileState((TileKind)((x * 2 + y * 3) % 6));
        board[0, 0].Kind = TileKind.Coral;
        board[1, 0].Kind = TileKind.Coral;
        board[2, 0].Kind = TileKind.Shell;
        board[3, 0].Kind = TileKind.Coral;
        board[4, 0].Kind = TileKind.Coral;

        controller.DebugStartLevel(1);
        yield return null;
        controller.DebugInstallBoard(board);
        yield return null;

        controller.DebugSelectCell(2, 0);
        controller.DebugSelectCell(3, 0);

        float timeout = 4f;
        while (timeout > 0f)
        {
            if (!controller.DebugIsResolving && controller.DebugScore > 0) break;
            timeout -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        Assert.That(controller.DebugScore, Is.GreaterThan(0));
        Assert.That(TelemetryService.Snapshot.SwapsMade, Is.GreaterThan(0));
    }
}
