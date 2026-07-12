using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class StoryModalView
    {
        public static void Show(
            Transform canvasRoot,
            RuntimeUiFactory ui,
            GamePalette palette,
            Func<string, object[], string> translate,
            string message,
            Action onContinue)
        {
            var panel = ui.CreateImage(canvasRoot, "Modal", new Vector2(.5f, .5f), new Vector2(920, 500), palette.ModalBackground);
            panel.transform.SetAsLastSibling();
            ui.CreateText(panel.transform, translate("modal_title", null), 34, palette.Gold, new Vector2(.5f, .70f), new Vector2(760, 55), TextAnchor.MiddleCenter);
            ui.CreateText(panel.transform, message, 29, palette.Foam, new Vector2(.5f, .47f), new Vector2(740, 180), TextAnchor.MiddleCenter);
            ui.CreateButton(panel.transform, translate("continue", null), new Vector2(.5f, .18f), new Vector2(310, 76), palette.Gold, palette.Navy, () =>
            {
                UnityEngine.Object.Destroy(panel.gameObject);
                onContinue?.Invoke();
            });
        }
    }
}
