using System;
using System.Collections.Generic;
using LighthouseMatch3;
using NUnit.Framework;

public sealed class LocalizationServiceTests
{
    [SetUp]
    public void SetUp()
    {
        SaveService.ConfigureForTests(new MemoryStore(), new FixedClock());
        SaveService.Load();
    }

    [TearDown]
    public void TearDown()
    {
        SaveService.ResetDependencies();
    }

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

    private sealed class FixedClock : IUtcClock
    {
        public DateTime UtcNow => new DateTime(2026, 7, 12, 10, 0, 0, DateTimeKind.Utc);
    }

    private sealed class MemoryStore : IProgressStore
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        public string GetString(string key, string defaultValue) => _values.TryGetValue(key, out string value) ? value : defaultValue;
        public void SetString(string key, string value) => _values[key] = value;
        public void Save() { }
    }
}
