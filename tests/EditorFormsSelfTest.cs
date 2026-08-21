using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SpaceRangersHdSaveEditor;

internal static class EditorFormsSelfTest
{
    [STAThread]
    private static int Main()
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (EditorFormDefinitions.AllResources.Length != 48)
                throw new InvalidOperationException("Expected 48 semantic forms.");
            VerifyGalaxyLabelColors();
            VerifyGalaxyMapGeometry();
            VerifyCanonicalPathCasing();
            VerifySpecializedLayouts();

            int nodes = 0;
            int compactForms = 0;
            int clippedLabels = 0;
            int phantomEditorCaptions = 0;
            int maximumBottomGap = 0;
            int maximumGroupGap = 0;
            foreach (string resource in EditorFormDefinitions.AllResources)
            {
                EditorFormDefinition definition = EditorFormDefinitions.Get(resource);
                if (definition == null) throw new InvalidOperationException("Missing form " + resource);
                nodes += definition.Nodes.Length;
                using (Form form = EditorFormFactory.Build(definition))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = new Point(-32000, -32000);
                    form.Show();
                    Application.DoEvents();
                    Dictionary<string, Control> controls = form.Tag as Dictionary<string, Control>;
                    if (controls == null) throw new InvalidOperationException("Missing control registry for " + resource);
                    foreach (EditorNodeDefinition node in definition.Nodes)
                        if (!controls.ContainsKey(node.Name))
                            throw new InvalidOperationException(resource + " is missing " + node.Name);
                    if (form.Text.IndexOf("SRHD Save Editor", StringComparison.OrdinalIgnoreCase) >= 0)
                        throw new InvalidOperationException("Old product name leaked into " + resource);
                    if (form.ClientSize.Width < 480 || form.ClientSize.Height < 110)
                        throw new InvalidOperationException("Layout is unusably small for " + resource);
                    if (definition.Nodes.Length <= 24 &&
                        resource != "TSTARMAPFORM" && resource != "TMAINFORM")
                    {
                        compactForms++;
                        if (form.ClientSize.Width > 720)
                            throw new InvalidOperationException("Small editor remained too wide: " + resource);
                    }
                    int contentBottom = VisibleContentBottom(form);
                    if (contentBottom > 0 && resource != "TSTARMAPFORM" && resource != "TMAINFORM" &&
                        resource != "TPATHDIALOGFORM")
                    {
                        int gap = Math.Max(0, form.ClientSize.Height - contentBottom);
                        maximumBottomGap = Math.Max(maximumBottomGap, gap);
                        if (gap > 72)
                            throw new InvalidOperationException(resource + " wastes " + gap +
                                " vertical pixels below its content.");
                    }
                    clippedLabels += CountClippedLabels(form);
                    phantomEditorCaptions += CountPhantomEditorCaptions(resource, form);
                    maximumGroupGap = Math.Max(maximumGroupGap, MaximumGroupBottomGap(form));
                    string wastedGroup = FindOversizedGroup(form, 32);
                    if (wastedGroup != null)
                        throw new InvalidOperationException(resource + " has an oversized group: " +
                            wastedGroup);
                    string overflow = FindHorizontalOverflow(form);
                    if (overflow != null)
                        throw new InvalidOperationException(resource + " has horizontal overflow: " + overflow);
                    form.Scale(new SizeF(1.5F, 1.5F));
                    Application.DoEvents();
                    overflow = FindHorizontalOverflow(form);
                    if (overflow != null)
                        throw new InvalidOperationException(resource +
                            " has horizontal overflow after 150% scaling: " + overflow);
                    form.Close();
                }
            }
            if (nodes != 1958)
                throw new InvalidOperationException("Expected 1958 semantic controls, got " + nodes + ".");
            if (clippedLabels != 0)
                throw new InvalidOperationException("Visible labels clipped: " + clippedLabels + ".");
            if (phantomEditorCaptions != 0)
                throw new InvalidOperationException("Empty editors contain derived captions: " +
                    phantomEditorCaptions + ".");
            Console.WriteLine("editor forms self-test: 48 forms, {0} semantic controls; " +
                "compact={1}; max-bottom-gap={2}; max-group-gap={3}; clipped-labels={4}; " +
                "phantom-editors={5}", nodes, compactForms, maximumBottomGap,
                maximumGroupGap, clippedLabels, phantomEditorCaptions);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static int VisibleContentBottom(Control container)
    {
        int bottom = 0;
        foreach (Control child in container.Controls)
            if (child.Visible && !child.Name.StartsWith("$", StringComparison.Ordinal))
                bottom = Math.Max(bottom, child.Bottom);
        return bottom;
    }

    private static int CountClippedLabels(Control root)
    {
        return CountClippedLabels(root, root.FindForm() as AdaptiveEditorForm ??
            root as AdaptiveEditorForm);
    }

    private static int CountClippedLabels(Control root, AdaptiveEditorForm form)
    {
        if (IsLayoutHidden(root, form)) return 0;
        int count = 0;
        foreach (Control child in root.Controls)
        {
            GroupBox group = child as GroupBox;
            if (group != null && group.Visible && !IsLayoutHidden(group, form) &&
                !string.IsNullOrWhiteSpace(group.Text))
            {
                Size measured = TextRenderer.MeasureText(group.Text.Trim(), group.Font, Size.Empty,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    TextFormatFlags.SingleLine);
                if (measured.Width > group.ClientSize.Width - 16)
                {
                    Console.WriteLine("clipped-group-title: {0}/{1} text={2} measured={3} width={4}",
                        root.FindForm() == null ? "?" : root.FindForm().Name,
                        group.Name, group.Text, measured.Width, group.ClientSize.Width - 16);
                    count++;
                }
            }
            Label label = child as Label;
            if (label != null && !IsLayoutHidden(label, form) && !string.IsNullOrWhiteSpace(label.Text))
            {
                bool multiline = label.Text.IndexOf('\n') >= 0 || label.ClientSize.Height >
                    label.Font.Height + 8;
                TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    (multiline ? TextFormatFlags.WordBreak : TextFormatFlags.SingleLine);
                Size proposed = multiline ? new Size(Math.Max(1, label.ClientSize.Width), 10000) : Size.Empty;
                Size measured = TextRenderer.MeasureText(label.Text, label.Font, proposed, flags);
                if (measured.Width > label.ClientSize.Width + 2 ||
                    multiline && measured.Height > label.ClientSize.Height + 2)
                {
                    Console.WriteLine("clipped-label: {0}/{1} text={2} measured={3} width={4}",
                        root.FindForm() == null ? "?" : root.FindForm().Name,
                        label.Name, label.Text, measured.Width, label.ClientSize.Width);
                    count++;
                }
            }
            ButtonBase button = child as ButtonBase;
            if (button != null && !IsLayoutHidden(button, form) && !string.IsNullOrWhiteSpace(button.Text) &&
                button.Text != "...")
            {
                int adornment = button is CheckBox || button is RadioButton ? 24 : 12;
                Size measured = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    TextFormatFlags.SingleLine);
                if (measured.Width + adornment > button.ClientSize.Width + 2)
                {
                    Console.WriteLine("clipped-button: {0}/{1} text={2} measured={3} width={4}",
                        root.FindForm() == null ? "?" : root.FindForm().Name,
                        button.Name, button.Text, measured.Width + adornment,
                        button.ClientSize.Width + " parent=" +
                        (button.Parent == null ? "?" : button.Parent.Name + "/" + button.Parent.ClientSize));
                    count++;
                }
            }
            count += CountClippedLabels(child, form);
        }
        return count;
    }

    private static bool IsLayoutHidden(Control control, AdaptiveEditorForm form)
    {
        if (control == null) return true;
        if (form == null || form.LayoutHidden == null) return false;
        Control current = control;
        while (current != null && !object.ReferenceEquals(current, form))
        {
            if (string.Equals(current.Name, "$hiddenTabPages", StringComparison.Ordinal)) return true;
            if (form.LayoutHidden.Contains(current)) return true;
            current = current.Parent;
        }
        return false;
    }

    private static int CountPhantomEditorCaptions(string resource, Control root)
    {
        int count = 0;
        foreach (Control child in root.Controls)
        {
            TextBox editor = child as TextBox;
            string storedCaption;
            if (editor != null && RussianCaptions.TryGet(resource, editor.Name,
                out storedCaption) && storedCaption.Length == 0 && editor.Text.Length != 0)
            {
                Console.WriteLine("phantom-editor-caption: {0}/{1} text={2}", resource,
                    editor.Name, editor.Text);
                count++;
            }
            count += CountPhantomEditorCaptions(resource, child);
        }
        return count;
    }

    private static int MaximumGroupBottomGap(Control root)
    {
        int maximum = 0;
        foreach (Control child in root.Controls)
        {
            GroupBox group = child as GroupBox;
            if (group != null && group.Visible)
            {
                int bottom = VisibleContentBottom(group);
                if (bottom > 0) maximum = Math.Max(maximum,
                    Math.Max(0, group.ClientSize.Height - bottom));
            }
            maximum = Math.Max(maximum, MaximumGroupBottomGap(child));
        }
        return maximum;
    }

    private static string FindOversizedGroup(Control root, int limit)
    {
        foreach (Control child in root.Controls)
        {
            GroupBox group = child as GroupBox;
            if (group != null && group.Visible)
            {
                int bottom = VisibleContentBottom(group);
                int gap = bottom == 0 ? 0 : Math.Max(0, group.ClientSize.Height - bottom);
                if (gap > limit) return group.Name + " gap=" + gap +
                    " client=" + group.ClientSize + " bottom=" + bottom;
            }
            string nested = FindOversizedGroup(child, limit);
            if (nested != null) return nested;
        }
        return null;
    }

    private static string FindHorizontalOverflow(Control root)
    {
        foreach (Control child in root.Controls)
        {
            if (!child.Visible || child.Name.StartsWith("$", StringComparison.Ordinal)) continue;
            if (child.Left < 0 || child.Right > root.ClientSize.Width + 2)
                return child.Name + " bounds=" + child.Bounds + " parent=" + root.Name +
                    " client=" + root.ClientSize + " scroll=" + root.AutoScrollOffset;
            string nested = FindHorizontalOverflow(child);
            if (nested != null) return nested;
        }
        return null;
    }

    private static string FindVerticalOverflow(Control root)
    {
        foreach (Control child in root.Controls)
        {
            if (!child.Visible || child.Name == "$hiddenTabPages") continue;
            ScrollableControl scrollable = root as ScrollableControl;
            if ((scrollable == null || !scrollable.AutoScroll) &&
                (child.Top < 0 || child.Bottom > root.ClientSize.Height + 2))
                return child.Name + " bounds=" + child.Bounds + " parent=" + root.Name +
                    " client=" + root.ClientSize;
            string nested = FindVerticalOverflow(child);
            if (nested != null) return nested;
        }
        return null;
    }

    private static void AssertPageContained(Control page, string context)
    {
        string overflow = FindVerticalOverflow(page);
        if (overflow != null)
            throw new InvalidOperationException(context + " has vertical overflow: " + overflow);
        overflow = FindHorizontalOverflow(page);
        if (overflow != null)
            throw new InvalidOperationException(context + " has horizontal overflow: " + overflow);
    }

    private static void VerifyGalaxyLabelColors()
    {
        List<GalaxyLabelSegment> split = MainForm.BuildGalaxyLabelSegments(
            "Альтаир", string.Empty, new byte[] { 2, 4, 2 }, null);
        if (split.Count != 2 || split[0].Text != "Аль" || split[1].Text != "таир" ||
            split[0].Color != Color.FromArgb(0x60, 0xAA, 0xFF) ||
            split[1].Color != Color.FromArgb(0xF0, 0xE0, 0x50))
            throw new InvalidOperationException("Multi-race galaxy label segmentation is incompatible.");
        if (MainForm.GalaxyOwnerColor(0) != Color.FromArgb(0xFF, 0x60, 0x60) ||
            MainForm.GalaxyOwnerColor(1) != Color.FromArgb(0x60, 0xE0, 0x60) ||
            MainForm.GalaxyOwnerColor(3) != Color.FromArgb(0xFF, 0xB0, 0xF0) ||
            MainForm.GalaxyOwnerColor(5) != Color.FromArgb(0x80, 0xC0, 0xD0) ||
            MainForm.GalaxyOwnerColor(7) != Color.White)
            throw new InvalidOperationException("Galaxy owner colors are incompatible.");
    }

    private static void VerifyGalaxyMapGeometry()
    {
        GalaxyMapLine regular = new GalaxyMapLine();
        GalaxyMapLine hidden = new GalaxyMapLine();
        ConstellationRecord ordinary = new ConstellationRecord();
        ordinary.BoundaryLines.Add(regular);
        ordinary.HiddenBoundaryLines.Add(hidden);
        if (!object.ReferenceEquals(MainForm.GalaxySectorBoundaryLines(ordinary),
                ordinary.BoundaryLines))
            throw new InvalidOperationException("ordinary sector used its hidden contour");

        ConstellationRecord hiddenPirate = new ConstellationRecord();
        hiddenPirate.HiddenBoundaryLines.Add(hidden);
        if (!object.ReferenceEquals(MainForm.GalaxySectorBoundaryLines(hiddenPirate),
                hiddenPirate.HiddenBoundaryLines))
            throw new InvalidOperationException("hidden pirate sector lost its only boundary");
        hiddenPirate.StarObjectIds.Add(20U);
        List<StarHeaderRecord> visibleStars = new List<StarHeaderRecord> {
            new StarHeaderRecord { ObjectId = 20U, X = 50F, Y = 40F }
        };
        if (!MainForm.GalaxySectorHasStarInsideBounds(hiddenPirate, visibleStars,
                0F, 0F, 100F, 100F))
            throw new InvalidOperationException("in-bounds hidden-sector star was rejected");
        visibleStars[0].X = 500F;
        if (MainForm.GalaxySectorHasStarInsideBounds(hiddenPirate, visibleStars,
                0F, 0F, 100F, 100F))
            throw new InvalidOperationException("off-map hidden sector can split a visible sector");
        if (MainForm.GalaxySystemLinkColor != Color.FromArgb(0x77, 0x77, 0x77))
            throw new InvalidOperationException("galaxy system-link color differs from the reference map");

        List<StarHeaderRecord> largeModdedGalaxy = new List<StarHeaderRecord>();
        for (int index = 0; index < 198; index++)
            largeModdedGalaxy.Add(new StarHeaderRecord {
                ObjectId = (uint)(index + 1), X = 9F + index * (262F / 197F),
                Y = 5F + (index % 80) * (186F / 79F)
            });
        largeModdedGalaxy.Add(new StarHeaderRecord { ObjectId = 199U, X = 500F, Y = 178F });
        largeModdedGalaxy.Add(new StarHeaderRecord { ObjectId = 200U, X = 500F, Y = 185F });
        float minX, minY, maxX, maxY;
        MainForm.CalculateGalaxyMapBounds(largeModdedGalaxy,
            out minX, out minY, out maxX, out maxY);
        if (minX > 10F || maxX < 270F || maxX >= 400F)
            throw new InvalidOperationException(
                "disabled pirate systems still collapse the large modded galaxy map");

        PointF ordinaryPoint = new PointF(12F, -20F);
        if (MainForm.StarMapDisplayWorldPoint(new object(), ordinaryPoint) != ordinaryPoint)
            throw new InvalidOperationException("ordinary system objects changed orientation");
        PointF jumpPoint = MainForm.StarMapDisplayWorldPoint(
            new SystemJumpPointRecord(), ordinaryPoint);
        if (jumpPoint.X != 12F || jumpPoint.Y != 20F)
            throw new InvalidOperationException("jump marker did not adopt galaxy-map Y direction");
    }

    private static void VerifyCanonicalPathCasing()
    {
        string windows = AppSettings.NormalizeDirectoryPath(@"c:\windows");
        if (System.IO.Directory.Exists(@"C:\Windows") && windows != @"C:\Windows")
            throw new InvalidOperationException("Directory path casing was not canonicalized: " + windows);
        string game = @"D:\Steam\steamapps\common\Space Rangers HD A War Apart";
        if (System.IO.Directory.Exists(game))
        {
            string normalized = AppSettings.NormalizeDirectoryPath(
                @"d:\steam\steamapps\common\space rangers hd a war apart");
            if (normalized != game)
                throw new InvalidOperationException("Game path casing was not canonicalized: " + normalized);
        }
    }

    private static void VerifySpecializedLayouts()
    {
        using (Form galaxy = EditorFormFactory.Build(EditorFormDefinitions.Get("TGALAXYFORM")))
        {
            Dictionary<string, Control> controls = (Dictionary<string, Control>)galaxy.Tag;
            galaxy.StartPosition = FormStartPosition.Manual;
            galaxy.Location = new Point(-32000, -32000);
            galaxy.Show();
            Application.DoEvents();
            // Hosted runners can expose a 1024 px working area.  The dialog must respect
            // that limit while retaining the intended three distinct columns.
            if (galaxy.ClientSize.Width < 900 || controls["gbDifficulty"].Left == controls["gbResearch"].Left ||
                controls["gbResearch"].Left == controls["gbPlanetNews"].Left)
                throw new InvalidOperationException("Galaxy editor did not use its wide three-column layout.");
            TabControl galaxyOuter = (TabControl)controls["pcGalaxy"];
            foreach (string outerName in new string[] { "tsMain", "tsCustomRules" })
            {
                galaxyOuter.SelectedTab = (TabPage)controls[outerName];
                Application.DoEvents();
                AssertPageContained(controls[outerName], "TGALAXYFORM/" + outerName);
            }
            foreach (string ruleName in new string[] { "$rulesBalance",
                "$rulesGalaxy", "$rulesOther", "$rulesFlags" })
            {
                AssertPageContained(controls[ruleName], "TGALAXYFORM/" + ruleName);
            }
        }
        using (Form achievements = EditorFormFactory.Build(
            EditorFormDefinitions.Get("TACHIEVEMENTSFORM")))
        {
            Dictionary<string, Control> controls = (Dictionary<string, Control>)achievements.Tag;
            if (controls["gbAchAlreadyReceived"].Left <= controls["edAsteroidsDestroyed"].Right ||
                achievements.ClientSize.Height > 500)
                throw new InvalidOperationException("Achievements editor was not split into two compact columns.");
        }
        using (Form shipForm = EditorFormFactory.Build(EditorFormDefinitions.Get("TSHIPFORM")))
        {
            ShipHeaderRecord ship = new ShipHeaderRecord { Type = 1, IsPlayer = true,
                HasCommonTail = true, HasNormalShipTail = true, HasRangerTail = true,
                HasPreCommonCollections = true };
            System.Reflection.MethodInfo configure = typeof(MainForm).GetMethod("ConfigureShipPages",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            configure.Invoke(null, new object[] { shipForm, ship });
            Dictionary<string, Control> controls = (Dictionary<string, Control>)shipForm.Tag;
            TabControl pages = (TabControl)controls["pcParams"];
            shipForm.StartPosition = FormStartPosition.Manual;
            shipForm.Location = new Point(-32000, -32000);
            shipForm.Show();
            // Match the compact client width produced by FitDialogToWorkingArea on the
            // 1024-wide Windows runner. Specialized pages must relayout after shrinking.
            shipForm.ClientSize = new Size(959, Math.Min(720, shipForm.ClientSize.Height));
            EditorFormFactory.Relayout(shipForm);
            Application.DoEvents();
            pages.SelectedTab = (TabPage)controls["tsSubType"];
            Application.DoEvents();
            Panel playerSections = controls.ContainsKey("$playerSections") ?
                controls["$playerSections"] as Panel : null;
            Panel playerContent = controls.ContainsKey("$playerContent") ?
                controls["$playerContent"] as Panel : null;
            if (!pages.TabPages.Contains((TabPage)controls["tsSubType"]) ||
                !pages.TabPages.Contains((TabPage)controls["tsPlayer"]) ||
                playerSections == null || playerContent == null || playerContent.Controls.Count != 4 ||
                controls["gbPlayerShip"].Parent != null ||
                !controls["gbNormalShip"].Visible || !controls["gbRangerShip"].Visible ||
                controls["gbWarriorShip"].Visible || controls["gbTransportShip"].Visible ||
                controls["gbPirateShip"].Visible || controls["gbDominatorShip"].Visible ||
                controls["gbNormalShip"].Bounds.IntersectsWith(controls["gbRangerShip"].Bounds))
                throw new InvalidOperationException("Dynamic player subtype pages overlap or expose wrong groups.");
            if (!(shipForm is AdaptiveEditorForm) || !(controls["tsSubType"] is BufferedTabPage))
                throw new InvalidOperationException("Editor buffering is not active for dynamic pages.");
            TabControl shipPages = (TabControl)controls["pcShip"];
            foreach (string outerName in new string[] { "tsParams", "tsHold", "tsMods" })
            {
                shipPages.SelectedTab = (TabPage)controls[outerName];
                Application.DoEvents();
                AssertPageContained(controls[outerName], "TSHIPFORM/" + outerName);
            }
            shipPages.SelectedTab = (TabPage)controls["tsParams"];
            foreach (TabPage parameterPage in pages.TabPages)
            {
                pages.SelectedTab = parameterPage;
                Application.DoEvents();
                AssertPageContained(parameterPage, "TSHIPFORM/" + parameterPage.Name);
                if (CountClippedLabels(parameterPage) != 0)
                    throw new InvalidOperationException("TSHIPFORM/" + parameterPage.Name +
                        " contains a clipped dynamic caption.");
            }
            pages.SelectedTab = (TabPage)controls["tsPlayer"];
            FlowLayoutPanel playerNavigation = (FlowLayoutPanel)controls["$playerNavigation"];
            foreach (Button sectionButton in playerNavigation.Controls)
            {
                sectionButton.PerformClick();
                Application.DoEvents();
                Panel playerPage = (Panel)sectionButton.Tag;
                AssertPageContained(playerPage, "TSHIPFORM/player/" + playerPage.Name);
                if (CountClippedLabels(playerPage) != 0)
                    throw new InvalidOperationException("TSHIPFORM/player/" + playerPage.Name +
                        " contains a clipped dynamic caption.");
            }
        }
    }
}
