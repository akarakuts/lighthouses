using System;
using System.Collections.Generic;
using LighthouseMatch3;
using NUnit.Framework;
using UnityEngine;

public sealed class SaveServiceTests
{
    private MemoryStore _store;
    private MutableClock _clock;

    [SetUp]
    public void SetUp()
    {
        _store = new MemoryStore();
        _clock = new MutableClock(new DateTime(2026, 7, 12, 10, 0, 0, DateTimeKind.Utc));
        SaveService.ConfigureForTests(_store, _clock);
    }

    [TearDown]
    public void TearDown()
    {
        SaveService.ResetDependencies();
    }

    [Test]
    public void Load_RecoversLivesUsingInjectedClock()
    {
        _store.SetString("lighthouse_match3_progress_v1", JsonUtility.ToJson(new PlayerProgress
        {
            Lives = 3,
            LastLifeUtcTicks = _clock.UtcNow.AddMinutes(-40).Ticks
        }));

        SaveService.Load();

        Assert.That(SaveService.Progress.Lives, Is.EqualTo(5));
        Assert.That(SaveService.Progress.LastLifeUtcTicks, Is.EqualTo(0));
    }

    [Test]
    public void Load_NormalizesFutureLifeTimestamp()
    {
        _store.SetString("lighthouse_match3_progress_v1", JsonUtility.ToJson(new PlayerProgress
        {
            Lives = 4,
            LastLifeUtcTicks = _clock.UtcNow.AddHours(3).Ticks
        }));

        SaveService.Load();

        Assert.That(SaveService.Progress.LastLifeUtcTicks, Is.EqualTo(_clock.UtcNow.Ticks));
        Assert.That(SaveService.LifeRecoveryLabel(), Is.EqualTo("20:00"));
    }

    [Test]
    public void ClaimDailyReward_TracksConsecutiveDays()
    {
        SaveService.Load();

        Assert.That(SaveService.ClaimDailyReward(), Is.EqualTo(30));
        _clock.UtcNow = _clock.UtcNow.AddDays(1);
        Assert.That(SaveService.ClaimDailyReward(), Is.EqualTo(40));
        Assert.That(SaveService.Progress.DailyRewardStreak, Is.EqualTo(2));
    }

    [Test]
    public void CompleteLevel_DoesNotAwardDuplicateRewards()
    {
        SaveService.Load();
        int startingCoins = SaveService.Progress.Coins;

        SaveService.CompleteLevel(1, 2, 15);
        SaveService.CompleteLevel(1, 2, 15);

        Assert.That(SaveService.Progress.Coins, Is.EqualTo(startingCoins + 15));
        Assert.That(SaveService.Progress.Stars, Is.EqualTo(2));
    }

    private sealed class MutableClock : IUtcClock
    {
        public MutableClock(DateTime utcNow) => UtcNow = utcNow;
        public DateTime UtcNow { get; set; }
    }

    private sealed class MemoryStore : IProgressStore
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public string GetString(string key, string defaultValue)
        {
            return _values.TryGetValue(key, out string value) ? value : defaultValue;
        }

        public void SetString(string key, string value) => _values[key] = value;
        public void Save() { }
    }
}
