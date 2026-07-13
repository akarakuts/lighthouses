using System;
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
        public Image[,] OverlayImages => _overlayImages;
        private Image[,] _tileImages;
        private Image[,] _overlayImages;
        public Text[,] Markers { get; private set; }
        public Text MovesText { get; private set; }
        public Text GoalText { get; private set; }
        public Text ScoreText { get; private set; }
        public Transform AnimationRoot { get; private set; }

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
            AnimationRoot = screen.transform;
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
            _overlayImages = new Image[size, size];
            Markers = new Text[size, size];
            for (int y = size - 1; y >= 0; y--)
            for (int x = 0; x < size; x++)
            {
                int cx = x;
                int cy = y;
                var cell = _pool.Rent(boardRoot.transform, $"Cell {x},{y}");
                var background = cell.GetComponent<Image>();
                background.sprite = PuzzleSpriteLibrary.CellPanel();
                background.type = Image.Type.Sliced;
                background.color = _palette.CellBackground;
                var button = cell.GetComponent<Button>();
                button.targetGraphic = background;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => selectCell(cx, cy));
                CellButtons[x, y] = button;

                var tile = _ui.CreateImage(cell.transform, "Tile", new Vector2(.5f, .5f), new Vector2(76, 76), Color.white);
                tile.raycastTarget = false;
                _tileImages[x, y] = tile;
                var overlay = _ui.CreateImage(cell.transform, "Effect", new Vector2(.5f, .5f), new Vector2(70, 70), Color.clear);
                overlay.raycastTarget = false;
                _overlayImages[x, y] = overlay;
                Markers[x, y] = _ui.CreateText(cell.transform, "", 18, Color.white, new Vector2(.76f, .24f), new Vector2(28, 28), TextAnchor.MiddleCenter);
                Markers[x, y].raycastTarget = false;
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
            _overlayImages = null;
            Markers = null;
            MovesText = null;
            GoalText = null;
            ScoreText = null;
            AnimationRoot = null;
        }

        public Vector3 GetCellWorldPosition(int x, int y) => CellButtons[x, y].transform.position;

        public Vector3 GetFallStartWorldPosition(int x, int fromY, int boardSize)
        {
            if (fromY < boardSize) return GetCellWorldPosition(x, fromY);

            Vector3 top = GetCellWorldPosition(x, boardSize - 1);
            Vector3 step = boardSize > 1
                ? top - GetCellWorldPosition(x, boardSize - 2)
                : new Vector3(0f, 100f, 0f);
            return top + step * (fromY - boardSize + 1);
        }

        public void SetTileVisible(int x, int y, bool visible)
        {
            if (_tileImages == null) return;
            if (_tileImages[x, y] != null)
            {
                Color color = _tileImages[x, y].color;
                color.a = visible ? 1f : 0f;
                _tileImages[x, y].color = color;
            }
            if (_overlayImages[x, y] != null)
            {
                Color overlayColor = _overlayImages[x, y].color;
                overlayColor.a = visible && _overlayImages[x, y].sprite != null ? 1f : 0f;
                _overlayImages[x, y].color = overlayColor;
            }
            if (Markers[x, y] != null)
                Markers[x, y].color = visible ? Color.white : Color.clear;
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
                    _overlayImages[x, y].color = Color.clear;
                    Markers[x, y].text = string.Empty;
                    continue;
                }
                _tileImages[x, y].sprite = PuzzleSpriteLibrary.Tile(tile.Kind);
                _tileImages[x, y].color = Color.white;
                CellButtons[x, y].image.color = selected.HasValue && selected.Value.x == x && selected.Value.y == y ? _palette.Gold : _palette.CellBackground;
                Sprite overlay = BlockerSprite(blockers[x, y]) ?? PuzzleSpriteLibrary.Special(tile.Special);
                _overlayImages[x, y].sprite = overlay;
                _overlayImages[x, y].color = overlay == null ? Color.clear : Color.white;
                Markers[x, y].text = CrateLabel(blockers[x, y]);
                Markers[x, y].color = Color.white;
            }

            if (MovesText == null) return;
            MovesText.text = translate("moves", new object[] { moves });
            ScoreText.text = translate("score", new object[] { score });
            string blockerGoal = level.Blocker == BlockerKind.None ? "" : translate("clear_goal", new object[] { blockersLeft });
            GoalText.text = translate("goal", new object[] { TileName(level.CollectKind, translate), collected, level.CollectTarget, blockerGoal });
        }

        public void ShowToast(Transform screenRoot, string message) =>
            _ui.CreateToast(screenRoot, message, 25, _palette.Gold, new Vector2(.5f, .77f), new Vector2(700, 42));

        private static Sprite BlockerSprite(BlockerState blocker)
        {
            return blocker == null || blocker.Kind == BlockerKind.None ? null : PuzzleSpriteLibrary.Blocker(blocker.Kind);
        }

        private static string CrateLabel(BlockerState blocker) =>
            blocker != null && blocker.Kind == BlockerKind.Crate && blocker.HitsRemaining > 1 ? blocker.HitsRemaining.ToString() : string.Empty;

        private static string TileName(TileKind kind, Func<string, object[], string> translate) => translate(kind switch
        {
            TileKind.Coral => "coral",
            TileKind.Shell => "shell",
            TileKind.Drop => "drop",
            TileKind.Sunstone => "sunstone",
            TileKind.Starfish => "starfish",
            _ => "crystal"
        }, null);

    }
}
