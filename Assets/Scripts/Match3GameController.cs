using System;
using System.Collections;
using System.Collections.Generic;
using LighthouseMatch3.UI;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3
{
    public sealed class Match3GameController : MonoBehaviour
    {
        private readonly GamePalette _palette = new GamePalette();
        private readonly RuntimeUiFactory _ui = new RuntimeUiFactory();
        private readonly LevelBoardView _boardView;
        private static IGameRandom _random = new UnityGameRandom();

        private Canvas _canvas;
        private GameObject _screen;
        private Match3LevelSession _session;
        private Vector2Int? _selected;
        private bool _resolving;

        public Match3GameController() => _boardView = new LevelBoardView(_ui, _palette);

        public int DebugScore => _session?.Score ?? 0;
        public int DebugMoves => _session?.Moves ?? 0;
        public bool DebugIsResolving => _resolving;

        public static void ConfigureRandomForTests(IGameRandom random) => _random = random;
        public static void ResetRandomForTests() => _random = new UnityGameRandom();

        private void Start()
        {
            CreateCanvas();
            ShowMap();
            if (!SaveService.Progress.HasSeenTutorial)
                ShowStory(T("tutorial"), () => { SaveService.Progress.HasSeenTutorial = true; SaveService.Save(); });
        }

        public void DebugStartLevel(int id) => StartLevel(id);
        public void DebugSelectCell(int x, int y) => SelectCell(x, y);
        public void DebugInstallBoard(TileState[,] tiles)
        {
            if (_session == null) StartLevel(1);
            _session.InstallBoard(tiles);
            RefreshView();
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

        private void ShowMap() { ClearScreen(); _screen = MapScreenView.Build(_canvas.transform, _ui, _palette, Translate, StartLevel, ShowLighthouse, ShowSettings, ClaimDailyReward); }
        private void ShowLighthouse() { ClearScreen(); _screen = LighthouseScreenView.Build(_canvas.transform, _ui, _palette, Translate, ShowMap, UpgradeLighthouse); }
        private void ShowSettings() { ClearScreen(); _screen = SettingsScreenView.Build(_canvas.transform, _ui, _palette, Translate, ShowMap, ToggleSound, ToggleHaptics, ToggleLanguage, () => ShowStory(T("privacy_copy"), ShowSettings)); }

        private void UpgradeLighthouse(int cost)
        {
            if (SaveService.Progress.Stars < cost || SaveService.Progress.LighthouseStage >= 4) return;
            SaveService.Progress.Stars -= cost;
            SaveService.Progress.LighthouseStage++;
            SaveService.Save();
            ShowStory(T("lighthouse_upgraded", SaveService.Progress.LighthouseStage), ShowLighthouse);
        }

        private void ClaimDailyReward()
        {
            int reward = SaveService.ClaimDailyReward();
            if (reward > 0) ShowStory(T("reward", reward), ShowMap);
        }

        private void ToggleSound() { SaveService.Progress.SoundEnabled = !SaveService.Progress.SoundEnabled; SaveService.Save(); ShowSettings(); }
        private void ToggleHaptics() { SaveService.Progress.HapticsEnabled = !SaveService.Progress.HapticsEnabled; SaveService.Save(); ShowSettings(); }
        private void ToggleLanguage() { LocalizationService.ToggleLanguage(); ShowSettings(); }

        private void StartLevel(int id)
        {
            if (SaveService.Progress.Lives <= 0) { ShowStory(T("no_lives", LocalizationService.LifeRecoveryLabel()), ShowMap); return; }
            _session = new Match3LevelSession(LevelCatalog.Get(id), _random);
            TelemetryService.LevelStarted();
            _selected = null;
            _resolving = false;
            ClearScreen();
            _screen = _boardView.Build(_canvas.transform, _session.Level, Translate, ShowMap, () => StartLevel(id), SelectCell);
            RefreshView();
        }

        private void SelectCell(int x, int y)
        {
            if (_resolving || _session == null) return;
            var point = new Vector2Int(x, y);
            if (!_selected.HasValue) { _selected = point; RefreshView(); return; }
            if (_selected.Value == point) { _selected = null; RefreshView(); return; }
            if (!Match3Rules.IsAdjacent(_selected.Value.x, _selected.Value.y, x, y)) { _selected = point; RefreshView(); return; }
            Vector2Int first = _selected.Value;
            _selected = null;
            StartCoroutine(TrySwap(first, point));
        }

        private IEnumerator TrySwap(Vector2Int first, Vector2Int second)
        {
            _resolving = true;
            if (!_session.SwapProducesMatch(first.x, first.y, second.x, second.y))
            {
                _resolving = false;
                yield break;
            }
            _session.Engine.Swap(first.x, first.y, second.x, second.y);
            yield return UiAnimation.ScalePulse(_boardView.TileImages[first.x, first.y].transform, 1.18f, 0.12f);
            yield return UiAnimation.ScalePulse(_boardView.TileImages[second.x, second.y].transform, 1.18f, 0.12f);
            RefreshView();
            _session.Moves--;
            TelemetryService.SwapMade();
            AudioService.Instance?.PlaySwap();
            HapticsService.Tap();
            yield return Resolve(Match3Rules.FindMatches(_session.Tiles), new[] { first, second });
            _resolving = false;
        }

        private IEnumerator Resolve(HashSet<int> initial, Vector2Int[] swapped)
        {
            int chain = 0;
            HashSet<int> matches = initial;
            BoardPoint[] swappedPoints = ToBoardPoints(swapped);
            int boardSize = LevelCatalog.BoardSize;

            while (MatchResolver.ShouldContinueResolution(matches, _session.Tiles, boardSize, swappedPoints, chain))
            {
                chain++;
                MatchResolver.PrepareResolutionStep(_session.Tiles, boardSize, matches, swappedPoints, out _, out _);
                if (SpecialTileRules.HasSpecialInMatches(_session.Tiles, boardSize, matches)) TelemetryService.SpecialTriggered();
                yield return UiAnimation.PulseMatchedTiles(_boardView.TileImages, matches, boardSize, 0.12f);
                _session.ClearMatches(matches, chain);
                AudioService.Instance?.PlayMatch();
                _session.CollapseBoard();
                RefreshView();
                yield return new WaitForSeconds(.20f);
                if (_session.IsWin) { ShowResult(true); yield break; }
                if (_session.Moves <= 0) { ShowResult(false); yield break; }
                matches = Match3Rules.FindMatches(_session.Tiles);
                swappedPoints = Array.Empty<BoardPoint>();
            }
            if (!Match3Rules.HasAvailableMove(_session.Tiles))
            {
                _session.EnsurePlayable();
                _boardView.ShowToast(_screen.transform, T("reshuffle"));
            }
            RefreshView();
        }

        private void ShowResult(bool won)
        {
            _resolving = true;
            if (won)
            {
                SaveService.CompleteLevel(_session.Level.Id, _session.CalculateStars(), _session.CalculateCoins());
                TelemetryService.LevelWon();
                AudioService.Instance?.PlayWin();
                HapticsService.Tap();
                ShowStory(T("win", _session.CalculateStars(), _session.CalculateCoins()), ShowMap);
            }
            else
            {
                SaveService.LoseLife();
                TelemetryService.LevelLost();
                AudioService.Instance?.PlayLose();
                ShowStory(T("lose"), () => StartLevel(_session.Level.Id));
            }
        }

        private void ShowStory(string message, Action next) =>
            StoryModalView.Show(_canvas.transform, _ui, _palette, Translate, message, next);

        private void ClearScreen()
        {
            _boardView.ReleaseCells();
            if (_screen != null) Destroy(_screen);
            _screen = null;
        }

        private void RefreshView()
        {
            if (_session == null) return;
            _boardView.Refresh(_session.Tiles, _session.Blockers, _selected, _session.Level, _session.Moves, _session.Score, _session.Collected, _session.BlockersLeft, Translate);
        }

        private static BoardPoint[] ToBoardPoints(Vector2Int[] swapped)
        {
            var points = new BoardPoint[swapped.Length];
            for (int i = 0; i < swapped.Length; i++) points[i] = new BoardPoint(swapped[i].x, swapped[i].y);
            return points;
        }

        private static string Translate(string key, object[] args) =>
            args == null || args.Length == 0 ? LocalizationService.Get(key) : LocalizationService.Get(key, args);

        private static string T(string key, params object[] arguments) => LocalizationService.Get(key, arguments);
    }
}
