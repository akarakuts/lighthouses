using System;
using System.Collections.Generic;
using UnityEngine;

namespace LighthouseMatch3
{
    public static class PuzzleSpriteLibrary
    {
        private const int Size = 128;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite Tile(TileKind kind) => Get($"tile-{kind}", pixels => DrawTile(pixels, kind));
        public static Sprite Blocker(BlockerKind kind) => kind == BlockerKind.None ? null : Get($"blocker-{kind}", pixels => DrawBlocker(pixels, kind));
        public static Sprite Special(SpecialKind kind) => kind == SpecialKind.None ? null : Get($"special-{kind}", pixels => DrawSpecial(pixels, kind));
        public static Sprite ButtonPanel() => Get("button-panel", DrawButtonPanel);
        public static Sprite CellPanel() => Get("cell-panel", DrawCellPanel);

        private static Sprite Get(string key, Action<Color[]> draw)
        {
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;
            var pixels = new Color[Size * Size];
            draw(pixels);
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                name = key,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            sprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, new Vector4(18, 18, 18, 18));
            Cache[key] = sprite;
            return sprite;
        }

        private static void DrawTile(Color[] pixels, TileKind kind)
        {
            DrawCircle(pixels, new Vector2(64, 61), 46, new Color(.02f, .12f, .18f, .8f));
            switch (kind)
            {
                case TileKind.Coral:
                    DrawCircle(pixels, new Vector2(46, 51), 24, new Color(.95f, .22f, .30f));
                    DrawCircle(pixels, new Vector2(65, 45), 28, new Color(1f, .38f, .38f));
                    DrawCircle(pixels, new Vector2(81, 59), 22, new Color(.86f, .14f, .29f));
                    DrawCircle(pixels, new Vector2(60, 76), 21, new Color(.98f, .32f, .43f));
                    break;
                case TileKind.Shell:
                    DrawCircle(pixels, new Vector2(64, 61), 39, new Color(1f, .67f, .18f));
                    for (int x = 30; x <= 98; x += 13) DrawLine(pixels, new Vector2(64, 35), new Vector2(x, 91), 4, new Color(1f, .88f, .43f));
                    break;
                case TileKind.Drop:
                    DrawDiamond(pixels, new Vector2(64, 58), 31, 43, new Color(.13f, .78f, .94f));
                    DrawCircle(pixels, new Vector2(55, 50), 12, new Color(.65f, .96f, 1f));
                    break;
                case TileKind.Sunstone:
                    DrawDiamond(pixels, new Vector2(64, 62), 39, 34, new Color(1f, .77f, .13f));
                    DrawDiamond(pixels, new Vector2(64, 62), 22, 19, new Color(1f, .95f, .49f));
                    break;
                case TileKind.Starfish:
                    DrawStar(pixels, new Vector2(64, 62), 43, 19, new Color(.61f, .34f, .91f));
                    DrawCircle(pixels, new Vector2(64, 62), 16, new Color(.76f, .54f, 1f));
                    break;
                default:
                    DrawDiamond(pixels, new Vector2(64, 60), 34, 47, new Color(.17f, .83f, .47f));
                    DrawDiamond(pixels, new Vector2(54, 54), 12, 18, new Color(.68f, 1f, .79f));
                    break;
            }
            DrawCircle(pixels, new Vector2(51, 82), 7, new Color(1f, 1f, 1f, .55f));
        }

        private static void DrawBlocker(Color[] pixels, BlockerKind kind)
        {
            if (kind == BlockerKind.Ice)
            {
                DrawRoundedRect(pixels, 17, 17, 94, 94, 19, new Color(.69f, .94f, 1f, .66f));
                DrawLine(pixels, new Vector2(35, 86), new Vector2(91, 43), 3, Color.white);
                DrawLine(pixels, new Vector2(63, 92), new Vector2(63, 39), 2, new Color(.88f, 1f, 1f));
                DrawLine(pixels, new Vector2(39, 53), new Vector2(83, 76), 2, new Color(.88f, 1f, 1f));
                return;
            }
            if (kind == BlockerKind.Crate)
            {
                DrawRoundedRect(pixels, 15, 15, 98, 98, 14, new Color(.42f, .20f, .08f));
                DrawRoundedRect(pixels, 21, 21, 86, 86, 9, new Color(.71f, .37f, .12f));
                DrawLine(pixels, new Vector2(28, 29), new Vector2(100, 97), 8, new Color(.46f, .21f, .07f));
                DrawLine(pixels, new Vector2(100, 29), new Vector2(28, 97), 8, new Color(.46f, .21f, .07f));
                return;
            }
            if (kind == BlockerKind.Seaweed)
            {
                DrawLine(pixels, new Vector2(39, 25), new Vector2(48, 98), 8, new Color(.03f, .42f, .23f));
                DrawLine(pixels, new Vector2(63, 22), new Vector2(61, 102), 9, new Color(.06f, .57f, .29f));
                DrawLine(pixels, new Vector2(86, 27), new Vector2(77, 99), 8, new Color(.02f, .37f, .20f));
                DrawCircle(pixels, new Vector2(47, 92), 10, new Color(.20f, .72f, .38f));
                DrawCircle(pixels, new Vector2(75, 78), 11, new Color(.17f, .67f, .32f));
            }
        }

