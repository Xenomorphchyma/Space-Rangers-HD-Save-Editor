using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SpaceRangersHdSaveEditor
{
    internal sealed class AppSettings
    {
        internal int LanguageIndex;
        internal string GamePath;
        internal bool FullLog;
        internal string LastDirectory;
        internal readonly Dictionary<string, bool> GalaxyFilters =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<string, bool> SearchFilters =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        private static string SettingsPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SpaceRangersHdSaveEditor", "settings.ini");
            }
        }

        internal static AppSettings Load()
        {
            AppSettings value = new AppSettings();
            value.GamePath = DetectGamePath();
            try
            {
                if (!File.Exists(SettingsPath)) return value;
                foreach (string line in File.ReadAllLines(SettingsPath, Encoding.UTF8))
                {
                    int separator = line.IndexOf('=');
                    if (separator <= 0) continue;
                    string key = line.Substring(0, separator);
                    string raw = line.Substring(separator + 1);
                    int language;
                    if (key == "Language" && int.TryParse(raw, out language))
                        value.LanguageIndex = Math.Max(0, Math.Min(1, language));
                    else if (key == "FullLog")
                        value.FullLog = raw == "1";
                    else if (key == "GamePathBase64")
                        value.GamePath = Encoding.UTF8.GetString(Convert.FromBase64String(raw));
                    else if (key == "LastDirectoryBase64")
                        value.LastDirectory = Encoding.UTF8.GetString(Convert.FromBase64String(raw));
                    else if (key.StartsWith("GalaxyFilter.", StringComparison.OrdinalIgnoreCase))
                        value.GalaxyFilters[key.Substring(13)] = raw == "1";
                    else if (key.StartsWith("SearchFilter.", StringComparison.OrdinalIgnoreCase))
                        value.SearchFilters[key.Substring(13)] = raw == "1";
                }
            }
            catch
            {
                // A malformed optional preference must never prevent SAV recovery.
            }
            value.GamePath = NormalizeDirectoryPath(value.GamePath);
            value.LastDirectory = NormalizeDirectoryPath(value.LastDirectory);
            return value;
        }

        internal void Save()
        {
            GamePath = NormalizeDirectoryPath(GamePath);
            LastDirectory = NormalizeDirectoryPath(LastDirectory);
            string path = SettingsPath;
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            string encodedPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(GamePath ?? string.Empty));
            List<string> lines = new List<string>(new string[] {
                "Language=" + LanguageIndex,
                "FullLog=" + (FullLog ? "1" : "0"),
                "GamePathBase64=" + encodedPath,
                "LastDirectoryBase64=" + Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(LastDirectory ?? string.Empty)),
            });
            foreach (KeyValuePair<string, bool> pair in GalaxyFilters)
                lines.Add("GalaxyFilter." + pair.Key + "=" + (pair.Value ? "1" : "0"));
            foreach (KeyValuePair<string, bool> pair in SearchFilters)
                lines.Add("SearchFilter." + pair.Key + "=" + (pair.Value ? "1" : "0"));
            File.WriteAllLines(path, lines.ToArray(), Encoding.UTF8);
        }

        private static string DetectGamePath()
        {
            const string gameDirectoryName = "Space Rangers HD A War Apart";
            List<string> steamRoots = new List<string>();
            AddUniqueDirectory(steamRoots, Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"));
            AddSteamRegistryPath(steamRoots, Registry.CurrentUser,
                @"Software\Valve\Steam", "SteamPath");
            AddSteamRegistryPath(steamRoots, Registry.LocalMachine,
                @"Software\WOW6432Node\Valve\Steam", "InstallPath");

            List<string> libraryRoots = new List<string>(steamRoots);
            foreach (string steamRoot in steamRoots)
            {
                string libraries = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
                try
                {
                    if (!File.Exists(libraries)) continue;
                    foreach (string line in File.ReadAllLines(libraries))
                    {
                        string[] parts = line.Split('"');
                        if (parts.Length < 4) continue;
                        string key = parts[1].Trim();
                        int numericKey;
                        if (!key.Equals("path", StringComparison.OrdinalIgnoreCase) &&
                            !int.TryParse(key, out numericKey))
                            continue;
                        AddUniqueDirectory(libraryRoots, parts[3].Replace(@"\\", @"\"));
                    }
                }
                catch
                {
                    // Auto-detection is optional; users can choose the game folder in settings.
                }
            }

            foreach (string libraryRoot in libraryRoots)
            {
                string candidate = Path.Combine(libraryRoot, "steamapps", "common",
                    gameDirectoryName);
                if (Directory.Exists(candidate)) return NormalizeDirectoryPath(candidate);
            }
            return string.Empty;
        }

        internal static string NormalizeDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            string full;
            try { full = Path.GetFullPath(path.Trim()); }
            catch { return path.Trim(); }
            string root = Path.GetPathRoot(full);
            if (string.IsNullOrEmpty(root)) return full;
            if (root.Length >= 2 && root[1] == ':')
                root = char.ToUpperInvariant(root[0]) + root.Substring(1);
            string current = root;
            string remainder = full.Substring(Path.GetPathRoot(full).Length);
            string[] parts = remainder.Split(new char[] { Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string actual = part;
                try
                {
                    if (Directory.Exists(current))
                        foreach (string entry in Directory.GetFileSystemEntries(current))
                            if (string.Equals(Path.GetFileName(entry), part,
                                StringComparison.OrdinalIgnoreCase))
                            { actual = Path.GetFileName(entry); break; }
                }
                catch
                {
                    // Preserve the supplied component if a parent cannot be enumerated.
                }
                current = Path.Combine(current, actual);
            }
            return current;
        }

        private static void AddSteamRegistryPath(List<string> roots, RegistryKey hive,
            string subKeyName, string valueName)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(subKeyName))
                    AddUniqueDirectory(roots, key == null ? null : key.GetValue(valueName) as string);
            }
            catch
            {
                // Missing or inaccessible Steam registry data is a normal condition.
            }
        }

        private static void AddUniqueDirectory(List<string> values, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            string normalized;
            try { normalized = Path.GetFullPath(path.Trim()); }
            catch { return; }
            foreach (string value in values)
                if (string.Equals(value, normalized, StringComparison.OrdinalIgnoreCase)) return;
            values.Add(normalized);
        }
    }
}
