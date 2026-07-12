# Android QA Test Plan

## Automated smoke test

1. Start a physical Android device or an emulator visible to `adb devices`.
2. Set `BUNDLETOOL_JAR` to a local bundletool JAR.
3. Run `Tools/qa/run_android_smoke.sh Release/Lighthouses-validation.aab`.
4. Review the generated screenshot under `Release/qa-*/`.

The script validates installation from the AAB, cold launch, process survival and foreground resume. It is intentionally safe to run against a debug-signed validation bundle only.

## Manual device matrix

| Scenario | Small 16:9 device | Tall 20:9 device | Physical device |
| --- | --- | --- | --- |
| Map and level layout | Pass/Fail | Pass/Fail | Pass/Fail |
| Win, loss, restart and map return | Pass/Fail | Pass/Fail | Pass/Fail |
| Ice, crate and seaweed objectives | Pass/Fail | Pass/Fail | Pass/Fail |
| Daily reward and life recovery | Pass/Fail | Pass/Fail | Pass/Fail |
| Background, resume and interrupted call | Pass/Fail | Pass/Fail | Pass/Fail |
| Accessibility text size and touch targets | Pass/Fail | Pass/Fail | Pass/Fail |

Record device model, Android version, build SHA-256 and screenshots with every completed matrix.
