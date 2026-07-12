# Lighthouses: Mystery of the Archipelago

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

English: [README.md](README.md)

Репозиторий: **https://github.com/akarakuts/lighthouses**

**Lighthouses: Mystery of the Archipelago** («Маяки: тайна архипелага») — офлайн match-3 для Android на **Unity 6000.3 LTS** и **C#** с UI, собираемым в рантайме. Меняйте соседние фишки, выполняйте цели уровней, восстанавливайте маяк на карте из 20 уровней. В версии 1.0 нет рекламы, аналитики и аккаунтов.

## Возможности

- **Match-3** — поле 8×8, шесть типов фишек, каскады, счёт, перемешивание «мёртвой» доски.
- **Бустеры** — луч (4 в ряд), бомба (5+), жемчужина цвета (5 в ряд); активация по совпадению.
- **Препятствия** — лёд, ящик (два удара), водоросли (только спецэффектом).
- **Прогресс** — карта уровней (20 процедурных конфигураций), звёзды, монеты, жизни с таймером, ежедневная награда, стадии маяка, локальное сохранение.
- **Атмосфера** — короткие сюжетные реплики после улучшений; интерфейс на **русском и английском**.
- **Офлайн** — звуки, опциональная вибрация, экран конфиденциальности; прогресс только на устройстве (PlayerPrefs).

Игровой UI создаётся кодом (цветные плитки и подписи), поэтому проект сразу играбелен без внешней графики. Для релиза замените заглушки на спрайты и доработайте звук.

## Требования и запуск

Как в [README.md](README.md): Unity 6000.3 LTS с Android Build Support, JDK 11+, Android SDK.

```bash
git clone https://github.com/akarakuts/lighthouses.git
cd lighthouses
```

1. Unity Hub → **Добавить проект** → выбрать папку репозитория.
2. Открыть `Assets/Scenes/Main.unity` → **Play**.
3. Для Android — платформа **Android** в Build Settings; стартовая сцена `Main` настраивается при импорте.

**Пакет Android:** `com.archipelagostudio.lighthouses`

## Управление

Нажмите одну фишку, затем соседнюю — обмен. Допустим только ход, дающий совпадение (или задействующий спецфишку). Четыре подряд — **луч**, пять — **жемчужина**; совпадение с бустером активирует его.

## CI (GitHub Actions)

Как в англ. README: workflow [Unity Quality](.github/workflows/unity-quality.yml) — EditMode и PlayMode через Game CI при push/PR в `master` или `main`. Нужен секрет **`UNITY_LICENSE`**.

## Тесты

- **EditMode** — `-executeMethod LighthouseMatch3.Editor.BatchTestRunner.RunEditModeTests` → `Release/editmode-tests.xml`.
- **PlayMode** — `-runTests -testPlatform PlayMode` **без** `-quit`.
- **Без Unity** — `dotnet run --project Tools/CoreRulesCheck`.

Подробнее — раздел Testing в [README.md](README.md#testing).

## Сборка Android

| Метод | Результат | Подпись |
|-------|-----------|---------|
| `BuildValidationAndroid` | `Release/Lighthouses-validation.aab` | debug |
| `BuildProductionAndroid` | `Release/Lighthouses-<version>.aab` | upload keystore |

Переменные окружения для production — в [README.md](README.md#android-release-builds). Материалы для магазина — каталог [`Release/`](Release/).

## Контакты

**Aleksey Karakuts** — [aleksey@karakuts.com](mailto:aleksey@karakuts.com)

## Лицензия

Программа распространяется на условиях **GNU GPLv3** — полный текст в файле [`LICENSE`](LICENSE).

Copyright (C) 2026 Aleksey Karakuts &lt;aleksey@karakuts.com&gt;
