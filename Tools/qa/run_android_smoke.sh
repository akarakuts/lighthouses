#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
AAB_PATH="${1:-$ROOT/Release/Lighthouses-validation.aab}"
BUNDLETOOL_JAR="${BUNDLETOOL_JAR:-$ROOT/Tools/bin/bundletool-all-1.18.3.jar}"
ANDROID_SDK_ROOT="${ANDROID_SDK_ROOT:-$HOME/Library/Android/sdk}"
ADB="${ADB:-$ANDROID_SDK_ROOT/platform-tools/adb}"
PACKAGE_NAME="com.archipelagostudio.lighthouses"
OUTPUT_DIR="$ROOT/Release/qa-$(date +%Y%m%d-%H%M%S)"

if [[ ! -f "$AAB_PATH" ]]; then
  echo "AAB not found: $AAB_PATH" >&2
  exit 1
fi
if [[ -z "$BUNDLETOOL_JAR" || ! -f "$BUNDLETOOL_JAR" ]]; then
  echo "Set BUNDLETOOL_JAR to the bundletool .jar before running this smoke test." >&2
  exit 1
fi
[[ -x "$ADB" ]] || { echo "adb is required at $ADB." >&2; exit 1; }
command -v java >/dev/null || { echo "java is required." >&2; exit 1; }

mkdir -p "$OUTPUT_DIR"
APKS_PATH="$OUTPUT_DIR/app.apks"
java -jar "$BUNDLETOOL_JAR" build-apks --bundle="$AAB_PATH" --output="$APKS_PATH" --mode=universal --overwrite
java -jar "$BUNDLETOOL_JAR" install-apks --apks="$APKS_PATH" --adb="$ADB"
"$ADB" shell am force-stop "$PACKAGE_NAME"
"$ADB" shell settings put secure immersive_mode_confirmations confirmed || true
"$ADB" shell monkey -p "$PACKAGE_NAME" 1 >/dev/null
sleep 3

if ! "$ADB" shell pidof "$PACKAGE_NAME" >/dev/null; then
  echo "Application did not stay running after launch." >&2
  exit 1
fi

"$ADB" exec-out screencap -p > "$OUTPUT_DIR/first-screen.png"
"$ADB" shell input keyevent KEYCODE_HOME
sleep 1
"$ADB" shell monkey -p "$PACKAGE_NAME" 1 >/dev/null
sleep 2
if ! "$ADB" shell pidof "$PACKAGE_NAME" >/dev/null; then
  echo "Application failed to resume after backgrounding." >&2
  exit 1
fi

echo "Android smoke test passed. Evidence: $OUTPUT_DIR/first-screen.png"