        private static void DrawSpecial(Color[] pixels, SpecialKind kind)
        {
            if (kind == SpecialKind.Beam)
            {
                DrawRoundedRect(pixels, 13, 54, 102, 20, 10, new Color(1f, .82f, .16f, .94f));
                DrawRoundedRect(pixels, 28, 60, 72, 8, 4, Color.white);
                DrawCircle(pixels, new Vector2(64, 64), 18, new Color(1f, .51f, .08f));
                return;
            }
            if (kind == SpecialKind.Bomb)
            {
                DrawCircle(pixels, new Vector2(62, 58), 34, new Color(.12f, .14f, .19f));
                DrawCircle(pixels, new Vector2(52, 69), 10, new Color(.45f, .51f, .60f));
                DrawLine(pixels, new Vector2(83, 84), new Vector2(101, 105), 5, new Color(.81f, .52f, .13f));
                DrawCircle(pixels, new Vector2(103, 106), 7, new Color(1f, .85f, .20f));
                return;
            }
            if (kind == SpecialKind.Pearl)
            {
                DrawCircle(pixels, new Vector2(64, 64), 38, new Color(.84f, .78f, 1f));
                DrawCircle(pixels, new Vector2(56, 73), 25, Color.white);
                DrawCircle(pixels, new Vector2(49, 80), 8, new Color(1f, 1f, 1f, .9f));
            }
        }

        private static void DrawButtonPanel(Color[] pixels)
        {
            DrawRoundedRect(pixels, 4, 4, 120, 120, 26, Color.white);
            DrawRoundedRect(pixels, 10, 10, 108, 102, 20, new Color(1f, 1f, 1f, .25f));
        }

        private static void DrawCellPanel(Color[] pixels)
        {
            DrawRoundedRect(pixels, 3, 3, 122, 122, 22, new Color(.01f, .07f, .11f, .96f));
            DrawRoundedRect(pixels, 9, 9, 110, 110, 17, new Color(.1f, .38f, .48f, .85f));
        }

        private static void DrawCircle(Color[] pixels, Vector2 center, float radius, Color color)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
            int maxX = Mathf.Min(Size - 1, Mathf.CeilToInt(center.x + radius));
            int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
            int maxY = Mathf.Min(Size - 1, Mathf.CeilToInt(center.y + radius));
            float squared = radius * radius;
            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                if ((new Vector2(x, y) - center).sqrMagnitude <= squared) Blend(pixels, x, y, color);
        }

        private static void DrawDiamond(Color[] pixels, Vector2 center, float radiusX, float radiusY, Color color)
        {
            for (int y = Mathf.Max(0, Mathf.FloorToInt(center.y - radiusY)); y <= Mathf.Min(Size - 1, Mathf.CeilToInt(center.y + radiusY)); y++)
            for (int x = Mathf.Max(0, Mathf.FloorToInt(center.x - radiusX)); x <= Mathf.Min(Size - 1, Mathf.CeilToInt(center.x + radiusX)); x++)
                if (Mathf.Abs((x - center.x) / radiusX) + Mathf.Abs((y - center.y) / radiusY) <= 1f) Blend(pixels, x, y, color);
        }

        private static void DrawStar(Color[] pixels, Vector2 center, float outer, float inner, Color color)
        {
            var points = new Vector2[10];
            for (int i = 0; i < points.Length; i++)
            {
                float angle = Mathf.Deg2Rad * (90 + i * 36);
                float radius = i % 2 == 0 ? outer : inner;
                points[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            FillPolygon(pixels, points, color);
        }

        private static void FillPolygon(Color[] pixels, Vector2[] points, Color color)
        {
            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
                if (InsidePolygon(new Vector2(x, y), points)) Blend(pixels, x, y, color);
        }

        private static bool InsidePolygon(Vector2 point, Vector2[] vertices)
        {
            bool inside = false;
            for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[j];
                if ((a.y > point.y) != (b.y > point.y) && point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x) inside = !inside;
            }
            return inside;
        }

        private static void DrawRoundedRect(Color[] pixels, int left, int bottom, int width, int height, int radius, Color color)
        {
            for (int y = bottom; y < bottom + height; y++)
            for (int x = left; x < left + width; x++)
            {
                float dx = Mathf.Max(left + radius - x, 0, x - (left + width - radius - 1));
                float dy = Mathf.Max(bottom + radius - y, 0, y - (bottom + height - radius - 1));
                if (dx * dx + dy * dy <= radius * radius) Blend(pixels, x, y, color);
            }
        }

        private static void DrawLine(Color[] pixels, Vector2 from, Vector2 to, float width, Color color)
        {
            Vector2 delta = to - from;
            float length = delta.magnitude;
            int steps = Mathf.CeilToInt(length * 1.5f);
            for (int i = 0; i <= steps; i++) DrawCircle(pixels, Vector2.Lerp(from, to, i / (float)steps), width * .5f, color);
        }

        private static void Blend(Color[] pixels, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= Size || y >= Size) return;
            int index = x + y * Size;
            Color current = pixels[index];
            float alpha = color.a + current.a * (1f - color.a);
            if (alpha <= 0f) return;
            pixels[index] = new Color(
                (color.r * color.a + current.r * current.a * (1f - color.a)) / alpha,
                (color.g * color.a + current.g * current.a * (1f - color.a)) / alpha,
                (color.b * color.a + current.b * current.a * (1f - color.a)) / alpha,
                alpha);
        }
    }
}
