#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
ADB="${ADB:-$HOME/Library/Android/sdk/platform-tools/adb}"
EMULATOR="${EMULATOR:-$HOME/Library/Android/sdk/emulator/emulator}"
PACKAGE="com.karakuts.lighthouses"
AAB="${AAB:-$ROOT/Release/Lighthouses-validation.aab}"
BUNDLETOOL_JAR="${BUNDLETOOL_JAR:-$ROOT/Tools/bin/bundletool-all-1.18.3.jar}"
OUT_DIR="$ROOT/Release/StoreAssets/screenshots"
RAW_DIR="$ROOT/Release/StoreAssets/screenshots/.raw"

mkdir -p "$OUT_DIR" "$RAW_DIR"

wait_for_device() {
  for _ in $(seq 1 60); do
    if "$ADB" shell getprop sys.boot_completed 2>/dev/null | tr -d '\r' | grep -q 1; then
      return 0
    fi
    sleep 2
  done
  return 1
}

ensure_emulator() {
  if "$ADB" devices | awk 'NR>1 && $2=="device"{print $1}' | grep -q .; then
    return 0
  fi
  "$EMULATOR" -avd Medium_Phone -no-snapshot-load -gpu swiftshader_indirect >/dev/null 2>&1 &
  wait_for_device
}

install_app() {
  if [[ ! -f "$AAB" ]]; then
    echo "AAB not found: $AAB" >&2
    exit 1
  fi
  if [[ ! -f "$BUNDLETOOL_JAR" ]]; then
    echo "bundletool jar not found: $BUNDLETOOL_JAR" >&2
    exit 1
  fi
  local apks="$RAW_DIR/app.apks"
  java -jar "$BUNDLETOOL_JAR" build-apks --bundle="$AAB" --output="$apks" --mode=universal --overwrite >/dev/null
  java -jar "$BUNDLETOOL_JAR" install-apks --apks="$apks" --adb="$ADB" >/dev/null
}

hide_system_ui() {
  "$ADB" shell settings put global policy_control 'immersive.full=*'
  "$ADB" shell settings put secure immersive_mode_confirmations confirmed
}

dismiss_tutorial() {
  # Story modal "Continue" button on 1080x2400 (empirically ~1280-1400).
  for y in 1280 1320 1360 1400; do
    tap 540 "$y"
    sleep 0.5
  done
  sleep 1
}

capture_raw() {
  local name="$1"
  sleep 3
  local out="$RAW_DIR/$name.png"
  for _ in $(seq 1 8); do
    "$ADB" exec-out screencap -p > "$out"
    local bytes
    bytes=$(stat -f%z "$out" 2>/dev/null || echo 0)
    if (( bytes > 100000 )); then
      return 0
    fi
    sleep 2
  done
  echo "Warning: $name capture looks empty (${bytes:-0} bytes)" >&2
}

tap() {
  "$ADB" shell input tap "$1" "$2"
}

launch_app() {
  "$ADB" shell am force-stop "$PACKAGE"
  "$ADB" shell monkey -p "$PACKAGE" 1 >/dev/null
  sleep 15
  dismiss_tutorial
  sleep 2
}

process_screenshot() {
  local src="$1"
  local dst="$2"
  local mode="${3:-center}"
  local status_bar=48
  local crop_h=1920
  local crop_w=1080
  local width height
  width=$(sips -g pixelWidth "$src" 2>/dev/null | awk '/pixelWidth/ {print $2}')
  height=$(sips -g pixelHeight "$src" 2>/dev/null | awk '/pixelHeight/ {print $2}')
  local usable_h=$((height - status_bar))
  local y_offset=$status_bar
  case "$mode" in
    top) y_offset=$status_bar ;;
    bottom) y_offset=$((height - crop_h)) ;;
    center) y_offset=$((status_bar + (usable_h - crop_h) / 2)) ;;
  esac
  if (( y_offset < 0 )); then y_offset=0; fi
  ffmpeg -y -loglevel error -i "$src" -vf "crop=${crop_w}:${crop_h}:0:${y_offset},scale=1080:1920" "$dst"
  echo "  $dst ($(($(stat -f%z "$dst") / 1024)) KB, 1080x1920, ${mode})"
}

echo "Starting emulator and installing app..."
ensure_emulator
install_app
hide_system_ui
"$ADB" shell pm clear "$PACKAGE"
launch_app

# Coordinates for 1080x2400 device (normalized Unity anchors converted).
capture_raw "01-map"
tap 160 980      # Level 1 PLAY
sleep 4
capture_raw "02-gameplay"
tap 97 168       # Back to map
sleep 3
capture_raw "03-map-return"
tap 140 206      # Rules
sleep 2
capture_raw "04-rules"
tap 540 2260     # Back to map (rules)
sleep 2
tap 940 206      # Settings
sleep 2
capture_raw "05-settings"
tap 540 2173     # Back to map (settings)
sleep 2
tap 778 620      # Restore lighthouse
sleep 3
capture_raw "06-lighthouse"
tap 540 2173     # Back to map
sleep 1

echo "Processing screenshots..."
process_screenshot "$RAW_DIR/01-map.png" "$OUT_DIR/rustore-01-map.png" top
process_screenshot "$RAW_DIR/02-gameplay.png" "$OUT_DIR/rustore-02-gameplay.png" top
process_screenshot "$RAW_DIR/04-rules.png" "$OUT_DIR/rustore-03-rules.png" bottom
process_screenshot "$RAW_DIR/05-settings.png" "$OUT_DIR/rustore-04-settings.png" bottom
process_screenshot "$RAW_DIR/06-lighthouse.png" "$OUT_DIR/rustore-05-lighthouse.png" bottom

rm -rf "$RAW_DIR"
echo "Done. Screenshots in $OUT_DIR"
"$ROOT/Tools/release/sync_rustore_handoff.sh"
