#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
RUN_BUILD="${RUN_BUILD:-1}"
RUN_SMOKE="${RUN_SMOKE:-0}"

echo "== Lighthouses RuStore release prep =="
echo "Project: $ROOT"
echo

echo "[1/5] Core rules smoke"
dotnet run --project "$ROOT/Tools/CoreRulesCheck/CoreRulesCheck.csproj"
echo

if [[ "$RUN_BUILD" == "1" ]]; then
  echo "[2/5] Validation AAB build"
  "$ROOT/Tools/release/build_validation_aab.sh"
else
  echo "[2/5] Validation AAB build skipped (RUN_BUILD=0)"
fi
echo

if [[ "$RUN_SMOKE" == "1" ]]; then
  echo "[3/5] Android smoke test"
  "$ROOT/Tools/qa/run_android_smoke.sh" "$ROOT/Release/Lighthouses-validation.aab"
else
  echo "[3/5] Android smoke test skipped (set RUN_SMOKE=1 with adb + emulator)"
fi
echo

if [[ -n "${LIGHTHOUSES_KEYSTORE_PATH:-}" ]]; then
  echo "[4/5] Export RuStore signing certificates"
  "$ROOT/Tools/release/export_rustore_signing.sh" "$ROOT/Release/signing"
else
  echo "[4/5] Signing export skipped (set LIGHTHOUSES_KEYSTORE_* to export PEM certs)"
fi
echo

echo "[5/5] Store handoff"
cat <<EOF
Ready-to-upload materials:
  AAB (validation): Release/Lighthouses-validation.aab
  AAB (production): Release/Lighthouses-<version>.aab via Tools/release/build_production_aab.sh
  Icon:             Release/StoreAssets/app-icon-512x512.png
  Screenshots:      Release/StoreAssets/screenshots/
  Listing copy:     Release/RUSTORE_LISTING.md
  Checklist:        Release/RUSTORE_CHECKLIST.md
  Privacy policy:   Release/PRIVACY_POLICY.md

Production build example:
  export LIGHTHOUSES_VERSION_NAME=1.0.1
  export LIGHTHOUSES_VERSION_CODE=2
  export LIGHTHOUSES_KEYSTORE_PATH=/path/to/upload.jks
  export LIGHTHOUSES_KEYSTORE_PASSWORD=...
  export LIGHTHOUSES_KEY_ALIAS=upload
  export LIGHTHOUSES_KEY_PASSWORD=...
  Tools/release/build_production_aab.sh
EOF
