#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
UNITY_VERSION="${UNITY_VERSION:-6000.3.19f1}"
UNITY="${UNITY:-/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity}"
LOG_FILE="${LOG_FILE:-$ROOT/Release/unity-build-production.log}"

required_vars=(LIGHTHOUSES_VERSION_NAME LIGHTHOUSES_VERSION_CODE LIGHTHOUSES_KEYSTORE_PATH LIGHTHOUSES_KEYSTORE_PASSWORD LIGHTHOUSES_KEY_ALIAS LIGHTHOUSES_KEY_PASSWORD)
for var in "${required_vars[@]}"; do
  if [[ -z "${!var:-}" ]]; then
    echo "Missing required environment variable: $var" >&2
    exit 1
  fi
done

if [[ ! -f "$LIGHTHOUSES_KEYSTORE_PATH" ]]; then
  echo "Keystore not found: $LIGHTHOUSES_KEYSTORE_PATH" >&2
  exit 1
fi

if [[ ! -x "$UNITY" ]]; then
  echo "Unity not found at $UNITY. Set UNITY or UNITY_VERSION." >&2
  exit 1
fi

if pgrep -f "Unity.*$ROOT" >/dev/null 2>&1; then
  echo "Close the Unity Editor for this project before running a batch build." >&2
  exit 1
fi

mkdir -p "$ROOT/Release"
"$UNITY" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod LighthouseMatch3.Editor.BuildCommandLine.BuildProductionAndroid \
  -logFile "$LOG_FILE"

OUTPUT_AAB="$ROOT/Release/Lighthouses-${LIGHTHOUSES_VERSION_NAME}.aab"
"$ROOT/Tools/release/verify_aab.sh" "$OUTPUT_AAB"
echo "Production AAB ready: $OUTPUT_AAB"
