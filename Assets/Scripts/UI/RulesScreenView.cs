using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class RulesScreenView
    {
        public static GameObject Build(
            Transform canvasRoot,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            Action showMap)
        {
            var screen = ui.CreateFullScreenPanel(canvasRoot, "RulesScreen", palette.Navy);
            ui.CreateText(screen.transform, translate("rules_title", null), 48, palette.Foam, new Vector2(.5f, .92f), new Vector2(900, 70), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("rules_intro", null), 26, palette.Subtitle, new Vector2(.5f, .855f), new Vector2(900, 60), TextAnchor.MiddleCenter);

            ui.CreateText(screen.transform, translate("rules_swap_title", null), 29, palette.Gold, new Vector2(.5f, .775f), new Vector2(840, 44), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("rules_swap", null), 25, palette.Foam, new Vector2(.5f, .73f), new Vector2(880, 55), TextAnchor.MiddleCenter);

            ui.CreateText(screen.transform, translate("rules_specials_title", null), 30, palette.Gold, new Vector2(.5f, .65f), new Vector2(840, 46), TextAnchor.MiddleCenter);
            CreateSpecialRule(screen.transform, ui, palette, translate, .20f, SpecialKind.Beam, "rules_beam_title", "rules_beam");
            CreateSpecialRule(screen.transform, ui, palette, translate, .50f, SpecialKind.Bomb, "rules_bomb_title", "rules_bomb");
            CreateSpecialRule(screen.transform, ui, palette, translate, .80f, SpecialKind.Pearl, "rules_pearl_title", "rules_pearl");

            ui.CreateText(screen.transform, translate("rules_blockers_title", null), 29, palette.Gold, new Vector2(.5f, .30f), new Vector2(880, 44), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("rules_blockers", null), 24, palette.Foam, new Vector2(.5f, .245f), new Vector2(900, 80), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("rules_goal_title", null), 29, palette.Gold, new Vector2(.5f, .16f), new Vector2(880, 44), TextAnchor.MiddleCenter);
            ui.CreateText(screen.transform, translate("rules_goal", null), 24, palette.Foam, new Vector2(.5f, .11f), new Vector2(900, 64), TextAnchor.MiddleCenter);
            ui.CreateButton(screen.transform, translate("back_to_map", null), new Vector2(.5f, .045f), new Vector2(330, 64), palette.Ocean, palette.Foam, showMap);
            return screen;
        }

        private static void CreateSpecialRule(
            Transform parent,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            float x,
            SpecialKind special,
            string titleKey,
            string descriptionKey)
        {
            Image icon = ui.CreateImage(parent, $"{special} Rule Icon", new Vector2(x, .535f), new Vector2(98, 98), Color.white);
            icon.sprite = PuzzleSpriteLibrary.Special(special);
            icon.preserveAspect = true;
            ui.CreateText(parent, translate(titleKey, null), 22, palette.Gold, new Vector2(x, .46f), new Vector2(280, 38), TextAnchor.MiddleCenter);
            ui.CreateText(parent, translate(descriptionKey, null), 20, palette.Foam, new Vector2(x, .395f), new Vector2(285, 90), TextAnchor.UpperCenter);
        }
    }
}
