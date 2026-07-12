using UnityEngine;

namespace LighthouseMatch3
{
    public static class HapticsService
    {
        public static void Tap()
        {
            if (SaveService.Progress != null && SaveService.Progress.HapticsEnabled) Handheld.Vibrate();
        }
    }
}
