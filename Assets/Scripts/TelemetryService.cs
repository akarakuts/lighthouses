using System;
using UnityEngine;

namespace LighthouseMatch3
{
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
        private static TelemetrySnapshot _snapshot;
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                _snapshot = JsonUtility.FromJson<TelemetrySnapshot>(PlayerPrefs.GetString(Key, string.Empty));
            }
            catch (ArgumentException)
            {
                _snapshot = null;
            }
            _snapshot ??= new TelemetrySnapshot();
            Application.logMessageReceived += CountErrors;
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

        private static void Record(Action<TelemetrySnapshot> action)
        {
            if (!_initialized) Initialize();
            action(_snapshot);
            Persist();
        }

        private static void Persist()
        {
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(_snapshot));
            PlayerPrefs.Save();
        }
    }
}
