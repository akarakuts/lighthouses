# RuStore Release Checklist

Чеклист публикации **Lighthouses** в RuStore. Общие QA-пункты — в [`RELEASE_CHECKLIST.md`](RELEASE_CHECKLIST.md).

## 1. Аккаунт и приложение

- [ ] Зарегистрировать разработчика в [RuStore Консоль](https://console.rustore.ru/).
- [ ] Создать приложение (роль «Владелец компании»).
- [ ] Выбрать тип **Универсальное** (телефон; ТВ не требуется).
- [ ] Убедиться, что package ID `com.karakuts.lighthouses` свободен в RuStore.

## 2. Ключ подписи (один раз)

- [ ] Создать upload-keystore **вне репозитория** (если ещё нет):

```bash
keytool -genkeypair -v \
  -keystore ~/secrets/lighthouses-upload.jks \
  -alias upload \
  -keyalg RSA -keysize 2048 -validity 10000
```

- [ ] Сохранить пароли keystore в менеджере секретов.
- [ ] Экспортировать PEM для RuStore:

```bash
export LIGHTHOUSES_KEYSTORE_PATH=~/secrets/lighthouses-upload.jks
export LIGHTHOUSES_KEYSTORE_PASSWORD=...
export LIGHTHOUSES_KEY_ALIAS=upload
Tools/release/export_rustore_signing.sh Release/signing
```

- [ ] В RuStore Console → **Подпись приложения** загрузить `Release/signing/upload-certificate.pem`.
- [ ] Создать или подтвердить ключ подписи приложения в консоли RuStore.

## 3. Сборка релиза

- [ ] Закрыть Unity Editor (batch-сборка не работает при открытом проекте).
- [ ] Прогнать автотесты:

```bash
dotnet run --project Tools/CoreRulesCheck/CoreRulesCheck.csproj
# опционально: Unity EditMode через BatchTestRunner
```

- [ ] Собрать validation AAB и проверить манифест:

```bash
Tools/release/build_validation_aab.sh
```

- [ ] Собрать production AAB:

```bash
export LIGHTHOUSES_VERSION_NAME=1.0.2
export LIGHTHOUSES_VERSION_CODE=3
export LIGHTHOUSES_KEYSTORE_PATH=~/secrets/lighthouses-upload.jks
export LIGHTHOUSES_KEYSTORE_PASSWORD=...
export LIGHTHOUSES_KEY_ALIAS=upload
export LIGHTHOUSES_KEY_PASSWORD=...
Tools/release/build_production_aab.sh
```

- [ ] Убедиться, что в AAB: package `com.karakuts.lighthouses`, version `1.0.2`, code `3`.

## 4. Локальная проверка

- [ ] Smoke-тест на эмуляторе/устройстве:

```bash
export BUNDLETOOL_JAR=Tools/bin/bundletool-all-1.18.3.jar
Tools/qa/run_android_smoke.sh Release/Lighthouses-validation.aab
```

- [ ] Ручной прогон по [`QA_TEST_PLAN.md`](QA_TEST_PLAN.md) на физическом устройстве.

## 5. Карточка в RuStore

- [ ] Заполнить поля из [`RUSTORE_LISTING.md`](RUSTORE_LISTING.md).
- [ ] Загрузить иконку `StoreAssets/app-icon-512x512.png`.
- [ ] Загрузить минимум 3 скриншота из `StoreAssets/screenshots/`.
- [ ] Указать email `aleksey@karakuts.com`.
- [ ] Опубликовать политику конфиденциальности на HTTPS и вставить URL.
- [ ] Заполнить декларацию разрешений: только `VIBRATE`, без сбора данных.
- [ ] Выбрать категорию «Игры», возраст 0+, маркировку игрового контента.

## 6. Загрузка версии

- [ ] **Сначала** подпись → **затем** AAB (`Release/Lighthouses-1.0.2.aab`).
- [ ] Дождаться обработки файла (до 14 дней на отправку в модерацию).
- [ ] Заполнить «Что нового?».
- [ ] Отправить на модерацию.

## 7. После публикации

- [ ] Сохранить SHA-256 production AAB в `RELEASE_EVIDENCE.md`.
- [ ] Проверить установку и обновление с тестового устройства.
- [ ] Для следующего релиза увеличить `LIGHTHOUSES_VERSION_CODE` и не менять keystore.

## Быстрый старт

```bash
RUN_BUILD=1 RUN_SMOKE=0 Tools/release/prepare_rustore_release.sh
```

С production-подписью и smoke-тестом:

```bash
# задать LIGHTHOUSES_KEYSTORE_* и LIGHTHOUSES_VERSION_*
RUN_BUILD=1 RUN_SMOKE=1 Tools/release/prepare_rustore_release.sh
Tools/release/build_production_aab.sh
```
