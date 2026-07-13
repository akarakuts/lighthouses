#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
SECRETS_DIR="${SECRETS_DIR:-$HOME/secrets/lighthouses-rustore}"
KEYSTORE="${LIGHTHOUSES_KEYSTORE_PATH:-$SECRETS_DIR/lighthouses-rustore.keystore}"
CREDS_FILE="${CREDS_FILE:-$SECRETS_DIR/credentials.env}"
OUTPUT_DIR="${1:-$ROOT/Release/signing}"
ENCRYPTION_KEY="${RUSTORE_ENCRYPTION_KEY:?Set RUSTORE_ENCRYPTION_KEY from RuStore Console}"
PEPK_JAR="${PEPK_JAR:-$SECRETS_DIR/pepk.jar}"
SIGN_ALIAS="${SIGN_ALIAS:-sign}"
UPLOAD_ALIAS="${UPLOAD_ALIAS:-upload}"

mkdir -p "$SECRETS_DIR" "$OUTPUT_DIR"

if [[ ! -f "$CREDS_FILE" ]]; then
  STORE_PASS="$(openssl rand -base64 24)"
  cat > "$CREDS_FILE" <<EOF
LIGHTHOUSES_KEYSTORE_PATH=$KEYSTORE
LIGHTHOUSES_KEYSTORE_PASSWORD=$STORE_PASS
LIGHTHOUSES_KEY_ALIAS=$UPLOAD_ALIAS
LIGHTHOUSES_KEY_PASSWORD=$STORE_PASS
SIGN_ALIAS=$SIGN_ALIAS
EOF
  chmod 600 "$CREDS_FILE"

  keytool -genkeypair -v \
    -keystore "$KEYSTORE" \
    -alias "$SIGN_ALIAS" \
    -keyalg RSA -keysize 2048 -validity 36500 \
    -storepass "$STORE_PASS" \
    -dname "CN=Aleksey Karakuts, OU=Mobile, O=Karakuts, L=Moscow, ST=Moscow, C=RU"

  keytool -genkeypair -v \
    -keystore "$KEYSTORE" \
    -alias "$UPLOAD_ALIAS" \
    -keyalg RSA -keysize 2048 -validity 36500 \
    -storepass "$STORE_PASS" \
    -dname "CN=Aleksey Karakuts, OU=Mobile, O=Karakuts, L=Moscow, ST=Moscow, C=RU"

  echo "Created keystore and credentials: $CREDS_FILE"
fi

# shellcheck disable=SC1090
source "$CREDS_FILE"

if [[ ! -f "$PEPK_JAR" ]]; then
  curl -fsSL -o "$PEPK_JAR" "https://www.gstatic.com/play-apps-publisher-rapid/signing-tool/prod/pepk.jar"
fi

PEPK_OUT="$SECRETS_DIR/pepk_out.zip"
UPLOAD_CERT="$OUTPUT_DIR/upload-certificate.pem"

java -jar "$PEPK_JAR" \
  --keystore="$LIGHTHOUSES_KEYSTORE_PATH" \
  --alias="$SIGN_ALIAS" \
  --output="$PEPK_OUT" \
  --keystore-pass="$LIGHTHOUSES_KEYSTORE_PASSWORD" \
  --encryptionkey="$ENCRYPTION_KEY" \
  --include-cert

keytool -exportcert \
  -alias "$LIGHTHOUSES_KEY_ALIAS" \
  -keystore "$LIGHTHOUSES_KEYSTORE_PATH" \
  -storepass "$LIGHTHOUSES_KEYSTORE_PASSWORD" \
  -rfc \
  -file "$UPLOAD_CERT"

cp "$PEPK_OUT" "$OUTPUT_DIR/rustore-pepk_out.zip"

echo "RuStore signature package ready:"
echo "  ZIP (app signing key): $OUTPUT_DIR/rustore-pepk_out.zip"
echo "  PEM (upload certificate): $UPLOAD_CERT"
echo "  Keystore: $LIGHTHOUSES_KEYSTORE_PATH"
echo "  Credentials: $CREDS_FILE"
echo
echo "Upload both files to RuStore Console -> App signature, then build:"
echo "  source $CREDS_FILE"
echo "  export LIGHTHOUSES_VERSION_NAME=1.0.1 LIGHTHOUSES_VERSION_CODE=2"
echo "  $ROOT/Tools/release/build_production_aab.sh"
