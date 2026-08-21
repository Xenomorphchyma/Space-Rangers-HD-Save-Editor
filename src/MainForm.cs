using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceRangersHdSaveEditor
{
    internal sealed class ExactTabControl : TabControl
    {
        private const int TcmSetItemSize = 0x1329;
        internal int ExactItemWidth;
        internal int ExactItemHeight = 25;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int packedSize = (ExactItemHeight << 16) | (ExactItemWidth & 0xffff);
            SendMessage(Handle, TcmSetItemSize, IntPtr.Zero, new IntPtr(packedSize));
        }
    }

    internal sealed class InteractivePictureBox : PictureBox
    {
        private Point previewOffset = Point.Empty;

        internal InteractivePictureBox()
        {
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            TabStop = true;
        }

        internal Point PreviewOffset
        {
            get { return previewOffset; }
            set
            {
                if (previewOffset == value) return;
                previewOffset = value;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null || previewOffset == Point.Empty)
            {
                base.OnPaint(e);
                return;
            }
            e.Graphics.Clear(BackColor);
            e.Graphics.DrawImageUnscaled(Image, previewOffset);
        }
    }

    internal sealed class FileBanner : Control
    {
        internal string FileNameText = "";
        internal string VersionText = "";

        internal FileBanner()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle border = new Rectangle(0, 0, Width - 1, Height - 1);
            using (Pen pen = new Pen(Color.Black))
            {
                pen.DashStyle = DashStyle.Dot;
                e.Graphics.DrawRectangle(pen, border);
            }
            if (string.IsNullOrEmpty(FileNameText))
                return;
            Font bold = new Font(Font, FontStyle.Bold);
            string suffix = " " + VersionText + " [support]";
            Size leftSize = TextRenderer.MeasureText(FileNameText, bold, Size.Empty, TextFormatFlags.NoPadding);
            Size suffixSize = TextRenderer.MeasureText(suffix, bold, Size.Empty, TextFormatFlags.NoPadding);
            int x = (Width - leftSize.Width - suffixSize.Width) / 2;
            int y = (Height - leftSize.Height) / 2;
            TextRenderer.DrawText(e.Graphics, FileNameText, bold, new Point(x, y), Color.Black, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(e.Graphics, suffix, bold, new Point(x + leftSize.Width, y), Color.Green, TextFormatFlags.NoPadding);
            bold.Dispose();
        }
    }

    internal sealed class SearchResultEntry
    {
        internal readonly object Value;
        internal readonly string Caption;
        internal readonly Color? FirstLineColor;

        internal SearchResultEntry(object value, string caption)
            : this(value, caption, null)
        {
        }

        internal SearchResultEntry(object value, string caption, Color? firstLineColor)
        {
            Value = value;
            Caption = caption;
            FirstLineColor = firstLineColor;
        }

        public override string ToString()
        {
            return Caption;
        }
    }

    internal sealed class ItemTypeSearchChoice
    {
        internal readonly string Caption;
        internal readonly int FirstType;
        internal readonly int LastType;

        internal ItemTypeSearchChoice(string caption, int firstType, int lastType)
        {
            Caption = caption; FirstType = firstType; LastType = lastType;
        }

        internal bool Matches(byte type)
        {
            return FirstType < 0 || type >= FirstType && type <= LastType;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class ModInfoShipEntry
    {
        internal readonly ShipHeaderRecord Owner;
        internal readonly CustomShipInfoRecord Record;

        internal ModInfoShipEntry(ShipHeaderRecord owner, CustomShipInfoRecord record)
        {
            Owner = owner; Record = record;
        }

        public override string ToString()
        {
            string owner = string.IsNullOrEmpty(Owner.Name) ? Owner.ObjectId.ToString(CultureInfo.InvariantCulture) : Owner.Name;
            string name = string.IsNullOrEmpty(Record.Name) ? "TCustomShipInfo" : Record.Name;
            return name + " [" + owner + "]";
        }
    }

    internal sealed class ModInfoStarEntry
    {
        internal readonly StarHeaderRecord Owner;
        internal readonly CustomSystemInfoRecord Record;

        internal ModInfoStarEntry(StarHeaderRecord owner, CustomSystemInfoRecord record)
        {
            Owner = owner; Record = record;
        }

        public override string ToString()
        {
            string owner = string.IsNullOrEmpty(Owner.Name) ? Owner.ObjectId.ToString(CultureInfo.InvariantCulture) : Owner.Name;
            string name = string.IsNullOrEmpty(Record.Name) ? "TCustomSystemInfo" : Record.Name;
            return name + " [" + owner + "]";
        }
    }

    internal sealed class StarMapHitRecord
    {
        internal object Value;
        internal PointF Point;
        internal RectangleF HitBox;
    }

    internal sealed class SystemJumpPointRecord
    {
        internal StarHeaderRecord TargetStar;
        internal PointF WorldPoint;

        public override string ToString()
        {
            return TargetStar == null ? "Точка перехода" : "Переход в систему " + TargetStar.Name;
        }
    }

    internal sealed class GalaxyLabelSegment
    {
        internal string Text;
        internal Color Color;
    }

    internal sealed class MissileReferenceChoice
    {
        internal readonly byte Type;
        internal readonly uint ObjectId;
        internal readonly string Caption;

        internal MissileReferenceChoice(byte type, uint objectId, string caption)
        {
            Type = type; ObjectId = objectId; Caption = caption;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class ByteValueChoice
    {
        internal readonly byte Value;
        internal readonly string Caption;

        internal ByteValueChoice(byte value, string caption)
        {
            Value = value; Caption = caption;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class UInt32ValueChoice
    {
        internal readonly uint Value;
        internal readonly string Caption;

        internal UInt32ValueChoice(uint value, string caption)
        {
            Value = value; Caption = caption;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class MicroModuleReferenceChoice
    {
        internal readonly int Index;
        internal readonly uint ReferenceId;
        internal readonly string BlockName;
        internal readonly string Caption;

        internal MicroModuleReferenceChoice(int index, uint referenceId, string blockName,
            string caption)
        {
            Index = index; ReferenceId = referenceId; BlockName = blockName ?? string.Empty;
            Caption = caption ?? string.Empty;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class HullSeriesReferenceChoice
    {
        internal readonly int Index;
        internal readonly uint ReferenceId;
        internal readonly string BlockName;
        internal readonly string Caption;

        internal HullSeriesReferenceChoice(int index, uint referenceId, string blockName,
            string caption)
        {
            Index = index; ReferenceId = referenceId; BlockName = blockName ?? string.Empty;
            Caption = caption ?? string.Empty;
        }

        public override string ToString() { return Caption; }
    }

    internal sealed class ItemExtraSpecialEditorRow
    {
        internal ItemExtraSpecialRecord Record;
        internal GroupBox Group;
        internal TextBox Number;
        internal TextBox BlockName;
        internal TextBox Crc;
        internal TextBox Count;
        internal ComboBox Name;
    }

    internal sealed class ItemEquipmentEditorState
    {
        internal TextBox Bonus;
        internal TextBox BonusBlock;
        internal TextBox BonusCrc;
        internal ComboBox BonusName;
        internal TextBox Special;
        internal TextBox SpecialBlock;
        internal TextBox SpecialCrc;
        internal ComboBox SpecialName;
        internal Panel ExtraPanel;
        internal List<ItemExtraSpecialRecord> ExtraSpecials = new List<ItemExtraSpecialRecord>();
        internal List<ItemExtraSpecialEditorRow> Rows = new List<ItemExtraSpecialEditorRow>();
    }

    internal sealed class CrcReferenceAuditResult
    {
        internal readonly List<string> Problems = new List<string>();
        internal readonly List<string> Corrections = new List<string>();
    }

    // Equipment references carry both a serialized list index and a stable CRC.
    // When the catalog order changes, prefer the CRC and repair the stale index.
    internal static class CrcReferencePolicy
    {
        internal static CrcReferenceAuditResult Apply(GameDataCatalog catalog,
            IList<ItemHeaderRecord> items, IList<MissileRecord> missiles, bool correct)
        {
            CrcReferenceAuditResult result = new CrcReferenceAuditResult();
            HashSet<string> problemKeys = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> correctionKeys = new HashSet<string>(StringComparer.Ordinal);
            if (catalog == null || !catalog.IsAvailable) return result;

            if (items != null)
                foreach (ItemHeaderRecord item in items)
                {
                    AuditMicro(catalog, "TItem", item.ObjectId, item.Start, "bonus",
                        ref item.Bonus, ref item.BonusReferenceId, correct, result,
                        problemKeys, correctionKeys);
                    AuditMicro(catalog, "TItem", item.ObjectId, item.Start, "special",
                        ref item.Special, ref item.SpecialReferenceId, correct, result,
                        problemKeys, correctionKeys);
                    if (item.ExtraSpecials != null)
                    {
                        for (int index = item.ExtraSpecials.Count - 1; index >= 0; index--)
                        {
                            ItemExtraSpecialRecord extra = item.ExtraSpecials[index];
                            int value = extra.Special; uint crc = extra.ReferenceId;
                            bool keep = AuditMicro(catalog, "TItem", item.ObjectId, item.Start,
                                "extra-special " + index.ToString(CultureInfo.InvariantCulture),
                                ref value, ref crc, correct, result, problemKeys, correctionKeys);
                            if (correct && !keep) item.ExtraSpecials.RemoveAt(index);
                            else { extra.Special = value; extra.ReferenceId = crc; }
                        }
                    }
                    AuditHullSeries(catalog, item, correct, result, problemKeys, correctionKeys);
                }

            if (missiles != null)
                foreach (MissileRecord missile in missiles)
                {
                    AuditMicro(catalog, "TMissile", missile.ObjectId, missile.Start, "bonus",
                        ref missile.Bonus, ref missile.BonusReferenceId, correct, result,
                        problemKeys, correctionKeys);
                    AuditMicro(catalog, "TMissile", missile.ObjectId, missile.Start, "special",
                        ref missile.Special, ref missile.SpecialReferenceId, correct, result,
                        problemKeys, correctionKeys);
                }
            return result;
        }

        private static bool AuditMicro(GameDataCatalog catalog, string kind, uint objectId,
            int start, string field, ref int index, ref uint crc, bool correct,
            CrcReferenceAuditResult result, HashSet<string> problemKeys,
            HashSet<string> correctionKeys)
        {
            if (index == 0 && crc == 0) return true;
            MicroModuleCatalogEntry entry = index > 0 && crc != 0 ?
                catalog.FindMicroModule(index, crc) : null;
            bool valid = entry != null && entry.Index == index && entry.ReferenceId == crc;
            if (valid) return true;
            string key = kind + ":" + start.ToString(CultureInfo.InvariantCulture) + ":" + field;
            if (problemKeys.Add(key))
                result.Problems.Add(kind + " ID=" + objectId.ToString(CultureInfo.InvariantCulture) +
                    " " + field + ": index=" + index.ToString(CultureInfo.InvariantCulture) +
                    ", CRC=" + crc.ToString("X8", CultureInfo.InvariantCulture));
            if (!correct) return true;

            if (entry != null && entry.ReferenceId == crc)
            {
                int previous = index; index = entry.Index;
                if (correctionKeys.Add(key))
                    result.Corrections.Add(kind + " ID=" + objectId.ToString(CultureInfo.InvariantCulture) +
                        " " + field + ": index " + previous.ToString(CultureInfo.InvariantCulture) +
                        " заменён на " + index.ToString(CultureInfo.InvariantCulture));
                return true;
            }

            index = 0; crc = 0;
            if (correctionKeys.Add(key))
                result.Corrections.Add(kind + " ID=" + objectId.ToString(CultureInfo.InvariantCulture) +
                    " " + field + ": CRC не найден, ссылка удалена");
            return false;
        }

        private static void AuditHullSeries(GameDataCatalog catalog, ItemHeaderRecord item,
            bool correct, CrcReferenceAuditResult result, HashSet<string> problemKeys,
            HashSet<string> correctionKeys)
        {
            if (item.DerivedFields == null) return;
            ItemDerivedField number = null, crcField = null;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                if (field.ControlName == "edSeriesNum") number = field;
                else if (field.ControlName == "edSeriesCRC") crcField = field;
            }
            if (number == null) return;
            int index = checked((int)number.IntegerValue);
            uint crc = crcField == null ? 0u : checked((uint)crcField.IntegerValue);
            if (index == -1 && crcField == null) return;
            HullSeriesCatalogEntry entry = index >= 0 && crc != 0 ?
                catalog.FindHullSeries(index, crc) : null;
            if (entry != null && entry.Index == index && entry.ReferenceId == crc) return;

            string key = "THull:" + item.Start.ToString(CultureInfo.InvariantCulture) + ":series";
            if (problemKeys.Add(key))
                result.Problems.Add("THull ID=" + item.ObjectId.ToString(CultureInfo.InvariantCulture) +
                    " series: index=" + index.ToString(CultureInfo.InvariantCulture) +
                    ", CRC=" + crc.ToString("X8", CultureInfo.InvariantCulture));
            if (!correct) return;
            if (entry != null && entry.ReferenceId == crc)
            {
                number.IntegerValue = entry.Index;
                if (correctionKeys.Add(key))
                    result.Corrections.Add("THull ID=" + item.ObjectId.ToString(CultureInfo.InvariantCulture) +
                        " series: index " + index.ToString(CultureInfo.InvariantCulture) +
                        " заменён на " + entry.Index.ToString(CultureInfo.InvariantCulture));
                return;
            }

            number.IntegerValue = -1;
            if (crcField != null) item.DerivedFields.Remove(crcField);
            if (correctionKeys.Add(key))
                result.Corrections.Add("THull ID=" + item.ObjectId.ToString(CultureInfo.InvariantCulture) +
                    " series: CRC не найден, серия удалена");
        }
    }

    internal sealed class MainForm : Form
    {
        private static readonly string[] specialBonusTypeNames = {
            "bonHull", "bonFuel", "bonSpeed", "bonJump", "bonRadar", "bonScan", "bonDroid",
            "bonHook", "bonDef", "bonWEnergy", "bonWSplinter", "bonWMissile", "bonWRadius",
            "bonSlotRadar", "bonSlotScaner", "bonSlotDroid", "bonSlotHook", "bonSlotDef",
            "bonSlotWeapon", "bonSlotArt", "bonSlotForsage", "bonHookRadius", "bonSkill1",
            "bonSkill2", "bonSkill3", "bonSkill4", "bonSkill5", "bonSkill6", "bonMass",
            "bonExtraAkrinEff", "bonExtraAkrinPenalty", "bonAmmo", "bonShots", "bonMissileSpeed",
            "bonShotSpeed", "bonHookMaxSpeed", "bonHookMinSpeed", "bonStimCapacity", "bonZonds",
            "bonAttacks", "bonResistAsteroid", "bonAIValue", "bonNull"
        };
        private static readonly string[] statusEffectTypeNames = {
            "steShock", "steAcid", "steMagnetic", "steWeaponBlock", "steDroidBlock",
            "steBWBuff", "steBWRepairDebuff"
        };
        private static readonly string[] searchItemTypeKeys = {
            "ITEMTYPE_FOOD", "ITEMTYPE_MEDICINE", "ITEMTYPE_TECHNICS", "ITEMTYPE_LUXURY",
            "ITEMTYPE_MINERALS", "ITEMTYPE_ALCOHOL", "ITEMTYPE_ARMS", "ITEMTYPE_NARCOTICS",
            "ITEMTYPE_ARTEFACT", "ITEMTYPE_ARTEFACT2", "ITEMTYPE_ART_HULL", "ITEMTYPE_ART_FUEL",
            "ITEMTYPE_ART_SPEED", "ITEMTYPE_ART_POWER", "ITEMTYPE_ART_RADAR", "ITEMTYPE_ART_SCANER",
            "ITEMTYPE_ART_DROID", "ITEMTYPE_ART_NANO", "ITEMTYPE_ART_HOOK", "ITEMTYPE_ART_DEF",
            "ITEMTYPE_ART_ANALYZER", "ITEMTYPE_ART_MINIEXPL", "ITEMTYPE_ART_ANTIGRAV",
            "ITEMTYPE_ART_TRANSMITTER", "ITEMTYPE_ART_BOMB", "ITEMTYPE_ART_TRANCLUCATOR",
            "ITEMTYPE_ART_DEF_TO_ENERGY", "ITEMTYPE_ART_ENERGY_PULSE", "ITEMTYPE_ART_ENERGY_DEF",
            "ITEMTYPE_ART_SPLINTER", "ITEMTYPE_ART_DECELERATE", "ITEMTYPE_ART_MISSILE_DEF",
            "ITEMTYPE_ART_FORSAGE", "ITEMTYPE_ART_WEAPON_TO_SPEED", "ITEMTYPE_ART_GIPER_JUMP",
            "ITEMTYPE_ART_BLACK_HOLE", "ITEMTYPE_ART_DEF_TO_ARMS1", "ITEMTYPE_ART_DEF_TO_ARMS2",
            "ITEMTYPE_ART_ARTEFACTOR", "ITEMTYPE_ART_BIO", "ITEMTYPE_ART_PD_TURRET",
            "ITEMTYPE_ART_FAST_RACKS", "ITEMTYPE_HULL", "ITEMTYPE_FUELTANKS", "ITEMTYPE_ENGINE",
            "ITEMTYPE_RADAR", "ITEMTYPE_SCANER", "ITEMTYPE_REPAIRROBOT", "ITEMTYPE_CARGOHOOK",
            "ITEMTYPE_DEFGENERATOR", "ITEMTYPE_WEAPON1", "ITEMTYPE_WEAPON2", "ITEMTYPE_WEAPON3",
            "ITEMTYPE_WEAPON4", "ITEMTYPE_WEAPON5", "ITEMTYPE_WEAPON6", "ITEMTYPE_WEAPON7",
            "ITEMTYPE_WEAPON8", "ITEMTYPE_WEAPON9", "ITEMTYPE_WEAPON10", "ITEMTYPE_WEAPON11",
            "ITEMTYPE_WEAPON12", "ITEMTYPE_WEAPON13", "ITEMTYPE_WEAPON14", "ITEMTYPE_WEAPON15",
            "ITEMTYPE_WEAPON16", "ITEMTYPE_WEAPON17", "ITEMTYPE_WEAPON18", "ITEMTYPE_CUSTOM_WEAPON",
            "ITEMTYPE_NODS", "ITEMTYPE_USELESS_ITEM", "ITEMTYPE_MICROMODULE", "ITEMTYPE_CISTERN",
            "ITEMTYPE_SATELLITE", "ITEMTYPE_TREASURE_MAP", "ITEMTYPE_USELESS_COUNTABLE"
        };
        private static readonly string[] searchItemTypeNamesEn = {
            "Food", "Medicine", "Technics", "Luxury", "Minerals", "Alcohol", "Arms", "Narcotics",
            "Artefact", "Artefact 2", "Iron Zoopie", "Black Goo", "Matter Psi-Accelerator", "Frostix",
            "Prolonger", "Scanner Cache", "Junior Droid", "Nanitoids", "Erimeter", "Polarizer",
            "Probability Analyzer", "Blast Wave Localizer", "Antigravitazer", "Transfactorial Beacon",
            "Quark bomb", "Tranclucator", "Proportionar", "Fiver", "Swallower", "Screw-on", "Zing",
            "Rocketang", "Oblivion Connector", "Nozzlenator", "Hypergenerator", "Subportal", "Proton",
            "Arms", "Artefactor", "Bioworld", "a'Egis", "Rals", "Hull", "Fuel Tanks", "Engine", "Radar",
            "Scaner", "Repair Robot", "Cargo Hook", "Def Generator", "Industrial Laser",
            "Fragmentation Cannon", "Flux", "Missile Launcher", "Treton", "Wave Phaser", "Flow Blaster",
            "Electron Cutter", "Multiresonator", "Atomic Vision", "Disintegrator", "Turbogravitron",
            "IMHO-9000", "Vertix", "Torpedo Tube", "Esodapher", "Caphasitor", "Lirecron",
            "Custom weapon", "Nods", "Useless item", "Micro Module", "Cistern", "Satellite",
            "Treasure Map", "Container"
        };
        private static readonly string[] searchItemTypeNamesRu = {
            "Еда", "Медикаменты", "Техника", "Роскошь", "Минералы", "Алкоголь", "Оружие", "Наркотики",
            "Артефакт", "Артефакт 2", "Железные жупи", "Черная жижа", "Пси-ускоритель материи",
            "Отморозки", "Пролонгер", "Сканерный кэш", "Дроид младший", "Нанитоиды", "Эриметр",
            "Поляризатор", "Вероятностный анализатор", "Локализатор взрывной волны", "Антигравитатор",
            "Трансфакторный маяк", "Кварковая бомба", "Транклюкатор", "Пропорционар", "Пятерик",
            "Проглот", "Навинт", "Вжик", "Ракетанг", "Обливионный коннектор", "Сопланатор",
            "Гипергенератор", "Субпортал", "Протон", "Армс", "Артефактор", "Биомир", "а'Эгис", "Ралс",
            "Корпус", "Топливный бак", "Двигатель", "Радар", "Сканер", "Ремонтный робот", "Захват",
            "Генератор защиты", "Промышленный лазер", "Осколочная пушка", "Флюктуационный излучатель",
            "Ракетница", "Третон", "Волновой фазер", "Потоковый бластер", "Электронный резак",
            "Мультирезонатор", "Атомное зрение", "Дезинтегратор", "Турбогравир", "ИМХО-9000",
            "Вертикс", "Торпедный аппарат", "Эсодафер", "Кафаситор", "Лирекрон",
            "Пользовательское оружие", "Ноды", "Ошмёток",
            "Микромодуль", "Цистерна", "Спутник", "Карта сокровищ", "Счётный предмет"
        };
        private readonly Font regularFont = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        private readonly Font boldFont = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
        private readonly Font compactValueFont = new Font("Segoe UI", 7.25F, FontStyle.Bold, GraphicsUnit.Point);
        private readonly Font tinyValueFont = new Font("Segoe UI", 6.5F, FontStyle.Bold, GraphicsUnit.Point);
        private readonly Dictionary<string, Label> values = new Dictionary<string, Label>();
        private readonly List<Control> requiresSave = new List<Control>();
        private readonly AppSettings appSettings = AppSettings.Load();
        private readonly ToolTip statusToolTip = new ToolTip();
        private FileBanner banner;
        private PictureBox previewImage;
        private PictureBox mapImage;
        private PictureBox galaxyMapImage;
        private TabControl mainTabs;
        private TabPage galaxyPage;
        private readonly List<StarMapHitRecord> galaxyMapHits = new List<StarMapHitRecord>();
        private float galaxyMapZoom = 1.0F;
        private PointF galaxyMapPan = PointF.Empty;
        private bool galaxyMapDragging;
        private Point galaxyMapDragStart;
        private PointF galaxyMapDragPanStart;
        private StarMapHitRecord galaxyMapPressedHit;
        private Form systemMapForm;
        private List<StarMapHitRecord> systemMapHits;
        private bool showSystemJumpPoints;
        private Label statusGame;
        private Label statusCrc;
        private Label statusRead;
        private Label statusItems;
        private Label statusLegal;
        private Label statusDump;
        private Button saveButton;
        private ListBox messageList;
        private RichTextBox messageText;
        private ListBox galaxyEventList;
        private ListBox customWeaponList;
        private ListBox storedItemList;
        private ListBox[] interfaceOverrideLists;
        private ListBox modInfoShipList;
        private ListBox modInfoStarList;
        private CheckBox modInfoShipsEnabled;
        private ListBox constellationList;
        private ListBox starList;
        private ListBox galaxyObjectList;
        private readonly Dictionary<string, CheckBox> galaxyFilters = new Dictionary<string, CheckBox>();
        private CheckBox galaxyObjectMaster;
        private CheckBox galaxyShipMaster;
        private ListBox itemList;
        private ListBox satelliteList;
        private ListBox scriptList;
        private ListBox globalVariableList;
        private ListBox scriptCacheList;
        private TextBox searchQuery;
        private TextBox searchId;
        private ComboBox searchItemType;
        private ListBox searchResults;
        private readonly Dictionary<string, CheckBox> searchFilters = new Dictionary<string, CheckBox>();
        private CheckBox searchFilterMaster;
        private TextBox[] logViews;
        private TextBox logSearch;
        private Label logSearchFound;
        private readonly List<string> crcReferenceProblems = new List<string>();
        private readonly List<string> crcReferenceCorrections = new List<string>();
        private bool crcReferencesReadAsIs;
        private SavContainer current;
        private SavMetadata pendingMetadata;
        private List<PlayerMessageRecord> pendingMessages;
        private GalaxyPrefixData pendingGalaxy;
        private GalaxySummaryData pendingGalaxySummary;
        private List<ConstellationRecord> pendingConstellations;
        private List<StarHeaderRecord> pendingStars;
        private List<PlanetHeaderRecord> pendingPlanets;
        private List<ShipHeaderRecord> pendingShips;
        private List<ItemHeaderRecord> pendingItems;
        private readonly HashSet<int> pendingDeletedItemStarts = new HashSet<int>();
        private List<HoleRecord> pendingHoles;
        private List<AsteroidRecord> pendingAsteroids;
        private List<MissileRecord> pendingMissiles;
        private List<CustomWeaponInfoRecord> pendingCustomWeapons;
        private List<InterfaceOverrideRecord> pendingInterfaceOverrides;
        private List<StoredItemRecord> pendingStoredItems;
        private AchievementStatsRecord pendingAchievements;
        private GameDataCatalog gameCatalog = new GameDataCatalog();
        private string startupPath;
        private bool suppressLoadPrompts;
        private Panel loadingOverlay;
        private Panel loadingCard;
        private Label loadingLabel;
        private ProgressBar loadingProgress;
        private bool isLoading;
        private bool persistUiSettings = true;

        private sealed class SaveLoadResult
        {
            internal SavContainer Save;
            internal GameDataCatalog Catalog;
        }

        internal MainForm() : this(null)
        {
        }

        internal MainForm(int? languageOverride)
        {
            if (languageOverride.HasValue)
            {
                appSettings.LanguageIndex = Math.Max(0, Math.Min(1, languageOverride.Value));
                persistUiSettings = false;
            }
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1150, 698);
            MinimumSize = new Size(1166, 737);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            Text = "Space Rangers HD Save Editor";
            Font = regularFont;
            Icon = EditorAssets.AppIcon();
            DoubleBuffered = true;
            EditorFormFactory.LanguageIndex = appSettings.LanguageIndex;
            BuildUi();
            BuildLoadingOverlay();
            if (appSettings.LanguageIndex == 1) ApplyMainEnglishLanguage();
            RestoreFilterSettings();
            ResetView();
            Shown += OnShown;
        }

        internal void OpenAtStartup(string path)
        {
            OpenAtStartup(path, false);
        }

        internal void OpenAtStartup(string path, bool suppressPrompts)
        {
            startupPath = path;
            suppressLoadPrompts = suppressPrompts;
        }

        private void OnShown(object sender, EventArgs e)
        {
            // Let Windows paint the shell before starting any disk/catalog work.
            BeginInvoke((MethodInvoker)delegate
            {
                if (!string.IsNullOrEmpty(startupPath)) OpenSave(startupPath);
                else OpenDefaultFolder();
            });
        }

        private void BuildLoadingOverlay()
        {
            loadingOverlay = new Panel();
            loadingOverlay.Name = "loadingOverlay";
            loadingOverlay.Dock = DockStyle.Fill;
            loadingOverlay.BackColor = SystemColors.Control;
            loadingOverlay.Visible = false;

            loadingCard = new Panel();
            loadingCard.Name = "loadingCard";
            loadingCard.Size = new Size(420, 116);
            loadingCard.BackColor = SystemColors.Window;
            loadingCard.BorderStyle = BorderStyle.FixedSingle;

            Label title = new Label();
            title.AutoSize = false;
            title.Text = appSettings.LanguageIndex == 1 ? "Loading save" : "Загрузка сохранения";
            title.Font = boldFont;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.SetBounds(22, 15, 374, 26);
            loadingCard.Controls.Add(title);

            loadingLabel = new Label();
            loadingLabel.AutoSize = false;
            loadingLabel.TextAlign = ContentAlignment.MiddleLeft;
            loadingLabel.SetBounds(22, 42, 374, 24);
            loadingCard.Controls.Add(loadingLabel);

            loadingProgress = new ProgressBar();
            loadingProgress.Style = ProgressBarStyle.Marquee;
            loadingProgress.MarqueeAnimationSpeed = 24;
            loadingProgress.SetBounds(22, 75, 374, 18);
            loadingCard.Controls.Add(loadingProgress);

            loadingOverlay.Controls.Add(loadingCard);
            Controls.Add(loadingOverlay);
            Resize += delegate { PositionLoadingCard(); };
            PositionLoadingCard();
        }

        private void PositionLoadingCard()
        {
            if (loadingOverlay == null || loadingCard == null) return;
            loadingCard.Left = Math.Max(8, (loadingOverlay.ClientSize.Width - loadingCard.Width) / 2);
            loadingCard.Top = Math.Max(8, (loadingOverlay.ClientSize.Height - loadingCard.Height) / 2);
        }

        private void ShowLoading(string text)
        {
            loadingLabel.Text = text;
            loadingOverlay.Visible = true;
            loadingOverlay.BringToFront();
            PositionLoadingCard();
            UseWaitCursor = true;
        }

        private void HideLoading()
        {
            if (loadingOverlay != null) loadingOverlay.Visible = false;
            UseWaitCursor = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Release the current save model and close its modeless system map.
            SaveUiSettings();
            ResetView();
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Form mapToDispose = systemMapForm;
                systemMapForm = null;
                systemMapHits = null;
                if (mapToDispose != null && !mapToDispose.IsDisposed)
                {
                    mapToDispose.Close();
                    mapToDispose.Dispose();
                }
                if (statusToolTip != null) statusToolTip.Dispose();
            }
            base.Dispose(disposing);
            if (disposing)
            {
                if (regularFont != null) regularFont.Dispose();
                if (boldFont != null) boldFont.Dispose();
                if (compactValueFont != null) compactValueFont.Dispose();
                if (tinyValueFont != null) tinyValueFont.Dispose();
            }
        }

        private void BuildUi()
        {
            Panel top = new Panel();
            top.Location = new Point(0, 0);
            top.Size = new Size(1150, 34);
            top.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            Controls.Add(top);

            banner = new FileBanner();
            banner.Location = new Point(8, 5);
            banner.Size = new Size(666, 26);
            banner.Font = regularFont;
            top.Controls.Add(banner);

            Button refresh = ButtonAt(top, "Обновить", "arrow_refresh", 680, 4, RefreshClicked);
            Button open = ButtonAt(top, "Открыть", "folder", 797, 4, OpenClicked);
            saveButton = ButtonAt(top, "Сохранить", "disk", 914, 4, SaveClicked);
            Button settings = ButtonAt(top, "Настройки", "gear_in", 1031, 4, SettingsClicked);
            requiresSave.Add(saveButton);

            Panel main = new Panel();
            main.Location = new Point(0, 34);
            main.Size = new Size(1150, 664);
            main.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(main);

            TabControl tabs = new ExactTabControl();
            mainTabs = tabs;
            tabs.Location = new Point(8, 5);
            tabs.Size = new Size(1134, 654);
            tabs.Font = regularFont;
            tabs.Padding = new Point(10, 4);
            main.Controls.Add(tabs);

            string[] names = { "Файл", " Галактика ", " Склады ", " Зонды ", " Скрипты ", " События ", " Сообщения ", " Моды ", " Поиск ", "Лог" };
            TabPage[] pages = new TabPage[names.Length];
            for (int index = 0; index < names.Length; index++)
            {
                pages[index] = new TabPage(names[index]);
                pages[index].BackColor = SystemColors.Control;
                pages[index].UseVisualStyleBackColor = true;
                tabs.TabPages.Add(pages[index]);
            }
            BuildFilePage(pages[0]);
            BuildGalaxyPage(pages[1]);
            galaxyPage = pages[1];
            itemList = BuildListPage(pages[2], "Предметы на складах");
            itemList.SelectionMode = SelectionMode.MultiExtended;
            itemList.DoubleClick += EditSelectedStorageItem;
            ContextMenuStrip storageItemMenu = new ContextMenuStrip();
            storageItemMenu.Items.Add("Редактировать", null, EditSelectedStorageItem);
            storageItemMenu.Items.Add("Удалить", null, DeleteSelectedStorageItems);
            itemList.ContextMenuStrip = storageItemMenu;
            satelliteList = BuildListPage(pages[3], "Зонды на планетах");
            satelliteList.SelectionMode = SelectionMode.MultiExtended;
            satelliteList.DoubleClick += EditSelectedSatellite;
            satelliteList.MouseDown += delegate(object sender, MouseEventArgs args)
            {
                if (args.Button == MouseButtons.Right)
                    EditorFormFactory.ApplyContextPopupSelection(satelliteList, args.Location);
            };
            ContextMenuStrip satelliteMenu = new ContextMenuStrip();
            satelliteMenu.Items.Add("Редактировать", null, EditSelectedSatellite);
            satelliteMenu.Items.Add("Удалить", null, DeleteSelectedSatellites);
            satelliteList.ContextMenuStrip = satelliteMenu;
            BuildScriptsPage(pages[4]);
            galaxyEventList = BuildListPage(pages[5], "Галактические события");
            galaxyEventList.SelectionMode = SelectionMode.MultiExtended;
            galaxyEventList.DoubleClick += ViewSelectedGalaxyEvent;
            ContextMenuStrip eventMenu = new ContextMenuStrip();
            eventMenu.Items.Add("Просмотреть", null, ViewSelectedGalaxyEvent);
            eventMenu.Items.Add("Удалить", null, DeleteSelectedGalaxyEvents);
            galaxyEventList.ContextMenuStrip = eventMenu;
            BuildMessagesPage(pages[6]);
            BuildModsPage(pages[7]);
            BuildSearchPage(pages[8]);
            BuildLogPage(pages[9]);
            refresh.Select();
        }

        private void ApplyMainEnglishLanguage()
        {
            Text = "Space Rangers HD Save Editor";
            Dictionary<string, string> captions = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Обновить", "Refresh" }, { "Открыть", "Open" }, { "Сохранить", "Save" },
                { "Настройки", "Settings" }, { "Файл", "File" }, { " Галактика ", " Galaxy " },
                { " Склады ", " Storage " }, { " Зонды ", " Probes " },
                { " Скрипты ", " Scripts " }, { " События ", " Events " },
                { " Сообщения ", " Messages " }, { " Моды ", " Mods " },
                { " Поиск ", " Search " }, { "Лог", "Log" },
                { "Превью", "Preview" }, { "Информация", "Information" },
                { "Скриншот", "Screenshot" }, { "Карта", "Map" }, { "Статус", "Status" },
                { "Статистика", "Statistics" }, { "Боссы", "Bosses" }, { "Моды", "Mods" },
                { "Название:", "Name:" }, { "Текущий ход:", "Current turn:" },
                { "Дата:", "Date:" }, { "Имя игрока:", "Player name:" },
                { "Раса игрока:", "Player race:" }, { "Деньги игрока:", "Credits:" },
                { "Сложность:", "Difficulty:" }, { "Железная воля:", "Iron will:" },
                { "Тонкие настройки:", "Custom rules:" },
                { "Планетарные бои:", "Planetary battles:" },
                { "Загрузок:", "Loads:" }, { "Сохранений:", "Saves:" },
                { "Звезд:", "Stars:" }, { "Планет:", "Planets:" },
                { "Кораблей:", "Ships:" }, { "Станций:", "Stations:" },
                { "Предметов:", "Items:" }, { "Рейнджеров:", "Rangers:" },
                { "Блазер:", "Blazer:" }, { "Келлер:", "Keller:" },
                { "Террон:", "Terron:" }, { "Модов:", "Mods:" },
                { "Мод.инфо:", "Mod info:" }, { "Мод.оружия:", "Custom weapons:" },
                { "Карта галактики", "Galaxy map" }, { "Редактирование", "Editing" },
                { "Просмотр", "View" }, { "Галактика", "Galaxy" }, { "Игрок", "Player" },
                { "Список модов", "Mod list" }, { "Достижения", "Achievements" },
                { "Сектора", "Sectors" }, { "Системы", "Systems" }, { "Объекты", "Objects" },
                { "Фильтр объектов", "Object filter" }, { "Корабли", "Ships" },
                { "Карта системы", "System map" }, { "Поиск", "Search" },
                { "Параметры", "Parameters" }, { "Найти", "Find" }, { "Фильтрация", "Filtering" },
                { "Сохранить лог", "Save log" }, { "Поиск по логу:", "Search log:" },
                { "Назад", "Previous" }, { "Далее", "Next" }
            };
            ApplyCaptionDictionary(this, captions);
        }

        private static void ApplyCaptionDictionary(Control root, Dictionary<string, string> captions)
        {
            string translated;
            if (captions.TryGetValue(root.Text ?? string.Empty, out translated)) root.Text = translated;
            foreach (Control child in root.Controls) ApplyCaptionDictionary(child, captions);
        }

        private Button ButtonAt(Control parent, string text, string imageName, int x, int y, EventHandler click)
        {
            Button button = new Button();
            button.Location = new Point(x, y);
            button.Size = new Size(111, 27);
            button.Text = text;
            button.Font = regularFont;
            button.UseVisualStyleBackColor = true;
            button.Image = EditorAssets.Image(imageName);
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(13, 0, 2, 0);
            button.Click += click;
            parent.Controls.Add(button);
            return button;
        }

        private void BuildFilePage(TabPage page)
        {
            GroupBox preview = Group(page, "Превью", 8, 5, 270, 148);
            ValuePair(preview, "save_name", "Название:", 23, 109);
            ValuePair(preview, "turn", "Текущий ход:", 42, 109);
            ValuePair(preview, "date", "Дата:", 61, 109);
            ValuePair(preview, "player", "Имя игрока:", 80, 109);
            ValuePair(preview, "race", "Раса игрока:", 99, 109);
            ValuePair(preview, "money", "Деньги игрока:", 118, 109);

            GroupBox info = Group(page, "Информация", 288, 5, 270, 148);
            ValuePair(info, "difficulty", "Сложность:", 23, 127);
            ValuePair(info, "iron", "Железная воля:", 42, 127);
            ValuePair(info, "custom", "Тонкие настройки:", 61, 127);
            ValuePair(info, "battles", "Планетарные бои:", 80, 127);
            ValuePair(info, "loads", "Загрузок:", 99, 127);
            ValuePair(info, "saves", "Сохранений:", 118, 127);

            GroupBox screenshot = Group(page, "Скриншот", 568, 5, 181, 148);
            previewImage = ImageAt(screenshot, 12, 18, 159, 121);
            GroupBox map = Group(page, "Карта", 761, 5, 181, 148);
            mapImage = ImageAt(map, 12, 18, 159, 121);

            GroupBox status = Group(page, "Статус", 8, 153, 270, 144);
            statusGame = StatusAt(status, 20);
            statusCrc = StatusAt(status, 39);
            statusRead = StatusAt(status, 58);
            statusItems = StatusAt(status, 77);
            statusLegal = StatusAt(status, 96);
            statusDump = StatusAt(status, 115);

            GroupBox stats = Group(page, "Статистика", 8, 298, 270, 142);
            ValuePair(stats, "stars", "Звезд:", 21, 109);
            ValuePair(stats, "planets", "Планет:", 40, 109);
            ValuePair(stats, "ships", "Кораблей:", 59, 109);
            ValuePair(stats, "stations", "Станций:", 78, 109);
            ValuePair(stats, "items", "Предметов:", 97, 109);
            ValuePair(stats, "rangers", "Рейнджеров:", 116, 109);

            GroupBox bosses = Group(page, "Боссы", 8, 440, 270, 87);
            ValuePair(bosses, "blazer", "Блазер:", 21, 109);
            ValuePair(bosses, "keller", "Келлер:", 40, 109);
            ValuePair(bosses, "terron", "Террон:", 59, 109);

            GroupBox mods = Group(page, "Моды", 8, 527, 270, 88);
            ValuePair(mods, "mods", "Модов:", 23, 109);
            ValuePair(mods, "mod_info", "Мод.инфо:", 42, 109);
            ValuePair(mods, "mod_weapons", "Мод.оружия:", 61, 109);

            GroupBox galaxy = Group(page, "Карта галактики", 288, 153, 654, 462);
            galaxyMapImage = new InteractivePictureBox();
            galaxyMapImage.Location = new Point(15, 20);
            galaxyMapImage.Size = new Size(628, 430);
            galaxy.Controls.Add(galaxyMapImage);
            galaxyMapImage.SizeMode = PictureBoxSizeMode.StretchImage;
            galaxyMapImage.MouseMove += GalaxyMapMouseMove;
            galaxyMapImage.MouseDown += GalaxyMapMouseDown;
            galaxyMapImage.MouseUp += GalaxyMapMouseUp;
            galaxyMapImage.MouseWheel += GalaxyMapMouseWheel;
            galaxyMapImage.MouseLeave += GalaxyMapMouseLeave;
            statusToolTip.SetToolTip(galaxyMapImage, appSettings.LanguageIndex == 1
                ? "LMB — open system; drag — pan; wheel — zoom; RMB — reset view"
                : "ЛКМ — открыть систему; перетаскивание — перемещение; " +
                    "колесо — масштаб; ПКМ — сброс вида");

            GroupBox edit = Group(page, "Редактирование", 952, 5, 163, 88);
            SmallButton(edit, "Галактика", "world", 10, 17, GalaxyClicked, null);
            SmallButton(edit, "Игрок", "controller", 10, 50, PlayerClicked, "TSHIPFORM");
            GroupBox view = Group(page, "Просмотр", 952, 94, 163, 88);
            SmallButton(view, "Список модов", "cross_reference", 10, 17, ModsListClicked, "TMODSLISTFORM");
            SmallButton(view, "Достижения", "cup_bronze", 10, 50, EditorFormClicked, "TACHIEVEMENTSFORM");
        }

        private void BuildGalaxyPage(TabPage page)
        {
            GroupBox sectors = Group(page, "Сектора", 8, 3, 249, 226);
            constellationList = ListAt(sectors, 12, 20, 221, 192);
            constellationList.FormattingEnabled = true;
            constellationList.Format += ConstellationListFormat;
            constellationList.SelectedIndexChanged += ConstellationSelectionChanged;
            constellationList.DoubleClick += ToggleSelectedConstellation;
            ContextMenuStrip constellationMenu = new ContextMenuStrip();
            constellationMenu.Items.Add("Изменить видимость", null, ToggleSelectedConstellation);
            constellationList.ContextMenuStrip = constellationMenu;
            GroupBox systems = Group(page, "Системы", 8, 230, 249, 325);
            starList = ListAt(systems, 12, 20, 221, 291);
            starList.Sorted = true;
            starList.FormattingEnabled = true;
            starList.Format += StarListFormat;
            starList.SelectedIndexChanged += StarSelectionChanged;
            starList.DoubleClick += EditSelectedStar;
            ContextMenuStrip starMenu = new ContextMenuStrip();
            starMenu.Items.Add("Редактировать", null, EditSelectedStar);
            starList.ContextMenuStrip = starMenu;
            GroupBox objects = Group(page, "Объекты", 268, 108, 849, 505);
            galaxyObjectList = ListAt(objects, 12, 20, 821, 471);
            galaxyObjectList.FormattingEnabled = true;
            galaxyObjectList.Format += GalaxyObjectFormat;
            galaxyObjectList.SelectionMode = SelectionMode.MultiExtended;
            galaxyObjectList.SelectedIndexChanged += GalaxyObjectSelectionChanged;
            galaxyObjectList.DoubleClick += EditSelectedGalaxyObject;
            ContextMenuStrip objectMenu = new ContextMenuStrip();
            objectMenu.Items.Add("Редактировать", null, EditSelectedGalaxyObject);
            objectMenu.Items.Add("Удалить", null, DeleteSelectedGalaxyObjects);
            galaxyObjectList.ContextMenuStrip = objectMenu;

            GroupBox filter = Group(page, "Фильтр объектов", 268, 3, 849, 104);
            GroupBox objectFilters = Group(filter, "", 14, 16, 343, 77);
            galaxyObjectMaster = MasterFilterAt(objectFilters, "Объекты", GalaxyObjectMasterChanged);
            GalaxyFilterAt(objectFilters, "planets", "Планеты", 9, 18, true);
            GalaxyFilterAt(objectFilters, "stations", "Станции", 9, 35, true);
            GalaxyFilterAt(objectFilters, "equipment", "Оборудование", 9, 52, true);
            GalaxyFilterAt(objectFilters, "goods", "Товары", 129, 18, true);
            GalaxyFilterAt(objectFilters, "useless", "Ошметки", 129, 35, true);
            GalaxyFilterAt(objectFilters, "nods", "Ноды", 129, 52, true);
            GalaxyFilterAt(objectFilters, "missiles", "Ракеты", 229, 18, true);
            GalaxyFilterAt(objectFilters, "asteroids", "Астероиды", 229, 35, true);
            GalaxyFilterAt(objectFilters, "holes", "Черные дыры", 229, 52, true);

            GroupBox shipFilters = Group(filter, "", 367, 16, 468, 77);
            galaxyShipMaster = MasterFilterAt(shipFilters, "Корабли", GalaxyShipMasterChanged);
            GalaxyFilterAt(shipFilters, "rangers", "Рейнджеры", 9, 18, true);
            GalaxyFilterAt(shipFilters, "warriors", "Военные", 9, 35, true);
            GalaxyFilterAt(shipFilters, "flagships", "Флагманы", 9, 52, true);
            GalaxyFilterAt(shipFilters, "transports", "Транспорты", 110, 18, true);
            GalaxyFilterAt(shipFilters, "liners", "Лайнеры", 110, 35, true);
            GalaxyFilterAt(shipFilters, "diplomats", "Дипломаты", 110, 52, true);
            GalaxyFilterAt(shipFilters, "pirates", "Пираты", 226, 18, true);
            GalaxyFilterAt(shipFilters, "clanpirates", "Клановые пираты", 226, 35, true);
            GalaxyFilterAt(shipFilters, "tranclucators", "Транклюкаторы", 226, 52, true);
            GalaxyFilterAt(shipFilters, "dominators", "Доминаторы", 362, 18, true);
            GalaxyFilterAt(shipFilters, "bertors", "Берторы", 362, 35, true);
            GalaxyFilterAt(shipFilters, "bosses", "Боссы", 362, 52, true);

            GroupBox view = Group(page, "Просмотр", 8, 556, 249, 57);
            Button map = new Button(); map.Text = "Карта системы"; map.Location = new Point(14, 20);
            map.Size = new Size(221, 27); map.UseVisualStyleBackColor = true; map.Click += ShowSelectedStarMap;
            view.Controls.Add(map);
        }

        private CheckBox MasterFilterAt(Control parent, string text, EventHandler changed)
        {
            CheckBox box = new CheckBox(); box.Text = text; box.AutoSize = true; box.Checked = true;
            box.Location = new Point(8, 0); box.Font = regularFont; box.BackColor = SystemColors.Control;
            box.CheckedChanged += changed; parent.Controls.Add(box); box.BringToFront(); return box;
        }

        private void GalaxyFilterAt(Control parent, string key, string text, int x, int y, bool supported)
        {
            CheckBox box = new CheckBox(); box.Text = text; box.AutoSize = true; box.Checked = true;
            box.Location = new Point(x, y); box.Font = regularFont; box.Enabled = supported;
            box.CheckedChanged += GalaxyFilterChanged; parent.Controls.Add(box); galaxyFilters[key] = box;
            if (!supported)
                statusToolTip.SetToolTip(box, "Этот производный класс ещё не размечен в SAV; фильтр станет активным вместе с его сериализатором.");
        }

        private ListBox BuildListPage(TabPage page, string caption)
        {
            GroupBox box = Group(page, caption, 8, 3, 1107, 610);
            return ListAt(box, 10, 20, 1087, 580);
        }

        private void BuildModsPage(TabPage page)
        {
            ExactTabControl sections = new ExactTabControl();
            sections.Location = new Point(12, 5);
            sections.Size = new Size(1102, 602);
            sections.Alignment = TabAlignment.Left;
            sections.Multiline = true;
            sections.SizeMode = TabSizeMode.Fixed;
            // Win32 swaps ItemSize axes for left-aligned tabs. Keep a wide strip
            // with compact horizontal captions on every supported Windows theme.
            sections.ItemSize = new Size(25, 110);
            sections.ExactItemWidth = 25;
            sections.ExactItemHeight = 110;
            sections.Font = regularFont;
            page.Controls.Add(sections);
            VerticalTabStrip sectionStrip =
                new VerticalTabStrip(sections, 110, 25);
            sectionStrip.Font = sections.Font;
            page.Controls.Add(sectionStrip);
            sectionStrip.BringToFront();

            TabPage storage = new TabPage("Хранилище");
            TabPage weapons = new TabPage("Оружие");
            TabPage interfacePage = new TabPage("Интерфейс");
            TabPage modInfo = new TabPage("Инфо");
            storage.UseVisualStyleBackColor = weapons.UseVisualStyleBackColor = interfacePage.UseVisualStyleBackColor =
                modInfo.UseVisualStyleBackColor = true;
            sections.TabPages.Add(storage);
            sections.TabPages.Add(weapons);
            sections.TabPages.Add(interfacePage);
            sections.TabPages.Add(modInfo);
            sections.SelectedTab = storage;

            GroupBox storageGroup = Group(storage, "Модовое хранилище", 12, 0, 950, 586);
            storedItemList = ListAt(storageGroup, 14, 20, 922, 552);
            storedItemList.SelectionMode = SelectionMode.MultiExtended;
            storedItemList.DoubleClick += EditSelectedStoredItem;
            ContextMenuStrip storedMenu = new ContextMenuStrip();
            storedMenu.Items.Add("Редактировать", null, EditSelectedStoredItem);
            storedMenu.Items.Add("Удалить", null, DeleteSelectedStoredItems);
            storedItemList.ContextMenuStrip = storedMenu;

            GroupBox weaponsGroup = Group(weapons, "Модифицированное оружие", 12, 0, 950, 586);
            customWeaponList = ListAt(weaponsGroup, 14, 20, 922, 552);
            customWeaponList.DoubleClick += EditSelectedCustomWeapon;
            ContextMenuStrip weaponMenu = new ContextMenuStrip();
            weaponMenu.Items.Add("Редактировать", null, EditSelectedCustomWeapon);
            weaponMenu.Items.Add("Удалить", null, DeleteSelectedCustomWeapon);
            customWeaponList.ContextMenuStrip = weaponMenu;

            interfaceOverrideLists = new ListBox[5];
            GroupBox stateGroup = Group(interfacePage, "Состояния", 12, 0, 299, 310);
            interfaceOverrideLists[0] = ListAt(stateGroup, 14, 20, 271, 276);
            GroupBox textGroup = Group(interfacePage, "Тексты", 323, 0, 326, 310);
            interfaceOverrideLists[1] = ListAt(textGroup, 14, 20, 298, 276);
            GroupBox imageGroup = Group(interfacePage, "Изображения", 660, 0, 305, 310);
            interfaceOverrideLists[2] = ListAt(imageGroup, 14, 20, 277, 276);
            GroupBox positionGroup = Group(interfacePage, "Позиции", 12, 311, 453, 278);
            interfaceOverrideLists[3] = ListAt(positionGroup, 14, 20, 425, 244);
            GroupBox sizeGroup = Group(interfacePage, "Размеры", 477, 311, 488, 278);
            interfaceOverrideLists[4] = ListAt(sizeGroup, 14, 20, 460, 244);
            foreach (ListBox list in interfaceOverrideLists)
            {
                list.SelectionMode = SelectionMode.MultiExtended;
                statusToolTip.SetToolTip(list, "Двойной щелчок открывает исходную форму переопределения интерфейса.");
                list.DoubleClick += EditSelectedInterfaceOverride;
                ContextMenuStrip interfaceMenu = new ContextMenuStrip();
                interfaceMenu.Items.Add("Редактировать", null, EditSelectedInterfaceOverride);
                interfaceMenu.Items.Add("Удалить", null, DeleteSelectedInterfaceOverrides);
                list.ContextMenuStrip = interfaceMenu;
            }

            GroupBox modInfoShips = Group(modInfo, "[Сортировка] Инфо на кораблях", 12, 0, 700, 587);
            modInfoShipList = ListAt(modInfoShips, 14, 20, 672, 553);
            modInfoShipList.SelectionMode = SelectionMode.MultiExtended;
            modInfoShipList.Sorted = true;
            modInfoShipList.DoubleClick += EditSelectedModInfoShip;
            ContextMenuStrip shipInfoMenu = new ContextMenuStrip();
            shipInfoMenu.Items.Add("Редактировать", null, EditSelectedModInfoShip);
            shipInfoMenu.Items.Add("Удалить", null, DeleteSelectedModInfoShips);
            modInfoShipList.ContextMenuStrip = shipInfoMenu;
            modInfoShipsEnabled = new CheckBox();
            modInfoShipsEnabled.Text = "Показывать"; modInfoShipsEnabled.AutoSize = true;
            modInfoShipsEnabled.Checked = true; modInfoShipsEnabled.Location = new Point(594, 0);
            modInfoShipsEnabled.BackColor = SystemColors.Control;
            modInfoShipsEnabled.CheckedChanged += ModInfoShipsEnabledChanged;
            modInfoShips.Controls.Add(modInfoShipsEnabled); modInfoShipsEnabled.BringToFront();

            GroupBox modInfoStars = Group(modInfo, "Инфо на звездах", 722, 0, 241, 587);
            modInfoStarList = ListAt(modInfoStars, 14, 20, 213, 553);
            modInfoStarList.SelectionMode = SelectionMode.MultiExtended;
            modInfoStarList.Sorted = true;
            modInfoStarList.DoubleClick += EditSelectedModInfoStar;
            ContextMenuStrip starInfoMenu = new ContextMenuStrip();
            starInfoMenu.Items.Add("Редактировать", null, EditSelectedModInfoStar);
            starInfoMenu.Items.Add("Удалить", null, DeleteSelectedModInfoStars);
            modInfoStarList.ContextMenuStrip = starInfoMenu;

        }

        private void BuildScriptsPage(TabPage page)
        {
            GroupBox cache = Group(page, "Кэш скриптов", 8, 3, 353, 610);
            scriptCacheList = ListAt(cache, 10, 20, 333, 580);
            scriptCacheList.Sorted = true;
            scriptCacheList.FormattingEnabled = true;
            scriptCacheList.Format += ScriptCacheListFormat;
            scriptCacheList.DoubleClick += EditSelectedScriptCache;
            ContextMenuStrip cacheMenu = new ContextMenuStrip();
            cacheMenu.Items.Add("Редактировать", null, EditSelectedScriptCache);
            cacheMenu.Items.Add("Удалить", null, DeleteSelectedScriptCache);
            scriptCacheList.ContextMenuStrip = cacheMenu;
            GroupBox active = Group(page, "Активные скрипты", 371, 3, 353, 610);
            scriptList = ListAt(active, 10, 20, 333, 580);
            scriptList.Sorted = true;
            scriptList.FormattingEnabled = true;
            scriptList.Format += ActiveScriptListFormat;
            scriptList.DoubleClick += ViewSelectedScript;
            ContextMenuStrip scriptsMenu = new ContextMenuStrip();
            scriptsMenu.Items.Add("Редактировать", null, ViewSelectedScript);
            scriptsMenu.Items.Add("Удалить", null, DeleteSelectedScript);
            scriptList.ContextMenuStrip = scriptsMenu;
            GroupBox globals = Group(page, "Глобальные переменные", 734, 3, 381, 610);
            globalVariableList = ListAt(globals, 10, 20, 361, 580);
            globalVariableList.Sorted = true;
            globalVariableList.FormattingEnabled = true;
            globalVariableList.Format += GlobalVariableListFormat;
            globalVariableList.DoubleClick += EditSelectedGlobalVariable;
            ContextMenuStrip globalsMenu = new ContextMenuStrip();
            globalsMenu.Items.Add("Редактировать", null, EditSelectedGlobalVariable);
            globalsMenu.Items.Add("Удалить", null, DeleteSelectedGlobalVariable);
            globalVariableList.ContextMenuStrip = globalsMenu;
        }

        private void BuildMessagesPage(TabPage page)
        {
            GroupBox list = Group(page, appSettings.LanguageIndex == 1 ?
                "Player messages: 0" : "Сообщения игрока: 0", 8, 3, 420, 608);
            list.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            messageList = ListAt(list, 14, 20, 392, 574);
            messageList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageList.IntegralHeight = false;
            messageList.SelectionMode = SelectionMode.MultiExtended;
            messageList.SelectedIndexChanged += MessageSelectionChanged;
            messageList.DoubleClick += EditSelectedMessage;
            ContextMenuStrip messageMenu = new ContextMenuStrip();
            messageMenu.Items.Add("Редактировать", null, EditSelectedMessage);
            messageMenu.Items.Add("Удалить", null, DeleteSelectedMessages);
            messageList.ContextMenuStrip = messageMenu;

            GroupBox text = Group(page, appSettings.LanguageIndex == 1 ?
                "Message text" : "Текст сообщения", 438, 3, 680, 608);
            text.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageText = new RichTextBox();
            messageText.ReadOnly = true;
            messageText.BorderStyle = BorderStyle.FixedSingle;
            messageText.BackColor = SystemColors.Window;
            messageText.DetectUrls = false;
            messageText.Location = new Point(14, 20);
            messageText.Size = new Size(652, 574);
            messageText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageText.Font = regularFont;
            text.Controls.Add(messageText);
        }

        private void BuildSearchPage(TabPage page)
        {
            GroupBox search = Group(page, "Поиск", 8, 3, 1110, 608);
            GroupBox parameters = Group(search, "Параметры", 12, 12, 1082, 106);
            PlainLabel(parameters, "Имя:", 16, 25, false);
            searchQuery = new TextBox(); searchQuery.Location = new Point(52, 22); searchQuery.Size = new Size(191, 21); parameters.Controls.Add(searchQuery);
            searchQuery.KeyDown += delegate(object sender, KeyEventArgs args) { if (args.KeyCode == Keys.Enter) { RunSearch(sender, EventArgs.Empty); args.SuppressKeyPress = true; } };
            PlainLabel(parameters, "ID:", 261, 25, false);
            searchId = new TextBox(); searchId.Location = new Point(281, 22); searchId.Size = new Size(112, 21); parameters.Controls.Add(searchId);
            searchId.KeyPress += delegate(object sender, KeyPressEventArgs args) { if (!char.IsControl(args.KeyChar) && !char.IsDigit(args.KeyChar)) args.Handled = true; };
            searchId.KeyDown += delegate(object sender, KeyEventArgs args) { if (args.KeyCode == Keys.Enter) { RunSearch(sender, EventArgs.Empty); args.SuppressKeyPress = true; } };
            PlainLabel(parameters, "Тип предмета:", 16, 52, false);
            searchItemType = new ComboBox(); searchItemType.DropDownStyle = ComboBoxStyle.DropDownList;
            searchItemType.Location = new Point(104, 49); searchItemType.Size = new Size(289, 21);
            RefreshSearchItemTypes();
            searchItemType.SelectedIndex = 0; parameters.Controls.Add(searchItemType);
            Button run = new Button(); run.Text = "Найти"; run.Location = new Point(16, 76); run.Size = new Size(377, 21);
            run.UseVisualStyleBackColor = true; run.Click += RunSearch; parameters.Controls.Add(run);

            GroupBox filters = Group(parameters, "", 408, 16, 661, 77);
            searchFilterMaster = MasterFilterAt(filters, "Фильтрация", SearchFilterMasterChanged);
            SearchFilterAt(filters, "stars", "Звезды", 9, 18, true);
            SearchFilterAt(filters, "planets", "Планеты", 9, 35, true);
            SearchFilterAt(filters, "ships", "Корабли", 9, 52, true);
            SearchFilterAt(filters, "stations", "Станции", 120, 18, true);
            SearchFilterAt(filters, "asteroids", "Астероиды", 120, 35, true);
            SearchFilterAt(filters, "missiles", "Ракеты", 120, 52, true);
            SearchFilterAt(filters, "spaceitems", "Предметы в космосе", 238, 18, true);
            SearchFilterAt(filters, "planetitems", "Предметы на планетах", 238, 35, true);
            SearchFilterAt(filters, "shopitems", "Предметы в магазинах", 238, 52, true);
            SearchFilterAt(filters, "holds", "Трюмы", 412, 18, true);
            SearchFilterAt(filters, "storage", "Склады", 412, 35, true);
            SearchFilterAt(filters, "satellites", "Зонды", 412, 52, true);
            SearchFilterAt(filters, "drops", "Трофеи", 516, 18, true);
            SearchFilterAt(filters, "modstorage", "Мод.хранилище", 516, 35, true);
            SearchFilterAt(filters, "tranclucators", "Транклюкаторы", 516, 52, true);

            searchResults = ListAt(search, 12, 127, 1082, 464);
            searchResults.DoubleClick += EditSelectedSearchResult;
            ContextMenuStrip searchMenu = new ContextMenuStrip();
            searchMenu.Items.Add("Редактировать", null, EditSelectedSearchResult);
            searchResults.ContextMenuStrip = searchMenu;
        }

        private void AddSearchItemType(string caption, int firstType, int lastType)
        {
            searchItemType.Items.Add(new ItemTypeSearchChoice(caption, firstType, lastType));
        }

        private void RefreshSearchItemTypes()
        {
            if (searchItemType == null) return;
            int selected = Math.Max(0, searchItemType.SelectedIndex);
            searchItemType.BeginUpdate();
            searchItemType.Items.Clear();
            AddSearchItemType(appSettings.LanguageIndex == 1 ? "All" : "Любой тип предмета", -1, -1);
            for (int type = 0; type < searchItemTypeKeys.Length; type++)
            {
                string[] captions = appSettings.LanguageIndex == 1 ? searchItemTypeNamesEn : searchItemTypeNamesRu;
                string caption = type < captions.Length ? captions[type] : ItemTypeName((byte)type);
                AddSearchItemType(caption, type, type);
            }
            searchItemType.EndUpdate();
            if (searchItemType.Items.Count != 0)
                searchItemType.SelectedIndex = Math.Min(selected, searchItemType.Items.Count - 1);
        }

        private void SearchFilterAt(Control parent, string key, string text, int x, int y, bool supported)
        {
            CheckBox box = new CheckBox(); box.Text = text; box.AutoSize = true; box.Checked = true;
            box.Location = new Point(x, y); box.Font = regularFont; box.Enabled = supported;
            searchFilters[key] = box; parent.Controls.Add(box);
            if (!supported)
                statusToolTip.SetToolTip(box, "Этот контейнер объектов пока не размечен отдельно от общего TGalaxy.");
        }

        private void SearchFilterMasterChanged(object sender, EventArgs e)
        {
            foreach (CheckBox box in searchFilters.Values)
                if (box.Enabled) box.Checked = searchFilterMaster.Checked;
        }

        private bool SearchFilterEnabled(string key)
        {
            CheckBox box;
            return searchFilters.TryGetValue(key, out box) && box.Checked;
        }

        private void BuildLogPage(TabPage page)
        {
            TabControl logs = new TabControl(); logs.Dock = DockStyle.Fill;
            string[] captions = { "Файл", "Ошибки", "Проблемы CRC", "Коррекция CRC" };
            logViews = new TextBox[captions.Length];
            int index = 0;
            foreach (string caption in captions)
            {
                TabPage tab = new TabPage(caption); tab.UseVisualStyleBackColor = true;
                TextBox text = new TextBox(); text.Dock = DockStyle.Fill; text.Multiline = true; text.ReadOnly = true; text.ScrollBars = ScrollBars.Both;
                if (index == 0)
                {
                    TableLayoutPanel host = new TableLayoutPanel(); host.Dock = DockStyle.Fill;
                    host.ColumnCount = 1; host.RowCount = 2;
                    host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                    host.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
                    FlowLayoutPanel searchPanel = new FlowLayoutPanel(); searchPanel.Dock = DockStyle.Fill;
                    searchPanel.FlowDirection = FlowDirection.RightToLeft; searchPanel.WrapContents = false;
                    searchPanel.Padding = new Padding(4, 4, 4, 0);
                    Label searchCaption = PlainLabel(searchPanel, "Поиск по логу:", 0, 0, false);
                    searchCaption.AutoSize = false; searchCaption.Size = new Size(105, 26);
                    searchCaption.TextAlign = ContentAlignment.MiddleLeft;
                    logSearchFound = PlainLabel(searchPanel, string.Empty, 0, 0, false);
                    logSearchFound.AutoSize = false; logSearchFound.Size = new Size(70, 26);
                    logSearchFound.TextAlign = ContentAlignment.MiddleRight;
                    logSearch = new TextBox(); logSearch.Size = new Size(220, 23);
                    logSearch.TextChanged += LogSearchChanged; searchPanel.Controls.Add(logSearch);
                    Button previous = new Button(); previous.Text = "Назад"; previous.Size = new Size(88, 26);
                    previous.Image = EditorAssets.Image("bullet_arrow_left"); previous.Click += FindPreviousLog;
                    previous.ImageAlign = ContentAlignment.MiddleLeft;
                    previous.TextImageRelation = TextImageRelation.ImageBeforeText;
                    previous.UseVisualStyleBackColor = true; searchPanel.Controls.Add(previous);
                    Button next = new Button(); next.Text = "Далее"; next.Size = new Size(88, 26);
                    next.Image = EditorAssets.Image("bullet_arrow_right"); next.Click += FindNextLog;
                    next.ImageAlign = ContentAlignment.MiddleRight;
                    next.TextImageRelation = TextImageRelation.TextBeforeImage;
                    next.UseVisualStyleBackColor = true; searchPanel.Controls.Add(next);
                    Button save = new Button(); save.Text = "Сохранить лог";
                    save.Size = new Size(150, 26); save.Image = EditorAssets.Image("disk");
                    save.ImageAlign = ContentAlignment.MiddleLeft; save.Click += SaveLogClicked;
                    save.UseVisualStyleBackColor = true; searchPanel.Controls.Add(save);
                    searchPanel.Controls.SetChildIndex(save, 0);
                    searchPanel.Controls.SetChildIndex(next, 1);
                    searchPanel.Controls.SetChildIndex(previous, 2);
                    searchPanel.Controls.SetChildIndex(logSearchFound, 3);
                    searchPanel.Controls.SetChildIndex(logSearch, 4);
                    searchPanel.Controls.SetChildIndex(searchCaption, 5);
                    statusToolTip.SetToolTip(previous, "Найти предыдущее совпадение");
                    statusToolTip.SetToolTip(next, "Найти следующее совпадение");
                    host.Controls.Add(text, 0, 0); host.Controls.Add(searchPanel, 0, 1);
                    tab.Controls.Add(host);
                }
                else tab.Controls.Add(text);
                logs.TabPages.Add(tab); logViews[index++] = text;
            }
            page.Controls.Add(logs);
        }

        private void LogSearchChanged(object sender, EventArgs e)
        {
            if (logViews == null || logViews.Length == 0) return;
            logViews[0].SelectionLength = 0;
            FindLogText(true, true);
        }

        private void FindNextLog(object sender, EventArgs e)
        {
            FindLogText(true, false);
        }

        private void FindPreviousLog(object sender, EventArgs e)
        {
            FindLogText(false, false);
        }

        private void FindLogText(bool forward, bool fromStart)
        {
            if (logViews == null || logViews.Length == 0 || logSearch == null) return;
            TextBox view = logViews[0];
            string query = logSearch.Text ?? string.Empty;
            if (query.Length == 0)
            {
                view.SelectionLength = 0;
                if (logSearchFound != null) logSearchFound.Text = string.Empty;
                return;
            }
            StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            int start = fromStart ? (forward ? 0 : view.TextLength) :
                (forward ? view.SelectionStart + Math.Max(1, view.SelectionLength) : view.SelectionStart - 1);
            int found;
            if (forward)
                found = view.Text.IndexOf(query, Math.Max(0, Math.Min(start, view.TextLength)), comparison);
            else if (view.TextLength == 0)
                found = -1;
            else
                found = view.Text.LastIndexOf(query,
                    Math.Max(0, Math.Min(start, view.TextLength - 1)), comparison);
            if (found < 0 && !fromStart)
                found = forward ? view.Text.IndexOf(query, comparison) : view.Text.LastIndexOf(query, comparison);
            if (found >= 0)
            {
                view.SelectionStart = found; view.SelectionLength = query.Length; view.ScrollToCaret();
                if (logSearchFound != null) logSearchFound.Text = "найдено";
            }
            else if (logSearchFound != null) logSearchFound.Text = "нет";
        }

        private void SaveLogClicked(object sender, EventArgs e)
        {
            if (logViews == null || logViews.Length == 0) return;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Сохранить лог";
                dialog.Filter = "Текстовый файл (*.txt)|*.txt";
                dialog.FileName = current == null ? "SRHDSaveEditor.log.txt" :
                    Path.GetFileNameWithoutExtension(current.SourcePath) + ".SRHDSaveEditor.log.txt";
                dialog.InitialDirectory = current == null ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                    Path.GetDirectoryName(current.SourcePath);
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                File.WriteAllText(dialog.FileName, logViews[0].Text, Encoding.UTF8);
            }
        }

        private GroupBox Group(Control parent, string text, int x, int y, int width, int height)
        {
            GroupBox box = new GroupBox();
            box.Text = text; box.Location = new Point(x, y); box.Size = new Size(width, height); box.Font = boldFont;
            parent.Controls.Add(box); return box;
        }

        private Label PlainLabel(Control parent, string text, int x, int y, bool bold)
        {
            Label label = new Label(); label.Text = text; label.AutoSize = true; label.Location = new Point(x, y);
            label.Font = bold ? boldFont : regularFont; parent.Controls.Add(label); return label;
        }

        private void ValuePair(Control parent, string key, string caption, int y, int valueX)
        {
            PlainLabel(parent, caption, 16, y, false);
            Label value = PlainLabel(parent, "-", valueX, y, true);
            ConfigureFittedValueLabel(value, Math.Max(24, parent.ClientSize.Width - valueX - 8));
            values[key] = value;
        }

        private Label StatusAt(Control parent, int y)
        {
            Label value = PlainLabel(parent, "-", 16, y, true);
            ConfigureFittedValueLabel(value, Math.Max(24, parent.ClientSize.Width - 24));
            return value;
        }

        private void ConfigureFittedValueLabel(Label label, int width)
        {
            label.AutoSize = false;
            label.Size = new Size(width, 18);
            label.AutoEllipsis = false;
            EventHandler fit = delegate
            {
                label.Font = boldFont;
                int measured = TextRenderer.MeasureText(label.Text ?? string.Empty, label.Font,
                    Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                    TextFormatFlags.SingleLine).Width;
                if (measured > label.ClientSize.Width)
                {
                    label.Font = compactValueFont;
                    measured = TextRenderer.MeasureText(label.Text ?? string.Empty, label.Font,
                        Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                        TextFormatFlags.SingleLine).Width;
                    if (measured > label.ClientSize.Width) label.Font = tinyValueFont;
                }
                statusToolTip.SetToolTip(label, label.Text ?? string.Empty);
            };
            label.TextChanged += fit;
            fit(label, EventArgs.Empty);
        }

        private PictureBox ImageAt(Control parent, int x, int y, int width, int height)
        {
            PictureBox image = new PictureBox(); image.Location = new Point(x, y); image.Size = new Size(width, height);
            image.SizeMode = PictureBoxSizeMode.StretchImage; parent.Controls.Add(image); return image;
        }

        private ListBox ListAt(Control parent, int x, int y, int width, int height)
        {
            ListBox list = new AdaptiveOwnerDrawListBox(); list.Location = new Point(x, y); list.Size = new Size(width, height); list.Font = regularFont;
            list.MouseDown += delegate(object sender, MouseEventArgs args)
            {
                if (args.Button == MouseButtons.Right)
                    EditorFormFactory.ApplyContextPopupSelection(list, args.Location);
            };
            parent.Controls.Add(list); return list;
        }

        private void SmallButton(Control parent, string text, string image, int x, int y, EventHandler click, string resource)
        {
            Button button = new Button(); button.Text = text; button.Location = new Point(x, y); button.Size = new Size(143, 27);
            button.Font = regularFont; button.Image = EditorAssets.Image(image); button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText; button.Padding = new Padding(8, 0, 0, 0);
            button.UseVisualStyleBackColor = true; button.Tag = resource; button.Click += click; parent.Controls.Add(button); requiresSave.Add(button);
        }

        private void ResetView()
        {
            if (systemMapForm != null && !systemMapForm.IsDisposed)
                systemMapForm.Close();
            systemMapForm = null;
            ResetGalaxyMapTransform(false);
            current = null; pendingMetadata = null; pendingMessages = null; pendingGalaxy = null;
            pendingGalaxySummary = null; pendingConstellations = null; pendingStars = null;
            pendingPlanets = null; pendingShips = null; pendingItems = null; pendingHoles = null;
            pendingDeletedItemStarts.Clear();
            pendingAsteroids = null; pendingMissiles = null; pendingCustomWeapons = null;
            pendingInterfaceOverrides = null; pendingStoredItems = null; pendingAchievements = null;
            gameCatalog = new GameDataCatalog();
            crcReferenceProblems.Clear(); crcReferenceCorrections.Clear();
            crcReferencesReadAsIs = false;
            banner.FileNameText = ""; banner.VersionText = ""; banner.Invalidate();
            foreach (Label label in values.Values) label.Text = "-";
            Label[] statuses = { statusGame, statusCrc, statusRead, statusItems, statusLegal, statusDump };
            foreach (Label label in statuses) { label.Text = "-"; label.ForeColor = SystemColors.ControlText; }
            ReplaceImage(previewImage, null); ReplaceImage(mapImage, null); ReplaceImage(galaxyMapImage, null);
            if (messageList != null) messageList.Items.Clear();
            if (messageText != null) messageText.Clear();
            if (galaxyEventList != null) galaxyEventList.Items.Clear();
            if (customWeaponList != null) customWeaponList.Items.Clear();
            if (storedItemList != null) storedItemList.Items.Clear();
            if (interfaceOverrideLists != null)
                foreach (ListBox list in interfaceOverrideLists) list.Items.Clear();
            if (modInfoShipList != null) modInfoShipList.Items.Clear();
            if (modInfoStarList != null) modInfoStarList.Items.Clear();
            if (constellationList != null) constellationList.Items.Clear();
            if (starList != null) starList.Items.Clear();
            if (galaxyObjectList != null) galaxyObjectList.Items.Clear();
            if (itemList != null) itemList.Items.Clear();
            if (satelliteList != null) satelliteList.Items.Clear();
            if (scriptList != null) scriptList.Items.Clear();
            if (globalVariableList != null) globalVariableList.Items.Clear();
            if (scriptCacheList != null) scriptCacheList.Items.Clear();
            if (searchResults != null) searchResults.Items.Clear();
            if (logViews != null) foreach (TextBox view in logViews) view.Clear();
            foreach (Control control in requiresSave) control.Enabled = false;
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            if (current != null) OpenSave(current.SourcePath);
            else OpenDefaultFolder();
        }

        private void OpenDefaultFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceRangersHD", "Save");
            if (!Directory.Exists(path)) return;
            string autoSave = Path.Combine(path, "AutoSave.sav");
            if (File.Exists(autoSave))
            {
                OpenSave(autoSave);
                return;
            }
            string selected = null;
            DateTime selectedTime = DateTime.MinValue;
            foreach (string file in Directory.GetFiles(path, "*.sav"))
            {
                DateTime time = File.GetLastWriteTimeUtc(file);
                if (selected == null || time > selectedTime)
                {
                    selected = file;
                    selectedTime = time;
                }
            }
            if (selected != null) OpenSave(selected);
        }

        private void OpenClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Открытие"; dialog.Filter = "Сохранения SRHD (*.sav)|*.sav|Все файлы (*.*)|*.*";
                string initial = !string.IsNullOrWhiteSpace(appSettings.LastDirectory) &&
                    Directory.Exists(appSettings.LastDirectory) ? appSettings.LastDirectory :
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceRangersHD", "Save");
                if (Directory.Exists(initial)) dialog.InitialDirectory = initial;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    appSettings.LastDirectory = Path.GetDirectoryName(dialog.FileName);
                    OpenSave(dialog.FileName);
                }
            }
        }

        private async void OpenSave(string path)
        {
            if (isLoading || string.IsNullOrWhiteSpace(path)) return;
            isLoading = true;
            ShowLoading(appSettings.LanguageIndex == 1 ? "Reading SAV…" : "Чтение SAV…");
            try
            {
                string gamePath = appSettings.GamePath;
                int languageIndex = appSettings.LanguageIndex;
                SaveLoadResult result = await Task.Factory.StartNew(delegate
                {
                    SavContainer save = SavContainer.Load(path);
                    return new SaveLoadResult {
                        Save = save,
                        Catalog = GameDataCatalog.Load(gamePath, save.GalaxyPrefix.UsedMods,
                            languageIndex)
                    };
                });
                if (IsDisposed || Disposing) return;
                loadingLabel.Text = languageIndex == 1
                    ? "Preparing interface…" : "Подготовка интерфейса…";
                SavContainer loaded = result.Save;
                current = loaded; pendingMetadata = loaded.Metadata.Clone();
                pendingMessages = new List<PlayerMessageRecord>();
                foreach (PlayerMessageRecord message in loaded.PlayerMessages)
                    pendingMessages.Add(message.Clone());
                pendingGalaxy = loaded.GalaxyPrefix.Clone();
                gameCatalog = result.Catalog;
                RefreshSearchItemTypes();
                pendingGalaxySummary = loaded.GalaxySummary.Clone();
                pendingConstellations = new List<ConstellationRecord>();
                foreach (ConstellationRecord constellation in loaded.GalaxyConstellations)
                    pendingConstellations.Add(constellation.Clone());
                pendingStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord star in loaded.GalaxyStars)
                    pendingStars.Add(star.Clone());
                pendingPlanets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord planet in loaded.GalaxyPlanets)
                    pendingPlanets.Add(planet.Clone());
                pendingShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in loaded.GalaxyShips)
                    pendingShips.Add(ship.Clone());
                pendingItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in loaded.GalaxyItems)
                    pendingItems.Add(item.Clone());
                pendingDeletedItemStarts.Clear();
                pendingHoles = new List<HoleRecord>();
                foreach (HoleRecord hole in loaded.GalaxyHoles)
                    pendingHoles.Add(hole.Clone());
                pendingAsteroids = new List<AsteroidRecord>();
                foreach (AsteroidRecord asteroid in loaded.GalaxyAsteroids)
                    pendingAsteroids.Add(asteroid.Clone());
                pendingMissiles = new List<MissileRecord>();
                foreach (MissileRecord missile in loaded.GalaxyMissiles)
                    pendingMissiles.Add(missile.Clone());
                pendingCustomWeapons = new List<CustomWeaponInfoRecord>();
                foreach (CustomWeaponInfoRecord weapon in loaded.CustomWeaponInfos)
                    pendingCustomWeapons.Add(weapon.Clone());
                pendingInterfaceOverrides = new List<InterfaceOverrideRecord>();
                foreach (InterfaceOverrideRecord record in loaded.GalaxySummary.InterfaceOverrides)
                    pendingInterfaceOverrides.Add(record.Clone());
                pendingStoredItems = new List<StoredItemRecord>();
                foreach (StoredItemRecord record in loaded.StoredItems)
                    pendingStoredItems.Add(record.Clone());
                pendingAchievements = loaded.AchievementStats.Clone();
                ApplyOriginalCrcReferencePolicy();
                banner.FileNameText = Path.GetFileName(path); banner.VersionText = loaded.Header[1]; banner.Invalidate();
                values["save_name"].Text = loaded.Header[2]; values["turn"].Text = loaded.Header[3]; values["date"].Text = loaded.Header[3];
                values["money"].Text = loaded.Header[4]; values["player"].Text = loaded.Header[5];
                string raceName = loaded.Header[6];
                string localizedRace;
                if (appSettings.LanguageIndex != 1 &&
                    EditorLocalization.TryRussian(raceName, out localizedRace))
                    raceName = localizedRace;
                values["race"].Text = raceName;
                values["mods"].Text = loaded.GalaxyPrefix.UsedModCount.ToString();
                int originalModInfoCount = TotalCustomShipInfoCount(pendingShips);
                if (pendingGalaxySummary.EminentRangerObjectIds != null)
                    originalModInfoCount += Math.Max(0, pendingGalaxySummary.EminentRangerObjectIds.Length - 1);
                values["mod_info"].Text = originalModInfoCount.ToString(CultureInfo.InvariantCulture);
                values["mod_weapons"].Text = loaded.GalaxyPrefix.CustomModWeaponCount.ToString();
                values["stars"].Text = loaded.GalaxyStarCount.ToString();
                values["planets"].Text = TotalPlanets(pendingStars).ToString();
                values["loads"].Text = loaded.GalaxyPrefix.LoadCount.ToString();
                values["saves"].Text = loaded.GalaxyPrefix.SaveCount.ToString();
                values["difficulty"].Text = loaded.GalaxySummary.DifficultyPercent.ToString() + "%";
                bool english = appSettings.LanguageIndex == 1;
                values["iron"].Text = loaded.GalaxySummary.IronWill
                    ? (english ? "Enabled" : "Включена") : (english ? "Disabled" : "Выключена");
                values["custom"].Text = loaded.GalaxySummary.CustomRules
                    ? (english ? "Enabled" : "Включены") : (english ? "Disabled" : "Выключены");
                values["battles"].Text = loaded.GalaxySummary.PlanetBattlesDisabled
                    ? (english ? "Disabled" : "Выключены") : (english ? "Enabled" : "Включены");
                values["ships"].Text = loaded.VisibleShipCount.ToString(CultureInfo.InvariantCulture);
                values["stations"].Text = loaded.StationCount.ToString();
                values["items"].Text = loaded.VisibleItemCount.ToString(CultureInfo.InvariantCulture);
                values["rangers"].Text = loaded.GalaxySummary.RangerCount.ToString();
                values["blazer"].Text = BossStateText(loaded.GalaxySummary.BlazerTurnWin);
                values["keller"].Text = BossStateText(loaded.GalaxySummary.KellerTurnWin);
                values["terron"].Text = BossStateText(loaded.GalaxySummary.TerronTurnWin);
                bool gameFound = !string.IsNullOrEmpty(appSettings.GamePath) && File.Exists(Path.Combine(appSettings.GamePath, "Rangers.exe"));
                statusGame.Text = gameFound
                    ? (english ? "Game found" : "Игра найдена")
                    : (english ? "Game not found" : "Игра не найдена");
                statusGame.ForeColor = gameFound ? Color.Green : Color.Red;
                if (gameFound)
                    statusToolTip.SetToolTip(statusGame, (english ? "Built-in catalog: " : "Встроенный каталог: ") +
                        gameCatalog.MicroModules.Count.ToString(CultureInfo.InvariantCulture) +
                        (english ? " micro-modules; " : " микромодулей; ") +
                        gameCatalog.HullSeries.Count.ToString(CultureInfo.InvariantCulture) +
                        (english ? " hull series; sources: " : " серий корпусов; источников: ") +
                        gameCatalog.SourceCount.ToString(CultureInfo.InvariantCulture));
                statusCrc.Text = english ? "Save CRC - OK" : "CRC сэйв-файла - OK";
                statusCrc.ForeColor = Color.Green;
                statusRead.Text = (english ? "TGalaxy bounds - OK @ 0x" : "Границы TGalaxy - OK @ 0x") +
                    loaded.GalaxyOffset.ToString("X") + " / 0x" +
                    loaded.GalaxySummary.CheatsTestOffset.ToString("X");
                statusRead.ForeColor = Color.Green;
                statusItems.Text = "TStar " + loaded.GalaxyStars.Count + "; TPlanet " + TotalPlanets(pendingStars) +
                    "; TItem " + loaded.ItemCount;
                statusToolTip.SetToolTip(statusItems, "TMissile: " + loaded.GalaxyMissiles.Count + "; TAsteroid: " + loaded.GalaxyAsteroids.Count + "; THole: " + loaded.GalaxyHoles.Count + "; TScript: " + loaded.ActiveScripts.Count +
                    (english ? "; events: " : "; событий: ") + loaded.GalaxySummary.GalaxyEventCount +
                    "; active list @ 0x" + loaded.ActiveScriptListOffset.ToString("X"));
                statusItems.ForeColor = Color.Green;
                UpdateLegalityAndModsStatus(loaded);
                statusDump.Text = loaded.GalaxySummary.PrepareToDump
                    ? (english ? "Dump: " : "Дамп: ") + loaded.GalaxySummary.DumpName
                    : (english ? "No dump" : "Без дампа");
                statusDump.ForeColor = loaded.GalaxySummary.PrepareToDump ? Color.Red : Color.Green;
                ReplaceImage(previewImage, loaded.PreviewImage()); ReplaceImage(mapImage, loaded.MapImage());
                galaxyMapImage.BackColor = Color.Black;
                RefreshMessageList();
                RefreshGalaxyEventList();
                RefreshCustomWeaponList();
                RefreshInterfaceOverrideLists();
                RefreshStoredItemList();
                RefreshModInfoLists();
                RefreshGalaxyView();
                RefreshObjectLists();
                RefreshLog();
                foreach (Control control in requiresSave) control.Enabled = true;
            }
            catch (Exception error)
            {
                if (IsDisposed || Disposing) return;
                ResetView();
                MessageBox.Show(this, error.Message, "Ошибка чтения SAV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
                if (!IsDisposed && !Disposing) HideLoading();
            }
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            if (current == null) return;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Сохранение безопасной копии"; dialog.Filter = "Сохранения SRHD (*.sav)|*.sav";
                dialog.InitialDirectory = !string.IsNullOrWhiteSpace(appSettings.LastDirectory) &&
                    Directory.Exists(appSettings.LastDirectory) ? appSettings.LastDirectory :
                    Path.GetDirectoryName(current.SourcePath);
                dialog.FileName = Path.GetFileNameWithoutExtension(current.SourcePath) + ".SRHDSaveEditor.sav";
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                appSettings.LastDirectory = Path.GetDirectoryName(dialog.FileName);
                if (File.Exists(dialog.FileName))
                {
                    MessageBox.Show(this, "Выберите новое имя: редактор не перезаписывает существующие SAV.", "Безопасная запись", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    current.WriteCopy(dialog.FileName, pendingMetadata, pendingMessages, pendingGalaxy, pendingStars,
                        pendingPlanets, pendingShips, pendingItems, pendingAchievements, pendingHoles, pendingAsteroids,
                        pendingMissiles, pendingCustomWeapons, pendingInterfaceOverrides, pendingStoredItems,
                        pendingGalaxySummary, pendingConstellations);
                    MessageBox.Show(this, "Копия записана и повторно проверена. Изменения TGalaxy (включая задания, новости, операции, врата и активные TScript), TStar/TPlanet/TShip/TItem/TMissile/TAsteroid/THole, TCustomWeaponInfo, TStoredItem, переопределений интерфейса и статистики достижений перечитаны из SAV; неизвестные модовые хвосты сохранены побайтно.", "Space Rangers HD Save Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception error)
                {
                    MessageBox.Show(this, error.Message, "Ошибка записи SAV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SettingsClicked(object sender, EventArgs e)
        {
            using (Form dialog = EditorFormFactory.Build(EditorFormDefinitions.Get("TSETTINGSFORM")))
            {
                ComboBox language = FindControl<ComboBox>(dialog, "cbLanguage");
                DirectoryEditControl gamePath = FindControl<DirectoryEditControl>(dialog, "edGamePath");
                CheckBox fullLog = FindControl<CheckBox>(dialog, "chbFullLog");
                GroupBox common = FindControl<GroupBox>(dialog, "gbCommon");
                Label languageLabel = FindControl<Label>(dialog, "lblLanguage");
                Label gamePathLabel = FindControl<Label>(dialog, "lblGamePath");
                Action refreshLanguage = delegate
                {
                    bool english = language.SelectedIndex == 1;
                    dialog.Text = english ? "Settings" : "Настройки";
                    common.Text = english ? "General" : "Общие";
                    languageLabel.Text = english ? "Language:" : "Язык:";
                    gamePathLabel.Text = english ? "Game path:" : "Путь к игре:";
                    fullLog.Text = english ? "Full logging" : "Полное логирование";
                };
                language.SelectedIndex = Math.Max(0, Math.Min(language.Items.Count - 1, appSettings.LanguageIndex));
                gamePath.Value = appSettings.GamePath;
                fullLog.Checked = appSettings.FullLog;
                language.SelectedIndexChanged += delegate { refreshLanguage(); };
                dialog.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) dialog.Close();
                };
                refreshLanguage();
                dialog.ShowDialog(this);
                int previousLanguage = appSettings.LanguageIndex;
                appSettings.LanguageIndex = language.SelectedIndex;
                appSettings.GamePath = gamePath.Value;
                appSettings.FullLog = fullLog.Checked;
                try { appSettings.Save(); }
                catch (Exception error)
                {
                    MessageBox.Show(this, error.Message, "Ошибка сохранения настроек", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (previousLanguage != appSettings.LanguageIndex)
                {
                    Application.Restart();
                    Close();
                }
            }
        }

        private void RestoreFilterSettings()
        {
            bool value;
            foreach (KeyValuePair<string, CheckBox> pair in galaxyFilters)
                if (appSettings.GalaxyFilters.TryGetValue(pair.Key, out value))
                    pair.Value.Checked = value;
            foreach (KeyValuePair<string, CheckBox> pair in searchFilters)
                if (appSettings.SearchFilters.TryGetValue(pair.Key, out value))
                    pair.Value.Checked = value;
            if (galaxyObjectMaster != null)
                galaxyObjectMaster.Checked = AllEnabledFiltersChecked(galaxyFilters,
                    new string[] { "planets", "stations", "equipment", "goods", "useless", "nods", "missiles", "asteroids", "holes" });
            if (galaxyShipMaster != null)
                galaxyShipMaster.Checked = AllEnabledFiltersChecked(galaxyFilters,
                    new string[] { "rangers", "warriors", "flagships", "transports", "liners", "diplomats", "pirates", "clanpirates", "tranclucators", "dominators", "bertors", "bosses" });
            if (searchFilterMaster != null)
                searchFilterMaster.Checked = AllEnabledFiltersChecked(searchFilters, null);
        }

        private static bool AllEnabledFiltersChecked(Dictionary<string, CheckBox> filters,
            string[] keys)
        {
            HashSet<string> allowed = keys == null ? null : new HashSet<string>(keys,
                StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, CheckBox> pair in filters)
                if ((allowed == null || allowed.Contains(pair.Key)) && pair.Value.Enabled &&
                    !pair.Value.Checked) return false;
            return true;
        }

        private void SaveUiSettings()
        {
            if (!persistUiSettings) return;
            foreach (KeyValuePair<string, CheckBox> pair in galaxyFilters)
                appSettings.GalaxyFilters[pair.Key] = pair.Value.Checked;
            foreach (KeyValuePair<string, CheckBox> pair in searchFilters)
                appSettings.SearchFilters[pair.Key] = pair.Value.Checked;
            try { appSettings.Save(); }
            catch { /* Optional UI preferences must never block shutdown. */ }
        }

        private void GalaxyClicked(object sender, EventArgs e)
        {
            EditGalaxy();
        }

        private void EditGalaxy()
        {
            if (current == null || pendingGalaxySummary == null) return;
            using (Form dialog = EditorFormFactory.Build(EditorFormDefinitions.Get("TGALAXYFORM")))
            {
                SetUnsupportedEditorsReadOnly(dialog);
                string[] difficultyNames = { "Легко (50%)", "Нормально (100%)", "Сложно (150%)",
                    "Эксперт (200%)", "250%", "300%", "350%", "400%", "450%", "500%" };
                string[] difficultyControls = { "cbPirateDifLevel", "cbTradeDifLevel", "cbScnDifLevel",
                    "cbRepairDifLevel", "cbTechDifLevel", "cbQuestDifLevel", "cbHoleDifLevel",
                    "cbBalanceDifLevel" };
                ComboBox[] difficulties = new ComboBox[difficultyControls.Length];
                for (int index = 0; index < difficulties.Length; index++)
                {
                    difficulties[index] = FindControl<ComboBox>(dialog, difficultyControls[index]);
                    PopulateByteCombo(difficulties[index], pendingGalaxySummary.DifficultyLevels[index],
                        difficultyNames);
                }
                FindControl<GroupBox>(dialog, "gbDifficulty").Text = "Сложность — " +
                    pendingGalaxySummary.DifficultyPercent.ToString(CultureInfo.InvariantCulture) + "%";

                TextBox blazerResearch = BindEditableText(dialog, "edBlazerResearch",
                    pendingGalaxySummary.BlazerResearch.ToString("R", CultureInfo.InvariantCulture));
                TextBox kellerResearch = BindEditableText(dialog, "edKellerResearch",
                    pendingGalaxySummary.KellerResearch.ToString("R", CultureInfo.InvariantCulture));
                TextBox terronResearch = BindEditableText(dialog, "edTerronResearch",
                    pendingGalaxySummary.TerronResearch.ToString("R", CultureInfo.InvariantCulture));
                TextBox blazerMaterial = BindEditableText(dialog, "edBlazerMaterial",
                    pendingGalaxySummary.BlazerMaterial.ToString(CultureInfo.InvariantCulture));
                TextBox kellerMaterial = BindEditableText(dialog, "edKellerMaterial",
                    pendingGalaxySummary.KellerMaterial.ToString(CultureInfo.InvariantCulture));
                TextBox terronMaterial = BindEditableText(dialog, "edTerronMaterial",
                    pendingGalaxySummary.TerronMaterial.ToString(CultureInfo.InvariantCulture));
                TextBox warDominators = BindEditableText(dialog, "edWarDeltaWinDominators",
                    pendingGalaxySummary.WarDeltaDominators.ToString(CultureInfo.InvariantCulture));
                TextBox warPirates = BindEditableText(dialog, "edWarDeltaWinPirates",
                    pendingGalaxySummary.WarDeltaPirates.ToString(CultureInfo.InvariantCulture));
                TextBox warCoalition = BindEditableText(dialog, "edWarDeltaWinCoalition",
                    pendingGalaxySummary.WarDeltaCoalition.ToString(CultureInfo.InvariantCulture));
                ComboBox kellerAttackStar = FindControl<ComboBox>(dialog, "cbKellerAttackStar");
                PopulateStarReferenceCombo(kellerAttackStar, pendingGalaxySummary.KellerAttackStarObjectId);
                TextBox kellerAttackState = BindEditableText(dialog, "edKellerAttackState",
                    pendingGalaxySummary.KellerAttackState.ToString(CultureInfo.InvariantCulture));

                CheckBox ironWill = BindEditableCheck(dialog, "chbIronWill", pendingGalaxySummary.IronWill);
                CheckBox rejectedBattles = BindEditableCheck(dialog, "chbRejectedPB",
                    pendingGalaxySummary.PlanetBattlesDisabled);
                CheckBox customRules = BindCheckableGroup(dialog, "gbCustomRules",
                    pendingGalaxySummary.CustomRules);
                customRules.Text = "Использовать тонкие настройки";
                customRules.AutoSize = false;
                customRules.Size = new Size(260, 24);
                customRules.Location = new Point(14, 7);

                string[] sliderNames = { "tbDominatorsStrength", "tbDominatorsAggro", "tbDominatorsSpawn",
                    "tbPirateAggro", "tbCoalAggro", "tbExtraInventions", "tbExtraRangers",
                    "tbAsteroidMod", "tbSunDamageMod", "tbAgPlanets", "tbMiPlanets", "tbInPlanets",
                    "tbAkrinMod", "tbNodeDropMod", "tbDropValueMod", "tbABDropValueMod",
                    "tbABHitpointsMod", "tbABDamageMod", "tbAITolerateJunk" };
                string[] sliderLabels = { "lblKlingStrengthVal", "lblKlingAggroVal", "lblKlingSpawnVal",
                    "lblPirateAggroVal", "lblCoalAggroVal", "lblExtraInventionsVal",
                    "lblExtraRangersVal", "lblAsteroidModVal", "lblSunDamageModVal", "lblAgPlanetsVal",
                    "lblMiPlanetsVal", "lblInPlanetsVal", "lblAkrinModVal", "lblNodeDropModVal",
                    "lblDropValueModVal", "lblABDropValueModVal", "lblABHitpointsModVal",
                    "lblABDamageModVal", "lblAITolerateJunkVal" };
                int[] sliderFieldIndices = { 0, 1, 2, 3, 4, 7, 15, 5, 6, 12, 13, 14,
                    8, 9, 11, 10, 16, 17, 18 };
                int[] sliderMaximums = { 73, 73, 73, 73, 24, 255, 50, 24, 24, 16, 16, 16,
                    100, 24, 24, 24, 24, 24, 50 };
                TrackBar[] sliders = new TrackBar[sliderNames.Length];
                for (int index = 0; index < sliders.Length; index++)
                    sliders[index] = BindGalaxySlider(dialog, sliderNames[index], sliderLabels[index],
                        pendingGalaxySummary.CustomRuleLevels[sliderFieldIndices[index]],
                        sliderMaximums[index]);

                string[] flagNames = { "chbRnd", "chbTechKnowledge", "chbRuinsPos",
                    "chbRuinsTargetting", "chbSpecialShips", "chbZeroExp", "chbABattleRoyale",
                    "chbDominatorsRacialWeapons", "chbStartCenter", "chbMaxRangeMissiles", "chbOldHyper",
                    "chbPirateNodes", "chbAIBuysEqFromShops", "chbRuinsUsingShop",
                    "chbDuplicateArtsAllowed" };
                CheckBox[] flags = new CheckBox[flagNames.Length];
                for (int index = 0; index < flags.Length; index++)
                    flags[index] = BindEditableCheck(dialog, flagNames[index],
                        pendingGalaxySummary.CustomRuleFlags[index]);
                string[] lateFlagNames = { "chbABChangeEq", "chbOldSpeedCalc", "chbOldMissileBonuses" };
                CheckBox[] lateFlags = new CheckBox[lateFlagNames.Length];
                for (int index = 0; index < lateFlags.Length; index++)
                    lateFlags[index] = BindEditableCheck(dialog, lateFlagNames[index],
                        pendingGalaxySummary.CustomRuleLateFlags[index]);
                ComboBox hullGrowth = FindControl<ComboBox>(dialog, "cbHullGrowth");
                PopulateByteCombo(hullGrowth, pendingGalaxySummary.HullGrowth,
                    new string[] { "Нормальный", "Медленный", "Только технологии" });

                ListBox oldQuests = FindControl<ListBox>(dialog, "lbOldQuest");
                foreach (CompleteQuestRecord record in pendingGalaxySummary.CompleteQuests)
                    oldQuests.Items.Add(record);
                oldQuests.DoubleClick += delegate
                {
                    CompleteQuestRecord record = oldQuests.SelectedItem as CompleteQuestRecord;
                    if (record == null) return;
                    EditCompleteQuest(record);
                    int selected = oldQuests.SelectedIndex;
                    if (selected >= 0) oldQuests.Items[selected] = record;
                };
                ContextMenuStrip oldQuestMenu = new ContextMenuStrip();
                ToolStripMenuItem editOldQuest = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deleteOldQuest = new ToolStripMenuItem("Удалить");
                oldQuestMenu.Items.Add(editOldQuest);
                oldQuestMenu.Items.Add(deleteOldQuest);
                oldQuests.ContextMenuStrip = oldQuestMenu;
                oldQuests.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                    {
                        int index = oldQuests.IndexFromPoint(args.Location);
                        if (index >= 0) oldQuests.SelectedIndex = index;
                    }
                };
                editOldQuest.Click += delegate
                {
                    CompleteQuestRecord record = oldQuests.SelectedItem as CompleteQuestRecord;
                    if (record == null) return;
                    EditCompleteQuest(record);
                    int selected = oldQuests.SelectedIndex;
                    if (selected >= 0) oldQuests.Items[selected] = record;
                };
                deleteOldQuest.Click += delegate
                {
                    CompleteQuestRecord record = oldQuests.SelectedItem as CompleteQuestRecord;
                    if (record == null || MessageBox.Show(dialog, "Удалить выбранное завершённое задание?",
                        "Галактика", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    pendingGalaxySummary.CompleteQuests.Remove(record);
                    oldQuests.Items.Remove(record);
                };
                ListBox planetNews = FindControl<ListBox>(dialog, "lbPlanetNews");
                planetNews.FormattingEnabled = true;
                planetNews.Format += delegate(object formatSender, ListControlConvertEventArgs args)
                {
                    GalaxyNewsRecord news = args.ListItem as GalaxyNewsRecord;
                    if (news != null) args.Value = GameTextPreview(news.Text, 150);
                };
                foreach (GalaxyNewsRecord record in pendingGalaxySummary.GalaxyNews)
                    planetNews.Items.Add(record);
                planetNews.DoubleClick += delegate
                {
                    GalaxyNewsRecord record = planetNews.SelectedItem as GalaxyNewsRecord;
                    if (record == null) return;
                    EditGalaxyNews(record);
                    int selected = planetNews.SelectedIndex;
                    if (selected >= 0) planetNews.Items[selected] = record;
                };
                ContextMenuStrip planetNewsMenu = new ContextMenuStrip();
                ToolStripMenuItem editPlanetNews = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deletePlanetNews = new ToolStripMenuItem("Удалить");
                planetNewsMenu.Items.Add(editPlanetNews);
                planetNewsMenu.Items.Add(deletePlanetNews);
                planetNews.ContextMenuStrip = planetNewsMenu;
                planetNews.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                    {
                        int index = planetNews.IndexFromPoint(args.Location);
                        if (index >= 0) planetNews.SelectedIndex = index;
                    }
                };
                editPlanetNews.Click += delegate
                {
                    GalaxyNewsRecord record = planetNews.SelectedItem as GalaxyNewsRecord;
                    if (record == null) return;
                    EditGalaxyNews(record);
                    int selected = planetNews.SelectedIndex;
                    if (selected >= 0) planetNews.Items[selected] = record;
                };
                deletePlanetNews.Click += delegate
                {
                    GalaxyNewsRecord record = planetNews.SelectedItem as GalaxyNewsRecord;
                    if (record == null || MessageBox.Show(dialog, "Удалить выбранную новость?", "Галактика",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    pendingGalaxySummary.GalaxyNews.Remove(record);
                    planetNews.Items.Remove(record);
                };
                ListBox warOperations = FindControl<ListBox>(dialog, "lbWarOperations");
                foreach (WarOperationRecord record in pendingGalaxySummary.WarOperations)
                    warOperations.Items.Add(record);
                warOperations.DoubleClick += delegate
                {
                    WarOperationRecord record = warOperations.SelectedItem as WarOperationRecord;
                    if (record == null) return;
                    EditWarOperation(record);
                    int selected = warOperations.SelectedIndex;
                    if (selected >= 0) warOperations.Items[selected] = record;
                };
                ContextMenuStrip warOperationMenu = new ContextMenuStrip();
                ToolStripMenuItem editWarOperation = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deleteWarOperation = new ToolStripMenuItem("Удалить");
                warOperationMenu.Items.Add(editWarOperation);
                warOperationMenu.Items.Add(deleteWarOperation);
                warOperations.ContextMenuStrip = warOperationMenu;
                warOperations.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                    {
                        int index = warOperations.IndexFromPoint(args.Location);
                        if (index >= 0) warOperations.SelectedIndex = index;
                    }
                };
                editWarOperation.Click += delegate
                {
                    WarOperationRecord record = warOperations.SelectedItem as WarOperationRecord;
                    if (record == null) return;
                    EditWarOperation(record);
                    int selected = warOperations.SelectedIndex;
                    if (selected >= 0) warOperations.Items[selected] = record;
                };
                deleteWarOperation.Click += delegate
                {
                    WarOperationRecord record = warOperations.SelectedItem as WarOperationRecord;
                    if (record == null || MessageBox.Show(dialog, "Удалить выбранную военную операцию?",
                        "Галактика", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    pendingGalaxySummary.WarOperations.Remove(record);
                    warOperations.Items.Remove(record);
                };
                ListBox gates = FindControl<ListBox>(dialog, "lbGates");
                foreach (GateRecord record in pendingGalaxySummary.Gates) gates.Items.Add(record);
                gates.DoubleClick += delegate
                {
                    GateRecord record = gates.SelectedItem as GateRecord;
                    if (record == null) return;
                    EditGate(record);
                    int selected = gates.SelectedIndex;
                    if (selected >= 0) gates.Items[selected] = record;
                };
                ContextMenuStrip gateMenu = new ContextMenuStrip();
                ToolStripMenuItem editGate = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deleteGate = new ToolStripMenuItem("Удалить");
                gateMenu.Items.Add(editGate); gateMenu.Items.Add(deleteGate); gates.ContextMenuStrip = gateMenu;
                editGate.Click += delegate
                {
                    GateRecord record = gates.SelectedItem as GateRecord;
                    if (record == null) return;
                    EditGate(record);
                    int selected = gates.SelectedIndex;
                    if (selected >= 0) gates.Items[selected] = record;
                };
                deleteGate.Click += delegate
                {
                    GateRecord record = gates.SelectedItem as GateRecord;
                    if (record == null || MessageBox.Show(dialog, "Удалить выбранные врата?", "Галактика",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    pendingGalaxySummary.Gates.Remove(record);
                    gates.Items.Remove(record);
                };

                EventHandler customEnabledChanged = delegate
                {
                    bool enabled = customRules.Checked;
                    foreach (TrackBar slider in sliders) slider.Enabled = enabled;
                    foreach (CheckBox flag in flags) flag.Enabled = enabled;
                    foreach (CheckBox flag in lateFlags) flag.Enabled = enabled;
                    hullGrowth.Enabled = enabled;
                };
                customRules.CheckedChanged += customEnabledChanged;
                customEnabledChanged(customRules, EventArgs.Empty);
                dialog.ShowDialog(this);

                float parsedBlazerResearch, parsedKellerResearch, parsedTerronResearch;
                uint parsedBlazerMaterial, parsedKellerMaterial, parsedTerronMaterial;
                int parsedWarDominators, parsedWarPirates, parsedWarCoalition, parsedKellerAttackState;
                if (!TryParseFiniteFloat(blazerResearch.Text, out parsedBlazerResearch) ||
                    !TryParseFiniteFloat(kellerResearch.Text, out parsedKellerResearch) ||
                    !TryParseFiniteFloat(terronResearch.Text, out parsedTerronResearch) ||
                    !TryParseUInt32(blazerMaterial.Text, out parsedBlazerMaterial) ||
                    !TryParseUInt32(kellerMaterial.Text, out parsedKellerMaterial) ||
                    !TryParseUInt32(terronMaterial.Text, out parsedTerronMaterial) ||
                    !TryParseInt32(warDominators.Text, out parsedWarDominators) ||
                    !TryParseInt32(warPirates.Text, out parsedWarPirates) ||
                    !TryParseInt32(warCoalition.Text, out parsedWarCoalition) ||
                    !TryParseInt32(kellerAttackState.Text, out parsedKellerAttackState))
                {
                    MessageBox.Show(this, "Поля исследований, материалов и дельты сил должны содержать корректные числа.",
                        "Галактика", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                for (int index = 0; index < difficulties.Length; index++)
                    pendingGalaxySummary.DifficultyLevels[index] = SelectedByteValue(difficulties[index],
                        pendingGalaxySummary.DifficultyLevels[index]);
                pendingGalaxySummary.IronWill = ironWill.Checked;
                pendingGalaxySummary.BlazerResearch = parsedBlazerResearch;
                pendingGalaxySummary.KellerResearch = parsedKellerResearch;
                pendingGalaxySummary.TerronResearch = parsedTerronResearch;
                pendingGalaxySummary.BlazerMaterial = parsedBlazerMaterial;
                pendingGalaxySummary.KellerMaterial = parsedKellerMaterial;
                pendingGalaxySummary.TerronMaterial = parsedTerronMaterial;
                pendingGalaxySummary.WarDeltaDominators = parsedWarDominators;
                pendingGalaxySummary.WarDeltaPirates = parsedWarPirates;
                pendingGalaxySummary.WarDeltaCoalition = parsedWarCoalition;
                UInt32ValueChoice selectedKellerStar = kellerAttackStar.SelectedItem as UInt32ValueChoice;
                if (selectedKellerStar != null)
                    pendingGalaxySummary.KellerAttackStarObjectId = selectedKellerStar.Value;
                pendingGalaxySummary.KellerAttackState = parsedKellerAttackState;
                pendingGalaxySummary.PlanetBattlesDisabled = rejectedBattles.Checked;
                pendingGalaxySummary.CustomRules = customRules.Checked;
                for (int index = 0; index < sliders.Length; index++)
                    pendingGalaxySummary.CustomRuleLevels[sliderFieldIndices[index]] =
                        checked((byte)sliders[index].Value);
                for (int index = 0; index < flags.Length; index++)
                    pendingGalaxySummary.CustomRuleFlags[index] = flags[index].Checked;
                for (int index = 0; index < lateFlags.Length; index++)
                    pendingGalaxySummary.CustomRuleLateFlags[index] = lateFlags[index].Checked;
                pendingGalaxySummary.HullGrowth = SelectedByteValue(hullGrowth,
                    pendingGalaxySummary.HullGrowth);
                values["difficulty"].Text = pendingGalaxySummary.DifficultyPercent.ToString(
                    CultureInfo.InvariantCulture) + "%";
                values["iron"].Text = pendingGalaxySummary.IronWill ? "Включена" : "Выключена";
                values["custom"].Text = pendingGalaxySummary.CustomRules ? "Включены" : "Выключены";
                values["battles"].Text = pendingGalaxySummary.PlanetBattlesDisabled ?
                    "Выключены" : "Включены";
            }
        }

        private static TrackBar BindGalaxySlider(Control root, string controlName, string labelName,
            byte value, int normalMaximum)
        {
            TrackBar slider = FindControl<TrackBar>(root, controlName);
            Label label = FindControl<Label>(root, labelName);
            slider.Minimum = 0;
            slider.Maximum = Math.Max(normalMaximum, value);
            slider.Value = value;
            slider.Enabled = true;
            EventHandler changed = delegate
            {
                label.Text = slider.Value.ToString(CultureInfo.InvariantCulture);
            };
            slider.ValueChanged += changed;
            changed(slider, EventArgs.Empty);
            return slider;
        }

        private static byte SelectedByteValue(ComboBox combo, byte fallback)
        {
            ByteValueChoice choice = combo.SelectedItem as ByteValueChoice;
            return choice == null ? fallback : choice.Value;
        }

        private void EditCompleteQuest(CompleteQuestRecord record)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TOLDQUESTFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox type = FindControl<ComboBox>(form, "cbTypeQuest");
                PopulateByteCombo(type, record.Type, new string[] { "Доставка письма", "Уничтожение корабля",
                    "Планетарное задание", "Защита системы", "Защита корабля" });
                TextBox number = BindEditableText(form, "edQuestNumber",
                    record.Number.ToString(CultureInfo.InvariantCulture));
                ComboBox planet = FindControl<ComboBox>(form, "cbPlanet");
                PopulatePlanetReferenceCombo(planet, record.PlanetObjectId);
                TextBox text = BindEditableText(form, "mmTextQuest", record.Text ?? string.Empty);
                CheckBox successful = BindEditableCheck(form, "chbSuccessful", record.Successful);
                CheckBox rejection = BindEditableCheck(form, "chbRejection", record.Rejection);
                CheckBox hideTags = BindEditableCheck(form, "chbHideTags", true);
                Func<string> readRawText = ConfigureTagFilteredEditor(text, hideTags,
                    record.Text ?? string.Empty);
                form.ShowDialog(this);
                ushort parsedNumber;
                UInt32ValueChoice selectedPlanet = planet.SelectedItem as UInt32ValueChoice;
                if (!TryParseUInt16(number.Text, out parsedNumber) || selectedPlanet == null)
                {
                    MessageBox.Show(this, "Номер задания или ссылка на планету имеют неверный формат.",
                        "Пройденное задание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Type = SelectedByteValue(type, record.Type);
                record.Number = parsedNumber;
                record.PlanetObjectId = selectedPlanet.Value;
                record.Text = readRawText();
                record.Successful = successful.Checked;
                record.Rejection = rejection.Checked;
            }
        }

        private bool EditRangerQuest(RangerQuestRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TQUESTFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox type = FindControl<ComboBox>(form, "cbTypeQuest");
                PopulateByteCombo(type, record.Type, new string[] { "Доставка письма",
                    "Уничтожение корабля", "Планетарное задание", "Защита системы",
                    "Защита корабля" });
                TextBox number = BindEditableText(form, "edQuestNumber",
                    record.Number.ToString(CultureInfo.InvariantCulture));
                ComboBox planet = FindControl<ComboBox>(form, "cbPlanet");
                PopulatePlanetReferenceCombo(planet, record.PlanetObjectId);
                TextBox turn = BindEditableText(form, "edTurn",
                    record.Turn.ToString(CultureInfo.InvariantCulture));
                TextBox reward = BindEditableText(form, "edSumm",
                    record.Reward.ToString(CultureInfo.InvariantCulture));
                ComboBox questObject = FindControl<ComboBox>(form, "cbObj");
                PopulateQuestObjectReferenceCombo(questObject, record.ObjectId);
                CheckBox successful = BindEditableCheck(form, "chbSuccessful", record.Successful);
                CheckBox showTags = BindEditableCheck(form, "chbHideTags", true);
                TextBox[] texts = {
                    BindEditableText(form, "mmText", record.Text ?? string.Empty),
                    BindEditableText(form, "mmCongratulations", record.Congratulations ?? string.Empty),
                    BindEditableText(form, "mmSpecial", record.SpecialText ?? string.Empty)
                };
                string[] rawTexts = { texts[0].Text, texts[1].Text, texts[2].Text };
                showTags.CheckedChanged += delegate
                {
                    for (int index = 0; index < texts.Length; index++)
                    {
                        if (showTags.Checked)
                        {
                            texts[index].Text = rawTexts[index];
                            texts[index].ReadOnly = false;
                        }
                        else
                        {
                            rawTexts[index] = texts[index].Text;
                            texts[index].Text = FilterGameTextTags(rawTexts[index]);
                            texts[index].ReadOnly = true;
                        }
                    }
                };
                form.KeyPreview = true;
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);

                ushort parsedNumber;
                int parsedTurn, parsedReward;
                UInt32ValueChoice selectedPlanet = planet.SelectedItem as UInt32ValueChoice;
                UInt32ValueChoice selectedObject = questObject.SelectedItem as UInt32ValueChoice;
                string[] updatedTexts = showTags.Checked
                    ? new string[] { texts[0].Text, texts[1].Text, texts[2].Text }
                    : rawTexts;
                if (!TryParseUInt16(number.Text, out parsedNumber) ||
                    !TryParseInt32(turn.Text, out parsedTurn) ||
                    !TryParseInt32(reward.Text, out parsedReward) || selectedPlanet == null ||
                    selectedObject == null || updatedTexts[0].Length > 32768 ||
                    updatedTexts[1].Length > 32768 || updatedTexts[2].Length > 32768)
                {
                    MessageBox.Show(owner,
                        "Поля задания не применены: проверьте номер, ход, награду, ссылки и длину текста.",
                        "TQuest", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                record.Type = SelectedByteValue(type, record.Type);
                record.Number = parsedNumber;
                record.PlanetObjectId = selectedPlanet.Value;
                record.Turn = parsedTurn;
                record.Reward = parsedReward;
                record.ObjectId = selectedObject.Value;
                record.Successful = successful.Checked;
                record.Text = updatedTexts[0];
                record.Congratulations = updatedTexts[1];
                record.SpecialText = updatedTexts[2];
                return true;
            }
        }

        private bool EditPlayerJournalRecord(PlayerJournalRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TJOURNALRECORDFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox turn = BindEditableText(form, "edTurn",
                    record.Turn.ToString(CultureInfo.InvariantCulture));
                TextBox text = BindEditableText(form, "mmText", record.Text ?? string.Empty);
                form.KeyPreview = true;
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);
                int parsedTurn;
                if (!TryParseInt32(turn.Text, out parsedTurn) || text.Text.Length > 32768)
                {
                    MessageBox.Show(owner,
                        "Запись журнала не применена: проверьте ход и длину текста.",
                        "TJournalRecord", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                record.Turn = parsedTurn;
                record.Text = text.Text;
                return true;
            }
        }

        private bool EditPlayerRobotMap(PlayerRobotMapRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TROBOTMAPSTATFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                form.Text = "Планетарный бой";
                string[] names = { "edId", "edTime", "edBuildRobot", "edKillRobot",
                    "edBuildTurret", "edKillTurret", "edKillBuilding", "edBonus",
                    "edState", "edTurn" };
                int[] values = { record.Id, record.Time, record.BuildRobot, record.KillRobot,
                    record.BuildTurret, record.KillTurret, record.KillBuilding, record.Bonus,
                    record.State, record.Turn };
                TextBox[] editors = new TextBox[names.Length];
                for (int index = 0; index < names.Length; index++)
                    editors[index] = BindEditableText(form, names[index],
                        values[index].ToString(CultureInfo.InvariantCulture));
                form.ShowDialog(owner);

                int[] parsed = new int[editors.Length];
                for (int index = 0; index < editors.Length; index++)
                    if (!TryParseInt32(editors[index].Text, out parsed[index]))
                    {
                        MessageBox.Show(owner,
                            "Карта роботов не применена: все десять полей должны быть Int32.",
                            "TRobotMapStat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                record.Id = parsed[0]; record.Time = parsed[1];
                record.BuildRobot = parsed[2]; record.KillRobot = parsed[3];
                record.BuildTurret = parsed[4]; record.KillTurret = parsed[5];
                record.KillBuilding = parsed[6]; record.Bonus = parsed[7];
                record.State = parsed[8]; record.Turn = parsed[9];
                return true;
            }
        }

        private bool EditShipIllness(ShipIllnessRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TILLNESSFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                form.Text = record.Stimulator ? "Стимулятор" : "Болезнь " + record.Index;
                TextBox infection = BindEditableText(form, "edInfection",
                    record.Infection.ToString("R", CultureInfo.InvariantCulture));
                TextBox infectionDay = BindEditableText(form, "edInfectionDay",
                    record.InfectionDay.ToString(CultureInfo.InvariantCulture));
                TextBox infectionEndDay = BindEditableText(form, "edInfectionEndDay",
                    record.InfectionEndDay.ToString(CultureInfo.InvariantCulture));
                TextBox infectionCount = BindEditableText(form, "edInfectionCount",
                    record.InfectionCount.ToString(CultureInfo.InvariantCulture));
                form.KeyPreview = true;
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);

                float parsedInfection;
                int parsedDay, parsedEndDay, parsedCount;
                if (!TryParseFiniteFloat(infection.Text, out parsedInfection) ||
                    !TryParseInt32(infectionDay.Text, out parsedDay) ||
                    !TryParseInt32(infectionEndDay.Text, out parsedEndDay) ||
                    !TryParseInt32(infectionCount.Text, out parsedCount))
                {
                    MessageBox.Show(owner,
                        "Запись болезни или стимулятора не применена: проверьте числовые поля.",
                        "TIllness", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                record.Infection = parsedInfection;
                record.InfectionDay = parsedDay;
                record.InfectionEndDay = parsedEndDay;
                record.InfectionCount = parsedCount;
                return true;
            }
        }

        private bool EditShipReward(byte current, IWin32Window owner, out byte updated)
        {
            updated = current;
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TREWARDFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox reward = FindControl<ComboBox>(form, "cbReward");
                int maximum = current;
                foreach (byte index in gameCatalog.RewardNames.Keys)
                    if (index > maximum) maximum = index;
                for (int index = 0; index <= maximum; index++)
                {
                    byte value = (byte)index;
                    reward.Items.Add(new ByteValueChoice(value, RewardDisplayName(value)));
                    if (value == current) reward.SelectedIndex = reward.Items.Count - 1;
                }
                if (reward.SelectedIndex < 0 && reward.Items.Count != 0) reward.SelectedIndex = 0;
                reward.Enabled = true;
                form.KeyPreview = true;
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);
                ByteValueChoice selected = reward.SelectedItem as ByteValueChoice;
                if (selected == null) return false;
                updated = selected.Value;
                return true;
            }
        }

        private string RewardDisplayName(byte value)
        {
            string name;
            return gameCatalog.RewardNames.TryGetValue(value, out name) &&
                !string.IsNullOrWhiteSpace(name)
                ? name : value.ToString(CultureInfo.InvariantCulture);
        }

        private void EditGalaxyNews(GalaxyNewsRecord record)
        {
            EditGalaxyNews(record, this);
        }

        private void EditGalaxyNews(GalaxyNewsRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TPLANETNEWSFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox id = BindEditableText(form, "edId", record.Id.ToString(CultureInfo.InvariantCulture));
                TextBox turn = BindEditableText(form, "edTurn", record.Turn.ToString(CultureInfo.InvariantCulture));
                ComboBox type = FindControl<ComboBox>(form, "cbType");
                PopulateByteCombo(type, record.Type, GalaxyNewsTypeNames());
                TextBox text = BindEditableText(form, "mmNewsText", record.Text ?? string.Empty);
                CheckBox hideTags = BindEditableCheck(form, "chbHideTags", true);
                Func<string> readRawText = ConfigureFormattedGameTextEditor(form, text, hideTags,
                    record.Text ?? string.Empty);
                form.ShowDialog(owner);
                uint parsedId, parsedTurn;
                if (!TryParseUInt32(id.Text, out parsedId) || !TryParseUInt32(turn.Text, out parsedTurn))
                {
                    MessageBox.Show(owner, "ID и ход новости должны быть целыми неотрицательными числами.",
                        "Новость", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Id = parsedId;
                record.Turn = parsedTurn;
                record.Type = SelectedByteValue(type, record.Type);
                record.Text = readRawText();
            }
        }

        private static Func<string> ConfigureTagFilteredEditor(TextBox editor,
            CheckBox showRaw, string initialText)
        {
            string rawText = initialText ?? string.Empty;
            editor.Text = rawText;
            editor.ReadOnly = false;
            showRaw.Enabled = true;
            showRaw.Checked = true;
            showRaw.CheckedChanged += delegate
            {
                if (showRaw.Checked)
                {
                    editor.Text = rawText;
                    editor.ReadOnly = false;
                }
                else
                {
                    rawText = editor.Text;
                    editor.Text = FilterGameTextTags(rawText);
                    editor.ReadOnly = true;
                }
            };
            return delegate { return showRaw.Checked ? editor.Text : rawText; };
        }

        private static Func<string> ConfigureFormattedGameTextEditor(Form form,
            TextBox source, CheckBox legacyToggle, string initialText)
        {
            string rawText = initialText ?? string.Empty;
            Control parent = source.Parent;
            Rectangle originalBounds = source.Bounds;
            source.Visible = false;
            legacyToggle.Visible = false;

            ComboBox mode = new ComboBox();
            mode.Name = "$gameTextMode";
            mode.DropDownStyle = ComboBoxStyle.DropDownList;
            mode.Items.AddRange(new object[] {
                "Форматированный вид", "Текст без тегов", "Исходник с тегами"
            });
            int modeWidth = Math.Min(230, Math.Max(185,
                parent.ClientSize.Width - legacyToggle.Left - 8));
            mode.SetBounds(Math.Max(8, legacyToggle.Left), legacyToggle.Top, modeWidth, 25);

            RichTextBox preview = new RichTextBox();
            preview.Name = "$formattedNewsText";
            preview.Font = source.Font;
            preview.ForeColor = source.ForeColor;
            preview.BackColor = SystemColors.Window;
            preview.BorderStyle = BorderStyle.FixedSingle;
            preview.ScrollBars = RichTextBoxScrollBars.Vertical;
            preview.DetectUrls = false;
            preview.SetBounds(originalBounds.Left, originalBounds.Top,
                originalBounds.Width, originalBounds.Height);
            preview.Anchor = source.Anchor;
            parent.Controls.Add(mode);
            parent.Controls.Add(preview);
            preview.BringToFront();
            mode.BringToFront();

            int previousMode = -1;
            Action refresh = delegate
            {
                if (previousMode == 2) rawText = preview.Text;
                previousMode = mode.SelectedIndex;
                if (previousMode == 0)
                {
                    preview.ReadOnly = true;
                    RenderGameText(preview, rawText);
                }
                else if (previousMode == 1)
                {
                    preview.ReadOnly = true;
                    preview.Text = FilterGameTextTags(rawText);
                }
                else
                {
                    preview.ReadOnly = false;
                    preview.Text = rawText;
                }
            };
            mode.SelectedIndexChanged += delegate { refresh(); };
            mode.SelectedIndex = 0;

            Dictionary<string, Control> controls = form.Tag as Dictionary<string, Control>;
            if (controls != null)
            {
                controls[mode.Name] = mode;
                controls[preview.Name] = preview;
            }
            return delegate
            {
                if (mode.SelectedIndex == 2) rawText = preview.Text;
                return rawText;
            };
        }

        private static string[] GalaxyNewsTypeNames()
        {
            return new string[] {
                "Новости галактики", "Мятеж: анархия", "Мятеж: диктатура", "Мятеж: монархия",
                "Мятеж: республика", "Мятеж: демократия", "Месторождение минералов",
                "Нужны минералы", "Много оружия", "Нужно оружие", "Много техники",
                "Много продовольствия", "Нужно продовольствие", "Много медикаментов",
                "Много роскоши", "Нужна роскошь", "Много алкоголя", "Нужен алкоголь",
                "Транспорт в системе", "Много пиратов", "Несколько пиратов", "Пиратов нет",
                "Рейнджеры в системе", "Лучший рейнджер", "Атака доминаторов",
                "Доминаторы проиграли", "Военная операция", "Атака пиратов", "Пираты проиграли",
                "Коалиция захватила доминаторскую систему", "Коалиция захватила пиратскую систему",
                "Пираты захватили доминаторскую систему", "Пираты захватили систему коалиции",
                "Доминаторы захватили систему коалиции", "Доминаторы захватили пиратскую систему",
                "Коалиция побеждена", "Новая чёрная дыра", "Выдающийся воин",
                "Выдающийся торговец", "Выдающийся пират", "Рейнджер заключён",
                "Новая станция", "Инвестиция", "Программа исследована", "Особый корабль",
                "Прыжок военной базы запланирован"
            };
        }

        private void EditWarOperation(WarOperationRecord record)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TWAROPERATIONFORM")))
            {
                TextBox turn = BindEditableText(form, "edTurn", record.Turn.ToString(CultureInfo.InvariantCulture));
                TextBox randomSeed = BindEditableText(form, "edRnd", record.RandomSeed.ToString(CultureInfo.InvariantCulture));
                TextBox randomOut = BindEditableText(form, "edRndOut", record.RandomOut.ToString(CultureInfo.InvariantCulture));
                ListBox ships = FindControl<ListBox>(form, "lbShips");
                foreach (uint shipId in record.ShipObjectIds)
                    ships.Items.Add(new UInt32ValueChoice(shipId, ShipName(shipId)));
                ships.DoubleClick += delegate
                {
                    UInt32ValueChoice selected = ships.SelectedItem as UInt32ValueChoice;
                    if (selected == null || pendingShips == null) return;
                    foreach (ShipHeaderRecord ship in pendingShips)
                        if (ship.ObjectId == selected.Value) { EditShip(ship); break; }
                };
                ContextMenuStrip shipMenu = new ContextMenuStrip();
                ToolStripMenuItem editShip = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deleteShip = new ToolStripMenuItem("Удалить из операции");
                shipMenu.Items.Add(editShip); shipMenu.Items.Add(deleteShip); ships.ContextMenuStrip = shipMenu;
                editShip.Click += delegate
                {
                    UInt32ValueChoice selected = ships.SelectedItem as UInt32ValueChoice;
                    if (selected == null || pendingShips == null) return;
                    foreach (ShipHeaderRecord ship in pendingShips)
                        if (ship.ObjectId == selected.Value) { EditShip(ship); break; }
                };
                deleteShip.Click += delegate
                {
                    UInt32ValueChoice selected = ships.SelectedItem as UInt32ValueChoice;
                    if (selected == null) return;
                    record.ShipObjectIds.Remove(selected.Value);
                    ships.Items.Remove(selected);
                };

                ListBox orders = FindControl<ListBox>(form, "lbOrders");
                foreach (WarOperationOrderRecord order in record.Orders) orders.Items.Add(order);
                orders.DoubleClick += delegate
                {
                    WarOperationOrderRecord order = orders.SelectedItem as WarOperationOrderRecord;
                    if (order == null) return;
                    EditWarOperationOrder(order);
                    int selected = orders.SelectedIndex;
                    if (selected >= 0) orders.Items[selected] = order;
                };
                ContextMenuStrip orderMenu = new ContextMenuStrip();
                ToolStripMenuItem editOrder = new ToolStripMenuItem("Редактировать");
                ToolStripMenuItem deleteOrder = new ToolStripMenuItem("Удалить");
                orderMenu.Items.Add(editOrder); orderMenu.Items.Add(deleteOrder); orders.ContextMenuStrip = orderMenu;
                editOrder.Click += delegate
                {
                    WarOperationOrderRecord order = orders.SelectedItem as WarOperationOrderRecord;
                    if (order == null) return;
                    EditWarOperationOrder(order);
                    int selected = orders.SelectedIndex;
                    if (selected >= 0) orders.Items[selected] = order;
                };
                deleteOrder.Click += delegate
                {
                    WarOperationOrderRecord order = orders.SelectedItem as WarOperationOrderRecord;
                    if (order == null) return;
                    record.Orders.Remove(order);
                    orders.Items.Remove(order);
                };
                form.ShowDialog(this);
                ushort parsedTurn; uint parsedRandomSeed, parsedRandomOut;
                if (!TryParseUInt16(turn.Text, out parsedTurn) ||
                    !TryParseUInt32(randomSeed.Text, out parsedRandomSeed) ||
                    !TryParseUInt32(randomOut.Text, out parsedRandomOut))
                {
                    MessageBox.Show(this, "Поля военной операции должны содержать корректные числа.",
                        "Военная операция", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Turn = parsedTurn; record.RandomSeed = parsedRandomSeed; record.RandomOut = parsedRandomOut;
            }
        }

        private void EditGate(GateRecord record)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TGATEFORM")))
            {
                TextBox angle = BindEditableText(form, "edAngle", record.Angle.ToString(CultureInfo.InvariantCulture));
                TextBox x = BindEditableText(form, "edPosX", record.X.ToString("R", CultureInfo.InvariantCulture));
                TextBox y = BindEditableText(form, "edPosY", record.Y.ToString("R", CultureInfo.InvariantCulture));
                TextBox text = BindEditableText(form, "edText", record.Text ?? string.Empty);
                TextBox sizeX = BindEditableText(form, "edSizeX", record.Size.ToString(CultureInfo.InvariantCulture));
                TextBox sizeY = BindEditableText(form, "edSizeY", record.Size.ToString(CultureInfo.InvariantCulture));
                bool synchronizing = false;
                sizeX.TextChanged += delegate
                {
                    if (synchronizing) return;
                    synchronizing = true; sizeY.Text = sizeX.Text; synchronizing = false;
                };
                sizeY.TextChanged += delegate
                {
                    if (synchronizing) return;
                    synchronizing = true; sizeX.Text = sizeY.Text; synchronizing = false;
                };
                form.ShowDialog(this);
                byte parsedAngle; float parsedX, parsedY; ushort parsedSize;
                if (!TryParseByte(angle.Text, out parsedAngle) || !TryParseFiniteFloat(x.Text, out parsedX) ||
                    !TryParseFiniteFloat(y.Text, out parsedY) || !TryParseUInt16(sizeX.Text, out parsedSize))
                {
                    MessageBox.Show(this, "Врата содержат неверный угол, координаты или размер.",
                        "Врата", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Angle = parsedAngle; record.X = parsedX; record.Y = parsedY;
                record.Size = parsedSize; record.Text = text.Text;
            }
        }

        private void EditWarOperationOrder(WarOperationOrderRecord record)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TORDERFORM")))
            {
                ComboBox type = FindControl<ComboBox>(form, "cbOrderType");
                PopulateByteCombo(type, record.Type, new string[] { "Нет", "Движение", "Посадка", "Прыжок" });
                ComboBox target = FindControl<ComboBox>(form, "cbOrderObj");
                Action populateTargets = delegate
                {
                    byte selectedType = SelectedByteValue(type, record.Type);
                    PopulateWarOperationTargetCombo(target, selectedType, record.ObjectId);
                };
                type.SelectedIndexChanged += delegate { populateTargets(); };
                populateTargets();
                TextBox destinationX = BindEditableText(form, "edOrderDesX",
                    record.DestinationX.ToString("R", CultureInfo.InvariantCulture));
                TextBox destinationY = BindEditableText(form, "edOrderDesY",
                    record.DestinationY.ToString("R", CultureInfo.InvariantCulture));
                ComboBox endMode = FindControl<ComboBox>(form, "cbOrderEnd");
                PopulateByteCombo(endMode, record.EndMode, new string[] {
                    "Обычное", "Ближайший в группе", "Группа на месте", "Группа к ходу" });
                TextBox endTurn = BindEditableText(form, "edOrderEndTime",
                    record.EndTurn.ToString(CultureInfo.InvariantCulture));
                form.ShowDialog(this);
                float parsedX, parsedY; int parsedEndTurn;
                UInt32ValueChoice selectedTarget = target.SelectedItem as UInt32ValueChoice;
                if (!TryParseFiniteFloat(destinationX.Text, out parsedX) ||
                    !TryParseFiniteFloat(destinationY.Text, out parsedY) ||
                    !TryParseInt32(endTurn.Text, out parsedEndTurn) || selectedTarget == null)
                {
                    MessageBox.Show(this, "Приказ содержит неверную цель, координаты или ход окончания.",
                        "Приказ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Type = SelectedByteValue(type, record.Type);
                record.ObjectId = selectedTarget.Value;
                record.DestinationX = parsedX; record.DestinationY = parsedY;
                record.EndMode = SelectedByteValue(endMode, record.EndMode); record.EndTurn = parsedEndTurn;
            }
        }

        private void PopulateWarOperationTargetCombo(ComboBox combo, byte type, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (type == 2 && pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                {
                    combo.Items.Add(new UInt32ValueChoice(planet.ObjectId,
                        planet.Name + " [ID " + planet.ObjectId + "]"));
                    if (planet.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (type == 2 && pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                {
                    uint encoded = ship.ObjectId | 0x80000000U;
                    combo.Items.Add(new UInt32ValueChoice(encoded, ship.Name + " [корабль " + ship.ObjectId + "]"));
                    if (encoded == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (type == 3 && pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                {
                    combo.Items.Add(new UInt32ValueChoice(star.ObjectId,
                        LocalizedStarName(star) + " [ID " + star.ObjectId + "]"));
                    if (star.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (type == 4 && pendingHoles != null)
                foreach (HoleRecord hole in pendingHoles)
                {
                    combo.Items.Add(new UInt32ValueChoice(hole.ObjectId, "Чёрная дыра " + hole.ObjectId));
                    if (hole.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (type == 6 && pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                {
                    combo.Items.Add(new UInt32ValueChoice(ship.ObjectId, ship.Name + " [ID " + ship.ObjectId + "]"));
                    if (ship.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId, "ID " + selectedId));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private static void PopulateFollowTypeCombo(ComboBox combo, uint selectedValue)
        {
            combo.Items.Clear();
            string[] names = { "Ближе", "Минимум", "Максимум", "Камикадзе" };
            for (byte index = 0; index < names.Length; index++)
                combo.Items.Add(new ByteValueChoice(index, names[index]));
            int selected = selectedValue < (uint)names.Length ? (int)selectedValue : 0;
            combo.SelectedIndex = selected;
            combo.Enabled = true;
        }

        private void PopulateShipOrderTargetCombo(ComboBox combo, ShipHeaderRecord source,
            byte type, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = type == 2 || type == 3 || type == 4 ||
                type == 6 || type == 7;
            int selectedIndex = -1;
            StarHeaderRecord sourceStar = FindStarForOffset(source.Start);
            if (type == 2 && pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                {
                    StarHeaderRecord parent = FindStarForOffset(planet.Start);
                    if (sourceStar != null && parent != sourceStar)
                        continue;
                    AddOrderTargetChoice(combo, planet.ObjectId, "Планета: " + planet.Name,
                        selectedId, ref selectedIndex);
                }
            if ((type == 2 || type == 6) && pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (ship.ObjectId != source.ObjectId &&
                        (sourceStar == null || FindStarForOffset(ship.Start) == sourceStar))
                        AddOrderTargetChoice(combo, type == 2 ? ship.ObjectId | 0x80000000U : ship.ObjectId,
                            (ship.IsStation ? "Станция: " : "Корабль: ") + ship.Name,
                            selectedId, ref selectedIndex);
            if ((type == 3 || type == 7) && pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                    AddOrderTargetChoice(combo, star.ObjectId,
                        (appSettings.LanguageIndex == 1 ? "System: " : "Система: ") + LocalizedStarName(star),
                        selectedId, ref selectedIndex);
            if (type == 4 && pendingHoles != null)
                foreach (HoleRecord hole in pendingHoles)
                    if (sourceStar == null || hole.FromStarId == sourceStar.ObjectId ||
                        hole.ToStarId == sourceStar.ObjectId)
                        AddOrderTargetChoice(combo, hole.ObjectId, "Чёрная дыра",
                            selectedId, ref selectedIndex);
            if (selectedIndex < 0 && selectedId != 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            if (selectedIndex < 0 && combo.Items.Count != 0) selectedIndex = 0;
            combo.SelectedIndex = selectedIndex;
        }

        private static void AddOrderTargetChoice(ComboBox combo, uint objectId, string caption,
            uint selectedId, ref int selectedIndex)
        {
            combo.Items.Add(new UInt32ValueChoice(objectId, caption + " [ID " +
                objectId.ToString(CultureInfo.InvariantCulture) + "]"));
            if (objectId == selectedId) selectedIndex = combo.Items.Count - 1;
        }

        private void UpdateShipOrderDetails(byte type, uint targetId, ShipHeaderRecord source,
            ComboBox followType, TextBox orderData, TextBox destinationX, TextBox destinationY,
            TextBox angleEditor)
        {
            if (source == null) return;
            uint data = 0;
            float x = 0.0F, y = 0.0F;
            bool found = type == 0 || type == 1 || type == 2 || type == 7;
            if (type == 1) { x = source.X; y = source.Y; }
            if (type == 7) { x = source.X; y = source.Y; }
            float takeoffAngle = source.Angle;
            if (type == 5)
                found = TryCalculateTakeoffDestination(source, out x, out y, out takeoffAngle);
            if (type == 6)
            {
                ShipHeaderRecord ship = FindShipById(targetId);
                ByteValueChoice follow = followType.SelectedItem as ByteValueChoice;
                if (ship != null && follow != null)
                { data = follow.Value; x = ship.X; y = ship.Y; found = true; }
            }
            if (type == 3 && targetId != 0)
            {
                StarHeaderRecord from = FindStarForOffset(source.Start);
                StarHeaderRecord to = FindStarById(targetId);
                if (from != null && to != null)
                {
                    data = ShipOrderRules.JumpData(from, to);
                    found = TryCalculateJumpDestination(source, from, to, out x, out y);
                }
            }
            if (type == 4 && pendingHoles != null)
                foreach (HoleRecord hole in pendingHoles)
                    if (hole.ObjectId == targetId)
                    {
                        StarHeaderRecord owner = FindStarForOffset(source.Start);
                        ShipOrderRules.HoleDestination(hole,
                            owner == null ? hole.FromStarId : owner.ObjectId, out data, out x, out y);
                        found = true; break;
                    }
            if (!found) return;
            if (type == 7) data = 10;
            orderData.Text = data.ToString(CultureInfo.InvariantCulture);
            destinationX.Text = x.ToString("0.00", CultureInfo.InvariantCulture);
            destinationY.Text = y.ToString("0.00", CultureInfo.InvariantCulture);
            if (type == 5 && angleEditor != null)
                angleEditor.Text = takeoffAngle.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private StarHeaderRecord FindStarById(uint objectId)
        {
            if (pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                    if (star.ObjectId == objectId) return star;
            return null;
        }

        private PlanetHeaderRecord FindPlanetById(uint objectId)
        {
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (planet.ObjectId == objectId) return planet;
            return null;
        }

        private bool TryCalculateTakeoffDestination(ShipHeaderRecord source,
            out float x, out float y, out float angle)
        {
            x = 0.0F; y = 0.0F; angle = 0.0F;
            if (source == null) return false;
            uint turn = pendingGalaxySummary == null ? 0U : pendingGalaxySummary.Turn;
            PlanetHeaderRecord planet = FindPlanetById(source.CurrentPlanetId);
            if (planet != null)
            {
                float baseX, baseY;
                PlanetPosition(planet, out baseX, out baseY);
                int attempt = 0;
                double direction = 0.0;
                float candidateAngle = 0.0F;
                do
                {
                    uint seed = unchecked(source.Rnd * unchecked((uint)planet.Raw08) * turn *
                        unchecked((uint)(attempt + 1)));
                    direction = ShipOrderRules.DeterministicRandom(0, 360, seed);
                    double radians = direction * Math.PI / 180.0;
                    x = (float)Math.Round(Math.Sin(radians) * 400.0 + baseX);
                    y = (float)Math.Round(-Math.Cos(radians) * 400.0 + baseY);
                    uint angleSeed = unchecked(source.Rnd * unchecked((uint)planet.Raw08) * turn *
                        unchecked((uint)(attempt + 4)));
                    candidateAngle = ShipOrderRules.PlanetTakeoffAngle(turn, baseX, baseY, x, y,
                        ShipOrderRules.DeterministicRandom(-5, 5, angleSeed));
                    attempt++;
                }
                while (attempt < 6 && HasTakeoffAngleConflict(source, planet, candidateAngle));
                angle = candidateAngle;
                return true;
            }
            ShipHeaderRecord carrier = FindShipById(source.CurrentShipId);
            if (carrier != null)
            {
                PlanetHeaderRecord carrierPlanet = FindPlanetById(carrier.CurrentPlanetId);
                float baseX, baseY;
                if (carrierPlanet == null) { baseX = carrier.X; baseY = carrier.Y; }
                else PlanetPosition(carrierPlanet, out baseX, out baseY);
                uint combined = unchecked(source.Rnd + carrier.Rnd);
                double direction = ShipOrderRules.DeterministicRandom(0, 360,
                    unchecked(combined * turn));
                double radians = direction * Math.PI / 180.0;
                x = (float)Math.Round(Math.Sin(radians) * 400.0 + baseX);
                y = (float)Math.Round(-Math.Cos(radians) * 400.0 + baseY);
                angle = ShipOrderRules.CarrierTakeoffAngle(baseX, baseY, x, y,
                    ShipOrderRules.DeterministicRandom(-5, 5,
                        unchecked((combined + 234U) * turn * 4U)));
            }
            return true;
        }

        private bool HasTakeoffAngleConflict(ShipHeaderRecord source, PlanetHeaderRecord planet,
            float candidateAngle)
        {
            if (pendingShips == null) return false;
            StarHeaderRecord owner = FindStarForOffset(source.Start);
            foreach (ShipHeaderRecord other in pendingShips)
            {
                if (other == source || other.OrderType != 5 || other.CurrentPlanetId != planet.ObjectId ||
                    FindStarForOffset(other.Start) != owner) continue;
                if (ShipOrderRules.AngleDifference(other.Angle, candidateAngle) < 20.0F) return true;
            }
            return false;
        }

        private static void PlanetPosition(PlanetHeaderRecord planet, out float x, out float y)
        {
            double radians = planet.PolarAngle * Math.PI / 180.0;
            x = (float)(Math.Sin(radians) * planet.PolarRadius);
            y = (float)(-Math.Cos(radians) * planet.PolarRadius);
        }

        private bool TryCalculateJumpDestination(ShipHeaderRecord source, StarHeaderRecord from,
            StarHeaderRecord to, out float x, out float y)
        {
            x = 0.0F; y = 0.0F;
            if (source == null || from == null || to == null) return false;
            PlanetHeaderRecord lastPlanet = null;
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (FindStarForOffset(planet.Start) == from &&
                        (lastPlanet == null || planet.Start > lastPlanet.Start)) lastPlanet = planet;
            ShipOrderRules.JumpDestination(source, from, to, lastPlanet, out x, out y);
            return true;
        }

        private void ShowPlayerInfectionPlaces(ShipHeaderRecord ship, IWin32Window owner)
        {
            StringBuilder text = new StringBuilder();
            for (int index = 0; index < 24; index++)
            {
                string value = ship.PlayerInfectionPlaces != null &&
                    index < ship.PlayerInfectionPlaces.Length ? ship.PlayerInfectionPlaces[index] : string.Empty;
                text.Append("Болезнь ").Append((index + 1).ToString("00", CultureInfo.InvariantCulture))
                    .Append(" | ").AppendLine(value ?? string.Empty);
            }
            ShowTextReport(owner, "Места заражения", text.ToString());
        }

        private void ShowPlayerEquipmentSets(ShipHeaderRecord ship, IWin32Window owner)
        {
            StringBuilder text = new StringBuilder();
            string[] equipmentNames = { "Оружие 1", "Оружие 2", "Оружие 3", "Оружие 4", "Оружие 5" };
            byte[] equipmentTypes = { 44, 43, 46, 45, 47, 48, 49 };
            string[] typeNames = { "Двигатель", "Топливный бак", "Сканер", "Радар",
                "Ремонтный дроид", "Захват", "Защитный генератор" };
            int nonEmptySets = 0;
            for (int set = 0; set < 10; set++)
            {
                bool nonEmpty = false;
                for (int slot = 0; slot < 12; slot++) nonEmpty |= ship.PlayerEquipmentSetItems[set, slot] != 0;
                for (int slot = 0; slot < 32; slot++) nonEmpty |= ship.PlayerArtefactSetItems[set, slot] != 0;
                if (!nonEmpty) continue;
                nonEmptySets++;
                text.Append("Комплект ").Append(set.ToString(CultureInfo.InvariantCulture));
                if (set == ship.PlayerHotEquipmentCurrent) text.Append(" [текущий]");
                text.AppendLine();
                for (int slot = 0; slot < 5; slot++)
                    AppendEquipmentSetLine(text, equipmentNames[slot],
                        ship.PlayerEquipmentSetItems[set, slot]);
                for (int typeIndex = 0; typeIndex < equipmentTypes.Length; typeIndex++)
                {
                    uint foundId = 0;
                    for (int slot = 0; slot < 12; slot++)
                    {
                        uint itemId = ship.PlayerEquipmentSetItems[set, slot];
                        ItemHeaderRecord item = FindItemById(itemId);
                        if (item != null && item.Type == equipmentTypes[typeIndex])
                        { foundId = itemId; break; }
                    }
                    AppendEquipmentSetLine(text, typeNames[typeIndex], foundId);
                }
                for (int slot = 0; slot < 32; slot++)
                    AppendEquipmentSetLine(text, "Артефакт " + (slot + 1).ToString(CultureInfo.InvariantCulture),
                        ship.PlayerArtefactSetItems[set, slot]);
                text.AppendLine();
            }
            if (nonEmptySets == 0) text.AppendLine("Сохранённых комплектов нет.");
            ShowTextReport(owner, "Комплекты оборудования", text.ToString());
        }

        private void AppendEquipmentSetLine(StringBuilder text, string label, uint itemId)
        {
            if (itemId == 0) return;
            ItemHeaderRecord item = FindItemById(itemId);
            string caption = item == null || string.IsNullOrEmpty(item.Name)
                ? "TItem" : item.Name;
            text.Append("  ").Append(label.PadRight(22)).Append(" | ").Append(caption)
                .Append(" [ID ").Append(itemId.ToString(CultureInfo.InvariantCulture)).AppendLine("]");
        }

        private static void ShowTextReport(IWin32Window owner, string title, string content)
        {
            using (Form report = new Form())
            using (TextBox memo = new TextBox())
            {
                report.Text = title; report.ClientSize = new Size(1150, 700);
                report.StartPosition = FormStartPosition.CenterParent;
                memo.Dock = DockStyle.Fill; memo.Multiline = true; memo.ReadOnly = true;
                memo.ScrollBars = ScrollBars.Both; memo.WordWrap = false;
                memo.Font = new Font("Consolas", 10.0F, FontStyle.Regular, GraphicsUnit.Point);
                memo.Text = content ?? string.Empty;
                report.Controls.Add(memo); report.ShowDialog(owner);
            }
        }

        private static void SetUnsupportedEditorsReadOnly(Control root)
        {
            foreach (Control control in root.Controls)
            {
                TextBox text = control as TextBox;
                ComboBox combo = control as ComboBox;
                CheckBox check = control as CheckBox;
                TrackBar track = control as TrackBar;
                if (text != null) text.ReadOnly = true;
                if (combo != null) combo.Enabled = false;
                if (check != null) check.Enabled = false;
                if (track != null) track.Enabled = false;
                SetUnsupportedEditorsReadOnly(control);
            }
        }

        private void EditorFormClicked(object sender, EventArgs e)
        {
            Control control = sender as Control;
            string resource = control == null ? null : control.Tag as string;
            if (string.Equals(resource, "TACHIEVEMENTSFORM", StringComparison.OrdinalIgnoreCase))
            {
                EditAchievements();
                return;
            }
            EditorFormFactory.Show(resource, this);
        }

        private void EditAchievements()
        {
            if (pendingAchievements == null) return;
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TACHIEVEMENTSFORM")))
            {
                TextBox asteroids = FindControl<TextBox>(form, "edAsteroidsDestroyed");
                TextBox fried = FindControl<TextBox>(form, "edFriedShips");
                TextBox defended = FindControl<TextBox>(form, "edDefendedSystem");
                TextBox pirateSystems = FindControl<TextBox>(form, "edPirateSystems");
                TextBox science = FindControl<TextBox>(form, "edScienceProgress");
                TextBox programs = FindControl<TextBox>(form, "edProgramsUsed");
                TextBox pirates = FindControl<TextBox>(form, "edPiratesFreed");
                TextBox health = FindControl<TextBox>(form, "edHealthDrained");
                TextBox fuel = FindControl<TextBox>(form, "edFuelGottenFromSun");
                TextBox fuelTank = FindControl<TextBox>(form, "edFuelTankLastId");
                TextBox planets = FindControl<TextBox>(form, "edPlanetsVisited");
                TextBox received = FindControl<TextBox>(form, "mmAchAlreadyReceived");

                asteroids.Text = pendingAchievements.AsteroidsDestroyed.ToString(CultureInfo.InvariantCulture);
                fried.Text = pendingAchievements.FriedShips.ToString(CultureInfo.InvariantCulture);
                defended.Text = pendingAchievements.DefendedSystem.ToString(CultureInfo.InvariantCulture);
                pirateSystems.Text = pendingAchievements.PirateSystems.ToString(CultureInfo.InvariantCulture);
                science.Text = pendingAchievements.ScienceProgress.ToString(CultureInfo.InvariantCulture);
                programs.Text = pendingAchievements.ProgramsUsed.ToString(CultureInfo.InvariantCulture);
                pirates.Text = pendingAchievements.PiratesFreed.ToString(CultureInfo.InvariantCulture);
                health.Text = pendingAchievements.HealthDrained.ToString(CultureInfo.InvariantCulture);
                fuel.Text = pendingAchievements.FuelGottenFromSun.ToString(CultureInfo.InvariantCulture);
                fuelTank.Text = pendingAchievements.FuelTankLastId.ToString(CultureInfo.InvariantCulture);
                planets.Text = pendingAchievements.PlanetsVisited.ToString(CultureInfo.InvariantCulture);
                // Keep the serialized achievement keys untouched in the SAV, but
                // present their localized names to the user. Unknown/modded keys
                // remain visible verbatim instead of being lost.
                List<string> receivedDisplayNames = new List<string>();
                foreach (string achievementKey in pendingAchievements.Received)
                    receivedDisplayNames.Add(AchievementCatalog.DisplayName(achievementKey));
                received.Lines = receivedDisplayNames.ToArray();
                received.ReadOnly = true;

                form.ShowDialog(this);

                int parsedAsteroids = 0, parsedFried = 0, parsedDefended = 0, parsedPirateSystems = 0;
                int parsedScience = 0, parsedPrograms = 0, parsedPirates = 0, parsedHealth = 0;
                int parsedFuel = 0, parsedFuelTank = 0, parsedPlanets = 0;
                bool valid = int.TryParse(asteroids.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedAsteroids) &&
                    int.TryParse(fried.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedFried) &&
                    int.TryParse(defended.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedDefended) &&
                    int.TryParse(pirateSystems.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedPirateSystems) &&
                    int.TryParse(science.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedScience) &&
                    int.TryParse(programs.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedPrograms) &&
                    int.TryParse(pirates.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedPirates) &&
                    int.TryParse(health.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedHealth) &&
                    int.TryParse(fuel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedFuel) &&
                    int.TryParse(fuelTank.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedFuelTank) &&
                    int.TryParse(planets.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedPlanets) &&
                    parsedScience >= byte.MinValue && parsedScience <= byte.MaxValue;
                if (!valid)
                {
                    MessageBox.Show(this, "Поля достижений не применены: ожидаются целые Int32, научный прогресс — от 0 до 255.",
                        "Достижения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                pendingAchievements.AsteroidsDestroyed = parsedAsteroids;
                pendingAchievements.FriedShips = parsedFried;
                pendingAchievements.DefendedSystem = parsedDefended;
                pendingAchievements.PirateSystems = parsedPirateSystems;
                pendingAchievements.ScienceProgress = checked((byte)parsedScience);
                pendingAchievements.ProgramsUsed = parsedPrograms;
                pendingAchievements.PiratesFreed = parsedPirates;
                pendingAchievements.HealthDrained = parsedHealth;
                pendingAchievements.FuelGottenFromSun = parsedFuel;
                pendingAchievements.FuelTankLastId = parsedFuelTank;
                pendingAchievements.PlanetsVisited = parsedPlanets;
            }
        }

        private void PlayerClicked(object sender, EventArgs e)
        {
            if (current == null || pendingShips == null) return;
            foreach (ShipHeaderRecord ship in pendingShips)
                if (ship.IsPlayer && ship.ObjectId == current.GalaxySummary.PlayerObjectId)
                {
                    EditShip(ship);
                    return;
                }
            MessageBox.Show(this, "Корабль игрока не найден среди проверенных TShip.", "TShip",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void EditSelectedGalaxyObject(object sender, EventArgs e)
        {
            PlanetHeaderRecord planet = galaxyObjectList.SelectedItem as PlanetHeaderRecord;
            if (planet != null) { EditPlanet(planet); return; }
            ShipHeaderRecord ship = galaxyObjectList.SelectedItem as ShipHeaderRecord;
            if (ship != null) { EditShip(ship); return; }
            ItemHeaderRecord item = galaxyObjectList.SelectedItem as ItemHeaderRecord;
            if (item != null) { EditItem(item); return; }
            HoleRecord hole = galaxyObjectList.SelectedItem as HoleRecord;
            if (hole != null) { EditHole(hole); return; }
            AsteroidRecord asteroid = galaxyObjectList.SelectedItem as AsteroidRecord;
            if (asteroid != null) { EditAsteroid(asteroid); return; }
            MissileRecord missile = galaxyObjectList.SelectedItem as MissileRecord;
            if (missile != null) EditMissile(missile);
        }

        private void DeleteSelectedGalaxyObjects(object sender, EventArgs e)
        {
            if (galaxyObjectList == null || galaxyObjectList.SelectedItems.Count == 0) return;
            HashSet<uint> asteroidIds = new HashSet<uint>();
            HashSet<uint> missileIds = new HashSet<uint>();
            HashSet<int> itemStarts = new HashSet<int>();
            HashSet<int> shipStarts = new HashSet<int>();
            List<HoleRecord> holes = new List<HoleRecord>();
            foreach (object value in galaxyObjectList.SelectedItems)
            {
                ItemHeaderRecord item = value as ItemHeaderRecord;
                ShipHeaderRecord ship = value as ShipHeaderRecord;
                HoleRecord hole = value as HoleRecord;
                AsteroidRecord asteroid = value as AsteroidRecord;
                MissileRecord missile = value as MissileRecord;
                if (item != null) itemStarts.Add(item.Start);
                else if (ship != null) shipStarts.Add(ship.Start);
                else if (hole != null) holes.Add(hole);
                else if (asteroid != null) asteroidIds.Add(asteroid.ObjectId);
                else if (missile != null) missileIds.Add(missile.ObjectId);
            }
            List<string> blockers = FindObjectDeletionBlockers(asteroidIds, missileIds);
            if (blockers.Count != 0)
            {
                MessageBox.Show(this, "Удаление отменено: объект ещё используется.\r\n" +
                    string.Join("\r\n", blockers.ToArray()), "Удаление объектов",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (itemStarts.Count != 0)
            {
                try
                {
                    if (shipStarts.Count != 0)
                        current.DeleteGalaxyShipsCascade(shipStarts, pendingStars, pendingPlanets,
                            pendingShips, pendingItems, pendingMissiles, pendingGalaxySummary);
                    current.DeleteGalaxyItemsCascade(itemStarts, pendingStars, pendingShips,
                        pendingItems, pendingMissiles);
                    foreach (int start in itemStarts) pendingDeletedItemStarts.Add(start);
                }
                catch (Exception error)
                {
                    MessageBox.Show(this, "Удаление TItem отменено: " + error.Message,
                        "Удаление объектов", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (shipStarts.Count != 0)
            {
                try
                {
                    current.DeleteGalaxyShipsCascade(shipStarts, pendingStars, pendingPlanets,
                        pendingShips, pendingItems, pendingMissiles, pendingGalaxySummary);
                }
                catch (Exception error)
                {
                    MessageBox.Show(this, "Удаление TShip отменено: " + error.Message,
                        "Удаление объектов", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            foreach (HoleRecord hole in holes) pendingHoles.Remove(hole);
            pendingAsteroids.RemoveAll(delegate(AsteroidRecord value)
                { return asteroidIds.Contains(value.ObjectId); });
            pendingMissiles.RemoveAll(delegate(MissileRecord value)
                { return missileIds.Contains(value.ObjectId); });
            int removed = holes.Count + asteroidIds.Count + missileIds.Count +
                itemStarts.Count + shipStarts.Count;
            int unsupported = galaxyObjectList.SelectedItems.Count - removed;
            RefreshGalaxyObjects();
            if (systemMapForm != null && !systemMapForm.IsDisposed) systemMapForm.Invalidate();
            if (unsupported > 0)
                MessageBox.Show(this, "Удалены только объекты с полностью размеченными списками: " + removed +
                    ". Планеты и защищённые/неразмеченные корабли не удалялись.",
                    "Удаление объектов", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<string> FindObjectDeletionBlockers(HashSet<uint> asteroidIds,
            HashSet<uint> missileIds)
        {
            List<string> result = new List<string>();
            if (pendingMissiles != null)
                foreach (MissileRecord value in pendingMissiles)
                {
                    if (missileIds.Contains(value.ObjectId)) continue;
                    if (value.TargetType == 3 && asteroidIds.Contains(value.TargetId) ||
                        value.TargetLostType == 3 && asteroidIds.Contains(value.TargetLostId) ||
                        value.TargetType == 4 && missileIds.Contains(value.TargetId) ||
                        value.TargetLostType == 4 && missileIds.Contains(value.TargetLostId))
                        result.Add("Ракета " + value.ObjectId + " ссылается на выбранную цель.");
                }
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (asteroidIds.Contains(ship.OrderObjectId) || missileIds.Contains(ship.OrderObjectId))
                        result.Add("Приказ корабля " + ship.ObjectId + " ссылается на выбранную цель.");
            if (pendingItems != null)
                foreach (ItemHeaderRecord item in pendingItems)
                {
                    if (item.DerivedFields == null) continue;
                    byte targetType = 0; uint targetId = 0;
                    foreach (ItemDerivedField field in item.DerivedFields)
                    {
                        if (field.ControlName == "edWeaponTargetType") targetType = checked((byte)field.IntegerValue);
                        else if (field.ControlName == "cbWeaponTarget") targetId = checked((uint)field.IntegerValue);
                    }
                    if (targetType == 3 && asteroidIds.Contains(targetId) ||
                        targetType == 4 && missileIds.Contains(targetId))
                        result.Add("Оружие " + item.ObjectId + " ссылается на выбранную цель.");
                }
            if (result.Count > 8)
            {
                int extra = result.Count - 8;
                result.RemoveRange(8, extra);
                result.Add("…и ещё ссылок: " + extra + ".");
            }
            return result;
        }

        private void EditSelectedSearchResult(object sender, EventArgs e)
        {
            SearchResultEntry entry = searchResults == null ? null : searchResults.SelectedItem as SearchResultEntry;
            if (entry == null) return;
            StarHeaderRecord star = entry.Value as StarHeaderRecord;
            if (star != null) { EditStar(star); return; }
            PlanetHeaderRecord planet = entry.Value as PlanetHeaderRecord;
            if (planet != null) { EditPlanet(planet); return; }
            ShipHeaderRecord ship = entry.Value as ShipHeaderRecord;
            if (ship != null) { EditShip(ship); return; }
            ItemHeaderRecord item = entry.Value as ItemHeaderRecord;
            if (item != null) { EditItem(item); return; }
            HoleRecord hole = entry.Value as HoleRecord;
            if (hole != null) { EditHole(hole); return; }
            AsteroidRecord asteroid = entry.Value as AsteroidRecord;
            if (asteroid != null) { EditAsteroid(asteroid); return; }
            MissileRecord missile = entry.Value as MissileRecord;
            if (missile != null) { EditMissile(missile); return; }
            PlayerMessageRecord message = entry.Value as PlayerMessageRecord;
            if (message != null && pendingMessages != null)
            {
                int index = pendingMessages.IndexOf(message);
                if (index >= 0) { messageList.SelectedIndex = index; EditSelectedMessage(sender, e); }
            }
        }

        private ShipHeaderRecord FindPlayerShip()
        {
            if (current == null || pendingShips == null) return null;
            foreach (ShipHeaderRecord ship in pendingShips)
                if (ship.IsPlayer && ship.ObjectId == current.GalaxySummary.PlayerObjectId) return ship;
            return null;
        }

        private void EditSelectedStorageItem(object sender, EventArgs e)
        {
            SearchResultEntry selectedEntry = itemList == null ? null :
                itemList.SelectedItem as SearchResultEntry;
            PlayerStorageItemRecord record = selectedEntry == null ? null :
                selectedEntry.Value as PlayerStorageItemRecord;
            if (record == null) return;
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TSTORAGEITEMFORM")))
            {
                form.Text = "Предмет на складе";
                TextBox slot = FindControl<TextBox>(form, "edSlot");
                ComboBox place = FindControl<ComboBox>(form, "cbItemPlace");
                Button editItem = FindControl<Button>(form, "btnItemEdit");
                slot.Text = record.Slot.ToString(CultureInfo.InvariantCulture);
                place.Items.Clear();
                if (pendingPlanets != null)
                    foreach (PlanetHeaderRecord planet in pendingPlanets) place.Items.Add(planet);
                if (pendingShips != null)
                    foreach (ShipHeaderRecord ship in pendingShips)
                        if (ship.IsStation) place.Items.Add(ship);
                for (int index = 0; index < place.Items.Count; index++)
                {
                    PlanetHeaderRecord planet = place.Items[index] as PlanetHeaderRecord;
                    ShipHeaderRecord station = place.Items[index] as ShipHeaderRecord;
                    if (!record.IsStation && planet != null && planet.ObjectId == record.PlaceObjectId ||
                        record.IsStation && station != null && station.ObjectId == record.PlaceObjectId)
                    {
                        place.SelectedIndex = index;
                        break;
                    }
                }
                ItemHeaderRecord nested = FindItemByStart(record.ItemStart);
                editItem.Enabled = nested != null;
                if (nested != null)
                {
                    editItem.Text = ItemDisplayName(nested) + " — ID " +
                        nested.ObjectId.ToString(CultureInfo.InvariantCulture);
                    editItem.Click += delegate { EditItem(nested, form); };
                }
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                form.ShowDialog(this);

                int parsedSlot;
                if (!int.TryParse(slot.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out parsedSlot))
                {
                    MessageBox.Show(this, "Слот должен быть 32-битным целым числом.",
                        "TStorageItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                PlanetHeaderRecord selectedPlanet = place.SelectedItem as PlanetHeaderRecord;
                ShipHeaderRecord selectedStation = place.SelectedItem as ShipHeaderRecord;
                if (selectedPlanet == null && selectedStation == null)
                {
                    MessageBox.Show(this, "Выберите планету или станцию хранения.",
                        "TStorageItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Slot = parsedSlot;
                record.IsStation = selectedStation != null;
                record.PlaceObjectId = selectedStation == null ?
                    selectedPlanet.ObjectId : selectedStation.ObjectId;
            }
            RefreshObjectLists();
        }

        private void DeleteSelectedStorageItems(object sender, EventArgs e)
        {
            ShipHeaderRecord player = FindPlayerShip();
            if (player == null || itemList == null || itemList.SelectedItems.Count == 0) return;
            List<PlayerStorageItemRecord> selected = new List<PlayerStorageItemRecord>();
            foreach (object value in itemList.SelectedItems)
            {
                SearchResultEntry entry = value as SearchResultEntry;
                PlayerStorageItemRecord record = entry == null ? null :
                    entry.Value as PlayerStorageItemRecord;
                if (record != null) selected.Add(record);
            }
            foreach (PlayerStorageItemRecord record in selected)
                if (player.PlayerStorageItems.Remove(record))
                    pendingDeletedItemStarts.Add(record.ItemStart);
            player.PlayerObjectStateCount = player.PlayerStorageItems.Count;
            RefreshObjectLists();
        }

        private void ViewSelectedScript(object sender, EventArgs e)
        {
            ScriptRecord script = scriptList == null ? null : scriptList.SelectedItem as ScriptRecord;
            if (script == null) return;
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSCRIPTFORM")))
            {
                form.Text = "Скрипт — " + script.Name;
                ListBox init = FindControl<ListBox>(form, "lbInitVars");
                ListBox turn = FindControl<ListBox>(form, "lbTurnVars");
                ListBox items = FindControl<ListBox>(form, "lbItems");
                ListBox ships = FindControl<ListBox>(form, "lbShips");
                ListBox ethers = FindControl<ListBox>(form, "lbEthers");

                Action refresh = delegate
                {
                    PopulateList(init, script.InitVariables);
                    PopulateList(turn, script.TurnVariables);
                    PopulateList(items, script.ItemBindings);
                    PopulateList(ships, script.ShipBindings);
                    ethers.BeginUpdate(); ethers.Items.Clear();
                    foreach (string ether in script.EtherStrings) ethers.Items.Add(ether);
                    ethers.EndUpdate();
                };

                init.DoubleClick += delegate
                {
                    ScriptVariableRecord value = init.SelectedItem as ScriptVariableRecord;
                    if (value != null) { EditScriptVariable(value, form); refresh(); }
                };
                turn.DoubleClick += delegate
                {
                    ScriptVariableRecord value = turn.SelectedItem as ScriptVariableRecord;
                    if (value != null) { EditScriptVariable(value, form); refresh(); }
                };
                items.DoubleClick += delegate
                {
                    ScriptItemRecord value = items.SelectedItem as ScriptItemRecord;
                    if (value != null) { EditScriptItem(value, form); refresh(); }
                };
                ships.DoubleClick += delegate
                {
                    ScriptShipRecord value = ships.SelectedItem as ScriptShipRecord;
                    if (value != null) { EditScriptShip(value, form); refresh(); }
                };
                init.ContextMenuStrip = BuildScriptListMenu(init,
                    delegate(object value) { EditScriptVariable((ScriptVariableRecord)value, form); },
                    delegate(object value) { script.InitVariables.Remove((ScriptVariableRecord)value); }, refresh);
                turn.ContextMenuStrip = BuildScriptListMenu(turn,
                    delegate(object value) { EditScriptVariable((ScriptVariableRecord)value, form); },
                    delegate(object value) { script.TurnVariables.Remove((ScriptVariableRecord)value); }, refresh);
                items.ContextMenuStrip = BuildScriptListMenu(items,
                    delegate(object value) { EditScriptItem((ScriptItemRecord)value, form); },
                    delegate(object value) { script.ItemBindings.Remove((ScriptItemRecord)value); }, refresh);
                ships.ContextMenuStrip = BuildScriptListMenu(ships,
                    delegate(object value) { EditScriptShip((ScriptShipRecord)value, form); },
                    delegate(object value) { script.ShipBindings.Remove((ScriptShipRecord)value); }, refresh);
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                refresh();
                form.ShowDialog(this);
            }
            RefreshObjectLists();
        }

        private void DeleteSelectedScript(object sender, EventArgs e)
        {
            ScriptRecord script = scriptList == null ? null : scriptList.SelectedItem as ScriptRecord;
            if (script == null || pendingGalaxySummary == null) return;
            pendingGalaxySummary.ActiveScripts.Remove(script);
            RefreshObjectLists();
        }

        private void EditSelectedGlobalVariable(object sender, EventArgs e)
        {
            ScriptVariableRecord value = globalVariableList == null ? null :
                globalVariableList.SelectedItem as ScriptVariableRecord;
            if (value == null) return;
            EditScriptVariable(value, this);
            RefreshObjectLists();
        }

        private void DeleteSelectedGlobalVariable(object sender, EventArgs e)
        {
            ScriptVariableRecord value = globalVariableList == null ? null :
                globalVariableList.SelectedItem as ScriptVariableRecord;
            if (value == null || pendingGalaxySummary == null) return;
            pendingGalaxySummary.GlobalVariables.Remove(value);
            RefreshObjectLists();
        }

        private void EditSelectedScriptCache(object sender, EventArgs e)
        {
            ScriptCacheRecord cache = scriptCacheList == null ? null :
                scriptCacheList.SelectedItem as ScriptCacheRecord;
            if (cache == null) return;
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSCRIPTCACHEFORM")))
            {
                form.Text = "Кэш скрипта — " + (cache.Name ?? string.Empty);
                TextBox name = FindControl<TextBox>(form, "edName");
                TextBox countUse = FindControl<TextBox>(form, "edCntUse");
                TextBox lastTurn = FindControl<TextBox>(form, "edLastTurn");
                TextBox runScript = FindControl<TextBox>(form, "edRunScript");
                name.Text = cache.Name ?? string.Empty;
                countUse.Text = cache.CountUse.ToString(CultureInfo.InvariantCulture);
                lastTurn.Text = cache.LastTurn.ToString(CultureInfo.InvariantCulture);
                runScript.Text = cache.RunScript.ToString(CultureInfo.InvariantCulture);
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                form.ShowDialog(this);

                ushort parsedCount;
                int parsedLastTurn, parsedRunScript;
                cache.Name = name.Text ?? string.Empty;
                if (TryParseUInt16(countUse.Text, out parsedCount)) cache.CountUse = parsedCount;
                if (TryParseInt32(lastTurn.Text, out parsedLastTurn)) cache.LastTurn = parsedLastTurn;
                if (TryParseInt32(runScript.Text, out parsedRunScript)) cache.RunScript = parsedRunScript;
            }
            RefreshObjectLists();
        }

        private void DeleteSelectedScriptCache(object sender, EventArgs e)
        {
            ScriptCacheRecord cache = scriptCacheList == null ? null :
                scriptCacheList.SelectedItem as ScriptCacheRecord;
            if (cache == null || pendingGalaxySummary == null) return;
            pendingGalaxySummary.ScriptCache.Remove(cache);
            RefreshObjectLists();
        }

        private static void PopulateList<T>(ListBox list, IList<T> values)
        {
            list.BeginUpdate(); list.Items.Clear();
            if (values != null)
                foreach (T value in values) list.Items.Add(value);
            list.EndUpdate();
        }

        private ContextMenuStrip BuildScriptListMenu(ListBox list, Action<object> edit,
            Action<object> delete, Action refresh)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Редактировать", null, delegate
            {
                object selected = list.SelectedItem;
                if (selected != null) { edit(selected); refresh(); }
            });
            menu.Items.Add("Удалить", null, delegate
            {
                List<object> selected = new List<object>();
                foreach (object value in list.SelectedItems) selected.Add(value);
                if (selected.Count == 0 && list.SelectedItem != null) selected.Add(list.SelectedItem);
                foreach (object value in selected) delete(value);
                refresh();
            });
            return menu;
        }

        private void EditScriptVariable(ScriptVariableRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TVARFORM")))
            {
                form.Text = "Переменная";
                TextBox name = FindControl<TextBox>(form, "edVarName");
                ComboBox type = FindControl<ComboBox>(form, "cbVarType");
                TextBox scalar = FindControl<TextBox>(form, "edVarValue");
                ListBox array = FindControl<ListBox>(form, "lbArray");
                Button showArray = FindControl<Button>(form, "btnShowArray");
                Label valueLabel = FindControl<Label>(form, "lblVarValue");
                string[] names = { "Null", "Integer", "Dword", "Float", "String", "Object",
                    "dllLibrary", "Type 7", "Type 8", "Array", "Type 10" };
                type.Items.Clear(); type.Items.AddRange(names);
                type.SelectedIndex = value.Type <= 10 ? value.Type : -1;
                type.Enabled = false;
                name.Text = value.Name ?? string.Empty;
                scalar.Text = ScriptVariableScalarText(value);
                array.Visible = value.Type == 9;
                showArray.Visible = value.Type == 9;
                scalar.ReadOnly = value.Type == 0 || value.Type == 6 || value.Type == 9;
                scalar.Enabled = value.Type != 0 && value.Type != 6;
                if (value.Type == 9)
                {
                    valueLabel.Text = "Элементов:";
                    Action refreshArray = delegate
                    {
                        scalar.Text = value.ArrayValue.Count.ToString(CultureInfo.InvariantCulture);
                        PopulateList(array, value.ArrayValue);
                    };
                    array.DoubleClick += delegate
                    {
                        ScriptVariableRecord nested = array.SelectedItem as ScriptVariableRecord;
                        if (nested != null) { EditScriptVariable(nested, form); refreshArray(); }
                    };
                    showArray.Click += delegate { ShowScriptArray(value.ArrayValue, form); };
                    refreshArray();
                }
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                form.ShowDialog(owner);

                int integer = 0;
                double real = 0;
                if (string.IsNullOrWhiteSpace(name.Text))
                {
                    MessageBox.Show(form, "Имя переменной не может быть пустым.", "TVar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((value.Type == 1 || value.Type == 2) &&
                    !int.TryParse(scalar.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out integer))
                {
                    MessageBox.Show(form, "Значение переменной должно быть 32-битным целым числом.", "TVar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (value.Type == 3 &&
                    !double.TryParse(scalar.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out real))
                {
                    MessageBox.Show(form, "Значение Float введено неверно.", "TVar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                value.Name = name.Text;
                if (value.Type == 1 || value.Type == 2) value.IntegerValue = integer;
                else if (value.Type == 3) value.DoubleValue = real;
                else if (value.Type == 4) value.StringValue = scalar.Text;
            }
        }

        private static string ScriptVariableScalarText(ScriptVariableRecord value)
        {
            if (value.Type == 1 || value.Type == 2)
                return value.IntegerValue.ToString(CultureInfo.InvariantCulture);
            if (value.Type == 3) return value.DoubleValue.ToString("R", CultureInfo.InvariantCulture);
            if (value.Type == 4 || value.Type == 6) return value.StringValue ?? string.Empty;
            if (value.Type == 9) return value.ArrayValue.Count.ToString(CultureInfo.InvariantCulture);
            return string.Empty;
        }

        private void ShowScriptArray(IList<ScriptVariableRecord> values, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TVARARRAYVIEWFORM")))
            {
                form.Text = "Просмотр массива";
                ListView list = FindControl<ListView>(form, "lvArray");
                TextBox search = FindControl<TextBox>(form, "edSearch");
                Action refresh = delegate
                {
                    list.BeginUpdate(); list.Items.Clear();
                    AddScriptArrayRows(list, values, string.Empty, search.Text ?? string.Empty);
                    list.EndUpdate();
                };
                search.TextChanged += delegate { refresh(); };
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                refresh();
                form.ShowDialog(owner);
            }
        }

        private static void AddScriptArrayRows(ListView list, IList<ScriptVariableRecord> values,
            string parentPath, string search)
        {
            if (values == null) return;
            for (int index = 0; index < values.Count; index++)
            {
                ScriptVariableRecord value = values[index];
                string path = parentPath.Length == 0 ? "[" + index + "] " + value.Name :
                    parentPath + ".[" + index + "] " + value.Name;
                string scalar = ScriptVariableScalarText(value);
                string haystack = path + " " + value.TypeName + " " + scalar;
                if (string.IsNullOrEmpty(search) || haystack.IndexOf(search,
                    StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    ListViewItem row = new ListViewItem(path);
                    row.SubItems.Add(value.TypeName);
                    row.SubItems.Add(scalar);
                    list.Items.Add(row);
                }
                if (value.Type == 9) AddScriptArrayRows(list, value.ArrayValue, path, search);
            }
        }

        private void EditScriptItem(ScriptItemRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSCRIPTITEMFORM")))
            {
                form.Text = "Скриптовый предмет";
                TextBox name = FindControl<TextBox>(form, "edItemName");
                TextBox data1 = FindControl<TextBox>(form, "edData1");
                TextBox data2 = FindControl<TextBox>(form, "edData2");
                TextBox data3 = FindControl<TextBox>(form, "edData3");
                TextBox text1 = FindControl<TextBox>(form, "edTextData1");
                TextBox text2 = FindControl<TextBox>(form, "edTextData2");
                TextBox text3 = FindControl<TextBox>(form, "edTextData3");
                TextBox onUse = FindControl<TextBox>(form, "mmOnUseCode");
                TextBox onAct = FindControl<TextBox>(form, "mmOnActCode");
                CheckBox canSell = FindControl<CheckBox>(form, "chbCanSell");
                Button editItem = FindControl<Button>(form, "btnItemEdit");
                name.Text = value.Name ?? string.Empty;
                data1.Text = value.Data1.ToString(CultureInfo.InvariantCulture);
                data2.Text = value.Data2.ToString(CultureInfo.InvariantCulture);
                data3.Text = value.Data3.ToString(CultureInfo.InvariantCulture);
                text1.Text = value.TextData1 ?? string.Empty;
                text2.Text = value.TextData2 ?? string.Empty;
                text3.Text = value.TextData3 ?? string.Empty;
                onUse.Text = value.OnUseCode ?? string.Empty;
                onAct.Text = value.OnActCode ?? string.Empty;
                canSell.Checked = value.CanSell;
                ItemHeaderRecord item = FindItemById(value.ItemObjectId);
                editItem.Enabled = item != null;
                if (item != null) editItem.Click += delegate { EditItem(item); };
                form.ShowDialog(owner);
                int parsed1, parsed2, parsed3;
                if (!int.TryParse(data1.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed1) ||
                    !int.TryParse(data2.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed2) ||
                    !int.TryParse(data3.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed3))
                {
                    MessageBox.Show(form, "Данные предмета должны быть 32-битными целыми числами.",
                        "TScriptItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                value.Name = name.Text; value.CanSell = canSell.Checked;
                value.Data1 = parsed1; value.Data2 = parsed2; value.Data3 = parsed3;
                value.TextData1 = text1.Text; value.TextData2 = text2.Text; value.TextData3 = text3.Text;
                value.OnUseCode = onUse.Text; value.OnActCode = onAct.Text;
            }
        }

        private void EditScriptShip(ScriptShipRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSCRIPTSHIPFORM")))
            {
                form.Text = "Скриптовый корабль";
                TextBox name = FindControl<TextBox>(form, "edShipName");
                TextBox group = FindControl<TextBox>(form, "edGroup");
                TextBox data0 = FindControl<TextBox>(form, "edData0");
                TextBox data1 = FindControl<TextBox>(form, "edData1");
                TextBox data2 = FindControl<TextBox>(form, "edData2");
                TextBox data3 = FindControl<TextBox>(form, "edData3");
                TextBox state = FindControl<TextBox>(form, "edStateNum");
                TextBox faction = FindControl<TextBox>(form, "edCustomFaction");
                CheckBox hit = FindControl<CheckBox>(form, "chbHit");
                CheckBox hitPlayer = FindControl<CheckBox>(form, "chbHitPlayer");
                Button editShip = FindControl<Button>(form, "btnShipEdit");
                ShipHeaderRecord ship = FindShipById(value.ShipObjectId);
                name.Text = ship == null ? "ID " + value.ShipObjectId : ship.Name;
                group.Text = value.Group.ToString(CultureInfo.InvariantCulture);
                data0.Text = value.Data0.ToString(CultureInfo.InvariantCulture);
                data1.Text = value.Data1.ToString(CultureInfo.InvariantCulture);
                data2.Text = value.Data2.ToString(CultureInfo.InvariantCulture);
                data3.Text = value.Data3.ToString(CultureInfo.InvariantCulture);
                state.Text = value.StateNum.ToString(CultureInfo.InvariantCulture);
                faction.Text = value.CustomFaction ?? string.Empty;
                hit.Checked = value.Hit; hitPlayer.Checked = value.HitPlayer;
                editShip.Enabled = ship != null;
                if (ship != null) editShip.Click += delegate { EditShip(ship); name.Text = ship.Name; };
                form.ShowDialog(owner);
                int parsedGroup, parsedState;
                uint parsed0, parsed1, parsed2, parsed3;
                if (!int.TryParse(group.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedGroup) ||
                    !uint.TryParse(data0.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed0) ||
                    !uint.TryParse(data1.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed1) ||
                    !uint.TryParse(data2.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed2) ||
                    !uint.TryParse(data3.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed3) ||
                    !int.TryParse(state.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedState))
                {
                    MessageBox.Show(form, "Поля корабля содержат неверные 32-битные числа.",
                        "TScriptShip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                value.Group = parsedGroup; value.Data0 = parsed0; value.Data1 = parsed1;
                value.Data2 = parsed2; value.Data3 = parsed3; value.StateNum = parsedState;
                value.CustomFaction = faction.Text; value.Hit = hit.Checked; value.HitPlayer = hitPlayer.Checked;
            }
        }

        private ItemHeaderRecord FindItemById(uint objectId)
        {
            if (pendingItems != null)
                foreach (ItemHeaderRecord item in pendingItems)
                    if (!pendingDeletedItemStarts.Contains(item.Start) && item.ObjectId == objectId) return item;
            return null;
        }

        private ItemHeaderRecord FindItem(byte type, uint objectId)
        {
            if (pendingItems != null)
                foreach (ItemHeaderRecord item in pendingItems)
                    if (!pendingDeletedItemStarts.Contains(item.Start) &&
                        item.Type == type && item.ObjectId == objectId) return item;
            return null;
        }

        private ItemHeaderRecord FindItemByStart(int start)
        {
            if (pendingItems != null)
                foreach (ItemHeaderRecord item in pendingItems)
                    if (!pendingDeletedItemStarts.Contains(item.Start) && item.Start == start) return item;
            return null;
        }

        private ShipHeaderRecord FindShipById(uint objectId)
        {
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (ship.ObjectId == objectId) return ship;
            return null;
        }

        private ShipHeaderRecord FindShipByStart(int start)
        {
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (ship.Start == start) return ship;
            return null;
        }

        private void EditSelectedSatellite(object sender, EventArgs e)
        {
            SearchResultEntry selected = satelliteList == null ? null :
                satelliteList.SelectedItem as SearchResultEntry;
            ShipItemListEntry record = selected == null ? null : selected.Value as ShipItemListEntry;
            ItemHeaderRecord item = record == null ? null : FindItemByStart(record.ItemStart);
            if (item == null) return;
            EditItem(item);
            RefreshObjectLists();
        }

        private void DeleteSelectedSatellites(object sender, EventArgs e)
        {
            ShipHeaderRecord player = FindPlayerShip();
            if (satelliteList == null || player == null || player.PlayerSatelliteItems == null ||
                satelliteList.SelectedItems.Count == 0)
                return;
            HashSet<int> selectedStarts = new HashSet<int>();
            foreach (object item in satelliteList.SelectedItems)
            {
                SearchResultEntry entry = item as SearchResultEntry;
                ShipItemListEntry record = entry == null ? null : entry.Value as ShipItemListEntry;
                if (record != null) selectedStarts.Add(record.ItemStart);
            }
            for (int index = player.PlayerSatelliteItems.Count - 1; index >= 0; index--)
                if (selectedStarts.Contains(player.PlayerSatelliteItems[index].ItemStart))
                    player.PlayerSatelliteItems.RemoveAt(index);
            player.PlayerSatelliteCount = player.PlayerSatelliteItems.Count;
            if (pendingItems != null)
                for (int index = pendingItems.Count - 1; index >= 0; index--)
                    if (selectedStarts.Contains(pendingItems[index].Start))
                    {
                        pendingDeletedItemStarts.Add(pendingItems[index].Start);
                        pendingItems.RemoveAt(index);
                    }
            RefreshObjectLists();
        }

        private void EditSputnik(PlanetSputnikRecord record)
        {
            if (record == null) return;
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSPUTNIKFORM")))
            {
                ComboBox graph = FindControl<ComboBox>(form, "cbGraphName");
                TextBox angle = FindControl<TextBox>(form, "edAngleCur");
                graph.DropDownStyle = ComboBoxStyle.DropDown;
                graph.Items.Clear();
                foreach (string value in gameCatalog.SputnikGraphs)
                    if (!graph.Items.Contains(value)) graph.Items.Add(value);
                if (pendingPlanets != null)
                    foreach (PlanetHeaderRecord planet in pendingPlanets)
                        foreach (PlanetSputnikRecord satellite in planet.Satellites)
                            if (!string.IsNullOrEmpty(satellite.GraphName) &&
                                !graph.Items.Contains(satellite.GraphName))
                                graph.Items.Add(satellite.GraphName);
                graph.Text = record.GraphName ?? string.Empty;
                angle.Text = record.AngleCurrent.ToString("0.00", CultureInfo.CurrentCulture);
                FindControl<GroupBox>(form, "gbSputnik").Text = "Спутник | ID: " +
                    record.ObjectId.ToString(CultureInfo.InvariantCulture);
                form.Text = "Зонд";
                form.ShowDialog(this);

                float parsedAngle;
                string graphName = graph.Text ?? string.Empty;
                if (graphName.Length == 0 || graphName.Length > 32768 ||
                    !TryParseFiniteFloat(angle.Text, out parsedAngle))
                {
                    MessageBox.Show(this, "Поля TSputnik не применены: выберите графику и проверьте угол.",
                        "TSputnik", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.GraphName = graphName;
                record.AngleCurrent = parsedAngle;
            }
        }

        private string PlanetGoneItemCaption(PlanetGoneItemRecord record)
        {
            ItemHeaderRecord item = record == null ? null : FindItemByStart(record.ItemStart);
            return item == null ? MissingItemCaption(record == null ? (byte)0 : record.ItemType,
                record == null ? 0U : record.ItemObjectId) : ItemDisplayCaption(item, null);
        }

        private string PlanetShopItemCaption(ShipItemListEntry record)
        {
            ItemHeaderRecord item = record == null ? null : FindItemByStart(record.ItemStart);
            return item == null ? MissingItemCaption(record == null ? (byte)0 : record.ItemType,
                record == null ? 0U : record.ItemObjectId) : ItemDisplayCaption(item, null);
        }

        private string MissingItemCaption(byte type, uint objectId)
        {
            return ItemTypeName(type) + " | Вложенный предмет не найден · ID " +
                objectId.ToString(CultureInfo.InvariantCulture);
        }

        private string CommodityName(byte type)
        {
            string[] names = appSettings.LanguageIndex == 1
                ? new string[] { "Food", "Medicine", "Technics", "Luxury", "Minerals", "Alcohol", "Arms", "Narcotics" }
                : new string[] { "Еда", "Медикаменты", "Техника", "Роскошь", "Минералы", "Алкоголь", "Оружие", "Наркотики" };
            return type < names.Length ? names[type] : "Товар";
        }

        private string WeaponTypeName(byte type)
        {
            string[] names = appSettings.LanguageIndex == 1
                ? new string[] { "Industrial Laser", "Fragmentation Cannon", "Flux", "Missile Launcher",
                    "Treton", "Wave Phaser", "Flow Blaster", "Electron Cutter", "Multiresonator",
                    "Atomic Vision", "Disintegrator", "Turbogravitron", "IMHO-9000", "Vertix",
                    "Torpedo Tube", "Esodapher", "Caphasitor", "Lirecron" }
                : new string[] { "Промышленный лазер", "Осколочная пушка",
                    "Флюктуационный излучатель", "Ракетница", "Третон", "Волновой фазер",
                    "Потоковый бластер", "Электронный резак", "Мультирезонатор", "Атомное зрение",
                    "Дезинтегратор", "Турбогравир", "ИМХО-9000", "Вертикс",
                    "Торпедный аппарат", "Эсодафер", "Кафаситор", "Лирекрон" };
            return type >= 50 && type <= 67 ? names[type - 50] : "Оружие";
        }

        private static bool IsTechnicalItemName(string value)
        {
            string text = (value ?? string.Empty).Trim();
            if (text.Length == 0) return true;
            if (!text.StartsWith("TItem", StringComparison.OrdinalIgnoreCase)) return false;
            if (text.Length == 5) return true;
            for (int index = 5; index < text.Length; index++)
                if (!char.IsWhiteSpace(text[index]) && !char.IsDigit(text[index])) return false;
            return true;
        }

        private static ItemDerivedField FindDerivedField(ItemHeaderRecord item, string name)
        {
            if (item == null || item.DerivedFields == null) return null;
            foreach (ItemDerivedField field in item.DerivedFields)
                if (string.Equals(field.ControlName, name, StringComparison.OrdinalIgnoreCase)) return field;
            return null;
        }

        private string ItemDisplayName(ItemHeaderRecord item)
        {
            if (item == null) return "Предмет";
            if (item.Type == 68 && !string.IsNullOrWhiteSpace(item.CustomWeaponName))
                return item.CustomWeaponName.Trim();
            if (item.Type == 71)
            {
                MicroModuleCatalogEntry micro = gameCatalog == null ? null :
                    gameCatalog.FindMicroModule(item.Bonus, item.BonusReferenceId);
                if (micro != null)
                {
                    string microName = string.IsNullOrWhiteSpace(micro.Name) ?
                        micro.BlockName : micro.Name;
                    if (!string.IsNullOrWhiteSpace(microName)) return microName.Trim();
                }
            }
            if (!IsTechnicalItemName(item.Name)) return item.Name.Trim();
            if (!IsTechnicalItemName(item.SystemName)) return item.SystemName.Trim();
            if (item.Type <= 7) return CommodityName(item.Type);
            if (item.Type >= 50 && item.Type <= 67) return WeaponTypeName(item.Type);
            return ItemTypeName(item.Type);
        }

        private string ItemDisplayCaption(ItemHeaderRecord item, string context)
        {
            if (item == null) return "Предмет не найден";
            string first = ItemDisplayName(item);
            if (!string.IsNullOrEmpty(context)) first = context + " — " + first;
            List<string> details = new List<string>();
            string techField = item.Type == 42 ? "edHullTechLevel" :
                item.Type == 43 ? "edFuelTanksTechLevel" :
                item.Type == 44 ? "edEngineTechLevel" :
                item.Type == 45 ? "edRadarTechLevel" :
                item.Type == 46 ? "edScanerTechLevel" :
                item.Type == 47 ? "edRepairRobotTechLevel" :
                item.Type == 48 ? "edCargoHookTechLevel" :
                item.Type == 49 ? "edDefGeneratorTechLevel" :
                item.Type >= 50 && item.Type <= 68 ? "edWeaponTechLevel" : null;
            ItemDerivedField tech = techField == null ? null : FindDerivedField(item, techField);
            if (tech != null) details.Add("ТУ " + tech.IntegerValue.ToString(CultureInfo.InvariantCulture));
            AddItemDetail(details, item, "edArmor", "броня");
            AddItemDetail(details, item, "edHitPoints", "корпус");
            AddItemDetail(details, item, "edCapacity", "ёмкость");
            AddItemDetail(details, item, "edFuel", "топливо");
            AddItemDetail(details, item, "edSpeed", "скорость");
            AddItemDetail(details, item, "edJump", "прыжок");
            AddItemDetail(details, item, "edEnginePower", "мощность");
            AddItemDetail(details, item, "edRadius", "радиус");
            AddItemDetail(details, item, "edScanProtect", "защита сканера");
            AddItemDetail(details, item, "edRecoverHitPoints", "ремонт");
            AddItemDetail(details, item, "edPickUpSize", "размер захвата");
            AddItemDetail(details, item, "edHookRadius", "радиус захвата");
            AddItemDetail(details, item, "edDefPower", "защита");
            ItemDerivedField minimumDamage = FindDerivedField(item, "edMinDamage");
            ItemDerivedField maximumDamage = FindDerivedField(item, "edMaxDamage");
            if (minimumDamage != null && maximumDamage != null)
                details.Add("урон " + minimumDamage.IntegerValue.ToString(CultureInfo.InvariantCulture) +
                    "–" + maximumDamage.IntegerValue.ToString(CultureInfo.InvariantCulture));
            AddItemDetail(details, item, "edWeaponRadius", "дальность");
            ItemDerivedField ammunition = FindDerivedField(item, "edAmmunition");
            ItemDerivedField maximumAmmunition = FindDerivedField(item, "edMaxAmmunition");
            if (ammunition != null && maximumAmmunition != null)
                details.Add("боезапас " + ammunition.IntegerValue.ToString(CultureInfo.InvariantCulture) +
                    "/" + maximumAmmunition.IntegerValue.ToString(CultureInfo.InvariantCulture));
            AddItemDetail(details, item, "edCountableItemCount", "количество");
            AddItemDetail(details, item, "edCisternCapacity", "ёмкость");
            AddItemDetail(details, item, "edCisternFuel", "топливо");
            AddItemDetail(details, item, "edWear", "износ");
            if (item.Type <= 7 && item.HasGoodsTail)
                details.Add("количество " + item.GoodsItemCount.ToString(CultureInfo.InvariantCulture));
            if (item.Weight != 0) details.Add("вес " + item.Weight.ToString(CultureInfo.InvariantCulture));
            if (item.Cost != 0) details.Add("цена " + item.Cost.ToString(CultureInfo.InvariantCulture));
            if (item.Strength != 0) details.Add("прочность " + item.Strength.ToString("0.##", CultureInfo.InvariantCulture));
            if (item.Broken != 0) details.Add("сломан");
            if (item.Bonus != 0) details.Add("бонус " + item.Bonus.ToString(CultureInfo.InvariantCulture));
            details.Add("ID " + item.ObjectId.ToString(CultureInfo.InvariantCulture));
            return first + " | " + string.Join(" · ", details.ToArray());
        }

        private static void AddItemDetail(List<string> details, ItemHeaderRecord item,
            string controlName, string caption)
        {
            ItemDerivedField field = FindDerivedField(item, controlName);
            if (field == null) return;
            string value = field.Kind == ItemDerivedField.Float32 ?
                field.FloatValue.ToString("0.##", CultureInfo.InvariantCulture) :
                field.IntegerValue.ToString(CultureInfo.InvariantCulture);
            details.Add(caption + " " + value);
        }

        private string PlanetWarriorCaption(PlanetWarriorRecord record)
        {
            ShipHeaderRecord ship = record == null ? null : FindShipByStart(record.ShipStart);
            string caption = ship == null || string.IsNullOrEmpty(ship.Name) ? "TShip" : ship.Name;
            if (record != null)
                caption += " [тип " + record.ShipType.ToString(CultureInfo.InvariantCulture) +
                    ", ID " + record.ShipObjectId.ToString(CultureInfo.InvariantCulture) + "]";
            return caption;
        }

        private void EditPlanetGoneItem(PlanetGoneItemRecord record, IWin32Window owner)
        {
            if (record == null) return;
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TPLANETGONEITEMFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox posX = FindControl<TextBox>(form, "edPosX");
                TextBox posY = FindControl<TextBox>(form, "edPosY");
                TextBox landType = FindControl<TextBox>(form, "edLandType");
                TextBox region = FindControl<TextBox>(form, "edRegion");
                CheckBox miss = FindControl<CheckBox>(form, "chbMiss");
                Button itemButton = FindControl<Button>(form, "btnItemEdit");
                GroupBox group = FindControl<GroupBox>(form, "gbGoneItem");
                foreach (TextBox editor in new TextBox[] { posX, posY, landType, region })
                    editor.ReadOnly = false;
                miss.Enabled = true;
                posX.Text = record.PosX.ToString(CultureInfo.InvariantCulture);
                posY.Text = record.PosY.ToString(CultureInfo.InvariantCulture);
                landType.Text = record.LandType.ToString(CultureInfo.InvariantCulture);
                region.Text = record.Region.ToString(CultureInfo.InvariantCulture);
                miss.Checked = record.Miss;
                group.Text = "Закопанный предмет | " + PlanetGoneItemCaption(record);
                itemButton.Enabled = FindItemByStart(record.ItemStart) != null;
                itemButton.Click += delegate
                {
                    ItemHeaderRecord item = FindItemByStart(record.ItemStart);
                    if (item == null) return;
                    EditItem(item, form);
                    group.Text = "Закопанный предмет | " + PlanetGoneItemCaption(record);
                };
                form.Text = "Закопанный предмет";
                form.ShowDialog(owner ?? this);

                int parsed;
                if (TryParseInt32(posX.Text, out parsed)) record.PosX = (byte)Math.Max(0, Math.Min(255, parsed));
                if (TryParseInt32(posY.Text, out parsed)) record.PosY = (byte)Math.Max(0, Math.Min(255, parsed));
                if (TryParseInt32(landType.Text, out parsed))
                    record.LandType = (byte)Math.Max(0, Math.Min(255, parsed));
                if (TryParseInt32(region.Text, out parsed)) record.Region = parsed;
                record.Miss = miss.Checked;
            }
        }

        private List<ShipHeaderRecord> PlanetRelationRangers()
        {
            List<ShipHeaderRecord> result = new List<ShipHeaderRecord>();
            if (pendingShips == null) return result;
            if (pendingGalaxySummary != null && pendingGalaxySummary.RangerObjectIds != null)
            {
                foreach (uint objectId in pendingGalaxySummary.RangerObjectIds)
                    foreach (ShipHeaderRecord ship in pendingShips)
                        if (ship.ObjectId == objectId)
                        {
                            result.Add(ship);
                            break;
                        }
                return result;
            }
            foreach (ShipHeaderRecord ship in pendingShips)
                if (ship.Type == 1) result.Add(ship);
            result.Sort(delegate(ShipHeaderRecord left, ShipHeaderRecord right)
                { return left.ObjectId.CompareTo(right.ObjectId); });
            return result;
        }

        private static string PlanetRelationRangerName(IList<ShipHeaderRecord> rangers, int index)
        {
            if (rangers != null && index >= 0 && index < rangers.Count)
            {
                ShipHeaderRecord ranger = rangers[index];
                if (!string.IsNullOrEmpty(ranger.Name)) return ranger.Name;
                return "TShip " + ranger.ObjectId.ToString(CultureInfo.InvariantCulture);
            }
            return "Рейнджер #" + (index + 1).ToString(CultureInfo.InvariantCulture);
        }

        private bool EditPlanetRangerRelation(PlanetHeaderRecord planet, int index,
            IList<ShipHeaderRecord> rangers, IWin32Window owner)
        {
            if (planet == null || planet.RelationToRangers == null || index < 0 ||
                index >= planet.RelationToRangers.Length) return false;
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TRELATIONFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox relation = FindControl<TextBox>(form, "edRelation");
                GroupBox group = FindControl<GroupBox>(form, "gbRelation");
                relation.ReadOnly = false;
                relation.Text = planet.RelationToRangers[index].ToString(CultureInfo.InvariantCulture);
                string rangerName = PlanetRelationRangerName(rangers, index);
                group.Text = "Отношение | " + rangerName;
                form.Text = "Отношение";
                form.ShowDialog(owner ?? this);

                int parsed;
                if (!TryParseInt32(relation.Text, out parsed) || parsed < 0 || parsed > 100)
                    return false;
                planet.RelationToRangers[index] = (byte)parsed;
                return true;
            }
        }

        private void EditPlanet(PlanetHeaderRecord planet)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TPLANETFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox name = FindControl<TextBox>(form, "edPlanetName");
                TextBox polarAngle = FindControl<TextBox>(form, "edPolarPosAngle");
                TextBox polarRadius = FindControl<TextBox>(form, "edPolarPosRadius");
                TextBox people = FindControl<TextBox>(form, "edPeopleCnt");
                TextBox money = FindControl<TextBox>(form, "edMoney");
                TextBox rnd = FindControl<TextBox>(form, "edRnd");
                TextBox rndOut = FindControl<TextBox>(form, "edRndOut");
                TextBox angle = FindControl<TextBox>(form, "edAngleSpeed");
                TextBox radius = FindControl<TextBox>(form, "edRadius");
                TextBox waterSpace = FindControl<TextBox>(form, "edWaterSpace");
                TextBox waterSpaceDone = FindControl<TextBox>(form, "edWaterSpaceDone");
                TextBox landSpace = FindControl<TextBox>(form, "edLandSpace");
                TextBox landSpaceDone = FindControl<TextBox>(form, "edLandSpaceDone");
                TextBox hillSpace = FindControl<TextBox>(form, "edHillSpace");
                TextBox hillSpaceDone = FindControl<TextBox>(form, "edHillSpaceDone");
                TextBox orbitCount = FindControl<TextBox>(form, "edOrbitCnt");
                TextBox openPoints = FindControl<TextBox>(form, "edOpenPointsInvention");
                TextBox necessaryPercent = FindControl<TextBox>(form, "edNecessaryPercent");
                TextBox necessaryPercentK = FindControl<TextBox>(form, "edNecessaryPercentK");
                TextBox customFaction = FindControl<TextBox>(form, "edCustomFaction");
                TextBox noShopUpdate = FindControl<TextBox>(form, "edNoPlanetShopUpdate");
                TextBox questNumber = FindControl<TextBox>(form, "edQuestNumber");
                TextBox rangerCount = FindControl<TextBox>(form, "edRangerCnt");
                TextBox transportCount = FindControl<TextBox>(form, "edTransportCnt");
                TextBox graphRadius = FindControl<TextBox>(form, "edGraphRadius");
                TextBox graphSpeed = FindControl<TextBox>(form, "edGraphSpeedRotate");
                TextBox graphStep = FindControl<TextBox>(form, "edGraphStepRotate");
                TextBox graphRing = FindControl<TextBox>(form, "edGraphRing");
                CheckBox visited = FindControl<CheckBox>(form, "chbVisitedByPlayer");
                CheckBox noLanding = FindControl<CheckBox>(form, "chbNoLanding");
                CheckBox noBuyShips = FindControl<CheckBox>(form, "chbNoBuyShips");
                CheckBox noRandomEvents = FindControl<CheckBox>(form, "chbNoRandomEvents");
                CheckBox isRogeria = FindControl<CheckBox>(form, "chbIsMainPiratePlanet");
                ComboBox economy = FindControl<ComboBox>(form, "cbEconomy");
                ComboBox government = FindControl<ComboBox>(form, "cbGoverment");
                ComboBox owner = FindControl<ComboBox>(form, "cbOwner");
                ComboBox race = FindControl<ComboBox>(form, "cbRace");
                ComboBox currentInvention = FindControl<ComboBox>(form, "cbCurrentInvention");
                ComboBox graphName = FindControl<ComboBox>(form, "cbGraphName");
                TextBox[] openInventions = new TextBox[20];
                for (int index = 0; index < openInventions.Length; index++)
                    openInventions[index] = FindControl<TextBox>(form, "edOpenInvention" + (index + 1));
                TextBox[,] shopGoods = new TextBox[8, 5];
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 5; field++)
                    {
                        shopGoods[good, field] = FindControl<TextBox>(form,
                            "edShopGoods" + (good + 1) + (field + 1));
                        shopGoods[good, field].ReadOnly = false;
                        uint value = field < 3 ? planet.ShopGoods[good, field] :
                            field == 3 ? planet.ShopDeficit[good] : planet.ShopSale[good];
                        shopGoods[good, field].Text = value.ToString(CultureInfo.InvariantCulture);
                    }
                ListBox satelliteEditor = FindControl<ListBox>(form, "lbSputniks");
                GroupBox satelliteGroup = FindControl<GroupBox>(form, "gbSputniks");
                Action refreshSatellites = delegate
                {
                    satelliteEditor.BeginUpdate();
                    satelliteEditor.Items.Clear();
                    foreach (PlanetSputnikRecord satellite in planet.Satellites)
                        satelliteEditor.Items.Add(satellite);
                    satelliteEditor.EndUpdate();
                    satelliteGroup.Text = "Спутники: " +
                        planet.Satellites.Count.ToString(CultureInfo.InvariantCulture);
                };
                Action editSatellite = delegate
                {
                    PlanetSputnikRecord satellite = satelliteEditor.SelectedItem as PlanetSputnikRecord;
                    if (satellite == null) return;
                    EditSputnik(satellite);
                    satelliteEditor.Refresh();
                };
                satelliteEditor.DoubleClick += delegate { editSatellite(); };
                ContextMenuStrip satelliteMenu = new ContextMenuStrip();
                satelliteMenu.Items.Add("Редактировать", null, delegate { editSatellite(); });
                satelliteMenu.Items.Add("Удалить", null, delegate
                {
                    int selectedIndex = satelliteEditor.SelectedIndex;
                    if (selectedIndex < 0) return;
                    planet.Satellites.RemoveAt(selectedIndex);
                    planet.SatelliteCount = checked((ushort)planet.Satellites.Count);
                    refreshSatellites();
                });
                satelliteEditor.ContextMenuStrip = satelliteMenu;
                refreshSatellites();

                ListBox goneItemEditor = FindControl<ListBox>(form, "lbGoneItems");
                GroupBox goneItemGroup = FindControl<GroupBox>(form, "gbGoneItems");
                goneItemEditor.SelectionMode = SelectionMode.MultiExtended;
                Action refreshGoneItems = delegate
                {
                    goneItemEditor.BeginUpdate();
                    goneItemEditor.Items.Clear();
                    foreach (PlanetGoneItemRecord goneItem in planet.GoneItems)
                        goneItemEditor.Items.Add(new SearchResultEntry(goneItem,
                            PlanetGoneItemCaption(goneItem)));
                    goneItemEditor.EndUpdate();
                    goneItemGroup.Text = "Закопанные предметы: " +
                        planet.GoneItems.Count.ToString(CultureInfo.InvariantCulture);
                };
                Action editGoneItem = delegate
                {
                    SearchResultEntry selected = goneItemEditor.SelectedItem as SearchResultEntry;
                    PlanetGoneItemRecord goneItem = selected == null ? null :
                        selected.Value as PlanetGoneItemRecord;
                    if (goneItem == null || !planet.GoneItems.Contains(goneItem)) return;
                    EditPlanetGoneItem(goneItem, form);
                    refreshGoneItems();
                };
                goneItemEditor.DoubleClick += delegate { editGoneItem(); };
                goneItemEditor.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                        EditorFormFactory.ApplyContextPopupSelection(goneItemEditor, args.Location);
                };
                ContextMenuStrip goneItemMenu = new ContextMenuStrip();
                goneItemMenu.Items.Add("Редактировать", null, delegate { editGoneItem(); });
                goneItemMenu.Items.Add("Удалить", null, delegate
                {
                    List<PlanetGoneItemRecord> selected = new List<PlanetGoneItemRecord>();
                    foreach (object selectedObject in goneItemEditor.SelectedItems)
                    {
                        SearchResultEntry entry = selectedObject as SearchResultEntry;
                        PlanetGoneItemRecord goneItem = entry == null ? null :
                            entry.Value as PlanetGoneItemRecord;
                        if (goneItem != null) selected.Add(goneItem);
                    }
                    foreach (PlanetGoneItemRecord goneItem in selected)
                        if (planet.GoneItems.Remove(goneItem))
                            pendingDeletedItemStarts.Add(goneItem.ItemStart);
                    planet.GoneItemCount = checked((ushort)planet.GoneItems.Count);
                    refreshGoneItems();
                    RefreshObjectLists();
                });
                goneItemEditor.ContextMenuStrip = goneItemMenu;
                refreshGoneItems();

                ListBox shopEditor = FindControl<ListBox>(form, "lbEquipmentShop");
                shopEditor.SelectionMode = SelectionMode.MultiExtended;
                Action refreshShop = delegate
                {
                    shopEditor.BeginUpdate();
                    shopEditor.Items.Clear();
                    foreach (ShipItemListEntry shopItem in planet.EquipmentShopItems)
                        shopEditor.Items.Add(new SearchResultEntry(shopItem,
                            PlanetShopItemCaption(shopItem)));
                    shopEditor.EndUpdate();
                    FindControl<GroupBox>(form, "gbEquipmentShop").Text =
                        "Магазин оборудования: " +
                        planet.EquipmentShopItems.Count.ToString(CultureInfo.InvariantCulture);
                };
                Action editShopItem = delegate
                {
                    SearchResultEntry selected = shopEditor.SelectedItem as SearchResultEntry;
                    ShipItemListEntry shopItem = selected == null ? null :
                        selected.Value as ShipItemListEntry;
                    if (shopItem == null || !planet.EquipmentShopItems.Contains(shopItem)) return;
                    ItemHeaderRecord item = FindItemByStart(shopItem.ItemStart);
                    if (item == null) return;
                    EditItem(item, form);
                    refreshShop();
                };
                shopEditor.DoubleClick += delegate { editShopItem(); };
                shopEditor.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                        EditorFormFactory.ApplyContextPopupSelection(shopEditor, args.Location);
                };
                ContextMenuStrip shopMenu = new ContextMenuStrip();
                shopMenu.Items.Add("Редактировать", null, delegate { editShopItem(); });
                shopMenu.Items.Add("Удалить", null, delegate
                {
                    List<ShipItemListEntry> selected = new List<ShipItemListEntry>();
                    foreach (object selectedObject in shopEditor.SelectedItems)
                    {
                        SearchResultEntry entry = selectedObject as SearchResultEntry;
                        ShipItemListEntry shopItem = entry == null ? null :
                            entry.Value as ShipItemListEntry;
                        if (shopItem != null) selected.Add(shopItem);
                    }
                    foreach (ShipItemListEntry shopItem in selected)
                        if (planet.EquipmentShopItems.Remove(shopItem))
                            pendingDeletedItemStarts.Add(shopItem.ItemStart);
                    planet.EquipmentShopCount = checked((ushort)planet.EquipmentShopItems.Count);
                    refreshShop();
                    RefreshObjectLists();
                });
                shopEditor.ContextMenuStrip = shopMenu;
                refreshShop();

                ListBox relationEditor = FindControl<ListBox>(form, "lbRelationToRangers");
                List<ShipHeaderRecord> relationRangers = PlanetRelationRangers();
                Action refreshRelations = delegate
                {
                    int selectedIndex = relationEditor.SelectedIndex;
                    relationEditor.BeginUpdate();
                    relationEditor.Items.Clear();
                    if (planet.RelationToRangers != null)
                        for (int index = 0; index < planet.RelationToRangers.Length; index++)
                            relationEditor.Items.Add(new SearchResultEntry(index,
                                PlanetRelationRangerName(relationRangers, index) + ": " +
                                planet.RelationToRangers[index].ToString(CultureInfo.InvariantCulture)));
                    relationEditor.EndUpdate();
                    if (selectedIndex >= 0 && selectedIndex < relationEditor.Items.Count)
                        relationEditor.SelectedIndex = selectedIndex;
                };
                Action editRelation = delegate
                {
                    SearchResultEntry selected = relationEditor.SelectedItem as SearchResultEntry;
                    if (selected == null || !(selected.Value is int)) return;
                    if (EditPlanetRangerRelation(planet, (int)selected.Value,
                        relationRangers, form)) refreshRelations();
                };
                relationEditor.DoubleClick += delegate { editRelation(); };
                relationEditor.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                        EditorFormFactory.ApplyContextPopupSelection(
                            relationEditor, args.Location);
                };
                ContextMenuStrip relationMenu = new ContextMenuStrip();
                relationMenu.Items.Add("Редактировать", null, delegate { editRelation(); });
                relationEditor.ContextMenuStrip = relationMenu;
                refreshRelations();

                ListBox warriorEditor = FindControl<ListBox>(form, "lbWarriors");
                Action refreshWarriors = delegate
                {
                    int selectedIndex = warriorEditor.SelectedIndex;
                    warriorEditor.BeginUpdate();
                    warriorEditor.Items.Clear();
                    foreach (PlanetWarriorRecord warrior in planet.Warriors)
                        warriorEditor.Items.Add(new SearchResultEntry(warrior,
                            PlanetWarriorCaption(warrior)));
                    warriorEditor.EndUpdate();
                    if (selectedIndex >= 0 && selectedIndex < warriorEditor.Items.Count)
                        warriorEditor.SelectedIndex = selectedIndex;
                    FindControl<GroupBox>(form, "gbWarriors").Text = "Корабли на планете: " +
                        planet.Warriors.Count.ToString(CultureInfo.InvariantCulture);
                };
                Action editWarrior = delegate
                {
                    SearchResultEntry selected = warriorEditor.SelectedItem as SearchResultEntry;
                    PlanetWarriorRecord warrior = selected == null ? null :
                        selected.Value as PlanetWarriorRecord;
                    if (warrior == null || !planet.Warriors.Contains(warrior)) return;
                    ShipHeaderRecord ship = FindShipByStart(warrior.ShipStart);
                    if (ship == null) return;
                    EditShip(ship, form);
                    refreshWarriors();
                };
                warriorEditor.DoubleClick += delegate { editWarrior(); };
                warriorEditor.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Right)
                        EditorFormFactory.ApplyContextPopupSelection(
                            warriorEditor, args.Location);
                };
                ContextMenuStrip warriorMenu = new ContextMenuStrip();
                warriorMenu.Items.Add("Редактировать", null, delegate { editWarrior(); });
                warriorEditor.ContextMenuStrip = warriorMenu;
                refreshWarriors();

                TextBox[] supportedText = { name, polarAngle, polarRadius, people, money, rnd, rndOut, angle,
                    radius, waterSpace, waterSpaceDone, landSpace, landSpaceDone, hillSpace, hillSpaceDone,
                    orbitCount, openPoints, necessaryPercent, necessaryPercentK };
                foreach (TextBox editor in supportedText) editor.ReadOnly = false;
                foreach (TextBox editor in openInventions) editor.ReadOnly = false;
                visited.Enabled = true;

                if (planet.HasLateFields)
                {
                    TextBox[] lateText = { questNumber, rangerCount, transportCount, graphRadius,
                        graphSpeed, graphStep, graphRing };
                    foreach (TextBox editor in lateText) editor.ReadOnly = false;
                    PopulatePlanetGraphNameCombo(graphName, planet.GraphName);
                    questNumber.Text = planet.QuestNumber.ToString(CultureInfo.InvariantCulture);
                    rangerCount.Text = planet.RangerCount.ToString(CultureInfo.InvariantCulture);
                    transportCount.Text = planet.TransportCount.ToString(CultureInfo.InvariantCulture);
                    graphRadius.Text = planet.GraphRadius.ToString(CultureInfo.InvariantCulture);
                    graphSpeed.Text = planet.GraphSpeedRotate.ToString(CultureInfo.InvariantCulture);
                    graphStep.Text = planet.GraphStepRotate.ToString(CultureInfo.InvariantCulture);
                    graphRing.Text = planet.GraphRing.ToString(CultureInfo.InvariantCulture);
                }
                if (planet.HasFlags)
                {
                    customFaction.ReadOnly = false; noShopUpdate.ReadOnly = false;
                    noLanding.Enabled = true; noBuyShips.Enabled = true;
                    noRandomEvents.Enabled = true; isRogeria.Enabled = true;
                    customFaction.Text = planet.CustomFaction;
                    noShopUpdate.Text = planet.NoPlanetShopUpdate.ToString(CultureInfo.InvariantCulture);
                    noLanding.Checked = planet.NoLanding; noBuyShips.Checked = planet.NoBuyShips;
                    noRandomEvents.Checked = planet.NoRandomEvents; isRogeria.Checked = planet.IsRogeria;
                }

                name.Text = planet.Name;
                polarAngle.Text = planet.PolarAngle.ToString("R", CultureInfo.InvariantCulture);
                polarRadius.Text = planet.PolarRadius.ToString("R", CultureInfo.InvariantCulture);
                people.Text = planet.PeopleCount.ToString(CultureInfo.InvariantCulture);
                money.Text = planet.Money.ToString(CultureInfo.InvariantCulture);
                rnd.Text = planet.Raw08.ToString(CultureInfo.InvariantCulture);
                rndOut.Text = planet.Raw0C.ToString(CultureInfo.InvariantCulture);
                angle.Text = planet.Angle.ToString("R", CultureInfo.InvariantCulture);
                radius.Text = planet.Radius.ToString(CultureInfo.InvariantCulture);
                waterSpace.Text = planet.WaterSpace.ToString(CultureInfo.InvariantCulture);
                waterSpaceDone.Text = planet.WaterSpaceDone.ToString(CultureInfo.InvariantCulture);
                landSpace.Text = planet.LandSpace.ToString(CultureInfo.InvariantCulture);
                landSpaceDone.Text = planet.LandSpaceDone.ToString(CultureInfo.InvariantCulture);
                hillSpace.Text = planet.HillSpace.ToString(CultureInfo.InvariantCulture);
                hillSpaceDone.Text = planet.HillSpaceDone.ToString(CultureInfo.InvariantCulture);
                orbitCount.Text = planet.OrbitCount.ToString(CultureInfo.InvariantCulture);
                visited.Checked = planet.VisitedByPlayer;
                openPoints.Text = planet.OpenPointsInvention.ToString("R", CultureInfo.InvariantCulture);
                necessaryPercent.Text = planet.NecessaryPercent.ToString(CultureInfo.InvariantCulture);
                necessaryPercentK.Text = planet.NecessaryPercentK.ToString(CultureInfo.InvariantCulture);
                for (int index = 0; index < openInventions.Length; index++)
                    openInventions[index].Text = planet.OpenInventions[index].ToString(CultureInfo.InvariantCulture);

                PopulateByteCombo(economy, planet.Economy, new string[] { "Аграрная", "Смешанная", "Индустриальная" });
                PopulateByteCombo(government, planet.Government,
                    new string[] { "Анархия", "Диктатура", "Монархия", "Республика", "Демократия" });
                PopulateByteCombo(owner, planet.Owner, new string[] { "Малоки", "Пеленги", "Люди", "Фэяне",
                    "Гаальцы", "Доминаторы", "Нет владельца", "Пиратский клан" });
                PopulateByteCombo(race, planet.Race,
                    new string[] { "Малоки", "Пеленги", "Люди", "Фэяне", "Гаальцы" });
                PopulateByteCombo(currentInvention, planet.CurrentInvention, new string[] { "Корпус", "Бак",
                    "Двигатель", "Радар", "Сканер", "Дроид", "Захват", "Тех. уровень",
                    "Промышленный лазер", "Осколочное орудие", "Лезка", "Ракетомёт", "Третон",
                    "Волновой фазер", "Потоковый бластер", "Электронный резак", "Мультирезонатор",
                    "Атомный визион", "Дезинтегратор", "Турбогравир" });

                StarHeaderRecord parentStar = FindStarForOffset(planet.Start);
                FindControl<Label>(form, "lblStarVal").Text = parentStar == null ? "—" : parentStar.Name;
                FindControl<Label>(form, "lblConstellationVal").Text = ConstellationName(parentStar);
                form.Text = "Планета — " + planet.Name + " [ID " + planet.ObjectId + "]";
                form.ShowDialog(this);

                string updatedName = (name.Text ?? string.Empty).Trim();
                int parsedRnd, parsedRadius, parsedWaterSpace, parsedWaterSpaceDone, parsedLandSpace,
                    parsedLandSpaceDone, parsedHillSpace, parsedHillSpaceDone;
                uint parsedRndOut, parsedPeople, parsedMoney;
                float parsedPolarAngle, parsedPolarRadius, parsedAngle, parsedOpenPoints;
                byte parsedOrbitCount, parsedNecessaryPercent, parsedNecessaryPercentK;
                byte[] parsedOpenInventions = new byte[20];
                uint[,] parsedShopGoods = new uint[8, 3];
                byte[] parsedShopDeficit = new byte[8];
                byte[] parsedShopSale = new byte[8];
                bool validOpenInventions = true;
                for (int index = 0; index < parsedOpenInventions.Length; index++)
                    validOpenInventions &= byte.TryParse(openInventions[index].Text, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out parsedOpenInventions[index]);
                bool validShopGoods = true;
                for (int good = 0; good < 8; good++)
                {
                    for (int field = 0; field < 3; field++)
                        validShopGoods &= uint.TryParse(shopGoods[good, field].Text,
                            NumberStyles.Integer, CultureInfo.InvariantCulture,
                            out parsedShopGoods[good, field]);
                    validShopGoods &= byte.TryParse(shopGoods[good, 3].Text,
                        NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedShopDeficit[good]);
                    validShopGoods &= byte.TryParse(shopGoods[good, 4].Text,
                        NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedShopSale[good]);
                }
                ByteValueChoice selectedEconomy = economy.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedGovernment = government.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedOwner = owner.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedRace = race.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedInvention = currentInvention.SelectedItem as ByteValueChoice;
                if (updatedName.Length == 0 || updatedName.Length > 80 ||
                    !TryParseInt32(rnd.Text, out parsedRnd) || !TryParseUInt32(rndOut.Text, out parsedRndOut) ||
                    !TryParseFiniteFloat(polarAngle.Text, out parsedPolarAngle) ||
                    !TryParseFiniteFloat(polarRadius.Text, out parsedPolarRadius) ||
                    !TryParseFiniteFloat(angle.Text, out parsedAngle) ||
                    !TryParseInt32(radius.Text, out parsedRadius) ||
                    !TryParseInt32(waterSpace.Text, out parsedWaterSpace) ||
                    !TryParseInt32(waterSpaceDone.Text, out parsedWaterSpaceDone) ||
                    !TryParseInt32(landSpace.Text, out parsedLandSpace) ||
                    !TryParseInt32(landSpaceDone.Text, out parsedLandSpaceDone) ||
                    !TryParseInt32(hillSpace.Text, out parsedHillSpace) ||
                    !TryParseInt32(hillSpaceDone.Text, out parsedHillSpaceDone) ||
                    !byte.TryParse(orbitCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedOrbitCount) ||
                    !TryParseUInt32(people.Text, out parsedPeople) || !TryParseUInt32(money.Text, out parsedMoney) ||
                    !TryParseFiniteFloat(openPoints.Text, out parsedOpenPoints) ||
                    !byte.TryParse(necessaryPercent.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedNecessaryPercent) ||
                    !byte.TryParse(necessaryPercentK.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedNecessaryPercentK) || !validOpenInventions || selectedEconomy == null ||
                    !validShopGoods || selectedGovernment == null || selectedOwner == null ||
                    selectedRace == null || selectedInvention == null)
                {
                    MessageBox.Show(this, "Поля TPlanet не применены: проверьте имя и числовые значения.",
                        "TPlanet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ushort parsedRangerCount = 0, parsedTransportCount = 0, parsedGraphRadius = 0,
                    parsedGraphSpeed = 0;
                int parsedGraphStep = 0, parsedQuestNumber = 0;
                byte parsedGraphRing = 0, parsedNoShopUpdate = 0;
                string parsedGraphName = (graphName.Text ?? string.Empty).Trim();
                string parsedCustomFaction = (customFaction.Text ?? string.Empty).Trim();
                if (planet.HasLateFields &&
                    (parsedGraphName.Length == 0 || parsedGraphName.Length > 128 ||
                    !ushort.TryParse(rangerCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedRangerCount) ||
                    !ushort.TryParse(transportCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedTransportCount) ||
                    !ushort.TryParse(graphRadius.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedGraphRadius) ||
                    !ushort.TryParse(graphSpeed.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedGraphSpeed) || !TryParseInt32(graphStep.Text, out parsedGraphStep) ||
                    !byte.TryParse(graphRing.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedGraphRing) || !TryParseInt32(questNumber.Text, out parsedQuestNumber)))
                {
                    MessageBox.Show(this, "Поздние поля TPlanet не применены: проверьте графику, квест и счётчики.",
                        "TPlanet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (planet.HasFlags && (parsedCustomFaction.Length > 128 ||
                    !byte.TryParse(noShopUpdate.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out parsedNoShopUpdate) || parsedNoShopUpdate > 3))
                {
                    MessageBox.Show(this, "Флаги TPlanet не применены: код запрета магазина должен быть от 0 до 3.",
                        "TPlanet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                planet.Name = updatedName;
                planet.Raw08 = parsedRnd; planet.Raw0C = parsedRndOut;
                planet.PolarAngle = parsedPolarAngle; planet.PolarRadius = parsedPolarRadius;
                planet.Angle = parsedAngle; planet.Radius = parsedRadius;
                planet.WaterSpace = parsedWaterSpace; planet.WaterSpaceDone = parsedWaterSpaceDone;
                planet.LandSpace = parsedLandSpace; planet.LandSpaceDone = parsedLandSpaceDone;
                planet.HillSpace = parsedHillSpace; planet.HillSpaceDone = parsedHillSpaceDone;
                planet.OrbitCount = parsedOrbitCount; planet.VisitedByPlayer = visited.Checked;
                planet.OpenInventions = parsedOpenInventions; planet.CurrentInvention = selectedInvention.Value;
                planet.OpenPointsInvention = parsedOpenPoints; planet.NecessaryPercent = parsedNecessaryPercent;
                planet.NecessaryPercentK = parsedNecessaryPercentK; planet.PeopleCount = parsedPeople;
                planet.Economy = selectedEconomy.Value; planet.Money = parsedMoney;
                planet.Owner = selectedOwner.Value; planet.Race = selectedRace.Value;
                planet.Government = selectedGovernment.Value;
                planet.ShopGoods = parsedShopGoods; planet.ShopDeficit = parsedShopDeficit;
                planet.ShopSale = parsedShopSale;
                if (planet.HasLateFields)
                {
                    planet.RangerCount = parsedRangerCount; planet.TransportCount = parsedTransportCount;
                    planet.GraphRadius = parsedGraphRadius; planet.GraphName = parsedGraphName;
                    planet.GraphSpeedRotate = parsedGraphSpeed; planet.GraphStepRotate = parsedGraphStep;
                    planet.GraphRing = parsedGraphRing; planet.QuestNumber = parsedQuestNumber;
                }
                if (planet.HasFlags)
                {
                    planet.NoLanding = noLanding.Checked; planet.NoPlanetShopUpdate = parsedNoShopUpdate;
                    planet.NoBuyShips = noBuyShips.Checked; planet.NoRandomEvents = noRandomEvents.Checked;
                    planet.IsRogeria = isRogeria.Checked; planet.CustomFaction = parsedCustomFaction;
                }
                RefreshGalaxyView();
                RefreshObjectLists();
            }
        }

        private void EditShip(ShipHeaderRecord ship, IWin32Window dialogOwner = null)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSHIPFORM")))
            {
                ConfigureShipPages(form, ship);
                SetUnsupportedEditorsReadOnly(form);
                TextBox name = FindControl<TextBox>(form, "edName");
                TextBox customTypeName = FindControl<TextBox>(form, "edCustomTypeName");
                TextBox x = FindControl<TextBox>(form, "edPosX");
                TextBox y = FindControl<TextBox>(form, "edPosY");
                TextBox money = FindControl<TextBox>(form, "edMoney");
                TextBox rnd = FindControl<TextBox>(form, "edRnd");
                TextBox rndOut = FindControl<TextBox>(form, "edRndOut");
                TextBox day = FindControl<TextBox>(form, "edDay");
                TextBox face = FindControl<TextBox>(form, "edFace");
                ComboBox type = FindControl<ComboBox>(form, "cbType");
                ComboBox owner = FindControl<ComboBox>(form, "cbOwner");
                ComboBox pilotRace = FindControl<ComboBox>(form, "cbPilotRace");
                TextBox angleEditor = null, orderDataEditor = null, orderXEditor = null, orderYEditor = null;
                TextBox scriptOrderEditor = null, daysLandedEditor = null, graphTransparencyEditor = null;
                TextBox radiusStopEditor = null, protoplasmEditor = null, pointsEditor = null, freePointsEditor = null;
                TextBox dayWithoutPlayerEditor = null, groupOrderEditor = null, lastNextDayEditor = null;
                TextBox blazerChargeEditor = null, kellerChargeEditor = null, terronChargeEditor = null;
                TextBox techKnowledgeEditor = null, tradePenaltyEditor = null, tradePointsEditor = null;
                TextBox contrabandEditor = null, rewardViewEditor = null, deflectedEditor = null;
                TextBox swarmedEditor = null, swarmAnimationEditor = null, averageSpeedEditor = null;
                TextBox averageEnemySpeedEditor = null, averageEquipmentEditor = null, averageCapitalEditor = null;
                TextBox averageMoneyEditor = null, averageFreeSpaceEditor = null, costlyRatioEditor = null;
                TextBox[] skillEditors = new TextBox[6];
                CheckBox forsageEditor = null, orderAbsoluteEditor = null, abductedEditor = null;
                CheckBox graphDominatorEditor = null, inHyperEditor = null, shipDestroyEditor = null;
                CheckBox chameleonEnabledEditor = null;
                CheckBox blazerDetectEditor = null, kellerDetectEditor = null;
                CheckBox terronDetectEditor = null, noDropEditor = null, noTalkEditor = null;
                CheckBox noScanEditor = null, scriptChameleonEditor = null, robbedEditor = null;
                ComboBox orderTypeEditor = null, orderObjectEditor = null, followTypeEditor = null;
                ComboBox graphNameEditor = null, chameleonSeriesEditor = null;
                ComboBox swarmedByShipEditor = null;
                ComboBox noTargetEditor = null, standingEditor = null;
                TextBox[] normalEditors = new TextBox[15];
                ComboBox coalitionRankEditor = null, pirateRankEditor = null;
                ComboBox liberationPlanetEditor = null, lastPlanetEditor = null;
                ComboBox dominatorTypeEditor = null, dominatorSeriesEditor = null, runProgramEditor = null;
                ComboBox transportTypeEditor = null, warriorTypeEditor = null, pirateTypeEditor = null;
                TextBox runProgramDateEditor = null, piratePrisonEditor = null, desireConflictEditor = null;
                TextBox[] rangerEditors = new TextBox[12];
                ComboBox rangerMoralEditor = null, lastShipEditor = null;
                CheckBox excludedFromRatingEditor = null;
                DataGridView programsEditor = null;
                TextBox playerKillHyperEditor = null, playerKillHoleEditor = null;
                TextBox[] playerChameleonEditors = new TextBox[3];
                CheckBox playerPrisonEditor = null, playerTalkLockedEditor = null;
                CheckBox playerScanLockedEditor = null;
                DataGridView playerDominatorKillsEditor = null;
                TextBox[] playerScalarEditors = new TextBox[28];
                TextBox playerDepositPercentEditor = null;
                ComboBox playerFlyToStarEditor = null;
                DataGridView playerInvestmentEditor = null, playerWarBaseProgramsEditor = null;
                CheckBox playerNoJumpEditor = null, playerPirateClanEditor = null;
                ComboBox playerBridgeShipEditor = null, playerBridgePlanetEditor = null;
                TextBox playerBridgeBackgroundEditor = null;
                TextBox tranclucatorArtSizeEditor = null, tranclucatorArtNameEditor = null;
                ComboBox tranclucatorProprietorEditor = null;
                CheckBox tranclucatorDockingEditor = null, tranclucatorAutoArrangeEditor = null;
                CheckBox tranclucatorSeekItemsEditor = null, tranclucatorLandStorageEditor = null;
                CheckBox[] tranclucatorLandPermitEditors = new CheckBox[2];
                CheckedListBox tranclucatorSeekPermitEditor = null;
                TextBox[,] ruinsShopEditors = new TextBox[8, 3];
                TextBox ruinsEnergyEditor = null, ruinsFlyDateEditor = null, ruinsNoShopEditor = null;
                ComboBox ruinsFlyToStarEditor = null;
                CheckBox ruinsSponsorEditor = null, ruinsSpecialShipEditor = null, ruinsNoLandingEditor = null;
                TextBox[,] goods = new TextBox[8, 4];
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 4; field++)
                    {
                        goods[good, field] = FindControl<TextBox>(form,
                            "edGoods" + (good + 1) + (field + 1));
                        goods[good, field].ReadOnly = false;
                        goods[good, field].Text = ship.Goods[good, field].ToString(CultureInfo.InvariantCulture);
                    }
                foreach (TextBox editor in new TextBox[] { name, customTypeName, x, y, money, rnd, rndOut, day, face })
                    editor.ReadOnly = false;
                name.Text = ship.Name;
                customTypeName.Text = ship.ScriptName ?? string.Empty;
                x.Text = ship.X.ToString("R", CultureInfo.InvariantCulture);
                y.Text = ship.Y.ToString("R", CultureInfo.InvariantCulture);
                money.Text = ship.Money.ToString(CultureInfo.InvariantCulture);
                rnd.Text = ship.Rnd.ToString(CultureInfo.InvariantCulture);
                rndOut.Text = ship.RndOut.ToString(CultureInfo.InvariantCulture);
                day.Text = ship.Day.ToString(CultureInfo.InvariantCulture);
                face.Text = ship.Face.ToString(CultureInfo.InvariantCulture);
                DisplayComboValue(type, ShipTypeDisplayName(ship.Type));
                ToolTip typeToolTip = new ToolTip();
                string typeExplanation =
                    "Тип определяет бинарный класс и длину производного блока TShip. " +
                    (ship.IsPlayer
                        ? "Игрок обязан оставаться TRanger/TPlayer, поэтому его тип менять нельзя."
                        : "Без полной перестройки производного объекта менять его небезопасно.");
                Label typeLabel = FindControl<Label>(form, "lblType");
                typeLabel.Text = appSettings.LanguageIndex == 1 ?
                    "Type (SAV class)" : "Тип (класс SAV)";
                typeToolTip.SetToolTip(type, typeExplanation);
                typeToolTip.SetToolTip(typeLabel, typeExplanation);
                type.AccessibleDescription = typeExplanation;
                form.Disposed += delegate { typeToolTip.Dispose(); };
                PopulateByteCombo(owner, ship.Owner, new string[] { "Малоки", "Пеленги", "Люди", "Фэяне",
                    "Гаальцы", "Доминаторы", "Нет владельца", "Пиратский клан" });
                PopulateByteCombo(pilotRace, ship.PilotRace,
                    new string[] { "Малок", "Пеленг", "Человек", "Фэянин", "Гаалец" });
                if (ship.HasCommonTail)
                {
                    angleEditor = BindEditableText(form, "edAngle", ship.Angle.ToString("R", CultureInfo.InvariantCulture));
                    orderDataEditor = BindEditableText(form, "edOrderData", ship.OrderData.ToString(CultureInfo.InvariantCulture));
                    orderXEditor = BindEditableText(form, "edOrderDesX", ship.OrderDestinationX.ToString("R", CultureInfo.InvariantCulture));
                    orderYEditor = BindEditableText(form, "edOrderDesY", ship.OrderDestinationY.ToString("R", CultureInfo.InvariantCulture));
                    scriptOrderEditor = BindEditableText(form, "edScriptOrderAbsolute", ship.ScriptOrderAbsolute.ToString(CultureInfo.InvariantCulture));
                    daysLandedEditor = BindEditableText(form, "edDaysLanded", ship.DaysLanded.ToString(CultureInfo.InvariantCulture));
                    graphTransparencyEditor = BindEditableText(form, "edGraphShipTrans", ship.GraphShipTransparency.ToString(CultureInfo.InvariantCulture));
                    radiusStopEditor = BindEditableText(form, "edRadiusStop", ship.RadiusStop.ToString("R", CultureInfo.InvariantCulture));
                    protoplasmEditor = BindEditableText(form, "edProtoplasm", ship.Protoplasm.ToString(CultureInfo.InvariantCulture));
                    pointsEditor = BindEditableText(form, "edPoints", ship.Points.ToString(CultureInfo.InvariantCulture));
                    freePointsEditor = BindEditableText(form, "edFreePoints", ship.FreePoints.ToString(CultureInfo.InvariantCulture));
                    dayWithoutPlayerEditor = BindEditableText(form, "edDayWithoutPlayer", ship.DayWithoutPlayer.ToString(CultureInfo.InvariantCulture));
                    groupOrderEditor = BindEditableText(form, "edGroupOrder", ship.GroupOrder.ToString(CultureInfo.InvariantCulture));
                    lastNextDayEditor = BindEditableText(form, "edLastNextDay", ship.LastNextDay.ToString(CultureInfo.InvariantCulture));
                    string[] skillNames = { "edAccuracy", "edMobility", "edTechnical", "edTrader", "edCharm", "edLeadership" };
                    for (int index = 0; index < skillEditors.Length; index++)
                        skillEditors[index] = BindEditableText(form, skillNames[index], ship.Skills[index].ToString(CultureInfo.InvariantCulture));
                    blazerChargeEditor = BindEditableText(form, "edBlazerChameleonCharge", ship.BlazerChameleonCharge.ToString(CultureInfo.InvariantCulture));
                    kellerChargeEditor = BindEditableText(form, "edKellerChameleonCharge", ship.KellerChameleonCharge.ToString(CultureInfo.InvariantCulture));
                    terronChargeEditor = BindEditableText(form, "edTerronChameleonCharge", ship.TerronChameleonCharge.ToString(CultureInfo.InvariantCulture));
                    techKnowledgeEditor = BindEditableText(form, "edTechLevelKnowledge", ship.TechLevelKnowledge.ToString(CultureInfo.InvariantCulture));
                    tradePenaltyEditor = BindEditableText(form, "edTradePenalty", ship.TradePenalty.ToString(CultureInfo.InvariantCulture));
                    tradePointsEditor = BindEditableText(form, "edTradePoints", ship.TradePoints.ToString(CultureInfo.InvariantCulture));
                    contrabandEditor = BindEditableText(form, "edContrabandPoints", ship.ContrabandPoints.ToString(CultureInfo.InvariantCulture));
                    rewardViewEditor = BindEditableText(form, "edRewardViewCount", ship.RewardViewCount.ToString(CultureInfo.InvariantCulture));
                    deflectedEditor = BindEditableText(form, "edCountOfDeflectedPlayerShots", ship.CountOfDeflectedPlayerShots.ToString(CultureInfo.InvariantCulture));
                    swarmedEditor = BindEditableText(form, "edSwarmed", ship.Swarmed.ToString(CultureInfo.InvariantCulture));
                    swarmAnimationEditor = BindEditableText(form, "edSwarmAnimation", ship.SwarmAnimation ?? string.Empty);
                    averageSpeedEditor = BindEditableText(form, "edAverageSpeed", ship.AverageSpeed.ToString(CultureInfo.InvariantCulture));
                    averageEnemySpeedEditor = BindEditableText(form, "edAverageEnemySpeed", ship.AverageEnemySpeed.ToString(CultureInfo.InvariantCulture));
                    averageEquipmentEditor = BindEditableText(form, "edAverageEqValue", ship.AverageEquipmentValue.ToString("R", CultureInfo.InvariantCulture));
                    averageCapitalEditor = BindEditableText(form, "edAverageCapital", ship.AverageCapital.ToString(CultureInfo.InvariantCulture));
                    averageMoneyEditor = BindEditableText(form, "edAverageMoneyToCapital", ship.AverageMoneyToCapital.ToString("R", CultureInfo.InvariantCulture));
                    averageFreeSpaceEditor = BindEditableText(form, "edAverageFreeSpaceRatio", ship.AverageFreeSpaceRatio.ToString("R", CultureInfo.InvariantCulture));
                    costlyRatioEditor = BindEditableText(form, "edRatioOfTooCostlyEqInShop", ship.RatioOfTooCostlyEquipmentInShop.ToString("R", CultureInfo.InvariantCulture));
                    forsageEditor = BindEditableCheck(form, "chbForsage", ship.Forsage);
                    orderAbsoluteEditor = BindEditableCheck(form, "chbOrderAbsolute", ship.OrderAbsolute);
                    abductedEditor = BindEditableCheck(form, "chbAbducted", ship.Abducted);
                    graphDominatorEditor = BindEditableCheck(form, "chbGraphDominator", ship.GraphDominator);
                    inHyperEditor = BindEditableCheck(form, "chbInHiperSpace", ship.InHyperSpace);
                    shipDestroyEditor = BindEditableCheck(form, "chbShipDestroy", ship.ShipDestroy);
                    scriptChameleonEditor = BindEditableCheck(form, "chbScriptChameleon", ship.ScriptChameleon);
                    blazerDetectEditor = BindEditableCheck(form, "chbBlazerChameleonDetect", ship.BlazerChameleonDetect != 0);
                    kellerDetectEditor = BindEditableCheck(form, "chbKellerChameleonDetect", ship.KellerChameleonDetect != 0);
                    terronDetectEditor = BindEditableCheck(form, "chbTerronChameleonDetect", ship.TerronChameleonDetect != 0);
                    noDropEditor = BindEditableCheck(form, "chbNoDrop", ship.NoDrop);
                    noTalkEditor = BindEditableCheck(form, "chbNoTalk", ship.NoTalk);
                    noScanEditor = BindEditableCheck(form, "chbNoScan", ship.NoScan);
                    robbedEditor = BindEditableCheck(form, "chbRobbedByPlayer", ship.RobbedByPlayer);
                    orderTypeEditor = FindControl<ComboBox>(form, "cbOrderType");
                    PopulateByteCombo(orderTypeEditor, ship.OrderType,
                        new string[] { "Нет", "Движение", "Посадка", "Прыжок", "Чёрная дыра", "Взлёт", "Следовать", "Телепорт" });
                    orderObjectEditor = FindControl<ComboBox>(form, "cbOrderObj");
                    followTypeEditor = FindControl<ComboBox>(form, "cbFollowType");
                    PopulateFollowTypeCombo(followTypeEditor, ship.OrderData);
                    bool initialOrderRefresh = true;
                    Action refreshOrderTargets = delegate
                    {
                        ByteValueChoice selectedOrder = orderTypeEditor.SelectedItem as ByteValueChoice;
                        byte orderType = selectedOrder == null ? ship.OrderType : selectedOrder.Value;
                        uint targetId = initialOrderRefresh ? ship.OrderObjectId : 0U;
                        PopulateShipOrderTargetCombo(orderObjectEditor, ship, orderType, targetId);
                        followTypeEditor.Enabled = orderType == 6;
                        UInt32ValueChoice refreshedTarget = orderObjectEditor.SelectedItem as UInt32ValueChoice;
                        UpdateShipOrderDetails(orderType,
                            refreshedTarget == null ? 0U : refreshedTarget.Value, ship,
                            followTypeEditor, orderDataEditor, orderXEditor, orderYEditor, angleEditor);
                        initialOrderRefresh = false;
                    };
                    orderTypeEditor.SelectedIndexChanged += delegate { refreshOrderTargets(); };
                    orderObjectEditor.SelectedIndexChanged += delegate
                    {
                        ByteValueChoice selectedOrder = orderTypeEditor.SelectedItem as ByteValueChoice;
                        UInt32ValueChoice selectedTarget = orderObjectEditor.SelectedItem as UInt32ValueChoice;
                        if (selectedOrder != null && selectedTarget != null)
                            UpdateShipOrderDetails(selectedOrder.Value, selectedTarget.Value, ship,
                                followTypeEditor, orderDataEditor, orderXEditor, orderYEditor, angleEditor);
                    };
                    followTypeEditor.SelectedIndexChanged += delegate
                    {
                        ByteValueChoice selectedFollow = followTypeEditor.SelectedItem as ByteValueChoice;
                        UInt32ValueChoice selectedTarget = orderObjectEditor.SelectedItem as UInt32ValueChoice;
                        ByteValueChoice selectedOrder = orderTypeEditor.SelectedItem as ByteValueChoice;
                        if (followTypeEditor.Enabled && selectedFollow != null && selectedTarget != null && selectedOrder != null)
                            UpdateShipOrderDetails(selectedOrder.Value, selectedTarget.Value, ship,
                                followTypeEditor, orderDataEditor, orderXEditor, orderYEditor, angleEditor);
                    };
                    refreshOrderTargets();
                    graphNameEditor = FindControl<ComboBox>(form, "cbGraphName");
                    PopulateShipGraphNameCombo(graphNameEditor, ship.GraphName);
                    chameleonSeriesEditor = FindControl<ComboBox>(form, "cbChameleonSeries");
                    PopulateByteCombo(chameleonSeriesEditor, ship.ChameleonSeries,
                        new string[] { "Блазер", "Келлер", "Террон" });
                    chameleonEnabledEditor = BindCheckableGroup(form, "gbChameleon",
                        ship.ChameleonEnabled);
                    noTargetEditor = FindControl<ComboBox>(form, "cbNoTarget");
                    PopulateByteCombo(noTargetEditor, ship.NoTarget,
                        new string[] { "Обычная", "Запрет всем", "Запрет ИИ", "Не грабить", "Не грабить рейнджерам", "Не грабить пиратам", "Привлекать пиратов" });
                    standingEditor = FindControl<ComboBox>(form, "cbCurStanding");
                    PopulateByteCombo(standingEditor, ship.CurrentStanding,
                        new string[] { "Доминатор", "Нет", "Коалиция: военный", "Коалиция: активный", "Коалиция: пассивный", "Нейтральный", "Пират: пассивный", "Пират: активный", "Пират: военный", "Особый" });
                    swarmedByShipEditor = FindControl<ComboBox>(form, "cbSwarmedByShip");
                    PopulateShipReferenceCombo(swarmedByShipEditor, ship.SwarmedByShipId);
                    BindShipIllnessCollection(FindControl<ListBox>(form, "lbIllness"),
                        ship.Illnesses, form);
                    BindShipRewardCollection(FindControl<ListBox>(form, "lbRewards"),
                        ship.Rewards, form);
                    if (ship.HasNormalShipTail)
                    {
                        string[] normalNames = { "edKillAllShips", "edKillPirates", "edKillDominators",
                            "edLiberationSystems", "edKillPacifics", "edKillWarriors", "edKillRangers",
                            "edKillInCurSystemDominators", "edKillInCurSystemPirates",
                            "edKillInCurSystemNormals", "edKillCustomInCurSystem", "edLiberationKills",
                            "edRankPoints", "edPirateRankPoints", "edTurnPlayerMoneyGoods" };
                        string[] normalValues = { ship.KillAllShips.ToString(CultureInfo.InvariantCulture),
                            ship.KillPirates.ToString(CultureInfo.InvariantCulture),
                            ship.KillDominators.ToString(CultureInfo.InvariantCulture),
                            ship.LiberationSystems.ToString(CultureInfo.InvariantCulture),
                            ship.KillPacifics.ToString(CultureInfo.InvariantCulture),
                            ship.KillWarriors.ToString(CultureInfo.InvariantCulture),
                            ship.KillRangers.ToString(CultureInfo.InvariantCulture),
                            ship.KillInCurrentSystemDominators.ToString(CultureInfo.InvariantCulture),
                            ship.KillInCurrentSystemPirates.ToString(CultureInfo.InvariantCulture),
                            ship.KillInCurrentSystemNormals.ToString(CultureInfo.InvariantCulture),
                            ship.KillCustomInCurrentSystem.ToString(CultureInfo.InvariantCulture),
                            ship.LiberationKills.ToString(CultureInfo.InvariantCulture),
                            ship.CoalitionRankPoints.ToString(CultureInfo.InvariantCulture),
                            ship.PirateRankPoints.ToString(CultureInfo.InvariantCulture),
                            ship.TurnPlayerMoneyGoods.ToString(CultureInfo.InvariantCulture) };
                        for (int index = 0; index < normalEditors.Length; index++)
                            normalEditors[index] = BindEditableText(form, normalNames[index], normalValues[index]);
                        coalitionRankEditor = FindControl<ComboBox>(form, "cbRank");
                        PopulateByteCombo(coalitionRankEditor, ship.CoalitionRank,
                            new string[] { "Новичок", "Кадет", "Пилот", "Ведомый", "Лидер", "Ас", "Командор", "Адмирал" });
                        pirateRankEditor = FindControl<ComboBox>(form, "cbPirateRank");
                        PopulateByteCombo(pirateRankEditor, ship.PirateRank,
                            new string[] { "Новичок", "Малек", "Рейдер", "Шкипер", "Громила", "Атаман", "Хан", "Барон" });
                        liberationPlanetEditor = FindControl<ComboBox>(form, "cbLiberationPlanet");
                        PopulatePlanetReferenceCombo(liberationPlanetEditor, ship.LiberationPlanetId);
                        lastPlanetEditor = FindControl<ComboBox>(form, "cbLastPlanet");
                        PopulatePlanetReferenceCombo(lastPlanetEditor, ship.LastPlanetId);
                    }
                    if (ship.HasSimpleDerivedTail)
                    {
                        if (ship.Type == 0)
                        {
                            dominatorTypeEditor = FindControl<ComboBox>(form, "cbDominatorType");
                            PopulateByteCombo(dominatorTypeEditor, ship.DominatorType,
                                new string[] { "K0", "K1", "K2", "K3", "K4", "K5", "K6", "K7" });
                            dominatorSeriesEditor = FindControl<ComboBox>(form, "cbDominatorSeries");
                            PopulateByteCombo(dominatorSeriesEditor, ship.DominatorSeries,
                                new string[] { "Блазер", "Келлер", "Террон" });
                            runProgramDateEditor = BindEditableText(form, "edRunProgrammDate",
                                ship.RunProgramDate.ToString(CultureInfo.InvariantCulture));
                            runProgramEditor = FindControl<ComboBox>(form, "cbRunProgrammName");
                            PopulateByteCombo(runProgramEditor, ship.RunProgramName,
                                new string[] { "Вызов Келлера", "Логическое отрицание", "Дематериализация",
                                    "Энерготрон", "Взлом САБ", "Интерком", "Кораблекрушение", "Блокировка оружия",
                                    "Безумие", "Шок", "Самоуничтожение", "Отключение" });
                        }
                        else if (ship.Type == 2)
                        {
                            transportTypeEditor = FindControl<ComboBox>(form, "cbTransportType");
                            PopulateByteCombo(transportTypeEditor, ship.TransportType,
                                new string[] { "Транспорт", "Лайнер", "Дипломат" });
                        }
                        else if (ship.Type == 3)
                        {
                            pirateTypeEditor = FindControl<ComboBox>(form, "cbPirateType");
                            PopulateByteCombo(pirateTypeEditor, ship.PirateType,
                                new string[] { "Мародёр", "Блокировщик", "Фланговый", "Поддержка" });
                            piratePrisonEditor = BindEditableText(form, "edPiratePrison",
                                ship.PiratePrison.ToString(CultureInfo.InvariantCulture));
                            desireConflictEditor = BindEditableText(form, "edDesireConflict",
                                ship.DesireConflict.ToString("R", CultureInfo.InvariantCulture));
                        }
                        else if (ship.Type == 4)
                        {
                            warriorTypeEditor = FindControl<ComboBox>(form, "cbWarriorType");
                            PopulateByteCombo(warriorTypeEditor, ship.WarriorType,
                                new string[] { "Истребитель", "Флагман" });
                        }
                    }
                    if (ship.HasRangerTail)
                    {
                        string[] rangerNames = { "edStatusTrader", "edStatusPirate", "edStatusWarrior",
                            "edEminentPointsTrader", "edEminentPointsPirate", "edEminentPointsWarrior",
                            "edCourageous", "edStatusChangeWarrior", "edStatusChangePirate",
                            "edStatusChangeTrader", "edRangerPrison", "edNods" };
                        string[] rangerValues = { ship.RangerStatusTrader.ToString(CultureInfo.InvariantCulture),
                            ship.RangerStatusPirate.ToString(CultureInfo.InvariantCulture),
                            ship.RangerStatusWarrior.ToString(CultureInfo.InvariantCulture),
                            ship.EminentPointsTrader.ToString(CultureInfo.InvariantCulture),
                            ship.EminentPointsPirate.ToString(CultureInfo.InvariantCulture),
                            ship.EminentPointsWarrior.ToString(CultureInfo.InvariantCulture),
                            ship.Courageous.ToString(CultureInfo.InvariantCulture),
                            ship.StatusChangeWarrior.ToString(CultureInfo.InvariantCulture),
                            ship.StatusChangePirate.ToString(CultureInfo.InvariantCulture),
                            ship.StatusChangeTrader.ToString(CultureInfo.InvariantCulture),
                            ship.RangerPrison.ToString(CultureInfo.InvariantCulture),
                            ship.Nods.ToString(CultureInfo.InvariantCulture) };
                        for (int index = 0; index < rangerEditors.Length; index++)
                            rangerEditors[index] = BindEditableText(form, rangerNames[index], rangerValues[index]);
                        rangerMoralEditor = FindControl<ComboBox>(form, "cbMoral");
                        PopulateByteCombo(rangerMoralEditor, ship.RangerMoral,
                            new string[] { "Торговец", "Пират", "Воин" });
                        lastShipEditor = FindControl<ComboBox>(form, "cbLastShip");
                        PopulateShipReferenceCombo(lastShipEditor, ship.LastShipId);
                        excludedFromRatingEditor = BindEditableCheck(form, "chbExcludedFromRating",
                            ship.ExcludedFromRating);
                        ListBox quests = FindControl<ListBox>(form, "lbQuests");
                        BindRangerQuestCollection(quests, ship, form);
                        programsEditor = FindControl<DataGridView>(form, "sgProgramms");
                        PopulateProgramGrid(programsEditor, ship.ProgramCounts);
                    }
                    if (ship.HasPlayerPrefix)
                    {
                        playerPrisonEditor = BindEditableCheck(form, "chbPlayerPrison", ship.PlayerPrison);
                        playerTalkLockedEditor = BindEditableCheck(form, "chbTalkLocked", ship.PlayerTalkLocked);
                        playerScanLockedEditor = BindEditableCheck(form, "chbScanLocked", ship.PlayerScanLocked);
                        playerKillHyperEditor = BindEditableText(form, "edKillShipInGiperSpace",
                            ship.KillShipInHyperSpace.ToString(CultureInfo.InvariantCulture));
                        playerKillHoleEditor = BindEditableText(form, "edKillShipInHole",
                            ship.KillShipInHole.ToString(CultureInfo.InvariantCulture));
                        string[] chameleonNames = { "edChameleonLogicBlazer", "edChameleonLogicKeller",
                            "edChameleonLogicTerron" };
                        for (int index = 0; index < playerChameleonEditors.Length; index++)
                            playerChameleonEditors[index] = BindEditableText(form, chameleonNames[index],
                                ship.ChameleonLogic[index].ToString(CultureInfo.InvariantCulture));
                        playerDominatorKillsEditor = FindControl<DataGridView>(form,
                            "sgKillDominatorsByType");
                        PopulatePlayerDominatorKillGrid(playerDominatorKillsEditor,
                            ship.KillDominatorsByType);
                        if (ship.HasPlayerJournal)
                            BindPlayerJournalCollection(FindControl<ListBox>(form, "lbJournal"),
                                ship, form);
                        if (ship.HasPlayerNews)
                            BindPlayerNewsCollection(FindControl<ListBox>(form, "lbPlanetNews"),
                                ship, form);
                    }
                    if (ship.HasPlayerFinancialTail)
                    {
                        string[] scalarNames = { "edDebt", "edDebtDate", "edDebtCnt", "edDeposit",
                            "edDepositDate", "edDepositDay", "edMedPolicy", "edPirateLicense",
                            "edPiratePoints", "edPirateNewPoints", "edImmunity",
                            "edDayWBGiveProgramms", "edHitEnemyAfterTakeProgramms",
                            "edPlanetBattlesWin", "edLastPlanetBattleDate", "edCntIll", "edCntStim",
                            "edCntPrison", "edUnkPlanetComplete", "edCntChangeRace", "edCntChangeSide",
                            "edHotEquipmentCur", "edGotoGov", "edExpPointsForDominatorKills",
                            "edExpPointsForPirateKills", "edExpPointsForGoodShipKills",
                            "edExpPointsForTrade", "edCaptainOnTheBridge" };
                        string[] scalarValues = { ship.PlayerDebt.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDebtDate.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDebtCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDeposit.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDepositDate.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDepositDay.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerMedPolicy.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerPirateLicense.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerPiratePoints.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerPirateNewPoints.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerImmunity.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerDayWarBaseGivePrograms.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerHitEnemyAfterPrograms.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerPlanetBattlesWin.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerLastPlanetBattleDate.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerIllnessCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerStimulatorCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerPrisonCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerUnknownPlanetComplete.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerChangeRaceCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerChangeSideCount.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerHotEquipmentCurrent.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerGoToGovernment.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerExperienceDominatorKills.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerExperiencePirateKills.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerExperienceGoodShipKills.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerExperienceTrade.ToString(CultureInfo.InvariantCulture),
                            ship.PlayerCaptainOnBridge.ToString(CultureInfo.InvariantCulture) };
                        for (int index = 0; index < playerScalarEditors.Length; index++)
                            playerScalarEditors[index] = BindEditableText(form, scalarNames[index],
                                scalarValues[index]);
                        playerDepositPercentEditor = BindEditableText(form, "edDepositPercent",
                            ship.PlayerDepositPercent.ToString("R", CultureInfo.InvariantCulture));
                        playerFlyToStarEditor = FindControl<ComboBox>(form, "cbFlyToStar");
                        PopulateStarReferenceCombo(playerFlyToStarEditor, ship.PlayerFlyToStarId);
                        playerInvestmentEditor = FindControl<DataGridView>(form, "sgInvestmentDay");
                        PopulatePlayerInvestmentGrid(playerInvestmentEditor, ship.PlayerInvestments);
                        playerWarBaseProgramsEditor = FindControl<DataGridView>(form, "sgProgrammsInWB");
                        PopulateProgramGrid(playerWarBaseProgramsEditor, ship.PlayerProgramsInWarBase);
                        playerNoJumpEditor = BindEditableCheck(form, "chbNoJump", ship.PlayerNoJump);
                        playerPirateClanEditor = BindEditableCheck(form, "chbPirateClanReal",
                            ship.PlayerPirateClanReal);
                        ListBox robotMaps = FindControl<ListBox>(form, "lbRobotMaps");
                        if (ship.HasPlayerRobotMaps)
                            BindPlayerRobotMapCollection(robotMaps, ship, form);
                        Button setsButton = FindControl<Button>(form, "btnSets");
                        setsButton.Enabled = true;
                        setsButton.Click += delegate { ShowPlayerEquipmentSets(ship, form); };
                        Button infectionsButton = FindControl<Button>(form, "btnInfectionsPlace");
                        infectionsButton.Enabled = true;
                        infectionsButton.Click += delegate { ShowPlayerInfectionPlaces(ship, form); };
                        if (ship.HasPlayerBridge && ship.PlayerBridgeRuins != null)
                        {
                            playerBridgeShipEditor = FindControl<ComboBox>(form, "cbBridgeCurShip");
                            playerBridgePlanetEditor = FindControl<ComboBox>(form, "cbBridgeCurPlanet");
                            PopulateShipReferenceCombo(playerBridgeShipEditor,
                                ship.PlayerBridgeCurrentShipId);
                            PopulatePlanetReferenceCombo(playerBridgePlanetEditor,
                                ship.PlayerBridgeCurrentPlanetId);
                            playerBridgeBackgroundEditor = BindEditableText(form,
                                "edBridgeBGReplace", ship.PlayerBridgeBackground ?? string.Empty);
                            Button bridgeButton = FindControl<Button>(form, "btnBridge");
                            bridgeButton.Enabled = true;
                            bridgeButton.Click += delegate
                            {
                                EditShip(ship.PlayerBridgeRuins, form);
                            };
                        }
                    }
                    if (ship.HasTranclucatorTail)
                    {
                        tranclucatorArtSizeEditor = BindEditableText(form, "edArtSize",
                            ship.TranclucatorArtSize.ToString(CultureInfo.InvariantCulture));
                        tranclucatorArtNameEditor = BindEditableText(form, "edArtSysName",
                            ship.TranclucatorArtSystemName ?? string.Empty);
                        tranclucatorDockingEditor = BindEditableCheck(form, "chbDocking",
                            ship.TranclucatorDocking);
                        tranclucatorAutoArrangeEditor = BindEditableCheck(form, "chbAutoArrange",
                            ship.TranclucatorAutoArrange);
                        tranclucatorProprietorEditor = FindControl<ComboBox>(form, "cbProprietor");
                        PopulateShipReferenceCombo(tranclucatorProprietorEditor,
                            ship.TranclucatorProprietorShipId);
                        tranclucatorSeekItemsEditor = BindCheckableGroup(form, "gbSeekItems",
                            ship.TranclucatorSeekItems);
                        tranclucatorLandStorageEditor = BindCheckableGroup(form, "gbLandStorage",
                            ship.TranclucatorLandStorage);
                        tranclucatorSeekPermitEditor = FindControl<CheckedListBox>(form, "clbSeekPermit");
                        string[] permitNames = { "Оружие", "Оборудование", "Артефакты", "Товары",
                            "Модули", "Ракеты", "Прочие предметы" };
                        tranclucatorSeekPermitEditor.Items.Clear();
                        for (int index = 0; index < permitNames.Length; index++)
                            tranclucatorSeekPermitEditor.Items.Add(permitNames[index],
                                ship.TranclucatorSeekPermits[index]);
                        tranclucatorSeekPermitEditor.Enabled = true;
                        tranclucatorLandPermitEditors[0] = BindEditableCheck(form, "chbLandPermit1",
                            ship.TranclucatorLandPermits[0]);
                        tranclucatorLandPermitEditors[1] = BindEditableCheck(form, "chbLandPermit2",
                            ship.TranclucatorLandPermits[1]);
                    }
                    if (ship.HasRuinsTail)
                    {
                        for (int good = 0; good < 8; good++)
                            for (int field = 0; field < 3; field++)
                                ruinsShopEditors[good, field] = BindEditableText(form,
                                    "edShopGoods" + (good + 1) + (field + 1),
                                    ship.RuinsShopGoods[good, field].ToString(CultureInfo.InvariantCulture));
                        ruinsEnergyEditor = BindEditableText(form, "edRuinsEnergy",
                            ship.RuinsEnergy.ToString(CultureInfo.InvariantCulture));
                        ruinsFlyDateEditor = BindEditableText(form, "edFlyDate",
                            ship.RuinsFlyDate.ToString(CultureInfo.InvariantCulture));
                        ruinsNoShopEditor = BindEditableText(form, "edNoShopUpdate",
                            ship.RuinsNoShopUpdate.ToString(CultureInfo.InvariantCulture));
                        ruinsFlyToStarEditor = FindControl<ComboBox>(form, "cbRuinsFlyToStar");
                        PopulateStarReferenceCombo(ruinsFlyToStarEditor, ship.RuinsFlyToStarId);
                        ruinsSponsorEditor = BindEditableCheck(form, "chbSponsor", ship.RuinsSponsor);
                        ruinsSpecialShipEditor = BindEditableCheck(form, "chbSpecialShip", ship.RuinsSpecialShip);
                        ruinsNoLandingEditor = BindEditableCheck(form, "chbNoLanding", ship.RuinsNoLanding);
                        ListBox equipmentShop = FindControl<ListBox>(form, "lbEquipmentShop");
                        BindShipItemCollection(equipmentShop, ship, ship.RuinsEquipmentItems, false,
                            delegate(int count) { ship.RuinsEquipmentItemCount = checked((ushort)count); });
                        FindControl<GroupBox>(form, "gbEquipmentShop").Text = "Магазин оборудования: " +
                            ship.RuinsEquipmentItems.Count.ToString(CultureInfo.InvariantCulture);
                        ListBox satellites = FindControl<ListBox>(form, "lbSaleSatellites");
                        BindRuinsSaleSatellite(satellites, ship, form);
                    }
                }
                BindShipPreCommonEditors(form, ship);
                FindControl<Label>(form, "lblHomePlanetVal").Text = PlanetName(ship.HomePlanetId);
                FindControl<Label>(form, "lblCurStarVal").Text = StarName(ship.CurrentStarId);
                FindControl<Label>(form, "lblCurPlanetVal").Text = PlanetName(ship.CurrentPlanetId);
                FindControl<Label>(form, "lblCurShipVal").Text = ShipName(ship.CurrentShipId);
                FindControl<Label>(form, "lblScriptShipVal").Text = "—";
                StarHeaderRecord parentStar = FindStarForOffset(ship.Start);
                FindControl<Label>(form, "lblCurConstellationVal").Text = ConstellationName(parentStar);
                form.Text = (ship.IsStation ? "Станция" : "Корабль") + " — " + ship.Name + " [ID " + ship.ObjectId + "]";
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(dialogOwner ?? this);

                float parsedX, parsedY;
                uint parsedMoney, parsedRnd, parsedRndOut, parsedDay;
                int parsedFace;
                uint[,] parsedGoods = new uint[8, 4];
                bool goodsValid = true;
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 4; field++)
                        goodsValid &= TryParseUInt32(goods[good, field].Text, out parsedGoods[good, field]);
                ByteValueChoice selectedOwner = owner.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedPilotRace = pilotRace.SelectedItem as ByteValueChoice;
                string updatedName = (name.Text ?? string.Empty).Trim();
                string updatedCustomTypeName = (customTypeName.Text ?? string.Empty).Trim();
                float parsedAngle = ship.Angle, parsedOrderX = ship.OrderDestinationX;
                float parsedOrderY = ship.OrderDestinationY, parsedRadiusStop = ship.RadiusStop;
                float parsedAverageEquipment = ship.AverageEquipmentValue;
                float parsedAverageMoney = ship.AverageMoneyToCapital;
                float parsedAverageFreeSpace = ship.AverageFreeSpaceRatio;
                float parsedCostlyRatio = ship.RatioOfTooCostlyEquipmentInShop;
                uint parsedOrderData = ship.OrderData, parsedPoints = ship.Points, parsedFreePoints = ship.FreePoints;
                ushort parsedProtoplasm = ship.Protoplasm, parsedDayWithoutPlayer = ship.DayWithoutPlayer;
                ushort parsedGroupOrder = ship.GroupOrder, parsedDeflected = ship.CountOfDeflectedPlayerShots;
                int parsedDaysLanded = ship.DaysLanded, parsedLastNextDay = ship.LastNextDay;
                int parsedBlazerCharge = ship.BlazerChameleonCharge, parsedKellerCharge = ship.KellerChameleonCharge;
                int parsedTerronCharge = ship.TerronChameleonCharge, parsedTradePenalty = ship.TradePenalty;
                int parsedTradePoints = ship.TradePoints, parsedContraband = ship.ContrabandPoints;
                int parsedRewardView = ship.RewardViewCount, parsedSwarmed = ship.Swarmed;
                int parsedAverageSpeed = ship.AverageSpeed, parsedAverageEnemySpeed = ship.AverageEnemySpeed;
                int parsedAverageCapital = ship.AverageCapital;
                byte parsedScriptOrder = ship.ScriptOrderAbsolute, parsedGraphTransparency = ship.GraphShipTransparency;
                byte parsedTechKnowledge = ship.TechLevelKnowledge;
                byte[] parsedSkills = (byte[])ship.Skills.Clone();
                ByteValueChoice selectedOrderType = null, selectedChameleonSeries = null;
                ByteValueChoice selectedNoTarget = null, selectedStanding = null;
                UInt32ValueChoice selectedOrderObject = null, selectedSwarmedByShip = null;
                string updatedGraphName = ship.GraphName;
                string updatedSwarmAnimation = ship.SwarmAnimation ?? string.Empty;
                bool tailValid = true;
                if (ship.HasCommonTail)
                {
                    selectedOrderType = orderTypeEditor.SelectedItem as ByteValueChoice;
                    selectedOrderObject = orderObjectEditor.SelectedItem as UInt32ValueChoice;
                    selectedSwarmedByShip = swarmedByShipEditor.SelectedItem as UInt32ValueChoice;
                    selectedChameleonSeries = chameleonSeriesEditor.SelectedItem as ByteValueChoice;
                    selectedNoTarget = noTargetEditor.SelectedItem as ByteValueChoice;
                    selectedStanding = standingEditor.SelectedItem as ByteValueChoice;
                    updatedGraphName = (graphNameEditor.Text ?? string.Empty).Trim();
                    updatedSwarmAnimation = (swarmAnimationEditor.Text ?? string.Empty).Trim();
                    bool orderNeedsObject = selectedOrderType != null &&
                        (selectedOrderType.Value == 2 || selectedOrderType.Value == 3 ||
                         selectedOrderType.Value == 4 || selectedOrderType.Value == 6 ||
                         selectedOrderType.Value == 7);
                    tailValid = selectedOrderType != null && (!orderNeedsObject || selectedOrderObject != null) &&
                        selectedSwarmedByShip != null && selectedChameleonSeries != null &&
                        selectedNoTarget != null && selectedStanding != null &&
                        IsEditableShipGraphName(updatedGraphName) && updatedSwarmAnimation.Length <= 128 &&
                        TryParseFiniteFloat(angleEditor.Text, out parsedAngle) &&
                        TryParseUInt32(orderDataEditor.Text, out parsedOrderData) &&
                        TryParseFiniteFloat(orderXEditor.Text, out parsedOrderX) &&
                        TryParseFiniteFloat(orderYEditor.Text, out parsedOrderY) &&
                        TryParseByte(scriptOrderEditor.Text, out parsedScriptOrder) &&
                        TryParseInt32(daysLandedEditor.Text, out parsedDaysLanded) &&
                        TryParseByte(graphTransparencyEditor.Text, out parsedGraphTransparency) &&
                        TryParseFiniteFloat(radiusStopEditor.Text, out parsedRadiusStop) &&
                        TryParseUInt16(protoplasmEditor.Text, out parsedProtoplasm) &&
                        TryParseUInt32(pointsEditor.Text, out parsedPoints) &&
                        TryParseUInt32(freePointsEditor.Text, out parsedFreePoints) &&
                        TryParseUInt16(dayWithoutPlayerEditor.Text, out parsedDayWithoutPlayer) &&
                        TryParseUInt16(groupOrderEditor.Text, out parsedGroupOrder) &&
                        TryParseInt32(lastNextDayEditor.Text, out parsedLastNextDay) &&
                        TryParseInt32(blazerChargeEditor.Text, out parsedBlazerCharge) &&
                        TryParseInt32(kellerChargeEditor.Text, out parsedKellerCharge) &&
                        TryParseInt32(terronChargeEditor.Text, out parsedTerronCharge) &&
                        TryParseByte(techKnowledgeEditor.Text, out parsedTechKnowledge) &&
                        TryParseInt32(tradePenaltyEditor.Text, out parsedTradePenalty) &&
                        TryParseInt32(tradePointsEditor.Text, out parsedTradePoints) &&
                        TryParseInt32(contrabandEditor.Text, out parsedContraband) &&
                        TryParseInt32(rewardViewEditor.Text, out parsedRewardView) &&
                        TryParseUInt16(deflectedEditor.Text, out parsedDeflected) &&
                        TryParseInt32(swarmedEditor.Text, out parsedSwarmed) &&
                        TryParseInt32(averageSpeedEditor.Text, out parsedAverageSpeed) &&
                        TryParseInt32(averageEnemySpeedEditor.Text, out parsedAverageEnemySpeed) &&
                        TryParseFiniteFloat(averageEquipmentEditor.Text, out parsedAverageEquipment) &&
                        TryParseInt32(averageCapitalEditor.Text, out parsedAverageCapital) &&
                        TryParseFiniteFloat(averageMoneyEditor.Text, out parsedAverageMoney) &&
                        TryParseFiniteFloat(averageFreeSpaceEditor.Text, out parsedAverageFreeSpace) &&
                        TryParseFiniteFloat(costlyRatioEditor.Text, out parsedCostlyRatio) &&
                        (parsedSwarmed <= 0 || updatedSwarmAnimation.Length > 0);
                    for (int index = 0; index < parsedSkills.Length; index++)
                        tailValid &= TryParseByte(skillEditors[index].Text, out parsedSkills[index]);
                }
                int parsedKillAll = ship.KillAllShips, parsedKillPirates = ship.KillPirates;
                int parsedKillDominators = ship.KillDominators, parsedLiberationSystems = ship.LiberationSystems;
                int parsedKillPacifics = ship.KillPacifics, parsedKillWarriors = ship.KillWarriors;
                int parsedKillRangers = ship.KillRangers, parsedLiberationKills = ship.LiberationKills;
                int parsedTurnMoneyGoods = ship.TurnPlayerMoneyGoods;
                ushort parsedCurrentDominators = ship.KillInCurrentSystemDominators;
                ushort parsedCurrentPirates = ship.KillInCurrentSystemPirates;
                ushort parsedCurrentNormals = ship.KillInCurrentSystemNormals;
                ushort parsedCurrentCustom = ship.KillCustomInCurrentSystem;
                ushort parsedCoalitionRankPoints = ship.CoalitionRankPoints;
                uint parsedPirateRankPoints = ship.PirateRankPoints;
                ByteValueChoice selectedCoalitionRank = null, selectedPirateRank = null;
                UInt32ValueChoice selectedLiberationPlanet = null, selectedLastPlanet = null;
                bool normalValid = true;
                if (ship.HasNormalShipTail)
                {
                    selectedCoalitionRank = coalitionRankEditor.SelectedItem as ByteValueChoice;
                    selectedPirateRank = pirateRankEditor.SelectedItem as ByteValueChoice;
                    selectedLiberationPlanet = liberationPlanetEditor.SelectedItem as UInt32ValueChoice;
                    selectedLastPlanet = lastPlanetEditor.SelectedItem as UInt32ValueChoice;
                    normalValid = selectedCoalitionRank != null && selectedPirateRank != null &&
                        selectedLiberationPlanet != null && selectedLastPlanet != null &&
                        TryParseInt32(normalEditors[0].Text, out parsedKillAll) &&
                        TryParseInt32(normalEditors[1].Text, out parsedKillPirates) &&
                        TryParseInt32(normalEditors[2].Text, out parsedKillDominators) &&
                        TryParseInt32(normalEditors[3].Text, out parsedLiberationSystems) &&
                        TryParseInt32(normalEditors[4].Text, out parsedKillPacifics) &&
                        TryParseInt32(normalEditors[5].Text, out parsedKillWarriors) &&
                        TryParseInt32(normalEditors[6].Text, out parsedKillRangers) &&
                        TryParseUInt16(normalEditors[7].Text, out parsedCurrentDominators) &&
                        TryParseUInt16(normalEditors[8].Text, out parsedCurrentPirates) &&
                        TryParseUInt16(normalEditors[9].Text, out parsedCurrentNormals) &&
                        TryParseUInt16(normalEditors[10].Text, out parsedCurrentCustom) &&
                        TryParseInt32(normalEditors[11].Text, out parsedLiberationKills) &&
                        TryParseUInt16(normalEditors[12].Text, out parsedCoalitionRankPoints) &&
                        TryParseUInt32(normalEditors[13].Text, out parsedPirateRankPoints) &&
                        TryParseInt32(normalEditors[14].Text, out parsedTurnMoneyGoods);
                }
                int parsedRunProgramDate = ship.RunProgramDate;
                uint parsedPiratePrison = ship.PiratePrison;
                float parsedDesireConflict = ship.DesireConflict;
                ByteValueChoice selectedDominatorType = null, selectedDominatorSeries = null;
                ByteValueChoice selectedRunProgram = null, selectedTransportType = null;
                ByteValueChoice selectedWarriorType = null, selectedPirateType = null;
                bool simpleDerivedValid = true;
                if (ship.HasSimpleDerivedTail)
                {
                    if (ship.Type == 0)
                    {
                        selectedDominatorType = dominatorTypeEditor.SelectedItem as ByteValueChoice;
                        selectedDominatorSeries = dominatorSeriesEditor.SelectedItem as ByteValueChoice;
                        selectedRunProgram = runProgramEditor.SelectedItem as ByteValueChoice;
                        simpleDerivedValid = selectedDominatorType != null && selectedDominatorSeries != null &&
                            selectedRunProgram != null && TryParseInt32(runProgramDateEditor.Text, out parsedRunProgramDate);
                    }
                    else if (ship.Type == 2)
                    {
                        selectedTransportType = transportTypeEditor.SelectedItem as ByteValueChoice;
                        simpleDerivedValid = selectedTransportType != null;
                    }
                    else if (ship.Type == 3)
                    {
                        selectedPirateType = pirateTypeEditor.SelectedItem as ByteValueChoice;
                        simpleDerivedValid = selectedPirateType != null &&
                            TryParseUInt32(piratePrisonEditor.Text, out parsedPiratePrison) &&
                            TryParseFiniteFloat(desireConflictEditor.Text, out parsedDesireConflict);
                    }
                    else if (ship.Type == 4)
                    {
                        selectedWarriorType = warriorTypeEditor.SelectedItem as ByteValueChoice;
                        simpleDerivedValid = selectedWarriorType != null;
                    }
                }
                byte[] parsedRangerValues = { ship.RangerStatusTrader, ship.RangerStatusPirate,
                    ship.RangerStatusWarrior, ship.EminentPointsTrader, ship.EminentPointsPirate,
                    ship.EminentPointsWarrior, ship.Courageous, ship.StatusChangeWarrior,
                    ship.StatusChangePirate, ship.StatusChangeTrader };
                uint parsedRangerPrison = ship.RangerPrison;
                int parsedNods = ship.Nods;
                int[] parsedPrograms = (int[])ship.ProgramCounts.Clone();
                ByteValueChoice selectedRangerMoral = null;
                UInt32ValueChoice selectedLastShip = null;
                bool rangerValid = true;
                if (ship.HasRangerTail)
                {
                    selectedRangerMoral = rangerMoralEditor.SelectedItem as ByteValueChoice;
                    selectedLastShip = lastShipEditor.SelectedItem as UInt32ValueChoice;
                    rangerValid = selectedRangerMoral != null && selectedLastShip != null &&
                        TryParseUInt32(rangerEditors[10].Text, out parsedRangerPrison) &&
                        TryParseInt32(rangerEditors[11].Text, out parsedNods) && programsEditor.Rows.Count >= 12;
                    for (int index = 0; index < parsedRangerValues.Length; index++)
                        rangerValid &= TryParseByte(rangerEditors[index].Text, out parsedRangerValues[index]);
                    for (int index = 0; index < parsedPrograms.Length; index++)
                        rangerValid &= TryParseInt32(Convert.ToString(programsEditor.Rows[index].Cells[1].Value,
                            CultureInfo.InvariantCulture), out parsedPrograms[index]);
                    rangerValid &= parsedRangerValues[6] <= 100;
                }
                int parsedPlayerKillHyper = ship.KillShipInHyperSpace;
                int parsedPlayerKillHole = ship.KillShipInHole;
                int[] parsedPlayerDominatorKills = (int[])ship.KillDominatorsByType.Clone();
                byte[] parsedPlayerChameleon = (byte[])ship.ChameleonLogic.Clone();
                bool playerValid = true;
                if (ship.HasPlayerPrefix)
                {
                    playerValid = TryParseInt32(playerKillHyperEditor.Text, out parsedPlayerKillHyper) &&
                        TryParseInt32(playerKillHoleEditor.Text, out parsedPlayerKillHole) &&
                        playerDominatorKillsEditor.Rows.Count >= 8;
                    for (int index = 0; index < parsedPlayerDominatorKills.Length; index++)
                        playerValid &= TryParseInt32(Convert.ToString(
                            playerDominatorKillsEditor.Rows[index].Cells[1].Value,
                            CultureInfo.InvariantCulture), out parsedPlayerDominatorKills[index]);
                    for (int index = 0; index < parsedPlayerChameleon.Length; index++)
                        playerValid &= TryParseByte(playerChameleonEditors[index].Text,
                            out parsedPlayerChameleon[index]);
                }
                int[] parsedPlayerScalars = new int[28];
                int[] parsedPlayerInvestments = (int[])ship.PlayerInvestments.Clone();
                int[] parsedPlayerWarBasePrograms = (int[])ship.PlayerProgramsInWarBase.Clone();
                float parsedPlayerDepositPercent = ship.PlayerDepositPercent;
                UInt32ValueChoice selectedPlayerFlyToStar = null;
                UInt32ValueChoice selectedPlayerBridgeShip = null, selectedPlayerBridgePlanet = null;
                string parsedPlayerBridgeBackground = ship.PlayerBridgeBackground ?? string.Empty;
                bool playerFinancialValid = true;
                if (ship.HasPlayerFinancialTail)
                {
                    selectedPlayerFlyToStar = playerFlyToStarEditor.SelectedItem as UInt32ValueChoice;
                    playerFinancialValid = selectedPlayerFlyToStar != null &&
                        TryParseFiniteFloat(playerDepositPercentEditor.Text,
                            out parsedPlayerDepositPercent) && parsedPlayerDepositPercent >= 0.0F &&
                        parsedPlayerDepositPercent <= 1000.0F &&
                        playerInvestmentEditor.Rows.Count >= 12 &&
                        playerWarBaseProgramsEditor.Rows.Count >= 12;
                    for (int index = 0; index < parsedPlayerScalars.Length; index++)
                        playerFinancialValid &= TryParseInt32(playerScalarEditors[index].Text,
                            out parsedPlayerScalars[index]);
                    for (int index = 0; index < 12; index++)
                    {
                        playerFinancialValid &= TryParseInt32(Convert.ToString(
                            playerInvestmentEditor.Rows[index].Cells[1].Value,
                            CultureInfo.InvariantCulture), out parsedPlayerInvestments[index]);
                        playerFinancialValid &= TryParseInt32(Convert.ToString(
                            playerWarBaseProgramsEditor.Rows[index].Cells[1].Value,
                            CultureInfo.InvariantCulture), out parsedPlayerWarBasePrograms[index]);
                    }
                    int[] byteIndices = { 10, 21, 22, 27 };
                    foreach (int index in byteIndices)
                        playerFinancialValid &= parsedPlayerScalars[index] >= 0 &&
                            parsedPlayerScalars[index] <= byte.MaxValue;
                    int[] wordIndices = { 15, 16, 17, 19, 20 };
                    foreach (int index in wordIndices)
                        playerFinancialValid &= parsedPlayerScalars[index] >= 0 &&
                            parsedPlayerScalars[index] <= ushort.MaxValue;
                    if (ship.HasPlayerBridge)
                    {
                        selectedPlayerBridgeShip = playerBridgeShipEditor.SelectedItem as UInt32ValueChoice;
                        selectedPlayerBridgePlanet = playerBridgePlanetEditor.SelectedItem as UInt32ValueChoice;
                        parsedPlayerBridgeBackground = (playerBridgeBackgroundEditor.Text ?? string.Empty).Trim();
                        playerFinancialValid &= selectedPlayerBridgeShip != null &&
                            selectedPlayerBridgePlanet != null &&
                            parsedPlayerBridgeBackground.Length <= 512;
                    }
                }
                int parsedTranclucatorArtSize = ship.TranclucatorArtSize;
                string parsedTranclucatorArtName = ship.TranclucatorArtSystemName ?? string.Empty;
                UInt32ValueChoice selectedTranclucatorProprietor = null;
                bool tranclucatorValid = true;
                if (ship.HasTranclucatorTail)
                {
                    selectedTranclucatorProprietor = tranclucatorProprietorEditor.SelectedItem as UInt32ValueChoice;
                    parsedTranclucatorArtName = (tranclucatorArtNameEditor.Text ?? string.Empty).Trim();
                    tranclucatorValid = selectedTranclucatorProprietor != null &&
                        parsedTranclucatorArtName.Length <= 512 &&
                        TryParseInt32(tranclucatorArtSizeEditor.Text, out parsedTranclucatorArtSize) &&
                        tranclucatorSeekPermitEditor.Items.Count == 7;
                }
                int[,] parsedRuinsShop = (int[,])ship.RuinsShopGoods.Clone();
                int parsedRuinsEnergy = ship.RuinsEnergy, parsedRuinsFlyDate = ship.RuinsFlyDate;
                byte parsedRuinsNoShop = ship.RuinsNoShopUpdate;
                UInt32ValueChoice selectedRuinsStar = null;
                bool ruinsValid = true;
                if (ship.HasRuinsTail)
                {
                    selectedRuinsStar = ruinsFlyToStarEditor.SelectedItem as UInt32ValueChoice;
                    ruinsValid = selectedRuinsStar != null &&
                        TryParseInt32(ruinsEnergyEditor.Text, out parsedRuinsEnergy) &&
                        TryParseInt32(ruinsFlyDateEditor.Text, out parsedRuinsFlyDate) &&
                        TryParseByte(ruinsNoShopEditor.Text, out parsedRuinsNoShop);
                    for (int good = 0; good < 8; good++)
                        for (int field = 0; field < 3; field++)
                            ruinsValid &= TryParseInt32(ruinsShopEditors[good, field].Text,
                                out parsedRuinsShop[good, field]);
                }
                if (updatedName.Length == 0 || updatedName.Length > 80 ||
                    updatedCustomTypeName.Length > 128 ||
                    !TryParseCoordinate(x.Text, out parsedX) || !TryParseCoordinate(y.Text, out parsedY) ||
                    !TryParseUInt32(money.Text, out parsedMoney) || !TryParseUInt32(rnd.Text, out parsedRnd) ||
                    !TryParseUInt32(rndOut.Text, out parsedRndOut) || !TryParseUInt32(day.Text, out parsedDay) ||
                    !TryParseInt32(face.Text, out parsedFace) || !goodsValid || !tailValid || !normalValid ||
                    !simpleDerivedValid || !rangerValid || !playerValid || !playerFinancialValid ||
                    !tranclucatorValid || !ruinsValid ||
                    selectedOwner == null || selectedPilotRace == null)
                {
                    MessageBox.Show(this, "Поля TShip не применены: проверьте имя, координаты, товары и числовые параметры.",
                        "TShip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ship.Name = updatedName; ship.ScriptName = updatedCustomTypeName;
                ship.X = parsedX; ship.Y = parsedY; ship.Owner = selectedOwner.Value;
                ship.PilotRace = selectedPilotRace.Value; ship.Money = parsedMoney;
                ship.Rnd = parsedRnd; ship.RndOut = parsedRndOut; ship.Day = parsedDay; ship.Face = parsedFace;
                ship.Goods = parsedGoods;
                if (ship.HasCommonTail)
                {
                    ship.Forsage = forsageEditor.Checked; ship.Angle = parsedAngle;
                    ship.OrderType = selectedOrderType.Value; ship.OrderData = parsedOrderData;
                    ship.OrderObjectId = selectedOrderObject == null ? 0U : selectedOrderObject.Value;
                    ship.OrderDestinationX = parsedOrderX; ship.OrderDestinationY = parsedOrderY;
                    ship.OrderAbsolute = orderAbsoluteEditor.Checked; ship.Abducted = abductedEditor.Checked;
                    ship.DaysLanded = parsedDaysLanded; ship.ScriptOrderAbsolute = parsedScriptOrder;
                    ship.GraphDominator = graphDominatorEditor.Checked; ship.GraphName = updatedGraphName;
                    ship.GraphShipTransparency = parsedGraphTransparency; ship.InHyperSpace = inHyperEditor.Checked;
                    ship.RadiusStop = parsedRadiusStop; ship.ShipDestroy = shipDestroyEditor.Checked;
                    ship.Skills = parsedSkills; ship.Protoplasm = parsedProtoplasm; ship.Points = parsedPoints;
                    ship.FreePoints = parsedFreePoints; ship.DayWithoutPlayer = parsedDayWithoutPlayer;
                    ship.GroupOrder = parsedGroupOrder; ship.LastNextDay = parsedLastNextDay;
                    ship.ChameleonEnabled = chameleonEnabledEditor.Checked;
                    ship.ChameleonSeries = selectedChameleonSeries.Value;
                    ship.BlazerChameleonDetect = blazerDetectEditor.Checked ? (byte)1 : (byte)0;
                    ship.KellerChameleonDetect = kellerDetectEditor.Checked ? (byte)1 : (byte)0;
                    ship.TerronChameleonDetect = terronDetectEditor.Checked ? (byte)1 : (byte)0;
                    ship.BlazerChameleonCharge = parsedBlazerCharge;
                    ship.KellerChameleonCharge = parsedKellerCharge;
                    ship.TerronChameleonCharge = parsedTerronCharge;
                    ship.TechLevelKnowledge = parsedTechKnowledge; ship.TradePenalty = parsedTradePenalty;
                    ship.TradePoints = parsedTradePoints; ship.ContrabandPoints = parsedContraband;
                    ship.RewardViewCount = parsedRewardView; ship.NoDrop = noDropEditor.Checked;
                    ship.NoTarget = selectedNoTarget.Value; ship.NoTalk = noTalkEditor.Checked;
                    ship.NoScan = noScanEditor.Checked; ship.ScriptChameleon = scriptChameleonEditor.Checked;
                    ship.RobbedByPlayer = robbedEditor.Checked; ship.CountOfDeflectedPlayerShots = parsedDeflected;
                    ship.Swarmed = parsedSwarmed; ship.SwarmedByShipId = selectedSwarmedByShip.Value;
                    ship.SwarmAnimation = parsedSwarmed > 0 ? updatedSwarmAnimation : string.Empty;
                    ship.CurrentStanding = selectedStanding.Value; ship.AverageSpeed = parsedAverageSpeed;
                    ship.AverageEnemySpeed = parsedAverageEnemySpeed; ship.AverageEquipmentValue = parsedAverageEquipment;
                    ship.AverageCapital = parsedAverageCapital; ship.AverageMoneyToCapital = parsedAverageMoney;
                    ship.AverageFreeSpaceRatio = parsedAverageFreeSpace;
                    ship.RatioOfTooCostlyEquipmentInShop = parsedCostlyRatio;
                }
                if (ship.HasNormalShipTail)
                {
                    ship.KillAllShips = parsedKillAll; ship.KillPirates = parsedKillPirates;
                    ship.KillDominators = parsedKillDominators; ship.LiberationSystems = parsedLiberationSystems;
                    ship.KillPacifics = parsedKillPacifics; ship.KillWarriors = parsedKillWarriors;
                    ship.KillRangers = parsedKillRangers;
                    ship.KillInCurrentSystemDominators = parsedCurrentDominators;
                    ship.KillInCurrentSystemPirates = parsedCurrentPirates;
                    ship.KillInCurrentSystemNormals = parsedCurrentNormals;
                    ship.KillCustomInCurrentSystem = parsedCurrentCustom;
                    ship.LiberationPlanetId = selectedLiberationPlanet.Value;
                    ship.LiberationKills = parsedLiberationKills;
                    ship.CoalitionRank = selectedCoalitionRank.Value;
                    ship.CoalitionRankPoints = parsedCoalitionRankPoints;
                    ship.PirateRank = selectedPirateRank.Value; ship.PirateRankPoints = parsedPirateRankPoints;
                    ship.LastPlanetId = selectedLastPlanet.Value; ship.TurnPlayerMoneyGoods = parsedTurnMoneyGoods;
                }
                if (ship.HasSimpleDerivedTail)
                {
                    if (ship.Type == 0)
                    {
                        ship.DominatorType = selectedDominatorType.Value;
                        ship.DominatorSeries = selectedDominatorSeries.Value;
                        ship.RunProgramDate = parsedRunProgramDate; ship.RunProgramName = selectedRunProgram.Value;
                    }
                    else if (ship.Type == 2) ship.TransportType = selectedTransportType.Value;
                    else if (ship.Type == 3)
                    {
                        ship.PiratePrison = parsedPiratePrison; ship.PirateType = selectedPirateType.Value;
                        ship.DesireConflict = parsedDesireConflict;
                    }
                    else if (ship.Type == 4) ship.WarriorType = selectedWarriorType.Value;
                }
                if (ship.HasRangerTail)
                {
                    ship.RangerStatusTrader = parsedRangerValues[0];
                    ship.RangerStatusPirate = parsedRangerValues[1];
                    ship.RangerStatusWarrior = parsedRangerValues[2];
                    ship.EminentPointsTrader = parsedRangerValues[3];
                    ship.EminentPointsPirate = parsedRangerValues[4];
                    ship.EminentPointsWarrior = parsedRangerValues[5];
                    ship.Courageous = parsedRangerValues[6];
                    ship.StatusChangeWarrior = parsedRangerValues[7];
                    ship.StatusChangePirate = parsedRangerValues[8];
                    ship.StatusChangeTrader = parsedRangerValues[9];
                    ship.RangerMoral = selectedRangerMoral.Value; ship.RangerPrison = parsedRangerPrison;
                    ship.LastShipId = selectedLastShip.Value; ship.Nods = parsedNods;
                    ship.ProgramCounts = parsedPrograms; ship.ExcludedFromRating = excludedFromRatingEditor.Checked;
                }
                if (ship.HasPlayerPrefix)
                {
                    ship.PlayerPrison = playerPrisonEditor.Checked;
                    ship.PlayerTalkLocked = playerTalkLockedEditor.Checked;
                    ship.PlayerScanLocked = playerScanLockedEditor.Checked;
                    ship.KillShipInHyperSpace = parsedPlayerKillHyper;
                    ship.KillShipInHole = parsedPlayerKillHole;
                    ship.KillDominatorsByType = parsedPlayerDominatorKills;
                    ship.ChameleonLogic = parsedPlayerChameleon;
                }
                if (ship.HasPlayerFinancialTail)
                {
                    ship.PlayerDebt = parsedPlayerScalars[0];
                    ship.PlayerDebtDate = parsedPlayerScalars[1];
                    ship.PlayerDebtCount = parsedPlayerScalars[2];
                    ship.PlayerDeposit = parsedPlayerScalars[3];
                    ship.PlayerDepositDate = parsedPlayerScalars[4];
                    ship.PlayerDepositDay = parsedPlayerScalars[5];
                    ship.PlayerMedPolicy = parsedPlayerScalars[6];
                    ship.PlayerPirateLicense = parsedPlayerScalars[7];
                    ship.PlayerPiratePoints = parsedPlayerScalars[8];
                    ship.PlayerPirateNewPoints = parsedPlayerScalars[9];
                    ship.PlayerImmunity = (byte)parsedPlayerScalars[10];
                    ship.PlayerDayWarBaseGivePrograms = parsedPlayerScalars[11];
                    ship.PlayerHitEnemyAfterPrograms = parsedPlayerScalars[12];
                    ship.PlayerPlanetBattlesWin = parsedPlayerScalars[13];
                    ship.PlayerLastPlanetBattleDate = parsedPlayerScalars[14];
                    ship.PlayerIllnessCount = (ushort)parsedPlayerScalars[15];
                    ship.PlayerStimulatorCount = (ushort)parsedPlayerScalars[16];
                    ship.PlayerPrisonCount = (ushort)parsedPlayerScalars[17];
                    ship.PlayerUnknownPlanetComplete = parsedPlayerScalars[18];
                    ship.PlayerChangeRaceCount = (ushort)parsedPlayerScalars[19];
                    ship.PlayerChangeSideCount = (ushort)parsedPlayerScalars[20];
                    ship.PlayerHotEquipmentCurrent = (byte)parsedPlayerScalars[21];
                    ship.PlayerGoToGovernment = (byte)parsedPlayerScalars[22];
                    ship.PlayerExperienceDominatorKills = parsedPlayerScalars[23];
                    ship.PlayerExperiencePirateKills = parsedPlayerScalars[24];
                    ship.PlayerExperienceGoodShipKills = parsedPlayerScalars[25];
                    ship.PlayerExperienceTrade = parsedPlayerScalars[26];
                    ship.PlayerCaptainOnBridge = (byte)parsedPlayerScalars[27];
                    ship.PlayerDepositPercent = parsedPlayerDepositPercent;
                    ship.PlayerFlyToStarId = selectedPlayerFlyToStar.Value;
                    ship.PlayerInvestments = parsedPlayerInvestments;
                    ship.PlayerProgramsInWarBase = parsedPlayerWarBasePrograms;
                    ship.PlayerNoJump = playerNoJumpEditor.Checked;
                    ship.PlayerPirateClanReal = playerPirateClanEditor.Checked;
                    if (ship.HasPlayerBridge)
                    {
                        ship.PlayerBridgeCurrentShipId = selectedPlayerBridgeShip.Value;
                        ship.PlayerBridgeCurrentPlanetId = selectedPlayerBridgePlanet.Value;
                        ship.PlayerBridgeBackground = parsedPlayerBridgeBackground;
                    }
                }
                if (ship.HasTranclucatorTail)
                {
                    ship.TranclucatorProprietorShipId = selectedTranclucatorProprietor.Value;
                    ship.TranclucatorDocking = tranclucatorDockingEditor.Checked;
                    ship.TranclucatorSeekItems = tranclucatorSeekItemsEditor.Checked;
                    ship.TranclucatorAutoArrange = tranclucatorAutoArrangeEditor.Checked;
                    ship.TranclucatorArtSize = parsedTranclucatorArtSize;
                    ship.TranclucatorArtSystemName = parsedTranclucatorArtName;
                    for (int index = 0; index < 7; index++)
                        ship.TranclucatorSeekPermits[index] = tranclucatorSeekPermitEditor.GetItemChecked(index);
                    for (int index = 0; index < 2; index++)
                        ship.TranclucatorLandPermits[index] = tranclucatorLandPermitEditors[index].Checked;
                    ship.TranclucatorLandStorage = tranclucatorLandStorageEditor.Checked;
                }
                if (ship.HasRuinsTail)
                {
                    ship.RuinsShopGoods = parsedRuinsShop;
                    ship.RuinsEnergy = parsedRuinsEnergy; ship.RuinsFlyToStarId = selectedRuinsStar.Value;
                    ship.RuinsFlyDate = parsedRuinsFlyDate; ship.RuinsSponsor = ruinsSponsorEditor.Checked;
                    ship.RuinsSpecialShip = ruinsSpecialShipEditor.Checked;
                    ship.RuinsNoLanding = ruinsNoLandingEditor.Checked;
                    ship.RuinsNoShopUpdate = parsedRuinsNoShop;
                }
                RefreshGalaxyView();
                RefreshObjectLists();
            }
        }

        private void BindShipPreCommonEditors(Form form, ShipHeaderRecord ship)
        {
            if (!ship.HasPreCommonCollections) return;

            ComboBox badShip = FindControl<ComboBox>(form, "cbShipBad");
            ComboBox goodShip = FindControl<ComboBox>(form, "cbShipGood");
            ComboBox partnerShip = FindControl<ComboBox>(form, "cbShipPartner");
            TextBox partnerDays = BindEditableText(form, "edShipPartnerDay",
                ship.PartnerGood.ToString(CultureInfo.InvariantCulture));
            PopulateShipReferenceCombo(badShip, ship.BadShipId);
            PopulateShipReferenceCombo(goodShip, ship.GoodShipId);
            PopulateShipReferenceCombo(partnerShip, ship.PartnerShipId);
            form.FormClosing += delegate(object sender, FormClosingEventArgs args)
            {
                int parsedPartnerDays;
                UInt32ValueChoice selectedBad = badShip.SelectedItem as UInt32ValueChoice;
                UInt32ValueChoice selectedGood = goodShip.SelectedItem as UInt32ValueChoice;
                UInt32ValueChoice selectedPartner = partnerShip.SelectedItem as UInt32ValueChoice;
                if (selectedBad == null || selectedGood == null || selectedPartner == null ||
                    !TryParseInt32(partnerDays.Text, out parsedPartnerDays))
                {
                    args.Cancel = true;
                    MessageBox.Show(form, "Проверьте текущего врага, друга, нанимателя и срок контракта.",
                        "TShip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ship.BadShipId = selectedBad.Value; ship.GoodShipId = selectedGood.Value;
                ship.PartnerShipId = selectedPartner.Value;
                ship.PartnerGood = selectedPartner.Value == 0 ? 0 : parsedPartnerDays;
            };

            BindShipItemCollection(FindControl<ListBox>(form, "lbEquipments"), ship,
                ship.EquipmentItems, true);
            BindShipItemCollection(FindControl<ListBox>(form, "lbArtefacts"), ship,
                ship.ArtefactItems, false);
            BindShipItemCollection(FindControl<ListBox>(form, "lbDropList"), ship,
                ship.DropListItems, false);
            BindSpecialBonusCollection(FindControl<ListBox>(form, "lbSpecialBonuses"), ship.SpecialBonuses, form);
            BindStatusEffectCollection(FindControl<ListBox>(form, "lbStatusEffects"), ship.StatusEffects, form);
            BindCustomShipInfoCollection(FindControl<ListBox>(form, "lbCustomShipInfos"), ship.CustomShipInfos, form);
            BindShipRelationCollection(FindControl<ListBox>(form, "lbRelationToRangers"), ship, form);
            BindShipReferencedItemCollection(FindControl<ListBox>(form, "lbTakeItems"),
                FindControl<GroupBox>(form, "gbTakeItems"), ship.TakeItemReferenceIds, form,
                "Предметы для подбора");
            BindShipReferencedItemCollection(FindControl<ListBox>(form, "lbRecentlyDroppedItems"),
                FindControl<GroupBox>(form, "gbRecentlyDroppedItems"), ship.RecentlyDroppedItemIds, form,
                "Недавно выброшенные предметы");
        }

        private void BindRangerQuestCollection(ListBox list, ShipHeaderRecord ship, IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (RangerQuestRecord record in ship.RangerQuests) list.Items.Add(record);
            };
            EventHandler edit = delegate
            {
                RangerQuestRecord record = list.SelectedItem as RangerQuestRecord;
                if (record == null || !ship.RangerQuests.Contains(record)) return;
                int selectedIndex = list.SelectedIndex;
                if (EditRangerQuest(record, owner))
                {
                    refresh();
                    if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                        list.SelectedIndex = selectedIndex;
                }
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            menu.Items.Add(editItem); menu.Items.Add(deleteItem); list.ContextMenuStrip = menu;
            editItem.Click += edit;
            deleteItem.Click += delegate
            {
                RangerQuestRecord record = list.SelectedItem as RangerQuestRecord;
                if (record == null || !ship.RangerQuests.Remove(record)) return;
                ship.RangerQuestCount = checked((ushort)ship.RangerQuests.Count);
                refresh();
            };
        }

        private void BindPlayerJournalCollection(ListBox list, ShipHeaderRecord ship,
            IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (PlayerJournalRecord record in ship.PlayerJournalRecords)
                    list.Items.Add(record);
            };
            EventHandler edit = delegate
            {
                PlayerJournalRecord record = list.SelectedItem as PlayerJournalRecord;
                if (record == null || !ship.PlayerJournalRecords.Contains(record)) return;
                int selectedIndex = list.SelectedIndex;
                if (EditPlayerJournalRecord(record, owner))
                {
                    refresh();
                    if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                        list.SelectedIndex = selectedIndex;
                }
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            menu.Items.Add(editItem); menu.Items.Add(deleteItem); list.ContextMenuStrip = menu;
            editItem.Click += edit;
            deleteItem.Click += delegate
            {
                PlayerJournalRecord record = list.SelectedItem as PlayerJournalRecord;
                if (record == null || !ship.PlayerJournalRecords.Remove(record)) return;
                refresh();
            };
        }

        private void BindPlayerRobotMapCollection(ListBox list, ShipHeaderRecord ship,
            IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (PlayerRobotMapRecord record in ship.PlayerRobotMaps)
                    list.Items.Add(record);
            };
            EventHandler edit = delegate
            {
                PlayerRobotMapRecord record = list.SelectedItem as PlayerRobotMapRecord;
                if (record == null || !ship.PlayerRobotMaps.Contains(record)) return;
                int selectedIndex = list.SelectedIndex;
                if (EditPlayerRobotMap(record, owner))
                {
                    refresh();
                    if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                        list.SelectedIndex = selectedIndex;
                }
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            menu.Items.Add(editItem); menu.Items.Add(deleteItem); list.ContextMenuStrip = menu;
            editItem.Click += edit;
            deleteItem.Click += delegate
            {
                PlayerRobotMapRecord record = list.SelectedItem as PlayerRobotMapRecord;
                if (record == null || !ship.PlayerRobotMaps.Remove(record)) return;
                ship.PlayerRobotMapCount = ship.PlayerRobotMaps.Count;
                refresh();
            };
        }

        private void BindPlayerNewsCollection(ListBox list, ShipHeaderRecord ship,
            IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (GalaxyNewsRecord record in ship.PlayerNewsRecords) list.Items.Add(record);
            };
            EventHandler edit = delegate
            {
                GalaxyNewsRecord record = list.SelectedItem as GalaxyNewsRecord;
                if (record == null || !ship.PlayerNewsRecords.Contains(record)) return;
                int selectedIndex = list.SelectedIndex;
                EditGalaxyNews(record, owner);
                refresh();
                if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                    list.SelectedIndex = selectedIndex;
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            menu.Items.Add(editItem); menu.Items.Add(deleteItem); list.ContextMenuStrip = menu;
            editItem.Click += edit;
            deleteItem.Click += delegate
            {
                GalaxyNewsRecord record = list.SelectedItem as GalaxyNewsRecord;
                if (record == null || !ship.PlayerNewsRecords.Remove(record)) return;
                refresh();
            };
        }

        private void BindShipIllnessCollection(ListBox list,
            List<ShipIllnessRecord> records, IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (ShipIllnessRecord record in records) list.Items.Add(record);
            };
            EventHandler edit = delegate
            {
                ShipIllnessRecord record = list.SelectedItem as ShipIllnessRecord;
                if (record == null || !records.Contains(record)) return;
                int selectedIndex = list.SelectedIndex;
                if (EditShipIllness(record, owner))
                {
                    refresh();
                    if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                        list.SelectedIndex = selectedIndex;
                }
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            menu.Items.Add(editItem);
            list.ContextMenuStrip = menu;
            editItem.Click += edit;
        }

        private void BindShipRewardCollection(ListBox list, List<byte> records,
            IWin32Window owner)
        {
            Action refresh = delegate
            {
                list.Items.Clear();
                foreach (byte reward in records)
                    list.Items.Add(new ByteValueChoice(reward, RewardDisplayName(reward)));
            };
            EventHandler edit = delegate
            {
                int selectedIndex = list.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= records.Count) return;
                byte updated;
                if (!EditShipReward(records[selectedIndex], owner, out updated)) return;
                records[selectedIndex] = updated;
                refresh();
                if (selectedIndex < list.Items.Count) list.SelectedIndex = selectedIndex;
            };
            refresh();
            list.DoubleClick += edit;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem addItem = new ToolStripMenuItem("Добавить");
            ToolStripMenuItem editItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            menu.Items.Add(addItem); menu.Items.Add(editItem); menu.Items.Add(deleteItem);
            list.ContextMenuStrip = menu;
            addItem.Click += delegate
            {
                records.Add(0);
                refresh();
                list.SelectedIndex = list.Items.Count - 1;
                edit(null, EventArgs.Empty);
            };
            editItem.Click += edit;
            deleteItem.Click += delegate
            {
                int selectedIndex = list.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= records.Count) return;
                records.RemoveAt(selectedIndex);
                refresh();
                if (list.Items.Count != 0)
                    list.SelectedIndex = Math.Min(selectedIndex, list.Items.Count - 1);
            };
        }

        private void BindShipItemCollection(ListBox list, ShipHeaderRecord ship,
            List<ShipItemListEntry> records, bool equipment, Action<int> countChanged = null)
        {
            PopulateShipItemCollection(list, records);
            list.DoubleClick += delegate { EditSelectedShipItem(list, records); };
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem edit = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem delete = new ToolStripMenuItem("Удалить");
            menu.Items.Add(edit); menu.Items.Add(delete); list.ContextMenuStrip = menu;
            edit.Click += delegate { EditSelectedShipItem(list, records); };
            delete.Click += delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                ShipItemListEntry entry = selected == null ? null : selected.Value as ShipItemListEntry;
                if (entry == null) return;
                records.Remove(entry); pendingDeletedItemStarts.Add(entry.ItemStart);
                if (equipment) ship.EquipmentItemCount = checked((ushort)records.Count);
                if (countChanged != null) countChanged(records.Count);
                PopulateShipItemCollection(list, records);
                RefreshObjectLists();
            };
        }

        private void BindRuinsSaleSatellite(ListBox list, ShipHeaderRecord ship, Form owner)
        {
            List<ShipItemListEntry> records = new List<ShipItemListEntry>();
            if (ship.RuinsSaleSatellite != null) records.Add(ship.RuinsSaleSatellite);
            PopulateShipItemCollection(list, records);
            Action edit = delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                ShipItemListEntry entry = selected == null ? null : selected.Value as ShipItemListEntry;
                if (entry == null) return;
                ItemHeaderRecord item = FindItemByStart(entry.ItemStart);
                if (item == null) return;
                EditItem(item, owner); PopulateShipItemCollection(list, records);
            };
            list.DoubleClick += delegate { edit(); };
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Редактировать", null, delegate { edit(); });
            list.ContextMenuStrip = menu;
        }

        private void PopulateShipItemCollection(ListBox list, List<ShipItemListEntry> records)
        {
            list.Items.Clear();
            foreach (ShipItemListEntry entry in records)
            {
                ItemHeaderRecord item = FindItemByStart(entry.ItemStart);
                string caption = item == null ? MissingItemCaption(entry.ItemType, entry.ItemObjectId) :
                    ItemDisplayCaption(item, null);
                list.Items.Add(new SearchResultEntry(entry, caption));
            }
        }

        private void EditSelectedShipItem(ListBox list, List<ShipItemListEntry> records)
        {
            SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
            ShipItemListEntry entry = selected == null ? null : selected.Value as ShipItemListEntry;
            if (entry == null || !records.Contains(entry)) return;
            ItemHeaderRecord item = FindItemByStart(entry.ItemStart);
            if (item == null) return;
            EditItem(item); PopulateShipItemCollection(list, records);
        }

        private void BindSpecialBonusCollection(ListBox list, List<ShipSpecialBonusRecord> records, Form owner)
        {
            PopulateSpecialBonusCollection(list, records);
            list.DoubleClick += delegate { EditSelectedSpecialBonus(list, records, owner); };
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem add = new ToolStripMenuItem("Добавить");
            ToolStripMenuItem edit = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem delete = new ToolStripMenuItem("Удалить");
            menu.Items.Add(add); menu.Items.Add(edit); menu.Items.Add(delete); list.ContextMenuStrip = menu;
            add.Click += delegate
            {
                ShipSpecialBonusRecord value = new ShipSpecialBonusRecord();
                if (EditSpecialBonus(value, owner)) { records.Add(value); PopulateSpecialBonusCollection(list, records); }
            };
            edit.Click += delegate { EditSelectedSpecialBonus(list, records, owner); };
            delete.Click += delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                ShipSpecialBonusRecord value = selected == null ? null : selected.Value as ShipSpecialBonusRecord;
                if (value != null) { records.Remove(value); PopulateSpecialBonusCollection(list, records); }
            };
        }

        private static void PopulateSpecialBonusCollection(ListBox list, List<ShipSpecialBonusRecord> records)
        {
            list.Items.Clear();
            foreach (ShipSpecialBonusRecord value in records)
            {
                string typeName = value.BonusType < specialBonusTypeNames.Length
                    ? specialBonusTypeNames[value.BonusType]
                    : "Тип " + value.BonusType.ToString(CultureInfo.InvariantCulture);
                list.Items.Add(new SearchResultEntry(value, typeName + ": " +
                    value.Value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private void EditSelectedSpecialBonus(ListBox list, List<ShipSpecialBonusRecord> records, Form owner)
        {
            SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
            ShipSpecialBonusRecord value = selected == null ? null : selected.Value as ShipSpecialBonusRecord;
            if (value != null && records.Contains(value) && EditSpecialBonus(value, owner))
                PopulateSpecialBonusCollection(list, records);
        }

        private bool EditSpecialBonus(ShipSpecialBonusRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSPECIALBONUSFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox type = FindControl<ComboBox>(form, "cbBonusType");
                PopulateByteCombo(type, value.BonusType, specialBonusTypeNames);
                TextBox bonusValue = BindEditableText(form, "edBonusValue",
                    value.Value.ToString(CultureInfo.InvariantCulture));
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);
                int parsedValue; ByteValueChoice selected = type.SelectedItem as ByteValueChoice;
                if (selected == null || !TryParseInt32(bonusValue.Text, out parsedValue))
                {
                    MessageBox.Show(owner, "Значение особого бонуса не применено.", "TSpecialBonus",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return false;
                }
                value.BonusType = selected.Value; value.Value = parsedValue; return true;
            }
        }

        private void BindStatusEffectCollection(ListBox list, List<ShipStatusEffectRecord> records, Form owner)
        {
            PopulateStatusEffectCollection(list, records);
            list.DoubleClick += delegate { EditSelectedStatusEffect(list, records, owner); };
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem add = new ToolStripMenuItem("Добавить");
            ToolStripMenuItem edit = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem delete = new ToolStripMenuItem("Удалить");
            menu.Items.Add(add); menu.Items.Add(edit); menu.Items.Add(delete); list.ContextMenuStrip = menu;
            add.Click += delegate
            {
                ShipStatusEffectRecord value = new ShipStatusEffectRecord();
                if (EditStatusEffect(value, owner)) { records.Add(value); PopulateStatusEffectCollection(list, records); }
            };
            edit.Click += delegate { EditSelectedStatusEffect(list, records, owner); };
            delete.Click += delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                ShipStatusEffectRecord value = selected == null ? null : selected.Value as ShipStatusEffectRecord;
                if (value != null) { records.Remove(value); PopulateStatusEffectCollection(list, records); }
            };
        }

        private void PopulateStatusEffectCollection(ListBox list, List<ShipStatusEffectRecord> records)
        {
            list.Items.Clear();
            foreach (ShipStatusEffectRecord value in records)
            {
                string typeName = value.EffectType < statusEffectTypeNames.Length
                    ? statusEffectTypeNames[value.EffectType]
                    : "Тип " + value.EffectType.ToString(CultureInfo.InvariantCulture);
                list.Items.Add(new SearchResultEntry(value, typeName + ": " +
                    value.Value.ToString("R", CultureInfo.InvariantCulture) +
                    (value.LastSourceShipId == 0 ? string.Empty : " — " + ShipName(value.LastSourceShipId))));
            }
        }

        private void EditSelectedStatusEffect(ListBox list, List<ShipStatusEffectRecord> records, Form owner)
        {
            SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
            ShipStatusEffectRecord value = selected == null ? null : selected.Value as ShipStatusEffectRecord;
            if (value != null && records.Contains(value) && EditStatusEffect(value, owner))
                PopulateStatusEffectCollection(list, records);
        }

        private bool EditStatusEffect(ShipStatusEffectRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSTATUSEFFECTFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox type = FindControl<ComboBox>(form, "cbEffectType");
                PopulateByteCombo(type, value.EffectType, statusEffectTypeNames);
                TextBox strength = BindEditableText(form, "edEffectStrength",
                    value.Value.ToString("R", CultureInfo.InvariantCulture));
                TextBox sourceId = BindEditableText(form, "edEffectLastSourceShipId",
                    value.LastSourceShipId.ToString(CultureInfo.InvariantCulture));
                Label shipName = FindControl<Label>(form, "sLabelShipName");
                EventHandler updateName = delegate
                {
                    uint id; shipName.Text = TryParseUInt32(sourceId.Text, out id) && id != 0 ? ShipName(id) : "—";
                };
                sourceId.TextChanged += updateName; updateName(sourceId, EventArgs.Empty);
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);
                float parsedStrength; uint parsedSource;
                ByteValueChoice selected = type.SelectedItem as ByteValueChoice;
                if (selected == null || !TryParseFiniteFloat(strength.Text, out parsedStrength) ||
                    !TryParseUInt32(sourceId.Text, out parsedSource) || parsedSource > 10000000)
                {
                    MessageBox.Show(owner, "Поля статус-эффекта не применены.", "TStatusEffect",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return false;
                }
                value.EffectType = selected.Value; value.Value = parsedStrength;
                value.LastSourceShipId = parsedSource; return true;
            }
        }

        private void BindCustomShipInfoCollection(ListBox list, List<CustomShipInfoRecord> records, Form owner)
        {
            PopulateCustomShipInfoCollection(list, records);
            list.DoubleClick += delegate { EditSelectedCustomShipInfo(list, records, owner); };
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem edit = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem delete = new ToolStripMenuItem("Удалить");
            menu.Items.Add(edit); menu.Items.Add(delete); list.ContextMenuStrip = menu;
            edit.Click += delegate { EditSelectedCustomShipInfo(list, records, owner); };
            delete.Click += delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                CustomShipInfoRecord value = selected == null ? null : selected.Value as CustomShipInfoRecord;
                if (value != null) { records.Remove(value); PopulateCustomShipInfoCollection(list, records); }
            };
        }

        private static void PopulateCustomShipInfoCollection(ListBox list, List<CustomShipInfoRecord> records)
        {
            list.Items.Clear();
            foreach (CustomShipInfoRecord value in records)
                list.Items.Add(new SearchResultEntry(value,
                    string.IsNullOrEmpty(value.Name) ? "TCustomShipInfo" : value.Name));
        }

        private void EditSelectedCustomShipInfo(ListBox list, List<CustomShipInfoRecord> records, Form owner)
        {
            SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
            CustomShipInfoRecord value = selected == null ? null : selected.Value as CustomShipInfoRecord;
            if (value != null && records.Contains(value) && EditCustomShipInfo(value, owner))
                PopulateCustomShipInfoCollection(list, records);
        }

        private bool EditCustomShipInfo(CustomShipInfoRecord value, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TCUSTOMSHIPINFOFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox name = BindEditableText(form, "edInfoName", value.Name ?? string.Empty);
                TextBox description = BindEditableText(form, "mmDescription", value.Description ?? string.Empty);
                TextBox data1 = BindEditableText(form, "edInfoData1", value.Data1.ToString(CultureInfo.InvariantCulture));
                TextBox data2 = BindEditableText(form, "edInfoData2", value.Data2.ToString(CultureInfo.InvariantCulture));
                TextBox data3 = BindEditableText(form, "edInfoData3", value.Data3.ToString(CultureInfo.InvariantCulture));
                TextBox text1 = BindEditableText(form, "mmInfoTextData1", value.TextData1 ?? string.Empty);
                TextBox text2 = BindEditableText(form, "mmInfoTextData2", value.TextData2 ?? string.Empty);
                TextBox text3 = BindEditableText(form, "mmInfoTextData3", value.TextData3 ?? string.Empty);
                CheckBox hideTags = FindControl<CheckBox>(form, "chbHideTags");
                string rawDescription = value.Description ?? string.Empty;
                hideTags.Enabled = true; hideTags.Checked = true;
                description.ReadOnly = false;
                hideTags.CheckedChanged += delegate
                {
                    if (hideTags.Checked)
                    {
                        description.Text = rawDescription;
                        description.ReadOnly = false;
                    }
                    else
                    {
                        rawDescription = description.Text;
                        description.Text = FilterGameTextTags(rawDescription);
                        description.ReadOnly = true;
                    }
                };
                form.KeyDown += delegate(object sender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);
                int parsed1, parsed2, parsed3;
                if (!TryParseInt32(data1.Text, out parsed1) || !TryParseInt32(data2.Text, out parsed2) ||
                    !TryParseInt32(data3.Text, out parsed3) || name.Text.Length > 32768 ||
                    description.Text.Length > 32768 || text1.Text.Length > 32768 ||
                    text2.Text.Length > 32768 || text3.Text.Length > 32768)
                {
                    MessageBox.Show(owner, "Поля TCustomShipInfo не применены.", "TCustomShipInfo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); return false;
                }
                value.Name = name.Text;
                value.Description = hideTags.Checked ? description.Text : rawDescription;
                value.Data1 = parsed1; value.Data2 = parsed2; value.Data3 = parsed3;
                value.TextData1 = text1.Text; value.TextData2 = text2.Text; value.TextData3 = text3.Text;
                return true;
            }
        }

        private void BindShipRelationCollection(ListBox list, ShipHeaderRecord ship, Form owner)
        {
            List<ShipHeaderRecord> rangers = PlanetRelationRangers();
            Action refresh = delegate
            {
                int selectedIndex = list.SelectedIndex;
                list.BeginUpdate(); list.Items.Clear();
                if (ship.RelationToRangers != null)
                    for (int index = 0; index < ship.RelationToRangers.Length; index++)
                        list.Items.Add(new SearchResultEntry(index,
                            PlanetRelationRangerName(rangers, index) + ": " +
                            ship.RelationToRangers[index].ToString(CultureInfo.InvariantCulture)));
                list.EndUpdate();
                if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                    list.SelectedIndex = selectedIndex;
            };
            Action edit = delegate
            {
                SearchResultEntry selected = list.SelectedItem as SearchResultEntry;
                if (selected == null || !(selected.Value is int)) return;
                int index = (int)selected.Value;
                if (ship.RelationToRangers == null || index < 0 || index >= ship.RelationToRangers.Length)
                    return;
                using (Form form = EditorFormFactory.Build(
                    EditorFormDefinitions.Get("TRELATIONFORM")))
                {
                    SetUnsupportedEditorsReadOnly(form);
                    TextBox relation = FindControl<TextBox>(form, "edRelation");
                    GroupBox group = FindControl<GroupBox>(form, "gbRelation");
                    relation.ReadOnly = false;
                    relation.Text = ship.RelationToRangers[index].ToString(CultureInfo.InvariantCulture);
                    group.Text = "Отношение | " + PlanetRelationRangerName(rangers, index);
                    form.Text = "Отношение";
                    form.ShowDialog(owner ?? this);
                    int parsed;
                    if (TryParseInt32(relation.Text, out parsed) && parsed >= 0 && parsed <= 100)
                        ship.RelationToRangers[index] = (byte)parsed;
                }
                refresh();
            };
            list.DoubleClick += delegate { edit(); };
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Редактировать", null, delegate { edit(); });
            list.ContextMenuStrip = menu;
            refresh();
        }

        private void BindShipReferencedItemCollection(ListBox list, GroupBox group,
            List<uint> references, Form owner, string caption)
        {
            Action refresh = delegate
            {
                int selectedIndex = list.SelectedIndex;
                list.BeginUpdate(); list.Items.Clear();
                foreach (uint reference in references)
                {
                    ItemHeaderRecord item = FindItemById(reference);
                    string text = item == null
                        ? "TItem ID " + reference.ToString(CultureInfo.InvariantCulture)
                        : (string.IsNullOrEmpty(item.Name) ? "TItem" : item.Name) +
                            " [тип " + item.Type.ToString(CultureInfo.InvariantCulture) +
                            ", ID " + reference.ToString(CultureInfo.InvariantCulture) + "]";
                    list.Items.Add(new UInt32ValueChoice(reference, text));
                }
                list.EndUpdate();
                if (selectedIndex >= 0 && selectedIndex < list.Items.Count)
                    list.SelectedIndex = selectedIndex;
                if (group != null) group.Text = caption + ": " +
                    references.Count.ToString(CultureInfo.InvariantCulture);
            };
            Action edit = delegate
            {
                UInt32ValueChoice selected = list.SelectedItem as UInt32ValueChoice;
                if (selected == null) return;
                ItemHeaderRecord item = FindItemById(selected.Value);
                if (item == null) return;
                EditItem(item, owner); refresh();
            };
            Action delete = delegate
            {
                int selectedIndex = list.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= references.Count) return;
                references.RemoveAt(selectedIndex); refresh();
                if (list.Items.Count != 0)
                    list.SelectedIndex = Math.Min(selectedIndex, list.Items.Count - 1);
            };
            list.DoubleClick += delegate { edit(); };
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Редактировать", null, delegate { edit(); });
            menu.Items.Add("Удалить", null, delegate { delete(); });
            list.ContextMenuStrip = menu;
            refresh();
        }

        private void EditItem(ItemHeaderRecord item, IWin32Window dialogOwner = null)
        {
            if (pendingDeletedItemStarts.Contains(item.Start))
            {
                MessageBox.Show(this, "Этот предмет уже удалён из сериализованной коллекции корабля.",
                    "TItem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TITEMFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox x = FindControl<TextBox>(form, "edPosX");
                TextBox y = FindControl<TextBox>(form, "edPosY");
                TextBox name = FindControl<TextBox>(form, "edCustomName");
                TextBox weight = FindControl<TextBox>(form, "edWeight");
                TextBox cost = FindControl<TextBox>(form, "edCost");
                TextBox destroy = FindControl<TextBox>(form, "edItemDestroy");
                TextBox noDrop = FindControl<TextBox>(form, "edNoDrop");
                TextBox systemName = FindControl<TextBox>(form, "edSysName");
                TextBox customFaction = FindControl<TextBox>(form, "edCustomFaction");
                TextBox strength = FindControl<TextBox>(form, "edStrength");
                TextBox slot = FindControl<TextBox>(form, "edSlot");
                CheckBox broken = FindControl<CheckBox>(form, "chbBroken");
                CheckBox exploitable = FindControl<CheckBox>(form, "chbExplotable");
                ComboBox owner = FindControl<ComboBox>(form, "cbOwner");
                ComboBox type = FindControl<ComboBox>(form, "cbItemType");
                ComboBox dominatorSeries = FindControl<ComboBox>(form, "cbDominatorSeries");
                TextBox goodsItemCount = FindControl<TextBox>(form, "edGoodsItemCount");
                CheckBox goodsItemNatural = FindControl<CheckBox>(form, "chbGoodsItemNatural");
                foreach (TextBox editor in new TextBox[] { x, y, name, weight, cost, destroy, noDrop })
                    editor.ReadOnly = false;
                x.Text = item.X.ToString("R", CultureInfo.InvariantCulture);
                y.Text = item.Y.ToString("R", CultureInfo.InvariantCulture);
                name.Text = item.Name ?? string.Empty;
                weight.Text = item.Weight.ToString(CultureInfo.InvariantCulture);
                cost.Text = item.Cost.ToString(CultureInfo.InvariantCulture);
                destroy.Text = item.ItemDestroy.ToString(CultureInfo.InvariantCulture);
                noDrop.Text = item.NoDrop.ToString(CultureInfo.InvariantCulture);
                owner.Items.Clear();
                for (int value = 0; value <= byte.MaxValue; value++) owner.Items.Add(value.ToString(CultureInfo.InvariantCulture));
                owner.SelectedIndex = item.Owner;
                owner.Enabled = true;
                DisplayComboValue(type, item.Type.ToString(CultureInfo.InvariantCulture));
                FindControl<Label>(form, "lblScriptItemVal").Text = "—";
                FindControl<Label>(form, "lblStoredItemVal").Text = "—";
                if (item.HasGoodsTail)
                {
                    weight.ReadOnly = true;
                    goodsItemCount.ReadOnly = false;
                    goodsItemCount.Text = item.GoodsItemCount.ToString(CultureInfo.InvariantCulture);
                    goodsItemNatural.Enabled = true; goodsItemNatural.Checked = item.GoodsItemNatural;
                }
                if (item.Type >= 8)
                {
                    foreach (TextBox editor in new TextBox[] { systemName, customFaction, strength, slot })
                        editor.ReadOnly = false;
                    systemName.Text = item.SystemName ?? string.Empty;
                    customFaction.Text = item.CustomFaction ?? string.Empty;
                    strength.Text = item.Strength.ToString("R", CultureInfo.InvariantCulture);
                    slot.Text = item.Slot.ToString(CultureInfo.InvariantCulture);
                    broken.Checked = item.Broken != 0; broken.Enabled = true;
                    exploitable.Checked = item.Exploitable != 0; exploitable.Enabled = true;
                    dominatorSeries.Items.Clear();
                    for (int value = 0; value <= byte.MaxValue; value++)
                        dominatorSeries.Items.Add(value.ToString(CultureInfo.InvariantCulture));
                    dominatorSeries.SelectedIndex = item.DominatorSeries;
                    dominatorSeries.Enabled = true;
                }
                BindItemDerivedEditors(form, item);
                CheckBox hullInterceptors = null;
                if (item.Type == 42)
                {
                    ItemDerivedField interceptorFlag = FindItemDerivedField(item,
                        "$HullHasInterceptors");
                    bool hasInterceptors = interceptorFlag != null &&
                        interceptorFlag.IntegerValue != 0;
                    hullInterceptors = BindCheckableGroup(form, "gbInterceptors",
                        hasInterceptors);
                    GroupBox interceptorGroup = FindControl<GroupBox>(form, "gbInterceptors");
                    ComboBox interceptorTarget = FindControl<ComboBox>(form,
                        "cbInterceptorsNextTarget");
                    ComboBox interceptorStrategy = FindControl<ComboBox>(form,
                        "cbInterceptorsStrategy");
                    TextBox interceptorDuration = FindControl<TextBox>(form,
                        "edInterceptorsDuration");
                    if (!hasInterceptors)
                    {
                        PopulateShipReferenceCombo(interceptorTarget, 0);
                        PopulateByteCombo(interceptorStrategy, 0, new string[] {
                            "Ручное управление", "Максимум прочности", "Минимум прочности",
                            "Максимум силы", "Максимум защиты", "Минимальная дистанция",
                            "Максимальная дистанция"
                        });
                        interceptorDuration.ReadOnly = false;
                        interceptorDuration.Text = "0";
                    }
                    EventHandler updateInterceptorState = delegate
                    {
                        interceptorGroup.Enabled = hullInterceptors.Checked;
                        hullInterceptors.Enabled = true;
                    };
                    hullInterceptors.CheckedChanged += updateInterceptorState;
                    updateInterceptorState(hullInterceptors, EventArgs.Empty);
                }
                ItemEquipmentEditorState equipmentEditors = BindItemEquipmentEditors(form, item);
                TextBox treasureInfo1 = null, treasureInfo2 = null;
                CheckBox treasureHideTags = null;
                string rawTreasureInfo1 = null, rawTreasureInfo2 = null;
                if (item.Type == 74)
                {
                    treasureInfo1 = FindControl<TextBox>(form, "mmPlanetInfo1");
                    treasureInfo2 = FindControl<TextBox>(form, "mmPlanetInfo2");
                    treasureHideTags = FindControl<CheckBox>(form, "chbHideTags");
                    rawTreasureInfo1 = treasureInfo1.Text;
                    rawTreasureInfo2 = treasureInfo2.Text;
                    treasureHideTags.Enabled = true;
                    treasureHideTags.Checked = true;
                    treasureHideTags.CheckedChanged += delegate
                    {
                        if (treasureHideTags.Checked)
                        {
                            treasureInfo1.Text = rawTreasureInfo1;
                            treasureInfo2.Text = rawTreasureInfo2;
                            treasureInfo1.ReadOnly = false;
                            treasureInfo2.ReadOnly = false;
                        }
                        else
                        {
                            rawTreasureInfo1 = treasureInfo1.Text;
                            rawTreasureInfo2 = treasureInfo2.Text;
                            treasureInfo1.Text = FilterGameTextTags(rawTreasureInfo1);
                            treasureInfo2.Text = FilterGameTextTags(rawTreasureInfo2);
                            treasureInfo1.ReadOnly = true;
                            treasureInfo2.ReadOnly = true;
                        }
                    };
                }
                Button customWeaponButton = FindControl<Button>(form, "btnCustomWeapon");
                CustomWeaponInfoRecord customWeaponInfo = FindCustomWeaponInfo(item.CustomWeaponName);
                if (customWeaponInfo != null)
                {
                    customWeaponButton.Enabled = true;
                    customWeaponButton.Click += delegate { EditCustomWeaponInfo(customWeaponInfo); };
                }
                bool editNestedTranclucator = false;
                Button tranclucatorButton = FindControl<Button>(form, "btnTranclucator");
                if (item.NestedTranclucator != null)
                {
                    tranclucatorButton.Enabled = true;
                    tranclucatorButton.Click += delegate
                    {
                        editNestedTranclucator = true;
                        form.Close();
                    };
                }
                ConfigureItemPages(form, item.Type);
                form.Text = "Предмет — ID " + item.ObjectId + " / type " + item.Type;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(dialogOwner ?? this);

                float parsedX = 0, parsedY = 0;
                int parsedWeight = 0, parsedDestroy = 0;
                uint parsedCost = 0;
                byte parsedNoDrop = 0;
                int parsedGoodsItemCount = item.GoodsItemCount;
                float parsedStrength = item.Strength;
                byte parsedSlot = item.Slot;
                List<ItemDerivedField> parsedDerivedFields;
                bool valid = TryParseFiniteFloat(x.Text, out parsedX) && TryParseFiniteFloat(y.Text, out parsedY) &&
                    int.TryParse(weight.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedWeight) &&
                    uint.TryParse(cost.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedCost) &&
                    int.TryParse(destroy.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedDestroy) &&
                    byte.TryParse(noDrop.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedNoDrop) &&
                    parsedNoDrop <= 1 &&
                    (name.Text ?? string.Empty).Length <= 512;
                if (item.HasGoodsTail)
                {
                    valid &= TryParseInt32(goodsItemCount.Text, out parsedGoodsItemCount) &&
                        parsedGoodsItemCount >= 0 && parsedGoodsItemCount <= 10000;
                    parsedWeight = parsedGoodsItemCount;
                }
                if (item.Type >= 8)
                    valid &= TryParseFiniteFloat(strength.Text, out parsedStrength) &&
                        byte.TryParse(slot.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedSlot) &&
                        (systemName.Text ?? string.Empty).Length <= 512 &&
                        (customFaction.Text ?? string.Empty).Length <= 512;
                int parsedBonus = item.Bonus, parsedSpecial = item.Special;
                uint parsedBonusReferenceId = item.BonusReferenceId;
                uint parsedSpecialReferenceId = item.SpecialReferenceId;
                List<ItemExtraSpecialRecord> parsedExtraSpecials = item.ExtraSpecials;
                valid &= TryReadItemEquipmentEditors(equipmentEditors, out parsedBonus,
                    out parsedBonusReferenceId, out parsedSpecial, out parsedSpecialReferenceId,
                    out parsedExtraSpecials);
                if (treasureHideTags != null && !treasureHideTags.Checked)
                {
                    treasureInfo1.Text = rawTreasureInfo1;
                    treasureInfo2.Text = rawTreasureInfo2;
                }
                bool derivedValid = TryReadItemDerivedEditors(form, item, out parsedDerivedFields);
                if (derivedValid && item.Type == 42)
                    derivedValid = NormalizeHullConditionalFields(form, hullInterceptors,
                        parsedDerivedFields);
                valid &= derivedValid;
                if (!valid)
                {
                    MessageBox.Show(this, "Поля TItem не применены: проверьте координаты, вес, цену и NoDrop (0/1).",
                        "TItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                item.X = parsedX; item.Y = parsedY; item.Weight = parsedWeight;
                item.Owner = checked((byte)Math.Max(0, owner.SelectedIndex));
                item.Cost = parsedCost; item.ItemDestroy = parsedDestroy;
                item.Name = name.Text ?? string.Empty; item.NoDrop = parsedNoDrop;
                if (item.HasGoodsTail)
                {
                    item.GoodsItemCount = parsedGoodsItemCount;
                    item.GoodsItemNatural = goodsItemNatural.Checked;
                }
                if (item.Type >= 8)
                {
                    item.SystemName = systemName.Text ?? string.Empty;
                    item.CustomFaction = customFaction.Text ?? string.Empty;
                    item.Strength = parsedStrength; item.Slot = parsedSlot;
                    item.Broken = broken.Checked ? (byte)1 : (byte)0;
                    item.Exploitable = exploitable.Checked ? (byte)1 : (byte)0;
                    item.DominatorSeries = checked((byte)Math.Max(0, dominatorSeries.SelectedIndex));
                    item.Bonus = parsedBonus;
                    item.BonusReferenceId = parsedBonusReferenceId;
                    item.Special = parsedSpecial;
                    item.SpecialReferenceId = parsedSpecialReferenceId;
                    item.ExtraSpecials = parsedExtraSpecials;
                }
                item.DerivedFields = parsedDerivedFields;
                if (editNestedTranclucator && item.NestedTranclucator != null)
                    EditShip(item.NestedTranclucator);
                RefreshObjectLists();
            }
        }

        private void BindItemDerivedEditors(Form form, ItemHeaderRecord item)
        {
            if (item.DerivedFields == null) return;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                Control[] found = form.Controls.Find(field.ControlName, true);
                if (found.Length == 0) continue;
                TextBox text = found[0] as TextBox;
                if (text != null)
                {
                    text.ReadOnly = field.ControlName == "edWeaponTargetType";
                    text.Text = field.Kind == ItemDerivedField.Float32
                        ? field.FloatValue.ToString("R", CultureInfo.InvariantCulture)
                        : field.Kind == ItemDerivedField.String ? field.StringValue ?? string.Empty
                        : field.IntegerValue.ToString(CultureInfo.InvariantCulture);
                    continue;
                }
                CheckBox check = found[0] as CheckBox;
                if (check != null)
                {
                    check.Enabled = true; check.Checked = field.IntegerValue != 0; continue;
                }
                ComboBox combo = found[0] as ComboBox;
                if (combo == null) continue;
                if (field.ControlName == "cbWeaponTarget")
                {
                    byte targetType = 0;
                    foreach (ItemDerivedField candidate in item.DerivedFields)
                        if (candidate.ControlName == "edWeaponTargetType")
                            targetType = checked((byte)candidate.IntegerValue);
                    PopulateMissileReferenceCombo(combo, targetType, checked((uint)field.IntegerValue), false);
                    TextBox targetTypeEditor = FindControl<TextBox>(form, "edWeaponTargetType");
                    combo.SelectedIndexChanged += delegate
                    { UpdateMissileReferenceType(combo, targetTypeEditor); };
                }
                else if (field.ControlName == "cbSatellitePlanet" || field.ControlName == "cbTreasureMapPlanet")
                    PopulatePlanetReferenceCombo(combo, checked((uint)field.IntegerValue));
                else if (field.ControlName == "cbInterceptorsNextTarget")
                    PopulateShipReferenceCombo(combo, checked((uint)field.IntegerValue));
                else if (field.ControlName == "cbInterceptorsStrategy")
                    PopulateByteCombo(combo, checked((byte)field.IntegerValue), new string[] {
                        "Ручное управление", "Максимум прочности", "Минимум прочности",
                        "Максимум силы", "Максимум защиты", "Минимальная дистанция",
                        "Максимальная дистанция"
                    });
                else
                {
                    combo.Items.Clear(); combo.Enabled = true;
                    for (int value = 0; value <= byte.MaxValue; value++)
                        combo.Items.Add(value.ToString(CultureInfo.InvariantCulture));
                    combo.SelectedIndex = field.IntegerValue >= 0 && field.IntegerValue <= byte.MaxValue
                        ? (int)field.IntegerValue : -1;
                }
            }
            if (item.Type == 42)
            {
                ItemDerivedField seriesNumber = FindItemDerivedField(item, "edSeriesNum");
                ItemDerivedField seriesReference = FindItemDerivedField(item, "edSeriesCRC");
                ComboBox seriesName = FindControl<ComboBox>(form, "cbSeriesName");
                TextBox seriesNumberEditor = FindControl<TextBox>(form, "edSeriesNum");
                TextBox seriesBlockEditor = FindControl<TextBox>(form, "edSeriesBlockName");
                TextBox seriesCrcEditor = FindControl<TextBox>(form, "edSeriesCRC");
                int currentSeries = seriesNumber == null ? -1 : checked((int)seriesNumber.IntegerValue);
                uint currentReference = seriesReference == null ? 0u :
                    checked((uint)seriesReference.IntegerValue);
                PopulateHullSeriesReferenceCombo(seriesName, currentSeries, currentReference,
                    seriesNumberEditor, seriesBlockEditor, seriesCrcEditor);
                HullSeriesReferenceChoice previousSeries = seriesName.SelectedItem as HullSeriesReferenceChoice;
                seriesName.SelectedIndexChanged += delegate
                {
                    HullSeriesReferenceChoice selected = seriesName.SelectedItem as HullSeriesReferenceChoice;
                    if (selected == null || previousSeries == null ||
                        selected.Index == previousSeries.Index &&
                        selected.ReferenceId == previousSeries.ReferenceId) return;
                    RecalculateHullSeries(form, previousSeries, selected);
                    previousSeries = selected;
                };
                ItemDerivedField interceptors = FindItemDerivedField(item, "$HullHasInterceptors");
                GroupBox group = FindControl<GroupBox>(form, "gbInterceptors");
                group.Enabled = interceptors != null && interceptors.IntegerValue != 0;
            }
        }

        private void PopulateHullSeriesReferenceCombo(ComboBox combo, int currentIndex,
            uint currentReferenceId, TextBox number, TextBox blockName, TextBox crc)
        {
            combo.Items.Clear();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Add(new HullSeriesReferenceChoice(-1, 0, string.Empty, "— нет серии —"));
            int selected = currentIndex < 0 ? 0 : -1;
            int selectedByReference = -1;
            if (gameCatalog != null)
            {
                foreach (HullSeriesCatalogEntry entry in gameCatalog.HullSeries)
                {
                    combo.Items.Add(new HullSeriesReferenceChoice(entry.Index, entry.ReferenceId,
                        entry.BlockName, entry.ToString()));
                    if (entry.Index == currentIndex && entry.ReferenceId == currentReferenceId)
                        selected = combo.Items.Count - 1;
                    else if (currentReferenceId != 0 && entry.ReferenceId == currentReferenceId)
                        selectedByReference = combo.Items.Count - 1;
                }
            }
            if (selected < 0 && selectedByReference >= 0) selected = selectedByReference;
            if (selected < 0)
            {
                combo.Items.Add(new HullSeriesReferenceChoice(currentIndex, currentReferenceId,
                    blockName.Text, "Неизвестная серия [" +
                    currentIndex.ToString(CultureInfo.InvariantCulture) + "]"));
                selected = combo.Items.Count - 1;
            }
            combo.SelectedIndexChanged += delegate
            {
                ApplyHullSeriesReferenceChoice(combo, number, blockName, crc);
            };
            combo.SelectedIndex = selected;
            combo.Enabled = gameCatalog != null && gameCatalog.HullSeries.Count != 0;
            blockName.ReadOnly = true;
            ApplyHullSeriesReferenceChoice(combo, number, blockName, crc);
        }

        private static void ApplyHullSeriesReferenceChoice(ComboBox combo, TextBox number,
            TextBox blockName, TextBox crc)
        {
            HullSeriesReferenceChoice choice = combo.SelectedItem as HullSeriesReferenceChoice;
            if (choice == null) return;
            number.Text = choice.Index.ToString(CultureInfo.InvariantCulture);
            blockName.Text = choice.BlockName;
            crc.Text = choice.ReferenceId.ToString("X8", CultureInfo.InvariantCulture);
        }

        private static ItemDerivedField FindItemDerivedField(ItemHeaderRecord item, string controlName)
        {
            if (item == null || item.DerivedFields == null) return null;
            foreach (ItemDerivedField field in item.DerivedFields)
                if (field.ControlName == controlName) return field;
            return null;
        }

        private static void ConfigureItemPages(Form form, byte itemType)
        {
            HashSet<string> visible = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            visible.Add("tsMain");
            if (itemType < 8) visible.Add("tsGoodsItem");
            else if (itemType == 8 || itemType == 9) visible.Add("tsArtefactCustom");
            else if (itemType == 23) visible.Add("tsTransmitter");
            else if (itemType == 25) visible.Add("tsTranclucator");
            else if (itemType == 42) { visible.Add("tsHull"); visible.Add("tsSW"); }
            else if (itemType == 43) visible.Add("tsFuelTank");
            else if (itemType == 44) visible.Add("tsEngine");
            else if (itemType == 45) visible.Add("tsRadar");
            else if (itemType == 46) visible.Add("tsScaner");
            else if (itemType == 47) visible.Add("tsRepairRobot");
            else if (itemType == 48) visible.Add("tsCargoHook");
            else if (itemType == 49) visible.Add("tsDefGenerator");
            else if (itemType >= 50 && itemType <= 68) visible.Add("tsWeapon");
            else if (itemType == 69 || itemType == 75) visible.Add("tsCountableItem");
            else if (itemType == 70) visible.Add("tsUselessItem");
            else if (itemType == 72) visible.Add("tsCistern");
            else if (itemType == 73) visible.Add("tsSatellite");
            else if (itemType == 74) visible.Add("tsTreasureMap");

            EditorFormFactory.ConfigureTabPages(form, "pcParams",
                new List<string>(visible).ToArray());
        }

        private static void ConfigureShipPages(Form form, ShipHeaderRecord ship)
        {
            if (ship.IsPlayer || ship.HasPlayerPrefix)
                EditorFormFactory.ConfigurePlayerSections(form);
            string[] subtypeGroups = { "gbNormalShip", "gbWarriorShip", "gbTransportShip",
                "gbPirateShip", "gbDominatorShip", "gbRangerShip" };
            foreach (string group in subtypeGroups)
                EditorFormFactory.SetLayoutControlVisible(form, group, false);
            if (ship.HasNormalShipTail)
                EditorFormFactory.SetLayoutControlVisible(form, "gbNormalShip", true);
            if (ship.Type == 0)
                EditorFormFactory.SetLayoutControlVisible(form, "gbDominatorShip", true);
            else if (ship.Type == 1 && ship.HasRangerTail)
                EditorFormFactory.SetLayoutControlVisible(form, "gbRangerShip", true);
            else if (ship.Type == 2)
                EditorFormFactory.SetLayoutControlVisible(form, "gbTransportShip", true);
            else if (ship.Type == 3)
                EditorFormFactory.SetLayoutControlVisible(form, "gbPirateShip", true);
            else if (ship.Type == 4)
                EditorFormFactory.SetLayoutControlVisible(form, "gbWarriorShip", true);

            List<string> parameterPages = new List<string>();
            parameterPages.Add("tsMain");
            // TShipForm.FormShow only hides subtype-dependent pages inside
            // pcParams.  The common "Дополнительные" page remains available
            // for every normal ship and contains fields from the shared tail.
            if (ship.HasCommonTail) parameterPages.Add("tsAdditional");
            if (ship.HasNormalShipTail || ship.HasSimpleDerivedTail || ship.HasRangerTail)
                parameterPages.Add("tsSubType");
            if (ship.IsPlayer || ship.HasPlayerPrefix) parameterPages.Add("tsPlayer");
            if (ship.HasTranclucatorTail) parameterPages.Add("tsTranclucator");
            EditorFormFactory.ConfigureTabPages(form, "pcParams", parameterPages.ToArray());

            List<string> shipPages = new List<string>();
            shipPages.Add("tsParams");
            // Ordinary ships expose cargo and modification collections here.
            if (ship.HasPreCommonCollections)
            {
                shipPages.Add("tsHold");
                shipPages.Add("tsMods");
            }
            if (ship.HasRuinsTail) shipPages.Add("tsRuins");
            EditorFormFactory.ConfigureTabPages(form, "pcShip", shipPages.ToArray());
            EditorFormFactory.Relayout(form);
        }

        private bool TryReadItemDerivedEditors(Form form, ItemHeaderRecord item,
            out List<ItemDerivedField> parsed)
        {
            parsed = null;
            if (item.DerivedFields == null) return true;
            parsed = new List<ItemDerivedField>();
            foreach (ItemDerivedField original in item.DerivedFields)
            {
                ItemDerivedField field = original.Clone();
                Control[] found = form.Controls.Find(field.ControlName, true);
                if (found.Length == 0) { parsed.Add(field); continue; }
                TextBox text = found[0] as TextBox;
                CheckBox check = found[0] as CheckBox;
                ComboBox combo = found[0] as ComboBox;
                if (field.Kind == ItemDerivedField.String)
                {
                    if (text == null || (text.Text ?? string.Empty).Length > 4096) return false;
                    field.StringValue = text.Text ?? string.Empty;
                }
                else if (field.Kind == ItemDerivedField.Float32)
                {
                    float value;
                    if (text == null || !TryParseFiniteFloat(text.Text, out value)) return false;
                    field.FloatValue = value;
                }
                else if (field.Kind == ItemDerivedField.Boolean)
                {
                    if (check == null) return false;
                    field.IntegerValue = check.Checked ? 1 : 0;
                }
                else if (field.Kind == ItemDerivedField.UInt32 && combo != null)
                {
                    if (field.ControlName == "cbWeaponTarget")
                    {
                        MissileReferenceChoice choice = combo.SelectedItem as MissileReferenceChoice;
                        if (choice == null) return false;
                        field.IntegerValue = choice.ObjectId;
                    }
                    else
                    {
                        UInt32ValueChoice choice = combo.SelectedItem as UInt32ValueChoice;
                        if (choice == null) return false;
                        field.IntegerValue = choice.Value;
                    }
                }
                else if (combo != null)
                {
                    if (combo.SelectedIndex < 0) return false;
                    field.IntegerValue = combo.SelectedIndex;
                }
                else
                {
                    long value;
                    uint referenceValue;
                    if (text != null && field.Kind == ItemDerivedField.UInt32 &&
                        field.ControlName.EndsWith("CRC", StringComparison.OrdinalIgnoreCase) &&
                        TryParseUInt32Flexible(text.Text, out referenceValue))
                        value = referenceValue;
                    else if (text == null || !long.TryParse((text.Text ?? string.Empty).Trim(),
                        NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) return false;
                    if ((field.Kind == ItemDerivedField.Byte && (value < 0 || value > byte.MaxValue)) ||
                        (field.Kind == ItemDerivedField.UInt16 && (value < 0 || value > ushort.MaxValue)) ||
                        (field.Kind == ItemDerivedField.Int32 && (value < int.MinValue || value > int.MaxValue)) ||
                        (field.Kind == ItemDerivedField.UInt32 && (value < 0 || value > uint.MaxValue))) return false;
                    field.IntegerValue = value;
                }
                parsed.Add(field);
            }
            if (item.Type == 42)
            {
                ComboBox seriesCombo = FindControl<ComboBox>(form, "cbSeriesName");
                HullSeriesReferenceChoice choice = seriesCombo.SelectedItem as HullSeriesReferenceChoice;
                if (choice == null) return false;
                int numberIndex = -1, crcIndex = -1;
                for (int index = 0; index < parsed.Count; index++)
                {
                    if (parsed[index].ControlName == "edSeriesNum") numberIndex = index;
                    else if (parsed[index].ControlName == "edSeriesCRC") crcIndex = index;
                }
                if (numberIndex < 0) return false;
                parsed[numberIndex].IntegerValue = choice.Index;
                if (choice.Index < 0)
                {
                    if (crcIndex >= 0) parsed.RemoveAt(crcIndex);
                }
                else if (crcIndex >= 0)
                    parsed[crcIndex].IntegerValue = choice.ReferenceId;
                else
                {
                    ItemDerivedField reference = new ItemDerivedField();
                    reference.ControlName = "edSeriesCRC";
                    reference.Kind = ItemDerivedField.UInt32;
                    reference.Offset = parsed[numberIndex].End;
                    reference.End = reference.Offset;
                    reference.IntegerValue = choice.ReferenceId;
                    parsed.Insert(numberIndex + 1, reference);
                }
            }
            return true;
        }

        private static bool NormalizeHullConditionalFields(Form form, CheckBox interceptors,
            List<ItemDerivedField> fields)
        {
            if (interceptors == null || fields == null) return false;
            ItemDerivedField flag = FindItemDerivedField(fields, "$HullHasInterceptors");
            ItemDerivedField energyMax = FindItemDerivedField(fields, "edEnergyMax");
            if (flag == null || energyMax == null) return false;
            flag.IntegerValue = interceptors.Checked ? 1 : 0;
            RemoveItemDerivedField(fields, "cbInterceptorsNextTarget");
            RemoveItemDerivedField(fields, "cbInterceptorsStrategy");
            RemoveItemDerivedField(fields, "edInterceptorsDuration");
            if (interceptors.Checked)
            {
                ComboBox target = FindControl<ComboBox>(form, "cbInterceptorsNextTarget");
                ComboBox strategy = FindControl<ComboBox>(form, "cbInterceptorsStrategy");
                TextBox duration = FindControl<TextBox>(form, "edInterceptorsDuration");
                UInt32ValueChoice targetValue = target.SelectedItem as UInt32ValueChoice;
                ByteValueChoice strategyValue = strategy.SelectedItem as ByteValueChoice;
                byte durationValue;
                if (targetValue == null || strategyValue == null ||
                    !byte.TryParse(duration.Text, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out durationValue)) return false;
                int insertion = fields.IndexOf(energyMax) + 1;
                fields.Insert(insertion++, NewItemDerivedInteger("cbInterceptorsNextTarget",
                    ItemDerivedField.UInt32, targetValue.Value));
                fields.Insert(insertion++, NewItemDerivedInteger("cbInterceptorsStrategy",
                    ItemDerivedField.Byte, strategyValue.Value));
                fields.Insert(insertion, NewItemDerivedInteger("edInterceptorsDuration",
                    ItemDerivedField.Byte, durationValue));
            }
            return true;
        }

        private static ItemDerivedField NewItemDerivedInteger(string controlName, byte kind,
            long value)
        {
            ItemDerivedField field = new ItemDerivedField();
            field.ControlName = controlName; field.Kind = kind; field.IntegerValue = value;
            return field;
        }

        private static void RemoveItemDerivedField(List<ItemDerivedField> fields,
            string controlName)
        {
            fields.RemoveAll(delegate(ItemDerivedField field)
                { return field.ControlName == controlName; });
        }

        private static void InsertItemDerivedFieldAfter(List<ItemDerivedField> fields,
            string precedingControl, ItemDerivedField value)
        {
            ItemDerivedField preceding = FindItemDerivedField(fields, precedingControl);
            if (preceding == null) fields.Add(value);
            else fields.Insert(fields.IndexOf(preceding) + 1, value);
        }

        private static ItemDerivedField FindItemDerivedField(List<ItemDerivedField> fields,
            string controlName)
        {
            if (fields == null) return null;
            foreach (ItemDerivedField field in fields)
                if (field.ControlName == controlName) return field;
            return null;
        }

        private ItemEquipmentEditorState BindItemEquipmentEditors(Form form, ItemHeaderRecord item)
        {
            if (item.Type < 8) return null;
            ItemEquipmentEditorState state = new ItemEquipmentEditorState();
            state.Bonus = FindControl<TextBox>(form, "edBonusNum");
            state.BonusBlock = FindControl<TextBox>(form, "edBonusBlockName");
            state.BonusCrc = FindControl<TextBox>(form, "edBonusCRC");
            state.BonusName = FindControl<ComboBox>(form, "cbBonusName");
            state.Special = FindControl<TextBox>(form, "edSpecialNum");
            state.SpecialBlock = FindControl<TextBox>(form, "edSpecialBlockName");
            state.SpecialCrc = FindControl<TextBox>(form, "edSpecialCRC");
            state.SpecialName = FindControl<ComboBox>(form, "cbSpecialName");
            foreach (TextBox editor in new TextBox[] { state.Bonus, state.BonusCrc,
                state.Special, state.SpecialCrc }) editor.ReadOnly = false;
            state.Bonus.Text = item.Bonus.ToString(CultureInfo.InvariantCulture);
            state.BonusCrc.Text = item.BonusReferenceId.ToString("X8", CultureInfo.InvariantCulture);
            state.Special.Text = item.Special.ToString(CultureInfo.InvariantCulture);
            state.SpecialCrc.Text = item.SpecialReferenceId.ToString("X8", CultureInfo.InvariantCulture);
            state.BonusBlock.ReadOnly = true;
            state.SpecialBlock.ReadOnly = true;
            PopulateMicroModuleReferenceCombo(state.BonusName, item.Bonus,
                item.BonusReferenceId, state.Bonus, state.BonusBlock, state.BonusCrc);
            PopulateMicroModuleReferenceCombo(state.SpecialName, item.Special,
                item.SpecialReferenceId, state.Special, state.SpecialBlock, state.SpecialCrc);
            MicroModuleReferenceChoice previousBonus = state.BonusName.SelectedItem as MicroModuleReferenceChoice;
            MicroModuleReferenceChoice previousSpecial = state.SpecialName.SelectedItem as MicroModuleReferenceChoice;
            state.BonusName.SelectedIndexChanged += delegate
            {
                MicroModuleReferenceChoice selected = state.BonusName.SelectedItem as MicroModuleReferenceChoice;
                if (SameMicroModuleChoice(previousBonus, selected)) return;
                RecalculateItemMicroModule(form, item, previousBonus, selected, false);
                previousBonus = selected;
            };
            state.SpecialName.SelectedIndexChanged += delegate
            {
                MicroModuleReferenceChoice selected = state.SpecialName.SelectedItem as MicroModuleReferenceChoice;
                if (SameMicroModuleChoice(previousSpecial, selected)) return;
                RecalculateItemMicroModule(form, item, previousSpecial, selected, true);
                previousSpecial = selected;
            };

            state.ExtraPanel = FindControl<Panel>(form, "sbExtraSpecial");
            foreach (ItemExtraSpecialRecord record in item.ExtraSpecials)
                state.ExtraSpecials.Add(record.Clone());
            ContextMenuStrip addMenu = new ContextMenuStrip();
            addMenu.Items.Add("Добавить экстра-бонус", null, delegate
            {
                List<ItemExtraSpecialRecord> synchronized;
                if (!TryReadItemExtraSpecialRows(state, out synchronized))
                {
                    MessageBox.Show(form, "Сначала исправьте номер, CRC или количество в текущей строке.",
                        "Экстра-бонус", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                state.ExtraSpecials = synchronized;
                ItemExtraSpecialRecord added = new ItemExtraSpecialRecord();
                if (gameCatalog != null && gameCatalog.MicroModules.Count != 0)
                {
                    added.Special = gameCatalog.MicroModules[0].Index;
                    added.ReferenceId = gameCatalog.MicroModules[0].ReferenceId;
                }
                state.ExtraSpecials.Add(added);
                RebuildItemExtraSpecialRows(state, form);
            });
            state.ExtraPanel.ContextMenuStrip = addMenu;
            RebuildItemExtraSpecialRows(state, form);
            return state;
        }

        private void PopulateMicroModuleReferenceCombo(ComboBox combo, int currentIndex,
            uint currentReferenceId, TextBox number, TextBox blockName, TextBox crc)
        {
            combo.Items.Clear();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Add(new MicroModuleReferenceChoice(0, 0, string.Empty, "— нет —"));
            int selected = currentIndex == 0 ? 0 : -1;
            int selectedByReference = -1;
            if (gameCatalog != null)
            {
                foreach (MicroModuleCatalogEntry entry in gameCatalog.MicroModules)
                {
                    combo.Items.Add(new MicroModuleReferenceChoice(entry.Index, entry.ReferenceId,
                        entry.BlockName, entry.ToString()));
                    if (entry.Index == currentIndex && entry.ReferenceId == currentReferenceId)
                        selected = combo.Items.Count - 1;
                    else if (currentReferenceId != 0 && entry.ReferenceId == currentReferenceId)
                        selectedByReference = combo.Items.Count - 1;
                }
            }
            if (selected < 0 && selectedByReference >= 0) selected = selectedByReference;
            if (selected < 0)
            {
                string caption = "Неизвестный микромодуль [" +
                    currentIndex.ToString(CultureInfo.InvariantCulture) + "]";
                combo.Items.Add(new MicroModuleReferenceChoice(currentIndex, currentReferenceId,
                    blockName.Text, caption));
                selected = combo.Items.Count - 1;
            }
            combo.SelectedIndexChanged += delegate
            {
                ApplyMicroModuleReferenceChoice(combo, number, blockName, crc);
            };
            combo.SelectedIndex = selected;
            combo.Enabled = gameCatalog != null && gameCatalog.MicroModules.Count != 0;
            ApplyMicroModuleReferenceChoice(combo, number, blockName, crc);
        }

        private static void ApplyMicroModuleReferenceChoice(ComboBox combo, TextBox number,
            TextBox blockName, TextBox crc)
        {
            MicroModuleReferenceChoice choice = combo.SelectedItem as MicroModuleReferenceChoice;
            if (choice == null) return;
            number.Text = choice.Index.ToString(CultureInfo.InvariantCulture);
            blockName.Text = choice.BlockName;
            crc.Text = choice.ReferenceId.ToString("X8", CultureInfo.InvariantCulture);
        }

        private static bool SameMicroModuleChoice(MicroModuleReferenceChoice left,
            MicroModuleReferenceChoice right)
        {
            return left == null && right == null || left != null && right != null &&
                left.Index == right.Index && left.ReferenceId == right.ReferenceId;
        }

        private MicroModuleCatalogEntry ResolveMicroModule(MicroModuleReferenceChoice choice)
        {
            if (choice == null || choice.Index <= 0 || gameCatalog == null) return null;
            MicroModuleCatalogEntry entry = gameCatalog.FindMicroModule(choice.Index,
                choice.ReferenceId);
            return entry != null && entry.Index == choice.Index &&
                entry.ReferenceId == choice.ReferenceId ? entry : null;
        }

        private HullSeriesCatalogEntry ResolveHullSeries(HullSeriesReferenceChoice choice)
        {
            if (choice == null || choice.Index < 0 || gameCatalog == null) return null;
            HullSeriesCatalogEntry entry = gameCatalog.FindHullSeries(choice.Index,
                choice.ReferenceId);
            return entry != null && entry.Index == choice.Index &&
                entry.ReferenceId == choice.ReferenceId ? entry : null;
        }

        private void RecalculateItemMicroModule(Form form, ItemHeaderRecord item,
            MicroModuleReferenceChoice previous, MicroModuleReferenceChoice selected,
            bool special)
        {
            MicroModuleCatalogEntry oldEntry = ResolveMicroModule(previous);
            MicroModuleCatalogEntry newEntry = ResolveMicroModule(selected);
            TextBox weight = FindControl<TextBox>(form, "edWeight");
            TextBox cost = FindControl<TextBox>(form, "edCost");
            int currentWeight;
            uint currentCost;
            if (!int.TryParse(weight.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out currentWeight) || !uint.TryParse(cost.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out currentCost)) return;

            if (oldEntry != null)
            {
                currentWeight = ScaleOriginalInteger(currentWeight, oldEntry.SizePercent, false, true);
                currentCost = ScaleOriginalCost(currentCost, oldEntry.CostPercent, false);
                ApplyMicroModuleBonuses(form, item, oldEntry, -1, special);
            }
            if (newEntry != null)
            {
                currentWeight = ScaleOriginalInteger(currentWeight, newEntry.SizePercent, true, true);
                currentCost = ScaleOriginalCost(currentCost, newEntry.CostPercent, true);
                ApplyMicroModuleBonuses(form, item, newEntry, 1, special);
            }
            weight.Text = currentWeight.ToString(CultureInfo.InvariantCulture);
            cost.Text = currentCost.ToString(CultureInfo.InvariantCulture);
            if (!special && item.Type == 42)
            {
                TextBox hitPoints = FindControl<TextBox>(form, "edHitPoints");
                int value;
                if (int.TryParse(hitPoints.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out value) && currentWeight < value)
                    hitPoints.Text = currentWeight.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void RecalculateHullSeries(Form form, HullSeriesReferenceChoice previous,
            HullSeriesReferenceChoice selected)
        {
            HullSeriesCatalogEntry oldEntry = ResolveHullSeries(previous);
            HullSeriesCatalogEntry newEntry = ResolveHullSeries(selected);
            TextBox weight = FindControl<TextBox>(form, "edWeight");
            TextBox cost = FindControl<TextBox>(form, "edCost");
            TextBox hitPoints = FindControl<TextBox>(form, "edHitPoints");
            int currentWeight;
            uint currentCost;
            if (!int.TryParse(weight.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out currentWeight) || !uint.TryParse(cost.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out currentCost)) return;
            if (oldEntry != null)
            {
                if (oldEntry.SizePercent != 0)
                    currentWeight = ScaleOriginalInteger(currentWeight,
                        oldEntry.SizePercent, false, false);
                currentCost = oldEntry.CostPercent == 0 ? 0u :
                    ScaleOriginalCost(currentCost, oldEntry.CostPercent, false);
            }
            if (newEntry != null)
            {
                currentWeight = ScaleOriginalInteger(currentWeight,
                    newEntry.SizePercent, true, false);
                currentCost = ScaleOriginalCost(currentCost, newEntry.CostPercent, true);
            }
            weight.Text = currentWeight.ToString(CultureInfo.InvariantCulture);
            cost.Text = currentCost.ToString(CultureInfo.InvariantCulture);
            hitPoints.Text = currentWeight.ToString(CultureInfo.InvariantCulture);
        }

        private static int ScaleOriginalInteger(int value, int percent, bool apply,
            bool minimumOne)
        {
            if (!apply && percent == 0) return value;
            double scaled = apply ? value / 100.0 * percent : value * 100.0 / percent;
            if (minimumOne && scaled < 1.0) scaled = 1.0;
            if (scaled > int.MaxValue) return int.MaxValue;
            if (scaled < int.MinValue) return int.MinValue;
            return checked((int)Math.Round(scaled, MidpointRounding.ToEven));
        }

        private static uint ScaleOriginalCost(uint value, int percent, bool apply)
        {
            if (!apply && percent == 0) return value;
            double scaled = apply ? value / 100.0 * percent : value * 100.0 / percent;
            if (scaled < 0.0 || scaled > 100000000.0) return 100000000u;
            return checked((uint)Math.Round(scaled, MidpointRounding.ToEven));
        }

        private static int MicroBonus(MicroModuleCatalogEntry entry, string name)
        {
            int value;
            return entry != null && entry.Bonuses.TryGetValue(name, out value) ? value : 0;
        }

        private void ApplyMicroModuleBonuses(Form form, ItemHeaderRecord item,
            MicroModuleCatalogEntry entry, int direction, bool special)
        {
            byte itemType = item.Type;
            if (!special)
            {
                if (itemType == 42) AdjustIntegralEditor(form, "edArmor",
                    direction * MicroBonus(entry, "bonHull"), 8);
                else if (itemType == 43) AdjustIntegralEditor(form, "edCapacity",
                    direction * MicroBonus(entry, "bonFuel"), 8);
                else if (itemType == 44)
                {
                    AdjustIntegralEditor(form, "edSpeed", direction * MicroBonus(entry, "bonSpeed"), 32);
                    AdjustIntegralEditor(form, "edJump", direction * MicroBonus(entry, "bonJump"), 8);
                }
                else if (itemType == 45) AdjustIntegralEditor(form, "edRadius",
                    direction * MicroBonus(entry, "bonRadar"), 16);
                else if (itemType == 46) AdjustIntegralEditor(form, "edScanProtect",
                    direction * MicroBonus(entry, "bonScan"), 8);
                else if (itemType == 47) AdjustIntegralEditor(form, "edRecoverHitPoints",
                    direction * MicroBonus(entry, "bonDroid"), 8);
                else if (itemType == 48)
                {
                    AdjustIntegralEditor(form, "edPickUpSize", direction * MicroBonus(entry, "bonHook"), 16);
                    AdjustIntegralEditor(form, "edHookRadius", direction * MicroBonus(entry, "bonHookRadius"), 16);
                    AdjustFloatEditor(form, "edSpeedMin", direction * MicroBonus(entry, "bonHookMinSpeed"));
                    AdjustFloatEditor(form, "edSpeedMax", direction * MicroBonus(entry, "bonHookMaxSpeed"));
                }
                else if (itemType == 49)
                {
                    int value = MicroBonus(entry, "bonDef");
                    if (value != 0)
                        AdjustFloatEditor(form, "edDefPower", -direction * (1.0f - value / 100.0f));
                }
            }
            if (itemType >= 50 && itemType <= 68)
            {
                string damageBonus = ResolveWeaponDamageBonus(item);
                if (damageBonus != null)
                    AdjustIntegralEditor(form, "edMaxDamage",
                        direction * MicroBonus(entry, damageBonus), 32);
                AdjustIntegralEditor(form, "edWeaponRadius",
                    direction * MicroBonus(entry, "bonWRadius"), 16);
                if (itemType == 53 || itemType == 64 || itemType == 67)
                    AdjustIntegralEditor(form, "edMaxAmmunition",
                        direction * MicroBonus(entry, "bonShots"), 32);
            }
        }

        private string ResolveWeaponDamageBonus(ItemHeaderRecord item)
        {
            int group;
            if (item.Type == 68)
            {
                CustomWeaponInfoRecord descriptor = FindCustomWeaponInfo(item.CustomWeaponName);
                if (descriptor == null) return null;
                group = (descriptor.DamageType & 4u) != 0 ? 2 :
                    (descriptor.DamageType & 2u) != 0 ? 1 : 0;
            }
            else
                group = gameCatalog == null ? StockWeaponDamageGroup(item.Type) :
                    gameCatalog.GetWeaponDamageGroup(item.Type);
            return group == 2 ? "bonWMissile" : group == 1 ? "bonWSplinter" : "bonWEnergy";
        }

        internal static int StockWeaponDamageGroup(byte itemType)
        {
            switch (itemType)
            {
                case 51: case 56: case 58: case 62: case 65: return 1;
                case 53: case 64: case 67: return 2;
                default: return 0;
            }
        }

        private static void AdjustIntegralEditor(Form form, string controlName, int delta,
            int bits)
        {
            if (delta == 0) return;
            TextBox editor = FindControl<TextBox>(form, controlName);
            long current;
            if (!long.TryParse(editor.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out current)) return;
            long result = current + delta;
            if (bits == 8) result = unchecked((byte)result);
            else if (bits == 16) result = unchecked((ushort)result);
            else result = unchecked((int)result);
            editor.Text = result.ToString(CultureInfo.InvariantCulture);
        }

        private static void AdjustFloatEditor(Form form, string controlName, float delta)
        {
            if (delta == 0.0f) return;
            TextBox editor = FindControl<TextBox>(form, controlName);
            float current;
            if (!TryParseFiniteFloat(editor.Text, out current)) return;
            editor.Text = (current + delta).ToString("R", CultureInfo.InvariantCulture);
        }

        private void RebuildItemExtraSpecialRows(ItemEquipmentEditorState state, Form owner)
        {
            state.ExtraPanel.SuspendLayout();
            state.ExtraPanel.Controls.Clear();
            state.Rows.Clear();
            for (int index = 0; index < state.ExtraSpecials.Count; index++)
            {
                ItemExtraSpecialRecord record = state.ExtraSpecials[index];
                ItemExtraSpecialEditorRow row = new ItemExtraSpecialEditorRow();
                row.Record = record;
                row.Group = new GroupBox();
                row.Group.Text = "Бонус";
                row.Group.Font = boldFont;
                row.Group.Location = new Point(0, index * 95);
                row.Group.Size = new Size(Math.Max(693, state.ExtraPanel.ClientSize.Width - 6), 95);
                row.Group.Tag = row;
                state.ExtraPanel.Controls.Add(row.Group);
                AddItemExtraSpecialLabel(row.Group, "Номер:", 16, 28);
                AddItemExtraSpecialLabel(row.Group, "Имя раздела:", 127, 28);
                AddItemExtraSpecialLabel(row.Group, "CRC:", 439, 28);
                AddItemExtraSpecialLabel(row.Group, "Количество:", 566, 28);
                AddItemExtraSpecialLabel(row.Group, "Имя:", 16, 60);
                row.Number = AddItemExtraSpecialText(row.Group, 69, 25, 42,
                    record.Special.ToString(CultureInfo.InvariantCulture), false);
                row.BlockName = AddItemExtraSpecialText(row.Group, 203, 25, 222,
                    string.Empty, true);
                row.Crc = AddItemExtraSpecialText(row.Group, 469, 25, 82,
                    record.ReferenceId.ToString("X8", CultureInfo.InvariantCulture), false);
                row.Count = AddItemExtraSpecialText(row.Group, 637, 25, 42,
                    record.Count.ToString(CultureInfo.InvariantCulture), false);
                row.Name = new ComboBox();
                row.Name.Location = new Point(69, 57);
                row.Name.Size = new Size(610, 22);
                row.Name.DropDownStyle = ComboBoxStyle.DropDownList;
                row.Name.Font = regularFont;
                row.Group.Controls.Add(row.Name);
                PopulateMicroModuleReferenceCombo(row.Name, record.Special, record.ReferenceId,
                    row.Number, row.BlockName, row.Crc);
                int removeIndex = index;
                ContextMenuStrip removeMenu = new ContextMenuStrip();
                removeMenu.Items.Add("Удалить экстра-бонус", null, delegate
                {
                    List<ItemExtraSpecialRecord> synchronized;
                    if (!TryReadItemExtraSpecialRows(state, out synchronized))
                    {
                        MessageBox.Show(owner, "Сначала исправьте номер, CRC или количество в текущей строке.",
                            "Экстра-бонус", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    state.ExtraSpecials = synchronized;
                    if (removeIndex >= 0 && removeIndex < state.ExtraSpecials.Count)
                        state.ExtraSpecials.RemoveAt(removeIndex);
                    RebuildItemExtraSpecialRows(state, owner);
                });
                row.Group.ContextMenuStrip = removeMenu;
                foreach (Control child in row.Group.Controls) child.ContextMenuStrip = removeMenu;
                state.Rows.Add(row);
            }
            state.ExtraPanel.ResumeLayout();
        }

        private void AddItemExtraSpecialLabel(Control parent, string text, int x, int y)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = regularFont;
            label.Text = text;
            label.Location = new Point(x, y);
            parent.Controls.Add(label);
        }

        private TextBox AddItemExtraSpecialText(Control parent, int x, int y, int width,
            string value, bool readOnly)
        {
            TextBox editor = new TextBox();
            editor.Location = new Point(x, y);
            editor.Size = new Size(width, 21);
            editor.Font = regularFont;
            editor.Text = value;
            editor.ReadOnly = readOnly;
            parent.Controls.Add(editor);
            return editor;
        }

        private static bool TryReadItemExtraSpecialRows(ItemEquipmentEditorState state,
            out List<ItemExtraSpecialRecord> records)
        {
            records = new List<ItemExtraSpecialRecord>();
            foreach (ItemExtraSpecialEditorRow row in state.Rows)
            {
                int special, count;
                uint referenceId;
                if (!int.TryParse(row.Number.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out special) || special < 0 || special > 1000000 ||
                    !TryParseUInt32Flexible(row.Crc.Text, out referenceId) ||
                    (special == 0 && referenceId != 0) ||
                    !int.TryParse(row.Count.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out count)) return false;
                ItemExtraSpecialRecord record = new ItemExtraSpecialRecord();
                record.Special = special;
                record.ReferenceId = referenceId;
                record.Count = count;
                records.Add(record);
            }
            return true;
        }

        private static bool TryReadItemEquipmentEditors(ItemEquipmentEditorState state,
            out int bonus, out uint bonusReferenceId, out int special,
            out uint specialReferenceId, out List<ItemExtraSpecialRecord> extraSpecials)
        {
            bonus = 0; bonusReferenceId = 0; special = 0; specialReferenceId = 0;
            extraSpecials = new List<ItemExtraSpecialRecord>();
            if (state == null) return true;
            return int.TryParse(state.Bonus.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                       out bonus) && bonus >= 0 && bonus <= 1000000 &&
                TryParseUInt32Flexible(state.BonusCrc.Text, out bonusReferenceId) &&
                (bonus != 0 || bonusReferenceId == 0) &&
                int.TryParse(state.Special.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out special) && special >= 0 && special <= 1000000 &&
                TryParseUInt32Flexible(state.SpecialCrc.Text, out specialReferenceId) &&
                (special != 0 || specialReferenceId == 0) &&
                TryReadItemExtraSpecialRows(state, out extraSpecials);
        }

        private void EditHole(HoleRecord hole)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("THOLEFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox fromStar = FindControl<ComboBox>(form, "cbStar1");
                ComboBox toStar = FindControl<ComboBox>(form, "cbStar2");
                PopulateStarCombo(fromStar, hole.FromStarId); PopulateStarCombo(toStar, hole.ToStarId);
                TextBox fromX = FindControl<TextBox>(form, "edPosXStar1");
                TextBox fromY = FindControl<TextBox>(form, "edPosYStar1");
                TextBox toX = FindControl<TextBox>(form, "edPosXStar2");
                TextBox toY = FindControl<TextBox>(form, "edPosYStar2");
                TextBox turn = FindControl<TextBox>(form, "edTurn");
                TextBox type = FindControl<TextBox>(form, "edType");
                TextBox graph = FindControl<TextBox>(form, "edGraph");
                TextBox mapName = FindControl<TextBox>(form, "edABMapName");
                foreach (TextBox editor in new TextBox[] { fromX, fromY, toX, toY, turn, type, graph, mapName })
                    editor.ReadOnly = false;
                fromX.Text = hole.FromX.ToString("R", CultureInfo.InvariantCulture);
                fromY.Text = hole.FromY.ToString("R", CultureInfo.InvariantCulture);
                toX.Text = hole.ToX.ToString("R", CultureInfo.InvariantCulture);
                toY.Text = hole.ToY.ToString("R", CultureInfo.InvariantCulture);
                turn.Text = hole.TurnCreate.ToString(CultureInfo.InvariantCulture);
                type.Text = hole.HoleType.ToString(CultureInfo.InvariantCulture);
                graph.Text = hole.GraphName; mapName.Text = hole.MapName;
                form.Text = "Чёрная дыра — ID " + hole.ObjectId;
                form.ShowDialog(this);

                float parsedFromX = 0, parsedFromY = 0, parsedToX = 0, parsedToY = 0;
                int parsedTurn = 0, parsedType = 0;
                StarHeaderRecord selectedFrom = fromStar.SelectedItem as StarHeaderRecord;
                StarHeaderRecord selectedTo = toStar.SelectedItem as StarHeaderRecord;
                bool valid = selectedFrom != null && selectedTo != null &&
                    TryParseCoordinate(fromX.Text, out parsedFromX) && TryParseCoordinate(fromY.Text, out parsedFromY) &&
                    TryParseCoordinate(toX.Text, out parsedToX) && TryParseCoordinate(toY.Text, out parsedToY) &&
                    int.TryParse(turn.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedTurn) && parsedTurn >= 0 &&
                    int.TryParse(type.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedType) && parsedType >= 0 && parsedType <= 1024 &&
                    !string.IsNullOrWhiteSpace(graph.Text) && graph.Text.Length <= 128 && mapName.Text.Length <= 128;
                if (!valid)
                {
                    MessageBox.Show(this, "Поля THole не применены: проверьте системы, координаты, ход, тип и имена ресурсов.",
                        "THole", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                hole.FromStarId = selectedFrom.ObjectId; hole.ToStarId = selectedTo.ObjectId;
                hole.FromX = parsedFromX; hole.FromY = parsedFromY; hole.ToX = parsedToX; hole.ToY = parsedToY;
                hole.TurnCreate = parsedTurn; hole.HoleType = parsedType;
                hole.GraphName = graph.Text.Trim(); hole.MapName = mapName.Text.Trim();
                RefreshGalaxyObjects();
            }
        }

        private void EditAsteroid(AsteroidRecord asteroid)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TASTEROIDFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                ComboBox star = FindControl<ComboBox>(form, "cbStar");
                PopulateStarCombo(star, asteroid.ParentStarId);
                star.Enabled = true;
                TextBox positionX = FindControl<TextBox>(form, "edPosX");
                TextBox positionY = FindControl<TextBox>(form, "edPosY");
                TextBox speedX = FindControl<TextBox>(form, "edSpeedX");
                TextBox speedY = FindControl<TextBox>(form, "edSpeedY");
                TextBox mass = FindControl<TextBox>(form, "edMass");
                TextBox minerals = FindControl<TextBox>(form, "edMinerals");
                ComboBox graph = FindControl<ComboBox>(form, "cbGraphName");
                foreach (TextBox editor in new TextBox[] { positionX, positionY, speedX, speedY, mass, minerals })
                    editor.ReadOnly = false;
                PopulateAsteroidGraphNameCombo(graph, asteroid.GraphName);
                positionX.Text = ((double)asteroid.PositionX * SavContainer.AsteroidPositionScale).ToString("R", CultureInfo.InvariantCulture);
                positionY.Text = ((double)asteroid.PositionY * SavContainer.AsteroidPositionScale).ToString("R", CultureInfo.InvariantCulture);
                speedX.Text = asteroid.SpeedX.ToString("R", CultureInfo.InvariantCulture);
                speedY.Text = asteroid.SpeedY.ToString("R", CultureInfo.InvariantCulture);
                mass.Text = asteroid.Mass.ToString("R", CultureInfo.InvariantCulture);
                minerals.Text = asteroid.Minerals.ToString(CultureInfo.InvariantCulture);
                form.Text = "Астероид — ID " + asteroid.ObjectId;
                form.ShowDialog(this);

                double parsedPositionX = 0, parsedPositionY = 0;
                float parsedSpeedX = 0, parsedSpeedY = 0, parsedMass = 0;
                int parsedMinerals = 0;
                StarHeaderRecord selectedStar = star.SelectedItem as StarHeaderRecord;
                bool valid = TryParseFiniteDouble(positionX.Text, out parsedPositionX) &&
                    TryParseFiniteDouble(positionY.Text, out parsedPositionY) &&
                    TryParseFiniteFloat(speedX.Text, out parsedSpeedX) &&
                    TryParseFiniteFloat(speedY.Text, out parsedSpeedY) &&
                    TryParseFiniteFloat(mass.Text, out parsedMass) &&
                    int.TryParse(minerals.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedMinerals) &&
                    selectedStar != null && selectedStar.AsteroidCountOffset >= 0 &&
                    !string.IsNullOrWhiteSpace(graph.Text) && graph.Text.Length <= 128;
                double storedPositionX = parsedPositionX / SavContainer.AsteroidPositionScale;
                double storedPositionY = parsedPositionY / SavContainer.AsteroidPositionScale;
                valid &= !double.IsNaN(storedPositionX) && !double.IsInfinity(storedPositionX) &&
                    !double.IsNaN(storedPositionY) && !double.IsInfinity(storedPositionY) &&
                    Math.Abs(storedPositionX) <= 1.0E15 && Math.Abs(storedPositionY) <= 1.0E15;
                if (!valid)
                {
                    MessageBox.Show(this, "Поля TAsteroid не применены: проверьте систему, координаты, скорость, массу, минералы и имя графики.",
                        "TAsteroid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                asteroid.PositionX = (float)storedPositionX; asteroid.PositionY = (float)storedPositionY;
                asteroid.SpeedX = parsedSpeedX; asteroid.SpeedY = parsedSpeedY;
                asteroid.Mass = parsedMass; asteroid.Minerals = parsedMinerals;
                asteroid.GraphName = graph.Text.Trim();
                asteroid.ParentStarId = selectedStar.ObjectId;
                RefreshGalaxyObjects();
            }
        }

        private void EditMissile(MissileRecord missile)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TMISSILEFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox weaponId = FindControl<TextBox>(form, "edWeaponID");
                ComboBox weaponType = FindControl<ComboBox>(form, "cbWeaponType");
                TextBox techLevel = FindControl<TextBox>(form, "edTechLevel");
                TextBox damageMin = FindControl<TextBox>(form, "edDamageMin");
                TextBox damageMax = FindControl<TextBox>(form, "edDamageMax");
                TextBox positionX = FindControl<TextBox>(form, "edPosX");
                TextBox positionY = FindControl<TextBox>(form, "edPosY");
                TextBox angle = FindControl<TextBox>(form, "edAngle");
                TextBox fromAngle = FindControl<TextBox>(form, "edFromAngle");
                ComboBox star = FindControl<ComboBox>(form, "cbStar");
                ComboBox ship = FindControl<ComboBox>(form, "cbShip");
                ComboBox target = FindControl<ComboBox>(form, "cbTarget");
                TextBox targetType = FindControl<TextBox>(form, "edTargetType");
                TextBox missileNo = FindControl<TextBox>(form, "edMissileNo");
                TextBox live = FindControl<TextBox>(form, "edLive");
                TextBox fromAngleOld = FindControl<TextBox>(form, "edFromAngleOld");
                TextBox speed = FindControl<TextBox>(form, "edSpeed");
                TextBox baseSpeed = FindControl<TextBox>(form, "edBaseSpeed");
                ComboBox targetLost = FindControl<ComboBox>(form, "cbTargetLost");
                TextBox targetLostType = FindControl<TextBox>(form, "edTargetTypeLost");
                TextBox lastPositionX = FindControl<TextBox>(form, "edTargetLastPosX");
                TextBox lastPositionY = FindControl<TextBox>(form, "edTargetLastPosY");
                TextBox lastDistanceMin = FindControl<TextBox>(form, "edTargetLastDistanceMin");
                TextBox bonus = FindControl<TextBox>(form, "edBonusNum");
                TextBox bonusBlock = FindControl<TextBox>(form, "edBonusBlockName");
                TextBox bonusCrc = FindControl<TextBox>(form, "edBonusCRC");
                ComboBox bonusName = FindControl<ComboBox>(form, "cbBonusName");
                TextBox special = FindControl<TextBox>(form, "edSpecialNum");
                TextBox specialBlock = FindControl<TextBox>(form, "edSpecialBlockName");
                TextBox specialCrc = FindControl<TextBox>(form, "edSpecialCRC");
                ComboBox specialName = FindControl<ComboBox>(form, "cbSpecialName");

                foreach (TextBox editor in new TextBox[] { techLevel, damageMin, damageMax, positionX,
                    positionY, angle, fromAngle, missileNo, live, fromAngleOld, speed, baseSpeed,
                    lastPositionX, lastPositionY, lastDistanceMin, bonus, bonusCrc, special, specialCrc })
                    editor.ReadOnly = false;
                PopulateStarCombo(star, missile.StarId);
                PopulateMissileReferenceCombo(ship, 1, missile.ShipId, true);
                PopulateMissileReferenceCombo(target, missile.TargetType, missile.TargetId, false);
                PopulateMissileReferenceCombo(targetLost, missile.TargetLostType, missile.TargetLostId, false);
                target.SelectedIndexChanged += delegate { UpdateMissileReferenceType(target, targetType); };
                targetLost.SelectedIndexChanged += delegate { UpdateMissileReferenceType(targetLost, targetLostType); };
                UpdateMissileReferenceType(target, targetType);
                UpdateMissileReferenceType(targetLost, targetLostType);

                weaponId.Text = missile.WeaponId.ToString(CultureInfo.InvariantCulture);
                for (int type = 0; type <= 96; type++)
                    weaponType.Items.Add(type + " — " + ItemTypeName((byte)type));
                weaponType.SelectedIndex = missile.WeaponType <= 96 ? missile.WeaponType : -1;
                techLevel.Text = missile.TechLevel.ToString(CultureInfo.InvariantCulture);
                damageMin.Text = missile.DamageMin.ToString(CultureInfo.InvariantCulture);
                damageMax.Text = missile.DamageMax.ToString(CultureInfo.InvariantCulture);
                positionX.Text = missile.PositionX.ToString("R", CultureInfo.InvariantCulture);
                positionY.Text = missile.PositionY.ToString("R", CultureInfo.InvariantCulture);
                angle.Text = missile.Angle.ToString("R", CultureInfo.InvariantCulture);
                fromAngle.Text = missile.FromAngle.ToString("R", CultureInfo.InvariantCulture);
                missileNo.Text = missile.MissileNo.ToString(CultureInfo.InvariantCulture);
                live.Text = missile.Live.ToString(CultureInfo.InvariantCulture);
                fromAngleOld.Text = missile.FromAngleOld.ToString("R", CultureInfo.InvariantCulture);
                speed.Text = missile.Speed.ToString("R", CultureInfo.InvariantCulture);
                baseSpeed.Text = missile.BaseSpeed.ToString("R", CultureInfo.InvariantCulture);
                lastPositionX.Text = missile.LastPositionX.ToString("R", CultureInfo.InvariantCulture);
                lastPositionY.Text = missile.LastPositionY.ToString("R", CultureInfo.InvariantCulture);
                lastDistanceMin.Text = missile.LastDistanceMin.ToString("R", CultureInfo.InvariantCulture);
                bonus.Text = missile.Bonus.ToString(CultureInfo.InvariantCulture);
                bonusCrc.Text = missile.BonusReferenceId.ToString("X8", CultureInfo.InvariantCulture);
                special.Text = missile.Special.ToString(CultureInfo.InvariantCulture);
                specialCrc.Text = missile.SpecialReferenceId.ToString("X8", CultureInfo.InvariantCulture);
                bonusBlock.ReadOnly = true; specialBlock.ReadOnly = true;
                PopulateMicroModuleReferenceCombo(bonusName, missile.Bonus,
                    missile.BonusReferenceId, bonus, bonusBlock, bonusCrc);
                PopulateMicroModuleReferenceCombo(specialName, missile.Special,
                    missile.SpecialReferenceId, special, specialBlock, specialCrc);
                form.Text = "Ракета — ID " + missile.ObjectId +
                    (missile.IsCustom ? " — " + missile.CustomWeaponName : string.Empty);
                form.ShowDialog(this);

                byte parsedTech = 0, parsedNo = 0;
                int parsedDamageMin = 0, parsedDamageMax = 0, parsedLive = 0, parsedBonus = 0, parsedSpecial = 0;
                uint parsedBonusCrc = 0, parsedSpecialCrc = 0;
                float parsedPositionX = 0, parsedPositionY = 0, parsedAngle = 0, parsedFromAngle = 0;
                float parsedFromAngleOld = 0, parsedSpeed = 0, parsedBaseSpeed = 0;
                float parsedLastX = 0, parsedLastY = 0, parsedLastDistance = 0;
                StarHeaderRecord selectedStar = star.SelectedItem as StarHeaderRecord;
                MissileReferenceChoice selectedShip = ship.SelectedItem as MissileReferenceChoice;
                MissileReferenceChoice selectedTarget = target.SelectedItem as MissileReferenceChoice;
                MissileReferenceChoice selectedTargetLost = targetLost.SelectedItem as MissileReferenceChoice;
                bool valid = selectedStar != null && selectedShip != null && selectedTarget != null &&
                    selectedTargetLost != null &&
                    byte.TryParse(techLevel.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedTech) &&
                    int.TryParse(damageMin.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedDamageMin) &&
                    int.TryParse(damageMax.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedDamageMax) &&
                    TryParseFiniteFloat(positionX.Text, out parsedPositionX) &&
                    TryParseFiniteFloat(positionY.Text, out parsedPositionY) &&
                    TryParseFiniteFloat(angle.Text, out parsedAngle) && TryParseFiniteFloat(fromAngle.Text, out parsedFromAngle) &&
                    byte.TryParse(missileNo.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedNo) &&
                    int.TryParse(live.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedLive) &&
                    TryParseFiniteFloat(fromAngleOld.Text, out parsedFromAngleOld) &&
                    TryParseFiniteFloat(speed.Text, out parsedSpeed) && TryParseFiniteFloat(baseSpeed.Text, out parsedBaseSpeed) &&
                    TryParseFiniteFloat(lastPositionX.Text, out parsedLastX) &&
                    TryParseFiniteFloat(lastPositionY.Text, out parsedLastY) &&
                    TryParseFiniteFloat(lastDistanceMin.Text, out parsedLastDistance) &&
                    int.TryParse(bonus.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedBonus) &&
                    parsedBonus >= 0 && parsedBonus <= 4096 && TryParseUInt32Flexible(bonusCrc.Text, out parsedBonusCrc) &&
                    int.TryParse(special.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedSpecial) &&
                    parsedSpecial >= 0 && parsedSpecial <= 4096 && TryParseUInt32Flexible(specialCrc.Text, out parsedSpecialCrc) &&
                    (parsedBonus == 0 ? parsedBonusCrc == 0 : parsedBonusCrc != 0) &&
                    (parsedSpecial == 0 ? parsedSpecialCrc == 0 : parsedSpecialCrc != 0);
                if (!valid)
                {
                    MessageBox.Show(this, "Поля TMissile не применены: проверьте числа, ссылки, бонус и спецэффект (CRC допускает 8-значный HEX).",
                        "TMissile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                missile.TechLevel = parsedTech; missile.DamageMin = parsedDamageMin; missile.DamageMax = parsedDamageMax;
                missile.PositionX = parsedPositionX; missile.PositionY = parsedPositionY; missile.Angle = parsedAngle;
                missile.FromAngle = parsedFromAngle; missile.StarId = selectedStar.ObjectId;
                missile.ShipId = selectedShip.ObjectId; missile.TargetType = selectedTarget.Type;
                missile.TargetId = selectedTarget.ObjectId; missile.MissileNo = parsedNo; missile.Live = parsedLive;
                missile.FromAngleOld = parsedFromAngleOld; missile.Speed = parsedSpeed; missile.BaseSpeed = parsedBaseSpeed;
                missile.TargetLostType = selectedTargetLost.Type; missile.TargetLostId = selectedTargetLost.ObjectId;
                missile.LastPositionX = parsedLastX; missile.LastPositionY = parsedLastY;
                missile.LastDistanceMin = parsedLastDistance; missile.Bonus = parsedBonus;
                missile.BonusReferenceId = parsedBonusCrc; missile.Special = parsedSpecial;
                missile.SpecialReferenceId = parsedSpecialCrc;
                RefreshGalaxyObjects();
            }
        }

        private void PopulateMissileReferenceCombo(ComboBox combo, byte selectedType, uint selectedId, bool shipsOnly)
        {
            combo.Items.Clear(); combo.Enabled = true; combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Add(new MissileReferenceChoice(0, 0, "— нет —"));
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    combo.Items.Add(new MissileReferenceChoice(1, ship.ObjectId,
                        "TShip " + ship.ObjectId + "  " + ship.Name));
            if (!shipsOnly)
            {
                if (pendingItems != null)
                    foreach (ItemHeaderRecord item in pendingItems)
                        if (!pendingDeletedItemStarts.Contains(item.Start))
                            combo.Items.Add(new MissileReferenceChoice(2, item.ObjectId,
                                "TItem " + item.ObjectId + "  " + item.Name));
                if (pendingAsteroids != null)
                    foreach (AsteroidRecord asteroid in pendingAsteroids)
                        combo.Items.Add(new MissileReferenceChoice(3, asteroid.ObjectId,
                            "TAsteroid " + asteroid.ObjectId + "  " + asteroid.GraphName));
                if (pendingMissiles != null)
                    foreach (MissileRecord missile in pendingMissiles)
                        combo.Items.Add(new MissileReferenceChoice(4, missile.ObjectId,
                            "TMissile " + missile.ObjectId));
            }
            for (int index = 0; index < combo.Items.Count; index++)
            {
                MissileReferenceChoice choice = combo.Items[index] as MissileReferenceChoice;
                if (choice != null && choice.Type == selectedType && choice.ObjectId == selectedId)
                { combo.SelectedIndex = index; return; }
            }
            combo.Items.Add(new MissileReferenceChoice(selectedType, selectedId,
                "Неизвестная ссылка " + selectedType + ":" + selectedId));
            combo.SelectedIndex = combo.Items.Count - 1;
        }

        private static void UpdateMissileReferenceType(ComboBox combo, TextBox editor)
        {
            MissileReferenceChoice choice = combo.SelectedItem as MissileReferenceChoice;
            editor.Text = choice == null ? "0" : choice.Type.ToString(CultureInfo.InvariantCulture);
        }

        private static bool TryParseUInt32Flexible(string text, out uint value)
        {
            string normalized = (text ?? string.Empty).Trim();
            if (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) normalized = normalized.Substring(2);
            bool looksHex = false;
            foreach (char character in normalized)
                if (character >= 'A' && character <= 'F' || character >= 'a' && character <= 'f')
                { looksHex = true; break; }
            if (looksHex || normalized.Length == 8)
                return uint.TryParse(normalized, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            return uint.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryParseInt32(string text, out int value)
        {
            return int.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out value) ||
                int.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out value);
        }

        private static bool TryParseUInt32(string text, out uint value)
        {
            return uint.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out value) ||
                uint.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out value);
        }

        private static bool TryParseUInt16(string text, out ushort value)
        {
            return ushort.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out value) ||
                ushort.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out value);
        }

        private static bool TryParseByte(string text, out byte value)
        {
            return byte.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out value) ||
                byte.TryParse((text ?? string.Empty).Trim(), NumberStyles.Integer,
                    CultureInfo.CurrentCulture, out value);
        }

        private static bool IsEditableShipGraphName(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3 || value.Length > 128) return false;
            for (int index = 0; index < value.Length; index++)
            {
                char character = value[index];
                if (character < 0x21 || character > 0x7E) return false;
            }
            return true;
        }

        private static TextBox BindEditableText(Control root, string name, string value)
        {
            TextBox editor = FindControl<TextBox>(root, name);
            editor.Enabled = true;
            editor.ReadOnly = false;
            editor.BackColor = SystemColors.Window;
            editor.ForeColor = SystemColors.WindowText;
            editor.Text = value ?? string.Empty;
            return editor;
        }

        private static CheckBox BindEditableCheck(Control root, string name, bool value)
        {
            CheckBox editor = FindControl<CheckBox>(root, name);
            editor.Enabled = true; editor.Checked = value;
            return editor;
        }

        private static CheckBox BindCheckableGroup(Control root, string groupName, bool value)
        {
            GroupBox group = FindControl<GroupBox>(root, groupName);
            if (!group.Text.StartsWith("     ", StringComparison.Ordinal)) group.Text = "     " + group.Text;
            CheckBox editor = new CheckBox();
            editor.Name = groupName + "Check";
            editor.AutoSize = false;
            editor.Size = new Size(15, 15);
            editor.Location = new Point(group.Left + 8, Math.Max(0, group.Top - 1));
            editor.Checked = value;
            editor.Enabled = true;
            group.Parent.Controls.Add(editor);
            editor.BringToFront();
            return editor;
        }

        private static void PopulateByteCombo(ComboBox combo, byte selectedValue, string[] labels)
        {
            combo.Items.Clear(); combo.Enabled = true;
            for (int index = 0; index < labels.Length; index++)
                combo.Items.Add(new ByteValueChoice(checked((byte)index), labels[index]));
            if (selectedValue >= labels.Length)
                combo.Items.Add(new ByteValueChoice(selectedValue,
                    "Неизвестное значение " + selectedValue.ToString(CultureInfo.InvariantCulture)));
            for (int index = 0; index < combo.Items.Count; index++)
            {
                ByteValueChoice choice = combo.Items[index] as ByteValueChoice;
                if (choice != null && choice.Value == selectedValue)
                { combo.SelectedIndex = index; return; }
            }
        }

        private static string ShipTypeDisplayName(byte type)
        {
            string[] names = { "Доминатор", "Рейнджер", "Транспорт", "Пират", "Военный",
                "Транклюкатор", "Военная база", "Научная база", "Бизнес-центр", "Медицинская база",
                "Пиратская база", "Доминаторская база", "Станция", "Клановая база" };
            return type < names.Length
                ? names[type] + " [" + type.ToString(CultureInfo.InvariantCulture) + "]"
                : "Тип " + type.ToString(CultureInfo.InvariantCulture);
        }

        private void PopulatePlanetReferenceCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                {
                    combo.Items.Add(new UInt32ValueChoice(planet.ObjectId,
                        planet.Name + " [ID " + planet.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (planet.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private void PopulateShipReferenceCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                {
                    combo.Items.Add(new UInt32ValueChoice(ship.ObjectId,
                        ship.Name + " [ID " + ship.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (ship.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private void PopulateShipGraphNameCombo(ComboBox combo, string selectedValue)
        {
            combo.Items.Clear(); combo.Enabled = true;
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (gameCatalog != null)
                foreach (string graph in gameCatalog.ShipGraphs)
                    if (!string.IsNullOrWhiteSpace(graph)) names.Add(graph);
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                {
                    if (!string.IsNullOrWhiteSpace(ship.GraphName)) names.Add(ship.GraphName);
                    if (ship.PlayerBridgeRuins != null &&
                        !string.IsNullOrWhiteSpace(ship.PlayerBridgeRuins.GraphName))
                        names.Add(ship.PlayerBridgeRuins.GraphName);
                }
            if (!string.IsNullOrWhiteSpace(selectedValue)) names.Add(selectedValue);
            List<string> ordered = new List<string>(names);
            ordered.Sort(StringComparer.OrdinalIgnoreCase);
            foreach (string graph in ordered) combo.Items.Add(graph);
            combo.Text = selectedValue ?? string.Empty;
        }

        private void PopulatePlanetGraphNameCombo(ComboBox combo, string selectedValue)
        {
            PopulateDirectGraphNameCombo(combo, selectedValue,
                gameCatalog == null ? null : gameCatalog.PlanetGraphs,
                delegate(HashSet<string> names)
                {
                    if (pendingPlanets == null) return;
                    foreach (PlanetHeaderRecord planet in pendingPlanets)
                        if (!string.IsNullOrWhiteSpace(planet.GraphName)) names.Add(planet.GraphName);
                });
        }

        private void PopulateAsteroidGraphNameCombo(ComboBox combo, string selectedValue)
        {
            PopulateDirectGraphNameCombo(combo, selectedValue,
                gameCatalog == null ? null : gameCatalog.AsteroidGraphs,
                delegate(HashSet<string> names)
                {
                    if (pendingAsteroids == null) return;
                    foreach (AsteroidRecord asteroid in pendingAsteroids)
                        if (!string.IsNullOrWhiteSpace(asteroid.GraphName)) names.Add(asteroid.GraphName);
                });
        }

        private static void PopulateDirectGraphNameCombo(ComboBox combo, string selectedValue,
            IEnumerable<string> catalog, Action<HashSet<string>> addObserved)
        {
            combo.Items.Clear(); combo.Enabled = true; combo.DropDownStyle = ComboBoxStyle.DropDown;
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (catalog != null)
                foreach (string graph in catalog)
                    if (!string.IsNullOrWhiteSpace(graph)) names.Add(graph);
            if (addObserved != null) addObserved(names);
            if (!string.IsNullOrWhiteSpace(selectedValue)) names.Add(selectedValue);
            List<string> ordered = new List<string>(names);
            ordered.Sort(StringComparer.OrdinalIgnoreCase);
            foreach (string graph in ordered) combo.Items.Add(graph);
            combo.Text = selectedValue ?? string.Empty;
        }

        private void PopulateStarReferenceCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                {
                    combo.Items.Add(new UInt32ValueChoice(star.ObjectId,
                        LocalizedStarName(star) + " [ID " +
                        star.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (star.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private void PopulateQuestObjectReferenceCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                {
                    combo.Items.Add(new UInt32ValueChoice(ship.ObjectId,
                        "Корабль: " + ship.Name + " [ID " +
                        ship.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (ship.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                {
                    combo.Items.Add(new UInt32ValueChoice(planet.ObjectId,
                        "Планета: " + planet.Name + " [ID " +
                        planet.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (planet.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                {
                    combo.Items.Add(new UInt32ValueChoice(star.ObjectId,
                        (appSettings.LanguageIndex == 1 ? "System: " : "Система: ") +
                        LocalizedStarName(star) + " [ID " +
                        star.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                    if (star.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private void PopulateRuinsReferenceCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            combo.Items.Add(new UInt32ValueChoice(0, "—"));
            int selectedIndex = selectedId == 0 ? 0 : -1;
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (ship.Type >= 6 || (ship.GraphName ?? string.Empty).StartsWith("Ruins.",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        combo.Items.Add(new UInt32ValueChoice(ship.ObjectId,
                            ship.Name + " [ID " + ship.ObjectId.ToString(CultureInfo.InvariantCulture) + "]"));
                        if (ship.ObjectId == selectedId) selectedIndex = combo.Items.Count - 1;
                    }
            if (selectedIndex < 0)
            {
                combo.Items.Add(new UInt32ValueChoice(selectedId,
                    "ID " + selectedId.ToString(CultureInfo.InvariantCulture)));
                selectedIndex = combo.Items.Count - 1;
            }
            combo.SelectedIndex = selectedIndex;
        }

        private static void PopulateProgramGrid(DataGridView grid, int[] values)
        {
            string[] names = { "Вызов Келлера", "Логическое отрицание", "Дематериализация",
                "Энерготрон", "Взлом САБ", "Интерком", "Кораблекрушение", "Блокировка оружия",
                "Безумие", "Шок", "Самоуничтожение", "Отключение" };
            grid.Columns.Clear(); grid.Rows.Clear(); grid.ReadOnly = false;
            grid.AllowUserToAddRows = false; grid.RowHeadersVisible = false; grid.ColumnHeadersVisible = false;
            grid.Columns.Add("Program", "Программа"); grid.Columns.Add("Count", "Значение");
            ConfigurePlayerValueGridColumns(grid, 4.0F, 1.0F);
            for (int index = 0; index < 12; index++)
                grid.Rows.Add(names[index], values[index].ToString(CultureInfo.InvariantCulture));
        }

        private static void PopulatePlayerDominatorKillGrid(DataGridView grid, int[] values)
        {
            grid.Columns.Clear(); grid.Rows.Clear(); grid.ReadOnly = false;
            grid.AllowUserToAddRows = false; grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.Columns.Add("DominatorType", "Тип доминатора");
            grid.Columns.Add("Count", "Уничтожено");
            ConfigurePlayerValueGridColumns(grid, 3.0F, 1.0F);
            for (int index = 0; index < 8; index++)
                grid.Rows.Add("K" + index.ToString(CultureInfo.InvariantCulture),
                    values[index].ToString(CultureInfo.InvariantCulture));
        }

        private static void PopulatePlayerInvestmentGrid(DataGridView grid, int[] values)
        {
            grid.Columns.Clear(); grid.Rows.Clear(); grid.ReadOnly = false;
            grid.AllowUserToAddRows = false; grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.Columns.Add("Investment", "Инвестиция");
            grid.Columns.Add("Value", "Значение");
            ConfigurePlayerValueGridColumns(grid, 3.0F, 1.0F);
            for (int index = 0; index < 12; index++)
                grid.Rows.Add((index + 1).ToString(CultureInfo.InvariantCulture),
                    values[index].ToString(CultureInfo.InvariantCulture));
        }

        private static void ConfigurePlayerValueGridColumns(DataGridView grid,
            float nameWeight, float valueWeight)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Columns[0].ReadOnly = true;
            grid.Columns[0].MinimumWidth = 70;
            grid.Columns[1].MinimumWidth = 54;
            grid.Columns[0].FillWeight = nameWeight;
            grid.Columns[1].FillWeight = valueWeight;
        }

        private void PopulateStarCombo(ComboBox combo, uint selectedId)
        {
            combo.Items.Clear(); combo.Enabled = true;
            if (pendingStars == null) return;
            foreach (StarHeaderRecord star in pendingStars) combo.Items.Add(star);
            for (int index = 0; index < combo.Items.Count; index++)
            {
                StarHeaderRecord star = combo.Items[index] as StarHeaderRecord;
                if (star != null && star.ObjectId == selectedId) { combo.SelectedIndex = index; break; }
            }
        }

        private static bool TryParseCoordinate(string text, out float value)
        {
            bool parsed = float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
                float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
            return parsed && !float.IsNaN(value) && !float.IsInfinity(value) && value >= -10000 && value <= 10000 &&
                (value == 0 || Math.Abs(value) >= 0.001F);
        }

        private static bool TryParseFiniteFloat(string text, out float value)
        {
            bool parsed = float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
                float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
            return parsed && !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static bool TryParseFiniteDouble(string text, out double value)
        {
            bool parsed = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
                double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
            return parsed && !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static void DisplayComboValue(ComboBox combo, string value)
        {
            combo.Items.Clear(); combo.Items.Add(value); combo.SelectedIndex = 0; combo.Enabled = false;
        }

        private StarHeaderRecord FindStarForOffset(int objectOffset)
        {
            if (pendingStars == null) return null;
            for (int index = 0; index < pendingStars.Count; index++)
            {
                int end = index + 1 < pendingStars.Count ? pendingStars[index + 1].Start : int.MaxValue;
                if (objectOffset > pendingStars[index].Start && objectOffset < end) return pendingStars[index];
            }
            return null;
        }

        private string ConstellationName(StarHeaderRecord star)
        {
            if (star == null || current == null) return "—";
            foreach (ConstellationRecord constellation in pendingConstellations ?? current.GalaxyConstellations)
                if (constellation.StarObjectIds.Contains(star.ObjectId))
                    return ConstellationDisplayName(constellation);
            return "—";
        }

        private string ConstellationDisplayName(ConstellationRecord constellation)
        {
            if (constellation == null) return "—";
            string fallback = (appSettings.LanguageIndex == 1 ? "Sector " : "Сектор ") +
                constellation.ObjectId.ToString(CultureInfo.InvariantCulture);
            return gameCatalog == null ? fallback :
                gameCatalog.GetConstellationName(constellation.ObjectId, fallback);
        }

        private string StarName(uint objectId)
        {
            if (objectId == 0) return "—";
            if (pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                    if (star.ObjectId == objectId) return LocalizedStarName(star) + " [" + objectId + "]";
            return "ID " + objectId;
        }

        private string LocalizedStarName(StarHeaderRecord star)
        {
            if (star == null) return string.Empty;
            return gameCatalog == null ? star.Name ?? string.Empty : gameCatalog.GetStarName(star.Name);
        }

        private string PlanetName(uint objectId)
        {
            if (objectId == 0) return "—";
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (planet.ObjectId == objectId) return planet.Name + " [" + objectId + "]";
            return "ID " + objectId;
        }

        private string ShipName(uint objectId)
        {
            if (objectId == 0) return "—";
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (ship.ObjectId == objectId) return ship.Name + " [" + objectId + "]";
            return "ID " + objectId;
        }

        private CustomWeaponInfoRecord FindCustomWeaponInfo(string systemName)
        {
            if (pendingCustomWeapons == null || string.IsNullOrEmpty(systemName)) return null;
            foreach (CustomWeaponInfoRecord weapon in pendingCustomWeapons)
                if (string.Equals(weapon.SystemName, systemName, StringComparison.Ordinal)) return weapon;
            return null;
        }

        private void EditCustomWeaponInfo(CustomWeaponInfoRecord weapon)
        {
            if (weapon == null || pendingCustomWeapons == null || customWeaponList == null) return;
            int index = pendingCustomWeapons.IndexOf(weapon);
            if (index < 0) return;
            customWeaponList.SelectedIndex = index;
            EditSelectedCustomWeapon(this, EventArgs.Empty);
        }

        private void ModsListClicked(object sender, EventArgs e)
        {
            if (pendingGalaxy == null) return;
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TMODSLISTFORM")))
            {
                CheckedListBox list = FindControl<CheckedListBox>(form, "clbModsList");
                TextBox raw = FindControl<TextBox>(form, "mmModsList");
                Label warning = FindControl<Label>(form, "lblModsDeleteWarning");
                Button cfg = FindControl<Button>(form, "btnModsCfg");
                Action fillList = delegate
                {
                    list.Items.Clear();
                    foreach (string item in SplitModList(pendingGalaxy.UsedMods))
                        list.Items.Add(item, true);
                };
                Action saveChecks = delegate { ApplyCheckedModsList(list, pendingGalaxy); };
                fillList();
                list.Visible = true; raw.Visible = false; raw.ReadOnly = true;
                warning.Text = "Снятые отметки удаляют только имена модов из UsedMods; модовые объекты SAV сохраняются.";
                cfg.Text = "ModsCFG";
                cfg.Click += delegate
                {
                    if (list.Visible)
                    {
                        saveChecks();
                        raw.Text = pendingGalaxy.UsedMods ?? string.Empty;
                        list.Visible = false; raw.Visible = true; cfg.Text = "Список модов";
                    }
                    else
                    {
                        raw.Visible = false; list.Visible = true; cfg.Text = "ModsCFG";
                        fillList();
                    }
                };
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) { args.Handled = true; form.Close(); }
                };
                form.ShowDialog(this);
                if (list.Visible) saveChecks();
                values["mods"].Text = pendingGalaxy.UsedModCount.ToString(CultureInfo.InvariantCulture);
                if (current != null) UpdateLegalityAndModsStatus(current);
            }
        }

        private static void ApplyCheckedModsList(CheckedListBox list, GalaxyPrefixData galaxy)
        {
            if (list == null || galaxy == null) return;
            List<string> checkedMods = new List<string>();
            foreach (object item in list.CheckedItems)
            {
                string value = Convert.ToString(item, CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(value)) checkedMods.Add(value.Trim());
            }
            galaxy.UsedMods = string.Join(", ", checkedMods.ToArray());
        }

        private void RefreshGalaxyEventList()
        {
            if (galaxyEventList == null) return;
            galaxyEventList.FormattingEnabled = true;
            galaxyEventList.Format -= GalaxyEventListFormat;
            galaxyEventList.Format += GalaxyEventListFormat;
            galaxyEventList.BeginUpdate(); galaxyEventList.Items.Clear();
            if (pendingGalaxySummary != null)
                foreach (GalaxyEventRecord galaxyEvent in pendingGalaxySummary.GalaxyEvents)
                    galaxyEventList.Items.Add(galaxyEvent);
            galaxyEventList.EndUpdate();
            SetListCountCaption(galaxyEventList, "Галактические события", "Galaxy events");
            if (galaxyEventList.Items.Count > 0) galaxyEventList.SelectedIndex = 0;
        }

        private void GalaxyEventListFormat(object sender, ListControlConvertEventArgs e)
        {
            GalaxyEventRecord value = e.ListItem as GalaxyEventRecord;
            if (value != null)
                e.Value = "[" + value.Turn.ToString(CultureInfo.InvariantCulture) + "] " +
                    (value.Type ?? "TGalaxyEvent");
        }

        private void ViewSelectedGalaxyEvent(object sender, EventArgs e)
        {
            GalaxyEventRecord galaxyEvent = galaxyEventList == null ? null :
                galaxyEventList.SelectedItem as GalaxyEventRecord;
            if (galaxyEvent == null) return;
            using (Form form = EditorFormFactory.Build(
                EditorFormDefinitions.Get("TGALAXYEVENTFORM")))
            {
                TextBox type = FindControl<TextBox>(form, "edType");
                TextBox turn = FindControl<TextBox>(form, "edTurn");
                ListBox data = FindControl<ListBox>(form, "lbData");
                ListBox textData = FindControl<ListBox>(form, "lbTextData");
                type.Text = galaxyEvent.Type ?? string.Empty; type.ReadOnly = true;
                turn.Text = galaxyEvent.Turn.ToString(CultureInfo.InvariantCulture); turn.ReadOnly = true;
                foreach (int value in galaxyEvent.Data)
                    data.Items.Add(value.ToString(CultureInfo.InvariantCulture));
                foreach (string value in galaxyEvent.TextData) textData.Items.Add(value ?? string.Empty);
                form.KeyPreview = true;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };
                form.ShowDialog(this);
            }
        }

        private void DeleteSelectedGalaxyEvents(object sender, EventArgs e)
        {
            if (pendingGalaxySummary == null || galaxyEventList == null ||
                galaxyEventList.SelectedIndices.Count == 0) return;
            List<int> indices = new List<int>();
            foreach (int index in galaxyEventList.SelectedIndices) indices.Add(index);
            indices.Sort();
            for (int position = indices.Count - 1; position >= 0; position--)
                if (indices[position] >= 0 && indices[position] < pendingGalaxySummary.GalaxyEvents.Count)
                    pendingGalaxySummary.GalaxyEvents.RemoveAt(indices[position]);
            pendingGalaxySummary.GalaxyEventCount = pendingGalaxySummary.GalaxyEvents.Count;
            RefreshGalaxyEventList();
        }

        private void RefreshMessageList()
        {
            messageList.BeginUpdate();
            messageList.Items.Clear();
            if (pendingMessages != null)
            {
                for (int index = 0; index < pendingMessages.Count; index++)
                {
                    PlayerMessageRecord message = pendingMessages[index];
                    messageList.Items.Add("[" + MessageTypeName(message.MessageType) + "] " +
                        (message.Text ?? string.Empty));
                }
            }
            messageList.EndUpdate();
            if (messageList.Parent is GroupBox)
                messageList.Parent.Text = (appSettings.LanguageIndex == 1 ? "Player messages: " : "Сообщения игрока: ") +
                    messageList.Items.Count.ToString(CultureInfo.InvariantCulture);
            if (messageList.Items.Count > 0) messageList.SelectedIndex = 0;
            else if (messageText != null) messageText.Clear();
        }

        private string MessageTypeName(byte messageType)
        {
            string[] names = appSettings.LanguageIndex == 1 ? new string[] {
                "Galaxy", "Ether", "ShipPlus", "Quest", "QuestOk", "QuestCancel",
                "Tips", "User", "ShipMinus", "Storage", "Ether2"
            } : new string[] {
                "Галактика", "Эфир", "Корабль+", "Задание", "Задание выполнено",
                "Задание отменено", "Советы", "Пользователь", "Корабль−",
                "Хранилище", "Эфир 2"
            };
            return messageType < names.Length ? names[messageType] :
                messageType.ToString(CultureInfo.InvariantCulture);
        }

        private void MessageSelectionChanged(object sender, EventArgs e)
        {
            int index = messageList.SelectedIndex;
            if (pendingMessages == null || index < 0 || index >= pendingMessages.Count)
            {
                messageText.Clear();
                return;
            }
            RenderGameText(messageText, pendingMessages[index].FormattedText ?? string.Empty);
        }

        private static void RenderGameText(RichTextBox target, string value)
        {
            target.Clear();
            target.SelectionColor = target.ForeColor;
            List<Color> colors = new List<Color>();
            colors.Add(target.ForeColor);
            SortedSet<int> tabStops = new SortedSet<int>();
            int index = 0;
            while (index < (value ?? string.Empty).Length)
            {
                if (value[index] != '<')
                {
                    int next = value.IndexOf('<', index);
                    if (next < 0) next = value.Length;
                    target.SelectionColor = colors[colors.Count - 1];
                    target.AppendText(value.Substring(index, next - index));
                    index = next;
                    continue;
                }
                if (index + 1 < value.Length && value[index + 1] == '<')
                {
                    target.SelectionColor = colors[colors.Count - 1];
                    target.AppendText("<");
                    index += 2;
                    continue;
                }
                int close = value.IndexOf('>', index + 1);
                if (close < 0)
                {
                    target.SelectionColor = colors[colors.Count - 1];
                    target.AppendText(value.Substring(index));
                    break;
                }
                string tag = value.Substring(index + 1, close - index - 1).Trim();
                if (tag.StartsWith("color=", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = tag.Substring(6).Split(',');
                    int red, green, blue;
                    if (parts.Length == 3 && int.TryParse(parts[0], out red) &&
                        int.TryParse(parts[1], out green) && int.TryParse(parts[2], out blue))
                        colors.Add(Color.FromArgb(Math.Max(0, Math.Min(255, red)),
                            Math.Max(0, Math.Min(255, green)), Math.Max(0, Math.Min(255, blue))));
                }
                else if (tag.Equals("/color", StringComparison.OrdinalIgnoreCase))
                {
                    if (colors.Count > 1) colors.RemoveAt(colors.Count - 1);
                }
                else if (tag.StartsWith("td=", StringComparison.OrdinalIgnoreCase))
                {
                    int stop;
                    if (int.TryParse(tag.Substring(3), out stop) && stop > 0)
                    {
                        tabStops.Add(stop);
                        target.SelectionColor = colors[colors.Count - 1];
                        target.AppendText("\t");
                    }
                }
                else if (tag.Equals("br", StringComparison.OrdinalIgnoreCase) ||
                    tag.Equals("br/", StringComparison.OrdinalIgnoreCase))
                {
                    target.AppendText(Environment.NewLine);
                }
                index = close + 1;
            }
            if (tabStops.Count != 0)
            {
                int selectionStart = target.SelectionStart;
                target.SelectAll();
                target.SelectionTabs = new List<int>(tabStops).ToArray();
                target.Select(Math.Min(selectionStart, target.TextLength), 0);
            }
            target.SelectionColor = target.ForeColor;
            target.Select(0, 0);
        }

        private void RefreshCustomWeaponList()
        {
            if (customWeaponList == null) return;
            customWeaponList.BeginUpdate();
            customWeaponList.Items.Clear();
            if (pendingCustomWeapons != null)
                foreach (CustomWeaponInfoRecord weapon in pendingCustomWeapons)
                    customWeaponList.Items.Add(weapon);
            customWeaponList.EndUpdate();
            SetListCountCaption(customWeaponList, "Модифицированное оружие", "Custom weapons");
        }

        private void EditSelectedCustomWeapon(object sender, EventArgs e)
        {
            if (pendingCustomWeapons == null || customWeaponList == null || customWeaponList.SelectedIndex < 0)
                return;
            int selectedIndex = customWeaponList.SelectedIndex;
            CustomWeaponInfoRecord weapon = pendingCustomWeapons[selectedIndex];
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TCUSTOMWEAPONINFOFORM")))
            {
                TextBox systemName = FindControl<TextBox>(form, "edSysName");
                TextBox type = FindControl<TextBox>(form, "edType");
                TextBox techLevel = FindControl<TextBox>(form, "edTechLevel");
                ComboBox techRadius = FindControl<ComboBox>(form, "cbTechRadius");
                TextBox modCost = FindControl<TextBox>(form, "edModCost");
                TextBox minDamage = FindControl<TextBox>(form, "edMinDamage");
                TextBox maxDamage = FindControl<TextBox>(form, "edMaxDamage");
                TextBox averageSize = FindControl<TextBox>(form, "edAvgSize");
                TextBox averageRadius = FindControl<TextBox>(form, "edAvgRadius");
                TextBox speed = FindControl<TextBox>(form, "edSpeed");
                TextBox missileRadius = FindControl<TextBox>(form, "edMissileRadius");
                TextBox missileMinSpeed = FindControl<TextBox>(form, "edMissileMinSpeed");
                TextBox missileMaxSpeed = FindControl<TextBox>(form, "edMissileMaxSpeed");
                TextBox missileChance = FindControl<TextBox>(form, "edMissileChanceToBeHit");
                CheckedListBox damageTypes = FindControl<CheckedListBox>(form, "clbDamageType");
                ComboBox shotType = FindControl<ComboBox>(form, "cbShotType");
                TextBox shotCount = FindControl<TextBox>(form, "edShotCount");
                TextBox attackCount = FindControl<TextBox>(form, "edAttackCount");
                TextBox secondaryRadius = FindControl<TextBox>(form, "edSecondaryDamageRadius");
                TextBox miningFactor = FindControl<TextBox>(form, "edMiningFactor");
                DataGridView damageSet = FindControl<DataGridView>(form, "vleWeaponDamageSet");
                TextBox primarySE = FindControl<TextBox>(form, "edPrimarySE");
                TextBox secondarySE = FindControl<TextBox>(form, "edSecondarySE");
                TextBox areaSE = FindControl<TextBox>(form, "edAreaSE");
                TextBox defaultPalette = FindControl<TextBox>(form, "edDefaultPalette");
                ComboBox availability = FindControl<ComboBox>(form, "cbAvailability");
                ComboBox abWeaponType = FindControl<ComboBox>(form, "cbABWeaponType");
                TextBox rnd = FindControl<TextBox>(form, "edRnd");

                systemName.Text = weapon.SystemName ?? string.Empty;
                systemName.ReadOnly = false;
                statusToolTip.SetToolTip(systemName,
                    "Переименование каскадно обновит все связанные TCustomWeapon и TCustomMissile.");
                type.Text = "TCustomWeaponInfo";
                type.ReadOnly = true;
                rnd.Text = "не сериализуется";
                rnd.ReadOnly = true;
                techLevel.Text = weapon.TechLevel.ToString(CultureInfo.InvariantCulture);
                PopulateByteCombo(techRadius, 8, 12, weapon.TechRadius, "радиус ");
                modCost.Text = FormatSingle(weapon.ModCost);
                minDamage.Text = weapon.MinDamage.ToString(CultureInfo.InvariantCulture);
                maxDamage.Text = weapon.MaxDamage.ToString(CultureInfo.InvariantCulture);
                averageSize.Text = weapon.AverageSize.ToString(CultureInfo.InvariantCulture);
                averageRadius.Text = weapon.AverageRadius.ToString(CultureInfo.InvariantCulture);
                speed.Text = weapon.Speed.ToString(CultureInfo.InvariantCulture);
                missileRadius.Text = weapon.MissileRadius.ToString(CultureInfo.InvariantCulture);
                missileMinSpeed.Text = weapon.MissileMinSpeed.ToString(CultureInfo.InvariantCulture);
                missileMaxSpeed.Text = weapon.MissileMaxSpeed.ToString(CultureInfo.InvariantCulture);
                missileChance.Text = weapon.MissileChanceToBeHit.ToString(CultureInfo.InvariantCulture);

                damageTypes.Items.Clear();
                for (int bit = 0; bit < 21; bit++)
                    damageTypes.Items.Add("Тип урона " + bit.ToString(CultureInfo.InvariantCulture),
                        (weapon.DamageType & (1U << bit)) != 0);
                PopulateByteCombo(shotType, 0, 8, weapon.ShotType, "тип ");
                shotCount.Text = weapon.ShotCount.ToString(CultureInfo.InvariantCulture);
                attackCount.Text = weapon.AttackCount.ToString(CultureInfo.InvariantCulture);
                secondaryRadius.Text = FormatSingle(weapon.SecondaryDamageRadius);
                miningFactor.Text = FormatSingle(weapon.MiningFactor);
                PopulateCustomWeaponDamageGrid(damageSet, weapon.WeaponDamageSet);
                damageSet.EditingControlShowing += delegate(object gridSender, DataGridViewEditingControlShowingEventArgs args)
                {
                    TextBox editor = args.Control as TextBox;
                    if (editor == null) return;
                    editor.KeyPress -= CustomWeaponDamageValueKeyPress;
                    editor.KeyPress += CustomWeaponDamageValueKeyPress;
                };
                primarySE.Text = weapon.PrimarySE ?? string.Empty;
                secondarySE.Text = weapon.SecondarySE ?? string.Empty;
                areaSE.Text = weapon.AreaSE ?? string.Empty;
                defaultPalette.Text = weapon.DefaultPalette.ToString(CultureInfo.InvariantCulture);
                PopulateByteCombo(availability, 0, 10, weapon.Availability, "доступность ");
                PopulateAbWeaponCombo(abWeaponType, weapon.ABWeaponType);

                form.ShowDialog(this);

                byte parsedTechLevel = 0, parsedMissileChance = 0, parsedShotCount = 0, parsedAttackCount = 0;
                int parsedMinDamage = 0, parsedMaxDamage = 0, parsedAverageSize = 0, parsedAverageRadius = 0, parsedSpeed = 0;
                int parsedMissileRadius = 0, parsedMissileMinSpeed = 0, parsedMissileMaxSpeed = 0, parsedDefaultPalette = 0;
                float parsedModCost = 0, parsedSecondaryRadius = 0, parsedMiningFactor = 0;
                byte parsedTechRadius = 0, parsedShotType = 0, parsedAvailability = 0, parsedAbType = 0;
                float[] parsedDamageSet = null;
                string parsedSystemName = systemName.Text ?? string.Empty;
                bool valid = parsedSystemName.Length > 0 && parsedSystemName.Length <= 512 &&
                    parsedSystemName.IndexOf('\0') < 0 &&
                    TryParseByte(techLevel.Text, out parsedTechLevel) &&
                    TrySelectedByte(techRadius, out parsedTechRadius) &&
                    TryParseFiniteFloat(modCost.Text, out parsedModCost) &&
                    TryParseInt32(minDamage.Text, out parsedMinDamage) &&
                    TryParseInt32(maxDamage.Text, out parsedMaxDamage) &&
                    TryParseInt32(averageSize.Text, out parsedAverageSize) &&
                    TryParseInt32(averageRadius.Text, out parsedAverageRadius) &&
                    TryParseInt32(speed.Text, out parsedSpeed) &&
                    TryParseInt32(missileRadius.Text, out parsedMissileRadius) &&
                    TryParseInt32(missileMinSpeed.Text, out parsedMissileMinSpeed) &&
                    TryParseInt32(missileMaxSpeed.Text, out parsedMissileMaxSpeed) &&
                    TryParseByte(missileChance.Text, out parsedMissileChance) &&
                    TrySelectedByte(shotType, out parsedShotType) &&
                    TryParseByte(shotCount.Text, out parsedShotCount) &&
                    TryParseByte(attackCount.Text, out parsedAttackCount) &&
                    TryParseFiniteFloat(secondaryRadius.Text, out parsedSecondaryRadius) &&
                    TryParseFiniteFloat(miningFactor.Text, out parsedMiningFactor) &&
                    TryReadCustomWeaponDamageGrid(damageSet, out parsedDamageSet) &&
                    TryParseInt32(defaultPalette.Text, out parsedDefaultPalette) &&
                    TrySelectedByte(availability, out parsedAvailability) &&
                    TrySelectedByte(abWeaponType, out parsedAbType);
                if (!valid)
                {
                    MessageBox.Show(this, "Изменения не применены: одно из числовых полей имеет недопустимое значение.",
                        "Модифицированное оружие", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string renameError;
                if (!RenameCustomWeaponReferences(weapon, parsedSystemName, out renameError))
                {
                    MessageBox.Show(this, renameError, "Модифицированное оружие",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                uint damageMask = 0;
                for (int bit = 0; bit < Math.Min(21, damageTypes.Items.Count); bit++)
                    if (damageTypes.GetItemChecked(bit)) damageMask |= 1U << bit;
                weapon.TechLevel = parsedTechLevel;
                weapon.TechRadius = parsedTechRadius;
                weapon.ModCost = parsedModCost;
                weapon.MinDamage = parsedMinDamage;
                weapon.MaxDamage = parsedMaxDamage;
                weapon.AverageSize = parsedAverageSize;
                weapon.AverageRadius = parsedAverageRadius;
                weapon.Speed = parsedSpeed;
                weapon.MissileRadius = parsedMissileRadius;
                weapon.MissileMinSpeed = parsedMissileMinSpeed;
                weapon.MissileMaxSpeed = parsedMissileMaxSpeed;
                weapon.MissileChanceToBeHit = parsedMissileChance;
                weapon.DamageType = damageMask;
                weapon.ShotType = parsedShotType;
                weapon.ShotCount = parsedShotCount;
                weapon.AttackCount = parsedAttackCount;
                weapon.SecondaryDamageRadius = parsedSecondaryRadius;
                weapon.MiningFactor = parsedMiningFactor;
                weapon.WeaponDamageSet = parsedDamageSet;
                weapon.PrimarySE = EmptyToNull(primarySE.Text);
                weapon.SecondarySE = EmptyToNull(secondarySE.Text);
                weapon.AreaSE = EmptyToNull(areaSE.Text);
                weapon.DefaultPalette = parsedDefaultPalette;
                weapon.Availability = parsedAvailability;
                weapon.ABWeaponType = parsedAbType;
                RefreshCustomWeaponList();
                customWeaponList.SelectedIndex = selectedIndex;
            }
        }

        private bool RenameCustomWeaponReferences(CustomWeaponInfoRecord weapon,
            string newSystemName, out string error)
        {
            error = null;
            string oldSystemName = weapon == null ? string.Empty : weapon.SystemName ?? string.Empty;
            if (weapon == null || string.IsNullOrEmpty(newSystemName))
            {
                error = "Системное имя оружия не может быть пустым.";
                return false;
            }
            if (string.Equals(oldSystemName, newSystemName, StringComparison.Ordinal)) return true;
            if (pendingCustomWeapons == null || pendingItems == null || pendingMissiles == null)
            {
                error = "Для каскадного переименования загружена неполная модель SAV.";
                return false;
            }
            foreach (CustomWeaponInfoRecord candidate in pendingCustomWeapons)
                if (!object.ReferenceEquals(candidate, weapon) && string.Equals(
                    candidate.SystemName, newSystemName, StringComparison.Ordinal))
                {
                    error = "Оружие с системным именем «" + newSystemName + "» уже существует.";
                    return false;
                }

            weapon.SystemName = newSystemName;
            foreach (ItemHeaderRecord item in pendingItems)
                if (item != null && item.Type == 68 && string.Equals(item.CustomWeaponName,
                    oldSystemName, StringComparison.Ordinal))
                    item.CustomWeaponName = newSystemName;
            foreach (MissileRecord missile in pendingMissiles)
                if (missile != null && missile.IsCustom && string.Equals(missile.CustomWeaponName,
                    oldSystemName, StringComparison.Ordinal))
                    missile.CustomWeaponName = newSystemName;
            return true;
        }

        private void DeleteSelectedCustomWeapon(object sender, EventArgs e)
        {
            if (current == null || pendingCustomWeapons == null || customWeaponList == null ||
                customWeaponList.SelectedIndex < 0) return;
            int selectedIndex = customWeaponList.SelectedIndex;
            CustomWeaponInfoRecord weapon = pendingCustomWeapons[selectedIndex];
            try
            {
                CustomWeaponDeleteResult result = current.DeleteCustomWeaponCascade(
                    weapon.SystemName, pendingCustomWeapons, pendingStars, pendingPlanets,
                    pendingShips, pendingItems, pendingMissiles, pendingStoredItems,
                    pendingGalaxySummary);
                foreach (int start in result.RemovedItemStarts)
                    pendingDeletedItemStarts.Add(start);
                pendingGalaxy.CustomModWeaponCount = pendingCustomWeapons.Count;
                values["mod_weapons"].Text = pendingCustomWeapons.Count.ToString(
                    CultureInfo.InvariantCulture);
                RefreshCustomWeaponList();
                if (customWeaponList.Items.Count > 0)
                    customWeaponList.SelectedIndex = Math.Min(selectedIndex,
                        customWeaponList.Items.Count - 1);
                RefreshObjectLists();
                RefreshStoredItemList();
                if (systemMapForm != null && !systemMapForm.IsDisposed) systemMapForm.Invalidate();
                MessageBox.Show(this, "Удалено объектов TCustomWeapon: " +
                    result.RemovedOwnerRecords.ToString(CultureInfo.InvariantCulture) +
                    "; активных TCustomMissile: " +
                    result.RemovedMissileIds.Count.ToString(CultureInfo.InvariantCulture) + ".",
                    "Модифицированное оружие", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(this, "Каскадное удаление отменено: " + error.Message,
                    "Модифицированное оружие", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static void PopulateByteCombo(ComboBox combo, int first, int count, byte selected, string prefix)
        {
            combo.Items.Clear();
            for (int index = 0; index < count; index++)
            {
                byte value = checked((byte)(first + index));
                combo.Items.Add(new ByteValueChoice(value, prefix + value.ToString(CultureInfo.InvariantCulture)));
            }
            SelectByteCombo(combo, selected);
        }

        private static void PopulateAbWeaponCombo(ComboBox combo, byte selected)
        {
            string[] names = { "Промышленный лазер", "Осколочная пушка", "Флюктуационный излучатель", "Ракетница",
                "Третон", "Волновой фазер", "Потоковый бластер", "Электронный резак", "Мультирезонатор",
                "Атомное зрение", "Дезинтегратор", "Турбогравир", "ИМХО-9000", "Вертикс", "Торпедный аппарат",
                "Эсодафер", "Кафаситор", "Лирекрон" };
            combo.Items.Clear();
            for (int index = 0; index < names.Length; index++)
                combo.Items.Add(new ByteValueChoice(checked((byte)(50 + index)), names[index]));
            SelectByteCombo(combo, selected);
        }

        private static void SelectByteCombo(ComboBox combo, byte selected)
        {
            for (int index = 0; index < combo.Items.Count; index++)
            {
                ByteValueChoice choice = combo.Items[index] as ByteValueChoice;
                if (choice != null && choice.Value == selected) { combo.SelectedIndex = index; return; }
            }
            combo.Items.Add(new ByteValueChoice(selected, "значение " + selected.ToString(CultureInfo.InvariantCulture)));
            combo.SelectedIndex = combo.Items.Count - 1;
        }

        private static bool TrySelectedByte(ComboBox combo, out byte value)
        {
            ByteValueChoice choice = combo.SelectedItem as ByteValueChoice;
            value = choice == null ? (byte)0 : choice.Value;
            return choice != null;
        }

        private static void PopulateCustomWeaponDamageGrid(DataGridView grid, float[] values)
        {
            grid.Columns.Clear(); grid.Rows.Clear(); grid.ReadOnly = false;
            grid.AllowUserToAddRows = false; grid.RowHeadersVisible = false; grid.ColumnHeadersVisible = false;
            grid.Columns.Add("DamageIndex", "№");
            grid.Columns.Add("DamageValue", "Значение");
            grid.Columns[0].ReadOnly = true; grid.Columns[0].Width = 42;
            grid.Columns[1].Width = Math.Max(75, grid.Width - 48);
            for (int index = 0; index < 8; index++)
                grid.Rows.Add(index.ToString(CultureInfo.InvariantCulture),
                    FormatSingle(values != null && index < values.Length ? values[index] : 0F));
        }

        private static void CustomWeaponDamageValueKeyPress(object sender, KeyPressEventArgs args)
        {
            TextBox editor = sender as TextBox;
            if (editor == null) return;
            if (char.IsControl(args.KeyChar) || char.IsDigit(args.KeyChar)) return;
            if (args.KeyChar == '.' && editor.Text.IndexOf('.') < 0) return;
            args.Handled = true;
        }

        private static bool TryReadCustomWeaponDamageGrid(DataGridView grid, out float[] values)
        {
            values = new float[8];
            if (grid.Rows.Count < 8) return false;
            for (int index = 0; index < 8; index++)
                if (!TryParseFiniteFloat(Convert.ToString(grid.Rows[index].Cells[1].Value,
                    CultureInfo.InvariantCulture), out values[index])) return false;
            return true;
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static string FormatSingle(float value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }

        private void RefreshInterfaceOverrideLists()
        {
            if (interfaceOverrideLists == null) return;
            foreach (ListBox list in interfaceOverrideLists) { list.BeginUpdate(); list.Items.Clear(); }
            if (pendingInterfaceOverrides != null)
                foreach (InterfaceOverrideRecord record in pendingInterfaceOverrides)
                {
                    int kind = (int)record.Kind;
                    if (kind >= 0 && kind < interfaceOverrideLists.Length)
                        interfaceOverrideLists[kind].Items.Add(record);
                }
            string[] russian = { "Состояния", "Тексты", "Изображения", "Позиции", "Размеры" };
            string[] english = { "States", "Texts", "Images", "Positions", "Sizes" };
            for (int index = 0; index < interfaceOverrideLists.Length; index++)
            {
                interfaceOverrideLists[index].EndUpdate();
                SetListCountCaption(interfaceOverrideLists[index], russian[index], english[index]);
            }
        }

        private void EditSelectedInterfaceOverride(object sender, EventArgs e)
        {
            ListBox source = sender as ListBox;
            ToolStripItem menuItem = sender as ToolStripItem;
            if (source == null && menuItem != null)
            {
                ContextMenuStrip menu = menuItem.Owner as ContextMenuStrip;
                if (menu != null) source = menu.SourceControl as ListBox;
            }
            if (source == null || source.SelectedItem == null) return;
            InterfaceOverrideRecord record = source.SelectedItem as InterfaceOverrideRecord;
            if (record == null) return;

            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TINTERFACEOVERRIDEFORM")))
            {
                string interfacePage = "ts" + ((int)record.Kind + 1).ToString(CultureInfo.InvariantCulture);
                EditorFormFactory.ConfigureTabPages(form, "pcInterfaceOverride", interfacePage);
                TextBox moduleName = FindControl<TextBox>(form, "edMLName");
                TextBox guiName = FindControl<TextBox>(form, "edGIName");
                TabControl pages = FindControl<TabControl>(form, "pcInterfaceOverride");
                ComboBox newState = FindControl<ComboBox>(form, "cbNewState");
                ComboBox oldState = FindControl<ComboBox>(form, "cbOldState");
                TextBox newText = FindControl<TextBox>(form, "mmNewText");
                TextBox oldText = FindControl<TextBox>(form, "mmOldText");
                TextBox newImage = FindControl<TextBox>(form, "edNewImage");
                TextBox oldImage = FindControl<TextBox>(form, "edOldImage");
                TextBox newX = FindControl<TextBox>(form, "edNewX");
                TextBox newY = FindControl<TextBox>(form, "edNewY");
                TextBox newZ = FindControl<TextBox>(form, "edNewZ");
                TextBox oldX = FindControl<TextBox>(form, "edOldX");
                TextBox oldY = FindControl<TextBox>(form, "edOldY");
                TextBox oldZ = FindControl<TextBox>(form, "edOldZ");
                TextBox newSizeX = FindControl<TextBox>(form, "edNewSizeX");
                TextBox newSizeY = FindControl<TextBox>(form, "edNewSizeY");
                TextBox oldSizeX = FindControl<TextBox>(form, "edOldSizeX");
                TextBox oldSizeY = FindControl<TextBox>(form, "edOldSizeY");

                moduleName.Text = record.ModuleName ?? string.Empty;
                guiName.Text = record.GuiName ?? string.Empty;
                PopulateInterfaceStateCombo(newState, record.NewState);
                PopulateInterfaceStateCombo(oldState, record.OldState);
                newText.Text = record.NewValue ?? string.Empty;
                oldText.Text = record.OldValue ?? string.Empty;
                newImage.Text = record.NewValue ?? string.Empty;
                oldImage.Text = record.OldValue ?? string.Empty;
                newX.Text = record.NewX.ToString(CultureInfo.InvariantCulture);
                newY.Text = record.NewY.ToString(CultureInfo.InvariantCulture);
                newZ.Text = record.NewZ.ToString("R", CultureInfo.InvariantCulture);
                oldX.Text = record.OldX.ToString(CultureInfo.InvariantCulture);
                oldY.Text = record.OldY.ToString(CultureInfo.InvariantCulture);
                oldZ.Text = record.OldZ.ToString("R", CultureInfo.InvariantCulture);
                newSizeX.Text = record.NewX.ToString(CultureInfo.InvariantCulture);
                newSizeY.Text = record.NewY.ToString(CultureInfo.InvariantCulture);
                oldSizeX.Text = record.OldX.ToString(CultureInfo.InvariantCulture);
                oldSizeY.Text = record.OldY.ToString(CultureInfo.InvariantCulture);
                pages.SelectedIndex = 0;

                form.ShowDialog(this);

                if (string.IsNullOrEmpty(moduleName.Text) || string.IsNullOrEmpty(guiName.Text))
                {
                    MessageBox.Show(this, "Имена формы и объекта не должны быть пустыми.", "Интерфейс",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                bool valid = true;
                if (record.Kind == InterfaceOverrideKind.State)
                {
                    byte parsedNew = 0, parsedOld = 0;
                    valid = TrySelectedByte(newState, out parsedNew) && TrySelectedByte(oldState, out parsedOld);
                    if (valid) { record.NewState = parsedNew; record.OldState = parsedOld; }
                }
                else if (record.Kind == InterfaceOverrideKind.Text)
                {
                    record.NewValue = newText.Text; record.OldValue = oldText.Text;
                }
                else if (record.Kind == InterfaceOverrideKind.Image)
                {
                    record.NewValue = newImage.Text; record.OldValue = oldImage.Text;
                }
                else if (record.Kind == InterfaceOverrideKind.Position)
                {
                    int parsedNewX = 0, parsedNewY = 0, parsedOldX = 0, parsedOldY = 0;
                    double parsedNewZ = 0, parsedOldZ = 0;
                    valid = TryParseInt32(newX.Text, out parsedNewX) && TryParseInt32(newY.Text, out parsedNewY) &&
                        TryParseFiniteDouble(newZ.Text, out parsedNewZ) &&
                        TryParseInt32(oldX.Text, out parsedOldX) && TryParseInt32(oldY.Text, out parsedOldY) &&
                        TryParseFiniteDouble(oldZ.Text, out parsedOldZ);
                    if (valid)
                    {
                        record.NewX = parsedNewX; record.NewY = parsedNewY; record.NewZ = parsedNewZ;
                        record.OldX = parsedOldX; record.OldY = parsedOldY; record.OldZ = parsedOldZ;
                    }
                }
                else if (record.Kind == InterfaceOverrideKind.Size)
                {
                    int parsedNewX = 0, parsedNewY = 0, parsedOldX = 0, parsedOldY = 0;
                    valid = TryParseInt32(newSizeX.Text, out parsedNewX) &&
                        TryParseInt32(newSizeY.Text, out parsedNewY) &&
                        TryParseInt32(oldSizeX.Text, out parsedOldX) &&
                        TryParseInt32(oldSizeY.Text, out parsedOldY);
                    if (valid)
                    {
                        record.NewX = parsedNewX; record.NewY = parsedNewY;
                        record.OldX = parsedOldX; record.OldY = parsedOldY;
                    }
                }
                if (!valid)
                {
                    MessageBox.Show(this, "Изменения не применены: неверное числовое значение.", "Интерфейс",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.ModuleName = moduleName.Text;
                record.GuiName = guiName.Text;
                RefreshInterfaceOverrideLists();
                if (source.Items.Contains(record)) source.SelectedItem = record;
            }
        }

        private void DeleteSelectedInterfaceOverrides(object sender, EventArgs e)
        {
            ListBox source = sender as ListBox;
            ToolStripItem menuItem = sender as ToolStripItem;
            if (source == null && menuItem != null)
            {
                ContextMenuStrip menu = menuItem.Owner as ContextMenuStrip;
                if (menu != null) source = menu.SourceControl as ListBox;
            }
            if (source == null || pendingInterfaceOverrides == null ||
                source.SelectedItems.Count == 0) return;
            List<InterfaceOverrideRecord> selected = new List<InterfaceOverrideRecord>();
            foreach (object value in source.SelectedItems)
            {
                InterfaceOverrideRecord record = value as InterfaceOverrideRecord;
                if (record != null) selected.Add(record);
            }
            foreach (InterfaceOverrideRecord record in selected)
                pendingInterfaceOverrides.Remove(record);
            RefreshInterfaceOverrideLists();
        }

        private static void PopulateInterfaceStateCombo(ComboBox combo, byte selected)
        {
            string[] names = { "Не активно", "Активно", "Отключено", "Включено" };
            combo.Items.Clear();
            for (int index = 0; index < names.Length; index++)
                combo.Items.Add(new ByteValueChoice((byte)index, names[index]));
            SelectByteCombo(combo, selected);
        }

        private void RefreshStoredItemList()
        {
            if (storedItemList == null) return;
            storedItemList.BeginUpdate(); storedItemList.Items.Clear();
            if (pendingStoredItems != null)
                foreach (StoredItemRecord record in pendingStoredItems)
                {
                    ItemHeaderRecord item = FindItemByStart(record.ItemStart);
                    string caption = item == null ? MissingItemCaption(record.ItemType, record.ItemObjectId) :
                        StoredItemCaption(item, record.ScriptTag);
                    storedItemList.Items.Add(new SearchResultEntry(record, caption,
                        item == null ? (Color?)null : StoredItemColor(item.Type)));
                }
            storedItemList.EndUpdate();
            SetListCountCaption(storedItemList, "Предметы", "Items");
        }

        private string StoredItemCaption(ItemHeaderRecord item, string scriptTag)
        {
            string name = ItemDisplayName(item);
            string tag = (scriptTag ?? string.Empty).Trim();
            bool hasSerializedName = item != null && !IsTechnicalItemName(item.Name);
            bool isResolvedMicroModule = item != null && item.Type == 71 &&
                gameCatalog != null && gameCatalog.FindMicroModule(item.Bonus,
                    item.BonusReferenceId) != null;
            bool usedTagAsName = false;

            // Script tags are often the only human-readable identity for unnamed
            // mod-created TUseLessItem records (for example Semerenka/Usl_62).
            if (!hasSerializedName && !isResolvedMicroModule && tag.Length != 0)
            {
                name = tag + " — " + ItemTypeName(item.Type);
                usedTagAsName = true;
            }

            string first = name;
            List<string> details = new List<string>();
            bool english = appSettings.LanguageIndex == 1;
            details.Add((english ? "Weight: " : "Вес: ") +
                item.Weight.ToString(CultureInfo.InvariantCulture));
            details.Add((english ? "Price: " : "Цена: ") +
                item.Cost.ToString(CultureInfo.InvariantCulture));
            details.Add("ID: " + item.ObjectId.ToString(CultureInfo.InvariantCulture));
            if (item.Type == 71 && item.Bonus != 0)
                details.Add((english ? "Catalog: " : "Каталог: ") +
                    item.Bonus.ToString(CultureInfo.InvariantCulture));
            if (tag.Length != 0 && !usedTagAsName &&
                !string.Equals(tag, name, StringComparison.OrdinalIgnoreCase))
                details.Add((english ? "Key: " : "Ключ: ") + tag);
            if (!string.IsNullOrWhiteSpace(item.SystemName) &&
                !string.Equals(item.SystemName.Trim(), name, StringComparison.OrdinalIgnoreCase))
                details.Add((english ? "System: " : "Система: ") + item.SystemName.Trim());
            return first + "|    " + string.Join(", ", details.ToArray());
        }

        private static Color? StoredItemColor(byte type)
        {
            if (type == 71) return Color.SteelBlue;
            if (type == 70 || type == 75) return Color.DarkRed;
            if (type == 73) return Color.Olive;
            return null;
        }

        private void EditSelectedStoredItem(object sender, EventArgs e)
        {
            ListBox source = sender as ListBox;
            ToolStripItem menuItem = sender as ToolStripItem;
            if (source == null && menuItem != null)
            {
                ContextMenuStrip menu = menuItem.Owner as ContextMenuStrip;
                if (menu != null) source = menu.SourceControl as ListBox;
            }
            SearchResultEntry selectedEntry = source == null ? null : source.SelectedItem as SearchResultEntry;
            StoredItemRecord record = selectedEntry == null ? null :
                selectedEntry.Value as StoredItemRecord;
            if (record == null) return;
            ItemHeaderRecord item = null;
            if (pendingItems != null)
                foreach (ItemHeaderRecord candidate in pendingItems)
                    if (candidate.Start == record.ItemStart && candidate.Type == record.ItemType &&
                        candidate.ObjectId == record.ItemObjectId)
                    { item = candidate; break; }
            if (item == null)
            {
                MessageBox.Show(this, "Вложенный TItem не найден в структурном каталоге.", "Хранилище",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSTORAGEITEMFORM")))
            {
                TextBox scriptTag = FindControl<TextBox>(form, "edSlot");
                ComboBox place = FindControl<ComboBox>(form, "cbItemPlace");
                Button editItem = FindControl<Button>(form, "btnItemEdit");
                scriptTag.Text = record.ScriptTag ?? string.Empty;
                place.Items.Clear();
                place.Items.Add("Определяется игровыми ссылками / script tag");
                place.SelectedIndex = 0; place.Enabled = false;
                editItem.Text = ItemDisplayName(item) + " — ID " +
                    item.ObjectId.ToString(CultureInfo.InvariantCulture);
                ItemHeaderRecord selectedItem = item;
                editItem.Click += delegate { EditItem(selectedItem); };
                form.ShowDialog(this);
                if (string.IsNullOrWhiteSpace(scriptTag.Text))
                {
                    MessageBox.Show(this, "Script tag не должен быть пустым.", "Хранилище",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.ScriptTag = scriptTag.Text.Trim();
                RefreshStoredItemList();
                foreach (object value in source.Items)
                {
                    SearchResultEntry entry = value as SearchResultEntry;
                    if (entry != null && ReferenceEquals(entry.Value, record))
                    { source.SelectedItem = value; break; }
                }
            }
        }

        private void DeleteSelectedStoredItems(object sender, EventArgs e)
        {
            if (pendingStoredItems == null || storedItemList == null ||
                storedItemList.SelectedItems.Count == 0) return;
            List<StoredItemRecord> selected = new List<StoredItemRecord>();
            foreach (object value in storedItemList.SelectedItems)
            {
                SearchResultEntry entry = value as SearchResultEntry;
                StoredItemRecord record = entry == null ? null : entry.Value as StoredItemRecord;
                if (record != null) selected.Add(record);
            }
            foreach (StoredItemRecord record in selected)
                if (pendingStoredItems.Remove(record)) pendingDeletedItemStarts.Add(record.ItemStart);
            RefreshStoredItemList();
            RefreshObjectLists();
        }

        private static int TotalPlanets(IList<StarHeaderRecord> stars)
        {
            int total = 0;
            if (stars != null)
                foreach (StarHeaderRecord star in stars) total += star.PlanetCount;
            return total;
        }

        private static int TotalCustomShipInfoCount(IList<ShipHeaderRecord> ships)
        {
            int total = 0;
            if (ships != null)
                foreach (ShipHeaderRecord ship in ships)
                    if (ship.CustomShipInfos != null) total += ship.CustomShipInfos.Count;
            return total;
        }

        private string BossStateText(int defeatedTurn)
        {
            if (appSettings.LanguageIndex == 1)
                return defeatedTurn == 0 ? "Alive" : "Defeated (turn " + defeatedTurn + ")";
            return defeatedTurn == 0 ? "Жив" : "Побеждён (ход " + defeatedTurn + ")";
        }

        private void UpdateLegalityAndModsStatus(SavContainer loaded)
        {
            GalaxyPrefixData galaxy = pendingGalaxy ?? loaded.GalaxyPrefix;
            GalaxySummaryData summary = pendingGalaxySummary ?? loaded.GalaxySummary;
            bool legal = !galaxy.Crack && !galaxy.Cheat && galaxy.CheatPoints <= 0 && summary.CheatsTest <= 0;
            int modCount = galaxy.UsedModCount;
            bool english = appSettings.LanguageIndex == 1;
            string modText = modCount == 0
                ? (english ? "without mods" : "без модов")
                : (english ? "with mods (" : "с модами (") + modCount + ")";
            statusLegal.Text = (legal
                ? (english ? "Save — legal · " : "Сохранение — легальное · ")
                : (english ? "Save — illegal · " : "Сохранение — нелегальное · ")) + modText;
            statusLegal.ForeColor = legal ? Color.Green : Color.Red;

            string configDetails;
            string[] saveMods = SplitModList(galaxy.UsedMods);
            string[] activeMods;
            if (TryReadActiveMods(out activeMods))
            {
                bool sameOrder = saveMods.Length == activeMods.Length;
                if (sameOrder)
                    for (int index = 0; index < saveMods.Length; index++)
                        if (!string.Equals(NormalizeModReference(saveMods[index]), NormalizeModReference(activeMods[index]), StringComparison.OrdinalIgnoreCase))
                        {
                            sameOrder = false;
                            break;
                        }
                configDetails = sameOrder
                    ? (english ? "The mod set and order match the current Mods\\ModCFG.txt."
                        : "Набор и порядок модов совпадают с текущим Mods\\ModCFG.txt.")
                    : (english ? "The mod set or order differs from the current Mods\\ModCFG.txt (SAV: "
                        : "Набор или порядок модов отличается от текущего Mods\\ModCFG.txt (в SAV: ") +
                        saveMods.Length + (english ? ", active: " : ", активно: ") + activeMods.Length + ").";
            }
            else
                configDetails = english
                    ? "The current Mods\\ModCFG.txt was not found; mod-set comparison is unavailable."
                    : "Текущий Mods\\ModCFG.txt не найден; сравнение набора модов недоступно.";

            statusToolTip.SetToolTip(statusLegal,
                (legal
                    ? (english ? "Compatibility check: the save is legal."
                        : "Проверка совместимости: сохранение легальное.")
                    : (english ? "Compatibility check: the save is illegal."
                        : "Проверка совместимости: сохранение нелегальное.")) +
                Environment.NewLine + "Crack=" + galaxy.Crack + ", Cheat=" + galaxy.Cheat +
                ", CheatPoints=" + galaxy.CheatPoints + ", Cheats.Test=" + summary.CheatsTest +
                " @ 0x" + summary.CheatsTestOffset.ToString("X") + "." + Environment.NewLine + configDetails);
        }

        private bool TryReadActiveMods(out string[] mods)
        {
            mods = new string[0];
            if (string.IsNullOrEmpty(appSettings.GamePath)) return false;
            string path = Path.Combine(appSettings.GamePath, "Mods", "ModCFG.txt");
            if (!File.Exists(path)) return false;
            try
            {
                foreach (string rawLine in File.ReadAllLines(path))
                {
                    string line = rawLine.Trim();
                    if (!line.StartsWith("CurrentMod=", StringComparison.OrdinalIgnoreCase)) continue;
                    mods = SplitModList(line.Substring(line.IndexOf('=') + 1));
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private static string[] SplitModList(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new string[0];
            string[] raw = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> result = new List<string>(raw.Length);
            foreach (string item in raw)
            {
                string trimmed = item.Trim();
                if (trimmed.Length > 0) result.Add(trimmed);
            }
            return result.ToArray();
        }

        private static string NormalizeModReference(string value)
        {
            return (value ?? string.Empty).Trim().Replace('/', '\\').TrimEnd('\\');
        }

        private void RefreshGalaxyView()
        {
            constellationList.BeginUpdate();
            constellationList.Items.Clear();
            constellationList.Items.Add("00  Все сектора");
            if (pendingConstellations != null)
                foreach (ConstellationRecord constellation in pendingConstellations)
                    constellationList.Items.Add(constellation);
            constellationList.EndUpdate();
            GroupBox constellationGroup = constellationList.Parent as GroupBox;
            if (constellationGroup != null)
                constellationGroup.Text = (appSettings.LanguageIndex == 1 ? "Sectors: " : "Сектора: ") +
                    Math.Max(0, constellationList.Items.Count - 1).ToString(CultureInfo.InvariantCulture);
            if (constellationList.Items.Count > 0)
                // Select the first real sector after inserting the synthetic
                // aggregate row.
                constellationList.SelectedIndex = constellationList.Items.Count > 1 ? 1 : 0;
            else
                PopulateStars(null);
            DrawGalaxyMap();
        }

        private void ConstellationSelectionChanged(object sender, EventArgs e)
        {
            PopulateStars(constellationList.SelectedItem as ConstellationRecord);
        }

        private void ToggleSelectedConstellation(object sender, EventArgs e)
        {
            ConstellationRecord constellation = constellationList.SelectedItem as ConstellationRecord;
            if (constellation == null) return;
            int selected = constellationList.SelectedIndex;
            constellation.Visible = !constellation.Visible;
            constellationList.Items[selected] = constellation;
            constellationList.SelectedIndex = selected;
            DrawGalaxyMap();
        }

        private void PopulateStars(ConstellationRecord constellation)
        {
            starList.BeginUpdate();
            starList.Items.Clear();
            starList.Items.Add("00  Все системы");
            if (pendingStars != null)
                foreach (StarHeaderRecord star in pendingStars)
                    if (constellation == null || constellation.StarObjectIds.Contains(star.ObjectId))
                        starList.Items.Add(star);
            starList.EndUpdate();
            GroupBox starGroup = starList.Parent as GroupBox;
            if (starGroup != null)
                starGroup.Text = (appSettings.LanguageIndex == 1 ? "Stars: " : "Системы: ") +
                    Math.Max(0, starList.Items.Count - 1).ToString(CultureInfo.InvariantCulture);
            starList.SelectedIndex = starList.Items.Count > 1 ? 1 : 0;
        }

        private void ConstellationListFormat(object sender, ListControlConvertEventArgs e)
        {
            ConstellationRecord value = e.ListItem as ConstellationRecord;
            if (value == null)
            {
                if (e.ListItem is string) e.Value = appSettings.LanguageIndex == 1 ? "All" : "Все";
                return;
            }
            string opened = value.Visible ? (appSettings.LanguageIndex == 1 ? " - open" : " - открыт") : string.Empty;
            e.Value = ConstellationDisplayName(value) + " [ID " +
                value.ObjectId.ToString(CultureInfo.InvariantCulture) + "]" + opened;
        }

        private void StarListFormat(object sender, ListControlConvertEventArgs e)
        {
            StarHeaderRecord value = e.ListItem as StarHeaderRecord;
            if (value == null)
            {
                if (e.ListItem is string) e.Value = appSettings.LanguageIndex == 1 ? "All" : "Все";
                return;
            }
            int ships = 0, stations = 0;
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (FindStarForOffset(ship.Start) == value)
                    {
                        if (ship.IsStation) stations++; else ships++;
                    }
            string details = appSettings.LanguageIndex == 1
                ? "Ships: " + ships.ToString(CultureInfo.InvariantCulture) + ", Stations: " + stations.ToString(CultureInfo.InvariantCulture)
                : "Кораблей: " + ships.ToString(CultureInfo.InvariantCulture) + ", Станций: " + stations.ToString(CultureInfo.InvariantCulture);
            e.Value = LocalizedStarName(value) + "|" + details;
        }

        private void RefreshObjectLists()
        {
            if (itemList != null)
            {
                itemList.BeginUpdate();
                itemList.Items.Clear();
                ShipHeaderRecord player = FindPlayerShip();
                if (player != null && player.PlayerStorageItems != null)
                    foreach (PlayerStorageItemRecord record in player.PlayerStorageItems)
                        itemList.Items.Add(new SearchResultEntry(record, StorageItemCaption(record)));
                itemList.EndUpdate();
                SetListCountCaption(itemList, "Предметы на складах", "Storage items");
            }
            if (satelliteList != null)
            {
                satelliteList.BeginUpdate();
                satelliteList.Items.Clear();
                ShipHeaderRecord player = FindPlayerShip();
                if (player != null && player.PlayerSatelliteItems != null)
                    foreach (ShipItemListEntry satellite in player.PlayerSatelliteItems)
                    {
                        ItemHeaderRecord item = FindItemByStart(satellite.ItemStart);
                        satelliteList.Items.Add(new SearchResultEntry(satellite,
                            item == null ? MissingItemCaption(satellite.ItemType,
                                satellite.ItemObjectId) : ItemDisplayCaption(item, null)));
                    }
                satelliteList.EndUpdate();
                SetListCountCaption(satelliteList, "Зонды на планетах", "Satellites on planets");
            }
            if (scriptList != null)
            {
                scriptList.BeginUpdate();
                scriptList.Items.Clear();
                if (pendingGalaxySummary != null)
                    foreach (ScriptRecord script in pendingGalaxySummary.ActiveScripts) scriptList.Items.Add(script);
                scriptList.EndUpdate();
                SetListCountCaption(scriptList, "Активные скрипты", "Active scripts");
            }
            if (globalVariableList != null)
            {
                globalVariableList.BeginUpdate();
                globalVariableList.Items.Clear();
                if (pendingGalaxySummary != null)
                    foreach (ScriptVariableRecord variable in pendingGalaxySummary.GlobalVariables)
                        globalVariableList.Items.Add(variable);
                globalVariableList.EndUpdate();
                SetListCountCaption(globalVariableList, "Глобальные переменные", "Global variables");
            }
            if (scriptCacheList != null)
            {
                scriptCacheList.BeginUpdate();
                scriptCacheList.Items.Clear();
                if (pendingGalaxySummary != null)
                    foreach (ScriptCacheRecord cache in pendingGalaxySummary.ScriptCache)
                        scriptCacheList.Items.Add(cache);
                scriptCacheList.EndUpdate();
                SetListCountCaption(scriptCacheList, "Кэш скриптов", "Script cache");
            }
        }

        private void SetListCountCaption(ListBox list, string russian, string english)
        {
            GroupBox group = list == null ? null : list.Parent as GroupBox;
            if (group == null) return;
            group.Text = (appSettings.LanguageIndex == 1 ? english : russian) + ": " +
                list.Items.Count.ToString(CultureInfo.InvariantCulture);
        }

        private void ScriptCacheListFormat(object sender, ListControlConvertEventArgs e)
        {
            ScriptCacheRecord value = e.ListItem as ScriptCacheRecord;
            if (value != null) e.Value = value.Name ?? string.Empty;
        }

        private void ActiveScriptListFormat(object sender, ListControlConvertEventArgs e)
        {
            ScriptRecord value = e.ListItem as ScriptRecord;
            if (value != null) e.Value = value.Name ?? string.Empty;
        }

        private void GlobalVariableListFormat(object sender, ListControlConvertEventArgs e)
        {
            ScriptVariableRecord value = e.ListItem as ScriptVariableRecord;
            if (value != null) e.Value = ScriptVariableCaption(value);
        }

        private static string ScriptVariableCaption(ScriptVariableRecord value)
        {
            string type;
            string data = string.Empty;
            switch (value.Type)
            {
                case 0: type = "null"; break;
                case 1: type = "int"; data = " = " + value.IntegerValue.ToString(CultureInfo.InvariantCulture); break;
                case 2: type = "dword"; data = " = " + unchecked((uint)value.IntegerValue).ToString(CultureInfo.InvariantCulture); break;
                case 3: type = "float"; data = " = " + value.DoubleValue.ToString("G", CultureInfo.InvariantCulture); break;
                case 4: type = "string"; data = " = " + (value.StringValue ?? string.Empty); break;
                case 6: type = "dllLibraryFunction"; data = string.IsNullOrEmpty(value.StringValue) ? string.Empty : " = " + value.StringValue; break;
                case 9: type = "array [" + (value.ArrayValue == null ? 0 : value.ArrayValue.Count).ToString(CultureInfo.InvariantCulture) + "]"; break;
                default: type = "type " + value.Type.ToString(CultureInfo.InvariantCulture); break;
            }
            return (value.Name ?? string.Empty) + "|    " + type + data;
        }

        private string StorageItemCaption(PlayerStorageItemRecord record)
        {
            string place = record.IsStation ? "Станция" : "Планета";
            if (record.IsStation && pendingShips != null)
                foreach (ShipHeaderRecord station in pendingShips)
                    if (station.IsStation && station.ObjectId == record.PlaceObjectId &&
                        !string.IsNullOrWhiteSpace(station.Name)) { place = station.Name; break; }
            if (!record.IsStation && pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (planet.ObjectId == record.PlaceObjectId && !string.IsNullOrWhiteSpace(planet.Name))
                    { place = planet.Name; break; }
            ItemHeaderRecord item = FindItemByStart(record.ItemStart);
            string context = place + ", слот " + record.Slot.ToString(CultureInfo.InvariantCulture);
            return item == null ? context + " | " + MissingItemCaption(record.ItemType, record.ItemObjectId) :
                ItemDisplayCaption(item, context);
        }

        private void RefreshModInfoLists()
        {
            if (modInfoShipList != null)
            {
                modInfoShipList.BeginUpdate(); modInfoShipList.Items.Clear();
                if (modInfoShipsEnabled == null || modInfoShipsEnabled.Checked)
                    if (pendingShips != null)
                        foreach (ShipHeaderRecord ship in pendingShips)
                            foreach (CustomShipInfoRecord record in ship.CustomShipInfos)
                                modInfoShipList.Items.Add(new ModInfoShipEntry(ship, record));
                modInfoShipList.EndUpdate();
                modInfoShipList.Enabled = modInfoShipsEnabled == null || modInfoShipsEnabled.Checked;
                SetListCountCaption(modInfoShipList,
                    "[Сортировка] Инфо на кораблях", "[Sorted] Info on ships");
            }
            if (modInfoStarList != null)
            {
                modInfoStarList.BeginUpdate(); modInfoStarList.Items.Clear();
                if (pendingStars != null)
                    foreach (StarHeaderRecord star in pendingStars)
                        foreach (CustomSystemInfoRecord record in star.CustomSystemInfos)
                            modInfoStarList.Items.Add(new ModInfoStarEntry(star, record));
                modInfoStarList.EndUpdate();
                SetListCountCaption(modInfoStarList, "Инфо на звездах", "Info on stars");
            }
        }

        private void ModInfoShipsEnabledChanged(object sender, EventArgs e)
        {
            RefreshModInfoLists();
        }

        private void EditSelectedModInfoShip(object sender, EventArgs e)
        {
            ModInfoShipEntry selected = modInfoShipList == null ? null : modInfoShipList.SelectedItem as ModInfoShipEntry;
            if (selected != null && selected.Owner.CustomShipInfos.Contains(selected.Record) &&
                EditCustomShipInfo(selected.Record, this))
                RefreshModInfoLists();
        }

        private void DeleteSelectedModInfoShips(object sender, EventArgs e)
        {
            if (modInfoShipList == null || modInfoShipList.SelectedItems.Count == 0) return;
            List<ModInfoShipEntry> selected = new List<ModInfoShipEntry>();
            foreach (object value in modInfoShipList.SelectedItems)
            {
                ModInfoShipEntry entry = value as ModInfoShipEntry;
                if (entry != null) selected.Add(entry);
            }
            foreach (ModInfoShipEntry entry in selected)
                entry.Owner.CustomShipInfos.Remove(entry.Record);
            RefreshModInfoLists();
        }

        private void EditSelectedModInfoStar(object sender, EventArgs e)
        {
            ModInfoStarEntry selected = modInfoStarList == null ? null : modInfoStarList.SelectedItem as ModInfoStarEntry;
            if (selected == null || !selected.Owner.CustomSystemInfos.Contains(selected.Record)) return;
            EditCustomSystemInfo(selected.Record);
            RefreshModInfoLists();
        }

        private void DeleteSelectedModInfoStars(object sender, EventArgs e)
        {
            if (modInfoStarList == null || modInfoStarList.SelectedItems.Count == 0) return;
            List<ModInfoStarEntry> selected = new List<ModInfoStarEntry>();
            foreach (object value in modInfoStarList.SelectedItems)
            {
                ModInfoStarEntry entry = value as ModInfoStarEntry;
                if (entry != null) selected.Add(entry);
            }
            foreach (ModInfoStarEntry entry in selected)
                entry.Owner.CustomSystemInfos.Remove(entry.Record);
            RefreshModInfoLists();
        }

        private void StarSelectionChanged(object sender, EventArgs e)
        {
            RefreshGalaxyObjects();
        }

        private void GalaxyFilterChanged(object sender, EventArgs e)
        {
            RefreshGalaxyObjects();
        }

        private void GalaxyObjectMasterChanged(object sender, EventArgs e)
        {
            SetGalaxyFilterGroup(new string[] { "planets", "stations", "equipment", "goods", "useless", "nods", "missiles", "asteroids", "holes" },
                galaxyObjectMaster.Checked);
        }

        private void GalaxyShipMasterChanged(object sender, EventArgs e)
        {
            SetGalaxyFilterGroup(new string[] { "rangers", "warriors", "flagships", "transports", "liners", "diplomats", "pirates", "clanpirates", "tranclucators", "dominators", "bertors", "bosses" },
                galaxyShipMaster.Checked);
        }

        private void SetGalaxyFilterGroup(string[] keys, bool value)
        {
            foreach (string key in keys)
            {
                CheckBox box;
                if (galaxyFilters.TryGetValue(key, out box) && box.Enabled)
                    box.Checked = value;
            }
            RefreshGalaxyObjects();
        }

        private bool GalaxyFilterEnabled(string key)
        {
            CheckBox box;
            return galaxyFilters.TryGetValue(key, out box) && box.Checked;
        }

        private void RefreshGalaxyObjects()
        {
            galaxyObjectList.BeginUpdate();
            galaxyObjectList.Items.Clear();
            if (pendingStars == null || starList.SelectedIndex < 0)
            {
                galaxyObjectList.EndUpdate();
                SetListCountCaption(galaxyObjectList, "Объекты", "Objects | Number of objects");
                return;
            }

            List<StarHeaderRecord> selectedStars = new List<StarHeaderRecord>();
            StarHeaderRecord selected = starList.SelectedItem as StarHeaderRecord;
            if (selected != null)
                selectedStars.Add(selected);
            else
                foreach (object value in starList.Items)
                {
                    StarHeaderRecord star = value as StarHeaderRecord;
                    if (star != null) selectedStars.Add(star);
                }
            HashSet<uint> selectedStarIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in selectedStars) selectedStarIds.Add(star.ObjectId);

            List<PlanetHeaderRecord> planets = new List<PlanetHeaderRecord>();
            List<ShipHeaderRecord> stations = new List<ShipHeaderRecord>();
            List<ShipHeaderRecord> ships = new List<ShipHeaderRecord>();
            List<ItemHeaderRecord> items = new List<ItemHeaderRecord>();
            List<MissileRecord> missiles = new List<MissileRecord>();
            List<AsteroidRecord> asteroids = new List<AsteroidRecord>();
            List<HoleRecord> holes = new List<HoleRecord>();
            if (pendingPlanets != null && GalaxyFilterEnabled("planets"))
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (BelongsToSelectedStar(planet.Start, selectedStarIds)) planets.Add(planet);
            if (pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if (BelongsToSelectedStar(ship.Start, selectedStarIds))
                    {
                        if (ship.IsStation)
                        {
                            if (GalaxyFilterEnabled("stations")) stations.Add(ship);
                        }
                        else if (ShipPassesFilter(ship)) ships.Add(ship);
                    }
            if (pendingItems != null)
            {
                Dictionary<int, ItemHeaderRecord> liveItems =
                    new Dictionary<int, ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in pendingItems)
                    if (!pendingDeletedItemStarts.Contains(item.Start)) liveItems[item.Start] = item;
                HashSet<int> listedItemStarts = new HashSet<int>();
                foreach (StarHeaderRecord star in selectedStars)
                    if (star.SpaceItems != null)
                        foreach (ShipItemListEntry entry in star.SpaceItems)
                        {
                            ItemHeaderRecord item;
                            if (entry != null && listedItemStarts.Add(entry.ItemStart) &&
                                liveItems.TryGetValue(entry.ItemStart, out item) &&
                                ItemPassesFilter(item)) items.Add(item);
                        }
            }
            if (pendingMissiles != null && GalaxyFilterEnabled("missiles"))
                foreach (MissileRecord missile in pendingMissiles)
                    if (selectedStarIds.Contains(missile.ParentStarId)) missiles.Add(missile);
            if (pendingAsteroids != null && GalaxyFilterEnabled("asteroids"))
                foreach (AsteroidRecord asteroid in pendingAsteroids)
                    if (selectedStarIds.Contains(asteroid.ParentStarId)) asteroids.Add(asteroid);
            if (pendingHoles != null && GalaxyFilterEnabled("holes"))
                foreach (HoleRecord hole in pendingHoles)
                    if (selectedStarIds.Contains(hole.FromStarId) || selectedStarIds.Contains(hole.ToStarId)) holes.Add(hole);

            AddGalaxySection("Планеты", planets);
            AddGalaxySection("Станции", stations);
            AddGalaxySection("Корабли", ships);
            AddGalaxySection("Предметы", items);
            AddGalaxySection("Ракеты", missiles);
            AddGalaxySection("Астероиды", asteroids);
            AddGalaxySection("Чёрные дыры", holes);
            galaxyObjectList.EndUpdate();
            GroupBox group = galaxyObjectList.Parent as GroupBox;
            if (group != null)
            {
                int count = planets.Count + stations.Count + ships.Count + items.Count + missiles.Count + asteroids.Count + holes.Count;
                group.Text = appSettings.LanguageIndex == 1
                    ? "Objects | Number of objects: " + count.ToString(CultureInfo.InvariantCulture)
                    : "Объекты | Количество объектов: " + count.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void GalaxyObjectFormat(object sender, ListControlConvertEventArgs e)
        {
            e.Value = GalaxyObjectCaption(e.ListItem);
        }

        private string GalaxyObjectCaption(object value)
        {
            PlanetHeaderRecord planet = value as PlanetHeaderRecord;
            if (planet != null) return PlanetListCaption(planet);
            ShipHeaderRecord ship = value as ShipHeaderRecord;
            if (ship != null) return ShipListCaption(ship);
            ItemHeaderRecord item = value as ItemHeaderRecord;
            return item == null ? (value == null ? string.Empty : value.ToString()) :
                ItemDisplayCaption(item, null);
        }

        private string PlanetListCaption(PlanetHeaderRecord planet)
        {
            string first = (appSettings.LanguageIndex == 1 ? "Planet: " : "Планета: ") +
                (string.IsNullOrWhiteSpace(planet.Name) ? "ID " + planet.ObjectId.ToString(CultureInfo.InvariantCulture) : planet.Name);
            if (planet.Owner == 6)
            {
                string surface = appSettings.LanguageIndex == 1
                    ? "Water: {0}/{1}, Land: {2}/{3}, Mountains: {4}/{5}"
                    : "Вода: {0}/{1}, Суша: {2}/{3}, Горы: {4}/{5}";
                return first + "|" + string.Format(CultureInfo.InvariantCulture, surface,
                    planet.WaterSpace - planet.WaterSpaceDone, planet.WaterSpace,
                    planet.LandSpace - planet.LandSpaceDone, planet.LandSpace,
                    planet.HillSpace - planet.HillSpaceDone, planet.HillSpace);
            }
            string[] economiesRu = { "Аграрная", "Смешанная", "Индустриальная" };
            string[] economiesEn = { "Agriculture", "Mixed", "Industrial" };
            string[] governmentsRu = { "Анархия", "Диктатура", "Монархия", "Республика", "Демократия" };
            string[] governmentsEn = { "Anarchy", "Dictatorship", "Monarchy", "Republic", "Democracy" };
            string economy = planet.Economy < 3 ? (appSettings.LanguageIndex == 1 ? economiesEn[planet.Economy] : economiesRu[planet.Economy]) : planet.Economy.ToString(CultureInfo.InvariantCulture);
            string government = planet.Government < 5 ? (appSettings.LanguageIndex == 1 ? governmentsEn[planet.Government] : governmentsRu[planet.Government]) : planet.Government.ToString(CultureInfo.InvariantCulture);
            byte techLevel = planet.OpenInventions != null && planet.OpenInventions.Length > 7
                ? planet.OpenInventions[7] : (byte)0;
            if (techLevel > 0) techLevel--;
            uint population = (uint)Math.Round(planet.PeopleCount / 1000.0,
                MidpointRounding.AwayFromZero);
            string second = appSettings.LanguageIndex == 1
                ? "Tech level: " + techLevel.ToString(CultureInfo.InvariantCulture) + ", Government: " + government + ", Economy: " + economy + ", Population: " + population.ToString(CultureInfo.InvariantCulture) + " M"
                : "Тех. уровень: " + techLevel.ToString(CultureInfo.InvariantCulture) + ", Правление: " + government + ", Экономика: " + economy + ", Население: " + population.ToString(CultureInfo.InvariantCulture) + " млн";
            return first + "|" + second;
        }

        private string ShipListCaption(ShipHeaderRecord ship)
        {
            string className = ShipClassName(ship);
            string first = className + ": " + (string.IsNullOrWhiteSpace(ship.Name)
                ? "ID " + ship.ObjectId.ToString(CultureInfo.InvariantCulture) : ship.Name);
            int currentHull, maximumHull;
            string hull = TryGetShipHull(ship, out currentHull, out maximumHull)
                ? currentHull.ToString(CultureInfo.InvariantCulture) + "/" + maximumHull.ToString(CultureInfo.InvariantCulture)
                : "-";
            if (ship.IsStation)
                return first + "|" + (appSettings.LanguageIndex == 1 ? "Hull: " : "Корпус: ") + hull;
            string order = ShipOrderListName(ship);
            return first + "|" + (appSettings.LanguageIndex == 1 ? "Hull: " : "Корпус: ") + hull +
                (appSettings.LanguageIndex == 1 ? ", Order: " : ", Приказ: ") + order;
        }

        private string ShipClassName(ShipHeaderRecord ship)
        {
            string[] ru = { "Доминатор", "Рейнджер", "Транспорт", "Пират", "Военный", "Транклюкатор",
                "Военная база", "Научная база", "Бизнес-центр", "Медицинская база", "Пиратская база",
                "База доминаторов", "Станция", "Клановая база" };
            string[] en = { "Dominator", "Ranger", "Transport", "Pirate", "Warrior", "Tranclucator",
                "Military Base", "Science Base", "Business Center", "Medical Base", "Pirate Base",
                "Dominator Base", "Station", "Clan Base" };
            return ship.Type < ru.Length ? (appSettings.LanguageIndex == 1 ? en[ship.Type] : ru[ship.Type]) :
                (appSettings.LanguageIndex == 1 ? "Ship" : "Корабль");
        }

        private bool TryGetShipHull(ShipHeaderRecord ship, out int currentHull, out int maximumHull)
        {
            currentHull = 0; maximumHull = 0;
            if (ship.EquipmentItems == null) return false;
            foreach (ShipItemListEntry entry in ship.EquipmentItems)
            {
                ItemHeaderRecord item = FindItemByStart(entry.ItemStart);
                if (item == null || item.Type != 42) continue;
                ItemDerivedField hitPoints = FindItemDerivedField(item, "edHitPoints");
                if (hitPoints == null || hitPoints.IntegerValue <= 0 || hitPoints.IntegerValue > int.MaxValue) return false;
                maximumHull = (int)hitPoints.IntegerValue;
                float strength = item.Strength;
                if (strength >= 0F && strength <= 1.01F)
                    currentHull = (int)Math.Round(maximumHull * Math.Min(1F, strength));
                else if (strength >= 0F && strength <= 100.01F)
                    currentHull = (int)Math.Round(maximumHull * Math.Min(100F, strength) / 100F);
                else currentHull = maximumHull;
                return true;
            }
            return false;
        }

        private string ShipOrderListName(ShipHeaderRecord ship)
        {
            string[] ru = { "свободен", "движется", "садится", "прыгает", "летит к ЧД", "взлетает", "следует", "телепортируется" };
            string[] en = { "free", "moving", "landing", "jumping", "entering a black hole", "taking off", "following", "teleporting" };
            string result = ship.OrderType < ru.Length ? (appSettings.LanguageIndex == 1 ? en[ship.OrderType] : ru[ship.OrderType]) : ship.OrderType.ToString(CultureInfo.InvariantCulture);
            if (ship.OrderType == 2)
            {
                PlanetHeaderRecord planet = FindPlanetById(ship.OrderObjectId & 0x7fffffffU);
                if (planet != null) result += (appSettings.LanguageIndex == 1 ? " on planet " : " на планету ") + planet.Name;
            }
            return result;
        }

        private bool BelongsToSelectedStar(int offset, HashSet<uint> starIds)
        {
            StarHeaderRecord parent = FindStarForOffset(offset);
            return parent != null && starIds.Contains(parent.ObjectId);
        }

        private bool ShipPassesFilter(ShipHeaderRecord ship)
        {
            string key = ShipFilterKey(ship, pendingGalaxySummary);
            return key != null && GalaxyFilterEnabled(key);
        }

        private static string ShipFilterKey(ShipHeaderRecord ship, GalaxySummaryData summary)
        {
            if (ship == null || ship.IsStation) return null;
            if (ship.IsPlayer || summary != null &&
                (ship.ObjectId == summary.PlayerObjectId || ship.ObjectId == summary.BlazerObjectId ||
                 ship.ObjectId == summary.KellerObjectId || ship.ObjectId == summary.TerronObjectId))
                return "bosses";
            switch (ship.Type)
            {
                case 0: return ship.DominatorType == 6 ? "bertors" : "dominators";
                case 1: return "rangers";
                case 2:
                    if (ship.TransportType == 0) return "transports";
                    if (ship.TransportType == 1) return "liners";
                    if (ship.TransportType == 2) return "diplomats";
                    return null;
                case 3: return ship.PirateType == 0 ? "pirates" : "clanpirates";
                case 4: return ship.WarriorType == 0 ? "warriors" : "flagships";
                case 5: return "tranclucators";
                default: return null;
            }
        }

        private bool ItemPassesFilter(ItemHeaderRecord item)
        {
            string key = ItemFilterKey(item == null ? (byte)255 : item.Type);
            return key != null && GalaxyFilterEnabled(key);
        }

        private static string ItemFilterKey(byte type)
        {
            if (type <= 7) return "goods";
            if (type == 69) return "nods";
            if (type == 70 || type == 72 || type == 74 || type == 75) return "useless";
            if (type >= 8 && type <= 68 || type == 71) return "equipment";
            return null;
        }

        private void AddGalaxySection<T>(string caption, IList<T> records)
        {
            if (records.Count == 0) return;
            galaxyObjectList.Items.Add("──────── " + caption + " [" + records.Count + "] ────────");
            foreach (T record in records) galaxyObjectList.Items.Add(record);
        }

        private void EditSelectedStar(object sender, EventArgs e)
        {
            StarHeaderRecord star = starList.SelectedItem as StarHeaderRecord;
            if (star == null) return;
            EditStar(star);
        }

        private void EditStar(StarHeaderRecord star)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSTARFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox name = BindEditableText(form, "edStarName", star.Name);
                TextBox x = BindEditableText(form, "edPosX", star.X.ToString("R", CultureInfo.InvariantCulture));
                TextBox y = BindEditableText(form, "edPosY", star.Y.ToString("R", CultureInfo.InvariantCulture));
                TextBox radius = BindEditableText(form, "edRadius", star.Raw1C.ToString(CultureInfo.InvariantCulture));
                TextBox background = BindEditableText(form, "edSystemBackground", star.Raw78.ToString(CultureInfo.InvariantCulture));
                TextBox mapLabel = BindEditableText(form, "edMapLabel", star.MapLabel ?? string.Empty);
                TextBox customFaction = BindEditableText(form, "edCustomFaction", star.CustomFaction ?? string.Empty);
                TextBox safety = BindEditableText(form, "edSafety", star.Safety.ToString(CultureInfo.InvariantCulture));
                TextBox safeRadius = BindEditableText(form, "edSafeRadius", star.SafeRadius.ToString("R", CultureInfo.InvariantCulture));
                TextBox damageRadius = BindEditableText(form, "edDamageRadius", star.DamageRadius.ToString("R", CultureInfo.InvariantCulture));
                TextBox graphRadius = BindEditableText(form, "edGraphRadius", star.GraphRadius.ToString(CultureInfo.InvariantCulture));
                TextBox dayBeforeOccupy = BindEditableText(form, "edDayBeforeOccupy", star.DayBeforeOccupy.ToString(CultureInfo.InvariantCulture));
                TextBox dayWithoutPlayer = BindEditableText(form, "edDayWithoutPlayer", star.DayWithoutPlayer.ToString(CultureInfo.InvariantCulture));
                TextBox dayWithoutCreateShip = BindEditableText(form, "edDayWithoutCreateShip", star.DayWithoutCreateShip.ToString(CultureInfo.InvariantCulture));
                TextBox lastDominatorDate = BindEditableText(form, "edLastDominatorDate", star.LastDominatorDate.ToString(CultureInfo.InvariantCulture));
                TextBox lastPirateDate = BindEditableText(form, "edLastPirateDate", star.LastPirateDate.ToString(CultureInfo.InvariantCulture));
                TextBox liberationDate = BindEditableText(form, "edLiberationDate", star.LiberationDate.ToString(CultureInfo.InvariantCulture));
                TextBox dayInvadeInertia = BindEditableText(form, "edDayInvadeInertia", star.DayInvadeInertia.ToString(CultureInfo.InvariantCulture));
                CheckBox battle = BindEditableCheck(form, "chbBattle", star.Battle);
                CheckBox noComeKling = BindEditableCheck(form, "chbNoComeKling", star.NoComeKling);

                ComboBox graphStar = FindControl<ComboBox>(form, "cbGraphStar");
                graphStar.Enabled = true; graphStar.Items.Clear();
                if (pendingStars != null)
                    foreach (StarHeaderRecord candidate in pendingStars)
                        if (!string.IsNullOrEmpty(candidate.GraphStar) && !graphStar.Items.Contains(candidate.GraphStar))
                            graphStar.Items.Add(candidate.GraphStar);
                graphStar.Text = star.GraphStar ?? string.Empty;

                ComboBox owners = FindControl<ComboBox>(form, "cbOwners");
                ComboBox lastOwners = FindControl<ComboBox>(form, "cbLastOwners");
                ComboBox dominatorSeries = FindControl<ComboBox>(form, "cbDominatorSeries");
                PopulateByteCombo(owners, star.Owners, new string[] { "Коалиция", "Доминаторы", "Пираты" });
                PopulateByteCombo(lastOwners, star.LastOwners, new string[] { "Коалиция", "Доминаторы", "Пираты" });
                PopulateByteCombo(dominatorSeries, star.DominatorSeries, new string[] { "Блазер", "Келлер", "Террон" });
                ComboBox dominion = FindControl<ComboBox>(form, "cbDominion");
                PopulateRuinsReferenceCombo(dominion, star.DominionObjectId);

                List<CustomSystemInfoRecord> editableInfos = new List<CustomSystemInfoRecord>();
                foreach (CustomSystemInfoRecord record in star.CustomSystemInfos) editableInfos.Add(record.Clone());
                ListBox customInfos = FindControl<ListBox>(form, "lbCustomStarInfo");
                Action refreshCustomInfos = delegate
                {
                    customInfos.Items.Clear();
                    foreach (CustomSystemInfoRecord record in editableInfos) customInfos.Items.Add(record);
                };
                EventHandler editCustomInfo = delegate
                {
                    CustomSystemInfoRecord record = customInfos.SelectedItem as CustomSystemInfoRecord;
                    if (record == null) return;
                    EditCustomSystemInfo(record);
                    int selected = customInfos.SelectedIndex; refreshCustomInfos();
                    if (selected >= 0 && selected < customInfos.Items.Count) customInfos.SelectedIndex = selected;
                };
                refreshCustomInfos(); customInfos.DoubleClick += editCustomInfo;
                ContextMenuStrip customMenu = new ContextMenuStrip();
                customMenu.Items.Add("Редактировать", null, editCustomInfo);
                customMenu.Items.Add("Удалить", null, delegate
                {
                    int selected = customInfos.SelectedIndex;
                    if (selected < 0) return;
                    editableInfos.RemoveAt(selected); refreshCustomInfos();
                });
                customInfos.ContextMenuStrip = customMenu;
                customInfos.Enabled = true;

                ListBox dropItems = FindControl<ListBox>(form, "lbItemsDrop");
                Action refreshDropItems = delegate
                {
                    dropItems.Items.Clear();
                    foreach (StarDropItemRecord record in star.DropItems) dropItems.Items.Add(record);
                };
                EventHandler editDropItem = delegate
                {
                    StarDropItemRecord record = dropItems.SelectedItem as StarDropItemRecord;
                    if (record == null) return;
                    EditStarDropItem(record, form);
                    int selected = dropItems.SelectedIndex; refreshDropItems();
                    if (selected >= 0 && selected < dropItems.Items.Count) dropItems.SelectedIndex = selected;
                };
                refreshDropItems(); dropItems.DoubleClick += editDropItem; dropItems.Enabled = true;
                ContextMenuStrip dropMenu = new ContextMenuStrip();
                dropMenu.Items.Add("Редактировать", null, editDropItem);
                dropMenu.Items.Add("Удалить", null, delegate
                {
                    int selected = dropItems.SelectedIndex;
                    if (selected < 0) return;
                    star.DropItems.RemoveAt(selected); refreshDropItems();
                });
                dropItems.ContextMenuStrip = dropMenu;
                statusToolTip.SetToolTip(dropItems,
                    "Двойной щелчок — исходная форма выпавшего предмета; удаление переписывает список TStar.");
                form.KeyPreview = true;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(this);

                int parsedX, parsedY;
                float parsedSafeRadius, parsedDamageRadius;
                ushort parsedRadius, parsedGraphRadius;
                byte parsedBackground, parsedSafety, parsedDayBeforeOccupy;
                int parsedDayWithoutPlayer, parsedDayWithoutCreateShip, parsedLastDominatorDate,
                    parsedLastPirateDate, parsedLiberationDate, parsedDayInvadeInertia;
                ByteValueChoice selectedOwners = owners.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedLastOwners = lastOwners.SelectedItem as ByteValueChoice;
                ByteValueChoice selectedSeries = dominatorSeries.SelectedItem as ByteValueChoice;
                UInt32ValueChoice selectedDominion = dominion.SelectedItem as UInt32ValueChoice;
                if (!TryParseInt32(x.Text, out parsedX) || !TryParseInt32(y.Text, out parsedY) ||
                    parsedX < -4096 || parsedX > 4096 || parsedY < -4096 || parsedY > 4096 ||
                    !TryParseUInt16(radius.Text, out parsedRadius) || parsedRadius < 200 || parsedRadius > 300 ||
                    !TryParseByte(background.Text, out parsedBackground) || !TryParseByte(safety.Text, out parsedSafety) ||
                    parsedSafety > 100 || !TryParseFiniteFloat(safeRadius.Text, out parsedSafeRadius) ||
                    !TryParseFiniteFloat(damageRadius.Text, out parsedDamageRadius) ||
                    !TryParseUInt16(graphRadius.Text, out parsedGraphRadius) ||
                    !TryParseByte(dayBeforeOccupy.Text, out parsedDayBeforeOccupy) ||
                    !TryParseInt32(dayWithoutPlayer.Text, out parsedDayWithoutPlayer) ||
                    !TryParseInt32(dayWithoutCreateShip.Text, out parsedDayWithoutCreateShip) ||
                    !TryParseInt32(lastDominatorDate.Text, out parsedLastDominatorDate) ||
                    !TryParseInt32(lastPirateDate.Text, out parsedLastPirateDate) ||
                    !TryParseInt32(liberationDate.Text, out parsedLiberationDate) ||
                    !TryParseInt32(dayInvadeInertia.Text, out parsedDayInvadeInertia) ||
                    selectedOwners == null || selectedLastOwners == null || selectedSeries == null ||
                    selectedDominion == null || string.IsNullOrWhiteSpace(name.Text) ||
                    string.IsNullOrWhiteSpace(graphStar.Text))
                {
                    MessageBox.Show(this, "Поля звезды не применены: проверьте числа, радиус 200..300, безопасность 0..100 и графику.", "TStar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                star.Name = name.Text.Trim();
                star.X = parsedX;
                star.Y = parsedY;
                star.Raw1C = parsedRadius; star.Raw78 = parsedBackground;
                star.MapLabel = mapLabel.Text; star.CustomFaction = customFaction.Text;
                star.Safety = parsedSafety; star.SafeRadius = parsedSafeRadius;
                star.DamageRadius = parsedDamageRadius; star.GraphRadius = parsedGraphRadius;
                star.GraphStar = graphStar.Text.Trim(); star.Battle = battle.Checked;
                star.NoComeKling = noComeKling.Checked; star.Owners = selectedOwners.Value;
                star.LastOwners = selectedLastOwners.Value; star.DominatorSeries = selectedSeries.Value;
                star.DominionObjectId = selectedDominion.Value; star.DayBeforeOccupy = parsedDayBeforeOccupy;
                star.DayWithoutPlayer = parsedDayWithoutPlayer;
                star.DayWithoutCreateShip = parsedDayWithoutCreateShip;
                star.LastDominatorDate = parsedLastDominatorDate; star.LastPirateDate = parsedLastPirateDate;
                star.LiberationDate = parsedLiberationDate; star.DayInvadeInertia = parsedDayInvadeInertia;
                star.CustomSystemInfos = editableInfos;
                RefreshGalaxyView();
                SelectStar(star.ObjectId);
            }
        }

        private void EditStarDropItem(StarDropItemRecord record, IWin32Window owner)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TSTARDROPITEMFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox x = BindEditableText(form, "edPosX",
                    record.X.ToString("R", CultureInfo.InvariantCulture));
                TextBox y = BindEditableText(form, "edPosY",
                    record.Y.ToString("R", CultureInfo.InvariantCulture));
                TextBox shipId = BindEditableText(form, "edShipID",
                    record.ShipObjectId.ToString(CultureInfo.InvariantCulture));
                Label shipName = FindControl<Label>(form, "lblShipName");
                ShipHeaderRecord ship = FindShipById(record.ShipObjectId);
                shipName.Text = ship == null ? string.Empty : ship.Name;
                CheckBox inStar = FindControl<CheckBox>(form, "chbInStar");
                inStar.Checked = true;
                inStar.Enabled = false;
                statusToolTip.SetToolTip(inStar,
                    "Поле InStar вычисляется положением записи в списке TStar и отдельно в SAV не хранится.");
                CheckBox inUse = BindEditableCheck(form, "chbInUse", record.InUse);
                Button editItem = FindControl<Button>(form, "btnItemEdit");
                ItemHeaderRecord item = FindItem(record.ItemType, record.ItemObjectId);
                editItem.Enabled = item != null;
                if (item != null) editItem.Click += delegate { EditItem(item); };
                form.KeyPreview = true;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(owner);

                float parsedX, parsedY;
                uint parsedShipId;
                if (!TryParseFiniteFloat(x.Text, out parsedX) ||
                    !TryParseFiniteFloat(y.Text, out parsedY) ||
                    !uint.TryParse(shipId.Text, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out parsedShipId) || parsedShipId > 10000000)
                {
                    MessageBox.Show(owner, "Координаты должны быть конечными Float32, а ID корабля — UInt32 до 10000000.",
                        "TStarDropItem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.X = parsedX; record.Y = parsedY;
                record.ShipObjectId = parsedShipId; record.InUse = inUse.Checked;
            }
        }

        private void EditCustomSystemInfo(CustomSystemInfoRecord record)
        {
            using (Form form = EditorFormFactory.Build(EditorFormDefinitions.Get("TCUSTOMSYSTEMINFOFORM")))
            {
                SetUnsupportedEditorsReadOnly(form);
                TextBox name = BindEditableText(form, "edCustomSystemName", record.Name ?? string.Empty);
                TextBox icon = BindEditableText(form, "edCustomSystemIcon", record.Icon ?? string.Empty);
                TextBox info = BindEditableText(form, "edCustomSystemInfo", record.Info ?? string.Empty);
                TextBox type = BindEditableText(form, "edCustomSystemType", record.Type ?? string.Empty);
                TextBox distance = BindEditableText(form, "edCustomSystemDist", record.Distance.ToString(CultureInfo.InvariantCulture));
                form.KeyPreview = true;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                { if (args.KeyCode == Keys.Escape) form.Close(); };
                form.ShowDialog(this);
                int parsedDistance;
                if (!TryParseInt32(distance.Text, out parsedDistance))
                {
                    MessageBox.Show(this, "Дистанция TCustomSystemInfo должна быть Int32.", "TStar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                record.Name = name.Text; record.Icon = icon.Text; record.Info = info.Text;
                record.Type = type.Text; record.Distance = parsedDistance;
            }
        }

        private void SelectStar(uint objectId)
        {
            for (int index = 0; index < starList.Items.Count; index++)
            {
                StarHeaderRecord candidate = starList.Items[index] as StarHeaderRecord;
                if (candidate != null && candidate.ObjectId == objectId)
                {
                    starList.SelectedIndex = index;
                    return;
                }
            }
        }

        private void ShowSelectedStarMap(object sender, EventArgs e)
        {
            StarHeaderRecord star = starList.SelectedItem as StarHeaderRecord;
            if (star == null)
            {
                MessageBox.Show(this, "Выберите одну систему, а не пункт «Все системы».", "Карта системы",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<object> objects = new List<object>();
            foreach (object value in galaxyObjectList.Items)
                if (!(value is string)) objects.Add(value);
            List<PlanetHeaderRecord> planets = new List<PlanetHeaderRecord>();
            foreach (object value in objects)
            {
                PlanetHeaderRecord planet = value as PlanetHeaderRecord;
                if (planet != null) planets.Add(planet);
            }

            if (systemMapForm != null && !systemMapForm.IsDisposed)
            {
                systemMapForm.Close();
                systemMapForm = null;
            }
            Form map = EditorFormFactory.Build(EditorFormDefinitions.Get("TSTARMAPFORM"));
            systemMapForm = map;
            map.Text = (appSettings.LanguageIndex == 1 ? "System map — " : "Карта системы — ") +
                LocalizedStarName(star);
            map.BackColor = Color.Black;
            map.TabStop = true;
            List<StarMapHitRecord> hits = new List<StarMapHitRecord>();
            systemMapHits = hits;
            float mapZoom = 1.0F;
            PointF mapPan = PointF.Empty;
            bool showJumpPoints = showSystemJumpPoints;
            bool mapDragging = false;
            bool mapDragged = false;
            Point dragStart = Point.Empty;
            PointF dragPanStart = PointF.Empty;
            MouseButtons dragButton = MouseButtons.None;
            StarMapHitRecord pressedHit = null;
            Button jumpToggle = new Button();
            jumpToggle.Name = "$jumpToggle";
            jumpToggle.Text = showJumpPoints ? "Переходы: вкл." : "Переходы: выкл.";
            jumpToggle.Size = new Size(142, 28);
            jumpToggle.Location = new Point(Math.Max(8, map.ClientSize.Width - 152), 8);
            jumpToggle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            jumpToggle.FlatStyle = FlatStyle.Flat;
            jumpToggle.FlatAppearance.BorderColor = Color.FromArgb(90, 220, 255);
            jumpToggle.BackColor = Color.FromArgb(28, 28, 28);
            jumpToggle.ForeColor = Color.FromArgb(130, 230, 255);
            jumpToggle.UseVisualStyleBackColor = false;
            jumpToggle.Click += delegate
            {
                showJumpPoints = !showJumpPoints;
                showSystemJumpPoints = showJumpPoints;
                jumpToggle.Text = showJumpPoints ? "Переходы: вкл." : "Переходы: выкл.";
                map.Invalidate();
            };
            map.Controls.Add(jumpToggle);
            jumpToggle.BringToFront();
            map.Paint += delegate(object paintSender, PaintEventArgs paintArgs)
            {
                DrawSelectedStarMap(paintArgs.Graphics, map.ClientRectangle,
                    star, planets, objects, hits, mapZoom, mapPan, showJumpPoints);
            };
            map.MouseMove += delegate(object moveSender, MouseEventArgs moveArgs)
            {
                if (mapDragging && (dragButton == MouseButtons.Middle || pressedHit == null))
                {
                    int dx = moveArgs.X - dragStart.X;
                    int dy = moveArgs.Y - dragStart.Y;
                    if (Math.Abs(dx) > 2 || Math.Abs(dy) > 2) mapDragged = true;
                    mapPan = new PointF(dragPanStart.X + dx, dragPanStart.Y + dy);
                    map.Cursor = Cursors.SizeAll;
                    map.Invalidate();
                    return;
                }
                StarMapHitRecord hover = FindStarMapHit(hits, moveArgs.Location);
                map.Cursor = hover == null ? Cursors.SizeAll : Cursors.Hand;
            };
            map.MouseDown += delegate(object downSender, MouseEventArgs downArgs)
            {
                if (downArgs.Button != MouseButtons.Left && downArgs.Button != MouseButtons.Middle) return;
                map.Focus();
                mapDragging = true;
                mapDragged = false;
                dragButton = downArgs.Button;
                dragStart = downArgs.Location;
                dragPanStart = mapPan;
                pressedHit = downArgs.Button == MouseButtons.Left
                    ? FindStarMapHit(hits, downArgs.Location) : null;
                map.Capture = true;
            };
            map.MouseUp += delegate(object upSender, MouseEventArgs upArgs)
            {
                if (!mapDragging || upArgs.Button != dragButton) return;
                mapDragging = false;
                map.Capture = false;
                if (dragButton == MouseButtons.Left && !mapDragged)
                {
                    SystemJumpPointRecord jump = pressedHit == null ? null :
                        pressedHit.Value as SystemJumpPointRecord;
                    if (jump != null && jump.TargetStar != null)
                    {
                        uint targetStarId = jump.TargetStar.ObjectId;
                        map.Close();
                        SelectStar(targetStarId);
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            ShowSelectedStarMap(this, EventArgs.Empty);
                        }));
                        return;
                    }
                    if (pressedHit == null)
                        galaxyObjectList.ClearSelected();
                    else
                    {
                        int selectedIndex = -1;
                        for (int index = 0; index < galaxyObjectList.Items.Count; index++)
                            if (object.ReferenceEquals(galaxyObjectList.Items[index], pressedHit.Value))
                            {
                                selectedIndex = index;
                                break;
                            }
                        if (selectedIndex >= 0)
                        {
                            bool additive = (ModifierKeys & Keys.Control) != 0;
                            bool alreadySelected = galaxyObjectList.GetSelected(selectedIndex);
                            if (additive)
                                galaxyObjectList.SetSelected(selectedIndex, !alreadySelected);
                            else if (alreadySelected && galaxyObjectList.SelectedIndices.Count == 1)
                                galaxyObjectList.ClearSelected();
                            else
                            {
                                galaxyObjectList.ClearSelected();
                                galaxyObjectList.SetSelected(selectedIndex, true);
                            }
                        }
                    }
                }
                pressedHit = null;
                dragButton = MouseButtons.None;
                map.Cursor = Cursors.Default;
                map.Invalidate();
            };
            map.MouseWheel += delegate(object wheelSender, MouseEventArgs wheelArgs)
            {
                float previousZoom = mapZoom;
                float factor = wheelArgs.Delta > 0 ? 1.18F : 1.0F / 1.18F;
                mapZoom = Math.Max(0.35F, Math.Min(6.0F, mapZoom * factor));
                if (Math.Abs(mapZoom - previousZoom) < 0.0001F) return;
                float ratio = mapZoom / previousZoom;
                float baseX = map.ClientSize.Width / 2.0F;
                float baseY = map.ClientSize.Height / 2.0F;
                mapPan = new PointF(
                    wheelArgs.X - baseX - (wheelArgs.X - baseX - mapPan.X) * ratio,
                    wheelArgs.Y - baseY - (wheelArgs.Y - baseY - mapPan.Y) * ratio);
                map.Invalidate();
            };
            map.KeyDown += delegate(object keySender, KeyEventArgs keyArgs)
            {
                if (keyArgs.KeyCode == Keys.Escape) map.Close();
                if (keyArgs.KeyCode == Keys.Home)
                {
                    mapZoom = 1.0F; mapPan = PointF.Empty; map.Invalidate();
                }
            };
            map.FormClosed += delegate
            {
                if (object.ReferenceEquals(systemMapForm, map))
                {
                    systemMapForm = null;
                    systemMapHits = null;
                }
            };
            map.Show(this);
        }

        private void GalaxyObjectSelectionChanged(object sender, EventArgs e)
        {
            if (systemMapForm != null && !systemMapForm.IsDisposed)
                {
                systemMapForm.Invalidate();
            }
        }

        private void DrawSelectedStarMap(Graphics graphics, Rectangle bounds,
            StarHeaderRecord star, List<PlanetHeaderRecord> planets,
            List<object> objects, List<StarMapHitRecord> hits, float zoom, PointF pan,
            bool showJumpPoints)
        {
            hits.Clear(); graphics.Clear(Color.Black);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float radius = 100.0F;
            foreach (PlanetHeaderRecord planet in planets)
                radius = Math.Max(radius, Math.Abs(planet.PolarRadius));
            foreach (object value in objects)
            {
                PointF world;
                if (TryGetStarMapWorldPoint(value, star.ObjectId, out world))
                    radius = Math.Max(radius, Math.Max(Math.Abs(world.X), Math.Abs(world.Y)));
            }
            List<object> renderObjects = new List<object>(objects);
            if (showJumpPoints)
                foreach (SystemJumpPointRecord jump in BuildSystemJumpPoints(star))
                {
                    renderObjects.Add(jump);
                    radius = Math.Max(radius, Math.Max(Math.Abs(jump.WorldPoint.X),
                        Math.Abs(jump.WorldPoint.Y)));
                }
            float centerX = bounds.Width / 2.0F + pan.X;
            float centerY = bounds.Height / 2.0F + pan.Y;
            float scale = (Math.Min(bounds.Width, bounds.Height) / 2.0F - 35.0F) /
                radius * Math.Max(0.1F, zoom);
            using (Pen orbitPen = new Pen(Color.FromArgb(55, 130, 130, 130)))
            using (Pen selectedPen = new Pen(Color.Lime, 2.0F))
            using (Pen markerOutline = new Pen(Color.FromArgb(210, 20, 20, 20), 1.0F))
            using (Pen jumpGuide = new Pen(Color.FromArgb(105, 90, 220, 255), 1.0F))
            using (Brush starBrush = new SolidBrush(Color.Gold))
            using (Brush textBrush = new SolidBrush(Color.Gainsboro))
            using (Brush helpBrush = new SolidBrush(Color.FromArgb(125, 225, 225, 225)))
            using (Font font = new Font("Tahoma", 8.0F, FontStyle.Regular, GraphicsUnit.Point))
            using (Font helpFont = new Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point))
            using (StringFormat helpFormat = new StringFormat())
            {
                helpFormat.Alignment = StringAlignment.Far;
                helpFormat.LineAlignment = StringAlignment.Far;
                jumpGuide.DashStyle = DashStyle.Dot;
                foreach (PlanetHeaderRecord planet in planets)
                {
                    float orbit = Math.Abs(planet.PolarRadius) * scale;
                    graphics.DrawEllipse(orbitPen, centerX - orbit, centerY - orbit,
                        orbit * 2.0F, orbit * 2.0F);
                }
                graphics.FillEllipse(starBrush, centerX - 7.0F, centerY - 7.0F, 14.0F, 14.0F);
                graphics.DrawString(LocalizedStarName(star), font, textBrush,
                    centerX + 10.0F, centerY - 10.0F);
                foreach (object value in renderObjects)
                {
                    PointF world;
                    if (!TryGetStarMapWorldPoint(value, star.ObjectId, out world)) continue;
                    PointF displayWorld = StarMapDisplayWorldPoint(value, world);
                    PointF point = new PointF(centerX + displayWorld.X * scale,
                        centerY - displayWorld.Y * scale);
                    Color color = StarMapObjectColor(value);
                    using (Brush marker = new SolidBrush(color))
                    {
                        SystemJumpPointRecord jump = value as SystemJumpPointRecord;
                        if (jump != null)
                        {
                            float dx = point.X - centerX, dy = point.Y - centerY;
                            graphics.DrawLine(jumpGuide, centerX + dx * 0.84F, centerY + dy * 0.84F,
                                point.X, point.Y);
                        }
                        DrawStarMapMarker(graphics, value, point, marker, markerOutline);
                    }
                    StarMapHitRecord hit = new StarMapHitRecord(); hit.Value = value; hit.Point = point;
                    hit.HitBox = new RectangleF(point.X - 7.0F, point.Y - 7.0F, 14.0F, 14.0F);
                    hits.Add(hit);
                    if (IsGalaxyObjectSelected(value))
                    {
                        int overlap = SelectedMapOverlapCount(hits, hit);
                        float selectionRadius = 7.0F + Math.Min(4, overlap) * 2.0F;
                        graphics.DrawEllipse(selectedPen, point.X - selectionRadius,
                            point.Y - selectionRadius, selectionRadius * 2.0F,
                            selectionRadius * 2.0F);
                    }
                    string label = StarMapObjectLabel(value);
                    if (!string.IsNullOrEmpty(label) && (value is PlanetHeaderRecord ||
                        value is ShipHeaderRecord && ((ShipHeaderRecord)value).IsStation ||
                        value is SystemJumpPointRecord ||
                        IsGalaxyObjectSelected(value)))
                    {
                        SizeF labelSize = graphics.MeasureString(label, font);
                        float labelX = point.X + 6.0F;
                        if (labelX + labelSize.Width > bounds.Right - 4.0F)
                            labelX = point.X - labelSize.Width - 6.0F;
                        labelX = Math.Max(bounds.Left + 4.0F,
                            Math.Min(bounds.Right - labelSize.Width - 4.0F, labelX));
                        float labelY = Math.Max(bounds.Top + 3.0F,
                            Math.Min(bounds.Bottom - labelSize.Height - 3.0F,
                                point.Y - 8.0F));
                        graphics.DrawString(label, font, textBrush, labelX, labelY);
                    }
                }
                string help = appSettings.LanguageIndex == 1
                    ? "LMB — select  ·  Ctrl+LMB — multiple  ·  drag — pan  ·  wheel — zoom  ·  Home — reset"
                    : "ЛКМ — выбор  ·  Ctrl+ЛКМ — несколько  ·  " +
                        "перетаскивание — перемещение  ·  колесо — масштаб  ·  Home — сброс";
                RectangleF helpBounds = new RectangleF(bounds.Left + 10F, bounds.Top + 10F,
                    Math.Max(20F, bounds.Width - 20F), Math.Max(20F, bounds.Height - 16F));
                graphics.DrawString(help, helpFont, helpBrush, helpBounds, helpFormat);
            }
        }

        internal static PointF StarMapDisplayWorldPoint(object value, PointF world)
        {
            // JumpDestination uses the galaxy coordinate convention (positive Y is down),
            // whereas ordinary in-system objects use the game's Cartesian Y-up convention.
            // Convert only jump markers at the rendering boundary so planets, ships and
            // asteroids keep their already verified orientation.
            return value is SystemJumpPointRecord ? new PointF(world.X, -world.Y) : world;
        }

        private bool IsGalaxyObjectSelected(object value)
        {
            if (galaxyObjectList == null || value == null) return false;
            foreach (object selected in galaxyObjectList.SelectedItems)
                if (object.ReferenceEquals(selected, value)) return true;
            return false;
        }

        private int SelectedMapOverlapCount(List<StarMapHitRecord> hits,
            StarMapHitRecord currentHit)
        {
            int count = 0;
            foreach (StarMapHitRecord candidate in hits)
            {
                if (object.ReferenceEquals(candidate, currentHit) ||
                    !IsGalaxyObjectSelected(candidate.Value)) continue;
                float dx = candidate.Point.X - currentHit.Point.X;
                float dy = candidate.Point.Y - currentHit.Point.Y;
                if (dx * dx + dy * dy <= 64.0F) count++;
            }
            return count;
        }

        private static StarMapHitRecord FindStarMapHit(List<StarMapHitRecord> hits, Point point)
        {
            StarMapHitRecord nearest = null; float best = float.MaxValue;
            foreach (StarMapHitRecord hit in hits)
                if (hit.HitBox.Contains(point))
                {
                    float dx = hit.Point.X - point.X, dy = hit.Point.Y - point.Y;
                    float distance = dx * dx + dy * dy;
                    if (distance < best) { best = distance; nearest = hit; }
                }
            return nearest;
        }

        private static bool TryGetStarMapWorldPoint(object value, uint starId, out PointF point)
        {
            PlanetHeaderRecord planet = value as PlanetHeaderRecord;
            if (planet != null) { point = PlanetCartesian(planet); return true; }
            ShipHeaderRecord ship = value as ShipHeaderRecord;
            if (ship != null) { point = new PointF(ship.X, ship.Y); return true; }
            ItemHeaderRecord item = value as ItemHeaderRecord;
            if (item != null) { point = new PointF(item.X, item.Y); return true; }
            MissileRecord missile = value as MissileRecord;
            if (missile != null) { point = new PointF(missile.PositionX, missile.PositionY); return true; }
            AsteroidRecord asteroid = value as AsteroidRecord;
            if (asteroid != null)
            {
                point = new PointF((float)(asteroid.PositionX * SavContainer.AsteroidPositionScale),
                    (float)(asteroid.PositionY * SavContainer.AsteroidPositionScale));
                return true;
            }
            HoleRecord hole = value as HoleRecord;
            if (hole != null)
            {
                point = hole.FromStarId == starId ? new PointF(hole.FromX, hole.FromY) :
                    new PointF(hole.ToX, hole.ToY);
                return true;
            }
            SystemJumpPointRecord jump = value as SystemJumpPointRecord;
            if (jump != null) { point = jump.WorldPoint; return true; }
            point = PointF.Empty; return false;
        }

        private static Color StarMapObjectColor(object value)
        {
            if (value is PlanetHeaderRecord) return Color.DeepSkyBlue;
            ShipHeaderRecord ship = value as ShipHeaderRecord;
            if (ship != null) return ship.IsStation ? Color.Orange : Color.White;
            ItemHeaderRecord item = value as ItemHeaderRecord;
            if (item != null)
            {
                if (item.Type == 71) return Color.Cyan;
                if (item.Type == 73) return Color.Orange;
                if (item.Type == 74) return Color.Gold;
                if (item.Type == 34 || item.Type == 28) return Color.Violet;
                if (item.Type >= 42 && item.Type <= 69) return Color.LimeGreen;
                return Color.PaleGreen;
            }
            if (value is MissileRecord) return Color.Red;
            if (value is AsteroidRecord) return Color.Silver;
            if (value is HoleRecord) return Color.MediumPurple;
            if (value is SystemJumpPointRecord) return Color.FromArgb(90, 220, 255);
            return Color.Gainsboro;
        }

        private static void DrawStarMapMarker(Graphics graphics, object value, PointF point,
            Brush fill, Pen outline)
        {
            if (value is PlanetHeaderRecord)
            {
                graphics.FillEllipse(fill, point.X - 4.5F, point.Y - 4.5F, 9.0F, 9.0F);
                graphics.DrawEllipse(outline, point.X - 4.5F, point.Y - 4.5F, 9.0F, 9.0F);
                return;
            }
            ShipHeaderRecord ship = value as ShipHeaderRecord;
            if (ship != null)
            {
                if (ship.IsStation)
                {
                    PointF[] diamond = { new PointF(point.X, point.Y - 5F),
                        new PointF(point.X + 5F, point.Y), new PointF(point.X, point.Y + 5F),
                        new PointF(point.X - 5F, point.Y) };
                    graphics.FillPolygon(fill, diamond); graphics.DrawPolygon(outline, diamond);
                }
                else
                {
                    PointF[] triangle = { new PointF(point.X, point.Y - 5F),
                        new PointF(point.X + 4.5F, point.Y + 4F),
                        new PointF(point.X - 4.5F, point.Y + 4F) };
                    graphics.FillPolygon(fill, triangle); graphics.DrawPolygon(outline, triangle);
                }
                return;
            }
            ItemHeaderRecord item = value as ItemHeaderRecord;
            if (item != null)
            {
                if (item.Type == 71)
                {
                    graphics.FillEllipse(fill, point.X - 4F, point.Y - 4F, 8F, 8F);
                    graphics.DrawLine(outline, point.X - 5F, point.Y, point.X + 5F, point.Y);
                    graphics.DrawLine(outline, point.X, point.Y - 5F, point.X, point.Y + 5F);
                }
                else if (item.Type == 74 || item.Type == 34 || item.Type == 28)
                {
                    PointF[] diamond = { new PointF(point.X, point.Y - 4.5F),
                        new PointF(point.X + 4.5F, point.Y), new PointF(point.X, point.Y + 4.5F),
                        new PointF(point.X - 4.5F, point.Y) };
                    graphics.FillPolygon(fill, diamond); graphics.DrawPolygon(outline, diamond);
                }
                else
                {
                    graphics.FillRectangle(fill, point.X - 3.5F, point.Y - 3.5F, 7F, 7F);
                    graphics.DrawRectangle(outline, point.X - 3.5F, point.Y - 3.5F, 7F, 7F);
                }
                return;
            }
            if (value is MissileRecord)
            {
                PointF[] arrow = { new PointF(point.X + 5F, point.Y),
                    new PointF(point.X - 4F, point.Y - 3.5F),
                    new PointF(point.X - 2F, point.Y), new PointF(point.X - 4F, point.Y + 3.5F) };
                graphics.FillPolygon(fill, arrow); graphics.DrawPolygon(outline, arrow); return;
            }
            if (value is AsteroidRecord)
            {
                PointF[] rock = { new PointF(point.X - 4.5F, point.Y - 2F),
                    new PointF(point.X - 1F, point.Y - 5F), new PointF(point.X + 4F, point.Y - 3F),
                    new PointF(point.X + 5F, point.Y + 2F), new PointF(point.X + 1F, point.Y + 5F),
                    new PointF(point.X - 4F, point.Y + 3F) };
                graphics.FillPolygon(fill, rock); graphics.DrawPolygon(outline, rock); return;
            }
            if (value is HoleRecord)
            {
                using (Pen ring = new Pen(((SolidBrush)fill).Color, 2F))
                    graphics.DrawEllipse(ring, point.X - 5F, point.Y - 5F, 10F, 10F);
                graphics.DrawEllipse(outline, point.X - 2F, point.Y - 2F, 4F, 4F); return;
            }
            if (value is SystemJumpPointRecord)
            {
                PointF[] chevron = { new PointF(point.X - 5F, point.Y - 5F),
                    new PointF(point.X + 2F, point.Y), new PointF(point.X - 5F, point.Y + 5F),
                    new PointF(point.X - 1F, point.Y + 5F), new PointF(point.X + 6F, point.Y),
                    new PointF(point.X - 1F, point.Y - 5F) };
                graphics.FillPolygon(fill, chevron); graphics.DrawPolygon(outline, chevron); return;
            }
            graphics.FillRectangle(fill, point.X - 3F, point.Y - 3F, 6F, 6F);
        }

        private string StarMapObjectLabel(object value)
        {
            PlanetHeaderRecord planet = value as PlanetHeaderRecord;
            if (planet != null) return planet.Name;
            ShipHeaderRecord ship = value as ShipHeaderRecord;
            if (ship != null) return ship.Name;
            ItemHeaderRecord item = value as ItemHeaderRecord;
            if (item != null) return ItemDisplayName(item);
            AsteroidRecord asteroid = value as AsteroidRecord;
            if (asteroid != null) return "Астероид " + asteroid.ObjectId.ToString(CultureInfo.InvariantCulture) +
                " — " + asteroid.GraphName + "; минералы: " +
                asteroid.Minerals.ToString(CultureInfo.InvariantCulture);
            MissileRecord missile = value as MissileRecord;
            if (missile != null) return "Ракета " + missile.ObjectId.ToString(CultureInfo.InvariantCulture);
            HoleRecord hole = value as HoleRecord;
            if (hole != null)
            {
                StarHeaderRecord viewedStar = starList.SelectedItem as StarHeaderRecord;
                uint viewedId = viewedStar == null ? CurrentGalaxyStarId() : viewedStar.ObjectId;
                uint targetId = hole.FromStarId == viewedId ? hole.ToStarId : hole.FromStarId;
                return "Чёрная дыра " + hole.ObjectId.ToString(CultureInfo.InvariantCulture) +
                    " → " + StarName(targetId);
            }
            SystemJumpPointRecord jump = value as SystemJumpPointRecord;
            if (jump != null && jump.TargetStar != null)
                return (appSettings.LanguageIndex == 1 ? "Jump → " : "Переход → ") +
                    LocalizedStarName(jump.TargetStar);
            return string.Empty;
        }

        private List<SystemJumpPointRecord> BuildSystemJumpPoints(StarHeaderRecord source)
        {
            List<SystemJumpPointRecord> result = new List<SystemJumpPointRecord>();
            if (source == null || pendingStars == null || pendingConstellations == null) return result;
            ShipHeaderRecord player = FindPlayerShip();
            if (player == null) return result;
            HashSet<uint> added = new HashSet<uint>();
            foreach (ConstellationRecord constellation in pendingConstellations)
            {
                if (constellation.MapLines == null) continue;
                foreach (GalaxyMapLine line in constellation.MapLines)
                {
                    bool fromFirst = GalaxyPointsNear(source.X, source.Y, line.X1, line.Y1);
                    bool fromSecond = GalaxyPointsNear(source.X, source.Y, line.X2, line.Y2);
                    if (fromFirst == fromSecond) continue;
                    float targetX = fromFirst ? line.X2 : line.X1;
                    float targetY = fromFirst ? line.Y2 : line.Y1;
                    StarHeaderRecord target = FindNearestGalaxyStar(targetX, targetY);
                    if (target == null || target.ObjectId == source.ObjectId || !added.Add(target.ObjectId))
                        continue;
                    float jumpX, jumpY;
                    if (!TryCalculateJumpDestination(player, source, target, out jumpX, out jumpY))
                        continue;
                    result.Add(new SystemJumpPointRecord { TargetStar = target,
                        WorldPoint = new PointF(jumpX, jumpY) });
                }
            }
            return result;
        }

        private static bool GalaxyPointsNear(float x1, float y1, float x2, float y2)
        {
            float dx = x1 - x2, dy = y1 - y2;
            return dx * dx + dy * dy <= 4.0F;
        }

        private StarHeaderRecord FindNearestGalaxyStar(float x, float y)
        {
            StarHeaderRecord nearest = null; float best = 4.0F;
            foreach (StarHeaderRecord candidate in pendingStars)
            {
                float dx = candidate.X - x, dy = candidate.Y - y;
                float distance = dx * dx + dy * dy;
                if (distance <= best) { best = distance; nearest = candidate; }
            }
            return nearest;
        }

        private static PointF PlanetCartesian(PlanetHeaderRecord planet)
        {
            double radians = planet.PolarAngle * Math.PI / 180.0;
            return new PointF((float)(Math.Cos(radians) * planet.PolarRadius),
                (float)(Math.Sin(radians) * planet.PolarRadius));
        }

        private void DrawGalaxyMap()
        {
            galaxyMapHits.Clear();
            if (pendingStars == null || pendingStars.Count == 0)
            {
                ReplaceImage(galaxyMapImage, null);
                return;
            }
            Bitmap bitmap = new Bitmap(Math.Max(1, galaxyMapImage.Width), Math.Max(1, galaxyMapImage.Height));
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Pen sectorPen = new Pen(Color.FromArgb(150, 80, 160, 255), 1.0F))
            using (Pen systemLinkPen = new Pen(GalaxySystemLinkColor, 1.0F))
            using (Brush point = new SolidBrush(Color.Yellow))
            using (Brush currentPoint = new SolidBrush(Color.Yellow))
            using (Pen currentOutline = new Pen(Color.Gold, 1.0F))
            using (Font font = new Font("Tahoma", 7.0F, FontStyle.Regular, GraphicsUnit.Point))
            using (Font helpFont = new Font("Segoe UI", 7.5F, FontStyle.Regular, GraphicsUnit.Point))
            using (Brush helpBrush = new SolidBrush(Color.FromArgb(145, 225, 225, 225)))
            using (StringFormat helpFormat = new StringFormat())
            {
                graphics.Clear(Color.Black);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                helpFormat.Alignment = StringAlignment.Far;
                helpFormat.LineAlignment = StringAlignment.Far;
                sectorPen.DashStyle = DashStyle.Solid;
                systemLinkPen.DashStyle = DashStyle.Dot;
                systemLinkPen.DashCap = DashCap.Round;

                float minX, minY, maxX, maxY;
                CalculateGalaxyMapBounds(pendingStars, out minX, out minY, out maxX, out maxY);

                const float margin = 40F;
                float baseScale = Math.Min((bitmap.Width - margin * 2F) / (maxX - minX),
                    (bitmap.Height - margin * 2F) / (maxY - minY));
                if (baseScale <= 0F || float.IsInfinity(baseScale) || float.IsNaN(baseScale))
                    baseScale = 1F;
                float scale = baseScale * galaxyMapZoom;
                float offsetX = (bitmap.Width - (maxX - minX) * scale) / 2F + galaxyMapPan.X;
                float offsetY = (bitmap.Height - (maxY - minY) * scale) / 2F + galaxyMapPan.Y;

                DrawGalaxySectorLabels(graphics, bitmap.Size, minX, minY,
                    scale, offsetX, offsetY);

                if (pendingConstellations != null)
                    foreach (ConstellationRecord constellation in pendingConstellations)
                    {
                        // A no-pirates mod keeps the disabled pirate systems in the SAV but
                        // moves their whole sector far outside the playable galaxy. Preserve
                        // those records, but do not let their orphan geometry cross or shrink
                        // the visible map.
                        if (!GalaxySectorHasStarInsideBounds(constellation, pendingStars,
                            minX, minY, maxX, maxY)) continue;
                        // HiddenBoundaryLines normally describe the fog/visibility contour and
                        // can run through a sector (notably Hisha). The hidden pirate sector is
                        // the exception: it has no regular PBound polygon, so PBoundHidden is
                        // its only available outer border.
                        IList<GalaxyMapLine> boundaries = GalaxySectorBoundaryLines(constellation);
                        if (boundaries != null)
                            foreach (GalaxyMapLine line in boundaries)
                                graphics.DrawLine(sectorPen,
                                    MapX(line.X1, minX, scale, offsetX),
                                    MapY(line.Y1, minY, scale, offsetY, bitmap.Height),
                                    MapX(line.X2, minX, scale, offsetX),
                                    MapY(line.Y2, minY, scale, offsetY, bitmap.Height));
                        // MapLines are the actual routes between systems.  The game renders
                        // these as a quieter dotted network inside each sector.
                        if (constellation.MapLines != null)
                            foreach (GalaxyMapLine line in constellation.MapLines)
                                graphics.DrawLine(systemLinkPen,
                                    MapX(line.X1, minX, scale, offsetX),
                                    MapY(line.Y1, minY, scale, offsetY, bitmap.Height),
                                    MapX(line.X2, minX, scale, offsetX),
                                    MapY(line.Y2, minY, scale, offsetY, bitmap.Height));
                    }

                uint currentStarId = CurrentGalaxyStarId();
                foreach (StarHeaderRecord star in pendingStars)
                {
                    float px = MapX(star.X, minX, scale, offsetX);
                    float py = MapY(star.Y, minY, scale, offsetY, bitmap.Height);
                    // A handful of mod-created systems can legitimately sit far outside the
                    // playable galaxy. They must not collapse the useful map or remain as
                    // invisible hit targets after robust fitting.
                    if (px < 0F || px >= bitmap.Width || py < 0F || py >= bitmap.Height) continue;
                    if (star.ObjectId == currentStarId)
                    {
                        PointF[] currentStar = FourPointStar(px, py, 8F, 2.5F);
                        graphics.FillPolygon(currentPoint, currentStar);
                        graphics.DrawPolygon(currentOutline, currentStar);
                    }
                    else
                    {
                        PointF[] diamond = { new PointF(px, py - 4F), new PointF(px + 4F, py),
                            new PointF(px, py + 4F), new PointF(px - 4F, py) };
                        graphics.FillPolygon(point, diamond);
                    }
                    galaxyMapHits.Add(new StarMapHitRecord {
                        Value = star, Point = new PointF(px, py),
                        HitBox = new RectangleF(px - 8F, py - 8F, 16F, 16F)
                    });
                }
                DrawGalaxyMapLabels(graphics, bitmap.Size, pendingStars, minX, minY,
                    scale, offsetX, offsetY, font);
                string help = appSettings.LanguageIndex == 1
                    ? "wheel — zoom  ·  drag — pan  ·  RMB — reset"
                    : "колесо — масштаб  ·  перетаскивание — перемещение  ·  ПКМ — сброс";
                graphics.DrawString(help, helpFont, helpBrush,
                    new RectangleF(8F, 8F, bitmap.Width - 16F, bitmap.Height - 14F), helpFormat);
            }
            ReplaceImage(galaxyMapImage, bitmap);
        }

        private void DrawGalaxySectorLabels(Graphics graphics, Size size,
            float minX, float minY, float scale, float offsetX, float offsetY)
        {
            if (pendingConstellations == null || pendingStars == null) return;
            using (Font font = new Font("Tahoma", 13.0F, FontStyle.Bold, GraphicsUnit.Point))
            using (Brush shadow = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
            using (Brush foreground = new SolidBrush(Color.FromArgb(58, 255, 255, 255)))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                foreach (ConstellationRecord constellation in pendingConstellations)
                {
                    float sumX = 0F, sumY = 0F;
                    int count = 0;
                    foreach (StarHeaderRecord star in pendingStars)
                        if (constellation.StarObjectIds.Contains(star.ObjectId))
                        { sumX += star.X; sumY += star.Y; count++; }
                    float worldX = count == 0 ? constellation.X : sumX / count;
                    float worldY = count == 0 ? constellation.Y : sumY / count;
                    float px = MapX(worldX, minX, scale, offsetX);
                    float py = MapY(worldY, minY, scale, offsetY, size.Height);
                    if (px < 0F || px >= size.Width || py < 0F || py >= size.Height) continue;
                    string text = ConstellationDisplayName(constellation);
                    graphics.DrawString(text, font, shadow, new PointF(px + 1F, py + 1F), format);
                    graphics.DrawString(text, font, foreground, new PointF(px, py), format);
                }
            }
        }

        internal static IList<GalaxyMapLine> GalaxySectorBoundaryLines(
            ConstellationRecord constellation)
        {
            if (constellation == null) return null;
            if (constellation.BoundaryLines != null && constellation.BoundaryLines.Count > 0)
                return constellation.BoundaryLines;
            return constellation.HiddenBoundaryLines;
        }

        internal static bool GalaxySectorHasStarInsideBounds(ConstellationRecord constellation,
            IList<StarHeaderRecord> stars, float minX, float minY, float maxX, float maxY)
        {
            if (constellation == null || stars == null || constellation.StarObjectIds == null)
                return false;
            foreach (StarHeaderRecord star in stars)
                if (constellation.StarObjectIds.Contains(star.ObjectId) &&
                    star.X >= minX && star.X <= maxX && star.Y >= minY && star.Y <= maxY)
                    return true;
            return false;
        }

        internal static Color GalaxySystemLinkColor
        {
            get { return Color.FromArgb(0x77, 0x77, 0x77); }
        }

        private uint CurrentGalaxyStarId()
        {
            if (pendingGalaxySummary != null && pendingGalaxySummary.CurrentStarObjectId != 0)
                return pendingGalaxySummary.CurrentStarObjectId;
            ShipHeaderRecord player = FindPlayerShip();
            return player == null ? 0U : player.CurrentStarId;
        }

        private static PointF[] FourPointStar(float centerX, float centerY,
            float outerRadius, float innerRadius)
        {
            PointF[] points = new PointF[8];
            for (int index = 0; index < points.Length; index++)
            {
                double angle = -Math.PI / 2.0 + index * Math.PI / 4.0;
                float radius = (index & 1) == 0 ? outerRadius : innerRadius;
                points[index] = new PointF(centerX + (float)Math.Cos(angle) * radius,
                    centerY + (float)Math.Sin(angle) * radius);
            }
            return points;
        }

        private void DrawGalaxyMapLabels(Graphics graphics, Size size,
            IList<StarHeaderRecord> stars, float minX, float minY, float scale,
            float offsetX, float offsetY, Font font)
        {
            List<RectangleF> occupied = new List<RectangleF>();
            using (StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone())
            foreach (StarHeaderRecord star in stars)
            {
                float px = MapX(star.X, minX, scale, offsetX);
                float py = MapY(star.Y, minY, scale, offsetY, size.Height);
                if (px < 0F || px >= size.Width || py < 0F || py >= size.Height) continue;
                List<GalaxyLabelSegment> segments = GalaxyStarLabelSegments(star);
                string text = string.Empty;
                foreach (GalaxyLabelSegment segment in segments) text += segment.Text;
                if (text.Length == 0) continue;
                SizeF measured = graphics.MeasureString(text, font, PointF.Empty, format);
                PointF[] candidates = new PointF[] {
                    new PointF(px - measured.Width / 2F, py - measured.Height - 7F),
                    new PointF(px + 6F, py - measured.Height / 2F),
                    new PointF(px - measured.Width - 6F, py - measured.Height / 2F),
                    new PointF(px - measured.Width / 2F, py + 6F),
                    new PointF(px + 6F, py - measured.Height - 6F),
                    new PointF(px - measured.Width - 6F, py + 5F)
                };
                RectangleF selected = RectangleF.Empty;
                float bestPenalty = float.MaxValue;
                foreach (PointF candidate in candidates)
                {
                    RectangleF rectangle = new RectangleF(candidate.X, candidate.Y,
                        measured.Width, measured.Height);
                    float penalty = LabelBoundsPenalty(rectangle, size);
                    foreach (RectangleF previous in occupied)
                        if (rectangle.IntersectsWith(previous)) penalty += 10000F +
                            IntersectionArea(rectangle, previous);
                    if (penalty < bestPenalty) { bestPenalty = penalty; selected = rectangle; }
                }
                selected.X = Math.Max(1F, Math.Min(size.Width - selected.Width - 1F, selected.X));
                selected.Y = Math.Max(1F, Math.Min(size.Height - selected.Height - 1F, selected.Y));
                float drawX = selected.X;
                foreach (GalaxyLabelSegment segment in segments)
                {
                    if (string.IsNullOrEmpty(segment.Text)) continue;
                    using (Brush brush = new SolidBrush(segment.Color))
                        graphics.DrawString(segment.Text, font, brush,
                            new PointF(drawX, selected.Y), format);
                    drawX += graphics.MeasureString(segment.Text, font,
                        PointF.Empty, format).Width;
                }
                occupied.Add(RectangleF.Inflate(selected, 1F, 1F));
            }
        }

        private List<GalaxyLabelSegment> GalaxyStarLabelSegments(StarHeaderRecord star)
        {
            List<byte> owners = new List<byte>();
            if (pendingPlanets != null)
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (FindStarForOffset(planet.Start) == star && planet.Owner != 6)
                        owners.Add(planet.Owner);
            return BuildGalaxyLabelSegments(LocalizedStarName(star), star.CustomFaction, owners, gameCatalog);
        }

        internal static List<GalaxyLabelSegment> BuildGalaxyLabelSegments(string name,
            string customFaction, IList<byte> planetOwners, GameDataCatalog catalog)
        {
            name = name ?? string.Empty;
            Color customColor;
            if (!string.IsNullOrWhiteSpace(customFaction) &&
                !customFaction.StartsWith("SubFaction", StringComparison.OrdinalIgnoreCase) &&
                TryGetCustomRaceColor(catalog, customFaction, out customColor))
                return SingleGalaxyLabel(name, customColor);

            byte[] ownerOrder = { 0, 1, 2, 3, 4, 5, 7 };
            Dictionary<byte, int> owners = new Dictionary<byte, int>();
            byte firstOwner = 6;
            if (planetOwners != null)
                foreach (byte owner in planetOwners)
                    if (owner != 6)
                    {
                        int count;
                        owners.TryGetValue(owner, out count);
                        owners[owner] = count + 1;
                        if (firstOwner == 6) firstOwner = owner;
                    }

            int ownerCount = 0;
            foreach (byte owner in ownerOrder)
                if (owners.ContainsKey(owner)) ownerCount++;
            if (ownerCount == 0) return SingleGalaxyLabel(name, GalaxyOwnerColor(6));

            int chunk = name.Length / ownerCount;
            List<GalaxyLabelSegment> result = new List<GalaxyLabelSegment>();
            string remaining = name;
            int firstLength = Math.Min(chunk, remaining.Length);
            result.Add(new GalaxyLabelSegment { Text = remaining.Substring(0, firstLength),
                Color = GalaxyOwnerColor(firstOwner) });
            remaining = remaining.Substring(firstLength);
            int usedOwners = 1;
            foreach (byte owner in ownerOrder)
            {
                if (remaining.Length == 0) break;
                if (owner == firstOwner || !owners.ContainsKey(owner)) continue;
                usedOwners++;
                int length = usedOwners == ownerCount ? remaining.Length :
                    Math.Min(chunk, remaining.Length);
                result.Add(new GalaxyLabelSegment { Text = remaining.Substring(0, length),
                    Color = GalaxyOwnerColor(owner) });
                remaining = remaining.Substring(length);
            }
            if (remaining.Length != 0)
                result[result.Count - 1].Text += remaining;
            return result;
        }

        private static List<GalaxyLabelSegment> SingleGalaxyLabel(string text, Color color)
        {
            return new List<GalaxyLabelSegment> {
                new GalaxyLabelSegment { Text = text, Color = color }
            };
        }

        private static bool TryGetCustomRaceColor(GameDataCatalog catalog,
            string name, out Color color)
        {
            int rgb;
            if (catalog != null && catalog.TryGetRaceColor(name, out rgb))
            {
                color = Color.FromArgb((rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255);
                return true;
            }
            color = Color.Empty;
            return false;
        }

        internal static Color GalaxyOwnerColor(byte owner)
        {
            switch (owner)
            {
                case 0: return Color.FromArgb(0xFF, 0x60, 0x60);
                case 1: return Color.FromArgb(0x60, 0xE0, 0x60);
                case 2: return Color.FromArgb(0x60, 0xAA, 0xFF);
                case 3: return Color.FromArgb(0xFF, 0xB0, 0xF0);
                case 4: return Color.FromArgb(0xF0, 0xE0, 0x50);
                case 5: return Color.FromArgb(0x80, 0xC0, 0xD0);
                case 7: return Color.White;
                default: return Color.FromArgb(0xC8, 0xC8, 0xC8);
            }
        }

        private static float LabelBoundsPenalty(RectangleF rectangle, Size bounds)
        {
            float penalty = 0F;
            if (rectangle.Left < 0F) penalty += -rectangle.Left * 100F;
            if (rectangle.Top < 0F) penalty += -rectangle.Top * 100F;
            if (rectangle.Right > bounds.Width) penalty += (rectangle.Right - bounds.Width) * 100F;
            if (rectangle.Bottom > bounds.Height) penalty += (rectangle.Bottom - bounds.Height) * 100F;
            return penalty;
        }

        private static float IntersectionArea(RectangleF left, RectangleF right)
        {
            RectangleF intersection = RectangleF.Intersect(left, right);
            return intersection.IsEmpty ? 0F : intersection.Width * intersection.Height;
        }

        internal static void CalculateGalaxyMapBounds(IList<StarHeaderRecord> stars,
            out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = float.MaxValue; minY = float.MaxValue;
            maxX = float.MinValue; maxY = float.MinValue;
            if (stars == null || stars.Count == 0)
            {
                minX = -100F; minY = -100F; maxX = 100F; maxY = 100F;
                return;
            }

            List<float> xs = new List<float>();
            List<float> ys = new List<float>();
            foreach (StarHeaderRecord star in stars)
            {
                if (float.IsNaN(star.X) || float.IsInfinity(star.X) ||
                    float.IsNaN(star.Y) || float.IsInfinity(star.Y)) continue;
                xs.Add(star.X); ys.Add(star.Y);
            }
            int finiteCount = xs.Count;
            if (finiteCount == 0)
            {
                minX = -100F; minY = -100F; maxX = 100F; maxY = 100F;
                return;
            }
            xs.Sort(); ys.Sort();
            float medianX = xs[xs.Count / 2], medianY = ys[ys.Count / 2];
            List<float> deviationsX = new List<float>(), deviationsY = new List<float>();
            foreach (float value in xs) deviationsX.Add(Math.Abs(value - medianX));
            foreach (float value in ys) deviationsY.Add(Math.Abs(value - medianY));
            deviationsX.Sort(); deviationsY.Sort();
            float madX = deviationsX[deviationsX.Count / 2];
            float madY = deviationsY[deviationsY.Count / 2];
            // Four MADs retain the complete playable spread in both the stock and large
            // modded galaxies, while excluding the small group of disabled pirate systems
            // conventionally parked at X=500 by no-pirates mods.
            float limitX = Math.Max(250F, madX * 4F);
            float limitY = Math.Max(250F, madY * 4F);

            int fittedCount = 0;
            foreach (StarHeaderRecord star in stars)
            {
                if (float.IsNaN(star.X) || float.IsInfinity(star.X) ||
                    float.IsNaN(star.Y) || float.IsInfinity(star.Y)) continue;
                bool insideX = Math.Abs(star.X - medianX) <= limitX;
                bool insideY = Math.Abs(star.Y - medianY) <= limitY;
                if (!insideX || !insideY) continue;
                minX = Math.Min(minX, star.X); maxX = Math.Max(maxX, star.X);
                minY = Math.Min(minY, star.Y); maxY = Math.Max(maxY, star.Y);
                fittedCount++;
            }

            // For a tiny or unusual galaxy the statistical filter is not meaningful;
            // fall back to all finite systems. Normal 73-star saves retain the proven
            // 2.5-sigma protection used by the earlier working renderer.
            if (fittedCount < Math.Min(3, finiteCount))
            {
                minX = float.MaxValue; minY = float.MaxValue;
                maxX = float.MinValue; maxY = float.MinValue;
                foreach (StarHeaderRecord star in stars)
                {
                    if (float.IsNaN(star.X) || float.IsInfinity(star.X) ||
                        float.IsNaN(star.Y) || float.IsInfinity(star.Y)) continue;
                    minX = Math.Min(minX, star.X); maxX = Math.Max(maxX, star.X);
                    minY = Math.Min(minY, star.Y); maxY = Math.Max(maxY, star.Y);
                }
            }
            if (minX == float.MaxValue || maxX <= minX || maxY <= minY)
            {
                minX = -100F; minY = -100F; maxX = 100F; maxY = 100F;
            }
        }

        private static float MapX(float value, float min, float scale, float offset)
        {
            return offset + (value - min) * scale;
        }

        private static float MapY(float value, float min, float scale, float offset, int height)
        {
            return offset + (value - min) * scale;
        }

        private void GalaxyMapMouseMove(object sender, MouseEventArgs e)
        {
            if (galaxyMapDragging || e.Button == MouseButtons.Left && galaxyMapImage.Capture)
            {
                int dx = e.X - galaxyMapDragStart.X;
                int dy = e.Y - galaxyMapDragStart.Y;
                if (!galaxyMapDragging && dx * dx + dy * dy >= 16)
                    galaxyMapDragging = true;
                if (galaxyMapDragging)
                {
                    galaxyMapImage.Cursor = Cursors.SizeAll;
                    InteractivePictureBox interactive = galaxyMapImage as InteractivePictureBox;
                    if (interactive != null) interactive.PreviewOffset = new Point(dx, dy);
                    return;
                }
            }
            galaxyMapImage.Cursor = FindStarMapHit(galaxyMapHits, e.Location) == null
                ? Cursors.Default : Cursors.Hand;
        }

        private void GalaxyMapMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ResetGalaxyMapTransform(true);
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            galaxyMapDragStart = e.Location;
            galaxyMapDragPanStart = galaxyMapPan;
            galaxyMapPressedHit = FindStarMapHit(galaxyMapHits, e.Location);
            galaxyMapDragging = false;
            galaxyMapImage.Capture = true;
        }

        private void GalaxyMapMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            galaxyMapImage.Capture = false;
            StarMapHitRecord hit = galaxyMapDragging ? null : galaxyMapPressedHit;
            if (galaxyMapDragging)
            {
                int dx = e.X - galaxyMapDragStart.X;
                int dy = e.Y - galaxyMapDragStart.Y;
                galaxyMapPan = new PointF(galaxyMapDragPanStart.X + dx,
                    galaxyMapDragPanStart.Y + dy);
                InteractivePictureBox interactive = galaxyMapImage as InteractivePictureBox;
                if (interactive != null) interactive.PreviewOffset = Point.Empty;
                DrawGalaxyMap();
            }
            galaxyMapDragging = false;
            galaxyMapPressedHit = null;
            galaxyMapImage.Cursor = FindStarMapHit(galaxyMapHits, e.Location) == null
                ? Cursors.Default : Cursors.Hand;
            StarHeaderRecord star = hit == null ? null : hit.Value as StarHeaderRecord;
            if (star == null) return;
            int constellationIndex = 0;
            if (pendingConstellations != null)
                for (int index = 0; index < pendingConstellations.Count; index++)
                    if (pendingConstellations[index].StarObjectIds.Contains(star.ObjectId))
                    { constellationIndex = index + 1; break; }
            if (constellationIndex >= 0 && constellationIndex < constellationList.Items.Count)
                constellationList.SelectedIndex = constellationIndex;
            for (int index = 0; index < starList.Items.Count; index++)
            {
                StarHeaderRecord candidate = starList.Items[index] as StarHeaderRecord;
                if (candidate != null && candidate.ObjectId == star.ObjectId)
                { starList.SelectedIndex = index; break; }
            }
            if (mainTabs != null && galaxyPage != null)
                mainTabs.SelectedTab = galaxyPage;
            ShowSelectedStarMap(sender, EventArgs.Empty);
        }

        private void GalaxyMapMouseWheel(object sender, MouseEventArgs e)
        {
            if (pendingStars == null || pendingStars.Count == 0 || e.Delta == 0) return;
            float previousZoom = galaxyMapZoom;
            float factor = e.Delta > 0 ? 1.18F : 1.0F / 1.18F;
            galaxyMapZoom = Math.Max(0.45F, Math.Min(6.0F, galaxyMapZoom * factor));
            if (Math.Abs(galaxyMapZoom - previousZoom) < 0.0001F) return;
            float ratio = galaxyMapZoom / previousZoom;
            float centerX = galaxyMapImage.ClientSize.Width / 2F;
            float centerY = galaxyMapImage.ClientSize.Height / 2F;
            galaxyMapPan = new PointF(
                e.X - centerX - (e.X - centerX - galaxyMapPan.X) * ratio,
                e.Y - centerY - (e.Y - centerY - galaxyMapPan.Y) * ratio);
            DrawGalaxyMap();
        }

        private void GalaxyMapMouseLeave(object sender, EventArgs e)
        {
            if (!galaxyMapDragging) galaxyMapImage.Cursor = Cursors.Default;
        }

        private void ResetGalaxyMapTransform(bool redraw)
        {
            galaxyMapZoom = 1.0F;
            galaxyMapPan = PointF.Empty;
            galaxyMapDragging = false;
            galaxyMapPressedHit = null;
            if (galaxyMapImage != null)
            {
                galaxyMapImage.Capture = false;
                InteractivePictureBox interactive = galaxyMapImage as InteractivePictureBox;
                if (interactive != null) interactive.PreviewOffset = Point.Empty;
            }
            if (redraw && galaxyMapImage != null) DrawGalaxyMap();
        }

        private void RunSearch(object sender, EventArgs e)
        {
            const int visibleResultLimit = 5000;
            searchResults.Items.Clear();
            if (current == null) return;
            searchResults.BeginUpdate();
            int searchMatchCount = 0;
            try
            {
            string query = (searchQuery.Text ?? string.Empty).Trim();
            uint searchedId;
            bool hasId = uint.TryParse((searchId.Text ?? string.Empty).Trim(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out searchedId);
            ItemTypeSearchChoice searchedItemType = searchItemType.SelectedItem as ItemTypeSearchChoice;
            bool itemTypeOnly = searchedItemType != null && searchedItemType.FirstType >= 0;
            StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (!itemTypeOnly && pendingStars != null && SearchFilterEnabled("stars"))
            {
                foreach (StarHeaderRecord star in pendingStars)
                {
                    string localizedName = LocalizedStarName(star);
                    if (MatchesSearch(star.Name, star.ObjectId, query, hasId, searchedId, comparison) ||
                        !string.Equals(localizedName, star.Name, StringComparison.OrdinalIgnoreCase) &&
                        MatchesSearch(localizedName, star.ObjectId, query, hasId, searchedId, comparison))
                        AddSearchResult(new SearchResultEntry(star,
                            (appSettings.LanguageIndex == 1 ? "Star — " : "Звезда — ") + localizedName +
                            " | ID " + star.ObjectId.ToString(CultureInfo.InvariantCulture)),
                            ref searchMatchCount, visibleResultLimit);
                }
            }
            if (!itemTypeOnly && pendingPlanets != null && SearchFilterEnabled("planets"))
                foreach (PlanetHeaderRecord planet in pendingPlanets)
                    if (MatchesSearch(planet.Name, planet.ObjectId, query, hasId, searchedId, comparison))
                        AddSearchResult(new SearchResultEntry(planet, "Планета — " + planet.Name +
                            " | ID " + planet.ObjectId.ToString(CultureInfo.InvariantCulture)),
                            ref searchMatchCount, visibleResultLimit);
            if (!itemTypeOnly && pendingShips != null)
                foreach (ShipHeaderRecord ship in pendingShips)
                    if ((ship.IsStation ? SearchFilterEnabled("stations") : SearchFilterEnabled("ships")) &&
                        (MatchesSearch(ship.Name, ship.ObjectId, query, hasId, searchedId, comparison) ||
                         (query.Length > 0 && (ship.ScriptName ?? string.Empty).IndexOf(query, comparison) >= 0 && (!hasId || ship.ObjectId == searchedId))))
                        AddSearchResult(new SearchResultEntry(ship,
                            (ship.IsStation ? "Станция — " : "Корабль — ") + ship.Name +
                            " | ID " + ship.ObjectId.ToString(CultureInfo.InvariantCulture)),
                            ref searchMatchCount, visibleResultLimit);
            if (pendingItems != null)
            {
                HashSet<int> planetItemStarts = new HashSet<int>();
                HashSet<int> shopItemStarts = new HashSet<int>();
                HashSet<int> spaceItemStarts = new HashSet<int>();
                HashSet<int> holdItemStarts = new HashSet<int>();
                HashSet<int> satelliteItemStarts = new HashSet<int>();
                HashSet<int> dropItemStarts = new HashSet<int>();
                HashSet<int> storageItemStarts = new HashSet<int>();
                HashSet<int> modStorageItemStarts = new HashSet<int>();
                HashSet<int> tranclucatorItemStarts = new HashSet<int>();
                if (pendingPlanets != null)
                    foreach (PlanetHeaderRecord planet in pendingPlanets)
                    {
                        foreach (PlanetGoneItemRecord entry in planet.GoneItems)
                            planetItemStarts.Add(entry.ItemStart);
                        foreach (ShipItemListEntry entry in planet.EquipmentShopItems)
                            shopItemStarts.Add(entry.ItemStart);
                    }
                if (pendingShips != null)
                    foreach (ShipHeaderRecord ship in pendingShips)
                    {
                        foreach (ShipItemListEntry entry in ship.EquipmentItems) holdItemStarts.Add(entry.ItemStart);
                        foreach (ShipItemListEntry entry in ship.ArtefactItems) holdItemStarts.Add(entry.ItemStart);
                        foreach (ShipItemListEntry entry in ship.DropListItems) dropItemStarts.Add(entry.ItemStart);
                        foreach (ShipItemListEntry entry in ship.RuinsEquipmentItems) shopItemStarts.Add(entry.ItemStart);
                        if (ship.RuinsSaleSatellite != null) shopItemStarts.Add(ship.RuinsSaleSatellite.ItemStart);
                        foreach (ShipItemListEntry entry in ship.PlayerSatelliteItems)
                            satelliteItemStarts.Add(entry.ItemStart);
                        foreach (PlayerStorageItemRecord entry in ship.PlayerStorageItems)
                            storageItemStarts.Add(entry.ItemStart);
                        if (ship.PlayerBridgeRuins != null)
                            foreach (ShipItemListEntry entry in ship.PlayerBridgeRuins.RuinsEquipmentItems)
                                shopItemStarts.Add(entry.ItemStart);
                    }
                if (pendingStars != null)
                    foreach (StarHeaderRecord star in pendingStars)
                    {
                        if (star.SpaceItems != null)
                            foreach (ShipItemListEntry entry in star.SpaceItems)
                                spaceItemStarts.Add(entry.ItemStart);
                        foreach (StarDropItemRecord entry in star.DropItems) dropItemStarts.Add(entry.ItemStart);
                    }
                foreach (ItemHeaderRecord outer in pendingItems)
                    if (outer.NestedTranclucator != null)
                    {
                        foreach (ShipItemListEntry entry in outer.NestedTranclucator.EquipmentItems)
                            tranclucatorItemStarts.Add(entry.ItemStart);
                        foreach (ShipItemListEntry entry in outer.NestedTranclucator.ArtefactItems)
                            tranclucatorItemStarts.Add(entry.ItemStart);
                    }
                if (pendingStoredItems != null)
                    foreach (StoredItemRecord entry in pendingStoredItems)
                        modStorageItemStarts.Add(entry.ItemStart);
                foreach (ItemHeaderRecord item in pendingItems)
                {
                    string container = null;
                    string containerCaption = null;
                    if (modStorageItemStarts.Contains(item.Start))
                    { container = "modstorage"; containerCaption = "мод.хранилище"; }
                    else if (storageItemStarts.Contains(item.Start))
                    { container = "storage"; containerCaption = "склад"; }
                    else if (satelliteItemStarts.Contains(item.Start))
                    { container = "satellites"; containerCaption = "зонд"; }
                    else if (dropItemStarts.Contains(item.Start))
                    { container = "drops"; containerCaption = "трофей"; }
                    else if (planetItemStarts.Contains(item.Start))
                    { container = "planetitems"; containerCaption = "планета"; }
                    else if (shopItemStarts.Contains(item.Start))
                    { container = "shopitems"; containerCaption = "магазин"; }
                    else if (holdItemStarts.Contains(item.Start))
                    { container = "holds"; containerCaption = "трюм"; }
                    else if (tranclucatorItemStarts.Contains(item.Start))
                    { container = "tranclucators"; containerCaption = "транклюкатор"; }
                    else if (spaceItemStarts.Contains(item.Start))
                    { container = "spaceitems"; containerCaption = "космос"; }
                    if (container != null && SearchFilterEnabled(container) &&
                        !pendingDeletedItemStarts.Contains(item.Start) &&
                        (searchedItemType == null || searchedItemType.Matches(item.Type)) &&
                        (MatchesSearch(ItemDisplayName(item), item.ObjectId, query, hasId, searchedId, comparison) ||
                         (query.Length > 0 && (item.Name ?? string.Empty).IndexOf(query, comparison) >= 0 && (!hasId || item.ObjectId == searchedId)) ||
                         (query.Length > 0 && (item.SystemName ?? string.Empty).IndexOf(query, comparison) >= 0 && (!hasId || item.ObjectId == searchedId))))
                        AddSearchResult(new SearchResultEntry(item,
                            ItemDisplayCaption(item, containerCaption)), ref searchMatchCount,
                            visibleResultLimit);
                }
            }
            if (!itemTypeOnly && pendingAsteroids != null && SearchFilterEnabled("asteroids"))
                foreach (AsteroidRecord asteroid in pendingAsteroids)
                    if (MatchesSearch(asteroid.GraphName, asteroid.ObjectId, query, hasId, searchedId, comparison))
                        AddSearchResult(new SearchResultEntry(asteroid,
                            "Астероид — " + asteroid.GraphName + " | ID " +
                            asteroid.ObjectId.ToString(CultureInfo.InvariantCulture)),
                            ref searchMatchCount, visibleResultLimit);
            if (!itemTypeOnly && pendingMissiles != null && SearchFilterEnabled("missiles"))
                foreach (MissileRecord missile in pendingMissiles)
                {
                    string missileName = missile.IsCustom ? missile.CustomWeaponName : "type " + missile.WeaponType;
                    if (MatchesSearch(missileName, missile.ObjectId, query, hasId, searchedId, comparison))
                        AddSearchResult(new SearchResultEntry(missile,
                            "Ракета — " + missileName + " | ID " +
                            missile.ObjectId.ToString(CultureInfo.InvariantCulture)),
                            ref searchMatchCount, visibleResultLimit);
                }
            if (searchMatchCount > visibleResultLimit)
                searchResults.Items.Add(new SearchResultEntry(null,
                    "Показаны первые " + visibleResultLimit.ToString(CultureInfo.InvariantCulture) +
                    " из " + searchMatchCount.ToString(CultureInfo.InvariantCulture) +
                    ". Уточните имя, ID, тип или контейнер."));
            }
            finally { searchResults.EndUpdate(); }
        }

        private void AddSearchResult(SearchResultEntry entry, ref int matchCount, int visibleLimit)
        {
            matchCount++;
            if (matchCount <= visibleLimit) searchResults.Items.Add(entry);
        }

        private static bool MatchesSearch(string name, uint objectId, string query, bool hasId,
            uint searchedId, StringComparison comparison)
        {
            bool nameMatches = query.Length == 0 || (name ?? string.Empty).IndexOf(query, comparison) >= 0;
            bool idMatches = !hasId || objectId == searchedId;
            return nameMatches && idMatches;
        }

        private string ItemTypeName(byte type)
        {
            string[] localized = appSettings.LanguageIndex == 1 ? searchItemTypeNamesEn : searchItemTypeNamesRu;
            if (type < localized.Length && (type < 50 || type > 67)) return localized[type];
            if (type <= 7) return CommodityName(type);
            switch (type)
            {
                default: return type >= 50 && type <= 67 ? WeaponTypeName(type) :
                    (appSettings.LanguageIndex == 1 ? "Item" : "Предмет");
            }
        }

        private void ApplyOriginalCrcReferencePolicy()
        {
            crcReferenceProblems.Clear(); crcReferenceCorrections.Clear();
            crcReferencesReadAsIs = false;
            CrcReferenceAuditResult audit = CrcReferencePolicy.Apply(gameCatalog,
                pendingItems, pendingMissiles, false);
            if (audit.Problems.Count == 0) return;
            crcReferenceProblems.AddRange(audit.Problems);

            if (suppressLoadPrompts)
            {
                crcReferencesReadAsIs = true;
                return;
            }

            bool correction = EditorFormFactory.ShowBonusCrcAlert(this);
            if (!correction)
            {
                crcReferencesReadAsIs = true;
                return;
            }
            CrcReferenceAuditResult corrected = CrcReferencePolicy.Apply(gameCatalog,
                pendingItems, pendingMissiles, true);
            crcReferenceCorrections.AddRange(corrected.Corrections);
        }

        private void RefreshLog()
        {
            if (logViews == null || current == null) return;
            logViews[0].Text = string.Join(Environment.NewLine, new string[]
            {
                "Файл: " + current.SourcePath,
                "Версия: " + current.Header[1],
                "Main payload: " + current.MainPayload.Length + " байт",
                "TGalaxy: 0x" + current.GalaxyOffset.ToString("X") + " / late: 0x" + current.GalaxySummary.TurnOffset.ToString("X") +
                    " / Cheats.Test: 0x" + current.GalaxySummary.CheatsTestOffset.ToString("X"),
                "Сообщений: " + current.PlayerMessageCount + "; трюм: " + current.PlayerHoldCount,
                "Созвездий: " + current.GalaxyConstellationCount + "; звёзд: " + current.GalaxyStars.Count +
                    "; Cheats.Test: " + current.GalaxySummary.CheatsTest,
            });
            logViews[1].Text = "Ошибок чтения нет.";
            List<string> crcLines = new List<string>();
            crcLines.Add(current.MainCrcValid ? "CRC32 основного ZL01-блока совпадает." :
                "CRC32 основного ZL01-блока не совпадает.");
            if (crcReferenceProblems.Count == 0)
                crcLines.Add("Индексированные CRC бонусов и серий корпусов совпадают с каталогами Lang.dat.");
            else
            {
                crcLines.AddRange(crcReferenceProblems);
                if (crcReferencesReadAsIs)
                    crcLines.Add("Выбрано чтение как есть: исходные index/CRC сохранены.");
            }
            logViews[2].Text = string.Join(Environment.NewLine, crcLines.ToArray());

            if (crcReferenceCorrections.Count != 0)
                logViews[3].Text = string.Join(Environment.NewLine, crcReferenceCorrections.ToArray());
            else if (crcReferenceProblems.Count != 0 && crcReferencesReadAsIs)
                logViews[3].Text = "Коррекция отменена: несовпадающие ссылки сохранены побайтно.";
            else
                logViews[3].Text = "Коррекция CRC не потребовалась.";
        }

        private void EditSelectedMessage(object sender, EventArgs e)
        {
            int index = messageList.SelectedIndex;
            if (pendingMessages == null || index < 0 || index >= pendingMessages.Count) return;
            PlayerMessageRecord message = pendingMessages[index];
            EditorFormDefinition definition = EditorFormDefinitions.Get("TMESSAGEFORM");
            using (Form form = EditorFormFactory.Build(definition))
            {
                TextBox edId = FindControl<TextBox>(form, "edID");
                TextBox edCustomType = FindControl<TextBox>(form, "edCustomType");
                ComboBox cbType = FindControl<ComboBox>(form, "cbMessageType");
                TextBox edSound = FindControl<TextBox>(form, "edSoundType");
                TextBox edTurn = FindControl<TextBox>(form, "edTurn");
                TextBox memo = FindControl<TextBox>(form, "mmTextMessage");
                CheckBox playerRead = FindControl<CheckBox>(form, "chbPlayerRead");
                CheckBox noSound = FindControl<CheckBox>(form, "chbNoSound");
                CheckBox hideTags = FindControl<CheckBox>(form, "chbHideTags");
                string[] objectNames = { "edObjShip1", "edObjShip2", "edObjShip3", "edObjPlanet1", "edObjPlanet2", "edObjPlanet3" };
                TextBox[] objects = new TextBox[objectNames.Length];
                for (int objectIndex = 0; objectIndex < objectNames.Length; objectIndex++)
                    objects[objectIndex] = FindControl<TextBox>(form, objectNames[objectIndex]);

                edId.Text = message.Text ?? string.Empty;
                edCustomType.Text = message.LateText ?? string.Empty;
                cbType.Items.Clear();
                for (int type = 0; type <= byte.MaxValue; type++) cbType.Items.Add(MessageTypeName((byte)type));
                cbType.SelectedIndex = message.MessageType;
                edSound.Text = message.Raw18.ToString();
                edTurn.Text = message.Raw1C.ToString();
                string rawMessageText = message.FormattedText ?? string.Empty;
                memo.Text = rawMessageText;
                playerRead.Checked = message.Flag40;
                noSound.Checked = message.Flag41;
                hideTags.Checked = true;
                memo.ReadOnly = false;
                for (int objectIndex = 0; objectIndex < objects.Length; objectIndex++)
                    objects[objectIndex].Text = message.RawU32[objectIndex].ToString();

                hideTags.CheckedChanged += delegate
                {
                    if (hideTags.Checked)
                    {
                        memo.Text = rawMessageText;
                        memo.ReadOnly = false;
                    }
                    else
                    {
                        rawMessageText = memo.Text;
                        memo.Text = FilterGameTextTags(rawMessageText);
                        memo.ReadOnly = true;
                    }
                };
                form.KeyPreview = true;
                form.KeyDown += delegate(object keySender, KeyEventArgs args)
                {
                    if (args.KeyCode == Keys.Escape) form.Close();
                };

                form.ShowDialog(this);

                int raw18 = 0, raw1C = 0;
                uint[] rawU32 = new uint[6];
                bool valid = int.TryParse(edSound.Text, out raw18) && int.TryParse(edTurn.Text, out raw1C);
                for (int objectIndex = 0; objectIndex < objects.Length; objectIndex++)
                    valid &= uint.TryParse(objects[objectIndex].Text, out rawU32[objectIndex]);
                if (!valid)
                {
                    MessageBox.Show(this, "Числовые поля сообщения не применены: ожидались Int32/UInt32.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                message.Text = edId.Text;
                message.LateText = edCustomType.Text;
                message.MessageType = checked((byte)Math.Max(0, cbType.SelectedIndex));
                message.Raw18 = raw18;
                message.Raw1C = raw1C;
                message.FormattedText = hideTags.Checked ? memo.Text : rawMessageText;
                message.Flag40 = playerRead.Checked;
                message.Flag41 = noSound.Checked;
                message.RawU32 = rawU32;
                RefreshMessageList();
                messageList.SelectedIndex = index;
            }
        }

        private void DeleteSelectedMessages(object sender, EventArgs e)
        {
            if (pendingMessages == null || messageList.SelectedIndices.Count == 0) return;
            List<int> indices = new List<int>();
            foreach (int index in messageList.SelectedIndices) indices.Add(index);
            indices.Sort();
            for (int position = indices.Count - 1; position >= 0; position--)
                if (indices[position] >= 0 && indices[position] < pendingMessages.Count)
                    pendingMessages.RemoveAt(indices[position]);
            if (pendingMetadata != null)
                pendingMetadata.PlayerMessageCount = checked((uint)pendingMessages.Count);
            RefreshMessageList();
        }

        private static string FilterGameTextTags(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            StringBuilder result = new StringBuilder(value.Length);
            int index = 0;
            while (index < value.Length)
            {
                if (value[index] != '<')
                {
                    result.Append(value[index++]);
                    continue;
                }
                if (index + 1 < value.Length && value[index + 1] == '<')
                {
                    index++;
                    continue;
                }
                int close = value.IndexOf('>', index + 1);
                if (close < 0)
                {
                    result.Append(value[index++]);
                    continue;
                }
                index = close + 1;
            }
            return result.ToString();
        }

        private static string GameTextPreview(string value, int maximumLength)
        {
            string plain = FilterGameTextTags(value);
            StringBuilder compact = new StringBuilder(plain.Length);
            bool pendingSpace = false;
            foreach (char character in plain)
            {
                if (char.IsWhiteSpace(character))
                {
                    pendingSpace = compact.Length != 0;
                    continue;
                }
                if (pendingSpace) compact.Append(' ');
                compact.Append(character);
                pendingSpace = false;
            }
            string result = compact.ToString();
            if (maximumLength > 3 && result.Length > maximumLength)
                result = result.Substring(0, maximumLength - 1).TrimEnd() + "…";
            return result;
        }

        private static T FindControl<T>(Control root, string name) where T : Control
        {
            Form form = root as Form ?? root.FindForm();
            Dictionary<string, Control> controls = form == null ? null :
                form.Tag as Dictionary<string, Control>;
            Control registered;
            if (controls != null && controls.TryGetValue(name, out registered) && registered is T)
                return (T)registered;
            Control[] found = root.Controls.Find(name, true);
            if (found.Length != 0 && found[0] is T) return (T)found[0];
            throw new InvalidOperationException("Контрол формы не найден: " + name);
        }

        private static void ReplaceImage(PictureBox box, Image image)
        {
            Image old = box.Image; box.Image = image; if (old != null) old.Dispose();
            if (image == null) box.BackColor = Color.Transparent;
        }
    }

}
