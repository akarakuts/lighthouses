# Lighthouses: Mystery of the Archipelago

Playable Android-oriented match-3 MVP built with Unity 2D and C#.

## Open and run

1. Install Unity 6000.3 LTS (or newer LTS) with Android Build Support.
2. In Unity Hub, add this folder as an existing project.
3. Open `Assets/Scenes/Main.unity` and press Play.
4. For Android, switch the platform to Android in Build Settings and build an AAB. The project configures `Main` as its startup scene on import.

The project creates all game UI at runtime. It intentionally has no external art dependency: the board, level map, upgrade screen, dialog and boosters are immediately playable. Replace the runtime colors with sprites and add audio in a production art pass.

## Controls

Tap one tile, then an adjacent tile to swap them. A valid swap must create a match. Four matched tiles create a beam, five create a pearl; matching a booster activates it.

## Included MVP

- 20 procedurally configured levels with moves and collection goals
- six tile types, cascades, dead-board shuffle and score
- beam, bomb and color-pearl boosters
- ice, crate and seaweed blockers
- map progression, stars, coins, timed lives, daily rewards, lighthouse upgrades and local save data
- short story beats after lighthouse upgrades
- offline sound effects, vibration controls and a local privacy screen

## Release material

The Android release configuration and Google Play handoff are in `Release/`. Run the EditMode suite headlessly with `-executeMethod LighthouseMatch3.Editor.BatchTestRunner.RunEditModeTests`; it writes an XML report under `Release/` and returns a failing process code for a failing test. Run PlayMode with Unity's standard `-runTests -testPlatform PlayMode -testResults Release/playmode-tests.xml` command, without `-quit`, so Unity can complete the PlayMode domain reload. `BuildCommandLine.BuildValidationAndroid` produces a debug-signed validation AAB; `BuildCommandLine.BuildProductionAndroid` requires an upload key and explicit version environment variables. The game has no ad, analytics, payment or account SDK. Do not claim those features until the corresponding provider has been intentionally integrated and its privacy disclosure has been revised.
