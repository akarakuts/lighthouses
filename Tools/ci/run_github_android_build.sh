#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
LOG_FILE="${LOG_FILE:-$ROOT/Release/unity-ci-build.log}"
VERSION_NAME="${LIGHTHOUSES_VERSION_NAME:?LIGHTHOUSES_VERSION_NAME is required}"
VERSION_CODE="${LIGHTHOUSES_VERSION_CODE:?LIGHTHOUSES_VERSION_CODE is required}"

find_unity() {
  if [[ -n "${UNITY:-}" && -x "$UNITY" ]]; then
    echo "$UNITY"
    return
  fi
  local candidate
  for candidate in \
    /Applications/Unity/Hub/Editor/6000.3.19f1/Unity.app/Contents/MacOS/Unity \
    /Applications/Unity/Hub/Editor/6000.5.3f1/Unity.app/Contents/MacOS/Unity \
    /Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity; do
    if [[ -x "$candidate" ]]; then
      echo "$candidate"
      return
    fi
  done
  echo "Unity Editor not found. Install Unity 6000.3+ with Android Build Support." >&2
  exit 1
}

if pgrep -f "Unity.*$ROOT" >/dev/null 2>&1; then
  echo "Close the Unity Editor for this project before running a CI build." >&2
  exit 1
fi

validate_upload_keystore() {
  command -v jarsigner >/dev/null || return 1
  local tmpdir tmpjar
  tmpdir="$(mktemp -d)"
  tmpjar="$tmpdir/test.jar"
  echo test > "$tmpdir/test.txt"
  (cd "$tmpdir" && jar cf test.jar test.txt)
  jarsigner \
    -keystore "$LIGHTHOUSES_KEYSTORE_PATH" \
    -storepass "${LIGHTHOUSES_KEYSTORE_PASSWORD:-}" \
    -keypass "${LIGHTHOUSES_KEY_PASSWORD:-}" \
    "$tmpjar" "${LIGHTHOUSES_KEY_ALIAS:-}" >/dev/null 2>&1
}

CREDENTIALS_FILE="${LIGHTHOUSES_CREDENTIALS_FILE:-$HOME/secrets/lighthouses-rustore/credentials.env}"
if [[ -f "$CREDENTIALS_FILE" ]]; then
  set -a
  # shellcheck disable=SC1090
  source "$CREDENTIALS_FILE"
  set +a
fi

if [[ "${LIGHTHOUSES_CI_FORCE_DEBUG_SIGN:-0}" == "1" ]]; then
  unset LIGHTHOUSES_KEYSTORE_PATH LIGHTHOUSES_KEYSTORE_PASSWORD LIGHTHOUSES_KEY_ALIAS LIGHTHOUSES_KEY_PASSWORD
elif [[ -n "${LIGHTHOUSES_KEYSTORE_PATH:-}" && -f "$LIGHTHOUSES_KEYSTORE_PATH" ]]; then
  if ! validate_upload_keystore; then
    echo "Upload keystore credentials are invalid; falling back to debug signing for CI." >&2
    unset LIGHTHOUSES_KEYSTORE_PATH LIGHTHOUSES_KEYSTORE_PASSWORD LIGHTHOUSES_KEY_ALIAS LIGHTHOUSES_KEY_PASSWORD
  fi
fi

UNITY_BIN="$(find_unity)"
mkdir -p "$ROOT/Release"

echo "Building Lighthouses $VERSION_NAME ($VERSION_CODE) with $UNITY_BIN"
"$UNITY_BIN" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod LighthouseMatch3.Editor.BuildCommandLine.BuildCiReleaseAndroid \
  -logFile "$LOG_FILE"

OUTPUT_AAB="$ROOT/Release/Lighthouses-${VERSION_NAME}.aab"
if [[ ! -f "$OUTPUT_AAB" ]]; then
  echo "Expected AAB not found: $OUTPUT_AAB" >&2
  tail -n 80 "$LOG_FILE" >&2 || true
  exit 1
fi

"$ROOT/Tools/release/verify_aab.sh" "$OUTPUT_AAB"
echo "CI Android build ready: $OUTPUT_AAB"
