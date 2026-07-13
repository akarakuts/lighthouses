#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
UNITY_VERSION="${UNITY_VERSION:-6000.3.19f1}"
UNITY="${UNITY:-/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity}"
LOG_FILE="${LOG_FILE:-$ROOT/Release/unity-build-validation.log}"

if [[ ! -x "$UNITY" ]]; then
  echo "Unity not found at $UNITY. Set UNITY or UNITY_VERSION." >&2
  exit 1
fi

if pgrep -f "Unity.*-projectPath.*$(basename "$ROOT")" >/dev/null 2>&1 || \
   pgrep -f "Unity.*$ROOT" >/dev/null 2>&1; then
  echo "Close the Unity Editor for this project before running a batch build." >&2
  exit 1
fi

mkdir -p "$ROOT/Release"
"$UNITY" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod LighthouseMatch3.Editor.BuildCommandLine.BuildValidationAndroid \
  -logFile "$LOG_FILE"

"$ROOT/Tools/release/verify_aab.sh" "$ROOT/Release/Lighthouses-validation.aab"
echo "Validation AAB ready: $ROOT/Release/Lighthouses-validation.aab"
