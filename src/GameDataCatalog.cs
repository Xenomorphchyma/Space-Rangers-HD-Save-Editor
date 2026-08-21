using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SpaceRangersHdSaveEditor
{
    internal sealed class MicroModuleCatalogEntry
    {
        internal int Index;
        internal int SortKey;
        internal string BlockName;
        internal uint ReferenceId;
        internal string Name;
        internal bool Special;
        internal int CostPercent = 100;
        internal int SizePercent = 100;
        internal readonly Dictionary<string, int> Bonuses =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public override string ToString()
        {
            string caption = string.IsNullOrWhiteSpace(Name) ? BlockName : Name;
            return caption + "  [" + Index.ToString(CultureInfo.InvariantCulture) + "]";
        }
    }

    internal sealed class HullSeriesCatalogEntry
    {
        internal int Index;
        internal int SortKey;
        internal string BlockName;
        internal uint ReferenceId;
        internal string Name;
        internal int CostPercent = 100;
        internal int SizePercent = 100;

        public override string ToString()
        {
            string caption = string.IsNullOrWhiteSpace(Name) ? BlockName : Name;
            return caption + "  [" + Index.ToString(CultureInfo.InvariantCulture) + "]";
        }
    }

    internal sealed class GameDataCatalog
    {
        private static readonly string[] BonusNames = {
            "bonHull", "bonFuel", "bonSpeed", "bonJump", "bonRadar", "bonScan", "bonDroid",
            "bonHook", "bonDef", "bonWEnergy", "bonWSplinter", "bonWMissile", "bonWRadius",
            "bonSlotRadar", "bonSlotScaner", "bonSlotDroid", "bonSlotHook", "bonSlotDef",
            "bonSlotWeapon", "bonSlotArt", "bonSlotForsage", "bonHookRadius", "bonSkill1",
            "bonSkill2", "bonSkill3", "bonSkill4", "bonSkill5", "bonSkill6", "bonMass",
            "bonExtraAkrinEff", "bonExtraAkrinPenalty", "bonAmmo", "bonShots", "bonMissileSpeed",
            "bonShotSpeed", "bonHookMaxSpeed", "bonHookMinSpeed", "bonStimCapacity", "bonZonds",
            "bonAttacks", "bonResistAsteroid", "bonAIValue", "bonNull"
        };

        internal readonly List<MicroModuleCatalogEntry> MicroModules =
            new List<MicroModuleCatalogEntry>();
        internal readonly List<HullSeriesCatalogEntry> HullSeries =
            new List<HullSeriesCatalogEntry>();
        internal readonly Dictionary<int, int> WeaponDamageGroups =
            new Dictionary<int, int>();
        internal readonly Dictionary<byte, string> RewardNames =
            new Dictionary<byte, string>();
        internal readonly Dictionary<string, string> LanguageStrings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<uint, string> ConstellationNames =
            new Dictionary<uint, string>();
        internal readonly Dictionary<uint, string> StarNames =
            new Dictionary<uint, string>();
        internal readonly Dictionary<string, string> EnglishStarNamesByRussian =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<string, int> RaceColors =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        internal readonly List<string> SputnikGraphs = new List<string>();
        internal readonly List<string> ShipGraphs = new List<string>();
        internal readonly List<string> PlanetGraphs = new List<string>();
        internal readonly List<string> AsteroidGraphs = new List<string>();
        internal readonly List<string> Diagnostics = new List<string>();
        internal int SourceCount;
        internal bool EnglishLanguage;

        internal GameDataCatalog()
        {
            AddStockWeaponDamageFallbacks(WeaponDamageGroups);
        }

        internal bool IsAvailable
        {
            get { return MicroModules.Count != 0 || HullSeries.Count != 0 || RewardNames.Count != 0 ||
                SputnikGraphs.Count != 0 || ShipGraphs.Count != 0 || PlanetGraphs.Count != 0 ||
                AsteroidGraphs.Count != 0; }
        }

        internal static GameDataCatalog Load(string gamePath, string usedMods)
        {
            return Load(gamePath, usedMods, 0);
        }

        internal static GameDataCatalog Load(string gamePath, string usedMods, int languageIndex)
        {
            GameDataCatalog result = new GameDataCatalog();
            result.EnglishLanguage = languageIndex == 1;
            if (string.IsNullOrWhiteSpace(gamePath) || !Directory.Exists(gamePath))
            {
                result.Diagnostics.Add("Папка игры не найдена: каталоги Lang.dat недоступны.");
                return result;
            }

            List<string> sources = CollectSources(gamePath, usedMods, result.Diagnostics,
                languageIndex == 1 ? "Eng" : "Rus");
            Dictionary<string, CatalogBlock> micro =
                new Dictionary<string, CatalogBlock>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, CatalogBlock> hull =
                new Dictionary<string, CatalogBlock>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, CatalogBlock> rewards =
                new Dictionary<string, CatalogBlock>(StringComparer.OrdinalIgnoreCase);
            int nextOrder = 0;
            foreach (string source in sources)
            {
                try
                {
                    if (string.Equals(Path.GetExtension(source), ".dat",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        List<BlockParEntry> roots = DecodeDat(source);
                        ApplyBinaryLanguageStrings(roots, result.LanguageStrings);
                        ApplyBinaryConstellationNames(roots, result.ConstellationNames);
                        ApplyBinaryStarNames(roots, result.StarNames);
                        ApplyBinaryCatalog(roots, "MicroModuls", micro, ref nextOrder);
                        ApplyBinaryCatalog(roots, "HullType", hull, ref nextOrder);
                        ApplyBinaryCatalog(roots, "Reward", rewards, ref nextOrder);
                        ApplyBinaryWeaponDamageGroups(roots, result.WeaponDamageGroups);
                    }
                    else
                    {
                        ApplyTextLanguageStrings(source, result.LanguageStrings);
                        ApplyTextConstellationNames(source, result.ConstellationNames);
                        ApplyTextStarNames(source, result.StarNames);
                        ParseCatalogPatches(source, "MicroModuls", micro, ref nextOrder);
                        ParseCatalogPatches(source, "HullType", hull, ref nextOrder);
                        ParseCatalogPatches(source, "Reward", rewards, ref nextOrder);
                        ParseTextWeaponDamageGroups(source, result.WeaponDamageGroups);
                    }
                    result.SourceCount++;
                }
                catch (Exception error)
                {
                    result.Diagnostics.Add(Path.GetFileName(source) + ": " + error.Message);
                }
            }
            foreach (string source in CollectMainSources(gamePath, usedMods, result.Diagnostics))
            {
                try
                {
                    if (string.Equals(Path.GetExtension(source), ".dat", StringComparison.OrdinalIgnoreCase))
                    {
                        List<BlockParEntry> roots = DecodeDat(source);
                        ApplyBinarySputnikGraphs(roots, result.SputnikGraphs);
                        ApplyBinaryShipGraphs(roots, result.ShipGraphs);
                        ApplyBinaryDirectGraphs(roots, "Planet", result.PlanetGraphs);
                        ApplyBinaryDirectGraphs(roots, "Asteroid", result.AsteroidGraphs);
                        ApplyBinaryRaceColors(roots, result.RaceColors);
                    }
                    else
                    {
                        ApplyTextSputnikGraphs(source, result.SputnikGraphs);
                        ApplyTextShipGraphs(source, result.ShipGraphs);
                        ApplyTextDirectGraphs(source, "Planet", result.PlanetGraphs);
                        ApplyTextDirectGraphs(source, "Asteroid", result.AsteroidGraphs);
                        ApplyTextRaceColors(source, result.RaceColors);
                    }
                }
                catch (Exception error)
                {
                    result.Diagnostics.Add(Path.GetFileName(source) + " (Data.SE graphics): " + error.Message);
                }
            }
            BuildMicroModules(micro, result.MicroModules);
            BuildHullSeries(hull, result.HullSeries);
            BuildRewardNames(rewards, result.RewardNames);
            if (languageIndex == 1)
                BuildEnglishStarNameAliases(gamePath, usedMods, result);
            if (!result.IsAvailable)
                result.Diagnostics.Add("В прочитанных Lang.dat не найдены MicroModuls/HullType.");
            return result;
        }

        internal string GetLanguageString(string key, string fallback)
        {
            string value;
            return !string.IsNullOrEmpty(key) && LanguageStrings.TryGetValue(key, out value) &&
                !string.IsNullOrWhiteSpace(value) ? value : fallback;
        }

        internal bool TryGetRaceColor(string name, out int rgb)
        {
            rgb = 0;
            return !string.IsNullOrWhiteSpace(name) && RaceColors.TryGetValue(name, out rgb);
        }

        internal string GetConstellationName(uint id, string fallback)
        {
            string value;
            return ConstellationNames.TryGetValue(id, out value) &&
                !string.IsNullOrWhiteSpace(value) ? value : fallback;
        }

        internal string GetStarName(string savedName)
        {
            if (string.IsNullOrWhiteSpace(savedName)) return savedName ?? string.Empty;
            string value;
            if (EnglishStarNamesByRussian.TryGetValue(savedName.Trim(), out value) &&
                !string.IsNullOrWhiteSpace(value)) return value;
            return EnglishLanguage ? TransliterateRussian(savedName) : savedName;
        }

        private static string TransliterateRussian(string value)
        {
            if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
            StringBuilder result = new StringBuilder(value.Length + 8);
            foreach (char character in value)
            {
                string replacement;
                switch (char.ToUpperInvariant(character))
                {
                    case 'А': replacement = "A"; break; case 'Б': replacement = "B"; break;
                    case 'В': replacement = "V"; break; case 'Г': replacement = "G"; break;
                    case 'Д': replacement = "D"; break; case 'Е': replacement = "E"; break;
                    case 'Ё': replacement = "Yo"; break; case 'Ж': replacement = "Zh"; break;
                    case 'З': replacement = "Z"; break; case 'И': replacement = "I"; break;
                    case 'Й': replacement = "Y"; break; case 'К': replacement = "K"; break;
                    case 'Л': replacement = "L"; break; case 'М': replacement = "M"; break;
                    case 'Н': replacement = "N"; break; case 'О': replacement = "O"; break;
                    case 'П': replacement = "P"; break; case 'Р': replacement = "R"; break;
                    case 'С': replacement = "S"; break; case 'Т': replacement = "T"; break;
                    case 'У': replacement = "U"; break; case 'Ф': replacement = "F"; break;
                    case 'Х': replacement = "Kh"; break; case 'Ц': replacement = "Ts"; break;
                    case 'Ч': replacement = "Ch"; break; case 'Ш': replacement = "Sh"; break;
                    case 'Щ': replacement = "Shch"; break; case 'Ы': replacement = "Y"; break;
                    case 'Э': replacement = "E"; break; case 'Ю': replacement = "Yu"; break;
                    case 'Я': replacement = "Ya"; break; case 'Ъ': case 'Ь': replacement = string.Empty; break;
                    default: result.Append(character); continue;
                }
                result.Append(char.IsUpper(character) ? replacement : replacement.ToLowerInvariant());
            }
            return result.ToString();
        }

        internal MicroModuleCatalogEntry FindMicroModule(int index, uint referenceId)
        {
            if (index > 0 && index <= MicroModules.Count)
            {
                MicroModuleCatalogEntry byIndex = MicroModules[index - 1];
                if (referenceId == 0 || byIndex.ReferenceId == referenceId) return byIndex;
            }
            if (referenceId != 0)
                foreach (MicroModuleCatalogEntry entry in MicroModules)
                    if (entry.ReferenceId == referenceId) return entry;
            return null;
        }

        internal HullSeriesCatalogEntry FindHullSeries(int index, uint referenceId)
        {
            if (index >= 0 && index < HullSeries.Count)
            {
                HullSeriesCatalogEntry byIndex = HullSeries[index];
                if (referenceId == 0 || byIndex.ReferenceId == referenceId) return byIndex;
            }
            if (referenceId != 0)
                foreach (HullSeriesCatalogEntry entry in HullSeries)
                    if (entry.ReferenceId == referenceId) return entry;
            return null;
        }

        internal int GetWeaponDamageGroup(byte itemType)
        {
            int group;
            int weaponNumber = itemType - 49;
            return WeaponDamageGroups.TryGetValue(weaponNumber, out group) ? group : 0;
        }

        private static List<string> CollectSources(string gamePath, string usedMods,
            List<string> diagnostics, string preferredLocale)
        {
            List<string> result = new List<string>();
            string baseSource = FindLanguageSource(Path.Combine(gamePath, "CFG"), preferredLocale);
            if (baseSource != null) result.Add(baseSource);
            else diagnostics.Add("Не найден базовый CFG\\Rus\\Lang.dat.");

            string modsRoot = Path.GetFullPath(Path.Combine(gamePath, "Mods"));
            string rootedPrefix = modsRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            foreach (string raw in SplitModList(usedMods))
            {
                try
                {
                    string relative = raw.Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar).Trim();
                    if (Path.IsPathRooted(relative))
                    {
                        diagnostics.Add("Пропущен абсолютный путь мода: " + raw);
                        continue;
                    }
                    string modPath = Path.GetFullPath(Path.Combine(modsRoot, relative));
                    if (!modPath.StartsWith(rootedPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        diagnostics.Add("Пропущен путь мода вне Mods: " + raw);
                        continue;
                    }
                    string source = FindLanguageSource(Path.Combine(modPath, "CFG"), preferredLocale);
                    if (source != null) result.Add(source);
                }
                catch (Exception error)
                {
                    diagnostics.Add("Мод " + raw + ": " + error.Message);
                }
            }
            return result;
        }

        private static IEnumerable<string> SplitModList(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) yield break;
            string[] parts = value.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (trimmed.Length != 0) yield return trimmed;
            }
        }

        private static string FindLanguageSource(string cfgPath, string preferredLocale)
        {
            string[] locales = string.Equals(preferredLocale, "Eng", StringComparison.OrdinalIgnoreCase)
                ? new string[] { "Eng", "Rus" } : new string[] { "Rus", "Eng" };
            foreach (string locale in locales)
            {
                string directory = Path.Combine(cfgPath, locale);
                string dat = Path.Combine(directory, "Lang.dat");
                if (File.Exists(dat)) return dat;
                string txt = Path.Combine(directory, "Lang.txt");
                if (File.Exists(txt)) return txt;
            }
            return null;
        }

        private static List<string> CollectMainSources(string gamePath, string usedMods,
            List<string> diagnostics)
        {
            List<string> result = new List<string>();
            string baseSource = FindMainSource(Path.Combine(gamePath, "CFG"));
            if (baseSource != null) result.Add(baseSource);
            else diagnostics.Add("Не найден базовый CFG\\Main.dat.");

            string modsRoot = Path.GetFullPath(Path.Combine(gamePath, "Mods"));
            string rootedPrefix = modsRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            foreach (string raw in SplitModList(usedMods))
            {
                try
                {
                    string relative = raw.Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar).Trim();
                    if (Path.IsPathRooted(relative)) continue;
                    string modPath = Path.GetFullPath(Path.Combine(modsRoot, relative));
                    if (!modPath.StartsWith(rootedPrefix, StringComparison.OrdinalIgnoreCase)) continue;
                    string source = FindMainSource(Path.Combine(modPath, "CFG"));
                    if (source != null) result.Add(source);
                }
                catch { }
            }
            return result;
        }

        private static string FindMainSource(string cfgPath)
        {
            string dat = Path.Combine(cfgPath, "Main.dat");
            if (File.Exists(dat)) return dat;
            string txt = Path.Combine(cfgPath, "Main.txt");
            return File.Exists(txt) ? txt : null;
        }

        private sealed class CatalogBlock
        {
            internal string Name;
            internal int Order;
            internal readonly Dictionary<string, string> Values =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static void ParseCatalogPatches(string path, string catalogName,
            Dictionary<string, CatalogBlock> destination, ref int nextOrder)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            int depth = 0;
            bool inCatalog = false;
            CatalogBlock current = null;
            char currentOperator = '^';
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    if (depth == 0)
                        inCatalog = string.Equals(blockName, catalogName,
                            StringComparison.OrdinalIgnoreCase);
                    else if (depth == 1 && inCatalog)
                    {
                        current = new CatalogBlock();
                        current.Name = blockName;
                        currentOperator = operation;
                    }
                    depth++;
                    continue;
                }
                if (line == "}")
                {
                    if (depth == 2 && inCatalog && current != null)
                    {
                        CatalogBlock existing;
                        if (!destination.TryGetValue(current.Name, out existing))
                        {
                            current.Order = nextOrder++;
                            destination.Add(current.Name, current);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, string> pair in current.Values)
                                existing.Values[pair.Key] = pair.Value;
                        }
                        current = null;
                    }
                    if (depth > 0) depth--;
                    if (depth == 0) inCatalog = false;
                    continue;
                }
                if (depth == 2 && inCatalog && current != null)
                {
                    int separator = line.IndexOf('=');
                    if (separator > 0)
                    {
                        string key = line.Substring(0, separator).Trim();
                        string value = line.Substring(separator + 1).Trim();
                        current.Values[key] = value;
                    }
                }
            }
        }

        private static void ParseTextWeaponDamageGroups(string path,
            Dictionary<int, int> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> pathStack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    pathStack.Add(blockName);
                    continue;
                }
                if (line == "}")
                {
                    if (pathStack.Count != 0) pathStack.RemoveAt(pathStack.Count - 1);
                    continue;
                }
                if (pathStack.Count != 3 ||
                    !string.Equals(pathStack[0], "Weapon", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(pathStack[1], "Stats", StringComparison.OrdinalIgnoreCase))
                    continue;
                int separator = line.IndexOf('=');
                int weaponNumber;
                if (separator <= 0 ||
                    !string.Equals(line.Substring(0, separator).Trim(), "DamageSet",
                        StringComparison.OrdinalIgnoreCase) ||
                    !int.TryParse(pathStack[2], NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out weaponNumber) || weaponNumber < 1 || weaponNumber > 18) continue;
                destination[weaponNumber] = DamageGroupFromSet(line.Substring(separator + 1).Trim());
            }
        }

        private static bool TryParseBlockStart(string line, out string name, out char operation)
        {
            name = null;
            operation = '\0';
            if (line.Length < 3 || line[line.Length - 1] != '{') return false;
            char candidate = line[line.Length - 2];
            if (candidate != '^' && candidate != '~') return false;
            name = line.Substring(0, line.Length - 2).Trim();
            operation = candidate;
            return name.Length != 0;
        }

        private static string ReadText(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length >= 2 && bytes[0] == 0xff && bytes[1] == 0xfe)
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            if (bytes.Length >= 2 && bytes[0] == 0xfe && bytes[1] == 0xff)
                return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
            if (bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(1251).GetString(bytes);
            }
        }

        private sealed class BlockParEntry
        {
            internal byte Type;
            internal int FirstIndex;
            internal int SecondIndex;
            internal string Name;
            internal string Value;
            internal List<BlockParEntry> Children;
        }

        private sealed class BlockParReader
        {
            private readonly byte[] data;
            private int offset;
            private int totalEntries;

            internal BlockParReader(byte[] value)
            {
                data = value;
            }

            internal List<BlockParEntry> ReadDocument()
            {
                List<BlockParEntry> result = ReadEntries(0);
                if (offset != data.Length)
                    throw new InvalidDataException("В бинарном дереве BlockPar остались лишние байты.");
                return result;
            }

            private List<BlockParEntry> ReadEntries(int depth)
            {
                if (depth > 128) throw new InvalidDataException("Слишком глубокое дерево BlockPar.");
                byte indexed = ReadByte();
                if (indexed > 1) throw new InvalidDataException("Неизвестный флаг индекса BlockPar.");
                int count = ReadInt32();
                if (count < 0 || count > 1000000 || totalEntries > 2000000 - count)
                    throw new InvalidDataException("Недопустимое число элементов BlockPar.");
                totalEntries += count;
                List<BlockParEntry> result = new List<BlockParEntry>(count);
                for (int index = 0; index < count; index++)
                {
                    BlockParEntry entry = new BlockParEntry();
                    if (indexed != 0)
                    {
                        entry.FirstIndex = ReadInt32();
                        entry.SecondIndex = ReadInt32();
                    }
                    entry.Type = ReadByte();
                    entry.Name = ReadUtf16Z();
                    if (entry.Type == 1) entry.Value = ReadUtf16Z();
                    else if (entry.Type == 2) entry.Children = ReadEntries(depth + 1);
                    else if (entry.Type != 0)
                        throw new InvalidDataException("Неизвестный тип элемента BlockPar: " + entry.Type + ".");
                    result.Add(entry);
                }
                return result;
            }

            private byte ReadByte()
            {
                if (offset >= data.Length) throw new EndOfStreamException("BlockPar: ожидался Byte.");
                return data[offset++];
            }

            private int ReadInt32()
            {
                if (offset > data.Length - 4) throw new EndOfStreamException("BlockPar: ожидался Int32.");
                int value = BitConverter.ToInt32(data, offset);
                offset += 4;
                return value;
            }

            private string ReadUtf16Z()
            {
                int start = offset;
                int characters = 0;
                while (offset <= data.Length - 2)
                {
                    ushort character = BitConverter.ToUInt16(data, offset);
                    offset += 2;
                    if (character == 0)
                        return Encoding.Unicode.GetString(data, start, characters * 2);
                    characters++;
                    if (characters > 1000000)
                        throw new InvalidDataException("Слишком длинная строка BlockPar.");
                }
                throw new EndOfStreamException("BlockPar: строка UTF-16 не завершена.");
            }
        }

        private static List<BlockParEntry> DecodeDat(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length >= 8 &&
                (ReadUInt32(bytes, 0) ^ 0xb589026eu) == (uint)(bytes.Length - 8))
            {
                byte[] inner = new byte[bytes.Length - 8];
                Buffer.BlockCopy(bytes, 8, inner, 0, inner.Length);
                bytes = inner;
            }

            bytes = DecodeEncryptedLayer(bytes);
            if (bytes.Length >= 8 && bytes[0] == (byte)'Z' && bytes[1] == (byte)'L' &&
                bytes[2] == (byte)'0' && bytes[3] == (byte)'1')
                bytes = DecompressZl01(bytes);
            return new BlockParReader(bytes).ReadDocument();
        }

        private static byte[] DecodeEncryptedLayer(byte[] bytes)
        {
            if (LooksLikePlainBlockPar(bytes) ||
                (bytes.Length >= 4 && bytes[0] == (byte)'Z' && bytes[1] == (byte)'L' &&
                 bytes[2] == (byte)'0' && bytes[3] == (byte)'1')) return bytes;
            if (bytes.Length <= 8) throw new InvalidDataException("Слишком короткий DAT BlockPar.");
            uint expectedCrc = ReadUInt32(bytes, 0);
            uint encodedSeed = ReadUInt32(bytes, 4);
            uint[] keys = { 0xbe970bf1u, 0xf2fde658u, 0u, 0xb1e8c689u, 0xea8f3f37u, 0x7cde1c5fu };
            foreach (uint key in keys)
            {
                byte[] decoded = new byte[bytes.Length - 8];
                Buffer.BlockCopy(bytes, 8, decoded, 0, decoded.Length);
                int seed = unchecked((int)(encodedSeed ^ key));
                unchecked
                {
                    for (int index = 0; index < decoded.Length; index++)
                    {
                        seed = (seed % 127773) * 16807 - (seed / 127773) * 2836;
                        if (seed <= 0) seed += int.MaxValue;
                        decoded[index] ^= (byte)(seed - 1);
                    }
                }
                if (Crc32(decoded, 0, decoded.Length) == expectedCrc) return decoded;
            }
            throw new InvalidDataException("DAT BlockPar не прошёл CRC ни с одним штатным ключом.");
        }

        private static bool LooksLikePlainBlockPar(byte[] bytes)
        {
            if (bytes.Length < 5 || bytes[0] > 1) return false;
            int count = BitConverter.ToInt32(bytes, 1);
            return count >= 0 && count <= 1000000;
        }

        private static byte[] DecompressZl01(byte[] bytes)
        {
            int expected = BitConverter.ToInt32(bytes, 4);
            if (expected < 0 || expected > 1024 * 1024 * 1024)
                throw new InvalidDataException("ZL01 содержит недопустимый размер результата.");
            Exception firstError = null;
            try { return Inflate(bytes, 8, bytes.Length - 8, expected); }
            catch (Exception error) { firstError = error; }
            if (bytes.Length > 14 && bytes[8] == 0x78)
            {
                try { return Inflate(bytes, 10, bytes.Length - 14, expected); }
                catch { }
            }
            throw new InvalidDataException("Не удалось распаковать ZL01.", firstError);
        }

        private static byte[] Inflate(byte[] bytes, int offset, int count, int expected)
        {
            using (MemoryStream source = new MemoryStream(bytes, offset, count, false))
            using (DeflateStream inflater = new DeflateStream(source, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream(expected > 0 ? expected : 0))
            {
                inflater.CopyTo(output);
                byte[] result = output.ToArray();
                if (result.Length != expected)
                    throw new InvalidDataException("Размер ZL01 не совпал с заголовком.");
                return result;
            }
        }

        private static void ApplyBinaryCatalog(List<BlockParEntry> roots, string catalogName,
            Dictionary<string, CatalogBlock> destination, ref int nextOrder)
        {
            foreach (BlockParEntry root in roots)
            {
                if (root.Type != 2 || !string.Equals(root.Name, catalogName,
                StringComparison.OrdinalIgnoreCase) || root.Children == null) continue;
                foreach (BlockParEntry child in root.Children)
                {
                    if (child.Type == 0)
                    {
                        destination.Remove(child.Name);
                        continue;
                    }
                    if (child.Type != 2 || child.Children == null) continue;
                    CatalogBlock existing;
                    if (!destination.TryGetValue(child.Name, out existing))
                    {
                        existing = new CatalogBlock();
                        existing.Name = child.Name;
                        existing.Order = nextOrder++;
                        destination.Add(existing.Name, existing);
                    }
                    foreach (BlockParEntry parameter in child.Children)
                        if (parameter.Type == 1)
                            existing.Values[parameter.Name] = parameter.Value ?? string.Empty;
                }
            }
        }

        private static void ApplyBinaryLanguageStrings(List<BlockParEntry> entries,
            Dictionary<string, string> destination)
        {
            if (entries == null) return;
            foreach (BlockParEntry entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.Name) &&
                    entry.Name.StartsWith("ITEMTYPE_", StringComparison.OrdinalIgnoreCase))
                {
                    if (entry.Type == 0) destination.Remove(entry.Name);
                    else if (entry.Type == 1) destination[entry.Name] = entry.Value ?? string.Empty;
                }
                if (entry.Children != null)
                    ApplyBinaryLanguageStrings(entry.Children, destination);
            }
        }

        private static void ApplyBinaryConstellationNames(List<BlockParEntry> roots,
            Dictionary<uint, string> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots,
                new string[] { "Constellations", "Name" }, 0);
            if (catalog == null)
                catalog = FindBinaryBlock(roots, new string[] { "Constellations.Name" }, 0);
            if (catalog == null || catalog.Children == null) return;
            foreach (BlockParEntry entry in catalog.Children)
            {
                uint id;
                if (!uint.TryParse(entry.Name, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out id)) continue;
                if (entry.Type == 0) destination.Remove(id);
                else if (entry.Type == 1) destination[id] = entry.Value ?? string.Empty;
            }
        }

        private static void ApplyTextLanguageStrings(string path,
            Dictionary<string, string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                int separator = line.IndexOf('=');
                if (separator <= 0) continue;
                string key = line.Substring(0, separator).Trim();
                if (!key.StartsWith("ITEMTYPE_", StringComparison.OrdinalIgnoreCase)) continue;
                string value = line.Substring(separator + 1).Trim().TrimEnd(';').Trim();
                if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') ||
                    (value[0] == '\'' && value[value.Length - 1] == '\'')))
                    value = value.Substring(1, value.Length - 2);
                destination[key] = value.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }

        private static void ApplyTextConstellationNames(string path,
            Dictionary<uint, string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> stack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    stack.Add(blockName);
                    continue;
                }
                if (line == "}")
                {
                    if (stack.Count != 0) stack.RemoveAt(stack.Count - 1);
                    continue;
                }
                bool inNames = stack.Count == 2 &&
                    string.Equals(stack[0], "Constellations", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(stack[1], "Name", StringComparison.OrdinalIgnoreCase) ||
                    stack.Count == 1 && string.Equals(stack[0], "Constellations.Name",
                        StringComparison.OrdinalIgnoreCase);
                if (!inNames) continue;
                int separator = line.IndexOf('=');
                if (separator <= 0) continue;
                uint id;
                if (!uint.TryParse(line.Substring(0, separator).Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out id)) continue;
                string value = line.Substring(separator + 1).Trim().TrimEnd(';').Trim();
                if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') ||
                    (value[0] == '\'' && value[value.Length - 1] == '\'')))
                    value = value.Substring(1, value.Length - 2);
                destination[id] = value;
            }
        }

        private static void ApplyBinaryStarNames(List<BlockParEntry> roots,
            Dictionary<uint, string> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots, new string[] { "Star" }, 0);
            if (catalog == null || catalog.Children == null) return;
            foreach (BlockParEntry entry in catalog.Children)
            {
                uint id;
                if (!uint.TryParse(entry.Name, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out id)) continue;
                if (entry.Type == 0) destination.Remove(id);
                else if (entry.Type == 1) destination[id] = NameBeforeCatalogSuffix(entry.Value);
            }
        }

        private static void ApplyTextStarNames(string path,
            Dictionary<uint, string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            int depth = 0;
            bool inCatalog = false;
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    if (depth == 0)
                        inCatalog = string.Equals(blockName, "Star", StringComparison.OrdinalIgnoreCase);
                    depth++;
                    continue;
                }
                if (line == "}")
                {
                    depth = Math.Max(0, depth - 1);
                    if (depth == 0) inCatalog = false;
                    continue;
                }
                if (!inCatalog || depth != 1) continue;
                int separator = line.IndexOf('=');
                if (separator <= 0) continue;
                uint id;
                if (!uint.TryParse(line.Substring(0, separator).Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out id)) continue;
                string value = line.Substring(separator + 1).Trim().TrimEnd(';').Trim();
                if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') ||
                    (value[0] == '\'' && value[value.Length - 1] == '\'')))
                    value = value.Substring(1, value.Length - 2);
                destination[id] = NameBeforeCatalogSuffix(value);
            }
        }

        private static string NameBeforeCatalogSuffix(string value)
        {
            string result = value ?? string.Empty;
            int separator = result.IndexOf(',');
            if (separator >= 0) result = result.Substring(0, separator);
            return result.Trim();
        }

        private static void BuildEnglishStarNameAliases(string gamePath, string usedMods,
            GameDataCatalog destination)
        {
            Dictionary<uint, string> russianNames = new Dictionary<uint, string>();
            List<string> ignoredDiagnostics = new List<string>();
            foreach (string source in CollectSources(gamePath, usedMods, ignoredDiagnostics, "Rus"))
            {
                try
                {
                    if (string.Equals(Path.GetExtension(source), ".dat", StringComparison.OrdinalIgnoreCase))
                        ApplyBinaryStarNames(DecodeDat(source), russianNames);
                    else
                        ApplyTextStarNames(source, russianNames);
                }
                catch
                {
                    // Star-name localization is optional; the SAV name remains a safe fallback.
                }
            }
            foreach (KeyValuePair<uint, string> pair in russianNames)
            {
                string english;
                if (string.IsNullOrWhiteSpace(pair.Value) ||
                    !destination.StarNames.TryGetValue(pair.Key, out english) ||
                    string.IsNullOrWhiteSpace(english)) continue;
                destination.EnglishStarNamesByRussian[pair.Value] = english;
            }
        }

        private static void ApplyBinarySputnikGraphs(List<BlockParEntry> roots, List<string> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots, new string[] { "Data", "SE", "Sputnik" }, 0);
            if (catalog == null)
                catalog = FindBinaryBlock(roots, new string[] { "Data.SE.Sputnik" }, 0);
            if (catalog == null || catalog.Children == null) return;
            foreach (BlockParEntry child in catalog.Children)
            {
                if (string.IsNullOrWhiteSpace(child.Name)) continue;
                string graph = child.Name.StartsWith("Sputnik.", StringComparison.OrdinalIgnoreCase)
                    ? child.Name : "Sputnik." + child.Name;
                int existing = destination.FindIndex(delegate(string value)
                    { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); });
                if (child.Type == 0)
                {
                    if (existing >= 0) destination.RemoveAt(existing);
                }
                else if (existing < 0)
                    destination.Add(graph);
            }
        }

        private static void ApplyBinaryShipGraphs(List<BlockParEntry> roots, List<string> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots, new string[] { "Data", "SE", "Ship" }, 0);
            if (catalog == null)
                catalog = FindBinaryBlock(roots, new string[] { "Data.SE.Ship" }, 0);
            if (catalog == null || catalog.Children == null) return;
            CollectBinaryShipGraphs(catalog.Children, new List<string>(), destination);
        }

        private static void ApplyBinaryDirectGraphs(List<BlockParEntry> roots,
            string catalogName, List<string> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots,
                new string[] { "Data", "SE", catalogName }, 0);
            if (catalog == null)
                catalog = FindBinaryBlock(roots, new string[] { "Data.SE." + catalogName }, 0);
            if (catalog == null || catalog.Children == null) return;
            foreach (BlockParEntry child in catalog.Children)
            {
                if (string.IsNullOrWhiteSpace(child.Name)) continue;
                string graph = child.Name.StartsWith(catalogName + ".",
                    StringComparison.OrdinalIgnoreCase)
                    ? child.Name : catalogName + "." + child.Name;
                int existing = destination.FindIndex(delegate(string value)
                    { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); });
                if (child.Type == 0)
                {
                    if (existing >= 0) destination.RemoveAt(existing);
                }
                else if (child.Type == 2 && existing < 0)
                    destination.Add(graph);
            }
        }

        private static void ApplyBinaryRaceColors(List<BlockParEntry> roots,
            Dictionary<string, int> destination)
        {
            BlockParEntry catalog = FindBinaryBlock(roots,
                new string[] { "Data", "Race", "Color" }, 0);
            if (catalog == null)
                catalog = FindBinaryBlock(roots, new string[] { "Data.Race.Color" }, 0);
            if (catalog == null || catalog.Children == null) return;
            foreach (BlockParEntry entry in catalog.Children)
            {
                if (entry.Type == 0) destination.Remove(entry.Name);
                else if (entry.Type == 1)
                {
                    int rgb;
                    if (TryParseRgb(entry.Value, out rgb)) destination[entry.Name] = rgb;
                }
            }
        }

        private static void CollectBinaryShipGraphs(List<BlockParEntry> entries,
            List<string> path, List<string> destination)
        {
            foreach (BlockParEntry entry in entries)
            {
                if (entry.Type != 2 || entry.Children == null || string.IsNullOrWhiteSpace(entry.Name))
                    continue;
                path.Add(entry.Name);
                bool hasImage = entry.Children.Exists(delegate(BlockParEntry child)
                    { return child.Type == 1 && string.Equals(child.Name, "Image",
                        StringComparison.OrdinalIgnoreCase); });
                string graph = "Ship." + string.Join(".", path.ToArray());
                int existing = destination.FindIndex(delegate(string value)
                    { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); });
                if (hasImage && existing < 0) destination.Add(graph);
                CollectBinaryShipGraphs(entry.Children, path, destination);
                path.RemoveAt(path.Count - 1);
            }
        }

        private static BlockParEntry FindBinaryBlock(List<BlockParEntry> entries,
            string[] path, int depth)
        {
            if (entries == null || depth >= path.Length) return null;
            foreach (BlockParEntry entry in entries)
                if (entry.Type == 2 && entry.Children != null &&
                    string.Equals(entry.Name, path[depth], StringComparison.OrdinalIgnoreCase))
                    return depth + 1 == path.Length ? entry :
                        FindBinaryBlock(entry.Children, path, depth + 1);
            return null;
        }

        private static void ApplyTextSputnikGraphs(string path, List<string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> stack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    bool parentIsCatalog = stack.Count == 3 &&
                        string.Equals(stack[0], "Data", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stack[1], "SE", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stack[2], "Sputnik", StringComparison.OrdinalIgnoreCase);
                    if (parentIsCatalog)
                    {
                        string graph = blockName.StartsWith("Sputnik.", StringComparison.OrdinalIgnoreCase)
                            ? blockName : "Sputnik." + blockName;
                        if (!destination.Exists(delegate(string value)
                            { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); }))
                            destination.Add(graph);
                    }
                    stack.Add(blockName);
                    continue;
                }
                if (line == "}" && stack.Count != 0) stack.RemoveAt(stack.Count - 1);
            }
        }

        private static void ApplyTextRaceColors(string path,
            Dictionary<string, int> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> stack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    stack.Add(blockName);
                    continue;
                }
                if (line == "}")
                {
                    if (stack.Count != 0) stack.RemoveAt(stack.Count - 1);
                    continue;
                }
                bool inColors = stack.Count == 3 &&
                    string.Equals(stack[0], "Data", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(stack[1], "Race", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(stack[2], "Color", StringComparison.OrdinalIgnoreCase) ||
                    stack.Count == 1 && string.Equals(stack[0], "Data.Race.Color",
                        StringComparison.OrdinalIgnoreCase);
                if (!inColors) continue;
                int separator = line.IndexOf('=');
                if (separator <= 0) continue;
                string key = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim().TrimEnd(';').Trim();
                if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') ||
                    (value[0] == '\'' && value[value.Length - 1] == '\'')))
                    value = value.Substring(1, value.Length - 2);
                int rgb;
                if (TryParseRgb(value, out rgb)) destination[key] = rgb;
            }
        }

        private static bool TryParseRgb(string value, out int rgb)
        {
            rgb = 0;
            if (string.IsNullOrWhiteSpace(value)) return false;
            string[] parts = value.Split(',');
            if (parts.Length < 3) return false;
            int red, green, blue;
            if (!int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out red) ||
                !int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out green) ||
                !int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out blue))
                return false;
            red = Math.Max(0, Math.Min(255, red));
            green = Math.Max(0, Math.Min(255, green));
            blue = Math.Max(0, Math.Min(255, blue));
            rgb = red << 16 | green << 8 | blue;
            return true;
        }

        private static void ApplyTextShipGraphs(string path, List<string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> stack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    stack.Add(blockName);
                    continue;
                }
                if (line == "}")
                {
                    if (stack.Count != 0) stack.RemoveAt(stack.Count - 1);
                    continue;
                }
                if (stack.Count <= 3 ||
                    !string.Equals(stack[0], "Data", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(stack[1], "SE", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(stack[2], "Ship", StringComparison.OrdinalIgnoreCase)) continue;
                int equals = line.IndexOf('=');
                if (equals <= 0 || !string.Equals(line.Substring(0, equals).Trim(), "Image",
                    StringComparison.OrdinalIgnoreCase)) continue;
                string[] suffix = stack.GetRange(3, stack.Count - 3).ToArray();
                if (Array.Exists(suffix, delegate(string value)
                    { return string.Equals(value, "Anim", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(value, "AnimS", StringComparison.OrdinalIgnoreCase); })) continue;
                string graph = "Ship." + string.Join(".", suffix);
                if (!destination.Exists(delegate(string value)
                    { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); }))
                    destination.Add(graph);
            }
        }

        private static void ApplyTextDirectGraphs(string path, string catalogName,
            List<string> destination)
        {
            string[] lines = ReadText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            List<string> stack = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw.Trim();
                char operation;
                string blockName;
                if (TryParseBlockStart(line, out blockName, out operation))
                {
                    bool parentIsCatalog = stack.Count == 3 &&
                        string.Equals(stack[0], "Data", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stack[1], "SE", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(stack[2], catalogName, StringComparison.OrdinalIgnoreCase) ||
                        stack.Count == 1 && string.Equals(stack[0], "Data.SE." + catalogName,
                            StringComparison.OrdinalIgnoreCase);
                    if (parentIsCatalog)
                    {
                        string graph = blockName.StartsWith(catalogName + ".",
                            StringComparison.OrdinalIgnoreCase)
                            ? blockName : catalogName + "." + blockName;
                        if (!destination.Exists(delegate(string value)
                            { return string.Equals(value, graph, StringComparison.OrdinalIgnoreCase); }))
                            destination.Add(graph);
                    }
                    stack.Add(blockName);
                    continue;
                }
                if (line == "}" && stack.Count != 0) stack.RemoveAt(stack.Count - 1);
            }
        }

        private static void ApplyBinaryWeaponDamageGroups(List<BlockParEntry> roots,
            Dictionary<int, int> destination)
        {
            foreach (BlockParEntry weapon in roots)
            {
                if (weapon.Type != 2 || weapon.Children == null ||
                    !string.Equals(weapon.Name, "Weapon", StringComparison.OrdinalIgnoreCase)) continue;
                foreach (BlockParEntry stats in weapon.Children)
                {
                    if (stats.Type != 2 || stats.Children == null ||
                        !string.Equals(stats.Name, "Stats", StringComparison.OrdinalIgnoreCase)) continue;
                    foreach (BlockParEntry record in stats.Children)
                    {
                        int weaponNumber;
                        if (record.Type != 2 || record.Children == null ||
                            !int.TryParse(record.Name, NumberStyles.Integer,
                                CultureInfo.InvariantCulture, out weaponNumber) ||
                            weaponNumber < 1 || weaponNumber > 18) continue;
                        foreach (BlockParEntry parameter in record.Children)
                            if (parameter.Type == 1 && string.Equals(parameter.Name, "DamageSet",
                                StringComparison.OrdinalIgnoreCase))
                                destination[weaponNumber] = DamageGroupFromSet(parameter.Value);
                    }
                }
            }
        }

        private static int DamageGroupFromSet(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                string[] parts = value.Split(',');
                bool splinter = false;
                foreach (string raw in parts)
                {
                    string part = raw.Trim();
                    if (string.Equals(part, "Missile", StringComparison.OrdinalIgnoreCase)) return 2;
                    if (string.Equals(part, "Splinter", StringComparison.OrdinalIgnoreCase)) splinter = true;
                }
                if (splinter) return 1;
            }
            return 0;
        }

        private static void AddStockWeaponDamageFallbacks(Dictionary<int, int> destination)
        {
            int[] groups = { 0, 1, 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 2, 1, 0, 2 };
            for (int index = 0; index < groups.Length; index++)
                destination[index + 1] = groups[index];
        }

        private static uint ReadUInt32(byte[] bytes, int offset)
        {
            return BitConverter.ToUInt32(bytes, offset);
        }

        private static uint Crc32(byte[] bytes, int offset, int count)
        {
            uint crc = 0xffffffffu;
            for (int index = 0; index < count; index++)
            {
                crc ^= bytes[offset + index];
                for (int bit = 0; bit < 8; bit++)
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xedb88320u : crc >> 1;
            }
            return ~crc;
        }

        private static void BuildMicroModules(Dictionary<string, CatalogBlock> blocks,
            List<MicroModuleCatalogEntry> output)
        {
            List<CatalogBlock> sorted = new List<CatalogBlock>(blocks.Values);
            sorted.Sort(delegate(CatalogBlock left, CatalogBlock right)
            {
                int compare = DigitSortKey(left.Name).CompareTo(DigitSortKey(right.Name));
                return compare != 0 ? compare : left.Order.CompareTo(right.Order);
            });
            for (int position = 0; position < sorted.Count; position++)
            {
                CatalogBlock block = sorted[position];
                MicroModuleCatalogEntry entry = new MicroModuleCatalogEntry();
                entry.Index = position + 1;
                entry.SortKey = DigitSortKey(block.Name);
                entry.BlockName = block.Name;
                entry.ReferenceId = Crc32Utf16(block.Name);
                entry.Name = GetValue(block, "Name", block.Name);
                entry.Special = DigitSortKey(GetValue(block, "Special", "0")) != 0;
                entry.CostPercent = GetInteger(block, "Cost", 100);
                entry.SizePercent = GetInteger(block, "Size", 100);
                foreach (string bonus in BonusNames)
                    entry.Bonuses[bonus] = GetInteger(block, bonus, 0);
                output.Add(entry);
            }
        }

        private static void BuildHullSeries(Dictionary<string, CatalogBlock> blocks,
            List<HullSeriesCatalogEntry> output)
        {
            List<CatalogBlock> sorted = new List<CatalogBlock>(blocks.Values);
            sorted.Sort(delegate(CatalogBlock left, CatalogBlock right)
            {
                int compare = DigitSortKey(left.Name).CompareTo(DigitSortKey(right.Name));
                return compare != 0 ? compare : left.Order.CompareTo(right.Order);
            });
            for (int position = 0; position < sorted.Count; position++)
            {
                CatalogBlock block = sorted[position];
                // Exclude the legacy marker node from the public series array.
                if (string.Equals(block.Name, "HullOldfag", StringComparison.OrdinalIgnoreCase)) continue;
                HullSeriesCatalogEntry entry = new HullSeriesCatalogEntry();
                entry.Index = output.Count;
                entry.SortKey = DigitSortKey(block.Name);
                entry.BlockName = block.Name;
                entry.ReferenceId = Crc32Utf16(block.Name);
                entry.Name = GetValue(block, "Name", block.Name);
                entry.CostPercent = GetInteger(block, "Cost", 100);
                entry.SizePercent = GetInteger(block, "Size", 100);
                output.Add(entry);
            }
        }

        private static void BuildRewardNames(Dictionary<string, CatalogBlock> blocks,
            Dictionary<byte, string> output)
        {
            foreach (CatalogBlock block in blocks.Values)
            {
                byte index;
                if (!byte.TryParse(block.Name, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out index)) continue;
                output[index] = GetValue(block, "Name", block.Name);
            }
        }

        private static string GetValue(CatalogBlock block, string name, string fallback)
        {
            string value;
            return block.Values.TryGetValue(name, out value) ? value : fallback;
        }

        private static int GetInteger(CatalogBlock block, string name, int fallback)
        {
            string text;
            int value;
            return block.Values.TryGetValue(name, out text) &&
                int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                ? value : fallback;
        }

        private static int DigitSortKey(string value)
        {
            int result = 0;
            if (value == null) return result;
            foreach (char character in value)
                if (character >= '0' && character <= '9')
                {
                    int digit = character - '0';
                    if (result > (int.MaxValue - digit) / 10) result = int.MaxValue;
                    else result = result * 10 + digit;
                }
            return result;
        }

        private static uint Crc32Utf16(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value ?? string.Empty);
            uint crc = 0xffffffffu;
            foreach (byte current in bytes)
            {
                crc ^= current;
                for (int bit = 0; bit < 8; bit++)
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xedb88320u : crc >> 1;
            }
            return ~crc;
        }
    }
}
