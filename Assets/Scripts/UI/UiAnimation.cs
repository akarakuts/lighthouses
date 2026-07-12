using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public static class UiAnimation
    {
        public static IEnumerator ScalePulse(Transform target, float peakScale, float duration)
        {
            if (target == null) yield break;
            Vector3 original = target.localScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = t < 0.5f
                    ? Mathf.Lerp(1f, peakScale, t * 2f)
                    : Mathf.Lerp(peakScale, 1f, (t - 0.5f) * 2f);
                target.localScale = original * scale;
                yield return null;
            }
            target.localScale = original;
        }

        public static IEnumerator FlashImage(Image image, Color flashColor, float duration)
        {
            if (image == null) yield break;
            Color original = image.color;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                image.color = Color.Lerp(original, flashColor, t < 0.5f ? t * 2f : (1f - t) * 2f);
                yield return null;
            }
            image.color = original;
        }

        public static IEnumerator PulseMatchedTiles(Image[,] tileImages, HashSet<int> matches, int boardSize, float duration)
        {
            if (tileImages == null || matches == null) yield break;
            var transforms = new System.Collections.Generic.List<Transform>();
            foreach (int index in matches)
            {
                int x = index % boardSize;
                int y = index / boardSize;
                if (tileImages[x, y] != null) transforms.Add(tileImages[x, y].transform);
            }
            foreach (Transform transform in transforms)
                yield return ScalePulse(transform, 1.25f, duration);
        }
    }
}
