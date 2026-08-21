using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SpaceRangersHdSaveEditor;

internal static class EditorDialogsVisualSmokeTest
{
    private static readonly string[] RepresentativeForms = {
        "TACHIEVEMENTSFORM", "TGALAXYFORM", "TPLANETFORM", "TSHIPFORM",
        "TITEMFORM", "TSCRIPTFORM", "TSTORAGEITEMFORM", "TSTARFORM",
        "TMISSILEFORM", "TSETTINGSFORM", "TCUSTOMWEAPONINFOFORM", "TMESSAGEFORM",
        "TMODSLISTFORM"
    };

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("usage: editor-dialogs-visual-smoke output.png");
            return 2;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        const int tileWidth = 480;
        const int tileHeight = 340;
        const int columns = 3;
        int rows = (RepresentativeForms.Length + columns - 1) / columns;
        using (Bitmap sheetBitmap = new Bitmap(tileWidth * columns, tileHeight * rows))
        using (Graphics sheet = Graphics.FromImage(sheetBitmap))
        {
            sheet.Clear(Color.White);
            for (int index = 0; index < RepresentativeForms.Length; index++)
            {
                EditorFormDefinition definition = EditorFormDefinitions.Get(RepresentativeForms[index]);
                using (Form form = EditorFormFactory.Build(definition))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(10, 10);
                    form.Show();
                    Application.DoEvents();
                    Thread.Sleep(10);
                    if (form.ClientSize.Width < 480 || form.ClientSize.Height < 110)
                        throw new InvalidOperationException("unusably small editor dialog: " + definition.Resource);
                    using (Bitmap dialog = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(dialog, new Rectangle(Point.Empty, dialog.Size));
                        string individual = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[0])),
                            "dialog-" + definition.Resource + ".png");
                        dialog.Save(individual, System.Drawing.Imaging.ImageFormat.Png);
                        float fit = Math.Min((float)(tileWidth - 8) / dialog.Width,
                            (float)(tileHeight - 8) / dialog.Height);
                        int fittedWidth = Math.Max(1, (int)Math.Round(dialog.Width * fit));
                        int fittedHeight = Math.Max(1, (int)Math.Round(dialog.Height * fit));
                        int tileX = (index % columns) * tileWidth;
                        int tileY = (index / columns) * tileHeight;
                        Rectangle target = new Rectangle(tileX + (tileWidth - fittedWidth) / 2,
                            tileY + (tileHeight - fittedHeight) / 2, fittedWidth, fittedHeight);
                        sheet.DrawImage(dialog, target);
                        sheet.DrawRectangle(Pens.DimGray, tileX, tileY,
                            tileWidth - 1, tileHeight - 1);
                    }
                    SaveScrolledVariant(form, definition.Resource,
                        Path.GetDirectoryName(Path.GetFullPath(args[0])));
                }
            }
            string output = Path.GetFullPath(args[0]);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            sheetBitmap.Save(output, System.Drawing.Imaging.ImageFormat.Png);
        }
        SaveDynamicPlayerVariants(Path.GetDirectoryName(Path.GetFullPath(args[0])));
        SaveGalaxyVariants(Path.GetDirectoryName(Path.GetFullPath(args[0])));
        SaveFormattedNewsVariant(Path.GetDirectoryName(Path.GetFullPath(args[0])));
        Console.WriteLine("editor-dialog visual smoke: {0} representative forms", RepresentativeForms.Length);
        return 0;
    }

    private static void SaveDynamicPlayerVariants(string directory)
    {
        using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSHIPFORM")))
        {
            ShipHeaderRecord ship = new ShipHeaderRecord { Type = 1, IsPlayer = true,
                HasCommonTail = true, HasNormalShipTail = true, HasRangerTail = true,
                HasPreCommonCollections = true };
            System.Reflection.MethodInfo configure = typeof(MainForm).GetMethod("ConfigureShipPages",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            configure.Invoke(null, new object[] { form, ship });
            var controls = (System.Collections.Generic.Dictionary<string, Control>)form.Tag;
            TabControl pages = (TabControl)controls["pcParams"];
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-32000, -32000);
            form.Show();
            TabControl shipPages = (TabControl)controls["pcShip"];
            foreach (string pageName in new string[] { "tsParams", "tsHold", "tsMods" })
            {
                shipPages.SelectedTab = (TabPage)controls[pageName];
                Application.DoEvents();
                SaveFormBitmap(form, Path.Combine(directory, "dialog-TSHIPFORM-outer-" + pageName + ".png"));
            }
            shipPages.SelectedTab = (TabPage)controls["tsParams"];
            TabPage additionalPage = (TabPage)controls["tsAdditional"];
            pages.SelectedTab = additionalPage;
            Application.DoEvents();
            SaveFormBitmap(form, Path.Combine(directory,
                "dialog-TSHIPFORM-additional.png"));
            TabPage subtypePage = (TabPage)controls["tsSubType"];
            pages.SelectedTab = subtypePage;
            Application.DoEvents();
            SaveFormBitmap(form, Path.Combine(directory, "dialog-TSHIPFORM-subtype.png"));
            SaveFormScrolledVariant(form, "TSHIPFORM-subtype", directory);
            TabPage playerPage = (TabPage)controls["tsPlayer"];
            pages.SelectedTab = playerPage;
            Application.DoEvents();
            SaveFormBitmap(form, Path.Combine(directory, "dialog-TSHIPFORM-player.png"));
            SaveFormScrolledVariant(form, "TSHIPFORM-player", directory);
            FlowLayoutPanel playerNavigation = (FlowLayoutPanel)controls["$playerNavigation"];
            foreach (Button sectionButton in playerNavigation.Controls)
            {
                sectionButton.PerformClick();
                form.AutoScrollPosition = Point.Empty;
                Application.DoEvents();
                Panel section = (Panel)sectionButton.Tag;
                SaveFormBitmap(form, Path.Combine(directory, "dialog-TSHIPFORM-player-" +
                    section.Name.TrimStart('$') + ".png"));
            }
            form.Close();
        }
    }

    private static void SaveFormattedNewsVariant(string directory)
    {
        using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TPLANETNEWSFORM")))
        {
            var controls = (System.Collections.Generic.Dictionary<string, Control>)form.Tag;
            TextBox source = (TextBox)controls["mmNewsText"];
            CheckBox legacy = (CheckBox)controls["chbHideTags"];
            const string raw = "<color=255,240,100>Планета</color><td=180>Новости<br>галактики";
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic;
            System.Reflection.MethodInfo configure = typeof(MainForm).GetMethod(
                "ConfigureFormattedGameTextEditor", flags);
            Func<string> readRaw = (Func<string>)configure.Invoke(null,
                new object[] { form, source, legacy, raw });
            ComboBox mode = (ComboBox)controls["$gameTextMode"];
            RichTextBox preview = (RichTextBox)controls["$formattedNewsText"];
            if (mode.SelectedIndex != 0 || !preview.ReadOnly || preview.Text.IndexOf("<color",
                StringComparison.OrdinalIgnoreCase) >= 0 || source.Visible || legacy.Visible)
                throw new InvalidOperationException("formatted planet-news mode is not the default");
            mode.SelectedIndex = 1;
            if (!preview.ReadOnly || preview.Text.IndexOf('<') >= 0)
                throw new InvalidOperationException("plain planet-news mode still exposes tags");
            mode.SelectedIndex = 2;
            preview.Text = raw + "!";
            if (preview.ReadOnly || readRaw() != raw + "!")
                throw new InvalidOperationException("raw planet-news mode does not preserve edits");
            mode.SelectedIndex = 0;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-32000, -32000);
            form.Show();
            Application.DoEvents();
            SaveFormBitmap(form, Path.Combine(directory, "dialog-TPLANETNEWSFORM-formatted.png"));
            form.Close();
        }
    }

    private static void SaveGalaxyVariants(string directory)
    {
        using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TGALAXYFORM")))
        {
            var controls = (System.Collections.Generic.Dictionary<string, Control>)form.Tag;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-32000, -32000);
            form.Show();
            TabControl outer = (TabControl)controls["pcGalaxy"];
            outer.SelectedTab = (TabPage)controls["tsCustomRules"];
            Application.DoEvents();
            SaveFormBitmap(form, Path.Combine(directory, "dialog-TGALAXYFORM-custom-rules.png"));
            form.Close();
        }
    }

    private static void SaveScrolledVariant(Form form, string resource, string directory)
    {
        ScrollableControl scroll = FindScrollablePage(form);
        if (scroll == null) return;
        int target = Math.Min(420, Math.Max(0,
            scroll.VerticalScroll.Maximum - scroll.VerticalScroll.LargeChange + 1));
        if (target <= 0) return;
        scroll.AutoScrollPosition = new Point(0, target);
        scroll.PerformLayout();
        scroll.Invalidate(true);
        form.Invalidate(true);
        Application.DoEvents();
        SaveFormBitmap(form, Path.Combine(directory, "dialog-" + resource + "-scrolled.png"));
        scroll.AutoScrollPosition = Point.Empty;
        Application.DoEvents();
    }

    private static void SaveFormScrolledVariant(Form form,
        string resource, string directory)
    {
        form.AutoScrollPosition = new Point(0, 420);
        form.PerformLayout();
        form.Invalidate(true);
        Application.DoEvents();
        SaveFormBitmap(form, Path.Combine(directory, "dialog-" + resource + "-scrolled.png"));
        form.AutoScrollPosition = Point.Empty;
        Application.DoEvents();
    }

    private static ScrollableControl FindScrollablePage(Control root)
    {
        TabControl tabs = root as TabControl;
        if (tabs != null && tabs.SelectedTab != null && tabs.SelectedTab.AutoScroll &&
            tabs.SelectedTab.DisplayRectangle.Height > tabs.SelectedTab.ClientSize.Height)
            return tabs.SelectedTab;
        foreach (Control child in root.Controls)
        {
            ScrollableControl result = FindScrollablePage(child);
            if (result != null) return result;
        }
        return null;
    }

    private static void SaveFormBitmap(Form form, string path)
    {
        using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
