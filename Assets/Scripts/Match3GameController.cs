using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3
{
    public sealed class Match3GameController : MonoBehaviour
    {
        private readonly Color _navy = new Color(0.035f, 0.12f, 0.19f);
        private readonly Color _ocean = new Color(0.055f, 0.31f, 0.43f);
        private readonly Color _foam = new Color(0.82f, 0.95f, 0.93f);
        private readonly Color _gold = new Color(1f, 0.71f, 0.20f);
        private readonly Color[] _tileColors =
        {
            new Color(0.96f, 0.37f, 0.31f), new Color(0.99f, 0.71f, 0.25f),
            new Color(0.20f, 0.75f, 0.85f), new Color(0.98f, 0.81f, 0.22f),
            new Color(0.55f, 0.38f, 0.86f), new Color(0.25f, 0.83f, 0.49f)
        };
        private readonly Dictionary<string, Sprite> _artSprites = new Dictionary<string, Sprite>();

        private Canvas _canvas;
        private GameObject _screen;
        private BoardEngine _engine;
        private TileState[,] _board => _engine.Tiles;
        private BlockerState[,] _blockers => _engine.Blockers;
        private Button[,] _cellButtons;
        private Image[,] _tileImages;
        private Text[,] _markers;
        private Vector2Int? _selected;
        private bool _resolving;
        private LevelDefinition _level;
        private int _moves;
        private int _score;
        private int _collected;
        private int _blockersLeft;
        private Text _movesText;
        private Text _goalText;
        private Text _scoreText;

        private void Start()
        {
            CreateCanvas();
            ShowMap();
            if (!SaveService.Progress.HasSeenTutorial)
            {
                ShowStory(T("tutorial"), () =>
                {
                    SaveService.Progress.HasSeenTutorial = true;
                    SaveService.Save();
                });
            }
        }

        private void CreateCanvas()
        {
            var root = new GameObject("Game Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _canvas = root.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.4f;
        }

        private void ShowMap()
        {
            ClearScreen();
            _screen = FullScreen("MapScreen", _navy);
            CreateText(_screen.transform, T("title"), 58, _foam, new Vector2(0.5f, 0.92f), new Vector2(650, 80), TextAnchor.MiddleCenter);
            CreateText(_screen.transform, T("subtitle"), 27, new Color(0.54f, 0.83f, 0.84f), new Vector2(0.5f, 0.875f), new Vector2(650, 42), TextAnchor.MiddleCenter);

            string lifeStatus = SaveService.Progress.Lives < 5 ? $" ({SaveService.LifeRecoveryLabel()})" : string.Empty;
            var resources = CreateText(_screen.transform, T("resources", SaveService.Progress.Stars, SaveService.Progress.Coins, SaveService.Progress.Lives, lifeStatus), 26, _foam, new Vector2(0.5f, 0.825f), new Vector2(920, 50), TextAnchor.MiddleCenter);
            resources.name = "Resources";
            var daily = CreateButton(_screen.transform, SaveService.CanClaimDailyReward() ? T("daily_reward") : T("daily_claimed"), new Vector2(.28f, .757f), new Vector2(360, 78), _gold, _navy, ClaimDailyReward);
            daily.interactable = SaveService.CanClaimDailyReward();
            CreateButton(_screen.transform, T("restore"), new Vector2(.72f, .757f), new Vector2(360, 78), _ocean, _foam, ShowLighthouse);
            CreateButton(_screen.transform, T("settings"), new Vector2(.88f, .93f), new Vector2(150, 62), _ocean, _foam, ShowSettings);

            var grid = new GameObject("Level Map", typeof(RectTransform), typeof(GridLayoutGroup));
            grid.transform.SetParent(_screen.transform, false);
            var rect = grid.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.13f);
            rect.anchorMax = new Vector2(0.5f, 0.13f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(880, 1210);
            var layout = grid.GetComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(200, 180);
            layout.spacing = new Vector2(26, 26);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;
            layout.childAlignment = TextAnchor.MiddleCenter;

            for (int id = 1; id <= 20; id++)
            {
                int level = id;
                bool unlocked = level <= SaveService.Progress.UnlockedLevel;
                bool complete = SaveService.Progress.CompletedLevels.Contains(level);
                string label = complete ? $"{level}\n{T("clear")}" : unlocked ? $"{level}\n{T("play")}" : T("locked");
                var button = CreateButton(grid.transform, label, Vector2.zero, new Vector2(200, 180), unlocked ? _ocean : new Color(0.10f, 0.17f, 0.20f), _foam, () => StartLevel(level));
                button.interactable = unlocked;
            }
        }

        private void ShowLighthouse()
        {
            ClearScreen();
            _screen = FullScreen("LighthouseScreen", _navy);
            CreateText(_screen.transform, T("lighthouse"), 48, _foam, new Vector2(.5f, .9f), new Vector2(900, 74), TextAnchor.MiddleCenter);
            int stage = SaveService.Progress.LighthouseStage;
            string[] names = { "A dark tower", "The keeper's house", "The repaired pier", "The observatory", "Beacon of the archipelago" };
            CreateText(_screen.transform, names[Mathf.Min(stage, names.Length - 1)], 34, _gold, new Vector2(.5f, .79f), new Vector2(900, 60), TextAnchor.MiddleCenter);
            Image lighthouseArt = CreateImage(_screen.transform, "Lighthouse Illustration", new Vector2(.5f, .51f), new Vector2(430, 620), stage >= 4 ? _gold : _ocean);
            ApplyArtwork(lighthouseArt, "Artwork/LighthouseIsland-v1");
            CreateText(_screen.transform, stage >= 4 ? "The islands shine again." : "Bring stars from the levels to rebuild the lighthouse.", 28, _foam, new Vector2(.5f, .27f), new Vector2(850, 120), TextAnchor.MiddleCenter);
            int cost = 8 + stage * 12;
            if (stage < 4)
            {
                var upgrade = CreateButton(_screen.transform, $"Upgrade for {cost} stars", new Vector2(.5f, .18f), new Vector2(460, 78), _gold, _navy, () => UpgradeLighthouse(cost));
                upgrade.interactable = SaveService.Progress.Stars >= cost;
            }
            CreateButton(_screen.transform, T("back_to_map"), new Vector2(.5f, .09f), new Vector2(330, 70), _ocean, _foam, ShowMap);
        }

        private void UpgradeLighthouse(int cost)
        {
            if (SaveService.Progress.Stars < cost || SaveService.Progress.LighthouseStage >= 4) return;
            SaveService.Progress.Stars -= cost;
            SaveService.Progress.LighthouseStage++;
            SaveService.Save();
            ShowStory($"The lighthouse reaches stage {SaveService.Progress.LighthouseStage}. A new island answers its light.", ShowLighthouse);
        }

        private void ClaimDailyReward()
        {
            int reward = SaveService.ClaimDailyReward();
            if (reward > 0) ShowStory(T("reward", reward), ShowMap);
        }

        private void ShowSettings()
        {
            ClearScreen();
            _screen = FullScreen("SettingsScreen", _navy);
            CreateText(_screen.transform, T("settings"), 48, _foam, new Vector2(.5f, .84f), new Vector2(780, 72), TextAnchor.MiddleCenter);
            string sound = SaveService.Progress.SoundEnabled ? T("sound_on") : T("sound_off");
            CreateButton(_screen.transform, sound, new Vector2(.5f, .66f), new Vector2(490, 86), _ocean, _foam, () => ToggleSound());
            string haptics = SaveService.Progress.HapticsEnabled ? T("haptics_on") : T("haptics_off");
            CreateButton(_screen.transform, haptics, new Vector2(.5f, .54f), new Vector2(490, 86), _ocean, _foam, () => ToggleHaptics());
            CreateButton(_screen.transform, T("language"), new Vector2(.5f, .42f), new Vector2(490, 86), _ocean, _foam, ToggleLanguage);
            CreateButton(_screen.transform, T("privacy"), new Vector2(.5f, .30f), new Vector2(490, 70), _ocean, _foam, ShowPrivacy);
            CreateText(_screen.transform, T("offline_storage"), 25, new Color(.60f, .84f, .84f), new Vector2(.5f, .20f), new Vector2(760, 100), TextAnchor.MiddleCenter);
            CreateButton(_screen.transform, T("back_to_map"), new Vector2(.5f, .08f), new Vector2(330, 70), _gold, _navy, ShowMap);
        }

        private void ToggleSound()
        {
            SaveService.Progress.SoundEnabled = !SaveService.Progress.SoundEnabled;
            SaveService.Save();
            ShowSettings();
        }

        private void ToggleHaptics()
        {
            SaveService.Progress.HapticsEnabled = !SaveService.Progress.HapticsEnabled;
            SaveService.Save();
            ShowSettings();
        }

        private void ToggleLanguage()
        {
            LocalizationService.ToggleLanguage();
            ShowSettings();
        }

        private void ShowPrivacy()
        {
            ShowStory(T("privacy_copy"), ShowSettings);
        }

        private void StartLevel(int id)
        {
            if (SaveService.Progress.Lives <= 0)
            {
                ShowStory(T("no_lives", SaveService.LifeRecoveryLabel()), ShowMap);
                return;
            }
            _level = LevelCatalog.Get(id);
            _moves = _level.Moves;
            _score = 0;
            _collected = 0;
            _blockersLeft = _level.BlockerCount;
            TelemetryService.LevelStarted();
            _selected = null;
            _resolving = false;
            CreateBoard();
            BuildLevelScreen();
            RefreshBoard();
        }

        private void BuildLevelScreen()
        {
            ClearScreen();
            _screen = FullScreen("LevelScreen", _navy);
            CreateButton(_screen.transform, "<", new Vector2(.09f, .93f), new Vector2(86, 72), _ocean, _foam, ShowMap);
            CreateText(_screen.transform, T("level", _level.Id), 38, _foam, new Vector2(.5f, .935f), new Vector2(340, 65), TextAnchor.MiddleCenter);
            _scoreText = CreateText(_screen.transform, T("score", 0), 26, _gold, new Vector2(.82f, .935f), new Vector2(210, 55), TextAnchor.MiddleCenter);
            _movesText = CreateText(_screen.transform, string.Empty, 32, _foam, new Vector2(.25f, .85f), new Vector2(360, 70), TextAnchor.MiddleCenter);
            _goalText = CreateText(_screen.transform, string.Empty, 26, new Color(.60f, .91f, .87f), new Vector2(.66f, .85f), new Vector2(590, 80), TextAnchor.MiddleCenter);

            var boardRoot = new GameObject("Board", typeof(RectTransform), typeof(GridLayoutGroup));
            boardRoot.transform.SetParent(_screen.transform, false);
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

            _cellButtons = new Button[LevelCatalog.BoardSize, LevelCatalog.BoardSize];
            _tileImages = new Image[LevelCatalog.BoardSize, LevelCatalog.BoardSize];
            _markers = new Text[LevelCatalog.BoardSize, LevelCatalog.BoardSize];
            for (int y = LevelCatalog.BoardSize - 1; y >= 0; y--)
            for (int x = 0; x < LevelCatalog.BoardSize; x++)
            {
                int cx = x;
                int cy = y;
                var cell = new GameObject($"Cell {x},{y}", typeof(RectTransform), typeof(Image), typeof(Button));
                cell.transform.SetParent(boardRoot.transform, false);
                var background = cell.GetComponent<Image>();
                background.color = new Color(.05f, .23f, .30f);
                var button = cell.GetComponent<Button>();
                button.targetGraphic = background;
                button.onClick.AddListener(() => SelectCell(cx, cy));
                _cellButtons[x, y] = button;

                var tile = CreateImage(cell.transform, "Tile", new Vector2(.5f, .5f), new Vector2(76, 76), Color.white);
                _tileImages[x, y] = tile;
                _markers[x, y] = CreateText(cell.transform, "", 25, Color.white, new Vector2(.5f, .5f), new Vector2(74, 35), TextAnchor.MiddleCenter);
            }
            CreateText(_screen.transform, T("tap_swap"), 24, new Color(.55f, .77f, .79f), new Vector2(.5f, .12f), new Vector2(800, 50), TextAnchor.MiddleCenter);
            CreateButton(_screen.transform, T("restart"), new Vector2(.30f, .055f), new Vector2(270, 70), _ocean, _foam, () => StartLevel(_level.Id));
            CreateButton(_screen.transform, T("map"), new Vector2(.70f, .055f), new Vector2(270, 70), _ocean, _foam, ShowMap);
            RefreshHud();
        }

        private void CreateBoard()
        {
            _engine = new BoardEngine(LevelCatalog.BoardSize, LevelCatalog.BoardSize);
            for (int y = 0; y < LevelCatalog.BoardSize; y++)
            for (int x = 0; x < LevelCatalog.BoardSize; x++)
            {
                do _board[x, y] = NewTile();
                while (CreatesStartingMatch(x, y));
            }
            var available = new List<Vector2Int>();
            for (int y = 0; y < LevelCatalog.BoardSize; y++)
            for (int x = 0; x < LevelCatalog.BoardSize; x++) available.Add(new Vector2Int(x, y));
            for (int i = 0; i < _level.BlockerCount; i++)
            {
                int pick = UnityEngine.Random.Range(0, available.Count);
                var point = available[pick];
                available.RemoveAt(pick);
                _blockers[point.x, point.y] = new BlockerState(_level.Blocker);
            }
            EnsurePlayableBoard();
        }

        private TileState NewTile()
        {
            return new TileState((TileKind)UnityEngine.Random.Range(0, 6));
        }

        private bool CreatesStartingMatch(int x, int y)
        {
            if (x >= 2 && _board[x - 1, y].Kind == _board[x - 2, y].Kind && _board[x, y].Kind == _board[x - 1, y].Kind) return true;
            return y >= 2 && _board[x, y - 1].Kind == _board[x, y - 2].Kind && _board[x, y].Kind == _board[x, y - 1].Kind;
        }

        private void SelectCell(int x, int y)
        {
            if (_resolving) return;
            var point = new Vector2Int(x, y);
            if (!_selected.HasValue)
            {
                _selected = point;
                RefreshBoard();
                return;
            }
            if (_selected.Value == point)
            {
                _selected = null;
                RefreshBoard();
                return;
            }
            if (!Match3Rules.IsAdjacent(_selected.Value.x, _selected.Value.y, x, y))
            {
                _selected = point;
                RefreshBoard();
                return;
            }
            Vector2Int first = _selected.Value;
            _selected = null;
            StartCoroutine(TrySwap(first, point));
        }

        private IEnumerator TrySwap(Vector2Int first, Vector2Int second)
        {
            _resolving = true;
            Swap(first, second);
            bool usesSpecial = _board[first.x, first.y].Special != SpecialKind.None || _board[second.x, second.y].Special != SpecialKind.None;
            var matches = Match3Rules.FindMatches(_board);
            if (matches.Count == 0 && !usesSpecial)
            {
                Swap(first, second);
                _resolving = false;
                RefreshBoard();
                yield break;
            }
            _moves--;
            TelemetryService.SwapMade();
            AudioService.Instance?.PlaySwap();
            HapticsService.Tap();
            RefreshBoard();
            yield return Resolve(matches, new[] { first, second });
            _resolving = false;
        }

        private IEnumerator Resolve(HashSet<int> initial, Vector2Int[] swapped)
        {
            int chain = 0;
            HashSet<int> matches = initial;
            while (matches.Count > 0 || (chain == 0 && swapped.Any(p => _board[p.x, p.y].Special != SpecialKind.None)))
            {
                chain++;
                SpecialKind made = PickCreatedSpecial(matches, swapped);
                int keepIndex = FindKeepIndex(matches, swapped);
                if (made != SpecialKind.None && keepIndex >= 0) matches.Remove(keepIndex);
                foreach (Vector2Int point in swapped)
                    if (_board[point.x, point.y].Special != SpecialKind.None) matches.Add(Index(point.x, point.y));
                ExpandSpecials(matches);
                if (matches.Any(index => _board[index % LevelCatalog.BoardSize, index / LevelCatalog.BoardSize]?.Special != SpecialKind.None))
                    TelemetryService.SpecialTriggered();
                if (made != SpecialKind.None && keepIndex >= 0)
                    _board[keepIndex % LevelCatalog.BoardSize, keepIndex / LevelCatalog.BoardSize].Special = made;

                RemoveMatches(matches, chain);
                CollapseBoard();
                RefreshBoard();
                RefreshHud();
                yield return new WaitForSeconds(.20f);
                if (IsWin()) { ShowResult(true); yield break; }
                if (_moves <= 0) { ShowResult(false); yield break; }
                matches = Match3Rules.FindMatches(_board);
                swapped = Array.Empty<Vector2Int>();
            }
            if (!Match3Rules.HasAvailableMove(_board))
            {
                EnsurePlayableBoard();
                ShowToast(T("reshuffle"));
            }
            RefreshBoard();
        }

        private SpecialKind PickCreatedSpecial(HashSet<int> matches, Vector2Int[] swapped)
        {
            if (matches.Count < 4) return SpecialKind.None;
            int longest = 0;
            foreach (Vector2Int p in swapped) longest = Mathf.Max(longest, RunLengthAt(p.x, p.y, 1, 0), RunLengthAt(p.x, p.y, 0, 1));
            if (longest >= 5) return SpecialKind.Pearl;
            if (longest >= 4) return SpecialKind.Beam;
            return matches.Count >= 5 ? SpecialKind.Bomb : SpecialKind.None;
        }

        private int RunLengthAt(int x, int y, int dx, int dy)
        {
            TileKind kind = _board[x, y].Kind;
            int count = 1;
            for (int d = 1; InBounds(x + d * dx, y + d * dy) && _board[x + d * dx, y + d * dy].Kind == kind; d++) count++;
            for (int d = 1; InBounds(x - d * dx, y - d * dy) && _board[x - d * dx, y - d * dy].Kind == kind; d++) count++;
            return count;
        }

        private int FindKeepIndex(HashSet<int> matches, Vector2Int[] swapped)
        {
            foreach (Vector2Int p in swapped) if (matches.Contains(Index(p.x, p.y))) return Index(p.x, p.y);
            return matches.Count == 0 ? -1 : matches.First();
        }

        private void ExpandSpecials(HashSet<int> matches)
        {
            var queue = new Queue<int>(matches);
            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                int x = index % LevelCatalog.BoardSize;
                int y = index / LevelCatalog.BoardSize;
                TileState tile = _board[x, y];
                if (tile == null || tile.Special == SpecialKind.None) continue;
                foreach (int target in SpecialTargets(x, y, tile))
                    if (matches.Add(target)) queue.Enqueue(target);
            }
        }

        private IEnumerable<int> SpecialTargets(int x, int y, TileState tile)
        {
            if (tile.Special == SpecialKind.Beam)
            {
                for (int i = 0; i < LevelCatalog.BoardSize; i++) yield return Index(i, y);
                yield break;
            }
            if (tile.Special == SpecialKind.Bomb)
            {
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                    if (InBounds(x + dx, y + dy)) yield return Index(x + dx, y + dy);
                yield break;
            }
            if (tile.Special == SpecialKind.Pearl)
            {
                for (int yy = 0; yy < LevelCatalog.BoardSize; yy++)
                for (int xx = 0; xx < LevelCatalog.BoardSize; xx++)
                    if (_board[xx, yy].Kind == tile.Kind) yield return Index(xx, yy);
            }
        }

        private void RemoveMatches(HashSet<int> matches, int chain)
        {
            bool triggeredBySpecial = matches.Any(index =>
            {
                TileState candidate = _board[index % LevelCatalog.BoardSize, index / LevelCatalog.BoardSize];
                return candidate != null && candidate.Special != SpecialKind.None;
            });
            foreach (int index in matches)
            {
                int x = index % LevelCatalog.BoardSize;
                int y = index / LevelCatalog.BoardSize;
                TileState tile = _board[x, y];
                if (tile == null) continue;
                if (BlockerRules.Damage(_blockers[x, y], triggeredBySpecial)) _blockersLeft = Mathf.Max(0, _blockersLeft - 1);
                if (tile.Kind == _level.CollectKind) _collected++;
                _board[x, y] = null;
            }
            _score += matches.Count * 100 * chain;
            AudioService.Instance?.PlayMatch();
        }

        private void CollapseBoard()
        {
            _engine.Collapse(NewTile);
        }

        private void EnsurePlayableBoard()
        {
            if (_engine.HasAvailableMove()) return;
            if (_engine.TryShuffleToPlayable(new System.Random())) return;

            for (int attempt = 0; attempt < 256; attempt++)
            {
                for (int y = 0; y < LevelCatalog.BoardSize; y++)
                for (int x = 0; x < LevelCatalog.BoardSize; x++)
                {
                    do _board[x, y] = NewTile(); while (CreatesStartingMatch(x, y));
                }
                if (_engine.HasAvailableMove()) return;
            }
            throw new InvalidOperationException("Unable to generate a playable match-3 board.");
        }

        private void Swap(Vector2Int first, Vector2Int second)
        {
            _engine.Swap(first.x, first.y, second.x, second.y);
            RefreshBoard();
        }

        private void RefreshBoard()
        {
            if (_tileImages == null) return;
            for (int y = 0; y < LevelCatalog.BoardSize; y++)
            for (int x = 0; x < LevelCatalog.BoardSize; x++)
            {
                TileState tile = _board[x, y];
                _tileImages[x, y].color = _tileColors[(int)tile.Kind];
                _cellButtons[x, y].image.color = _selected.HasValue && _selected.Value.x == x && _selected.Value.y == y ? _gold : new Color(.05f, .23f, .30f);
                string glyph = TileGlyph(tile.Kind);
                string special = tile.Special == SpecialKind.Beam ? "B" : tile.Special == SpecialKind.Bomb ? "O" : tile.Special == SpecialKind.Pearl ? "P" : "";
                string blocker = BlockerMarker(_blockers[x, y]);
                string state = string.IsNullOrEmpty(blocker) ? special : blocker;
                _markers[x, y].text = string.IsNullOrEmpty(state) ? glyph : $"{glyph}\n{state}";
                _markers[x, y].color = string.IsNullOrEmpty(state) ? _navy : Color.white;
            }
        }

        private void RefreshHud()
        {
            if (_movesText == null) return;
            _movesText.text = T("moves", _moves);
            _scoreText.text = T("score", _score);
            string blockerGoal = _level.Blocker == BlockerKind.None ? "" : T("clear_goal", _blockersLeft);
            _goalText.text = T("goal", TileName(_level.CollectKind), _collected, _level.CollectTarget, blockerGoal);
        }

        private static string BlockerMarker(BlockerState blocker)
        {
            if (blocker == null || blocker.Kind == BlockerKind.None) return "";
            if (blocker.Kind == BlockerKind.Ice) return "I";
            if (blocker.Kind == BlockerKind.Crate) return blocker.HitsRemaining > 1 ? $"C{blocker.HitsRemaining}" : "C";
            return blocker.Kind == BlockerKind.Seaweed ? "W" : "";
        }

        private bool IsWin()
        {
            return _collected >= _level.CollectTarget && _blockersLeft <= 0;
        }

        private void ShowResult(bool won)
        {
            _resolving = true;
            if (won)
            {
                int stars = 1 + (_score >= _level.StarTwoScore ? 1 : 0) + (_score >= _level.StarThreeScore ? 1 : 0);
                int coins = 15 + _level.Id * 2;
                SaveService.CompleteLevel(_level.Id, stars, coins);
                TelemetryService.LevelWon();
                AudioService.Instance?.PlayWin();
                HapticsService.Tap();
                ShowStory(T("win", stars, coins), ShowMap);
            }
            else
            {
                SaveService.LoseLife();
                TelemetryService.LevelLost();
                AudioService.Instance?.PlayLose();
                ShowStory(T("lose"), () => StartLevel(_level.Id));
            }
        }

        private void ShowStory(string message, Action next)
        {
            var panel = CreateImage(_canvas.transform, "Modal", new Vector2(.5f, .5f), new Vector2(920, 500), new Color(.04f, .16f, .23f, .98f));
            panel.transform.SetAsLastSibling();
            CreateText(panel.transform, T("modal_title"), 34, _gold, new Vector2(.5f, .70f), new Vector2(760, 55), TextAnchor.MiddleCenter);
            CreateText(panel.transform, message, 29, _foam, new Vector2(.5f, .47f), new Vector2(740, 180), TextAnchor.MiddleCenter);
            CreateButton(panel.transform, T("continue"), new Vector2(.5f, .18f), new Vector2(310, 76), _gold, _navy, () => { Destroy(panel.gameObject); next?.Invoke(); });
        }

        private void ShowToast(string message)
        {
            CreateText(_screen.transform, message, 25, _gold, new Vector2(.5f, .77f), new Vector2(700, 42), TextAnchor.MiddleCenter);
        }

        private void ClearScreen()
        {
            if (_screen != null) Destroy(_screen);
            _screen = null;
            _tileImages = null;
            _cellButtons = null;
            _markers = null;
        }

        private GameObject FullScreen(string name, Color color)
        {
            var panel = CreateImage(_canvas.transform, name, new Vector2(.5f, .5f), new Vector2(1080, 1920), color);
            panel.rectTransform.anchorMin = Vector2.zero;
            panel.rectTransform.anchorMax = Vector2.one;
            panel.rectTransform.offsetMin = Vector2.zero;
            panel.rectTransform.offsetMax = Vector2.zero;
            var backdrop = new GameObject("Ocean Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(OceanBackdrop));
            backdrop.transform.SetParent(panel.transform, false);
            var backdropRect = backdrop.GetComponent<RectTransform>();
            backdropRect.anchorMin = Vector2.zero;
            backdropRect.anchorMax = Vector2.one;
            backdropRect.offsetMin = Vector2.zero;
            backdropRect.offsetMax = Vector2.zero;
            backdrop.transform.SetAsFirstSibling();
            return panel.gameObject;
        }

        private Image CreateImage(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private Text CreateText(Transform parent, string value, int fontSize, Color color, Vector2 anchor, Vector2 size, TextAnchor alignment)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private Button CreateButton(Transform parent, string label, Vector2 anchor, Vector2 size, Color color, Color textColor, Action action)
        {
            Image image = CreateImage(parent, "Button", anchor, size, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());
            CreateText(image.transform, label, 25, textColor, new Vector2(.5f, .5f), size - new Vector2(16, 12), TextAnchor.MiddleCenter);
            return button;
        }

        private void ApplyArtwork(Image image, string resourcePath)
        {
            if (!_artSprites.TryGetValue(resourcePath, out Sprite sprite))
            {
                Texture2D texture = Resources.Load<Texture2D>(resourcePath);
                if (texture == null) return;
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                _artSprites[resourcePath] = sprite;
            }
            image.sprite = sprite;
            image.color = Color.white;
            image.preserveAspect = true;
        }

        private int Index(int x, int y) => x + y * LevelCatalog.BoardSize;
        private bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < LevelCatalog.BoardSize && y < LevelCatalog.BoardSize;
        private static string T(string key, params object[] arguments) => LocalizationService.Get(key, arguments);

        private static string TileName(TileKind kind) => T(kind switch
        {
            TileKind.Coral => "coral",
            TileKind.Shell => "shell",
            TileKind.Drop => "drop",
            TileKind.Sunstone => "sunstone",
            TileKind.Starfish => "starfish",
            _ => "crystal"
        });

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
