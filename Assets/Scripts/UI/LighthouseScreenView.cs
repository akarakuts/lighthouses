using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class LighthouseScreenView
    {
        public static GameObject Build(
            Transform canvasRoot,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            Action showMap,
            Action<int> upgradeLighthouse)
        {
            var screen = ui.CreateFullScreenPanel(canvasRoot, "LighthouseScreen", palette.Navy);
            ui.CreateText(screen.transform, translate("lighthouse", null), 48, palette.Foam, new Vector2(.5f, .9f), new Vector2(900, 74), TextAnchor.MiddleCenter);
            int stage = SaveService.Progress.LighthouseStage;
            ui.CreateText(screen.transform, translate($"lighthouse_stage_{Mathf.Min(stage, 4)}", null), 34, palette.Gold, new Vector2(.5f, .79f), new Vector2(900, 60), TextAnchor.MiddleCenter);
            Image lighthouseArt = ui.CreateImage(screen.transform, "Lighthouse Illustration", new Vector2(.5f, .51f), new Vector2(430, 620), stage >= 4 ? palette.Gold : palette.Ocean);
            ui.ApplyArtwork(lighthouseArt, "Artwork/LighthouseIsland-v1");
            ui.CreateText(
                screen.transform,
                stage >= 4 ? translate("lighthouse_complete", null) : translate("lighthouse_progress", null),
                28, palette.Foam, new Vector2(.5f, .27f), new Vector2(850, 120), TextAnchor.MiddleCenter);
            int cost = 8 + stage * 12;
            if (stage < 4)
            {
                var upgrade = ui.CreateButton(screen.transform, translate("lighthouse_upgrade", new object[] { cost }), new Vector2(.5f, .18f), new Vector2(460, 78), palette.Gold, palette.Navy, () => upgradeLighthouse(cost));
                upgrade.interactable = SaveService.Progress.Stars >= cost;
            }
            ui.CreateButton(screen.transform, translate("back_to_map", null), new Vector2(.5f, .09f), new Vector2(330, 70), palette.Ocean, palette.Foam, showMap);
            return screen;
        }
    }
}
