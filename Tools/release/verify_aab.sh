#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
AAB_PATH="${1:-$ROOT/Release/Lighthouses-validation.aab}"
BUNDLETOOL_JAR="${BUNDLETOOL_JAR:-$ROOT/Tools/bin/bundletool-all-1.18.3.jar}"
EXPECTED_PACKAGE="${EXPECTED_PACKAGE:-com.karakuts.lighthouses}"

if [[ ! -f "$AAB_PATH" ]]; then
  echo "AAB not found: $AAB_PATH" >&2
  exit 1
fi

echo "AAB: $AAB_PATH"
echo "Size: $(du -h "$AAB_PATH" | awk '{print $1}')"
echo "SHA-256: $(shasum -a 256 "$AAB_PATH" | awk '{print $1}')"

if [[ -f "$BUNDLETOOL_JAR" ]]; then
  MANIFEST="$(java -jar "$BUNDLETOOL_JAR" dump manifest --bundle="$AAB_PATH")"
  echo "$MANIFEST" | head -3
  if ! echo "$MANIFEST" | grep -q "package=\"$EXPECTED_PACKAGE\""; then
    echo "Expected package $EXPECTED_PACKAGE was not found in the AAB manifest." >&2
    exit 1
  fi
else
  echo "Set BUNDLETOOL_JAR to validate the manifest with bundletool." >&2
fi
