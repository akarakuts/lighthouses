using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class LevelCatalogTests
{
    [TearDown]
    public void TearDown() => LevelCatalog.ResetSource();

    [Test]
    public void Get_Level1_HasNoBlockers()
    {
        LevelDefinition level = LevelCatalog.Get(1);

        Assert.That(level.Id, Is.EqualTo(1));
        Assert.That(level.Blocker, Is.EqualTo(BlockerKind.None));
        Assert.That(level.BlockerCount, Is.EqualTo(0));
        Assert.That(level.Moves, Is.GreaterThanOrEqualTo(15));
    }

    [Test]
    public void Get_Level5_IntroducesBlockers()
    {
        LevelDefinition level = LevelCatalog.Get(5);

        Assert.That(level.Blocker, Is.Not.EqualTo(BlockerKind.None));
        Assert.That(level.BlockerCount, Is.GreaterThan(0));
        Assert.That(level.CollectTarget, Is.EqualTo(18 + 5 * 2));
    }

    [Test]
    public void Get_HigherLevels_IncreaseStarThresholds()
    {
        LevelDefinition early = LevelCatalog.Get(2);
        LevelDefinition late = LevelCatalog.Get(20);

        Assert.That(late.StarTwoScore, Is.GreaterThan(early.StarTwoScore));
        Assert.That(late.StarThreeScore, Is.GreaterThan(early.StarThreeScore));
    }
}
