# Release Checklist

## Product QA

- [ ] Run the full EditMode and PlayMode suites; keep their XML reports with the release evidence.
- [ ] Run `Tools/qa/run_android_smoke.sh` against the validation AAB on an Android emulator or device.
- [ ] Play levels 1, 5, 10, 15 and 20 on a physical Android device using `QA_TEST_PLAN.md`.
- [ ] Verify victory, defeat, restart, map return, daily reward, life recovery and settings persistence.
- [ ] Verify portrait layout on a 16:9 and a tall 20:9 device.
- [ ] Verify the app resumes correctly after backgrounding and after an interrupted phone call.
- [ ] Review the lighthouse illustration and replace it only if approved artwork supersedes it; keep gameplay assets readable at 1080x1920.

## Android build

- [ ] Install Unity 6000.3 LTS with Android Build Support, SDK, NDK and OpenJDK.
- [ ] Open the project once so `Assets/Editor/ProjectSetup.cs` applies its settings.
- [ ] Create a private upload keystore outside this repository. Set `LIGHTHOUSES_KEYSTORE_PATH`, `LIGHTHOUSES_KEYSTORE_PASSWORD`, `LIGHTHOUSES_KEY_ALIAS` and `LIGHTHOUSES_KEY_PASSWORD` only in the build environment.
- [ ] Build an Android App Bundle, not an APK. Use `BuildCommandLine.BuildValidationAndroid` for a debug-signed local validation bundle.
- [ ] For a Google Play upload, set `LIGHTHOUSES_VERSION_NAME`, a new positive `LIGHTHOUSES_VERSION_CODE`, and all four `LIGHTHOUSES_KEYSTORE_*` variables; then use `BuildCommandLine.BuildProductionAndroid`.
- [ ] Install the bundle through internal testing and inspect Android vitals for crashes and ANRs.

## Google Play Console

- [ ] Replace the example privacy contact, host the policy over HTTPS and submit its public URL.
- [ ] Upload the feature graphic from `Release/StoreAssets/feature-graphic-1024x500.png`, the app icon from `Release/StoreAssets/app-icon-512x512.png`, and real-device screenshots listed in `GOOGLE_PLAY_LISTING.md`.
- [ ] Complete Data safety based on the offline-only version 1.0 declaration.
- [ ] Complete content rating, target audience and app access declarations truthfully.
- [ ] Use internal testing, then closed testing, before production rollout.
- [ ] Add `UNITY_LICENSE` to GitHub Actions secrets and confirm both Unity Quality workflow jobs pass.
