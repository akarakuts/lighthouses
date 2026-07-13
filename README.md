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
| UI | TextMeshPro + Unity UI (runtime-generated Canvas) |
| Levels | ScriptableObject assets in `Resources/Levels/` with procedural fallback |
| Save | PlayerPrefs + `JsonUtility` |
| Tests | Unity Test Framework (EditMode + PlayMode), NUnit |
| CI | GitHub Actions + [Game CI](https://game.ci/) |

**Android package:** `com.karakuts.lighthouses`

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
| [Unity Quality](.github/workflows/unity-quality.yml) | push / PR to `main` | dotnet smoke + level asset sync |
| [Release Build](.github/workflows/release.yml) | tag `v*` | Android AAB build + GitHub Release |

Configure repository secrets for Unity CI (one-time):

```bash
./Tools/ci/setup-github-secrets.sh
```

Required for **Release Build** and optional Unity tests: **`UNITY_LICENSE`** (`.ulf` from Unity Hub), **`UNITY_EMAIL`**, **`UNITY_PASSWORD`**. Pro/Plus licenses also need **`UNITY_SERIAL`**.

Optional production signing for tag builds:

| Secret | Purpose |
|--------|---------|
| `ANDROID_KEYSTORE_BASE64` | Upload keystore (`base64 -i upload.keystore`) |
| `ANDROID_KEYSTORE_PASSWORD` | Keystore password |
| `ANDROID_KEYALIAS_NAME` | Key alias |
| `ANDROID_KEYALIAS_PASSWORD` | Key password |

Without signing secrets, tag builds produce a debug-signed AAB.

| Job | Requirements |
|-----|--------------|
| Core rules | .NET 10 only |
| Level assets sync | .NET 10 only |
| EditMode / PlayMode | Manual `workflow_dispatch` with `run_unity_tests=true` |
| Release Build | Tag `vX.Y.Z` matching `ProjectSettings.bundleVersion` + Unity secrets |

Create a release:

```bash
git tag v1.0.2
git push origin v1.0.2
```

Without Unity secrets, Game CI test jobs fail; dotnet jobs run without a license.

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
dotnet run --project Tools/CoreRulesCheck/CoreRulesCheck.csproj
```

**Regenerate level ScriptableObject assets:**

```bash
dotnet run --project Tools/GenerateLevelAssets/GenerateLevelAssets.csproj
```

Or in Unity: **Lighthouses → Generate Level Assets**.

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

Store listing copy, privacy policy draft, QA checklist, and store graphics live under [`Release/`](Release/). RuStore handoff: [`Release/RUSTORE_CHECKLIST.md`](Release/RUSTORE_CHECKLIST.md). Device smoke test: [`Tools/qa/run_android_smoke.sh`](Tools/qa/run_android_smoke.sh). Release scripts: [`Tools/release/`](Tools/release/).

## Project layout

| Path | Role |
|------|------|
| `Assets/Scripts/Core/` | Pure match-3 domain (`Match3Rules`, `MatchResolver`, `Match3LevelSession`, `BoardGenerator`) |
| `Assets/Scripts/UI/` | Runtime UI views, `UiAnimation`, `GamePalette` |
| `Assets/Resources/Levels/` | ScriptableObject level definitions (auto-generated via **Lighthouses → Generate Level Assets**) |
| `Assets/Scripts/Match3GameController.cs` | Game orchestration (~200 lines) |
| `Assets/Scripts/SaveService.cs`, `LocalizationService.cs`, `TelemetryService.cs` | Progress, i18n, local telemetry |
| `Assets/Editor/BuildCommandLine.cs`, `BatchTestRunner.cs` | CLI build and tests |
| `Tools/CoreRulesCheck/` | Standalone rule smoke tests (`CoreRulesCheck.csproj`) |
| `Tools/GenerateLevelAssets/` | Regenerates `Assets/Resources/Levels/*.asset` from procedural catalog |
| `Release/` | Play handoff docs and store assets (binaries gitignored) |
| `graphify-out/` | Code knowledge graph (`graphify update .` after changes) |

## Code graph (graphify)

Interactive architecture graph: `graphify-out/graph.html` (build with `graphify update .`).

After clone, install the CLI once: `uv tool install graphifyy`. Enable auto-rebuild on commit: `graphify hook install`.


**Aleksey Karakuts** — [aleksey@karakuts.com](mailto:aleksey@karakuts.com)

## License

This program is free software: you can redistribute it and/or modify it under the terms of the **GNU General Public License** as published by the Free Software Foundation, either **version 3** of the License, or (at your option) any later version.

See the [`LICENSE`](LICENSE) file for the full GPLv3 text.

Copyright (C) 2026 Aleksey Karakuts &lt;aleksey@karakuts.com&gt;
