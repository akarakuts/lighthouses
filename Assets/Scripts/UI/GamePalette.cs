using UnityEngine;

namespace LighthouseMatch3.UI
{
    public sealed class GamePalette
    {
        public Color Navy { get; } = new Color(0.025f, 0.15f, 0.23f);
        public Color Ocean { get; } = new Color(0.035f, 0.39f, 0.57f);
        public Color Foam { get; } = new Color(0.91f, 0.99f, 0.98f);
        public Color Gold { get; } = new Color(1f, 0.76f, 0.25f);
        public Color CellBackground { get; } = new Color(.045f, .31f, .40f);
        public Color Subtitle { get; } = new Color(0.72f, 0.94f, 0.95f);
        public Color Goal { get; } = new Color(.72f, .98f, .90f);
        public Color Hint { get; } = new Color(.70f, .90f, .92f);
        public Color Locked { get; } = new Color(0.025f, 0.12f, 0.17f, .90f);
        public Color ModalBackground { get; } = new Color(.025f, .13f, .20f, .97f);
        public Color SettingsNote { get; } = new Color(.72f, .94f, .95f);

        public Color[] TileColors { get; } =
        {
            new Color(0.96f, 0.37f, 0.31f), new Color(0.99f, 0.71f, 0.25f),
            new Color(0.20f, 0.75f, 0.85f), new Color(0.98f, 0.81f, 0.22f),
            new Color(0.55f, 0.38f, 0.86f), new Color(0.25f, 0.83f, 0.49f)
        };
    }
}
