using LighthouseMatch3;
using NUnit.Framework;

public sealed class ResourcesLevelCatalogTests
{
    [TearDown]
    public void TearDown() => LevelCatalog.ResetSource();

    [Test]
    public void Get_FallsBackToProceduralWhenAssetMissing()
    {
        LevelCatalog.Source = new ResourcesLevelCatalog();

        LevelDefinition level = LevelCatalog.Get(1);

        Assert.That(level.Id, Is.EqualTo(1));
        Assert.That(level.Moves, Is.GreaterThan(0));
    }

    [Test]
    public void Source_UsesInjectedCatalog()
    {
        LevelCatalog.Source = new FixedLevelCatalog(new LevelDefinition { Id = 7, Moves = 99, CollectTarget = 10, CollectKind = TileKind.Coral });

        Assert.That(LevelCatalog.Get(7).Moves, Is.EqualTo(99));
        Assert.That(LevelCatalog.Get(1).Moves, Is.EqualTo(99));
    }

    private sealed class FixedLevelCatalog : ILevelCatalog
    {
        private readonly LevelDefinition _definition;
        public FixedLevelCatalog(LevelDefinition definition) => _definition = definition;
        public LevelDefinition Get(int id) => _definition;
    }
}
