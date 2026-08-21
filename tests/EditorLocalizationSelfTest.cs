using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SpaceRangersHdSaveEditor;

internal static class EditorLocalizationSelfTest
{
    private static readonly Regex LatinWord = new Regex("[A-Za-z]{2,}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> Allowed = new HashSet<string>(
        StringComparer.OrdinalIgnoreCase)
    {
        "AB", "AI", "CRC", "DAT", "ID", "Infos", "K0", "K1", "K2", "K3",
        "K4", "K5", "K6", "Mod", "Mods", "RSON", "SCR", "SRHD", "Sub",
        "TItem", "Type", "URL", "English", "Lang", "dat", "Space", "Rangers",
        "HD", "Save", "Editor", "ModsCFG", "OK", "MS", "MP", "CE", "MC",
        "sqrt", "MR"
    };

    [STAThread]
    private static int Main()
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SortedSet<string> findings = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string resource in EditorFormDefinitions.AllResources)
            {
                using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get(resource)))
                {
                    Inspect(resource, form, findings);
                }
            }
            foreach (string finding in findings) Console.WriteLine(finding);
            Console.WriteLine("editor localization self-test: untranslated-visible={0}", findings.Count);
            return findings.Count == 0 ? 0 : 1;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void Inspect(string resource, Control root, ISet<string> findings)
    {
        string text = (root.Text ?? string.Empty).Trim();
        bool isCaption = root is Form || root is Label || root is GroupBox ||
            root is TabPage || root is ButtonBase;
        if (isCaption && text.Length != 0 && HasUntranslatedLatin(text))
            findings.Add(resource + "/" + root.Name + ": " + text);
        ComboBox combo = root as ComboBox;
        if (combo != null)
            foreach (object item in combo.Items)
            {
                string value = item == null ? string.Empty : item.ToString();
                if (HasUntranslatedLatin(value))
                    findings.Add(resource + "/" + root.Name + "[item]: " + value);
            }
        foreach (Control child in root.Controls) Inspect(resource, child, findings);
    }

    private static bool HasUntranslatedLatin(string text)
    {
        foreach (Match match in LatinWord.Matches(text))
            if (!Allowed.Contains(match.Value)) return true;
        return false;
    }
}
