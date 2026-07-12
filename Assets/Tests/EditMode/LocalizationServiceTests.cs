using LighthouseMatch3;
using LighthouseMatch3.Tests;
using NUnit.Framework;

public sealed class LocalizationServiceTests
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
    public void Get_UsesSavedRussianLanguage()
    {
        SaveService.Progress.LanguageCode = "ru";

        Assert.That(LocalizationService.Get("title"), Is.EqualTo("МАЯКИ"));
        Assert.That(LocalizationService.Get("moves", 7), Is.EqualTo("Ходы: 7"));
    }

    [Test]
    public void ToggleLanguage_PersistsTheSelectedLanguage()
    {
        SaveService.Progress.LanguageCode = "en";

        LocalizationService.ToggleLanguage();

        Assert.That(SaveService.Progress.LanguageCode, Is.EqualTo("ru"));
    }
}
