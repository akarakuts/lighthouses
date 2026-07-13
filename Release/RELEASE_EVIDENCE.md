# Release Evidence - 2026-07-13

## Target store

RuStore (primary handoff). Google Play materials remain in `GOOGLE_PLAY_LISTING.md`.

## Build metadata

| Field | Value |
| --- | --- |
| Package | `com.karakuts.lighthouses` |
| Version name | `1.0.2` |
| Version code | `3` |
| Min SDK | `26` |
| Target SDK (manifest) | `36` |
| ABI | `arm64-v8a` |
| Product name | `Lighthouses: Mystery of the Archipelago` |

## Production artifacts

| Artifact | Path | Status |
| --- | --- | --- |
| Production AAB | `Release/Lighthouses-1.0.2.aab` | Rebuild required after version bump |
| AAB SHA-256 | `85a88a7b7aa4e112a37bc634d6cc3e550a7dd54e663e5316a5aef1f1552f0dfa` | Verified |
| RuStore PEPK ZIP | `Release/signing/rustore-pepk_out.zip` | **Ready** (не коммитить) |
| Upload certificate PEM | `Release/signing/upload-certificate.pem` | **Ready** |
| Keystore | `~/secrets/lighthouses-rustore/lighthouses-rustore.keystore` | Локально |
| Credentials | `~/secrets/lighthouses-rustore/credentials.env` | Локально, chmod 600 |

## RuStore upload order

1. RuStore Console → **Подпись приложения**
2. Загрузить `Release/signing/rustore-pepk_out.zip`
3. Загрузить `Release/signing/upload-certificate.pem`
4. Нажать **Отправить подпись**
5. Загрузить `Release/Lighthouses-1.0.1.aab`

## Automated checks

- Core rules smoke: passed on 2026-07-13.
- Production AAB manifest: package `com.karakuts.lighthouses`, version `1.0.1` / code `2`.
- AAB signed with upload key (`alias=upload`).

## Remaining manual gates

1. Загрузить подпись и AAB в RuStore Console.
2. Опубликовать privacy policy на HTTPS.
3. Заполнить карточку по `RUSTORE_LISTING.md`.
4. Пройти `QA_TEST_PLAN.md` на физическом устройстве.
5. Отправить на модерацию.

## Commands

```bash
# Recreate RuStore signature package (new encryption key from console)
export RUSTORE_ENCRYPTION_KEY='<hex from RuStore Console>'
Tools/release/create_rustore_signature.sh

# Rebuild production AAB
set -a && source ~/secrets/lighthouses-rustore/credentials.env && set +a
export LIGHTHOUSES_VERSION_NAME=1.0.2
export LIGHTHOUSES_VERSION_CODE=3
Tools/release/build_production_aab.sh
```
