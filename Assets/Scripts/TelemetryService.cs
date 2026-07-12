using System;
using UnityEngine;

namespace LighthouseMatch3
{
    public interface ITelemetryStore
    {
        string GetString(string key, string defaultValue);
        void SetString(string key, string value);
        void Save();
    }

    public sealed class PlayerPrefsTelemetryStore : ITelemetryStore
    {
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void Save() => PlayerPrefs.Save();
    }

    [Serializable]
    public sealed class TelemetrySnapshot
    {
        public int SessionsStarted;
        public int LevelsStarted;
        public int LevelsWon;
        public int LevelsLost;
        public int SwapsMade;
        public int SpecialEffectsTriggered;
        public int RuntimeErrors;
    }

    public static class TelemetryService
    {
        private const string Key = "lighthouse_match3_telemetry_v1";
        private static ITelemetryStore _store = new PlayerPrefsTelemetryStore();
        private static TelemetrySnapshot _snapshot;
        private static bool _initialized;
        private static bool _countErrors;

        public static TelemetrySnapshot Snapshot
        {
            get
            {
                EnsureInitialized();
                return _snapshot;
            }
        }

        public static void ConfigureForTests(ITelemetryStore store)
        {
            ResetDependencies();
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static void ResetDependencies()
        {
            if (_countErrors) Application.logMessageReceived -= CountErrors;
            _store = new PlayerPrefsTelemetryStore();
            _snapshot = null;
            _initialized = false;
            _countErrors = false;
        }

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                _snapshot = JsonUtility.FromJson<TelemetrySnapshot>(_store.GetString(Key, string.Empty));
            }
            catch (ArgumentException)
            {
                _snapshot = null;
            }
            _snapshot ??= new TelemetrySnapshot();
            Application.logMessageReceived += CountErrors;
            _countErrors = true;
            _snapshot.SessionsStarted++;
            Persist();
        }

        public static void LevelStarted() => Record(snapshot => snapshot.LevelsStarted++);
        public static void LevelWon() => Record(snapshot => snapshot.LevelsWon++);
        public static void LevelLost() => Record(snapshot => snapshot.LevelsLost++);
        public static void SwapMade() => Record(snapshot => snapshot.SwapsMade++);
        public static void SpecialTriggered() => Record(snapshot => snapshot.SpecialEffectsTriggered++);

        private static void CountErrors(string _, string __, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                Record(snapshot => snapshot.RuntimeErrors++);
        }

        private static void EnsureInitialized()
        {
            if (!_initialized) Initialize();
        }

        private static void Record(Action<TelemetrySnapshot> action)
        {
            EnsureInitialized();
            action(_snapshot);
            Persist();
        }

        private static void Persist()
        {
            _store.SetString(Key, JsonUtility.ToJson(_snapshot));
            _store.Save();
        }
    }
}
