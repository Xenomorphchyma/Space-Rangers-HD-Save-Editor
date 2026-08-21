using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SpaceRangersHdSaveEditor
{
    internal sealed class AdaptiveEditorForm : Form
    {
        internal Action Relayout;
        internal HashSet<Control> LayoutHidden;

        internal AdaptiveEditorForm()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }

    internal sealed class BufferedTabPage : TabPage
    {
        internal BufferedTabPage()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = SystemColors.Control;
            UseVisualStyleBackColor = true;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }
    }

    internal sealed class BufferedSectionPanel : Panel
    {
        internal BufferedSectionPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = SystemColors.Control;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }

    internal sealed class DirectoryEditControl : UserControl
    {
        private readonly TextBox editor;

        internal DirectoryEditControl()
        {
            editor = new TextBox { BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill };
            Button browse = new Button { Text = "...", Dock = DockStyle.Right, Width = 23, TabStop = false };
            browse.Click += BrowseClicked;
            Controls.Add(editor);
            Controls.Add(browse);
        }

        internal string Value
        {
            get { return editor.Text; }
            set { editor.Text = value ?? string.Empty; }
        }

        private void BrowseClicked(object sender, EventArgs e)
        {
            string selected;
            if (EditorFormFactory.SelectDirectory(FindForm(), editor.Text, out selected))
                editor.Text = selected;
        }
    }

    // Shared two-line list rendering with alternating row backgrounds.
    internal sealed class AdaptiveOwnerDrawListBox : ListBox
    {
        private static readonly Color AlternateRowColor = Color.FromArgb(249, 249, 249);

        internal AdaptiveOwnerDrawListBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        internal static int CaptionHeight(Font font, string caption)
        {
            int lineHeight = TextRenderer.MeasureText("Hg", font, Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;
            return (caption ?? string.Empty).IndexOf('|') >= 0 ? lineHeight * 2 + 4 : lineHeight + 4;
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            string caption = e.Index >= 0 && e.Index < Items.Count && Items[e.Index] != null
                ? GetItemText(Items[e.Index]) : string.Empty;
            e.ItemHeight = CaptionHeight(Font, caption);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            Color background = selected ? SystemColors.Highlight :
                ((e.Index & 1) == 0 ? SystemColors.Window : AlternateRowColor);
            Color foreground = selected ? SystemColors.HighlightText : ForeColor;
            using (SolidBrush brush = new SolidBrush(background))
                e.Graphics.FillRectangle(brush, e.Bounds);

            string caption = Items[e.Index] == null ? string.Empty : GetItemText(Items[e.Index]);
            int separator = caption.IndexOf('|');
            string first = separator < 0 ? caption : caption.Substring(0, separator);
            SearchResultEntry styled = Items[e.Index] as SearchResultEntry;
            Color firstForeground = selected || styled == null || !styled.FirstLineColor.HasValue
                ? foreground : styled.FirstLineColor.Value;
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, first, Font,
                new Point(e.Bounds.Left + 2, e.Bounds.Top + 2), firstForeground, flags);
            if (separator >= 0)
            {
                string second = caption.Substring(separator + 1);
                int lineHeight = TextRenderer.MeasureText("Hg", Font, Size.Empty,
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Height;
                Color secondForeground = selected ? foreground : SystemColors.ControlText;
                TextRenderer.DrawText(e.Graphics, second, Font,
                    new Point(e.Bounds.Left + 2, e.Bounds.Top + lineHeight + 4), secondForeground, flags);
            }
        }
    }

    // AlphaControls rotates captions for tpLeft/tpRight page controls itself.
    // WinForms reserves the correct strip, but its native TabControl can omit
    // owner-drawn text for vertical tabs on some Windows themes.  Keep the
    // native page geometry and draw/click the thin Delphi strip explicitly.
    internal sealed class VerticalTabStrip : Control
    {
        private readonly TabControl pages;
        private readonly int itemExtent;
        private readonly bool horizontalCaptions;

        internal VerticalTabStrip(TabControl pages, int stripWidth, int itemExtent)
        {
            this.pages = pages;
            this.itemExtent = Math.Max(1, itemExtent);
            horizontalCaptions = stripWidth > itemExtent;
            Name = "$verticalTabs_" + pages.Name;
            TabStop = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Width = Math.Max(1, stripWidth);
            BackColor = SystemColors.Control;
            pages.SelectedIndexChanged += PagesChanged;
            pages.ControlAdded += PagesChanged;
            pages.ControlRemoved += PagesChanged;
            pages.LocationChanged += PagesChanged;
            pages.SizeChanged += PagesChanged;
            SyncBounds();
        }

        private void PagesChanged(object sender, EventArgs args)
        {
            SyncBounds();
            Invalidate();
        }

        private void SyncBounds()
        {
            if (pages.Parent == null) return;
            int left = pages.Alignment == TabAlignment.Right
                ? pages.Right - Width - 2 : pages.Left + 2;
            Location = new Point(left, pages.Top + 2);
            Height = Math.Max(1, Math.Min(Math.Max(1, pages.Height - 4),
                pages.TabPages.Count * itemExtent));
            BringToFront();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(SystemColors.Control);
            for (int index = 0; index < pages.TabPages.Count; index++)
            {
                Rectangle item = new Rectangle(0, index * itemExtent,
                    Width, Math.Min(itemExtent, Height - index * itemExtent));
                if (item.Height <= 0) break;
                bool selected = index == pages.SelectedIndex;
                using (Brush background = new SolidBrush(selected
                    ? SystemColors.Window : SystemColors.Control))
                    e.Graphics.FillRectangle(background, item);
                ControlPaint.DrawBorder(e.Graphics, item, selected
                    ? SystemColors.ControlDark : SystemColors.ControlLight,
                    ButtonBorderStyle.Solid);

                GraphicsState state = e.Graphics.Save();
                try
                {
                    RectangleF textBounds;
                    if (horizontalCaptions)
                    {
                        textBounds = new RectangleF(item.Left + 2, item.Top + 1,
                            Math.Max(1, item.Width - 4), Math.Max(1, item.Height - 2));
                    }
                    else if (pages.Alignment == TabAlignment.Left)
                    {
                        e.Graphics.TranslateTransform(item.Left, item.Bottom);
                        e.Graphics.RotateTransform(-90F);
                        textBounds = new RectangleF(0F, 0F, item.Height, item.Width);
                    }
                    else
                    {
                        e.Graphics.TranslateTransform(item.Right, item.Top);
                        e.Graphics.RotateTransform(90F);
                        textBounds = new RectangleF(0F, 0F, item.Height, item.Width);
                    }
                    using (StringFormat format = new StringFormat())
                    using (Brush foreground = new SolidBrush(SystemColors.ControlText))
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        format.Trimming = StringTrimming.EllipsisCharacter;
                        Font font = selected ? new Font(Font, FontStyle.Bold) : Font;
                        e.Graphics.DrawString(pages.TabPages[index].Text, font,
                            foreground, textBounds, format);
                        if (!object.ReferenceEquals(font, Font)) font.Dispose();
                    }
                }
                finally
                {
                    e.Graphics.Restore(state);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int index = e.Y / itemExtent;
            if (index >= 0 && index < pages.TabPages.Count &&
                pages.TabPages[index].Enabled)
                pages.SelectedIndex = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pages.SelectedIndexChanged -= PagesChanged;
                pages.ControlAdded -= PagesChanged;
                pages.ControlRemoved -= PagesChanged;
                pages.LocationChanged -= PagesChanged;
                pages.SizeChanged -= PagesChanged;
            }
            base.Dispose(disposing);
        }
    }

    internal sealed class EditorNodeDefinition
    {
        internal readonly string Parent;
        internal readonly string Name;
        internal readonly string Kind;
        internal readonly string Text;
        internal readonly int X;
        internal readonly int Y;
        internal readonly int Width;
        internal readonly int Height;
        internal readonly bool Bold;
        internal readonly bool Visible;
        internal readonly bool TabVisible;
        internal readonly bool Enabled;
        internal readonly bool Checked;
        internal readonly bool ReadOnly;
        internal readonly int ItemIndex;
        internal readonly string[] Items;
        internal readonly string ActivePage;
        internal readonly string TabAlignment;
        internal readonly int TabWidth;
        internal readonly int TabHeight;
        internal readonly string Align;
        internal readonly bool AlignWithMargins;
        internal readonly int MarginLeft;
        internal readonly int MarginTop;
        internal readonly int MarginRight;
        internal readonly int MarginBottom;

        internal EditorNodeDefinition(
            string parent, string name, string kind, string text,
            int x, int y, int width, int height,
            bool bold, bool visible, bool tabVisible, bool enabled, bool isChecked,
            bool readOnly, int itemIndex, string[] items, string activePage,
            string tabAlignment, int tabWidth, int tabHeight, string align,
            bool alignWithMargins, int marginLeft, int marginTop,
            int marginRight, int marginBottom)
        {
            Parent = parent; Name = name; Kind = kind; Text = text;
            X = x; Y = y; Width = width; Height = height;
            Bold = bold; Visible = visible; TabVisible = tabVisible; Enabled = enabled;
            Checked = isChecked; ReadOnly = readOnly; ItemIndex = itemIndex;
            Items = items; ActivePage = activePage; TabAlignment = tabAlignment;
            TabWidth = tabWidth; TabHeight = tabHeight;
            Align = align;
            AlignWithMargins = alignWithMargins; MarginLeft = marginLeft;
            MarginTop = marginTop; MarginRight = marginRight; MarginBottom = marginBottom;
        }

        internal EditorNodeDefinition(
            string parent, string name, string kind,
            bool bold, bool visible, bool tabVisible, bool enabled, bool isChecked,
            bool readOnly, int itemIndex, string[] items, string activePage)
            : this(parent, name, kind, string.Empty,
                0, 0, 120, 26,
                bold, visible, tabVisible, enabled, isChecked,
                readOnly, itemIndex, items, activePage,
                "Top", 0, 0, "None", false, 8, 8, 8, 8)
        {
        }
    }

    internal sealed class EditorFormDefinition
    {
        internal readonly string Resource;
        internal readonly string Title;
        internal readonly int ClientWidth;
        internal readonly int ClientHeight;
        internal readonly bool Dialog;
        internal readonly EditorNodeDefinition[] Nodes;

        internal EditorFormDefinition(
            string resource, string title, int clientWidth, int clientHeight,
            bool dialog, EditorNodeDefinition[] nodes)
        {
            Resource = resource; Title = title; ClientWidth = clientWidth;
            ClientHeight = clientHeight; Dialog = dialog; Nodes = nodes;
        }

        internal EditorFormDefinition(
            string resource, string title, EditorNodeDefinition[] nodes)
            : this(resource, title, 980, 700, true, nodes)
        {
        }
    }

    internal static class EditorFormFactory
    {
        internal static int LanguageIndex;
        private static readonly Font Regular = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font Bold = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);

        internal static void Show(string resource, IWin32Window owner)
        {
            EditorFormDefinition definition = EditorFormDefinitions.Get(resource);
            if (definition == null)
            {
                MessageBox.Show(owner, "Форма редактора не найдена: " + resource, "Space Rangers HD Save Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (Form form = Build(definition))
                form.ShowDialog(owner);
        }

        internal static Form Build(EditorFormDefinition definition)
        {
            bool fixedSemanticLayout = string.Equals(definition.Resource, "TSHIPFORM",
                StringComparison.OrdinalIgnoreCase) || string.Equals(definition.Resource, "TGALAXYFORM",
                StringComparison.OrdinalIgnoreCase) || string.Equals(definition.Resource, "TMODSLISTFORM",
                StringComparison.OrdinalIgnoreCase);
            AdaptiveEditorForm form = new AdaptiveEditorForm();
            form.Name = definition.Resource;
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.AutoScaleDimensions = new SizeF(96F, 96F);
            int preferredWidth = PreferredClientWidth(definition);
            form.ClientSize = new Size(preferredWidth, definition.ClientHeight);
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MaximizeBox = true;
            form.MinimizeBox = false;
            form.AutoScroll = !fixedSemanticLayout;
            form.KeyPreview = true;
            form.Text = definition.Title;
            form.Font = Regular;
            form.Icon = EditorAssets.AppIcon();
            form.KeyDown += delegate(object sender, KeyEventArgs args) { if (args.KeyCode == Keys.Escape) form.Close(); };

            Dictionary<string, Control> controls = new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);
            controls["$form"] = form;
            form.Tag = controls;
            List<KeyValuePair<TabControl, string>> activePages = new List<KeyValuePair<TabControl, string>>();
            List<TabPage> hiddenPages = new List<TabPage>();
            HashSet<Control> layoutHidden = new HashSet<Control>();
            form.LayoutHidden = layoutHidden;
            foreach (EditorNodeDefinition node in definition.Nodes)
            {
                Control parent;
                if (!controls.TryGetValue(node.Parent, out parent))
                    parent = form;
                Control control = Create(node);
                control.Name = node.Name;
                control.Text = FriendlyCaption(definition.Resource, node.Name, node.Kind);
                if (control is Label && node.Name.EndsWith("Val", StringComparison.OrdinalIgnoreCase) &&
                    (node.Parent.StartsWith("tsCustomRules", StringComparison.OrdinalIgnoreCase) ||
                    node.Parent.StartsWith("sbCustomRules", StringComparison.OrdinalIgnoreCase)))
                    control.Text = "0";
                control.Location = new Point(node.X, node.Y);
                if (!(control is TabPage))
                    control.Size = new Size(Math.Max(1, node.Width), Math.Max(1, node.Height));
                control.Font = node.Bold ? Bold : Regular;
                control.Visible = node.Visible;
                if (!node.Visible) layoutHidden.Add(control);
                control.Enabled = node.Enabled;
                if (parent is TabControl && control is TabPage)
                {
                    ((TabControl)parent).TabPages.Add((TabPage)control);
                    if (!node.TabVisible) hiddenPages.Add((TabPage)control);
                }
                else
                    parent.Controls.Add(control);
                controls[node.Name] = control;
                ApplyAlignedLayout(control, parent, node);
                ApplyItems(control, node);
                ApplyTabLayout(control, node);
                if (control is TabControl && !string.IsNullOrEmpty(node.ActivePage))
                    activePages.Add(new KeyValuePair<TabControl, string>((TabControl)control, node.ActivePage));
            }
            foreach (KeyValuePair<TabControl, string> pair in activePages)
            {
                Control page;
                if (controls.TryGetValue(pair.Value, out page) && page is TabPage)
                    pair.Key.SelectedTab = (TabPage)page;
            }
            // Delphi's TabVisible is independent from Control.Visible. Build
            // children first, then detach hidden pages while keeping them in the
            // registry for conditional FormShow-style activation.
            TabControl hiddenPageHost = new TabControl();
            hiddenPageHost.Name = "$hiddenTabPages";
            hiddenPageHost.Visible = false;
            hiddenPageHost.Size = new Size(1, 1);
            form.Controls.Add(hiddenPageHost);
            foreach (TabPage page in hiddenPages)
            {
                TabControl owner = page.Parent as TabControl;
                if (owner != null) owner.TabPages.Remove(page);
                hiddenPageHost.TabPages.Add(page);
            }
            bool layoutInProgress = false;
            form.Relayout = delegate
            {
                if (layoutInProgress || form.IsDisposed) return;
                layoutInProgress = true;
                form.SuspendLayout();
                try
                {
                    int contentHeight = fixedSemanticLayout
                        ? definition.ClientHeight
                        : ApplySemanticLayout(form, layoutHidden);
                    contentHeight = ApplySpecializedLayout(form, definition, controls, contentHeight);
                    FitButtonCaptions(form, layoutHidden);
                    ApplyPreferredContentHeight(form, definition, preferredWidth, contentHeight);
                    EnsureScrollExtents(form);
                }
                finally
                {
                    form.ResumeLayout(false);
                    layoutInProgress = false;
                }
                form.Invalidate();
            };
            form.Relayout();
            form.MinimumSize = new Size(Math.Min(form.Width, 560),
                Math.Min(form.Height, 260));
            foreach (Control initiallyHidden in new List<Control>(layoutHidden))
            {
                Control tracked = initiallyHidden;
                tracked.VisibleChanged += delegate
                {
                    if (!tracked.Visible || !layoutHidden.Remove(tracked)) return;
                    form.Relayout();
                };
            }
            form.Shown += delegate
            {
                FitDialogToWorkingArea(form);
                if (form.Relayout != null) form.Relayout();
            };
            if (fixedSemanticLayout)
                form.ResizeEnd += delegate { if (form.Relayout != null) form.Relayout(); };
            BindListContextPopups(definition.Resource, controls);
            if (LanguageIndex == 1) LocalizeEnglish(form);
            return form;
        }

        private static string FriendlyCaption(string resource, string name, string kind)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            string controlCaption;
            if (LanguageIndex != 1 && RussianControlOverrides.TryGetValue(
                resource + "/" + name, out controlCaption)) return controlCaption;

            // Delphi editors normally have an empty Caption/Text.  Deriving a
            // caption from an ed*/mm* control name placed a second, clipped copy
            // of the label inside unbound input fields during initial display.
            string storedValue;
            if (IsValueControlKind(kind) && RussianCaptions.TryGet(
                resource, name, out storedValue) && storedValue.Length == 0)
                return string.Empty;
            string value = name;
            string[] prefixes = {
                "lbl", "ed", "cb", "chb", "chk", "btn", "gb", "pc", "ts",
                "mm", "lb", "clb", "tv", "lv", "pb", "pnl", "se", "tb", "s"
            };
            foreach (string prefix in prefixes)
                if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                    value.Length > prefix.Length)
                {
                    value = value.Substring(prefix.Length);
                    break;
                }
            value = System.Text.RegularExpressions.Regex.Replace(value, "([a-z0-9])([A-ZА-Я])", "$1 $2");
            value = value.Replace('_', ' ').Trim();
            if (value.Length == 0) value = name;
            if (kind == "button")
            {
                if (value.IndexOf("Close", StringComparison.OrdinalIgnoreCase) >= 0) value = "Close";
                else if (value.IndexOf("Save", StringComparison.OrdinalIgnoreCase) >= 0) value = "Save";
                else if (value.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0) value = "Delete";
                else if (value.IndexOf("Edit", StringComparison.OrdinalIgnoreCase) >= 0) value = "Edit";
                else if (value.IndexOf("Add", StringComparison.OrdinalIgnoreCase) >= 0) value = "Add";
            }
            if (LanguageIndex == 1) return value;
            string translated;
            if (RussianCaptionOverrides.TryGetValue(value, out translated)) return translated;
            if (EditorLocalization.TryRussian(value, out translated)) return translated;
            string canonical;
            if (CaptionAliases.TryGetValue(value, out canonical))
            {
                if (RussianCaptionOverrides.TryGetValue(canonical, out translated) ||
                    EditorLocalization.TryRussian(canonical, out translated)) return translated;
                value = canonical;
            }
            string russianCaption;
            if (RussianCaptions.TryGet(resource, name, out russianCaption))
            {
                if (string.IsNullOrEmpty(russianCaption)) return string.Empty;
                if (RussianCaptionOverrides.TryGetValue(russianCaption, out translated) ||
                    EditorLocalization.TryRussian(russianCaption, out translated)) return translated;
                return TranslateCaptionTokens(russianCaption);
            }
            return TranslateCaptionTokens(value);
        }

        private static bool IsValueControlKind(string kind)
        {
            return string.Equals(kind, "edit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "memo", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "combo", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "directory", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "list", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "owner-list", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "checklist", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "listview", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "tree", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kind, "grid", StringComparison.OrdinalIgnoreCase);
        }

        private static readonly Dictionary<string, string> RussianControlOverrides =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Keep the field meaning instead of the historical presentation typo.
                { "TSHIPFORM/lblKellerChameleonCharge", "Келлер:" },
                { "TSHIPFORM/lblTerronChameleonCharge", "Террон:" },
                { "TGALAXYFORM/chbRejectedPB", "Без планетарных боёв" },
                { "TPLANETFORM/chbNoBuyShips", "Не производить корабли" },
                { "TSCOLORDIALOGFORM/sBitBtn4", "Настроить цвета" },
                { "TSHIPFORM/chbGraphDominator", "Графика доминатора" },
                { "TSHIPFORM/lblAverageEqValue", "Средняя сила оснащения:" },
                { "TSHIPFORM/lblAverageFreeSpaceRatio", "Свободное место в корпусах:" },
                { "TSHIPFORM/lblAverageMoneyToCapital", "Деньги / капитал:" },
                { "TSHIPFORM/lblRatioOfTooCostlyEqInShop", "Дорогое оснащение, %:" },
                { "TSHIPFORM/lblLiberationKills", "Побед при освобожд.:" },
                { "TSHIPFORM/lblKillAllShips", "Убито кораблей:" },
                { "TSHIPFORM/lblKillDominators", "Убито доминаторов:" },
                { "TSHIPFORM/lblKillRangers", "Убито рейнджеров:" },
                { "TSHIPFORM/lblKillWarriors", "Убито военных:" },
                { "TSHIPFORM/lblKillCustomInCurSystem", "Мод. кораблей в системе:" },
                { "TSHIPFORM/lblKillInCurSystemNormals", "Коалиционных в системе:" },
                { "TSHIPFORM/lblKillInCurSystemDominators", "Доминаторов в сист.:" },
                { "TSHIPFORM/lblKillInCurSystemPirates", "Пиратов в системе:" },
                { "TSHIPFORM/lblRank", "Ранг коалиции:" },
                { "TSHIPFORM/lblLiberationPlanet", "Освобожд. планета:" },
                { "TSHIPFORM/lblPirateRankPoints", "Очки ранга пиратов:" },
                { "TSHIPFORM/lblStatusChangeTrader", "Смена торг. статуса:" },
                { "TSHIPFORM/chbOrderAbsolute", "Приоритет" }
            };

        private static readonly Dictionary<string, string> RussianCaptionOverrides =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Asteroids Destroyed", "Астероидов уничтожено" },
                { "Ach Already Received", "Полученные достижения" },
                { "Storage Item", "Предмет на складе" },
                { "Pirate Dif Level", "Сложность пиратов" },
                { "Trade Dif Level", "Сложность торговли" },
                { "Scn Dif Level", "Сложность сценария" },
                { "Repair Dif Level", "Сложность ремонта" },
                { "Tech Dif Level", "Сложность технологий" },
                { "Tex. Dif Level", "Сложность технологий" },
                { "Quest Dif Level", "Сложность заданий" },
                { "Hole Dif Level", "Сложность гиперпереходов" },
                { "Balance Dif Level", "Общая сложность" },
                { "Blazer Research", "Исследования Блазера" },
                { "Keller Research", "Исследования Келлера" },
                { "Terron Research", "Исследования Террона" },
                { "Blazer Material", "Материалы Блазера" },
                { "Keller Material", "Материалы Келлера" },
                { "Terron Material", "Материалы Террона" },
                { "War Delta Win", "Изменение баланса войны" },
                { "War Delta Win Dominators", "Доминаторы" },
                { "War Delta Win Pirates", "Пираты" },
                { "War Delta Win Coalition", "Коалиция" },
                { "Quest Old", "Завершённые задания" },
                { "Keller Attack State", "Состояние атаки Келлера" },
                { "Keller Attack Star", "Цель атаки Келлера" },
                { "File Flags", "Параметры сохранения" },
                { "Rejected PB", "Отказ от планетарных боёв" },
                { "Player Read", "Прочитано игроком" },
                { "Obj Ship1", "Корабль 1" },
                { "Obj Ship2", "Корабль 2" },
                { "Obj Ship3", "Корабль 3" },
                { "Obj Planet1", "Планета 1" },
                { "Obj Planet2", "Планета 2" },
                { "Obj Planet3", "Планета 3" },
                { "Fried Ships", "Кораблей сгорело у звёзд" },
                { "Defended System", "Защищено систем" },
                { "Pirate Systems", "Пиратских систем" },
                { "Science Progress", "Научный прогресс" },
                { "Programs Used", "Программ использовано" },
                { "Pirates Freed", "Пиратов освобождено" },
                { "Health Drained", "Здоровья поглощено" },
                { "Fuel Gotten From Sun", "Топлива получено от звёзд" },
                { "Fuel Tank Last Id", "ID последнего бака" },
                { "Planets Visited", "Планет посещено" },
                { "Already Received", "Полученные достижения" },
                { "People Cnt", "Население" }, { "Custom Faction", "Пользовательская фракция" },
                { "No Planet Shop Update", "Не обновлять магазин планеты" },
                { "Pilot Race", "Раса пилота" }, { "Custom Type Name", "Пользовательский тип" },
                { "Pilot Race Name", "Название расы пилота" }, { "Ship Partner", "Наниматель" },
                { "Item Type", "Тип предмета" }, { "Script Item", "Скриптовый предмет" },
                { "Stored Item", "Сохранённый предмет" }, { "Item Destroy", "Уничтожить предмет" },
                { "No Drop", "Запрет выпадения" }, { "Init Vars", "Начальные переменные" },
                { "Item Place", "Расположение предмета" }, { "Graph Name", "Имя графики" },
                { "Map Label", "Подпись на карте" }, { "System Background", "Фон системы" },
                { "Weapon Id", "ID оружия" }, { "Weapon Type", "Тип оружия" },
                { "Tech Level", "Технический уровень" }, { "From Angle", "Исходный угол" },
                { "Game Path", "Путь к игре" }, { "Full Log", "Полное логирование" },
                { "System Name", "Системное имя" }, { "Tech Radius", "Радиус техуровня" },
                { "Mod Cost", "Стоимость мода" }, { "Avg Size", "Средний размер" },
                { "Avg Radius", "Средний радиус" }, { "Shot Count", "Количество выстрелов" },
                { "Attack Count", "Количество атак" }, { "Message Type", "Тип сообщения" },
                { "Custom Type", "Пользовательский тип" }, { "Sound Type", "Тип звука" },
                { "Turn", "Ход" }, { "Text Message", "Текст сообщения" },
                { "No Sound", "Без звука" }, { "Hide Tags", "Скрыть теги" },
                { "Additional", "Дополнительные" }, { "Extra Bonus", "Экстра-бонусы" },
                { "Custom Rules", "Тонкие настройки" },
                { "Custom Rules Balance", "Баланс" },
                { "Custom Rules Galaxy", "Галактика" },
                { "Custom Rules Others", "Прочее" },
                { "Kling Strength", "Сила клингов" },
                { "Kling Aggro", "Агрессивность клингов" },
                { "Kling Spawn", "Появление клингов" },
                { "Pirate Aggro", "Агрессивность пиратов" },
                { "Coal Aggro", "Агрессивность коалиции" },
                { "Extra Inventions", "Дополнительные изобретения" },
                { "Extra Rangers", "Дополнительные рейнджеры" },
                { "Hull Growth", "Рост корпусов" },
                { "Mods Drop List", "Выпадение модов" },
                { "Custom Ship Infos", "Пользовательские данные корабля" },
                { "Goods Statistic", "История покупок" },
                { "Goods Buy Cnt", "Куплено" },
                { "Goods Buy Cost", "Потрачено" },
                { "Goods Cnt", "Количество" },
                { "Goods Cost", "Стоимость" },
                { "Normal Ship", "Обычный корабль" },
                { "Ranger Ship", "Рейнджер" },
                { "Player Ship", "Игрок" },
                { "Free Points", "Свободные очки" },
                { "Day Without Player", "Дней без игрока" },
                { "Cur Star", "Система" },
                { "Cur Star Val", "—" },
                { "Cur Constellation", "Сектор" },
                { "Cur Constellation Val", "—" },
                { "Cur Planet", "Планета" },
                { "Cur Planet Val", "—" },
                { "Home Planet", "Родная" },
                { "Home Planet Val", "—" },
                { "Cur Ship", "Корабль" },
                { "Cur Ship Val", "—" },
                { "Script Ship", "Скриптовый" },
                { "Script Ship Val", "—" },
                { "Order Data", "Данные приказа" },
                { "Order Obj", "Объект приказа" },
                { "Order Des", "Координаты приказа" },
                { "Sys", "Системные" }, { "Graph Ship Trans", "Прозрачность" },
                { "Graph Dominator", "Доминаторская графика" },
                { "Script Chameleon", "Скриптовый хамелеон" },
                { "In Hiper Space", "В гиперпространстве" },
                { "Robbed By Player", "Ограблен игроком" },
                { "Relation To Rangers", "Отношение к рейнджерам" },
                { "Take Items", "Поднять предметы" },
                { "Goods Hold", "В трюме" }, { "Foods", "Еда" },
                { "Liberation System", "Освобождено систем" },
                { "Liberation Kills", "Побед при освобождении" },
                { "Liberation Planet", "Освобождённая планета" },
                { "Kill Pacifics", "Уничтожено мирных" },
                { "Kill All Ships", "Уничтожено кораблей" },
                { "Kill Dominators", "Уничтожено доминаторов" },
                { "Kill Warriors", "Уничтожено военных" },
                { "Kill Rangers", "Уничтожено рейнджеров" },
                { "Kill In Cur System Normals", "Мирных в текущей системе" },
                { "Kill In Cur System Dominators", "Доминаторов в текущей системе" },
                { "Kill In Cur System Pirates", "Пиратов в текущей системе" },
                { "Kill Custom In Cur System", "Пользовательских в текущей системе" },
                { "Ranger Prison", "Тюремный срок" },
                { "Status Trader", "Статус торговца" },
                { "Status Pirate", "Статус пирата" },
                { "Status Warrior", "Статус воина" },
                { "Eminent Points Trader", "Очки торговца" },
                { "Eminent Points Pirate", "Очки пирата" },
                { "Eminent Points Warrior", "Очки воина" },
                { "Status Change Trader", "Смена статуса торговца" },
                { "Status Change Pirate", "Смена статуса пирата" },
                { "Status Change Warrior", "Смена статуса воина" },
                { "Excluded From Rating", "Исключён из рейтинга" },
                { "Programs", "Программы" }, { "Programms", "Программы" },
                { "Kill Ship In Giper Space", "Уничтожено в гиперпространстве" },
                { "Kill Ship In Hole", "Уничтожено в чёрной дыре" },
                { "Day WBGive Programms", "День выдачи программ" },
                { "Planet Battles Win", "Побед в планетарных боях" },
                { "Last Planet Battle Date", "Дата последнего боя" },
                { "Unk Planet Complete", "Неизвестных планет пройдено" },
                { "Hot Equipment Cur", "Текущий комплект" },
                { "Goto Gov", "Переходов к правительству" },
                { "Exp Points For Trade", "Опыт за торговлю" },
                { "Hit Enemy After Take Programms", "Попаданий после получения программ" },
                { "Player Prison", "Игрок в тюрьме" },
                { "Pirate Clan Real", "Настоящий пиратский клан" },
                { "Exp Points For Kills", "Опыт за уничтожения" },
                { "Exp Points For Dominator Kills", "Опыт за доминаторов" },
                { "Exp Points For Pirate Kills", "Опыт за пиратов" },
                { "Exp Points For Good Kорабль", "Опыт за союзников" },
                { "Infections Place", "Места заражений" },
                { "Captain On The Bridge", "Капитан на мостике" },
                { "Bridge Cur Ship", "Корабль на мостике" },
                { "Bridge Cur Planet", "Планета на мостике" },
                { "Bridge BGReplace", "Фон мостика" },
                { "Programms In WB", "Программы на военной базе" },
                { "Investment Day", "День инвестиций" },
                { "Kill Dominators By Type", "Доминаторы по типам" },
                { "Asteroid Mod", "Количество астероидов" },
                { "Sun Damage Mod", "Урон звезды" },
                { "Ag Planets", "Аграрные планеты" },
                { "Mi Planets", "Добывающие планеты" },
                { "In Planets", "Индустриальные планеты" },
                { "Start Center", "Старт в центре" },
                { "Dominators Racial Weapons", "Расовое оружие доминаторов" },
                { "Zero Exp", "Нулевой опыт" },
                { "Max Range Missiles", "Максимальная дальность ракет" },
                { "Useless", "Счётный предмет" },
                { "Useless Item", "Счётный предмет" },
                { "Calculator", "Калькулятор" },
                { "Add color", "Добавить цвет" },
                { "Define custom colors", "Настроить пользовательские цвета" },
                { "Additional colors:", "Дополнительные цвета:" },
                { "Help", "Справка" },
                { "Ethers", "Эфиры" },
                { "Turn Vars", "Переменные хода" },
                { "TURN переменные", "Переменные хода" },
                { "On Act Code", "Код действия" },
                { "On Use Code", "Код при использовании" },
                { "OnActCode:", "Код действия:" },
                { "OnUseCode:", "Код при использовании:" }
            };

        private static readonly Dictionary<string, string> CaptionAliases =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Params", "Parameters" }, { "Main", "General" },
                { "Init Vars", "Initial Variables" }, { "Extra Bonus", "Extra Bonuses" },
                { "Pos X", "X" }, { "Pos Y", "Y" }, { "Cnt", "Count" }
            };

        private static readonly Dictionary<string, string> RussianTokens =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", "Название" }, { "Type", "Тип" }, { "Count", "Количество" },
                { "Cnt", "Количество" }, { "ID", "ID" },
                { "Custom", "Пользовательский" }, { "Faction", "Фракция" },
                { "Owner", "Владелец" }, { "Cost", "Стоимость" }, { "Price", "Цена" },
                { "Weight", "Вес" }, { "Day", "День" }, { "Money", "Деньги" },
                { "Race", "Раса" }, { "Government", "Правительство" },
                { "Economy", "Экономика" }, { "Graph", "Графика" },
                { "Radius", "Радиус" }, { "Angle", "Угол" }, { "Speed", "Скорость" },
                { "Target", "Цель" }, { "Star", "Звезда" }, { "Planet", "Планета" },
                { "Ship", "Корабль" }, { "Item", "Предмет" }, { "Script", "Скрипт" },
                { "Message", "Сообщение" }, { "Text", "Текст" }, { "Data", "Данные" },
                { "People", "Население" }, { "Ships", "Кораблей" }, { "Asteroids", "Астероидов" },
                { "Systems", "Систем" }, { "Planets", "Планет" }, { "Programs", "Программ" },
                { "Pirates", "Пиратов" }, { "Fuel", "Топливо" }, { "Health", "Здоровье" },
                { "Position", "Позиция" }, { "Damage", "Урон" }, { "Min", "Мин." },
                { "Max", "Макс." }, { "Current", "Текущий" }, { "Last", "Последний" },
                { "Home", "Родная" }, { "Level", "Уровень" }, { "Tech", "Тех." },
                { "Bonus", "Бонус" }, { "Special", "Особый" }, { "Additional", "Дополнительные" },
                { "Common", "Общие" }, { "General", "Основные" },
                { "Parameters", "Параметры" }, { "Hold", "Трюм" },
                { "Mods", "Моды" }, { "Settings", "Настройки" },
                { "Close", "Закрыть" }, { "Save", "Сохранить" },
                { "Delete", "Удалить" }, { "Edit", "Изменить" }, { "Add", "Добавить" },
                { "Fried", "Сгоревшие" }, { "Destroyed", "Уничтожено" },
                { "Received", "Полученные" }, { "Shop", "Магазин" },
                { "Surface", "Поверхность" }, { "Main", "Основные" },
                { "Vars", "Переменные" }, { "Place", "Расположение" },
                { "Path", "Путь" }, { "Full", "Полное" }, { "Log", "Лог" },
                { "Old", "Старый" }, { "Sound", "Звук" }, { "Hide", "Скрыть" },
                { "Tags", "Теги" }, { "Update", "Обновление" }, { "Map", "Карта" }
            };

        private static string TranslateCaptionTokens(string value)
        {
            string[] words = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < words.Length; index++)
            {
                string translated;
                if (RussianTokens.TryGetValue(words[index], out translated)) words[index] = translated;
            }
            return string.Join(" ", words);
        }

        private static int PreferredClientWidth(EditorFormDefinition definition)
        {
            if (string.Equals(definition.Resource, "TSTARMAPFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(definition.Resource, "TMAINFORM", StringComparison.OrdinalIgnoreCase))
                return definition.ClientWidth;
            if (string.Equals(definition.Resource, "TGALAXYFORM", StringComparison.OrdinalIgnoreCase))
                return Math.Max(definition.ClientWidth, 1180);
            if (string.Equals(definition.Resource, "TSHIPFORM", StringComparison.OrdinalIgnoreCase))
                return Math.Max(definition.ClientWidth, 1120);
            if (string.Equals(definition.Resource, "TMODSLISTFORM", StringComparison.OrdinalIgnoreCase))
                return 520;
            if (string.Equals(definition.Resource, "TPLANETFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(definition.Resource, "TITEMFORM", StringComparison.OrdinalIgnoreCase))
                return Math.Min(definition.ClientWidth, 940);
            bool hasTabs = false;
            foreach (EditorNodeDefinition node in definition.Nodes)
                if (string.Equals(node.Kind, "tabs", StringComparison.OrdinalIgnoreCase))
                { hasTabs = true; break; }
            if (hasTabs || definition.Nodes.Length > 45) return Math.Min(definition.ClientWidth, 980);
            if (definition.Nodes.Length > 24) return Math.Min(definition.ClientWidth, 820);
            return Math.Min(definition.ClientWidth, 700);
        }

        private static void ApplyPreferredContentHeight(Form form,
            EditorFormDefinition definition, int preferredWidth, int contentHeight)
        {
            if (string.Equals(definition.Resource, "TSTARMAPFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(definition.Resource, "TMAINFORM", StringComparison.OrdinalIgnoreCase)) return;
            int preferredHeight = Math.Max(120,
                Math.Min(definition.ClientHeight, contentHeight + 6));
            // Preserve a user- or working-area-constrained width during relayout. Build()
            // already starts at preferredWidth; forcing it again here made sizable dialogs
            // snap back to their design width and hid narrow-screen layout defects locally.
            int targetHeight = string.Equals(definition.Resource, "TSHIPFORM",
                StringComparison.OrdinalIgnoreCase)
                ? Math.Min(preferredHeight, form.ClientSize.Height)
                : preferredHeight;
            Size target = new Size(form.ClientSize.Width, targetHeight);
            if (form.ClientSize != target) form.ClientSize = target;
        }

        private static int ApplySpecializedLayout(Form form, EditorFormDefinition definition,
            Dictionary<string, Control> controls, int contentHeight)
        {
            if (string.Equals(definition.Resource, "TGALAXYFORM",
                StringComparison.OrdinalIgnoreCase))
                return ApplyGalaxyLayout(form, controls);
            if (string.Equals(definition.Resource, "TSHIPFORM",
                StringComparison.OrdinalIgnoreCase))
                return ApplyShipLayout(form, controls);
            if (string.Equals(definition.Resource, "TMODSLISTFORM",
                StringComparison.OrdinalIgnoreCase))
                return ApplyModsListLayout(form, controls);
            if (string.Equals(definition.Resource, "TBONUSALERTFORM",
                StringComparison.OrdinalIgnoreCase))
                return ApplyBonusAlertLayout(form, controls);
            if (!string.Equals(definition.Resource, "TACHIEVEMENTSFORM",
                StringComparison.OrdinalIgnoreCase)) return contentHeight;
            Control mainRaw, receivedRaw, memoRaw;
            if (!controls.TryGetValue("gbAchievements", out mainRaw) || !(mainRaw is GroupBox) ||
                !controls.TryGetValue("gbAchAlreadyReceived", out receivedRaw) ||
                !(receivedRaw is GroupBox) || !controls.TryGetValue("mmAchAlreadyReceived", out memoRaw))
                return contentHeight;
            GroupBox main = (GroupBox)mainRaw;
            GroupBox received = (GroupBox)receivedRaw;
            int mainWidth = Math.Max(700, form.ClientSize.Width - 20);
            main.SetBounds(10, 12, mainWidth, 410);
            int innerWidth = main.ClientSize.Width - 28;
            int leftWidth = Math.Max(330, (innerWidth - 14) / 2);
            int labelWidth = Math.Min(180, leftWidth / 2);
            string[] labels = { "lblAsteroidsDestroyed", "lblFriedShips", "lblDefendedSystem",
                "lblPirateSystems", "lblScienceProgress", "lblProgramsUsed", "lblPiratesFreed",
                "lblHealthDrained", "lblFuelGottenFromSun", "lblFuelTankLastId", "lblPlanetsVisited" };
            string[] fields = { "edAsteroidsDestroyed", "edFriedShips", "edDefendedSystem",
                "edPirateSystems", "edScienceProgress", "edProgramsUsed", "edPiratesFreed",
                "edHealthDrained", "edFuelGottenFromSun", "edFuelTankLastId", "edPlanetsVisited" };
            int y = 28;
            for (int index = 0; index < labels.Length; index++)
            {
                Control label, field;
                if (!controls.TryGetValue(labels[index], out label) ||
                    !controls.TryGetValue(fields[index], out field)) continue;
                label.SetBounds(14, y, labelWidth, 26);
                field.SetBounds(19 + labelWidth, y, leftWidth - labelWidth - 5, 26);
                y += 31;
            }
            int rightX = 28 + leftWidth;
            received.SetBounds(rightX, 28, main.ClientSize.Width - rightX - 14,
                Math.Max(220, y - 28));
            memoRaw.SetBounds(12, 24, received.ClientSize.Width - 24,
                received.ClientSize.Height - 38);
            main.Height = Math.Max(y + 14, received.Bottom + 14);
            return main.Bottom + 12;
        }

        private static int ApplyModsListLayout(Form form, Dictionary<string, Control> controls)
        {
            form.AutoScroll = false;
            Label warning = Registered<Label>(controls, "lblModsDeleteWarning");
            Button toggle = Registered<Button>(controls, "btnModsCfg");
            Control list = Registered<Control>(controls, "clbModsList");
            Control raw = Registered<Control>(controls, "mmModsList");
            int margin = 10;
            if (warning != null)
            {
                warning.AutoSize = false;
                warning.SetBounds(margin, 8, Math.Max(300, form.ClientSize.Width - margin * 2), 52);
            }
            if (toggle != null) toggle.SetBounds(margin, 66, 180, 30);
            Rectangle content = new Rectangle(margin, 104,
                Math.Max(300, form.ClientSize.Width - margin * 2), 190);
            if (list != null) list.Bounds = content;
            if (raw != null) raw.Bounds = content;
            return 304;
        }

        private static int ApplyBonusAlertLayout(Form form, Dictionary<string, Control> controls)
        {
            int margin = 14;
            Label title = Registered<Label>(controls, "lblBonusCRCTitle");
            Label options = Registered<Label>(controls, "lblBonusCRCOptions");
            Button correction = Registered<Button>(controls, "btnCorrection");
            Button readAsIs = Registered<Button>(controls, "btnReadAsIs");
            Control image = Registered<Control>(controls, "sImage1");
            if (image != null) image.Visible = false;
            if (title != null)
            {
                title.AutoSize = false; title.SetBounds(margin, 10,
                    form.ClientSize.Width - margin * 2, 24);
            }
            if (options != null)
            {
                options.AutoSize = false; options.TextAlign = ContentAlignment.TopLeft;
                options.SetBounds(margin, 38, form.ClientSize.Width - margin * 2, 86);
            }
            int buttonWidth = Math.Max(180, (form.ClientSize.Width - margin * 2 - 10) / 2);
            if (correction != null) correction.SetBounds(margin, 132, buttonWidth, 30);
            if (readAsIs != null) readAsIs.SetBounds(margin + buttonWidth + 10, 132,
                buttonWidth, 30);
            return 172;
        }

        private static T Registered<T>(Dictionary<string, Control> controls, string name)
            where T : Control
        {
            Control raw;
            return controls != null && controls.TryGetValue(name, out raw) ? raw as T : null;
        }

        private static HashSet<Control> HiddenControls(Form form)
        {
            AdaptiveEditorForm adaptive = form as AdaptiveEditorForm;
            return adaptive == null ? null : adaptive.LayoutHidden;
        }

        private static int ApplyGalaxyLayout(Form form, Dictionary<string, Control> controls)
        {
            form.AutoScroll = false;
            GroupBox frame = Registered<GroupBox>(controls, "gbGalaxy");
            TabControl galaxyTabs = Registered<TabControl>(controls, "pcGalaxy");
            if (frame == null || galaxyTabs == null) return 680;
            if (!object.ReferenceEquals(galaxyTabs.Parent, form))
            {
                galaxyTabs.Parent.Controls.Remove(galaxyTabs);
                form.Controls.Add(galaxyTabs);
            }
            frame.Visible = false;
            HashSet<Control> hidden = HiddenControls(form);
            if (hidden != null) hidden.Add(frame);
            galaxyTabs.SetBounds(8, 6, Math.Max(760, form.ClientSize.Width - 16), 540);

            TabPage main = Registered<TabPage>(controls, "tsMain");
            if (main != null) LayoutGalaxyMain(main, controls, hidden);

            TabPage custom = Registered<TabPage>(controls, "tsCustomRules");
            GroupBox customGroup = Registered<GroupBox>(controls, "gbCustomRules");
            TabControl customTabs = Registered<TabControl>(controls, "pcCustomRules");
            if (custom != null && customGroup != null && customTabs != null)
            {
                customGroup.Visible = false;
                customTabs.Visible = false;
                if (hidden != null) { hidden.Add(customGroup); hidden.Add(customTabs); }
                GroupBox balance = EnsureGalaxyRuleSection(controls, custom,
                    "$rulesBalance", "Баланс и ИИ", Registered<Control>(controls, "tsCustomRulesBalance"));
                GroupBox galaxy = EnsureGalaxyRuleSection(controls, custom,
                    "$rulesGalaxy", "Параметры галактики", Registered<Control>(controls, "tsCustomRulesGalaxy"));
                GroupBox otherRules = EnsureGalaxyRuleSection(controls, custom,
                    "$rulesOther", "Прочие правила", Registered<Control>(controls, "sbCustomRulesOther"));
                GroupBox flags = EnsureGalaxyRuleSection(controls, custom,
                    "$rulesFlags", "Правила и совместимость", null);
                int margin = 8, gap = 10, top = 36;
                int sectionWidth = Math.Max(280,
                    (custom.ClientSize.Width - margin * 2 - gap * 2) / 3);
                int sectionHeight = Math.Max(250,
                    Math.Min(278, custom.ClientSize.Height - top - 210));
                balance.SetBounds(margin, top, sectionWidth, sectionHeight);
                galaxy.SetBounds(margin + sectionWidth + gap, top, sectionWidth, sectionHeight);
                otherRules.SetBounds(margin + (sectionWidth + gap) * 2, top,
                    Math.Max(280, custom.ClientSize.Width - margin * 2 - sectionWidth * 2 - gap * 2),
                    sectionHeight);
                int flagsTop = top + sectionHeight + gap;
                flags.SetBounds(margin, flagsTop, Math.Max(760, custom.ClientSize.Width - margin * 2),
                    Math.Max(170, custom.ClientSize.Height - flagsTop - margin));
                LayoutTrackSection(balance, controls,
                    new string[] { "lblKlingStrength", "lblKlingAggro", "lblKlingSpawn",
                        "lblPirateAggro", "lblCoalAggro", "lblExtraInventions", "lblExtraRangers" },
                    new string[] { "tbDominatorsStrength", "tbDominatorsAggro", "tbDominatorsSpawn",
                        "tbPirateAggro", "tbCoalAggro", "tbExtraInventions", "tbExtraRangers" },
                    new string[] { "lblKlingStrengthVal", "lblKlingAggroVal", "lblKlingSpawnVal",
                        "lblPirateAggroVal", "lblCoalAggroVal", "lblExtraInventionsVal", "lblExtraRangersVal" },
                    "lblHullGrowth", "cbHullGrowth", new string[0]);
                LayoutTrackSection(galaxy, controls,
                    new string[] { "lblAsteroidMod", "lblSunDamageMod", "lblAgPlanets",
                        "lblMiPlanets", "lblInPlanets" },
                    new string[] { "tbAsteroidMod", "tbSunDamageMod", "tbAgPlanets",
                        "tbMiPlanets", "tbInPlanets" },
                    new string[] { "lblAsteroidModVal", "lblSunDamageModVal", "lblAgPlanetsVal",
                        "lblMiPlanetsVal", "lblInPlanetsVal" }, null, null,
                    new string[0]);
                LayoutTrackSection(otherRules, controls,
                    new string[] { "lblABDamageMod", "lblABHitpointsMod", "lblAITolerateJunk",
                        "lblABDropValueMod", "lblDropValueMod", "lblAkrinMod", "lblNodeDropMod" },
                    new string[] { "tbABDamageMod", "tbABHitpointsMod", "tbAITolerateJunk",
                        "tbABDropValueMod", "tbDropValueMod", "tbAkrinMod", "tbNodeDropMod" },
                    new string[] { "lblABDamageModVal", "lblABHitpointsModVal", "lblAITolerateJunkVal",
                        "lblABDropValueModVal", "lblDropValueModVal", "lblAkrinModVal", "lblNodeDropModVal" },
                    null, null, new string[0]);
                LayoutRuleFlags(flags, controls, new string[] {
                        "chbDominatorsRacialWeapons", "chbZeroExp", "chbMaxRangeMissiles",
                        "chbStartCenter", "chbABChangeEq", "chbOldMissileBonuses",
                        "chbOldSpeedCalc", "chbAIBuysEqFromShops", "chbABattleRoyale",
                        "chbDuplicateArtsAllowed", "chbRuinsUsingShop", "chbTechKnowledge",
                        "chbSpecialShips", "chbRuinsTargetting", "chbRnd", "chbRuinsPos",
                        "chbPirateNodes", "chbOldHyper" });
            }
            return 552;
        }

        private static GroupBox EnsureGalaxyRuleSection(Dictionary<string, Control> controls,
            Control parent, string name, string text, Control source)
        {
            GroupBox group = Registered<GroupBox>(controls, name);
            if (group == null)
            {
                group = new GroupBox { Name = name, Text = text, Font = Bold };
                parent.Controls.Add(group);
                controls[name] = group;
            }
            if (source != null)
            {
                List<Control> children = new List<Control>();
                foreach (Control child in source.Controls) children.Add(child);
                foreach (Control child in children) group.Controls.Add(child);
                source.Visible = false;
            }
            group.Visible = true;
            return group;
        }

        private static void LayoutTrackSection(GroupBox group,
            Dictionary<string, Control> controls, string[] labelNames, string[] trackNames,
            string[] valueNames, string extraLabelName, string extraFieldName, string[] checkNames)
        {
            if (group == null) return;
            int margin = 10, rowHeight = 30, labelWidth = Math.Min(145,
                Math.Max(112, group.ClientSize.Width * 38 / 100));
            int valueWidth = 36;
            int trackWidth = Math.Max(80, group.ClientSize.Width - margin * 2 - labelWidth - valueWidth - 6);
            int y = 22;
            for (int index = 0; index < labelNames.Length; index++, y += rowHeight)
            {
                Label label = Registered<Label>(controls, labelNames[index]);
                TrackBar track = Registered<TrackBar>(controls, trackNames[index]);
                Label value = Registered<Label>(controls, valueNames[index]);
                if (label != null)
                {
                    label.Text = CompactGalaxySliderCaption(label.Name, label.Text);
                    label.AutoSize = false; label.TextAlign = ContentAlignment.MiddleLeft;
                    label.SetBounds(margin, y, labelWidth, 26);
                }
                if (track != null)
                {
                    track.AutoSize = false; track.TickStyle = TickStyle.None;
                    track.SetBounds(margin + labelWidth, y, trackWidth, 26);
                }
                if (value != null)
                {
                    value.AutoSize = false; value.TextAlign = ContentAlignment.MiddleRight;
                    value.Font = Bold;
                    value.SetBounds(margin + labelWidth + trackWidth + 4, y, valueWidth, 26);
                }
            }
            if (!string.IsNullOrEmpty(extraLabelName) && !string.IsNullOrEmpty(extraFieldName))
            {
                Label label = Registered<Label>(controls, extraLabelName);
                Control field = Registered<Control>(controls, extraFieldName);
                if (label != null)
                {
                    label.AutoSize = false; label.TextAlign = ContentAlignment.MiddleLeft;
                    label.SetBounds(margin, y, labelWidth, 26);
                }
                if (field != null) field.SetBounds(margin + labelWidth, y + 1,
                    Math.Max(100, group.ClientSize.Width - margin * 2 - labelWidth), 25);
                y += rowHeight;
            }
            int gap = 6;
            int checkWidth = Math.Max(120, (group.ClientSize.Width - margin * 2 - gap) / 2);
            for (int index = 0; index < checkNames.Length; index++)
            {
                Control check = Registered<Control>(controls, checkNames[index]);
                if (check == null) continue;
                int column = index & 1;
                int row = index / 2;
                check.SetBounds(margin + column * (checkWidth + gap), y + row * 26,
                    checkWidth, 24);
            }
        }

        private static void LayoutRuleFlags(GroupBox group,
            Dictionary<string, Control> controls, string[] checkNames)
        {
            if (group == null) return;
            int margin = 10, gap = 12, top = 22, rowHeight = 28;
            int columns = 3;
            int rows = (checkNames.Length + columns - 1) / columns;
            int columnWidth = Math.Max(210,
                (group.ClientSize.Width - margin * 2 - gap * (columns - 1)) / columns);
            for (int index = 0; index < checkNames.Length; index++)
            {
                Control check = Registered<Control>(controls, checkNames[index]);
                if (check == null) continue;
                if (!object.ReferenceEquals(check.Parent, group))
                {
                    check.Parent.Controls.Remove(check);
                    group.Controls.Add(check);
                }
                check.Text = CompactGalaxyRuleCaption(check.Name, check.Text);
                int column = index / rows;
                int row = index % rows;
                check.SetBounds(margin + column * (columnWidth + gap),
                    top + row * rowHeight, columnWidth, 24);
                check.Visible = true;
            }
        }

        private static string CompactGalaxyRuleCaption(string name, string fallback)
        {
            switch (name)
            {
                case "chbDominatorsRacialWeapons": return "Рядовым доминаторам недоступно чужое оружие";
                case "chbZeroExp": return "Корабли появляются со стартовым опытом";
                case "chbMaxRangeMissiles": return "ИИ стреляет ракетами с максимальной дистанции";
                case "chbStartCenter": return "Стартовая система игрока — в центре";
                case "chbABChangeEq": return "В ЧД разрешена смена оборудования";
                case "chbOldMissileBonuses": return "Бонус ракет делится между ракетами залпа";
                case "chbOldSpeedCalc": return "Нелинейная формула скорости корабля";
                case "chbAIBuysEqFromShops": return "ИИ покупает оборудование в магазинах";
                case "chbABattleRoyale": return "Противники в ЧД враждуют между собой";
                case "chbDuplicateArtsAllowed": return "Разрешены однотипные артефакты";
                case "chbRuinsUsingShop": return "Станции получают оборудование из магазина";
                case "chbTechKnowledge": return "Оборудование без ограничений знаний";
                case "chbSpecialShips": return "Включены старые уникальные корпуса";
                case "chbRuinsTargetting": return "Дальние станции сражаются как обычно";
                case "chbRnd": return "Случайные события меняются после загрузки";
                case "chbRuinsPos": return "Станции строятся близко к центру";
                case "chbPirateNodes": return "Продажа нодов на пиратских станциях";
                case "chbOldHyper": return "Бои с гиперпиратами при перелётах";
                default: return fallback;
            }
        }

        private static string CompactGalaxySliderCaption(string name, string fallback)
        {
            switch (name)
            {
                case "lblCoalAggro": return "Агрессия коалиции";
                case "lblExtraInventions": return "Доп. изобретения";
                case "lblExtraRangers": return "Доп. рейнджеры";
                case "lblInPlanets": return "Индустр. планеты";
                case "lblABHitpointsMod": return "Прочность врагов в ЧД";
                case "lblAITolerateJunk": return "Лимит предметов";
                case "lblABDropValueMod": return "Качество трофеев в ЧД";
                case "lblDropValueMod": return "Макс. цена трофеев";
                case "lblAkrinMod": return "Акриновые вещи";
                case "lblNodeDropMod": return "Ноды из доминаторов";
                default: return fallback;
            }
        }

        private static void FitButtonCaptions(Control root, HashSet<Control> hidden)
        {
            foreach (Control child in root.Controls)
            {
                if (hidden != null && hidden.Contains(child)) continue;
                ButtonBase button = child as ButtonBase;
                if (button != null && !string.IsNullOrWhiteSpace(button.Text) && button.Text != "...")
                {
                    button.Text = CompactButtonCaption(button.Name, button.Text);
                    int adornment = button is CheckBox || button is RadioButton ? 24 : 12;
                    int desired = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty,
                        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                        TextFormatFlags.SingleLine).Width + adornment;
                    int maximum = Math.Max(button.Width, child.Parent.ClientSize.Width - child.Left - 4);
                    if (button.Width < desired) button.Width = Math.Min(desired, maximum);
                }
                FitButtonCaptions(child, hidden);
            }
        }

        private static string CompactButtonCaption(string name, string fallback)
        {
            switch (name)
            {
                case "chbBuiltByPirate": return "Сделано пиратами";
                case "chbGoodsItemNatural":
                case "chbCountableItemNatural": return "Натуральный предмет";
                case "chbTalkLocked": return "Интерком закрыт";
                case "btnSets": return "Сеты";
                case "chbNoJump": return "Запрет прыжка";
                case "chbPirateClanReal": return "Член пиратского клана";
                case "chbAutoArrange": return "Автоподбор";
                case "chbSpecialShip": return "Особый корабль";
                case "chbNoScan": return "Не сканировать";
                default: return fallback;
            }
        }

        private static void LayoutGalaxyMain(TabPage page, Dictionary<string, Control> controls,
            HashSet<Control> hidden)
        {
            int margin = 8, gap = 10;
            int columnWidth = Math.Max(240, (page.ClientSize.Width - margin * 2 - gap * 2) / 3);
            int x0 = margin, x1 = margin + columnWidth + gap, x2 = x1 + columnWidth + gap;
            int y0 = margin, y1 = margin, y2 = margin;
            y0 = PlaceAutoGroup(Registered<GroupBox>(controls, "gbDifficulty"), x0, y0,
                columnWidth, hidden) + gap;
            y0 = PlaceAutoGroup(Registered<GroupBox>(controls, "gbFileFlags"), x0, y0,
                columnWidth, hidden) + gap;
            y0 = PlaceAutoGroup(Registered<GroupBox>(controls, "gbKellerAttack"), x0, y0,
                columnWidth, hidden) + gap;
            y1 = PlaceAutoGroup(Registered<GroupBox>(controls, "gbResearch"), x1, y1,
                columnWidth, hidden) + gap;
            y1 = PlaceAutoGroup(Registered<GroupBox>(controls, "gbWarDeltaWin"), x1, y1,
                columnWidth, hidden) + gap;
            y1 = PlaceListGroup(Registered<GroupBox>(controls, "gbGates"),
                Registered<ListBox>(controls, "lbGates"), x1, y1, columnWidth, 116) + gap;
            y2 = PlaceListGroup(Registered<GroupBox>(controls, "gbPlanetNews"),
                Registered<ListBox>(controls, "lbPlanetNews"), x2, y2, columnWidth, 150) + gap;
            y2 = PlaceListGroup(Registered<GroupBox>(controls, "gbQuestOld"),
                Registered<ListBox>(controls, "lbOldQuest"), x2, y2, columnWidth, 150) + gap;
            PlaceListGroup(Registered<GroupBox>(controls, "gbWarOperations"),
                Registered<ListBox>(controls, "lbWarOperations"), x2, y2, columnWidth, 150);
        }

        private static int PlaceAutoGroup(GroupBox group, int x, int y, int width,
            HashSet<Control> hidden)
        {
            if (group == null) return y;
            group.SetBounds(x, y, width, 1);
            int height = LayoutContainer(group, Math.Max(220, width - 8), hidden);
            group.Height = Math.Max(58, height);
            return group.Bottom;
        }

        private static int PlaceListGroup(GroupBox group, ListBox list, int x, int y,
            int width, int height)
        {
            if (group == null) return y;
            group.SetBounds(x, y, width, height);
            if (list != null) list.SetBounds(10, 22, Math.Max(120, group.ClientSize.Width - 20),
                Math.Max(50, group.ClientSize.Height - 32));
            return group.Bottom;
        }

        private static void LayoutTrackPage(Control container, Dictionary<string, Control> controls,
            string[] labelNames, string[] trackNames, string[] valueNames,
            string extraLabelName, string extraFieldName, string[] checkNames)
        {
            if (container == null) return;
            int margin = 14, gap = 20;
            int columnWidth = Math.Max(300, (container.ClientSize.Width - margin * 2 - gap) / 2);
            int labelWidth = Math.Min(180, Math.Max(130, columnWidth / 3));
            int valueWidth = 48;
            int trackWidth = Math.Max(100, columnWidth - labelWidth - valueWidth - 12);
            int rowsPerColumn = Math.Max(1, (labelNames.Length + 1) / 2);
            int[] columnRows = { 0, 0 };
            for (int index = 0; index < labelNames.Length; index++)
            {
                int column = Math.Min(1, index / rowsPerColumn);
                int row = columnRows[column]++;
                int x = margin + column * (columnWidth + gap);
                int y = 16 + row * 38;
                Label label = Registered<Label>(controls, labelNames[index]);
                TrackBar track = Registered<TrackBar>(controls, trackNames[index]);
                Label value = Registered<Label>(controls, valueNames[index]);
                if (label != null)
                {
                    label.AutoSize = false; label.TextAlign = ContentAlignment.MiddleLeft;
                    label.SetBounds(x, y, labelWidth, 30);
                }
                if (track != null)
                {
                    track.AutoSize = false; track.TickStyle = TickStyle.BottomRight;
                    track.SetBounds(x + labelWidth, y, trackWidth, 30);
                }
                if (value != null)
                {
                    value.AutoSize = false; value.TextAlign = ContentAlignment.MiddleRight;
                    value.Font = Bold;
                    value.SetBounds(x + labelWidth + trackWidth + 4, y, valueWidth, 30);
                }
            }
            int rowBase = Math.Max(columnRows[0], columnRows[1]);
            if (!string.IsNullOrEmpty(extraLabelName) && !string.IsNullOrEmpty(extraFieldName))
            {
                int x = margin + columnWidth + gap;
                int y = 16 + columnRows[1] * 38;
                Label label = Registered<Label>(controls, extraLabelName);
                Control field = Registered<Control>(controls, extraFieldName);
                if (label != null)
                {
                    label.AutoSize = false; label.TextAlign = ContentAlignment.MiddleLeft;
                    label.SetBounds(x, y, labelWidth, 26);
                }
                if (field != null) field.SetBounds(x + labelWidth, y + 1,
                    Math.Max(110, columnWidth - labelWidth), 25);
                columnRows[1]++;
                rowBase = Math.Max(columnRows[0], columnRows[1]);
            }
            int checkY = 20 + rowBase * 38;
            for (int index = 0; index < checkNames.Length; index++)
            {
                Control check = Registered<Control>(controls, checkNames[index]);
                if (check == null) continue;
                int column = index & 1;
                int row = index / 2;
                check.SetBounds(margin + column * (columnWidth + gap), checkY + row * 30,
                    columnWidth, 26);
            }
        }

        private static int ApplyShipLayout(Form form, Dictionary<string, Control> controls)
        {
            form.AutoScroll = false;
            GroupBox frame = Registered<GroupBox>(controls, "gbShip");
            TabControl shipTabs = Registered<TabControl>(controls, "pcShip");
            if (frame == null || shipTabs == null) return 680;
            if (!object.ReferenceEquals(shipTabs.Parent, form))
            {
                shipTabs.Parent.Controls.Remove(shipTabs);
                form.Controls.Add(shipTabs);
            }
            frame.Visible = false;
            HashSet<Control> hidden = HiddenControls(form);
            if (hidden != null) hidden.Add(frame);
            int shipTabsWidth = Math.Max(760, form.ClientSize.Width - 16);
            int shipTabsHeight = Math.Max(628, form.ClientSize.Height - 16);
            shipTabs.SetBounds(8, 6, shipTabsWidth, shipTabsHeight);

            TabPage parameters = Registered<TabPage>(controls, "tsParams");
            TabControl parameterTabs = Registered<TabControl>(controls, "pcParams");
            int parameterTabsWidth = Math.Max(660, shipTabsWidth - 24);
            int parameterTabsHeight = Math.Max(510, shipTabs.Height - 44);
            if (parameters != null && parameterTabs != null)
            {
                parameterTabs.SetBounds(6, 6, parameterTabsWidth, parameterTabsHeight);
                // The fixed semantic form is explicitly relaid out on resize.  Anchoring this
                // nested tab control as well would apply the DPI delta twice during Scale().
                parameterTabs.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            int parameterPageWidth = parameterTabs == null ? form.ClientSize.Width - 64 :
                Math.Max(700, parameterTabsWidth - 12);
            int parameterPageHeight = parameterTabs == null ? form.ClientSize.Height - 110 :
                Math.Max(470, parameterTabsHeight - 34);
            LayoutShipMainPage(Registered<TabPage>(controls, "tsMain"), controls,
                HiddenControls(form), parameterPageWidth, parameterPageHeight);
            LayoutShipAdditionalPage(Registered<TabPage>(controls, "tsAdditional"), controls,
                HiddenControls(form), parameterPageWidth, parameterPageHeight);
            string[] packedPages = { "tsSubType", "tsTranclucator" };
            foreach (string pageName in packedPages)
                PackSections(Registered<TabPage>(controls, pageName), HiddenControls(form),
                    pageName == "tsSubType" ? 2 : 3);
            LayoutRangerSections(controls);

            LayoutPlayerPages(form, controls);
            int outerPageWidth = Math.Max(720, shipTabsWidth - 12);
            LayoutShipHold(Registered<TabPage>(controls, "tsHold"), controls, outerPageWidth);
            LayoutShipMods(Registered<TabPage>(controls, "tsMods"), controls, outerPageWidth);
            PackSections(Registered<TabPage>(controls, "tsRuins"), HiddenControls(form), 3);
            // Keep the full 700 px design height.  Collapsing the dialog to 650 px left
            // too little vertical room for the five collection lists on systems whose
            // font metrics make the statistics captions wrap.
            return 694;
        }

        private static void LayoutShipAdditionalPage(TabPage page,
            Dictionary<string, Control> controls, HashSet<Control> hidden, int availableWidth,
            int availableHeight)
        {
            if (page == null) return;
            int margin = 8, gap = 8;
            int pageWidth = availableWidth;
            // Give the label-heavy statistics block enough room to retain its compact
            // two-column layout with the wider Segoe UI metrics used by hosted Windows
            // runners.  The chameleon controls need much less horizontal space.
            int rightWidth = Math.Max(330, pageWidth * 28 / 100);
            int leftWidth = pageWidth - margin * 2 - gap - rightWidth;
            int mainBottom = PlaceAutoGroup(Registered<GroupBox>(controls, "gbAdditional"),
                margin, margin, leftWidth, hidden);
            int chameleonBottom = PlaceAutoGroup(Registered<GroupBox>(controls, "gbChameleon"),
                margin + leftWidth + gap, margin, rightWidth, hidden);
            int listTop = Math.Max(mainBottom, chameleonBottom) + gap;
            int listGap = 7;
            int listWidth = Math.Max(140,
                (pageWidth - margin * 2 - listGap * 4) / 5);
            int listHeight = Math.Max(70, availableHeight - listTop - margin);
            string[] groups = { "gbIllness", "gbRecentlyDroppedItems", "gbSpecialBonuses",
                "gbStatusEffects", "gbRewards" };
            string[] lists = { "lbIllness", "lbRecentlyDroppedItems", "lbSpecialBonuses",
                "lbStatusEffects", "lbRewards" };
            GroupBox recent = Registered<GroupBox>(controls, "gbRecentlyDroppedItems");
            if (recent != null) recent.Text = "Недавние предметы";
            for (int index = 0; index < groups.Length; index++)
                PlaceListGroup(Registered<GroupBox>(controls, groups[index]),
                    Registered<ListBox>(controls, lists[index]),
                    margin + index * (listWidth + listGap), listTop, listWidth, listHeight);
        }

        private static void LayoutShipMainPage(TabPage page, Dictionary<string, Control> controls,
            HashSet<Control> hidden, int availableWidth, int availableHeight)
        {
            if (page == null) return;
            int margin = 8, gap = 10;
            int rightWidth = Math.Min(400, Math.Max(340, availableWidth / 3));
            int leftWidth = availableWidth - margin * 2 - gap - rightWidth;
            int columnWidth = Math.Max(230, (leftWidth - gap) / 2);
            int twoColumns = columnWidth * 2 + gap;
            int thirdX = margin + twoColumns + gap;

            int commonBottom = PlaceAutoGroup(Registered<GroupBox>(controls, "gbCommon"),
                margin, margin, twoColumns, hidden);
            int locationBottom = PlaceShipLocation(Registered<GroupBox>(controls, "gbLocation"),
                controls, thirdX, margin, rightWidth);
            PlaceAutoGroup(Registered<GroupBox>(controls, "gbOrder"),
                margin, commonBottom + gap, columnWidth, hidden);
            PlaceAutoGroup(Registered<GroupBox>(controls, "gbSkills"),
                margin + columnWidth + gap, commonBottom + gap, columnWidth, hidden);
            int rightBottom = PlaceAutoGroup(Registered<GroupBox>(controls, "gbSys"),
                thirdX, locationBottom + gap, rightWidth, hidden);
            rightBottom = PlaceAutoGroup(Registered<GroupBox>(controls, "gbGraph"),
                thirdX, rightBottom + gap, rightWidth, hidden);
            int collectionTop = rightBottom + gap;
            int takeWidth = Math.Max(150, (rightWidth - gap) / 2);
            int relationWidth = Math.Max(150, rightWidth - gap - takeWidth);
            int collectionHeight = Math.Max(84, availableHeight - collectionTop - margin);
            PlaceListGroup(Registered<GroupBox>(controls, "gbTakeItems"),
                Registered<ListBox>(controls, "lbTakeItems"), thirdX, collectionTop,
                takeWidth, collectionHeight);
            GroupBox relations = Registered<GroupBox>(controls, "gbRelationToRangers");
            if (relations != null && string.Equals(relations.Text, "Отношение к рейнджерам",
                StringComparison.OrdinalIgnoreCase)) relations.Text = "Отношения";
            PlaceListGroup(relations,
                Registered<ListBox>(controls, "lbRelationToRangers"),
                thirdX + takeWidth + gap, collectionTop, relationWidth, collectionHeight);
        }

        private static int PlaceShipLocation(GroupBox group,
            Dictionary<string, Control> controls, int x, int y, int width)
        {
            if (group == null) return y;
            group.SetBounds(x, y, width, 116);
            string[] captions = { "lblCurStar", "lblCurConstellation", "lblCurPlanet",
                "lblHomePlanet", "lblCurShip", "lblScriptShip" };
            string[] values = { "lblCurStarVal", "lblCurConstellationVal", "lblCurPlanetVal",
                "lblHomePlanetVal", "lblCurShipVal", "lblScriptShipVal" };
            int margin = 10, gap = 10;
            int columnWidth = Math.Max(150, (group.ClientSize.Width - margin * 2 - gap) / 2);
            int labelWidth = Math.Max(78, columnWidth * 54 / 100);
            for (int index = 0; index < captions.Length; index++)
            {
                int column = index / 3;
                int row = index % 3;
                int rowX = margin + column * (columnWidth + gap);
                int rowY = 24 + row * 27;
                Label caption = Registered<Label>(controls, captions[index]);
                Label value = Registered<Label>(controls, values[index]);
                if (caption != null)
                {
                    caption.AutoSize = false; caption.TextAlign = ContentAlignment.MiddleLeft;
                    caption.SetBounds(rowX, rowY, labelWidth, 24);
                }
                if (value != null)
                {
                    value.AutoSize = false; value.TextAlign = ContentAlignment.MiddleLeft;
                    value.Font = Bold;
                    value.SetBounds(rowX + labelWidth + 4, rowY,
                        Math.Max(60, columnWidth - labelWidth - 4), 24);
                }
            }
            return group.Bottom;
        }

        private static void LayoutRangerSections(Dictionary<string, Control> controls)
        {
            GroupBox ranger = Registered<GroupBox>(controls, "gbRangerShip");
            GroupBox quests = Registered<GroupBox>(controls, "gbQuests");
            GroupBox programs = Registered<GroupBox>(controls, "gbProgramms");
            if (ranger == null || quests == null || programs == null) return;
            int leafBottom = 24;
            foreach (Control child in ranger.Controls)
                if (!(child is GroupBox)) leafBottom = Math.Max(leafBottom, child.Bottom);
            int margin = 10, gap = 10;
            int width = Math.Max(180, (ranger.ClientSize.Width - margin * 2 - gap) / 2);
            int top = leafBottom + 8;
            const int collectionHeight = 124;
            quests.SetBounds(margin, top, width, collectionHeight);
            programs.SetBounds(margin + width + gap, top, width, collectionHeight);
            ListBox questList = Registered<ListBox>(controls, "lbQuests");
            DataGridView programGrid = Registered<DataGridView>(controls, "sgProgramms");
            if (questList != null) questList.SetBounds(10, 22,
                Math.Max(100, quests.ClientSize.Width - 20), Math.Max(60, quests.ClientSize.Height - 32));
            if (programGrid != null) programGrid.SetBounds(10, 22,
                Math.Max(100, programs.ClientSize.Width - 20), Math.Max(60, programs.ClientSize.Height - 32));
            ranger.Height = Math.Max(220, top + collectionHeight + 10);
        }

        private static void PackSections(Control page, HashSet<Control> hidden, int columns)
        {
            if (page == null) return;
            List<Control> sections = new List<Control>();
            foreach (Control child in page.Controls)
                if ((hidden == null || !hidden.Contains(child)) &&
                    (child is GroupBox || child is Panel || child is TabControl)) sections.Add(child);
            if (sections.Count == 0) return;
            columns = Math.Max(1, Math.Min(columns, sections.Count));
            int margin = 8, gap = 10;
            TabControl ownerTabs = page.Parent as TabControl;
            int availableWidth = ownerTabs == null ? page.ClientSize.Width :
                Math.Max(page.ClientSize.Width, ownerTabs.ClientSize.Width - 8);
            Form ownerForm = page.FindForm();
            if (ownerForm != null && ownerTabs != null &&
                string.Equals(ownerTabs.Name, "pcParams", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(ownerForm.Name, "TSHIPFORM", StringComparison.OrdinalIgnoreCase))
                availableWidth = Math.Max(620, ownerForm.ClientSize.Width - 64);
            int sectionWidth = Math.Max(220,
                (availableWidth - margin * 2 - gap * (columns - 1)) / columns);
            int[] columnY = new int[columns];
            for (int index = 0; index < columns; index++) columnY[index] = margin;
            foreach (Control section in sections)
            {
                int target = 0;
                for (int column = 1; column < columns; column++)
                    if (columnY[column] < columnY[target]) target = column;
                int x = margin + target * (sectionWidth + gap);
                section.SetBounds(x, columnY[target], sectionWidth, 1);
                int height = LayoutContainer(section, Math.Max(200, sectionWidth - 8), hidden);
                section.Height = Math.Max(56, height);
                columnY[target] += section.Height + gap;
            }
        }

        private static void LayoutPlayerPages(Form form, Dictionary<string, Control> controls)
        {
            TabPage player = Registered<TabPage>(controls, "tsPlayer");
            Panel sections = Registered<Panel>(controls, "$playerSections");
            FlowLayoutPanel navigation = Registered<FlowLayoutPanel>(controls, "$playerNavigation");
            Panel content = Registered<Panel>(controls, "$playerContent");
            if (player == null || sections == null || navigation == null || content == null) return;
            TabControl owner = player.Parent as TabControl;
            int viewportWidth = owner == null ? player.ClientSize.Width :
                Math.Max(620, owner.ClientSize.Width - 8);
            int viewportHeight = owner == null ? player.ClientSize.Height :
                Math.Max(400, owner.ClientSize.Height - 34);
            sections.SetBounds(4, 4, Math.Max(620, viewportWidth - 8),
                Math.Max(400, viewportHeight - 8));
            navigation.SetBounds(0, 0, sections.ClientSize.Width, 36);
            content.SetBounds(0, 40, sections.ClientSize.Width,
                Math.Max(300, sections.ClientSize.Height - 40));
            string[] pageNames = { "$playerGeneralPage", "$playerFinancePage",
                "$playerJournalPage", "$playerServicePage" };
            foreach (string pageName in pageNames)
            {
                Panel sectionPage = Registered<Panel>(controls, pageName);
                if (sectionPage != null) sectionPage.SetBounds(0, 0,
                    content.ClientSize.Width, content.ClientSize.Height);
            }
            PackSections(Registered<Panel>(controls, "$playerJournalPage"), HiddenControls(form), 2);
            LayoutPlayerServicePage(Registered<Panel>(controls, "$playerServicePage"),
                controls, HiddenControls(form));
            GroupBox general = Registered<GroupBox>(controls, "$playerGeneral");
            Panel generalPage = Registered<Panel>(controls, "$playerGeneralPage");
            if (general != null && generalPage != null)
            {
                int groupWidth = Math.Min(780, Math.Max(560, generalPage.ClientSize.Width - 16));
                general.SetBounds(Math.Max(8, (generalPage.ClientSize.Width - groupWidth) / 2),
                    8, groupWidth, 1);
                general.Height = LayoutContainer(general, general.Width - 8, HiddenControls(form));
            }
            GroupBox finance = Registered<GroupBox>(controls, "$playerFinance");
            Panel financePage = Registered<Panel>(controls, "$playerFinancePage");
            if (finance != null && financePage != null)
            {
                int groupWidth = Math.Min(780, Math.Max(560, financePage.ClientSize.Width - 16));
                finance.SetBounds(Math.Max(8, (financePage.ClientSize.Width - groupWidth) / 2),
                    8, groupWidth, 1);
                finance.Height = LayoutContainer(finance, finance.Width - 8, HiddenControls(form));
            }
        }

        private static void LayoutPlayerServicePage(Panel page,
            Dictionary<string, Control> controls, HashSet<Control> hidden)
        {
            if (page == null) return;
            page.AutoScroll = false;
            const int margin = 8, gap = 10;
            int width = Math.Max(720, page.ClientSize.Width - margin * 2);
            int columnWidth = Math.Max(220, (width - gap * 2) / 3);
            int x0 = margin, x1 = x0 + columnWidth + gap, x2 = x1 + columnWidth + gap;
            int availableHeight = Math.Max(430, page.ClientSize.Height - margin * 2);
            int topHeight = Math.Max(220, Math.Min(255, availableHeight * 52 / 100));
            int bottomY = margin + topHeight + gap;
            int bottomHeight = Math.Max(160, availableHeight - topHeight - gap);

            GroupBox service = Registered<GroupBox>(controls, "$playerService");
            GroupBox chameleon = Registered<GroupBox>(controls, "gbChameleonLogic");
            int chameleonHeight = 96;
            int serviceHeight = Math.Max(112, topHeight - chameleonHeight - gap);
            LayoutFixedGroup(service, x0, margin, columnWidth, serviceHeight, hidden);
            LayoutFixedGroup(chameleon, x0, margin + serviceHeight + gap,
                columnWidth, chameleonHeight, hidden);
            LayoutFixedGroup(Registered<GroupBox>(controls, "gbBridge"),
                x1, margin, columnWidth, topHeight, hidden);
            LayoutFixedGroup(Registered<GroupBox>(controls, "gbRobotMap"),
                x2, margin, columnWidth, topHeight, hidden);

            LayoutFixedGroup(Registered<GroupBox>(controls, "gbProgrammsInWB"),
                x0, bottomY, columnWidth, bottomHeight, hidden);
            LayoutFixedGroup(Registered<GroupBox>(controls, "gbInvestmentDay"),
                x1, bottomY, columnWidth, bottomHeight, hidden);
            LayoutFixedGroup(Registered<GroupBox>(controls, "gbKillDominatorsByType"),
                x2, bottomY, columnWidth, bottomHeight, hidden);
        }

        private static void LayoutFixedGroup(GroupBox group, int x, int y, int width,
            int height, HashSet<Control> hidden)
        {
            if (group == null || hidden != null && hidden.Contains(group)) return;
            group.SetBounds(x, y, width, height);
            LayoutContainer(group, Math.Max(200, width - 8), hidden);
            group.SetBounds(x, y, width, height);
            foreach (Control child in group.Controls)
            {
                DataGridView grid = child as DataGridView;
                ListBox list = child as ListBox;
                if (grid == null && list == null) continue;
                child.SetBounds(10, 22, Math.Max(100, group.ClientSize.Width - 20),
                    Math.Max(54, group.ClientSize.Height - 32));
            }
        }

        private static void LayoutShipHold(TabPage page, Dictionary<string, Control> controls,
            int availableWidth)
        {
            if (page == null) return;
            int margin = 8, gap = 10;
            int width = Math.Max(680, availableWidth - margin * 2);
            int half = (width - gap) / 2;
            GroupBox equipment = Registered<GroupBox>(controls, "gbEquipments");
            GroupBox artefacts = Registered<GroupBox>(controls, "gbArtefacts");
            ListBox equipmentList = Registered<ListBox>(controls, "lbEquipments");
            ListBox artefactList = Registered<ListBox>(controls, "lbArtefacts");
            PlaceListGroup(equipment, equipmentList, margin, margin, half, 214);
            PlaceListGroup(artefacts, artefactList, margin + half + gap, margin, half, 214);

            GroupBox goods = Registered<GroupBox>(controls, "gbGoods");
            if (goods == null) return;
            goods.Text = "Товары";
            goods.SetBounds(margin, 232, width, 314);
            GroupBox hold = Registered<GroupBox>(controls, "gbGoodsHold");
            GroupBox statistics = Registered<GroupBox>(controls, "gbGoodsStatistic");
            if (hold != null) hold.Visible = false;
            if (statistics != null) statistics.Visible = false;
            Label holdTitle = EnsureLayoutLabel(controls, goods, "$goodsHoldTitle", "В трюме", true);
            Label historyTitle = EnsureLayoutLabel(controls, goods, "$goodsHistoryTitle", "История покупок", true);
            string[] headers = { "lblGoodsCnt", "lblGoodsCost", "lblGoodsBuyCnt", "lblGoodsBuyCost" };
            for (int index = 0; index < headers.Length; index++)
            {
                Label header = Registered<Label>(controls, headers[index]);
                if (header != null && !object.ReferenceEquals(header.Parent, goods)) goods.Controls.Add(header);
            }
            for (int row = 0; row < 8; row++)
                for (int field = 1; field <= 4; field++)
                {
                    TextBox editor = Registered<TextBox>(controls, "edGoods" + (row + 1) + field);
                    if (editor != null && !object.ReferenceEquals(editor.Parent, goods)) goods.Controls.Add(editor);
                }
            int labelWidth = Math.Min(170, Math.Max(125, goods.ClientSize.Width / 7));
            int tableX = labelWidth + 18;
            int columnGap = 8;
            int columnWidth = Math.Max(90,
                (goods.ClientSize.Width - tableX - 12 - columnGap * 3) / 4);
            holdTitle.SetBounds(tableX, 20, columnWidth * 2 + columnGap, 22);
            historyTitle.SetBounds(tableX + (columnWidth + columnGap) * 2, 20,
                columnWidth * 2 + columnGap, 22);
            for (int index = 0; index < headers.Length; index++)
            {
                Label header = Registered<Label>(controls, headers[index]);
                if (header == null) continue;
                header.AutoSize = false;
                header.TextAlign = ContentAlignment.MiddleCenter;
                header.SetBounds(tableX + index * (columnWidth + columnGap), 43,
                    columnWidth, 24);
            }
            string[] rowLabels = { "lblFoods", "lblMedicine", "lblTechnics", "lblLuxury",
                "lblMinerals", "lblAlcohol", "lblArms", "lblNarcotics" };
            for (int row = 0; row < rowLabels.Length; row++)
            {
                Label label = Registered<Label>(controls, rowLabels[row]);
                if (label == null) continue;
                label.AutoSize = false; label.TextAlign = ContentAlignment.MiddleLeft;
                label.SetBounds(12, 70 + row * 28, labelWidth, 24);
                for (int field = 1; field <= 4; field++)
                {
                    TextBox editor = Registered<TextBox>(controls, "edGoods" + (row + 1) + field);
                    if (editor != null) editor.SetBounds(tableX + (field - 1) * (columnWidth + columnGap),
                        70 + row * 28, columnWidth, 24);
                }
            }
        }

        private static Label EnsureLayoutLabel(Dictionary<string, Control> controls, Control parent,
            string name, string text, bool bold)
        {
            Label label = Registered<Label>(controls, name);
            if (label == null)
            {
                label = new Label { Name = name };
                controls[name] = label;
                parent.Controls.Add(label);
            }
            label.Text = text;
            label.Font = bold ? Bold : Regular;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            return label;
        }

        private static void LayoutGoodsColumns(GroupBox group, Dictionary<string, Control> controls,
            string firstHeaderName, string secondHeaderName, int firstField, int secondField)
        {
            if (group == null) return;
            int margin = 10, gap = 8;
            int columnWidth = Math.Max(80, (group.ClientSize.Width - margin * 2 - gap) / 2);
            Label firstHeader = Registered<Label>(controls, firstHeaderName);
            Label secondHeader = Registered<Label>(controls, secondHeaderName);
            if (firstHeader != null)
            {
                firstHeader.AutoSize = false; firstHeader.TextAlign = ContentAlignment.MiddleCenter;
                firstHeader.SetBounds(margin, 20, columnWidth, 24);
            }
            if (secondHeader != null)
            {
                secondHeader.AutoSize = false; secondHeader.TextAlign = ContentAlignment.MiddleCenter;
                secondHeader.SetBounds(margin + columnWidth + gap, 20, columnWidth, 24);
            }
            for (int row = 0; row < 8; row++)
            {
                TextBox first = Registered<TextBox>(controls, "edGoods" + (row + 1) + firstField);
                TextBox second = Registered<TextBox>(controls, "edGoods" + (row + 1) + secondField);
                if (first != null) first.SetBounds(margin, 46 + row * 28, columnWidth, 24);
                if (second != null) second.SetBounds(margin + columnWidth + gap,
                    46 + row * 28, columnWidth, 24);
            }
        }

        private static void LayoutShipMods(TabPage page, Dictionary<string, Control> controls,
            int availableWidth)
        {
            if (page == null) return;
            int margin = 8, gap = 10;
            int width = Math.Max(680, availableWidth - margin * 2);
            int half = (width - gap) / 2;
            PlaceListGroup(Registered<GroupBox>(controls, "gbModsDropList"),
                Registered<ListBox>(controls, "lbDropList"), margin, margin, half, 536);
            PlaceListGroup(Registered<GroupBox>(controls, "gbCustomShipInfos"),
                Registered<ListBox>(controls, "lbCustomShipInfos"), margin + half + gap,
                margin, half, 536);
        }

        private static int ApplySemanticLayout(Form form, HashSet<Control> layoutHidden)
        {
            int contentHeight = form.ClientSize.Height;
            form.SuspendLayout();
            try
            {
                contentHeight = LayoutContainer(form, Math.Max(520, form.ClientSize.Width - 28),
                    layoutHidden);
            }
            finally
            {
                form.ResumeLayout(true);
            }
            return contentHeight;
        }

        private static int LayoutContainer(Control container, int availableWidth,
            HashSet<Control> layoutHidden)
        {
            TabControl tabs = container as TabControl;
            if (tabs != null)
            {
                int maximumPageHeight = 0;
                int selectedPageHeight = 0;
                foreach (TabPage page in tabs.TabPages)
                {
                    page.AutoScroll = false;
                    int pageHeight = LayoutContainer(page,
                        Math.Max(320, tabs.ClientSize.Width - 18), layoutHidden);
                    maximumPageHeight = Math.Max(maximumPageHeight, pageHeight);
                    if (page == tabs.SelectedTab) selectedPageHeight = pageHeight;
                }
                int effectiveHeight = UseSelectedTabHeight(tabs) && selectedPageHeight > 0
                    ? selectedPageHeight : maximumPageHeight;
                return Math.Max(150, effectiveHeight + 32);
            }

            List<Control> children = new List<Control>();
            foreach (Control child in container.Controls)
                if (!child.Name.StartsWith("$", StringComparison.Ordinal) && !(child is TabPage) &&
                    (layoutHidden == null || !layoutHidden.Contains(child))) children.Add(child);

            int padding = container is GroupBox ? 12 : 8;
            int y = container is GroupBox ? 24 : 8;
            int width = Math.Max(300, availableWidth - padding * 2);
            List<Control> leaves = new List<Control>();
            List<Control> sections = new List<Control>();
            foreach (Control child in children)
            {
                if (child is GroupBox || child is TabControl || child is Panel && !(child is PictureBox))
                    sections.Add(child);
                else
                    leaves.Add(child);
            }

            HashSet<Control> placed = new HashSet<Control>();
            int labelWidth = PreferredLabelWidth(leaves, width);
            if (UseTwoColumnFieldPairs(container, leaves, width))
            {
                int pairGap = 12;
                int pairWidth = (width - pairGap) / 2;
                int pairLabelWidth = PreferredPairLabelWidth(leaves, pairWidth);
                int pairColumn = 0;
                int pairRowHeight = 0;
                foreach (Control rawLabel in leaves)
                {
                    Label label = rawLabel as Label;
                    if (label == null || placed.Contains(label)) continue;
                    Control field = FindSemanticField(label, leaves, placed);
                    if (field == null) continue;
                    int baseX = padding + pairColumn * (pairWidth + pairGap);
                    int labelHeight = SetSemanticLabelBounds(label, baseX, y,
                        pairLabelWidth);
                    SetSemanticBounds(field, baseX + pairLabelWidth + 5,
                        y + Math.Max(0, (labelHeight - 24) / 2),
                        pairWidth - pairLabelWidth - 5);
                    if (!(field is TextBox) || !((TextBox)field).Multiline)
                        field.Height = Math.Min(field.Height, 24);
                    placed.Add(label);
                    placed.Add(field);
                    pairRowHeight = Math.Max(pairRowHeight,
                        Math.Max(labelHeight + 1, field.Height + 1));
                    if (++pairColumn >= 2)
                    {
                        pairColumn = 0;
                        y += pairRowHeight;
                        pairRowHeight = 0;
                    }
                }
                if (pairColumn != 0) y += pairRowHeight;
            }
            foreach (Control label in leaves)
            {
                if (!(label is Label) || placed.Contains(label)) continue;
                Control field = FindSemanticField(label, leaves, placed);
                placed.Add(label);
                if (field != null)
                {
                    int labelHeight = SetSemanticLabelBounds((Label)label, padding,
                        y, labelWidth);
                    SetSemanticBounds(field, padding + labelWidth + 5,
                        y + Math.Max(0, (labelHeight - 24) / 2),
                        width - labelWidth - 5);
                    placed.Add(field);
                    y += Math.Max(labelHeight + 1, field.Height + 1);
                }
                else
                {
                    y += SetSemanticLabelBounds((Label)label, padding, y,
                        width) + 1;
                }
            }

            int column = 0;
            int rowHeight = 0;
            int columnWidth = Math.Max(140, (width - 10) / 2);
            foreach (Control child in leaves)
            {
                if (placed.Contains(child)) continue;
                bool fullRow = child is TextBox && ((TextBox)child).Multiline ||
                    child is ListBox || child is CheckedListBox || child is ListView ||
                    child is TreeView || child is DataGridView || child is PictureBox ||
                    child is CheckBox && string.Equals(container.Name, "gbProhibitions",
                        StringComparison.OrdinalIgnoreCase);
                if (fullRow && column != 0)
                {
                    y += rowHeight;
                    column = 0;
                    rowHeight = 0;
                }
                int x = padding + column * (columnWidth + 10);
                SetSemanticBounds(child, x, y, columnWidth);
                placed.Add(child);
                rowHeight = Math.Max(rowHeight, child.Height + 6);
                if (fullRow)
                {
                    child.Width = width;
                    column = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }
                else if (++column >= 2)
                {
                    column = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }
            }
            if (column != 0) y += rowHeight;

            if (UseTwoColumnSections(container, sections, width))
            {
                const int gap = 12;
                int sectionWidth = (width - gap) / 2;
                int[] columnY = { y, y };
                foreach (Control section in sections)
                {
                    int targetColumn = columnY[0] <= columnY[1] ? 0 : 1;
                    int sectionX = padding + targetColumn * (sectionWidth + gap);
                    section.SetBounds(sectionX, columnY[targetColumn], sectionWidth, 1);
                    int sectionInset = section is TabControl ? 0 : 8;
                    int sectionHeight = LayoutContainer(section, sectionWidth - sectionInset,
                        layoutHidden);
                    section.Height = section is TabControl ? sectionHeight : Math.Max(60, sectionHeight);
                    columnY[targetColumn] += section.Height + 8;
                }
                y = Math.Max(columnY[0], columnY[1]) - 8;
            }
            else
            {
                foreach (Control section in sections)
                {
                    section.SetBounds(padding, y, width, 1);
                    int sectionInset = section is TabControl ? 0 : 8;
                    int sectionHeight = LayoutContainer(section, width - sectionInset, layoutHidden);
                    section.Height = section is TabControl ? sectionHeight : Math.Max(60, sectionHeight);
                    y += section.Height + 8;
                }
            }
            int rightLimit = Math.Max(padding + 40, container.ClientSize.Width - padding);
            foreach (Control child in children)
                if (child.Right > rightLimit)
                    child.Width = Math.Max(40, rightLimit - child.Left);
            if (container is Panel && !(container is TabPage))
                ((Panel)container).AutoScroll = true;
            return y + padding;
        }

        private static bool UseTwoColumnSections(Control container,
            List<Control> sections, int availableWidth)
        {
            if (sections.Count < 2) return false;
            if (container is GroupBox && string.Equals(container.Name, "gbAdditional",
                StringComparison.OrdinalIgnoreCase) && availableWidth >= 500) return true;
            if (!(container is TabPage) || availableWidth < 720) return false;
            Form form = container.FindForm();
            string resource = form == null ? string.Empty : form.Name;
            return string.Equals(resource, "TGALAXYFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(resource, "TSHIPFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(resource, "TPLANETFORM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(resource, "TITEMFORM", StringComparison.OrdinalIgnoreCase);
        }

        private static bool UseTwoColumnFieldPairs(Control container,
            List<Control> leaves, int availableWidth)
        {
            if (!(container is GroupBox) || leaves.Count < 8 || availableWidth < 400) return false;
            Form form = container.FindForm();
            return form != null && string.Equals(form.Name, "TSHIPFORM",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool UseSelectedTabHeight(TabControl tabs)
        {
            Form form = tabs == null ? null : tabs.FindForm();
            return form != null && string.Equals(form.Name, "TSHIPFORM",
                StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(tabs.Name, "pcShip", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(tabs.Name, "pcParams", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(tabs.Name, "$playerSections", StringComparison.OrdinalIgnoreCase);
        }

        private static Control FindSemanticField(Control label, List<Control> leaves,
            HashSet<Control> placed)
        {
            string key = SemanticKey(label.Name);
            foreach (Control candidate in leaves)
            {
                if (placed.Contains(candidate) || candidate == label || candidate is Label ||
                    candidate is Button) continue;
                string candidateKey = SemanticKey(candidate.Name);
                if (candidateKey.Equals(key, StringComparison.OrdinalIgnoreCase) ||
                    candidateKey.StartsWith(key, StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith(candidateKey, StringComparison.OrdinalIgnoreCase))
                    return candidate;
            }
            return null;
        }

        private static int PreferredLabelWidth(List<Control> controls, int availableWidth)
        {
            int width = 170;
            foreach (Control control in controls)
            {
                Label label = control as Label;
                if (label == null) continue;
                int measured = TextRenderer.MeasureText(label.Text ?? string.Empty, label.Font,
                    Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    TextFormatFlags.SingleLine).Width + 12;
                width = Math.Max(width, measured);
            }
            return Math.Min(Math.Max(170, (int)(availableWidth * 0.58F)), width);
        }

        private static int PreferredPairLabelWidth(List<Control> controls, int availableWidth)
        {
            int width = 150;
            foreach (Control control in controls)
            {
                Label label = control as Label;
                if (label == null) continue;
                int measured = TextRenderer.MeasureText(label.Text ?? string.Empty, label.Font,
                    Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    TextFormatFlags.SingleLine).Width + 10;
                width = Math.Max(width, measured);
            }
            int maximum = Math.Max(130, availableWidth - 85);
            return Math.Min(maximum, Math.Max(150, width));
        }

        private static int SetSemanticLabelBounds(Label label, int x, int y, int width)
        {
            int measured = TextRenderer.MeasureText(label.Text ?? string.Empty, label.Font,
                Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                TextFormatFlags.SingleLine).Width;
            // A few pixels of overhang are visually harmless because labels do
            // not draw a border; wrapping such captions made whole sections
            // jump by a row.  Reserve wrapping for genuinely long Russian text.
            int height = measured > width + 18 ? Math.Max(38, label.Font.Height * 2 + 6) : 24;
            label.AutoSize = false;
            label.AutoEllipsis = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.SetBounds(x, y, width, height);
            return height;
        }

        private static void SetSemanticBounds(Control control, int x, int y, int width)
        {
            int height = 24;
            if (control is Button) { width = Math.Min(width, 210); height = 30; }
            else if (control is CheckBox || control is RadioButton) { height = 25; }
            else if (control is TextBox && ((TextBox)control).Multiline) height = 108;
            else if (control is ListBox || control is CheckedListBox || control is ListView || control is TreeView || control is DataGridView) height = 158;
            else if (control is PictureBox) height = 260;
            else if (control is TrackBar) height = 34;
            control.SetBounds(x, y, Math.Max(80, width), height);
        }

        private static string SemanticKey(string name)
        {
            string value = name ?? string.Empty;
            string[] prefixes = { "lbl", "ed", "cb", "chb", "chk", "btn", "mm", "lb", "clb", "tv", "lv", "se", "tb", "s" };
            foreach (string prefix in prefixes)
                if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && value.Length > prefix.Length)
                    return value.Substring(prefix.Length);
            return value;
        }

        private static void ApplyAlignedLayout(Control control, Control parent,
            EditorNodeDefinition node)
        {
            if (parent == null || node.Align == "None" || control is TabPage) return;
            Rectangle originalBounds = control.Bounds;
            EventHandler layout = delegate
            {
                int marginLeft = node.AlignWithMargins ? node.MarginLeft : 0;
                int marginTop = node.AlignWithMargins ? node.MarginTop : 0;
                int marginRight = node.AlignWithMargins ? node.MarginRight : 0;
                int marginBottom = node.AlignWithMargins ? node.MarginBottom : 0;
                int frameInset = Math.Max(0, originalBounds.X - marginLeft);
                Rectangle bounds = originalBounds;
                if (node.Align == "Client")
                {
                    // Preserve the semantic container inset and recompute the
                    // far edges when the parent is resized.
                    bounds = new Rectangle(originalBounds.X, originalBounds.Y,
                        Math.Max(1, parent.ClientSize.Width - originalBounds.X -
                            marginRight - frameInset),
                        Math.Max(1, parent.ClientSize.Height - originalBounds.Y -
                            marginBottom - frameInset));
                }
                else if (node.Align == "Top")
                    bounds = new Rectangle(originalBounds.X, originalBounds.Y,
                        Math.Max(1, parent.ClientSize.Width - originalBounds.X -
                            marginRight - frameInset),
                        originalBounds.Height);
                else if (node.Align == "Bottom")
                {
                    bounds = new Rectangle(originalBounds.X,
                        parent.ClientSize.Height - marginBottom - frameInset - originalBounds.Height,
                        Math.Max(1, parent.ClientSize.Width - originalBounds.X -
                            marginRight - frameInset),
                        originalBounds.Height);
                }
                else if (node.Align == "Left")
                    bounds = new Rectangle(originalBounds.X, originalBounds.Y, originalBounds.Width,
                        Math.Max(1, parent.ClientSize.Height - originalBounds.Y -
                            marginBottom - frameInset));
                else if (node.Align == "Right")
                {
                    bounds = new Rectangle(parent.ClientSize.Width - marginRight - frameInset -
                        originalBounds.Width, originalBounds.Y, originalBounds.Width,
                        Math.Max(1, parent.ClientSize.Height - originalBounds.Y -
                            marginBottom - frameInset));
                }
                if (control.Bounds != bounds) control.Bounds = bounds;
            };
            parent.ClientSizeChanged += layout;
            layout(parent, EventArgs.Empty);
        }

        private static void ApplyTabLayout(Control control, EditorNodeDefinition node)
        {
            TabControl tabs = control as TabControl;
            if (tabs == null) return;
            if (node.TabAlignment == "Left") tabs.Alignment = TabAlignment.Left;
            else if (node.TabAlignment == "Right") tabs.Alignment = TabAlignment.Right;
            else if (node.TabAlignment == "Bottom") tabs.Alignment = TabAlignment.Bottom;
            if (tabs.Alignment == TabAlignment.Left || tabs.Alignment == TabAlignment.Right)
            {
                tabs.Multiline = true;
                tabs.SizeMode = TabSizeMode.Fixed;
                // WinForms swaps ItemSize axes for Left/Right alignment. Delphi's
                // TabHeight is the long along-edge extent and TabWidth is the thin
                // strip thickness.
                tabs.ItemSize = new Size(node.TabHeight > 0 ? node.TabHeight : 100,
                    node.TabWidth > 0 ? node.TabWidth : 25);
                tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabs.DrawItem += DrawVerticalTab;
                if (tabs.Parent != null)
                {
                    VerticalTabStrip strip = new VerticalTabStrip(tabs,
                        node.TabWidth > 0 ? node.TabWidth : 25,
                        node.TabHeight > 0 ? node.TabHeight : 100);
                    strip.Font = tabs.Font;
                    tabs.Parent.Controls.Add(strip);
                    strip.BringToFront();
                }
            }
            else if (node.TabWidth > 0 || node.TabHeight > 0)
            {
                tabs.SizeMode = TabSizeMode.Fixed;
                tabs.ItemSize = new Size(node.TabWidth > 0 ? node.TabWidth : 100,
                    node.TabHeight > 0 ? node.TabHeight : 21);
            }
        }

        private static void DrawVerticalTab(object sender, DrawItemEventArgs e)
        {
            TabControl tabs = (TabControl)sender;
            if (e.Index < 0 || e.Index >= tabs.TabPages.Count) return;
            Rectangle bounds = e.Bounds;
            bool selected = e.Index == tabs.SelectedIndex;
            using (Bitmap tabImage = new Bitmap(Math.Max(1, bounds.Width),
                Math.Max(1, bounds.Height)))
            using (Graphics imageGraphics = Graphics.FromImage(tabImage))
            {
                imageGraphics.Clear(selected ? SystemColors.Window : SystemColors.Control);
                ControlPaint.DrawBorder(imageGraphics,
                    new Rectangle(0, 0, tabImage.Width, tabImage.Height),
                    SystemColors.ControlDark, ButtonBorderStyle.Solid);
                RectangleF textBounds;
                if (tabs.Alignment == TabAlignment.Left)
                {
                    imageGraphics.TranslateTransform(0F, tabImage.Height);
                    imageGraphics.RotateTransform(-90F);
                    textBounds = new RectangleF(0F, 0F, bounds.Height, bounds.Width);
                }
                else
                {
                    imageGraphics.TranslateTransform(tabImage.Width, 0F);
                    imageGraphics.RotateTransform(90F);
                    textBounds = new RectangleF(0F, 0F, bounds.Height, bounds.Width);
                }
                using (StringFormat format = new StringFormat())
                using (Brush text = new SolidBrush(SystemColors.ControlText))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    Font font = selected ? new Font(tabs.Font, FontStyle.Bold) : tabs.Font;
                    imageGraphics.DrawString(tabs.TabPages[e.Index].Text, font, text,
                        textBounds, format);
                    if (!object.ReferenceEquals(font, tabs.Font)) font.Dispose();
                }
                e.Graphics.DrawImageUnscaled(tabImage, bounds.Location);
            }
        }

        private static readonly Dictionary<string, string> EnglishCaptions =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Настройки", "Settings" }, { "Общие", "General" },
                { "Язык:", "Language:" }, { "Путь к игре:", "Game path:" },
                { "Полное логирование", "Full logging" }, { "Параметры", "Parameters" },
                { "Основные", "General" }, { "Редактировать", "Edit" },
                { "Удалить", "Delete" }, { "Добавить", "Add" },
                { "Закрыть", "Close" }, { "Отмена", "Cancel" },
                { "Имя:", "Name:" }, { "Тип:", "Type:" }, { "Количество:", "Quantity:" },
                { "Цена:", "Price:" }, { "Вес:", "Weight:" }, { "Состояние", "State" },
                { "Текст", "Text" }, { "Изображение", "Image" },
                { "Позиция", "Position" }, { "Размер", "Size" },
                { "Корабль", "Ship" }, { "Игрок", "Player" }, { "Станция", "Station" },
                { "Планета", "Planet" }, { "Галактика", "Galaxy" },
                { "Магазин", "Shop" }, { "Трюм", "Hold" }, { "Оборудование", "Equipment" },
                { "Оружие", "Weapon" }, { "Товар", "Goods" }, { "Карта сокровищ", "Treasure map" },
                { "Транклюкатор", "Tranclucator" }, { "Скрипт", "Script" },
                { "События", "Events" }, { "Сообщения", "Messages" },
                { "Моды", "Mods" }, { "Поиск", "Search" }, { "Лог", "Log" }
            };

        private static void LocalizeEnglish(Control root)
        {
            string translated;
            if (EditorLocalization.TryEnglish(root.Text, out translated) ||
                EnglishCaptions.TryGetValue(root.Text ?? string.Empty, out translated))
                root.Text = translated;
            foreach (Control child in root.Controls) LocalizeEnglish(child);
        }

        internal static void ConfigureTabPages(Form form, string tabControlName,
            params string[] visiblePageNames)
        {
            Dictionary<string, Control> controls = form.Tag as Dictionary<string, Control>;
            if (controls == null) throw new InvalidOperationException("Реестр контролов формы недоступен.");
            Control raw;
            if (!controls.TryGetValue(tabControlName, out raw) || !(raw is TabControl))
                throw new InvalidOperationException("Контейнер вкладок не найден: " + tabControlName);
            TabControl pages = (TabControl)raw;
            HashSet<string> visible = new HashSet<string>(visiblePageNames,
                StringComparer.OrdinalIgnoreCase);
            for (int index = pages.TabPages.Count - 1; index >= 0; index--)
                if (!visible.Contains(pages.TabPages[index].Name)) pages.TabPages.RemoveAt(index);
            foreach (string name in visiblePageNames)
            {
                Control page;
                if (controls.TryGetValue(name, out page) && page is TabPage &&
                    !pages.TabPages.Contains((TabPage)page))
                {
                    TabControl previous = page.Parent as TabControl;
                    if (previous != null) previous.TabPages.Remove((TabPage)page);
                    pages.TabPages.Add((TabPage)page);
                }
            }
            if (pages.TabPages.Count > 0) pages.SelectedIndex = 0;
            Relayout(form);
        }

        internal static void SetLayoutControlVisible(Form form, string controlName, bool visible)
        {
            AdaptiveEditorForm adaptive = form as AdaptiveEditorForm;
            Dictionary<string, Control> controls = form.Tag as Dictionary<string, Control>;
            Control control;
            if (adaptive == null || controls == null || adaptive.LayoutHidden == null ||
                !controls.TryGetValue(controlName, out control)) return;
            control.Visible = visible;
            if (visible) adaptive.LayoutHidden.Remove(control);
            else adaptive.LayoutHidden.Add(control);
        }

        internal static void ConfigurePlayerSections(Form form)
        {
            Dictionary<string, Control> controls = form.Tag as Dictionary<string, Control>;
            if (controls == null || controls.ContainsKey("$playerSections")) return;
            Control playerPageRaw, oldGroupRaw;
            if (!controls.TryGetValue("tsPlayer", out playerPageRaw) || !(playerPageRaw is TabPage) ||
                !controls.TryGetValue("gbPlayerShip", out oldGroupRaw) || !(oldGroupRaw is GroupBox)) return;
            TabPage playerPage = (TabPage)playerPageRaw;
            GroupBox oldGroup = (GroupBox)oldGroupRaw;

            Panel sections = new Panel();
            sections.Name = "$playerSections";
            sections.Font = form.Font;
            sections.BackColor = SystemColors.Control;
            FlowLayoutPanel navigation = new FlowLayoutPanel();
            navigation.Name = "$playerNavigation";
            navigation.WrapContents = false;
            navigation.FlowDirection = FlowDirection.LeftToRight;
            navigation.Padding = new Padding(0);
            navigation.Margin = new Padding(0);
            navigation.BackColor = SystemColors.Control;
            Panel content = new Panel();
            content.Name = "$playerContent";
            content.BackColor = SystemColors.Control;
            BufferedSectionPanel generalPage = PlayerPage("$playerGeneralPage", "Основное");
            BufferedSectionPanel financePage = PlayerPage("$playerFinancePage", "Финансы");
            BufferedSectionPanel journalPage = PlayerPage("$playerJournalPage", "Журнал");
            BufferedSectionPanel servicePage = PlayerPage("$playerServicePage", "Дополнительно");
            Panel[] pages = { generalPage, financePage, journalPage, servicePage };
            foreach (Panel page in pages)
            {
                page.Visible = false;
                content.Controls.Add(page);
            }
            sections.Controls.Add(content);
            sections.Controls.Add(navigation);
            playerPage.Controls.Add(sections);

            List<Button> navigationButtons = new List<Button>();
            foreach (Panel page in pages)
            {
                Button button = new Button();
                button.Name = "$show" + page.Name.TrimStart('$');
                button.Text = page.Text;
                button.Tag = page;
                button.Size = new Size(page == servicePage ? 142 : 112, 32);
                button.Margin = new Padding(0, 0, 4, 0);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = SystemColors.Control;
                button.Cursor = Cursors.Hand;
                navigationButtons.Add(button);
                navigation.Controls.Add(button);
            }
            Action<Panel> activatePage = delegate(Panel selected)
            {
                foreach (Panel page in pages)
                {
                    bool active = object.ReferenceEquals(page, selected);
                    page.Visible = active;
                    if (active) page.BringToFront();
                }
                foreach (Button button in navigationButtons)
                {
                    bool active = object.ReferenceEquals(button.Tag, selected);
                    button.BackColor = active ? SystemColors.Window : SystemColors.Control;
                    button.Font = active ? Bold : Regular;
                }
            };
            foreach (Button button in navigationButtons)
            {
                Button targetButton = button;
                targetButton.Click += delegate { activatePage((Panel)targetButton.Tag); };
            }

            GroupBox general = PlayerGroup("$playerGeneral", "Основные показатели", form.Font);
            GroupBox finance = PlayerGroup("$playerFinance", "Финансы и лицензии", form.Font);
            GroupBox service = PlayerGroup("$playerService", "Дополнительные параметры", form.Font);
            generalPage.Controls.Add(general); financePage.Controls.Add(finance);
            servicePage.Controls.Add(service);

            MoveNamedControls(controls, general, new string[] {
                "lblKillShipInGiperSpace", "edKillShipInGiperSpace", "lblKillShipInHole", "edKillShipInHole",
                "lblFlyToStar", "cbFlyToStar", "lblImmunity", "edImmunity",
                "lblDayWBGiveProgramms", "edDayWBGiveProgramms", "lblPlanetBattlesWin", "edPlanetBattlesWin",
                "lblLastPlanetBattleDate", "edLastPlanetBattleDate", "lblCntIll", "edCntIll",
                "lblCntStim", "edCntStim", "lblCntPrison", "edCntPrison",
                "lblUnkPlanetComplete", "edUnkPlanetComplete", "lblCntChangeRace", "edCntChangeRace",
                "lblCntChangeSide", "edCntChangeSide", "lblHotEquipmentCur", "edHotEquipmentCur",
                "lblGotoGov", "edGotoGov", "lblExpPointsForTrade", "edExpPointsForTrade",
                "lblHitEnemyAfterTakeProgramms", "edHitEnemyAfterTakeProgramms",
                "chbPlayerPrison", "chbTalkLocked", "chbScanLocked", "chbNoJump", "chbPirateClanReal"
            });
            MoveNamedControls(controls, finance, new string[] {
                "lblDebt", "edDebt", "lblDebtDate", "edDebtDate", "lblDebtCnt", "edDebtCnt",
                "lblDeposit", "edDeposit", "lblDepositDate", "edDepositDate",
                "lblDepositDay", "edDepositDay", "lblDepositPercent", "edDepositPercent",
                "lblMedPolicy", "edMedPolicy", "lblPirateLicense", "edPirateLicense",
                "lblPiratePoints", "edPiratePoints", "lblPirateNewPoints", "edPirateNewPoints"
            });

            MoveNamedControl(controls, journalPage, "gbJournal");
            MoveNamedControl(controls, journalPage, "gbPlanetNews");
            string[] serviceGroups = { "gbBridge", "gbRobotMap", "gbProgrammsInWB",
                "gbInvestmentDay", "gbKillDominatorsByType", "gbChameleonLogic" };
            foreach (string name in serviceGroups) MoveNamedControl(controls, servicePage, name);

            List<Control> remaining = new List<Control>();
            foreach (Control child in oldGroup.Controls) remaining.Add(child);
            foreach (Control child in remaining) service.Controls.Add(child);
            if (oldGroup.Parent != null) oldGroup.Parent.Controls.Remove(oldGroup);
            oldGroup.Visible = false;
            AdaptiveEditorForm adaptive = form as AdaptiveEditorForm;
            if (adaptive != null && adaptive.LayoutHidden != null) adaptive.LayoutHidden.Add(oldGroup);

            controls["$playerSections"] = sections;
            controls["$playerNavigation"] = navigation;
            controls["$playerContent"] = content;
            controls["$playerGeneralPage"] = generalPage;
            controls["$playerFinancePage"] = financePage;
            controls["$playerJournalPage"] = journalPage;
            controls["$playerServicePage"] = servicePage;
            controls["$playerGeneral"] = general;
            controls["$playerFinance"] = finance;
            controls["$playerService"] = service;
            activatePage(generalPage);
            Relayout(form);
        }

        private static BufferedSectionPanel PlayerPage(string name, string text)
        {
            BufferedSectionPanel page = new BufferedSectionPanel();
            page.Name = name; page.Text = text; page.AutoScroll = false;
            return page;
        }

        private static GroupBox PlayerGroup(string name, string text, Font font)
        {
            GroupBox group = new GroupBox();
            group.Name = name; group.Text = text; group.Font = font;
            return group;
        }

        private static void MoveNamedControls(Dictionary<string, Control> controls,
            Control target, IEnumerable<string> names)
        {
            foreach (string name in names) MoveNamedControl(controls, target, name);
        }

        private static void MoveNamedControl(Dictionary<string, Control> controls,
            Control target, string name)
        {
            Control control;
            if (!controls.TryGetValue(name, out control) || control == target) return;
            if (control.Parent != null) control.Parent.Controls.Remove(control);
            target.Controls.Add(control);
        }

        internal static void Relayout(Form form)
        {
            AdaptiveEditorForm adaptive = form as AdaptiveEditorForm;
            if (adaptive != null && adaptive.Relayout != null) adaptive.Relayout();
        }

        private static void EnsureScrollExtents(Control root)
        {
            foreach (Control child in root.Controls) EnsureScrollExtents(child);
            Panel panel = root as Panel;
            if (panel == null || !panel.AutoScroll) return;
            int right = 0, bottom = 0;
            foreach (Control child in panel.Controls)
            {
                right = Math.Max(right, child.Right);
                bottom = Math.Max(bottom, child.Bottom);
            }
            panel.AutoScrollMinSize = new Size(right + 10, bottom + 10);
        }

        private static void FitDialogToWorkingArea(Form form)
        {
            Rectangle work = Screen.FromControl(form).WorkingArea;
            int maximumWidth = Math.Max(480, work.Width - 24);
            int maximumHeight = Math.Max(360, work.Height - 24);
            if (form.Width <= maximumWidth && form.Height <= maximumHeight) return;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MaximizeBox = true;
            form.AutoScroll = true;
            form.Size = new Size(Math.Min(form.Width, maximumWidth),
                Math.Min(form.Height, maximumHeight));
            form.Location = new Point(work.Left + Math.Max(0, (work.Width - form.Width) / 2),
                work.Top + Math.Max(0, (work.Height - form.Height) / 2));
        }

        // Internal directory picker used by path editors. It never starts an
        // external tool and only appears after an explicit browse action.
        internal static bool SelectDirectory(IWin32Window owner, string initialPath,
            out string selectedPath)
        {
            selectedPath = null;
            EditorFormDefinition definition = EditorFormDefinitions.Get("TPATHDIALOGFORM");
            if (definition == null) return false;
            using (Form form = Build(definition))
            {
                TreeView tree = FindControl<TreeView>(form, "sShellTreeView1");
                Label selectedLabel = FindControl<Label>(form, "sLabel1");
                Button accept = FindControl<Button>(form, "sBitBtn1");
                Button cancel = FindControl<Button>(form, "sBitBtn2");
                Button create = FindControl<Button>(form, "sBitBtn3");
                if (tree == null || selectedLabel == null || accept == null ||
                    cancel == null || create == null) return false;

                form.Text = "Выбор папки";
                form.FormBorderStyle = FormBorderStyle.Sizable;
                form.MinimumSize = new Size(386, 454);
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                accept.DialogResult = DialogResult.OK;
                cancel.DialogResult = DialogResult.Cancel;
                form.AcceptButton = accept;
                form.CancelButton = cancel;
                create.Enabled = false; // TSETTINGSFORM has DialogOptions = []

                PopulateComputerRoots(tree);
                tree.BeforeExpand += PathTreeBeforeExpand;
                tree.AfterSelect += delegate
                {
                    string value = SelectedDirectory(tree);
                    selectedLabel.Text = value;
                    accept.Enabled = !string.IsNullOrEmpty(value) && Directory.Exists(value);
                };
                tree.NodeMouseDoubleClick += delegate
                {
                    if (accept.Enabled) form.DialogResult = DialogResult.OK;
                };
                create.Click += delegate
                {
                    string parent = SelectedDirectory(tree);
                    string name;
                    if (string.IsNullOrEmpty(parent) || !Directory.Exists(parent) ||
                        !PromptDirectoryName(form, out name)) return;
                    string path = Path.Combine(parent, name);
                    try
                    {
                        Directory.CreateDirectory(path);
                        TreeNode selected = tree.SelectedNode;
                        if (selected != null)
                        {
                            selected.Nodes.Clear();
                            AddDirectoryChildren(selected);
                            SelectTreePath(tree, path);
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(form, "Папка " + path + " не может быть создана.\r\n" +
                            error.Message, "Space Rangers HD Save Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                form.Shown += delegate
                {
                    string requested = Directory.Exists(initialPath) ?
                        Path.GetFullPath(initialPath) : string.Empty;
                    if (!string.IsNullOrEmpty(requested)) SelectTreePath(tree, requested);
                    if (tree.SelectedNode == null && tree.Nodes.Count != 0)
                        tree.SelectedNode = tree.Nodes[0];
                };

                if (form.ShowDialog(owner) != DialogResult.OK) return false;
                selectedPath = SelectedDirectory(tree);
                return !string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath);
            }
        }

        internal static bool ShowBonusCrcAlert(IWin32Window owner)
        {
            EditorFormDefinition definition = EditorFormDefinitions.Get("TBONUSALERTFORM");
            if (definition == null) return false;
            using (Form form = Build(definition))
            {
                Button correction = FindControl<Button>(form, "btnCorrection");
                Button readAsIs = FindControl<Button>(form, "btnReadAsIs");
                if (correction == null || readAsIs == null) return false;
                // Delphi ModalResult values are mrYesToAll (10) and
                // mrNoToAll (12); WinForms has no matching enum members, so
                // retain the two observable choices with explicit handlers.
                correction.Click += delegate { form.Tag = true; form.DialogResult = DialogResult.OK; };
                readAsIs.Click += delegate { form.Tag = false; form.DialogResult = DialogResult.Ignore; };
                form.AcceptButton = correction;
                form.CancelButton = readAsIs;
                DialogResult result = form.ShowDialog(owner);
                return result == DialogResult.OK && form.Tag is bool && (bool)form.Tag;
            }
        }

        private static void PopulateComputerRoots(TreeView tree)
        {
            tree.BeginUpdate();
            try
            {
                tree.Nodes.Clear();
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    string path = drive.RootDirectory.FullName;
                    string caption = path;
                    try
                    {
                        if (drive.IsReady && !string.IsNullOrEmpty(drive.VolumeLabel))
                            caption = drive.VolumeLabel + " (" + path.TrimEnd('\\') + ")";
                    }
                    catch { }
                    TreeNode node = new TreeNode(caption); node.Tag = path;
                    AddDirectoryPlaceholder(node); tree.Nodes.Add(node);
                }
            }
            finally { tree.EndUpdate(); }
        }

        private static void PathTreeBeforeExpand(object sender, TreeViewCancelEventArgs args)
        {
            if (args.Node.Nodes.Count == 1 && args.Node.Nodes[0].Tag == null)
            {
                args.Node.Nodes.Clear();
                AddDirectoryChildren(args.Node);
            }
        }

        private static void AddDirectoryPlaceholder(TreeNode node)
        {
            string path = node.Tag as string;
            try
            {
                if (!string.IsNullOrEmpty(path) && Directory.EnumerateDirectories(path).GetEnumerator().MoveNext())
                    node.Nodes.Add(new TreeNode(string.Empty));
            }
            catch { }
        }

        private static void AddDirectoryChildren(TreeNode node)
        {
            string path = node.Tag as string;
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                List<string> directories = new List<string>(Directory.EnumerateDirectories(path));
                directories.Sort(StringComparer.CurrentCultureIgnoreCase);
                foreach (string directory in directories)
                {
                    DirectoryInfo info = new DirectoryInfo(directory);
                    TreeNode child = new TreeNode(info.Name); child.Tag = info.FullName;
                    AddDirectoryPlaceholder(child); node.Nodes.Add(child);
                }
            }
            catch { }
        }

        private static void SelectTreePath(TreeView tree, string path)
        {
            string full;
            try { full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar); }
            catch { return; }
            TreeNode current = null;
            foreach (TreeNode root in tree.Nodes)
            {
                string rootPath = root.Tag as string;
                if (!string.IsNullOrEmpty(rootPath) &&
                    full.StartsWith(rootPath.TrimEnd(Path.DirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase))
                {
                    current = root; break;
                }
            }
            if (current == null) return;
            string currentPath = ((string)current.Tag).TrimEnd(Path.DirectorySeparatorChar);
            if (current.Nodes.Count == 1 && current.Nodes[0].Tag == null)
            {
                current.Nodes.Clear(); AddDirectoryChildren(current);
            }
            while (!string.Equals(currentPath, full, StringComparison.OrdinalIgnoreCase))
            {
                TreeNode next = null;
                foreach (TreeNode child in current.Nodes)
                {
                    string childPath = child.Tag as string;
                    if (!string.IsNullOrEmpty(childPath) &&
                        (string.Equals(childPath.TrimEnd(Path.DirectorySeparatorChar), full,
                            StringComparison.OrdinalIgnoreCase) ||
                         full.StartsWith(childPath.TrimEnd(Path.DirectorySeparatorChar) +
                            Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                    {
                        next = child; break;
                    }
                }
                if (next == null) break;
                current.Expand(); current = next;
                currentPath = ((string)current.Tag).TrimEnd(Path.DirectorySeparatorChar);
                if (current.Nodes.Count == 1 && current.Nodes[0].Tag == null &&
                    !string.Equals(currentPath, full, StringComparison.OrdinalIgnoreCase))
                {
                    current.Nodes.Clear(); AddDirectoryChildren(current);
                }
            }
            tree.SelectedNode = current;
            current.EnsureVisible();
        }

        private static string SelectedDirectory(TreeView tree)
        {
            return tree == null || tree.SelectedNode == null ? null : tree.SelectedNode.Tag as string;
        }

        private static bool PromptDirectoryName(IWin32Window owner, out string name)
        {
            name = null;
            using (Form prompt = new Form())
            {
                prompt.Text = "Создание папки";
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.ClientSize = new Size(330, 92);
                prompt.MaximizeBox = false; prompt.MinimizeBox = false;
                Label label = new Label { Text = "Имя новой папки:", AutoSize = true,
                    Location = new Point(10, 12) };
                TextBox edit = new TextBox { Location = new Point(10, 32), Width = 310 };
                Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK,
                    Location = new Point(164, 61), Width = 75 };
                Button cancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel,
                    Location = new Point(245, 61), Width = 75 };
                prompt.Controls.AddRange(new Control[] { label, edit, ok, cancel });
                prompt.AcceptButton = ok; prompt.CancelButton = cancel;
                if (prompt.ShowDialog(owner) != DialogResult.OK) return false;
                string value = (edit.Text ?? string.Empty).Trim();
                if (value.Length == 0 || value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;
                name = value; return true;
            }
        }

        private static T FindControl<T>(Control root, string name) where T : Control
        {
            if (root == null) return null;
            if (root is T && string.Equals(root.Name, name, StringComparison.OrdinalIgnoreCase))
                return (T)root;
            foreach (Control child in root.Controls)
            {
                T found = FindControl<T>(child, name);
                if (found != null) return found;
            }
            Form form = root as Form ?? root.FindForm();
            Dictionary<string, Control> controls = form == null ? null :
                form.Tag as Dictionary<string, Control>;
            Control registered;
            if (controls != null && controls.TryGetValue(name, out registered))
                return registered as T;
            return null;
        }

        private static void BindListContextPopups(string resource,
            Dictionary<string, Control> controls)
        {
            string[] names;
            switch (resource)
            {
                case "TGALAXYFORM":
                    names = new string[] { "lbPlanetNews", "lbOldQuest", "lbWarOperations", "lbGates" };
                    break;
                case "TPLANETFORM":
                    names = new string[] { "lbWarriors", "lbSputniks", "lbRelationToRangers",
                        "lbEquipmentShop", "lbGoneItems" };
                    break;
                case "TSCRIPTFORM":
                    names = new string[] { "lbInitVars", "lbItems", "lbShips", "lbTurnVars" };
                    break;
                case "TSHIPFORM":
                    names = new string[] { "lbTakeItems", "lbRelationToRangers", "lbIllness",
                        "lbRecentlyDroppedItems", "lbSpecialBonuses", "lbStatusEffects", "lbRewards",
                        "lbQuests", "lbJournal", "lbPlanetNews", "lbRobotMaps", "lbEquipments",
                        "lbArtefacts", "lbEquipmentShop", "lbSaleSatellites", "lbDropList",
                        "lbCustomShipInfos" };
                    break;
                case "TSTARFORM":
                    names = new string[] { "lbItemsDrop", "lbCustomStarInfo" };
                    break;
                case "TWAROPERATIONFORM":
                    names = new string[] { "lbShips", "lbOrders" };
                    break;
                default:
                    return;
            }

            foreach (string name in names)
            {
                Control control;
                ListBox list;
                if (!controls.TryGetValue(name, out control) || (list = control as ListBox) == null)
                    continue;
                list.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                        ApplyContextPopupSelection((ListBox)sender, args.Location);
                };
            }
        }

        internal static void ApplyContextPopupSelection(ListBox list, Point location)
        {
            if (list == null) return;
            int index = list.IndexFromPoint(location);
            if (index == ListBox.NoMatches)
            {
                list.ClearSelected();
                list.SelectedIndex = -1;
                return;
            }
            if (!list.GetSelected(index))
            {
                list.ClearSelected();
                list.SetSelected(index, true);
            }
            list.SelectedIndex = index;
        }

        private static Control Create(EditorNodeDefinition node)
        {
            switch (node.Kind)
            {
                case "group": return new GroupBox();
                case "label": return new Label { AutoSize = true, BackColor = Color.Transparent };
                case "button": return new Button { UseVisualStyleBackColor = true };
                case "checkbox": return new CheckBox { AutoSize = node.Width <= 1 || node.Height <= 1, Checked = node.Checked, UseVisualStyleBackColor = true };
                case "radio": return new RadioButton { AutoSize = true, Checked = node.Checked, UseVisualStyleBackColor = true };
                case "combo": return new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                case "memo": return new TextBox { Multiline = true, ScrollBars = ScrollBars.Both, ReadOnly = node.ReadOnly };
                case "edit": return new TextBox { ReadOnly = node.ReadOnly };
                case "directory": return new DirectoryEditControl();
                case "list": return new ListBox();
                case "owner-list": return new AdaptiveOwnerDrawListBox();
                case "checklist": return new CheckedListBox();
                case "listview": return new ListView { View = View.Details };
                case "tree": return new TreeView();
                case "tabs": return new ExactTabControl { Padding = new Point(10, 4) };
                case "tab": return new BufferedTabPage();
                case "image": return new PictureBox { SizeMode = PictureBoxSizeMode.StretchImage, BackColor = Color.Transparent };
                case "progress": return new ProgressBar();
                case "track": return new TrackBar();
                case "grid": return new DataGridView { ReadOnly = node.ReadOnly, AllowUserToAddRows = false };
                case "scroll": return new Panel { AutoScroll = true };
                case "splitter": return new Splitter();
                default: return new Panel();
            }
        }

        private static void ApplyItems(Control control, EditorNodeDefinition node)
        {
            if (node.Items != null)
            {
                if (control is ComboBox) ((ComboBox)control).Items.AddRange(node.Items);
                else if (control is ListBox) ((ListBox)control).Items.AddRange(node.Items);
                else if (control is CheckedListBox) ((CheckedListBox)control).Items.AddRange(node.Items);
            }
            if (control is ComboBox && node.ItemIndex >= -1 && node.ItemIndex < ((ComboBox)control).Items.Count)
                ((ComboBox)control).SelectedIndex = node.ItemIndex;
            else if (control is ListBox && node.ItemIndex >= -1 && node.ItemIndex < ((ListBox)control).Items.Count)
                ((ListBox)control).SelectedIndex = node.ItemIndex;
        }
    }
}
