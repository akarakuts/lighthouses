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
            AddQuad(vh, rect.xMin, rect.yMin, rect.xMax, rect.yMax, new Color(.015f, .10f, .16f, .24f));

            float[] heights = { .12f, .31f, .55f, .76f };
            Color[] colors =
            {
                new Color(.03f, .31f, .42f, .16f), new Color(.06f, .43f, .53f, .13f),
                new Color(.16f, .55f, .61f, .11f), new Color(.36f, .68f, .68f, .08f)
            };
            for (int i = 0; i < heights.Length; i++)
            {
                float y = Mathf.Lerp(rect.yMin, rect.yMax, heights[i]);
                AddQuad(vh, rect.xMin, y, rect.xMax, y + rect.height * .10f, colors[i]);
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
