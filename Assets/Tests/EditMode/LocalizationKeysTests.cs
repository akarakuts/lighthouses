using System.Collections.Generic;
using System.Reflection;
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
        var english = (Dictionary<string, string>)typeof(LocalizationService)
            .GetField("English", BindingFlags.Static | BindingFlags.NonPublic)
            .GetValue(null);
        var russian = (Dictionary<string, string>)typeof(LocalizationService)
            .GetField("Russian", BindingFlags.Static | BindingFlags.NonPublic)
            .GetValue(null);
        CollectionAssert.AreEquivalent(english.Keys, russian.Keys);

        SaveService.Progress.LanguageCode = "en";
        Assert.That(LocalizationService.Get("lighthouse_stage_0"), Is.EqualTo("A dark tower"));

        SaveService.Progress.LanguageCode = "ru";
        Assert.That(LocalizationService.Get("lighthouse_stage_0"), Is.EqualTo("Тёмная башня"));
        Assert.That(LocalizationService.Get("lighthouse_upgrade", 20), Is.EqualTo("Улучшить за 20 звёзд"));
        Assert.That(LocalizationService.Get("rules_title"), Is.EqualTo("КАК ИГРАТЬ"));
        Assert.That(LocalizationService.Get("rules_pearl"), Is.EqualTo("Жемчужина очищает все фишки своего цвета."));
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
