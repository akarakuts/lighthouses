using System.Collections.Generic;
using UnityEngine;

namespace LighthouseMatch3
{
    public static class ArtworkCatalog
    {
        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        public static Sprite LoadSprite(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath)) return null;
            if (Sprites.TryGetValue(resourcePath, out Sprite cached)) return cached;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            Sprites[resourcePath] = sprite;
            return sprite;
        }

        public static void ClearCache() => Sprites.Clear();
    }
}
