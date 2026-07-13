using UnityEngine;
using UnityEngine.EventSystems;

namespace LighthouseMatch3
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (FindFirstObjectByType<GameBootstrap>() != null) return;
            new GameObject("Lighthouse Match3").AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            SaveService.Load();
            LevelCatalog.Source = new ResourcesLevelCatalog();
            TelemetryService.Initialize();
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var system = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                DontDestroyOnLoad(system);
            }
            if (FindFirstObjectByType<AudioListener>() == null)
                gameObject.AddComponent<AudioListener>();
            gameObject.AddComponent<AudioService>();
            gameObject.AddComponent<Match3GameController>();
        }
    }
}
