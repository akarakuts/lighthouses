using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3
{
    public sealed class RuntimeUiFactory
    {
        private readonly TMP_FontAsset _font;

        public RuntimeUiFactory()
        {
            _font = TMP_Settings.defaultFontAsset ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }

        public Image CreateImage(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public TextMeshProUGUI CreateText(Transform parent, string value, int fontSize, Color color, Vector2 anchor, Vector2 size, TextAnchor alignment)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var text = go.GetComponent<TextMeshProUGUI>();
            if (_font != null) text.font = _font;
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = ToTextAlignment(alignment);
            text.color = color;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            return text;
        }

        public Button CreateButton(Transform parent, string label, Vector2 anchor, Vector2 size, Color color, Color textColor, Action action)
        {
            Image image = CreateImage(parent, "Button", anchor, size, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());
            CreateText(image.transform, label, 25, textColor, new Vector2(.5f, .5f), size - new Vector2(16, 12), TextAnchor.MiddleCenter);
            return button;
        }

        public GameObject CreateFullScreenPanel(Transform parent, string name, Color color)
        {
            var panel = CreateImage(parent, name, new Vector2(.5f, .5f), new Vector2(1080, 1920), color);
            panel.rectTransform.anchorMin = Vector2.zero;
            panel.rectTransform.anchorMax = Vector2.one;
            panel.rectTransform.offsetMin = Vector2.zero;
            panel.rectTransform.offsetMax = Vector2.zero;
            var backdrop = new GameObject("Ocean Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(OceanBackdrop));
            backdrop.transform.SetParent(panel.transform, false);
            var backdropRect = backdrop.GetComponent<RectTransform>();
            backdropRect.anchorMin = Vector2.zero;
            backdropRect.anchorMax = Vector2.one;
            backdropRect.offsetMin = Vector2.zero;
            backdropRect.offsetMax = Vector2.zero;
            backdrop.transform.SetAsFirstSibling();
            return panel.gameObject;
        }

        public void ApplyArtwork(Image image, string resourcePath)
        {
            Sprite sprite = ArtworkCatalog.LoadSprite(resourcePath);
            if (sprite == null) return;
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
        }

        public TextMeshProUGUI CreateToast(Transform parent, string message, int fontSize, Color color, Vector2 anchor, Vector2 size, float lifetimeSeconds = 2f)
        {
            TextMeshProUGUI toast = CreateText(parent, message, fontSize, color, anchor, size, TextAnchor.MiddleCenter);
            UnityEngine.Object.Destroy(toast.gameObject, lifetimeSeconds);
            return toast;
        }

        private static TextAlignmentOptions ToTextAlignment(TextAnchor anchor) => anchor switch
        {
            TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
            TextAnchor.UpperCenter => TextAlignmentOptions.Top,
            TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
            TextAnchor.MiddleLeft => TextAlignmentOptions.MidlineLeft,
            TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
            TextAnchor.MiddleRight => TextAlignmentOptions.MidlineRight,
            TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
            TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
            TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
            _ => TextAlignmentOptions.Center
        };
    }
}
