using System;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3
{
    public sealed class RuntimeUiFactory
    {
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

        public Text CreateText(Transform parent, string value, int fontSize, Color color, Vector2 anchor, Vector2 size, TextAnchor alignment)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = Mathf.Max(24, Mathf.RoundToInt(fontSize * 1.18f));
            text.fontStyle = text.fontSize >= 32 ? FontStyle.Bold : FontStyle.Normal;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.lineSpacing = 1.05f;
            text.raycastTarget = false;

            var outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(.005f, .04f, .07f, .9f);
            outline.effectDistance = new Vector2(1.6f, -1.6f);
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, .7f);
            shadow.effectDistance = new Vector2(3f, -3f);
            return text;
        }

        public Button CreateButton(Transform parent, string label, Vector2 anchor, Vector2 size, Color color, Color textColor, Action action)
        {
            Image image = CreateImage(parent, "Button", anchor, size, color);
            image.sprite = PuzzleSpriteLibrary.ButtonPanel();
            image.type = Image.Type.Sliced;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());
            int labelSize = size.y >= 150f ? 34 : size.y >= 80f ? 29 : size.y >= 70f ? 26 : 23;
            CreateText(image.transform, label, labelSize, textColor, new Vector2(.5f, .5f), size - new Vector2(16, 12), TextAnchor.MiddleCenter);
            return button;
        }

        public GameObject CreateFullScreenPanel(Transform parent, string name, Color color)
        {
            var panel = CreateImage(parent, name, new Vector2(.5f, .5f), new Vector2(1080, 1920), color);
            panel.rectTransform.anchorMin = Vector2.zero;
            panel.rectTransform.anchorMax = Vector2.one;
            panel.rectTransform.offsetMin = Vector2.zero;
            panel.rectTransform.offsetMax = Vector2.zero;
            Image atmosphere = CreateImage(panel.transform, "Archipelago Atmosphere", new Vector2(.5f, .48f), new Vector2(980, 1420), new Color(1f, 1f, 1f, .44f));
            ApplyArtwork(atmosphere, "Artwork/LighthouseIsland-v1");
            atmosphere.color = new Color(1f, 1f, 1f, .44f);
            atmosphere.transform.SetAsFirstSibling();
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

        public Text CreateToast(Transform parent, string message, int fontSize, Color color, Vector2 anchor, Vector2 size, float lifetimeSeconds = 2f)
        {
            Text toast = CreateText(parent, message, fontSize, color, anchor, size, TextAnchor.MiddleCenter);
            UnityEngine.Object.Destroy(toast.gameObject, lifetimeSeconds);
            return toast;
        }

    }
}
