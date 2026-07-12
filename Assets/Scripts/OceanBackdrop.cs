using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3
{
    public sealed class OceanBackdrop : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = GetPixelAdjustedRect();
            AddQuad(vh, rect.xMin, rect.yMin, rect.xMax, rect.yMax, new Color(.025f, .11f, .18f, .55f));

            float[] heights = { .14f, .22f, .33f, .48f, .66f, .79f };
            Color[] colors =
            {
                new Color(.04f, .23f, .33f, .70f), new Color(.03f, .30f, .40f, .58f),
                new Color(.06f, .35f, .44f, .46f), new Color(.10f, .43f, .49f, .32f),
                new Color(.17f, .49f, .53f, .22f), new Color(.30f, .64f, .63f, .14f)
            };
            for (int i = 0; i < heights.Length; i++)
            {
                float y = Mathf.Lerp(rect.yMin, rect.yMax, heights[i]);
                AddQuad(vh, rect.xMin, y, rect.xMax, y + rect.height * .075f, colors[i]);
            }
        }

        private static void AddQuad(VertexHelper vh, float x0, float y0, float x1, float y1, Color color)
        {
            int start = vh.currentVertCount;
            vh.AddVert(new Vector3(x0, y0), color, Vector2.zero);
            vh.AddVert(new Vector3(x0, y1), color, Vector2.up);
            vh.AddVert(new Vector3(x1, y1), color, Vector2.one);
            vh.AddVert(new Vector3(x1, y0), color, Vector2.right);
            vh.AddTriangle(start, start + 1, start + 2);
            vh.AddTriangle(start + 2, start + 3, start);
        }
    }
}
