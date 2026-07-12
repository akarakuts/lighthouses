using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace LighthouseMatch3
{
    public static class LocalizationService
    {
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            ["title"] = "LIGHTHOUSES",
            ["subtitle"] = "Mystery of the Archipelago",
            ["resources"] = "Stars {0}    Coins {1}    Lives {2}{3}",
            ["daily_reward"] = "Daily reward",
            ["daily_claimed"] = "Daily reward claimed",
            ["restore"] = "Restore lighthouse",
            ["settings"] = "Settings",
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
            ["win"] = "Island restored! {0} stars and {1} coins earned.",
            ["lose"] = "The tide was too strong. Try the level again.",
            ["reward"] = "Daily reward: {0} coins. Come back tomorrow to extend your streak.",
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
            ["win"] = "Остров восстановлен! Получено: звёзды {0}, монеты {1}.",
            ["lose"] = "Прилив оказался сильнее. Попробуйте уровень снова.",
            ["reward"] = "Ежедневная награда: {0} монет. Возвращайтесь завтра, чтобы продолжить серию.",
            ["coral"] = "коралл", ["shell"] = "ракушки", ["drop"] = "капли", ["sunstone"] = "солнечные камни", ["starfish"] = "морские звёзды", ["crystal"] = "кристаллы"
        };

        public static bool IsRussian
        {
            get
            {
                string language = SaveService.Progress?.LanguageCode;
                return string.IsNullOrWhiteSpace(language)
                    ? Application.systemLanguage == SystemLanguage.Russian
                    : language == "ru";
            }
        }

        public static string Get(string key, params object[] arguments)
        {
            Dictionary<string, string> table = IsRussian ? Russian : English;
            string value = table.TryGetValue(key, out string translated) ? translated : English[key];
            return arguments.Length == 0 ? value : string.Format(CultureInfo.InvariantCulture, value, arguments);
        }

        public static void ToggleLanguage()
        {
            SaveService.Progress.LanguageCode = IsRussian ? "en" : "ru";
            SaveService.Save();
        }
    }
}
