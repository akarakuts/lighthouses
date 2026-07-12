using System.Collections;
using LighthouseMatch3;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

public sealed class GameBootstrapPlayModeTests
{
    [UnityTest]
    public IEnumerator Bootstrap_CreatesInteractiveGameRoot()
    {
        if (Object.FindFirstObjectByType<GameBootstrap>() == null)
            new GameObject("Lighthouse Match3 Test").AddComponent<GameBootstrap>();

        yield return null;

        Assert.That(Object.FindFirstObjectByType<GameBootstrap>(), Is.Not.Null);
        Assert.That(Object.FindFirstObjectByType<Match3GameController>(), Is.Not.Null);
        Assert.That(Object.FindFirstObjectByType<EventSystem>(), Is.Not.Null);
        Assert.That(GameObject.Find("Game Canvas"), Is.Not.Null);
    }
}
