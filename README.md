# Lighthouses: Mystery of the Archipelago

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

Russian / Русский: [README.ru.md](README.ru.md)

Repository: **https://github.com/akarakuts/lighthouses**

**Lighthouses: Mystery of the Archipelago** — offline match-3 for Android (Unity 6000.3 LTS, C#, runtime-built UI). Swap adjacent tiles, clear collection goals, unlock lighthouse upgrades across 20 levels. No ads, analytics, or account SDK in v1.0.

## Features

- **Match-3 core** — 8×8 board, six tile types, cascades, score, dead-board shuffle.
- **Boosters** — beam (4-in-a-row), bomb (5+), color pearl (5-in-a-row); specials chain on activation.
- **Blockers** — ice, crate (two hits), seaweed (needs a special).
- **Progression** — level map (20 procedural levels), stars, coins, timed lives, daily rewards, lighthouse upgrade stages, local save.
- **Meta** — short story beats after upgrades; English and Russian UI.
- **Offline** — sound effects, optional vibration, privacy screen; progress stored on device via PlayerPrefs.

The project builds all gameplay UI at runtime (colored tiles and labels) so it is playable without external art. Replace runtime visuals with sprites and polish audio in a production pass.

## Stack

| Area | Choice |
|------|--------|
| Engine | Unity 6000.3 LTS, 2D, IL2CPP |
| Language | C# |
| UI | Unity UI (runtime-generated Canvas) |
| Save | PlayerPrefs + `JsonUtility` |
| Tests | Unity Test Framework (EditMode + PlayMode), NUnit |
| CI | GitHub Actions + [Game CI](https://game.ci/) |

**Android package:** `com.archipelagostudio.lighthouses`

## Requirements

- **Unity 6000.3 LTS** (or newer LTS) with **Android Build Support**
- **JDK 11+** and **Android SDK** for device builds
- Optional: **.NET SDK** for `Tools/CoreRulesCheck` (headless rule smoke tests without Unity)

## Open and run

```bash
git clone https://github.com/akarakuts/lighthouses.git
cd lighthouses
```

1. In Unity Hub, **Add project from disk** and select the cloned folder.
2. Open `Assets/Scenes/Main.unity` and press **Play**.
3. For Android: switch platform to **Android** in Build Settings; startup scene `Main` is configured on import.

## Controls

Tap one tile, then an adjacent tile to swap. A valid swap must create a match (or use a special tile). Four matched tiles create a **beam**, five create a **pearl**; matching a booster activates it.

## CI & automation

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| [Unity Quality](.github/workflows/unity-quality.yml) | push / PR to `master` or `main` | EditMode and PlayMode tests via Game CI |

Configure repository secret **`UNITY_LICENSE`** (Unity activation file) for CI. Without it, Game CI jobs fail until the license is added.

## Testing

**EditMode (headless, from repo root with Unity in PATH):**

```bash
/path/to/Unity -batchmode -nographics -projectPath . \
  -executeMethod LighthouseMatch3.Editor.BatchTestRunner.RunEditModeTests
```

Writes `Release/editmode-tests.xml` and exits non-zero on failure.

**PlayMode:**

```bash
/path/to/Unity -batchmode -nographics -projectPath . \
  -runTests -testPlatform PlayMode -testResults Release/playmode-tests.xml
```

Do **not** pass `-quit` for PlayMode — Unity must finish the PlayMode domain reload.

**Core rules without Unity:**

```bash
dotnet run --project Tools/CoreRulesCheck
```

| Suite | Location | Coverage |
|-------|----------|----------|
| Match-3 rules | `Assets/Tests/EditMode/Match3RulesTests.cs` | Matches, adjacency, shuffle, blockers, collapse |
| Save / lives | `Assets/Tests/EditMode/SaveServiceTests.cs` | Recovery, daily reward, duplicate rewards |
| Localization | `Assets/Tests/EditMode/LocalizationServiceTests.cs` | EN / RU strings |
| Bootstrap | `Assets/Tests/PlayMode/GameBootstrapPlayModeTests.cs` | Game root starts interactively |

## Android release builds

Editor menu methods (also callable with `-executeMethod`):

| Method | Output | Signing |
|--------|--------|---------|
| `LighthouseMatch3.Editor.BuildCommandLine.BuildValidationAndroid` | `Release/Lighthouses-validation.aab` | Debug keystore |
| `LighthouseMatch3.Editor.BuildCommandLine.BuildProductionAndroid` | `Release/Lighthouses-<version>.aab` | Upload keystore (required) |

Production builds require environment variables:

| Variable | Purpose |
|----------|---------|
| `LIGHTHOUSES_VERSION_NAME` | User-visible version (e.g. `1.0.0`) |
| `LIGHTHOUSES_VERSION_CODE` | Positive integer version code |
| `LIGHTHOUSES_KEYSTORE_PATH` | Path to upload keystore |
| `LIGHTHOUSES_KEYSTORE_PASSWORD` | Keystore password |
| `LIGHTHOUSES_KEY_ALIAS` | Key alias |
| `LIGHTHOUSES_KEY_PASSWORD` | Key password |

Store listing copy, privacy policy draft, QA checklist, and store graphics live under [`Release/`](Release/). Device smoke test: [`Tools/qa/run_android_smoke.sh`](Tools/qa/run_android_smoke.sh).

## Project layout

| Path | Role |
|------|------|
| `Assets/Scripts/Match3Rules.cs`, `BoardEngine.cs`, `BoardShuffler.cs` | Pure match-3 logic |
| `Assets/Scripts/Match3GameController.cs` | Game loop, UI, level flow |
| `Assets/Scripts/SaveService.cs`, `LocalizationService.cs` | Progress and i18n |
| `Assets/Editor/BuildCommandLine.cs`, `BatchTestRunner.cs` | CLI build and tests |
| `Tools/CoreRulesCheck/` | Standalone rule smoke tests |
| `Release/` | Play handoff docs and store assets (binaries gitignored) |

## Contact

**Aleksey Karakuts** — [aleksey@karakuts.com](mailto:aleksey@karakuts.com)

## License

This program is free software: you can redistribute it and/or modify it under the terms of the **GNU General Public License** as published by the Free Software Foundation, either **version 3** of the License, or (at your option) any later version.

See the [`LICENSE`](LICENSE) file for the full GPLv3 text.

Copyright (C) 2026 Aleksey Karakuts &lt;aleksey@karakuts.com&gt;
