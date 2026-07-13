#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
DEST="${RUSTORE_HANDOFF_DIR:-/Users/a.karakuts/Bars/rustore/lighthouses}"
RELEASE="$ROOT/Release"

mkdir -p "$DEST"/{aab,signing,store-assets/screenshots,docs}

copy_if_exists() {
  local src="$1"
  local dst="$2"
  if [[ -f "$src" ]]; then
    cp -f "$src" "$dst"
    echo "  $dst"
  else
    echo "  skip (missing): $src" >&2
  fi
}

pick_production_aab() {
  local candidate=""
  for f in "$RELEASE"/Lighthouses-[0-9]*.[0-9]*.[0-9]*.aab; do
    [[ -f "$f" ]] || continue
    candidate="$f"
  done
  if [[ -n "$candidate" ]]; then
    echo "$candidate"
    return
  fi
  if [[ -f "$RELEASE/Lighthouses-1.0.1.aab" ]]; then
    echo "$RELEASE/Lighthouses-1.0.1.aab"
  fi
}

echo "Sync RuStore handoff -> $DEST"

PROD_AAB="$(pick_production_aab || true)"
if [[ -n "${PROD_AAB:-}" ]]; then
  copy_if_exists "$PROD_AAB" "$DEST/aab/$(basename "$PROD_AAB")"
fi
copy_if_exists "$RELEASE/Lighthouses-validation.aab" "$DEST/aab/Lighthouses-validation.aab"

copy_if_exists "$RELEASE/signing/rustore-pepk_out.zip" "$DEST/signing/rustore-pepk_out.zip"
copy_if_exists "$RELEASE/signing/upload-certificate.pem" "$DEST/signing/upload-certificate.pem"

copy_if_exists "$RELEASE/StoreAssets/app-icon-512x512.png" "$DEST/store-assets/app-icon-512x512.png"
copy_if_exists "$RELEASE/StoreAssets/feature-graphic-1024x500.png" "$DEST/store-assets/feature-graphic-1024x500.png"
for shot in "$RELEASE"/StoreAssets/screenshots/rustore-*.png; do
  [[ -f "$shot" ]] || continue
  copy_if_exists "$shot" "$DEST/store-assets/screenshots/$(basename "$shot")"
done

for doc in RUSTORE_LISTING.md RUSTORE_CHECKLIST.md PRIVACY_POLICY.md RELEASE_EVIDENCE.md; do
  copy_if_exists "$RELEASE/$doc" "$DEST/docs/$doc"
done

{
  echo "# RuStore handoff manifest"
  echo "generated: $(date -u +"%Y-%m-%dT%H:%M:%SZ")"
  echo "source: $ROOT"
  echo
  find "$DEST" -type f ! -name MANIFEST.md | sort | while read -r f; do
    rel="${f#"$DEST"/}"
    size=$(stat -f%z "$f" 2>/dev/null || stat -c%s "$f")
    hash=$(shasum -a 256 "$f" | awk '{print $1}')
    printf '%s\t%s bytes\tsha256:%s\n' "$rel" "$size" "$hash"
  done
} > "$DEST/MANIFEST.md"

echo "Done. Handoff folder: $DEST"
