using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SpaceRangersHdSaveEditor;

internal static class GitHubScreenshotGenerator
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("usage: github-screenshot <ru|en> save.sav output.png");
            return 2;
        }

        bool english;
        if (string.Equals(args[0], "en", StringComparison.OrdinalIgnoreCase)) english = true;
        else if (string.Equals(args[0], "ru", StringComparison.OrdinalIgnoreCase)) english = false;
        else throw new ArgumentException("Language must be ru or en.");

        string savePath = Path.GetFullPath(args[1]);
        string output = Path.GetFullPath(args[2]);
        if (!File.Exists(savePath)) throw new FileNotFoundException("SAV fixture not found.", savePath);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using (MainForm form = new MainForm(english ? 1 : 0))
        {
            form.OpenAtStartup(savePath, true);
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(32, 32);
            form.Show();
            WaitForSave(form);

            TabControl tabs = (TabControl)typeof(MainForm).GetField("mainTabs", PrivateInstance).GetValue(form);
            tabs.SelectedIndex = 0;
            Application.DoEvents();
            Thread.Sleep(120);

            AssertCaption(form, english ? "Preview" : "Превью");
            AssertCaption(form, english ? "Player name:" : "Имя игрока:");
            AssertCaption(form, english ? "Save log" : "Сохранить лог");
            if (english)
            {
                GameDataCatalog catalog = (GameDataCatalog)typeof(MainForm)
                    .GetField("gameCatalog", PrivateInstance).GetValue(form);
                if (!string.Equals(catalog.GetStarName("Солнце"), "Sol", StringComparison.Ordinal))
                    throw new InvalidOperationException("The English game catalog did not localize Sol.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(output));
            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(output, ImageFormat.Png);
            }
            Console.WriteLine("GitHub screenshot: {0} ({1}x{2})", output, form.Width, form.Height);
        }
        return 0;
    }

    private static void WaitForSave(MainForm form)
    {
        FieldInfo currentField = typeof(MainForm).GetField("current", PrivateInstance);
        FieldInfo loadingField = typeof(MainForm).GetField("isLoading", PrivateInstance);
        DateTime deadline = DateTime.UtcNow.AddSeconds(60);
        while (DateTime.UtcNow < deadline)
        {
            Application.DoEvents();
            Thread.Sleep(20);
            if (!(bool)loadingField.GetValue(form) && currentField.GetValue(form) != null) return;
        }
        throw new InvalidOperationException("The save did not finish loading.");
    }

    private static void AssertCaption(Control root, string expected)
    {
        if (!ContainsCaption(root, expected))
            throw new InvalidOperationException("Missing localized caption: " + expected);
    }

    private static bool ContainsCaption(Control root, string expected)
    {
        if (string.Equals(root.Text, expected, StringComparison.Ordinal)) return true;
        foreach (Control child in root.Controls)
            if (ContainsCaption(child, expected)) return true;
        return false;
    }
}
