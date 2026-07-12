using UnityEngine;

namespace LighthouseMatch3.UI
{
    public sealed class GamePalette
    {
        public Color Navy { get; } = new Color(0.035f, 0.12f, 0.19f);
        public Color Ocean { get; } = new Color(0.055f, 0.31f, 0.43f);
        public Color Foam { get; } = new Color(0.82f, 0.95f, 0.93f);
        public Color Gold { get; } = new Color(1f, 0.71f, 0.20f);
        public Color CellBackground { get; } = new Color(.05f, .23f, .30f);
        public Color Subtitle { get; } = new Color(0.54f, 0.83f, 0.84f);
        public Color Goal { get; } = new Color(.60f, .91f, .87f);
        public Color Hint { get; } = new Color(.55f, .77f, .79f);
        public Color Locked { get; } = new Color(0.10f, 0.17f, 0.20f);
        public Color ModalBackground { get; } = new Color(.04f, .16f, .23f, .98f);
        public Color SettingsNote { get; } = new Color(.60f, .84f, .84f);

        public Color[] TileColors { get; } =
        {
            new Color(0.96f, 0.37f, 0.31f), new Color(0.99f, 0.71f, 0.25f),
            new Color(0.20f, 0.75f, 0.85f), new Color(0.98f, 0.81f, 0.22f),
            new Color(0.55f, 0.38f, 0.86f), new Color(0.25f, 0.83f, 0.49f)
        };
    }
}
