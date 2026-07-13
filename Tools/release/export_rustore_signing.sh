#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
OUTPUT_DIR="${1:-$ROOT/Release/signing}"

: "${LIGHTHOUSES_KEYSTORE_PATH:?Set LIGHTHOUSES_KEYSTORE_PATH}"
: "${LIGHTHOUSES_KEYSTORE_PASSWORD:?Set LIGHTHOUSES_KEYSTORE_PASSWORD}"
: "${LIGHTHOUSES_KEY_ALIAS:?Set LIGHTHOUSES_KEY_ALIAS}"

mkdir -p "$OUTPUT_DIR"

UPLOAD_CERT="$OUTPUT_DIR/upload-certificate.pem"
APP_CERT="$OUTPUT_DIR/app-certificate.pem"

keytool -exportcert \
  -alias "$LIGHTHOUSES_KEY_ALIAS" \
  -keystore "$LIGHTHOUSES_KEYSTORE_PATH" \
  -storepass "$LIGHTHOUSES_KEYSTORE_PASSWORD" \
  -rfc \
  -file "$UPLOAD_CERT"

keytool -list -v \
  -alias "$LIGHTHOUSES_KEY_ALIAS" \
  -keystore "$LIGHTHOUSES_KEYSTORE_PATH" \
  -storepass "$LIGHTHOUSES_KEYSTORE_PASSWORD" \
  > "$OUTPUT_DIR/keystore-fingerprints.txt"

cp "$UPLOAD_CERT" "$APP_CERT"

echo "RuStore signing export complete:"
echo "  Upload certificate (PEM): $UPLOAD_CERT"
echo "  Fingerprints: $OUTPUT_DIR/keystore-fingerprints.txt"
echo
echo "RuStore AAB flow:"
echo "  1. In RuStore Console open App signature before uploading the AAB."
echo "  2. Upload $UPLOAD_CERT as the upload-key certificate."
echo "  3. Create or confirm the app signing key in RuStore."
echo "  4. Upload the production AAB signed with the same upload keystore."
