using System;
using System.Collections.Generic;
using UnityEngine;

namespace LighthouseMatch3
{
    public interface IProgressStore
    {
        string GetString(string key, string defaultValue);
        void SetString(string key, string value);
        void Save();
    }

    public interface IUtcClock
    {
        DateTime UtcNow { get; }
    }

    public sealed class SystemUtcClock : IUtcClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public sealed class PlayerPrefsProgressStore : IProgressStore
    {
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void Save() => PlayerPrefs.Save();
    }

    public static class SaveService
    {
        private const string Key = "lighthouse_match3_progress_v1";
        private const int LifeRecoveryMinutes = 20;
        private const int MaxLives = 5;
        private static IProgressStore _store = new PlayerPrefsProgressStore();
        private static IUtcClock _clock = new SystemUtcClock();

        public static PlayerProgress Progress { get; private set; }

        public static void ConfigureForTests(IProgressStore store, IUtcClock clock)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Progress = null;
        }

        public static void ResetDependencies()
        {
            _store = new PlayerPrefsProgressStore();
            _clock = new SystemUtcClock();
            Progress = null;
        }

        public static void Load()
        {
            string json = _store.GetString(Key, string.Empty);
            try
            {
                Progress = string.IsNullOrWhiteSpace(json) ? new PlayerProgress() : JsonUtility.FromJson<PlayerProgress>(json);
            }
            catch (ArgumentException)
            {
                Progress = new PlayerProgress();
            }
            Progress ??= new PlayerProgress();
            NormalizeProgress();
            RefreshLives();
        }

        public static void Save()
        {
            EnsureLoaded();
            _store.SetString(Key, JsonUtility.ToJson(Progress));
            _store.Save();
        }

        public static void CompleteLevel(int level, int stars, int coins)
        {
            EnsureLoaded();
            if (!Progress.CompletedLevels.Contains(level))
            {
                Progress.CompletedLevels.Add(level);
                Progress.Stars += Mathf.Max(0, stars);
                Progress.Coins += Mathf.Max(0, coins);
            }
            Progress.UnlockedLevel = Mathf.Max(Progress.UnlockedLevel, level + 1);
            Progress.Lives = Mathf.Min(MaxLives, Progress.Lives + 1);
            if (Progress.Lives == MaxLives) Progress.LastLifeUtcTicks = 0;
            Save();
        }

        public static int ClaimDailyReward()
        {
            EnsureLoaded();
            string today = Today();
            if (Progress.LastRewardUtcDate == today) return 0;
            string yesterday = _clock.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            Progress.DailyRewardStreak = Progress.LastRewardUtcDate == yesterday ? Progress.DailyRewardStreak + 1 : 1;
            Progress.DailyRewardStreak = Mathf.Clamp(Progress.DailyRewardStreak, 1, 7);
            int reward = 20 + Progress.DailyRewardStreak * 10;
            Progress.Coins += reward;
            Progress.LastRewardUtcDate = today;
            Save();
            return reward;
        }

        public static bool CanClaimDailyReward()
        {
            EnsureLoaded();
            return Progress.LastRewardUtcDate != Today();
        }

        public static string LifeRecoveryLabel()
        {
            EnsureLoaded();
            RefreshLives();
            if (Progress.Lives >= MaxLives) return "Full";
            long elapsed = _clock.UtcNow.Ticks - Progress.LastLifeUtcTicks;
            long cycle = TimeSpan.FromMinutes(LifeRecoveryMinutes).Ticks;
            TimeSpan remaining = TimeSpan.FromTicks(Math.Max(0L, cycle - elapsed));
            return $"{remaining.Minutes:00}:{remaining.Seconds:00}";
        }

        public static bool TryGetLifeRecoveryRemaining(out int minutes, out int seconds)
        {
            EnsureLoaded();
            RefreshLives();
            if (Progress.Lives >= MaxLives)
            {
                minutes = seconds = 0;
                return false;
            }
            long elapsed = _clock.UtcNow.Ticks - Progress.LastLifeUtcTicks;
            long cycle = TimeSpan.FromMinutes(LifeRecoveryMinutes).Ticks;
            TimeSpan remaining = TimeSpan.FromTicks(Math.Max(0L, cycle - elapsed));
            minutes = remaining.Minutes;
            seconds = remaining.Seconds;
            return true;
        }

        public static void LoseLife()
        {
            EnsureLoaded();
            RefreshLives();
            if (Progress.Lives <= 0) return;
            Progress.Lives--;
            if (Progress.Lives < MaxLives && Progress.LastLifeUtcTicks == 0) Progress.LastLifeUtcTicks = _clock.UtcNow.Ticks;
            Save();
        }

        private static void EnsureLoaded()
        {
            if (Progress == null) Load();
        }

        private static void NormalizeProgress()
        {
            Progress.CompletedLevels ??= new List<int>();
            Progress.UnlockedLevel = Mathf.Max(1, Progress.UnlockedLevel);
            Progress.Coins = Mathf.Max(0, Progress.Coins);
            Progress.Stars = Mathf.Max(0, Progress.Stars);
            Progress.Lives = Mathf.Clamp(Progress.Lives, 0, MaxLives);
            Progress.DailyRewardStreak = Mathf.Clamp(Progress.DailyRewardStreak, 0, 7);
            if (Progress.Lives >= MaxLives)
            {
                Progress.LastLifeUtcTicks = 0;
                return;
            }

            long now = _clock.UtcNow.Ticks;
            if (Progress.LastLifeUtcTicks <= 0 || Progress.LastLifeUtcTicks > now) Progress.LastLifeUtcTicks = now;
        }

        private static void RefreshLives()
        {
            if (Progress.Lives >= MaxLives)
            {
                Progress.Lives = MaxLives;
                Progress.LastLifeUtcTicks = 0;
                return;
            }
            if (Progress.LastLifeUtcTicks == 0) Progress.LastLifeUtcTicks = _clock.UtcNow.Ticks;
            long cycle = TimeSpan.FromMinutes(LifeRecoveryMinutes).Ticks;
            long elapsed = Math.Max(0, _clock.UtcNow.Ticks - Progress.LastLifeUtcTicks);
            int recovered = (int)(elapsed / cycle);
            if (recovered <= 0) return;
            Progress.Lives = Mathf.Min(MaxLives, Progress.Lives + recovered);
            Progress.LastLifeUtcTicks += recovered * cycle;
            if (Progress.Lives == MaxLives) Progress.LastLifeUtcTicks = 0;
            Save();
        }

        private static string Today() => _clock.UtcNow.ToString("yyyy-MM-dd");
    }
}
