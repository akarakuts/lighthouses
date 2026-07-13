using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class SettingsScreenView
    {
        public static GameObject Build(
            Transform canvasRoot,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            Action showMap,
            Action toggleSound,
            Action toggleHaptics,
            Action toggleLanguage,
            Action showPrivacy)
        {
            var screen = ui.CreateFullScreenPanel(canvasRoot, "SettingsScreen", palette.Navy);
            ui.CreateText(screen.transform, translate("settings", null), 48, palette.Foam, new Vector2(.5f, .84f), new Vector2(780, 72), TextAnchor.MiddleCenter);
            string sound = SaveService.Progress.SoundEnabled ? translate("sound_on", null) : translate("sound_off", null);
            ui.CreateButton(screen.transform, sound, new Vector2(.5f, .66f), new Vector2(490, 86), palette.Ocean, palette.Foam, toggleSound);
            string haptics = SaveService.Progress.HapticsEnabled ? translate("haptics_on", null) : translate("haptics_off", null);
            ui.CreateButton(screen.transform, haptics, new Vector2(.5f, .54f), new Vector2(490, 86), palette.Ocean, palette.Foam, toggleHaptics);
            ui.CreateButton(screen.transform, translate("language", null), new Vector2(.5f, .42f), new Vector2(490, 86), palette.Ocean, palette.Foam, toggleLanguage);
            ui.CreateButton(screen.transform, translate("privacy", null), new Vector2(.5f, .30f), new Vector2(490, 70), palette.Ocean, palette.Foam, showPrivacy);
            ui.CreateText(screen.transform, translate("offline_storage", null), 25, palette.SettingsNote, new Vector2(.5f, .20f), new Vector2(760, 100), TextAnchor.MiddleCenter);
            ui.CreateButton(screen.transform, translate("back_to_map", null), new Vector2(.5f, .08f), new Vector2(330, 70), palette.Ocean, palette.Foam, showMap);
            return screen;
        }
    }
}
