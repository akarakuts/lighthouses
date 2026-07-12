using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class TelemetryServiceTests
{
    private MemoryStore _store;

    [SetUp]
    public void SetUp()
    {
        _store = new MemoryStore();
        TelemetryService.ConfigureForTests(_store);
        TelemetryService.Initialize();
    }

    [TearDown]
    public void TearDown() => TelemetryService.ResetDependencies();

    [Test]
    public void Initialize_IncrementsSessionsStarted()
    {
        Assert.That(TelemetryService.Snapshot.SessionsStarted, Is.EqualTo(1));
    }

    [Test]
    public void LevelStarted_PersistsToInjectedStore()
    {
        TelemetryService.LevelStarted();
        TelemetryService.SwapMade();

        Assert.That(TelemetryService.Snapshot.LevelsStarted, Is.EqualTo(1));
        Assert.That(TelemetryService.Snapshot.SwapsMade, Is.EqualTo(1));
        Assert.That(_store.GetString("lighthouse_match3_telemetry_v1", string.Empty), Does.Contain("\"LevelsStarted\":1"));
    }
}
