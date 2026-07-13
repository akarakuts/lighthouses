using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace LighthouseMatch3
{
    public static class LocalizationService
    {
        private static Func<SystemLanguage> _systemLanguageProvider = () => Application.systemLanguage;

        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            ["title"] = "LIGHTHOUSES",
            ["subtitle"] = "Mystery of the Archipelago",
            ["resources"] = "Stars {0}    Coins {1}    Lives {2}{3}",
            ["daily_reward"] = "Daily reward",
            ["daily_claimed"] = "Daily reward claimed",
            ["restore"] = "Restore lighthouse",
            ["settings"] = "Settings",
            ["rules"] = "Rules",
            ["clear"] = "CLEAR",
            ["play"] = "PLAY",
            ["locked"] = "LOCKED",
            ["lighthouse"] = "THE LIGHTHOUSE",
            ["back_to_map"] = "Back to map",
            ["sound_on"] = "Sound: on",
            ["sound_off"] = "Sound: off",
            ["haptics_on"] = "Vibration: on",
            ["haptics_off"] = "Vibration: off",
            ["language"] = "Language: English",
            ["privacy"] = "Privacy",
            ["offline_storage"] = "The offline version stores only game progress on this device.",
            ["level"] = "LEVEL {0}",
            ["score"] = "Score {0}",
            ["moves"] = "Moves: {0}",
            ["goal"] = "Collect {0}: {1}/{2}{3}",
            ["clear_goal"] = "  Clear: {0}",
            ["tap_swap"] = "Tap two adjacent tiles to swap",
            ["restart"] = "Restart",
            ["map"] = "Map",
            ["continue"] = "Continue",
            ["modal_title"] = "ARCHIPELAGO",
            ["privacy_copy"] = "This version is offline. It does not collect, transmit, sell, or share personal information. Progress and settings remain on this device.",
            ["no_lives"] = "No lives left. The next life arrives in {0}.",
            ["reshuffle"] = "The tide reshuffles the board",
            ["tutorial"] = "Make rows of three. Four create a beam, five create a pearl. Clear every goal before the moves run out.",
            ["rules_title"] = "HOW TO PLAY",
            ["rules_intro"] = "Restore the archipelago by clearing the goals on each island.",
            ["rules_swap_title"] = "SWAP TILES",
            ["rules_swap"] = "Swap two neighboring tiles to make a line of three or more.",
            ["rules_specials_title"] = "CREATE SPECIALS",
            ["rules_beam_title"] = "4 IN A ROW",
            ["rules_beam"] = "A beam clears its whole row.",
            ["rules_bomb_title"] = "T OR L SHAPE",
            ["rules_bomb"] = "A bomb clears a 3 x 3 area.",
            ["rules_pearl_title"] = "5 IN A ROW",
            ["rules_pearl"] = "A pearl clears every tile of its color.",
            ["rules_blockers_title"] = "CLEAR BLOCKERS",
            ["rules_blockers"] = "Ice breaks with a match. Crates need two hits. Seaweed only clears with a special.",
            ["rules_goal_title"] = "WIN THE LEVEL",
            ["rules_goal"] = "Collect the required tiles and clear blockers before the moves run out.",
            ["win"] = "Island restored! {0} stars and {1} coins earned.",
            ["lose"] = "The tide was too strong. Try the level again.",
            ["reward"] = "Daily reward: {0} coins. Come back tomorrow to extend your streak.",
            ["lives_full"] = "Full",
            ["life_recovery"] = "{0:00}:{1:00}",
            ["lighthouse_stage_0"] = "A dark tower",
            ["lighthouse_stage_1"] = "The keeper's house",
            ["lighthouse_stage_2"] = "The repaired pier",
            ["lighthouse_stage_3"] = "The observatory",
            ["lighthouse_stage_4"] = "Beacon of the archipelago",
            ["lighthouse_complete"] = "The islands shine again.",
            ["lighthouse_progress"] = "Bring stars from the levels to rebuild the lighthouse.",
            ["lighthouse_upgrade"] = "Upgrade for {0} stars",
            ["lighthouse_upgraded"] = "The lighthouse reaches stage {0}. A new island answers its light.",
            ["coral"] = "coral", ["shell"] = "shells", ["drop"] = "drops", ["sunstone"] = "sunstones", ["starfish"] = "starfish", ["crystal"] = "crystals"
        };

        private static readonly Dictionary<string, string> Russian = new Dictionary<string, string>
        {
            ["title"] = "МАЯКИ",
            ["subtitle"] = "Тайна архипелага",
            ["resources"] = "Звёзды {0}    Монеты {1}    Жизни {2}{3}",
            ["daily_reward"] = "Ежедневная награда",
            ["daily_claimed"] = "Награда получена",
            ["restore"] = "Восстановить маяк",
            ["settings"] = "Настройки",
            ["rules"] = "Правила",
            ["clear"] = "ПРОЙДЕНО",
            ["play"] = "ИГРАТЬ",
            ["locked"] = "ЗАКРЫТО",
            ["lighthouse"] = "МАЯК",
            ["back_to_map"] = "К карте",
            ["sound_on"] = "Звук: вкл.",
            ["sound_off"] = "Звук: выкл.",
            ["haptics_on"] = "Вибрация: вкл.",
            ["haptics_off"] = "Вибрация: выкл.",
            ["language"] = "Язык: Русский",
            ["privacy"] = "Конфиденциальность",
            ["offline_storage"] = "Офлайн-версия хранит прогресс только на устройстве.",
            ["level"] = "УРОВЕНЬ {0}",
            ["score"] = "Счёт {0}",
            ["moves"] = "Ходы: {0}",
            ["goal"] = "Собрать {0}: {1}/{2}{3}",
            ["clear_goal"] = "  Очистить: {0}",
            ["tap_swap"] = "Нажмите две соседние фишки для обмена",
            ["restart"] = "Заново",
            ["map"] = "Карта",
            ["continue"] = "Продолжить",
            ["modal_title"] = "АРХИПЕЛАГ",
            ["privacy_copy"] = "Эта версия работает офлайн. Она не собирает, не передаёт, не продаёт и не передаёт третьим лицам персональные данные. Прогресс и настройки остаются на устройстве.",
            ["no_lives"] = "Жизни закончились. Следующая появится через {0}.",
            ["reshuffle"] = "Прилив перемешивает поле",
            ["tutorial"] = "Собирайте ряды по три. Четыре создают луч, пять - жемчужину. Выполните цели до окончания ходов.",
            ["rules_title"] = "КАК ИГРАТЬ",
            ["rules_intro"] = "Восстанавливайте архипелаг, выполняя цели на каждом острове.",
            ["rules_swap_title"] = "МЕНЯЙТЕ ФИШКИ",
            ["rules_swap"] = "Меняйте местами две соседние фишки, чтобы собрать ряд из трёх или больше.",
            ["rules_specials_title"] = "СОЗДАВАЙТЕ БОНУСЫ",
            ["rules_beam_title"] = "4 В РЯД",
            ["rules_beam"] = "Луч очищает весь ряд.",
            ["rules_bomb_title"] = "ФИГУРА Т ИЛИ Г",
            ["rules_bomb"] = "Бомба очищает область 3 x 3.",
            ["rules_pearl_title"] = "5 В РЯД",
            ["rules_pearl"] = "Жемчужина очищает все фишки своего цвета.",
            ["rules_blockers_title"] = "УБИРАЙТЕ ПРЕПЯТСТВИЯ",
            ["rules_blockers"] = "Лёд разбивается совпадением. Ящик нужно ударить дважды. Водоросли убираются только бонусом.",
            ["rules_goal_title"] = "ПОБЕДА НА УРОВНЕ",
            ["rules_goal"] = "Соберите нужные фишки и очистите препятствия до окончания ходов.",
            ["win"] = "Остров восстановлен! Получено: звёзды {0}, монеты {1}.",
            ["lose"] = "Прилив оказался сильнее. Попробуйте уровень снова.",
            ["reward"] = "Ежедневная награда: {0} монет. Возвращайтесь завтра, чтобы продолжить серию.",
            ["lives_full"] = "Полные",
            ["life_recovery"] = "{0:00}:{1:00}",
            ["lighthouse_stage_0"] = "Тёмная башня",
            ["lighthouse_stage_1"] = "Дом смотрителя",
            ["lighthouse_stage_2"] = "Восстановленный пирс",
            ["lighthouse_stage_3"] = "Обсерватория",
            ["lighthouse_stage_4"] = "Маяк архипелага",
            ["lighthouse_complete"] = "Острова снова сияют.",
            ["lighthouse_progress"] = "Принесите звёзды с уровней, чтобы восстановить маяк.",
            ["lighthouse_upgrade"] = "Улучшить за {0} звёзд",
            ["lighthouse_upgraded"] = "Маяк достиг стадии {0}. Новый остров откликается на его свет.",
            ["coral"] = "коралл", ["shell"] = "ракушки", ["drop"] = "капли", ["sunstone"] = "солнечные камни", ["starfish"] = "морские звёзды", ["crystal"] = "кристаллы"
        };

        public static bool IsRussian
        {
            get
            {
                string language = SaveService.Progress?.LanguageCode;
                return ResolveLanguageCode(language) == "ru";
            }
        }

        public static string Get(string key, params object[] arguments)
        {
            Dictionary<string, string> table = IsRussian ? Russian : English;
            if (!table.TryGetValue(key, out string value))
                value = English.TryGetValue(key, out string fallback) ? fallback : key;
            return arguments.Length == 0 ? value : string.Format(CultureInfo.InvariantCulture, value, arguments);
        }

        public static string LifeRecoveryLabel()
        {
            if (!SaveService.TryGetLifeRecoveryRemaining(out int minutes, out int seconds))
                return Get("lives_full");
            return Get("life_recovery", minutes, seconds);
        }

        public static void ToggleLanguage()
        {
            SaveService.Progress.LanguageCode = IsRussian ? "en" : "ru";
            SaveService.Save();
        }

        public static void ConfigureSystemLanguageProviderForTests(Func<SystemLanguage> provider)
        {
            _systemLanguageProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public static void ResetSystemLanguageProviderForTests()
        {
            _systemLanguageProvider = () => Application.systemLanguage;
        }

        private static string ResolveLanguageCode(string savedLanguage)
        {
            if (!string.IsNullOrWhiteSpace(savedLanguage))
                return savedLanguage == "ru" ? "ru" : "en";

            return _systemLanguageProvider() == SystemLanguage.Russian ? "ru" : "en";
        }
    }
}
