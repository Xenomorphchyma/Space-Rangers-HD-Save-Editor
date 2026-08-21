using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SpaceRangersHdSaveEditor;

internal static class MainWindowVisualSmokeTest
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("usage: visual-smoke save.sav screenshot.png");
            return 2;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Console.WriteLine("visual stage: construct");
        using (MainForm form = new MainForm())
        {
            Console.WriteLine("visual stage: configure startup");
            form.OpenAtStartup(args[0], true);
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(20, 20);
            Console.WriteLine("visual stage: show/load");
            form.Show();
            Console.WriteLine("visual stage: shown visible={0} handle={1} disposed={2}",
                form.Visible, form.IsHandleCreated, form.IsDisposed);
            DateTime loadDeadline = DateTime.UtcNow.AddSeconds(45);
            const System.Reflection.BindingFlags privateInstance =
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.FieldInfo currentField = typeof(MainForm).GetField("current", privateInstance);
            System.Reflection.FieldInfo loadingField = typeof(MainForm).GetField("isLoading", privateInstance);
            bool sawLoadingOverlay = false;
            Panel loadingOverlay = (Panel)typeof(MainForm).GetField("loadingOverlay",
                privateInstance).GetValue(form);
            while (DateTime.UtcNow < loadDeadline)
            {
                Application.DoEvents();
                Thread.Sleep(20);
                bool loading = (bool)loadingField.GetValue(form);
                sawLoadingOverlay |= loadingOverlay.Visible;
                if (!loading && currentField.GetValue(form) != null) break;
            }
            if ((bool)loadingField.GetValue(form) || currentField.GetValue(form) == null)
                throw new InvalidOperationException("asynchronous startup load did not complete");
            if (!sawLoadingOverlay)
                throw new InvalidOperationException("startup load did not expose its progress UI");

            if (!form.Text.StartsWith("Space Rangers HD Save Editor", StringComparison.Ordinal))
                throw new InvalidOperationException("main window branding was not applied");
            if (form.ClientSize.Width < 1000 || form.ClientSize.Height < 620)
                throw new InvalidOperationException("main window is unexpectedly compressed");

            TabControl tabs = FindFirst<TabControl>(form);
            if (tabs == null || tabs.TabPages.Count < 9)
                throw new InvalidOperationException("main navigation was not created");
            ListBox constellationList = (ListBox)typeof(MainForm).GetField("constellationList",
                privateInstance).GetValue(form);
            if (constellationList == null || constellationList.Items.Count < 2)
                throw new InvalidOperationException("sector list was not populated");
            string sectorCaption = constellationList.GetItemText(constellationList.Items[1]);
            if (sectorCaption.StartsWith("ID:", StringComparison.OrdinalIgnoreCase) ||
                sectorCaption.IndexOf("[ID ", StringComparison.Ordinal) < 0)
                throw new InvalidOperationException("sector list caption has no localized name: " +
                    sectorCaption);
            var constellations = (System.Collections.Generic.List<ConstellationRecord>)
                typeof(MainForm).GetField("pendingConstellations", privateInstance).GetValue(form);
            bool hiddenOnlyBoundaryVerified = false;
            foreach (ConstellationRecord sector in constellations)
                if ((sector.BoundaryLines == null || sector.BoundaryLines.Count == 0) &&
                    sector.HiddenBoundaryLines != null && sector.HiddenBoundaryLines.Count > 0)
                {
                    hiddenOnlyBoundaryVerified = object.ReferenceEquals(
                        MainForm.GalaxySectorBoundaryLines(sector), sector.HiddenBoundaryLines);
                    break;
                }
            if (!hiddenOnlyBoundaryVerified)
                throw new InvalidOperationException("hidden pirate sector boundary was not restored");
            string output = Path.GetFullPath(args[1]);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            int thumbWidth = Math.Max(1, form.Width / 2);
            int thumbHeight = Math.Max(1, form.Height / 2);
            int columns = 2;
            int rows = (tabs.TabPages.Count + columns - 1) / columns;
            using (Bitmap contactSheet = new Bitmap(thumbWidth * columns, thumbHeight * rows))
            using (Graphics sheet = Graphics.FromImage(contactSheet))
            {
                sheet.Clear(Color.White);
                for (int index = 0; index < tabs.TabPages.Count; index++)
                {
                    TabPage page = tabs.TabPages[index];
                    Console.WriteLine("visual stage: tab {0}/{1} {2}", index + 1,
                        tabs.TabPages.Count, page.Name);
                    tabs.SelectedTab = page;
                    Application.DoEvents();
                    if (page.DisplayRectangle.Width <= 0 || page.DisplayRectangle.Height <= 0)
                        throw new InvalidOperationException("empty navigation page: " + page.Name);
                    using (Bitmap pageBitmap = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(pageBitmap, new Rectangle(Point.Empty, pageBitmap.Size));
                        pageBitmap.Save(Path.Combine(Path.GetDirectoryName(output),
                            "main-tab-" + index.ToString("00") + ".png"),
                            System.Drawing.Imaging.ImageFormat.Png);
                        Rectangle target = new Rectangle((index % columns) * thumbWidth,
                            (index / columns) * thumbHeight, thumbWidth, thumbHeight);
                        sheet.DrawImage(pageBitmap, target);
                        sheet.DrawRectangle(Pens.DimGray, target.X, target.Y,
                            target.Width - 1, target.Height - 1);
                    }
                }
                Console.WriteLine("visual stage: save contact sheet");
                contactSheet.Save(output, System.Drawing.Imaging.ImageFormat.Png);
            }
            VerifyPlayerSubtypeEditors(form, Path.GetDirectoryName(output));
            VerifyGalaxyMapClick(form, tabs);
            tabs.SelectedIndex = 0;
            Application.DoEvents();
            Console.WriteLine("main-window visual smoke: {0} tabs, {1}x{2}",
                tabs.TabPages.Count, form.ClientSize.Width, form.ClientSize.Height);
        }
        return 0;
    }

    private static void VerifyPlayerSubtypeEditors(MainForm owner, string directory)
    {
        const System.Reflection.BindingFlags flags =
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var ships = (System.Collections.Generic.List<ShipHeaderRecord>)typeof(MainForm)
            .GetField("pendingShips", flags).GetValue(owner);
        ShipHeaderRecord player = null;
        foreach (ShipHeaderRecord ship in ships)
            if ((ship.IsPlayer || ship.HasPlayerPrefix) && ship.HasNormalShipTail && ship.HasRangerTail)
            { player = ship; break; }
        if (player == null) throw new InvalidOperationException("player subtype fixture not found");

        Exception inspectionError = null;
        bool inspected = false;
        using (System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer())
        {
            timer.Interval = 100;
            timer.Tick += delegate
            {
                if (inspected) return;
                Form dialog = null;
                foreach (Form candidate in Application.OpenForms)
                    if (candidate.Name == "TSHIPFORM") { dialog = candidate; break; }
                if (dialog == null) return;
                inspected = true;
                try
                {
                    var controls = (System.Collections.Generic.Dictionary<string, Control>)dialog.Tag;
                    string[] editableNames = { "edKillAllShips", "edKillPirates", "edRankPoints",
                        "edStatusTrader", "edStatusWarrior", "edRangerPrison", "edNods" };
                    foreach (string name in editableNames)
                    {
                        TextBox editor = (TextBox)controls[name];
                        if (!editor.Enabled || editor.ReadOnly || editor.BackColor != SystemColors.Window)
                            throw new InvalidOperationException("subtype field is not editable: " + name);
                    }
                    ComboBox shipType = (ComboBox)controls["cbType"];
                    if (shipType.Enabled || string.IsNullOrEmpty(shipType.AccessibleDescription) ||
                        shipType.AccessibleDescription.IndexOf("TPlayer", StringComparison.Ordinal) < 0)
                        throw new InvalidOperationException("immutable player type has no clear SAV explanation");
                    DataGridView programs = (DataGridView)controls["sgProgramms"];
                    if (programs.ReadOnly || programs.Columns.Count < 2 || programs.Columns[1].ReadOnly)
                        throw new InvalidOperationException("ranger program values are not editable");
                    foreach (string groupName in new string[] { "gbTakeItems", "gbRelationToRangers" })
                    {
                        GroupBox group = (GroupBox)controls[groupName];
                        int titleWidth = TextRenderer.MeasureText(group.Text, group.Font).Width;
                        if (titleWidth > group.ClientSize.Width - 12)
                            throw new InvalidOperationException("group title is clipped: " + group.Text);
                    }
                    TabControl pages = (TabControl)controls["pcParams"];
                    pages.SelectedTab = (TabPage)controls["tsSubType"];
                    Application.DoEvents();
                    using (Bitmap bitmap = new Bitmap(dialog.Width, dialog.Height))
                    {
                        dialog.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                        bitmap.Save(Path.Combine(directory, "dialog-player-subtype-live.png"),
                            System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                catch (Exception error) { inspectionError = error; }
                finally { dialog.Close(); }
            };
            timer.Start();
            typeof(MainForm).GetMethod("EditShip", flags).Invoke(owner,
                new object[] { player, owner });
            timer.Stop();
        }
        if (!inspected) throw new InvalidOperationException("player editor did not open");
        if (inspectionError != null) throw inspectionError;
        Console.WriteLine("player subtype editability smoke: writable fields and programs OK");
    }

    private static void VerifyGalaxyMapClick(MainForm form, TabControl tabs)
    {
        const System.Reflection.BindingFlags flags =
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        PictureBox map = (PictureBox)typeof(MainForm).GetField("galaxyMapImage", flags).GetValue(form);
        System.Collections.Generic.List<StarMapHitRecord> hits =
            (System.Collections.Generic.List<StarMapHitRecord>)typeof(MainForm)
                .GetField("galaxyMapHits", flags).GetValue(form);
        TabPage galaxyPage = (TabPage)typeof(MainForm).GetField("galaxyPage", flags).GetValue(form);
        if (map == null || hits == null || hits.Count == 0 || galaxyPage == null)
            throw new InvalidOperationException("galaxy map click test has no target");
        tabs.SelectedIndex = 0;
        Application.DoEvents();
        map.Refresh();

        PointF originalHitPoint = hits[0].Point;
        RaiseMouse(map, "OnMouseWheel", new MouseEventArgs(MouseButtons.None, 0,
            map.ClientSize.Width / 3, map.ClientSize.Height / 3, 120));
        Application.DoEvents();
        float galaxyZoom = (float)typeof(MainForm).GetField("galaxyMapZoom", flags).GetValue(form);
        if (galaxyZoom <= 1F || hits[0].Point == originalHitPoint)
            throw new InvalidOperationException("galaxy map wheel did not zoom around the cursor");
        Image imageBeforeDrag = map.Image;
        RaiseMouse(map, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 5, 5, 0));
        RaiseMouse(map, "OnMouseMove", new MouseEventArgs(MouseButtons.Left, 0, 25, 20, 0));
        if (!object.ReferenceEquals(map.Image, imageBeforeDrag))
            throw new InvalidOperationException(
                "galaxy map drag rebuilt its expensive bitmap during mouse movement");
        System.Diagnostics.Stopwatch dragTimer = System.Diagnostics.Stopwatch.StartNew();
        for (int frame = 0; frame < 120; frame++)
        {
            RaiseMouse(map, "OnMouseMove", new MouseEventArgs(MouseButtons.Left, 0,
                25 + frame % 3, 20 + frame % 2, 0));
            Application.DoEvents();
            map.Update();
        }
        dragTimer.Stop();
        if (dragTimer.ElapsedMilliseconds > 5000)
            throw new InvalidOperationException("galaxy map drag preview is too slow: " +
                dragTimer.ElapsedMilliseconds + " ms for 120 frames");
        Console.WriteLine("galaxy drag preview: 120 frames in {0} ms",
            dragTimer.ElapsedMilliseconds);
        RaiseMouse(map, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1, 25, 20, 0));
        Application.DoEvents();
        PointF galaxyPan = (PointF)typeof(MainForm).GetField("galaxyMapPan", flags).GetValue(form);
        if (galaxyPan == PointF.Empty)
            throw new InvalidOperationException("galaxy map drag did not pan the view");
        RaiseMouse(map, "OnMouseDown", new MouseEventArgs(MouseButtons.Right, 1, 20, 20, 0));
        Application.DoEvents();
        galaxyZoom = (float)typeof(MainForm).GetField("galaxyMapZoom", flags).GetValue(form);
        galaxyPan = (PointF)typeof(MainForm).GetField("galaxyMapPan", flags).GetValue(form);
        if (Math.Abs(galaxyZoom - 1F) > 0.0001F || galaxyPan != PointF.Empty)
            throw new InvalidOperationException("galaxy map right click did not reset the view");

        StarMapHitRecord hit = hits[0];
        MouseEventArgs click = new MouseEventArgs(MouseButtons.Left, 1,
            (int)Math.Round(hit.Point.X), (int)Math.Round(hit.Point.Y), 0);
        RaiseMouse(map, "OnMouseDown", click);
        RaiseMouse(map, "OnMouseUp", click);
        Application.DoEvents();
        Form systemMap = (Form)typeof(MainForm).GetField("systemMapForm", flags).GetValue(form);
        StarHeaderRecord star = hit.Value as StarHeaderRecord;
        if (tabs.SelectedTab != galaxyPage || systemMap == null || systemMap.IsDisposed ||
            star == null || systemMap.Text != "Карта системы — " + star.Name)
            throw new InvalidOperationException("galaxy map LMB did not activate the selected system");
        systemMap.Refresh();
        Application.DoEvents();
        System.Collections.Generic.List<StarMapHitRecord> systemHits =
            (System.Collections.Generic.List<StarMapHitRecord>)typeof(MainForm)
                .GetField("systemMapHits", flags).GetValue(form);
        ListBox objectList = (ListBox)typeof(MainForm).GetField("galaxyObjectList", flags).GetValue(form);
        if (systemHits == null || systemHits.Count == 0 || objectList == null)
            throw new InvalidOperationException("system map interaction test has no object target");
        StarMapHitRecord objectHit = null;
        int jumpCount = 0;
        foreach (StarMapHitRecord candidate in systemHits)
        {
            if (candidate.Value is SystemJumpPointRecord)
            {
                jumpCount++;
                continue;
            }
            if (objectHit == null) objectHit = candidate;
        }
        if (jumpCount != 0)
            throw new InvalidOperationException("system jump points must be hidden by default");
        if (objectHit == null)
            throw new InvalidOperationException("system map has no selectable non-jump object");
        Button jumpToggle = systemMap.Controls["$jumpToggle"] as Button;
        if (jumpToggle == null || jumpToggle.Text.IndexOf("выкл", StringComparison.Ordinal) < 0)
            throw new InvalidOperationException("system jump toggle is missing or has wrong default state");
        jumpToggle.PerformClick();
        systemMap.Refresh();
        Application.DoEvents();
        jumpCount = 0;
        SystemJumpPointRecord firstJump = null;
        foreach (StarMapHitRecord candidate in systemHits)
            if (candidate.Value is SystemJumpPointRecord)
            {
                jumpCount++;
                if (firstJump == null) firstJump = (SystemJumpPointRecord)candidate.Value;
            }
        if (jumpCount == 0 || jumpToggle.Text.IndexOf("вкл", StringComparison.Ordinal) < 0)
            throw new InvalidOperationException("system jump toggle did not reveal jump points");
        ShipHeaderRecord player = (ShipHeaderRecord)typeof(MainForm).GetMethod("FindPlayerShip", flags)
            .Invoke(form, null);
        object[] jumpArguments = { player, star, firstJump.TargetStar, 0.0F, 0.0F };
        bool jumpCalculated = (bool)typeof(MainForm).GetMethod(
            "TryCalculateJumpDestination", flags).Invoke(form, jumpArguments);
        if (!jumpCalculated || Math.Abs(firstJump.WorldPoint.X - (float)jumpArguments[3]) > 0.01F ||
            Math.Abs(firstJump.WorldPoint.Y - (float)jumpArguments[4]) > 0.01F)
            throw new InvalidOperationException("system map transition is not at the exact player jump destination");
        StarMapHitRecord firstJumpHit = null;
        foreach (StarMapHitRecord candidate in systemHits)
            if (object.ReferenceEquals(candidate.Value, firstJump))
            { firstJumpHit = candidate; break; }
        if (firstJumpHit == null)
            throw new InvalidOperationException("system map transition has no rendered hit point");
        float galaxyDx = firstJump.TargetStar.X - star.X;
        float galaxyDy = firstJump.TargetStar.Y - star.Y;
        float screenDx = firstJumpHit.Point.X - systemMap.ClientSize.Width / 2F;
        float screenDy = firstJumpHit.Point.Y - systemMap.ClientSize.Height / 2F;
        if (galaxyDx * screenDx + galaxyDy * screenDy <= 0F)
            throw new InvalidOperationException(
                "system transition is rendered opposite to its target on the galaxy map");

        AsteroidRecord probeAsteroid = new AsteroidRecord
        {
            ObjectId = 17,
            GraphName = "Asteroid.Yellow00",
            Minerals = 42
        };
        string asteroidLabel = (string)typeof(MainForm).GetMethod("StarMapObjectLabel", flags)
            .Invoke(form, new object[] { probeAsteroid });
        if (!asteroidLabel.StartsWith("Астероид 17", StringComparison.Ordinal) ||
            asteroidLabel.IndexOf("минералы: 42", StringComparison.Ordinal) < 0)
            throw new InvalidOperationException("asteroid map label is not localized or incomplete");
        objectList.ClearSelected();
        RaiseMouse(systemMap, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1,
            (int)objectHit.Point.X, (int)objectHit.Point.Y, 0));
        RaiseMouse(systemMap, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1,
            (int)objectHit.Point.X, (int)objectHit.Point.Y, 0));
        if (!object.ReferenceEquals(objectList.SelectedItem, objectHit.Value))
            throw new InvalidOperationException("system map object was not selected");
        RaiseMouse(systemMap, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1,
            (int)objectHit.Point.X, (int)objectHit.Point.Y, 0));
        RaiseMouse(systemMap, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1,
            (int)objectHit.Point.X, (int)objectHit.Point.Y, 0));
        if (objectList.SelectedIndex >= 0)
            throw new InvalidOperationException("second system map click did not clear selection");
        StarMapHitRecord secondObjectHit = null;
        foreach (StarMapHitRecord candidate in systemHits)
            if (!(candidate.Value is SystemJumpPointRecord) &&
                !object.ReferenceEquals(candidate.Value, objectHit.Value))
            {
                secondObjectHit = candidate;
                break;
            }
        if (secondObjectHit != null)
        {
            int firstIndex = objectList.Items.IndexOf(objectHit.Value);
            int secondIndex = objectList.Items.IndexOf(secondObjectHit.Value);
            if (firstIndex >= 0 && secondIndex >= 0)
            {
                objectList.SetSelected(firstIndex, true);
                objectList.SetSelected(secondIndex, true);
                bool firstSelected = (bool)typeof(MainForm).GetMethod("IsGalaxyObjectSelected", flags)
                    .Invoke(form, new object[] { objectHit.Value });
                bool secondSelected = (bool)typeof(MainForm).GetMethod("IsGalaxyObjectSelected", flags)
                    .Invoke(form, new object[] { secondObjectHit.Value });
                if (objectList.SelectedItems.Count != 2 || !firstSelected || !secondSelected)
                    throw new InvalidOperationException("system map multiple selection is not rendered consistently");
                systemMap.Refresh();
            }
        }
        RaiseMouse(systemMap, "OnMouseWheel", new MouseEventArgs(MouseButtons.None, 0,
            systemMap.ClientSize.Width / 2, systemMap.ClientSize.Height / 2, 120));
        RaiseMouse(systemMap, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 5, 5, 0));
        RaiseMouse(systemMap, "OnMouseMove", new MouseEventArgs(MouseButtons.Left, 0, 25, 20, 0));
        RaiseMouse(systemMap, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1, 25, 20, 0));
        Application.DoEvents();
        using (Bitmap systemBitmap = new Bitmap(systemMap.Width, systemMap.Height))
        {
            systemMap.DrawToBitmap(systemBitmap, new Rectangle(Point.Empty, systemBitmap.Size));
            systemBitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "system-map-interaction.png"), System.Drawing.Imaging.ImageFormat.Png);
        }
        StarMapHitRecord jumpHit = null;
        foreach (StarMapHitRecord candidate in systemHits)
            if (candidate.Value is SystemJumpPointRecord) { jumpHit = candidate; break; }
        if (jumpHit == null)
            throw new InvalidOperationException("enabled jump map has no transition target");
        Form previousMap = systemMap;
        RaiseMouse(previousMap, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1,
            (int)jumpHit.Point.X, (int)jumpHit.Point.Y, 0));
        RaiseMouse(previousMap, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1,
            (int)jumpHit.Point.X, (int)jumpHit.Point.Y, 0));
        DateTime transitionDeadline = DateTime.UtcNow.AddSeconds(5);
        do
        {
            Application.DoEvents();
            Thread.Sleep(20);
            systemMap = (Form)typeof(MainForm).GetField("systemMapForm", flags).GetValue(form);
        }
        while (DateTime.UtcNow < transitionDeadline &&
            (systemMap == null || object.ReferenceEquals(systemMap, previousMap)));
        if (systemMap == null || systemMap.IsDisposed || object.ReferenceEquals(systemMap, previousMap))
            throw new InvalidOperationException("jump transition did not open the target system map");
        Button persistedToggle = systemMap.Controls["$jumpToggle"] as Button;
        systemMap.Refresh();
        Application.DoEvents();
        System.Collections.Generic.List<StarMapHitRecord> targetHits =
            (System.Collections.Generic.List<StarMapHitRecord>)typeof(MainForm)
                .GetField("systemMapHits", flags).GetValue(form);
        int targetJumpCount = 0;
        foreach (StarMapHitRecord candidate in targetHits)
            if (candidate.Value is SystemJumpPointRecord) targetJumpCount++;
        if (persistedToggle == null ||
            persistedToggle.Text.IndexOf("вкл", StringComparison.Ordinal) < 0 ||
            targetJumpCount == 0)
            throw new InvalidOperationException("jump visibility was reset after changing systems");
        persistedToggle.PerformClick();
        systemMap.Close();
        Application.DoEvents();
        typeof(MainForm).GetMethod("ShowSelectedStarMap", flags).Invoke(form,
            new object[] { form, EventArgs.Empty });
        Application.DoEvents();
        Form disabledMap = (Form)typeof(MainForm).GetField("systemMapForm", flags).GetValue(form);
        Button disabledToggle = disabledMap == null ? null :
            disabledMap.Controls["$jumpToggle"] as Button;
        if (disabledToggle == null ||
            disabledToggle.Text.IndexOf("выкл", StringComparison.Ordinal) < 0)
            throw new InvalidOperationException("disabled jump visibility was not preserved");
        disabledMap.Close();
        Application.DoEvents();
        Console.WriteLine("galaxy-map interaction smoke: {0}; jumps={1}; asteroid-label=OK",
            star.Name, jumpCount);
    }

    private static void RaiseMouse(Control target, string methodName, MouseEventArgs args)
    {
        System.Reflection.MethodInfo method = typeof(Control).GetMethod(methodName,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        method.Invoke(target, new object[] { args });
    }

    private static T FindFirst<T>(Control root) where T : Control
    {
        foreach (Control child in root.Controls)
        {
            T typed = child as T;
            if (typed != null) return typed;
            typed = FindFirst<T>(child);
            if (typed != null) return typed;
        }
        return null;
    }
}
