using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class MapScreenView
    {
        public static GameObject Build(
            Transform canvasRoot,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            Action<int> startLevel,
            Action showLighthouse,
            Action showSettings,
            Action claimDailyReward)
        {
            var screen = ui.CreateFullScreenPanel(canvasRoot, "MapScreen", palette.Navy);
            ui.CreateText(screen.transform, translate("title", null), 58, palette.Foam, new Vector2(0.5f, 0.92f), new Vector2(650, 80), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("subtitle", null), 27, palette.Subtitle, new Vector2(0.5f, 0.875f), new Vector2(650, 42), TextAnchor.MiddleCenter);

            string lifeStatus = SaveService.Progress.Lives < 5 ? $" ({LocalizationService.LifeRecoveryLabel()})" : string.Empty;
            var resources = ui.CreateText(
                screen.transform,
                translate("resources", new object[] { SaveService.Progress.Stars, SaveService.Progress.Coins, SaveService.Progress.Lives, lifeStatus }),
                26, palette.Foam, new Vector2(0.5f, 0.825f), new Vector2(920, 50), TextAnchor.MiddleCenter);
            resources.name = "Resources";

            var daily = ui.CreateButton(
                screen.transform,
                SaveService.CanClaimDailyReward() ? translate("daily_reward", null) : translate("daily_claimed", null),
                new Vector2(.28f, .757f), new Vector2(360, 78), palette.Gold, palette.Navy, claimDailyReward);
            daily.interactable = SaveService.CanClaimDailyReward();
            ui.CreateButton(screen.transform, translate("restore", null), new Vector2(.72f, .757f), new Vector2(360, 78), palette.Ocean, palette.Foam, showLighthouse);
            ui.CreateButton(screen.transform, translate("settings", null), new Vector2(.88f, .93f), new Vector2(150, 62), palette.Ocean, palette.Foam, showSettings);

            var grid = new GameObject("Level Map", typeof(RectTransform), typeof(GridLayoutGroup));
            grid.transform.SetParent(screen.transform, false);
            var rect = grid.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.13f);
            rect.anchorMax = new Vector2(0.5f, 0.13f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(880, 1210);
            var layout = grid.GetComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(200, 180);
            layout.spacing = new Vector2(26, 26);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;
            layout.childAlignment = TextAnchor.MiddleCenter;

            for (int id = 1; id <= LevelCatalog.LevelCount; id++)
            {
                int level = id;
                bool unlocked = level <= SaveService.Progress.UnlockedLevel;
                bool complete = SaveService.Progress.CompletedLevels.Contains(level);
                string label = complete ? $"{level}\n{translate("clear", null)}" : unlocked ? $"{level}\n{translate("play", null)}" : translate("locked", null);
                var button = ui.CreateButton(grid.transform, label, Vector2.zero, new Vector2(200, 180), unlocked ? palette.Ocean : palette.Locked, palette.Foam, () => startLevel(level));
                button.interactable = unlocked;
            }

            return screen;
        }
    }
}
