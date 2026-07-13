using System.Collections;
using System.Collections.Generic;
using LighthouseMatch3;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public static class UiAnimation
    {
        private sealed class FallFlyer
        {
            public RectTransform Rect;
            public Vector3 Start;
            public Vector3 End;
            public Image Image;
            public Image Overlay;
        }

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

        public static IEnumerator AnimateTileFalls(LevelBoardView board, IReadOnlyList<TileFallMove> moves, int boardSize, float duration)
        {
            if (board == null || moves == null || moves.Count == 0 || board.AnimationRoot == null) yield break;

            var flyers = new List<FallFlyer>();
            foreach (TileFallMove move in moves)
            {
                if (move.FromY == move.ToY) continue;

                if (move.FromY < boardSize) board.SetTileVisible(move.X, move.FromY, false);
                board.SetTileVisible(move.X, move.ToY, false);

                Image tile = board.TileImages[move.X, move.ToY];
                Image flyer = CreateFlyerImage(board.AnimationRoot, move.Tile, tile != null ? tile.rectTransform.sizeDelta : new Vector2(76f, 76f));
                Image flyerOverlay = CreateFlyerOverlay(flyer.transform, move.Tile);

                var rect = flyer.rectTransform;
                Vector3 start = board.GetFallStartWorldPosition(move.X, move.FromY, boardSize);
                Vector3 end = board.GetCellWorldPosition(move.X, move.ToY);
                rect.position = start;
                flyers.Add(new FallFlyer { Rect = rect, Start = start, End = end, Image = flyer, Overlay = flyerOverlay });
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                foreach (FallFlyer flyer in flyers)
                    flyer.Rect.position = Vector3.Lerp(flyer.Start, flyer.End, t);
                yield return null;
            }

            foreach (FallFlyer flyer in flyers)
            {
                if (flyer.Image != null) Object.Destroy(flyer.Image.gameObject);
            }
        }

        private static Image CreateFlyerImage(Transform parent, TileState tile, Vector2 size)
        {
            var go = new GameObject("FallFlyer", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.sprite = PuzzleSpriteLibrary.Tile(tile.Kind);
            image.raycastTarget = false;
            return image;
        }

        private static Image CreateFlyerOverlay(Transform parent, TileState tile)
        {
            var go = new GameObject("FallOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(.5f, .5f);
            rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(70f, 70f);
            var image = go.GetComponent<Image>();
            Sprite overlay = PuzzleSpriteLibrary.Special(tile.Special);
            if (overlay != null)
            {
                image.sprite = overlay;
                image.color = Color.white;
            }
            else image.color = Color.clear;
            image.raycastTarget = false;
            return image;
        }

        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    }
}
