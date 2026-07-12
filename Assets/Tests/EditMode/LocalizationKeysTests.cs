using System.Collections.Generic;
using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class LocalizationKeysTests
{
    [SetUp]
    public void SetUp()
    {
        SaveService.ConfigureForTests(new MemoryStore(), new FixedClock(new System.DateTime(2026, 7, 12, 10, 0, 0, System.DateTimeKind.Utc)));
        SaveService.Load();
    }

    [TearDown]
    public void TearDown() => SaveService.ResetDependencies();

    [Test]
    public void EnglishAndRussianTables_ContainMatchingKeys()
    {
        SaveService.Progress.LanguageCode = "en";
        Assert.That(LocalizationService.Get("lighthouse_stage_0"), Is.EqualTo("A dark tower"));

        SaveService.Progress.LanguageCode = "ru";
        Assert.That(LocalizationService.Get("lighthouse_stage_0"), Is.EqualTo("Тёмная башня"));
        Assert.That(LocalizationService.Get("lighthouse_upgrade", 20), Is.EqualTo("Улучшить за 20 звёзд"));
    }

    [Test]
    public void Get_ReturnsKeyWhenMissingFromBothTables()
    {
        SaveService.Progress.LanguageCode = "en";
        Assert.That(LocalizationService.Get("missing_key_xyz"), Is.EqualTo("missing_key_xyz"));
    }

    [Test]
    public void LifeRecoveryLabel_UsesLocalizedFullLabel()
    {
        SaveService.Progress.LanguageCode = "ru";
        SaveService.Progress.Lives = 5;

        Assert.That(LocalizationService.LifeRecoveryLabel(), Is.EqualTo("Полные"));
    }
}
