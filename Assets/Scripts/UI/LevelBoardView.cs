using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class LevelBoardView
    {
        private readonly BoardCellPool _pool = new BoardCellPool();
        private readonly RuntimeUiFactory _ui;
        private readonly GamePalette _palette;

        public Button[,] CellButtons { get; private set; }
        public Image[,] TileImages => _tileImages;
        private Image[,] _tileImages;
        public TextMeshProUGUI[,] Markers { get; private set; }
        public TextMeshProUGUI MovesText { get; private set; }
        public TextMeshProUGUI GoalText { get; private set; }
        public TextMeshProUGUI ScoreText { get; private set; }

        public LevelBoardView(RuntimeUiFactory ui, GamePalette palette)
        {
            _ui = ui;
            _palette = palette;
        }

        public GameObject Build(
            Transform canvasRoot,
            LevelDefinition level,
            Func<string, object[], string> translate,
            Action showMap,
            Action restartLevel,
            Action<int, int> selectCell)
        {
            var screen = _ui.CreateFullScreenPanel(canvasRoot, "LevelScreen", _palette.Navy);
            _ui.CreateButton(screen.transform, "<", new Vector2(.09f, .93f), new Vector2(86, 72), _palette.Ocean, _palette.Foam, showMap);
            _ui.CreateText(screen.transform, translate("level", new object[] { level.Id }), 38, _palette.Foam, new Vector2(.5f, .935f), new Vector2(340, 65), TextAnchor.MiddleCenter);
            ScoreText = _ui.CreateText(screen.transform, translate("score", new object[] { 0 }), 26, _palette.Gold, new Vector2(.82f, .935f), new Vector2(210, 55), TextAnchor.MiddleCenter);
            MovesText = _ui.CreateText(screen.transform, string.Empty, 32, _palette.Foam, new Vector2(.25f, .85f), new Vector2(360, 70), TextAnchor.MiddleCenter);
            GoalText = _ui.CreateText(screen.transform, string.Empty, 26, _palette.Goal, new Vector2(.66f, .85f), new Vector2(590, 80), TextAnchor.MiddleCenter);

            var boardRoot = new GameObject("Board", typeof(RectTransform), typeof(GridLayoutGroup));
            boardRoot.transform.SetParent(screen.transform, false);
            var rect = boardRoot.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(.5f, .43f);
            rect.anchorMax = new Vector2(.5f, .43f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(820, 820);
            var grid = boardRoot.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(92, 92);
            grid.spacing = new Vector2(8, 8);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = LevelCatalog.BoardSize;
            grid.childAlignment = TextAnchor.MiddleCenter;

            int size = LevelCatalog.BoardSize;
            CellButtons = new Button[size, size];
            _tileImages = new Image[size, size];
            Markers = new TextMeshProUGUI[size, size];
            for (int y = size - 1; y >= 0; y--)
            for (int x = 0; x < size; x++)
            {
                int cx = x;
                int cy = y;
                var cell = _pool.Rent(boardRoot.transform, $"Cell {x},{y}");
                var background = cell.GetComponent<Image>();
                background.color = _palette.CellBackground;
                var button = cell.GetComponent<Button>();
                button.targetGraphic = background;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => selectCell(cx, cy));
                CellButtons[x, y] = button;

                var tile = _ui.CreateImage(cell.transform, "Tile", new Vector2(.5f, .5f), new Vector2(76, 76), Color.white);
                _tileImages[x, y] = tile;
                Markers[x, y] = _ui.CreateText(cell.transform, "", 25, Color.white, new Vector2(.5f, .5f), new Vector2(74, 35), TextAnchor.MiddleCenter);
            }

            _ui.CreateText(screen.transform, translate("tap_swap", null), 24, _palette.Hint, new Vector2(.5f, .12f), new Vector2(800, 50), TextAnchor.MiddleCenter);
            _ui.CreateButton(screen.transform, translate("restart", null), new Vector2(.30f, .055f), new Vector2(270, 70), _palette.Ocean, _palette.Foam, restartLevel);
            _ui.CreateButton(screen.transform, translate("map", null), new Vector2(.70f, .055f), new Vector2(270, 70), _palette.Ocean, _palette.Foam, showMap);
            return screen;
        }

        public void ReleaseCells()
        {
            if (CellButtons == null) return;
            var cells = new System.Collections.Generic.List<GameObject>();
            int size = LevelCatalog.BoardSize;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                if (CellButtons[x, y] != null) cells.Add(CellButtons[x, y].gameObject);
            _pool.ReturnAll(cells);
            CellButtons = null;
            _tileImages = null;
            Markers = null;
            MovesText = null;
            GoalText = null;
            ScoreText = null;
        }

        public void Refresh(
            TileState[,] board,
            BlockerState[,] blockers,
            Vector2Int? selected,
            LevelDefinition level,
            int moves,
            int score,
            int collected,
            int blockersLeft,
            Func<string, object[], string> translate)
        {
            if (_tileImages == null) return;
            int size = LevelCatalog.BoardSize;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                TileState tile = board[x, y];
                if (tile == null)
                {
                    _tileImages[x, y].color = Color.clear;
                    Markers[x, y].text = string.Empty;
                    continue;
                }
                _tileImages[x, y].color = _palette.TileColors[(int)tile.Kind];
                CellButtons[x, y].image.color = selected.HasValue && selected.Value.x == x && selected.Value.y == y ? _palette.Gold : _palette.CellBackground;
                string glyph = TileGlyph(tile.Kind);
                string special = tile.Special == SpecialKind.Beam ? "B" : tile.Special == SpecialKind.Bomb ? "O" : tile.Special == SpecialKind.Pearl ? "P" : "";
                string blocker = BlockerMarker(blockers[x, y]);
                string state = string.IsNullOrEmpty(blocker) ? special : blocker;
                Markers[x, y].text = string.IsNullOrEmpty(state) ? glyph : $"{glyph}\n{state}";
                Markers[x, y].color = string.IsNullOrEmpty(state) ? _palette.Navy : Color.white;
            }

            if (MovesText == null) return;
            MovesText.text = translate("moves", new object[] { moves });
            ScoreText.text = translate("score", new object[] { score });
            string blockerGoal = level.Blocker == BlockerKind.None ? "" : translate("clear_goal", new object[] { blockersLeft });
            GoalText.text = translate("goal", new object[] { TileName(level.CollectKind, translate), collected, level.CollectTarget, blockerGoal });
        }

        public void ShowToast(Transform screenRoot, string message) =>
            _ui.CreateToast(screenRoot, message, 25, _palette.Gold, new Vector2(.5f, .77f), new Vector2(700, 42));

        private static string BlockerMarker(BlockerState blocker)
        {
            if (blocker == null || blocker.Kind == BlockerKind.None) return "";
            if (blocker.Kind == BlockerKind.Ice) return "I";
            if (blocker.Kind == BlockerKind.Crate) return blocker.HitsRemaining > 1 ? $"C{blocker.HitsRemaining}" : "C";
            return blocker.Kind == BlockerKind.Seaweed ? "W" : "";
        }

        private static string TileName(TileKind kind, Func<string, object[], string> translate) => translate(kind switch
        {
            TileKind.Coral => "coral",
            TileKind.Shell => "shell",
            TileKind.Drop => "drop",
            TileKind.Sunstone => "sunstone",
            TileKind.Starfish => "starfish",
            _ => "crystal"
        }, null);

        private static string TileGlyph(TileKind kind) => kind switch
        {
            TileKind.Coral => "C",
            TileKind.Shell => "S",
            TileKind.Drop => "D",
            TileKind.Sunstone => "U",
            TileKind.Starfish => "F",
            _ => "R"
        };
    }
}
