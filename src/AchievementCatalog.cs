using System;
using System.Collections.Generic;

namespace SpaceRangersHdSaveEditor
{
    internal static class AchievementCatalog
    {
        private static readonly Dictionary<string, string> RussianNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "AGENT", "Спецагент" },
                { "ARCHEOLOGY", "Археолог" },
                { "BLACKHEAD", "Чёрный Череп" },
                { "BREZHNEV", "Потомок Брежнева" },
                { "BUMMER", "Великий Нехочуха" },
                { "CHAMPION", "Чемпион" },
                { "DOLGOZHID", "Долгожитель" },
                { "HOLEMAN", "Потусторонний воин" },
                { "ILL", "Вечный пациент" },
                { "KIBERMAN", "Киберманьяк" },
                { "MANYFACES", "Многоликий" },
                { "MONEY", "Богатенький Буратино" },
                { "NARKOMAN", "Раб анаболиков" },
                { "OLDFAG", "Ветеран" },
                { "PEACELOVER", "Пацифист" },
                { "PIECECREATOR", "Миротворец" },
                { "PRISON", "Пахан" },
                { "SHIELD", "Страж порядка" },
                { "SPEED", "Гонщик болида" },
                { "SPRINTER", "Спринтер" },
                { "TERMINATOR", "Терминатор" },
                { "MASTER", "Вождь" },
                { "POSTMAN", "Почта Карагона" },
                { "HULL", "Мир и порядок!" },
                { "PIRATE", "Йо-хо-хо" },
                { "FRY", "Жареные факты" },
                { "COALLITION", "За коалицию!" },
                { "DEALER", "Магнат" },
                { "JUMPER", "Гиперактивность" },
                { "DEFENDER", "Врагу не сдается..." },
                { "NEGOCIANT", "Переговорщик" },
                { "HATER", "Изгой" },
                { "CREDITOR", "Злостный неплательщик" },
                { "HOLEPEACE", "Таранов" },
                { "SKILL", "Отличник боевой и политической" },
                { "GUARD", "На страже" },
                { "SCIENCE", "Двигатель прогресса" },
                { "IRONMAN", "Полководец" },
                { "BOMBER", "Взрывотехник" },
                { "ROCKET", "Зато мы делаем ракеты" },
                { "CONTRABAND", "Эль контрабандисто" },
                { "ASTEROID", "Мусоросборщик" },
                { "QUEST", "Чтение - залог здоровья!" },
                { "KELLERRESEARCH", "К прогрессу!" },
                { "KELLERDESTROY", "Аста ла виста" },
                { "BLAZERPROGRAM", "Блаззард" },
                { "BLAZERPIECE", "Спаси и сохрани" },
                { "TERRONSTAR", "Трансформируй это!" },
                { "TERRONBATTLE", "За ВДВ!" },
                { "PIRATESYSTEMS", "Мистер Хаос" },
                { "NODES", "Нода бене" },
                { "RATING", "Поул-позишн" },
                { "RUINS", "Меценат" },
                { "PIRATEWIN", "Рачехан и все-все-все" },
                { "BEST", "Более лучше" },
                { "COMMANDOR", "Настоящий полковник" },
                { "BARON", "Убивальников начальник" },
                { "GIRLSQUEST", "Подкаблучник" },
                { "GIRLSHIRE", "Если б я был султан..." },
                { "SHU", "Клонобоец" },
                { "SIDECHANGER", "Принцип неопределенности" },
                { "ENERGY", "Высокие энергии" },
                { "SCRATCHDAMAGE", "Не пробил!" },
                { "SPLINTER", "Осколки империи" },
                { "EXPLORER", "На пыльных тропинках далеких планет" },
                { "TRANCLUCATORS", "Слава роботам!" },
                { "SUNFUEL", "Термоядерная бензоколонка" },
                { "TERRORIST", "Подрывник-затейник" },
                { "COUNTERTERRORIST", "Правообладатель" },
                { "BLUEKILLS", "Антинаучный" },
                { "GREENKILLS", "И трава зеленее" },
                { "REDKILLS", "Война-войной" },
                { "BERTORSLAYER", "Монстр мания" },
                { "MAPBUILDER", "Географ карту купил" },
                { "HACKER", "Хактивист" },
                { "DELIVERY", "Почтальон Печкин" },
                { "INVESTOR", "Почётный инвестор" },
                { "INSURANCE", "Застрахуй братуху" },
                { "PRISONBAIL", "Своих не бросаем!" },
                { "ROBBER", "Это наша корова" },
                { "WARRIORKILLS", "Глоток свободы" },
                { "DRAIN", "Механический вампир" }
            };

        internal static string DisplayName(string key)
        {
            string name;
            return !string.IsNullOrEmpty(key) && RussianNames.TryGetValue(key, out name) ? name : key ?? string.Empty;
        }
    }
}
