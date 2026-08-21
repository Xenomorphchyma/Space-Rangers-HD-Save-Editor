using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SpaceRangersHdSaveEditor
{
    internal sealed class SavFormatException : Exception
    {
        internal SavFormatException(string message) : base(message) { }
    }

    internal sealed class SavMetadata
    {
        internal int CurrentForm;
        internal int CameraX;
        internal int CameraY;
        internal bool ShowPanel;
        internal bool ViewFollow;
        internal bool CalcHeader;
        internal uint Tips;
        internal uint PlayerMessageCount;

        internal SavMetadata Clone()
        {
            return (SavMetadata)MemberwiseClone();
        }

        internal bool EditableEquals(SavMetadata other)
        {
            return CameraX == other.CameraX && CameraY == other.CameraY &&
                ShowPanel == other.ShowPanel && ViewFollow == other.ViewFollow &&
                CalcHeader == other.CalcHeader && Tips == other.Tips;
        }
    }

    internal sealed class PlayerMessageRecord
    {
        internal int Start;
        internal int End;
        internal string Text;
        internal byte MessageType;
        internal int Raw18;
        internal int Raw1C;
        internal string FormattedText;
        internal bool RawBool;
        internal uint[] RawU32;
        internal bool Flag40;
        internal bool Flag41;
        internal string LateText;

        internal PlayerMessageRecord Clone()
        {
            PlayerMessageRecord result = (PlayerMessageRecord)MemberwiseClone();
            result.RawU32 = (uint[])RawU32.Clone();
            return result;
        }

        internal bool ContentEquals(PlayerMessageRecord other)
        {
            if (other == null || Text != other.Text || MessageType != other.MessageType ||
                Raw18 != other.Raw18 || Raw1C != other.Raw1C || FormattedText != other.FormattedText ||
                RawBool != other.RawBool || Flag40 != other.Flag40 || Flag41 != other.Flag41 ||
                LateText != other.LateText || RawU32.Length != other.RawU32.Length)
                return false;
            for (int index = 0; index < RawU32.Length; index++)
                if (RawU32[index] != other.RawU32[index]) return false;
            return true;
        }
    }

    internal sealed class PlayerHoldRecord
    {
        internal int Start;
        internal int End;
        internal byte UnitType;
        internal byte Goods;
        internal uint ObjectId;
    }

    internal sealed class CustomSystemInfoRecord
    {
        internal int Start;
        internal int End;
        internal string Name;
        internal string Icon;
        internal string Info;
        internal string Type;
        internal int Distance;

        internal CustomSystemInfoRecord Clone()
        {
            return (CustomSystemInfoRecord)MemberwiseClone();
        }

        internal bool ContentEquals(CustomSystemInfoRecord other)
        {
            return other != null && Name == other.Name && Icon == other.Icon &&
                Info == other.Info && Type == other.Type && Distance == other.Distance;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? "TCustomSystemInfo" : Name;
        }
    }

    internal sealed class StarDropItemRecord
    {
        internal int Start;
        internal int End;
        internal float X;
        internal float Y;
        internal uint ShipObjectId;
        internal bool InUse;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal StarDropItemRecord Clone()
        {
            return (StarDropItemRecord)MemberwiseClone();
        }

        internal bool ContentEquals(StarDropItemRecord other)
        {
            return other != null && X == other.X && Y == other.Y &&
                ShipObjectId == other.ShipObjectId && InUse == other.InUse &&
                ItemType == other.ItemType && ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return "TItem " + ItemType + ":" + ItemObjectId + "  [" + X + "; " + Y + "]";
        }
    }

    internal sealed class StarShipRecord
    {
        internal int Start;
        internal int End;
        internal byte ShipType;
        internal int ShipStart;
        internal uint ShipObjectId;
        internal bool OpaqueTail;

        internal StarShipRecord Clone()
        {
            return (StarShipRecord)MemberwiseClone();
        }

        internal bool ContentEquals(StarShipRecord other)
        {
            return other != null && ShipType == other.ShipType &&
                ShipObjectId == other.ShipObjectId && OpaqueTail == other.OpaqueTail;
        }
    }

    internal sealed class StarHeaderRecord
    {
        internal int Start;
        internal int HeaderEnd;
        internal int AsteroidCountOffset;
        internal int SpaceShipCountOffset;
        internal bool HasExactSpaceShipList;
        internal List<StarShipRecord> SpaceShips = new List<StarShipRecord>();
        internal int MissileCountOffset;
        internal uint ObjectId;
        internal int Raw08;
        internal uint Raw0C;
        internal string Name;
        internal float X;
        internal float Y;
        internal ushort Raw1C;
        internal byte Raw78;
        internal ushort PlanetCount;
        internal int TailStart;
        internal int TailEnd;
        internal uint ConstellationObjectId;
        internal string GraphType;
        internal bool Battle;
        internal byte Safety;
        internal byte Overloading;
        internal byte Owners;
        internal byte LastOwners;
        internal byte DominatorSeries;
        internal string CustomFaction;
        internal float SafeRadius;
        internal float DamageRadius;
        internal ushort GraphRadius;
        internal string GraphStar;
        internal bool WarPlayer;
        internal byte DayBeforeOccupy;
        internal int DayWithoutPlayer;
        internal int DayWithoutCreateShip;
        internal int LastDominatorDate;
        internal int LastPirateDate;
        internal int LiberationDate;
        internal int DayInvadeInertia;
        internal bool NoComeKling;
        internal uint DominionObjectId;
        internal string MapLabel;
        internal int CustomInfoCountOffset;
        internal List<CustomSystemInfoRecord> CustomSystemInfos = new List<CustomSystemInfoRecord>();
        internal int SpaceItemCountOffset;
        internal List<ShipItemListEntry> SpaceItems = new List<ShipItemListEntry>();
        internal int DropItemCountOffset;
        internal int MissileListOffset;
        internal List<StarDropItemRecord> DropItems = new List<StarDropItemRecord>();

        internal StarHeaderRecord Clone()
        {
            StarHeaderRecord clone = (StarHeaderRecord)MemberwiseClone();
            clone.CustomSystemInfos = new List<CustomSystemInfoRecord>();
            foreach (CustomSystemInfoRecord record in CustomSystemInfos)
                clone.CustomSystemInfos.Add(record.Clone());
            clone.SpaceItems = new List<ShipItemListEntry>();
            foreach (ShipItemListEntry record in SpaceItems) clone.SpaceItems.Add(record.Clone());
            clone.SpaceShips = new List<StarShipRecord>();
            foreach (StarShipRecord record in SpaceShips) clone.SpaceShips.Add(record.Clone());
            clone.DropItems = new List<StarDropItemRecord>();
            foreach (StarDropItemRecord record in DropItems) clone.DropItems.Add(record.Clone());
            return clone;
        }

        internal bool ContentEquals(StarHeaderRecord other)
        {
            return other != null && ObjectId == other.ObjectId && Raw08 == other.Raw08 &&
                Raw0C == other.Raw0C && Name == other.Name && X == other.X && Y == other.Y &&
                Raw1C == other.Raw1C && Raw78 == other.Raw78 && PlanetCount == other.PlanetCount &&
                TailContentEquals(other) && SpaceShipsContentEquals(other) && SpaceItemsContentEquals(other) &&
                DropItemsContentEquals(other);
        }

        internal bool SpaceShipsContentEquals(StarHeaderRecord other)
        {
            if (other == null || SpaceShips == null || other.SpaceShips == null ||
                SpaceShips.Count != other.SpaceShips.Count) return false;
            for (int index = 0; index < SpaceShips.Count; index++)
                if (!SpaceShips[index].ContentEquals(other.SpaceShips[index])) return false;
            return true;
        }

        internal bool TailContentEquals(StarHeaderRecord other)
        {
            if (other == null || ConstellationObjectId != other.ConstellationObjectId ||
                GraphType != other.GraphType || Battle != other.Battle || Safety != other.Safety ||
                Overloading != other.Overloading || Owners != other.Owners ||
                LastOwners != other.LastOwners || DominatorSeries != other.DominatorSeries ||
                CustomFaction != other.CustomFaction || SafeRadius != other.SafeRadius ||
                DamageRadius != other.DamageRadius || GraphRadius != other.GraphRadius ||
                GraphStar != other.GraphStar || WarPlayer != other.WarPlayer ||
                DayBeforeOccupy != other.DayBeforeOccupy || DayWithoutPlayer != other.DayWithoutPlayer ||
                DayWithoutCreateShip != other.DayWithoutCreateShip ||
                LastDominatorDate != other.LastDominatorDate || LastPirateDate != other.LastPirateDate ||
                LiberationDate != other.LiberationDate || DayInvadeInertia != other.DayInvadeInertia ||
                NoComeKling != other.NoComeKling || DominionObjectId != other.DominionObjectId ||
                MapLabel != other.MapLabel || CustomSystemInfos == null ||
                other.CustomSystemInfos == null || CustomSystemInfos.Count != other.CustomSystemInfos.Count)
                return false;
            for (int index = 0; index < CustomSystemInfos.Count; index++)
                if (!CustomSystemInfos[index].ContentEquals(other.CustomSystemInfos[index])) return false;
            return true;
        }

        internal bool SpaceItemsContentEquals(StarHeaderRecord other)
        {
            if (other == null || SpaceItems == null || other.SpaceItems == null ||
                SpaceItems.Count != other.SpaceItems.Count) return false;
            for (int index = 0; index < SpaceItems.Count; index++)
                if (!SpaceItems[index].ContentEquals(other.SpaceItems[index])) return false;
            return true;
        }

        internal bool DropItemsContentEquals(StarHeaderRecord other)
        {
            if (other == null || DropItems == null || other.DropItems == null ||
                DropItems.Count != other.DropItems.Count) return false;
            for (int index = 0; index < DropItems.Count; index++)
                if (!DropItems[index].ContentEquals(other.DropItems[index])) return false;
            return true;
        }

        public override string ToString()
        {
            return ObjectId.ToString("00") + "  " + Name;
        }
    }

    internal sealed class ShipItemListEntry
    {
        internal int Start;
        internal int End;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal ShipItemListEntry Clone() { return (ShipItemListEntry)MemberwiseClone(); }
        internal bool ContentEquals(ShipItemListEntry other)
        {
            return other != null && ItemType == other.ItemType && ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return "TItem " + ItemType + ":" + ItemObjectId + " 0x" + Start.ToString("X") +
                "..0x" + End.ToString("X");
        }
    }

    internal sealed class PlayerStorageItemRecord
    {
        internal int Start;
        internal int End;
        internal bool IsStation;
        internal uint PlaceObjectId;
        internal int Slot;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal PlayerStorageItemRecord Clone()
        {
            return (PlayerStorageItemRecord)MemberwiseClone();
        }

        internal bool ContentEquals(PlayerStorageItemRecord other)
        {
            return other != null && IsStation == other.IsStation &&
                PlaceObjectId == other.PlaceObjectId && Slot == other.Slot &&
                ItemType == other.ItemType && ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return (IsStation ? "Станция " : "Планета ") + PlaceObjectId +
                ", слот " + Slot + "  [" + ItemType + ":" + ItemObjectId + "]";
        }
    }

    internal sealed class ShipSpecialBonusRecord
    {
        internal int Start;
        internal int End;
        internal byte BonusType;
        internal int Value;
        internal ShipSpecialBonusRecord Clone() { return (ShipSpecialBonusRecord)MemberwiseClone(); }
    }

    internal sealed class ShipStatusEffectRecord
    {
        internal int Start;
        internal int End;
        internal byte EffectType;
        internal float Value;
        internal uint LastSourceShipId;
        internal ShipStatusEffectRecord Clone() { return (ShipStatusEffectRecord)MemberwiseClone(); }
    }

    internal sealed class RangerQuestRecord
    {
        internal int Start;
        internal int End;
        internal byte Type;
        internal ushort Number;
        internal uint PlanetObjectId;
        internal int Turn;
        internal int Reward;
        internal uint ObjectId;
        internal bool Successful;
        internal string Text;
        internal string Congratulations;
        internal string SpecialText;

        internal RangerQuestRecord Clone() { return (RangerQuestRecord)MemberwiseClone(); }
        internal bool ContentEquals(RangerQuestRecord other)
        {
            return other != null && Type == other.Type && Number == other.Number &&
                PlanetObjectId == other.PlanetObjectId && Turn == other.Turn && Reward == other.Reward &&
                ObjectId == other.ObjectId && Successful == other.Successful && Text == other.Text &&
                Congratulations == other.Congratulations && SpecialText == other.SpecialText;
        }
        public override string ToString()
        {
            string caption = string.IsNullOrWhiteSpace(Text) ? "Задание " + Number :
                Text.Replace('\r', ' ').Replace('\n', ' ').Trim();
            return caption.Length > 70 ? caption.Substring(0, 67) + "..." : caption;
        }
    }

    internal sealed class PlayerJournalRecord
    {
        internal int Start;
        internal int End;
        internal int Turn;
        internal string Text;

        internal PlayerJournalRecord Clone() { return (PlayerJournalRecord)MemberwiseClone(); }
        internal bool ContentEquals(PlayerJournalRecord other)
        {
            return other != null && Turn == other.Turn && Text == other.Text;
        }
        public override string ToString()
        {
            string caption = (Text ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ').Trim();
            if (caption.Length > 70) caption = caption.Substring(0, 67) + "...";
            return "Ход " + Turn + (caption.Length == 0 ? string.Empty : ": " + caption);
        }
    }

    internal sealed class PlayerRobotMapRecord
    {
        internal int Start;
        internal int End;
        internal int Id;
        internal int Time;
        internal int BuildRobot;
        internal int KillRobot;
        internal int BuildTurret;
        internal int KillTurret;
        internal int KillBuilding;
        internal int Bonus;
        internal int State;
        internal int Turn;

        internal PlayerRobotMapRecord Clone()
        {
            return (PlayerRobotMapRecord)MemberwiseClone();
        }

        internal bool ContentEquals(PlayerRobotMapRecord other)
        {
            return other != null && Id == other.Id && Time == other.Time &&
                BuildRobot == other.BuildRobot && KillRobot == other.KillRobot &&
                BuildTurret == other.BuildTurret && KillTurret == other.KillTurret &&
                KillBuilding == other.KillBuilding && Bonus == other.Bonus &&
                State == other.State && Turn == other.Turn;
        }

        public override string ToString()
        {
            return "ID " + Id + " — ход " + Turn + ", состояние " + State;
        }
    }

    internal sealed class ShipIllnessRecord
    {
        internal int Start;
        internal int Index;
        internal bool Stimulator;
        internal float Infection;
        internal int InfectionDay;
        internal int InfectionEndDay;
        internal int InfectionCount;

        internal ShipIllnessRecord Clone() { return (ShipIllnessRecord)MemberwiseClone(); }
        internal bool ContentEquals(ShipIllnessRecord other)
        {
            return other != null && Index == other.Index && Stimulator == other.Stimulator &&
                Infection == other.Infection && InfectionDay == other.InfectionDay &&
                InfectionEndDay == other.InfectionEndDay && InfectionCount == other.InfectionCount;
        }
        public override string ToString()
        {
            return (Stimulator ? "Стимулятор " : "Болезнь ") + Index +
                ": " + Infection.ToString("0.00", CultureInfo.InvariantCulture) + "%";
        }
    }

    internal sealed class CustomShipInfoRecord
    {
        internal int Start;
        internal int End;
        internal string Name;
        internal string Description;
        internal int Data1;
        internal int Data2;
        internal int Data3;
        internal string TextData1;
        internal string TextData2;
        internal string TextData3;

        internal CustomShipInfoRecord Clone() { return (CustomShipInfoRecord)MemberwiseClone(); }
        public override string ToString() { return string.IsNullOrEmpty(Name) ? "TCustomShipInfo" : Name; }
    }

    internal sealed class ShipHeaderRecord
    {
        internal int Start;
        internal int NameEnd;
        internal int ScriptNameEnd;
        internal int FixedPrefixEnd;
        internal uint ObjectId;
        internal byte Type;
        internal byte Owner;
        internal bool IsPlayer;
        internal string Name;
        internal string ScriptName;
        internal float X;
        internal float Y;
        internal uint HomePlanetId;
        internal uint CurrentStarId;
        internal uint CurrentPlanetId;
        internal uint CurrentShipId;
        internal uint[,] Goods = new uint[8, 4];
        internal uint Money;
        internal uint Rnd;
        internal uint RndOut;
        internal uint Day;
        internal int Face;
        internal byte PilotRace;
        internal ushort EquipmentItemCount;
        internal int EquipmentListStart;
        internal int ArtefactCountOffset;
        internal int DropListCountOffset;
        internal int SpecialBonusCountOffset;
        internal int StatusEffectCountOffset;
        internal int CustomShipInfoCountOffset;
        internal int TakeItemReferenceCountOffset;
        internal int RecentlyDroppedItemCountOffset;
        internal int PreCommonTailEnd;
        internal bool HasPreCommonCollections;
        internal List<ShipItemListEntry> EquipmentItems = new List<ShipItemListEntry>();
        internal List<ShipItemListEntry> ArtefactItems = new List<ShipItemListEntry>();
        internal List<ShipItemListEntry> DropListItems = new List<ShipItemListEntry>();
        internal List<ShipSpecialBonusRecord> SpecialBonuses = new List<ShipSpecialBonusRecord>();
        internal List<ShipStatusEffectRecord> StatusEffects = new List<ShipStatusEffectRecord>();
        internal List<CustomShipInfoRecord> CustomShipInfos = new List<CustomShipInfoRecord>();
        internal List<uint> TakeItemReferenceIds = new List<uint>();
        internal List<uint> RecentlyDroppedItemIds = new List<uint>();
        internal uint GoodShipId;
        internal uint BadShipId;
        internal uint PartnerShipId;
        internal int PartnerGood;
        internal bool HasCommonTail;
        internal int CommonTailOffset;
        internal int GraphNameOffset;
        internal int GraphNameEnd;
        internal int CommonScalarOffset;
        internal int RewardListOffset;
        internal int RewardListEndOffset;
        internal int RelationCountOffset;
        internal int RelationEndOffset;
        internal ushort RelationCount;
        internal byte[] RelationToRangers = new byte[0];
        internal int SwarmAnimationOffset;
        internal int SwarmAnimationEnd;
        internal int CommonTailEnd;
        internal List<ShipIllnessRecord> Illnesses = new List<ShipIllnessRecord>();
        internal List<byte> Rewards = new List<byte>();
        internal bool Forsage;
        internal float Angle;
        internal byte OrderType;
        internal uint OrderData;
        internal uint OrderObjectId;
        internal float OrderDestinationX;
        internal float OrderDestinationY;
        internal bool OrderAbsolute;
        internal bool Abducted;
        internal int DaysLanded;
        internal byte ScriptOrderAbsolute;
        internal bool GraphDominator;
        internal string GraphName;
        internal byte GraphShipTransparency;
        internal bool InHyperSpace;
        internal float RadiusStop;
        internal bool ShipDestroy;
        internal byte[] Skills = new byte[6];
        internal ushort Protoplasm;
        internal uint Points;
        internal uint FreePoints;
        internal ushort DayWithoutPlayer;
        internal ushort GroupOrder;
        internal int LastNextDay;
        internal bool ChameleonEnabled;
        internal byte ChameleonSeries;
        internal byte BlazerChameleonDetect;
        internal byte KellerChameleonDetect;
        internal byte TerronChameleonDetect;
        internal int BlazerChameleonCharge;
        internal int KellerChameleonCharge;
        internal int TerronChameleonCharge;
        internal byte TechLevelKnowledge;
        internal int TradePenalty;
        internal int TradePoints;
        internal int ContrabandPoints;
        internal int RewardViewCount;
        internal bool NoDrop;
        internal byte NoTarget;
        internal bool NoTalk;
        internal bool NoScan;
        internal bool ScriptChameleon;
        internal bool RobbedByPlayer;
        internal ushort CountOfDeflectedPlayerShots;
        internal int Swarmed;
        internal uint SwarmedByShipId;
        internal string SwarmAnimation;
        internal byte CurrentStanding;
        internal int AverageSpeed;
        internal int AverageEnemySpeed;
        internal float AverageEquipmentValue;
        internal int AverageCapital;
        internal float AverageMoneyToCapital;
        internal float AverageFreeSpaceRatio;
        internal float RatioOfTooCostlyEquipmentInShop;
        internal bool HasNormalShipTail;
        internal int NormalShipTailOffset;
        internal int KillAllShips;
        internal int KillPirates;
        internal int KillDominators;
        internal int LiberationSystems;
        internal int KillPacifics;
        internal int KillWarriors;
        internal int KillRangers;
        internal ushort KillInCurrentSystemDominators;
        internal ushort KillInCurrentSystemPirates;
        internal ushort KillInCurrentSystemNormals;
        internal ushort KillCustomInCurrentSystem;
        internal uint LiberationPlanetId;
        internal int LiberationKills;
        internal byte CoalitionRank;
        internal ushort CoalitionRankPoints;
        internal byte PirateRank;
        internal uint PirateRankPoints;
        internal uint LastPlanetId;
        internal int TurnPlayerMoneyGoods;
        internal bool HasSimpleDerivedTail;
        internal int SimpleDerivedTailOffset;
        internal byte DominatorType;
        internal byte DominatorSeries;
        internal int RunProgramDate;
        internal byte RunProgramName;
        internal byte TransportType;
        internal byte WarriorType;
        internal uint PiratePrison;
        internal byte PirateType;
        internal float DesireConflict;
        internal bool HasRangerTail;
        internal int RangerTailOffset;
        internal int RangerPostQuestOffset;
        internal ushort RangerQuestCount;
        internal List<RangerQuestRecord> RangerQuests = new List<RangerQuestRecord>();
        internal byte RangerStatusTrader;
        internal byte RangerStatusPirate;
        internal byte RangerStatusWarrior;
        internal byte EminentPointsTrader;
        internal byte EminentPointsPirate;
        internal byte EminentPointsWarrior;
        internal byte RangerMoral;
        internal byte Courageous;
        internal byte StatusChangeWarrior;
        internal byte StatusChangePirate;
        internal byte StatusChangeTrader;
        internal uint RangerPrison;
        internal uint LastShipId;
        internal int Nods;
        internal int[] ProgramCounts = new int[12];
        internal bool ExcludedFromRating;
        internal bool HasTranclucatorTail;
        internal int TranclucatorTailOffset;
        internal int TranclucatorArtStringEnd;
        internal int TranclucatorPostArtOffset;
        internal uint TranclucatorProprietorShipId;
        internal bool TranclucatorDocking;
        internal bool TranclucatorSeekItems;
        internal bool TranclucatorAutoArrange;
        internal int TranclucatorArtSize;
        internal string TranclucatorArtSystemName;
        internal bool[] TranclucatorSeekPermits = new bool[7];
        internal bool[] TranclucatorLandPermits = new bool[2];
        internal bool TranclucatorLandStorage;
        internal bool HasRuinsTail;
        internal int RuinsEquipmentCountOffset;
        internal int RuinsEquipmentEndOffset;
        internal int RuinsShopTailOffset;
        internal int RuinsFinalFlagsOffset;
        internal ushort RuinsEquipmentItemCount;
        internal List<ShipItemListEntry> RuinsEquipmentItems = new List<ShipItemListEntry>();
        internal ShipItemListEntry RuinsSaleSatellite;
        internal int[,] RuinsShopGoods = new int[8, 3];
        internal int RuinsEnergy;
        internal uint RuinsFlyToStarId;
        internal int RuinsFlyDate;
        internal bool RuinsSponsor;
        internal bool RuinsSpecialShip;
        internal bool RuinsNoLanding;
        internal byte RuinsNoShopUpdate;
        internal bool HasPlayerPrefix;
        internal int PlayerPrefixOffset;
        internal bool PlayerPrison;
        internal bool PlayerTalkLocked;
        internal bool PlayerScanLocked;
        internal int KillShipInHyperSpace;
        internal int KillShipInHole;
        internal int[] KillDominatorsByType = new int[8];
        internal byte[] ChameleonLogic = new byte[3];
        internal bool HasPlayerStorageItems;
        internal int PlayerStorageItemCountOffset;
        internal int PlayerStorageItemsEndOffset;
        internal List<PlayerStorageItemRecord> PlayerStorageItems =
            new List<PlayerStorageItemRecord>();
        // Compatibility alias for older probes; this is the serialized
        // TPlayer.StorageItems count.
        internal int PlayerObjectStateCount;
        internal bool HasPlayerFinancialTail;
        internal int PlayerFinancialOffset;
        internal int PlayerDebt;
        internal int PlayerDebtDate;
        internal int PlayerDebtCount;
        internal int PlayerDeposit;
        internal int PlayerDepositDate;
        internal int PlayerDepositDay;
        internal float PlayerDepositPercent;
        internal int PlayerMedPolicy;
        internal int PlayerPirateLicense;
        internal int PlayerPiratePoints;
        internal int PlayerPirateNewPoints;
        internal uint PlayerFlyToStarId;
        internal int[] PlayerInvestments = new int[12];
        internal int PlayerInfectionPlacesOffset;
        internal int PlayerInfectionPlacesEndOffset;
        internal string[] PlayerInfectionPlaces = new string[24];
        internal byte PlayerImmunity;
        internal int PlayerProgramsOffset;
        internal int[] PlayerProgramsInWarBase = new int[12];
        internal int PlayerDayWarBaseGivePrograms;
        internal int PlayerHitEnemyAfterPrograms;
        internal int PlayerSatelliteCount;
        internal int PlayerSatelliteListOffset;
        internal int PlayerSatelliteEndOffset;
        internal List<ShipItemListEntry> PlayerSatelliteItems = new List<ShipItemListEntry>();
        internal int PlayerRobotMapCount;
        internal bool HasPlayerRobotMaps;
        internal int PlayerRobotMapListOffset;
        internal int PlayerRobotMapEndOffset;
        internal List<PlayerRobotMapRecord> PlayerRobotMaps = new List<PlayerRobotMapRecord>();
        internal int PlayerLateStatsOffset;
        internal int PlayerPlanetBattlesWin;
        internal int PlayerLastPlanetBattleDate;
        internal bool PlayerPlanetBattlesRejected;
        internal ushort PlayerIllnessCount;
        internal ushort PlayerStimulatorCount;
        internal ushort PlayerPrisonCount;
        internal int PlayerUnknownPlanetComplete;
        internal ushort PlayerChangeRaceCount;
        internal ushort PlayerChangeSideCount;
        internal byte PlayerHotEquipmentCurrent;
        internal int PlayerEquipmentSetsOffset;
        internal int PlayerEquipmentSetsEndOffset;
        internal byte PlayerEquipmentSetCount;
        internal uint[,] PlayerEquipmentSetItems = new uint[10, 12];
        internal uint[,] PlayerArtefactSetItems = new uint[10, 32];
        internal int PlayerPreAchievementFlagsOffset;
        internal byte PlayerGoToGovernment;
        internal bool PlayerNoJump;
        internal bool PlayerPirateClanReal;
        internal int PlayerExperienceOffset;
        internal int PlayerExperienceDominatorKills;
        internal int PlayerExperiencePirateKills;
        internal int PlayerExperienceGoodShipKills;
        internal int PlayerExperienceTrade;
        internal byte PlayerCaptainOnBridge;
        internal bool HasPlayerBridge;
        internal ShipHeaderRecord PlayerBridgeRuins;
        internal int PlayerBridgeRuinsEndOffset;
        internal int PlayerBridgeReferenceOffset;
        internal uint PlayerBridgeCurrentShipId;
        internal uint PlayerBridgeCurrentPlanetId;
        internal int PlayerBridgeBackgroundOffset;
        internal int PlayerBridgeBackgroundEndOffset;
        internal string PlayerBridgeBackground = string.Empty;
        internal bool HasPlayerJournal;
        internal int PlayerJournalListOffset;
        internal int PlayerJournalEndOffset;
        internal List<PlayerJournalRecord> PlayerJournalRecords = new List<PlayerJournalRecord>();
        internal bool HasPlayerNews;
        internal int PlayerNewsListOffset;
        internal int PlayerNewsEndOffset;
        internal List<GalaxyNewsRecord> PlayerNewsRecords = new List<GalaxyNewsRecord>();

        internal bool IsStation
        {
            get { return (0x3FC0 & (1 << Type)) != 0; }
        }

        internal ShipHeaderRecord Clone()
        {
            ShipHeaderRecord copy = (ShipHeaderRecord)MemberwiseClone();
            copy.Goods = (uint[,])Goods.Clone();
            copy.Skills = (byte[])Skills.Clone();
            copy.ProgramCounts = (int[])ProgramCounts.Clone();
            copy.RangerQuests = new List<RangerQuestRecord>();
            foreach (RangerQuestRecord record in RangerQuests) copy.RangerQuests.Add(record.Clone());
            copy.PlayerJournalRecords = new List<PlayerJournalRecord>();
            foreach (PlayerJournalRecord record in PlayerJournalRecords)
                copy.PlayerJournalRecords.Add(record.Clone());
            copy.PlayerRobotMaps = new List<PlayerRobotMapRecord>();
            foreach (PlayerRobotMapRecord record in PlayerRobotMaps)
                copy.PlayerRobotMaps.Add(record.Clone());
            copy.PlayerSatelliteItems = CloneShipItemEntries(PlayerSatelliteItems);
            copy.PlayerNewsRecords = new List<GalaxyNewsRecord>();
            foreach (GalaxyNewsRecord record in PlayerNewsRecords)
                copy.PlayerNewsRecords.Add(record.Clone());
            copy.Illnesses = new List<ShipIllnessRecord>();
            foreach (ShipIllnessRecord record in Illnesses) copy.Illnesses.Add(record.Clone());
            copy.Rewards = new List<byte>(Rewards);
            copy.TranclucatorSeekPermits = (bool[])TranclucatorSeekPermits.Clone();
            copy.TranclucatorLandPermits = (bool[])TranclucatorLandPermits.Clone();
            copy.RuinsShopGoods = (int[,])RuinsShopGoods.Clone();
            copy.RuinsEquipmentItems = CloneShipItemEntries(RuinsEquipmentItems);
            copy.RuinsSaleSatellite = RuinsSaleSatellite == null ? null : RuinsSaleSatellite.Clone();
            copy.KillDominatorsByType = (int[])KillDominatorsByType.Clone();
            copy.ChameleonLogic = (byte[])ChameleonLogic.Clone();
            copy.PlayerStorageItems = new List<PlayerStorageItemRecord>();
            foreach (PlayerStorageItemRecord record in PlayerStorageItems)
                copy.PlayerStorageItems.Add(record.Clone());
            copy.PlayerInvestments = (int[])PlayerInvestments.Clone();
            copy.PlayerInfectionPlaces = (string[])PlayerInfectionPlaces.Clone();
            copy.PlayerProgramsInWarBase = (int[])PlayerProgramsInWarBase.Clone();
            copy.PlayerEquipmentSetItems = (uint[,])PlayerEquipmentSetItems.Clone();
            copy.PlayerArtefactSetItems = (uint[,])PlayerArtefactSetItems.Clone();
            copy.PlayerBridgeRuins = PlayerBridgeRuins == null ? null : PlayerBridgeRuins.Clone();
            copy.EquipmentItems = CloneShipItemEntries(EquipmentItems);
            copy.ArtefactItems = CloneShipItemEntries(ArtefactItems);
            copy.DropListItems = CloneShipItemEntries(DropListItems);
            copy.SpecialBonuses = new List<ShipSpecialBonusRecord>();
            foreach (ShipSpecialBonusRecord record in SpecialBonuses) copy.SpecialBonuses.Add(record.Clone());
            copy.StatusEffects = new List<ShipStatusEffectRecord>();
            foreach (ShipStatusEffectRecord record in StatusEffects) copy.StatusEffects.Add(record.Clone());
            copy.CustomShipInfos = new List<CustomShipInfoRecord>();
            foreach (CustomShipInfoRecord record in CustomShipInfos) copy.CustomShipInfos.Add(record.Clone());
            copy.TakeItemReferenceIds = new List<uint>(TakeItemReferenceIds);
            copy.RecentlyDroppedItemIds = new List<uint>(RecentlyDroppedItemIds);
            copy.RelationToRangers = RelationToRangers == null ? null :
                (byte[])RelationToRangers.Clone();
            return copy;
        }

        private static List<ShipItemListEntry> CloneShipItemEntries(List<ShipItemListEntry> source)
        {
            List<ShipItemListEntry> result = new List<ShipItemListEntry>();
            foreach (ShipItemListEntry record in source) result.Add(record.Clone());
            return result;
        }

        private static bool RangerQuestsEqual(IList<RangerQuestRecord> left,
            IList<RangerQuestRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool PlayerJournalEqual(IList<PlayerJournalRecord> left,
            IList<PlayerJournalRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool PlayerRobotMapsEqual(IList<PlayerRobotMapRecord> left,
            IList<PlayerRobotMapRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool PlayerNewsEqual(IList<GalaxyNewsRecord> left,
            IList<GalaxyNewsRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool PlayerStorageItemsEqual(IList<PlayerStorageItemRecord> left,
            IList<PlayerStorageItemRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool IllnessesEqual(IList<ShipIllnessRecord> left,
            IList<ShipIllnessRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool ByteListsEqual(IList<byte> left, IList<byte> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (left[index] != right[index]) return false;
            return true;
        }

        internal bool ContentEquals(ShipHeaderRecord other)
        {
            return other != null && ObjectId == other.ObjectId && Type == other.Type &&
                Owner == other.Owner && IsPlayer == other.IsPlayer &&
                Name == other.Name && ScriptName == other.ScriptName &&
                X == other.X && Y == other.Y && HomePlanetId == other.HomePlanetId &&
                CurrentStarId == other.CurrentStarId && CurrentPlanetId == other.CurrentPlanetId &&
                CurrentShipId == other.CurrentShipId && Money == other.Money && Rnd == other.Rnd &&
                RndOut == other.RndOut && Day == other.Day && Face == other.Face &&
                PilotRace == other.PilotRace && EquipmentItemCount == other.EquipmentItemCount &&
                PreCommonContentEquals(other) &&
                GoodsEqual(Goods, other.Goods) && HasCommonTail == other.HasCommonTail &&
                (!HasCommonTail || (Forsage == other.Forsage && Angle == other.Angle &&
                OrderType == other.OrderType && OrderData == other.OrderData &&
                OrderObjectId == other.OrderObjectId && OrderDestinationX == other.OrderDestinationX &&
                OrderDestinationY == other.OrderDestinationY && OrderAbsolute == other.OrderAbsolute &&
                Abducted == other.Abducted && DaysLanded == other.DaysLanded &&
                ScriptOrderAbsolute == other.ScriptOrderAbsolute && GraphDominator == other.GraphDominator &&
                GraphName == other.GraphName && GraphShipTransparency == other.GraphShipTransparency &&
                InHyperSpace == other.InHyperSpace && RadiusStop == other.RadiusStop &&
                ShipDestroy == other.ShipDestroy && EqualByteArrays(Skills, other.Skills) &&
                Protoplasm == other.Protoplasm && Points == other.Points && FreePoints == other.FreePoints &&
                DayWithoutPlayer == other.DayWithoutPlayer && GroupOrder == other.GroupOrder &&
                LastNextDay == other.LastNextDay && ChameleonEnabled == other.ChameleonEnabled &&
                ChameleonSeries == other.ChameleonSeries &&
                BlazerChameleonDetect == other.BlazerChameleonDetect &&
                KellerChameleonDetect == other.KellerChameleonDetect &&
                TerronChameleonDetect == other.TerronChameleonDetect &&
                BlazerChameleonCharge == other.BlazerChameleonCharge &&
                KellerChameleonCharge == other.KellerChameleonCharge &&
                TerronChameleonCharge == other.TerronChameleonCharge &&
                TechLevelKnowledge == other.TechLevelKnowledge && TradePenalty == other.TradePenalty &&
                TradePoints == other.TradePoints && ContrabandPoints == other.ContrabandPoints &&
                RewardViewCount == other.RewardViewCount && NoDrop == other.NoDrop &&
                NoTarget == other.NoTarget && NoTalk == other.NoTalk && NoScan == other.NoScan &&
                ScriptChameleon == other.ScriptChameleon && RobbedByPlayer == other.RobbedByPlayer &&
                CountOfDeflectedPlayerShots == other.CountOfDeflectedPlayerShots &&
                Swarmed == other.Swarmed && SwarmedByShipId == other.SwarmedByShipId &&
                SwarmAnimation == other.SwarmAnimation && CurrentStanding == other.CurrentStanding &&
                AverageSpeed == other.AverageSpeed && AverageEnemySpeed == other.AverageEnemySpeed &&
                AverageEquipmentValue == other.AverageEquipmentValue && AverageCapital == other.AverageCapital &&
                AverageMoneyToCapital == other.AverageMoneyToCapital &&
                AverageFreeSpaceRatio == other.AverageFreeSpaceRatio &&
                RatioOfTooCostlyEquipmentInShop == other.RatioOfTooCostlyEquipmentInShop &&
                IllnessesEqual(Illnesses, other.Illnesses) &&
                EqualByteArrays(RelationToRangers, other.RelationToRangers) &&
                RelationCount == other.RelationCount && ByteListsEqual(Rewards, other.Rewards) &&
                HasNormalShipTail == other.HasNormalShipTail &&
                (!HasNormalShipTail || (KillAllShips == other.KillAllShips &&
                KillPirates == other.KillPirates && KillDominators == other.KillDominators &&
                LiberationSystems == other.LiberationSystems && KillPacifics == other.KillPacifics &&
                KillWarriors == other.KillWarriors && KillRangers == other.KillRangers &&
                KillInCurrentSystemDominators == other.KillInCurrentSystemDominators &&
                KillInCurrentSystemPirates == other.KillInCurrentSystemPirates &&
                KillInCurrentSystemNormals == other.KillInCurrentSystemNormals &&
                KillCustomInCurrentSystem == other.KillCustomInCurrentSystem &&
                LiberationPlanetId == other.LiberationPlanetId && LiberationKills == other.LiberationKills &&
                CoalitionRank == other.CoalitionRank && CoalitionRankPoints == other.CoalitionRankPoints &&
                PirateRank == other.PirateRank && PirateRankPoints == other.PirateRankPoints &&
                LastPlanetId == other.LastPlanetId && TurnPlayerMoneyGoods == other.TurnPlayerMoneyGoods)) &&
                HasSimpleDerivedTail == other.HasSimpleDerivedTail &&
                (!HasSimpleDerivedTail || (DominatorType == other.DominatorType &&
                DominatorSeries == other.DominatorSeries && RunProgramDate == other.RunProgramDate &&
                RunProgramName == other.RunProgramName && TransportType == other.TransportType &&
                WarriorType == other.WarriorType && PiratePrison == other.PiratePrison &&
                PirateType == other.PirateType && DesireConflict == other.DesireConflict)) &&
                HasRangerTail == other.HasRangerTail &&
                (!HasRangerTail || (RangerQuestCount == other.RangerQuestCount &&
                RangerQuestsEqual(RangerQuests, other.RangerQuests) &&
                RangerStatusTrader == other.RangerStatusTrader && RangerStatusPirate == other.RangerStatusPirate &&
                RangerStatusWarrior == other.RangerStatusWarrior && EminentPointsTrader == other.EminentPointsTrader &&
                EminentPointsPirate == other.EminentPointsPirate && EminentPointsWarrior == other.EminentPointsWarrior &&
                RangerMoral == other.RangerMoral && Courageous == other.Courageous &&
                StatusChangeWarrior == other.StatusChangeWarrior && StatusChangePirate == other.StatusChangePirate &&
                StatusChangeTrader == other.StatusChangeTrader && RangerPrison == other.RangerPrison &&
                LastShipId == other.LastShipId && Nods == other.Nods &&
                EqualIntArrays(ProgramCounts, other.ProgramCounts) && ExcludedFromRating == other.ExcludedFromRating)) &&
                HasTranclucatorTail == other.HasTranclucatorTail &&
                (!HasTranclucatorTail || (TranclucatorProprietorShipId == other.TranclucatorProprietorShipId &&
                TranclucatorDocking == other.TranclucatorDocking &&
                TranclucatorSeekItems == other.TranclucatorSeekItems &&
                TranclucatorAutoArrange == other.TranclucatorAutoArrange &&
                TranclucatorArtSize == other.TranclucatorArtSize &&
                TranclucatorArtSystemName == other.TranclucatorArtSystemName &&
                EqualBoolArrays(TranclucatorSeekPermits, other.TranclucatorSeekPermits) &&
                EqualBoolArrays(TranclucatorLandPermits, other.TranclucatorLandPermits) &&
                TranclucatorLandStorage == other.TranclucatorLandStorage)) &&
                HasRuinsTail == other.HasRuinsTail &&
                (!HasRuinsTail || (RuinsEquipmentItemCount == other.RuinsEquipmentItemCount &&
                ShipItemEntriesEqual(RuinsEquipmentItems, other.RuinsEquipmentItems) &&
                ((RuinsSaleSatellite == null && other.RuinsSaleSatellite == null) ||
                    (RuinsSaleSatellite != null && RuinsSaleSatellite.ContentEquals(other.RuinsSaleSatellite))) &&
                IntMatrixEqual(RuinsShopGoods, other.RuinsShopGoods, 8, 3) &&
                RuinsEnergy == other.RuinsEnergy && RuinsFlyToStarId == other.RuinsFlyToStarId &&
                RuinsFlyDate == other.RuinsFlyDate && RuinsSponsor == other.RuinsSponsor &&
                RuinsSpecialShip == other.RuinsSpecialShip && RuinsNoLanding == other.RuinsNoLanding &&
                RuinsNoShopUpdate == other.RuinsNoShopUpdate)) &&
                HasPlayerPrefix == other.HasPlayerPrefix &&
                (!HasPlayerPrefix || (PlayerPrison == other.PlayerPrison &&
                PlayerTalkLocked == other.PlayerTalkLocked && PlayerScanLocked == other.PlayerScanLocked &&
                KillShipInHyperSpace == other.KillShipInHyperSpace && KillShipInHole == other.KillShipInHole &&
                EqualIntArrays(KillDominatorsByType, other.KillDominatorsByType) &&
                EqualByteArrays(ChameleonLogic, other.ChameleonLogic) &&
                HasPlayerStorageItems == other.HasPlayerStorageItems &&
                PlayerObjectStateCount == other.PlayerObjectStateCount &&
                PlayerStorageItemsEqual(PlayerStorageItems, other.PlayerStorageItems))) &&
                HasPlayerFinancialTail == other.HasPlayerFinancialTail &&
                (!HasPlayerFinancialTail || (PlayerDebt == other.PlayerDebt &&
                PlayerDebtDate == other.PlayerDebtDate && PlayerDebtCount == other.PlayerDebtCount &&
                PlayerDeposit == other.PlayerDeposit && PlayerDepositDate == other.PlayerDepositDate &&
                PlayerDepositDay == other.PlayerDepositDay &&
                PlayerDepositPercent == other.PlayerDepositPercent && PlayerMedPolicy == other.PlayerMedPolicy &&
                PlayerPirateLicense == other.PlayerPirateLicense && PlayerPiratePoints == other.PlayerPiratePoints &&
                PlayerPirateNewPoints == other.PlayerPirateNewPoints &&
                PlayerFlyToStarId == other.PlayerFlyToStarId &&
                EqualIntArrays(PlayerInvestments, other.PlayerInvestments) &&
                EqualStringArrays(PlayerInfectionPlaces, other.PlayerInfectionPlaces) &&
                PlayerImmunity == other.PlayerImmunity &&
                EqualIntArrays(PlayerProgramsInWarBase, other.PlayerProgramsInWarBase) &&
                PlayerDayWarBaseGivePrograms == other.PlayerDayWarBaseGivePrograms &&
                PlayerHitEnemyAfterPrograms == other.PlayerHitEnemyAfterPrograms &&
                PlayerSatelliteCount == other.PlayerSatelliteCount &&
                PlayerRobotMapCount == other.PlayerRobotMapCount &&
                PlayerPlanetBattlesWin == other.PlayerPlanetBattlesWin &&
                PlayerLastPlanetBattleDate == other.PlayerLastPlanetBattleDate &&
                PlayerPlanetBattlesRejected == other.PlayerPlanetBattlesRejected &&
                PlayerIllnessCount == other.PlayerIllnessCount &&
                PlayerStimulatorCount == other.PlayerStimulatorCount &&
                PlayerPrisonCount == other.PlayerPrisonCount &&
                PlayerUnknownPlanetComplete == other.PlayerUnknownPlanetComplete &&
                PlayerChangeRaceCount == other.PlayerChangeRaceCount &&
                PlayerChangeSideCount == other.PlayerChangeSideCount &&
                PlayerHotEquipmentCurrent == other.PlayerHotEquipmentCurrent &&
                PlayerEquipmentSetCount == other.PlayerEquipmentSetCount &&
                UIntMatrixEqual(PlayerEquipmentSetItems, other.PlayerEquipmentSetItems, 10, 12) &&
                UIntMatrixEqual(PlayerArtefactSetItems, other.PlayerArtefactSetItems, 10, 32) &&
                PlayerGoToGovernment == other.PlayerGoToGovernment && PlayerNoJump == other.PlayerNoJump &&
                PlayerPirateClanReal == other.PlayerPirateClanReal &&
                PlayerExperienceDominatorKills == other.PlayerExperienceDominatorKills &&
                PlayerExperiencePirateKills == other.PlayerExperiencePirateKills &&
                PlayerExperienceGoodShipKills == other.PlayerExperienceGoodShipKills &&
                PlayerExperienceTrade == other.PlayerExperienceTrade &&
                PlayerCaptainOnBridge == other.PlayerCaptainOnBridge)) &&
                HasPlayerBridge == other.HasPlayerBridge &&
                (!HasPlayerBridge || (PlayerBridgeRuins != null && other.PlayerBridgeRuins != null &&
                PlayerBridgeRuins.ContentEquals(other.PlayerBridgeRuins) &&
                PlayerBridgeCurrentShipId == other.PlayerBridgeCurrentShipId &&
                PlayerBridgeCurrentPlanetId == other.PlayerBridgeCurrentPlanetId &&
                PlayerBridgeBackground == other.PlayerBridgeBackground)) &&
                HasPlayerRobotMaps == other.HasPlayerRobotMaps &&
                (!HasPlayerRobotMaps || PlayerRobotMapsEqual(PlayerRobotMaps,
                    other.PlayerRobotMaps)) &&
                HasPlayerJournal == other.HasPlayerJournal &&
                (!HasPlayerJournal || PlayerJournalEqual(PlayerJournalRecords,
                    other.PlayerJournalRecords)) &&
                HasPlayerNews == other.HasPlayerNews &&
                (!HasPlayerNews || PlayerNewsEqual(PlayerNewsRecords,
                    other.PlayerNewsRecords))));
        }

        private bool PreCommonContentEquals(ShipHeaderRecord other)
        {
            if (HasPreCommonCollections != other.HasPreCommonCollections) return false;
            if (!HasPreCommonCollections) return true;
            if (!ShipItemEntriesEqual(EquipmentItems, other.EquipmentItems) ||
                !ShipItemEntriesEqual(ArtefactItems, other.ArtefactItems) ||
                !ShipItemEntriesEqual(DropListItems, other.DropListItems) ||
                SpecialBonuses == null || other.SpecialBonuses == null ||
                SpecialBonuses.Count != other.SpecialBonuses.Count ||
                StatusEffects == null || other.StatusEffects == null ||
                StatusEffects.Count != other.StatusEffects.Count ||
                CustomShipInfos == null || other.CustomShipInfos == null ||
                CustomShipInfos.Count != other.CustomShipInfos.Count ||
                !UIntListsEqual(TakeItemReferenceIds, other.TakeItemReferenceIds) ||
                !UIntListsEqual(RecentlyDroppedItemIds, other.RecentlyDroppedItemIds) ||
                GoodShipId != other.GoodShipId || BadShipId != other.BadShipId ||
                PartnerShipId != other.PartnerShipId || PartnerGood != other.PartnerGood) return false;
            for (int index = 0; index < SpecialBonuses.Count; index++)
                if (SpecialBonuses[index].BonusType != other.SpecialBonuses[index].BonusType ||
                    SpecialBonuses[index].Value != other.SpecialBonuses[index].Value) return false;
            for (int index = 0; index < StatusEffects.Count; index++)
                if (StatusEffects[index].EffectType != other.StatusEffects[index].EffectType ||
                    StatusEffects[index].Value != other.StatusEffects[index].Value ||
                    StatusEffects[index].LastSourceShipId != other.StatusEffects[index].LastSourceShipId) return false;
            for (int index = 0; index < CustomShipInfos.Count; index++)
            {
                CustomShipInfoRecord left = CustomShipInfos[index], right = other.CustomShipInfos[index];
                if (left.Name != right.Name || left.Description != right.Description ||
                    left.Data1 != right.Data1 || left.Data2 != right.Data2 || left.Data3 != right.Data3 ||
                    left.TextData1 != right.TextData1 || left.TextData2 != right.TextData2 ||
                    left.TextData3 != right.TextData3) return false;
            }
            return true;
        }

        private static bool ShipItemEntriesEqual(List<ShipItemListEntry> left, List<ShipItemListEntry> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        private static bool UIntListsEqual(List<uint> left, List<uint> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++) if (left[index] != right[index]) return false;
            return true;
        }

        private static bool EqualByteArrays(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++) if (left[index] != right[index]) return false;
            return true;
        }

        private static bool EqualIntArrays(int[] left, int[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++) if (left[index] != right[index]) return false;
            return true;
        }

        private static bool EqualBoolArrays(bool[] left, bool[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++) if (left[index] != right[index]) return false;
            return true;
        }

        private static bool EqualStringArrays(string[] left, string[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index]) return false;
            return true;
        }

        private static bool GoodsEqual(uint[,] left, uint[,] right)
        {
            if (left == null || right == null || left.GetLength(0) != 8 || right.GetLength(0) != 8 ||
                left.GetLength(1) != 4 || right.GetLength(1) != 4) return false;
            for (int good = 0; good < 8; good++)
                for (int field = 0; field < 4; field++)
                    if (left[good, field] != right[good, field]) return false;
            return true;
        }

        private static bool IntMatrixEqual(int[,] left, int[,] right, int rows, int columns)
        {
            if (left == null || right == null || left.GetLength(0) != rows || right.GetLength(0) != rows ||
                left.GetLength(1) != columns || right.GetLength(1) != columns) return false;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    if (left[row, column] != right[row, column]) return false;
            return true;
        }

        private static bool UIntMatrixEqual(uint[,] left, uint[,] right, int rows, int columns)
        {
            if (left == null || right == null || left.GetLength(0) != rows || right.GetLength(0) != rows ||
                left.GetLength(1) != columns || right.GetLength(1) != columns) return false;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    if (left[row, column] != right[row, column]) return false;
            return true;
        }

        public override string ToString()
        {
            return (IsPlayer ? "Игрок" : IsStation ? "Станция" : "Корабль") + " " + ObjectId + "  " + Name;
        }
    }

    internal static class ShipOrderRules
    {
        internal static int DeterministicRandom(int left, int right, uint seed)
        {
            int minimum = Math.Min(left, right), maximum = Math.Max(left, right);
            uint width = checked((uint)(maximum - minimum + 1));
            return unchecked((int)(seed % width) + minimum);
        }

        internal static uint JumpData(StarHeaderRecord from, StarHeaderRecord to)
        {
            if (from == null || to == null) throw new ArgumentNullException();
            double dx = (double)from.X - to.X, dy = (double)from.Y - to.Y;
            // The game uses half-up rounding for this non-negative distance,
            // rather than the banker's rounding of Math.Round.
            int distance = (int)Math.Floor(Math.Sqrt(dx * dx + dy * dy) * 0.1 + 0.5);
            return (uint)Math.Max(2, distance + 1);
        }

        internal static void JumpDestination(ShipHeaderRecord source, StarHeaderRecord from,
            StarHeaderRecord to, PlanetHeaderRecord lastPlanet, out float x, out float y)
        {
            if (source == null || from == null || to == null) throw new ArgumentNullException();
            double angle = Math.Atan2(to.X - from.X, from.Y - to.Y);
            uint seed = unchecked((unchecked((uint)from.Raw08) + source.Rnd) * unchecked((uint)to.Raw08));
            angle += ((int)(seed % 9U) - 4) * Math.PI / 180.0;
            // A jump starts outside the gravity well of the system being left.
            // Therefore the fallback radius belongs to `from`, not `to`.
            double radius = lastPlanet == null ? from.GraphRadius + 800.0 :
                lastPlanet.PolarRadius + lastPlanet.Radius + 800.0;
            radius = Math.Round(radius);
            // The game converts the coordinates to integers by truncating toward
            // zero. Rounding shifts most exits by one map unit.
            x = TruncateJumpCoordinate(Math.Sin(angle) * radius);
            y = TruncateJumpCoordinate(-Math.Cos(angle) * radius);
        }

        private static float TruncateJumpCoordinate(double value)
        {
            // atan2 plus an integer-degree correction can leave a cardinal
            // direction infinitesimally below an exact integer (3539.999999…).
            // The game's Single-precision path lands on the integer first.
            double nearest = Math.Round(value);
            if (Math.Abs(value - nearest) < 0.001) return (float)nearest;
            return (float)Math.Truncate(value);
        }

        internal static void HoleDestination(HoleRecord hole, uint currentStarId,
            out uint data, out float x, out float y)
        {
            if (hole == null) throw new ArgumentNullException("hole");
            bool from = hole.FromStarId == currentStarId;
            data = from ? 2U : 0x10002U;
            x = from ? hole.FromX : hole.ToX;
            y = from ? hole.FromY : hole.ToY;
        }

        internal static float PlanetTakeoffAngle(uint turn, float baseX, float baseY,
            float x, float y, int jitter)
        {
            double heading = Math.Atan2(y - baseY, x - baseX) * 180.0 / Math.PI;
            if ((turn & 1U) != 0U) heading += 180.0;
            return NormalizeAngle((float)(heading + jitter));
        }

        internal static float CarrierTakeoffAngle(float baseX, float baseY,
            float x, float y, int jitter)
        {
            double heading = Math.Atan2(-(x - baseX), y - baseY) * 180.0 / Math.PI;
            return NormalizeAngle((float)(heading + jitter));
        }

        internal static float AngleDifference(float left, float right)
        {
            float difference = Math.Abs(NormalizeAngle(left - right));
            return difference > 180.0F ? 360.0F - difference : difference;
        }

        private static float NormalizeAngle(float value)
        {
            while (value >= 360.0F) value -= 360.0F;
            while (value < 0.0F) value += 360.0F;
            return value;
        }
    }

    internal sealed class PlanetSputnikRecord
    {
        internal int Start;
        internal int End;
        internal uint ObjectId;
        internal string GraphName;
        internal byte[] OpaqueData;
        internal float AngleCurrent;

        internal PlanetSputnikRecord Clone()
        {
            PlanetSputnikRecord copy = (PlanetSputnikRecord)MemberwiseClone();
            copy.OpaqueData = OpaqueData == null ? null : (byte[])OpaqueData.Clone();
            return copy;
        }

        internal bool ContentEquals(PlanetSputnikRecord other)
        {
            return other != null && ObjectId == other.ObjectId && GraphName == other.GraphName &&
                AngleCurrent == other.AngleCurrent && EqualBytes(OpaqueData, other.OpaqueData);
        }

        private static bool EqualBytes(byte[] left, byte[] right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index]) return false;
            return true;
        }

        public override string ToString()
        {
            return GraphName ?? string.Empty;
        }
    }

    internal sealed class PlanetGoneItemRecord
    {
        internal int Start;
        internal int End;
        internal byte PosX;
        internal byte PosY;
        internal byte LandType;
        internal bool Miss;
        internal int Region;
        internal int FactoryDiscriminatorOffset;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal PlanetGoneItemRecord Clone()
        {
            return (PlanetGoneItemRecord)MemberwiseClone();
        }

        internal bool ContentEquals(PlanetGoneItemRecord other)
        {
            return other != null && PosX == other.PosX && PosY == other.PosY &&
                LandType == other.LandType && Miss == other.Miss && Region == other.Region &&
                ItemType == other.ItemType && ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return "TItem " + ItemObjectId + "  type " + ItemType;
        }
    }

    internal sealed class PlanetWarriorRecord
    {
        internal int Start;
        internal int End;
        internal byte ShipType;
        internal int ShipStart;
        internal uint ShipObjectId;

        internal PlanetWarriorRecord Clone()
        {
            return (PlanetWarriorRecord)MemberwiseClone();
        }

        internal bool ContentEquals(PlanetWarriorRecord other)
        {
            return other != null && ShipType == other.ShipType &&
                ShipObjectId == other.ShipObjectId;
        }

        public override string ToString()
        {
            return "TShip " + ShipObjectId + "  type " + ShipType;
        }
    }

    internal sealed class PlanetHeaderRecord
    {
        internal int Start;
        internal int ScalarOffset;
        internal int FixedPrefixEnd;
        internal uint ObjectId;
        internal int Raw08;
        internal uint Raw0C;
        internal string Name;
        internal float PolarAngle;
        internal float PolarRadius;
        internal float Angle;
        internal int Mass;
        internal int Radius;
        internal int WaterSpace;
        internal int WaterSpaceDone;
        internal int LandSpace;
        internal int LandSpaceDone;
        internal int HillSpace;
        internal int HillSpaceDone;
        internal byte OrbitCount;
        internal bool VisitedByPlayer;
        internal byte[] OpenInventions;
        internal byte CurrentInvention;
        internal float OpenPointsInvention;
        internal byte NecessaryPercent;
        internal byte NecessaryPercentK;
        internal uint PeopleCount;
        internal byte Economy;
        internal uint Money;
        internal byte Owner;
        internal byte Race;
        internal byte Government;
        // Eight commodity rows stored directly in the fixed TPlanet prefix.
        // Columns: quantity, sale price and purchase price. The two byte arrays
        // are the deficit and sale-event flags used by the game.
        internal uint[,] ShopGoods = new uint[8, 3];
        internal byte[] ShopDeficit = new byte[8];
        internal byte[] ShopSale = new byte[8];
        internal int RelationCountOffset;
        internal int RelationEndOffset;
        internal ushort RelationCount;
        internal byte[] RelationToRangers;
        internal int EquipmentShopCountOffset;
        internal int EquipmentShopEndOffset;
        internal ushort EquipmentShopCount;
        internal List<ShipItemListEntry> EquipmentShopItems = new List<ShipItemListEntry>();
        internal int WarriorCountOffset;
        internal ushort WarriorCount;
        internal bool HasWarriorList;
        internal int WarriorEndOffset;
        internal List<PlanetWarriorRecord> Warriors = new List<PlanetWarriorRecord>();
        internal ushort FirstListCount;
        internal int End;
        internal int LateFieldsOffset;
        internal int GraphNameEnd;
        internal int SatelliteCountOffset;
        internal int SatelliteEndOffset;
        internal int FlagsOffset;
        internal int CustomFactionOffset;
        internal ushort RangerCount;
        internal ushort TransportCount;
        internal ushort GraphRadius;
        internal string GraphName;
        internal ushort GraphSpeedRotate;
        internal int GraphStepRotate;
        internal byte GraphRing;
        internal int QuestNumber;
        internal ushort SatelliteCount;
        internal List<PlanetSputnikRecord> Satellites = new List<PlanetSputnikRecord>();
        internal int GoneItemCountOffset;
        internal int GoneItemEndOffset;
        internal ushort GoneItemCount;
        internal List<PlanetGoneItemRecord> GoneItems = new List<PlanetGoneItemRecord>();
        internal bool NoLanding;
        internal byte NoPlanetShopUpdate;
        internal bool NoBuyShips;
        internal bool NoRandomEvents;
        internal bool IsRogeria;
        internal string CustomFaction;

        internal bool HasLateFields { get { return LateFieldsOffset > 0; } }
        internal bool HasFlags { get { return FlagsOffset > 0; } }

        internal PlanetHeaderRecord Clone()
        {
            PlanetHeaderRecord copy = (PlanetHeaderRecord)MemberwiseClone();
            copy.OpenInventions = OpenInventions == null ? null : (byte[])OpenInventions.Clone();
            copy.ShopGoods = ShopGoods == null ? null : (uint[,])ShopGoods.Clone();
            copy.ShopDeficit = ShopDeficit == null ? null : (byte[])ShopDeficit.Clone();
            copy.ShopSale = ShopSale == null ? null : (byte[])ShopSale.Clone();
            copy.RelationToRangers = RelationToRangers == null ? null :
                (byte[])RelationToRangers.Clone();
            copy.EquipmentShopItems = new List<ShipItemListEntry>();
            if (EquipmentShopItems != null)
                foreach (ShipItemListEntry item in EquipmentShopItems)
                    copy.EquipmentShopItems.Add(item.Clone());
            copy.Warriors = new List<PlanetWarriorRecord>();
            if (Warriors != null)
                foreach (PlanetWarriorRecord warrior in Warriors)
                    copy.Warriors.Add(warrior.Clone());
            copy.Satellites = new List<PlanetSputnikRecord>();
            if (Satellites != null)
                foreach (PlanetSputnikRecord satellite in Satellites)
                    copy.Satellites.Add(satellite.Clone());
            copy.GoneItems = new List<PlanetGoneItemRecord>();
            if (GoneItems != null)
                foreach (PlanetGoneItemRecord goneItem in GoneItems)
                    copy.GoneItems.Add(goneItem.Clone());
            return copy;
        }

        internal bool ContentEquals(PlanetHeaderRecord other)
        {
            return other != null && ObjectId == other.ObjectId && Raw08 == other.Raw08 &&
                Raw0C == other.Raw0C && Name == other.Name && PolarAngle == other.PolarAngle &&
                PolarRadius == other.PolarRadius && Angle == other.Angle && Mass == other.Mass &&
                Radius == other.Radius && WaterSpace == other.WaterSpace &&
                WaterSpaceDone == other.WaterSpaceDone && LandSpace == other.LandSpace &&
                LandSpaceDone == other.LandSpaceDone && HillSpace == other.HillSpace &&
                HillSpaceDone == other.HillSpaceDone && OrbitCount == other.OrbitCount &&
                VisitedByPlayer == other.VisitedByPlayer && EqualBytes(OpenInventions, other.OpenInventions) &&
                CurrentInvention == other.CurrentInvention && OpenPointsInvention == other.OpenPointsInvention &&
                NecessaryPercent == other.NecessaryPercent && NecessaryPercentK == other.NecessaryPercentK &&
                PeopleCount == other.PeopleCount && Economy == other.Economy && Money == other.Money &&
                Owner == other.Owner && Race == other.Race && Government == other.Government &&
                EqualUIntMatrix(ShopGoods, other.ShopGoods, 8, 3) &&
                EqualBytes(ShopDeficit, other.ShopDeficit) && EqualBytes(ShopSale, other.ShopSale) &&
                RelationCount == other.RelationCount &&
                EqualBytes(RelationToRangers, other.RelationToRangers) &&
                EquipmentShopCount == other.EquipmentShopCount &&
                ShopItemsEqual(other) &&
                HasWarriorList == other.HasWarriorList && WarriorCount == other.WarriorCount &&
                WarriorsEqual(other) &&
                RangerCount == other.RangerCount && TransportCount == other.TransportCount &&
                GraphRadius == other.GraphRadius && GraphName == other.GraphName &&
                GraphSpeedRotate == other.GraphSpeedRotate && GraphStepRotate == other.GraphStepRotate &&
                GraphRing == other.GraphRing && QuestNumber == other.QuestNumber &&
                SatelliteCount == other.SatelliteCount && SatellitesEqual(other) &&
                GoneItemCount == other.GoneItemCount && GoneItemsEqual(other) &&
                NoLanding == other.NoLanding &&
                NoPlanetShopUpdate == other.NoPlanetShopUpdate && NoBuyShips == other.NoBuyShips &&
                NoRandomEvents == other.NoRandomEvents && IsRogeria == other.IsRogeria &&
                CustomFaction == other.CustomFaction &&
                FirstListCount == other.FirstListCount;
        }

        private bool SatellitesEqual(PlanetHeaderRecord other)
        {
            if (Satellites == null || other.Satellites == null || Satellites.Count != other.Satellites.Count)
                return false;
            for (int index = 0; index < Satellites.Count; index++)
                if (!Satellites[index].ContentEquals(other.Satellites[index])) return false;
            return true;
        }

        private bool ShopItemsEqual(PlanetHeaderRecord other)
        {
            if (EquipmentShopItems == null || other.EquipmentShopItems == null ||
                EquipmentShopItems.Count != other.EquipmentShopItems.Count) return false;
            for (int index = 0; index < EquipmentShopItems.Count; index++)
                if (!EquipmentShopItems[index].ContentEquals(other.EquipmentShopItems[index])) return false;
            return true;
        }

        private bool WarriorsEqual(PlanetHeaderRecord other)
        {
            if (Warriors == null || other.Warriors == null || Warriors.Count != other.Warriors.Count)
                return false;
            for (int index = 0; index < Warriors.Count; index++)
                if (!Warriors[index].ContentEquals(other.Warriors[index])) return false;
            return true;
        }

        private bool GoneItemsEqual(PlanetHeaderRecord other)
        {
            if (GoneItems == null || other.GoneItems == null || GoneItems.Count != other.GoneItems.Count)
                return false;
            for (int index = 0; index < GoneItems.Count; index++)
                if (!GoneItems[index].ContentEquals(other.GoneItems[index])) return false;
            return true;
        }

        private static bool EqualBytes(byte[] left, byte[] right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index]) return false;
            return true;
        }

        private static bool EqualUIntMatrix(uint[,] left, uint[,] right, int rows, int columns)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.GetLength(0) != rows ||
                right.GetLength(0) != rows || left.GetLength(1) != columns ||
                right.GetLength(1) != columns) return false;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    if (left[row, column] != right[row, column]) return false;
            return true;
        }

        public override string ToString()
        {
            return "Планета " + ObjectId + "  " + Name;
        }
    }

    internal sealed class ItemHeaderRecord
    {
        internal int Start;
        internal int BaseEnd;
        internal int SharedPrefixEnd;
        internal int EquipmentFirstStringEnd;
        internal int EquipmentSecondStringEnd;
        internal int EquipmentScalarOffset;
        internal uint ObjectId;
        internal byte Type;
        internal float X;
        internal float Y;
        internal int Weight;
        internal byte Owner;
        internal uint Cost;
        internal int ItemDestroy;
        internal string Name;
        internal byte NoDrop;
        internal string CustomFaction;
        internal string SystemName;
        internal byte Exploitable;
        internal float Strength;
        internal byte Broken;
        internal byte Slot;
        internal byte DominatorSeries;
        internal int BonusOffset;
        internal int BonusEnd;
        internal int Bonus;
        internal uint BonusReferenceId;
        internal int SpecialOffset;
        internal int SpecialEnd;
        internal int Special;
        internal uint SpecialReferenceId;
        internal int ExtraSpecialCountOffset;
        internal int ExtraSpecialEnd;
        internal List<ItemExtraSpecialRecord> ExtraSpecials = new List<ItemExtraSpecialRecord>();
        internal string CustomWeaponName;
        internal int CustomWeaponDiscriminatorOffset;
        internal bool HasGoodsTail;
        internal int GoodsTailOffset;
        internal int GoodsItemCount;
        internal bool GoodsItemNatural;
        internal bool HasDerivedTail;
        internal int DerivedTailOffset;
        internal int DerivedTailEnd;
        internal List<ItemDerivedField> DerivedFields;
        internal ShipHeaderRecord NestedTranclucator;

        internal ItemHeaderRecord Clone()
        {
            ItemHeaderRecord copy = (ItemHeaderRecord)MemberwiseClone();
            copy.DerivedFields = null;
            if (DerivedFields != null)
            {
                copy.DerivedFields = new List<ItemDerivedField>();
                foreach (ItemDerivedField field in DerivedFields) copy.DerivedFields.Add(field.Clone());
            }
            copy.ExtraSpecials = new List<ItemExtraSpecialRecord>();
            foreach (ItemExtraSpecialRecord record in ExtraSpecials)
                copy.ExtraSpecials.Add(record.Clone());
            copy.NestedTranclucator = NestedTranclucator == null ? null : NestedTranclucator.Clone();
            return copy;
        }

        internal bool ContentEquals(ItemHeaderRecord other)
        {
            return other != null && ObjectId == other.ObjectId && Type == other.Type &&
                X == other.X && Y == other.Y && Weight == other.Weight && Owner == other.Owner &&
                Cost == other.Cost && ItemDestroy == other.ItemDestroy && Name == other.Name &&
                NoDrop == other.NoDrop && CustomFaction == other.CustomFaction &&
                SystemName == other.SystemName && Exploitable == other.Exploitable &&
                Strength == other.Strength && Broken == other.Broken && Slot == other.Slot &&
                DominatorSeries == other.DominatorSeries && Bonus == other.Bonus &&
                BonusReferenceId == other.BonusReferenceId && Special == other.Special &&
                SpecialReferenceId == other.SpecialReferenceId &&
                EqualExtraSpecials(ExtraSpecials, other.ExtraSpecials) &&
                CustomWeaponName == other.CustomWeaponName &&
                HasGoodsTail == other.HasGoodsTail &&
                (!HasGoodsTail || (GoodsItemCount == other.GoodsItemCount &&
                GoodsItemNatural == other.GoodsItemNatural)) && HasDerivedTail == other.HasDerivedTail &&
                EqualDerivedFields(DerivedFields, other.DerivedFields) &&
                ((NestedTranclucator == null && other.NestedTranclucator == null) ||
                (NestedTranclucator != null && NestedTranclucator.ContentEquals(other.NestedTranclucator)));
        }

        private static bool EqualExtraSpecials(List<ItemExtraSpecialRecord> left,
            List<ItemExtraSpecialRecord> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        private static bool EqualDerivedFields(List<ItemDerivedField> left, List<ItemDerivedField> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        public override string ToString()
        {
            string label = string.IsNullOrEmpty(Name) ? "без имени" : Name;
            return "TItem " + ObjectId + "  type " + Type + "  " + label;
        }
    }

    internal sealed class ItemExtraSpecialRecord
    {
        internal int Special;
        internal uint ReferenceId;
        internal int Count;

        internal ItemExtraSpecialRecord Clone()
        {
            return (ItemExtraSpecialRecord)MemberwiseClone();
        }

        internal bool ContentEquals(ItemExtraSpecialRecord other)
        {
            return other != null && Special == other.Special &&
                ReferenceId == other.ReferenceId && Count == other.Count;
        }
    }

    internal sealed class ItemDerivedField
    {
        internal const byte Byte = 1;
        internal const byte UInt16 = 2;
        internal const byte Int32 = 3;
        internal const byte UInt32 = 4;
        internal const byte Float32 = 5;
        internal const byte Boolean = 6;
        internal const byte String = 7;

        internal string ControlName;
        internal byte Kind;
        internal int Offset;
        internal int End;
        internal long IntegerValue;
        internal float FloatValue;
        internal string StringValue;

        internal ItemDerivedField Clone() { return (ItemDerivedField)MemberwiseClone(); }

        internal bool ContentEquals(ItemDerivedField other)
        {
            return other != null && ControlName == other.ControlName && Kind == other.Kind &&
                IntegerValue == other.IntegerValue && FloatValue == other.FloatValue &&
                StringValue == other.StringValue;
        }
    }

    internal sealed class HoleRecord
    {
        internal int Start;
        internal int GraphNameEnd;
        internal int End;
        internal uint ObjectId;
        internal uint FromStarId;
        internal float FromX;
        internal float FromY;
        internal uint ToStarId;
        internal float ToX;
        internal float ToY;
        internal int TurnCreate;
        internal int HoleType;
        internal string GraphName;
        internal string MapName;

        internal HoleRecord Clone()
        {
            return (HoleRecord)MemberwiseClone();
        }

        internal bool ContentEquals(HoleRecord other)
        {
            return other != null && ObjectId == other.ObjectId && FromStarId == other.FromStarId &&
                FromX == other.FromX && FromY == other.FromY && ToStarId == other.ToStarId &&
                ToX == other.ToX && ToY == other.ToY && TurnCreate == other.TurnCreate &&
                HoleType == other.HoleType && GraphName == other.GraphName && MapName == other.MapName;
        }

        public override string ToString()
        {
            return "Чёрная дыра " + ObjectId + "  " + GraphName;
        }
    }

    internal sealed class AsteroidRecord
    {
        internal int Start;
        internal int GraphNameEnd;
        internal int End;
        internal uint ParentStarId;
        internal uint ObjectId;
        internal string GraphName;
        internal float PositionX;
        internal float PositionY;
        internal float SpeedX;
        internal float SpeedY;
        internal float Mass;
        internal int Minerals;

        internal AsteroidRecord Clone() { return (AsteroidRecord)MemberwiseClone(); }

        internal bool ContentEquals(AsteroidRecord other)
        {
            return other != null && ParentStarId == other.ParentStarId && ObjectId == other.ObjectId && GraphName == other.GraphName &&
                PositionX == other.PositionX && PositionY == other.PositionY &&
                SpeedX == other.SpeedX && SpeedY == other.SpeedY && Mass == other.Mass &&
                Minerals == other.Minerals;
        }

        public override string ToString()
        {
            return "Астероид " + ObjectId + "  " + GraphName;
        }
    }

    internal sealed class MissileRecord
    {
        internal int Start;
        internal int BaseStart;
        internal int End;
        internal int BonusOffset;
        internal int BonusEnd;
        internal int SpecialOffset;
        internal int SpecialEnd;
        internal int PositionOffset;
        internal int StarOffset;
        internal int TargetOffset;
        internal int TargetEnd;
        internal int MissileNoOffset;
        internal int LiveOffset;
        internal int MotionOffset;
        internal int TargetLostOffset;
        internal int TargetLostEnd;
        internal int LastMotionOffset;
        internal uint ParentStarId;
        internal bool IsCustom;
        internal string CustomWeaponName;
        internal uint ObjectId;
        internal uint WeaponId;
        internal byte WeaponType;
        internal byte TechLevel;
        internal int DamageMin;
        internal int DamageMax;
        internal int Bonus;
        internal uint BonusReferenceId;
        internal int Special;
        internal uint SpecialReferenceId;
        internal float PositionX;
        internal float PositionY;
        internal float Angle;
        internal float FromAngle;
        internal uint StarId;
        internal uint ShipId;
        internal byte TargetType;
        internal uint TargetId;
        internal byte MissileNo;
        internal int Live;
        internal float FromAngleOld;
        internal float Speed;
        internal float BaseSpeed;
        internal byte TargetLostType;
        internal uint TargetLostId;
        internal float LastPositionX;
        internal float LastPositionY;
        internal float LastDistanceMin;

        internal MissileRecord Clone() { return (MissileRecord)MemberwiseClone(); }

        internal bool ContentEquals(MissileRecord other)
        {
            return other != null && ParentStarId == other.ParentStarId && IsCustom == other.IsCustom &&
                CustomWeaponName == other.CustomWeaponName && ObjectId == other.ObjectId &&
                WeaponId == other.WeaponId && WeaponType == other.WeaponType && TechLevel == other.TechLevel &&
                DamageMin == other.DamageMin && DamageMax == other.DamageMax &&
                Bonus == other.Bonus && BonusReferenceId == other.BonusReferenceId &&
                Special == other.Special && SpecialReferenceId == other.SpecialReferenceId &&
                PositionX == other.PositionX && PositionY == other.PositionY && Angle == other.Angle &&
                FromAngle == other.FromAngle && StarId == other.StarId && ShipId == other.ShipId &&
                TargetType == other.TargetType && TargetId == other.TargetId && MissileNo == other.MissileNo &&
                Live == other.Live && FromAngleOld == other.FromAngleOld && Speed == other.Speed &&
                BaseSpeed == other.BaseSpeed && TargetLostType == other.TargetLostType &&
                TargetLostId == other.TargetLostId && LastPositionX == other.LastPositionX &&
                LastPositionY == other.LastPositionY && LastDistanceMin == other.LastDistanceMin;
        }

        public override string ToString()
        {
            string custom = IsCustom && !string.IsNullOrEmpty(CustomWeaponName) ? "  " + CustomWeaponName : string.Empty;
            return "Ракета " + ObjectId + "  type " + WeaponType + custom;
        }
    }

    internal sealed class ScriptVariableRecord
    {
        internal string Name;
        internal byte Type;
        internal int IntegerValue;
        internal double DoubleValue;
        internal string StringValue;
        internal List<ScriptVariableRecord> ArrayValue = new List<ScriptVariableRecord>();

        internal ScriptVariableRecord Clone()
        {
            ScriptVariableRecord clone = (ScriptVariableRecord)MemberwiseClone();
            clone.ArrayValue = new List<ScriptVariableRecord>();
            if (ArrayValue != null)
                foreach (ScriptVariableRecord value in ArrayValue) clone.ArrayValue.Add(value.Clone());
            return clone;
        }

        internal bool ContentEquals(ScriptVariableRecord other)
        {
            if (other == null || Name != other.Name || Type != other.Type ||
                IntegerValue != other.IntegerValue || DoubleValue != other.DoubleValue ||
                StringValue != other.StringValue) return false;
            if (Type != 9) return true;
            if (ArrayValue == null || other.ArrayValue == null || ArrayValue.Count != other.ArrayValue.Count)
                return false;
            for (int index = 0; index < ArrayValue.Count; index++)
                if (!ArrayValue[index].ContentEquals(other.ArrayValue[index])) return false;
            return true;
        }

        internal string TypeName
        {
            get
            {
                switch (Type)
                {
                    case 0: return "Null";
                    case 1: return "Integer";
                    case 2: return "Dword";
                    case 3: return "Float";
                    case 4: return "String";
                    case 6: return "dllLibrary";
                    case 9: return "Array";
                    default: return "Type " + Type;
                }
            }
        }

        public override string ToString()
        {
            string value = string.Empty;
            if (Type == 1 || Type == 2) value = IntegerValue.ToString();
            else if (Type == 3) value = DoubleValue.ToString("G");
            else if (Type == 4 || Type == 6) value = StringValue ?? string.Empty;
            else if (Type == 9) value = (ArrayValue == null ? 0 : ArrayValue.Count) + " элементов";
            return (Name ?? string.Empty) + "  [" + TypeName + "]" +
                (value.Length == 0 ? string.Empty : " = " + value);
        }
    }

    internal sealed class ScriptPlanetBindingRecord
    {
        internal string Name;
        internal uint PlanetObjectId;
        internal ScriptPlanetBindingRecord Clone() { return (ScriptPlanetBindingRecord)MemberwiseClone(); }
        internal bool ContentEquals(ScriptPlanetBindingRecord other)
        {
            return other != null && Name == other.Name && PlanetObjectId == other.PlanetObjectId;
        }
    }

    internal sealed class ScriptStarBindingRecord
    {
        internal string Name;
        internal uint StarObjectId;
        internal int LegacyZero;
        internal List<ScriptPlanetBindingRecord> Planets = new List<ScriptPlanetBindingRecord>();

        internal ScriptStarBindingRecord Clone()
        {
            ScriptStarBindingRecord clone = (ScriptStarBindingRecord)MemberwiseClone();
            clone.Planets = new List<ScriptPlanetBindingRecord>();
            foreach (ScriptPlanetBindingRecord planet in Planets) clone.Planets.Add(planet.Clone());
            return clone;
        }

        internal bool ContentEquals(ScriptStarBindingRecord other)
        {
            if (other == null || Name != other.Name || StarObjectId != other.StarObjectId ||
                LegacyZero != other.LegacyZero || Planets.Count != other.Planets.Count) return false;
            for (int index = 0; index < Planets.Count; index++)
                if (!Planets[index].ContentEquals(other.Planets[index])) return false;
            return true;
        }
    }

    internal sealed class ScriptItemRecord
    {
        internal string Name;
        internal bool CanSell;
        internal int Data1;
        internal int Data2;
        internal int Data3;
        internal string TextData1;
        internal string TextData2;
        internal string TextData3;
        internal string OnUseCode;
        internal string OnActCode;
        internal uint ItemObjectId;

        internal ScriptItemRecord Clone() { return (ScriptItemRecord)MemberwiseClone(); }
        internal bool ContentEquals(ScriptItemRecord other)
        {
            return other != null && Name == other.Name && CanSell == other.CanSell &&
                Data1 == other.Data1 && Data2 == other.Data2 && Data3 == other.Data3 &&
                TextData1 == other.TextData1 && TextData2 == other.TextData2 &&
                TextData3 == other.TextData3 && OnUseCode == other.OnUseCode &&
                OnActCode == other.OnActCode && ItemObjectId == other.ItemObjectId;
        }
        public override string ToString() { return Name ?? string.Empty; }
    }

    internal sealed class ScriptShipRecord
    {
        internal int Group;
        internal uint ShipObjectId;
        internal uint Data0;
        internal uint Data1;
        internal uint Data2;
        internal uint Data3;
        internal int StateNum;
        internal string CustomFaction;
        internal bool Hit;
        internal bool HitPlayer;

        internal ScriptShipRecord Clone() { return (ScriptShipRecord)MemberwiseClone(); }
        internal bool ContentEquals(ScriptShipRecord other)
        {
            return other != null && Group == other.Group && ShipObjectId == other.ShipObjectId &&
                Data0 == other.Data0 && Data1 == other.Data1 && Data2 == other.Data2 &&
                Data3 == other.Data3 && StateNum == other.StateNum &&
                CustomFaction == other.CustomFaction && Hit == other.Hit && HitPlayer == other.HitPlayer;
        }
        public override string ToString() { return "Корабль " + ShipObjectId + "  (группа " + Group + ")"; }
    }

    internal sealed class ScriptCacheRecord
    {
        internal int Start;
        internal int End;
        internal string Name;
        internal ushort CountUse;
        internal int LastTurn;
        internal int RunScript;

        internal ScriptCacheRecord Clone() { return (ScriptCacheRecord)MemberwiseClone(); }
        internal bool ContentEquals(ScriptCacheRecord other)
        {
            return other != null && Name == other.Name && CountUse == other.CountUse &&
                LastTurn == other.LastTurn && RunScript == other.RunScript;
        }
        public override string ToString()
        {
            return (Name ?? string.Empty) + "  запусков " + CountUse + ", ход " + LastTurn;
        }
    }

    internal sealed class ScriptOldEtherRecord
    {
        internal string Name;
        internal int Value;

        internal ScriptOldEtherRecord Clone() { return (ScriptOldEtherRecord)MemberwiseClone(); }
        internal bool ContentEquals(ScriptOldEtherRecord other)
        {
            return other != null && Name == other.Name && Value == other.Value;
        }
        public override string ToString() { return (Name ?? string.Empty) + " = " + Value; }
    }

    internal sealed class ScriptRecord
    {
        internal int Start;
        internal int End;
        internal string Name;
        internal List<ScriptOldEtherRecord> OldEthers = new List<ScriptOldEtherRecord>();
        internal List<ScriptVariableRecord> InitVariables = new List<ScriptVariableRecord>();
        internal List<ScriptVariableRecord> TurnVariables = new List<ScriptVariableRecord>();
        internal List<ScriptStarBindingRecord> StarBindings = new List<ScriptStarBindingRecord>();
        internal List<ScriptItemRecord> ItemBindings = new List<ScriptItemRecord>();
        internal List<ScriptShipRecord> ShipBindings = new List<ScriptShipRecord>();
        internal List<string> EtherStrings = new List<string>();

        internal ushort InitVariableCount { get { return checked((ushort)InitVariables.Count); } }
        internal ushort TurnVariableCount { get { return checked((ushort)TurnVariables.Count); } }
        internal int StarBindingCount { get { return StarBindings.Count; } }
        internal int ItemBindingCount { get { return ItemBindings.Count; } }
        internal ushort ShipBindingCount { get { return checked((ushort)ShipBindings.Count); } }
        internal ushort EtherStringCount { get { return checked((ushort)EtherStrings.Count); } }

        internal ScriptRecord Clone()
        {
            ScriptRecord clone = (ScriptRecord)MemberwiseClone();
            clone.OldEthers = new List<ScriptOldEtherRecord>();
            foreach (ScriptOldEtherRecord value in OldEthers) clone.OldEthers.Add(value.Clone());
            clone.InitVariables = new List<ScriptVariableRecord>();
            foreach (ScriptVariableRecord value in InitVariables) clone.InitVariables.Add(value.Clone());
            clone.TurnVariables = new List<ScriptVariableRecord>();
            foreach (ScriptVariableRecord value in TurnVariables) clone.TurnVariables.Add(value.Clone());
            clone.StarBindings = new List<ScriptStarBindingRecord>();
            foreach (ScriptStarBindingRecord value in StarBindings) clone.StarBindings.Add(value.Clone());
            clone.ItemBindings = new List<ScriptItemRecord>();
            foreach (ScriptItemRecord value in ItemBindings) clone.ItemBindings.Add(value.Clone());
            clone.ShipBindings = new List<ScriptShipRecord>();
            foreach (ScriptShipRecord value in ShipBindings) clone.ShipBindings.Add(value.Clone());
            clone.EtherStrings = new List<string>(EtherStrings);
            return clone;
        }

        internal bool ContentEquals(ScriptRecord other)
        {
            if (other == null || Name != other.Name || OldEthers.Count != other.OldEthers.Count ||
                InitVariables.Count != other.InitVariables.Count || TurnVariables.Count != other.TurnVariables.Count ||
                StarBindings.Count != other.StarBindings.Count || ItemBindings.Count != other.ItemBindings.Count ||
                ShipBindings.Count != other.ShipBindings.Count || EtherStrings.Count != other.EtherStrings.Count)
                return false;
            for (int index = 0; index < OldEthers.Count; index++)
                if (!OldEthers[index].ContentEquals(other.OldEthers[index])) return false;
            for (int index = 0; index < InitVariables.Count; index++)
                if (!InitVariables[index].ContentEquals(other.InitVariables[index])) return false;
            for (int index = 0; index < TurnVariables.Count; index++)
                if (!TurnVariables[index].ContentEquals(other.TurnVariables[index])) return false;
            for (int index = 0; index < StarBindings.Count; index++)
                if (!StarBindings[index].ContentEquals(other.StarBindings[index])) return false;
            for (int index = 0; index < ItemBindings.Count; index++)
                if (!ItemBindings[index].ContentEquals(other.ItemBindings[index])) return false;
            for (int index = 0; index < ShipBindings.Count; index++)
                if (!ShipBindings[index].ContentEquals(other.ShipBindings[index])) return false;
            for (int index = 0; index < EtherStrings.Count; index++)
                if (EtherStrings[index] != other.EtherStrings[index]) return false;
            return true;
        }

        public override string ToString()
        {
            return Name + "  INIT " + InitVariables.Count + " / TURN " + TurnVariables.Count;
        }
    }

    internal sealed class AchievementStatsRecord
    {
        internal int StructureStart;
        internal int Start;
        internal int End;
        internal int ReceivedListStart;
        internal int PlayerEnd;
        internal int AsteroidsDestroyed;
        internal int FriedShips;
        internal int DefendedSystem;
        internal int PirateSystems;
        internal byte ScienceProgress;
        internal int ProgramsUsed;
        internal int PiratesFreed;
        internal int HealthDrained;
        internal int FuelGottenFromSun;
        internal int FuelTankLastId;
        internal int PlanetsVisited;
        internal List<string> Received = new List<string>();
        internal int JournalListOffset;
        internal int JournalEndOffset;
        internal List<PlayerJournalRecord> JournalRecords = new List<PlayerJournalRecord>();
        internal int PlayerNewsListOffset;
        internal int PlayerNewsEndOffset;
        internal List<GalaxyNewsRecord> PlayerNewsRecords = new List<GalaxyNewsRecord>();

        internal AchievementStatsRecord Clone()
        {
            AchievementStatsRecord result = (AchievementStatsRecord)MemberwiseClone();
            result.Received = new List<string>(Received);
            result.JournalRecords = new List<PlayerJournalRecord>();
            foreach (PlayerJournalRecord record in JournalRecords)
                result.JournalRecords.Add(record.Clone());
            result.PlayerNewsRecords = new List<GalaxyNewsRecord>();
            foreach (GalaxyNewsRecord record in PlayerNewsRecords)
                result.PlayerNewsRecords.Add(record.Clone());
            return result;
        }

        internal bool ContentEquals(AchievementStatsRecord other)
        {
            if (other == null || AsteroidsDestroyed != other.AsteroidsDestroyed ||
                FriedShips != other.FriedShips || DefendedSystem != other.DefendedSystem ||
                PirateSystems != other.PirateSystems || ScienceProgress != other.ScienceProgress ||
                ProgramsUsed != other.ProgramsUsed || PiratesFreed != other.PiratesFreed ||
                HealthDrained != other.HealthDrained || FuelGottenFromSun != other.FuelGottenFromSun ||
                FuelTankLastId != other.FuelTankLastId || PlanetsVisited != other.PlanetsVisited ||
                Received.Count != other.Received.Count) return false;
            for (int index = 0; index < Received.Count; index++)
                if (Received[index] != other.Received[index]) return false;
            return true;
        }
    }

    internal sealed class GalaxyPrefixData
    {
        internal int Start;
        internal int End;
        internal string UsedMods;
        internal int RandomSeed;
        internal uint RandomOut;
        internal int RangersAverageCapital;
        internal int RangersMaxCapital;
        internal float RangersAverageStrength;
        internal float RangersMaxStrength;
        internal bool Crack;
        internal bool Cheat;
        internal int ReservedZero;
        internal int CheatPoints;
        internal int SaveCount;
        internal int LoadCount;
        internal int CustomModWeaponCount;

        internal GalaxyPrefixData Clone()
        {
            return (GalaxyPrefixData)MemberwiseClone();
        }

        internal int UsedModCount
        {
            get
            {
                return string.IsNullOrEmpty(UsedMods) ? 0 :
                    UsedMods.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }

        internal bool ContentEquals(GalaxyPrefixData other)
        {
            return other != null && UsedMods == other.UsedMods && RandomSeed == other.RandomSeed &&
                RandomOut == other.RandomOut && RangersAverageCapital == other.RangersAverageCapital &&
                RangersMaxCapital == other.RangersMaxCapital && RangersAverageStrength == other.RangersAverageStrength &&
                RangersMaxStrength == other.RangersMaxStrength && Crack == other.Crack && Cheat == other.Cheat &&
                ReservedZero == other.ReservedZero && CheatPoints == other.CheatPoints && SaveCount == other.SaveCount &&
                LoadCount == other.LoadCount && CustomModWeaponCount == other.CustomModWeaponCount;
        }
    }

    internal sealed class GalaxySummaryData
    {
        internal int TurnOffset;
        internal uint Turn;
        internal int DifficultyOffset;
        internal int PrincipalObjectOffset;
        internal int CompleteQuestListOffset;
        internal int GalaxyNewsListOffset;
        internal int KellerAttackOffset;
        internal int WarOperationListOffset;
        internal int GateListOffset;
        internal ushort PirateCount;
        internal ushort ClanPirateCount;
        internal ushort TransportCount;
        internal byte[] DifficultyLevels;
        internal uint PlayerObjectId;
        internal uint AutoBattleShipObjectId;
        internal uint BlazerObjectId;
        internal uint KellerObjectId;
        internal uint TerronObjectId;
        internal uint CurrentStarObjectId;
        internal uint KellerAttackStarObjectId;
        internal int KellerAttackState;
        internal uint TerronStarObjectId;
        internal uint[] EminentRangerObjectIds;
        internal int CompleteQuestCount;
        internal List<CompleteQuestRecord> CompleteQuests = new List<CompleteQuestRecord>();
        internal int GalaxyNewsCount;
        internal List<GalaxyNewsRecord> GalaxyNews = new List<GalaxyNewsRecord>();
        internal int ScriptShopSlotCountOffset;
        internal int ScriptShopSlotListEndOffset;
        internal List<ScriptShopSlotRecord> ScriptShopSlots = new List<ScriptShopSlotRecord>();
        internal int GlobalVariableListOffset;
        internal List<ScriptVariableRecord> GlobalVariables = new List<ScriptVariableRecord>();
        internal int ScriptCacheListOffset;
        internal List<ScriptCacheRecord> ScriptCache = new List<ScriptCacheRecord>();
        internal int ActiveScriptListOffset;
        internal List<ScriptRecord> ActiveScripts = new List<ScriptRecord>();
        internal List<WarOperationRecord> WarOperations = new List<WarOperationRecord>();
        internal List<GateRecord> Gates = new List<GateRecord>();
        internal int LateScalarOffset;
        internal uint DayShipsNotTalkWithPlayer;
        internal uint DayShipsNotGreetingPlayer;
        internal float OpenCommunicator;
        internal float BlazerResearch;
        internal uint BlazerMaterial;
        internal float KellerResearch;
        internal uint KellerMaterial;
        internal float TerronResearch;
        internal uint TerronMaterial;
        internal float ScienceBaseWorkPercentOld;
        internal int WarDeltaDominators;
        internal int WarDeltaPirates;
        internal int WarDeltaCoalition;
        internal int GarbageCount;
        internal int HangarOffset;
        internal uint[] HangarShipObjectIds;
        internal int CheatsOffset;
        internal int CheatsUpdate;
        internal int CheatsAssigned;
        internal int CheatsTest;
        internal int CheatsValue;
        internal int CheatsTestOffset;
        internal int PostCheatsOffset;
        internal int TerronTurnWin;
        internal int KellerTurnWin;
        internal int BlazerTurnWin;
        internal int PirateTurnWin;
        internal int PirateWinType;
        internal int CoalitionDefeatedTurn;
        internal bool GraphDominator;
        internal byte Gluk;
        internal bool IronWill;
        internal int IronWillOffset;
        internal uint PlanetNewsObjectId;
        internal int NextSpecialShipTurn;
        internal int GalaxyEventListOffset;
        internal int GalaxyEventListEndOffset;
        internal int GalaxyEventCount;
        internal List<GalaxyEventRecord> GalaxyEvents = new List<GalaxyEventRecord>();
        internal int[] InterfaceOverrideListOffsets = new int[5];
        internal int[] InterfaceOverrideListEndOffsets = new int[5];
        internal List<InterfaceOverrideRecord> InterfaceOverrides = new List<InterfaceOverrideRecord>();
        internal int PlanetReferenceListOffset;
        internal int RangerReferenceListOffset;
        internal int RangerCount;
        internal uint[] RangerObjectIds;
        internal bool PlanetBattlesDisabled;
        internal int PlanetBattlesDisabledOffset;
        internal bool PrepareToDump;
        internal string DumpName;
        internal bool CustomRules;
        internal int CustomRulesOffset;
        internal byte[] CustomRuleLevels;
        internal bool[] CustomRuleFlags;
        internal byte HullGrowth;
        internal bool[] CustomRuleLateFlags;
        internal uint CurrentObjectId;
        internal uint NextObjectId;
        internal uint SystemCrc;
        internal int End;

        internal int DifficultyPercent
        {
            get
            {
                int total = 0;
                foreach (byte level in DifficultyLevels)
                    total += level * 50 + 50;
                return total / DifficultyLevels.Length;
            }
        }

        internal GalaxySummaryData Clone()
        {
            GalaxySummaryData clone = (GalaxySummaryData)MemberwiseClone();
            clone.DifficultyLevels = DifficultyLevels == null ? null : (byte[])DifficultyLevels.Clone();
            clone.EminentRangerObjectIds = EminentRangerObjectIds == null ? null :
                (uint[])EminentRangerObjectIds.Clone();
            clone.HangarShipObjectIds = HangarShipObjectIds == null ? null :
                (uint[])HangarShipObjectIds.Clone();
            clone.RangerObjectIds = RangerObjectIds == null ? null :
                (uint[])RangerObjectIds.Clone();
            clone.CustomRuleLevels = CustomRuleLevels == null ? null : (byte[])CustomRuleLevels.Clone();
            clone.CustomRuleFlags = CustomRuleFlags == null ? null : (bool[])CustomRuleFlags.Clone();
            clone.CustomRuleLateFlags = CustomRuleLateFlags == null ? null :
                (bool[])CustomRuleLateFlags.Clone();
            clone.InterfaceOverrides = new List<InterfaceOverrideRecord>();
            foreach (InterfaceOverrideRecord record in InterfaceOverrides)
                clone.InterfaceOverrides.Add(record.Clone());
            clone.CompleteQuests = new List<CompleteQuestRecord>();
            foreach (CompleteQuestRecord record in CompleteQuests)
                clone.CompleteQuests.Add(record.Clone());
            clone.GalaxyNews = new List<GalaxyNewsRecord>();
            foreach (GalaxyNewsRecord record in GalaxyNews)
                clone.GalaxyNews.Add(record.Clone());
            clone.ScriptShopSlots = new List<ScriptShopSlotRecord>();
            foreach (ScriptShopSlotRecord record in ScriptShopSlots)
                clone.ScriptShopSlots.Add(record.Clone());
            clone.GalaxyEvents = new List<GalaxyEventRecord>();
            foreach (GalaxyEventRecord record in GalaxyEvents)
                clone.GalaxyEvents.Add(record.Clone());
            clone.InterfaceOverrideListOffsets = (int[])InterfaceOverrideListOffsets.Clone();
            clone.InterfaceOverrideListEndOffsets = (int[])InterfaceOverrideListEndOffsets.Clone();
            clone.GlobalVariables = new List<ScriptVariableRecord>();
            foreach (ScriptVariableRecord record in GlobalVariables)
                clone.GlobalVariables.Add(record.Clone());
            clone.ScriptCache = new List<ScriptCacheRecord>();
            foreach (ScriptCacheRecord record in ScriptCache)
                clone.ScriptCache.Add(record.Clone());
            clone.ActiveScripts = new List<ScriptRecord>();
            foreach (ScriptRecord record in ActiveScripts)
                clone.ActiveScripts.Add(record.Clone());
            clone.WarOperations = new List<WarOperationRecord>();
            foreach (WarOperationRecord record in WarOperations)
                clone.WarOperations.Add(record.Clone());
            clone.Gates = new List<GateRecord>();
            foreach (GateRecord record in Gates) clone.Gates.Add(record.Clone());
            return clone;
        }

        internal bool EditableContentEquals(GalaxySummaryData other)
        {
            return other != null && IronWill == other.IronWill &&
                PlanetBattlesDisabled == other.PlanetBattlesDisabled && CustomRules == other.CustomRules &&
                HullGrowth == other.HullGrowth && BlazerResearch == other.BlazerResearch &&
                BlazerMaterial == other.BlazerMaterial && KellerResearch == other.KellerResearch &&
                KellerMaterial == other.KellerMaterial && TerronResearch == other.TerronResearch &&
                TerronMaterial == other.TerronMaterial && WarDeltaDominators == other.WarDeltaDominators &&
                WarDeltaPirates == other.WarDeltaPirates && WarDeltaCoalition == other.WarDeltaCoalition &&
                EqualRecords(CompleteQuests, other.CompleteQuests) &&
                EqualRecords(GalaxyNews, other.GalaxyNews) &&
                EqualScriptShopSlots(ScriptShopSlots, other.ScriptShopSlots) &&
                EqualRecords(GalaxyEvents, other.GalaxyEvents) &&
                EqualVariables(GlobalVariables, other.GlobalVariables) &&
                EqualScriptCache(ScriptCache, other.ScriptCache) &&
                EqualScripts(ActiveScripts, other.ActiveScripts) &&
                EqualWarOperations(WarOperations, other.WarOperations) &&
                EqualGates(Gates, other.Gates) &&
                AutoBattleShipObjectId == other.AutoBattleShipObjectId &&
                RangerCount == other.RangerCount &&
                EqualArray(RangerObjectIds, other.RangerObjectIds) &&
                EqualArray(EminentRangerObjectIds, other.EminentRangerObjectIds) &&
                KellerAttackStarObjectId == other.KellerAttackStarObjectId &&
                KellerAttackState == other.KellerAttackState &&
                EqualArray(DifficultyLevels, other.DifficultyLevels) &&
                EqualArray(CustomRuleLevels, other.CustomRuleLevels) &&
                EqualArray(CustomRuleFlags, other.CustomRuleFlags) &&
                EqualArray(CustomRuleLateFlags, other.CustomRuleLateFlags);
        }

        private static bool EqualScriptShopSlots(IList<ScriptShopSlotRecord> left,
            IList<ScriptShopSlotRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        private static bool EqualArray<T>(T[] left, T[] right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Length != right.Length) return false;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int index = 0; index < left.Length; index++)
                if (!comparer.Equals(left[index], right[index])) return false;
            return true;
        }

        internal static bool EqualRecords<T>(IList<T> left, IList<T> right) where T : class
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
            {
                CompleteQuestRecord leftQuest = left[index] as CompleteQuestRecord;
                CompleteQuestRecord rightQuest = right[index] as CompleteQuestRecord;
                if (leftQuest != null && rightQuest != null)
                {
                    if (!leftQuest.ContentEquals(rightQuest)) return false;
                    continue;
                }
                GalaxyNewsRecord leftNews = left[index] as GalaxyNewsRecord;
                GalaxyNewsRecord rightNews = right[index] as GalaxyNewsRecord;
                if (leftNews != null && rightNews != null)
                {
                    if (!leftNews.ContentEquals(rightNews)) return false;
                    continue;
                }
                GalaxyEventRecord leftEvent = left[index] as GalaxyEventRecord;
                GalaxyEventRecord rightEvent = right[index] as GalaxyEventRecord;
                if (leftEvent == null || rightEvent == null || !leftEvent.ContentEquals(rightEvent)) return false;
            }
            return true;
        }

        private static bool EqualWarOperations(IList<WarOperationRecord> left,
            IList<WarOperationRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        private static bool EqualScripts(IList<ScriptRecord> left, IList<ScriptRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool EqualVariables(IList<ScriptVariableRecord> left,
            IList<ScriptVariableRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        internal static bool EqualScriptCache(IList<ScriptCacheRecord> left,
            IList<ScriptCacheRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }

        private static bool EqualGates(IList<GateRecord> left, IList<GateRecord> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (!left[index].ContentEquals(right[index])) return false;
            return true;
        }
    }

    internal sealed class GateRecord
    {
        internal int Start;
        internal int End;
        internal float X;
        internal float Y;
        internal byte Angle;
        internal ushort Size;
        internal string Text;

        internal GateRecord Clone() { return (GateRecord)MemberwiseClone(); }
        internal bool ContentEquals(GateRecord other)
        {
            return other != null && X == other.X && Y == other.Y && Angle == other.Angle &&
                Size == other.Size && Text == other.Text;
        }
        public override string ToString()
        {
            string caption = string.IsNullOrWhiteSpace(Text) ? "Врата" : Text.Replace('\r', ' ').Replace('\n', ' ');
            if (caption.Length > 45) caption = caption.Substring(0, 42) + "...";
            return caption + "  (" + X.ToString("0.##") + "; " + Y.ToString("0.##") + ")";
        }
    }

    internal sealed class WarOperationOrderRecord
    {
        internal byte Type;
        internal uint ObjectId;
        internal float DestinationX;
        internal float DestinationY;
        internal byte EndMode;
        internal int EndTurn;

        internal WarOperationOrderRecord Clone() { return (WarOperationOrderRecord)MemberwiseClone(); }
        internal bool ContentEquals(WarOperationOrderRecord other)
        {
            return other != null && Type == other.Type && ObjectId == other.ObjectId &&
                DestinationX == other.DestinationX && DestinationY == other.DestinationY &&
                EndMode == other.EndMode && EndTurn == other.EndTurn;
        }
        public override string ToString()
        {
            return "Тип " + Type + " → " + ObjectId + "  (" + DestinationX.ToString("0.##") +
                "; " + DestinationY.ToString("0.##") + ")";
        }
    }

    internal sealed class WarOperationRecord
    {
        internal int Start;
        internal int End;
        internal ushort Turn;
        internal uint RandomSeed;
        internal uint RandomOut;
        internal byte LegacyZero;
        internal List<uint> ShipObjectIds = new List<uint>();
        internal List<WarOperationOrderRecord> Orders = new List<WarOperationOrderRecord>();

        internal WarOperationRecord Clone()
        {
            WarOperationRecord clone = (WarOperationRecord)MemberwiseClone();
            clone.ShipObjectIds = new List<uint>(ShipObjectIds);
            clone.Orders = new List<WarOperationOrderRecord>();
            foreach (WarOperationOrderRecord order in Orders) clone.Orders.Add(order.Clone());
            return clone;
        }
        internal bool ContentEquals(WarOperationRecord other)
        {
            if (other == null || Turn != other.Turn || RandomSeed != other.RandomSeed ||
                RandomOut != other.RandomOut || LegacyZero != other.LegacyZero ||
                ShipObjectIds.Count != other.ShipObjectIds.Count || Orders.Count != other.Orders.Count)
                return false;
            for (int index = 0; index < ShipObjectIds.Count; index++)
                if (ShipObjectIds[index] != other.ShipObjectIds[index]) return false;
            for (int index = 0; index < Orders.Count; index++)
                if (!Orders[index].ContentEquals(other.Orders[index])) return false;
            return true;
        }
        public override string ToString()
        {
            return "Ход " + Turn + " — кораблей " + ShipObjectIds.Count + ", приказов " + Orders.Count;
        }
    }

    internal sealed class CompleteQuestRecord
    {
        internal int Start;
        internal int End;
        internal uint PlanetObjectId;
        internal byte Type;
        internal ushort Number;
        internal string Text;
        internal bool Successful;
        internal bool Rejection;

        internal CompleteQuestRecord Clone() { return (CompleteQuestRecord)MemberwiseClone(); }
        internal bool ContentEquals(CompleteQuestRecord other)
        {
            return other != null && PlanetObjectId == other.PlanetObjectId && Type == other.Type &&
                Number == other.Number && Text == other.Text && Successful == other.Successful &&
                Rejection == other.Rejection;
        }
        public override string ToString()
        {
            string value = string.IsNullOrWhiteSpace(Text) ? "Задание " + Number : Text;
            value = value.Replace('\r', ' ').Replace('\n', ' ').Trim();
            return value.Length > 60 ? value.Substring(0, 57) + "..." : value;
        }
    }

    internal sealed class GalaxyNewsRecord
    {
        internal int Start;
        internal int End;
        internal uint Id;
        internal uint Turn;
        internal byte Type;
        internal string Text;

        internal GalaxyNewsRecord Clone() { return (GalaxyNewsRecord)MemberwiseClone(); }
        internal bool ContentEquals(GalaxyNewsRecord other)
        {
            return other != null && Id == other.Id && Turn == other.Turn && Type == other.Type &&
                Text == other.Text;
        }
        public override string ToString()
        {
            string value = string.IsNullOrWhiteSpace(Text) ? "Новость " + Id : Text;
            value = value.Replace('\r', ' ').Replace('\n', ' ').Trim();
            return value.Length > 60 ? value.Substring(0, 57) + "..." : value;
        }
    }

    internal sealed class GalaxyEventRecord
    {
        internal int Start;
        internal int End;
        internal string Type;
        internal int Turn;
        internal List<int> Data = new List<int>();
        internal List<string> TextData = new List<string>();

        internal GalaxyEventRecord Clone()
        {
            GalaxyEventRecord result = (GalaxyEventRecord)MemberwiseClone();
            result.Data = new List<int>(Data);
            result.TextData = new List<string>(TextData);
            return result;
        }

        internal bool ContentEquals(GalaxyEventRecord other)
        {
            if (other == null || Type != other.Type || Turn != other.Turn ||
                Data.Count != other.Data.Count || TextData.Count != other.TextData.Count) return false;
            for (int index = 0; index < Data.Count; index++)
                if (Data[index] != other.Data[index]) return false;
            for (int index = 0; index < TextData.Count; index++)
                if (TextData[index] != other.TextData[index]) return false;
            return true;
        }

        public override string ToString()
        {
            return (Type ?? "TGalaxyEvent") + " — ход " + Turn +
                "; данные " + Data.Count + "/" + TextData.Count;
        }
    }

    internal sealed class GalaxyMapLine
    {
        internal float X1;
        internal float Y1;
        internal float X2;
        internal float Y2;
    }

    internal sealed class ConstellationRecord
    {
        internal int Start;
        internal int VisibleOffset;
        internal uint ObjectId;
        internal bool Visible;
        internal ushort Color;
        internal float X;
        internal float Y;
        internal List<uint> StarObjectIds = new List<uint>();
        internal List<uint> ConnectionObjectIds = new List<uint>();
        internal List<GalaxyMapLine> BoundaryLines = new List<GalaxyMapLine>();
        internal List<GalaxyMapLine> HiddenBoundaryLines = new List<GalaxyMapLine>();
        internal List<GalaxyMapLine> MapLines = new List<GalaxyMapLine>();

        internal ConstellationRecord Clone()
        {
            ConstellationRecord copy = (ConstellationRecord)MemberwiseClone();
            copy.StarObjectIds = new List<uint>(StarObjectIds);
            copy.ConnectionObjectIds = new List<uint>(ConnectionObjectIds);
            copy.BoundaryLines = CloneMapLines(BoundaryLines);
            copy.HiddenBoundaryLines = CloneMapLines(HiddenBoundaryLines);
            copy.MapLines = CloneMapLines(MapLines);
            return copy;
        }

        internal bool ContentEquals(ConstellationRecord other)
        {
            return ContentEquals(other, true);
        }

        private bool ContentEquals(ConstellationRecord other, bool includeVisibility)
        {
            if (other == null || ObjectId != other.ObjectId ||
                (includeVisibility && Visible != other.Visible) || Color != other.Color ||
                X != other.X || Y != other.Y || StarObjectIds.Count != other.StarObjectIds.Count ||
                ConnectionObjectIds.Count != other.ConnectionObjectIds.Count ||
                !MapLinesEqual(BoundaryLines, other.BoundaryLines) ||
                !MapLinesEqual(HiddenBoundaryLines, other.HiddenBoundaryLines) ||
                !MapLinesEqual(MapLines, other.MapLines))
                return false;
            for (int index = 0; index < StarObjectIds.Count; index++)
                if (StarObjectIds[index] != other.StarObjectIds[index]) return false;
            for (int index = 0; index < ConnectionObjectIds.Count; index++)
                if (ConnectionObjectIds[index] != other.ConnectionObjectIds[index]) return false;
            return true;
        }

        private static List<GalaxyMapLine> CloneMapLines(IList<GalaxyMapLine> source)
        {
            List<GalaxyMapLine> result = new List<GalaxyMapLine>();
            if (source != null)
                foreach (GalaxyMapLine line in source)
                    result.Add(new GalaxyMapLine { X1 = line.X1, Y1 = line.Y1,
                        X2 = line.X2, Y2 = line.Y2 });
            return result;
        }

        private static bool MapLinesEqual(IList<GalaxyMapLine> left, IList<GalaxyMapLine> right)
        {
            if (left == null || right == null || left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
                if (left[index].X1 != right[index].X1 || left[index].Y1 != right[index].Y1 ||
                    left[index].X2 != right[index].X2 || left[index].Y2 != right[index].Y2)
                    return false;
            return true;
        }

        internal bool StructuralContentEquals(ConstellationRecord other)
        {
            return ContentEquals(other, false);
        }

        public override string ToString()
        {
            return string.Format("{0:00}  Сектор ({1} систем){2}", ObjectId, StarObjectIds.Count,
                Visible ? "  [открыт]" : string.Empty);
        }
    }

    internal sealed class CustomWeaponInfoRecord
    {
        internal int Start;
        internal int End;
        internal string SystemName;
        internal byte TechLevel;
        internal byte TechRadius;
        internal float ModCost;
        internal int MinDamage;
        internal int MaxDamage;
        internal int AverageSize;
        internal int AverageRadius;
        internal int Speed;
        internal int MissileRadius;
        internal int MissileMinSpeed;
        internal int MissileMaxSpeed;
        internal byte MissileChanceToBeHit;
        internal uint DamageType;
        internal byte ShotType;
        internal byte ShotCount;
        internal byte AttackCount;
        internal float SecondaryDamageRadius;
        internal float MiningFactor;
        internal float[] WeaponDamageSet = new float[8];
        internal string PrimarySE;
        internal string SecondarySE;
        internal string AreaSE;
        internal int DefaultPalette;
        internal byte Availability;
        internal byte ABWeaponType;

        internal CustomWeaponInfoRecord Clone()
        {
            CustomWeaponInfoRecord copy = (CustomWeaponInfoRecord)MemberwiseClone();
            copy.WeaponDamageSet = (float[])WeaponDamageSet.Clone();
            return copy;
        }

        internal bool ContentEquals(CustomWeaponInfoRecord other)
        {
            if (other == null || SystemName != other.SystemName || TechLevel != other.TechLevel ||
                TechRadius != other.TechRadius || ModCost != other.ModCost || MinDamage != other.MinDamage ||
                MaxDamage != other.MaxDamage || AverageSize != other.AverageSize ||
                AverageRadius != other.AverageRadius || Speed != other.Speed ||
                MissileRadius != other.MissileRadius || MissileMinSpeed != other.MissileMinSpeed ||
                MissileMaxSpeed != other.MissileMaxSpeed ||
                MissileChanceToBeHit != other.MissileChanceToBeHit || DamageType != other.DamageType ||
                ShotType != other.ShotType || ShotCount != other.ShotCount || AttackCount != other.AttackCount ||
                SecondaryDamageRadius != other.SecondaryDamageRadius || MiningFactor != other.MiningFactor ||
                PrimarySE != other.PrimarySE || SecondarySE != other.SecondarySE || AreaSE != other.AreaSE ||
                DefaultPalette != other.DefaultPalette || Availability != other.Availability ||
                ABWeaponType != other.ABWeaponType || WeaponDamageSet == null ||
                other.WeaponDamageSet == null || WeaponDamageSet.Length != other.WeaponDamageSet.Length)
                return false;
            for (int index = 0; index < WeaponDamageSet.Length; index++)
                if (WeaponDamageSet[index] != other.WeaponDamageSet[index]) return false;
            return true;
        }

        public override string ToString() { return SystemName ?? string.Empty; }
    }

    internal sealed class CustomWeaponDeleteResult
    {
        internal readonly HashSet<int> RemovedItemStarts = new HashSet<int>();
        internal readonly HashSet<uint> RemovedItemIds = new HashSet<uint>();
        internal readonly HashSet<uint> RemovedMissileIds = new HashSet<uint>();
        internal int RemovedOwnerRecords;
    }

    internal enum InterfaceOverrideKind : byte
    {
        State = 0,
        Text = 1,
        Image = 2,
        Position = 3,
        Size = 4
    }

    internal sealed class StoredItemRecord
    {
        internal int Start;
        internal int End;
        internal string ScriptTag;
        internal int ItemTypeOffset;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal StoredItemRecord Clone()
        {
            return (StoredItemRecord)MemberwiseClone();
        }

        internal bool ContentEquals(StoredItemRecord other)
        {
            return other != null && ScriptTag == other.ScriptTag && ItemType == other.ItemType &&
                ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return (ScriptTag ?? string.Empty) + "  [" + ItemType + ":" + ItemObjectId + "]";
        }
    }

    internal sealed class ScriptShopSlotRecord
    {
        internal int Start;
        internal int End;
        internal byte X;
        internal byte Y;
        internal bool HasEquipment;
        internal int FactoryDiscriminatorOffset;
        internal byte ItemType;
        internal int ItemStart;
        internal uint ItemObjectId;

        internal ScriptShopSlotRecord Clone()
        {
            return (ScriptShopSlotRecord)MemberwiseClone();
        }

        internal bool ContentEquals(ScriptShopSlotRecord other)
        {
            return other != null && X == other.X && Y == other.Y &&
                HasEquipment == other.HasEquipment && ItemType == other.ItemType &&
                ItemObjectId == other.ItemObjectId;
        }

        public override string ToString()
        {
            return "Shop [" + X + "; " + Y + "] " +
                (HasEquipment ? ItemType + ":" + ItemObjectId : "пусто");
        }
    }

    internal sealed class InterfaceOverrideRecord
    {
        internal int Start;
        internal int End;
        internal InterfaceOverrideKind Kind;
        internal string ModuleName;
        internal string GuiName;
        internal byte NewState;
        internal byte OldState;
        internal string NewValue;
        internal string OldValue;
        internal int NewX;
        internal int NewY;
        internal double NewZ;
        internal int OldX;
        internal int OldY;
        internal double OldZ;

        internal InterfaceOverrideRecord Clone()
        {
            return (InterfaceOverrideRecord)MemberwiseClone();
        }

        internal bool ContentEquals(InterfaceOverrideRecord other)
        {
            return other != null && Kind == other.Kind && ModuleName == other.ModuleName &&
                GuiName == other.GuiName && NewState == other.NewState && OldState == other.OldState &&
                NewValue == other.NewValue && OldValue == other.OldValue && NewX == other.NewX &&
                NewY == other.NewY && NewZ == other.NewZ && OldX == other.OldX &&
                OldY == other.OldY && OldZ == other.OldZ;
        }

        public override string ToString()
        {
            return "[" + (ModuleName ?? string.Empty) + "] " + (GuiName ?? string.Empty);
        }
    }

    internal sealed class SavContainer
    {
        internal const double AsteroidPositionScale = 6.0E-9;
        private const int MetadataSize = 32;
        private static readonly uint[] CrcTable = BuildCrcTable();

        internal string SourcePath;
        internal byte[] OriginalData;
        internal string[] Header;
        internal byte[] PreviewBlock;
        internal byte[] MapBlock;
        internal uint StoredCrc32;
        internal uint EncryptionKey;
        internal byte[] EncryptedMainBlock;
        internal byte[] FilmBlock;
        internal byte[] MainPayload;
        internal SavMetadata Metadata;
        internal bool MainCrcValid;
        internal int PlayerMessageCount;
        internal int PlayerHoldCount;
        internal int GalaxyOffset;
        internal List<PlayerMessageRecord> PlayerMessages = new List<PlayerMessageRecord>();
        internal List<PlayerHoldRecord> PlayerHoldUnits = new List<PlayerHoldRecord>();
        internal GalaxyPrefixData GalaxyPrefix;
        internal GalaxySummaryData GalaxySummary;
        internal List<string> CustomWeaponNames = new List<string>();
        internal List<CustomWeaponInfoRecord> CustomWeaponInfos = new List<CustomWeaponInfoRecord>();
        internal Dictionary<string, byte> CustomWeaponDescriptorTypes =
            new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        internal int GalaxyConstellationCount;
        internal List<ConstellationRecord> GalaxyConstellations = new List<ConstellationRecord>();
        internal int GalaxyStarCount;
        internal int GalaxyStarsOffset;
        internal List<StarHeaderRecord> GalaxyStars = new List<StarHeaderRecord>();
        internal List<PlanetHeaderRecord> GalaxyPlanets = new List<PlanetHeaderRecord>();
        internal List<ShipHeaderRecord> GalaxyShips = new List<ShipHeaderRecord>();
        internal List<ItemHeaderRecord> GalaxyItems = new List<ItemHeaderRecord>();
        internal List<StoredItemRecord> StoredItems = new List<StoredItemRecord>();
        internal bool HasExactStoredItemList;
        internal int StoredItemCountOffset = -1;
        internal int StoredItemListEndOffset = -1;
        internal List<HoleRecord> GalaxyHoles = new List<HoleRecord>();
        internal int HoleListCountOffset = -1;
        internal int HoleListEndOffset = -1;
        internal List<AsteroidRecord> GalaxyAsteroids = new List<AsteroidRecord>();
        internal List<MissileRecord> GalaxyMissiles = new List<MissileRecord>();
        internal List<ScriptRecord> ActiveScripts = new List<ScriptRecord>();
        internal int ActiveScriptListOffset = -1;
        internal AchievementStatsRecord AchievementStats;
        internal int ShipCount;
        internal int StationCount;

        // Display live world objects, not every factory object found while
        // structurally scanning the payload. Keep the full catalog for safe
        // reference editing and a separate user-facing statistics count.
        internal int VisibleShipCount
        {
            get
            {
                // Include the four principal-ship references and eminent-ranger
                // slots that live outside the main world list.
                int eminent = GalaxySummary.EminentRangerObjectIds == null ? 0 :
                    GalaxySummary.EminentRangerObjectIds.Length;
                return ShipCount + 4 + eminent;
            }
        }

        internal int VisibleItemCount
        {
            get
            {
                // The statistics counter is an ownership walk, not the size of the structural TItem
                // catalog: stored-item wrappers and script-shop slots are not
                // included, while galaxy garbage and the TSatellite objects
                // nested in ruins are.
                int count = GalaxySummary.GarbageCount;
                foreach (StarHeaderRecord star in GalaxyStars)
                    count += star.SpaceItems.Count + star.DropItems.Count;
                foreach (PlanetHeaderRecord planet in GalaxyPlanets)
                    count += planet.EquipmentShopItems.Count + planet.GoneItems.Count;
                foreach (ShipHeaderRecord ship in GalaxyShips)
                {
                    count += ship.EquipmentItems.Count + ship.ArtefactItems.Count + ship.DropListItems.Count;
                    count += ship.RuinsEquipmentItems.Count;
                    if (ship.PlayerBridgeRuins != null)
                        count += ship.PlayerBridgeRuins.RuinsEquipmentItems.Count;
                }
                foreach (ItemHeaderRecord item in GalaxyItems)
                {
                    if (item.Type == 73) count++;
                    if (item.NestedTranclucator != null)
                        count += item.NestedTranclucator.EquipmentItems.Count +
                            item.NestedTranclucator.ArtefactItems.Count;
                }
                return count;
            }
        }

        internal int ItemCount
        {
            get { return GalaxyItems.Count; }
        }

        internal int Version
        {
            get
            {
                StringBuilder digits = new StringBuilder();
                foreach (char value in Header[1])
                    if (char.IsDigit(value))
                        digits.Append(value);
                int parsed;
                return int.TryParse(digits.ToString(), out parsed) ? parsed : -1;
            }
        }

        internal static SavContainer Load(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            int offset = 0;
            SavContainer result = new SavContainer();
            result.SourcePath = Path.GetFullPath(path);
            result.OriginalData = data;
            result.Header = new string[8];
            string[] labels = { "signature", "version", "save name", "turn", "money", "player", "race", "post signature" };
            for (int index = 0; index < result.Header.Length; index++)
                result.Header[index] = ReadUtf16Z(data, ref offset, labels[index]);
            if (result.Header[0] != "RSG")
                throw new SavFormatException("Ожидалась сигнатура RSG.");
            if (result.Header[7] != "EZ")
                throw new SavFormatException("Ожидалась сигнатура EZ после заголовка.");

            result.PreviewBlock = ReadSizedBlock(data, ref offset, "превью");
            result.MapBlock = ReadSizedBlock(data, ref offset, "карта");
            result.StoredCrc32 = ReadUInt32(data, ref offset, "CRC");
            result.EncryptionKey = ReadUInt32(data, ref offset, "ключ шифрования");
            result.EncryptedMainBlock = ReadSizedBlock(data, ref offset, "основной блок");
            result.FilmBlock = Take(data, ref offset, data.Length - offset, "фильм");

            byte[] packedMain = Crypt(result.EncryptedMainBlock, result.EncryptionKey);
            result.MainCrcValid = Crc32(packedMain) == result.StoredCrc32;
            if (!result.MainCrcValid)
                throw new SavFormatException("CRC основного блока SAV не совпадает.");
            result.MainPayload = DecompressZl01(packedMain, "основной блок");
            result.Metadata = ReadMetadata(result.MainPayload);
            result.ParseInnerEnvelope();
            result.ParseGalaxyPrefix();
            result.ParseGalaxyDirectory();
            result.ParseGalaxySummary();

            byte[] serialized = result.Serialize();
            if (!EqualBytes(data, serialized))
                throw new SavFormatException("Побайтная обратная сборка контейнера не совпала.");
            return result;
        }

        internal Bitmap PreviewImage()
        {
            return DecodeImage(PreviewBlock, "превью");
        }

        internal Bitmap MapImage()
        {
            Bitmap bitmap = DecodeImage(MapBlock, "карта");
            if (bitmap != null)
                // Unlike the preview screenshot, the tactical-map scanlines
                // need one additional top/bottom reversal. Keep the horizontal
                // axis intact and localize the correction to the map block.
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        internal void WriteCopy(string outputPath, SavMetadata updated)
        {
            WriteCopy(outputPath, updated, null, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages)
        {
            WriteCopy(outputPath, updated, updatedMessages, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages, GalaxyPrefixData updatedGalaxy)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages, GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, null, null, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars,
                updatedPlanets, updatedShips, updatedItems, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary)
        {
            WriteCopy(outputPath, updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems,
                updatedGalaxySummary, null);
        }

        internal void WriteCopy(string outputPath, SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary, IList<ConstellationRecord> updatedConstellations)
        {
            if (File.Exists(outputPath))
                throw new IOException("Файл уже существует; исходные SAV не перезаписываются.");
            if (Version != 166 && Version != 167)
                throw new SavFormatException("Запись проверена только для SAV v166 и v167.");

            bool messagesChanged = updatedMessages != null && !MessagesEqual(updatedMessages);
            bool galaxyChanged = updatedGalaxy != null && !GalaxyPrefix.ContentEquals(updatedGalaxy);
            bool starsChanged = updatedStars != null && !StarsEqual(updatedStars);
            bool planetsChanged = updatedPlanets != null && !PlanetsEqual(updatedPlanets);
            bool shipsChanged = updatedShips != null && !ShipsEqual(updatedShips);
            bool itemsChanged = updatedItems != null && !ItemsEqual(updatedItems);
            bool achievementsChanged = updatedAchievements != null && !AchievementStats.ContentEquals(updatedAchievements);
            bool holesChanged = updatedHoles != null && !HolesEqual(updatedHoles);
            bool asteroidsChanged = updatedAsteroids != null && !AsteroidsEqual(updatedAsteroids);
            bool missilesChanged = updatedMissiles != null && !MissilesEqual(updatedMissiles);
            bool customWeaponsChanged = updatedCustomWeapons != null && !CustomWeaponsEqual(updatedCustomWeapons);
            bool interfaceOverridesChanged = updatedInterfaceOverrides != null &&
                !InterfaceOverridesEqual(updatedInterfaceOverrides);
            bool storedItemsChanged = updatedStoredItems != null && !StoredItemsEqual(updatedStoredItems);
            bool galaxySummaryChanged = updatedGalaxySummary != null &&
                !GalaxySummary.EditableContentEquals(updatedGalaxySummary);
            bool constellationsChanged = updatedConstellations != null &&
                !ConstellationsEqual(updatedConstellations);
            byte[] output;
            if (Metadata.EditableEquals(updated) && !messagesChanged && !galaxyChanged && !starsChanged &&
                !planetsChanged && !shipsChanged && !itemsChanged && !achievementsChanged && !holesChanged &&
                !asteroidsChanged && !missilesChanged && !customWeaponsChanged && !interfaceOverridesChanged &&
                !storedItemsChanged && !galaxySummaryChanged && !constellationsChanged)
            {
                output = OriginalData;
            }
            else
            {
                byte[] patched = BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars,
                    updatedPlanets, updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                    updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems,
                    updatedGalaxySummary, updatedConstellations);

                byte[] packed = CompressZl01(patched);
                uint crc = Crc32(packed);
                byte[] encrypted = Crypt(packed, EncryptionKey);
                output = Serialize(crc, encrypted);
            }

            string directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (FileStream stream = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                stream.Write(output, 0, output.Length);

            try
            {
                SavContainer check = Load(outputPath);
                byte[] expected = Metadata.EditableEquals(updated) && !messagesChanged && !galaxyChanged && !starsChanged &&
                    !planetsChanged && !shipsChanged && !itemsChanged && !achievementsChanged && !holesChanged &&
                    !asteroidsChanged && !missilesChanged && !customWeaponsChanged && !interfaceOverridesChanged &&
                    !storedItemsChanged && !galaxySummaryChanged && !constellationsChanged
                    ? MainPayload : BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars,
                        updatedPlanets, updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                        updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems,
                        updatedGalaxySummary, updatedConstellations);
                if (!EqualBytes(check.MainPayload, expected))
                    throw new SavFormatException("Проверка записанного payload не прошла.");
                if (!EqualBytes(check.PreviewBlock, PreviewBlock) || !EqualBytes(check.MapBlock, MapBlock) ||
                    !EqualBytes(check.FilmBlock, FilmBlock))
                    throw new SavFormatException("При записи изменился непрозрачный блок контейнера.");
            }
            catch
            {
                try { File.Delete(outputPath); } catch { }
                throw;
            }
        }

        private bool MessagesEqual(IList<PlayerMessageRecord> updated)
        {
            if (updated.Count != PlayerMessages.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!PlayerMessages[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool StarsEqual(IList<StarHeaderRecord> updated)
        {
            if (updated.Count != GalaxyStars.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyStars[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool ConstellationsEqual(IList<ConstellationRecord> updated)
        {
            if (updated.Count != GalaxyConstellations.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyConstellations[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool StarTailsEqual(IList<StarHeaderRecord> updated)
        {
            if (updated.Count != GalaxyStars.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyStars[index].TailContentEquals(updated[index])) return false;
            return true;
        }

        private bool StarDropItemsEqual(IList<StarHeaderRecord> updated)
        {
            if (updated.Count != GalaxyStars.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyStars[index].DropItemsContentEquals(updated[index])) return false;
            return true;
        }

        private bool StarSpaceItemsEqual(IList<StarHeaderRecord> updated)
        {
            if (updated.Count != GalaxyStars.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyStars[index].SpaceItemsContentEquals(updated[index])) return false;
            return true;
        }

        private bool PlanetsEqual(IList<PlanetHeaderRecord> updated)
        {
            if (updated.Count != GalaxyPlanets.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyPlanets[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool ShipsEqual(IList<ShipHeaderRecord> updated)
        {
            if (updated.Count != GalaxyShips.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyShips[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool ItemsEqual(IList<ItemHeaderRecord> updated)
        {
            if (updated.Count != GalaxyItems.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyItems[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool HolesEqual(IList<HoleRecord> updated)
        {
            if (updated.Count != GalaxyHoles.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyHoles[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool AsteroidsEqual(IList<AsteroidRecord> updated)
        {
            if (updated.Count != GalaxyAsteroids.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyAsteroids[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool MissilesEqual(IList<MissileRecord> updated)
        {
            if (updated.Count != GalaxyMissiles.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxyMissiles[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool CustomWeaponsEqual(IList<CustomWeaponInfoRecord> updated)
        {
            if (updated.Count != CustomWeaponInfos.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!CustomWeaponInfos[index].ContentEquals(updated[index])) return false;
            return true;
        }

        internal CustomWeaponDeleteResult DeleteCustomWeaponCascade(string systemName,
            List<CustomWeaponInfoRecord> updatedWeapons, List<StarHeaderRecord> updatedStars,
            List<PlanetHeaderRecord> updatedPlanets, List<ShipHeaderRecord> updatedShips,
            List<ItemHeaderRecord> updatedItems, List<MissileRecord> updatedMissiles,
            List<StoredItemRecord> updatedStoredItems, GalaxySummaryData updatedGalaxySummary)
        {
            if (string.IsNullOrEmpty(systemName) || updatedWeapons == null ||
                updatedStars == null || updatedPlanets == null || updatedShips == null ||
                updatedItems == null || updatedMissiles == null || updatedStoredItems == null ||
                updatedGalaxySummary == null)
                throw new InvalidOperationException("Каскад TCustomWeapon получил неполную модель SAV.");
            if (updatedStars.Count != GalaxyStars.Count || updatedPlanets.Count != GalaxyPlanets.Count ||
                updatedShips.Count != GalaxyShips.Count || updatedItems.Count != GalaxyItems.Count)
                throw new InvalidOperationException(
                    "Каскад TCustomWeapon требует исходный порядок TStar/TPlanet/TShip/TItem.");

            int descriptorIndex = -1;
            for (int index = 0; index < updatedWeapons.Count; index++)
                if (string.Equals(updatedWeapons[index].SystemName, systemName,
                    StringComparison.Ordinal))
                {
                    descriptorIndex = index;
                    break;
                }
            if (descriptorIndex < 0)
                throw new InvalidOperationException("TCustomWeaponInfo «" + systemName + "» не найден.");

            CustomWeaponDeleteResult result = new CustomWeaponDeleteResult();
            foreach (ItemHeaderRecord item in updatedItems)
                if (item != null && item.Type == 68 && string.Equals(item.CustomWeaponName,
                    systemName, StringComparison.Ordinal))
                {
                    result.RemovedItemStarts.Add(item.Start);
                    result.RemovedItemIds.Add(item.ObjectId);
                }

            HashSet<int> locatedOwnerStarts = CollectRemovedCustomWeaponOwnerStarts(updatedStars,
                updatedPlanets, updatedShips, updatedItems, updatedStoredItems,
                updatedGalaxySummary.ScriptShopSlots, result.RemovedItemStarts);
            if (locatedOwnerStarts.Count != result.RemovedItemStarts.Count)
                throw new InvalidOperationException(
                    "TCustomWeapon: не для каждого удаляемого предмета локализован владелец.");

            foreach (StarHeaderRecord star in updatedStars)
            {
                result.RemovedOwnerRecords += RemoveCustomWeaponShipItems(
                    star.SpaceItems, result.RemovedItemStarts);
                result.RemovedOwnerRecords += RemoveCustomWeaponStarDropItems(
                    star.DropItems, result.RemovedItemStarts);
            }
            foreach (PlanetHeaderRecord planet in updatedPlanets)
            {
                result.RemovedOwnerRecords += RemoveCustomWeaponShipItems(
                    planet.EquipmentShopItems, result.RemovedItemStarts);
                planet.EquipmentShopCount = checked((ushort)planet.EquipmentShopItems.Count);
                result.RemovedOwnerRecords += RemoveCustomWeaponGoneItems(
                    planet.GoneItems, result.RemovedItemStarts);
                planet.GoneItemCount = checked((ushort)planet.GoneItems.Count);
            }
            foreach (ShipHeaderRecord ship in updatedShips)
                result.RemovedOwnerRecords += RemoveCustomWeaponFromShip(ship,
                    result.RemovedItemStarts, result.RemovedItemIds);
            foreach (ItemHeaderRecord item in updatedItems)
                if (item != null && item.NestedTranclucator != null)
                    result.RemovedOwnerRecords += RemoveCustomWeaponFromShip(
                        item.NestedTranclucator, result.RemovedItemStarts, result.RemovedItemIds);

            for (int index = updatedStoredItems.Count - 1; index >= 0; index--)
                if (result.RemovedItemStarts.Contains(updatedStoredItems[index].ItemStart))
                {
                    updatedStoredItems.RemoveAt(index);
                    result.RemovedOwnerRecords++;
                }
            result.RemovedOwnerRecords += RemoveCustomWeaponScriptShopSlots(
                updatedGalaxySummary.ScriptShopSlots, result.RemovedItemStarts);

            HashSet<int> ownedRemovedStarts = CollectRemovedCustomWeaponOwnerStarts(updatedStars,
                updatedPlanets, updatedShips, updatedItems, updatedStoredItems,
                updatedGalaxySummary.ScriptShopSlots, result.RemovedItemStarts);
            if (ownedRemovedStarts.Count != 0)
                throw new InvalidOperationException(
                    "TCustomWeapon: после каскада остались владельцы удаляемых предметов.");

            for (int index = updatedMissiles.Count - 1; index >= 0; index--)
            {
                MissileRecord missile = updatedMissiles[index];
                if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                    systemName, StringComparison.Ordinal))
                {
                    result.RemovedMissileIds.Add(missile.ObjectId);
                    updatedMissiles.RemoveAt(index);
                }
            }
            UnlinkRemovedCustomWeaponTargets(updatedShips, updatedItems, updatedMissiles,
                result.RemovedItemIds, result.RemovedMissileIds);
            updatedWeapons.RemoveAt(descriptorIndex);
            return result;
        }

        internal int DeleteGalaxyItemsCascade(ICollection<int> selectedItemStarts,
            List<StarHeaderRecord> updatedStars, List<ShipHeaderRecord> updatedShips,
            List<ItemHeaderRecord> updatedItems, List<MissileRecord> updatedMissiles)
        {
            if (selectedItemStarts == null || updatedStars == null || updatedShips == null ||
                updatedItems == null || updatedMissiles == null)
                throw new InvalidOperationException("Каскад TItem получил неполную модель SAV.");
            if (updatedStars.Count != GalaxyStars.Count || updatedShips.Count != GalaxyShips.Count ||
                updatedItems.Count != GalaxyItems.Count)
                throw new InvalidOperationException(
                    "Удаление TItem требует исходный порядок TStar/TShip/TItem.");
            if (selectedItemStarts.Count == 0) return 0;

            Dictionary<int, ItemHeaderRecord> itemsByStart =
                new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in updatedItems)
                if (item != null) itemsByStart[item.Start] = item;
            HashSet<int> removedStarts = new HashSet<int>();
            HashSet<uint> removedIds = new HashSet<uint>();
            foreach (int start in selectedItemStarts)
            {
                ItemHeaderRecord item;
                if (!itemsByStart.TryGetValue(start, out item))
                    throw new InvalidOperationException(
                        "Удаляемый TItem @ 0x" + start.ToString("X") + " не найден.");
                removedStarts.Add(start);
                removedIds.Add(item.ObjectId);
            }

            int locatedOwners = 0;
            foreach (StarHeaderRecord star in updatedStars)
                if (star != null && star.SpaceItems != null)
                    foreach (ShipItemListEntry value in star.SpaceItems)
                        if (value != null && removedStarts.Contains(value.ItemStart)) locatedOwners++;
            if (locatedOwners != removedStarts.Count)
                throw new InvalidOperationException(
                    "TItem можно удалить из галактики только при единственном доказанном владельце TStar.ItemsInSpace.");
            foreach (StarHeaderRecord star in updatedStars)
                if (star != null && star.SpaceItems != null)
                    star.SpaceItems.RemoveAll(delegate(ShipItemListEntry value)
                        { return value != null && removedStarts.Contains(value.ItemStart); });

            foreach (ShipHeaderRecord ship in updatedShips)
                ClearRemovedGalaxyItemReferencesFromShip(ship, removedIds);
            HashSet<uint> noMissiles = new HashSet<uint>();
            UnlinkRemovedCustomWeaponTargets(updatedShips, updatedItems, updatedMissiles,
                removedIds, noMissiles);
            return removedStarts.Count;
        }

        internal int DeleteGalaxyShipsCascade(ICollection<int> selectedShipStarts,
            List<StarHeaderRecord> updatedStars, List<PlanetHeaderRecord> updatedPlanets,
            List<ShipHeaderRecord> updatedShips, List<ItemHeaderRecord> updatedItems,
            List<MissileRecord> updatedMissiles, GalaxySummaryData updatedSummary)
        {
            if (selectedShipStarts == null || updatedStars == null || updatedPlanets == null ||
                updatedShips == null || updatedItems == null || updatedMissiles == null ||
                updatedSummary == null)
                throw new InvalidOperationException("Каскад TShip получил неполную модель SAV.");
            if (updatedStars.Count != GalaxyStars.Count ||
                updatedPlanets.Count != GalaxyPlanets.Count ||
                updatedShips.Count != GalaxyShips.Count || updatedItems.Count != GalaxyItems.Count)
                throw new InvalidOperationException(
                    "Удаление TShip требует исходный порядок TStar/TPlanet/TShip/TItem.");
            if (selectedShipStarts.Count == 0) return 0;

            Dictionary<int, ShipHeaderRecord> shipsByStart =
                new Dictionary<int, ShipHeaderRecord>();
            foreach (ShipHeaderRecord ship in updatedShips)
                if (ship != null) shipsByStart[ship.Start] = ship;
            HashSet<int> removedStarts = new HashSet<int>();
            HashSet<uint> removedIds = new HashSet<uint>();
            foreach (int start in selectedShipStarts)
            {
                ShipHeaderRecord ship;
                if (!shipsByStart.TryGetValue(start, out ship))
                    throw new InvalidOperationException(
                        "Удаляемый TShip @ 0x" + start.ToString("X") + " не найден.");
                if (ship.IsPlayer || ship.ObjectId == updatedSummary.PlayerObjectId)
                    throw new InvalidOperationException("Нельзя удалять корабль игрока.");
                if (ship.ObjectId == updatedSummary.BlazerObjectId ||
                    ship.ObjectId == updatedSummary.KellerObjectId ||
                    ship.ObjectId == updatedSummary.TerronObjectId)
                    throw new InvalidOperationException("Нельзя удалять корабли боссов.");
                removedStarts.Add(start);
                removedIds.Add(ship.ObjectId);
            }

            List<int> removedRangerIndexes = new List<int>();
            if (updatedSummary.RangerObjectIds != null)
                for (int index = 0; index < updatedSummary.RangerObjectIds.Length; index++)
                    if (removedIds.Contains(updatedSummary.RangerObjectIds[index]))
                        removedRangerIndexes.Add(index);
            bool removesRanger = removedRangerIndexes.Count != 0;
            if (removesRanger)
            {
                foreach (StarHeaderRecord star in updatedStars)
                {
                    if (!star.HasExactSpaceShipList)
                        throw new InvalidOperationException(
                            "TRanger нельзя удалить при неоднозначном модовом TStar.Ships.");
                    foreach (StarShipRecord record in star.SpaceShips)
                        if (record.OpaqueTail)
                            throw new InvalidOperationException(
                                "TRanger нельзя удалить при неизвестном модовом хвосте TShip.");
                }
                foreach (PlanetHeaderRecord planet in updatedPlanets)
                {
                    if (planet.RelationToRangers == null ||
                        planet.RelationToRangers.Length != updatedSummary.RangerObjectIds.Length)
                        throw new InvalidOperationException(
                            "TRanger: индекс TPlanet.RelationToRangers не совпадает с TGalaxy.Rangers.");
                    planet.RelationToRangers = RemoveByteIndexes(
                        planet.RelationToRangers, removedRangerIndexes);
                    planet.RelationCount = checked((ushort)planet.RelationToRangers.Length);
                }
                foreach (ShipHeaderRecord ship in updatedShips)
                    if (ship.RelationToRangers != null)
                    {
                        ship.RelationToRangers = RemoveByteIndexesIfPresent(
                            ship.RelationToRangers, removedRangerIndexes);
                        ship.RelationCount = checked((ushort)ship.RelationToRangers.Length);
                    }
                updatedSummary.RangerObjectIds = RemoveUIntIndexes(
                    updatedSummary.RangerObjectIds, removedRangerIndexes);
                updatedSummary.RangerCount = updatedSummary.RangerObjectIds.Length;
                if (updatedSummary.EminentRangerObjectIds != null)
                    for (int index = 0; index < updatedSummary.EminentRangerObjectIds.Length; index++)
                        if (removedIds.Contains(updatedSummary.EminentRangerObjectIds[index]))
                            updatedSummary.EminentRangerObjectIds[index] = 0;
            }

            int locatedOwners = 0;
            foreach (StarHeaderRecord star in updatedStars)
                if (star.SpaceShips != null)
                    foreach (StarShipRecord record in star.SpaceShips)
                        if (record != null && removedStarts.Contains(record.ShipStart))
                        {
                            if (record.OpaqueTail)
                                throw new InvalidOperationException(
                                    "TShip содержит неизвестный модовый хвост и доступен только для чтения.");
                            locatedOwners++;
                        }
            foreach (PlanetHeaderRecord planet in updatedPlanets)
                if (planet.Warriors != null)
                    foreach (PlanetWarriorRecord record in planet.Warriors)
                        if (record != null && removedStarts.Contains(record.ShipStart)) locatedOwners++;
            if (locatedOwners != removedStarts.Count)
                throw new InvalidOperationException(
                    "TShip можно удалить только при единственном доказанном владельце TStar.Ships/TPlanet.Warriors.");

            foreach (StarHeaderRecord star in updatedStars)
                if (star.SpaceShips != null)
                    star.SpaceShips.RemoveAll(delegate(StarShipRecord value)
                        { return value != null && removedStarts.Contains(value.ShipStart); });
            foreach (PlanetHeaderRecord planet in updatedPlanets)
                if (planet.Warriors != null)
                {
                    planet.Warriors.RemoveAll(delegate(PlanetWarriorRecord value)
                        { return value != null && removedStarts.Contains(value.ShipStart); });
                    planet.WarriorCount = checked((ushort)planet.Warriors.Count);
                }
            foreach (StarHeaderRecord star in updatedStars)
                if (star.DropItems != null)
                    foreach (StarDropItemRecord record in star.DropItems)
                        if (record != null && removedIds.Contains(record.ShipObjectId))
                            record.ShipObjectId = 0;
            updatedShips.RemoveAll(delegate(ShipHeaderRecord value)
                { return value != null && removedStarts.Contains(value.Start); });
            foreach (ShipHeaderRecord ship in updatedShips)
                ClearRemovedGalaxyShipReferencesFromShip(ship, removedIds);
            foreach (MissileRecord missile in updatedMissiles)
            {
                if (removedIds.Contains(missile.ShipId)) missile.ShipId = 0;
                if (missile.TargetType == 1 && removedIds.Contains(missile.TargetId))
                { missile.TargetType = 0; missile.TargetId = 0; }
                if (missile.TargetLostType == 1 && removedIds.Contains(missile.TargetLostId))
                { missile.TargetLostType = 0; missile.TargetLostId = 0; }
            }
            ClearRemovedGalaxyShipWeaponTargets(updatedItems, removedIds);
            if (removedIds.Contains(updatedSummary.AutoBattleShipObjectId))
                updatedSummary.AutoBattleShipObjectId = 0;
            foreach (ScriptRecord script in updatedSummary.ActiveScripts)
                if (script != null && script.ShipBindings != null)
                    script.ShipBindings.RemoveAll(delegate(ScriptShipRecord value)
                        { return value != null && removedIds.Contains(value.ShipObjectId); });
            foreach (WarOperationRecord operation in updatedSummary.WarOperations)
                if (operation != null)
                {
                    if (operation.ShipObjectIds != null)
                        operation.ShipObjectIds.RemoveAll(delegate(uint value)
                            { return removedIds.Contains(value); });
                    if (operation.Orders != null)
                        foreach (WarOperationOrderRecord order in operation.Orders)
                            if (order != null && removedIds.Contains(order.ObjectId))
                            { order.Type = 0; order.ObjectId = 0; }
                }
            return removedStarts.Count;
        }

        private static byte[] RemoveByteIndexes(byte[] values, IList<int> indexes)
        {
            HashSet<int> removed = new HashSet<int>(indexes);
            byte[] result = new byte[values.Length - removed.Count];
            int target = 0;
            for (int index = 0; index < values.Length; index++)
                if (!removed.Contains(index)) result[target++] = values[index];
            return result;
        }

        private static byte[] RemoveByteIndexesIfPresent(byte[] values, IList<int> indexes)
        {
            List<int> present = new List<int>();
            foreach (int index in indexes) if (index < values.Length) present.Add(index);
            return RemoveByteIndexes(values, present);
        }

        private static uint[] RemoveUIntIndexes(uint[] values, IList<int> indexes)
        {
            HashSet<int> removed = new HashSet<int>(indexes);
            uint[] result = new uint[values.Length - removed.Count];
            int target = 0;
            for (int index = 0; index < values.Length; index++)
                if (!removed.Contains(index)) result[target++] = values[index];
            return result;
        }

        private static void ClearRemovedGalaxyShipReferencesFromShip(ShipHeaderRecord ship,
            HashSet<uint> removedShipIds)
        {
            if (ship == null) return;
            if (removedShipIds.Contains(ship.CurrentShipId)) ship.CurrentShipId = 0;
                if (removedShipIds.Contains(ship.OrderObjectId) ||
                    (ship.OrderType == 2 && (ship.OrderObjectId & 0x80000000U) != 0 &&
                     removedShipIds.Contains(ship.OrderObjectId & 0x7FFFFFFFU)))
                { ship.OrderType = 0; ship.OrderObjectId = 0; }
            if (removedShipIds.Contains(ship.PlayerBridgeCurrentShipId))
                ship.PlayerBridgeCurrentShipId = 0;
            if (ship.PlayerBridgeRuins != null)
                ClearRemovedGalaxyShipReferencesFromShip(ship.PlayerBridgeRuins, removedShipIds);
        }

        private static void ClearRemovedGalaxyShipWeaponTargets(
            IList<ItemHeaderRecord> items, HashSet<uint> removedShipIds)
        {
            foreach (ItemHeaderRecord item in items)
            {
                if (item == null || item.DerivedFields == null) continue;
                ItemDerivedField targetType = null, target = null;
                foreach (ItemDerivedField field in item.DerivedFields)
                {
                    if (field.ControlName == "edWeaponTargetType") targetType = field;
                    else if (field.ControlName == "cbWeaponTarget") target = field;
                }
                if (targetType != null && target != null && targetType.IntegerValue == 1 &&
                    removedShipIds.Contains(checked((uint)target.IntegerValue)))
                { targetType.IntegerValue = 0; target.IntegerValue = 0; }
            }
        }

        private static void ClearRemovedGalaxyItemReferencesFromShip(ShipHeaderRecord ship,
            HashSet<uint> removedItemIds)
        {
            if (ship == null) return;
            if (ship.TakeItemReferenceIds != null)
                ship.TakeItemReferenceIds.RemoveAll(delegate(uint value)
                    { return removedItemIds.Contains(value); });
            if (ship.RecentlyDroppedItemIds != null)
                ship.RecentlyDroppedItemIds.RemoveAll(delegate(uint value)
                    { return removedItemIds.Contains(value); });
            ClearRemovedItemSetReferences(ship.PlayerEquipmentSetItems, removedItemIds);
            ClearRemovedItemSetReferences(ship.PlayerArtefactSetItems, removedItemIds);
            if (ship.PlayerBridgeRuins != null)
                ClearRemovedGalaxyItemReferencesFromShip(ship.PlayerBridgeRuins, removedItemIds);
        }

        private static int RemoveCustomWeaponFromShip(ShipHeaderRecord ship,
            HashSet<int> itemStarts, HashSet<uint> itemIds)
        {
            if (ship == null) return 0;
            int removed = 0;
            removed += RemoveCustomWeaponShipItems(ship.EquipmentItems, itemStarts);
            ship.EquipmentItemCount = checked((ushort)ship.EquipmentItems.Count);
            removed += RemoveCustomWeaponShipItems(ship.ArtefactItems, itemStarts);
            removed += RemoveCustomWeaponShipItems(ship.DropListItems, itemStarts);
            removed += RemoveCustomWeaponShipItems(ship.RuinsEquipmentItems, itemStarts);
            ship.RuinsEquipmentItemCount = checked((ushort)ship.RuinsEquipmentItems.Count);
            if (ship.PlayerStorageItems != null)
                for (int index = ship.PlayerStorageItems.Count - 1; index >= 0; index--)
                    if (itemStarts.Contains(ship.PlayerStorageItems[index].ItemStart))
                    {
                        ship.PlayerStorageItems.RemoveAt(index);
                        removed++;
                    }
            ship.PlayerObjectStateCount = ship.PlayerStorageItems == null ? 0 :
                ship.PlayerStorageItems.Count;
            if (ship.TakeItemReferenceIds != null)
                ship.TakeItemReferenceIds.RemoveAll(delegate(uint value) { return itemIds.Contains(value); });
            if (ship.RecentlyDroppedItemIds != null)
                ship.RecentlyDroppedItemIds.RemoveAll(delegate(uint value) { return itemIds.Contains(value); });
            ClearRemovedItemSetReferences(ship.PlayerEquipmentSetItems, itemIds);
            ClearRemovedItemSetReferences(ship.PlayerArtefactSetItems, itemIds);
            if (ship.PlayerBridgeRuins != null)
                removed += RemoveCustomWeaponFromShip(ship.PlayerBridgeRuins, itemStarts, itemIds);
            return removed;
        }

        private static int RemoveCustomWeaponShipItems(List<ShipItemListEntry> records,
            HashSet<int> itemStarts)
        {
            if (records == null) return 0;
            int before = records.Count;
            records.RemoveAll(delegate(ShipItemListEntry value)
                { return value != null && itemStarts.Contains(value.ItemStart); });
            return before - records.Count;
        }

        private static int RemoveCustomWeaponGoneItems(List<PlanetGoneItemRecord> records,
            HashSet<int> itemStarts)
        {
            if (records == null) return 0;
            int before = records.Count;
            records.RemoveAll(delegate(PlanetGoneItemRecord value)
                { return value != null && itemStarts.Contains(value.ItemStart); });
            return before - records.Count;
        }

        private static int RemoveCustomWeaponStarDropItems(List<StarDropItemRecord> records,
            HashSet<int> itemStarts)
        {
            if (records == null) return 0;
            int before = records.Count;
            records.RemoveAll(delegate(StarDropItemRecord value)
                { return value != null && itemStarts.Contains(value.ItemStart); });
            return before - records.Count;
        }

        private static int RemoveCustomWeaponScriptShopSlots(List<ScriptShopSlotRecord> records,
            HashSet<int> itemStarts)
        {
            if (records == null) return 0;
            int before = records.Count;
            records.RemoveAll(delegate(ScriptShopSlotRecord value)
                { return value != null && value.HasEquipment &&
                    itemStarts.Contains(value.ItemStart); });
            return before - records.Count;
        }

        private static void ClearRemovedItemSetReferences(uint[,] values, HashSet<uint> itemIds)
        {
            if (values == null) return;
            for (int row = 0; row < values.GetLength(0); row++)
                for (int column = 0; column < values.GetLength(1); column++)
                    if (itemIds.Contains(values[row, column])) values[row, column] = 0;
        }

        private static HashSet<int> CollectRemovedCustomWeaponOwnerStarts(
            IList<StarHeaderRecord> stars, IList<PlanetHeaderRecord> planets,
            IList<ShipHeaderRecord> ships, IList<ItemHeaderRecord> items,
            IList<StoredItemRecord> storedItems, IList<ScriptShopSlotRecord> scriptShopSlots,
            HashSet<int> removedStarts)
        {
            HashSet<int> result = new HashSet<int>();
            foreach (StarHeaderRecord star in stars)
            {
                CollectShipItemStarts(star.SpaceItems, removedStarts, result);
                CollectShipItemStarts(star.DropItems, removedStarts, result);
            }
            foreach (PlanetHeaderRecord planet in planets)
            {
                CollectShipItemStarts(planet.EquipmentShopItems, removedStarts, result);
                if (planet.GoneItems != null)
                    foreach (PlanetGoneItemRecord record in planet.GoneItems)
                        if (record != null && removedStarts.Contains(record.ItemStart))
                            result.Add(record.ItemStart);
            }
            foreach (ShipHeaderRecord ship in ships)
                CollectShipCustomWeaponOwnerStarts(ship, removedStarts, result);
            foreach (ItemHeaderRecord item in items)
                if (item != null && item.NestedTranclucator != null)
                    CollectShipCustomWeaponOwnerStarts(item.NestedTranclucator,
                        removedStarts, result);
            foreach (StoredItemRecord record in storedItems)
                if (record != null && removedStarts.Contains(record.ItemStart)) result.Add(record.ItemStart);
            foreach (ScriptShopSlotRecord record in scriptShopSlots)
                if (record != null && record.HasEquipment &&
                    removedStarts.Contains(record.ItemStart)) result.Add(record.ItemStart);
            return result;
        }

        private static void CollectShipCustomWeaponOwnerStarts(ShipHeaderRecord ship,
            HashSet<int> removedStarts, HashSet<int> result)
        {
            if (ship == null) return;
            CollectShipItemStarts(ship.EquipmentItems, removedStarts, result);
            CollectShipItemStarts(ship.ArtefactItems, removedStarts, result);
            CollectShipItemStarts(ship.DropListItems, removedStarts, result);
            CollectShipItemStarts(ship.RuinsEquipmentItems, removedStarts, result);
            if (ship.PlayerStorageItems != null)
                foreach (PlayerStorageItemRecord record in ship.PlayerStorageItems)
                    if (record != null && removedStarts.Contains(record.ItemStart))
                        result.Add(record.ItemStart);
            if (ship.PlayerBridgeRuins != null)
                CollectShipCustomWeaponOwnerStarts(ship.PlayerBridgeRuins, removedStarts, result);
        }

        private static void CollectShipItemStarts<T>(IList<T> records,
            HashSet<int> removedStarts, HashSet<int> result) where T : class
        {
            if (records == null) return;
            foreach (T value in records)
            {
                ShipItemListEntry shipItem = value as ShipItemListEntry;
                StarDropItemRecord starItem = value as StarDropItemRecord;
                int start = shipItem != null ? shipItem.ItemStart :
                    starItem != null ? starItem.ItemStart : -1;
                if (start >= 0 && removedStarts.Contains(start)) result.Add(start);
            }
        }

        private static void UnlinkRemovedCustomWeaponTargets(IList<ShipHeaderRecord> ships,
            IList<ItemHeaderRecord> items, IList<MissileRecord> missiles,
            HashSet<uint> removedItemIds, HashSet<uint> removedMissileIds)
        {
            foreach (MissileRecord missile in missiles)
            {
                if (missile.TargetType == 2 && removedItemIds.Contains(missile.TargetId) ||
                    missile.TargetType == 4 && removedMissileIds.Contains(missile.TargetId))
                { missile.TargetType = 0; missile.TargetId = 0; }
                if (missile.TargetLostType == 2 && removedItemIds.Contains(missile.TargetLostId) ||
                    missile.TargetLostType == 4 && removedMissileIds.Contains(missile.TargetLostId))
                { missile.TargetLostType = 0; missile.TargetLostId = 0; }
            }
            foreach (ShipHeaderRecord ship in ships)
                UnlinkRemovedCustomWeaponShipTarget(ship, removedItemIds, removedMissileIds);
            foreach (ItemHeaderRecord item in items)
            {
                if (item == null || item.DerivedFields == null) continue;
                ItemDerivedField targetType = null, target = null;
                foreach (ItemDerivedField field in item.DerivedFields)
                {
                    if (field.ControlName == "edWeaponTargetType") targetType = field;
                    else if (field.ControlName == "cbWeaponTarget") target = field;
                }
                if (targetType == null || target == null) continue;
                byte kind = checked((byte)targetType.IntegerValue);
                uint objectId = checked((uint)target.IntegerValue);
                if (kind == 2 && removedItemIds.Contains(objectId) ||
                    kind == 4 && removedMissileIds.Contains(objectId))
                { targetType.IntegerValue = 0; target.IntegerValue = 0; }
            }
        }

        private static void UnlinkRemovedCustomWeaponShipTarget(ShipHeaderRecord ship,
            HashSet<uint> removedItemIds, HashSet<uint> removedMissileIds)
        {
            if (ship == null) return;
            if (removedItemIds.Contains(ship.OrderObjectId) ||
                removedMissileIds.Contains(ship.OrderObjectId)) ship.OrderObjectId = 0;
            if (ship.PlayerBridgeRuins != null)
                UnlinkRemovedCustomWeaponShipTarget(ship.PlayerBridgeRuins,
                    removedItemIds, removedMissileIds);
        }

        private bool InterfaceOverridesEqual(IList<InterfaceOverrideRecord> updated)
        {
            if (updated.Count != GalaxySummary.InterfaceOverrides.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!GalaxySummary.InterfaceOverrides[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private bool StoredItemsEqual(IList<StoredItemRecord> updated)
        {
            if (updated.Count != StoredItems.Count) return false;
            for (int index = 0; index < updated.Count; index++)
                if (!StoredItems[index].ContentEquals(updated[index])) return false;
            return true;
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages, GalaxyPrefixData updatedGalaxy)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages, GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, null, null, null, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary)
        {
            return BuildPatchedPayload(updated, updatedMessages, updatedGalaxy, updatedStars, updatedPlanets,
                updatedShips, updatedItems, updatedAchievements, updatedHoles, updatedAsteroids,
                updatedMissiles, updatedCustomWeapons, updatedInterfaceOverrides, updatedStoredItems,
                updatedGalaxySummary, null);
        }

        private byte[] BuildPatchedPayload(SavMetadata updated, IList<PlayerMessageRecord> updatedMessages,
            GalaxyPrefixData updatedGalaxy, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<CustomWeaponInfoRecord> updatedCustomWeapons,
            IList<InterfaceOverrideRecord> updatedInterfaceOverrides, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary, IList<ConstellationRecord> updatedConstellations)
        {
            IList<PlayerMessageRecord> messages = updatedMessages ?? PlayerMessages;
            if ((ulong)messages.Count > uint.MaxValue)
                throw new InvalidOperationException("Слишком много сообщений TMessagePlayer.");

            byte[] basePayload = PrepareConstellationPayload(updatedConstellations);
            byte[] metadata = new byte[MetadataSize];
            Buffer.BlockCopy(MainPayload, 0, metadata, 0, MetadataSize);
            WriteInt32(metadata, 4, updated.CameraX);
            WriteInt32(metadata, 8, updated.CameraY);
            metadata[12] = updated.ShowPanel ? (byte)1 : (byte)0;
            metadata[14] = updated.ViewFollow ? (byte)1 : (byte)0;
            metadata[19] = updated.CalcHeader ? (byte)1 : (byte)0;
            WriteUInt32(metadata, 20, updated.Tips);
            WriteUInt32(metadata, 28, checked((uint)messages.Count));

            using (MemoryStream stream = new MemoryStream(MainPayload.Length + 1024))
            {
                stream.Write(metadata, 0, metadata.Length);
                foreach (PlayerMessageRecord message in messages)
                {
                    WriteUtf16Z(stream, message.Text ?? string.Empty);
                    stream.WriteByte(message.MessageType);
                    WriteInt32(stream, message.Raw18);
                    WriteInt32(stream, message.Raw1C);
                    WriteUtf16Z(stream, message.FormattedText ?? string.Empty);
                    WriteBoolean(stream, message.RawBool);
                    if (message.RawU32 == null || message.RawU32.Length != 6)
                        throw new InvalidOperationException("Сообщение содержит неверное число объектных ссылок.");
                    for (int index = 0; index < message.RawU32.Length; index++)
                        WriteUInt32(stream, message.RawU32[index]);
                    WriteBoolean(stream, message.Flag40);
                    WriteBoolean(stream, message.Flag41);
                    if (Version > 108)
                        WriteUtf16Z(stream, message.LateText ?? string.Empty);
                }
                WriteUInt16(stream, checked((ushort)PlayerHoldUnits.Count));
                foreach (PlayerHoldRecord unit in PlayerHoldUnits)
                {
                    stream.WriteByte(unit.UnitType);
                    stream.WriteByte(unit.Goods);
                    WriteUInt32(stream, unit.ObjectId);
                }
                if (updatedGalaxy == null || GalaxyPrefix.ContentEquals(updatedGalaxy))
                {
                    stream.Write(basePayload, GalaxyOffset, basePayload.Length - GalaxyOffset);
                }
                else
                {
                    if (updatedGalaxy.ReservedZero != 0)
                        throw new InvalidOperationException("Зарезервированное поле TGalaxy должно быть равно нулю.");
                    int effectiveCustomWeaponCount = updatedCustomWeapons == null
                        ? GalaxyPrefix.CustomModWeaponCount : updatedCustomWeapons.Count;
                    if (updatedGalaxy.CustomModWeaponCount != effectiveCustomWeaponCount)
                        throw new InvalidOperationException(
                            "TGalaxy.CustomModWeaponCount не совпадает со списком TCustomWeaponInfo.");
                    WriteUtf16Z(stream, updatedGalaxy.UsedMods ?? string.Empty);
                    WriteInt32(stream, updatedGalaxy.RandomSeed);
                    WriteUInt32(stream, updatedGalaxy.RandomOut);
                    WriteInt32(stream, updatedGalaxy.RangersAverageCapital);
                    WriteInt32(stream, updatedGalaxy.RangersMaxCapital);
                    WriteSingle(stream, updatedGalaxy.RangersAverageStrength);
                    WriteSingle(stream, updatedGalaxy.RangersMaxStrength);
                    WriteBoolean(stream, updatedGalaxy.Crack);
                    WriteBoolean(stream, updatedGalaxy.Cheat);
                    WriteInt32(stream, updatedGalaxy.ReservedZero);
                    WriteInt32(stream, updatedGalaxy.CheatPoints);
                    WriteInt32(stream, updatedGalaxy.SaveCount);
                    WriteInt32(stream, updatedGalaxy.LoadCount);
                    WriteUInt16(stream, checked((ushort)updatedGalaxy.CustomModWeaponCount));
                    stream.Write(basePayload, GalaxyPrefix.End, basePayload.Length - GalaxyPrefix.End);
                }
                byte[] prefixPatched = stream.ToArray();
                if (updatedCustomWeapons != null && !CustomWeaponsEqual(updatedCustomWeapons))
                    ValidateRemovedCustomWeaponReferences(updatedCustomWeapons,
                        updatedStars ?? GalaxyStars, updatedPlanets ?? GalaxyPlanets,
                        updatedShips ?? GalaxyShips, updatedItems ?? GalaxyItems,
                        updatedMissiles ?? GalaxyMissiles, updatedStoredItems ?? StoredItems,
                        updatedGalaxySummary ?? GalaxySummary);
                byte[] customWeaponPatched = updatedCustomWeapons == null || CustomWeaponsEqual(updatedCustomWeapons)
                    ? prefixPatched : RewriteCustomWeaponInfos(prefixPatched, updatedCustomWeapons);
                byte[] starPatched = updatedStars == null || StarsEqual(updatedStars)
                    ? customWeaponPatched : RewriteStarHeaders(customWeaponPatched, updatedStars);
                if ((updatedPlanets == null || PlanetsEqual(updatedPlanets)) &&
                    (updatedStars == null || StarTailsEqual(updatedStars)) &&
                    (updatedStars == null || StarSpaceItemsEqual(updatedStars)) &&
                    (updatedStars == null || StarDropItemsEqual(updatedStars)) &&
                    (updatedShips == null || ShipsEqual(updatedShips)) &&
                    (updatedItems == null || ItemsEqual(updatedItems)) &&
                    (updatedAchievements == null || AchievementStats.ContentEquals(updatedAchievements)) &&
                    (updatedHoles == null || HolesEqual(updatedHoles)) &&
                    (updatedAsteroids == null || AsteroidsEqual(updatedAsteroids)) &&
                    (updatedMissiles == null || MissilesEqual(updatedMissiles)) &&
                    (updatedInterfaceOverrides == null || InterfaceOverridesEqual(updatedInterfaceOverrides)) &&
                    (updatedStoredItems == null || StoredItemsEqual(updatedStoredItems)) &&
                    (updatedGalaxySummary == null || GalaxySummary.EditableContentEquals(updatedGalaxySummary)))
                    return starPatched;
                return RewriteKnownObjectHeaders(starPatched, updatedStars, updatedPlanets, updatedShips,
                    updatedItems, updatedAchievements, updatedHoles, updatedAsteroids, updatedMissiles,
                    updatedInterfaceOverrides, updatedStoredItems, updatedGalaxySummary);
            }
        }

        private byte[] PrepareConstellationPayload(IList<ConstellationRecord> updatedConstellations)
        {
            if (updatedConstellations == null || ConstellationsEqual(updatedConstellations)) return MainPayload;
            if (updatedConstellations.Count != GalaxyConstellations.Count)
                throw new InvalidOperationException("Добавление и удаление секторов требует полной разметки TGalaxy.");
            byte[] result = (byte[])MainPayload.Clone();
            for (int index = 0; index < GalaxyConstellations.Count; index++)
            {
                ConstellationRecord original = GalaxyConstellations[index];
                ConstellationRecord updated = updatedConstellations[index];
                if (!original.StructuralContentEquals(updated))
                    throw new InvalidOperationException(
                        "TConstellation: кроме видимости остальные поля сектора доступны только для чтения.");
                if (original.VisibleOffset < 0 || original.VisibleOffset >= result.Length)
                    throw new SavFormatException("TConstellation: неверное смещение поля видимости.");
                result[original.VisibleOffset] = updated.Visible ? (byte)1 : (byte)0;
            }
            return result;
        }

        private byte[] RewriteStarHeaders(byte[] prefixPatched, IList<StarHeaderRecord> updatedStars)
        {
            if (updatedStars.Count != GalaxyStars.Count)
                throw new InvalidOperationException("Добавление и удаление звёзд требует полной разметки TGalaxy.");
            int prefixDelta = prefixPatched.Length - MainPayload.Length;
            using (MemoryStream output = new MemoryStream(prefixPatched.Length + 512))
            {
                int sourceOffset = 0;
                for (int index = 0; index < GalaxyStars.Count; index++)
                {
                    StarHeaderRecord original = GalaxyStars[index];
                    StarHeaderRecord updated = updatedStars[index];
                    if (updated.ObjectId != original.ObjectId || updated.Raw08 != original.Raw08 ||
                        updated.Raw0C != original.Raw0C || updated.PlanetCount != original.PlanetCount)
                        throw new InvalidOperationException("TStar: ID, RNG и число планет доступны только для чтения.");
                    if (!IsSupportedStarName(updated.Name) || !IsSupportedStarCoordinate(updated.X) || !IsSupportedStarCoordinate(updated.Y))
                        throw new InvalidOperationException("TStar: неверное имя или координата вне диапазона -4096..4096.");
                    if (updated.Raw1C < 200 || updated.Raw1C > 300)
                        throw new InvalidOperationException("TStar: радиус должен быть в диапазоне 200..300.");

                    int start = checked(original.Start + prefixDelta);
                    int end = checked(original.HeaderEnd + prefixDelta);
                    output.Write(prefixPatched, sourceOffset, start - sourceOffset);
                    WriteUInt32(output, updated.ObjectId);
                    WriteInt32(output, updated.Raw08);
                    WriteUInt32(output, updated.Raw0C);
                    WriteUtf16Z(output, updated.Name ?? string.Empty);
                    WriteSingle(output, updated.X);
                    WriteSingle(output, updated.Y);
                    WriteUInt16(output, updated.Raw1C);
                    output.WriteByte(updated.Raw78);
                    WriteUInt16(output, updated.PlanetCount);
                    sourceOffset = end;
                }
                output.Write(prefixPatched, sourceOffset, prefixPatched.Length - sourceOffset);
                return output.ToArray();
            }
        }

        private byte[] RewriteCustomWeaponInfos(byte[] prefixPatched,
            IList<CustomWeaponInfoRecord> updatedWeapons)
        {
            if (updatedWeapons == null || updatedWeapons.Count > CustomWeaponInfos.Count ||
                updatedWeapons.Count > ushort.MaxValue)
                throw new InvalidOperationException("TCustomWeaponInfo: неверное число записей.");
            int prefixDelta = prefixPatched.Length - MainPayload.Length;
            using (MemoryStream output = new MemoryStream(prefixPatched.Length + 512))
            {
                int countOffset = checked(GalaxyPrefix.End - 2 + prefixDelta);
                int sourceOffset = CustomWeaponInfos.Count == 0
                    ? checked(GalaxyPrefix.End + prefixDelta)
                    : checked(CustomWeaponInfos[CustomWeaponInfos.Count - 1].End + prefixDelta);
                output.Write(prefixPatched, 0, countOffset);
                WriteUInt16(output, checked((ushort)updatedWeapons.Count));
                int sourceIndex = 0;
                for (int index = 0; index < updatedWeapons.Count; index++)
                {
                    CustomWeaponInfoRecord updated = updatedWeapons[index];
                    while (sourceIndex < CustomWeaponInfos.Count &&
                        CustomWeaponInfos[sourceIndex].Start != updated.Start) sourceIndex++;
                    if (sourceIndex >= CustomWeaponInfos.Count)
                        throw new InvalidOperationException(
                            "TCustomWeaponInfo: добавление или перестановка записей не разрешены.");
                    CustomWeaponInfoRecord original = CustomWeaponInfos[sourceIndex++];
                    if (!IsSupportedEditableText(updated.SystemName, 512, false) ||
                        updated.WeaponDamageSet == null || updated.WeaponDamageSet.Length != 8 ||
                        !IsSupportedAsteroidScalar(updated.ModCost) ||
                        !IsSupportedAsteroidScalar(updated.SecondaryDamageRadius) ||
                        !IsSupportedAsteroidScalar(updated.MiningFactor) ||
                        !IsSupportedEditableText(updated.PrimarySE, 512, true) ||
                        !IsSupportedEditableText(updated.SecondarySE, 512, true) ||
                        !IsSupportedEditableText(updated.AreaSE, 512, true))
                        throw new InvalidOperationException("TCustomWeaponInfo: неверное числовое или строковое поле.");
                    for (int field = 0; field < updated.WeaponDamageSet.Length; field++)
                        if (!IsSupportedAsteroidScalar(updated.WeaponDamageSet[field]))
                            throw new InvalidOperationException("TCustomWeaponInfo: неверный коэффициент урона.");

                    WriteUtf16Z(output, updated.SystemName);
                    output.WriteByte(updated.TechLevel);
                    output.WriteByte(updated.TechRadius);
                    WriteSingle(output, updated.ModCost);
                    WriteInt32(output, updated.MinDamage);
                    WriteInt32(output, updated.MaxDamage);
                    WriteInt32(output, updated.AverageSize);
                    WriteInt32(output, updated.AverageRadius);
                    WriteInt32(output, updated.Speed);
                    WriteInt32(output, updated.MissileRadius);
                    WriteInt32(output, updated.MissileMinSpeed);
                    WriteInt32(output, updated.MissileMaxSpeed);
                    output.WriteByte(updated.MissileChanceToBeHit);
                    WriteUInt32(output, updated.DamageType);
                    output.WriteByte(updated.ShotType);
                    output.WriteByte(updated.ShotCount);
                    output.WriteByte(updated.AttackCount);
                    WriteSingle(output, updated.SecondaryDamageRadius);
                    WriteSingle(output, updated.MiningFactor);
                    for (int field = 0; field < updated.WeaponDamageSet.Length; field++)
                        WriteSingle(output, updated.WeaponDamageSet[field]);
                    WriteOptionalString(output, updated.PrimarySE);
                    WriteOptionalString(output, updated.SecondarySE);
                    WriteOptionalString(output, updated.AreaSE);
                    WriteInt32(output, updated.DefaultPalette);
                    output.WriteByte(updated.Availability);
                    output.WriteByte(updated.ABWeaponType);
                }
                output.Write(prefixPatched, sourceOffset, prefixPatched.Length - sourceOffset);
                return output.ToArray();
            }
        }

        private void ValidateRemovedCustomWeaponReferences(
            IList<CustomWeaponInfoRecord> updatedWeapons,
            IList<StarHeaderRecord> updatedStars, IList<PlanetHeaderRecord> updatedPlanets,
            IList<ShipHeaderRecord> updatedShips, IList<ItemHeaderRecord> updatedItems,
            IList<MissileRecord> updatedMissiles, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary)
        {
            Dictionary<int, CustomWeaponInfoRecord> updatedByStart =
                new Dictionary<int, CustomWeaponInfoRecord>();
            HashSet<string> retainedNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (CustomWeaponInfoRecord weapon in updatedWeapons)
                if (weapon != null)
                {
                    if (!retainedNames.Add(weapon.SystemName ?? string.Empty))
                        throw new InvalidOperationException(
                            "TCustomWeaponInfo: системное имя должно быть уникальным.");
                    updatedByStart[weapon.Start] = weapon;
                }
            HashSet<string> removedNames = new HashSet<string>(StringComparer.Ordinal);
            Dictionary<string, string> renamedNames =
                new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (CustomWeaponInfoRecord weapon in CustomWeaponInfos)
                if (weapon != null)
                {
                    CustomWeaponInfoRecord updated;
                    if (!updatedByStart.TryGetValue(weapon.Start, out updated))
                        removedNames.Add(weapon.SystemName ?? string.Empty);
                    else if (!string.Equals(weapon.SystemName, updated.SystemName,
                        StringComparison.Ordinal))
                        renamedNames[weapon.SystemName ?? string.Empty] =
                            updated.SystemName ?? string.Empty;
                }

            if (renamedNames.Count != 0)
            {
                Dictionary<int, ItemHeaderRecord> itemsByStart =
                    new Dictionary<int, ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in updatedItems)
                    if (item != null) itemsByStart[item.Start] = item;
                foreach (ItemHeaderRecord original in GalaxyItems)
                {
                    string renamed;
                    if (original == null || original.Type != 68 ||
                        !renamedNames.TryGetValue(original.CustomWeaponName ?? string.Empty,
                            out renamed)) continue;
                    ItemHeaderRecord updated;
                    if (!itemsByStart.TryGetValue(original.Start, out updated) ||
                        !string.Equals(updated.CustomWeaponName, renamed, StringComparison.Ordinal))
                        throw new InvalidOperationException("TCustomWeaponInfo «" +
                            original.CustomWeaponName + "» переименован не во всех предметах.");
                }
                Dictionary<int, MissileRecord> missilesByStart =
                    new Dictionary<int, MissileRecord>();
                foreach (MissileRecord missile in updatedMissiles)
                    if (missile != null) missilesByStart[missile.Start] = missile;
                foreach (MissileRecord original in GalaxyMissiles)
                {
                    string renamed;
                    if (original == null || !original.IsCustom ||
                        !renamedNames.TryGetValue(original.CustomWeaponName ?? string.Empty,
                            out renamed)) continue;
                    MissileRecord updated;
                    if (!missilesByStart.TryGetValue(original.Start, out updated) ||
                        !string.Equals(updated.CustomWeaponName, renamed, StringComparison.Ordinal))
                        throw new InvalidOperationException("TCustomWeaponInfo «" +
                            original.CustomWeaponName + "» переименован не во всех ракетах.");
                }
            }
            if (removedNames.Count == 0) return;

            HashSet<int> removedItemStarts = RemovedNestedItemStarts(updatedStars,
                updatedPlanets, updatedShips, updatedItems, updatedStoredItems,
                updatedGalaxySummary);
            foreach (ItemHeaderRecord item in GalaxyItems)
                if (item != null && item.Type == 68 &&
                    removedNames.Contains(item.CustomWeaponName ?? string.Empty) &&
                    !removedItemStarts.Contains(item.Start))
                    throw new InvalidOperationException(
                        "TCustomWeaponInfo «" + item.CustomWeaponName +
                        "» ещё используется предметом " + item.ObjectId + ".");
            foreach (MissileRecord missile in updatedMissiles)
                if (missile != null && missile.IsCustom &&
                    removedNames.Contains(missile.CustomWeaponName ?? string.Empty))
                    throw new InvalidOperationException(
                        "TCustomWeaponInfo «" + missile.CustomWeaponName +
                        "» ещё используется ракетой " + missile.ObjectId + ".");
        }

        private sealed class PayloadPatch
        {
            internal int Start;
            internal int Length;
            internal byte[] Value;
            internal string Source;

            internal PayloadPatch(int start, int length, byte[] value,
                [System.Runtime.CompilerServices.CallerMemberName] string source = null)
            {
                Start = start;
                Length = length;
                Value = value;
                Source = source ?? "unknown";
            }
        }

        private byte[] RewriteKnownObjectHeaders(byte[] starPatched, IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, AchievementStatsRecord updatedAchievements,
            IList<HoleRecord> updatedHoles, IList<AsteroidRecord> updatedAsteroids,
            IList<MissileRecord> updatedMissiles, IList<InterfaceOverrideRecord> updatedInterfaceOverrides,
            IList<StoredItemRecord> updatedStoredItems, GalaxySummaryData updatedGalaxySummary)
        {
            IList<StarHeaderRecord> stars = updatedStars ?? GalaxyStars;
            int starDelta = 0;
            for (int index = 0; index < GalaxyStars.Count; index++)
                starDelta += Utf16ZLength(stars[index].Name) - Utf16ZLength(GalaxyStars[index].Name);
            int prefixDelta = starPatched.Length - MainPayload.Length - starDelta;
            List<PayloadPatch> patches = new List<PayloadPatch>();

            IList<PlanetHeaderRecord> planets = updatedPlanets ?? GalaxyPlanets;
            if (planets.Count != GalaxyPlanets.Count)
                throw new InvalidOperationException("Добавление и удаление планет требует полной разметки TGalaxy.");
            for (int index = 0; index < planets.Count; index++)
                AddPlanetHeaderPatches(patches, GalaxyPlanets[index], planets[index], prefixDelta, stars);

            for (int index = 0; index < GalaxyStars.Count; index++)
                AddStarTailPatch(patches, GalaxyStars[index], stars[index], prefixDelta, stars);

            for (int index = 0; index < GalaxyStars.Count; index++)
                AddStarSpaceShipPatches(patches, GalaxyStars[index], stars[index], prefixDelta, stars);

            for (int index = 0; index < GalaxyStars.Count; index++)
                AddStarSpaceItemPatches(patches, GalaxyStars[index], stars[index], prefixDelta, stars);

            for (int index = 0; index < GalaxyStars.Count; index++)
                AddStarDropItemPatches(patches, GalaxyStars[index], stars[index], prefixDelta, stars);

            IList<ShipHeaderRecord> ships = updatedShips ?? GalaxyShips;
            Dictionary<int, ShipHeaderRecord> retainedShips =
                ValidateShipDeletionOrder(ships, stars, planets);
            foreach (ShipHeaderRecord originalShip in GalaxyShips)
            {
                ShipHeaderRecord updatedShip;
                if (retainedShips.TryGetValue(originalShip.Start, out updatedShip))
                    AddShipHeaderPatches(patches, originalShip, updatedShip, prefixDelta, stars);
            }

            IList<ItemHeaderRecord> items = updatedItems ?? GalaxyItems;
            if (items.Count != GalaxyItems.Count)
                throw new InvalidOperationException("Добавление и удаление предметов требует полной разметки TGalaxy.");
            IList<StoredItemRecord> storedItems = updatedStoredItems ?? StoredItems;
            GalaxySummaryData galaxySummary = updatedGalaxySummary ?? GalaxySummary;
            HashSet<int> removedNestedItemStarts = RemovedNestedItemStarts(stars, planets,
                ships, items, storedItems, galaxySummary);
            for (int index = 0; index < items.Count; index++)
                if (!removedNestedItemStarts.Contains(GalaxyItems[index].Start))
                    AddItemHeaderPatches(patches, GalaxyItems[index], items[index], prefixDelta, stars);

            if (updatedAchievements != null)
                AddAchievementStatsPatches(patches, AchievementStats, updatedAchievements, prefixDelta, stars);

            IList<HoleRecord> holes = updatedHoles ?? GalaxyHoles;
            AddHoleListPatches(patches, holes, prefixDelta, stars);

            IList<AsteroidRecord> asteroids = updatedAsteroids ?? GalaxyAsteroids;
            IList<MissileRecord> missiles = updatedMissiles ?? GalaxyMissiles;
            ValidateRemovedTargetReferences(stars, ships, items, asteroids, missiles,
                galaxySummary);
            AddAsteroidListPatches(patches, asteroids, prefixDelta, stars);
            AddMissileListPatches(patches, missiles, prefixDelta, stars);

            if (updatedGalaxySummary != null)
                AddGalaxySummaryPatches(patches, GalaxySummary, updatedGalaxySummary, prefixDelta, stars);

            IList<InterfaceOverrideRecord> interfaceOverrides = updatedInterfaceOverrides ??
                GalaxySummary.InterfaceOverrides;
            AddInterfaceOverrideListPatches(patches, interfaceOverrides, prefixDelta, stars);

            AddStoredItemListPatches(patches, storedItems, prefixDelta, stars);
            AddScriptShopSlotPatches(patches, galaxySummary, prefixDelta, stars);

            patches.Sort(delegate(PayloadPatch left, PayloadPatch right)
            {
                int byStart = left.Start.CompareTo(right.Start);
                return byStart != 0 ? byStart : left.Length.CompareTo(right.Length);
            });
            for (int index = patches.Count - 1; index > 0; index--)
            {
                PayloadPatch left = patches[index - 1];
                PayloadPatch right = patches[index];
                if (left.Start != right.Start || left.Length != right.Length) continue;
                if (!EqualBytes(left.Value, right.Value))
                    throw new InvalidOperationException("Два редактора задали разные значения одному полю SAV: 0x" +
                        left.Start.ToString("X") + ".");
                patches.RemoveAt(index);
            }
            using (MemoryStream output = new MemoryStream(starPatched.Length + 1024))
            {
                int sourceOffset = 0;
                PayloadPatch previousPatch = null;
                foreach (PayloadPatch patch in patches)
                {
                    if (patch.Start < sourceOffset || patch.Length < 0 || patch.Start > starPatched.Length - patch.Length)
                        throw new InvalidOperationException("Пересекающиеся или неверные границы изменяемых полей SAV: start=0x" +
                            patch.Start.ToString("X") + ", length=" + patch.Length + ", previousEnd=0x" +
                            sourceOffset.ToString("X") + ", previousStart=0x" +
                            (previousPatch == null ? "-" : previousPatch.Start.ToString("X")) +
                            ", previousLength=" + (previousPatch == null ? 0 : previousPatch.Length) +
                            ", source=" + patch.Source + ", previousSource=" +
                            (previousPatch == null ? "-" : previousPatch.Source) +
                            ", payloadLength=0x" + starPatched.Length.ToString("X") + ".");
                    output.Write(starPatched, sourceOffset, patch.Start - sourceOffset);
                    output.Write(patch.Value, 0, patch.Value.Length);
                    sourceOffset = patch.Start + patch.Length;
                    previousPatch = patch;
                }
                output.Write(starPatched, sourceOffset, starPatched.Length - sourceOffset);
                return output.ToArray();
            }
        }

        private Dictionary<int, ShipHeaderRecord> ValidateShipDeletionOrder(
            IList<ShipHeaderRecord> ships, IList<StarHeaderRecord> stars,
            IList<PlanetHeaderRecord> planets)
        {
            if (ships == null || ships.Count > GalaxyShips.Count)
                throw new InvalidOperationException("TShip: добавление новых кораблей не разрешено.");
            Dictionary<int, ShipHeaderRecord> retained =
                new Dictionary<int, ShipHeaderRecord>();
            int sourceIndex = 0;
            foreach (ShipHeaderRecord value in ships)
            {
                if (value == null)
                    throw new InvalidOperationException("TShip: пустая запись.");
                while (sourceIndex < GalaxyShips.Count &&
                    GalaxyShips[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= GalaxyShips.Count)
                    throw new InvalidOperationException(
                        "TShip: разрешены только исходный порядок и удаление кораблей.");
                retained.Add(value.Start, value);
                sourceIndex++;
            }

            HashSet<int> removed = new HashSet<int>();
            foreach (ShipHeaderRecord source in GalaxyShips)
                if (!retained.ContainsKey(source.Start)) removed.Add(source.Start);
            HashSet<int> removedFromOwners = new HashSet<int>();
            for (int index = 0; index < GalaxyStars.Count; index++)
                CollectRemovedStarShipStarts(GalaxyStars[index].SpaceShips,
                    stars[index].SpaceShips, removedFromOwners);
            for (int index = 0; index < GalaxyPlanets.Count; index++)
                CollectRemovedPlanetWarriorStarts(GalaxyPlanets[index].Warriors,
                    planets[index].Warriors, removedFromOwners);
            if (!removed.SetEquals(removedFromOwners))
                throw new InvalidOperationException(
                    "TShip: удаление должно одновременно изменить точный список владельца TStar/TPlanet.");
            return retained;
        }

        private static void CollectRemovedStarShipStarts(IList<StarShipRecord> original,
            IList<StarShipRecord> updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (StarShipRecord value in updated)
                if (value != null) retained.Add(value.ShipStart);
            foreach (StarShipRecord value in original)
                if (value != null && !retained.Contains(value.ShipStart)) removed.Add(value.ShipStart);
        }

        private static void CollectRemovedPlanetWarriorStarts(
            IList<PlanetWarriorRecord> original, IList<PlanetWarriorRecord> updated,
            HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (PlanetWarriorRecord value in updated)
                if (value != null) retained.Add(value.ShipStart);
            foreach (PlanetWarriorRecord value in original)
                if (value != null && !retained.Contains(value.ShipStart)) removed.Add(value.ShipStart);
        }

        private HashSet<int> RemovedNestedItemStarts(IList<StarHeaderRecord> updatedStars,
            IList<PlanetHeaderRecord> updatedPlanets, IList<ShipHeaderRecord> updatedShips,
            IList<ItemHeaderRecord> updatedItems, IList<StoredItemRecord> updatedStoredItems,
            GalaxySummaryData updatedGalaxySummary)
        {
            HashSet<int> removed = new HashSet<int>();
            if (updatedPlanets == null || updatedPlanets.Count != GalaxyPlanets.Count) return removed;
            for (int planetIndex = 0; planetIndex < GalaxyPlanets.Count; planetIndex++)
            {
                PlanetHeaderRecord original = GalaxyPlanets[planetIndex];
                PlanetHeaderRecord updated = updatedPlanets[planetIndex];
                if (updated == null) continue;
                AddRemovedGoneItemStarts(original.GoneItems, updated.GoneItems, removed);
                AddRemovedShipItemStarts(original.EquipmentShopItems,
                    updated.EquipmentShopItems, removed);
            }
            if (updatedShips != null)
            {
                Dictionary<int, ShipHeaderRecord> retainedShips =
                    new Dictionary<int, ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in updatedShips)
                    if (ship != null) retainedShips[ship.Start] = ship;
                foreach (ShipHeaderRecord originalShip in GalaxyShips)
                {
                    ShipHeaderRecord updatedShip;
                    if (retainedShips.TryGetValue(originalShip.Start, out updatedShip))
                        AddRemovedShipNestedItemStarts(originalShip, updatedShip, removed);
                    else
                        AddAllShipNestedItemStarts(originalShip, removed);
                }
            }
            if (updatedStars != null && updatedStars.Count == GalaxyStars.Count)
                for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
                {
                    AddRemovedShipItemStarts(GalaxyStars[starIndex].SpaceItems,
                        updatedStars[starIndex].SpaceItems, removed);
                    AddRemovedStarDropItemStarts(GalaxyStars[starIndex].DropItems,
                        updatedStars[starIndex].DropItems, removed);
                }
            if (updatedStoredItems != null)
            {
                HashSet<int> retainedStored = new HashSet<int>();
                foreach (StoredItemRecord record in updatedStoredItems)
                    if (record != null) retainedStored.Add(record.ItemStart);
                foreach (StoredItemRecord record in StoredItems)
                    if (!retainedStored.Contains(record.ItemStart)) removed.Add(record.ItemStart);
            }
            if (updatedGalaxySummary != null)
                AddRemovedScriptShopSlotStarts(GalaxySummary.ScriptShopSlots,
                    updatedGalaxySummary.ScriptShopSlots, removed);
            if (updatedItems != null && updatedItems.Count == GalaxyItems.Count)
                for (int itemIndex = 0; itemIndex < GalaxyItems.Count; itemIndex++)
                {
                    ItemHeaderRecord originalItem = GalaxyItems[itemIndex];
                    ItemHeaderRecord updatedItem = updatedItems[itemIndex];
                    if (updatedItem != null && originalItem.Start == updatedItem.Start &&
                        originalItem.NestedTranclucator != null &&
                        updatedItem.NestedTranclucator != null)
                        AddRemovedShipNestedItemStarts(originalItem.NestedTranclucator,
                            updatedItem.NestedTranclucator, removed);
                }
            return removed;
        }

        private static void AddRemovedScriptShopSlotStarts(
            IList<ScriptShopSlotRecord> original, IList<ScriptShopSlotRecord> updated,
            HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (ScriptShopSlotRecord record in updated)
                if (record != null && record.HasEquipment) retained.Add(record.ItemStart);
            foreach (ScriptShopSlotRecord record in original)
                if (record != null && record.HasEquipment &&
                    !retained.Contains(record.ItemStart)) removed.Add(record.ItemStart);
        }

        private static void AddRemovedShipNestedItemStarts(ShipHeaderRecord original,
            ShipHeaderRecord updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            AddRemovedShipItemStarts(original.EquipmentItems, updated.EquipmentItems, removed);
            AddRemovedShipItemStarts(original.ArtefactItems, updated.ArtefactItems, removed);
            AddRemovedShipItemStarts(original.DropListItems, updated.DropListItems, removed);
            AddRemovedShipItemStarts(original.RuinsEquipmentItems,
                updated.RuinsEquipmentItems, removed);
            AddRemovedPlayerStorageItemStarts(original.PlayerStorageItems,
                updated.PlayerStorageItems, removed);
            AddRemovedShipItemStarts(original.PlayerSatelliteItems,
                updated.PlayerSatelliteItems, removed);
            if (original.PlayerBridgeRuins != null && updated.PlayerBridgeRuins != null)
                AddRemovedShipNestedItemStarts(original.PlayerBridgeRuins,
                    updated.PlayerBridgeRuins, removed);
        }

        private static void AddAllShipNestedItemStarts(ShipHeaderRecord ship,
            HashSet<int> removed)
        {
            if (ship == null) return;
            AddAllShipItemStarts(ship.EquipmentItems, removed);
            AddAllShipItemStarts(ship.ArtefactItems, removed);
            AddAllShipItemStarts(ship.DropListItems, removed);
            AddAllShipItemStarts(ship.RuinsEquipmentItems, removed);
            if (ship.PlayerStorageItems != null)
                foreach (PlayerStorageItemRecord record in ship.PlayerStorageItems)
                    if (record != null) removed.Add(record.ItemStart);
            AddAllShipItemStarts(ship.PlayerSatelliteItems, removed);
            if (ship.PlayerBridgeRuins != null)
                AddAllShipNestedItemStarts(ship.PlayerBridgeRuins, removed);
        }

        private static void AddAllShipItemStarts(IList<ShipItemListEntry> records,
            HashSet<int> removed)
        {
            if (records == null) return;
            foreach (ShipItemListEntry record in records)
                if (record != null) removed.Add(record.ItemStart);
        }

        private static void AddRemovedShipItemStarts(IList<ShipItemListEntry> original,
            IList<ShipItemListEntry> updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (ShipItemListEntry record in updated)
                if (record != null) retained.Add(record.ItemStart);
            foreach (ShipItemListEntry record in original)
                if (record != null && !retained.Contains(record.ItemStart)) removed.Add(record.ItemStart);
        }

        private static void AddRemovedPlayerStorageItemStarts(IList<PlayerStorageItemRecord> original,
            IList<PlayerStorageItemRecord> updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (PlayerStorageItemRecord record in updated)
                if (record != null) retained.Add(record.ItemStart);
            foreach (PlayerStorageItemRecord record in original)
                if (record != null && !retained.Contains(record.ItemStart)) removed.Add(record.ItemStart);
        }

        private static void AddRemovedGoneItemStarts(IList<PlanetGoneItemRecord> original,
            IList<PlanetGoneItemRecord> updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (PlanetGoneItemRecord record in updated)
                if (record != null) retained.Add(record.ItemStart);
            foreach (PlanetGoneItemRecord record in original)
                if (record != null && !retained.Contains(record.ItemStart)) removed.Add(record.ItemStart);
        }

        private static void AddRemovedStarDropItemStarts(IList<StarDropItemRecord> original,
            IList<StarDropItemRecord> updated, HashSet<int> removed)
        {
            if (original == null || updated == null) return;
            HashSet<int> retained = new HashSet<int>();
            foreach (StarDropItemRecord record in updated)
                if (record != null) retained.Add(record.ItemStart);
            foreach (StarDropItemRecord record in original)
                if (record != null && !retained.Contains(record.ItemStart)) removed.Add(record.ItemStart);
        }

        private void AddStarTailPatch(List<PayloadPatch> patches, StarHeaderRecord original,
            StarHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.TailStart <= original.HeaderEnd || original.TailEnd <= original.TailStart ||
                updated.TailStart != original.TailStart || updated.TailEnd != original.TailEnd ||
                updated.CustomInfoCountOffset != original.CustomInfoCountOffset ||
                updated.CustomSystemInfos == null || updated.CustomSystemInfos.Count > 10000)
                throw new InvalidOperationException("TStar: неверные границы хвоста или списка TCustomSystemInfo.");
            if (original.TailContentEquals(updated)) return;

            bool knownConstellation = false;
            foreach (ConstellationRecord constellation in GalaxyConstellations)
                if (constellation.ObjectId == updated.ConstellationObjectId) knownConstellation = true;
            if (!knownConstellation || !IsSupportedItemText(updated.GraphType, 256) ||
                updated.Safety > 100 || updated.Owners > 2 || updated.LastOwners > 2 ||
                updated.DominatorSeries > 2 || !IsSupportedItemText(updated.CustomFaction, 4096) ||
                !IsSupportedStarScalar(updated.SafeRadius) || !IsSupportedStarScalar(updated.DamageRadius) ||
                !IsSupportedStarGraphName(updated.GraphStar) || !IsSupportedItemText(updated.MapLabel, 4096) ||
                updated.DominionObjectId > 10000000)
                throw new InvalidOperationException("TStar: неверное строковое, числовое поле или ссылка.");
            if (updated.DominionObjectId != 0 && !IsKnownRuinsObjectId(updated.DominionObjectId))
                throw new InvalidOperationException("TStar.Dominion должен ссылаться на станцию/руины.");

            using (MemoryStream encoded = new MemoryStream())
            {
                WriteUInt32(encoded, updated.ConstellationObjectId);
                WriteUtf16Z(encoded, updated.GraphType ?? string.Empty);
                WriteBoolean(encoded, updated.Battle);
                encoded.WriteByte(updated.Safety);
                encoded.WriteByte(updated.Overloading);
                encoded.WriteByte(updated.Owners);
                encoded.WriteByte(updated.LastOwners);
                encoded.WriteByte(updated.DominatorSeries);
                WriteUtf16Z(encoded, updated.CustomFaction ?? string.Empty);
                WriteSingle(encoded, updated.SafeRadius);
                WriteSingle(encoded, updated.DamageRadius);
                WriteUInt16(encoded, updated.GraphRadius);
                WriteUtf16Z(encoded, updated.GraphStar ?? string.Empty);
                WriteBoolean(encoded, updated.WarPlayer);
                encoded.WriteByte(updated.DayBeforeOccupy);
                WriteInt32(encoded, updated.DayWithoutPlayer);
                WriteInt32(encoded, updated.DayWithoutCreateShip);
                WriteInt32(encoded, updated.LastDominatorDate);
                WriteInt32(encoded, updated.LastPirateDate);
                WriteInt32(encoded, updated.LiberationDate);
                WriteInt32(encoded, updated.DayInvadeInertia);
                WriteBoolean(encoded, updated.NoComeKling);
                WriteUInt32(encoded, updated.DominionObjectId);
                WriteUtf16Z(encoded, updated.MapLabel ?? string.Empty);
                WriteUInt16(encoded, checked((ushort)updated.CustomSystemInfos.Count));
                foreach (CustomSystemInfoRecord record in updated.CustomSystemInfos)
                {
                    if (record == null || !IsSupportedItemText(record.Name, 32768) ||
                        !IsSupportedItemText(record.Icon, 32768) || !IsSupportedItemText(record.Info, 32768) ||
                        !IsSupportedItemText(record.Type, 32768))
                        throw new InvalidOperationException("TCustomSystemInfo: неверное строковое поле.");
                    WriteUtf16Z(encoded, record.Name ?? string.Empty);
                    WriteUtf16Z(encoded, record.Icon ?? string.Empty);
                    WriteUtf16Z(encoded, record.Info ?? string.Empty);
                    WriteUtf16Z(encoded, record.Type ?? string.Empty);
                    WriteInt32(encoded, record.Distance);
                }
                patches.Add(new PayloadPatch(MapKnownOffset(original.TailStart, prefixDelta, stars),
                    original.TailEnd - original.TailStart, encoded.ToArray()));
            }
        }

        private bool IsKnownRuinsObjectId(uint objectId)
        {
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.ObjectId == objectId && (ship.Type >= 6 ||
                    (ship.GraphName ?? string.Empty).StartsWith("Ruins.", StringComparison.OrdinalIgnoreCase)))
                    return true;
            return false;
        }

        private void AddStarSpaceShipPatches(List<PayloadPatch> patches,
            StarHeaderRecord original, StarHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (!original.HasExactSpaceShipList)
            {
                if (updated.HasExactSpaceShipList != original.HasExactSpaceShipList ||
                    !original.SpaceShipsContentEquals(updated))
                    throw new InvalidOperationException(
                        "TStar.Ships: модовый/неоднозначный список доступен только для чтения.");
                return;
            }
            if (original.SpaceShips == null || updated.SpaceShips == null ||
                updated.SpaceShips.Count > original.SpaceShips.Count ||
                updated.SpaceShipCountOffset != original.SpaceShipCountOffset ||
                !updated.HasExactSpaceShipList)
                throw new InvalidOperationException(
                    "TStar.Ships: неверный счётчик или добавление нового TShip.");
            if (original.SpaceShipsContentEquals(updated)) return;
            if (original.SpaceShipCountOffset < 0)
                throw new InvalidOperationException("TStar.Ships: границы списка не доказаны.");

            Dictionary<int, StarShipRecord> retained = new Dictionary<int, StarShipRecord>();
            int sourceIndex = 0;
            foreach (StarShipRecord value in updated.SpaceShips)
            {
                if (value == null)
                    throw new InvalidOperationException("TStar.Ships: пустая запись.");
                while (sourceIndex < original.SpaceShips.Count &&
                    original.SpaceShips[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.SpaceShips.Count)
                    throw new InvalidOperationException(
                        "TStar.Ships: разрешены только исходный порядок и удаление кораблей.");
                StarShipRecord source = original.SpaceShips[sourceIndex++];
                if (value.End != source.End || value.ShipStart != source.ShipStart ||
                    value.ShipType != source.ShipType || value.ShipObjectId != source.ShipObjectId)
                    throw new InvalidOperationException(
                        "TStar.Ships: тип, вложенный TShip и границы доступны только для чтения.");
                retained.Add(source.Start, value);
            }
            if (updated.SpaceShips.Count != original.SpaceShips.Count)
                AddUInt16Patch(patches, original.SpaceShipCountOffset,
                    checked((ushort)original.SpaceShips.Count),
                    checked((ushort)updated.SpaceShips.Count), prefixDelta, stars);
            foreach (StarShipRecord source in original.SpaceShips)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddStarSpaceItemPatches(List<PayloadPatch> patches,
            StarHeaderRecord original, StarHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (original.SpaceItems == null || updated.SpaceItems == null ||
                updated.SpaceItems.Count > original.SpaceItems.Count ||
                updated.SpaceItemCountOffset != original.SpaceItemCountOffset)
                throw new InvalidOperationException(
                    "TStar.ItemsInSpace: неверный счётчик или добавление нового TItem.");
            if (original.SpaceItemsContentEquals(updated)) return;

            Dictionary<int, ShipItemListEntry> retained =
                new Dictionary<int, ShipItemListEntry>();
            int sourceIndex = 0;
            foreach (ShipItemListEntry value in updated.SpaceItems)
            {
                if (value == null)
                    throw new InvalidOperationException("TStar.ItemsInSpace: пустая запись.");
                while (sourceIndex < original.SpaceItems.Count &&
                    original.SpaceItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.SpaceItems.Count)
                    throw new InvalidOperationException(
                        "TStar.ItemsInSpace: добавление и перестановка записей не разрешены.");
                ShipItemListEntry source = original.SpaceItems[sourceIndex++];
                if (value.End != source.End || value.ItemStart != source.ItemStart ||
                    value.ItemType != source.ItemType || value.ItemObjectId != source.ItemObjectId)
                    throw new InvalidOperationException(
                        "TStar.ItemsInSpace: тип, вложенный предмет и границы доступны только для чтения.");
                retained.Add(source.Start, value);
            }

            if (updated.SpaceItems.Count != original.SpaceItems.Count)
                AddUInt16Patch(patches, original.SpaceItemCountOffset,
                    checked((ushort)original.SpaceItems.Count),
                    checked((ushort)updated.SpaceItems.Count), prefixDelta, stars);
            foreach (ShipItemListEntry source in original.SpaceItems)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddStarDropItemPatches(List<PayloadPatch> patches, StarHeaderRecord original,
            StarHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.DropItems == null || updated.DropItems == null ||
                updated.DropItems.Count > original.DropItems.Count ||
                updated.DropItemCountOffset != original.DropItemCountOffset)
                throw new InvalidOperationException(
                    "TStar.DropItems: добавление новых записей требует сериализации нового TItem.");
            if (original.DropItemsContentEquals(updated)) return;

            Dictionary<int, StarDropItemRecord> retained = new Dictionary<int, StarDropItemRecord>();
            int sourceIndex = 0;
            foreach (StarDropItemRecord value in updated.DropItems)
            {
                if (value == null) throw new InvalidOperationException("TStar.DropItems: пустая запись.");
                while (sourceIndex < original.DropItems.Count &&
                    original.DropItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.DropItems.Count)
                    throw new InvalidOperationException(
                        "TStar.DropItems: добавление и перестановка записей не разрешены.");
                StarDropItemRecord source = original.DropItems[sourceIndex++];
                if (value.End != source.End ||
                    value.ItemStart != source.ItemStart || value.ItemType != source.ItemType ||
                    value.ItemObjectId != source.ItemObjectId)
                    throw new InvalidOperationException(
                        "TStar.DropItems: тип, вложенный предмет и границы записи доступны только для чтения.");
                retained.Add(source.Start, value);
            }

            if (updated.DropItems.Count != original.DropItems.Count)
                AddUInt16Patch(patches, original.DropItemCountOffset,
                    checked((ushort)original.DropItems.Count), checked((ushort)updated.DropItems.Count),
                    prefixDelta, stars);

            foreach (StarDropItemRecord source in original.DropItems)
            {
                StarDropItemRecord value;
                if (!retained.TryGetValue(source.Start, out value))
                {
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
                    continue;
                }
                if (!IsSupportedMissileScalar(value.X) || !IsSupportedMissileScalar(value.Y) ||
                    value.ShipObjectId > 10000000)
                    throw new InvalidOperationException(
                        "TStar.DropItems: неверная координата или ссылка на корабль.");

                AddFloatPatch(patches, source.Start, source.X, value.X, prefixDelta, stars);
                AddFloatPatch(patches, source.Start + 4, source.Y, value.Y, prefixDelta, stars);
                AddUInt32Patch(patches, source.Start + 8, source.ShipObjectId,
                    value.ShipObjectId, prefixDelta, stars);
                AddBytePatch(patches, source.Start + 12, source.InUse ? (byte)1 : (byte)0,
                    value.InUse ? (byte)1 : (byte)0, prefixDelta, stars);
            }
        }

        private void AddPlanetHeaderPatches(List<PayloadPatch> patches, PlanetHeaderRecord original,
            PlanetHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.ObjectId != original.ObjectId || updated.FirstListCount != original.FirstListCount ||
                updated.OpenInventions == null || updated.OpenInventions.Length != 20)
                throw new InvalidOperationException("TPlanet: идентификатор и границы вложенных списков доступны только для чтения.");
            if (!IsSupportedEditableText(updated.Name, 80, false) ||
                !IsSupportedObjectCoordinate(updated.PolarAngle) ||
                !IsSupportedObjectCoordinate(updated.PolarRadius) ||
                !IsSupportedObjectCoordinate(updated.Angle) ||
                !IsSupportedPlanetScalar(updated.OpenPointsInvention))
                throw new InvalidOperationException("TPlanet: неверное имя или числовое значение.");

            int originalNameStart = original.Start + 12;
            int originalNameEnd = original.ScalarOffset;
            if (updated.Name != original.Name)
                patches.Add(new PayloadPatch(MapKnownOffset(originalNameStart, prefixDelta, stars),
                    originalNameEnd - originalNameStart, EncodeUtf16Z(updated.Name)));
            AddInt32Patch(patches, original.Start + 4, original.Raw08, updated.Raw08, prefixDelta, stars);
            if (original.Raw0C != updated.Raw0C)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 8, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.Raw0C)));
            AddFloatPatch(patches, originalNameEnd, original.PolarAngle, updated.PolarAngle, prefixDelta, stars);
            AddFloatPatch(patches, originalNameEnd + 4, original.PolarRadius, updated.PolarRadius, prefixDelta, stars);
            AddFloatPatch(patches, originalNameEnd + 8, original.Angle, updated.Angle, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 12, original.Mass, updated.Mass, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 16, original.Radius, updated.Radius, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 20, original.WaterSpace, updated.WaterSpace, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 24, original.WaterSpaceDone, updated.WaterSpaceDone, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 28, original.LandSpace, updated.LandSpace, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 32, original.LandSpaceDone, updated.LandSpaceDone, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 36, original.HillSpace, updated.HillSpace, prefixDelta, stars);
            AddInt32Patch(patches, originalNameEnd + 40, original.HillSpaceDone, updated.HillSpaceDone, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 44, original.OrbitCount, updated.OrbitCount, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 45, original.VisitedByPlayer ? (byte)1 : (byte)0,
                updated.VisitedByPlayer ? (byte)1 : (byte)0, prefixDelta, stars);
            for (int index = 0; index < 20; index++)
                AddBytePatch(patches, originalNameEnd + 46 + index, original.OpenInventions[index],
                    updated.OpenInventions[index], prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 66, original.CurrentInvention, updated.CurrentInvention,
                prefixDelta, stars);
            AddFloatPatch(patches, originalNameEnd + 67, original.OpenPointsInvention,
                updated.OpenPointsInvention, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 71, original.NecessaryPercent, updated.NecessaryPercent,
                prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 72, original.NecessaryPercentK, updated.NecessaryPercentK,
                prefixDelta, stars);
            AddUInt32Patch(patches, originalNameEnd + 73, original.PeopleCount, updated.PeopleCount, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 77, original.Economy, updated.Economy, prefixDelta, stars);
            AddUInt32Patch(patches, originalNameEnd + 78, original.Money, updated.Money, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 82, original.Owner, updated.Owner, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 83, original.Race, updated.Race, prefixDelta, stars);
            AddBytePatch(patches, originalNameEnd + 84, original.Government, updated.Government, prefixDelta, stars);
            if (original.ShopGoods == null || updated.ShopGoods == null ||
                original.ShopGoods.GetLength(0) != 8 || updated.ShopGoods.GetLength(0) != 8 ||
                original.ShopGoods.GetLength(1) != 3 || updated.ShopGoods.GetLength(1) != 3 ||
                original.ShopDeficit == null || updated.ShopDeficit == null ||
                original.ShopSale == null || updated.ShopSale == null ||
                original.ShopDeficit.Length != 8 || updated.ShopDeficit.Length != 8 ||
                original.ShopSale.Length != 8 || updated.ShopSale.Length != 8)
                throw new InvalidOperationException("TPlanet.ShopGoods: неверный размер товарной таблицы.");
            for (int index = 0; index < 8; index++)
            {
                int row = originalNameEnd + 85 + index * 18;
                AddUInt32Patch(patches, row, original.ShopGoods[index, 0],
                    updated.ShopGoods[index, 0], prefixDelta, stars);
                // The game keeps a float mirror of the sale price between the
                // quantity and the two integer prices. Preserve it unless that
                // price changes so the game's mirrored value remains consistent.
                if (original.ShopGoods[index, 1] != updated.ShopGoods[index, 1])
                    patches.Add(new PayloadPatch(MapKnownOffset(row + 4, prefixDelta, stars), 4,
                        BitConverter.GetBytes((float)updated.ShopGoods[index, 1])));
                AddUInt32Patch(patches, row + 8, original.ShopGoods[index, 1],
                    updated.ShopGoods[index, 1], prefixDelta, stars);
                AddUInt32Patch(patches, row + 12, original.ShopGoods[index, 2],
                    updated.ShopGoods[index, 2], prefixDelta, stars);
                AddBytePatch(patches, row + 16, original.ShopDeficit[index],
                    updated.ShopDeficit[index], prefixDelta, stars);
                AddBytePatch(patches, row + 17, original.ShopSale[index],
                    updated.ShopSale[index], prefixDelta, stars);
            }
            AddPlanetRelationPatches(patches, original, updated, prefixDelta, stars);

            if (updated.HasLateFields != original.HasLateFields || updated.HasFlags != original.HasFlags ||
                original.Satellites == null || updated.Satellites == null ||
                original.SatelliteCount != original.Satellites.Count ||
                updated.SatelliteCount != updated.Satellites.Count)
                throw new InvalidOperationException("TPlanet: неверные границы или счётчик списка TSputnik.");
            if (original.HasLateFields)
            {
                AddPlanetEquipmentShopPatches(patches, original, updated, prefixDelta, stars);
                if (!IsSupportedEditableText(updated.GraphName, 128, false))
                    throw new InvalidOperationException("TPlanet: неверное имя графики.");
                AddUInt16Patch(patches, original.LateFieldsOffset - 10, original.RangerCount,
                    updated.RangerCount, prefixDelta, stars);
                AddUInt16Patch(patches, original.LateFieldsOffset - 8, original.TransportCount,
                    updated.TransportCount, prefixDelta, stars);
                AddUInt16Patch(patches, original.LateFieldsOffset, original.GraphRadius,
                    updated.GraphRadius, prefixDelta, stars);
                if (original.GraphName != updated.GraphName)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.LateFieldsOffset + 2, prefixDelta, stars),
                        original.GraphNameEnd - (original.LateFieldsOffset + 2), EncodeUtf16Z(updated.GraphName)));
                AddUInt16Patch(patches, original.GraphNameEnd, original.GraphSpeedRotate,
                    updated.GraphSpeedRotate, prefixDelta, stars);
                AddInt32Patch(patches, original.GraphNameEnd + 2, original.GraphStepRotate,
                    updated.GraphStepRotate, prefixDelta, stars);
                AddBytePatch(patches, original.GraphNameEnd + 6, original.GraphRing,
                    updated.GraphRing, prefixDelta, stars);
                AddInt32Patch(patches, original.GraphNameEnd + 7, original.QuestNumber,
                    updated.QuestNumber, prefixDelta, stars);
                AddPlanetSputnikPatch(patches, original, updated, prefixDelta, stars);
                AddPlanetGoneItemPatches(patches, original, updated, prefixDelta, stars);
            }
            if (original.HasFlags)
            {
                if (updated.NoPlanetShopUpdate > 3 ||
                    !IsSupportedEditableText(updated.CustomFaction, 128, true))
                    throw new InvalidOperationException("TPlanet: неверные флаги или модовая фракция.");
                AddBytePatch(patches, original.FlagsOffset, original.NoLanding ? (byte)1 : (byte)0,
                    updated.NoLanding ? (byte)1 : (byte)0, prefixDelta, stars);
                byte originalFlags = (byte)(original.NoPlanetShopUpdate |
                    (original.NoBuyShips ? 4 : 0) | (original.NoRandomEvents ? 8 : 0));
                byte updatedFlags = (byte)(updated.NoPlanetShopUpdate |
                    (updated.NoBuyShips ? 4 : 0) | (updated.NoRandomEvents ? 8 : 0));
                AddBytePatch(patches, original.FlagsOffset + 1, originalFlags, updatedFlags, prefixDelta, stars);
                AddBytePatch(patches, original.FlagsOffset + 2, original.IsRogeria ? (byte)1 : (byte)0,
                    updated.IsRogeria ? (byte)1 : (byte)0, prefixDelta, stars);
                if (original.CustomFaction != updated.CustomFaction)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.CustomFactionOffset, prefixDelta, stars),
                        original.End - original.CustomFactionOffset, EncodeUtf16Z(updated.CustomFaction)));
            }
        }

        private void AddPlanetEquipmentShopPatches(List<PayloadPatch> patches,
            PlanetHeaderRecord original, PlanetHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (original.EquipmentShopItems == null || updated.EquipmentShopItems == null ||
                original.EquipmentShopCountOffset != original.RelationEndOffset ||
                original.EquipmentShopEndOffset != (original.HasWarriorList ?
                    original.WarriorCountOffset : original.LateFieldsOffset - 10) ||
                updated.HasWarriorList != original.HasWarriorList ||
                original.EquipmentShopCount != original.EquipmentShopItems.Count ||
                updated.EquipmentShopCount != updated.EquipmentShopItems.Count ||
                updated.EquipmentShopItems.Count > original.EquipmentShopItems.Count)
                throw new InvalidOperationException(
                    "TPlanet.EquipmentShop: добавление новых предметов или неверная граница списка.");
            ValidatePlanetWarriors(original, updated);

            Dictionary<int, ShipItemListEntry> retained = new Dictionary<int, ShipItemListEntry>();
            int sourceIndex = 0;
            foreach (ShipItemListEntry value in updated.EquipmentShopItems)
            {
                if (value == null)
                    throw new InvalidOperationException("TPlanet.EquipmentShop: пустая запись.");
                while (sourceIndex < original.EquipmentShopItems.Count &&
                    original.EquipmentShopItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.EquipmentShopItems.Count)
                    throw new InvalidOperationException(
                        "TPlanet.EquipmentShop: разрешены только правка и удаление исходных предметов.");
                ShipItemListEntry source = original.EquipmentShopItems[sourceIndex++];
                if (value.ItemType != source.ItemType || value.ItemObjectId != source.ItemObjectId ||
                    value.ItemStart != source.ItemStart || value.End != source.End)
                    throw new InvalidOperationException(
                        "TPlanet.EquipmentShop: структура и идентификатор вложенного TItem доступны только для чтения.");
                retained.Add(source.Start, value);
            }
            if (updated.EquipmentShopItems.Count != original.EquipmentShopItems.Count)
                AddUInt16Patch(patches, original.EquipmentShopCountOffset,
                    checked((ushort)original.EquipmentShopItems.Count),
                    checked((ushort)updated.EquipmentShopItems.Count), prefixDelta, stars);
            foreach (ShipItemListEntry source in original.EquipmentShopItems)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
            AddPlanetWarriorPatches(patches, original, updated, prefixDelta, stars);
        }

        private static void ValidatePlanetWarriors(PlanetHeaderRecord original,
            PlanetHeaderRecord updated)
        {
            if (original.Warriors == null || updated.Warriors == null ||
                original.WarriorCount != original.Warriors.Count ||
                updated.WarriorCount != updated.Warriors.Count ||
                updated.Warriors.Count > original.Warriors.Count ||
                updated.HasWarriorList != original.HasWarriorList)
                throw new InvalidOperationException(
                    "TPlanet.Warriors: неверный счётчик или добавление нового TShip.");
            int sourceIndex = 0;
            foreach (PlanetWarriorRecord value in updated.Warriors)
            {
                if (value == null)
                    throw new InvalidOperationException("TPlanet.Warriors: пустая запись.");
                while (sourceIndex < original.Warriors.Count &&
                    original.Warriors[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.Warriors.Count)
                    throw new InvalidOperationException(
                        "TPlanet.Warriors: разрешены только исходный порядок и удаление кораблей.");
                PlanetWarriorRecord source = original.Warriors[sourceIndex++];
                if (value.Start != source.Start || value.End != source.End ||
                    value.ShipStart != source.ShipStart || value.ShipType != source.ShipType ||
                    value.ShipObjectId != source.ShipObjectId)
                    throw new InvalidOperationException(
                        "TPlanet.Warriors: структура и идентификатор TShip доступны только для чтения.");
            }
        }

        private void AddPlanetWarriorPatches(List<PayloadPatch> patches,
            PlanetHeaderRecord original, PlanetHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (!original.HasWarriorList) return;
            Dictionary<int, PlanetWarriorRecord> retained =
                new Dictionary<int, PlanetWarriorRecord>();
            foreach (PlanetWarriorRecord value in updated.Warriors) retained.Add(value.Start, value);
            if (updated.Warriors.Count != original.Warriors.Count)
                AddUInt16Patch(patches, original.WarriorCountOffset,
                    checked((ushort)original.Warriors.Count),
                    checked((ushort)updated.Warriors.Count), prefixDelta, stars);
            foreach (PlanetWarriorRecord source in original.Warriors)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddPlanetRelationPatches(List<PayloadPatch> patches, PlanetHeaderRecord original,
            PlanetHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.RelationToRangers == null || updated.RelationToRangers == null ||
                original.RelationCount != original.RelationToRangers.Length ||
                updated.RelationCount != updated.RelationToRangers.Length ||
                updated.RelationCount > original.RelationCount ||
                original.RelationCountOffset <= 0 ||
                original.RelationEndOffset != original.RelationCountOffset + 2 + original.RelationCount)
                throw new InvalidOperationException(
                    "TPlanet.RelationToRangers: неверный размер или добавление новой связи.");
            if (updated.RelationCount != original.RelationCount)
            {
                if (!IsOrderedByteSubset(original.RelationToRangers,
                    updated.RelationToRangers))
                    throw new InvalidOperationException(
                        "TPlanet.RelationToRangers: уменьшенный список должен сохранять исходный порядок.");
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, updated.RelationCount);
                    encoded.Write(updated.RelationToRangers, 0, updated.RelationToRangers.Length);
                    patches.Add(new PayloadPatch(MapKnownOffset(original.RelationCountOffset,
                        prefixDelta, stars), original.RelationEndOffset -
                        original.RelationCountOffset, encoded.ToArray()));
                }
                return;
            }
            for (int index = 0; index < updated.RelationToRangers.Length; index++)
            {
                if (updated.RelationToRangers[index] != original.RelationToRangers[index] &&
                    updated.RelationToRangers[index] > 100)
                    throw new InvalidOperationException(
                        "TPlanet.RelationToRangers: отношение должно быть от 0 до 100.");
                AddBytePatch(patches, original.RelationCountOffset + 2 + index,
                    original.RelationToRangers[index], updated.RelationToRangers[index],
                    prefixDelta, stars);
            }
        }

        private void AddPlanetSputnikPatch(List<PayloadPatch> patches, PlanetHeaderRecord original,
            PlanetHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            bool equal = original.Satellites.Count == updated.Satellites.Count;
            if (equal)
                for (int index = 0; index < original.Satellites.Count; index++)
                    if (!original.Satellites[index].ContentEquals(updated.Satellites[index]))
                    {
                        equal = false;
                        break;
                    }
            if (equal) return;
            if (updated.Satellites.Count > original.Satellites.Count ||
                updated.Satellites.Count > ushort.MaxValue ||
                original.SatelliteEndOffset < original.SatelliteCountOffset + 2 ||
                original.SatelliteEndOffset > original.FlagsOffset - 2)
                throw new InvalidOperationException("TPlanet.TSputnik: добавление новых зондов или неверная граница списка.");

            int previousSourceIndex = -1;
            foreach (PlanetSputnikRecord value in updated.Satellites)
            {
                if (value == null || !IsSupportedEditableText(value.GraphName, 32768, false) ||
                    float.IsNaN(value.AngleCurrent) || float.IsInfinity(value.AngleCurrent) ||
                    value.OpaqueData == null)
                    throw new InvalidOperationException("TPlanet.TSputnik: неверная графика, угол или служебные данные.");
                int sourceIndex = -1;
                for (int index = previousSourceIndex + 1; index < original.Satellites.Count; index++)
                    if (original.Satellites[index].Start == value.Start)
                    {
                        sourceIndex = index;
                        break;
                    }
                if (sourceIndex < 0)
                    throw new InvalidOperationException("TPlanet.TSputnik: разрешены только правка и удаление исходных зондов.");
                PlanetSputnikRecord source = original.Satellites[sourceIndex];
                if (value.ObjectId != source.ObjectId || !EqualBytes(value.OpaqueData, source.OpaqueData))
                    throw new InvalidOperationException("TPlanet.TSputnik: ID и служебный буфер доступны только для чтения.");
                previousSourceIndex = sourceIndex;
            }

            using (MemoryStream encoded = new MemoryStream())
            {
                WriteUInt16(encoded, checked((ushort)updated.Satellites.Count));
                foreach (PlanetSputnikRecord value in updated.Satellites)
                {
                    WriteUInt32(encoded, value.ObjectId);
                    WriteUtf16Z(encoded, value.GraphName ?? string.Empty);
                    WriteUInt32(encoded, checked((uint)value.OpaqueData.Length));
                    encoded.Write(value.OpaqueData, 0, value.OpaqueData.Length);
                    WriteSingle(encoded, value.AngleCurrent);
                }
                patches.Add(new PayloadPatch(MapKnownOffset(original.SatelliteCountOffset, prefixDelta, stars),
                    original.SatelliteEndOffset - original.SatelliteCountOffset, encoded.ToArray()));
            }
        }

        private void AddPlanetGoneItemPatches(List<PayloadPatch> patches, PlanetHeaderRecord original,
            PlanetHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.GoneItems == null || updated.GoneItems == null ||
                original.GoneItemCountOffset != original.SatelliteEndOffset ||
                original.GoneItemEndOffset != original.FlagsOffset ||
                updated.GoneItems.Count > original.GoneItems.Count ||
                updated.GoneItems.Count > ushort.MaxValue)
                throw new InvalidOperationException("TPlanet.GoneItems: добавление новых предметов или неверная граница списка.");

            Dictionary<int, PlanetGoneItemRecord> retained = new Dictionary<int, PlanetGoneItemRecord>();
            int sourceIndex = 0;
            foreach (PlanetGoneItemRecord value in updated.GoneItems)
            {
                if (value == null)
                    throw new InvalidOperationException("TPlanet.GoneItems: пустая запись.");
                while (sourceIndex < original.GoneItems.Count &&
                    original.GoneItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.GoneItems.Count)
                    throw new InvalidOperationException("TPlanet.GoneItems: разрешены только правка и удаление исходных записей.");
                PlanetGoneItemRecord source = original.GoneItems[sourceIndex++];
                if (value.End != source.End || value.FactoryDiscriminatorOffset != source.FactoryDiscriminatorOffset ||
                    value.ItemType != source.ItemType || value.ItemStart != source.ItemStart ||
                    value.ItemObjectId != source.ItemObjectId)
                    throw new InvalidOperationException("TPlanet.GoneItems: структура и идентификатор вложенного TItem доступны только для чтения.");
                retained.Add(source.Start, value);
            }

            if (updated.GoneItems.Count != original.GoneItems.Count)
                AddUInt16Patch(patches, original.GoneItemCountOffset, checked((ushort)original.GoneItems.Count),
                    checked((ushort)updated.GoneItems.Count), prefixDelta, stars);
            foreach (PlanetGoneItemRecord source in original.GoneItems)
            {
                PlanetGoneItemRecord value;
                if (!retained.TryGetValue(source.Start, out value))
                {
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
                    continue;
                }
                AddBytePatch(patches, source.Start, source.PosX, value.PosX, prefixDelta, stars);
                AddBytePatch(patches, source.Start + 1, source.PosY, value.PosY, prefixDelta, stars);
                AddBytePatch(patches, source.Start + 2, source.LandType, value.LandType, prefixDelta, stars);
                AddInt32Patch(patches, source.Start + 3, source.Region, value.Region, prefixDelta, stars);
                AddBytePatch(patches, source.Start + 7, source.Miss ? (byte)1 : (byte)0,
                    value.Miss ? (byte)1 : (byte)0, prefixDelta, stars);
            }
        }

        private void AddShipPreCommonPatches(List<PayloadPatch> patches, ShipHeaderRecord original,
            ShipHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (!original.HasPreCommonCollections || !updated.HasPreCommonCollections ||
                original.PreCommonTailEnd != original.CommonTailOffset ||
                updated.PreCommonTailEnd != original.PreCommonTailEnd ||
                updated.EquipmentItems == null || updated.ArtefactItems == null ||
                updated.DropListItems == null || updated.SpecialBonuses == null ||
                updated.StatusEffects == null || updated.CustomShipInfos == null ||
                updated.TakeItemReferenceIds == null || updated.RecentlyDroppedItemIds == null)
                throw new InvalidOperationException("TShip: неверные границы вложенных коллекций.");
            if (updated.EquipmentItemCount != updated.EquipmentItems.Count)
                throw new InvalidOperationException("TShip: число оборудования не совпадает со списком.");

            AddShipItemListDeletionPatches(patches, original.FixedPrefixEnd - 2,
                original.EquipmentItems, updated.EquipmentItems, prefixDelta, stars, "оборудование");
            AddShipItemListDeletionPatches(patches, original.ArtefactCountOffset,
                original.ArtefactItems, updated.ArtefactItems, prefixDelta, stars, "артефакты");
            AddShipItemListDeletionPatches(patches, original.DropListCountOffset,
                original.DropListItems, updated.DropListItems, prefixDelta, stars, "drop-list");

            bool bonusesEqual = original.SpecialBonuses.Count == updated.SpecialBonuses.Count;
            for (int index = 0; bonusesEqual && index < original.SpecialBonuses.Count; index++)
                bonusesEqual = original.SpecialBonuses[index].BonusType == updated.SpecialBonuses[index].BonusType &&
                    original.SpecialBonuses[index].Value == updated.SpecialBonuses[index].Value;
            if (!bonusesEqual)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.SpecialBonuses.Count));
                    foreach (ShipSpecialBonusRecord record in updated.SpecialBonuses)
                    {
                        if (record == null) throw new InvalidOperationException("TShip: пустой специальный бонус.");
                        encoded.WriteByte(record.BonusType); WriteInt32(encoded, record.Value);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.SpecialBonusCountOffset, prefixDelta, stars),
                        original.StatusEffectCountOffset - original.SpecialBonusCountOffset, encoded.ToArray()));
                }
            }

            bool effectsEqual = original.StatusEffects.Count == updated.StatusEffects.Count;
            for (int index = 0; effectsEqual && index < original.StatusEffects.Count; index++)
                effectsEqual = original.StatusEffects[index].EffectType == updated.StatusEffects[index].EffectType &&
                    original.StatusEffects[index].Value == updated.StatusEffects[index].Value &&
                    original.StatusEffects[index].LastSourceShipId == updated.StatusEffects[index].LastSourceShipId;
            if (!effectsEqual)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.StatusEffects.Count));
                    foreach (ShipStatusEffectRecord record in updated.StatusEffects)
                    {
                        if (record == null || !IsSupportedMissileScalar(record.Value) ||
                            record.LastSourceShipId > 10000000)
                            throw new InvalidOperationException("TShip: неверный статус-эффект.");
                        encoded.WriteByte(record.EffectType); WriteSingle(encoded, record.Value);
                        WriteUInt32(encoded, record.LastSourceShipId);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.StatusEffectCountOffset, prefixDelta, stars),
                        original.CustomShipInfoCountOffset - original.StatusEffectCountOffset, encoded.ToArray()));
                }
            }

            bool infosEqual = original.CustomShipInfos.Count == updated.CustomShipInfos.Count;
            for (int index = 0; infosEqual && index < original.CustomShipInfos.Count; index++)
            {
                CustomShipInfoRecord left = original.CustomShipInfos[index], right = updated.CustomShipInfos[index];
                infosEqual = left.Name == right.Name && left.Description == right.Description &&
                    left.Data1 == right.Data1 && left.Data2 == right.Data2 && left.Data3 == right.Data3 &&
                    left.TextData1 == right.TextData1 && left.TextData2 == right.TextData2 &&
                    left.TextData3 == right.TextData3;
            }
            if (!infosEqual)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteInt32(encoded, updated.CustomShipInfos.Count);
                    foreach (CustomShipInfoRecord record in updated.CustomShipInfos)
                    {
                        if (record == null || !IsSupportedItemText(record.Name, 32768) ||
                            !IsSupportedItemText(record.Description, 32768) ||
                            !IsSupportedItemText(record.TextData1, 32768) ||
                            !IsSupportedItemText(record.TextData2, 32768) ||
                            !IsSupportedItemText(record.TextData3, 32768))
                            throw new InvalidOperationException("TCustomShipInfo: неверное строковое поле.");
                        WriteUtf16Z(encoded, record.Name ?? string.Empty);
                        WriteUtf16Z(encoded, record.Description ?? string.Empty);
                        WriteInt32(encoded, record.Data1); WriteInt32(encoded, record.Data2);
                        WriteInt32(encoded, record.Data3);
                        WriteUtf16Z(encoded, record.TextData1 ?? string.Empty);
                        WriteUtf16Z(encoded, record.TextData2 ?? string.Empty);
                        WriteUtf16Z(encoded, record.TextData3 ?? string.Empty);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.CustomShipInfoCountOffset, prefixDelta, stars),
                        original.TakeItemReferenceCountOffset - original.CustomShipInfoCountOffset, encoded.ToArray()));
                }
            }

            AddUInt32ReferenceListPatch(patches, original.TakeItemReferenceCountOffset,
                original.RecentlyDroppedItemCountOffset, original.TakeItemReferenceIds,
                updated.TakeItemReferenceIds, prefixDelta, stars, "take-items");
            int originalPartnerOffset = original.RecentlyDroppedItemCountOffset + 2 +
                original.RecentlyDroppedItemIds.Count * 4;
            AddUInt32ReferenceListPatch(patches, original.RecentlyDroppedItemCountOffset,
                originalPartnerOffset, original.RecentlyDroppedItemIds, updated.RecentlyDroppedItemIds,
                prefixDelta, stars, "recently-dropped-items");
            if (updated.GoodShipId > 10000000 || updated.BadShipId > 10000000 ||
                updated.PartnerShipId > 10000000)
                throw new InvalidOperationException("TShip: неверная ссылка good/bad/partner.");
            if (original.GoodShipId != updated.GoodShipId || original.BadShipId != updated.BadShipId ||
                original.PartnerShipId != updated.PartnerShipId || original.PartnerGood != updated.PartnerGood)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt32(encoded, updated.GoodShipId); WriteUInt32(encoded, updated.BadShipId);
                    WriteUInt32(encoded, updated.PartnerShipId);
                    if (updated.PartnerShipId != 0) WriteInt32(encoded, updated.PartnerGood);
                    patches.Add(new PayloadPatch(MapKnownOffset(originalPartnerOffset, prefixDelta, stars),
                        original.CommonTailOffset - originalPartnerOffset, encoded.ToArray()));
                }
            }
        }

        private void AddShipItemListDeletionPatches(List<PayloadPatch> patches, int countOffset,
            List<ShipItemListEntry> original, List<ShipItemListEntry> updated, int prefixDelta,
            IList<StarHeaderRecord> stars, string label)
        {
            if (original == null || updated == null || updated.Count > original.Count)
                throw new InvalidOperationException("TShip: добавление в список «" + label + "» требует нового TItem.");
            Dictionary<int, ShipItemListEntry> retained = new Dictionary<int, ShipItemListEntry>();
            int sourceIndex = 0;
            foreach (ShipItemListEntry value in updated)
            {
                while (sourceIndex < original.Count && original[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.Count)
                    throw new InvalidOperationException("TShip: перестановка списка «" + label + "» не разрешена.");
                ShipItemListEntry source = original[sourceIndex++];
                if (value.ItemType != source.ItemType || value.ItemObjectId != source.ItemObjectId ||
                    value.ItemStart != source.ItemStart || value.End != source.End)
                    throw new InvalidOperationException("TShip: идентификатор вложенного TItem доступен только для чтения.");
                retained.Add(source.Start, value);
            }
            if (updated.Count != original.Count)
                AddUInt16Patch(patches, countOffset, checked((ushort)original.Count),
                    checked((ushort)updated.Count), prefixDelta, stars);
            foreach (ShipItemListEntry source in original)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddUInt32ReferenceListPatch(List<PayloadPatch> patches, int start, int end,
            List<uint> original, List<uint> updated, int prefixDelta, IList<StarHeaderRecord> stars, string label)
        {
            bool equal = original != null && updated != null && original.Count == updated.Count;
            for (int index = 0; equal && index < original.Count; index++) equal = original[index] == updated[index];
            if (equal) return;
            if (updated == null || updated.Count > 10000)
                throw new InvalidOperationException("TShip: неверный список «" + label + "».");
            using (MemoryStream encoded = new MemoryStream())
            {
                WriteUInt16(encoded, checked((ushort)updated.Count));
                foreach (uint value in updated)
                {
                    if (value > 10000000) throw new InvalidOperationException("TShip: неверная объектная ссылка.");
                    WriteUInt32(encoded, value);
                }
                patches.Add(new PayloadPatch(MapKnownOffset(start, prefixDelta, stars), end - start, encoded.ToArray()));
            }
        }

        private void AddPlayerStorageItemPatches(List<PayloadPatch> patches,
            ShipHeaderRecord original, ShipHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (!original.HasPlayerStorageItems) return;
            if (original.PlayerStorageItems == null || updated.PlayerStorageItems == null ||
                updated.PlayerStorageItems.Count > original.PlayerStorageItems.Count ||
                original.PlayerObjectStateCount != original.PlayerStorageItems.Count ||
                updated.PlayerObjectStateCount != updated.PlayerStorageItems.Count)
                throw new InvalidOperationException(
                    "TPlayer.StorageItems: неверный счётчик или добавление нового TItem.");

            HashSet<uint> stationIds = new HashSet<uint>();
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.IsStation) stationIds.Add(ship.ObjectId);
            Dictionary<int, PlayerStorageItemRecord> retained =
                new Dictionary<int, PlayerStorageItemRecord>();
            int sourceIndex = 0;
            foreach (PlayerStorageItemRecord value in updated.PlayerStorageItems)
            {
                while (sourceIndex < original.PlayerStorageItems.Count &&
                    original.PlayerStorageItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.PlayerStorageItems.Count)
                    throw new InvalidOperationException(
                        "TPlayer.StorageItems: перестановка или добавление записей не разрешены.");
                PlayerStorageItemRecord source = original.PlayerStorageItems[sourceIndex++];
                if (value.ItemType != source.ItemType || value.ItemStart != source.ItemStart ||
                    value.ItemObjectId != source.ItemObjectId || value.End != source.End)
                    throw new InvalidOperationException(
                        "TPlayer.StorageItems: замена вложенного TItem требует обновления ссылок.");
                if (value.IsStation ? !stationIds.Contains(value.PlaceObjectId) :
                    !IsKnownPlanetObjectId(value.PlaceObjectId))
                    throw new InvalidOperationException(
                        "TPlayer.StorageItems: место хранения не разрешается в текущем SAV.");
                int record = source.Start;
                AddBytePatch(patches, record, source.IsStation ? (byte)1 : (byte)0,
                    value.IsStation ? (byte)1 : (byte)0, prefixDelta, stars);
                AddUInt32Patch(patches, record + 1, source.PlaceObjectId,
                    value.PlaceObjectId, prefixDelta, stars);
                AddInt32Patch(patches, record + 5, source.Slot,
                    value.Slot, prefixDelta, stars);
                retained.Add(source.Start, value);
            }
            if (updated.PlayerStorageItems.Count != original.PlayerStorageItems.Count)
                AddInt32Patch(patches, original.PlayerStorageItemCountOffset,
                    original.PlayerStorageItems.Count, updated.PlayerStorageItems.Count,
                    prefixDelta, stars);
            foreach (PlayerStorageItemRecord source in original.PlayerStorageItems)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddPlayerSatellitePatches(List<PayloadPatch> patches,
            ShipHeaderRecord original, ShipHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            if (!original.HasPlayerFinancialTail) return;
            if (original.PlayerSatelliteItems == null || updated.PlayerSatelliteItems == null ||
                updated.PlayerSatelliteItems.Count > original.PlayerSatelliteItems.Count ||
                original.PlayerSatelliteCount != original.PlayerSatelliteItems.Count ||
                updated.PlayerSatelliteCount != updated.PlayerSatelliteItems.Count ||
                original.PlayerSatelliteEndOffset < original.PlayerSatelliteListOffset + 4)
                throw new InvalidOperationException(
                    "TPlayer.Satellites: неверный счётчик, границы или добавление нового TSatellite.");

            Dictionary<int, ShipItemListEntry> retained =
                new Dictionary<int, ShipItemListEntry>();
            int sourceIndex = 0;
            foreach (ShipItemListEntry value in updated.PlayerSatelliteItems)
            {
                while (sourceIndex < original.PlayerSatelliteItems.Count &&
                    original.PlayerSatelliteItems[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= original.PlayerSatelliteItems.Count)
                    throw new InvalidOperationException(
                        "TPlayer.Satellites: перестановка или добавление записей не разрешены.");
                ShipItemListEntry source = original.PlayerSatelliteItems[sourceIndex++];
                if (value.ItemType != source.ItemType || value.ItemType != 73 ||
                    value.ItemStart != source.ItemStart ||
                    value.ItemObjectId != source.ItemObjectId || value.End != source.End)
                    throw new InvalidOperationException(
                        "TPlayer.Satellites: замена вложенного TSatellite не разрешена.");
                retained.Add(source.Start, value);
            }
            if (updated.PlayerSatelliteItems.Count != original.PlayerSatelliteItems.Count)
                AddInt32Patch(patches, original.PlayerSatelliteListOffset,
                    original.PlayerSatelliteItems.Count, updated.PlayerSatelliteItems.Count,
                    prefixDelta, stars);
            foreach (ShipItemListEntry source in original.PlayerSatelliteItems)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddShipHeaderPatches(List<PayloadPatch> patches, ShipHeaderRecord original,
            ShipHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            bool fullGalaxyShip = original.HasPreCommonCollections || updated.HasPreCommonCollections;
            if (updated.ObjectId != original.ObjectId || updated.Type != original.Type ||
                updated.IsPlayer != original.IsPlayer || updated.HomePlanetId != original.HomePlanetId ||
                updated.CurrentStarId != original.CurrentStarId || updated.CurrentPlanetId != original.CurrentPlanetId ||
                updated.CurrentShipId != original.CurrentShipId && updated.CurrentShipId != 0)
                throw new InvalidOperationException("TShip: изменение ID, производного типа или базовых ссылок не разрешено.");
            if (updated.Name != original.Name &&
                    !IsSupportedEditableText(updated.Name, 80, !fullGalaxyShip) ||
                updated.ScriptName != original.ScriptName &&
                    !IsSupportedEditableText(updated.ScriptName, 128, true) ||
                updated.X != original.X &&
                    !(fullGalaxyShip ? IsSupportedShipCoordinate(updated.X) : IsSupportedMissileScalar(updated.X)) ||
                updated.Y != original.Y &&
                    !(fullGalaxyShip ? IsSupportedShipCoordinate(updated.Y) : IsSupportedMissileScalar(updated.Y)) ||
                updated.Owner != original.Owner && updated.Owner > 7 ||
                updated.PilotRace != original.PilotRace && updated.PilotRace > 4 ||
                updated.Goods == null ||
                updated.Goods.GetLength(0) != 8 || updated.Goods.GetLength(1) != 4)
                throw new InvalidOperationException("TShip: неверное имя, координаты, владелец, раса пилота или товары.");

            int nameStart = original.Start + 4;
            if (updated.Name != original.Name)
                patches.Add(new PayloadPatch(MapKnownOffset(nameStart, prefixDelta, stars),
                    original.NameEnd - nameStart, EncodeUtf16Z(updated.Name)));
            if (updated.ScriptName != original.ScriptName)
                patches.Add(new PayloadPatch(MapKnownOffset(original.NameEnd, prefixDelta, stars),
                    original.ScriptNameEnd - original.NameEnd, EncodeUtf16Z(updated.ScriptName)));
            int typeOffset = original.ScriptNameEnd;
            AddBytePatch(patches, typeOffset + 1, original.Owner, updated.Owner, prefixDelta, stars);
            AddFloatPatch(patches, typeOffset + 2, original.X, updated.X, prefixDelta, stars);
            AddFloatPatch(patches, typeOffset + 6, original.Y, updated.Y, prefixDelta, stars);
            AddUInt32Patch(patches, typeOffset + 22, original.CurrentShipId,
                updated.CurrentShipId, prefixDelta, stars);
            int goodsOffset = typeOffset + 26;
            for (int good = 0; good < 8; good++)
                for (int field = 0; field < 4; field++)
                    AddUInt32Patch(patches, goodsOffset + good * 16 + field * 4,
                        original.Goods[good, field], updated.Goods[good, field], prefixDelta, stars);
            AddUInt32Patch(patches, typeOffset + 154, original.Money, updated.Money, prefixDelta, stars);
            AddUInt32Patch(patches, typeOffset + 158, original.Rnd, updated.Rnd, prefixDelta, stars);
            AddUInt32Patch(patches, typeOffset + 162, original.RndOut, updated.RndOut, prefixDelta, stars);
            AddUInt32Patch(patches, typeOffset + 166, original.Day, updated.Day, prefixDelta, stars);
            AddInt32Patch(patches, typeOffset + 170, original.Face, updated.Face, prefixDelta, stars);
            AddBytePatch(patches, typeOffset + 174, original.PilotRace, updated.PilotRace, prefixDelta, stars);

            // A TTranclucator embedded in TArtefactTranclucator starts at the
            // common TShip header but is not preceded by the three outer ship
            // item lists.  Full galaxy ships always carry the proven
            // pre-common collection boundary; the embedded writer therefore
            // must keep using its shorter, context-specific route.
            if (fullGalaxyShip)
                AddShipPreCommonPatches(patches, original, updated, prefixDelta, stars);

            if (updated.HasCommonTail != original.HasCommonTail)
                throw new InvalidOperationException("TShip: граница общего хвоста доступна только для чтения.");
            if (original.HasCommonTail)
            {
                if (updated.OrderType > 7 || updated.NoTarget > 6 || updated.CurrentStanding > 9 ||
                    updated.ChameleonSeries > 2 || updated.BlazerChameleonDetect > 1 ||
                    updated.KellerChameleonDetect > 1 || updated.TerronChameleonDetect > 1 ||
                    updated.Skills == null || updated.Skills.Length != 6 ||
                    updated.Illnesses == null || updated.Illnesses.Count != 25 ||
                    updated.RelationToRangers == null ||
                    updated.Rewards == null || updated.Rewards.Count > byte.MaxValue ||
                    !IsSupportedAsteroidScalar(updated.Angle) ||
                    !IsSupportedAsteroidScalar(updated.OrderDestinationX) ||
                    !IsSupportedAsteroidScalar(updated.OrderDestinationY) ||
                    !IsSupportedAsteroidScalar(updated.RadiusStop) ||
                    !IsSupportedAsteroidScalar(updated.AverageEquipmentValue) ||
                    !IsSupportedAsteroidScalar(updated.AverageMoneyToCapital) ||
                    !IsSupportedAsteroidScalar(updated.AverageFreeSpaceRatio) ||
                    !IsSupportedAsteroidScalar(updated.RatioOfTooCostlyEquipmentInShop) ||
                    !IsSupportedShipGraphName(updated.GraphName) ||
                    !IsSupportedEditableText(updated.SwarmAnimation, 128, true))
                    throw new InvalidOperationException("TShip: неверное поле общего хвоста.");

                int tail = original.CommonTailOffset;
                AddBytePatch(patches, tail, original.Forsage ? (byte)1 : (byte)0,
                    updated.Forsage ? (byte)1 : (byte)0, prefixDelta, stars);
                AddFloatPatch(patches, tail + 1, original.Angle, updated.Angle, prefixDelta, stars);
                AddBytePatch(patches, tail + 5, original.OrderType, updated.OrderType, prefixDelta, stars);
                AddUInt32Patch(patches, tail + 6, original.OrderData, updated.OrderData, prefixDelta, stars);
                AddUInt32Patch(patches, tail + 10, original.OrderObjectId, updated.OrderObjectId, prefixDelta, stars);
                AddFloatPatch(patches, tail + 14, original.OrderDestinationX, updated.OrderDestinationX, prefixDelta, stars);
                AddFloatPatch(patches, tail + 18, original.OrderDestinationY, updated.OrderDestinationY, prefixDelta, stars);
                AddBytePatch(patches, tail + 22, original.OrderAbsolute ? (byte)1 : (byte)0,
                    updated.OrderAbsolute ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, tail + 23, original.Abducted ? (byte)1 : (byte)0,
                    updated.Abducted ? (byte)1 : (byte)0, prefixDelta, stars);
                AddInt32Patch(patches, tail + 24, original.DaysLanded, updated.DaysLanded, prefixDelta, stars);
                AddBytePatch(patches, tail + 28, original.ScriptOrderAbsolute, updated.ScriptOrderAbsolute, prefixDelta, stars);
                AddBytePatch(patches, tail + 29, original.GraphDominator ? (byte)1 : (byte)0,
                    updated.GraphDominator ? (byte)1 : (byte)0, prefixDelta, stars);
                if (original.GraphName != updated.GraphName)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.GraphNameOffset, prefixDelta, stars),
                        original.GraphNameEnd - original.GraphNameOffset, EncodeUtf16Z(updated.GraphName)));
                AddBytePatch(patches, original.GraphNameEnd, original.GraphShipTransparency,
                    updated.GraphShipTransparency, prefixDelta, stars);
                AddBytePatch(patches, original.GraphNameEnd + 1, original.InHyperSpace ? (byte)1 : (byte)0,
                    updated.InHyperSpace ? (byte)1 : (byte)0, prefixDelta, stars);
                AddFloatPatch(patches, original.GraphNameEnd + 2, original.RadiusStop,
                    updated.RadiusStop, prefixDelta, stars);

                if (original.RelationToRangers == null ||
                    original.RelationCount != original.RelationToRangers.Length ||
                    updated.RelationCount != updated.RelationToRangers.Length ||
                    updated.RelationToRangers.Length > original.RelationToRangers.Length ||
                    original.RelationCountOffset != original.GraphNameEnd + 6 ||
                    original.RelationEndOffset != original.RewardListOffset)
                    throw new InvalidOperationException(
                        "TShip.RelationToRangers: неверный размер или добавление новой связи.");
                if (updated.RelationCount != original.RelationCount)
                {
                    if (!IsOrderedByteSubset(original.RelationToRangers,
                        updated.RelationToRangers))
                        throw new InvalidOperationException(
                            "TShip.RelationToRangers: уменьшенный список должен сохранять исходный порядок.");
                    using (MemoryStream encoded = new MemoryStream())
                    {
                        WriteUInt16(encoded, updated.RelationCount);
                        encoded.Write(updated.RelationToRangers, 0,
                            updated.RelationToRangers.Length);
                        patches.Add(new PayloadPatch(MapKnownOffset(original.RelationCountOffset,
                            prefixDelta, stars), original.RelationEndOffset -
                            original.RelationCountOffset, encoded.ToArray()));
                    }
                }
                else
                for (int index = 0; index < updated.RelationToRangers.Length; index++)
                {
                    if (updated.RelationToRangers[index] != original.RelationToRangers[index] &&
                        updated.RelationToRangers[index] > 100)
                        throw new InvalidOperationException(
                            "TShip.RelationToRangers: отношение должно быть от 0 до 100.");
                    AddBytePatch(patches, original.RelationCountOffset + 2 + index,
                        original.RelationToRangers[index], updated.RelationToRangers[index],
                        prefixDelta, stars);
                }

                if (!ShipHeaderRecord.ByteListsEqual(original.Rewards, updated.Rewards))
                {
                    if (original.RewardListOffset < original.GraphNameEnd ||
                        original.RewardListEndOffset <= original.RewardListOffset ||
                        original.RewardListEndOffset != original.CommonScalarOffset)
                        throw new InvalidOperationException("TShip: неверная граница списка наград.");
                    using (MemoryStream encoded = new MemoryStream(updated.Rewards.Count + 1))
                    {
                        encoded.WriteByte((byte)updated.Rewards.Count);
                        foreach (byte reward in updated.Rewards) encoded.WriteByte(reward);
                        patches.Add(new PayloadPatch(MapKnownOffset(original.RewardListOffset,
                            prefixDelta, stars), original.RewardListEndOffset - original.RewardListOffset,
                            encoded.ToArray()));
                    }
                }

                int scalar = original.CommonScalarOffset;
                AddBytePatch(patches, scalar, original.ShipDestroy ? (byte)1 : (byte)0,
                    updated.ShipDestroy ? (byte)1 : (byte)0, prefixDelta, stars);
                for (int index = 0; index < 6; index++)
                    AddBytePatch(patches, scalar + 1 + index, original.Skills[index], updated.Skills[index], prefixDelta, stars);
                AddUInt16Patch(patches, scalar + 7, original.Protoplasm, updated.Protoplasm, prefixDelta, stars);
                AddUInt32Patch(patches, scalar + 9, original.Points, updated.Points, prefixDelta, stars);
                AddUInt32Patch(patches, scalar + 13, original.FreePoints, updated.FreePoints, prefixDelta, stars);
                AddUInt16Patch(patches, scalar + 17, original.DayWithoutPlayer, updated.DayWithoutPlayer, prefixDelta, stars);
                AddUInt16Patch(patches, scalar + 23, original.GroupOrder, updated.GroupOrder, prefixDelta, stars);
                for (int index = 0; index < 25; index++)
                {
                    ShipIllnessRecord before = original.Illnesses[index];
                    ShipIllnessRecord after = updated.Illnesses[index];
                    if (after == null || after.Index != before.Index ||
                        after.Stimulator != before.Stimulator ||
                        !IsSupportedAsteroidScalar(after.Infection))
                        throw new InvalidOperationException("TShip: неверная запись болезни/стимулятора.");
                    int illnessOffset = index < 24 ? scalar + 25 + index * 16 : scalar + 439;
                    AddFloatPatch(patches, illnessOffset, before.Infection, after.Infection,
                        prefixDelta, stars);
                    AddInt32Patch(patches, illnessOffset + 4, before.InfectionDay,
                        after.InfectionDay, prefixDelta, stars);
                    AddInt32Patch(patches, illnessOffset + 8, before.InfectionEndDay,
                        after.InfectionEndDay, prefixDelta, stars);
                    AddInt32Patch(patches, illnessOffset + 12, before.InfectionCount,
                        after.InfectionCount, prefixDelta, stars);
                }
                AddInt32Patch(patches, scalar + 409, original.LastNextDay, updated.LastNextDay, prefixDelta, stars);
                AddBytePatch(patches, scalar + 417, original.ChameleonEnabled ? (byte)1 : (byte)0,
                    updated.ChameleonEnabled ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, scalar + 418, original.ChameleonSeries, updated.ChameleonSeries, prefixDelta, stars);
                AddBytePatch(patches, scalar + 424, original.BlazerChameleonDetect,
                    updated.BlazerChameleonDetect, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 425, original.BlazerChameleonCharge,
                    updated.BlazerChameleonCharge, prefixDelta, stars);
                AddBytePatch(patches, scalar + 429, original.KellerChameleonDetect,
                    updated.KellerChameleonDetect, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 430, original.KellerChameleonCharge,
                    updated.KellerChameleonCharge, prefixDelta, stars);
                AddBytePatch(patches, scalar + 434, original.TerronChameleonDetect,
                    updated.TerronChameleonDetect, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 435, original.TerronChameleonCharge,
                    updated.TerronChameleonCharge, prefixDelta, stars);
                AddBytePatch(patches, scalar + 455, original.TechLevelKnowledge,
                    updated.TechLevelKnowledge, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 456, original.TradePenalty, updated.TradePenalty, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 460, original.TradePoints, updated.TradePoints, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 464, original.ContrabandPoints, updated.ContrabandPoints, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 468, original.RewardViewCount, updated.RewardViewCount, prefixDelta, stars);
                AddBytePatch(patches, scalar + 472, original.NoDrop ? (byte)1 : (byte)0,
                    updated.NoDrop ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, scalar + 473, original.NoTarget, updated.NoTarget, prefixDelta, stars);
                AddBytePatch(patches, scalar + 474, original.NoTalk ? (byte)1 : (byte)0,
                    updated.NoTalk ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, scalar + 475, original.NoScan ? (byte)1 : (byte)0,
                    updated.NoScan ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, scalar + 476, original.ScriptChameleon ? (byte)1 : (byte)0,
                    updated.ScriptChameleon ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, scalar + 477, original.RobbedByPlayer ? (byte)1 : (byte)0,
                    updated.RobbedByPlayer ? (byte)1 : (byte)0, prefixDelta, stars);
                AddUInt16Patch(patches, scalar + 478, original.CountOfDeflectedPlayerShots,
                    updated.CountOfDeflectedPlayerShots, prefixDelta, stars);
                AddInt32Patch(patches, scalar + 480, original.Swarmed, updated.Swarmed, prefixDelta, stars);
                AddUInt32Patch(patches, scalar + 484, original.SwarmedByShipId,
                    updated.SwarmedByShipId, prefixDelta, stars);
                string originalAnimation = original.Swarmed > 0 ? original.SwarmAnimation : string.Empty;
                string updatedAnimation = updated.Swarmed > 0 ? updated.SwarmAnimation : string.Empty;
                int animationOffset = scalar + 488;
                int originalAnimationSize = original.Swarmed > 0 ? original.SwarmAnimationEnd - animationOffset : 0;
                if (originalAnimation != updatedAnimation || (original.Swarmed > 0) != (updated.Swarmed > 0))
                    patches.Add(new PayloadPatch(MapKnownOffset(animationOffset, prefixDelta, stars),
                        originalAnimationSize, updated.Swarmed > 0 ? EncodeUtf16Z(updatedAnimation) : new byte[0]));

                int finalOffset = original.Swarmed > 0 ? original.SwarmAnimationEnd : scalar + 488;
                AddBytePatch(patches, finalOffset, original.CurrentStanding, updated.CurrentStanding, prefixDelta, stars);
                AddInt32Patch(patches, finalOffset + 1, original.AverageSpeed, updated.AverageSpeed, prefixDelta, stars);
                AddInt32Patch(patches, finalOffset + 5, original.AverageEnemySpeed, updated.AverageEnemySpeed, prefixDelta, stars);
                AddFloatPatch(patches, finalOffset + 9, original.AverageEquipmentValue,
                    updated.AverageEquipmentValue, prefixDelta, stars);
                AddInt32Patch(patches, finalOffset + 13, original.AverageCapital, updated.AverageCapital, prefixDelta, stars);
                AddFloatPatch(patches, finalOffset + 17, original.AverageMoneyToCapital,
                    updated.AverageMoneyToCapital, prefixDelta, stars);
                AddFloatPatch(patches, finalOffset + 21, original.AverageFreeSpaceRatio,
                    updated.AverageFreeSpaceRatio, prefixDelta, stars);
                AddFloatPatch(patches, finalOffset + 25, original.RatioOfTooCostlyEquipmentInShop,
                    updated.RatioOfTooCostlyEquipmentInShop, prefixDelta, stars);
            }
            if (updated.HasNormalShipTail != original.HasNormalShipTail)
                throw new InvalidOperationException("TNormalShip: граница производного хвоста доступна только для чтения.");
            if (original.HasNormalShipTail)
            {
                if (updated.CoalitionRank > 7 || updated.PirateRank > 7 ||
                    updated.LiberationPlanetId > 100000 || updated.LastPlanetId > 100000)
                    throw new InvalidOperationException("TNormalShip: неверный ранг или ссылка на планету.");
                int normal = original.NormalShipTailOffset;
                AddInt32Patch(patches, normal, original.KillAllShips, updated.KillAllShips, prefixDelta, stars);
                AddInt32Patch(patches, normal + 4, original.KillPirates, updated.KillPirates, prefixDelta, stars);
                AddInt32Patch(patches, normal + 8, original.KillDominators, updated.KillDominators, prefixDelta, stars);
                AddInt32Patch(patches, normal + 12, original.LiberationSystems, updated.LiberationSystems, prefixDelta, stars);
                AddInt32Patch(patches, normal + 16, original.KillPacifics, updated.KillPacifics, prefixDelta, stars);
                AddInt32Patch(patches, normal + 20, original.KillWarriors, updated.KillWarriors, prefixDelta, stars);
                AddInt32Patch(patches, normal + 24, original.KillRangers, updated.KillRangers, prefixDelta, stars);
                AddUInt16Patch(patches, normal + 28, original.KillInCurrentSystemDominators,
                    updated.KillInCurrentSystemDominators, prefixDelta, stars);
                AddUInt16Patch(patches, normal + 30, original.KillInCurrentSystemPirates,
                    updated.KillInCurrentSystemPirates, prefixDelta, stars);
                AddUInt16Patch(patches, normal + 32, original.KillInCurrentSystemNormals,
                    updated.KillInCurrentSystemNormals, prefixDelta, stars);
                AddUInt16Patch(patches, normal + 34, original.KillCustomInCurrentSystem,
                    updated.KillCustomInCurrentSystem, prefixDelta, stars);
                AddUInt32Patch(patches, normal + 36, original.LiberationPlanetId,
                    updated.LiberationPlanetId, prefixDelta, stars);
                AddInt32Patch(patches, normal + 40, original.LiberationKills,
                    updated.LiberationKills, prefixDelta, stars);
                AddBytePatch(patches, normal + 44, original.CoalitionRank,
                    updated.CoalitionRank, prefixDelta, stars);
                AddUInt16Patch(patches, normal + 45, original.CoalitionRankPoints,
                    updated.CoalitionRankPoints, prefixDelta, stars);
                AddBytePatch(patches, normal + 47, original.PirateRank,
                    updated.PirateRank, prefixDelta, stars);
                AddUInt32Patch(patches, normal + 48, original.PirateRankPoints,
                    updated.PirateRankPoints, prefixDelta, stars);
                AddUInt32Patch(patches, normal + 52, original.LastPlanetId,
                    updated.LastPlanetId, prefixDelta, stars);
                AddInt32Patch(patches, normal + 56, original.TurnPlayerMoneyGoods,
                    updated.TurnPlayerMoneyGoods, prefixDelta, stars);
            }
            if (updated.HasSimpleDerivedTail != original.HasSimpleDerivedTail)
                throw new InvalidOperationException("TShip: граница простого производного хвоста доступна только для чтения.");
            if (original.HasSimpleDerivedTail)
            {
                int derived = original.SimpleDerivedTailOffset;
                switch (original.Type)
                {
                    case 0:
                        if (updated.DominatorType > 7 || updated.DominatorSeries > 2 ||
                            updated.RunProgramName > 11)
                            throw new InvalidOperationException("TDominator: неверный тип, серия или программа.");
                        AddBytePatch(patches, derived, original.DominatorType, updated.DominatorType, prefixDelta, stars);
                        AddBytePatch(patches, derived + 1, original.DominatorSeries, updated.DominatorSeries, prefixDelta, stars);
                        AddInt32Patch(patches, derived + 2, original.RunProgramDate, updated.RunProgramDate, prefixDelta, stars);
                        AddBytePatch(patches, derived + 6, original.RunProgramName, updated.RunProgramName, prefixDelta, stars);
                        break;
                    case 2:
                        if (updated.TransportType > 2) throw new InvalidOperationException("TTransport: неверный тип.");
                        AddBytePatch(patches, derived, original.TransportType, updated.TransportType, prefixDelta, stars);
                        break;
                    case 3:
                        if (updated.PirateType > 3 || !IsSupportedAsteroidScalar(updated.DesireConflict))
                            throw new InvalidOperationException("TPirate: неверный тип или склонность к конфликту.");
                        AddUInt32Patch(patches, derived, original.PiratePrison, updated.PiratePrison, prefixDelta, stars);
                        AddBytePatch(patches, derived + 4, original.PirateType, updated.PirateType, prefixDelta, stars);
                        AddFloatPatch(patches, derived + 5, original.DesireConflict, updated.DesireConflict, prefixDelta, stars);
                        break;
                    case 4:
                        if (updated.WarriorType > 1) throw new InvalidOperationException("TWarrior: неверный тип.");
                        AddBytePatch(patches, derived, original.WarriorType, updated.WarriorType, prefixDelta, stars);
                        break;
                }
            }
            if (updated.HasRangerTail != original.HasRangerTail)
                throw new InvalidOperationException("TRanger: граница производного потока доступна только для чтения.");
            if (original.HasRangerTail)
            {
                if (updated.RangerMoral > 2 || updated.Courageous > 100 || updated.LastShipId > 100000 ||
                    updated.ProgramCounts == null || updated.ProgramCounts.Length != 12 ||
                    updated.RangerQuests == null || updated.RangerQuests.Count > ushort.MaxValue ||
                    updated.RangerQuestCount != updated.RangerQuests.Count)
                    throw new InvalidOperationException("TRanger: неверная мораль, смелость, ссылка или список программ.");
                int ranger = original.RangerTailOffset;
                using (MemoryStream encoded = new MemoryStream())
                {
                    encoded.WriteByte(updated.RangerStatusTrader);
                    encoded.WriteByte(updated.RangerStatusPirate);
                    encoded.WriteByte(updated.RangerStatusWarrior);
                    encoded.WriteByte(updated.EminentPointsTrader);
                    encoded.WriteByte(updated.EminentPointsPirate);
                    encoded.WriteByte(updated.EminentPointsWarrior);
                    encoded.WriteByte(updated.RangerMoral);
                    encoded.WriteByte(updated.Courageous);
                    WriteUInt16(encoded, checked((ushort)updated.RangerQuests.Count));
                    for (int questIndex = 0; questIndex < updated.RangerQuests.Count; questIndex++)
                    {
                        RangerQuestRecord quest = updated.RangerQuests[questIndex];
                        RangerQuestRecord sourceQuest = original.RangerQuests != null &&
                            questIndex < original.RangerQuests.Count ? original.RangerQuests[questIndex] : null;
                        bool retainedPlanetReference = quest != null && sourceQuest != null &&
                            quest.PlanetObjectId == sourceQuest.PlanetObjectId;
                        bool retainedObjectReference = quest != null && sourceQuest != null &&
                            quest.ObjectId == sourceQuest.ObjectId;
                        if (quest == null ||
                            (quest.PlanetObjectId != 0 && !retainedPlanetReference &&
                                !IsKnownPlanetObjectId(quest.PlanetObjectId)) ||
                            (quest.ObjectId != 0 && !IsKnownShipObjectId(quest.ObjectId) &&
                                !IsKnownPlanetObjectId(quest.ObjectId) && !IsKnownStarObjectId(quest.ObjectId) &&
                                !retainedObjectReference) ||
                            !IsSupportedItemText(quest.Text, 32768) ||
                            !IsSupportedItemText(quest.Congratulations, 32768) ||
                            !IsSupportedItemText(quest.SpecialText, 32768))
                            throw new InvalidOperationException("TRanger: квест содержит неверную ссылку или текст.");
                        encoded.WriteByte(quest.Type);
                        WriteUInt16(encoded, quest.Number);
                        WriteUInt32(encoded, quest.PlanetObjectId);
                        WriteInt32(encoded, quest.Turn);
                        WriteInt32(encoded, quest.Reward);
                        WriteUInt32(encoded, quest.ObjectId);
                        WriteBoolean(encoded, quest.Successful);
                        WriteUtf16Z(encoded, quest.Text ?? string.Empty);
                        WriteUtf16Z(encoded, quest.Congratulations ?? string.Empty);
                        WriteUtf16Z(encoded, quest.SpecialText ?? string.Empty);
                    }
                    encoded.WriteByte(updated.StatusChangeWarrior);
                    encoded.WriteByte(updated.StatusChangePirate);
                    encoded.WriteByte(updated.StatusChangeTrader);
                    WriteUInt32(encoded, updated.RangerPrison);
                    WriteUInt32(encoded, updated.LastShipId);
                    WriteInt32(encoded, updated.Nods);
                    for (int index = 0; index < updated.ProgramCounts.Length; index++)
                        WriteInt32(encoded, updated.ProgramCounts[index]);
                    WriteBoolean(encoded, updated.ExcludedFromRating);
                    patches.Add(new PayloadPatch(MapKnownOffset(ranger, prefixDelta, stars),
                        original.RangerPostQuestOffset + 64 - ranger, encoded.ToArray()));
                }
            }
            if (updated.HasPlayerPrefix != original.HasPlayerPrefix ||
                updated.HasPlayerStorageItems != original.HasPlayerStorageItems)
                throw new InvalidOperationException(
                    "TPlayer: граница списка складских предметов доступна только для чтения.");
            if (original.HasPlayerPrefix)
            {
                if (updated.KillDominatorsByType == null || updated.KillDominatorsByType.Length != 8 ||
                    updated.ChameleonLogic == null || updated.ChameleonLogic.Length != 3)
                    throw new InvalidOperationException("TPlayer: неверный массив типов или логики хамелеона.");
                int player = original.PlayerPrefixOffset;
                AddBytePatch(patches, player, original.PlayerPrison ? (byte)1 : (byte)0,
                    updated.PlayerPrison ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, player + 1, original.PlayerTalkLocked ? (byte)1 : (byte)0,
                    updated.PlayerTalkLocked ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, player + 2, original.PlayerScanLocked ? (byte)1 : (byte)0,
                    updated.PlayerScanLocked ? (byte)1 : (byte)0, prefixDelta, stars);
                AddInt32Patch(patches, player + 3, original.KillShipInHyperSpace,
                    updated.KillShipInHyperSpace, prefixDelta, stars);
                AddInt32Patch(patches, player + 7, original.KillShipInHole,
                    updated.KillShipInHole, prefixDelta, stars);
                for (int index = 0; index < 8; index++)
                    AddInt32Patch(patches, player + 11 + index * 4,
                        original.KillDominatorsByType[index], updated.KillDominatorsByType[index],
                        prefixDelta, stars);
                for (int index = 0; index < 3; index++)
                    AddBytePatch(patches, player + 43 + index, original.ChameleonLogic[index],
                        updated.ChameleonLogic[index], prefixDelta, stars);
                AddPlayerStorageItemPatches(patches, original, updated, prefixDelta, stars);
            }
            if (updated.HasPlayerJournal != original.HasPlayerJournal)
                throw new InvalidOperationException("TPlayer: граница списка журнала доступна только для чтения.");
            if (original.HasPlayerJournal)
            {
                if (updated.PlayerJournalRecords == null || updated.PlayerJournalRecords.Count > 10000)
                    throw new InvalidOperationException("TPlayer: неверный список журнала.");
                if (!ShipHeaderRecord.PlayerJournalEqual(original.PlayerJournalRecords,
                    updated.PlayerJournalRecords))
                {
                    using (MemoryStream encoded = new MemoryStream())
                    {
                        WriteInt32(encoded, updated.PlayerJournalRecords.Count);
                        foreach (PlayerJournalRecord record in updated.PlayerJournalRecords)
                        {
                            if (record == null || !IsSupportedItemText(record.Text, 32768))
                                throw new InvalidOperationException(
                                    "TPlayer: запись журнала содержит неверный текст.");
                            WriteInt32(encoded, record.Turn);
                            WriteUtf16Z(encoded, record.Text ?? string.Empty);
                        }
                        patches.Add(new PayloadPatch(MapKnownOffset(original.PlayerJournalListOffset,
                            prefixDelta, stars), original.PlayerJournalEndOffset -
                            original.PlayerJournalListOffset, encoded.ToArray()));
                    }
                }
            }
            if (updated.HasPlayerNews != original.HasPlayerNews)
                throw new InvalidOperationException("TPlayer: граница списка новостей доступна только для чтения.");
            if (original.HasPlayerNews)
            {
                if (updated.PlayerNewsRecords == null || updated.PlayerNewsRecords.Count > 10000)
                    throw new InvalidOperationException("TPlayer: неверный список новостей.");
                if (!ShipHeaderRecord.PlayerNewsEqual(original.PlayerNewsRecords,
                    updated.PlayerNewsRecords))
                {
                    using (MemoryStream encoded = new MemoryStream())
                    {
                        WriteUInt16(encoded, checked((ushort)updated.PlayerNewsRecords.Count));
                        foreach (GalaxyNewsRecord record in updated.PlayerNewsRecords)
                        {
                            if (record == null || !IsSupportedItemText(record.Text, 32768))
                                throw new InvalidOperationException(
                                    "TPlayer: новость содержит неверный текст.");
                            WriteUInt32(encoded, record.Id);
                            WriteUInt32(encoded, record.Turn);
                            encoded.WriteByte(record.Type);
                            WriteUtf16Z(encoded, record.Text ?? string.Empty);
                        }
                        patches.Add(new PayloadPatch(MapKnownOffset(original.PlayerNewsListOffset,
                            prefixDelta, stars), original.PlayerNewsEndOffset -
                            original.PlayerNewsListOffset, encoded.ToArray()));
                    }
                }
            }
            if (updated.HasPlayerFinancialTail != original.HasPlayerFinancialTail ||
                updated.PlayerSatelliteItems == null ||
                updated.PlayerSatelliteCount != updated.PlayerSatelliteItems.Count)
                throw new InvalidOperationException(
                    "TPlayer: неверные границы или счётчик вложенного списка спутников.");
            if (original.HasPlayerFinancialTail)
                AddPlayerSatellitePatches(patches, original, updated, prefixDelta, stars);
            if (updated.HasPlayerRobotMaps != original.HasPlayerRobotMaps)
                throw new InvalidOperationException(
                    "TPlayer: граница списка карт роботов доступна только для чтения.");
            if (original.HasPlayerRobotMaps)
            {
                if (updated.PlayerRobotMaps == null || updated.PlayerRobotMaps.Count > 10000 ||
                    updated.PlayerRobotMapCount != updated.PlayerRobotMaps.Count)
                    throw new InvalidOperationException("TPlayer: неверный список карт роботов.");
                if (!ShipHeaderRecord.PlayerRobotMapsEqual(original.PlayerRobotMaps,
                    updated.PlayerRobotMaps))
                {
                    using (MemoryStream encoded = new MemoryStream())
                    {
                        WriteInt32(encoded, updated.PlayerRobotMaps.Count);
                        foreach (PlayerRobotMapRecord record in updated.PlayerRobotMaps)
                        {
                            if (record == null)
                                throw new InvalidOperationException(
                                    "TPlayer: пустая запись карты роботов.");
                            WriteInt32(encoded, record.Id);
                            WriteInt32(encoded, record.Time);
                            WriteInt32(encoded, record.BuildRobot);
                            WriteInt32(encoded, record.KillRobot);
                            WriteInt32(encoded, record.BuildTurret);
                            WriteInt32(encoded, record.KillTurret);
                            WriteInt32(encoded, record.KillBuilding);
                            WriteInt32(encoded, record.Bonus);
                            WriteInt32(encoded, record.State);
                            WriteInt32(encoded, record.Turn);
                        }
                        patches.Add(new PayloadPatch(MapKnownOffset(
                            original.PlayerRobotMapListOffset, prefixDelta, stars),
                            original.PlayerRobotMapEndOffset - original.PlayerRobotMapListOffset,
                            encoded.ToArray()));
                    }
                }
            }
            if (original.HasPlayerFinancialTail)
            {
                HashSet<uint> starIds = new HashSet<uint>();
                foreach (StarHeaderRecord star in stars) starIds.Add(star.ObjectId);
                if (updated.PlayerInvestments == null || updated.PlayerInvestments.Length != 12 ||
                    updated.PlayerProgramsInWarBase == null || updated.PlayerProgramsInWarBase.Length != 12 ||
                    float.IsNaN(updated.PlayerDepositPercent) ||
                    float.IsInfinity(updated.PlayerDepositPercent) ||
                    updated.PlayerDepositPercent < 0.0F || updated.PlayerDepositPercent > 1000.0F ||
                    updated.PlayerFlyToStarId != 0 && !starIds.Contains(updated.PlayerFlyToStarId) ||
                    updated.PlayerHotEquipmentCurrent > 9)
                    throw new InvalidOperationException("TPlayer: неверные финансовые поля, система или наборы.");
                int financial = original.PlayerFinancialOffset;
                AddInt32Patch(patches, financial, original.PlayerDebt, updated.PlayerDebt, prefixDelta, stars);
                AddInt32Patch(patches, financial + 4, original.PlayerDebtDate,
                    updated.PlayerDebtDate, prefixDelta, stars);
                AddInt32Patch(patches, financial + 8, original.PlayerDebtCount,
                    updated.PlayerDebtCount, prefixDelta, stars);
                AddInt32Patch(patches, financial + 12, original.PlayerDeposit,
                    updated.PlayerDeposit, prefixDelta, stars);
                AddInt32Patch(patches, financial + 16, original.PlayerDepositDate,
                    updated.PlayerDepositDate, prefixDelta, stars);
                AddInt32Patch(patches, financial + 20, original.PlayerDepositDay,
                    updated.PlayerDepositDay, prefixDelta, stars);
                AddFloatPatch(patches, financial + 24, original.PlayerDepositPercent,
                    updated.PlayerDepositPercent, prefixDelta, stars);
                AddInt32Patch(patches, financial + 28, original.PlayerMedPolicy,
                    updated.PlayerMedPolicy, prefixDelta, stars);
                AddInt32Patch(patches, financial + 32, original.PlayerPirateLicense,
                    updated.PlayerPirateLicense, prefixDelta, stars);
                AddInt32Patch(patches, financial + 36, original.PlayerPiratePoints,
                    updated.PlayerPiratePoints, prefixDelta, stars);
                AddInt32Patch(patches, financial + 40, original.PlayerPirateNewPoints,
                    updated.PlayerPirateNewPoints, prefixDelta, stars);
                AddUInt32Patch(patches, financial + 44, original.PlayerFlyToStarId,
                    updated.PlayerFlyToStarId, prefixDelta, stars);
                for (int index = 0; index < 12; index++)
                    AddInt32Patch(patches, financial + 48 + index * 4,
                        original.PlayerInvestments[index], updated.PlayerInvestments[index],
                        prefixDelta, stars);
                int programs = original.PlayerProgramsOffset;
                AddBytePatch(patches, programs - 1, original.PlayerImmunity,
                    updated.PlayerImmunity, prefixDelta, stars);
                for (int index = 0; index < 12; index++)
                    AddInt32Patch(patches, programs + index * 4,
                        original.PlayerProgramsInWarBase[index], updated.PlayerProgramsInWarBase[index],
                        prefixDelta, stars);
                AddInt32Patch(patches, programs + 48, original.PlayerDayWarBaseGivePrograms,
                    updated.PlayerDayWarBaseGivePrograms, prefixDelta, stars);
                AddInt32Patch(patches, programs + 52, original.PlayerHitEnemyAfterPrograms,
                    updated.PlayerHitEnemyAfterPrograms, prefixDelta, stars);
                int late = original.PlayerLateStatsOffset;
                AddInt32Patch(patches, late, original.PlayerPlanetBattlesWin,
                    updated.PlayerPlanetBattlesWin, prefixDelta, stars);
                AddInt32Patch(patches, late + 4, original.PlayerLastPlanetBattleDate,
                    updated.PlayerLastPlanetBattleDate, prefixDelta, stars);
                AddBytePatch(patches, late + 8, original.PlayerPlanetBattlesRejected ? (byte)1 : (byte)0,
                    updated.PlayerPlanetBattlesRejected ? (byte)1 : (byte)0, prefixDelta, stars);
                AddUInt16Patch(patches, late + 9, original.PlayerIllnessCount,
                    updated.PlayerIllnessCount, prefixDelta, stars);
                AddUInt16Patch(patches, late + 11, original.PlayerStimulatorCount,
                    updated.PlayerStimulatorCount, prefixDelta, stars);
                AddUInt16Patch(patches, late + 13, original.PlayerPrisonCount,
                    updated.PlayerPrisonCount, prefixDelta, stars);
                AddInt32Patch(patches, late + 15, original.PlayerUnknownPlanetComplete,
                    updated.PlayerUnknownPlanetComplete, prefixDelta, stars);
                AddUInt16Patch(patches, late + 19, original.PlayerChangeRaceCount,
                    updated.PlayerChangeRaceCount, prefixDelta, stars);
                AddUInt16Patch(patches, late + 21, original.PlayerChangeSideCount,
                    updated.PlayerChangeSideCount, prefixDelta, stars);
                AddBytePatch(patches, late + 23, original.PlayerHotEquipmentCurrent,
                    updated.PlayerHotEquipmentCurrent, prefixDelta, stars);
                int flags = original.PlayerPreAchievementFlagsOffset;
                AddBytePatch(patches, flags, original.PlayerGoToGovernment,
                    updated.PlayerGoToGovernment, prefixDelta, stars);
                AddBytePatch(patches, flags + 1, original.PlayerNoJump ? (byte)1 : (byte)0,
                    updated.PlayerNoJump ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, flags + 2, original.PlayerPirateClanReal ? (byte)1 : (byte)0,
                    updated.PlayerPirateClanReal ? (byte)1 : (byte)0, prefixDelta, stars);
                int experience = original.PlayerExperienceOffset;
                AddInt32Patch(patches, experience, original.PlayerExperienceDominatorKills,
                    updated.PlayerExperienceDominatorKills, prefixDelta, stars);
                AddInt32Patch(patches, experience + 4, original.PlayerExperiencePirateKills,
                    updated.PlayerExperiencePirateKills, prefixDelta, stars);
                AddInt32Patch(patches, experience + 8, original.PlayerExperienceGoodShipKills,
                    updated.PlayerExperienceGoodShipKills, prefixDelta, stars);
                AddInt32Patch(patches, experience + 12, original.PlayerExperienceTrade,
                    updated.PlayerExperienceTrade, prefixDelta, stars);
                AddBytePatch(patches, experience + 16, original.PlayerCaptainOnBridge,
                    updated.PlayerCaptainOnBridge, prefixDelta, stars);
                if (updated.HasPlayerBridge != original.HasPlayerBridge ||
                    original.HasPlayerBridge && (original.PlayerBridgeRuins == null ||
                    updated.PlayerBridgeRuins == null))
                    throw new InvalidOperationException("TPlayer.Bridge: граница вложенного TRuins доступна только для чтения.");
                if (original.HasPlayerBridge)
                {
                    if (updated.PlayerBridgeCurrentShipId > 10000000 ||
                        updated.PlayerBridgeCurrentPlanetId > 10000000 ||
                        !IsSupportedEditableText(updated.PlayerBridgeBackground, 512, true))
                        throw new InvalidOperationException("TPlayer.Bridge: неверная ссылка или имя фона.");
                    AddShipHeaderPatches(patches, original.PlayerBridgeRuins,
                        updated.PlayerBridgeRuins, prefixDelta, stars);
                    bool originalHasReferences = original.PlayerCaptainOnBridge != 0;
                    bool updatedHasReferences = updated.PlayerCaptainOnBridge != 0;
                    if (originalHasReferences && updatedHasReferences)
                    {
                        AddUInt32Patch(patches, original.PlayerBridgeReferenceOffset,
                            original.PlayerBridgeCurrentShipId, updated.PlayerBridgeCurrentShipId,
                            prefixDelta, stars);
                        AddUInt32Patch(patches, original.PlayerBridgeReferenceOffset + 4,
                            original.PlayerBridgeCurrentPlanetId, updated.PlayerBridgeCurrentPlanetId,
                            prefixDelta, stars);
                    }
                    else if (!originalHasReferences && updatedHasReferences)
                    {
                        byte[] references = new byte[8];
                        Buffer.BlockCopy(BitConverter.GetBytes(updated.PlayerBridgeCurrentShipId), 0,
                            references, 0, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes(updated.PlayerBridgeCurrentPlanetId), 0,
                            references, 4, 4);
                        patches.Add(new PayloadPatch(MapKnownOffset(
                            original.PlayerBridgeRuinsEndOffset, prefixDelta, stars), 0, references));
                    }
                    else if (originalHasReferences && !updatedHasReferences)
                    {
                        patches.Add(new PayloadPatch(MapKnownOffset(
                            original.PlayerBridgeReferenceOffset, prefixDelta, stars), 8, new byte[0]));
                    }
                    if (updated.PlayerBridgeBackground != original.PlayerBridgeBackground)
                        patches.Add(new PayloadPatch(MapKnownOffset(
                            original.PlayerBridgeBackgroundOffset, prefixDelta, stars),
                            original.PlayerBridgeBackgroundEndOffset - original.PlayerBridgeBackgroundOffset,
                            EncodeUtf16Z(updated.PlayerBridgeBackground)));
                }
            }
            if (updated.HasTranclucatorTail != original.HasTranclucatorTail)
                throw new InvalidOperationException("TTranclucator: граница производного хвоста доступна только для чтения.");
            if (original.HasTranclucatorTail)
            {
                if (updated.TranclucatorProprietorShipId > 100000 ||
                    !IsSupportedEditableText(updated.TranclucatorArtSystemName, 512, true) ||
                    updated.TranclucatorSeekPermits == null || updated.TranclucatorSeekPermits.Length != 7 ||
                    updated.TranclucatorLandPermits == null || updated.TranclucatorLandPermits.Length != 2)
                    throw new InvalidOperationException("TTranclucator: неверная ссылка, строка или набор разрешений.");
                int tranclucator = original.TranclucatorTailOffset;
                AddUInt32Patch(patches, tranclucator, original.TranclucatorProprietorShipId,
                    updated.TranclucatorProprietorShipId, prefixDelta, stars);
                AddBytePatch(patches, tranclucator + 4, original.TranclucatorDocking ? (byte)1 : (byte)0,
                    updated.TranclucatorDocking ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, tranclucator + 5, original.TranclucatorSeekItems ? (byte)1 : (byte)0,
                    updated.TranclucatorSeekItems ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, tranclucator + 6, original.TranclucatorAutoArrange ? (byte)1 : (byte)0,
                    updated.TranclucatorAutoArrange ? (byte)1 : (byte)0, prefixDelta, stars);
                AddInt32Patch(patches, tranclucator + 7, original.TranclucatorArtSize,
                    updated.TranclucatorArtSize, prefixDelta, stars);
                if (original.TranclucatorArtSystemName != updated.TranclucatorArtSystemName)
                    patches.Add(new PayloadPatch(MapKnownOffset(tranclucator + 11, prefixDelta, stars),
                        original.TranclucatorArtStringEnd - tranclucator - 11,
                        EncodeOptionalString(updated.TranclucatorArtSystemName)));
                int postArt = original.TranclucatorPostArtOffset;
                for (int index = 0; index < 7; index++)
                    AddBytePatch(patches, postArt + index,
                        original.TranclucatorSeekPermits[index] ? (byte)1 : (byte)0,
                        updated.TranclucatorSeekPermits[index] ? (byte)1 : (byte)0, prefixDelta, stars);
                for (int index = 0; index < 2; index++)
                    AddBytePatch(patches, postArt + 7 + index,
                        original.TranclucatorLandPermits[index] ? (byte)1 : (byte)0,
                        updated.TranclucatorLandPermits[index] ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, postArt + 9, original.TranclucatorLandStorage ? (byte)1 : (byte)0,
                    updated.TranclucatorLandStorage ? (byte)1 : (byte)0, prefixDelta, stars);
            }
            if (updated.HasRuinsTail != original.HasRuinsTail)
                throw new InvalidOperationException("TRuins: граница производного хвоста доступна только для чтения.");
            if (original.HasRuinsTail)
            {
                if (original.RuinsEquipmentItems == null || updated.RuinsEquipmentItems == null ||
                    updated.RuinsEquipmentItemCount != updated.RuinsEquipmentItems.Count ||
                    original.RuinsEquipmentItemCount != original.RuinsEquipmentItems.Count ||
                    original.RuinsEquipmentCountOffset != original.CommonTailEnd ||
                    original.RuinsEquipmentEndOffset != original.RuinsShopTailOffset ||
                    original.RuinsSaleSatellite == null || updated.RuinsSaleSatellite == null ||
                    original.RuinsSaleSatellite.Start != original.RuinsShopTailOffset + 140 ||
                    original.RuinsSaleSatellite.End != original.RuinsFinalFlagsOffset ||
                    !original.RuinsSaleSatellite.ContentEquals(updated.RuinsSaleSatellite) ||
                    updated.RuinsShopGoods == null || updated.RuinsShopGoods.GetLength(0) != 8 ||
                    updated.RuinsShopGoods.GetLength(1) != 3 || updated.RuinsFlyToStarId > 100000)
                    throw new InvalidOperationException("TRuins: неверный список оборудования, спутник, магазин или ссылка.");
                AddShipItemListDeletionPatches(patches, original.RuinsEquipmentCountOffset,
                    original.RuinsEquipmentItems, updated.RuinsEquipmentItems,
                    prefixDelta, stars, "оборудование TRuins");
                int ruins = original.RuinsShopTailOffset;
                for (int good = 0; good < 8; good++)
                {
                    int row = ruins + good * 16;
                    AddInt32Patch(patches, row, original.RuinsShopGoods[good, 0],
                        updated.RuinsShopGoods[good, 0], prefixDelta, stars);
                    AddInt32Patch(patches, row + 8, original.RuinsShopGoods[good, 1],
                        updated.RuinsShopGoods[good, 1], prefixDelta, stars);
                    AddInt32Patch(patches, row + 12, original.RuinsShopGoods[good, 2],
                        updated.RuinsShopGoods[good, 2], prefixDelta, stars);
                }
                AddInt32Patch(patches, ruins + 128, original.RuinsEnergy,
                    updated.RuinsEnergy, prefixDelta, stars);
                AddUInt32Patch(patches, ruins + 132, original.RuinsFlyToStarId,
                    updated.RuinsFlyToStarId, prefixDelta, stars);
                AddInt32Patch(patches, ruins + 136, original.RuinsFlyDate,
                    updated.RuinsFlyDate, prefixDelta, stars);
                int flags = original.RuinsFinalFlagsOffset;
                AddBytePatch(patches, flags, original.RuinsSponsor ? (byte)1 : (byte)0,
                    updated.RuinsSponsor ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, flags + 1, original.RuinsSpecialShip ? (byte)1 : (byte)0,
                    updated.RuinsSpecialShip ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, flags + 2, original.RuinsNoLanding ? (byte)1 : (byte)0,
                    updated.RuinsNoLanding ? (byte)1 : (byte)0, prefixDelta, stars);
                AddBytePatch(patches, flags + 3, original.RuinsNoShopUpdate,
                    updated.RuinsNoShopUpdate, prefixDelta, stars);
            }
        }

        private void AddItemHeaderPatches(List<PayloadPatch> patches, ItemHeaderRecord original,
            ItemHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.ObjectId != original.ObjectId || updated.Type != original.Type)
                throw new InvalidOperationException("Изменение ID или производного типа TItem не разрешено.");
            // Live and modded saves can contain legacy scalar values outside the
            // ranges accepted for newly edited data.  Byte-identical records need
            // no validation or patches; preserve them exactly instead of making an
            // unrelated edit elsewhere impossible.
            if (original.ContentEquals(updated)) return;
            if (updated.CustomWeaponName != original.CustomWeaponName)
            {
                if (original.Type != 68 || original.CustomWeaponDiscriminatorOffset < 0 ||
                    original.CustomWeaponDiscriminatorOffset >= original.Start ||
                    !IsSupportedEditableText(updated.CustomWeaponName, 512, false))
                    throw new InvalidOperationException(
                        "TCustomWeapon: неверное новое системное имя или граница factory-обёртки.");
                int nameOffset = original.CustomWeaponDiscriminatorOffset + 1;
                patches.Add(new PayloadPatch(MapKnownOffset(nameOffset, prefixDelta, stars),
                    original.Start - nameOffset, EncodeUtf16Z(updated.CustomWeaponName)));
            }
            if (!IsSupportedMissileScalar(updated.X) || !IsSupportedMissileScalar(updated.Y) ||
                updated.NoDrop > 1 ||
                !IsSupportedEditableText(updated.Name, 512, true))
                throw new InvalidOperationException("TItem: неверные координаты, вес, имя или флаг NoDrop.");
            if (original.Type < 8 && (updated.Weight < 0 || updated.Weight > 10000 ||
                !updated.HasGoodsTail || updated.GoodsItemCount != updated.Weight))
                throw new InvalidOperationException("TGoodsItem: количество товара должно быть от 0 до 10000.");
            if (updated.HasGoodsTail != original.HasGoodsTail)
                throw new InvalidOperationException("TGoodsItem: граница производного хвоста доступна только для чтения.");

            AddFloatPatch(patches, original.Start + 5, original.X, updated.X, prefixDelta, stars);
            AddFloatPatch(patches, original.Start + 9, original.Y, updated.Y, prefixDelta, stars);
            AddInt32Patch(patches, original.Start + 13, original.Weight, updated.Weight, prefixDelta, stars);
            if (original.Owner != updated.Owner)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 17, prefixDelta, stars), 1,
                    new byte[] { updated.Owner }));
            if (original.Cost != updated.Cost)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 18, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.Cost)));
            AddInt32Patch(patches, original.Start + 22, original.ItemDestroy, updated.ItemDestroy, prefixDelta, stars);

            int optionalNameStart = original.Start + 26;
            int noDropOffset = original.BaseEnd - 1;
            if (updated.Name != original.Name)
            {
                byte[] encodedName = string.IsNullOrEmpty(updated.Name)
                    ? new byte[] { 0 }
                    : JoinBytes(new byte[] { 1 }, EncodeUtf16Z(updated.Name));
                patches.Add(new PayloadPatch(MapKnownOffset(optionalNameStart, prefixDelta, stars),
                    noDropOffset - optionalNameStart, encodedName));
            }
            if (original.NoDrop != updated.NoDrop)
                patches.Add(new PayloadPatch(MapKnownOffset(noDropOffset, prefixDelta, stars), 1,
                    new byte[] { updated.NoDrop }));
            if (original.Type < 8 && original.Weight != updated.Weight)
                patches.Add(new PayloadPatch(MapKnownOffset(original.BaseEnd, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.Weight)));
            if (original.HasGoodsTail)
                AddBytePatch(patches, original.GoodsTailOffset + 4,
                    original.GoodsItemNatural ? (byte)1 : (byte)0,
                    updated.GoodsItemNatural ? (byte)1 : (byte)0, prefixDelta, stars);
            if (original.Type >= 8)
            {
                if (!IsSupportedEditableText(updated.CustomFaction, 512, true) ||
                    !IsSupportedEditableText(updated.SystemName, 512, true) ||
                    updated.Exploitable > 1 || updated.Broken > 1 ||
                    float.IsNaN(updated.Strength) || float.IsInfinity(updated.Strength))
                    throw new InvalidOperationException("TEquipment: неверное системное имя, модовая фракция, прочность или флаг.");
                if (original.CustomFaction != updated.CustomFaction)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.BaseEnd, prefixDelta, stars),
                        original.EquipmentFirstStringEnd - original.BaseEnd,
                        EncodeOptionalString(updated.CustomFaction)));
                if (original.SystemName != updated.SystemName)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.EquipmentFirstStringEnd, prefixDelta, stars),
                        original.EquipmentSecondStringEnd - original.EquipmentFirstStringEnd,
                        EncodeOptionalString(updated.SystemName)));
                if (original.Exploitable != updated.Exploitable)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.EquipmentScalarOffset, prefixDelta, stars), 1,
                        new byte[] { updated.Exploitable }));
                AddFloatPatch(patches, original.EquipmentScalarOffset + 1, original.Strength, updated.Strength,
                    prefixDelta, stars);
                if (original.Broken != updated.Broken)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.EquipmentScalarOffset + 5, prefixDelta, stars), 1,
                        new byte[] { updated.Broken }));
                if (original.Slot != updated.Slot)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.EquipmentScalarOffset + 6, prefixDelta, stars), 1,
                        new byte[] { updated.Slot }));
                if (original.DominatorSeries != updated.DominatorSeries)
                    patches.Add(new PayloadPatch(MapKnownOffset(original.SharedPrefixEnd - 1, prefixDelta, stars), 1,
                        new byte[] { updated.DominatorSeries }));
                AddItemEquipmentReferencePatches(patches, original, updated, prefixDelta, stars);
            }
            AddItemDerivedPatches(patches, original, updated, prefixDelta, stars);
        }

        private void AddItemEquipmentReferencePatches(List<PayloadPatch> patches,
            ItemHeaderRecord original, ItemHeaderRecord updated, int prefixDelta,
            IList<StarHeaderRecord> stars)
        {
            ValidateItemIndexedReference(updated.Bonus, updated.BonusReferenceId, "Bonus");
            ValidateItemIndexedReference(updated.Special, updated.SpecialReferenceId, "Special");
            if (updated.ExtraSpecials == null || updated.ExtraSpecials.Count > 10000)
                throw new InvalidOperationException("TEquipment: неверное число дополнительных спецэффектов.");
            foreach (ItemExtraSpecialRecord record in updated.ExtraSpecials)
                ValidateItemIndexedReference(record.Special, record.ReferenceId, "ExtraSpecial");

            if (original.Bonus != updated.Bonus ||
                original.BonusReferenceId != updated.BonusReferenceId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.BonusOffset, prefixDelta, stars),
                    original.BonusEnd - original.BonusOffset,
                    EncodeIndexedReference(updated.Bonus, updated.BonusReferenceId)));
            if (original.Special != updated.Special ||
                original.SpecialReferenceId != updated.SpecialReferenceId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.SpecialOffset, prefixDelta, stars),
                    original.SpecialEnd - original.SpecialOffset,
                    EncodeIndexedReference(updated.Special, updated.SpecialReferenceId)));

            bool extrasEqual = original.ExtraSpecials != null &&
                original.ExtraSpecials.Count == updated.ExtraSpecials.Count;
            for (int index = 0; extrasEqual && index < original.ExtraSpecials.Count; index++)
                extrasEqual = original.ExtraSpecials[index].ContentEquals(updated.ExtraSpecials[index]);
            if (!extrasEqual)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteInt32(encoded, updated.ExtraSpecials.Count);
                    foreach (ItemExtraSpecialRecord record in updated.ExtraSpecials)
                    {
                        WriteInt32(encoded, record.Special);
                        if (record.Special > 0) WriteUInt32(encoded, record.ReferenceId);
                        WriteInt32(encoded, record.Count);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.ExtraSpecialCountOffset,
                        prefixDelta, stars), original.ExtraSpecialEnd - original.ExtraSpecialCountOffset,
                        encoded.ToArray()));
                }
            }
        }

        private static void ValidateItemIndexedReference(int index, uint referenceId, string field)
        {
            if (index < 0 || index > 1000000 || (index == 0 && referenceId != 0))
                throw new InvalidOperationException("TEquipment: неверные index/CRC в поле " + field + ".");
        }

        private void AddItemDerivedPatches(List<PayloadPatch> patches, ItemHeaderRecord original,
            ItemHeaderRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            List<ItemDerivedField> originalFields = original.DerivedFields == null
                ? new List<ItemDerivedField>() : original.DerivedFields;
            List<ItemDerivedField> updatedFields = updated.DerivedFields == null
                ? new List<ItemDerivedField>() : updated.DerivedFields;
            if (original.Type == 42)
            {
                ItemDerivedField originalSeries = FindDerivedField(originalFields, "edSeriesNum");
                ItemDerivedField originalCrc = FindDerivedField(originalFields, "edSeriesCRC");
                ItemDerivedField updatedSeries = FindDerivedField(updatedFields, "edSeriesNum");
                ItemDerivedField updatedCrc = FindDerivedField(updatedFields, "edSeriesCRC");
                if (originalSeries == null || updatedSeries == null ||
                    updatedSeries.IntegerValue < -1 || updatedSeries.IntegerValue > 1000000 ||
                    (updatedSeries.IntegerValue < 0) != (updatedCrc == null) ||
                    updatedCrc != null && (updatedCrc.IntegerValue < 0 ||
                        updatedCrc.IntegerValue > uint.MaxValue))
                    throw new InvalidOperationException("THull: неверные index/CRC серии корпуса.");
                bool changed = originalSeries.IntegerValue != updatedSeries.IntegerValue ||
                    (originalCrc == null) != (updatedCrc == null) ||
                    originalCrc != null && originalCrc.IntegerValue != updatedCrc.IntegerValue;
                if (changed)
                {
                    byte[] encoded = updatedCrc == null
                        ? BitConverter.GetBytes((int)updatedSeries.IntegerValue)
                        : JoinBytes(BitConverter.GetBytes((int)updatedSeries.IntegerValue),
                            BitConverter.GetBytes((uint)updatedCrc.IntegerValue));
                    int originalEnd = originalCrc == null ? originalSeries.End : originalCrc.End;
                    patches.Add(new PayloadPatch(MapKnownOffset(originalSeries.Offset,
                        prefixDelta, stars), originalEnd - originalSeries.Offset, encoded));
                }
                originalFields = WithoutHullSeriesFields(originalFields);
                updatedFields = WithoutHullSeriesFields(updatedFields);
                AddHullInterceptorPatch(patches, originalFields, updatedFields,
                    prefixDelta, stars, original.ObjectId);
                originalFields = WithoutHullInterceptorFields(originalFields);
                updatedFields = WithoutHullInterceptorFields(updatedFields);
            }
            int originalCount = originalFields.Count;
            int updatedCount = updatedFields.Count;
            if (updated.HasDerivedTail != original.HasDerivedTail || updatedCount != originalCount)
                throw new InvalidOperationException("TItem: граница производного хвоста доступна только для чтения.");
            for (int index = 0; index < originalCount; index++)
            {
                ItemDerivedField before = originalFields[index];
                ItemDerivedField after = updatedFields[index];
                if (before.ControlName != after.ControlName || before.Kind != after.Kind)
                    throw new InvalidOperationException("TItem: структура производного поля изменена.");
                if (before.ControlName.StartsWith("$", StringComparison.Ordinal) &&
                    !before.ContentEquals(after))
                    throw new InvalidOperationException("TItem: структурный флаг " +
                        before.ControlName + " доступен только для чтения.");
                int mapped = MapKnownOffset(before.Offset, prefixDelta, stars);
                switch (before.Kind)
                {
                    case ItemDerivedField.Byte:
                    case ItemDerivedField.Boolean:
                        if (after.IntegerValue < 0 || after.IntegerValue > byte.MaxValue ||
                            (before.Kind == ItemDerivedField.Boolean && after.IntegerValue > 1))
                            throw new InvalidOperationException("TItem: производное Byte/Boolean поле вне диапазона.");
                        if (before.IntegerValue != after.IntegerValue)
                            patches.Add(new PayloadPatch(mapped, 1, new byte[] { (byte)after.IntegerValue }));
                        break;
                    case ItemDerivedField.UInt16:
                        if (after.IntegerValue < 0 || after.IntegerValue > ushort.MaxValue)
                            throw new InvalidOperationException("TItem: производное UInt16 поле вне диапазона.");
                        if (before.IntegerValue != after.IntegerValue)
                            patches.Add(new PayloadPatch(mapped, 2, BitConverter.GetBytes((ushort)after.IntegerValue)));
                        break;
                    case ItemDerivedField.Int32:
                        if (after.IntegerValue < int.MinValue || after.IntegerValue > int.MaxValue)
                            throw new InvalidOperationException("TItem: производное Int32 поле вне диапазона.");
                        if (before.IntegerValue != after.IntegerValue)
                            patches.Add(new PayloadPatch(mapped, 4, BitConverter.GetBytes((int)after.IntegerValue)));
                        break;
                    case ItemDerivedField.UInt32:
                        if (after.IntegerValue < 0 || after.IntegerValue > uint.MaxValue)
                            throw new InvalidOperationException("TItem: производная ссылка вне диапазона UInt32.");
                        if (before.IntegerValue != after.IntegerValue)
                            patches.Add(new PayloadPatch(mapped, 4, BitConverter.GetBytes((uint)after.IntegerValue)));
                        break;
                    case ItemDerivedField.Float32:
                        if (float.IsNaN(after.FloatValue) || float.IsInfinity(after.FloatValue))
                            throw new InvalidOperationException("TItem: производное Float32 поле не является конечным.");
                        if (before.FloatValue != after.FloatValue)
                            patches.Add(new PayloadPatch(mapped, 4, BitConverter.GetBytes(after.FloatValue)));
                        break;
                    case ItemDerivedField.String:
                        if (!IsSupportedItemText(after.StringValue, 4096))
                            throw new InvalidOperationException("TItem type " + original.Type + ": производная строка " +
                                after.ControlName + " недопустима (длина " +
                                (after.StringValue == null ? -1 : after.StringValue.Length) + ").");
                        if (before.StringValue != after.StringValue)
                        {
                            byte[] encoded = before.IntegerValue == 1
                                ? EncodeOptionalString(after.StringValue) : EncodeUtf16Z(after.StringValue);
                            patches.Add(new PayloadPatch(mapped, before.End - before.Offset, encoded));
                        }
                        break;
                    default:
                        throw new InvalidOperationException("TItem: неизвестный формат производного поля.");
                }
            }
            if ((original.NestedTranclucator == null) != (updated.NestedTranclucator == null))
                throw new InvalidOperationException("TArtefactTranclucator: вложенный объект доступен только в доказанной границе.");
            if (original.NestedTranclucator != null)
                AddShipHeaderPatches(patches, original.NestedTranclucator,
                    updated.NestedTranclucator, prefixDelta, stars);
        }

        private static ItemDerivedField FindDerivedField(List<ItemDerivedField> fields,
            string controlName)
        {
            foreach (ItemDerivedField field in fields)
                if (field.ControlName == controlName) return field;
            return null;
        }

        private static List<ItemDerivedField> WithoutHullSeriesFields(List<ItemDerivedField> fields)
        {
            List<ItemDerivedField> result = new List<ItemDerivedField>();
            foreach (ItemDerivedField field in fields)
                if (field.ControlName != "edSeriesNum" && field.ControlName != "edSeriesCRC")
                    result.Add(field);
            return result;
        }

        private void AddHullInterceptorPatch(List<PayloadPatch> patches,
            List<ItemDerivedField> originalFields, List<ItemDerivedField> updatedFields,
            int prefixDelta, IList<StarHeaderRecord> stars, uint objectId)
        {
            ItemDerivedField originalFlag = FindDerivedField(originalFields,
                "$HullHasInterceptors");
            ItemDerivedField updatedFlag = FindDerivedField(updatedFields,
                "$HullHasInterceptors");
            ItemDerivedField originalEnergy = FindDerivedField(originalFields, "edEnergy");
            ItemDerivedField updatedEnergy = FindDerivedField(updatedFields, "edEnergy");
            ItemDerivedField originalEnergyMax = FindDerivedField(originalFields, "edEnergyMax");
            ItemDerivedField updatedEnergyMax = FindDerivedField(updatedFields, "edEnergyMax");
            if (!ValidHullInterceptorCore(originalFlag, originalEnergy, originalEnergyMax) ||
                !ValidHullInterceptorCore(updatedFlag, updatedEnergy, updatedEnergyMax))
                throw new InvalidOperationException("THull: неверный основной блок перехватчиков.");

            byte[] originalBytes;
            byte[] updatedBytes;
            try { originalBytes = EncodeHullInterceptorBlock(originalFields); }
            catch (InvalidOperationException error)
            { throw new InvalidOperationException("THull source interceptor block: " + error.Message, error); }
            try { updatedBytes = EncodeHullInterceptorBlock(updatedFields); }
            catch (InvalidOperationException error)
            { throw new InvalidOperationException("THull " + objectId +
                " updated interceptor block: " + error.Message, error); }
            int originalEnd = originalEnergyMax.End;
            if (originalFlag.IntegerValue != 0)
            {
                ItemDerivedField duration = FindDerivedField(originalFields,
                    "edInterceptorsDuration");
                if (duration == null) throw new InvalidOperationException(
                    "THull: не найдена граница блока перехватчиков.");
                originalEnd = duration.End;
            }
            if (!EqualBytes(originalBytes, updatedBytes))
                patches.Add(new PayloadPatch(MapKnownOffset(originalFlag.Offset,
                    prefixDelta, stars), originalEnd - originalFlag.Offset, updatedBytes));
        }

        private static bool ValidHullInterceptorCore(ItemDerivedField flag,
            ItemDerivedField energy, ItemDerivedField energyMax)
        {
            return flag != null && flag.Kind == ItemDerivedField.Boolean &&
                flag.IntegerValue >= 0 && flag.IntegerValue <= 1 &&
                energy != null && energy.Kind == ItemDerivedField.Int32 &&
                energy.IntegerValue >= int.MinValue && energy.IntegerValue <= int.MaxValue &&
                energyMax != null && energyMax.Kind == ItemDerivedField.Int32 &&
                energyMax.IntegerValue >= int.MinValue && energyMax.IntegerValue <= int.MaxValue;
        }

        private byte[] EncodeHullInterceptorBlock(List<ItemDerivedField> fields)
        {
            ItemDerivedField flag = FindDerivedField(fields, "$HullHasInterceptors");
            ItemDerivedField energy = FindDerivedField(fields, "edEnergy");
            ItemDerivedField energyMax = FindDerivedField(fields, "edEnergyMax");
            if (!ValidHullInterceptorCore(flag, energy, energyMax))
                throw new InvalidOperationException("THull: неверный основной блок перехватчиков.");
            using (MemoryStream encoded = new MemoryStream())
            {
                WriteBoolean(encoded, flag.IntegerValue != 0);
                WriteInt32(encoded, checked((int)energy.IntegerValue));
                WriteInt32(encoded, checked((int)energyMax.IntegerValue));
                ItemDerivedField target = FindDerivedField(fields, "cbInterceptorsNextTarget");
                ItemDerivedField strategy = FindDerivedField(fields, "cbInterceptorsStrategy");
                ItemDerivedField duration = FindDerivedField(fields, "edInterceptorsDuration");
                if (flag.IntegerValue == 0)
                {
                    if (target != null || strategy != null || duration != null)
                        throw new InvalidOperationException(
                            "THull: выключенный блок содержит поля перехватчиков " +
                            "(target=" + (target != null) + ", strategy=" +
                            (strategy != null) + ", duration=" + (duration != null) + ").");
                }
                else
                {
                    if (target == null || target.Kind != ItemDerivedField.UInt32 ||
                        target.IntegerValue < 0 || target.IntegerValue > uint.MaxValue ||
                        target.IntegerValue != 0 && !ContainsShipId((uint)target.IntegerValue) ||
                        strategy == null || strategy.Kind != ItemDerivedField.Byte ||
                        strategy.IntegerValue < 0 || strategy.IntegerValue > byte.MaxValue ||
                        duration == null || duration.Kind != ItemDerivedField.Byte ||
                        duration.IntegerValue < 0 || duration.IntegerValue > byte.MaxValue)
                        throw new InvalidOperationException(
                            "THull: неверная цель, стратегия или длительность перехватчиков.");
                    WriteUInt32(encoded, (uint)target.IntegerValue);
                    encoded.WriteByte((byte)strategy.IntegerValue);
                    encoded.WriteByte((byte)duration.IntegerValue);
                }
                return encoded.ToArray();
            }
        }

        private static List<ItemDerivedField> WithoutHullInterceptorFields(
            List<ItemDerivedField> fields)
        {
            List<ItemDerivedField> result = new List<ItemDerivedField>();
            foreach (ItemDerivedField field in fields)
                if (field.ControlName != "$HullHasInterceptors" &&
                    field.ControlName != "edEnergy" && field.ControlName != "edEnergyMax" &&
                    field.ControlName != "cbInterceptorsNextTarget" &&
                    field.ControlName != "cbInterceptorsStrategy" &&
                    field.ControlName != "edInterceptorsDuration") result.Add(field);
            return result;
        }

        private void AddHolePatches(List<PayloadPatch> patches, HoleRecord original,
            HoleRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.ObjectId != original.ObjectId)
                throw new InvalidOperationException("Изменение ID THole не разрешено.");
            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in stars) starIds.Add(star.ObjectId);
            if (!starIds.Contains(updated.FromStarId) || !starIds.Contains(updated.ToStarId) ||
                !IsSupportedObjectCoordinate(updated.FromX) || !IsSupportedObjectCoordinate(updated.FromY) ||
                !IsSupportedObjectCoordinate(updated.ToX) || !IsSupportedObjectCoordinate(updated.ToY) ||
                updated.TurnCreate < 0 || updated.HoleType < 0 || updated.HoleType > 1024 ||
                !IsSupportedEditableText(updated.GraphName, 128, false) ||
                !IsSupportedEditableText(updated.MapName, 128, true))
                throw new InvalidOperationException("THole: неверная звезда, координаты, тип или имя ресурса.");

            if (original.FromStarId != updated.FromStarId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 4, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.FromStarId)));
            AddFloatPatch(patches, original.Start + 8, original.FromX, updated.FromX, prefixDelta, stars);
            AddFloatPatch(patches, original.Start + 12, original.FromY, updated.FromY, prefixDelta, stars);
            if (original.ToStarId != updated.ToStarId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 16, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.ToStarId)));
            AddFloatPatch(patches, original.Start + 20, original.ToX, updated.ToX, prefixDelta, stars);
            AddFloatPatch(patches, original.Start + 24, original.ToY, updated.ToY, prefixDelta, stars);
            AddInt32Patch(patches, original.Start + 28, original.TurnCreate, updated.TurnCreate, prefixDelta, stars);
            AddInt32Patch(patches, original.Start + 32, original.HoleType, updated.HoleType, prefixDelta, stars);
            if (original.GraphName != updated.GraphName)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 36, prefixDelta, stars),
                    original.GraphNameEnd - (original.Start + 36), EncodeUtf16Z(updated.GraphName)));
            if (original.MapName != updated.MapName)
                patches.Add(new PayloadPatch(MapKnownOffset(original.GraphNameEnd, prefixDelta, stars),
                    original.End - original.GraphNameEnd, EncodeUtf16Z(updated.MapName)));
        }

        private void AddHoleListPatches(List<PayloadPatch> patches, IList<HoleRecord> updated,
            int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || updated.Count > GalaxyHoles.Count || !HasExactStoredItemList ||
                HoleListCountOffset < 0 || HoleListEndOffset != StoredItemCountOffset)
                throw new InvalidOperationException("THole: точная граница списка недоступна.");
            Dictionary<int, HoleRecord> retained = new Dictionary<int, HoleRecord>();
            int sourceIndex = 0;
            foreach (HoleRecord value in updated)
            {
                while (sourceIndex < GalaxyHoles.Count && GalaxyHoles[sourceIndex].Start != value.Start)
                    sourceIndex++;
                if (sourceIndex >= GalaxyHoles.Count)
                    throw new InvalidOperationException("THole: перестановка или добавление записи не разрешены.");
                HoleRecord source = GalaxyHoles[sourceIndex++];
                if (value.End != source.End || value.ObjectId != source.ObjectId)
                    throw new InvalidOperationException("THole: замена записи не разрешена.");
                retained.Add(source.Start, value);
                AddHolePatches(patches, source, value, prefixDelta, stars);
            }
            if (updated.Count != GalaxyHoles.Count)
                AddUInt16Patch(patches, HoleListCountOffset, checked((ushort)GalaxyHoles.Count),
                    checked((ushort)updated.Count), prefixDelta, stars);
            foreach (HoleRecord source in GalaxyHoles)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddAsteroidListPatches(List<PayloadPatch> patches,
            IList<AsteroidRecord> updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || updated.Count > GalaxyAsteroids.Count)
                throw new InvalidOperationException("TAsteroid: добавление записи не разрешено.");
            Dictionary<uint, StarHeaderRecord> starsById = new Dictionary<uint, StarHeaderRecord>();
            foreach (StarHeaderRecord star in GalaxyStars)
                starsById[star.ObjectId] = star;
            Dictionary<uint, List<AsteroidRecord>> movedByTarget =
                new Dictionary<uint, List<AsteroidRecord>>();
            Dictionary<int, AsteroidRecord> retained = new Dictionary<int, AsteroidRecord>();
            int sourceIndex = 0;
            foreach (AsteroidRecord value in updated)
            {
                while (sourceIndex < GalaxyAsteroids.Count && GalaxyAsteroids[sourceIndex].Start != value.Start)
                    sourceIndex++;
                if (sourceIndex >= GalaxyAsteroids.Count)
                    throw new InvalidOperationException("TAsteroid: перестановка или добавление записи не разрешены.");
                AsteroidRecord source = GalaxyAsteroids[sourceIndex++];
                if (value.End != source.End || value.ObjectId != source.ObjectId)
                    throw new InvalidOperationException("TAsteroid: замена записи не разрешена.");
                ValidateAsteroidEditableFields(value);
                StarHeaderRecord targetStar;
                if (!starsById.TryGetValue(value.ParentStarId, out targetStar) ||
                    targetStar.AsteroidCountOffset < 0)
                    throw new InvalidOperationException(
                        "TAsteroid: выбранная родительская звезда не имеет точной границы списка.");
                retained.Add(source.Start, value);
                if (value.ParentStarId == source.ParentStarId)
                    AddAsteroidPatches(patches, source, value, prefixDelta, stars);
                else
                {
                    List<AsteroidRecord> targetValues;
                    if (!movedByTarget.TryGetValue(value.ParentStarId, out targetValues))
                    {
                        targetValues = new List<AsteroidRecord>();
                        movedByTarget.Add(value.ParentStarId, targetValues);
                    }
                    targetValues.Add(value);
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
                }
            }
            foreach (StarHeaderRecord star in GalaxyStars)
            {
                int originalCount = 0, updatedCount = 0;
                foreach (AsteroidRecord value in GalaxyAsteroids)
                    if (value.ParentStarId == star.ObjectId) originalCount++;
                foreach (AsteroidRecord value in updated)
                    if (value.ParentStarId == star.ObjectId) updatedCount++;
                if (updatedCount != originalCount)
                {
                    if (star.AsteroidCountOffset < 0)
                        throw new InvalidOperationException("TAsteroid: count владельца не локализован.");
                    AddUInt16Patch(patches, star.AsteroidCountOffset, checked((ushort)originalCount),
                        checked((ushort)updatedCount), prefixDelta, stars);
                }
            }
            foreach (AsteroidRecord source in GalaxyAsteroids)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));

            foreach (KeyValuePair<uint, List<AsteroidRecord>> pair in movedByTarget)
            {
                StarHeaderRecord targetStar = starsById[pair.Key];
                int insertionOffset = targetStar.AsteroidCountOffset + 2;
                foreach (AsteroidRecord original in GalaxyAsteroids)
                    if (original.ParentStarId == pair.Key)
                        insertionOffset = Math.Max(insertionOffset, original.End);
                using (MemoryStream encoded = new MemoryStream())
                {
                    foreach (AsteroidRecord value in pair.Value)
                        WriteAsteroidRecord(encoded, value);
                    patches.Add(new PayloadPatch(MapKnownOffset(insertionOffset, prefixDelta, stars),
                        0, encoded.ToArray()));
                }
            }
        }

        private void AddMissileListPatches(List<PayloadPatch> patches,
            IList<MissileRecord> updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || updated.Count > GalaxyMissiles.Count)
                throw new InvalidOperationException("TMissile: добавление записи не разрешено.");
            Dictionary<int, MissileRecord> retained = new Dictionary<int, MissileRecord>();
            int sourceIndex = 0;
            foreach (MissileRecord value in updated)
            {
                while (sourceIndex < GalaxyMissiles.Count && GalaxyMissiles[sourceIndex].Start != value.Start)
                    sourceIndex++;
                if (sourceIndex >= GalaxyMissiles.Count)
                    throw new InvalidOperationException("TMissile: перестановка или добавление записи не разрешены.");
                MissileRecord source = GalaxyMissiles[sourceIndex++];
                if (value.End != source.End || value.ObjectId != source.ObjectId ||
                    value.ParentStarId != source.ParentStarId)
                    throw new InvalidOperationException("TMissile: замена записи или владельца не разрешена.");
                retained.Add(source.Start, value);
                AddMissilePatches(patches, source, value, prefixDelta, stars);
            }
            foreach (StarHeaderRecord star in GalaxyStars)
            {
                int originalCount = 0, updatedCount = 0;
                foreach (MissileRecord value in GalaxyMissiles)
                    if (value.ParentStarId == star.ObjectId) originalCount++;
                foreach (MissileRecord value in updated)
                    if (value.ParentStarId == star.ObjectId) updatedCount++;
                if (updatedCount != originalCount)
                {
                    if (star.MissileCountOffset < 0)
                        throw new InvalidOperationException("TMissile: count владельца не локализован.");
                    AddUInt16Patch(patches, star.MissileCountOffset, checked((ushort)originalCount),
                        checked((ushort)updatedCount), prefixDelta, stars);
                }
            }
            foreach (MissileRecord source in GalaxyMissiles)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void ValidateRemovedTargetReferences(IList<StarHeaderRecord> stars,
            IList<ShipHeaderRecord> ships, IList<ItemHeaderRecord> items,
            IList<AsteroidRecord> asteroids, IList<MissileRecord> missiles,
            GalaxySummaryData summary)
        {
            HashSet<int> retainedSpaceItemStarts = new HashSet<int>();
            foreach (StarHeaderRecord star in stars)
                if (star != null && star.SpaceItems != null)
                    foreach (ShipItemListEntry entry in star.SpaceItems)
                        if (entry != null) retainedSpaceItemStarts.Add(entry.ItemStart);
            HashSet<int> removedItemStarts = new HashSet<int>();
            HashSet<uint> removedItems = new HashSet<uint>();
            Dictionary<int, uint> originalItemsByStart = new Dictionary<int, uint>();
            foreach (ItemHeaderRecord item in GalaxyItems)
                if (item != null) originalItemsByStart[item.Start] = item.ObjectId;
            foreach (StarHeaderRecord star in GalaxyStars)
                if (star.SpaceItems != null)
                    foreach (ShipItemListEntry entry in star.SpaceItems)
                        if (entry != null && !retainedSpaceItemStarts.Contains(entry.ItemStart))
                        {
                            uint objectId;
                            if (!originalItemsByStart.TryGetValue(entry.ItemStart, out objectId))
                                throw new InvalidOperationException(
                                    "Удаляемый TStar.ItemsInSpace не разрешается в таблице TItem.");
                            removedItemStarts.Add(entry.ItemStart);
                            removedItems.Add(objectId);
                        }

            HashSet<uint> asteroidIds = new HashSet<uint>();
            HashSet<uint> missileIds = new HashSet<uint>();
            HashSet<uint> shipIds = new HashSet<uint>();
            foreach (ShipHeaderRecord value in ships) shipIds.Add(value.ObjectId);
            foreach (AsteroidRecord value in asteroids) asteroidIds.Add(value.ObjectId);
            foreach (MissileRecord value in missiles) missileIds.Add(value.ObjectId);
            HashSet<uint> removedAsteroids = new HashSet<uint>();
            HashSet<uint> removedMissiles = new HashSet<uint>();
            HashSet<uint> removedShips = new HashSet<uint>();
            foreach (ShipHeaderRecord value in GalaxyShips)
                if (!shipIds.Contains(value.ObjectId)) removedShips.Add(value.ObjectId);
            foreach (AsteroidRecord value in GalaxyAsteroids)
                if (!asteroidIds.Contains(value.ObjectId)) removedAsteroids.Add(value.ObjectId);
            foreach (MissileRecord value in GalaxyMissiles)
                if (!missileIds.Contains(value.ObjectId)) removedMissiles.Add(value.ObjectId);
            if (removedItems.Count == 0 && removedShips.Count == 0 && removedAsteroids.Count == 0 &&
                removedMissiles.Count == 0) return;

            foreach (MissileRecord value in missiles)
                if (removedShips.Contains(value.ShipId) ||
                    value.TargetType == 1 && removedShips.Contains(value.TargetId) ||
                    value.TargetLostType == 1 && removedShips.Contains(value.TargetLostId) ||
                    value.TargetType == 2 && removedItems.Contains(value.TargetId) ||
                    value.TargetLostType == 2 && removedItems.Contains(value.TargetLostId) ||
                    value.TargetType == 3 && removedAsteroids.Contains(value.TargetId) ||
                    value.TargetLostType == 3 && removedAsteroids.Contains(value.TargetLostId) ||
                    value.TargetType == 4 && removedMissiles.Contains(value.TargetId) ||
                    value.TargetLostType == 4 && removedMissiles.Contains(value.TargetLostId))
                    throw new InvalidOperationException(
                        "Удаляемый TItem/TAsteroid/TMissile ещё используется другой ракетой.");
            foreach (ShipHeaderRecord ship in ships)
                ValidateRemovedItemReferencesInShip(ship, removedItems, removedShips,
                    removedAsteroids, removedMissiles);
            foreach (StarHeaderRecord star in stars)
                if (star.DropItems != null)
                    foreach (StarDropItemRecord drop in star.DropItems)
                        if (drop != null && removedShips.Contains(drop.ShipObjectId))
                            throw new InvalidOperationException(
                                "Удаляемый TShip ещё используется TStar.DropItems.");
            ValidateRemovedShipSummaryReferences(summary, removedShips);
            foreach (ItemHeaderRecord item in items)
            {
                byte targetType = 0; uint targetId = 0;
                if (item.DerivedFields == null || removedItemStarts.Contains(item.Start)) continue;
                foreach (ItemDerivedField field in item.DerivedFields)
                {
                    if (field.ControlName == "edWeaponTargetType") targetType = checked((byte)field.IntegerValue);
                    else if (field.ControlName == "cbWeaponTarget") targetId = checked((uint)field.IntegerValue);
                }
                if (targetType == 1 && removedShips.Contains(targetId) ||
                    targetType == 2 && removedItems.Contains(targetId) ||
                    targetType == 3 && removedAsteroids.Contains(targetId) ||
                    targetType == 4 && removedMissiles.Contains(targetId))
                    throw new InvalidOperationException(
                        "Удаляемый TItem/TAsteroid/TMissile ещё используется оружием.");
            }
        }

        private static void ValidateRemovedShipSummaryReferences(GalaxySummaryData summary,
            HashSet<uint> removedShips)
        {
            if (summary == null || removedShips.Count == 0) return;
            if (removedShips.Contains(summary.PlayerObjectId) ||
                removedShips.Contains(summary.BlazerObjectId) ||
                removedShips.Contains(summary.KellerObjectId) ||
                removedShips.Contains(summary.TerronObjectId) ||
                removedShips.Contains(summary.AutoBattleShipObjectId))
                throw new InvalidOperationException(
                    "Удаляемый TShip ещё используется основной ссылкой TGalaxy.");
            if (summary.EminentRangerObjectIds != null)
                foreach (uint value in summary.EminentRangerObjectIds)
                    if (removedShips.Contains(value))
                        throw new InvalidOperationException(
                            "Удаляемый TShip ещё используется списком знаменитых рейнджеров.");
            if (summary.HangarShipObjectIds != null)
                foreach (uint value in summary.HangarShipObjectIds)
                    if (removedShips.Contains(value))
                        throw new InvalidOperationException(
                            "Удаляемый TShip ещё используется ангаром.");
            if (summary.RangerObjectIds != null)
                foreach (uint value in summary.RangerObjectIds)
                    if (removedShips.Contains(value))
                        throw new InvalidOperationException(
                            "Удаляемый TRanger ещё используется индексным списком TGalaxy.Rangers.");
            if (summary.ActiveScripts != null)
                foreach (ScriptRecord script in summary.ActiveScripts)
                    if (script != null && script.ShipBindings != null)
                        foreach (ScriptShipRecord binding in script.ShipBindings)
                            if (binding != null && removedShips.Contains(binding.ShipObjectId))
                                throw new InvalidOperationException(
                                    "Удаляемый TShip ещё используется TScriptShip.");
            if (summary.WarOperations != null)
                foreach (WarOperationRecord operation in summary.WarOperations)
                    if (operation != null && operation.ShipObjectIds != null)
                        foreach (uint value in operation.ShipObjectIds)
                            if (removedShips.Contains(value))
                                throw new InvalidOperationException(
                                    "Удаляемый TShip ещё используется TWarOperation.");
        }

        private static void ValidateRemovedItemReferencesInShip(ShipHeaderRecord ship,
            HashSet<uint> removedItems, HashSet<uint> removedShips, HashSet<uint> removedAsteroids,
            HashSet<uint> removedMissiles)
        {
            if (ship == null) return;
            if (removedShips.Contains(ship.CurrentShipId) ||
                removedShips.Contains(ship.PlayerBridgeCurrentShipId) ||
                removedItems.Contains(ship.OrderObjectId) || removedShips.Contains(ship.OrderObjectId) ||
                (ship.OrderType == 2 && (ship.OrderObjectId & 0x80000000U) != 0 &&
                 removedShips.Contains(ship.OrderObjectId & 0x7FFFFFFFU)) ||
                removedAsteroids.Contains(ship.OrderObjectId) ||
                removedMissiles.Contains(ship.OrderObjectId))
                throw new InvalidOperationException(
                    "Удаляемый TItem/TAsteroid/TMissile ещё используется приказом корабля.");
            if (ship.TakeItemReferenceIds != null)
                foreach (uint value in ship.TakeItemReferenceIds)
                    if (removedItems.Contains(value))
                        throw new InvalidOperationException(
                            "Удаляемый TItem ещё используется списком TShip.TakeItems.");
            if (ship.RecentlyDroppedItemIds != null)
                foreach (uint value in ship.RecentlyDroppedItemIds)
                    if (removedItems.Contains(value))
                        throw new InvalidOperationException(
                            "Удаляемый TItem ещё используется списком TShip.RecentlyDroppedItems.");
            ValidateRemovedItemSetReferences(ship.PlayerEquipmentSetItems, removedItems);
            ValidateRemovedItemSetReferences(ship.PlayerArtefactSetItems, removedItems);
            if (ship.PlayerBridgeRuins != null)
                ValidateRemovedItemReferencesInShip(ship.PlayerBridgeRuins, removedItems, removedShips,
                    removedAsteroids, removedMissiles);
        }

        private static void ValidateRemovedItemSetReferences(uint[,] values,
            HashSet<uint> removedItems)
        {
            if (values == null) return;
            for (int row = 0; row < values.GetLength(0); row++)
                for (int column = 0; column < values.GetLength(1); column++)
                    if (removedItems.Contains(values[row, column]))
                        throw new InvalidOperationException(
                            "Удаляемый TItem ещё используется комплектом оборудования TPlayer.");
        }

        private void AddAsteroidPatches(List<PayloadPatch> patches, AsteroidRecord original,
            AsteroidRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.ObjectId != original.ObjectId || updated.ParentStarId != original.ParentStarId)
                throw new InvalidOperationException("Изменение ID или родительской звезды TAsteroid не разрешено.");
            ValidateAsteroidEditableFields(updated);

            if (original.GraphName != updated.GraphName)
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 4, prefixDelta, stars),
                    original.GraphNameEnd - (original.Start + 4), EncodeUtf16Z(updated.GraphName)));
            AddFloatPatch(patches, original.GraphNameEnd, original.PositionX, updated.PositionX, prefixDelta, stars);
            AddFloatPatch(patches, original.GraphNameEnd + 4, original.PositionY, updated.PositionY, prefixDelta, stars);
            AddFloatPatch(patches, original.GraphNameEnd + 8, original.SpeedX, updated.SpeedX, prefixDelta, stars);
            AddFloatPatch(patches, original.GraphNameEnd + 12, original.SpeedY, updated.SpeedY, prefixDelta, stars);
            AddFloatPatch(patches, original.GraphNameEnd + 16, original.Mass, updated.Mass, prefixDelta, stars);
            AddInt32Patch(patches, original.GraphNameEnd + 20, original.Minerals, updated.Minerals, prefixDelta, stars);
        }

        private static void ValidateAsteroidEditableFields(AsteroidRecord value)
        {
            if (value == null || !IsSupportedEditableText(value.GraphName, 128, false) ||
                !IsSupportedAsteroidScalar(value.PositionX) || !IsSupportedAsteroidScalar(value.PositionY) ||
                !IsSupportedAsteroidScalar(value.SpeedX) || !IsSupportedAsteroidScalar(value.SpeedY) ||
                !IsSupportedAsteroidScalar(value.Mass))
                throw new InvalidOperationException("TAsteroid: неверное имя ресурса или числовое поле.");
        }

        private static void WriteAsteroidRecord(Stream stream, AsteroidRecord value)
        {
            ValidateAsteroidEditableFields(value);
            WriteUInt32(stream, value.ObjectId);
            WriteUtf16Z(stream, value.GraphName);
            WriteSingle(stream, value.PositionX); WriteSingle(stream, value.PositionY);
            WriteSingle(stream, value.SpeedX); WriteSingle(stream, value.SpeedY);
            WriteSingle(stream, value.Mass); WriteInt32(stream, value.Minerals);
        }

        private void AddMissilePatches(List<PayloadPatch> patches, MissileRecord original,
            MissileRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.ParentStarId != original.ParentStarId || updated.IsCustom != original.IsCustom ||
                updated.ObjectId != original.ObjectId ||
                updated.WeaponId != original.WeaponId || updated.WeaponType != original.WeaponType)
                throw new InvalidOperationException("Изменение ID, типа, оружия или секции TMissile не разрешено.");
            if (updated.CustomWeaponName != original.CustomWeaponName)
            {
                if (!original.IsCustom || original.BaseStart <= original.Start + 1 ||
                    !IsSupportedEditableText(updated.CustomWeaponName, 512, false))
                    throw new InvalidOperationException(
                        "TCustomMissile: неверное новое системное имя или граница wrapper.");
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start + 1, prefixDelta, stars),
                    original.BaseStart - (original.Start + 1),
                    EncodeUtf16Z(updated.CustomWeaponName)));
            }
            if (updated.Bonus < 0 || updated.Bonus > 4096 || updated.Special < 0 || updated.Special > 4096 ||
                (updated.Bonus == 0) != (updated.BonusReferenceId == 0) ||
                (updated.Special == 0) != (updated.SpecialReferenceId == 0) ||
                !IsSupportedMissileScalar(updated.PositionX) || !IsSupportedMissileScalar(updated.PositionY) ||
                !IsSupportedMissileScalar(updated.Angle) || !IsSupportedMissileScalar(updated.FromAngle) ||
                !IsSupportedMissileScalar(updated.FromAngleOld) || !IsSupportedMissileScalar(updated.Speed) ||
                !IsSupportedMissileScalar(updated.BaseSpeed) ||
                !IsSupportedMissileScalar(updated.LastPositionX) ||
                !IsSupportedMissileScalar(updated.LastPositionY) ||
                !IsSupportedMissileScalar(updated.LastDistanceMin))
                throw new InvalidOperationException("TMissile: неверное числовое поле, бонус или спецэффект.");

            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in stars) starIds.Add(star.ObjectId);
            // Some real and modded saves contain stale TMissile references left by the game.
            // Preserve those bytes when the user edits an unrelated field, but never allow a
            // newly entered reference to point outside the parsed object directory.
            if ((updated.StarId != original.StarId && !starIds.Contains(updated.StarId)) ||
                (updated.ShipId != original.ShipId && updated.ShipId != 0 &&
                    !ContainsShipId(updated.ShipId)) ||
                ((updated.TargetType != original.TargetType || updated.TargetId != original.TargetId) &&
                    !IsValidMissileReference(updated.TargetType, updated.TargetId)) ||
                ((updated.TargetLostType != original.TargetLostType ||
                    updated.TargetLostId != original.TargetLostId) &&
                    !IsValidMissileReference(updated.TargetLostType, updated.TargetLostId)))
                throw new InvalidOperationException("TMissile: неверная ссылка на звезду, корабль или цель.");

            if (original.TechLevel != updated.TechLevel)
                patches.Add(new PayloadPatch(MapKnownOffset(original.BaseStart + 9, prefixDelta, stars), 1,
                    new byte[] { updated.TechLevel }));
            AddInt32Patch(patches, original.BaseStart + 10, original.DamageMin, updated.DamageMin, prefixDelta, stars);
            AddInt32Patch(patches, original.BaseStart + 14, original.DamageMax, updated.DamageMax, prefixDelta, stars);
            if (original.Bonus != updated.Bonus || original.BonusReferenceId != updated.BonusReferenceId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.BonusOffset, prefixDelta, stars),
                    original.BonusEnd - original.BonusOffset,
                    EncodeIndexedReference(updated.Bonus, updated.BonusReferenceId)));
            if (original.Special != updated.Special || original.SpecialReferenceId != updated.SpecialReferenceId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.SpecialOffset, prefixDelta, stars),
                    original.SpecialEnd - original.SpecialOffset,
                    EncodeIndexedReference(updated.Special, updated.SpecialReferenceId)));
            AddFloatPatch(patches, original.PositionOffset, original.PositionX, updated.PositionX, prefixDelta, stars);
            AddFloatPatch(patches, original.PositionOffset + 4, original.PositionY, updated.PositionY, prefixDelta, stars);
            AddFloatPatch(patches, original.PositionOffset + 8, original.Angle, updated.Angle, prefixDelta, stars);
            AddFloatPatch(patches, original.PositionOffset + 12, original.FromAngle, updated.FromAngle, prefixDelta, stars);
            if (original.StarId != updated.StarId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.StarOffset, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.StarId)));
            if (original.ShipId != updated.ShipId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.StarOffset + 4, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated.ShipId)));
            if (original.TargetType != updated.TargetType || original.TargetId != updated.TargetId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.TargetOffset, prefixDelta, stars),
                    original.TargetEnd - original.TargetOffset,
                    EncodeMissileReference(updated.TargetType, updated.TargetId)));
            if (original.MissileNo != updated.MissileNo)
                patches.Add(new PayloadPatch(MapKnownOffset(original.MissileNoOffset, prefixDelta, stars), 1,
                    new byte[] { updated.MissileNo }));
            AddInt32Patch(patches, original.LiveOffset, original.Live, updated.Live, prefixDelta, stars);
            AddFloatPatch(patches, original.MotionOffset, original.FromAngleOld, updated.FromAngleOld, prefixDelta, stars);
            AddFloatPatch(patches, original.MotionOffset + 4, original.Speed, updated.Speed, prefixDelta, stars);
            AddFloatPatch(patches, original.MotionOffset + 8, original.BaseSpeed, updated.BaseSpeed, prefixDelta, stars);
            if (original.TargetLostType != updated.TargetLostType || original.TargetLostId != updated.TargetLostId)
                patches.Add(new PayloadPatch(MapKnownOffset(original.TargetLostOffset, prefixDelta, stars),
                    original.TargetLostEnd - original.TargetLostOffset,
                    EncodeMissileReference(updated.TargetLostType, updated.TargetLostId)));
            AddFloatPatch(patches, original.LastMotionOffset, original.LastPositionX, updated.LastPositionX, prefixDelta, stars);
            AddFloatPatch(patches, original.LastMotionOffset + 4, original.LastPositionY, updated.LastPositionY, prefixDelta, stars);
            AddFloatPatch(patches, original.LastMotionOffset + 8, original.LastDistanceMin, updated.LastDistanceMin, prefixDelta, stars);
        }

        private bool ContainsShipId(uint objectId)
        {
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.ObjectId == objectId) return true;
            return false;
        }

        private bool IsValidMissileReference(byte type, uint objectId)
        {
            if (type == 0) return objectId == 0;
            if (objectId == 0 || type > 4) return false;
            if (type == 1) return ContainsShipId(objectId);
            if (type == 2)
            {
                foreach (ItemHeaderRecord item in GalaxyItems)
                    if (item.ObjectId == objectId) return true;
                return false;
            }
            if (type == 3)
            {
                foreach (AsteroidRecord asteroid in GalaxyAsteroids)
                    if (asteroid.ObjectId == objectId) return true;
                return false;
            }
            foreach (MissileRecord missile in GalaxyMissiles)
                if (missile.ObjectId == objectId) return true;
            return false;
        }

        private static byte[] EncodeIndexedReference(int index, uint objectId)
        {
            return index == 0 ? BitConverter.GetBytes(0) :
                JoinBytes(BitConverter.GetBytes(index), BitConverter.GetBytes(objectId));
        }

        private static byte[] EncodeMissileReference(byte type, uint objectId)
        {
            return type == 0 ? new byte[] { 0 } :
                JoinBytes(new byte[] { type }, BitConverter.GetBytes(objectId));
        }

        private void AddAchievementStatsPatches(List<PayloadPatch> patches, AchievementStatsRecord original,
            AchievementStatsRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.Received == null || updated.Received.Count != original.Received.Count)
                throw new InvalidOperationException("Список уже полученных достижений доступен только для чтения.");
            for (int index = 0; index < original.Received.Count; index++)
                if (updated.Received[index] != original.Received[index])
                    throw new InvalidOperationException("Список уже полученных достижений доступен только для чтения.");

            int start = original.Start;
            AddInt32Patch(patches, start, original.AsteroidsDestroyed, updated.AsteroidsDestroyed, prefixDelta, stars);
            AddInt32Patch(patches, start + 4, original.FriedShips, updated.FriedShips, prefixDelta, stars);
            AddInt32Patch(patches, start + 8, original.DefendedSystem, updated.DefendedSystem, prefixDelta, stars);
            AddInt32Patch(patches, start + 12, original.PirateSystems, updated.PirateSystems, prefixDelta, stars);
            if (original.ScienceProgress != updated.ScienceProgress)
                patches.Add(new PayloadPatch(MapKnownOffset(start + 16, prefixDelta, stars), 1,
                    new byte[] { updated.ScienceProgress }));
            AddInt32Patch(patches, start + 17, original.ProgramsUsed, updated.ProgramsUsed, prefixDelta, stars);
            AddInt32Patch(patches, start + 21, original.PiratesFreed, updated.PiratesFreed, prefixDelta, stars);
            AddInt32Patch(patches, start + 25, original.HealthDrained, updated.HealthDrained, prefixDelta, stars);
            AddInt32Patch(patches, start + 29, original.FuelGottenFromSun, updated.FuelGottenFromSun, prefixDelta, stars);
            AddInt32Patch(patches, start + 33, original.FuelTankLastId, updated.FuelTankLastId, prefixDelta, stars);
            AddInt32Patch(patches, start + 37, original.PlanetsVisited, updated.PlanetsVisited, prefixDelta, stars);
        }

        private void AddInterfaceOverridePatch(List<PayloadPatch> patches, InterfaceOverrideRecord original,
            InterfaceOverrideRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.ContentEquals(updated)) return;
            if (updated.Kind != original.Kind ||
                !IsSupportedItemText(updated.ModuleName, 4096) ||
                !IsSupportedItemText(updated.GuiName, 4096) ||
                !IsSupportedItemText(updated.NewValue, 4096) ||
                !IsSupportedItemText(updated.OldValue, 4096) ||
                updated.NewState > 3 || updated.OldState > 3 ||
                double.IsNaN(updated.NewZ) || double.IsInfinity(updated.NewZ) ||
                double.IsNaN(updated.OldZ) || double.IsInfinity(updated.OldZ))
                throw new InvalidOperationException("Переопределение интерфейса содержит неверный тип, текст или число.");

            using (MemoryStream encoded = new MemoryStream(original.End - original.Start + 128))
            {
                WriteUtf16Z(encoded, updated.ModuleName ?? string.Empty);
                WriteUtf16Z(encoded, updated.GuiName ?? string.Empty);
                if (updated.Kind == InterfaceOverrideKind.State)
                {
                    encoded.WriteByte(updated.NewState);
                    encoded.WriteByte(updated.OldState);
                }
                else if (updated.Kind == InterfaceOverrideKind.Text || updated.Kind == InterfaceOverrideKind.Image)
                {
                    WriteUtf16Z(encoded, updated.NewValue ?? string.Empty);
                    WriteUtf16Z(encoded, updated.OldValue ?? string.Empty);
                }
                else if (updated.Kind == InterfaceOverrideKind.Position)
                {
                    WriteInt32(encoded, updated.NewX); WriteInt32(encoded, updated.NewY);
                    WriteDouble(encoded, updated.NewZ);
                    WriteInt32(encoded, updated.OldX); WriteInt32(encoded, updated.OldY);
                    WriteDouble(encoded, updated.OldZ);
                }
                else if (updated.Kind == InterfaceOverrideKind.Size)
                {
                    WriteInt32(encoded, updated.NewX); WriteInt32(encoded, updated.NewY);
                    WriteInt32(encoded, updated.OldX); WriteInt32(encoded, updated.OldY);
                }
                patches.Add(new PayloadPatch(MapKnownOffset(original.Start, prefixDelta, stars),
                    original.End - original.Start, encoded.ToArray()));
            }
        }

        private void AddInterfaceOverrideListPatches(List<PayloadPatch> patches,
            IList<InterfaceOverrideRecord> updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || GalaxySummary.InterfaceOverrideListOffsets == null ||
                GalaxySummary.InterfaceOverrideListEndOffsets == null ||
                GalaxySummary.InterfaceOverrideListOffsets.Length != 5 ||
                GalaxySummary.InterfaceOverrideListEndOffsets.Length != 5)
                throw new InvalidOperationException("Переопределения интерфейса имеют неверную структуру списков.");
            for (int kindIndex = 0; kindIndex < 5; kindIndex++)
            {
                InterfaceOverrideKind kind = (InterfaceOverrideKind)kindIndex;
                List<InterfaceOverrideRecord> originalKind = new List<InterfaceOverrideRecord>();
                List<InterfaceOverrideRecord> updatedKind = new List<InterfaceOverrideRecord>();
                foreach (InterfaceOverrideRecord record in GalaxySummary.InterfaceOverrides)
                    if (record.Kind == kind) originalKind.Add(record);
                foreach (InterfaceOverrideRecord record in updated)
                {
                    if ((int)record.Kind < 0 || (int)record.Kind > 4)
                        throw new InvalidOperationException("Переопределение интерфейса содержит неизвестный тип.");
                    if (record.Kind == kind) updatedKind.Add(record);
                }
                bool changed = originalKind.Count != updatedKind.Count;
                for (int index = 0; !changed && index < originalKind.Count; index++)
                    changed = !originalKind[index].ContentEquals(updatedKind[index]);
                if (!changed) continue;
                if (updatedKind.Count > ushort.MaxValue)
                    throw new InvalidOperationException("Слишком много переопределений интерфейса одного типа.");
                int start = GalaxySummary.InterfaceOverrideListOffsets[kindIndex];
                int end = GalaxySummary.InterfaceOverrideListEndOffsets[kindIndex];
                if (start < GalaxySummary.GalaxyEventListEndOffset || end < start || end > MainPayload.Length)
                    throw new InvalidOperationException("Переопределения интерфейса имеют неверную границу списка.");
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updatedKind.Count));
                    foreach (InterfaceOverrideRecord record in updatedKind)
                        WriteInterfaceOverride(encoded, record, kind);
                    patches.Add(new PayloadPatch(MapKnownOffset(start, prefixDelta, stars),
                        end - start, encoded.ToArray()));
                }
            }
        }

        private void WriteInterfaceOverride(Stream encoded, InterfaceOverrideRecord record,
            InterfaceOverrideKind expectedKind)
        {
            if (record == null || record.Kind != expectedKind ||
                !IsSupportedItemText(record.ModuleName, 4096) ||
                !IsSupportedItemText(record.GuiName, 4096) ||
                !IsSupportedItemText(record.NewValue, 4096) ||
                !IsSupportedItemText(record.OldValue, 4096) ||
                record.NewState > 3 || record.OldState > 3 ||
                double.IsNaN(record.NewZ) || double.IsInfinity(record.NewZ) ||
                double.IsNaN(record.OldZ) || double.IsInfinity(record.OldZ))
                throw new InvalidOperationException("Переопределение интерфейса содержит неверный тип, текст или число.");
            WriteUtf16Z(encoded, record.ModuleName ?? string.Empty);
            WriteUtf16Z(encoded, record.GuiName ?? string.Empty);
            if (expectedKind == InterfaceOverrideKind.State)
            {
                encoded.WriteByte(record.NewState); encoded.WriteByte(record.OldState);
            }
            else if (expectedKind == InterfaceOverrideKind.Text || expectedKind == InterfaceOverrideKind.Image)
            {
                WriteUtf16Z(encoded, record.NewValue ?? string.Empty);
                WriteUtf16Z(encoded, record.OldValue ?? string.Empty);
            }
            else if (expectedKind == InterfaceOverrideKind.Position)
            {
                WriteInt32(encoded, record.NewX); WriteInt32(encoded, record.NewY);
                WriteDouble(encoded, record.NewZ);
                WriteInt32(encoded, record.OldX); WriteInt32(encoded, record.OldY);
                WriteDouble(encoded, record.OldZ);
            }
            else
            {
                WriteInt32(encoded, record.NewX); WriteInt32(encoded, record.NewY);
                WriteInt32(encoded, record.OldX); WriteInt32(encoded, record.OldY);
            }
        }

        private void AddStoredItemPatch(List<PayloadPatch> patches, StoredItemRecord original,
            StoredItemRecord updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (original.ContentEquals(updated)) return;
            if (updated.ItemType != original.ItemType || updated.ItemTypeOffset != original.ItemTypeOffset ||
                updated.ItemStart != original.ItemStart ||
                updated.ItemObjectId != original.ItemObjectId)
                throw new InvalidOperationException("TStoredItem: замена вложенного предмета требует обновления объектных ссылок.");
            if (!IsSupportedItemText(updated.ScriptTag, 256) || string.IsNullOrEmpty(updated.ScriptTag))
                throw new InvalidOperationException("TStoredItem: неверный script tag.");
            int originalTagLength = original.ItemTypeOffset - original.Start;
            patches.Add(new PayloadPatch(MapKnownOffset(original.Start, prefixDelta, stars),
                originalTagLength, EncodeUtf16Z(updated.ScriptTag)));
        }

        private void AddStoredItemListPatches(List<PayloadPatch> patches,
            IList<StoredItemRecord> updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || updated.Count > StoredItems.Count)
                throw new InvalidOperationException("TStoredItem: добавление нового вложенного TItem не разрешено.");
            if (!HasExactStoredItemList || StoredItemCountOffset < 0)
            {
                if (updated.Count != StoredItems.Count)
                    throw new InvalidOperationException("TStoredItem: граница count не локализована для удаления.");
                for (int index = 0; index < updated.Count; index++)
                    AddStoredItemPatch(patches, StoredItems[index], updated[index], prefixDelta, stars);
                return;
            }

            Dictionary<int, StoredItemRecord> retained = new Dictionary<int, StoredItemRecord>();
            int sourceIndex = 0;
            foreach (StoredItemRecord value in updated)
            {
                while (sourceIndex < StoredItems.Count && StoredItems[sourceIndex].Start != value.Start)
                    sourceIndex++;
                if (sourceIndex >= StoredItems.Count)
                    throw new InvalidOperationException("TStoredItem: перестановка или добавление записи не разрешены.");
                StoredItemRecord source = StoredItems[sourceIndex++];
                if (value.ItemType != source.ItemType || value.ItemTypeOffset != source.ItemTypeOffset ||
                    value.ItemStart != source.ItemStart || value.ItemObjectId != source.ItemObjectId ||
                    value.End != source.End)
                    throw new InvalidOperationException("TStoredItem: замена вложенного TItem не разрешена.");
                retained.Add(source.Start, value);
                AddStoredItemPatch(patches, source, value, prefixDelta, stars);
            }
            if (updated.Count != StoredItems.Count)
                AddUInt16Patch(patches, StoredItemCountOffset, checked((ushort)StoredItems.Count),
                    checked((ushort)updated.Count), prefixDelta, stars);
            foreach (StoredItemRecord source in StoredItems)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void AddScriptShopSlotPatches(List<PayloadPatch> patches,
            GalaxySummaryData updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated == null || updated.ScriptShopSlots == null ||
                updated.ScriptShopSlots.Count > GalaxySummary.ScriptShopSlots.Count ||
                updated.ScriptShopSlotCountOffset != GalaxySummary.ScriptShopSlotCountOffset ||
                updated.ScriptShopSlotListEndOffset != GalaxySummary.ScriptShopSlotListEndOffset)
                throw new InvalidOperationException(
                    "TScript shop slots: неверные границы или добавление новой записи.");

            Dictionary<int, ScriptShopSlotRecord> retained =
                new Dictionary<int, ScriptShopSlotRecord>();
            int sourceIndex = 0;
            foreach (ScriptShopSlotRecord value in updated.ScriptShopSlots)
            {
                if (value == null)
                    throw new InvalidOperationException("TScript shop slots: пустая запись.");
                while (sourceIndex < GalaxySummary.ScriptShopSlots.Count &&
                    GalaxySummary.ScriptShopSlots[sourceIndex].Start != value.Start) sourceIndex++;
                if (sourceIndex >= GalaxySummary.ScriptShopSlots.Count)
                    throw new InvalidOperationException(
                        "TScript shop slots: добавление и перестановка записей не разрешены.");
                ScriptShopSlotRecord source = GalaxySummary.ScriptShopSlots[sourceIndex++];
                if (value.End != source.End ||
                    value.FactoryDiscriminatorOffset != source.FactoryDiscriminatorOffset ||
                    !source.ContentEquals(value))
                    throw new InvalidOperationException(
                        "TScript shop slots: координаты и вложенный TItem доступны только для чтения.");
                retained.Add(source.Start, value);
            }

            if (updated.ScriptShopSlots.Count != GalaxySummary.ScriptShopSlots.Count)
                AddUInt16Patch(patches, GalaxySummary.ScriptShopSlotCountOffset,
                    checked((ushort)GalaxySummary.ScriptShopSlots.Count),
                    checked((ushort)updated.ScriptShopSlots.Count), prefixDelta, stars);
            foreach (ScriptShopSlotRecord source in GalaxySummary.ScriptShopSlots)
                if (!retained.ContainsKey(source.Start))
                    patches.Add(new PayloadPatch(MapKnownOffset(source.Start, prefixDelta, stars),
                        source.End - source.Start, new byte[0]));
        }

        private void WriteScriptVariableArray(Stream stream, IList<ScriptVariableRecord> values,
            int depth, bool wideCount)
        {
            if (depth > 16 || values == null || values.Count > 10000 ||
                (!wideCount && values.Count > ushort.MaxValue))
                throw new InvalidOperationException("TScript: неверная глубина или длина массива переменных.");
            if (wideCount) WriteInt32(stream, values.Count);
            else WriteUInt16(stream, checked((ushort)values.Count));
            foreach (ScriptVariableRecord value in values)
            {
                if (value == null || !IsSupportedItemText(value.Name, 4096))
                    throw new InvalidOperationException("TScript: переменная содержит неверное имя.");
                WriteUtf16Z(stream, value.Name ?? string.Empty);
                stream.WriteByte(value.Type);
                switch (value.Type)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                        WriteInt32(stream, value.IntegerValue);
                        break;
                    case 3:
                        if (double.IsNaN(value.DoubleValue) || double.IsInfinity(value.DoubleValue))
                            throw new InvalidOperationException("TScript: переменная Float должна быть конечным числом.");
                        WriteDouble(stream, value.DoubleValue);
                        break;
                    case 4:
                        if (!IsSupportedItemText(value.StringValue, 4096))
                            throw new InvalidOperationException("TScript: строковая переменная содержит неверный текст.");
                        WriteUtf16Z(stream, value.StringValue ?? string.Empty);
                        break;
                    case 6:
                        if (!wideCount)
                        {
                            if (!IsSupportedItemText(value.StringValue, 4096))
                                throw new InvalidOperationException("TScript: dllLibrary содержит неверное имя.");
                            WriteUtf16Z(stream, value.StringValue ?? string.Empty);
                        }
                        break;
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                        // The original TVarEC writer emits only the name and type for
                        // these runtime/object/function/reference markers.
                        break;
                    case 9:
                        WriteScriptVariableArray(stream, value.ArrayValue, depth + 1, true);
                        break;
                    default:
                        throw new InvalidOperationException("TScript: неизвестный тип переменной " + value.Type + ".");
                }
            }
        }

        private void WriteScript(Stream stream, ScriptRecord script)
        {
            if (script == null || string.IsNullOrEmpty(script.Name) ||
                !IsSupportedItemText(script.Name, 512) || script.OldEthers == null ||
                script.OldEthers.Count > 10000 ||
                script.StarBindings == null || script.StarBindings.Count > 10000 ||
                script.ItemBindings == null || script.ItemBindings.Count > 10000 ||
                script.ShipBindings == null || script.ShipBindings.Count > ushort.MaxValue ||
                script.EtherStrings == null || script.EtherStrings.Count > ushort.MaxValue)
                throw new InvalidOperationException("TScript: неверное имя, список или служебное поле.");

            WriteUtf16Z(stream, script.Name);
            WriteInt32(stream, script.OldEthers.Count);
            foreach (ScriptOldEtherRecord oldEther in script.OldEthers)
            {
                if (oldEther == null || !IsSupportedItemText(oldEther.Name, 4096))
                    throw new InvalidOperationException("TScript: неверная запись old ether.");
                WriteUtf16Z(stream, oldEther.Name ?? string.Empty);
                WriteInt32(stream, oldEther.Value);
            }
            WriteScriptVariableArray(stream, script.InitVariables, 0, false);
            WriteScriptVariableArray(stream, script.TurnVariables, 0, false);

            WriteInt32(stream, script.StarBindings.Count);
            foreach (ScriptStarBindingRecord star in script.StarBindings)
            {
                if (star == null || !IsSupportedItemText(star.Name, 512) ||
                    !IsKnownStarObjectId(star.StarObjectId) ||
                    star.Planets == null || star.Planets.Count > 10000)
                    throw new InvalidOperationException("TScriptStar: неверная система, строка или список планет.");
                WriteUtf16Z(stream, star.Name ?? string.Empty);
                WriteUInt32(stream, star.StarObjectId);
                WriteInt32(stream, star.Planets.Count);
                foreach (ScriptPlanetBindingRecord planet in star.Planets)
                {
                    if (planet == null || !IsSupportedItemText(planet.Name, 512) ||
                        !IsKnownPlanetObjectId(planet.PlanetObjectId))
                        throw new InvalidOperationException("TScriptStar: неверная ссылка на планету.");
                    WriteUtf16Z(stream, planet.Name ?? string.Empty);
                    WriteUInt32(stream, planet.PlanetObjectId);
                }
                WriteInt32(stream, star.LegacyZero);
            }

            WriteInt32(stream, script.ItemBindings.Count);
            foreach (ScriptItemRecord item in script.ItemBindings)
            {
                if (item == null || !IsSupportedItemText(item.Name, 512) ||
                    !IsSupportedItemText(item.TextData1, 4096) ||
                    !IsSupportedItemText(item.TextData2, 4096) ||
                    !IsSupportedItemText(item.TextData3, 4096) ||
                    !IsSupportedItemText(item.OnUseCode, 262144) ||
                    !IsSupportedItemText(item.OnActCode, 262144) ||
                    (item.ItemObjectId != 0 && !IsKnownItemObjectId(item.ItemObjectId)))
                    throw new InvalidOperationException("TScriptItem: неверный текст или ссылка на предмет.");
                WriteUtf16Z(stream, item.Name ?? string.Empty);
                WriteBoolean(stream, item.CanSell);
                WriteInt32(stream, item.Data1);
                WriteInt32(stream, item.Data2);
                WriteInt32(stream, item.Data3);
                WriteUtf16Z(stream, item.TextData1 ?? string.Empty);
                WriteUtf16Z(stream, item.TextData2 ?? string.Empty);
                WriteUtf16Z(stream, item.TextData3 ?? string.Empty);
                WriteUtf16Z(stream, item.OnUseCode ?? string.Empty);
                WriteUtf16Z(stream, item.OnActCode ?? string.Empty);
                WriteUInt32(stream, item.ItemObjectId);
            }

            WriteUInt16(stream, checked((ushort)script.ShipBindings.Count));
            foreach (ScriptShipRecord ship in script.ShipBindings)
            {
                if (ship == null || !IsKnownShipObjectId(ship.ShipObjectId) ||
                    !IsSupportedItemText(ship.CustomFaction, 4096))
                    throw new InvalidOperationException("TScriptShip: неверная ссылка на корабль или фракция.");
                WriteInt32(stream, ship.Group);
                WriteUInt32(stream, ship.ShipObjectId);
                WriteUInt32(stream, ship.Data0);
                WriteUInt32(stream, ship.Data1);
                WriteUInt32(stream, ship.Data2);
                WriteUInt32(stream, ship.Data3);
                WriteInt32(stream, ship.StateNum);
                WriteUtf16Z(stream, ship.CustomFaction ?? string.Empty);
                WriteBoolean(stream, ship.Hit);
                WriteBoolean(stream, ship.HitPlayer);
            }

            WriteUInt16(stream, checked((ushort)script.EtherStrings.Count));
            foreach (string ether in script.EtherStrings)
            {
                if (!IsSupportedItemText(ether, 262144))
                    throw new InvalidOperationException("TScript: неверная строка эфира.");
                WriteUtf16Z(stream, ether ?? string.Empty);
            }
        }

        private void AddGalaxySummaryPatches(List<PayloadPatch> patches, GalaxySummaryData original,
            GalaxySummaryData updated, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            if (updated.DifficultyLevels == null || updated.DifficultyLevels.Length != 8 ||
                updated.CustomRuleLevels == null || updated.CustomRuleLevels.Length != 19 ||
                updated.CustomRuleFlags == null || updated.CustomRuleFlags.Length != 15 ||
                updated.CustomRuleLateFlags == null || updated.CustomRuleLateFlags.Length != 8)
                throw new InvalidOperationException("TGalaxy: неверный размер массива уровней сложности или тонких настроек.");
            for (int index = 0; index < updated.DifficultyLevels.Length; index++)
                if (updated.DifficultyLevels[index] > 16)
                    throw new InvalidOperationException("TGalaxy: уровень сложности должен быть в диапазоне 0..16.");
            if (updated.HullGrowth > 2)
                throw new InvalidOperationException("TGalaxy: режим роста корпуса должен быть в диапазоне 0..2.");
            if (!IsFiniteGalaxyScalar(updated.BlazerResearch) ||
                !IsFiniteGalaxyScalar(updated.KellerResearch) ||
                !IsFiniteGalaxyScalar(updated.TerronResearch))
                throw new InvalidOperationException("TGalaxy: значения исследований должны быть конечными числами.");
            if (updated.TurnOffset != original.TurnOffset ||
                updated.DifficultyOffset != original.DifficultyOffset ||
                updated.LateScalarOffset != original.LateScalarOffset ||
                updated.KellerAttackOffset != original.KellerAttackOffset ||
                updated.ScriptShopSlotCountOffset != original.ScriptShopSlotCountOffset ||
                updated.ScriptShopSlotListEndOffset != original.ScriptShopSlotListEndOffset ||
                updated.GlobalVariableListOffset != original.GlobalVariableListOffset ||
                updated.ScriptCacheListOffset != original.ScriptCacheListOffset ||
                updated.ActiveScriptListOffset != original.ActiveScriptListOffset ||
                updated.WarOperationListOffset != original.WarOperationListOffset ||
                updated.GateListOffset != original.GateListOffset ||
                updated.RangerReferenceListOffset != original.RangerReferenceListOffset ||
                updated.GalaxyEventListOffset != original.GalaxyEventListOffset ||
                updated.GalaxyEventListEndOffset != original.GalaxyEventListEndOffset ||
                updated.IronWillOffset != original.IronWillOffset ||
                updated.PlanetBattlesDisabledOffset != original.PlanetBattlesDisabledOffset ||
                updated.CustomRulesOffset != original.CustomRulesOffset || updated.End != original.End ||
                updated.PlayerObjectId != original.PlayerObjectId ||
                updated.CurrentObjectId != original.CurrentObjectId ||
                updated.NextObjectId != original.NextObjectId || updated.SystemCrc != original.SystemCrc)
                throw new InvalidOperationException("TGalaxy: структурные смещения и объектные ссылки доступны только для чтения.");
            if (updated.PrincipalObjectOffset != original.PrincipalObjectOffset ||
                updated.AutoBattleShipObjectId != original.AutoBattleShipObjectId &&
                updated.AutoBattleShipObjectId != 0)
                throw new InvalidOperationException(
                    "TGalaxy.AutoBattleShip: разрешено только обнуление удалённой ссылки.");
            if (updated.RangerObjectIds == null || original.RangerObjectIds == null ||
                updated.RangerCount != updated.RangerObjectIds.Length ||
                original.RangerCount != original.RangerObjectIds.Length ||
                updated.RangerCount > original.RangerCount ||
                !IsOrderedUIntSubset(original.RangerObjectIds, updated.RangerObjectIds))
                throw new InvalidOperationException(
                    "TGalaxy.Rangers: разрешены только исходный порядок и удаление рейнджеров.");
            if (updated.EminentRangerObjectIds == null ||
                original.EminentRangerObjectIds == null ||
                updated.EminentRangerObjectIds.Length != original.EminentRangerObjectIds.Length)
                throw new InvalidOperationException("TGalaxy: неверный массив знаменитых рейнджеров.");
            for (int index = 0; index < updated.EminentRangerObjectIds.Length; index++)
                if (updated.EminentRangerObjectIds[index] !=
                    original.EminentRangerObjectIds[index] &&
                    updated.EminentRangerObjectIds[index] != 0)
                    throw new InvalidOperationException(
                        "TGalaxy: ссылку знаменитого рейнджера можно только обнулить.");
            if (!IsKnownStarObjectId(updated.KellerAttackStarObjectId))
                throw new InvalidOperationException("TGalaxy: для атаки Келлера выбрана неизвестная система.");
            if (updated.WarOperations == null || updated.WarOperations.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число военных операций.");
            if (updated.ActiveScripts == null || updated.ActiveScripts.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число активных скриптов.");
            if (updated.GlobalVariables == null || updated.GlobalVariables.Count > 10000 ||
                updated.ScriptCache == null || updated.ScriptCache.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число глобальных переменных или записей кэша.");
            if (updated.Gates == null || updated.Gates.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число врат.");
            if (updated.CompleteQuests == null || updated.CompleteQuests.Count > ushort.MaxValue ||
                updated.GalaxyNews == null || updated.GalaxyNews.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число заданий или новостей.");
            if (updated.GalaxyEvents == null || updated.GalaxyEvents.Count > ushort.MaxValue)
                throw new InvalidOperationException("TGalaxy: неверное число галактических событий.");

            bool questsChanged = updated.CompleteQuests.Count != original.CompleteQuests.Count;
            if (!questsChanged)
                for (int index = 0; index < original.CompleteQuests.Count; index++)
                    if (!original.CompleteQuests[index].ContentEquals(updated.CompleteQuests[index]))
                    { questsChanged = true; break; }
            if (questsChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.CompleteQuests.Count));
                    foreach (CompleteQuestRecord record in updated.CompleteQuests)
                    {
                        if (!IsKnownPlanetObjectId(record.PlanetObjectId) ||
                            !IsSupportedItemText(record.Text, 32768))
                            throw new InvalidOperationException("TGalaxy: задание содержит неверную планету или текст.");
                        WriteUInt32(encoded, record.PlanetObjectId);
                        encoded.WriteByte(record.Type);
                        WriteUInt16(encoded, record.Number);
                        WriteUtf16Z(encoded, record.Text ?? string.Empty);
                        WriteBoolean(encoded, record.Successful);
                        WriteBoolean(encoded, record.Rejection);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.CompleteQuestListOffset,
                        prefixDelta, stars), original.GalaxyNewsListOffset - original.CompleteQuestListOffset,
                        encoded.ToArray()));
                }
            }
            bool newsChanged = updated.GalaxyNews.Count != original.GalaxyNews.Count;
            if (!newsChanged)
                for (int index = 0; index < original.GalaxyNews.Count; index++)
                    if (!original.GalaxyNews[index].ContentEquals(updated.GalaxyNews[index]))
                    { newsChanged = true; break; }
            if (newsChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.GalaxyNews.Count));
                    foreach (GalaxyNewsRecord record in updated.GalaxyNews)
                    {
                        if (!IsSupportedItemText(record.Text, 32768))
                            throw new InvalidOperationException("TGalaxy: новость содержит неверный текст.");
                        WriteUInt32(encoded, record.Id);
                        WriteUInt32(encoded, record.Turn);
                        encoded.WriteByte(record.Type);
                        WriteUtf16Z(encoded, record.Text ?? string.Empty);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.GalaxyNewsListOffset,
                        prefixDelta, stars), original.LateScalarOffset - original.GalaxyNewsListOffset,
                        encoded.ToArray()));
                }
            }

            bool eventsChanged = !GalaxySummaryData.EqualRecords(original.GalaxyEvents,
                updated.GalaxyEvents);
            if (eventsChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.GalaxyEvents.Count));
                    foreach (GalaxyEventRecord galaxyEvent in updated.GalaxyEvents)
                    {
                        if (galaxyEvent == null || string.IsNullOrEmpty(galaxyEvent.Type) ||
                            !IsSupportedItemText(galaxyEvent.Type, 128) || galaxyEvent.Data == null ||
                            galaxyEvent.Data.Count > 10000 || galaxyEvent.TextData == null ||
                            galaxyEvent.TextData.Count > 10000)
                            throw new InvalidOperationException("TGalaxyEvent: неверный тип или размер списка.");
                        WriteUtf16Z(encoded, galaxyEvent.Type);
                        WriteInt32(encoded, galaxyEvent.Turn);
                        WriteInt32(encoded, galaxyEvent.Data.Count);
                        foreach (int value in galaxyEvent.Data) WriteInt32(encoded, value);
                        WriteInt32(encoded, galaxyEvent.TextData.Count);
                        foreach (string value in galaxyEvent.TextData)
                        {
                            if (!IsSupportedItemText(value, 32768))
                                throw new InvalidOperationException("TGalaxyEvent: текстовые данные слишком велики.");
                            WriteUtf16Z(encoded, value ?? string.Empty);
                        }
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.GalaxyEventListOffset,
                        prefixDelta, stars), original.GalaxyEventListEndOffset -
                        original.GalaxyEventListOffset, encoded.ToArray()));
                }
            }

            bool globalsChanged = !GalaxySummaryData.EqualVariables(original.GlobalVariables,
                updated.GlobalVariables);
            bool cacheChanged = !GalaxySummaryData.EqualScriptCache(original.ScriptCache,
                updated.ScriptCache);
            if (globalsChanged || cacheChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteScriptVariableArray(encoded, updated.GlobalVariables, 0, true);
                    WriteUInt16(encoded, checked((ushort)updated.ScriptCache.Count));
                    foreach (ScriptCacheRecord cache in updated.ScriptCache)
                    {
                        if (cache == null || !IsSupportedItemText(cache.Name, 4096))
                            throw new InvalidOperationException("TScriptCache: неверное имя записи.");
                        WriteUtf16Z(encoded, cache.Name ?? string.Empty);
                        WriteUInt16(encoded, cache.CountUse);
                        WriteInt32(encoded, cache.LastTurn);
                        WriteInt32(encoded, cache.RunScript);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.GlobalVariableListOffset,
                        prefixDelta, stars), original.ActiveScriptListOffset - original.GlobalVariableListOffset,
                        encoded.ToArray()));
                }
            }

            bool scriptsChanged = updated.ActiveScripts.Count != original.ActiveScripts.Count;
            if (!scriptsChanged)
                for (int index = 0; index < original.ActiveScripts.Count; index++)
                    if (!original.ActiveScripts[index].ContentEquals(updated.ActiveScripts[index]))
                    { scriptsChanged = true; break; }
            if (scriptsChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.ActiveScripts.Count));
                    foreach (ScriptRecord script in updated.ActiveScripts) WriteScript(encoded, script);
                    patches.Add(new PayloadPatch(MapKnownOffset(original.ActiveScriptListOffset,
                        prefixDelta, stars), original.WarOperationListOffset - original.ActiveScriptListOffset,
                        encoded.ToArray()));
                }
            }

            bool warOperationsChanged = updated.WarOperations.Count != original.WarOperations.Count;
            if (!warOperationsChanged)
                for (int index = 0; index < original.WarOperations.Count; index++)
                    if (!original.WarOperations[index].ContentEquals(updated.WarOperations[index]))
                    { warOperationsChanged = true; break; }
            if (warOperationsChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.WarOperations.Count));
                    foreach (WarOperationRecord operation in updated.WarOperations)
                    {
                        if (operation.LegacyZero != 0 || operation.ShipObjectIds == null ||
                            operation.ShipObjectIds.Count > ushort.MaxValue || operation.Orders == null ||
                            operation.Orders.Count > ushort.MaxValue)
                            throw new InvalidOperationException("TWarOperation: неверный размер списка или служебный байт.");
                        WriteUInt16(encoded, operation.Turn);
                        WriteUInt32(encoded, operation.RandomSeed);
                        WriteUInt32(encoded, operation.RandomOut);
                        encoded.WriteByte(operation.LegacyZero);
                        WriteUInt16(encoded, checked((ushort)operation.ShipObjectIds.Count));
                        foreach (uint shipId in operation.ShipObjectIds)
                        {
                            if (!IsKnownShipObjectId(shipId))
                                throw new InvalidOperationException("TWarOperation: ссылка ведёт на неизвестный корабль.");
                            WriteUInt32(encoded, shipId);
                        }
                        WriteUInt16(encoded, checked((ushort)operation.Orders.Count));
                        foreach (WarOperationOrderRecord order in operation.Orders)
                        {
                            if (order.Type > 7 || !IsFiniteGalaxyScalar(order.DestinationX) ||
                                !IsFiniteGalaxyScalar(order.DestinationY))
                                throw new InvalidOperationException("TWarOperation: приказ содержит неверный тип или координаты.");
                            encoded.WriteByte(order.Type);
                            WriteUInt32(encoded, order.ObjectId);
                            WriteSingle(encoded, order.DestinationX);
                            WriteSingle(encoded, order.DestinationY);
                            encoded.WriteByte(order.EndMode);
                            WriteInt32(encoded, order.EndTurn);
                        }
                    }
                    int oldEnd = checked(original.TurnOffset - 11);
                    patches.Add(new PayloadPatch(MapKnownOffset(original.WarOperationListOffset,
                        prefixDelta, stars), oldEnd - original.WarOperationListOffset, encoded.ToArray()));
                }
            }

            bool gatesChanged = updated.Gates.Count != original.Gates.Count;
            if (!gatesChanged)
                for (int index = 0; index < original.Gates.Count; index++)
                    if (!original.Gates[index].ContentEquals(updated.Gates[index]))
                    { gatesChanged = true; break; }
            if (gatesChanged)
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.Gates.Count));
                    foreach (GateRecord gate in updated.Gates)
                    {
                        if (!IsFiniteGalaxyScalar(gate.X) || !IsFiniteGalaxyScalar(gate.Y) ||
                            Math.Abs((double)gate.X) > 10000 || Math.Abs((double)gate.Y) > 10000 ||
                            gate.Size > 10000 || !IsSupportedItemText(gate.Text, 4096))
                            throw new InvalidOperationException("TGate: неверные координаты, размер или текст.");
                        WriteSingle(encoded, gate.X);
                        WriteSingle(encoded, gate.Y);
                        encoded.WriteByte(gate.Angle);
                        WriteUInt16(encoded, gate.Size);
                        WriteUtf16Z(encoded, gate.Text ?? string.Empty);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.GateListOffset, prefixDelta, stars),
                        original.PlanetReferenceListOffset - original.GateListOffset, encoded.ToArray()));
                }
            }

            for (int index = 0; index < updated.DifficultyLevels.Length; index++)
                AddBytePatch(patches, original.DifficultyOffset + index, original.DifficultyLevels[index],
                    updated.DifficultyLevels[index], prefixDelta, stars);
            if (updated.AutoBattleShipObjectId != original.AutoBattleShipObjectId &&
                updated.AutoBattleShipObjectId != 0 &&
                !IsKnownShipObjectId(updated.AutoBattleShipObjectId))
                throw new InvalidOperationException(
                    "TGalaxy.AutoBattleShip: новая ссылка ведёт на неизвестный корабль.");
            AddUInt32Patch(patches, original.PrincipalObjectOffset + 4,
                original.AutoBattleShipObjectId, updated.AutoBattleShipObjectId,
                prefixDelta, stars);
            for (int index = 0; index < updated.EminentRangerObjectIds.Length; index++)
            {
                if (updated.EminentRangerObjectIds[index] !=
                    original.EminentRangerObjectIds[index] &&
                    updated.EminentRangerObjectIds[index] != 0 &&
                    !IsKnownShipObjectId(updated.EminentRangerObjectIds[index]))
                    throw new InvalidOperationException(
                        "TGalaxy.EminentRangers: новая ссылка ведёт на неизвестный корабль.");
                AddUInt32Patch(patches, original.PrincipalObjectOffset + 28 + index * 4,
                    original.EminentRangerObjectIds[index],
                    updated.EminentRangerObjectIds[index], prefixDelta, stars);
            }
            if (!EqualUIntArrays(original.RangerObjectIds, updated.RangerObjectIds))
            {
                using (MemoryStream encoded = new MemoryStream())
                {
                    WriteUInt16(encoded, checked((ushort)updated.RangerCount));
                    foreach (uint objectId in updated.RangerObjectIds)
                    {
                        if (!IsKnownShipObjectId(objectId) &&
                            !ContainsUInt(original.RangerObjectIds, objectId))
                            throw new InvalidOperationException(
                                "TGalaxy.Rangers: новая ссылка ведёт на неизвестный корабль.");
                        WriteUInt32(encoded, objectId);
                    }
                    patches.Add(new PayloadPatch(MapKnownOffset(original.RangerReferenceListOffset,
                        prefixDelta, stars), 2 + original.RangerCount * 4, encoded.ToArray()));
                }
            }
            AddUInt32Patch(patches, original.KellerAttackOffset, original.KellerAttackStarObjectId,
                updated.KellerAttackStarObjectId, prefixDelta, stars);
            AddInt32Patch(patches, original.KellerAttackOffset + 4, original.KellerAttackState,
                updated.KellerAttackState, prefixDelta, stars);
            int scalarOffset = original.LateScalarOffset;
            AddFloatPatch(patches, scalarOffset + 12, original.BlazerResearch,
                updated.BlazerResearch, prefixDelta, stars);
            AddUInt32Patch(patches, scalarOffset + 16, original.BlazerMaterial,
                updated.BlazerMaterial, prefixDelta, stars);
            AddFloatPatch(patches, scalarOffset + 20, original.KellerResearch,
                updated.KellerResearch, prefixDelta, stars);
            AddUInt32Patch(patches, scalarOffset + 24, original.KellerMaterial,
                updated.KellerMaterial, prefixDelta, stars);
            AddFloatPatch(patches, scalarOffset + 28, original.TerronResearch,
                updated.TerronResearch, prefixDelta, stars);
            AddUInt32Patch(patches, scalarOffset + 32, original.TerronMaterial,
                updated.TerronMaterial, prefixDelta, stars);
            AddInt32Patch(patches, scalarOffset + 40, original.WarDeltaDominators,
                updated.WarDeltaDominators, prefixDelta, stars);
            AddInt32Patch(patches, scalarOffset + 44, original.WarDeltaPirates,
                updated.WarDeltaPirates, prefixDelta, stars);
            AddInt32Patch(patches, scalarOffset + 48, original.WarDeltaCoalition,
                updated.WarDeltaCoalition, prefixDelta, stars);
            AddBytePatch(patches, original.IronWillOffset, original.IronWill ? (byte)1 : (byte)0,
                updated.IronWill ? (byte)1 : (byte)0, prefixDelta, stars);
            AddBytePatch(patches, original.PlanetBattlesDisabledOffset,
                original.PlanetBattlesDisabled ? (byte)1 : (byte)0,
                updated.PlanetBattlesDisabled ? (byte)1 : (byte)0, prefixDelta, stars);

            int customOffset = original.CustomRulesOffset;
            AddBytePatch(patches, customOffset, original.CustomRules ? (byte)1 : (byte)0,
                updated.CustomRules ? (byte)1 : (byte)0, prefixDelta, stars);
            for (int index = 0; index < updated.CustomRuleLevels.Length; index++)
                AddBytePatch(patches, customOffset + 1 + index, original.CustomRuleLevels[index],
                    updated.CustomRuleLevels[index], prefixDelta, stars);
            for (int index = 0; index < updated.CustomRuleFlags.Length; index++)
                AddBytePatch(patches, customOffset + 20 + index,
                    original.CustomRuleFlags[index] ? (byte)1 : (byte)0,
                    updated.CustomRuleFlags[index] ? (byte)1 : (byte)0, prefixDelta, stars);
            AddBytePatch(patches, customOffset + 35, original.HullGrowth, updated.HullGrowth,
                prefixDelta, stars);
            for (int index = 0; index < updated.CustomRuleLateFlags.Length; index++)
                AddBytePatch(patches, customOffset + 36 + index,
                    original.CustomRuleLateFlags[index] ? (byte)1 : (byte)0,
                    updated.CustomRuleLateFlags[index] ? (byte)1 : (byte)0, prefixDelta, stars);
        }

        private static bool IsOrderedUIntSubset(uint[] original, uint[] updated)
        {
            int source = 0;
            foreach (uint value in updated)
            {
                while (source < original.Length && original[source] != value) source++;
                if (source >= original.Length) return false;
                source++;
            }
            return true;
        }

        private static bool IsOrderedByteSubset(byte[] original, byte[] updated)
        {
            int source = 0;
            foreach (byte value in updated)
            {
                while (source < original.Length && original[source] != value) source++;
                if (source >= original.Length) return false;
                source++;
            }
            return true;
        }

        private static bool EqualUIntArrays(uint[] left, uint[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index]) return false;
            return true;
        }

        private static bool ContainsUInt(uint[] values, uint expected)
        {
            if (values == null) return false;
            foreach (uint value in values) if (value == expected) return true;
            return false;
        }

        private bool IsKnownPlanetObjectId(uint objectId)
        {
            if (objectId == 0) return true;
            foreach (PlanetHeaderRecord planet in GalaxyPlanets)
                if (planet.ObjectId == objectId) return true;
            return false;
        }

        private bool IsKnownStarObjectId(uint objectId)
        {
            if (objectId == 0) return true;
            foreach (StarHeaderRecord star in GalaxyStars)
                if (star.ObjectId == objectId) return true;
            return false;
        }

        private bool IsKnownShipObjectId(uint objectId)
        {
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.ObjectId == objectId) return true;
            return false;
        }

        private bool IsKnownItemObjectId(uint objectId)
        {
            foreach (ItemHeaderRecord item in GalaxyItems)
                if (item.ObjectId == objectId) return true;
            return false;
        }

        private void AddFloatPatch(List<PayloadPatch> patches, int originalOffset, float original,
            float updated, int prefixDelta, IList<StarHeaderRecord> stars,
            [System.Runtime.CompilerServices.CallerMemberName] string source = null)
        {
            if (original != updated)
                patches.Add(new PayloadPatch(MapKnownOffset(originalOffset, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated), source + "@0x" + originalOffset.ToString("X")));
        }

        private void AddInt32Patch(List<PayloadPatch> patches, int originalOffset, int original,
            int updated, int prefixDelta, IList<StarHeaderRecord> stars,
            [System.Runtime.CompilerServices.CallerMemberName] string source = null)
        {
            if (original != updated)
                patches.Add(new PayloadPatch(MapKnownOffset(originalOffset, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated), source + "@0x" + originalOffset.ToString("X")));
        }

        private void AddUInt32Patch(List<PayloadPatch> patches, int originalOffset, uint original,
            uint updated, int prefixDelta, IList<StarHeaderRecord> stars,
            [System.Runtime.CompilerServices.CallerMemberName] string source = null)
        {
            if (original != updated)
                patches.Add(new PayloadPatch(MapKnownOffset(originalOffset, prefixDelta, stars), 4,
                    BitConverter.GetBytes(updated), source + "@0x" + originalOffset.ToString("X")));
        }

        private void AddUInt16Patch(List<PayloadPatch> patches, int originalOffset, ushort original,
            ushort updated, int prefixDelta, IList<StarHeaderRecord> stars,
            [System.Runtime.CompilerServices.CallerMemberName] string source = null)
        {
            if (original != updated)
                patches.Add(new PayloadPatch(MapKnownOffset(originalOffset, prefixDelta, stars), 2,
                    BitConverter.GetBytes(updated), source + "@0x" + originalOffset.ToString("X")));
        }

        private void AddBytePatch(List<PayloadPatch> patches, int originalOffset, byte original,
            byte updated, int prefixDelta, IList<StarHeaderRecord> stars,
            [System.Runtime.CompilerServices.CallerMemberName] string source = null)
        {
            if (original != updated)
                patches.Add(new PayloadPatch(MapKnownOffset(originalOffset, prefixDelta, stars), 1,
                    new byte[] { updated }, source + "@0x" + originalOffset.ToString("X")));
        }

        private int MapKnownOffset(int originalOffset, int prefixDelta, IList<StarHeaderRecord> stars)
        {
            int mapped = checked(originalOffset + prefixDelta);
            for (int index = 0; index < GalaxyStars.Count; index++)
                if (originalOffset >= GalaxyStars[index].HeaderEnd)
                    mapped = checked(mapped + Utf16ZLength(stars[index].Name) - Utf16ZLength(GalaxyStars[index].Name));
            return mapped;
        }

        private static bool IsSupportedEditableText(string value, int maximumLength, bool allowEmpty)
        {
            value = value ?? string.Empty;
            if ((!allowEmpty && value.Length == 0) || value.Length > maximumLength) return false;
            foreach (char character in value)
                if (!IsSupportedObjectTextCharacter(character)) return false;
            return true;
        }

        private static bool IsSupportedItemText(string value, int maximumLength)
        {
            value = value ?? string.Empty;
            if (value.Length > maximumLength) return false;
            foreach (char character in value)
                if (character == '\0' || (char.IsControl(character) && character != '\r' &&
                    character != '\n' && character != '\t')) return false;
            return true;
        }

        private static int Utf16ZLength(string value)
        {
            return checked(((value ?? string.Empty).Length + 1) * 2);
        }

        private static byte[] EncodeUtf16Z(string value)
        {
            byte[] text = Encoding.Unicode.GetBytes(value ?? string.Empty);
            byte[] result = new byte[text.Length + 2];
            Buffer.BlockCopy(text, 0, result, 0, text.Length);
            return result;
        }

        private static byte[] JoinBytes(byte[] left, byte[] right)
        {
            byte[] result = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, result, 0, left.Length);
            Buffer.BlockCopy(right, 0, result, left.Length, right.Length);
            return result;
        }

        private static byte[] EncodeOptionalString(string value)
        {
            return string.IsNullOrEmpty(value) ? new byte[] { 0 } :
                JoinBytes(new byte[] { 1 }, EncodeUtf16Z(value));
        }

        private byte[] Serialize()
        {
            return Serialize(StoredCrc32, EncryptedMainBlock);
        }

        private void ParseInnerEnvelope()
        {
            int offset = MetadataSize;
            PlayerMessageCount = checked((int)Metadata.PlayerMessageCount);
            for (int index = 0; index < PlayerMessageCount; index++)
            {
                PlayerMessageRecord message = new PlayerMessageRecord();
                message.Start = offset;
                message.Text = ReadUtf16Z(MainPayload, ref offset, "player message text");
                message.MessageType = ReadByte(MainPayload, ref offset, "player message type");
                message.Raw18 = ReadInt32(MainPayload, ref offset, "player message raw 18");
                message.Raw1C = ReadInt32(MainPayload, ref offset, "player message raw 1C");
                message.FormattedText = ReadUtf16Z(MainPayload, ref offset, "player message formatted text");
                message.RawBool = ReadBoolean(MainPayload, ref offset, "player message raw flag");
                message.RawU32 = new uint[6];
                for (int field = 0; field < message.RawU32.Length; field++)
                    message.RawU32[field] = ReadUInt32(MainPayload, ref offset, "player message UInt32 field");
                message.Flag40 = ReadBoolean(MainPayload, ref offset, "player message flag 40");
                message.Flag41 = ReadBoolean(MainPayload, ref offset, "player message flag 41");
                if (Version > 108)
                    message.LateText = ReadUtf16Z(MainPayload, ref offset, "player message late text");
                message.End = offset;
                PlayerMessages.Add(message);
            }
            PlayerHoldCount = ReadUInt16(MainPayload, ref offset, "player hold count");
            for (int index = 0; index < PlayerHoldCount; index++)
            {
                PlayerHoldRecord unit = new PlayerHoldRecord();
                unit.Start = offset;
                unit.UnitType = ReadByte(MainPayload, ref offset, "player hold unit type");
                unit.Goods = ReadByte(MainPayload, ref offset, "player hold goods");
                unit.ObjectId = ReadUInt32(MainPayload, ref offset, "player hold object id");
                unit.End = offset;
                PlayerHoldUnits.Add(unit);
            }
            GalaxyOffset = offset;
            if (GalaxyOffset >= MainPayload.Length)
                throw new SavFormatException("После player envelope отсутствуют данные TGalaxy.");
        }

        private void ParseGalaxyPrefix()
        {
            if (Version < 127)
                throw new SavFormatException("Префикс TGalaxy доказан только для актуальной схемы SAV.");
            int offset = GalaxyOffset;
            GalaxyPrefixData value = new GalaxyPrefixData();
            value.Start = offset;
            value.UsedMods = Version > 100 ? ReadUtf16Z(MainPayload, ref offset, "galaxy used mods") : string.Empty;
            value.RandomSeed = ReadInt32(MainPayload, ref offset, "galaxy random seed");
            value.RandomOut = ReadUInt32(MainPayload, ref offset, "galaxy random out");
            value.RangersAverageCapital = ReadInt32(MainPayload, ref offset, "galaxy rangers average capital");
            value.RangersMaxCapital = ReadInt32(MainPayload, ref offset, "galaxy rangers max capital");
            value.RangersAverageStrength = ReadSingle(MainPayload, ref offset, "galaxy rangers average strength");
            value.RangersMaxStrength = ReadSingle(MainPayload, ref offset, "galaxy rangers max strength");
            value.Crack = ReadBoolean(MainPayload, ref offset, "galaxy crack");
            value.Cheat = ReadBoolean(MainPayload, ref offset, "galaxy cheat");
            value.ReservedZero = ReadInt32(MainPayload, ref offset, "galaxy reserved zero");
            if (value.ReservedZero != 0)
                throw new SavFormatException("Зарезервированное поле TGalaxy не равно нулю.");
            value.CheatPoints = ReadInt32(MainPayload, ref offset, "galaxy cheat points");
            value.SaveCount = ReadInt32(MainPayload, ref offset, "galaxy save count");
            value.LoadCount = ReadInt32(MainPayload, ref offset, "galaxy load count");
            value.CustomModWeaponCount = ReadUInt16(MainPayload, ref offset, "galaxy custom mod weapon count");
            value.End = offset;
            GalaxyPrefix = value;
        }

        private void ParseGalaxyDirectory()
        {
            int offset = GalaxyPrefix.End;
            for (int index = 0; index < GalaxyPrefix.CustomModWeaponCount; index++)
            {
                CustomWeaponInfoRecord weapon = new CustomWeaponInfoRecord();
                weapon.Start = offset;
                weapon.SystemName = ReadUtf16Z(MainPayload, ref offset, "custom weapon system name");
                CustomWeaponNames.Add(weapon.SystemName);
                weapon.TechLevel = ReadByte(MainPayload, ref offset, "custom weapon tech level");
                weapon.TechRadius = ReadByte(MainPayload, ref offset, "custom weapon tech radius");
                weapon.ModCost = ReadSingle(MainPayload, ref offset, "custom weapon mod cost");
                weapon.MinDamage = ReadInt32(MainPayload, ref offset, "custom weapon min damage");
                weapon.MaxDamage = ReadInt32(MainPayload, ref offset, "custom weapon max damage");
                weapon.AverageSize = ReadInt32(MainPayload, ref offset, "custom weapon average size");
                weapon.AverageRadius = ReadInt32(MainPayload, ref offset, "custom weapon average radius");
                weapon.Speed = ReadInt32(MainPayload, ref offset, "custom weapon speed");
                weapon.MissileRadius = ReadInt32(MainPayload, ref offset, "custom weapon missile radius");
                weapon.MissileMinSpeed = ReadInt32(MainPayload, ref offset, "custom weapon missile min speed");
                weapon.MissileMaxSpeed = ReadInt32(MainPayload, ref offset, "custom weapon missile max speed");
                weapon.MissileChanceToBeHit = ReadByte(MainPayload, ref offset,
                    "custom weapon missile chance to be hit");
                weapon.DamageType = ReadUInt32(MainPayload, ref offset, "custom weapon damage type");
                weapon.ShotType = ReadByte(MainPayload, ref offset, "custom weapon shot type");
                weapon.ShotCount = ReadByte(MainPayload, ref offset, "custom weapon shot count");
                weapon.AttackCount = ReadByte(MainPayload, ref offset, "custom weapon attack count");
                weapon.SecondaryDamageRadius = ReadSingle(MainPayload, ref offset,
                    "custom weapon secondary damage radius");
                weapon.MiningFactor = ReadSingle(MainPayload, ref offset, "custom weapon mining factor");
                for (int field = 0; field < weapon.WeaponDamageSet.Length; field++)
                    weapon.WeaponDamageSet[field] = ReadSingle(MainPayload, ref offset,
                        "custom weapon damage set");
                weapon.PrimarySE = ReadOptionalString(MainPayload, ref offset, "custom weapon primary SE");
                weapon.SecondarySE = ReadOptionalString(MainPayload, ref offset, "custom weapon secondary SE");
                weapon.AreaSE = ReadOptionalString(MainPayload, ref offset, "custom weapon area SE");
                weapon.DefaultPalette = ReadInt32(MainPayload, ref offset, "custom weapon default palette");
                weapon.Availability = ReadByte(MainPayload, ref offset, "custom weapon availability");
                weapon.ABWeaponType = ReadByte(MainPayload, ref offset, "custom weapon AB weapon type");
                weapon.End = offset;
                CustomWeaponInfos.Add(weapon);
                if (!CustomWeaponDescriptorTypes.ContainsKey(weapon.SystemName))
                    CustomWeaponDescriptorTypes.Add(weapon.SystemName, weapon.ShotType);
            }

            GalaxyConstellationCount = ReadBoundedCount(MainPayload, ref offset, "constellation count");
            for (int index = 0; index < GalaxyConstellationCount; index++)
            {
                ConstellationRecord constellation = new ConstellationRecord();
                constellation.Start = offset;
                constellation.ObjectId = ReadUInt32(MainPayload, ref offset, "constellation id");
                constellation.VisibleOffset = offset;
                constellation.Visible = ReadBoolean(MainPayload, ref offset, "constellation visible");
                constellation.Color = (ushort)ReadUInt16(MainPayload, ref offset, "constellation color");
                constellation.X = ReadSingle(MainPayload, ref offset, "constellation x");
                constellation.Y = ReadSingle(MainPayload, ref offset, "constellation y");
                constellation.StarObjectIds = ReadUInt32List(MainPayload, ref offset, "constellation star references");
                constellation.ConnectionObjectIds = ReadUInt32List(MainPayload, ref offset, "constellation connection references");
                constellation.BoundaryLines = ReadMapLineList(MainPayload, ref offset,
                    "constellation visible boundaries");
                constellation.HiddenBoundaryLines = ReadMapLineList(MainPayload, ref offset,
                    "constellation hidden boundaries");
                Skip(MainPayload, ref offset, 24, "constellation scalar fields");
                constellation.MapLines = ReadMapLineList(MainPayload, ref offset, "constellation map lines");
                SkipPolygonList(MainPayload, ref offset, "constellation polygon group 1");
                SkipPolygonList(MainPayload, ref offset, "constellation polygon group 2");
                GalaxyConstellations.Add(constellation);
            }
            GalaxyStarCount = ReadBoundedCount(MainPayload, ref offset, "star count");
            GalaxyStarsOffset = offset;
            ParseStarHeaders();
        }

        private void ParseStarHeaders()
        {
            List<StarHeaderRecord>[] matches = new List<StarHeaderRecord>[GalaxyStarCount + 1];
            for (int index = 0; index < matches.Length; index++)
                matches[index] = new List<StarHeaderRecord>();
            for (int offset = GalaxyStarsOffset; offset <= MainPayload.Length - 32; offset++)
            {
                uint objectId = ReadUInt32(MainPayload, offset);
                if (objectId < 1 || objectId > GalaxyStarCount) continue;
                StarHeaderRecord candidate = TryReadStarHeader(offset, objectId);
                if (candidate != null) matches[objectId].Add(candidate);
            }
            int previous = GalaxyStarsOffset - 1;
            for (uint objectId = 1; objectId <= GalaxyStarCount; objectId++)
            {
                StarHeaderRecord selected = null;
                int selectedCount = 0;
                foreach (StarHeaderRecord candidate in matches[objectId])
                    if (candidate.Start > previous)
                    {
                        selected = candidate;
                        selectedCount++;
                    }
                if (selectedCount != 1)
                    throw new SavFormatException("TStar " + objectId + ": ожидался один последовательный заголовок, найдено " + selectedCount + ".");
                GalaxyStars.Add(selected);
                previous = selected.Start;
            }
        }

        private void ParseGalaxySummary()
        {
            int turn;
            if (!int.TryParse(Header[3], out turn) || turn < 0)
                throw new SavFormatException("Текущий ход в заголовке SAV не является неотрицательным Int32.");

            GalaxySummaryData selected = null;
            int selectedCount = 0;
            for (int candidate = GalaxyStarsOffset; candidate <= MainPayload.Length - 52; candidate++)
            {
                if (ReadInt32(MainPayload, candidate) != turn) continue;
                GalaxySummaryData value;
                if (!TryParseGalaxySummaryCandidate(candidate, turn, out value)) continue;
                selected = value;
                selectedCount++;
            }
            if (selectedCount != 1)
                throw new SavFormatException("Поздний блок TGalaxy: ожидался один структурный маршрут, найдено " + selectedCount + ".");
            LocateGalaxyReferenceLists(selected);
            ParsePlanetHeaders(selected);
            ParseShipHeaders(selected);
            ParseAsteroidHeaders(selected);
            ParsePlanetLateFields(selected);
            ParsePlanetEquipmentShop(selected);
            ParsePlanetGoneItems(selected);
            ParseAchievementStats(selected);
            ParseShipPreCommonCollections(selected);
            ParseHoleHeaders(selected);
            ParseStoredItems(selected);
            ParseStarTails(selected);
            ParseStarDropItems(selected);
            ParseStarSpaceItems(selected);
            // Discover exact owner-linked items first. The fallback structural
            // scan runs last so byte patterns inside ship/planet scalar tails
            // cannot displace an item reached through a serialized collection.
            ParseItemHeaders(selected);
            ParseStarSpaceShips();
            LocateGateRecords(selected);
            ParseScriptGlobalsAndCache(selected);
            LocatePlayerPlanetBattleFlag(selected);
            GalaxySummary = selected;
        }

        private void ParseAchievementStats(GalaxySummaryData summary)
        {
            ShipHeaderRecord player = null;
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.IsPlayer && ship.ObjectId == summary.PlayerObjectId)
                {
                    player = ship;
                    break;
                }
            if (player == null)
                throw new SavFormatException("TAchievementStats: не найден TPlayer.");

            int starEnd = summary.PlanetReferenceListOffset;
            foreach (StarHeaderRecord star in GalaxyStars)
                if (star.Start > player.Start)
                {
                    starEnd = star.Start;
                    break;
                }
            int playerEnd = starEnd - 1;
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.Type != 0 && ship.Start > player.Start && ship.Start < starEnd &&
                    ship.Start - 1 < playerEnd)
                    playerEnd = ship.Start - 1;

            AchievementStatsRecord selected = null;
            int selectedCount = 0;
            for (int marker = player.FixedPrefixEnd; marker <= playerEnd - 1848; marker++)
            {
                if (MainPayload[marker] != 10) continue;
                AchievementStatsRecord candidate;
                if (!TryReadAchievementStats(marker, playerEnd, out candidate)) continue;
                selected = candidate;
                selectedCount++;
            }
            if (selectedCount != 1)
                throw new SavFormatException("TAchievementStats: ожидался один структурный блок, найдено " + selectedCount + ".");
            AchievementStats = selected;
            player.HasPlayerJournal = true;
            player.PlayerJournalListOffset = selected.JournalListOffset;
            player.PlayerJournalEndOffset = selected.JournalEndOffset;
            player.PlayerJournalRecords = new List<PlayerJournalRecord>();
            foreach (PlayerJournalRecord record in selected.JournalRecords)
                player.PlayerJournalRecords.Add(record.Clone());
            player.HasPlayerNews = true;
            player.PlayerNewsListOffset = selected.PlayerNewsListOffset;
            player.PlayerNewsEndOffset = selected.PlayerNewsEndOffset;
            player.PlayerNewsRecords = new List<GalaxyNewsRecord>();
            foreach (GalaxyNewsRecord record in selected.PlayerNewsRecords)
                player.PlayerNewsRecords.Add(record.Clone());
            ParsePlayerFinancialTail(player, selected, summary.NextObjectId);
        }

        private bool TryReadAchievementStats(int marker, int playerEnd, out AchievementStatsRecord value)
        {
            value = null;
            try
            {
                int offset = marker;
                if (ReadByte(MainPayload, ref offset, "achievement block count") != 10) return false;
                for (int block = 0; block < 10; block++)
                {
                    if (ReadUInt16(MainPayload, ref offset, "achievement 12-value block") != 12) return false;
                    Skip(MainPayload, ref offset, 12 * 4, "achievement 12-value payload");
                    if (ReadUInt16(MainPayload, ref offset, "achievement 32-value block") != 32) return false;
                    Skip(MainPayload, ref offset, 32 * 4, "achievement 32-value payload");
                }

                int historicCount = ReadByte(MainPayload, ref offset, "achievement historic count");
                Skip(MainPayload, ref offset, checked(historicCount * 4), "achievement historic values");
                if (ReadByte(MainPayload, ref offset, "achievement legacy flag count") != 6) return false;
                for (int index = 0; index < 6; index++)
                    ReadBoolean(MainPayload, ref offset, "achievement legacy flag");

                int journalListOffset = offset;
                int infoCount = ReadInt32(MainPayload, ref offset, "journal record count");
                if (infoCount < 0 || infoCount > 10000) return false;
                List<PlayerJournalRecord> journalRecords = new List<PlayerJournalRecord>(infoCount);
                for (int index = 0; index < infoCount; index++)
                {
                    PlayerJournalRecord record = new PlayerJournalRecord();
                    record.Start = offset;
                    record.Turn = ReadInt32(MainPayload, ref offset, "journal record turn");
                    record.Text = ReadUtf16Z(MainPayload, ref offset, "journal record text");
                    if (record.Text.Length > 32768) return false;
                    record.End = offset;
                    journalRecords.Add(record);
                }
                int journalEndOffset = offset;

                int playerNewsListOffset = offset;
                int newsCount = ReadUInt16(MainPayload, ref offset, "player news count");
                if (newsCount > 10000) return false;
                List<GalaxyNewsRecord> playerNews = new List<GalaxyNewsRecord>(newsCount);
                for (int index = 0; index < newsCount; index++)
                {
                    GalaxyNewsRecord record = new GalaxyNewsRecord();
                    record.Start = offset;
                    record.Id = unchecked((uint)ReadInt32(MainPayload, ref offset, "player news id"));
                    record.Turn = unchecked((uint)ReadInt32(MainPayload, ref offset, "player news date"));
                    record.Type = ReadByte(MainPayload, ref offset, "player news type");
                    record.Text = ReadUtf16Z(MainPayload, ref offset, "player news text");
                    if (record.Text.Length > 32768) return false;
                    record.End = offset;
                    playerNews.Add(record);
                }
                int playerNewsEndOffset = offset;

                ReadByte(MainPayload, ref offset, "go to government");
                ReadBoolean(MainPayload, ref offset, "no jump");
                ReadBoolean(MainPayload, ref offset, "pirate clan real");
                if (offset > playerEnd - 41) return false;

                AchievementStatsRecord result = new AchievementStatsRecord();
                result.StructureStart = marker;
                result.Start = offset;
                result.AsteroidsDestroyed = ReadInt32(MainPayload, ref offset, "asteroids destroyed");
                result.FriedShips = ReadInt32(MainPayload, ref offset, "fried ships");
                result.DefendedSystem = ReadInt32(MainPayload, ref offset, "defended systems");
                result.PirateSystems = ReadInt32(MainPayload, ref offset, "pirate systems");
                result.ScienceProgress = ReadByte(MainPayload, ref offset, "science progress");
                result.ProgramsUsed = ReadInt32(MainPayload, ref offset, "programs used");
                result.PiratesFreed = ReadInt32(MainPayload, ref offset, "pirates freed");
                result.HealthDrained = ReadInt32(MainPayload, ref offset, "health drained");
                result.FuelGottenFromSun = ReadInt32(MainPayload, ref offset, "fuel gotten from sun");
                result.FuelTankLastId = ReadInt32(MainPayload, ref offset, "fuel tank last id");
                result.PlanetsVisited = ReadInt32(MainPayload, ref offset, "planets visited");
                result.End = offset;
                result.PlayerEnd = playerEnd;
                result.JournalListOffset = journalListOffset;
                result.JournalEndOffset = journalEndOffset;
                result.JournalRecords = journalRecords;
                result.PlayerNewsListOffset = playerNewsListOffset;
                result.PlayerNewsEndOffset = playerNewsEndOffset;
                result.PlayerNewsRecords = playerNews;
                int receivedStart;
                List<string> received;
                if (!TryReadReceivedAchievements(result.End, playerEnd, out receivedStart, out received)) return false;
                result.ReceivedListStart = receivedStart;
                result.Received = received;
                value = result;
                return true;
            }
            catch (SavFormatException) { return false; }
            catch (OverflowException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private void ParsePlayerFinancialTail(ShipHeaderRecord player,
            AchievementStatsRecord achievements, uint nextObjectId)
        {
            if (!player.HasPlayerPrefix)
                throw new SavFormatException("TPlayer: перед финансовым хвостом не найден фиксированный префикс.");
            int minimum = player.PlayerPrefixOffset + 50;
            int marker = achievements.StructureStart;
            int lateStats = marker - 24;
            if (lateStats < minimum || achievements.Start < 3 || achievements.End > achievements.PlayerEnd - 17)
                throw new SavFormatException("TPlayer: границы финансового/достиженческого хвоста повреждены.");

            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in GalaxyStars) starIds.Add(star.ObjectId);
            int selectedStart = -1, selectedProgramOffset = -1, selectedSatelliteOffset = -1;
            int selectedRobotOffset = -1, selectedRobotCount = -1;
            int selectedInfectionOffset = -1, selectedInfectionEndOffset = -1;
            string[] selectedInfectionPlaces = null;
            List<ItemHeaderRecord> selectedSatelliteItems = null;
            int matchCount = 0, bestCount = 0, bestScore = int.MinValue;
            List<string> matchedStarts = new List<string>();
            for (int candidate = minimum; candidate <= lateStats - 181; candidate++)
            {
                if (player.PlayerObjectStateCount == 0 && candidate != minimum) break;
                bool fixedScalarsValid = true;
                for (int index = 0; index < 6; index++)
                {
                    int scalar = ReadInt32(MainPayload, candidate + index * 4);
                    if (scalar < 0 || scalar > 1000000000) { fixedScalarsValid = false; break; }
                }
                for (int index = 0; index < 4 && fixedScalarsValid; index++)
                {
                    int scalar = ReadInt32(MainPayload, candidate + 28 + index * 4);
                    if (scalar < 0 || scalar > 1000000000) fixedScalarsValid = false;
                }
                if (!fixedScalarsValid) continue;
                float depositPercent = BitConverter.ToSingle(MainPayload, candidate + 24);
                if (float.IsNaN(depositPercent) || float.IsInfinity(depositPercent) ||
                    depositPercent < 0.0F || depositPercent > 1000.0F ||
                    depositPercent != 0.0F && depositPercent < 0.01F) continue;
                uint flyToStarId = ReadUInt32(MainPayload, candidate + 44);
                if (flyToStarId != 0 && !starIds.Contains(flyToStarId)) continue;
                for (int index = 0; index < 12 && fixedScalarsValid; index++)
                {
                    int scalar = ReadInt32(MainPayload, candidate + 48 + index * 4);
                    if (scalar < 0 || scalar > 1000000000) fixedScalarsValid = false;
                }
                if (!fixedScalarsValid) continue;

                int cursor = candidate + 96;
                int infectionOffset = cursor;
                string[] candidateInfectionPlaces = new string[24];
                bool stringsValid = true;
                for (int index = 0; index < 24; index++)
                    if (!TryReadObjectString(ref cursor, 4096, true,
                        out candidateInfectionPlaces[index]) || cursor > lateStats)
                    { stringsValid = false; break; }
                int infectionEndOffset = cursor;
                if (!stringsValid || cursor > lateStats - 61) continue;
                cursor++;
                int programOffset = cursor;
                for (int index = 0; index < 14 && stringsValid; index++)
                {
                    int scalar = ReadInt32(MainPayload, programOffset + index * 4);
                    if (scalar < 0 || scalar > 1000000000) stringsValid = false;
                }
                if (!stringsValid) continue;
                cursor += 12 * 4 + 8;
                if (cursor > lateStats - 4) continue;
                int satelliteCount = ReadInt32(MainPayload, cursor);
                if (satelliteCount < 0 || satelliteCount > 10000) continue;
                int satelliteOffset = cursor;
                cursor += 4;

                int robotMatches = 0, robotOffset = -1, robotCount = -1;
                int maximumRobots = Math.Min(10000, Math.Max(0, (lateStats - cursor - 4) / 40));
                for (int count = 0; count <= maximumRobots; count++)
                {
                    int offset = lateStats - 4 - count * 40;
                    if (offset < cursor) break;
                    if (ReadInt32(MainPayload, offset) != count) continue;
                    robotMatches++; robotOffset = offset; robotCount = count;
                }
                if (robotMatches != 1 || satelliteCount == 0 && robotOffset != cursor ||
                    satelliteCount > 0 && robotOffset <= cursor) continue;
                List<ItemHeaderRecord> candidateSatelliteItems;
                if (!TryReadPlayerSatelliteList(cursor, robotOffset, satelliteCount,
                    nextObjectId, out candidateSatelliteItems)) continue;
                if (MainPayload[lateStats + 8] > 1 || MainPayload[lateStats + 23] > 9 ||
                    MainPayload[achievements.Start - 2] > 1 || MainPayload[achievements.Start - 1] > 1)
                    continue;

                matchCount++;
                matchedStarts.Add("0x" + candidate.ToString("X") + ":" +
                    ReadInt32(MainPayload, candidate) + "/" + ReadInt32(MainPayload, candidate + 4) +
                    "/" + depositPercent.ToString("R") + "/" + flyToStarId);
                int score = depositPercent == 0.0F ? 0 : 100;
                for (int index = 0; index < 12; index++)
                    if (ReadInt32(MainPayload, candidate + index * 4) != 0) score++;
                for (int index = 0; index < 14; index++)
                    if (ReadInt32(MainPayload, programOffset + index * 4) != 0) score++;
                if (MainPayload[programOffset - 1] != 0) score++;
                if (score > bestScore)
                {
                    bestScore = score; bestCount = 1;
                    selectedStart = candidate;
                    selectedProgramOffset = programOffset;
                    selectedSatelliteOffset = satelliteOffset;
                    selectedRobotOffset = robotOffset;
                    selectedRobotCount = robotCount;
                    selectedInfectionOffset = infectionOffset;
                    selectedInfectionEndOffset = infectionEndOffset;
                    selectedInfectionPlaces = candidateInfectionPlaces;
                    selectedSatelliteItems = candidateSatelliteItems;
                }
                // Equal-score matches are shifts inside the already parsed fixed block;
                // the stream reader reaches the earliest structurally valid boundary first.
            }
            if (bestCount != 1)
                throw new SavFormatException("TPlayer: ожидался один финансовый маршрут, найдено " +
                    matchCount + ", лучших " + bestCount + " (" +
                    string.Join(", ", matchedStarts.ToArray()) + ").");

            ParsePlayerStorageItems(player, nextObjectId, selectedStart);

            player.HasPlayerFinancialTail = true;
            player.PlayerFinancialOffset = selectedStart;
            player.PlayerDebt = ReadInt32(MainPayload, selectedStart);
            player.PlayerDebtDate = ReadInt32(MainPayload, selectedStart + 4);
            player.PlayerDebtCount = ReadInt32(MainPayload, selectedStart + 8);
            player.PlayerDeposit = ReadInt32(MainPayload, selectedStart + 12);
            player.PlayerDepositDate = ReadInt32(MainPayload, selectedStart + 16);
            player.PlayerDepositDay = ReadInt32(MainPayload, selectedStart + 20);
            player.PlayerDepositPercent = BitConverter.ToSingle(MainPayload, selectedStart + 24);
            player.PlayerMedPolicy = ReadInt32(MainPayload, selectedStart + 28);
            player.PlayerPirateLicense = ReadInt32(MainPayload, selectedStart + 32);
            player.PlayerPiratePoints = ReadInt32(MainPayload, selectedStart + 36);
            player.PlayerPirateNewPoints = ReadInt32(MainPayload, selectedStart + 40);
            player.PlayerFlyToStarId = ReadUInt32(MainPayload, selectedStart + 44);
            for (int index = 0; index < 12; index++)
                player.PlayerInvestments[index] = ReadInt32(MainPayload, selectedStart + 48 + index * 4);
            if (selectedInfectionPlaces == null || selectedInfectionPlaces.Length != 24)
                throw new SavFormatException("TPlayer: строки мест заражения не выбраны финансовым маршрутом.");
            player.PlayerInfectionPlacesOffset = selectedInfectionOffset;
            player.PlayerInfectionPlacesEndOffset = selectedInfectionEndOffset;
            player.PlayerInfectionPlaces = selectedInfectionPlaces;
            player.PlayerImmunity = MainPayload[selectedProgramOffset - 1];
            player.PlayerProgramsOffset = selectedProgramOffset;
            for (int index = 0; index < 12; index++)
                player.PlayerProgramsInWarBase[index] = ReadInt32(MainPayload,
                    selectedProgramOffset + index * 4);
            player.PlayerDayWarBaseGivePrograms = ReadInt32(MainPayload, selectedProgramOffset + 48);
            player.PlayerHitEnemyAfterPrograms = ReadInt32(MainPayload, selectedProgramOffset + 52);
            player.PlayerSatelliteCount = ReadInt32(MainPayload, selectedSatelliteOffset);
            player.PlayerSatelliteListOffset = selectedSatelliteOffset;
            player.PlayerSatelliteEndOffset = selectedRobotOffset;
            player.PlayerSatelliteItems = new List<ShipItemListEntry>(player.PlayerSatelliteCount);
            int satelliteCursor = selectedSatelliteOffset + 4;
            if (selectedSatelliteItems == null ||
                selectedSatelliteItems.Count != player.PlayerSatelliteCount)
                throw new SavFormatException("TPlayer: список TSatellite не выбран финансовым маршрутом.");
            for (int index = 0; index < selectedSatelliteItems.Count; index++)
            {
                ItemHeaderRecord item = selectedSatelliteItems[index];
                if (item.Start != satelliteCursor)
                    throw new SavFormatException("TPlayer: разрыв перед вложенным TSatellite " + index + ".");
                int end = SerializedItemEnd(item);
                if (end <= satelliteCursor || end > selectedRobotOffset)
                    throw new SavFormatException("TPlayer: граница вложенного TSatellite повреждена.");
                foreach (ItemHeaderRecord existing in GalaxyItems)
                    if (existing.Type == item.Type && existing.ObjectId == item.ObjectId)
                        throw new SavFormatException("TPlayer: повторный TSatellite type/id " +
                            item.Type + "/" + item.ObjectId + ".");
                GalaxyItems.Add(item);
                ShipItemListEntry entry = new ShipItemListEntry();
                entry.Start = satelliteCursor;
                entry.End = end;
                entry.ItemType = item.Type;
                entry.ItemStart = item.Start;
                entry.ItemObjectId = item.ObjectId;
                player.PlayerSatelliteItems.Add(entry);
                satelliteCursor = end;
            }
            if (satelliteCursor != selectedRobotOffset)
                throw new SavFormatException("TPlayer: список TSatellite не завершился у списка карт роботов.");
            player.PlayerRobotMapCount = selectedRobotCount;
            player.HasPlayerRobotMaps = true;
            player.PlayerRobotMapListOffset = selectedRobotOffset;
            player.PlayerRobotMapEndOffset = lateStats;
            player.PlayerRobotMaps = new List<PlayerRobotMapRecord>(selectedRobotCount);
            for (int index = 0; index < selectedRobotCount; index++)
            {
                int start = selectedRobotOffset + 4 + index * 40;
                PlayerRobotMapRecord record = new PlayerRobotMapRecord();
                record.Start = start;
                record.Id = ReadInt32(MainPayload, start);
                record.Time = ReadInt32(MainPayload, start + 4);
                record.BuildRobot = ReadInt32(MainPayload, start + 8);
                record.KillRobot = ReadInt32(MainPayload, start + 12);
                record.BuildTurret = ReadInt32(MainPayload, start + 16);
                record.KillTurret = ReadInt32(MainPayload, start + 20);
                record.KillBuilding = ReadInt32(MainPayload, start + 24);
                record.Bonus = ReadInt32(MainPayload, start + 28);
                record.State = ReadInt32(MainPayload, start + 32);
                record.Turn = ReadInt32(MainPayload, start + 36);
                record.End = start + 40;
                player.PlayerRobotMaps.Add(record);
            }
            player.PlayerLateStatsOffset = lateStats;
            player.PlayerPlanetBattlesWin = ReadInt32(MainPayload, lateStats);
            player.PlayerLastPlanetBattleDate = ReadInt32(MainPayload, lateStats + 4);
            player.PlayerPlanetBattlesRejected = MainPayload[lateStats + 8] != 0;
            player.PlayerIllnessCount = BitConverter.ToUInt16(MainPayload, lateStats + 9);
            player.PlayerStimulatorCount = BitConverter.ToUInt16(MainPayload, lateStats + 11);
            player.PlayerPrisonCount = BitConverter.ToUInt16(MainPayload, lateStats + 13);
            player.PlayerUnknownPlanetComplete = ReadInt32(MainPayload, lateStats + 15);
            player.PlayerChangeRaceCount = BitConverter.ToUInt16(MainPayload, lateStats + 19);
            player.PlayerChangeSideCount = BitConverter.ToUInt16(MainPayload, lateStats + 21);
            player.PlayerHotEquipmentCurrent = MainPayload[lateStats + 23];
            int equipmentSetCursor = marker;
            player.PlayerEquipmentSetsOffset = marker;
            player.PlayerEquipmentSetCount = MainPayload[equipmentSetCursor++];
            if (player.PlayerEquipmentSetCount != 10)
                throw new SavFormatException("TPlayer: ожидалось 10 комплектов оборудования.");
            player.PlayerEquipmentSetItems = new uint[10, 12];
            player.PlayerArtefactSetItems = new uint[10, 32];
            for (int set = 0; set < 10; set++)
            {
                if (BitConverter.ToUInt16(MainPayload, equipmentSetCursor) != 12)
                    throw new SavFormatException("TPlayer: неверное число слотов оборудования в комплекте.");
                equipmentSetCursor += 2;
                for (int slot = 0; slot < 12; slot++)
                {
                    player.PlayerEquipmentSetItems[set, slot] = ReadUInt32(MainPayload, equipmentSetCursor);
                    equipmentSetCursor += 4;
                }
                if (BitConverter.ToUInt16(MainPayload, equipmentSetCursor) != 32)
                    throw new SavFormatException("TPlayer: неверное число слотов артефактов в комплекте.");
                equipmentSetCursor += 2;
                for (int slot = 0; slot < 32; slot++)
                {
                    player.PlayerArtefactSetItems[set, slot] = ReadUInt32(MainPayload, equipmentSetCursor);
                    equipmentSetCursor += 4;
                }
            }
            player.PlayerEquipmentSetsEndOffset = equipmentSetCursor;
            player.PlayerPreAchievementFlagsOffset = achievements.Start - 3;
            player.PlayerGoToGovernment = MainPayload[achievements.Start - 3];
            player.PlayerNoJump = MainPayload[achievements.Start - 2] != 0;
            player.PlayerPirateClanReal = MainPayload[achievements.Start - 1] != 0;
            player.PlayerExperienceOffset = achievements.End;
            player.PlayerExperienceDominatorKills = ReadInt32(MainPayload, achievements.End);
            player.PlayerExperiencePirateKills = ReadInt32(MainPayload, achievements.End + 4);
            player.PlayerExperienceGoodShipKills = ReadInt32(MainPayload, achievements.End + 8);
            player.PlayerExperienceTrade = ReadInt32(MainPayload, achievements.End + 12);
            player.PlayerCaptainOnBridge = MainPayload[achievements.End + 16];
            if (!TryReadPlayerBridge(player, achievements, nextObjectId))
                throw new SavFormatException("TPlayer.Bridge: вложенный TRuins и ссылки не распознаны.");
        }

        private void ParsePlayerStorageItems(ShipHeaderRecord player, uint nextObjectId,
            int financialStart)
        {
            int countOffset = player.PlayerPrefixOffset + 46;
            int cursor = countOffset + 4;
            int count = player.PlayerObjectStateCount;
            if (count < 0 || count > 10000 || cursor > financialStart)
                throw new SavFormatException("TPlayer.StorageItems: неверный счётчик или граница списка.");

            HashSet<uint> planetIds = new HashSet<uint>();
            foreach (PlanetHeaderRecord planet in GalaxyPlanets) planetIds.Add(planet.ObjectId);
            HashSet<uint> stationIds = new HashSet<uint>();
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.IsStation) stationIds.Add(ship.ObjectId);
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;

            List<PlayerStorageItemRecord> records = new List<PlayerStorageItemRecord>(count);
            for (int index = 0; index < count; index++)
            {
                if (cursor > financialStart - 10)
                    throw new SavFormatException("TPlayer.StorageItems: запись " + index + " обрезана.");
                PlayerStorageItemRecord record = new PlayerStorageItemRecord();
                record.Start = cursor;
                byte placeType = MainPayload[cursor++];
                if (placeType > 1)
                    throw new SavFormatException("TPlayer.StorageItems: неизвестный тип места " + placeType + ".");
                record.IsStation = placeType != 0;
                record.PlaceObjectId = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (record.IsStation ? !stationIds.Contains(record.PlaceObjectId) :
                    !planetIds.Contains(record.PlaceObjectId))
                    throw new SavFormatException("TPlayer.StorageItems: ссылка места " +
                        record.PlaceObjectId + " не разрешается.");
                record.Slot = ReadInt32(MainPayload, cursor); cursor += 4;
                record.ItemType = MainPayload[cursor++];
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= financialStart)
                        throw new SavFormatException(
                            "TPlayer.StorageItems: повреждено имя TCustomWeapon.");
                }
                record.ItemStart = cursor;
                ItemHeaderRecord parsed;
                if (!TryReadItemHeader(cursor, nextObjectId, out parsed, true) ||
                    parsed.Type != record.ItemType ||
                    parsed.Type >= 8 && !TryReadKnownItemDerivedTail(parsed, parsed.SharedPrefixEnd))
                    throw new SavFormatException("TPlayer.StorageItems: вложенный TItem " + index +
                        " не разобран @ 0x" + cursor.ToString("X") + ".");
                record.ItemObjectId = parsed.ObjectId;
                record.End = SerializedItemEnd(parsed);
                if (record.End <= record.ItemStart || record.End > financialStart)
                    throw new SavFormatException("TPlayer.StorageItems: неверная граница вложенного TItem.");
                ItemHeaderRecord existing;
                if (!itemsByStart.TryGetValue(record.ItemStart, out existing))
                {
                    foreach (ItemHeaderRecord known in GalaxyItems)
                        if (known.Type == parsed.Type && known.ObjectId == parsed.ObjectId)
                            throw new SavFormatException("TPlayer.StorageItems: TItem " +
                                parsed.Type + "/" + parsed.ObjectId +
                                " уже имеет другой структурный адрес.");
                    GalaxyItems.Add(parsed);
                    itemsByStart.Add(parsed.Start, parsed);
                }
                else if (existing.Type != record.ItemType ||
                    existing.ObjectId != record.ItemObjectId)
                    throw new SavFormatException(
                        "TPlayer.StorageItems: точный TItem конфликтует с каталогом.");
                cursor = record.End;
                records.Add(record);
            }
            if (cursor != financialStart)
                throw new SavFormatException("TPlayer.StorageItems: список закончился в 0x" +
                    cursor.ToString("X") + ", финансовый хвост начинается в 0x" +
                    financialStart.ToString("X") + ".");

            player.HasPlayerStorageItems = true;
            player.PlayerStorageItemCountOffset = countOffset;
            player.PlayerStorageItemsEndOffset = financialStart;
            player.PlayerStorageItems = records;
        }

        private bool TryReadPlayerBridge(ShipHeaderRecord player,
            AchievementStatsRecord achievements, uint nextObjectId)
        {
            int start = achievements.End + 17;
            bool zeroReceivedList = achievements.ReceivedListStart < 0;
            int limit = zeroReceivedList ? achievements.PlayerEnd : achievements.ReceivedListStart;
            if (start < 0 || limit <= start || limit > achievements.PlayerEnd) return false;

            uint objectId = ReadUInt32(MainPayload, start);
            if (objectId == 0 || objectId >= nextObjectId) return false;
            int offset = start + 4;
            string name, scriptName;
            if (!TryReadObjectString(ref offset, 80, true, out name)) return false;
            int nameEnd = offset;
            if (!TryReadObjectString(ref offset, 128, true, out scriptName)) return false;
            int scriptNameEnd = offset;
            if (offset > limit - 177) return false;
            byte type = MainPayload[offset];
            byte owner = MainPayload[offset + 1];
            if (type < 6 || type > 13 || owner > 7) return false;
            float x = BitConverter.ToSingle(MainPayload, offset + 2);
            float y = BitConverter.ToSingle(MainPayload, offset + 6);
            if (!IsSupportedObjectCoordinate(x) || !IsSupportedObjectCoordinate(y)) return false;
            int itemCountOffset = offset + 175;
            int equipmentCount = BitConverter.ToUInt16(MainPayload, itemCountOffset);
            if (MainPayload[offset + 174] > 4 || equipmentCount > 128) return false;

            ShipHeaderRecord bridge = new ShipHeaderRecord();
            bridge.Start = start; bridge.NameEnd = nameEnd; bridge.ScriptNameEnd = scriptNameEnd;
            bridge.FixedPrefixEnd = itemCountOffset + 2; bridge.ObjectId = objectId;
            bridge.Type = type; bridge.Owner = owner; bridge.Name = name; bridge.ScriptName = scriptName;
            bridge.X = x; bridge.Y = y;
            bridge.HomePlanetId = ReadUInt32(MainPayload, offset + 10);
            bridge.CurrentStarId = ReadUInt32(MainPayload, offset + 14);
            bridge.CurrentPlanetId = ReadUInt32(MainPayload, offset + 18);
            bridge.CurrentShipId = ReadUInt32(MainPayload, offset + 22);
            int goodsOffset = offset + 26;
            for (int good = 0; good < 8; good++)
                for (int field = 0; field < 4; field++)
                    bridge.Goods[good, field] = ReadUInt32(MainPayload,
                        goodsOffset + good * 16 + field * 4);
            bridge.Money = ReadUInt32(MainPayload, offset + 154);
            bridge.Rnd = ReadUInt32(MainPayload, offset + 158);
            bridge.RndOut = ReadUInt32(MainPayload, offset + 162);
            bridge.Day = ReadUInt32(MainPayload, offset + 166);
            bridge.Face = ReadInt32(MainPayload, offset + 170);
            bridge.PilotRace = MainPayload[offset + 174];
            bridge.EquipmentItemCount = checked((ushort)equipmentCount);

            ShipHeaderRecord parsed = null;
            for (int graphStart = bridge.FixedPrefixEnd + 30; graphStart <= limit - 520; graphStart++)
            {
                if (MainPayload[graphStart + 1] != 0 || MainPayload[graphStart] < 0x20 ||
                    MainPayload[graphStart] > 0x7E) continue;
                ShipHeaderRecord candidate = bridge.Clone();
                if (!TryReadShipCommonTail(candidate, graphStart, limit) ||
                    !IsPreferredShipGraphName(candidate.GraphName) ||
                    !TryScanShipPreCommonPrefix(candidate, nextObjectId) ||
                    !TryReadRuinsShipTail(candidate, limit, nextObjectId)) continue;
                parsed = candidate;
                break;
            }
            if (parsed == null) return false;

            int cursor = parsed.RuinsFinalFlagsOffset + 4;
            int referenceOffset = -1;
            uint currentShipId = 0, currentPlanetId = 0;
            if (player.PlayerCaptainOnBridge != 0)
            {
                if (cursor > limit - 10) return false;
                referenceOffset = cursor;
                currentShipId = ReadUInt32(MainPayload, cursor); cursor += 4;
                currentPlanetId = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (currentShipId > 10000000 || currentPlanetId > 10000000) return false;
            }
            int backgroundOffset = cursor;
            string background;
            if (!TryReadObjectString(ref cursor, 512, true, out background)) return false;
            int backgroundEnd = cursor;
            if (zeroReceivedList)
            {
                if (cursor != achievements.PlayerEnd - 4 || ReadInt32(MainPayload, cursor) != 0)
                    return false;
                achievements.ReceivedListStart = cursor;
            }
            else if (cursor != limit) return false;

            player.HasPlayerBridge = true;
            player.PlayerBridgeRuins = parsed;
            player.PlayerBridgeRuinsEndOffset = parsed.RuinsFinalFlagsOffset + 4;
            player.PlayerBridgeReferenceOffset = referenceOffset;
            player.PlayerBridgeCurrentShipId = currentShipId;
            player.PlayerBridgeCurrentPlanetId = currentPlanetId;
            player.PlayerBridgeBackgroundOffset = backgroundOffset;
            player.PlayerBridgeBackgroundEndOffset = backgroundEnd;
            player.PlayerBridgeBackground = background;
            return true;
        }

        private bool TryReadPlayerSatelliteList(int start, int end, int count,
            uint nextObjectId, out List<ItemHeaderRecord> items)
        {
            items = new List<ItemHeaderRecord>();
            if (count < 0 || count > 10000 || start < 0 || end < start ||
                end > MainPayload.Length || count > (end - start) / 31) return false;
            int cursor = start;
            HashSet<uint> objectIds = new HashSet<uint>();
            for (int index = 0; index < count; index++)
            {
                ItemHeaderRecord item;
                if (!TryReadItemHeader(cursor, nextObjectId, out item, true, true) ||
                    item.Type != 73 || !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd) ||
                    !objectIds.Add(item.ObjectId)) return false;
                int itemEnd = SerializedItemEnd(item);
                if (itemEnd <= cursor || itemEnd > end) return false;
                items.Add(item);
                cursor = itemEnd;
            }
            return cursor == end;
        }

        private bool TryReadReceivedAchievements(int minimum, int end, out int selectedStart,
            out List<string> selected)
        {
            selectedStart = -1;
            selected = null;
            int matches = 0;
            for (int start = minimum; start <= end - 4; start++)
            {
                int count = ReadInt32(MainPayload, start);
                if (count <= 0 || count > 256) continue;
                int offset = start + 4;
                List<string> values = new List<string>(count);
                bool valid = true;
                for (int index = 0; index < count; index++)
                {
                    string text;
                    if (!TryReadObjectString(ref offset, 256, false, out text))
                    {
                        valid = false;
                        break;
                    }
                    if (!IsAchievementKey(text))
                    {
                        valid = false;
                        break;
                    }
                    values.Add(text);
                }
                if (!valid || offset > end) continue;
                selectedStart = start;
                selected = values;
                matches++;
            }
            if (matches == 0)
            {
                selectedStart = -1;
                selected = new List<string>();
                return true;
            }
            return matches == 1;
        }

        private static bool IsAchievementKey(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 64) return false;
            foreach (char character in value)
                if (!((character >= 'A' && character <= 'Z') ||
                    (character >= '0' && character <= '9') || character == '_'))
                    return false;
            return true;
        }

        private void ParsePlanetHeaders(GalaxySummaryData summary)
        {
            int expectedTotal = 0;
            foreach (StarHeaderRecord star in GalaxyStars)
                expectedTotal = checked(expectedTotal + star.PlanetCount);
            HashSet<uint> referencedIds = new HashSet<uint>();
            for (int index = 0; index < expectedTotal; index++)
                if (!referencedIds.Add(ReadUInt32(MainPayload,
                    summary.PlanetReferenceListOffset + 2 + index * 4)))
                    throw new SavFormatException("TGalaxy.Planets: повторная объектная ссылка.");

            HashSet<uint> parsedIds = new HashSet<uint>();
            for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
            {
                StarHeaderRecord star = GalaxyStars[starIndex];
                int limit = starIndex + 1 < GalaxyStars.Count
                    ? GalaxyStars[starIndex + 1].Start : summary.PlanetReferenceListOffset;
                int foundForStar = 0;
                for (int start = star.HeaderEnd; start <= limit - 250; start++)
                {
                    uint objectId = ReadUInt32(MainPayload, start);
                    if (!referencedIds.Contains(objectId)) continue;
                    PlanetHeaderRecord planet;
                    if (!TryReadPlanetHeader(start, limit, out planet)) continue;
                    if (!parsedIds.Add(objectId))
                        throw new SavFormatException("TPlanet: повторно найден object id " + objectId + ".");
                    GalaxyPlanets.Add(planet);
                    foundForStar++;
                }
                if (foundForStar != star.PlanetCount)
                    throw new SavFormatException("TStar " + star.ObjectId + ": ожидалось TPlanet " +
                        star.PlanetCount + ", найдено " + foundForStar + ".");
            }
            if (parsedIds.Count != referencedIds.Count)
                throw new SavFormatException("TPlanet: структурные заголовки не совпали со списком ссылок TGalaxy.");
        }

        private bool TryReadPlanetHeader(int start, int limit, out PlanetHeaderRecord value)
        {
            value = null;
            if (start < 0 || start > limit - 250) return false;
            int offset = start + 12;
            string name;
            if (!TryReadPlanetString(ref offset, 80, out name) || name.Length < 2) return false;
            if (offset > limit - 231) return false;
            float polarAngle = BitConverter.ToSingle(MainPayload, offset);
            float polarRadius = BitConverter.ToSingle(MainPayload, offset + 4);
            float angle = BitConverter.ToSingle(MainPayload, offset + 8);
            if (!IsSupportedObjectCoordinate(polarAngle) || !IsSupportedObjectCoordinate(polarRadius) ||
                !IsSupportedObjectCoordinate(angle)) return false;
            if (polarAngle == 0 && polarRadius == 0 && angle == 0) return false;
            if (MainPayload[offset + 45] > 1) return false;
            for (int index = 0; index < 20; index++)
                if (MainPayload[offset + 46 + index] > 16) return false;
            float scalar = BitConverter.ToSingle(MainPayload, offset + 67);
            if (float.IsNaN(scalar) || float.IsInfinity(scalar)) return false;
            for (int index = 0; index < 8; index++)
            {
                float loopScalar = BitConverter.ToSingle(MainPayload, offset + 89 + index * 18);
                if (float.IsNaN(loopScalar) || float.IsInfinity(loopScalar)) return false;
            }
            int firstListCount = MainPayload[offset + 229] | MainPayload[offset + 230] << 8;
            int relationCountOffset = offset + 229;
            int relationEnd;
            try { relationEnd = checked(relationCountOffset + 2 + firstListCount); }
            catch (OverflowException) { return false; }
            if (firstListCount > 10000 || relationEnd > limit) return false;

            PlanetHeaderRecord planet = new PlanetHeaderRecord();
            planet.Start = start;
            planet.ScalarOffset = offset;
            planet.FixedPrefixEnd = offset + 231;
            planet.ObjectId = ReadUInt32(MainPayload, start);
            planet.Raw08 = ReadInt32(MainPayload, start + 4);
            planet.Raw0C = ReadUInt32(MainPayload, start + 8);
            planet.Name = name;
            planet.PolarAngle = polarAngle;
            planet.PolarRadius = polarRadius;
            planet.Angle = angle;
            planet.Mass = ReadInt32(MainPayload, offset + 12);
            planet.Radius = ReadInt32(MainPayload, offset + 16);
            planet.WaterSpace = ReadInt32(MainPayload, offset + 20);
            planet.WaterSpaceDone = ReadInt32(MainPayload, offset + 24);
            planet.LandSpace = ReadInt32(MainPayload, offset + 28);
            planet.LandSpaceDone = ReadInt32(MainPayload, offset + 32);
            planet.HillSpace = ReadInt32(MainPayload, offset + 36);
            planet.HillSpaceDone = ReadInt32(MainPayload, offset + 40);
            planet.OrbitCount = MainPayload[offset + 44];
            planet.VisitedByPlayer = MainPayload[offset + 45] != 0;
            planet.OpenInventions = new byte[20];
            Buffer.BlockCopy(MainPayload, offset + 46, planet.OpenInventions, 0, 20);
            planet.CurrentInvention = MainPayload[offset + 66];
            planet.OpenPointsInvention = scalar;
            planet.NecessaryPercent = MainPayload[offset + 71];
            planet.NecessaryPercentK = MainPayload[offset + 72];
            planet.PeopleCount = ReadUInt32(MainPayload, offset + 73);
            planet.Economy = MainPayload[offset + 77];
            planet.Money = ReadUInt32(MainPayload, offset + 78);
            planet.Owner = MainPayload[offset + 82];
            planet.Race = MainPayload[offset + 83];
            planet.Government = MainPayload[offset + 84];
            planet.ShopGoods = new uint[8, 3];
            planet.ShopDeficit = new byte[8];
            planet.ShopSale = new byte[8];
            for (int index = 0; index < 8; index++)
            {
                int row = offset + 85 + index * 18;
                planet.ShopGoods[index, 0] = ReadUInt32(MainPayload, row);
                planet.ShopGoods[index, 1] = ReadUInt32(MainPayload, row + 8);
                planet.ShopGoods[index, 2] = ReadUInt32(MainPayload, row + 12);
                planet.ShopDeficit[index] = MainPayload[row + 16];
                planet.ShopSale[index] = MainPayload[row + 17];
            }
            planet.RelationCountOffset = relationCountOffset;
            planet.RelationEndOffset = relationEnd;
            planet.RelationCount = checked((ushort)firstListCount);
            planet.RelationToRangers = new byte[firstListCount];
            if (firstListCount > 0)
                Buffer.BlockCopy(MainPayload, relationCountOffset + 2,
                    planet.RelationToRangers, 0, firstListCount);
            planet.FirstListCount = checked((ushort)firstListCount);
            value = planet;
            return true;
        }

        private bool TryReadPlanetString(ref int offset, int maximumLength, out string value)
        {
            value = null;
            StringBuilder text = new StringBuilder();
            for (int index = 0; index <= maximumLength; index++)
            {
                if (offset < 0 || offset > MainPayload.Length - 2) return false;
                int codeUnit = MainPayload[offset] | MainPayload[offset + 1] << 8;
                offset += 2;
                if (codeUnit == 0)
                {
                    value = text.ToString();
                    return true;
                }
                char character = (char)codeUnit;
                if (!((character >= ' ' && character <= '~') ||
                    (character >= '\u0410' && character <= '\u044F') ||
                    character == '\u0401' || character == '\u0451')) return false;
                text.Append(character);
            }
            return false;
        }

        private void ParseAsteroidHeaders(GalaxySummaryData summary)
        {
            Dictionary<uint, AsteroidRecord> asteroids = new Dictionary<uint, AsteroidRecord>();
            for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
            {
                StarHeaderRecord star = GalaxyStars[starIndex];
                int starLimit = starIndex + 1 < GalaxyStars.Count
                    ? GalaxyStars[starIndex + 1].Start : summary.PlanetReferenceListOffset;
                List<ShipHeaderRecord> containedShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in GalaxyShips)
                    if (ship.Start > star.Start && ship.Start < starLimit)
                        containedShips.Add(ship);
                containedShips.Sort(delegate(ShipHeaderRecord left, ShipHeaderRecord right)
                    { return left.Start.CompareTo(right.Start); });

                int shipCountOffset = -1;
                int asteroidCountOffset = -1;
                List<AsteroidRecord> selected = null;
                int shipSectionMatches = 0;
                for (int index = 0; index < containedShips.Count; index++)
                {
                    int candidate = containedShips[index].Start - 3;
                    if (candidate < star.HeaderEnd || BitConverter.ToUInt16(MainPayload, candidate) != containedShips.Count - index)
                        continue;
                    List<AsteroidRecord> route;
                    int routeCountOffset;
                    if (!TryLocateAsteroidSection(star, candidate, summary.NextObjectId, out route,
                        out routeCountOffset)) continue;
                    shipCountOffset = candidate;
                    asteroidCountOffset = routeCountOffset;
                    selected = route;
                    shipSectionMatches++;
                }
                // A completely empty TStar has no object after the zero ship count from which
                // its boundary can be inferred without parsing the complete TPlanet tail.
                // Such a star cannot contribute a structurally proven non-empty asteroid route.
                if (shipSectionMatches == 0) continue;
                if (shipSectionMatches != 1)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": найдено несколько границ секции ships для TAsteroid.");
                star.AsteroidCountOffset = asteroidCountOffset;
                star.SpaceShipCountOffset = shipCountOffset;
                foreach (AsteroidRecord asteroid in selected)
                {
                    AsteroidRecord previous;
                    if (asteroids.TryGetValue(asteroid.ObjectId, out previous))
                        throw new SavFormatException("TAsteroid: object id " + asteroid.ObjectId +
                            " повторяется в звёздах " + previous.ParentStarId + " и " + asteroid.ParentStarId + ".");
                    asteroids.Add(asteroid.ObjectId, asteroid);
                    GalaxyAsteroids.Add(asteroid);
                }
            }
        }

        private void ParsePlanetLateFields(GalaxySummaryData summary)
        {
            for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
            {
                StarHeaderRecord star = GalaxyStars[starIndex];
                int starLimit = starIndex + 1 < GalaxyStars.Count
                    ? GalaxyStars[starIndex + 1].Start : summary.PlanetReferenceListOffset;
                List<PlanetHeaderRecord> planets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord planet in GalaxyPlanets)
                    if (planet.Start > star.Start && planet.Start < starLimit) planets.Add(planet);
                planets.Sort(delegate(PlanetHeaderRecord left, PlanetHeaderRecord right)
                    { return left.Start.CompareTo(right.Start); });
                for (int index = 0; index < planets.Count; index++)
                {
                    int exactEnd = index + 1 < planets.Count ? planets[index + 1].Start : -1;
                    int searchEnd = exactEnd > planets[index].FixedPrefixEnd ? exactEnd : starLimit;
                    if (searchEnd <= planets[index].FixedPrefixEnd) continue;
                    if (!TryLocatePlanetLateFields(planets[index], searchEnd)) continue;
                    if (exactEnd > planets[index].FixedPrefixEnd)
                    {
                        if (!TryLocatePlanetEndFields(planets[index], exactEnd) ||
                            !TryReadPlanetSputniks(planets[index]))
                            throw new SavFormatException("TPlanet " + planets[index].ObjectId +
                                ": список TSputnik не совпал с точной границей планеты (count=0x" +
                                planets[index].SatelliteCountOffset.ToString("X") + ", flags=0x" +
                                planets[index].FlagsOffset.ToString("X") + ", value=" +
                                planets[index].SatelliteCount + ").");
                        planets[index].End = exactEnd;
                    }
                    else
                    {
                        int emptyEnd;
                        if (TryLocateLastPlanetEnd(planets[index], starLimit,
                            summary.NextObjectId, out emptyEnd))
                            planets[index].End = emptyEnd;
                    }
                }
            }
        }

        private bool TryLocateLastPlanetEnd(PlanetHeaderRecord planet, int starLimit,
            uint nextObjectId, out int selectedEnd)
        {
            selectedEnd = -1;
            if (!TryReadPlanetSputniks(planet, starLimit)) return false;
            int goneItemEnd;
            if (!TryReadPlanetGoneItemsStandalone(planet, nextObjectId, starLimit, out goneItemEnd) ||
                goneItemEnd > starLimit - 5 || MainPayload[goneItemEnd] > 1 ||
                MainPayload[goneItemEnd + 1] > 15 || MainPayload[goneItemEnd + 2] > 1) return false;
            int end = goneItemEnd + 3;
            string customFaction;
            if (!TryReadObjectString(ref end, 128, true, out customFaction) || end > starLimit ||
                !TryLocatePlanetEndFields(planet, end) || planet.FlagsOffset != goneItemEnd) return false;
            selectedEnd = end;
            return true;
        }

        private bool TryReadPlanetGoneItemsStandalone(PlanetHeaderRecord planet, uint nextObjectId,
            int limit, out int selectedEnd)
        {
            selectedEnd = -1;
            if (planet == null || planet.SatelliteEndOffset <= 0 ||
                planet.SatelliteEndOffset > limit - 2) return false;
            int cursor = planet.SatelliteEndOffset;
            int count = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
            if (count > 10000) return false;
            List<PlanetGoneItemRecord> records = new List<PlanetGoneItemRecord>(count);
            for (int index = 0; index < count; index++)
            {
                if (cursor > limit - 9) return false;
                PlanetGoneItemRecord record = new PlanetGoneItemRecord();
                record.Start = cursor;
                record.PosX = MainPayload[cursor++];
                record.PosY = MainPayload[cursor++];
                record.LandType = MainPayload[cursor++];
                record.Region = ReadInt32(MainPayload, cursor); cursor += 4;
                byte miss = MainPayload[cursor++];
                if (miss > 1) return false;
                record.Miss = miss != 0;
                record.FactoryDiscriminatorOffset = cursor;
                record.ItemType = MainPayload[cursor++];
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= limit) return false;
                }
                record.ItemStart = cursor;
                ItemHeaderRecord item;
                if (!TryReadItemHeader(record.ItemStart, nextObjectId, out item, true) ||
                    item.Type != record.ItemType || item.Type >= 8 &&
                        !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd)) return false;
                if (record.ItemType == 68 &&
                    item.CustomWeaponDiscriminatorOffset != record.FactoryDiscriminatorOffset) return false;
                record.ItemObjectId = item.ObjectId;
                record.End = SerializedItemEnd(item);
                if (record.End <= record.ItemStart || record.End > limit) return false;
                records.Add(record);
                cursor = record.End;
            }
            planet.GoneItemCountOffset = planet.SatelliteEndOffset;
            planet.GoneItemEndOffset = cursor;
            planet.GoneItemCount = checked((ushort)count);
            planet.GoneItems = records;
            selectedEnd = cursor;
            return true;
        }

        private bool TryLocatePlanetLateFields(PlanetHeaderRecord planet, int end)
        {
            int selectedOffset = -1;
            int selectedNameEnd = -1;
            int selectedSatelliteOffset = -1;
            string selectedName = string.Empty;
            int matches = 0;
            for (int candidate = planet.RelationEndOffset + 10; candidate <= end - 15; candidate++)
            {
                ushort graphRadius = BitConverter.ToUInt16(MainPayload, candidate);
                if (graphRadius > 10000) continue;
                int cursor = candidate + 2;
                string graphName;
                if (!TryReadObjectString(ref cursor, 128, false, out graphName) || cursor > end - 13) continue;
                if (!graphName.StartsWith("Planet.", StringComparison.OrdinalIgnoreCase) &&
                    !graphName.StartsWith("PRuins.", StringComparison.OrdinalIgnoreCase)) continue;
                ushort graphSpeed = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
                int graphStep = ReadInt32(MainPayload, cursor); cursor += 4;
                byte graphRing = MainPayload[cursor++];
                int questNumber = ReadInt32(MainPayload, cursor); cursor += 4;
                ushort satelliteCount = BitConverter.ToUInt16(MainPayload, cursor);
                if (graphSpeed > 10000 || graphStep < -100000000 || graphStep > 100000000 ||
                    graphRing > 32 || satelliteCount > 1024 || questNumber < -100000000 ||
                    questNumber > 100000000) continue;
                selectedOffset = candidate;
                selectedNameEnd = cursor - 11;
                selectedSatelliteOffset = cursor;
                selectedName = graphName;
                matches++;
            }
            if (matches == 0) return false;

            planet.LateFieldsOffset = selectedOffset;
            planet.RangerCount = BitConverter.ToUInt16(MainPayload, selectedOffset - 10);
            planet.TransportCount = BitConverter.ToUInt16(MainPayload, selectedOffset - 8);
            planet.GraphRadius = BitConverter.ToUInt16(MainPayload, selectedOffset);
            planet.GraphName = selectedName;
            planet.GraphNameEnd = selectedNameEnd;
            planet.GraphSpeedRotate = BitConverter.ToUInt16(MainPayload, selectedNameEnd);
            planet.GraphStepRotate = ReadInt32(MainPayload, selectedNameEnd + 2);
            planet.GraphRing = MainPayload[selectedNameEnd + 6];
            planet.QuestNumber = ReadInt32(MainPayload, selectedNameEnd + 7);
            planet.SatelliteCountOffset = selectedSatelliteOffset;
            planet.SatelliteCount = BitConverter.ToUInt16(MainPayload, selectedSatelliteOffset);
            return true;
        }

        private bool TryLocatePlanetEndFields(PlanetHeaderRecord planet, int end)
        {
            if (end < planet.SatelliteCountOffset + 7 || MainPayload[end - 2] != 0 || MainPayload[end - 1] != 0)
                return false;
            int stringStart = end - 2;
            int cursor = end - 4;
            int characters = 0;
            while (cursor >= planet.SatelliteCountOffset + 2 && characters < 128)
            {
                int codeUnit = MainPayload[cursor] | MainPayload[cursor + 1] << 8;
                if (codeUnit == 0 || !IsSupportedObjectTextCharacter((char)codeUnit)) break;
                stringStart = cursor;
                cursor -= 2;
                characters++;
            }
            int flagsOffset = stringStart - 3;
            if (flagsOffset < planet.SatelliteCountOffset + 2 || MainPayload[flagsOffset] > 1 ||
                MainPayload[flagsOffset + 1] > 15 || MainPayload[flagsOffset + 2] > 1) return false;
            int textCursor = stringStart;
            string customFaction;
            if (!TryReadObjectString(ref textCursor, 128, true, out customFaction) || textCursor != end) return false;
            byte flags = MainPayload[flagsOffset + 1];
            planet.FlagsOffset = flagsOffset;
            planet.CustomFactionOffset = stringStart;
            planet.NoLanding = MainPayload[flagsOffset] != 0;
            planet.NoPlanetShopUpdate = (byte)(flags & 3);
            planet.NoBuyShips = (flags & 4) != 0;
            planet.NoRandomEvents = (flags & 8) != 0;
            planet.IsRogeria = MainPayload[flagsOffset + 2] != 0;
            planet.CustomFaction = customFaction;
            return true;
        }

        private bool TryReadPlanetSputniks(PlanetHeaderRecord planet)
        {
            return TryReadPlanetSputniks(planet, planet.FlagsOffset);
        }

        private bool TryReadPlanetSputniks(PlanetHeaderRecord planet, int limit)
        {
            if (planet == null || planet.SatelliteCountOffset <= 0 || limit <= 0 ||
                planet.SatelliteCountOffset > limit - 2) return false;
            int cursor = planet.SatelliteCountOffset + 2;
            int count = BitConverter.ToUInt16(MainPayload, planet.SatelliteCountOffset);
            List<PlanetSputnikRecord> records = new List<PlanetSputnikRecord>(count);
            for (int index = 0; index < count; index++)
            {
                if (cursor > limit - 10) return false;
                PlanetSputnikRecord record = new PlanetSputnikRecord();
                record.Start = cursor;
                record.ObjectId = ReadUInt32(MainPayload, cursor); cursor += 4;
                string graphName;
                if (!TryReadObjectString(ref cursor, 32768, false, out graphName) ||
                    cursor > limit - 8) return false;
                uint opaqueLength = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (opaqueLength > int.MaxValue || opaqueLength > (uint)(limit - cursor - 4))
                    return false;
                record.OpaqueData = new byte[(int)opaqueLength];
                if (opaqueLength != 0)
                    Buffer.BlockCopy(MainPayload, cursor, record.OpaqueData, 0, (int)opaqueLength);
                cursor += (int)opaqueLength;
                record.AngleCurrent = BitConverter.ToSingle(MainPayload, cursor); cursor += 4;
                if (float.IsNaN(record.AngleCurrent) || float.IsInfinity(record.AngleCurrent)) return false;
                record.GraphName = graphName;
                record.End = cursor;
                records.Add(record);
            }
            // The next TPlanet collection is GoneItems and starts with its own UInt16 count.
            // It is deliberately outside the TSputnik replacement span.
            if (cursor > limit - 2 || BitConverter.ToUInt16(MainPayload, cursor) > 10000)
                return false;
            planet.SatelliteCount = checked((ushort)count);
            planet.SatelliteEndOffset = cursor;
            planet.Satellites = records;
            return true;
        }

        private void ParsePlanetGoneItems(GalaxySummaryData summary)
        {
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            foreach (PlanetHeaderRecord planet in GalaxyPlanets)
            {
                if (!planet.HasFlags || planet.SatelliteEndOffset <= 0)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": границы GoneItems не локализованы.");
                int countOffset = planet.SatelliteEndOffset;
                if (countOffset > planet.FlagsOffset - 2)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": отсутствует счётчик GoneItems.");
                int count = BitConverter.ToUInt16(MainPayload, countOffset);
                int cursor = countOffset + 2;
                List<PlanetGoneItemRecord> records = new List<PlanetGoneItemRecord>(count);
                for (int index = 0; index < count; index++)
                {
                    if (cursor > planet.FlagsOffset - 9)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": GoneItem " + index + " обрезан.");
                    PlanetGoneItemRecord record = new PlanetGoneItemRecord();
                    record.Start = cursor;
                    record.PosX = MainPayload[cursor++];
                    record.PosY = MainPayload[cursor++];
                    record.LandType = MainPayload[cursor++];
                    record.Region = ReadInt32(MainPayload, cursor); cursor += 4;
                    byte miss = MainPayload[cursor++];
                    if (miss > 1)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": GoneItem " + index + " содержит неверный флаг Miss.");
                    record.Miss = miss != 0;
                    record.FactoryDiscriminatorOffset = cursor;
                    record.ItemType = MainPayload[cursor++];
                    if (record.ItemType == 68)
                    {
                        string customWeaponName;
                        if (!TryReadItemString(ref cursor, 512, out customWeaponName) ||
                            cursor >= planet.FlagsOffset)
                            throw new SavFormatException("TPlanet " + planet.ObjectId +
                                ": имя TCustomWeapon в GoneItem " + index + " повреждено.");
                    }
                    record.ItemStart = cursor;
                    ItemHeaderRecord item;
                    if (!itemsByStart.TryGetValue(record.ItemStart, out item))
                    {
                        if (!TryReadItemHeader(record.ItemStart, summary.NextObjectId, out item, true) ||
                            item.Type != record.ItemType || item.Type >= 8 &&
                                !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd))
                            throw new SavFormatException("TPlanet " + planet.ObjectId +
                                ": вложенный TItem GoneItem " + index + " не найден @ 0x" +
                                record.ItemStart.ToString("X") + " (type=" + record.ItemType + ").");
                        itemsByStart.Add(item.Start, item);
                        GalaxyItems.Add(item);
                    }
                    if (item.Type != record.ItemType ||
                        record.ItemType == 68 &&
                            item.CustomWeaponDiscriminatorOffset != record.FactoryDiscriminatorOffset)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": вложенный TItem GoneItem " + index + " не найден @ 0x" +
                            record.ItemStart.ToString("X") + ".");
                    record.ItemObjectId = item.ObjectId;
                    record.End = SerializedItemEnd(item);
                    if (record.End <= record.ItemStart || record.End > planet.FlagsOffset)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": неверная граница вложенного TItem GoneItem " + index +
                            " (type=" + record.ItemType + ", item=0x" + record.ItemStart.ToString("X") +
                            ", end=0x" + record.End.ToString("X") + ", flags=0x" +
                            planet.FlagsOffset.ToString("X") + ").");
                    records.Add(record);
                    cursor = record.End;
                }
                if (cursor != planet.FlagsOffset)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": список GoneItems закончился в 0x" + cursor.ToString("X") +
                        ", флаги начинаются в 0x" + planet.FlagsOffset.ToString("X") + ".");
                planet.GoneItemCountOffset = countOffset;
                planet.GoneItemEndOffset = cursor;
                planet.GoneItemCount = checked((ushort)count);
                planet.GoneItems = records;
            }

            HashSet<int> provenGoneItemStarts = new HashSet<int>();
            List<PlanetGoneItemRecord> goneItemRanges = new List<PlanetGoneItemRecord>();
            foreach (PlanetHeaderRecord planet in GalaxyPlanets)
                foreach (PlanetGoneItemRecord record in planet.GoneItems)
                {
                    provenGoneItemStarts.Add(record.ItemStart);
                    goneItemRanges.Add(record);
                }
            for (int itemIndex = GalaxyItems.Count - 1; itemIndex >= 0; itemIndex--)
            {
                ItemHeaderRecord item = GalaxyItems[itemIndex];
                if (provenGoneItemStarts.Contains(item.Start)) continue;
                foreach (PlanetGoneItemRecord range in goneItemRanges)
                    if (item.Start >= range.Start && item.Start < range.End)
                    {
                        GalaxyItems.RemoveAt(itemIndex);
                        break;
                    }
            }
        }

        private void ParsePlanetEquipmentShop(GalaxySummaryData summary)
        {
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            List<ShipItemListEntry> allRecords = new List<ShipItemListEntry>();
            foreach (PlanetHeaderRecord planet in GalaxyPlanets)
            {
                if (!planet.HasLateFields || planet.RelationEndOffset <= 0)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": границы EquipmentShop не локализованы.");
                int limit = planet.LateFieldsOffset - 10;
                int cursor = planet.RelationEndOffset;
                if (cursor > limit - 2)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": отсутствует счётчик EquipmentShop.");
                int countOffset = cursor;
                int count = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
                List<ShipItemListEntry> records = ReadPlanetShopItemEntries(planet, ref cursor,
                    limit, count, summary.NextObjectId, itemsByStart);
                if (cursor > limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": EquipmentShop пересекает поздние поля.");
                bool hasWarriorList = cursor <= limit - 2;
                int warriorCount = 0;
                if (hasWarriorList)
                {
                    warriorCount = BitConverter.ToUInt16(MainPayload, cursor);
                    if (warriorCount > 10000)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": неверный счётчик Warriors после EquipmentShop.");
                }
                else if (cursor != limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": между EquipmentShop и поздними полями остался один байт.");
                planet.EquipmentShopCountOffset = countOffset;
                planet.EquipmentShopEndOffset = cursor;
                planet.EquipmentShopCount = checked((ushort)count);
                planet.EquipmentShopItems = records;
                planet.WarriorCountOffset = hasWarriorList ? cursor : -1;
                planet.WarriorCount = checked((ushort)warriorCount);
                planet.HasWarriorList = hasWarriorList;
                planet.Warriors = hasWarriorList ? ReadPlanetWarriorEntries(planet,
                    cursor + 2, limit, warriorCount) : new List<PlanetWarriorRecord>();
                planet.WarriorEndOffset = limit;
                allRecords.AddRange(records);
            }

            // The one-byte factory discriminator (and the TCustomWeapon name wrapper)
            // precedes the real TItem object id. Remove scanner candidates only from
            // that proven wrapper prefix; nested object spans remain available.
            for (int itemIndex = GalaxyItems.Count - 1; itemIndex >= 0; itemIndex--)
            {
                ItemHeaderRecord item = GalaxyItems[itemIndex];
                foreach (ShipItemListEntry record in allRecords)
                    if (item.Start >= record.Start && item.Start < record.ItemStart)
                    {
                        GalaxyItems.RemoveAt(itemIndex);
                        break;
                    }
            }
        }

        private List<PlanetWarriorRecord> ReadPlanetWarriorEntries(PlanetHeaderRecord planet,
            int dataStart, int limit, int count)
        {
            List<ShipHeaderRecord> ships = new List<ShipHeaderRecord>();
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.Start > dataStart && ship.Start < limit &&
                    ship.Start > 0 && MainPayload[ship.Start - 1] == ship.Type)
                    ships.Add(ship);
            ships.Sort(delegate(ShipHeaderRecord left, ShipHeaderRecord right)
                { return left.Start.CompareTo(right.Start); });
            if (count == 0)
            {
                if (dataStart != limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": после пустого Warriors остались непрочитанные байты.");
                return new List<PlanetWarriorRecord>();
            }
            if (count == 1)
            {
                ShipHeaderRecord exact = null;
                foreach (ShipHeaderRecord candidate in ships)
                    if (candidate.Start == dataStart + 1) { exact = candidate; break; }
                if (exact == null || !exact.HasCommonTail)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": единственный TShip Warriors не разобран от начала списка.");
                PlanetWarriorRecord single = new PlanetWarriorRecord();
                single.Start = dataStart; single.End = limit;
                single.ShipType = MainPayload[dataStart]; single.ShipStart = exact.Start;
                single.ShipObjectId = exact.ObjectId;
                GalaxyShips.RemoveAll(delegate(ShipHeaderRecord candidate)
                {
                    return candidate.Start > dataStart && candidate.Start < limit &&
                        candidate.Start != exact.Start;
                });
                return new List<PlanetWarriorRecord> { single };
            }
            Dictionary<int, ShipHeaderRecord> shipsByStart =
                new Dictionary<int, ShipHeaderRecord>();
            foreach (ShipHeaderRecord ship in ships) shipsByStart[ship.Start] = ship;
            List<PlanetWarriorRecord> records = new List<PlanetWarriorRecord>(count);
            HashSet<int> selectedStarts = new HashSet<int>();
            int cursor = dataStart;
            for (int index = 0; index < count; index++)
            {
                ShipHeaderRecord ship;
                if (!shipsByStart.TryGetValue(cursor + 1, out ship))
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": TShip Warriors " + index + " не следует за границей записи.");
                int knownEnd = SerializedGalaxyShipEnd(ship);
                if (knownEnd <= ship.Start || knownEnd > limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": TShip Warriors " + index + " не имеет доказанного конца" +
                        " (id=" + ship.ObjectId + ", type=" + ship.Type + ", start=0x" +
                        ship.Start.ToString("X") + ", common=" + ship.HasCommonTail +
                        ", simple=" + ship.HasSimpleDerivedTail + ", ranger=" +
                        ship.HasRangerTail + ", ruins=" + ship.HasRuinsTail +
                        ", knownEnd=0x" + knownEnd.ToString("X") + ", limit=0x" +
                        limit.ToString("X") + ").");
                int end = limit;
                if (index + 1 < count)
                {
                    int nextStart = int.MaxValue;
                    foreach (ShipHeaderRecord candidate in ships)
                        if (candidate.Start - 1 >= knownEnd && candidate.Start < nextStart)
                            nextStart = candidate.Start;
                    if (nextStart == int.MaxValue)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": не найден следующий TShip Warriors после 0x" +
                            knownEnd.ToString("X") + ".");
                    end = nextStart - 1;
                }
                PlanetWarriorRecord record = new PlanetWarriorRecord();
                record.Start = cursor;
                record.End = end;
                record.ShipType = MainPayload[cursor];
                record.ShipStart = ship.Start;
                record.ShipObjectId = ship.ObjectId;
                records.Add(record);
                selectedStarts.Add(ship.Start);
                cursor = end;
            }
            if (cursor != limit)
                throw new SavFormatException("TPlanet " + planet.ObjectId +
                    ": Warriors не завершился на границе поздних полей.");
            GalaxyShips.RemoveAll(delegate(ShipHeaderRecord candidate)
            {
                return candidate.Start > dataStart && candidate.Start < limit &&
                    !selectedStarts.Contains(candidate.Start);
            });
            return records;
        }

        private List<ShipItemListEntry> ReadPlanetShopItemEntries(PlanetHeaderRecord planet,
            ref int cursor, int limit, int count, uint nextObjectId,
            Dictionary<int, ItemHeaderRecord> itemsByStart)
        {
            if (count < 0 || count > 10000)
                throw new SavFormatException("TPlanet " + planet.ObjectId +
                    ": неверное число предметов EquipmentShop " + count + ".");
            List<ShipItemListEntry> records = new List<ShipItemListEntry>();
            for (int index = 0; index < count; index++)
            {
                if (cursor >= limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": EquipmentShop item " + index + " начинается за границей.");
                if (records.Count > 0 && records[records.Count - 1].ItemType == 25 && cursor > 0)
                {
                    ItemHeaderRecord adjacent;
                    byte adjacentType = MainPayload[cursor - 1];
                    if (adjacentType <= 75 && TryReadItemHeader(cursor, nextObjectId,
                        out adjacent, true) && adjacent.Type == adjacentType)
                    {
                        ShipItemListEntry previous = records[records.Count - 1];
                        previous.End--;
                        ItemHeaderRecord previousItem;
                        if (itemsByStart.TryGetValue(previous.ItemStart, out previousItem) &&
                            previousItem.Type == 25 && previousItem.DerivedTailEnd == cursor)
                            previousItem.DerivedTailEnd--;
                        cursor--;
                    }
                }
                ShipItemListEntry record = new ShipItemListEntry();
                record.Start = cursor;
                record.ItemType = MainPayload[cursor++];
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= limit)
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": EquipmentShop item " + index + " содержит повреждённое имя TCustomWeapon.");
                }
                record.ItemStart = cursor;
                ItemHeaderRecord item;
                if (!itemsByStart.TryGetValue(record.ItemStart, out item))
                {
                    if (!TryReadItemHeader(record.ItemStart, nextObjectId, out item, true) ||
                        item.Type != record.ItemType || item.Type >= 8 &&
                            !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd))
                        throw new SavFormatException("TPlanet " + planet.ObjectId +
                            ": EquipmentShop item " + index + " не разобран @ 0x" +
                            record.ItemStart.ToString("X") + " (type=" + record.ItemType + ").");
                    itemsByStart.Add(item.Start, item);
                    GalaxyItems.Add(item);
                }
                if (item.Type != record.ItemType || record.ItemType == 68 &&
                    item.CustomWeaponDiscriminatorOffset != record.Start)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": EquipmentShop item " + index + " не совпал с factory type @ 0x" +
                        record.Start.ToString("X") + ".");
                record.ItemObjectId = item.ObjectId;
                record.End = SerializedItemEnd(item);
                if (record.End <= record.ItemStart || record.End > limit)
                    throw new SavFormatException("TPlanet " + planet.ObjectId +
                        ": EquipmentShop item " + index + " имеет неверный end=0x" +
                        record.End.ToString("X") + " (type=" + record.ItemType + ").");
                records.Add(record);
                cursor = record.End;
            }
            return records;
        }

        private bool TryLocateAsteroidSection(StarHeaderRecord star, int shipCountOffset,
            uint nextObjectId, out List<AsteroidRecord> selected, out int selectedCountOffset)
        {
            selected = null;
            selectedCountOffset = -1;
            bool zeroCandidate = shipCountOffset >= star.HeaderEnd + 2 &&
                BitConverter.ToUInt16(MainPayload, shipCountOffset - 2) == 0;
            int nonZeroMatches = 0;
            for (int countOffset = star.HeaderEnd; countOffset <= shipCountOffset - 2; countOffset++)
            {
                int count = BitConverter.ToUInt16(MainPayload, countOffset);
                if (count < 1 || count > 10000) continue;
                int cursor = countOffset + 2;
                List<AsteroidRecord> route = new List<AsteroidRecord>(count);
                bool valid = true;
                for (int index = 0; index < count; index++)
                {
                    AsteroidRecord asteroid;
                    if (!TryReadAsteroidRecord(cursor, shipCountOffset, nextObjectId,
                        star.ObjectId, out asteroid))
                    {
                        valid = false;
                        break;
                    }
                    route.Add(asteroid);
                    cursor = asteroid.End;
                }
                if (!valid || cursor != shipCountOffset) continue;
                selected = route;
                selectedCountOffset = countOffset;
                nonZeroMatches++;
            }
            if (nonZeroMatches == 1) return true;
            if (nonZeroMatches != 0 || !zeroCandidate) return false;
            selected = new List<AsteroidRecord>();
            selectedCountOffset = shipCountOffset - 2;
            return true;
        }

        private bool TryReadAsteroidRecord(int start, int limit, uint nextObjectId, uint parentStarId,
            out AsteroidRecord value)
        {
            value = null;
            if (start < 0 || start > limit - 30) return false;
            uint objectId = ReadUInt32(MainPayload, start);
            if (objectId == 0 || objectId >= nextObjectId) return false;
            int offset = start + 4;
            string graphName;
            if (!TryReadObjectString(ref offset, 128, false, out graphName) || offset > limit - 24) return false;
            if (!graphName.StartsWith("Asteroid.", StringComparison.OrdinalIgnoreCase)) return false;
            int graphNameEnd = offset;
            float positionX = BitConverter.ToSingle(MainPayload, offset);
            float positionY = BitConverter.ToSingle(MainPayload, offset + 4);
            float speedX = BitConverter.ToSingle(MainPayload, offset + 8);
            float speedY = BitConverter.ToSingle(MainPayload, offset + 12);
            float mass = BitConverter.ToSingle(MainPayload, offset + 16);
            if (!IsSupportedAsteroidScalar(positionX) || !IsSupportedAsteroidScalar(positionY) ||
                !IsSupportedAsteroidScalar(speedX) || !IsSupportedAsteroidScalar(speedY) ||
                !IsSupportedAsteroidScalar(mass)) return false;

            AsteroidRecord asteroid = new AsteroidRecord();
            asteroid.Start = start; asteroid.GraphNameEnd = graphNameEnd; asteroid.End = offset + 24;
            asteroid.ParentStarId = parentStarId; asteroid.ObjectId = objectId; asteroid.GraphName = graphName;
            asteroid.PositionX = positionX; asteroid.PositionY = positionY;
            asteroid.SpeedX = speedX; asteroid.SpeedY = speedY;
            asteroid.Mass = mass; asteroid.Minerals = ReadInt32(MainPayload, offset + 20);
            value = asteroid;
            return asteroid.End <= limit;
        }

        private static bool IsSupportedAsteroidScalar(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && Math.Abs((double)value) <= 1.0E15;
        }

        private static bool IsSupportedPlanetScalar(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && Math.Abs((double)value) <= 1.0E15;
        }

        private void ParseMissileRecords(GalaxySummaryData summary)
        {
            Dictionary<uint, MissileRecord> missiles = new Dictionary<uint, MissileRecord>();
            for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
            {
                StarHeaderRecord star = GalaxyStars[starIndex];
                int limit = starIndex + 1 < GalaxyStars.Count
                    ? GalaxyStars[starIndex + 1].Start : summary.PlanetReferenceListOffset;
                List<MissileRecord> selected = null;
                int selectedCountOffset = -1;
                int matches = 0;
                for (int countOffset = star.HeaderEnd; countOffset <= limit - 84; countOffset++)
                {
                    int count = MainPayload[countOffset] | MainPayload[countOffset + 1] << 8;
                    if (count < 1 || count > 1024) continue;
                    int cursor = countOffset + 2;
                    List<MissileRecord> route = new List<MissileRecord>(count);
                    bool valid = true;
                    for (int index = 0; index < count; index++)
                    {
                        MissileRecord missile;
                        if (!TryReadMissileRecord(ref cursor, limit, star.ObjectId, out missile))
                        {
                            valid = false;
                            break;
                        }
                        route.Add(missile);
                    }
                    if (!valid) continue;
                    selected = route;
                    selectedCountOffset = countOffset;
                    matches++;
                }
                if (matches == 0) continue;
                if (matches != 1)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": найдено несколько структурных маршрутов TMissile.");
                star.MissileCountOffset = selectedCountOffset;
                foreach (MissileRecord missile in selected)
                {
                    MissileRecord previous;
                    if (missiles.TryGetValue(missile.ObjectId, out previous))
                        throw new SavFormatException("TMissile: object id " + missile.ObjectId +
                            " повторяется в звёздах " + previous.ParentStarId + " и " + missile.ParentStarId + ".");
                    missiles.Add(missile.ObjectId, missile);
                    GalaxyMissiles.Add(missile);
                }
            }
        }

        private bool TryReadMissileRecord(ref int cursor, int limit, uint parentStarId, out MissileRecord value)
        {
            value = null;
            int start = cursor;
            if (cursor >= limit) return false;
            byte outerType = MainPayload[cursor++];
            if (outerType == 0 || outerType > 96) return false;
            string customWeaponName = string.Empty;
            bool isCustom = outerType == 68;
            if (isCustom && !TryReadObjectString(ref cursor, 128, false, out customWeaponName)) return false;
            int baseStart = cursor;
            if (cursor > limit - 81) return false;

            uint objectId = ReadUInt32(MainPayload, cursor); cursor += 4;
            uint weaponId = ReadUInt32(MainPayload, cursor); cursor += 4;
            byte weaponType = MainPayload[cursor++];
            byte techLevel = MainPayload[cursor++];
            int damageMin = ReadInt32(MainPayload, cursor); cursor += 4;
            int damageMax = ReadInt32(MainPayload, cursor); cursor += 4;
            if (objectId == 0 || objectId > 10000000 || weaponId > 10000000 ||
                weaponType != outerType || techLevel > 32 ||
                damageMin < -100000000 || damageMin > 100000000 ||
                damageMax < -100000000 || damageMax > 100000000) return false;

            int bonusOffset = cursor;
            int bonus = ReadInt32(MainPayload, cursor); cursor += 4;
            if (bonus < 0 || bonus > 4096) return false;
            uint bonusReferenceId = 0;
            if (bonus > 0)
            {
                if (cursor > limit - 4) return false;
                bonusReferenceId = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (bonusReferenceId == 0) return false;
            }
            int bonusEnd = cursor;

            int specialOffset = cursor;
            int special = ReadInt32(MainPayload, cursor); cursor += 4;
            if (special < 0 || special > 4096) return false;
            uint specialReferenceId = 0;
            if (special > 0)
            {
                if (cursor > limit - 4) return false;
                specialReferenceId = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (specialReferenceId == 0) return false;
            }
            int specialEnd = cursor;

            int positionOffset = cursor;
            float positionX, positionY, angle, fromAngle;
            if (!TryReadMissileFloat(ref cursor, limit, out positionX) ||
                !TryReadMissileFloat(ref cursor, limit, out positionY) ||
                !TryReadMissileFloat(ref cursor, limit, out angle) ||
                !TryReadMissileFloat(ref cursor, limit, out fromAngle)) return false;
            int starOffset = cursor;
            if (cursor > limit - 8) return false;
            uint starId = ReadUInt32(MainPayload, cursor); cursor += 4;
            uint shipId = ReadUInt32(MainPayload, cursor); cursor += 4;
            if (starId != parentStarId || shipId > 10000000) return false;

            int targetOffset = cursor;
            byte targetType;
            uint targetId;
            if (!TryReadMissileReference(ref cursor, limit, out targetType, out targetId)) return false;
            int targetEnd = cursor;
            if (cursor > limit - 5) return false;
            int missileNoOffset = cursor;
            byte missileNo = MainPayload[cursor++];
            int liveOffset = cursor;
            int live = ReadInt32(MainPayload, cursor); cursor += 4;
            if (live < -100000000 || live > 100000000) return false;

            int motionOffset = cursor;
            float fromAngleOld, speed, baseSpeed;
            if (!TryReadMissileFloat(ref cursor, limit, out fromAngleOld) ||
                !TryReadMissileFloat(ref cursor, limit, out speed) ||
                !TryReadMissileFloat(ref cursor, limit, out baseSpeed)) return false;
            int targetLostOffset = cursor;
            byte targetLostType;
            uint targetLostId;
            if (!TryReadMissileReference(ref cursor, limit, out targetLostType, out targetLostId)) return false;
            int targetLostEnd = cursor;
            int lastMotionOffset = cursor;
            float lastPositionX, lastPositionY, lastDistanceMin;
            if (!TryReadMissileFloat(ref cursor, limit, out lastPositionX) ||
                !TryReadMissileFloat(ref cursor, limit, out lastPositionY) ||
                !TryReadMissileFloat(ref cursor, limit, out lastDistanceMin)) return false;

            MissileRecord missile = new MissileRecord();
            missile.Start = start; missile.BaseStart = baseStart; missile.End = cursor;
            missile.BonusOffset = bonusOffset; missile.BonusEnd = bonusEnd;
            missile.SpecialOffset = specialOffset; missile.SpecialEnd = specialEnd;
            missile.PositionOffset = positionOffset; missile.StarOffset = starOffset;
            missile.TargetOffset = targetOffset; missile.TargetEnd = targetEnd;
            missile.MissileNoOffset = missileNoOffset; missile.LiveOffset = liveOffset;
            missile.MotionOffset = motionOffset; missile.TargetLostOffset = targetLostOffset;
            missile.TargetLostEnd = targetLostEnd; missile.LastMotionOffset = lastMotionOffset;
            missile.ParentStarId = parentStarId; missile.IsCustom = isCustom;
            missile.CustomWeaponName = customWeaponName; missile.ObjectId = objectId;
            missile.WeaponId = weaponId; missile.WeaponType = weaponType; missile.TechLevel = techLevel;
            missile.DamageMin = damageMin; missile.DamageMax = damageMax;
            missile.Bonus = bonus; missile.BonusReferenceId = bonusReferenceId;
            missile.Special = special; missile.SpecialReferenceId = specialReferenceId;
            missile.PositionX = positionX; missile.PositionY = positionY; missile.Angle = angle;
            missile.FromAngle = fromAngle; missile.StarId = starId; missile.ShipId = shipId;
            missile.TargetType = targetType; missile.TargetId = targetId; missile.MissileNo = missileNo;
            missile.Live = live; missile.FromAngleOld = fromAngleOld; missile.Speed = speed;
            missile.BaseSpeed = baseSpeed; missile.TargetLostType = targetLostType;
            missile.TargetLostId = targetLostId; missile.LastPositionX = lastPositionX;
            missile.LastPositionY = lastPositionY; missile.LastDistanceMin = lastDistanceMin;
            value = missile;
            return true;
        }

        private bool TryReadMissileReference(ref int cursor, int limit, out byte type, out uint objectId)
        {
            type = 0; objectId = 0;
            if (cursor >= limit) return false;
            type = MainPayload[cursor++];
            if (type > 4) return false;
            if (type == 0) return true;
            if (cursor > limit - 4) return false;
            objectId = ReadUInt32(MainPayload, cursor); cursor += 4;
            return objectId > 0 && objectId <= 10000000;
        }

        private bool TryReadMissileFloat(ref int cursor, int limit, out float value)
        {
            value = 0;
            if (cursor > limit - 4) return false;
            value = BitConverter.ToSingle(MainPayload, cursor); cursor += 4;
            // TMissile.Write stores raw Single values.  In particular the sentinel-like
            // LastDistanceMin used by live saves can be around 1E20, so the heuristic
            // asteroid limit (1E15) is not valid for this serializer.  The exact list
            // boundary supplies the structural validation; reject only non-finite data.
            return IsSupportedMissileScalar(value);
        }

        private static bool IsSupportedMissileScalar(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private void ParseShipHeaders(GalaxySummaryData summary)
        {
            Dictionary<uint, int> zeroTypeCounts = new Dictionary<uint, int>();
            HashSet<uint> starIds = new HashSet<uint>();
            HashSet<uint> planetIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in GalaxyStars) starIds.Add(star.ObjectId);
            foreach (PlanetHeaderRecord planet in GalaxyPlanets) planetIds.Add(planet.ObjectId);
            int playerHeaderCount = 0;
            for (int start = GalaxyStarsOffset + 1; start <= summary.PlanetReferenceListOffset - 190; start++)
            {
                byte discriminator = MainPayload[start - 1];
                if (discriminator > 13 && discriminator != 0xFF) continue;
                uint objectId = ReadUInt32(MainPayload, start);
                if (objectId == 0 || objectId >= summary.NextObjectId) continue;
                if (discriminator == 0xFF && objectId != summary.PlayerObjectId) continue;

                int offset = start + 4;
                string name;
                if (!TryReadObjectString(ref offset, 80, true, out name)) continue;
                if (discriminator == 0 && name.Length < 3) continue;
                int nameEnd = offset;
                string scriptName;
                if (!TryReadObjectString(ref offset, 128, true, out scriptName)) continue;
                int scriptNameEnd = offset;
                if (offset > summary.PlanetReferenceListOffset - 177) continue;
                byte type = MainPayload[offset];
                byte owner = MainPayload[offset + 1];
                if (type > 13 || owner > 7) continue;
                if (discriminator != 0xFF && discriminator != type) continue;

                float x = BitConverter.ToSingle(MainPayload, offset + 2);
                float y = BitConverter.ToSingle(MainPayload, offset + 6);
                if (!IsSupportedShipCoordinate(x) || !IsSupportedShipCoordinate(y)) continue;
                bool referencesValid = true;
                for (int index = 0; index < 4; index++)
                    if (ReadUInt32(MainPayload, offset + 10 + index * 4) >= summary.NextObjectId)
                    {
                        referencesValid = false;
                        break;
                    }
                if (!referencesValid) continue;
                uint homePlanetId = ReadUInt32(MainPayload, offset + 10);
                uint currentStarId = ReadUInt32(MainPayload, offset + 14);
                uint currentPlanetId = ReadUInt32(MainPayload, offset + 18);
                if (type == 0 &&
                    ((homePlanetId != 0 && !planetIds.Contains(homePlanetId)) ||
                     (currentStarId != 0 && !starIds.Contains(currentStarId)) ||
                     (currentPlanetId != 0 && !planetIds.Contains(currentPlanetId)))) continue;
                if (type == 0 && objectId <= 128 && !ContainsCyrillic(name)) continue;
                int itemCountOffset = offset + 175;
                int itemCount = MainPayload[itemCountOffset] | MainPayload[itemCountOffset + 1] << 8;
                if (MainPayload[offset + 174] > 4 || itemCount > 128) continue;
                // Do not enforce ObjectId uniqueness during the broad prefix scan:
                // header-like bytes inside a real ship can duplicate its id.  Those
                // candidates are pruned after their complete common tail and exact
                // owner collection are checked.  Serialized position is the stable
                // identity used by the writer.  Type-zero candidates still require
                // a unique prefix occurrence below because their pattern is even
                // more prone to false positives.
                if (type == 0)
                {
                    int count;
                    zeroTypeCounts.TryGetValue(objectId, out count);
                    zeroTypeCounts[objectId] = count + 1;
                }

                ShipHeaderRecord ship = new ShipHeaderRecord();
                ship.Start = start;
                ship.NameEnd = nameEnd;
                ship.ScriptNameEnd = scriptNameEnd;
                ship.FixedPrefixEnd = itemCountOffset + 2;
                ship.ObjectId = objectId;
                ship.Type = type;
                ship.Owner = owner;
                ship.IsPlayer = discriminator == 0xFF;
                ship.Name = name;
                ship.ScriptName = scriptName;
                ship.X = x;
                ship.Y = y;
                ship.HomePlanetId = homePlanetId;
                ship.CurrentStarId = currentStarId;
                ship.CurrentPlanetId = currentPlanetId;
                ship.CurrentShipId = ReadUInt32(MainPayload, offset + 22);
                int goodsOffset = offset + 26;
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 4; field++)
                        ship.Goods[good, field] = ReadUInt32(MainPayload, goodsOffset + good * 16 + field * 4);
                ship.Money = ReadUInt32(MainPayload, offset + 154);
                ship.Rnd = ReadUInt32(MainPayload, offset + 158);
                ship.RndOut = ReadUInt32(MainPayload, offset + 162);
                ship.Day = ReadUInt32(MainPayload, offset + 166);
                ship.Face = ReadInt32(MainPayload, offset + 170);
                ship.PilotRace = MainPayload[offset + 174];
                ship.EquipmentItemCount = checked((ushort)itemCount);
                GalaxyShips.Add(ship);
                if (discriminator == 0xFF) playerHeaderCount++;
            }
            GalaxyShips.RemoveAll(delegate(ShipHeaderRecord ship)
            {
                return ship.Type == 0 && zeroTypeCounts[ship.ObjectId] != 1;
            });
            ShipCount = 0; StationCount = 0;
            foreach (ShipHeaderRecord ship in GalaxyShips)
                if (ship.IsStation) StationCount++; else ShipCount++;
            if (GalaxyShips.Count == 0 || playerHeaderCount != 1)
                throw new SavFormatException("TShip: структурный проход не нашёл единственный корабль игрока.");
            ParseShipCommonTails(summary);
        }

        private void ParseShipCommonTails(GalaxySummaryData summary)
        {
            GalaxyShips.Sort(delegate(ShipHeaderRecord left, ShipHeaderRecord right)
            {
                return left.Start.CompareTo(right.Start);
            });
            for (int shipIndex = 0; shipIndex < GalaxyShips.Count; shipIndex++)
            {
                ShipHeaderRecord ship = GalaxyShips[shipIndex];
                int hardLimit = summary.PlanetReferenceListOffset;
                foreach (StarHeaderRecord star in GalaxyStars)
                    if (star.Start > ship.Start)
                    {
                        if (star.Start - 1 < hardLimit) hardLimit = star.Start - 1;
                        break;
                    }
                int limit = hardLimit;
                int recordEnd = hardLimit;
                if (shipIndex + 1 < GalaxyShips.Count && GalaxyShips[shipIndex + 1].Start - 1 < limit)
                {
                    limit = GalaxyShips[shipIndex + 1].Start - 1;
                    recordEnd = GalaxyShips[shipIndex + 1].Start - 2;
                }
                ShipHeaderRecord parsed = null;
                for (int boundaryPass = 0; boundaryPass < 2 && parsed == null; boundaryPass++)
                {
                    int candidateLimit = boundaryPass == 0 ? limit : hardLimit;
                    int candidateRecordEnd = boundaryPass == 0 ? recordEnd : hardLimit;
                    if (boundaryPass != 0 && (candidateLimit == limit || ship.Type >= 6)) continue;
                    // Header-like byte sequences can occur inside a generated ship.
                    // Retry non-ruins records against the enclosing star/galaxy span
                    // when the nearest scanned candidate truncates the real record.
                    for (int graphPass = 0; graphPass < 2 && parsed == null; graphPass++)
                    for (int graphStart = ship.FixedPrefixEnd + 30;
                        graphStart <= candidateLimit - 520; graphStart++)
                    {
                        if (MainPayload[graphStart + 1] != 0 || MainPayload[graphStart] < 0x20 ||
                            MainPayload[graphStart] > 0x7E) continue;
                        ShipHeaderRecord candidate = ship.Clone();
                        if (!TryReadShipCommonTail(candidate, graphStart, candidateLimit)) continue;
                        bool preferredGraph = IsPreferredShipGraphName(candidate.GraphName);
                        if ((graphPass == 0 && !preferredGraph) ||
                            (graphPass == 1 && preferredGraph)) continue;
                        if (candidate.Type >= 1 && candidate.Type <= 4 &&
                            !TryReadNormalShipTail(candidate, candidateLimit)) continue;
                        if (candidate.Type == 1 &&
                            !TryReadRangerShipTail(candidate, candidateLimit)) continue;
                        if (candidate.IsPlayer &&
                            !TryReadPlayerPrefix(candidate, candidateLimit)) continue;
                        if ((candidate.Type == 0 || candidate.Type == 2 || candidate.Type == 3 ||
                            candidate.Type == 4) &&
                            !TryReadSimpleDerivedShipTail(candidate, candidateLimit)) continue;
                        if (candidate.Type == 5 &&
                            !TryReadTranclucatorShipTail(candidate, candidateLimit)) continue;
                        if (candidate.Type >= 6 && candidate.Type <= 13 &&
                            !TryReadRuinsShipTail(candidate, candidateRecordEnd,
                                summary.NextObjectId)) continue;
                        parsed = candidate;
                        break;
                    }
                }
                if (parsed != null)
                {
                    int index = GalaxyShips.IndexOf(ship);
                    GalaxyShips[index] = parsed;
                }
            }
        }

        private bool TryReadNormalShipTail(ShipHeaderRecord ship, int limit)
        {
            int offset = ship.CommonTailEnd;
            if (offset < 0 || offset > limit - 60) return false;
            byte coalitionRank = MainPayload[offset + 44];
            byte pirateRank = MainPayload[offset + 47];
            uint liberationPlanetId = ReadUInt32(MainPayload, offset + 36);
            uint lastPlanetId = ReadUInt32(MainPayload, offset + 52);
            if (coalitionRank > 7 || pirateRank > 7 || liberationPlanetId > 100000 ||
                lastPlanetId > 100000) return false;
            ship.HasNormalShipTail = true;
            ship.NormalShipTailOffset = offset;
            ship.KillAllShips = ReadInt32(MainPayload, offset);
            ship.KillPirates = ReadInt32(MainPayload, offset + 4);
            ship.KillDominators = ReadInt32(MainPayload, offset + 8);
            ship.LiberationSystems = ReadInt32(MainPayload, offset + 12);
            ship.KillPacifics = ReadInt32(MainPayload, offset + 16);
            ship.KillWarriors = ReadInt32(MainPayload, offset + 20);
            ship.KillRangers = ReadInt32(MainPayload, offset + 24);
            ship.KillInCurrentSystemDominators = BitConverter.ToUInt16(MainPayload, offset + 28);
            ship.KillInCurrentSystemPirates = BitConverter.ToUInt16(MainPayload, offset + 30);
            ship.KillInCurrentSystemNormals = BitConverter.ToUInt16(MainPayload, offset + 32);
            ship.KillCustomInCurrentSystem = BitConverter.ToUInt16(MainPayload, offset + 34);
            ship.LiberationPlanetId = liberationPlanetId;
            ship.LiberationKills = ReadInt32(MainPayload, offset + 40);
            ship.CoalitionRank = coalitionRank;
            ship.CoalitionRankPoints = BitConverter.ToUInt16(MainPayload, offset + 45);
            ship.PirateRank = pirateRank;
            ship.PirateRankPoints = ReadUInt32(MainPayload, offset + 48);
            ship.LastPlanetId = lastPlanetId;
            ship.TurnPlayerMoneyGoods = ReadInt32(MainPayload, offset + 56);
            return true;
        }

        private bool TryReadSimpleDerivedShipTail(ShipHeaderRecord ship, int limit)
        {
            int offset = ship.Type == 0 ? ship.CommonTailEnd : ship.NormalShipTailOffset + 60;
            if (offset < 0 || offset > limit) return false;
            ship.SimpleDerivedTailOffset = offset;
            switch (ship.Type)
            {
                case 0:
                    if (offset > limit - 7 || MainPayload[offset] > 7 || MainPayload[offset + 1] > 2 ||
                        MainPayload[offset + 6] > 11) return false;
                    ship.DominatorType = MainPayload[offset];
                    ship.DominatorSeries = MainPayload[offset + 1];
                    ship.RunProgramDate = ReadInt32(MainPayload, offset + 2);
                    ship.RunProgramName = MainPayload[offset + 6];
                    break;
                case 2:
                    if (MainPayload[offset] > 2) return false;
                    ship.TransportType = MainPayload[offset];
                    break;
                case 3:
                    if (offset > limit - 9 || MainPayload[offset + 4] > 3) return false;
                    float desireConflict = BitConverter.ToSingle(MainPayload, offset + 5);
                    if (!IsSupportedAsteroidScalar(desireConflict)) return false;
                    ship.PiratePrison = ReadUInt32(MainPayload, offset);
                    ship.PirateType = MainPayload[offset + 4];
                    ship.DesireConflict = desireConflict;
                    break;
                case 4:
                    if (MainPayload[offset] > 1) return false;
                    ship.WarriorType = MainPayload[offset];
                    break;
                default:
                    return false;
            }
            ship.HasSimpleDerivedTail = true;
            return true;
        }

        private bool TryReadRangerShipTail(ShipHeaderRecord ship, int limit)
        {
            int offset = ship.NormalShipTailOffset + 60;
            if (offset < 0 || offset > limit - 10 || MainPayload[offset + 6] > 2 ||
                MainPayload[offset + 7] > 100) return false;
            int cursor = offset + 8;
            ushort questCount = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
            if (questCount > 2048) return false;
            List<RangerQuestRecord> quests = new List<RangerQuestRecord>(questCount);
            for (int quest = 0; quest < questCount; quest++)
            {
                if (cursor > limit - 20) return false;
                RangerQuestRecord record = new RangerQuestRecord();
                record.Start = cursor;
                record.Type = MainPayload[cursor];
                record.Number = BitConverter.ToUInt16(MainPayload, cursor + 1);
                record.PlanetObjectId = ReadUInt32(MainPayload, cursor + 3);
                record.Turn = ReadInt32(MainPayload, cursor + 7);
                record.Reward = ReadInt32(MainPayload, cursor + 11);
                record.ObjectId = ReadUInt32(MainPayload, cursor + 15);
                if (MainPayload[cursor + 19] > 1) return false;
                record.Successful = MainPayload[cursor + 19] != 0;
                cursor += 20;
                if (!TryReadStreamText(ref cursor, 32768, limit, out record.Text) ||
                    !TryReadStreamText(ref cursor, 32768, limit, out record.Congratulations) ||
                    !TryReadStreamText(ref cursor, 32768, limit, out record.SpecialText))
                    return false;
                record.End = cursor;
                quests.Add(record);
            }
            if (cursor > limit - 64 || MainPayload[cursor + 63] > 1) return false;
            uint lastShipId = ReadUInt32(MainPayload, cursor + 7);
            if (lastShipId > 100000) return false;
            ship.HasRangerTail = true;
            ship.RangerTailOffset = offset;
            ship.RangerPostQuestOffset = cursor;
            ship.RangerQuestCount = questCount;
            ship.RangerQuests = quests;
            ship.RangerStatusTrader = MainPayload[offset];
            ship.RangerStatusPirate = MainPayload[offset + 1];
            ship.RangerStatusWarrior = MainPayload[offset + 2];
            ship.EminentPointsTrader = MainPayload[offset + 3];
            ship.EminentPointsPirate = MainPayload[offset + 4];
            ship.EminentPointsWarrior = MainPayload[offset + 5];
            ship.RangerMoral = MainPayload[offset + 6];
            ship.Courageous = MainPayload[offset + 7];
            ship.StatusChangeWarrior = MainPayload[cursor];
            ship.StatusChangePirate = MainPayload[cursor + 1];
            ship.StatusChangeTrader = MainPayload[cursor + 2];
            ship.RangerPrison = ReadUInt32(MainPayload, cursor + 3);
            ship.LastShipId = lastShipId;
            ship.Nods = ReadInt32(MainPayload, cursor + 11);
            for (int index = 0; index < 12; index++)
                ship.ProgramCounts[index] = ReadInt32(MainPayload, cursor + 15 + index * 4);
            ship.ExcludedFromRating = MainPayload[cursor + 63] != 0;
            return true;
        }

        private bool TryReadPlayerPrefix(ShipHeaderRecord ship, int limit)
        {
            int offset = ship.RangerPostQuestOffset + 64;
            if (!ship.IsPlayer || !ship.HasRangerTail || offset < 0 || offset > limit - 49 ||
                MainPayload[offset] > 1 || MainPayload[offset + 1] > 1 ||
                MainPayload[offset + 2] > 1) return false;
            int objectStateCount = ReadInt32(MainPayload, offset + 46);
            if (objectStateCount < 0 || objectStateCount > 10000) return false;

            ship.HasPlayerPrefix = true;
            ship.PlayerPrefixOffset = offset;
            ship.PlayerPrison = MainPayload[offset] != 0;
            ship.PlayerTalkLocked = MainPayload[offset + 1] != 0;
            ship.PlayerScanLocked = MainPayload[offset + 2] != 0;
            ship.KillShipInHyperSpace = ReadInt32(MainPayload, offset + 3);
            ship.KillShipInHole = ReadInt32(MainPayload, offset + 7);
            for (int index = 0; index < 8; index++)
                ship.KillDominatorsByType[index] = ReadInt32(MainPayload, offset + 11 + index * 4);
            for (int index = 0; index < 3; index++)
                ship.ChameleonLogic[index] = MainPayload[offset + 43 + index];
            ship.PlayerObjectStateCount = objectStateCount;
            return true;
        }

        private bool TryReadTranclucatorShipTail(ShipHeaderRecord ship, int limit)
        {
            int offset = ship.CommonTailEnd;
            if (offset < 0 || offset > limit - 22) return false;
            uint proprietorShipId = ReadUInt32(MainPayload, offset);
            if (proprietorShipId > 100000 || MainPayload[offset + 4] > 1 ||
                MainPayload[offset + 5] > 1 || MainPayload[offset + 6] > 1) return false;
            int cursor = offset + 11;
            string artSystemName;
            if (!TryReadOptionalItemString(ref cursor, out artSystemName)) return false;
            int artStringEnd = cursor;
            if (cursor > limit - 10) return false;
            for (int index = 0; index < 10; index++)
                if (MainPayload[cursor + index] > 1) return false;

            ship.HasTranclucatorTail = true;
            ship.TranclucatorTailOffset = offset;
            ship.TranclucatorArtStringEnd = artStringEnd;
            ship.TranclucatorPostArtOffset = cursor;
            ship.TranclucatorProprietorShipId = proprietorShipId;
            ship.TranclucatorDocking = MainPayload[offset + 4] != 0;
            ship.TranclucatorSeekItems = MainPayload[offset + 5] != 0;
            ship.TranclucatorAutoArrange = MainPayload[offset + 6] != 0;
            ship.TranclucatorArtSize = ReadInt32(MainPayload, offset + 7);
            ship.TranclucatorArtSystemName = artSystemName;
            for (int index = 0; index < 7; index++)
                ship.TranclucatorSeekPermits[index] = MainPayload[cursor + index] != 0;
            ship.TranclucatorLandPermits[0] = MainPayload[cursor + 7] != 0;
            ship.TranclucatorLandPermits[1] = MainPayload[cursor + 8] != 0;
            ship.TranclucatorLandStorage = MainPayload[cursor + 9] != 0;
            return true;
        }

        private bool TryReadRuinsShipTail(ShipHeaderRecord ship, int recordEnd, uint nextObjectId)
        {
            int equipmentOffset = ship.CommonTailEnd;
            if (equipmentOffset < 0 || equipmentOffset > recordEnd - 146) return false;
            ushort equipmentCount = BitConverter.ToUInt16(MainPayload, equipmentOffset);
            if (equipmentCount > 10000) return false;
            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in GalaxyStars) starIds.Add(star.ObjectId);

            int selected = -1;
            int selectedScore = int.MinValue;
            int finalFlags = -1;
            List<ShipItemListEntry> equipmentItems;
            List<ItemHeaderRecord> newlyParsedItems;
            int equipmentEnd;
            if (!TryReadRuinsEquipmentEntries(equipmentOffset + 2, recordEnd, equipmentCount,
                nextObjectId, out equipmentItems, out newlyParsedItems, out equipmentEnd)) return false;
            ItemHeaderRecord saleItem = null;
            for (int candidate = equipmentEnd; candidate <= equipmentEnd; candidate++)
            {
                if (candidate > recordEnd - 147) continue;
                bool valid = true;
                int score = 0;
                for (int good = 0; good < 8 && valid; good++)
                {
                    int row = candidate + good * 16;
                    int count = ReadInt32(MainPayload, row);
                    float hidden = BitConverter.ToSingle(MainPayload, row + 4);
                    int sale = ReadInt32(MainPayload, row + 8);
                    int buy = ReadInt32(MainPayload, row + 12);
                    valid = count >= 0 && count <= 1000000000 && sale >= 0 && sale <= 1000000000 &&
                        buy >= 0 && buy <= 1000000000 && !float.IsNaN(hidden) &&
                        !float.IsInfinity(hidden) && hidden >= 0.0F && hidden <= 1000000000.0F;
                    if (count != 0) score++;
                    if (sale != 0) score++;
                    if (buy != 0) score++;
                    if (hidden >= 0.001F && hidden <= 1000000.0F) score += 10;
                }
                if (!valid) continue;
                int energy = ReadInt32(MainPayload, candidate + 128);
                uint flyToStarId = ReadUInt32(MainPayload, candidate + 132);
                int flyDate = ReadInt32(MainPayload, candidate + 136);
                if (energy < 0 || flyDate < -1 || !starIds.Contains(flyToStarId) && flyToStarId != 0)
                    continue;
                int candidateSaleStart = candidate + 140;
                ItemHeaderRecord candidateSaleItem = null;
                bool saleAlreadyParsed = false;
                foreach (ItemHeaderRecord knownItem in GalaxyItems)
                    if (knownItem.Start == candidateSaleStart)
                    {
                        candidateSaleItem = knownItem;
                        saleAlreadyParsed = true;
                        break;
                    }
                if (candidateSaleItem == null &&
                    (!TryReadItemHeader(candidateSaleStart, nextObjectId,
                        out candidateSaleItem, true, true) ||
                    candidateSaleItem.Type != 73 ||
                    !TryReadKnownItemDerivedTail(candidateSaleItem,
                        candidateSaleItem.SharedPrefixEnd))) continue;
                int candidateFinalFlags = SerializedItemEnd(candidateSaleItem);
                if (candidateSaleItem.Type != 73 || candidateFinalFlags < candidateSaleStart ||
                    candidateFinalFlags > recordEnd - 3 || MainPayload[candidateFinalFlags] > 1 ||
                    MainPayload[candidateFinalFlags + 1] > 1 ||
                    MainPayload[candidateFinalFlags + 2] > 1) continue;
                if (score <= selectedScore) continue;
                selected = candidate;
                selectedScore = score;
                finalFlags = candidateFinalFlags;
                saleItem = candidateSaleItem;
                if (!saleAlreadyParsed) newlyParsedItems.Add(saleItem);
            }
            if (selected < 0 || finalFlags < 0 || saleItem == null) return false;
            int saleStart = selected + 140;
            ShipItemListEntry saleSatellite = new ShipItemListEntry();
            saleSatellite.Start = saleStart; saleSatellite.ItemStart = saleStart;
            saleSatellite.End = finalFlags; saleSatellite.ItemType = saleItem.Type;
            saleSatellite.ItemObjectId = saleItem.ObjectId;

            ship.HasRuinsTail = true;
            ship.RuinsEquipmentCountOffset = equipmentOffset;
            ship.RuinsEquipmentEndOffset = selected;
            ship.RuinsShopTailOffset = selected;
            ship.RuinsFinalFlagsOffset = finalFlags;
            ship.RuinsEquipmentItemCount = equipmentCount;
            ship.RuinsEquipmentItems = equipmentItems;
            ship.RuinsSaleSatellite = saleSatellite;
            for (int good = 0; good < 8; good++)
            {
                int row = selected + good * 16;
                ship.RuinsShopGoods[good, 0] = ReadInt32(MainPayload, row);
                ship.RuinsShopGoods[good, 1] = ReadInt32(MainPayload, row + 8);
                ship.RuinsShopGoods[good, 2] = ReadInt32(MainPayload, row + 12);
            }
            ship.RuinsEnergy = ReadInt32(MainPayload, selected + 128);
            ship.RuinsFlyToStarId = ReadUInt32(MainPayload, selected + 132);
            ship.RuinsFlyDate = ReadInt32(MainPayload, selected + 136);
            ship.RuinsSponsor = MainPayload[finalFlags] != 0;
            ship.RuinsSpecialShip = MainPayload[finalFlags + 1] != 0;
            ship.RuinsNoLanding = MainPayload[finalFlags + 2] != 0;
            ship.RuinsNoShopUpdate = MainPayload[finalFlags + 3];
            foreach (ItemHeaderRecord item in newlyParsedItems)
                GalaxyItems.Add(item);
            return true;
        }

        private bool TryReadRuinsEquipmentEntries(int dataStart, int limit, int count,
            uint nextObjectId,
            out List<ShipItemListEntry> records, out List<ItemHeaderRecord> newlyParsedItems,
            out int dataEnd)
        {
            records = new List<ShipItemListEntry>();
            newlyParsedItems = new List<ItemHeaderRecord>();
            dataEnd = dataStart;
            int cursor = dataStart;
            for (int index = 0; index < count; index++)
            {
                if (cursor >= limit) return false;
                if (records.Count > 0 && records[records.Count - 1].ItemType == 25 && cursor > dataStart)
                {
                    byte adjacentType = MainPayload[cursor - 1];
                    ItemHeaderRecord adjacent = null;
                    foreach (ItemHeaderRecord item in GalaxyItems)
                        if (item.Start == cursor && item.Type == adjacentType) { adjacent = item; break; }
                    if (adjacent != null)
                    {
                        records[records.Count - 1].End--;
                        cursor--;
                    }
                }
                ShipItemListEntry record = new ShipItemListEntry();
                record.Start = cursor;
                record.ItemType = MainPayload[cursor++];
                if (record.ItemType > 75) return false;
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= limit)
                        return false;
                }
                record.ItemStart = cursor;
                ItemHeaderRecord parsed = null;
                foreach (ItemHeaderRecord item in GalaxyItems)
                    if (item.Start == record.ItemStart) { parsed = item; break; }
                if (parsed == null)
                {
                    if (!TryReadItemHeader(record.ItemStart, nextObjectId,
                        out parsed, true) || parsed.Type != record.ItemType ||
                        parsed.Type >= 8 && !TryReadKnownItemDerivedTail(parsed, parsed.SharedPrefixEnd))
                        return false;
                    newlyParsedItems.Add(parsed);
                }
                if (parsed.Type != record.ItemType || record.ItemType == 68 &&
                    parsed.CustomWeaponDiscriminatorOffset != record.Start) return false;
                record.ItemObjectId = parsed.ObjectId;
                record.End = SerializedItemEnd(parsed);
                if (record.End <= record.ItemStart || record.End > limit) return false;
                records.Add(record);
                cursor = record.End;
            }
            dataEnd = cursor;
            return cursor <= limit;
        }

        private bool TrySkipUtf16Z(ref int cursor, int maximumLength, int limit)
        {
            for (int index = 0; index <= maximumLength; index++)
            {
                if (cursor < 0 || cursor > limit - 1 || cursor > MainPayload.Length - 2) return false;
                ushort code = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
                if (code == 0) return true;
            }
            return false;
        }

        private bool TryReadShipCommonTail(ShipHeaderRecord ship, int graphStart, int limit)
        {
            int tail = graphStart - 30;
            if (tail < ship.FixedPrefixEnd || tail > limit - 30 || MainPayload[tail] > 1 ||
                MainPayload[tail + 5] > 7 || MainPayload[tail + 22] > 1 ||
                MainPayload[tail + 23] > 1 || MainPayload[tail + 29] > 1) return false;
            float angle = BitConverter.ToSingle(MainPayload, tail + 1);
            float destinationX = BitConverter.ToSingle(MainPayload, tail + 14);
            float destinationY = BitConverter.ToSingle(MainPayload, tail + 18);
            if (!IsSupportedAsteroidScalar(angle) || !IsSupportedAsteroidScalar(destinationX) ||
                !IsSupportedAsteroidScalar(destinationY)) return false;

            int cursor = graphStart;
            string graphName;
            if (!TryReadObjectString(ref cursor, 128, false, out graphName) ||
                !IsSupportedShipGraphName(graphName))
                return false;
            int graphEnd = cursor;
            if (cursor > limit - 8 || MainPayload[cursor + 1] > 1) return false;
            byte graphTransparency = MainPayload[cursor++];
            bool inHyperSpace = MainPayload[cursor++] != 0;
            float radiusStop = BitConverter.ToSingle(MainPayload, cursor); cursor += 4;
            if (!IsSupportedAsteroidScalar(radiusStop)) return false;
            if (cursor > limit - 2) return false;
            int relationCountOffset = cursor;
            int relationCount = BitConverter.ToUInt16(MainPayload, cursor); cursor += 2;
            if (relationCount > 10000 || cursor > limit - relationCount) return false;
            byte[] relationToRangers = new byte[relationCount];
            if (relationCount != 0)
                Buffer.BlockCopy(MainPayload, cursor, relationToRangers, 0, relationCount);
            cursor += relationCount;
            int relationEndOffset = cursor;
            if (cursor > limit - 1) return false;
            int rewardListOffset = cursor;
            int rewardCount = MainPayload[cursor++];
            if (cursor > limit - rewardCount) return false;
            List<byte> rewards = new List<byte>(rewardCount);
            for (int index = 0; index < rewardCount; index++) rewards.Add(MainPayload[cursor++]);
            int rewardListEnd = cursor;
            int scalar = cursor;
            if (scalar > limit - 517 || MainPayload[scalar] > 1 ||
                MainPayload[scalar + 417] > 1 || MainPayload[scalar + 418] > 2 ||
                MainPayload[scalar + 424] > 1 || MainPayload[scalar + 429] > 1 ||
                MainPayload[scalar + 434] > 1 || MainPayload[scalar + 472] > 1 ||
                MainPayload[scalar + 473] > 6 || MainPayload[scalar + 474] > 1 ||
                MainPayload[scalar + 475] > 1 || MainPayload[scalar + 476] > 1 ||
                MainPayload[scalar + 477] > 1) return false;

            int swarmed = ReadInt32(MainPayload, scalar + 480);
            uint swarmedByShipId = ReadUInt32(MainPayload, scalar + 484);
            List<ShipIllnessRecord> illnesses = new List<ShipIllnessRecord>(25);
            for (int index = 0; index < 24; index++)
            {
                int recordOffset = scalar + 25 + index * 16;
                ShipIllnessRecord record = ReadShipIllnessRecord(recordOffset, index + 1, false);
                if (record == null) return false;
                illnesses.Add(record);
            }
            ShipIllnessRecord stimulator = ReadShipIllnessRecord(scalar + 439, 1, true);
            if (stimulator == null) return false;
            illnesses.Add(stimulator);
            cursor = scalar + 488;
            int swarmAnimationOffset = -1, swarmAnimationEnd = cursor;
            string swarmAnimation = string.Empty;
            if (swarmed > 0)
            {
                swarmAnimationOffset = cursor;
                if (!TryReadObjectString(ref cursor, 128, true, out swarmAnimation)) return false;
                swarmAnimationEnd = cursor;
            }
            if (cursor > limit - 29 || MainPayload[cursor] > 9) return false;
            int finalOffset = cursor;
            float averageEquipment = BitConverter.ToSingle(MainPayload, finalOffset + 9);
            float moneyToCapital = BitConverter.ToSingle(MainPayload, finalOffset + 17);
            float freeSpaceRatio = BitConverter.ToSingle(MainPayload, finalOffset + 21);
            float costlyRatio = BitConverter.ToSingle(MainPayload, finalOffset + 25);
            if (!IsSupportedAsteroidScalar(averageEquipment) || !IsSupportedAsteroidScalar(moneyToCapital) ||
                !IsSupportedAsteroidScalar(freeSpaceRatio) || !IsSupportedAsteroidScalar(costlyRatio)) return false;

            ship.HasCommonTail = true; ship.CommonTailOffset = tail;
            ship.GraphNameOffset = graphStart; ship.GraphNameEnd = graphEnd;
            ship.CommonScalarOffset = scalar; ship.SwarmAnimationOffset = swarmAnimationOffset;
            ship.SwarmAnimationEnd = swarmAnimationEnd; ship.CommonTailEnd = finalOffset + 29;
            ship.Illnesses = illnesses;
            ship.RelationCountOffset = relationCountOffset; ship.RelationEndOffset = relationEndOffset;
            ship.RelationCount = checked((ushort)relationCount); ship.RelationToRangers = relationToRangers;
            ship.RewardListOffset = rewardListOffset; ship.RewardListEndOffset = rewardListEnd;
            ship.Rewards = rewards;
            ship.Forsage = MainPayload[tail] != 0; ship.Angle = angle;
            ship.OrderType = MainPayload[tail + 5]; ship.OrderData = ReadUInt32(MainPayload, tail + 6);
            ship.OrderObjectId = ReadUInt32(MainPayload, tail + 10);
            ship.OrderDestinationX = destinationX; ship.OrderDestinationY = destinationY;
            ship.OrderAbsolute = MainPayload[tail + 22] != 0; ship.Abducted = MainPayload[tail + 23] != 0;
            ship.DaysLanded = ReadInt32(MainPayload, tail + 24);
            ship.ScriptOrderAbsolute = MainPayload[tail + 28]; ship.GraphDominator = MainPayload[tail + 29] != 0;
            ship.GraphName = graphName; ship.GraphShipTransparency = graphTransparency;
            ship.InHyperSpace = inHyperSpace; ship.RadiusStop = radiusStop;
            ship.ShipDestroy = MainPayload[scalar] != 0;
            for (int index = 0; index < 6; index++) ship.Skills[index] = MainPayload[scalar + 1 + index];
            ship.Protoplasm = BitConverter.ToUInt16(MainPayload, scalar + 7);
            ship.Points = ReadUInt32(MainPayload, scalar + 9); ship.FreePoints = ReadUInt32(MainPayload, scalar + 13);
            ship.DayWithoutPlayer = BitConverter.ToUInt16(MainPayload, scalar + 17);
            ship.GroupOrder = BitConverter.ToUInt16(MainPayload, scalar + 23);
            ship.LastNextDay = ReadInt32(MainPayload, scalar + 409);
            ship.ChameleonEnabled = MainPayload[scalar + 417] != 0;
            ship.ChameleonSeries = MainPayload[scalar + 418];
            ship.BlazerChameleonDetect = MainPayload[scalar + 424];
            ship.BlazerChameleonCharge = ReadInt32(MainPayload, scalar + 425);
            ship.KellerChameleonDetect = MainPayload[scalar + 429];
            ship.KellerChameleonCharge = ReadInt32(MainPayload, scalar + 430);
            ship.TerronChameleonDetect = MainPayload[scalar + 434];
            ship.TerronChameleonCharge = ReadInt32(MainPayload, scalar + 435);
            ship.TechLevelKnowledge = MainPayload[scalar + 455];
            ship.TradePenalty = ReadInt32(MainPayload, scalar + 456);
            ship.TradePoints = ReadInt32(MainPayload, scalar + 460);
            ship.ContrabandPoints = ReadInt32(MainPayload, scalar + 464);
            ship.RewardViewCount = ReadInt32(MainPayload, scalar + 468);
            ship.NoDrop = MainPayload[scalar + 472] != 0; ship.NoTarget = MainPayload[scalar + 473];
            ship.NoTalk = MainPayload[scalar + 474] != 0; ship.NoScan = MainPayload[scalar + 475] != 0;
            ship.ScriptChameleon = MainPayload[scalar + 476] != 0;
            ship.RobbedByPlayer = MainPayload[scalar + 477] != 0;
            ship.CountOfDeflectedPlayerShots = BitConverter.ToUInt16(MainPayload, scalar + 478);
            ship.Swarmed = swarmed; ship.SwarmedByShipId = swarmedByShipId;
            ship.SwarmAnimation = swarmAnimation; ship.CurrentStanding = MainPayload[finalOffset];
            ship.AverageSpeed = ReadInt32(MainPayload, finalOffset + 1);
            ship.AverageEnemySpeed = ReadInt32(MainPayload, finalOffset + 5);
            ship.AverageEquipmentValue = averageEquipment;
            ship.AverageCapital = ReadInt32(MainPayload, finalOffset + 13);
            ship.AverageMoneyToCapital = moneyToCapital;
            ship.AverageFreeSpaceRatio = freeSpaceRatio;
            ship.RatioOfTooCostlyEquipmentInShop = costlyRatio;
            return true;
        }

        private ShipIllnessRecord ReadShipIllnessRecord(int offset, int index, bool stimulator)
        {
            float infection = BitConverter.ToSingle(MainPayload, offset);
            if (!IsSupportedAsteroidScalar(infection)) return null;
            ShipIllnessRecord record = new ShipIllnessRecord();
            record.Start = offset;
            record.Index = index;
            record.Stimulator = stimulator;
            record.Infection = infection;
            record.InfectionDay = ReadInt32(MainPayload, offset + 4);
            record.InfectionEndDay = ReadInt32(MainPayload, offset + 8);
            record.InfectionCount = ReadInt32(MainPayload, offset + 12);
            return record;
        }

        private static bool IsSupportedShipGraphName(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3 || value.Length > 128) return false;
            foreach (char character in value)
                if (character < 0x21 || character > 0x7E) return false;
            return true;
        }

        private static bool ContainsCyrillic(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char character in value)
                if (character >= '\u0400' && character <= '\u052F') return true;
            return false;
        }

        private static bool IsPreferredShipGraphName(string value)
        {
            return value == "Usl_74" || value.StartsWith("Ship.", StringComparison.Ordinal) ||
                value.StartsWith("Ruins.", StringComparison.Ordinal);
        }

        private bool TryReadObjectString(ref int offset, int maximumLength, bool allowEmpty, out string value)
        {
            value = null;
            StringBuilder text = new StringBuilder();
            for (int index = 0; index <= maximumLength; index++)
            {
                if (offset < 0 || offset > MainPayload.Length - 2) return false;
                int codeUnit = MainPayload[offset] | MainPayload[offset + 1] << 8;
                offset += 2;
                if (codeUnit == 0)
                {
                    if (!allowEmpty && text.Length == 0) return false;
                    value = text.ToString();
                    return true;
                }
                char character = (char)codeUnit;
                if (!IsSupportedObjectTextCharacter(character)) return false;
                text.Append(character);
            }
            return false;
        }

        private bool TryReadStreamText(ref int offset, int maximumLength, int limit, out string value)
        {
            value = null;
            StringBuilder text = new StringBuilder();
            for (int index = 0; index <= maximumLength; index++)
            {
                if (offset < 0 || offset > limit - 2 || offset > MainPayload.Length - 2) return false;
                char character = (char)(MainPayload[offset] | MainPayload[offset + 1] << 8);
                offset += 2;
                if (character == '\0')
                {
                    value = text.ToString();
                    return true;
                }
                if (char.IsControl(character) && character != '\r' && character != '\n' &&
                    character != '\t') return false;
                text.Append(character);
            }
            return false;
        }

        private void ParseItemHeaders(GalaxySummaryData summary)
        {
            if (summary.NextObjectId < 2 || summary.NextObjectId > 10000000)
                throw new SavFormatException("TItem: недопустимый TGalaxy.NextObjectId " + summary.NextObjectId + ".");

            List<ItemHeaderRecord> candidates = new List<ItemHeaderRecord>();
            for (int start = GalaxyStarsOffset + 1; start <= MainPayload.Length - 31; start++)
            {
                ItemHeaderRecord candidate;
                if (TryReadItemHeader(start, summary.NextObjectId, out candidate))
                    candidates.Add(candidate);
            }

            HashSet<int> exactStarts = new HashSet<int>();
            List<KeyValuePair<int, int>> exactRanges = new List<KeyValuePair<int, int>>();
            foreach (ItemHeaderRecord item in GalaxyItems)
            {
                exactStarts.Add(item.Start);
                exactRanges.Add(new KeyValuePair<int, int>(item.Start, SerializedItemEnd(item)));
            }

            HashSet<int> nonZeroStarts = new HashSet<int>();
            HashSet<int> nonZeroPrefixBytes = new HashSet<int>();
            foreach (ItemHeaderRecord candidate in candidates)
                if (candidate.Type > 0)
                {
                    nonZeroStarts.Add(candidate.Start);
                    for (int offset = candidate.Start; offset < candidate.SharedPrefixEnd; offset++)
                        nonZeroPrefixBytes.Add(offset);
                }

            // TItem identifiers are local to the concrete serialized item class.
            // The original factory selects that class from Type first, therefore
            // goods, weapons and other equipment may legally share ObjectId.
            Dictionary<ulong, ItemHeaderRecord> objectIds = new Dictionary<ulong, ItemHeaderRecord>();
            foreach (ItemHeaderRecord existing in GalaxyItems)
                objectIds[((ulong)existing.Type << 32) | existing.ObjectId] = existing;
            foreach (ItemHeaderRecord candidate in candidates)
            {
                if (exactStarts.Contains(candidate.Start)) continue;
                bool overlapsExact = false;
                foreach (KeyValuePair<int, int> range in exactRanges)
                    if (candidate.Start < range.Value && candidate.SharedPrefixEnd > range.Key)
                    {
                        overlapsExact = true;
                        break;
                    }
                if (overlapsExact) continue;

                // Goods records have an all-zero factory discriminator and are
                // especially prone to false positives inside numeric scalar
                // streams. Every safely editable goods item is reached through
                // an exact owner collection above; never invent an unowned one
                // from a byte-pattern scan.
                if (candidate.Type < 8) continue;
                // TCustomWeapon has an additional factory name wrapper and every
                // real editable instance is likewise reached through an exact
                // owner collection.  Descriptor-shaped bytes in opaque tails must
                // not create an orphan weapon in the global item catalogue.
                if (candidate.Type == 68) continue;

                // Equipment prefixes contain many zeroes and can mechanically resemble
                // a zero-valued TGoodsItem. A real serialized object cannot overlap a
                // second TItem prefix, so retain the exact non-zero route in that case.
                if (candidate.Type == 0)
                {
                    bool overlaps = nonZeroPrefixBytes.Contains(candidate.Start);
                    for (int offset = candidate.Start + 1; !overlaps && offset < candidate.SharedPrefixEnd; offset++)
                        overlaps = nonZeroStarts.Contains(offset);
                    if (overlaps) continue;
                }
                ulong identity = ((ulong)candidate.Type << 32) | candidate.ObjectId;
                ItemHeaderRecord previous;
                if (objectIds.TryGetValue(identity, out previous))
                {
                    if (previous.Start == candidate.Start && previous.Type == candidate.Type)
                        continue;
                    if (previous.Type == 0 && candidate.Type == 0) continue;
                    throw new SavFormatException("TItem: type " + candidate.Type + ", object id " +
                        candidate.ObjectId + " имеет несколько структурных заголовков: 0x" +
                        previous.Start.ToString("X") + " и 0x" + candidate.Start.ToString("X") + ".");
                }
                objectIds.Add(identity, candidate);
                GalaxyItems.Add(candidate);
            }
            if (GalaxyItems.Count == 0)
                throw new SavFormatException("TItem: структурный проход не нашёл ни одного предмета.");
            foreach (ItemHeaderRecord item in GalaxyItems)
                if (item.Type >= 8 && !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd))
                    throw new SavFormatException("TItem: производный хвост type " + item.Type +
                        " / id " + item.ObjectId + " @ 0x" + item.SharedPrefixEnd.ToString("X") +
                        " повреждён: " + BitConverter.ToString(MainPayload, item.SharedPrefixEnd,
                            Math.Min(96, MainPayload.Length - item.SharedPrefixEnd)) + ".");
        }

        private void ParseShipPreCommonCollections(GalaxySummaryData summary)
        {
            // The fixed-prefix scan is deliberately broad enough to recover modded
            // ships, so arbitrary payload bytes can occasionally satisfy it.  A real
            // top-level ship must also expose the common serialized tail; discard
            // header-only lookalikes before treating them as collection owners.
            GalaxyShips.RemoveAll(delegate(ShipHeaderRecord ship)
            {
                return !ship.HasCommonTail ||
                    !TryScanShipPreCommonPrefix(ship, summary.NextObjectId);
            });
            int remainingPlayers = 0;
            ShipCount = 0;
            StationCount = 0;
            foreach (ShipHeaderRecord ship in GalaxyShips)
            {
                if (ship.IsPlayer) remainingPlayers++;
                if (ship.IsStation) StationCount++; else ShipCount++;
            }
            if (remainingPlayers != 1)
                throw new SavFormatException(
                    "TShip: после проверки общего хвоста не найден единственный корабль игрока.");
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            List<ShipHeaderRecord> collectionShips = new List<ShipHeaderRecord>(GalaxyShips);
            foreach (ShipHeaderRecord player in GalaxyShips)
                if (player.IsPlayer && player.HasPlayerBridge && player.PlayerBridgeRuins != null)
                    collectionShips.Add(player.PlayerBridgeRuins);
            HashSet<int> collectionShipStarts = new HashSet<int>();
            foreach (ShipHeaderRecord ship in collectionShips) collectionShipStarts.Add(ship.Start);
            foreach (ItemHeaderRecord item in GalaxyItems)
                if (item.NestedTranclucator != null &&
                    collectionShipStarts.Add(item.NestedTranclucator.Start))
                    collectionShips.Add(item.NestedTranclucator);
            // Nested TArtefactTranclucator objects can first be discovered while
            // traversing an outer ship list.  Grow this work queue as new exact
            // owner-linked items appear so every embedded TShip receives the same
            // collection parse and all of its child TItems enter the global catalog.
            for (int collectionIndex = 0; collectionIndex < collectionShips.Count;
                collectionIndex++)
            {
                ShipHeaderRecord ship = collectionShips[collectionIndex];
                if (!ship.HasCommonTail)
                    throw new SavFormatException("TShip " + ship.ObjectId + ": общий хвост не локализован.");
                int cursor = ship.FixedPrefixEnd;
                ship.EquipmentListStart = cursor;
                if (!TryReadShipItemEntries(ref cursor, ship.CommonTailOffset,
                    ship.EquipmentItemCount, summary.NextObjectId, itemsByStart, out ship.EquipmentItems))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": список оборудования повреждён" +
                        " (count=" + ship.EquipmentItemCount + ", start=0x" + ship.FixedPrefixEnd.ToString("X") +
                        ", tail=0x" + ship.CommonTailOffset.ToString("X") + ", cursor=0x" + cursor.ToString("X") +
                        ", bytes=" + BitConverter.ToString(MainPayload, ship.FixedPrefixEnd,
                            Math.Min(96, ship.CommonTailOffset - ship.FixedPrefixEnd)) + ", near=" +
                        BitConverter.ToString(MainPayload, Math.Max(ship.FixedPrefixEnd, cursor - 16),
                            Math.Min(96, ship.CommonTailOffset - Math.Max(ship.FixedPrefixEnd, cursor - 16))) + ").");

                ship.ArtefactCountOffset = cursor;
                int artefactCount;
                if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out artefactCount) ||
                    !TryReadShipItemEntries(ref cursor, ship.CommonTailOffset,
                        artefactCount, summary.NextObjectId, itemsByStart, out ship.ArtefactItems))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": список артефактов повреждён" +
                        " (count=" + artefactCount + ", countAt=0x" + ship.ArtefactCountOffset.ToString("X") +
                        ", cursor=0x" + cursor.ToString("X") + ", tail=0x" +
                        ship.CommonTailOffset.ToString("X") + ", parsed=" +
                        string.Join("; ", ship.ArtefactItems.ConvertAll<string>(
                            delegate(ShipItemListEntry entry) { return entry.ToString(); }).ToArray()) + ", near=" +
                        BitConverter.ToString(MainPayload, Math.Max(ship.ArtefactCountOffset, cursor - 16),
                            Math.Min(128, ship.CommonTailOffset -
                                Math.Max(ship.ArtefactCountOffset, cursor - 16))) + ").");

                ship.DropListCountOffset = cursor;
                int dropCount;
                if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out dropCount) ||
                    !TryReadShipItemEntries(ref cursor, ship.CommonTailOffset,
                        dropCount, summary.NextObjectId, itemsByStart, out ship.DropListItems))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": список выбрасываемых предметов повреждён.");

                ship.SpecialBonusCountOffset = cursor;
                int bonusCount;
                if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out bonusCount))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": число специальных бонусов повреждено.");
                ship.SpecialBonuses = new List<ShipSpecialBonusRecord>();
                for (int index = 0; index < bonusCount; index++)
                {
                    if (cursor > ship.CommonTailOffset - 5)
                        throw new SavFormatException("TShip " + ship.ObjectId + ": специальный бонус обрезан.");
                    ShipSpecialBonusRecord record = new ShipSpecialBonusRecord();
                    record.Start = cursor; record.BonusType = MainPayload[cursor++];
                    record.Value = ReadInt32(MainPayload, cursor); cursor += 4; record.End = cursor;
                    ship.SpecialBonuses.Add(record);
                }

                ship.StatusEffectCountOffset = cursor;
                int effectCount;
                if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out effectCount))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": число статус-эффектов повреждено.");
                ship.StatusEffects = new List<ShipStatusEffectRecord>();
                for (int index = 0; index < effectCount; index++)
                {
                    if (cursor > ship.CommonTailOffset - 9)
                        throw new SavFormatException("TShip " + ship.ObjectId + ": статус-эффект обрезан.");
                    ShipStatusEffectRecord record = new ShipStatusEffectRecord();
                    record.Start = cursor; record.EffectType = MainPayload[cursor++];
                    record.Value = BitConverter.ToSingle(MainPayload, cursor); cursor += 4;
                    record.LastSourceShipId = ReadUInt32(MainPayload, cursor); cursor += 4; record.End = cursor;
                    if (float.IsNaN(record.Value) || float.IsInfinity(record.Value))
                        throw new SavFormatException("TShip " + ship.ObjectId + ": статус-эффект содержит нечисловое значение.");
                    ship.StatusEffects.Add(record);
                }

                ship.CustomShipInfoCountOffset = cursor;
                if (cursor > ship.CommonTailOffset - 4)
                    throw new SavFormatException("TShip " + ship.ObjectId + ": число TCustomShipInfo обрезано.");
                int customCount = ReadInt32(MainPayload, cursor); cursor += 4;
                if (customCount < 0 || customCount > 10000)
                    throw new SavFormatException("TShip " + ship.ObjectId + ": неверное число TCustomShipInfo.");
                ship.CustomShipInfos = new List<CustomShipInfoRecord>();
                for (int index = 0; index < customCount; index++)
                {
                    CustomShipInfoRecord record = new CustomShipInfoRecord(); record.Start = cursor;
                    if (!TryReadItemString(ref cursor, 32768, out record.Name) || cursor > ship.CommonTailOffset ||
                        !TryReadItemString(ref cursor, 32768, out record.Description) || cursor > ship.CommonTailOffset - 12)
                        throw new SavFormatException("TShip " + ship.ObjectId + ": TCustomShipInfo обрезан" +
                            " (index=" + index + "/" + customCount + ", cursor=0x" + cursor.ToString("X") +
                            ", countAt=0x" + ship.CustomShipInfoCountOffset.ToString("X") +
                            ", tail=0x" + ship.CommonTailOffset.ToString("X") + ", bytes=" +
                            BitConverter.ToString(MainPayload, ship.CustomShipInfoCountOffset,
                                Math.Min(160, ship.CommonTailOffset - ship.CustomShipInfoCountOffset)) + ").");
                    record.Data1 = ReadInt32(MainPayload, cursor); cursor += 4;
                    record.Data2 = ReadInt32(MainPayload, cursor); cursor += 4;
                    record.Data3 = ReadInt32(MainPayload, cursor); cursor += 4;
                    if (!TryReadItemString(ref cursor, 32768, out record.TextData1) || cursor > ship.CommonTailOffset ||
                        !TryReadItemString(ref cursor, 32768, out record.TextData2) || cursor > ship.CommonTailOffset ||
                        !TryReadItemString(ref cursor, 32768, out record.TextData3) || cursor > ship.CommonTailOffset)
                        throw new SavFormatException("TShip " + ship.ObjectId + ": текст TCustomShipInfo обрезан" +
                            " (index=" + index + "/" + customCount + ", cursor=0x" + cursor.ToString("X") +
                            ", countAt=0x" + ship.CustomShipInfoCountOffset.ToString("X") +
                            ", tail=0x" + ship.CommonTailOffset.ToString("X") + ", bytes=" +
                            BitConverter.ToString(MainPayload, ship.CustomShipInfoCountOffset,
                                Math.Min(160, ship.CommonTailOffset - ship.CustomShipInfoCountOffset)) + ").");
                    record.End = cursor; ship.CustomShipInfos.Add(record);
                }

                ship.TakeItemReferenceCountOffset = cursor;
                if (!TryReadShipReferenceList(ref cursor, ship.CommonTailOffset, out ship.TakeItemReferenceIds))
                    throw new SavFormatException("TShip " + ship.ObjectId + ": список TakeItems повреждён.");
                ship.RecentlyDroppedItemCountOffset = cursor;
                if (!TryReadShipReferenceList(ref cursor, ship.CommonTailOffset, out ship.RecentlyDroppedItemIds))
                    throw new SavFormatException("TShip " + ship.ObjectId +
                        ": список RecentlyDroppedItems повреждён.");
                if (cursor > ship.CommonTailOffset - 12)
                    throw new SavFormatException("TShip " + ship.ObjectId + ": ссылки партнёров обрезаны.");
                ship.GoodShipId = ReadUInt32(MainPayload, cursor); cursor += 4;
                ship.BadShipId = ReadUInt32(MainPayload, cursor); cursor += 4;
                ship.PartnerShipId = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (ship.GoodShipId > 10000000 || ship.BadShipId > 10000000 || ship.PartnerShipId > 10000000)
                    throw new SavFormatException("TShip " + ship.ObjectId + ": ссылка партнёра вне диапазона.");
                ship.PartnerGood = 0;
                if (ship.PartnerShipId != 0)
                {
                    if (cursor > ship.CommonTailOffset - 4)
                        throw new SavFormatException("TShip " + ship.ObjectId + ": отношение партнёра обрезано.");
                    ship.PartnerGood = ReadInt32(MainPayload, cursor); cursor += 4;
                }
                ship.PreCommonTailEnd = cursor;
                if (cursor != ship.CommonTailOffset)
                    throw new SavFormatException("TShip " + ship.ObjectId +
                        ": последовательный префикс закончился в 0x" + cursor.ToString("X") +
                        ", общий хвост начинается в 0x" + ship.CommonTailOffset.ToString("X") + ".");
                ship.HasPreCommonCollections = true;

                foreach (ItemHeaderRecord item in GalaxyItems)
                    if (item.NestedTranclucator != null &&
                        collectionShipStarts.Add(item.NestedTranclucator.Start))
                        collectionShips.Add(item.NestedTranclucator);
            }

            // The initial byte scan is intentionally permissive so items can be
            // found before their owning collections are known. Long runs of
            // zero-valued TShip scalars can mechanically resemble several
            // one-byte-shifted type-0 TGoodsItem headers. Once the exact three
            // pre-common lists are traversed, only their proven item starts are
            // valid inside the common TShip record.
            HashSet<int> provenShipItemStarts = new HashSet<int>();
            foreach (ShipHeaderRecord ship in collectionShips)
            {
                foreach (ShipItemListEntry entry in ship.EquipmentItems) provenShipItemStarts.Add(entry.ItemStart);
                foreach (ShipItemListEntry entry in ship.ArtefactItems) provenShipItemStarts.Add(entry.ItemStart);
                foreach (ShipItemListEntry entry in ship.DropListItems) provenShipItemStarts.Add(entry.ItemStart);
            }
            for (int itemIndex = GalaxyItems.Count - 1; itemIndex >= 0; itemIndex--)
            {
                ItemHeaderRecord item = GalaxyItems[itemIndex];
                if (provenShipItemStarts.Contains(item.Start)) continue;
                foreach (ShipHeaderRecord ship in collectionShips)
                    if (item.Start >= ship.Start && item.Start < ship.CommonTailEnd)
                    {
                        GalaxyItems.RemoveAt(itemIndex);
                        break;
                    }
            }
        }

        private bool TryReadShipItemEntries(ref int cursor, int limit, int count, uint nextObjectId,
            Dictionary<int, ItemHeaderRecord> itemsByStart, out List<ShipItemListEntry> records)
        {
            records = new List<ShipItemListEntry>();
            if (count < 0 || count > 10000) return false;
            for (int index = 0; index < count; index++)
            {
                if (cursor >= limit) return false;
                // The nested TTranclucator tail scanner includes one runtime
                // flag that TArtefactTranclucator.Write does not retain in this route.
                // An immediately following factory discriminator and valid TItem header
                // provide an exact one-byte boundary correction.
                if (records.Count > 0 && records[records.Count - 1].ItemType == 25 && cursor > 0)
                {
                    ItemHeaderRecord adjacent;
                    byte adjacentType = MainPayload[cursor - 1];
                    if (adjacentType <= 75 && TryReadItemHeader(cursor, nextObjectId, out adjacent, true) &&
                        adjacent.Type == adjacentType)
                    {
                        ShipItemListEntry previous = records[records.Count - 1];
                        previous.End--;
                        ItemHeaderRecord previousItem;
                        if (itemsByStart.TryGetValue(previous.ItemStart, out previousItem) &&
                            previousItem.Type == 25 && previousItem.DerivedTailEnd == cursor)
                            previousItem.DerivedTailEnd--;
                        cursor--;
                    }
                }
                ShipItemListEntry record = new ShipItemListEntry();
                record.Start = cursor; record.ItemType = MainPayload[cursor++];
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= limit) return false;
                }
                record.ItemStart = cursor;
                ItemHeaderRecord item;
                if (!itemsByStart.TryGetValue(record.ItemStart, out item))
                {
                    if (!TryReadItemHeader(record.ItemStart, nextObjectId, out item, true) ||
                        item.Type != record.ItemType ||
                        item.Type >= 8 && !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd)) return false;
                    itemsByStart.Add(item.Start, item);
                    GalaxyItems.Add(item);
                }
                if (item.Type != record.ItemType) return false;
                if (record.ItemType == 68 && item.CustomWeaponDiscriminatorOffset != record.Start) return false;
                record.ItemObjectId = item.ObjectId; record.End = SerializedItemEnd(item);
                if (record.End <= record.ItemStart || record.End > limit) return false;
                records.Add(record); cursor = record.End;
            }
            return true;
        }

        private bool TryReadShipListCount(ref int cursor, int limit, out int count)
        {
            count = 0;
            if (cursor > limit - 2) return false;
            count = MainPayload[cursor] | MainPayload[cursor + 1] << 8; cursor += 2;
            return count <= 10000;
        }

        private bool TryReadShipReferenceList(ref int cursor, int limit, out List<uint> values)
        {
            values = new List<uint>();
            int count;
            if (!TryReadShipListCount(ref cursor, limit, out count) || cursor > limit - count * 4) return false;
            for (int index = 0; index < count; index++)
            {
                uint value = ReadUInt32(MainPayload, cursor); cursor += 4;
                if (value > 10000000) return false;
                values.Add(value);
            }
            return true;
        }

        private void ParseStoredItems(GalaxySummaryData summary)
        {
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            int minimum = GalaxyStarsOffset;
            foreach (HoleRecord hole in GalaxyHoles)
                if (hole.End > minimum) minimum = hole.End;

            // StoredItems follow the final THole record. This structural route
            // distinguishes empty and singleton lists without byte-pattern guesses.
            if (HoleListEndOffset >= 0)
            {
                List<StoredItemRecord> exact;
                int exactEnd;
                if (!TryReadStoredItemList(HoleListEndOffset, summary.TurnOffset, itemsByStart,
                    out exact, out exactEnd))
                    throw new SavFormatException("TStoredItem: список после THole повреждён @ 0x" +
                        HoleListEndOffset.ToString("X") + ".");
                HasExactStoredItemList = true;
                StoredItemCountOffset = HoleListEndOffset;
                StoredItemListEndOffset = exactEnd;
                StoredItems.AddRange(exact);
                return;
            }

            List<StoredItemRecord> selected = null;
            int selectedOffset = -1;
            int selectedEnd = -1;
            int selectedCount = 0;
            List<string> candidateDetails = new List<string>();
            for (int candidate = minimum; candidate <= summary.TurnOffset - 4; candidate++)
            {
                int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                // A single wrapper is indistinguishable from nested item contexts without
                // the complete preceding TStar traversal. Current verified SAVs use a
                // multi-record storage list; a possible singleton stays byte-preserved.
                if (count < 2 || count > 10000) continue;
                int cursor;
                List<StoredItemRecord> records;
                if (!TryReadStoredItemList(candidate, summary.TurnOffset, itemsByStart,
                    out records, out cursor)) continue;
                selected = records;
                selectedOffset = candidate;
                selectedEnd = cursor;
                selectedCount++;
                candidateDetails.Add("0x" + candidate.ToString("X") + ":" + count + "->0x" + cursor.ToString("X"));
            }
            if (selectedCount > 1)
                throw new SavFormatException("TStoredItem: ожидался один структурный список, найдено " +
                    selectedCount + " (" + string.Join(", ", candidateDetails.ToArray()) + ").");
            if (selected != null)
            {
                HasExactStoredItemList = true;
                StoredItemCountOffset = selectedOffset;
                StoredItemListEndOffset = selectedEnd;
                StoredItems.AddRange(selected);
            }
        }

        private bool TryReadStoredItemList(int start, int limit,
            Dictionary<int, ItemHeaderRecord> itemsByStart,
            out List<StoredItemRecord> records, out int end)
        {
            records = null; end = start;
            if (start < 0 || start > limit - 2) return false;
            int count = MainPayload[start] | MainPayload[start + 1] << 8;
            if (count > 10000) return false;
            int cursor = start + 2;
            List<StoredItemRecord> parsed = new List<StoredItemRecord>(count);
            for (int index = 0; index < count; index++)
            {
                StoredItemRecord record = new StoredItemRecord();
                record.Start = cursor;
                if (!TryReadObjectString(ref cursor, 256, false, out record.ScriptTag) || cursor >= limit)
                    return false;
                record.ItemTypeOffset = cursor;
                record.ItemType = MainPayload[cursor++];
                if (record.ItemType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref cursor, 512, out customWeaponName) || cursor >= limit)
                        return false;
                }
                ItemHeaderRecord item;
                if (!itemsByStart.TryGetValue(cursor, out item) || item.Type != record.ItemType)
                    return false;
                if (record.ItemType == 68 &&
                    item.CustomWeaponDiscriminatorOffset != record.ItemTypeOffset) return false;
                record.ItemStart = item.Start;
                record.ItemObjectId = item.ObjectId;
                cursor = SerializedItemEnd(item);
                if (cursor <= item.Start || cursor > limit) return false;
                record.End = cursor;
                parsed.Add(record);
            }
            records = parsed; end = cursor; return true;
        }

        private void LocateGateRecords(GalaxySummaryData summary)
        {
            int end = summary.PlanetReferenceListOffset;
            if (end < 2) throw new SavFormatException("TGate: недопустимая граница списка планет.");
            int minimum = GalaxyStarsOffset;
            foreach (HoleRecord hole in GalaxyHoles) if (hole.End > minimum) minimum = hole.End;
            foreach (StoredItemRecord stored in StoredItems) if (stored.End > minimum) minimum = stored.End;

            int selectedOffset = -1;
            List<GateRecord> selected = null;
            int selectedCount = 0;
            for (int candidate = minimum; candidate <= end - 13; candidate++)
            {
                int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                if (count < 1 || count > 10000) continue;
                List<GateRecord> records;
                if (!TryReadGateList(candidate, end, out records)) continue;
                selectedOffset = candidate;
                selected = records;
                selectedCount++;
            }
            if (selectedCount > 1)
                throw new SavFormatException("TGate: найдено несколько структурных маршрутов: " + selectedCount + ".");
            if (selectedCount == 1)
            {
                summary.GateListOffset = selectedOffset;
                summary.Gates.AddRange(selected);
                return;
            }
            int emptyOffset = end - 2;
            if ((MainPayload[emptyOffset] | MainPayload[emptyOffset + 1] << 8) != 0)
                throw new SavFormatException("TGate: не найден ни непустой, ни пустой список.");
            summary.GateListOffset = emptyOffset;
        }

        private bool TryReadGateList(int start, int expectedEnd, out List<GateRecord> records)
        {
            records = null;
            int offset = start;
            if (offset > expectedEnd - 2) return false;
            int count = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (count < 1 || count > 10000) return false;
            List<GateRecord> parsed = new List<GateRecord>(count);
            for (int index = 0; index < count; index++)
            {
                if (offset > expectedEnd - 11) return false;
                GateRecord record = new GateRecord();
                record.Start = offset;
                record.X = BitConverter.ToSingle(MainPayload, offset);
                record.Y = BitConverter.ToSingle(MainPayload, offset + 4);
                record.Angle = MainPayload[offset + 8];
                record.Size = (ushort)(MainPayload[offset + 9] | MainPayload[offset + 10] << 8);
                if (!IsFiniteGalaxyScalar(record.X) || !IsFiniteGalaxyScalar(record.Y) ||
                    Math.Abs((double)record.X) > 10000 || Math.Abs((double)record.Y) > 10000 ||
                    record.Size > 10000) return false;
                offset += 11;
                if (!TryReadItemString(ref offset, 4096, out record.Text)) return false;
                record.End = offset;
                parsed.Add(record);
            }
            if (offset != expectedEnd) return false;
            records = parsed;
            return true;
        }

        private static int SerializedItemEnd(ItemHeaderRecord item)
        {
            if (item.HasDerivedTail && item.DerivedTailEnd > item.Start) return item.DerivedTailEnd;
            if (item.HasGoodsTail && item.GoodsTailOffset > item.Start) return item.GoodsTailOffset + 5;
            return item.SharedPrefixEnd;
        }

        private bool TryReadItemHeader(int start, uint nextObjectId, out ItemHeaderRecord value,
            bool allowUnboundedWeight = false, bool standaloneWithoutDiscriminator = false)
        {
            value = null;
            if (start < 1 || start > MainPayload.Length - 31) return false;
            uint objectId = ReadUInt32(MainPayload, start);
            byte type = MainPayload[start + 4];
            if (objectId == 0 || objectId >= nextObjectId || type > 75) return false;
            string customWeaponName = string.Empty;
            int customWeaponDiscriminatorOffset = -1;
            if (type == 68)
            {
                if (!TryGetCustomWeaponDiscriminator(start, out customWeaponName,
                    out customWeaponDiscriminatorOffset)) return false;
            }
            else if (!standaloneWithoutDiscriminator && MainPayload[start - 1] != type) return false;

            float x = BitConverter.ToSingle(MainPayload, start + 5);
            float y = BitConverter.ToSingle(MainPayload, start + 9);
            if (allowUnboundedWeight)
            {
                if (!IsSupportedMissileScalar(x) || !IsSupportedMissileScalar(y)) return false;
            }
            else if (!IsSupportedObjectCoordinate(x) || !IsSupportedObjectCoordinate(y)) return false;
            int weight = ReadInt32(MainPayload, start + 13);
            if (!allowUnboundedWeight && (weight < 0 || weight > 1000000000)) return false;

            int offset = start + 26;
            byte namePresent = MainPayload[offset++];
            if (namePresent > 1) return false;
            string name = string.Empty;
            if (namePresent != 0 && !TryReadItemString(ref offset, 512, out name)) return false;
            if (offset >= MainPayload.Length) return false;
            byte noDrop = MainPayload[offset++];

            int sharedPrefixEnd = offset;
            EquipmentPrefixData equipment = null;
            if (type < 8)
            {
                if (offset > MainPayload.Length - 5) return false;
                int goodsCount = ReadInt32(MainPayload, offset);
                if (weight < 0 || weight > 10000 || goodsCount != weight || MainPayload[offset + 4] > 1)
                    return false;
                sharedPrefixEnd = offset + 5;
            }
            else if (!TryReadEquipmentPrefix(ref sharedPrefixEnd, out equipment)) return false;

            ItemHeaderRecord item = new ItemHeaderRecord();
            item.Start = start;
            item.BaseEnd = offset;
            item.SharedPrefixEnd = sharedPrefixEnd;
            item.ObjectId = objectId;
            item.Type = type;
            item.X = x;
            item.Y = y;
            item.Weight = weight;
            item.Owner = MainPayload[start + 17];
            item.Cost = ReadUInt32(MainPayload, start + 18);
            item.ItemDestroy = ReadInt32(MainPayload, start + 22);
            item.Name = name;
            item.NoDrop = noDrop;
            item.CustomFaction = equipment == null ? string.Empty : equipment.CustomFaction;
            item.SystemName = equipment == null ? string.Empty : equipment.SystemName;
            if (equipment != null)
            {
                item.EquipmentFirstStringEnd = equipment.FirstStringEnd;
                item.EquipmentSecondStringEnd = equipment.SecondStringEnd;
                item.EquipmentScalarOffset = equipment.ScalarOffset;
                item.Exploitable = equipment.Exploitable;
                item.Strength = equipment.Strength;
                item.Broken = equipment.Broken;
                item.Slot = equipment.Slot;
                item.BonusOffset = equipment.BonusOffset;
                item.BonusEnd = equipment.BonusEnd;
                item.Bonus = equipment.Bonus;
                item.BonusReferenceId = equipment.BonusReferenceId;
                item.SpecialOffset = equipment.SpecialOffset;
                item.SpecialEnd = equipment.SpecialEnd;
                item.Special = equipment.Special;
                item.SpecialReferenceId = equipment.SpecialReferenceId;
                item.ExtraSpecialCountOffset = equipment.ExtraSpecialCountOffset;
                item.ExtraSpecialEnd = equipment.ExtraSpecialEnd;
                item.ExtraSpecials = equipment.ExtraSpecials;
                item.DominatorSeries = equipment.DominatorSeries;
            }
            item.CustomWeaponName = customWeaponName;
            item.CustomWeaponDiscriminatorOffset = customWeaponDiscriminatorOffset;
            if (type < 8)
            {
                item.HasGoodsTail = true;
                item.GoodsTailOffset = offset;
                item.GoodsItemCount = ReadInt32(MainPayload, offset);
                item.GoodsItemNatural = MainPayload[offset + 4] != 0;
            }
            value = item;
            return true;
        }

        private bool TryReadKnownItemDerivedTail(ItemHeaderRecord item, int offset)
        {
            if (item.HasDerivedTail)
                return item.DerivedTailOffset == offset && item.DerivedTailEnd >= offset;
            // A TItem can be reached through several owner indices.  Parsing the same
            // physical record again must not append a second copy of its derived fields.
            item.DerivedFields = null;
            int start = offset;
            switch (item.Type)
            {
                case 8:
                case 9:
                    for (int index = 1; index <= 3; index++)
                        if (!TryAddItemInt32(item, ref offset, "edCustomArtData" + index)) return false;
                    for (int index = 1; index <= 3; index++)
                        if (!TryAddItemOptionalString(item, ref offset, "edCustomArtTextData" + index)) return false;
                    break;
                case 23:
                    if (!TryAddItemInt32(item, ref offset, "edTransmitterPower")) return false;
                    break;
                case 25:
                    // Structural header scanning can retain opaque type-25 lookalikes inside
                    // mod data. Only promote the exact nested TTranclucator route.
                    TryReadNestedItemTranclucator(item, offset);
                    return true;
                case 42:
                    if (!TryAddItemInt32(item, ref offset, "edHitPoints") ||
                        !TryAddItemByte(item, ref offset, "edArmor") ||
                        !TryAddItemByte(item, ref offset, "edHullTechLevel") ||
                        !TryAddItemByte(item, ref offset, "cbShipType") ||
                        !TryAddItemInt32(item, ref offset, "edSeriesNum")) return false;
                    int seriesNumber = (int)item.DerivedFields[item.DerivedFields.Count - 1].IntegerValue;
                    if (seriesNumber != -1 && !TryAddItemUInt32(item, ref offset, "edSeriesCRC")) return false;
                    if (!TryAddItemBoolean(item, ref offset, "chbBuiltByPirate") ||
                        !TryAddItemByte(item, ref offset, "edBridgeType") ||
                        !TryAddItemBoolean(item, ref offset, "chbImpulseShields") ||
                        !TryAddItemBoolean(item, ref offset, "$HullHasInterceptors") ||
                        !TryAddItemInt32(item, ref offset, "edEnergy") ||
                        !TryAddItemInt32(item, ref offset, "edEnergyMax")) return false;
                    bool hasInterceptors = item.DerivedFields[item.DerivedFields.Count - 3].IntegerValue != 0;
                    if (hasInterceptors &&
                        (!TryAddItemUInt32(item, ref offset, "cbInterceptorsNextTarget") ||
                        !TryAddItemByte(item, ref offset, "cbInterceptorsStrategy") ||
                        !TryAddItemByte(item, ref offset, "edInterceptorsDuration"))) return false;
                    break;
                case 43:
                    if (!TryAddItemByte(item, ref offset, "edFuelTanksTechLevel") ||
                        !TryAddItemUInt16(item, ref offset, "edFuel") ||
                        !TryAddItemByte(item, ref offset, "edCapacity")) return false;
                    break;
                case 44:
                    if (!TryAddItemByte(item, ref offset, "edEngineTechLevel") ||
                        !TryAddItemInt32(item, ref offset, "edSpeed") ||
                        !TryAddItemByte(item, ref offset, "edJump") ||
                        !TryAddItemByte(item, ref offset, "edEnginePower")) return false;
                    break;
                case 45:
                    if (!TryAddItemByte(item, ref offset, "edRadarTechLevel") ||
                        !TryAddItemUInt16(item, ref offset, "edRadius")) return false;
                    break;
                case 46:
                    if (!TryAddItemByte(item, ref offset, "edScanerTechLevel") ||
                        !TryAddItemByte(item, ref offset, "edScanProtect")) return false;
                    break;
                case 47:
                    if (!TryAddItemByte(item, ref offset, "edRepairRobotTechLevel") ||
                        !TryAddItemByte(item, ref offset, "edRecoverHitPoints")) return false;
                    break;
                case 48:
                    if (!TryAddItemByte(item, ref offset, "edCargoHookTechLevel") ||
                        !TryAddItemUInt16(item, ref offset, "edPickUpSize") ||
                        !TryAddItemUInt16(item, ref offset, "edHookRadius") ||
                        !TryAddItemFloat(item, ref offset, "edSpeedMin") ||
                        !TryAddItemFloat(item, ref offset, "edSpeedMax")) return false;
                    break;
                case 49:
                    if (!TryAddItemByte(item, ref offset, "edDefGeneratorTechLevel") ||
                        !TryAddItemFloat(item, ref offset, "edDefPower")) return false;
                    break;
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                    if (!TryAddItemByte(item, ref offset, "edWeaponTechLevel") ||
                        !TryAddItemUInt16(item, ref offset, "edWeaponRadius") ||
                        !TryAddItemInt32(item, ref offset, "edMinDamage") ||
                        !TryAddItemInt32(item, ref offset, "edMaxDamage") ||
                        !TryAddItemByte(item, ref offset, "edWeaponTargetType")) return false;
                    long targetType = item.DerivedFields[item.DerivedFields.Count - 1].IntegerValue;
                    if (targetType < 0 || targetType > 4) return false;
                    if (targetType != 0 && !TryAddItemUInt32(item, ref offset, "cbWeaponTarget")) return false;
                    // The original reader keeps explicit compatibility routes for the three
                    // stock ammunition weapons: Missile Launcher, Torpedo Tube and Lirecron.
                    // Their factory types are 53, 64 and 67 respectively.
                    if (item.Type == 53 || item.Type == 64 || item.Type == 67)
                        if (!TryAddItemUInt32(item, ref offset, "edAmmunition") ||
                            !TryAddItemUInt32(item, ref offset, "edMaxAmmunition")) return false;
                    break;
                case 68:
                    if (!TryAddItemByte(item, ref offset, "edWeaponTechLevel") ||
                        !TryAddItemUInt16(item, ref offset, "edWeaponRadius") ||
                        !TryAddItemInt32(item, ref offset, "edMinDamage") ||
                        !TryAddItemInt32(item, ref offset, "edMaxDamage") ||
                        !TryAddItemByte(item, ref offset, "edWeaponTargetType")) return false;
                    long customTargetType = item.DerivedFields[item.DerivedFields.Count - 1].IntegerValue;
                    if (customTargetType < 0 || customTargetType > 4) return false;
                    if (customTargetType != 0 && !TryAddItemUInt32(item, ref offset, "cbWeaponTarget")) return false;
                    if (CustomWeaponUsesAmmunition(item.CustomWeaponName))
                        if (!TryAddItemUInt32(item, ref offset, "edAmmunition") ||
                            !TryAddItemUInt32(item, ref offset, "edMaxAmmunition")) return false;
                    break;
                case 69:
                case 75:
                    if (!TryAddItemInt32(item, ref offset, "edCountableItemCount") ||
                        !TryAddItemBoolean(item, ref offset, "chbCountableItemNatural")) return false;
                    break;
                case 70:
                    if (!TryAddItemString(item, ref offset, "mmUselessItemCustomText") ||
                        !TryAddItemInt32(item, ref offset, "edUselessItemData1") ||
                        !TryAddItemInt32(item, ref offset, "edUselessItemData2") ||
                        !TryAddItemInt32(item, ref offset, "edUselessItemData3")) return false;
                    break;
                case 71:
                    break;
                case 72:
                    if (!TryAddItemByte(item, ref offset, "edCisternCapacity") ||
                        !TryAddItemInt32(item, ref offset, "edCisternFuel")) return false;
                    break;
                case 73:
                    if (!TryAddItemByte(item, ref offset, "edSatelliteType") ||
                        !TryAddItemInt32(item, ref offset, "edWear") ||
                        !TryAddItemUInt32(item, ref offset, "cbSatellitePlanet") ||
                        !TryAddItemByte(item, ref offset, "edWaterSpeed") ||
                        !TryAddItemByte(item, ref offset, "edLandSpeed") ||
                        !TryAddItemByte(item, ref offset, "edHillSpeed") ||
                        !TryAddItemFloat(item, ref offset, "edPlace")) return false;
                    break;
                case 74:
                    if (!TryAddItemUInt32(item, ref offset, "cbTreasureMapPlanet") ||
                        !TryAddItemString(item, ref offset, "edShipName") ||
                        !TryAddItemString(item, ref offset, "mmPlanetInfo1") ||
                        !TryAddItemString(item, ref offset, "mmPlanetInfo2")) return false;
                    break;
                default:
                    // Generic artefacts and weapon tails are handled separately; preserving them is safe.
                    return true;
            }
            item.HasDerivedTail = true;
            item.DerivedTailOffset = start;
            item.DerivedTailEnd = offset;
            return true;
        }

        private bool CustomWeaponUsesAmmunition(string systemName)
        {
            byte descriptorType;
            return !string.IsNullOrEmpty(systemName) &&
                CustomWeaponDescriptorTypes.TryGetValue(systemName, out descriptorType) &&
                descriptorType >= 5 && descriptorType <= 7;
        }

        private bool TryReadNestedItemTranclucator(ItemHeaderRecord item, int start)
        {
            int limit = GalaxySummary == null ? MainPayload.Length - 1 :
                Math.Min(MainPayload.Length - 1, GalaxySummary.PlanetReferenceListOffset);
            if (start < 0 || start > limit - 190) return false;
            uint objectId = ReadUInt32(MainPayload, start);
            if (objectId == 0 || objectId > 10000000) return false;
            int offset = start + 4;
            string name, scriptName;
            if (!TryReadObjectString(ref offset, 80, true, out name)) return false;
            int nameEnd = offset;
            if (!TryReadObjectString(ref offset, 128, true, out scriptName)) return false;
            int scriptNameEnd = offset;
            if (offset > limit - 177 || MainPayload[offset] != 5 || MainPayload[offset + 1] > 7) return false;
            float x = BitConverter.ToSingle(MainPayload, offset + 2);
            float y = BitConverter.ToSingle(MainPayload, offset + 6);
            if (!IsSupportedObjectCoordinate(x) || !IsSupportedObjectCoordinate(y)) return false;
            int itemCountOffset = offset + 175;
            int equipmentCount = MainPayload[itemCountOffset] | MainPayload[itemCountOffset + 1] << 8;
            if (MainPayload[offset + 174] > 4 || equipmentCount > 128) return false;

            ShipHeaderRecord ship = new ShipHeaderRecord();
            ship.Start = start; ship.NameEnd = nameEnd; ship.ScriptNameEnd = scriptNameEnd;
            ship.FixedPrefixEnd = itemCountOffset + 2; ship.ObjectId = objectId; ship.Type = 5;
            ship.Owner = MainPayload[offset + 1]; ship.Name = name; ship.ScriptName = scriptName;
            ship.X = x; ship.Y = y; ship.HomePlanetId = ReadUInt32(MainPayload, offset + 10);
            ship.CurrentStarId = ReadUInt32(MainPayload, offset + 14);
            ship.CurrentPlanetId = ReadUInt32(MainPayload, offset + 18);
            ship.CurrentShipId = ReadUInt32(MainPayload, offset + 22);
            int goodsOffset = offset + 26;
            for (int good = 0; good < 8; good++)
                for (int field = 0; field < 4; field++)
                    ship.Goods[good, field] = ReadUInt32(MainPayload, goodsOffset + good * 16 + field * 4);
            ship.Money = ReadUInt32(MainPayload, offset + 154);
            ship.Rnd = ReadUInt32(MainPayload, offset + 158);
            ship.RndOut = ReadUInt32(MainPayload, offset + 162);
            ship.Day = ReadUInt32(MainPayload, offset + 166);
            ship.Face = ReadInt32(MainPayload, offset + 170);
            ship.PilotRace = MainPayload[offset + 174];
            ship.EquipmentItemCount = checked((ushort)equipmentCount);

            ShipHeaderRecord parsed = null;
            int scanLimit = Math.Min(limit, start + 250000);
            for (int graphStart = ship.FixedPrefixEnd + 30; graphStart <= scanLimit - 520; graphStart++)
            {
                if (MainPayload[graphStart + 1] != 0 || MainPayload[graphStart] < 0x20 ||
                    MainPayload[graphStart] > 0x7E) continue;
                ShipHeaderRecord candidate = ship.Clone();
                if (!TryReadShipCommonTail(candidate, graphStart, scanLimit) ||
                    !IsPreferredShipGraphName(candidate.GraphName) ||
                    !TryScanShipPreCommonPrefix(candidate, 10000000) ||
                    !TryReadTranclucatorShipTail(candidate, scanLimit)) continue;
                parsed = candidate;
            }
            if (parsed == null) return false;
            item.NestedTranclucator = parsed;
            item.HasDerivedTail = true;
            item.DerivedTailOffset = start;
            item.DerivedTailEnd = parsed.TranclucatorPostArtOffset + 10;
            return true;
        }

        private bool TryScanShipPreCommonPrefix(ShipHeaderRecord ship, uint nextObjectId)
        {
            int cursor = ship.FixedPrefixEnd;
            if (!TrySkipShipItemEntries(ref cursor, ship.CommonTailOffset,
                ship.EquipmentItemCount, nextObjectId)) return false;
            int count;
            if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out count) ||
                !TrySkipShipItemEntries(ref cursor, ship.CommonTailOffset, count, nextObjectId) ||
                !TryReadShipListCount(ref cursor, ship.CommonTailOffset, out count) ||
                !TrySkipShipItemEntries(ref cursor, ship.CommonTailOffset, count, nextObjectId) ||
                !TryReadShipListCount(ref cursor, ship.CommonTailOffset, out count) ||
                cursor > ship.CommonTailOffset - count * 5) return false;
            cursor += count * 5;
            if (!TryReadShipListCount(ref cursor, ship.CommonTailOffset, out count) ||
                cursor > ship.CommonTailOffset - count * 9) return false;
            cursor += count * 9;
            if (cursor > ship.CommonTailOffset - 4) return false;
            int customCount = ReadInt32(MainPayload, cursor); cursor += 4;
            if (customCount < 0 || customCount > 10000) return false;
            for (int index = 0; index < customCount; index++)
            {
                string text;
                if (!TryReadItemString(ref cursor, 32768, out text) || cursor > ship.CommonTailOffset ||
                    !TryReadItemString(ref cursor, 32768, out text) || cursor > ship.CommonTailOffset - 12)
                    return false;
                cursor += 12;
                if (!TryReadItemString(ref cursor, 32768, out text) || cursor > ship.CommonTailOffset ||
                    !TryReadItemString(ref cursor, 32768, out text) || cursor > ship.CommonTailOffset ||
                    !TryReadItemString(ref cursor, 32768, out text) || cursor > ship.CommonTailOffset)
                    return false;
            }
            if (!TrySkipShipReferenceList(ref cursor, ship.CommonTailOffset) ||
                !TrySkipShipReferenceList(ref cursor, ship.CommonTailOffset) ||
                cursor > ship.CommonTailOffset - 12) return false;
            cursor += 8;
            uint partner = ReadUInt32(MainPayload, cursor); cursor += 4;
            if (partner > 10000000) return false;
            if (partner != 0)
            {
                if (cursor > ship.CommonTailOffset - 4) return false;
                cursor += 4;
            }
            return cursor == ship.CommonTailOffset;
        }

        private bool TrySkipShipItemEntries(ref int cursor, int limit, int count, uint nextObjectId)
        {
            if (count < 0 || count > 10000) return false;
            for (int index = 0; index < count; index++)
            {
                if (cursor >= limit) return false;
                byte type = MainPayload[cursor++];
                if (type == 68)
                {
                    string customName;
                    if (!TryReadItemString(ref cursor, 512, out customName) || cursor >= limit) return false;
                }
                ItemHeaderRecord item;
                if (!TryReadItemHeader(cursor, nextObjectId, out item, true) || item.Type != type ||
                    item.Type >= 8 && !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd)) return false;
                cursor = SerializedItemEnd(item);
                if (cursor > limit) return false;
            }
            return true;
        }

        private bool TrySkipShipReferenceList(ref int cursor, int limit)
        {
            int count;
            if (!TryReadShipListCount(ref cursor, limit, out count) || cursor > limit - count * 4) return false;
            cursor += count * 4;
            return true;
        }

        private bool TryAddItemByte(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset >= MainPayload.Length) return false;
            AddItemField(item, control, ItemDerivedField.Byte, offset, offset + 1, MainPayload[offset]);
            offset++; return true;
        }

        private bool TryAddItemBoolean(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset >= MainPayload.Length || MainPayload[offset] > 1) return false;
            AddItemField(item, control, ItemDerivedField.Boolean, offset, offset + 1, MainPayload[offset]);
            offset++; return true;
        }

        private bool TryAddItemUInt16(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset > MainPayload.Length - 2) return false;
            AddItemField(item, control, ItemDerivedField.UInt16, offset, offset + 2,
                MainPayload[offset] | MainPayload[offset + 1] << 8);
            offset += 2; return true;
        }

        private bool TryAddItemInt32(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset > MainPayload.Length - 4) return false;
            AddItemField(item, control, ItemDerivedField.Int32, offset, offset + 4, ReadInt32(MainPayload, offset));
            offset += 4; return true;
        }

        private bool TryAddItemUInt32(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset > MainPayload.Length - 4) return false;
            AddItemField(item, control, ItemDerivedField.UInt32, offset, offset + 4, ReadUInt32(MainPayload, offset));
            offset += 4; return true;
        }

        private bool TryAddItemFloat(ItemHeaderRecord item, ref int offset, string control)
        {
            if (offset > MainPayload.Length - 4) return false;
            float value = BitConverter.ToSingle(MainPayload, offset);
            if (float.IsNaN(value) || float.IsInfinity(value)) return false;
            ItemDerivedField field = new ItemDerivedField();
            field.ControlName = control; field.Kind = ItemDerivedField.Float32;
            field.Offset = offset; field.End = offset + 4; field.FloatValue = value;
            EnsureItemDerivedFields(item).Add(field); offset += 4; return true;
        }

        private bool TryAddItemString(ItemHeaderRecord item, ref int offset, string control)
        {
            int start = offset; string text;
            if (!TryReadItemString(ref offset, 4096, out text)) return false;
            ItemDerivedField field = new ItemDerivedField();
            field.ControlName = control; field.Kind = ItemDerivedField.String;
            field.Offset = start; field.End = offset; field.StringValue = text;
            EnsureItemDerivedFields(item).Add(field); return true;
        }

        private bool TryAddItemOptionalString(ItemHeaderRecord item, ref int offset, string control)
        {
            int start = offset; string text;
            if (!TryReadOptionalItemString(ref offset, out text)) return false;
            ItemDerivedField field = new ItemDerivedField();
            field.ControlName = control; field.Kind = ItemDerivedField.String;
            field.Offset = start; field.End = offset; field.StringValue = text ?? string.Empty;
            field.IntegerValue = 1; // serialized as Boolean + optional UTF-16Z
            EnsureItemDerivedFields(item).Add(field); return true;
        }

        private static void AddItemField(ItemHeaderRecord item, string control, byte kind,
            int offset, int end, long value)
        {
            ItemDerivedField field = new ItemDerivedField();
            field.ControlName = control; field.Kind = kind; field.Offset = offset;
            field.End = end; field.IntegerValue = value; EnsureItemDerivedFields(item).Add(field);
        }

        private static List<ItemDerivedField> EnsureItemDerivedFields(ItemHeaderRecord item)
        {
            if (item.DerivedFields == null) item.DerivedFields = new List<ItemDerivedField>();
            return item.DerivedFields;
        }

        private bool TrySkipItemBytes(ref int offset, int count)
        {
            if (count < 0 || offset > MainPayload.Length - count) return false;
            offset += count; return true;
        }

        private void ParseHoleHeaders(GalaxySummaryData summary)
        {
            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in GalaxyStars) starIds.Add(star.ObjectId);
            Dictionary<uint, HoleRecord> holes = new Dictionary<uint, HoleRecord>();
            for (int start = GalaxyStarsOffset + 1; start <= summary.PlanetReferenceListOffset - 44; start++)
            {
                uint objectId = ReadUInt32(MainPayload, start);
                if (objectId == 0 || objectId >= summary.NextObjectId) continue;
                uint fromStarId = ReadUInt32(MainPayload, start + 4);
                uint toStarId = ReadUInt32(MainPayload, start + 16);
                if (!starIds.Contains(fromStarId) || !starIds.Contains(toStarId)) continue;
                float fromX = BitConverter.ToSingle(MainPayload, start + 8);
                float fromY = BitConverter.ToSingle(MainPayload, start + 12);
                float toX = BitConverter.ToSingle(MainPayload, start + 20);
                float toY = BitConverter.ToSingle(MainPayload, start + 24);
                if (!IsSupportedObjectCoordinate(fromX) || !IsSupportedObjectCoordinate(fromY) ||
                    !IsSupportedObjectCoordinate(toX) || !IsSupportedObjectCoordinate(toY)) continue;
                int turnCreate = ReadInt32(MainPayload, start + 28);
                int type = ReadInt32(MainPayload, start + 32);
                if (turnCreate < 0 || turnCreate > 100000000 || type < 0 || type > 1024) continue;
                int offset = start + 36;
                string graphName, mapName;
                if (!TryReadObjectString(ref offset, 128, false, out graphName)) continue;
                int graphNameEnd = offset;
                if (!TryReadObjectString(ref offset, 128, true, out mapName)) continue;
                if (graphName.Length < 3) continue;

                HoleRecord previous;
                if (holes.TryGetValue(objectId, out previous))
                    throw new SavFormatException("THole: object id " + objectId +
                        " имеет несколько структурных заголовков: 0x" + previous.Start.ToString("X") +
                        " [" + previous.FromStarId + "→" + previous.ToStarId + ", " + previous.GraphName +
                        ", " + previous.MapName + "] и 0x" + start.ToString("X") + " [" + fromStarId +
                        "→" + toStarId + ", " + graphName + ", " + mapName + "].");
                HoleRecord hole = new HoleRecord();
                hole.Start = start; hole.GraphNameEnd = graphNameEnd; hole.End = offset; hole.ObjectId = objectId;
                hole.FromStarId = fromStarId; hole.FromX = fromX; hole.FromY = fromY;
                hole.ToStarId = toStarId; hole.ToX = toX; hole.ToY = toY;
                hole.TurnCreate = turnCreate; hole.HoleType = type;
                hole.GraphName = graphName; hole.MapName = mapName;
                holes.Add(objectId, hole); GalaxyHoles.Add(hole);
            }
            LocateExactHoleList(summary);
        }

        private void LocateExactHoleList(GalaxySummaryData summary)
        {
            int gateStart = FindExactGateListStart(summary);
            if (gateStart < 0)
            {
                GalaxyHoles.Clear();
                return;
            }
            Dictionary<int, HoleRecord> byStart = new Dictionary<int, HoleRecord>();
            foreach (HoleRecord hole in GalaxyHoles) byStart[hole.Start] = hole;
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            int pathCount = 0;
            int selectedHoleCount = -1;
            int selectedOffset = -1;
            int selectedEnd = -1;
            List<HoleRecord> selected = null;
            foreach (HoleRecord first in GalaxyHoles)
            {
                int candidate = first.Start - 2;
                if (candidate < GalaxyStarsOffset || candidate > MainPayload.Length - 2) continue;
                int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                if (count < 1 || count > 10000) continue;
                int cursor = candidate + 2;
                List<HoleRecord> chain = new List<HoleRecord>(count);
                bool valid = true;
                for (int index = 0; index < count; index++)
                {
                    HoleRecord record;
                    if (!byStart.TryGetValue(cursor, out record) || record.End <= record.Start)
                    { valid = false; break; }
                    chain.Add(record); cursor = record.End;
                }
                if (!valid) continue;
                List<StoredItemRecord> stored;
                int storedEnd;
                if (!TryReadStoredItemList(cursor, gateStart, itemsByStart, out stored, out storedEnd) ||
                    storedEnd != gateStart) continue;
                if (count > selectedHoleCount)
                {
                    selectedHoleCount = count; pathCount = 1;
                    selectedOffset = candidate; selectedEnd = cursor; selected = chain;
                }
                else if (count == selectedHoleCount && candidate < selectedOffset)
                {
                    selectedOffset = candidate; selectedEnd = cursor; selected = chain;
                }
            }

            // The zero-THole case has no record from which to recover the count.
            // Locate the immediately following StoredItems list by requiring that it
            // reaches the exact TGate list boundary and is preceded by UInt16(0).
            if (pathCount == 0)
                for (int storedStart = GalaxyStarsOffset + 2; storedStart <= gateStart - 2; storedStart++)
                {
                    if ((MainPayload[storedStart - 2] | MainPayload[storedStart - 1] << 8) != 0) continue;
                    int storedCount = MainPayload[storedStart] | MainPayload[storedStart + 1] << 8;
                    if (storedCount == 0 && storedStart + 2 != gateStart) continue;
                    if (storedCount > 10000) continue;
                    List<StoredItemRecord> stored;
                    int storedEnd;
                    if (!TryReadStoredItemList(storedStart, gateStart, itemsByStart,
                        out stored, out storedEnd) || storedEnd != gateStart) continue;
                    if (pathCount == 0)
                    {
                        pathCount = 1;
                        selectedOffset = storedStart - 2;
                        selectedEnd = storedStart;
                        selected = new List<HoleRecord>();
                    }
                }
            if (pathCount == 0)
            {
                GalaxyHoles.Clear();
                return;
            }
            GalaxyHoles.Clear(); GalaxyHoles.AddRange(selected);
            HoleListCountOffset = selectedOffset;
            HoleListEndOffset = selectedEnd;
        }

        private int FindExactGateListStart(GalaxySummaryData summary)
        {
            int end = summary.PlanetReferenceListOffset;
            int selected = -1;
            int count = 0;
            for (int candidate = GalaxyStarsOffset; candidate <= end - 13; candidate++)
            {
                int value = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                if (value < 1 || value > 10000) continue;
                List<GateRecord> records;
                if (!TryReadGateList(candidate, end, out records)) continue;
                selected = candidate; count++;
            }
            if (count > 1)
                throw new SavFormatException("TGate: найдено несколько точных списков перед ссылками планет.");
            if (count == 1) return selected;
            int empty = end - 2;
            return empty >= GalaxyStarsOffset &&
                (MainPayload[empty] | MainPayload[empty + 1] << 8) == 0 ? empty : -1;
        }

        private sealed class EquipmentPrefixData
        {
            internal int FirstStringEnd;
            internal int SecondStringEnd;
            internal int ScalarOffset;
            internal string CustomFaction;
            internal string SystemName;
            internal byte Exploitable;
            internal float Strength;
            internal byte Broken;
            internal byte Slot;
            internal int BonusOffset;
            internal int BonusEnd;
            internal int Bonus;
            internal uint BonusReferenceId;
            internal int SpecialOffset;
            internal int SpecialEnd;
            internal int Special;
            internal uint SpecialReferenceId;
            internal int ExtraSpecialCountOffset;
            internal int ExtraSpecialEnd;
            internal List<ItemExtraSpecialRecord> ExtraSpecials = new List<ItemExtraSpecialRecord>();
            internal byte DominatorSeries;
        }

        private bool TryReadEquipmentPrefix(ref int offset, out EquipmentPrefixData value)
        {
            value = null;
            string customFaction;
            if (!TryReadOptionalItemString(ref offset, out customFaction)) return false;
            int firstStringEnd = offset;
            string systemName;
            if (!TryReadOptionalItemString(ref offset, out systemName)) return false;
            int secondStringEnd = offset;
            if (offset > MainPayload.Length - 16) return false;
            if (MainPayload[offset] > 1) return false;
            float scalar = BitConverter.ToSingle(MainPayload, offset + 1);
            if (float.IsNaN(scalar) || float.IsInfinity(scalar)) return false;
            if (MainPayload[offset + 5] > 1) return false;
            int scalarOffset = offset;
            byte exploitable = MainPayload[offset];
            byte broken = MainPayload[offset + 5];
            byte slot = MainPayload[offset + 6];
            offset += 7;

            int bonusOffset = offset, bonusEnd, bonus, specialOffset, specialEnd, special;
            uint bonusReferenceId, specialReferenceId;
            if (!TryReadEquipmentIndexedReference(ref offset, out bonus, out bonusReferenceId)) return false;
            bonusEnd = offset;
            specialOffset = offset;
            if (!TryReadEquipmentIndexedReference(ref offset, out special, out specialReferenceId)) return false;
            specialEnd = offset;
            if (offset > MainPayload.Length - 4) return false;
            int extraSpecialCountOffset = offset;
            int count = ReadInt32(MainPayload, offset);
            offset += 4;
            if (count < 0 || count > 10000) return false;
            List<ItemExtraSpecialRecord> extraSpecials = new List<ItemExtraSpecialRecord>(count);
            for (int index = 0; index < count; index++)
            {
                if (offset > MainPayload.Length - 8) return false;
                int extraSpecial;
                uint referenceId;
                if (!TryReadEquipmentIndexedReference(ref offset, out extraSpecial, out referenceId) ||
                    offset > MainPayload.Length - 4) return false;
                ItemExtraSpecialRecord record = new ItemExtraSpecialRecord();
                record.Special = extraSpecial;
                record.ReferenceId = referenceId;
                record.Count = ReadInt32(MainPayload, offset);
                offset += 4;
                extraSpecials.Add(record);
            }
            int extraSpecialEnd = offset;
            if (offset >= MainPayload.Length) return false;
            byte dominatorSeries = MainPayload[offset++];
            EquipmentPrefixData parsed = new EquipmentPrefixData();
            parsed.FirstStringEnd = firstStringEnd;
            parsed.SecondStringEnd = secondStringEnd;
            parsed.ScalarOffset = scalarOffset;
            parsed.CustomFaction = customFaction;
            parsed.SystemName = systemName;
            parsed.Exploitable = exploitable;
            parsed.Strength = scalar;
            parsed.Broken = broken;
            parsed.Slot = slot;
            parsed.BonusOffset = bonusOffset;
            parsed.BonusEnd = bonusEnd;
            parsed.Bonus = bonus;
            parsed.BonusReferenceId = bonusReferenceId;
            parsed.SpecialOffset = specialOffset;
            parsed.SpecialEnd = specialEnd;
            parsed.Special = special;
            parsed.SpecialReferenceId = specialReferenceId;
            parsed.ExtraSpecialCountOffset = extraSpecialCountOffset;
            parsed.ExtraSpecialEnd = extraSpecialEnd;
            parsed.ExtraSpecials = extraSpecials;
            parsed.DominatorSeries = dominatorSeries;
            value = parsed;
            return true;
        }

        private bool TryReadEquipmentIndexedReference(ref int offset, out int index,
            out uint referenceId)
        {
            index = 0;
            referenceId = 0;
            if (offset > MainPayload.Length - 4) return false;
            index = ReadInt32(MainPayload, offset);
            offset += 4;
            if (index < 0 || index > 1000000) return false;
            if (index > 0)
            {
                if (offset > MainPayload.Length - 4) return false;
                referenceId = ReadUInt32(MainPayload, offset);
                offset += 4;
            }
            return true;
        }

        private bool TryReadOptionalItemString(ref int offset, out string value)
        {
            value = string.Empty;
            if (offset >= MainPayload.Length) return false;
            byte present = MainPayload[offset++];
            if (present > 1) return false;
            return present == 0 || TryReadItemString(ref offset, 512, out value);
        }

        private bool TryGetCustomWeaponDiscriminator(int itemStart, out string customWeaponName,
            out int discriminatorOffset)
        {
            customWeaponName = string.Empty;
            discriminatorOffset = -1;
            foreach (string systemName in CustomWeaponNames)
            {
                int length = Encoding.Unicode.GetByteCount(systemName ?? string.Empty) + 2;
                int discriminator = itemStart - length - 1;
                if (discriminator < 0 || MainPayload[discriminator] != 68) continue;
                int stringOffset = discriminator + 1;
                string parsed;
                if (TryReadItemString(ref stringOffset, 512, out parsed) &&
                    stringOffset == itemStart && parsed == systemName)
                {
                    customWeaponName = parsed;
                    discriminatorOffset = discriminator;
                    return true;
                }
            }
            return false;
        }

        private bool TryReadItemString(ref int offset, int maximumLength, out string value)
        {
            value = null;
            int start = offset;
            for (int index = 0; index <= maximumLength; index++)
            {
                if (offset < 0 || offset > MainPayload.Length - 2) return false;
                int codeUnit = MainPayload[offset] | MainPayload[offset + 1] << 8;
                offset += 2;
                if (codeUnit == 0)
                {
                    value = Encoding.Unicode.GetString(MainPayload, start, offset - start - 2);
                    return true;
                }
                if (char.IsControl((char)codeUnit) && codeUnit != '\r' && codeUnit != '\n' && codeUnit != '\t')
                    return false;
            }
            return false;
        }

        private void ParseActiveScripts(GalaxySummaryData summary)
        {
            int warOperationsEnd = summary.TurnOffset - 11;
            if (warOperationsEnd <= GalaxyStarsOffset)
                throw new SavFormatException("TScript: поздняя граница военных операций недопустима.");

            int selectedOffset = -1;
            List<ScriptRecord> selected = null;
            int selectedWarOffset = -1;
            List<WarOperationRecord> selectedWarOperations = null;
            int selectedCount = 0;
            for (int scriptStart = GalaxyStarsOffset + 2; scriptStart < warOperationsEnd; scriptStart++)
            {
                if (MainPayload[scriptStart + 1] != 0) continue;
                int listOffset = scriptStart - 2;
                int scriptCount = MainPayload[listOffset] | MainPayload[listOffset + 1] << 8;
                if (scriptCount < 1 || scriptCount > 1024) continue;

                int offset = scriptStart;
                List<ScriptRecord> scripts = new List<ScriptRecord>(scriptCount);
                bool valid = true;
                for (int index = 0; index < scriptCount; index++)
                {
                    ScriptRecord script;
                    if (!TryReadScript(ref offset, summary.NextObjectId, out script))
                    {
                        valid = false;
                        break;
                    }
                    scripts.Add(script);
                }
                int warOffset = offset;
                List<WarOperationRecord> warOperations;
                if (!valid || !TryReadWarOperations(ref offset, warOperationsEnd, out warOperations) ||
                    offset != warOperationsEnd)
                    continue;
                if (MainPayload[warOperationsEnd] != 0 || ReadInt32(MainPayload, warOperationsEnd + 1) != 0)
                    continue;
                selectedOffset = listOffset;
                selected = scripts;
                selectedWarOffset = warOffset;
                selectedWarOperations = warOperations;
                selectedCount++;
            }

            if (selectedCount > 1)
                throw new SavFormatException("TScript: найдено несколько маршрутов списка активных скриптов.");
            if (selectedCount == 1)
            {
                ActiveScriptListOffset = selectedOffset;
                ActiveScripts.AddRange(selected);
                summary.ActiveScriptListOffset = selectedOffset;
                summary.ActiveScripts.AddRange(selected);
                summary.WarOperationListOffset = selectedWarOffset;
                summary.WarOperations.AddRange(selectedWarOperations);
                return;
            }

            // With no active scripts the writer emits UInt16(0), immediately followed
            // by the war-operation list. The overwhelmingly common empty/empty route
            // is therefore a fixed four-byte suffix before the two legacy zero fields.
            int zeroOffset = warOperationsEnd - 4;
            if (zeroOffset >= GalaxyStarsOffset &&
                (MainPayload[zeroOffset] | MainPayload[zeroOffset + 1] << 8) == 0)
            {
                int warOffset = zeroOffset + 2;
                int warStart = warOffset;
                List<WarOperationRecord> warOperations;
                if (TryReadWarOperations(ref warOffset, warOperationsEnd, out warOperations))
                {
                    ActiveScriptListOffset = zeroOffset;
                    summary.ActiveScriptListOffset = zeroOffset;
                    summary.WarOperationListOffset = warStart;
                    summary.WarOperations.AddRange(warOperations);
                    return;
                }
            }

            int zeroSelected = -1;
            int zeroSelectedWarOffset = -1;
            List<WarOperationRecord> zeroSelectedWarOperations = null;
            int zeroSelectedCount = 0;
            for (int candidate = GalaxyStarsOffset; candidate <= warOperationsEnd - 4; candidate++)
            {
                if ((MainPayload[candidate] | MainPayload[candidate + 1] << 8) != 0) continue;
                int warOffset = candidate + 2;
                int warStart = warOffset;
                List<WarOperationRecord> warOperations;
                if (!TryReadWarOperations(ref warOffset, warOperationsEnd, out warOperations)) continue;
                zeroSelected = candidate;
                zeroSelectedWarOffset = warStart;
                zeroSelectedWarOperations = warOperations;
                zeroSelectedCount++;
            }
            if (zeroSelectedCount != 1)
                throw new SavFormatException("TScript: не удалось однозначно найти пустой список активных скриптов.");
            ActiveScriptListOffset = zeroSelected;
            summary.ActiveScriptListOffset = zeroSelected;
            summary.WarOperationListOffset = zeroSelectedWarOffset;
            summary.WarOperations.AddRange(zeroSelectedWarOperations);
        }

        private void ParseScriptGlobalsAndCache(GalaxySummaryData summary)
        {
            int sectionEnd = summary.TurnOffset - 11;
            int offset = summary.KellerAttackOffset + 8;
            if (offset > sectionEnd - 2)
                throw new SavFormatException("TScript globals: отсутствует список shop slots.");
            summary.ScriptShopSlotCountOffset = offset;
            int shopCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (shopCount > 10000)
                throw new SavFormatException("TScript globals: неверное число shop slots.");
            for (int index = 0; index < shopCount; index++)
            {
                if (offset > sectionEnd - 3)
                    throw new SavFormatException("TScript globals: обрезан shop slot.");
                ScriptShopSlotRecord slot = new ScriptShopSlotRecord();
                slot.Start = offset;
                slot.X = MainPayload[offset++];
                slot.Y = MainPayload[offset++];
                byte hasEquipment = MainPayload[offset++];
                if (hasEquipment > 1)
                    throw new SavFormatException("TScript globals: неверный флаг предмета shop slot.");
                slot.HasEquipment = hasEquipment != 0;
                if (hasEquipment == 0)
                {
                    slot.FactoryDiscriminatorOffset = -1;
                    slot.ItemStart = -1;
                    slot.End = offset;
                    summary.ScriptShopSlots.Add(slot);
                    continue;
                }
                if (offset >= sectionEnd)
                    throw new SavFormatException("TScript globals: отсутствует discriminator предмета shop slot.");
                slot.FactoryDiscriminatorOffset = offset;
                byte factoryType = MainPayload[offset++];
                slot.ItemType = factoryType;
                if (factoryType == 68)
                {
                    string customWeaponName;
                    if (!TryReadItemString(ref offset, 512, out customWeaponName))
                        throw new SavFormatException("TScript globals: обрезано имя TCustomWeapon shop slot.");
                }
                // The object id starts here after the factory discriminator and,
                // for TCustomWeapon, its serialized system-name wrapper.
                slot.ItemStart = offset;
                ItemHeaderRecord nested = null;
                foreach (ItemHeaderRecord item in GalaxyItems)
                    if (item.Start == offset) { nested = item; break; }
                if (nested == null)
                {
                    if (!TryReadItemHeader(offset, summary.NextObjectId, out nested, true) ||
                        nested.Type != factoryType || nested.Type >= 8 &&
                            !TryReadKnownItemDerivedTail(nested, nested.SharedPrefixEnd))
                    {
                        StringBuilder nearby = new StringBuilder();
                        foreach (ItemHeaderRecord item in GalaxyItems)
                            if (Math.Abs(item.Start - offset) <= 128)
                                nearby.Append(" 0x").Append(item.Start.ToString("X")).Append("-")
                                    .Append(SerializedItemEnd(item).ToString("X")).Append(" t").Append(item.Type);
                        throw new SavFormatException("TScript globals: вложенный предмет shop slot не найден @ 0x" +
                            offset.ToString("X") + ", slot " + index + "/" + shopCount + "; nearby:" + nearby + ".");
                    }
                    GalaxyItems.Add(nested);
                }
                offset = SerializedItemEnd(nested);
                if (offset > sectionEnd)
                    throw new SavFormatException("TScript globals: предмет shop slot пересекает список скриптов.");
                slot.ItemObjectId = nested.ObjectId;
                slot.End = offset;
                summary.ScriptShopSlots.Add(slot);
            }

            summary.ScriptShopSlotListEndOffset = offset;
            summary.GlobalVariableListOffset = offset;
            List<ScriptVariableRecord> globals;
            if (!TryReadScriptVariableArray(ref offset, 0, true, out globals))
                throw new SavFormatException("TScript globals: массив глобальных переменных повреждён @ 0x" +
                    summary.GlobalVariableListOffset.ToString("X") + ".");
            summary.GlobalVariables.AddRange(globals);

            summary.ScriptCacheListOffset = offset;
            if (offset > sectionEnd - 2)
                throw new SavFormatException("TScript cache: отсутствует счётчик записей.");
            int cacheCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (cacheCount > 10000)
                throw new SavFormatException("TScript cache: неверное число записей.");
            for (int index = 0; index < cacheCount; index++)
            {
                ScriptCacheRecord record = new ScriptCacheRecord();
                record.Start = offset;
                if (!TryReadItemString(ref offset, 4096, out record.Name) ||
                    offset > sectionEnd - 10)
                    throw new SavFormatException("TScript cache: запись обрезана.");
                record.CountUse = checked((ushort)(MainPayload[offset] | MainPayload[offset + 1] << 8));
                offset += 2;
                record.LastTurn = ReadInt32(MainPayload, offset); offset += 4;
                record.RunScript = ReadInt32(MainPayload, offset); offset += 4;
                record.End = offset;
                summary.ScriptCache.Add(record);
            }

            summary.ActiveScriptListOffset = offset;
            ActiveScriptListOffset = offset;
            if (offset > sectionEnd - 2)
                throw new SavFormatException("TScript: отсутствует счётчик active scripts.");
            int scriptCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (scriptCount > 1024)
                throw new SavFormatException("TScript: неверное число active scripts " + scriptCount +
                    " @ 0x" + summary.ActiveScriptListOffset.ToString("X") + ", globals 0x" +
                    summary.GlobalVariableListOffset.ToString("X") + ", cache 0x" +
                    summary.ScriptCacheListOffset.ToString("X") + " (" + cacheCount + ").");
            for (int index = 0; index < scriptCount; index++)
            {
                ScriptRecord script;
                lastScriptParseFailure = null;
                if (!TryReadScript(ref offset, summary.NextObjectId, out script))
                    throw new SavFormatException("TScript: повреждена active запись " + index +
                        (string.IsNullOrEmpty(lastScriptParseFailure) ? "." : ": " + lastScriptParseFailure + "."));
                ActiveScripts.Add(script);
                summary.ActiveScripts.Add(script);
            }

            summary.WarOperationListOffset = offset;
            List<WarOperationRecord> operations;
            if (!TryReadWarOperations(ref offset, sectionEnd, out operations) || offset != sectionEnd)
                throw new SavFormatException("TScript: список военных операций не завершает script section.");
            summary.WarOperations.AddRange(operations);
        }

        private string lastScriptParseFailure;

        private bool FailScriptParse(string stage, int offset)
        {
            lastScriptParseFailure = stage + " @ 0x" + offset.ToString("X");
            return false;
        }

        private bool TryReadScript(ref int offset, uint nextObjectId, out ScriptRecord value)
        {
            value = null;
            int start = offset;
            string name;
            if (!TryReadItemString(ref offset, 512, out name) || string.IsNullOrEmpty(name))
                return FailScriptParse("name", offset);
            if (offset > MainPayload.Length - 4) return FailScriptParse("old ethers count", offset);
            int oldEtherCount = ReadInt32(MainPayload, offset);
            offset += 4;
            if (oldEtherCount < 0 || oldEtherCount > 10000)
                return FailScriptParse("old ethers count " + oldEtherCount, offset - 4);
            List<ScriptOldEtherRecord> oldEthers = new List<ScriptOldEtherRecord>(oldEtherCount);
            for (int index = 0; index < oldEtherCount; index++)
            {
                ScriptOldEtherRecord oldEther = new ScriptOldEtherRecord();
                if (!TryReadItemString(ref offset, 4096, out oldEther.Name) ||
                    offset > MainPayload.Length - 4) return FailScriptParse("old ether " + index, offset);
                oldEther.Value = ReadInt32(MainPayload, offset);
                offset += 4;
                oldEthers.Add(oldEther);
            }

            List<ScriptVariableRecord> initVariables;
            if (!TryReadScriptVariableArray(ref offset, 0, false, out initVariables))
                return FailScriptParse("init variables", offset);
            List<ScriptVariableRecord> turnVariables;
            if (!TryReadScriptVariableArray(ref offset, 0, false, out turnVariables))
                return FailScriptParse("turn variables", offset);

            int starCount;
            if (!TryReadScriptIntCount(ref offset, 10000, out starCount))
                return FailScriptParse("stars count", offset);
            List<ScriptStarBindingRecord> stars = new List<ScriptStarBindingRecord>();
            for (int star = 0; star < starCount; star++)
            {
                string starName;
                if (!TryReadItemString(ref offset, 512, out starName) || offset > MainPayload.Length - 8)
                    return FailScriptParse("star " + star, offset);
                uint starId = ReadUInt32(MainPayload, offset);
                offset += 4;
                if (starId >= nextObjectId) return FailScriptParse("star object id " + starId, offset - 4);
                int planetCount;
                if (!TryReadScriptIntCount(ref offset, 10000, out planetCount))
                    return FailScriptParse("planet count for star " + star, offset);
                ScriptStarBindingRecord starBinding = new ScriptStarBindingRecord();
                starBinding.Name = starName;
                starBinding.StarObjectId = starId;
                for (int planet = 0; planet < planetCount; planet++)
                {
                    string planetName;
                    if (!TryReadItemString(ref offset, 512, out planetName) || offset > MainPayload.Length - 4)
                        return FailScriptParse("planet " + planet + " for star " + star, offset);
                    uint planetId = ReadUInt32(MainPayload, offset);
                    offset += 4;
                    if (planetId >= nextObjectId)
                        return FailScriptParse("planet object id " + planetId, offset - 4);
                    ScriptPlanetBindingRecord planetBinding = new ScriptPlanetBindingRecord();
                    planetBinding.Name = planetName;
                    planetBinding.PlanetObjectId = planetId;
                    starBinding.Planets.Add(planetBinding);
                }
                if (offset > MainPayload.Length - 4) return FailScriptParse("star legacy tail", offset);
                starBinding.LegacyZero = ReadInt32(MainPayload, offset);
                offset += 4;
                stars.Add(starBinding);
            }

            int itemCount;
            if (!TryReadScriptIntCount(ref offset, 10000, out itemCount))
                return FailScriptParse("items count", offset);
            List<ScriptItemRecord> items = new List<ScriptItemRecord>();
            for (int item = 0; item < itemCount; item++)
            {
                string itemName;
                if (!TryReadItemString(ref offset, 512, out itemName) || offset > MainPayload.Length - 13)
                    return FailScriptParse("item " + item, offset);
                if (MainPayload[offset] > 1) return FailScriptParse("item CanSell", offset);
                ScriptItemRecord itemBinding = new ScriptItemRecord();
                itemBinding.Name = itemName;
                itemBinding.CanSell = MainPayload[offset] != 0;
                itemBinding.Data1 = ReadInt32(MainPayload, offset + 1);
                itemBinding.Data2 = ReadInt32(MainPayload, offset + 5);
                itemBinding.Data3 = ReadInt32(MainPayload, offset + 9);
                offset += 13;
                if (!TryReadItemString(ref offset, 4096, out itemBinding.TextData1) ||
                    !TryReadItemString(ref offset, 4096, out itemBinding.TextData2) ||
                    !TryReadItemString(ref offset, 4096, out itemBinding.TextData3) ||
                    !TryReadItemString(ref offset, 262144, out itemBinding.OnUseCode) ||
                    !TryReadItemString(ref offset, 262144, out itemBinding.OnActCode))
                    return FailScriptParse("item strings " + item, offset);
                if (offset > MainPayload.Length - 4) return FailScriptParse("item reference " + item, offset);
                uint itemId = ReadUInt32(MainPayload, offset);
                offset += 4;
                if (itemId >= nextObjectId) return FailScriptParse("item object id " + itemId, offset - 4);
                itemBinding.ItemObjectId = itemId;
                items.Add(itemBinding);
            }

            if (offset > MainPayload.Length - 2) return FailScriptParse("ships count", offset);
            int shipCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (shipCount > 10000) return FailScriptParse("ships count " + shipCount, offset - 2);
            List<ScriptShipRecord> ships = new List<ScriptShipRecord>();
            for (int ship = 0; ship < shipCount; ship++)
            {
                if (offset > MainPayload.Length - 28) return FailScriptParse("ship " + ship, offset);
                uint shipId = ReadUInt32(MainPayload, offset + 4);
                if (shipId >= nextObjectId) return FailScriptParse("ship object id " + shipId, offset + 4);
                ScriptShipRecord shipBinding = new ScriptShipRecord();
                shipBinding.Group = ReadInt32(MainPayload, offset);
                shipBinding.ShipObjectId = shipId;
                shipBinding.Data0 = ReadUInt32(MainPayload, offset + 8);
                shipBinding.Data1 = ReadUInt32(MainPayload, offset + 12);
                shipBinding.Data2 = ReadUInt32(MainPayload, offset + 16);
                shipBinding.Data3 = ReadUInt32(MainPayload, offset + 20);
                shipBinding.StateNum = ReadInt32(MainPayload, offset + 24);
                offset += 28;
                if (!TryReadItemString(ref offset, 4096, out shipBinding.CustomFaction) ||
                    offset > MainPayload.Length - 2) return FailScriptParse("ship faction " + ship, offset);
                if (MainPayload[offset] > 1 || MainPayload[offset + 1] > 1)
                    return FailScriptParse("ship flags " + ship, offset);
                shipBinding.Hit = MainPayload[offset] != 0;
                shipBinding.HitPlayer = MainPayload[offset + 1] != 0;
                offset += 2;
                ships.Add(shipBinding);
            }

            if (offset > MainPayload.Length - 2) return FailScriptParse("ethers count", offset);
            int etherCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (etherCount > 10000) return FailScriptParse("ethers count " + etherCount, offset - 2);
            List<string> ethers = new List<string>();
            for (int index = 0; index < etherCount; index++)
            {
                string ether;
                if (!TryReadItemString(ref offset, 262144, out ether))
                    return FailScriptParse("ether " + index, offset);
                ethers.Add(ether);
            }

            ScriptRecord script = new ScriptRecord();
            script.Start = start;
            script.End = offset;
            script.Name = name;
            script.OldEthers = oldEthers;
            script.InitVariables = initVariables;
            script.TurnVariables = turnVariables;
            script.StarBindings = stars;
            script.ItemBindings = items;
            script.ShipBindings = ships;
            script.EtherStrings = ethers;
            value = script;
            return true;
        }

        private bool TryReadScriptVariableArray(ref int offset, int depth, bool wideCount,
            out List<ScriptVariableRecord> values)
        {
            values = null;
            int countWidth = wideCount ? 4 : 2;
            if (depth > 16 || offset > MainPayload.Length - countWidth) return false;
            int count = wideCount ? ReadInt32(MainPayload, offset) :
                MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += countWidth;
            if (count > 10000) return false;
            if (count < 0) return false;
            List<ScriptVariableRecord> parsed = new List<ScriptVariableRecord>(count);
            for (int index = 0; index < count; index++)
            {
                string name;
                if (!TryReadItemString(ref offset, 4096, out name) || offset >= MainPayload.Length) return false;
                byte type = MainPayload[offset++];
                ScriptVariableRecord value = new ScriptVariableRecord();
                value.Name = name;
                value.Type = type;
                switch (type)
                {
                    case 0: break;
                    case 1:
                    case 2:
                        if (offset > MainPayload.Length - 4) return false;
                        value.IntegerValue = ReadInt32(MainPayload, offset);
                        offset += 4;
                        break;
                    case 3:
                        if (offset > MainPayload.Length - 8) return false;
                        value.DoubleValue = BitConverter.ToDouble(MainPayload, offset);
                        offset += 8;
                        break;
                    case 4:
                        if (!TryReadItemString(ref offset, 4096, out value.StringValue)) return false;
                        break;
                    case 6:
                        if (!wideCount && !TryReadItemString(ref offset, 4096,
                            out value.StringValue)) return false;
                        break;
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                        // TVarEC stream format has no payload for these types.
                        break;
                    case 9:
                        if (!TryReadScriptVariableArray(ref offset, depth + 1, true,
                            out value.ArrayValue)) return false;
                        break;
                    default:
                        return false;
                }
                parsed.Add(value);
            }
            values = parsed;
            return true;
        }

        private bool TryReadScriptIntCount(ref int offset, int maximum, out int value)
        {
            value = 0;
            if (offset > MainPayload.Length - 4) return false;
            value = ReadInt32(MainPayload, offset);
            offset += 4;
            return value >= 0 && value <= maximum;
        }

        private bool TryReadWarOperations(ref int offset, int expectedEnd)
        {
            List<WarOperationRecord> ignored;
            return TryReadWarOperations(ref offset, expectedEnd, out ignored);
        }

        private bool TryReadWarOperations(ref int offset, int expectedEnd,
            out List<WarOperationRecord> records)
        {
            records = null;
            if (offset > expectedEnd - 2) return false;
            int count = MainPayload[offset] | MainPayload[offset + 1] << 8;
            offset += 2;
            if (count > 10000) return false;
            List<WarOperationRecord> parsed = new List<WarOperationRecord>(count);
            for (int operation = 0; operation < count; operation++)
            {
                if (offset > expectedEnd - 13 || MainPayload[offset + 10] != 0) return false;
                WarOperationRecord record = new WarOperationRecord();
                record.Start = offset;
                record.Turn = (ushort)(MainPayload[offset] | MainPayload[offset + 1] << 8);
                record.RandomSeed = ReadUInt32(MainPayload, offset + 2);
                record.RandomOut = ReadUInt32(MainPayload, offset + 6);
                record.LegacyZero = MainPayload[offset + 10];
                offset += 11;
                int referenceCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
                offset += 2;
                if (referenceCount > 10000 || offset > expectedEnd - referenceCount * 4) return false;
                for (int reference = 0; reference < referenceCount; reference++)
                {
                    record.ShipObjectIds.Add(ReadUInt32(MainPayload, offset));
                    offset += 4;
                }
                if (offset > expectedEnd - 2) return false;
                int actionCount = MainPayload[offset] | MainPayload[offset + 1] << 8;
                offset += 2;
                if (actionCount > 10000 || offset > expectedEnd - actionCount * 18) return false;
                for (int action = 0; action < actionCount; action++)
                {
                    byte targetType = MainPayload[offset];
                    if (targetType > 7) return false;
                    float scalar1 = BitConverter.ToSingle(MainPayload, offset + 5);
                    float scalar2 = BitConverter.ToSingle(MainPayload, offset + 9);
                    if (float.IsNaN(scalar1) || float.IsInfinity(scalar1) ||
                        float.IsNaN(scalar2) || float.IsInfinity(scalar2)) return false;
                    WarOperationOrderRecord order = new WarOperationOrderRecord();
                    order.Type = targetType;
                    order.ObjectId = ReadUInt32(MainPayload, offset + 1);
                    order.DestinationX = scalar1;
                    order.DestinationY = scalar2;
                    order.EndMode = MainPayload[offset + 13];
                    order.EndTurn = ReadInt32(MainPayload, offset + 14);
                    record.Orders.Add(order);
                    offset += 18;
                }
                record.End = offset;
                parsed.Add(record);
            }
            if (offset != expectedEnd) return false;
            records = parsed;
            return true;
        }

        private static bool IsSupportedObjectTextCharacter(char value)
        {
            return (value >= ' ' && value <= '~') || (value >= '\u0400' && value <= '\u052F');
        }

        private static bool IsSupportedObjectCoordinate(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= -10000 && value <= 10000 &&
                (value == 0 || Math.Abs(value) >= 0.001f);
        }

        private static bool IsSupportedShipCoordinate(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= -10000 && value <= 10000 &&
                (value == 0 || Math.Abs(value) >= 0.0000001f);
        }

        private static bool IsFiniteGalaxyScalar(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && Math.Abs((double)value) <= 1.0E9;
        }

        private void LocateGalaxyReferenceLists(GalaxySummaryData summary)
        {
            int planetCount = 0;
            foreach (StarHeaderRecord star in GalaxyStars)
                planetCount = checked(planetCount + star.PlanetCount);
            if (planetCount < 1 || planetCount > 10000)
                throw new SavFormatException("TGalaxy.Planets: недопустимое суммарное число планет " + planetCount + ".");

            int selectedPlanetOffset = -1;
            int selectedRangerOffset = -1;
            int selectedRangerCount = -1;
            int selectedCount = 0;
            for (int candidate = GalaxyStarsOffset; candidate <= MainPayload.Length - 4; candidate++)
            {
                int storedPlanetCount = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                if (storedPlanetCount != planetCount) continue;
                int rangerCountOffset;
                try { rangerCountOffset = checked(candidate + 2 + planetCount * 4); }
                catch (OverflowException) { continue; }
                if (rangerCountOffset > MainPayload.Length - 2) continue;

                bool valid = true;
                uint previousPlanetId = 0;
                for (int index = 0; index < planetCount; index++)
                {
                    uint objectId = ReadUInt32(MainPayload, candidate + 2 + index * 4);
                    if (objectId == 0 || objectId > 10000000 || objectId <= previousPlanetId)
                    {
                        valid = false;
                        break;
                    }
                    previousPlanetId = objectId;
                }
                if (!valid) continue;

                int rangerCount = MainPayload[rangerCountOffset] | MainPayload[rangerCountOffset + 1] << 8;
                if (rangerCount > 10000) continue;
                int rangerIdsOffset = rangerCountOffset + 2;
                int rangerEnd;
                try { rangerEnd = checked(rangerIdsOffset + rangerCount * 4); }
                catch (OverflowException) { continue; }
                if (rangerEnd > MainPayload.Length) continue;
                uint previousRangerId = 0;
                for (int index = 0; index < rangerCount; index++)
                {
                    uint objectId = ReadUInt32(MainPayload, rangerIdsOffset + index * 4);
                    if (objectId == 0 || objectId > 10000000 || objectId <= previousRangerId)
                    {
                        valid = false;
                        break;
                    }
                    previousRangerId = objectId;
                }
                if (!valid) continue;

                selectedPlanetOffset = candidate;
                selectedRangerOffset = rangerCountOffset;
                selectedRangerCount = rangerCount;
                selectedCount++;
            }
            if (selectedCount != 1)
                throw new SavFormatException("TGalaxy.Planets/Rangers: ожидался один структурный span, найдено " + selectedCount + ".");
            summary.PlanetReferenceListOffset = selectedPlanetOffset;
            summary.RangerReferenceListOffset = selectedRangerOffset;
            summary.RangerCount = selectedRangerCount;
            summary.RangerObjectIds = new uint[selectedRangerCount];
            for (int index = 0; index < selectedRangerCount; index++)
                summary.RangerObjectIds[index] = ReadUInt32(MainPayload,
                    selectedRangerOffset + 2 + index * 4);
            int kellerOffset = checked(selectedRangerOffset + 2 + selectedRangerCount * 4);
            if (Version > 132) kellerOffset = checked(kellerOffset + 20);
            if (Version < 102)
                throw new SavFormatException("TGalaxy.KellerAttack: старая схема flagmans не поддерживается.");
            if (kellerOffset > MainPayload.Length - 8)
                throw new SavFormatException("TGalaxy.KellerAttack: блок выходит за границу payload.");
            summary.KellerAttackOffset = kellerOffset;
            summary.KellerAttackStarObjectId = ReadUInt32(MainPayload, kellerOffset);
            summary.KellerAttackState = ReadInt32(MainPayload, kellerOffset + 4);
            bool knownStar = summary.KellerAttackStarObjectId == 0;
            foreach (StarHeaderRecord star in GalaxyStars)
                if (star.ObjectId == summary.KellerAttackStarObjectId) { knownStar = true; break; }
            if (!knownStar)
                throw new SavFormatException("TGalaxy.KellerAttack: ссылка ведёт на неизвестную систему " +
                    summary.KellerAttackStarObjectId + ".");
        }

        private void LocatePlayerPlanetBattleFlag(GalaxySummaryData summary)
        {
            int selectedOffset = -1;
            int selectedCount = 0;
            for (int candidate = GalaxyOffset; candidate <= MainPayload.Length - 1818; candidate++)
            {
                if (MainPayload[candidate] > 1 || MainPayload[candidate + 16] != 10) continue;
                bool matches = true;
                for (int block = 0; block < 10; block++)
                {
                    int blockOffset = candidate + 17 + block * 180;
                    if (MainPayload[blockOffset] != 12 || MainPayload[blockOffset + 1] != 0 ||
                        MainPayload[blockOffset + 50] != 32 || MainPayload[blockOffset + 51] != 0)
                    {
                        matches = false;
                        break;
                    }
                }
                if (!matches) continue;
                selectedOffset = candidate;
                selectedCount++;
            }
            if (selectedCount != 1)
                throw new SavFormatException("TPlayer.PlanetBattlesDisabled: ожидался один структурный span, найдено " + selectedCount + ".");
            summary.PlanetBattlesDisabledOffset = selectedOffset;
            summary.PlanetBattlesDisabled = MainPayload[selectedOffset] != 0;
        }

        private bool TryParseGalaxySummaryCandidate(int turnOffset, int expectedTurn, out GalaxySummaryData value)
        {
            value = null;
            try
            {
                int offset = turnOffset - 6;
                GalaxySummaryData result = new GalaxySummaryData();
                result.TurnOffset = turnOffset;
                result.PirateCount = checked((ushort)ReadUInt16(MainPayload, ref offset, "galaxy pirate count"));
                result.ClanPirateCount = checked((ushort)ReadUInt16(MainPayload, ref offset, "galaxy clan pirate count"));
                result.TransportCount = checked((ushort)ReadUInt16(MainPayload, ref offset, "galaxy transport count"));
                if (result.PirateCount > 10000 || result.ClanPirateCount > 10000 || result.TransportCount > 10000)
                    return false;
                result.Turn = ReadUInt32(MainPayload, ref offset, "galaxy turn");
                if (result.Turn != (uint)expectedTurn)
                    return false;

                result.DifficultyOffset = offset;
                result.DifficultyLevels = new byte[8];
                for (int index = 0; index < result.DifficultyLevels.Length; index++)
                {
                    result.DifficultyLevels[index] = ReadByte(MainPayload, ref offset, "galaxy difficulty level");
                    if (result.DifficultyLevels[index] > 16) return false;
                }

                result.PrincipalObjectOffset = offset;
                uint[] objectIds = new uint[10];
                for (int index = 0; index < objectIds.Length; index++)
                {
                    objectIds[index] = ReadUInt32(MainPayload, ref offset, "galaxy principal object id");
                    if (objectIds[index] > 10000000) return false;
                }
                if (objectIds[0] == 0 || objectIds[2] == 0 || objectIds[3] == 0 || objectIds[4] == 0 || objectIds[5] == 0)
                    return false;
                result.PlayerObjectId = objectIds[0];
                result.AutoBattleShipObjectId = objectIds[1];
                result.BlazerObjectId = objectIds[2];
                result.KellerObjectId = objectIds[3];
                result.TerronObjectId = objectIds[4];
                result.CurrentStarObjectId = objectIds[5];
                result.TerronStarObjectId = objectIds[6];
                result.EminentRangerObjectIds = new uint[] { objectIds[7], objectIds[8], objectIds[9] };

                result.CompleteQuestListOffset = offset;
                result.CompleteQuestCount = ReadBoundedCount(MainPayload, ref offset, "complete quest count");
                for (int index = 0; index < result.CompleteQuestCount; index++)
                {
                    CompleteQuestRecord record = new CompleteQuestRecord();
                    record.Start = offset;
                    record.PlanetObjectId = ReadUInt32(MainPayload, ref offset, "complete quest planet id");
                    record.Type = ReadByte(MainPayload, ref offset, "complete quest type");
                    record.Number = checked((ushort)ReadUInt16(MainPayload, ref offset,
                        "complete quest number"));
                    record.Text = ReadUtf16Z(MainPayload, ref offset, "complete quest text");
                    record.Successful = ReadBoolean(MainPayload, ref offset, "complete quest successful");
                    record.Rejection = ReadBoolean(MainPayload, ref offset, "complete quest rejection");
                    record.End = offset;
                    if (record.Text.Length > 32768) return false;
                    result.CompleteQuests.Add(record);
                }

                result.GalaxyNewsListOffset = offset;
                result.GalaxyNewsCount = ReadBoundedCount(MainPayload, ref offset, "galaxy news count");
                for (int index = 0; index < result.GalaxyNewsCount; index++)
                {
                    GalaxyNewsRecord record = new GalaxyNewsRecord();
                    record.Start = offset;
                    record.Id = ReadUInt32(MainPayload, ref offset, "galaxy news id");
                    record.Turn = ReadUInt32(MainPayload, ref offset, "galaxy news date");
                    record.Type = ReadByte(MainPayload, ref offset, "galaxy news type");
                    record.Text = ReadUtf16Z(MainPayload, ref offset, "galaxy news text");
                    record.End = offset;
                    if (record.Text.Length > 32768) return false;
                    result.GalaxyNews.Add(record);
                }

                result.LateScalarOffset = offset;
                result.DayShipsNotTalkWithPlayer = ReadUInt32(MainPayload, ref offset,
                    "day ships not talk with player");
                result.DayShipsNotGreetingPlayer = ReadUInt32(MainPayload, ref offset,
                    "day ships not greeting special with player");
                result.OpenCommunicator = ReadSingle(MainPayload, ref offset, "open communicator");
                result.BlazerResearch = ReadSingle(MainPayload, ref offset, "Blazer research");
                result.BlazerMaterial = ReadUInt32(MainPayload, ref offset, "Blazer material");
                result.KellerResearch = ReadSingle(MainPayload, ref offset, "Keller research");
                result.KellerMaterial = ReadUInt32(MainPayload, ref offset, "Keller material");
                result.TerronResearch = ReadSingle(MainPayload, ref offset, "Terron research");
                result.TerronMaterial = ReadUInt32(MainPayload, ref offset, "Terron material");
                result.ScienceBaseWorkPercentOld = ReadSingle(MainPayload, ref offset,
                    "science base work percent old");
                result.WarDeltaDominators = ReadInt32(MainPayload, ref offset,
                    "war delta win dominators");
                result.WarDeltaPirates = ReadInt32(MainPayload, ref offset, "war delta win pirates");
                result.WarDeltaCoalition = ReadInt32(MainPayload, ref offset,
                    "war delta win coalition");
                if (!IsFiniteGalaxyScalar(result.OpenCommunicator) ||
                    !IsFiniteGalaxyScalar(result.BlazerResearch) ||
                    !IsFiniteGalaxyScalar(result.KellerResearch) ||
                    !IsFiniteGalaxyScalar(result.TerronResearch) ||
                    !IsFiniteGalaxyScalar(result.ScienceBaseWorkPercentOld))
                    return false;
                uint cheatControlLength = ReadUInt32(MainPayload, ref offset, "cheat controls length");
                if (cheatControlLength > int.MaxValue) return false;
                Skip(MainPayload, ref offset, checked((int)cheatControlLength), "cheat controls");
                result.GarbageCount = ReadInt32(MainPayload, ref offset, "galaxy garbage count");
                if (result.GarbageCount < 0 || result.GarbageCount > 10000) return false;
                Skip(MainPayload, ref offset, checked(result.GarbageCount * 48), "galaxy garbage records");

                result.HangarOffset = offset;
                result.HangarShipObjectIds = new uint[9];
                for (int index = 0; index < result.HangarShipObjectIds.Length; index++)
                {
                    result.HangarShipObjectIds[index] = ReadUInt32(MainPayload, ref offset, "hangar ship id");
                    if (result.HangarShipObjectIds[index] > 10000000) return false;
                }
                result.CheatsOffset = offset;
                result.CheatsUpdate = ReadInt32(MainPayload, ref offset, "cheats update");
                result.CheatsAssigned = ReadInt32(MainPayload, ref offset, "cheats assigned");
                result.CheatsTestOffset = offset;
                result.CheatsTest = ReadInt32(MainPayload, ref offset, "cheats test");
                result.CheatsValue = ReadInt32(MainPayload, ref offset, "cheats value");

                int changedShipCount = ReadBoundedCount(MainPayload, ref offset, "changed-star ship count");
                Skip(MainPayload, ref offset, checked(changedShipCount * 4), "changed-star ship ids");
                result.PostCheatsOffset = offset;
                for (int index = 0; index < 5; index++) ReadInt32(MainPayload, ref offset, "late boss scalar");
                ReadUInt32(MainPayload, ref offset, "Keller new research object id");
                ReadUInt32(MainPayload, ref offset, "Blazer landing object id");
                ReadInt32(MainPayload, ref offset, "Blazer self destruction turn");
                result.TerronTurnWin = ReadInt32(MainPayload, ref offset, "Terron turn win");
                result.KellerTurnWin = ReadInt32(MainPayload, ref offset, "Keller turn win");
                result.BlazerTurnWin = ReadInt32(MainPayload, ref offset, "Blazer turn win");
                result.PirateTurnWin = ReadInt32(MainPayload, ref offset, "pirate turn win");
                result.PirateWinType = ReadInt32(MainPayload, ref offset, "pirate win type");
                result.CoalitionDefeatedTurn = ReadInt32(MainPayload, ref offset, "coalition defeated turn");
                result.GraphDominator = ReadBoolean(MainPayload, ref offset, "graph dominator");
                result.Gluk = ReadByte(MainPayload, ref offset, "galaxy gluk");
                result.IronWillOffset = offset;
                result.IronWill = ReadBoolean(MainPayload, ref offset, "iron will");
                for (int index = 0; index < 6; index++)
                    ReadByte(MainPayload, ref offset, "legacy mod flag");
                result.PlanetNewsObjectId = ReadUInt32(MainPayload, ref offset, "planet news id");
                result.NextSpecialShipTurn = ReadInt32(MainPayload, ref offset, "next special ship turn");
                result.GalaxyEventListOffset = offset;
                result.GalaxyEventCount = ReadBoundedCount(MainPayload, ref offset, "galaxy event count");
                for (int eventIndex = 0; eventIndex < result.GalaxyEventCount; eventIndex++)
                {
                    GalaxyEventRecord galaxyEvent = new GalaxyEventRecord();
                    galaxyEvent.Start = offset;
                    galaxyEvent.Type = ReadUtf16Z(MainPayload, ref offset, "galaxy event type");
                    if (string.IsNullOrEmpty(galaxyEvent.Type) || galaxyEvent.Type.Length > 128) return false;
                    galaxyEvent.Turn = ReadInt32(MainPayload, ref offset, "galaxy event turn");
                    int dataCount = ReadInt32(MainPayload, ref offset, "galaxy event data count");
                    if (dataCount < 0 || dataCount > 10000) return false;
                    for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                        galaxyEvent.Data.Add(ReadInt32(MainPayload, ref offset, "galaxy event data"));
                    int textDataCount = ReadInt32(MainPayload, ref offset, "galaxy event text data count");
                    if (textDataCount < 0 || textDataCount > 10000) return false;
                    for (int textIndex = 0; textIndex < textDataCount; textIndex++)
                        galaxyEvent.TextData.Add(ReadUtf16Z(MainPayload, ref offset, "galaxy event text data"));
                    galaxyEvent.End = offset;
                    result.GalaxyEvents.Add(galaxyEvent);
                }
                result.GalaxyEventListEndOffset = offset;

                result.InterfaceOverrideListOffsets[0] = offset;
                int interfaceStateCount = ReadBoundedCount(MainPayload, ref offset, "interface state count");
                for (int index = 0; index < interfaceStateCount; index++)
                {
                    InterfaceOverrideRecord record = new InterfaceOverrideRecord();
                    record.Start = offset; record.Kind = InterfaceOverrideKind.State;
                    record.ModuleName = ReadUtf16Z(MainPayload, ref offset, "interface state ML name");
                    record.GuiName = ReadUtf16Z(MainPayload, ref offset, "interface state GI name");
                    record.NewState = ReadByte(MainPayload, ref offset, "interface new state");
                    record.OldState = ReadByte(MainPayload, ref offset, "interface old state");
                    if (record.NewState > 3 || record.OldState > 3) return false;
                    record.End = offset; result.InterfaceOverrides.Add(record);
                }
                result.InterfaceOverrideListEndOffsets[0] = offset;
                result.InterfaceOverrideListOffsets[1] = offset;
                int interfaceTextCount = ReadBoundedCount(MainPayload, ref offset, "interface text count");
                for (int index = 0; index < interfaceTextCount; index++)
                {
                    InterfaceOverrideRecord record = new InterfaceOverrideRecord();
                    record.Start = offset; record.Kind = InterfaceOverrideKind.Text;
                    record.ModuleName = ReadUtf16Z(MainPayload, ref offset, "interface text ML name");
                    record.GuiName = ReadUtf16Z(MainPayload, ref offset, "interface text GI name");
                    record.NewValue = ReadUtf16Z(MainPayload, ref offset, "interface new text");
                    record.OldValue = ReadUtf16Z(MainPayload, ref offset, "interface old text");
                    record.End = offset; result.InterfaceOverrides.Add(record);
                }
                result.InterfaceOverrideListEndOffsets[1] = offset;
                result.InterfaceOverrideListOffsets[2] = offset;
                int interfaceImageCount = ReadBoundedCount(MainPayload, ref offset, "interface image count");
                for (int index = 0; index < interfaceImageCount; index++)
                {
                    InterfaceOverrideRecord record = new InterfaceOverrideRecord();
                    record.Start = offset; record.Kind = InterfaceOverrideKind.Image;
                    record.ModuleName = ReadUtf16Z(MainPayload, ref offset, "interface image ML name");
                    record.GuiName = ReadUtf16Z(MainPayload, ref offset, "interface image GI name");
                    record.NewValue = ReadUtf16Z(MainPayload, ref offset, "interface new image");
                    record.OldValue = ReadUtf16Z(MainPayload, ref offset, "interface old image");
                    record.End = offset; result.InterfaceOverrides.Add(record);
                }
                result.InterfaceOverrideListEndOffsets[2] = offset;
                result.InterfaceOverrideListOffsets[3] = offset;
                int interfacePositionCount = ReadBoundedCount(MainPayload, ref offset, "interface position count");
                for (int index = 0; index < interfacePositionCount; index++)
                {
                    InterfaceOverrideRecord record = new InterfaceOverrideRecord();
                    record.Start = offset; record.Kind = InterfaceOverrideKind.Position;
                    record.ModuleName = ReadUtf16Z(MainPayload, ref offset, "interface position ML name");
                    record.GuiName = ReadUtf16Z(MainPayload, ref offset, "interface position GI name");
                    record.NewX = ReadInt32(MainPayload, ref offset, "interface position new x");
                    record.NewY = ReadInt32(MainPayload, ref offset, "interface position new y");
                    record.NewZ = ReadDouble(MainPayload, ref offset, "interface position new z");
                    record.OldX = ReadInt32(MainPayload, ref offset, "interface position old x");
                    record.OldY = ReadInt32(MainPayload, ref offset, "interface position old y");
                    record.OldZ = ReadDouble(MainPayload, ref offset, "interface position old z");
                    if (double.IsNaN(record.NewZ) || double.IsInfinity(record.NewZ) ||
                        double.IsNaN(record.OldZ) || double.IsInfinity(record.OldZ)) return false;
                    record.End = offset; result.InterfaceOverrides.Add(record);
                }
                result.InterfaceOverrideListEndOffsets[3] = offset;
                result.InterfaceOverrideListOffsets[4] = offset;
                int interfaceSizeCount = ReadBoundedCount(MainPayload, ref offset, "interface size count");
                for (int index = 0; index < interfaceSizeCount; index++)
                {
                    InterfaceOverrideRecord record = new InterfaceOverrideRecord();
                    record.Start = offset; record.Kind = InterfaceOverrideKind.Size;
                    record.ModuleName = ReadUtf16Z(MainPayload, ref offset, "interface size ML name");
                    record.GuiName = ReadUtf16Z(MainPayload, ref offset, "interface size GI name");
                    record.NewX = ReadInt32(MainPayload, ref offset, "interface size new x");
                    record.NewY = ReadInt32(MainPayload, ref offset, "interface size new y");
                    record.OldX = ReadInt32(MainPayload, ref offset, "interface size old x");
                    record.OldY = ReadInt32(MainPayload, ref offset, "interface size old y");
                    record.End = offset; result.InterfaceOverrides.Add(record);
                }
                result.InterfaceOverrideListEndOffsets[4] = offset;
                result.CurrentObjectId = ReadUInt32(MainPayload, ref offset, "galaxy current object id");
                result.NextObjectId = ReadUInt32(MainPayload, ref offset, "galaxy next object id");
                result.SystemCrc = ReadUInt32(MainPayload, ref offset, "galaxy system CRC");
                if (result.NextObjectId < 2 || result.NextObjectId > 10000000 ||
                    result.CurrentObjectId >= result.NextObjectId)
                    return false;
                result.PrepareToDump = ReadBoolean(MainPayload, ref offset, "prepare to dump");
                result.DumpName = ReadUtf16Z(MainPayload, ref offset, "dump name");
                result.CustomRulesOffset = offset;
                result.CustomRules = ReadBoolean(MainPayload, ref offset, "custom rules");
                result.CustomRuleLevels = new byte[19];
                for (int index = 0; index < result.CustomRuleLevels.Length; index++)
                    result.CustomRuleLevels[index] = ReadByte(MainPayload, ref offset,
                        "custom rule byte level");
                result.CustomRuleFlags = new bool[15];
                for (int index = 0; index < result.CustomRuleFlags.Length; index++)
                    result.CustomRuleFlags[index] = ReadBoolean(MainPayload, ref offset,
                        "custom rule Boolean");
                result.HullGrowth = ReadByte(MainPayload, ref offset, "custom rule hull growth");
                result.CustomRuleLateFlags = new bool[8];
                for (int index = 0; index < result.CustomRuleLateFlags.Length; index++)
                    result.CustomRuleLateFlags[index] = ReadBoolean(MainPayload, ref offset,
                        "custom rule late Boolean");
                result.End = offset;
                value = result;
                return true;
            }
            catch (SavFormatException) { return false; }
            catch (OverflowException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private StarHeaderRecord TryReadStarHeader(int start, uint expectedId)
        {
            try
            {
                int offset = start;
                if (ReadUInt32(MainPayload, ref offset, "star id") != expectedId) return null;
                StarHeaderRecord value = new StarHeaderRecord();
                value.AsteroidCountOffset = -1;
                value.SpaceShipCountOffset = -1;
                value.MissileCountOffset = -1;
                value.SpaceItemCountOffset = -1;
                value.DropItemCountOffset = -1;
                value.MissileListOffset = -1;
                value.Start = start;
                value.ObjectId = expectedId;
                value.Raw08 = ReadInt32(MainPayload, ref offset, "star raw 08");
                value.Raw0C = ReadUInt32(MainPayload, ref offset, "star raw 0C");
                value.Name = ReadUtf16Z(MainPayload, ref offset, "star name");
                if (!IsSupportedStarName(value.Name)) return null;
                value.X = ReadSingle(MainPayload, ref offset, "star x");
                value.Y = ReadSingle(MainPayload, ref offset, "star y");
                value.Raw1C = checked((ushort)ReadUInt16(MainPayload, ref offset, "star raw 1C"));
                value.Raw78 = ReadByte(MainPayload, ref offset, "star raw 78");
                value.PlanetCount = checked((ushort)ReadUInt16(MainPayload, ref offset, "star planet count"));
                if (!IsSupportedStarCoordinate(value.X) || !IsSupportedStarCoordinate(value.Y) ||
                    value.Raw1C < 200 || value.Raw1C > 300 || value.PlanetCount < 1 || value.PlanetCount > 64)
                    return null;
                value.HeaderEnd = offset;
                return value;
            }
            catch (SavFormatException)
            {
                return null;
            }
        }

        private void ParseStarTails(GalaxySummaryData summary)
        {
            HashSet<uint> constellationIds = new HashSet<uint>();
            foreach (ConstellationRecord constellation in GalaxyConstellations)
                constellationIds.Add(constellation.ObjectId);
            HashSet<uint> starIds = new HashSet<uint>();
            foreach (StarHeaderRecord star in GalaxyStars) starIds.Add(star.ObjectId);

            for (int starIndex = 0; starIndex < GalaxyStars.Count; starIndex++)
            {
                StarHeaderRecord star = GalaxyStars[starIndex];
                bool exactEnd = starIndex + 1 < GalaxyStars.Count || GalaxyHoles.Count > 0;
                int end;
                if (starIndex + 1 < GalaxyStars.Count)
                    end = GalaxyStars[starIndex + 1].Start;
                else if (GalaxyHoles.Count > 0)
                {
                    end = GalaxyHoles[0].Start - 2;
                    foreach (HoleRecord hole in GalaxyHoles)
                        if (hole.Start - 2 < end) end = hole.Start - 2;
                }
                else
                    end = summary.PlanetReferenceListOffset;
                StarHeaderRecord selected = null;
                int matches = 0;
                List<string> matchDetails = new List<string>();
                for (int candidate = star.HeaderEnd; candidate <= end - 48; candidate++)
                {
                    if (!constellationIds.Contains(ReadUInt32(MainPayload, candidate))) continue;
                    StarHeaderRecord parsed;
                    if (!TryReadStarTail(star, candidate, end, exactEnd, constellationIds, starIds,
                        out parsed)) continue;
                    selected = parsed;
                    matches++;
                    if (matchDetails.Count < 80)
                        matchDetails.Add("0x" + candidate.ToString("X") + "->0x" + parsed.TailEnd.ToString("X") +
                            " graph=" + parsed.GraphStar + " map=" + parsed.MapLabel);
                }
                if (matches == 0 || exactEnd && matches != 1)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": ожидался один точный хвост status/custom-info, найдено " + matches +
                        " (" + string.Join("; ", matchDetails.ToArray()) + ").");
                CopyStarTail(selected, star);
            }
        }

        private bool TryReadStarTail(StarHeaderRecord source, int start, int end, bool exactEnd,
            HashSet<uint> constellationIds, HashSet<uint> starIds, out StarHeaderRecord value)
        {
            value = null;
            try
            {
                int offset = start;
                StarHeaderRecord result = source.Clone();
                result.CustomSystemInfos.Clear();
                result.TailStart = start;
                result.ConstellationObjectId = ReadUInt32(MainPayload, ref offset, "star constellation");
                if (!constellationIds.Contains(result.ConstellationObjectId)) return false;
                if (!TryReadItemString(ref offset, 256, out result.GraphType) || offset > end - 40) return false;
                result.Battle = ReadBoolean(MainPayload, ref offset, "star battle");
                result.Safety = ReadByte(MainPayload, ref offset, "star safety");
                result.Overloading = ReadByte(MainPayload, ref offset, "star overloading");
                result.Owners = ReadByte(MainPayload, ref offset, "star owners");
                result.LastOwners = ReadByte(MainPayload, ref offset, "star last owners");
                result.DominatorSeries = ReadByte(MainPayload, ref offset, "star dominator series");
                if (result.Safety > 100 || result.Owners > 2 || result.LastOwners > 2 ||
                    result.DominatorSeries > 2) return false;
                if (!TryReadItemString(ref offset, 4096, out result.CustomFaction) || offset > end - 34)
                    return false;
                result.SafeRadius = ReadSingle(MainPayload, ref offset, "star safe radius");
                result.DamageRadius = ReadSingle(MainPayload, ref offset, "star damage radius");
                if (!IsSupportedStarScalar(result.SafeRadius) || !IsSupportedStarScalar(result.DamageRadius))
                    return false;
                result.GraphRadius = checked((ushort)ReadUInt16(MainPayload, ref offset, "star graph radius"));
                if (!TryReadItemString(ref offset, 4096, out result.GraphStar) || offset > end - 24) return false;
                if (!IsSupportedStarGraphName(result.GraphStar)) return false;
                if (!exactEnd && !result.GraphStar.StartsWith("Star.", StringComparison.OrdinalIgnoreCase))
                    return false;
                result.WarPlayer = ReadBoolean(MainPayload, ref offset, "star war player");
                result.DayBeforeOccupy = ReadByte(MainPayload, ref offset, "star day before occupy");
                result.DayWithoutPlayer = ReadInt32(MainPayload, ref offset, "star day without player");
                result.DayWithoutCreateShip = ReadInt32(MainPayload, ref offset, "star day without create ship");
                result.LastDominatorDate = ReadInt32(MainPayload, ref offset, "star last dominator date");
                result.LastPirateDate = ReadInt32(MainPayload, ref offset, "star last pirate date");
                result.LiberationDate = ReadInt32(MainPayload, ref offset, "star liberation date");
                result.DayInvadeInertia = ReadInt32(MainPayload, ref offset, "star day invade inertia");
                result.NoComeKling = ReadBoolean(MainPayload, ref offset, "star no come kling");
                result.DominionObjectId = ReadUInt32(MainPayload, ref offset, "star dominion");
                if (result.DominionObjectId > 10000000) return false;
                if (!TryReadItemString(ref offset, 4096, out result.MapLabel) || offset > end - 2) return false;
                result.CustomInfoCountOffset = offset;
                int count = ReadUInt16(MainPayload, ref offset, "star custom info count");
                if (count > 10000) return false;
                for (int index = 0; index < count; index++)
                {
                    CustomSystemInfoRecord record = new CustomSystemInfoRecord();
                    record.Start = offset;
                    if (!TryReadItemString(ref offset, 32768, out record.Name) ||
                        !TryReadItemString(ref offset, 32768, out record.Icon) ||
                        !TryReadItemString(ref offset, 32768, out record.Info) ||
                        !TryReadItemString(ref offset, 32768, out record.Type) || offset > end - 4)
                        return false;
                    record.Distance = ReadInt32(MainPayload, ref offset, "star custom info distance");
                    record.End = offset;
                    result.CustomSystemInfos.Add(record);
                }
                if (exactEnd)
                {
                    if (offset != end) return false;
                }
                else
                {
                    if (offset > end - 2) return false;
                }
                result.TailEnd = offset;
                value = result;
                return true;
            }
            catch (SavFormatException) { return false; }
            catch (OverflowException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private static bool IsSupportedStarScalar(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && Math.Abs((double)value) <= 1.0E9;
        }

        private static bool IsSupportedStarGraphName(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3 || value.Length > 256) return false;
            foreach (char character in value)
                if (character < 0x21 || character > 0x7e) return false;
            return true;
        }

        private static void CopyStarTail(StarHeaderRecord source, StarHeaderRecord target)
        {
            target.TailStart = source.TailStart; target.TailEnd = source.TailEnd;
            target.ConstellationObjectId = source.ConstellationObjectId; target.GraphType = source.GraphType;
            target.Battle = source.Battle; target.Safety = source.Safety; target.Overloading = source.Overloading;
            target.Owners = source.Owners; target.LastOwners = source.LastOwners;
            target.DominatorSeries = source.DominatorSeries; target.CustomFaction = source.CustomFaction;
            target.SafeRadius = source.SafeRadius; target.DamageRadius = source.DamageRadius;
            target.GraphRadius = source.GraphRadius; target.GraphStar = source.GraphStar;
            target.WarPlayer = source.WarPlayer; target.DayBeforeOccupy = source.DayBeforeOccupy;
            target.DayWithoutPlayer = source.DayWithoutPlayer;
            target.DayWithoutCreateShip = source.DayWithoutCreateShip;
            target.LastDominatorDate = source.LastDominatorDate; target.LastPirateDate = source.LastPirateDate;
            target.LiberationDate = source.LiberationDate; target.DayInvadeInertia = source.DayInvadeInertia;
            target.NoComeKling = source.NoComeKling; target.DominionObjectId = source.DominionObjectId;
            target.MapLabel = source.MapLabel; target.CustomInfoCountOffset = source.CustomInfoCountOffset;
            target.CustomSystemInfos = source.CustomSystemInfos;
        }

        private void ParseStarDropItems(GalaxySummaryData summary)
        {
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            Dictionary<uint, MissileRecord> missilesById = new Dictionary<uint, MissileRecord>();
            foreach (StarHeaderRecord star in GalaxyStars)
            {
                List<MissileRecord> starMissiles;
                int missileOffset = LocateExactMissileList(star, out starMissiles);
                star.MissileListOffset = missileOffset;
                star.MissileCountOffset = missileOffset;
                foreach (MissileRecord missile in starMissiles)
                {
                    MissileRecord previous;
                    if (missilesById.TryGetValue(missile.ObjectId, out previous))
                        throw new SavFormatException("TMissile: object id " + missile.ObjectId +
                            " повторяется в звёздах " + previous.ParentStarId + " и " + missile.ParentStarId + ".");
                    missilesById.Add(missile.ObjectId, missile); GalaxyMissiles.Add(missile);
                }

                List<StarDropItemRecord> selected = null;
                int selectedOffset = -1;
                int matches = 0;
                for (int candidate = star.HeaderEnd; candidate <= missileOffset - 2; candidate++)
                {
                    int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                    if (count < 1 || count > 10000) continue;
                    int cursor = candidate + 2;
                    List<StarDropItemRecord> records = new List<StarDropItemRecord>(count);
                    bool valid = true;
                    for (int index = 0; index < count; index++)
                    {
                        if (cursor > missileOffset - 14) { valid = false; break; }
                        StarDropItemRecord record = new StarDropItemRecord();
                        record.Start = cursor;
                        record.X = BitConverter.ToSingle(MainPayload, cursor);
                        record.Y = BitConverter.ToSingle(MainPayload, cursor + 4);
                        record.ShipObjectId = ReadUInt32(MainPayload, cursor + 8);
                        if (!IsSupportedStarScalar(record.X) || !IsSupportedStarScalar(record.Y) ||
                            record.ShipObjectId > summary.NextObjectId || MainPayload[cursor + 12] > 1)
                        { valid = false; break; }
                        record.InUse = MainPayload[cursor + 12] != 0;
                        record.ItemType = MainPayload[cursor + 13];
                        record.ItemStart = cursor + 14;
                        ItemHeaderRecord item;
                        if (!itemsByStart.TryGetValue(record.ItemStart, out item) ||
                            item.Type != record.ItemType)
                        { valid = false; break; }
                        record.ItemObjectId = item.ObjectId;
                        record.End = SerializedItemEnd(item);
                        if (record.End <= record.ItemStart || record.End > missileOffset)
                        { valid = false; break; }
                        records.Add(record); cursor = record.End;
                    }
                    if (!valid || cursor != missileOffset) continue;
                    selected = records; selectedOffset = candidate; matches++;
                }
                if (matches == 0)
                {
                    int zeroOffset = missileOffset - 2;
                    if (zeroOffset < star.HeaderEnd ||
                        (MainPayload[zeroOffset] | MainPayload[zeroOffset + 1] << 8) != 0)
                        throw new SavFormatException("TStar " + star.ObjectId +
                            ": не найден список выпавших предметов.");
                    selected = new List<StarDropItemRecord>(); selectedOffset = zeroOffset;
                }
                else if (matches != 1)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": найдено несколько списков выпавших предметов: " + matches + ".");
                star.DropItemCountOffset = selectedOffset;
                star.DropItems = selected;
            }
        }

        private void ParseStarSpaceItems(GalaxySummaryData summary)
        {
            Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in GalaxyItems) itemsByStart[item.Start] = item;
            foreach (StarHeaderRecord star in GalaxyStars)
            {
                if (star.DropItemCountOffset < star.HeaderEnd + 2)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": отсутствуют границы списка предметов в космосе.");

                List<ShipItemListEntry> selected = null;
                List<ItemHeaderRecord> selectedNewItems = null;
                int selectedOffset = -1;
                int selectedCount = -1;
                int matches = 0;
                for (int candidate = star.HeaderEnd;
                    candidate <= star.DropItemCountOffset - 3; candidate++)
                {
                    int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                    if (count < 1 || count > 10000) continue;
                    int cursor = candidate + 2;
                    List<ShipItemListEntry> records = new List<ShipItemListEntry>(count);
                    Dictionary<int, ItemHeaderRecord> candidateItems =
                        new Dictionary<int, ItemHeaderRecord>();
                    bool valid = true;
                    for (int index = 0; index < count; index++)
                    {
                        if (cursor >= star.DropItemCountOffset) { valid = false; break; }
                        ShipItemListEntry record = new ShipItemListEntry();
                        record.Start = cursor;
                        record.ItemType = MainPayload[cursor++];
                        string customWeaponName = string.Empty;
                        if (record.ItemType == 68 &&
                            (!TryReadItemString(ref cursor, 512, out customWeaponName) ||
                            cursor >= star.DropItemCountOffset))
                        { valid = false; break; }
                        record.ItemStart = cursor;
                        ItemHeaderRecord item;
                        if (!itemsByStart.TryGetValue(record.ItemStart, out item) &&
                            !candidateItems.TryGetValue(record.ItemStart, out item))
                        {
                            if (!TryReadItemHeader(record.ItemStart, summary.NextObjectId,
                                out item, true) || item.Type != record.ItemType ||
                                item.Type >= 8 &&
                                !TryReadKnownItemDerivedTail(item, item.SharedPrefixEnd))
                            { valid = false; break; }
                            candidateItems.Add(item.Start, item);
                        }
                        if (item.Type != record.ItemType || record.ItemType == 68 &&
                            (item.CustomWeaponDiscriminatorOffset != record.Start ||
                            !string.Equals(item.CustomWeaponName, customWeaponName,
                                StringComparison.Ordinal)))
                        { valid = false; break; }
                        record.ItemObjectId = item.ObjectId;
                        record.End = SerializedItemEnd(item);
                        if (record.End <= record.ItemStart ||
                            record.End > star.DropItemCountOffset)
                        { valid = false; break; }
                        records.Add(record);
                        cursor = record.End;
                    }
                    if (!valid || cursor != star.DropItemCountOffset) continue;
                    // A valid list suffix can itself look like a shorter list when the two
                    // bytes before an inner item happen to equal the remaining item count.
                    // The serialized TStar list begins at the route containing the greatest
                    // number of complete TItem records; equal maxima remain an ambiguity.
                    if (count > selectedCount)
                    {
                        selected = records;
                        selectedNewItems = new List<ItemHeaderRecord>(candidateItems.Values);
                        selectedOffset = candidate;
                        selectedCount = count;
                        matches = 1;
                    }
                    else if (count == selectedCount)
                        matches++;
                }
                if (matches == 0)
                {
                    int zeroOffset = star.DropItemCountOffset - 2;
                    if (zeroOffset < star.HeaderEnd ||
                        (MainPayload[zeroOffset] | MainPayload[zeroOffset + 1] << 8) != 0)
                        throw new SavFormatException("TStar " + star.ObjectId +
                            ": не найден список предметов в космосе.");
                    selected = new List<ShipItemListEntry>();
                    selectedNewItems = new List<ItemHeaderRecord>();
                    selectedOffset = zeroOffset;
                }
                else if (matches != 1)
                    throw new SavFormatException("TStar " + star.ObjectId +
                        ": найдено несколько максимальных списков предметов в космосе: " +
                        matches + ".");
                star.SpaceItemCountOffset = selectedOffset;
                star.SpaceItems = selected;
                foreach (ItemHeaderRecord item in selectedNewItems)
                {
                    itemsByStart.Add(item.Start, item);
                    GalaxyItems.Add(item);
                }
            }
        }

        private void ParseStarSpaceShips()
        {
            Dictionary<int, ShipHeaderRecord> shipsByStart =
                new Dictionary<int, ShipHeaderRecord>();
            foreach (ShipHeaderRecord ship in GalaxyShips) shipsByStart[ship.Start] = ship;
            foreach (StarHeaderRecord star in GalaxyStars)
            {
                List<StarShipRecord> selected = null;
                int selectedOffset = -1, selectedCount = -1, matches = 0;
                HashSet<int> candidates = new HashSet<int>();
                if (star.SpaceShipCountOffset >= star.HeaderEnd)
                    candidates.Add(star.SpaceShipCountOffset);
                foreach (ShipHeaderRecord ship in GalaxyShips)
                    if (ship.Start > star.HeaderEnd && ship.Start < star.SpaceItemCountOffset)
                        candidates.Add(ship.Start - 3);
                if (star.SpaceItemCountOffset >= star.HeaderEnd + 2)
                    candidates.Add(star.SpaceItemCountOffset - 2);
                foreach (int candidate in candidates)
                {
                    List<StarShipRecord> records;
                    if (!TryReadStarSpaceShipList(candidate, star.SpaceItemCountOffset,
                        shipsByStart, out records)) continue;
                    if (records.Count > selectedCount)
                    {
                        selected = records; selectedOffset = candidate;
                        selectedCount = records.Count; matches = 1;
                    }
                    else if (records.Count == selectedCount) matches++;
                }
                if (matches != 1)
                {
                    star.HasExactSpaceShipList = false;
                    star.SpaceShips.Clear();
                    continue;
                }
                star.SpaceShipCountOffset = selectedOffset;
                star.HasExactSpaceShipList = true;
                star.SpaceShips = selected;
            }
        }

        private bool TryReadStarSpaceShipList(int start, int end,
            Dictionary<int, ShipHeaderRecord> shipsByStart,
            out List<StarShipRecord> records)
        {
            records = null;
            if (start < 0 || end < start + 2 || end > MainPayload.Length) return false;
            int count = BitConverter.ToUInt16(MainPayload, start);
            if (count > 10000) return false;
            int cursor = start + 2;
            List<StarShipRecord> parsed = new List<StarShipRecord>(count);
            for (int index = 0; index < count; index++)
            {
                ShipHeaderRecord ship;
                if (!shipsByStart.TryGetValue(cursor + 1, out ship)) return false;
                int knownEnd = SerializedGalaxyShipEnd(ship);
                if (knownEnd <= ship.Start || knownEnd > end) return false;
                int shipEnd = end;
                if (index + 1 < count)
                {
                    int nextStart = int.MaxValue;
                    foreach (int candidateStart in shipsByStart.Keys)
                        if (candidateStart - 1 >= knownEnd && candidateStart - 1 < end &&
                            candidateStart < nextStart) nextStart = candidateStart;
                    if (nextStart == int.MaxValue) return false;
                    shipEnd = nextStart - 1;
                }
                StarShipRecord record = new StarShipRecord();
                record.Start = cursor; record.End = shipEnd; record.ShipStart = ship.Start;
                record.ShipType = MainPayload[cursor]; record.ShipObjectId = ship.ObjectId;
                record.OpaqueTail = knownEnd != shipEnd;
                parsed.Add(record);
                cursor = shipEnd;
            }
            if (cursor != end) return false;
            records = parsed;
            return true;
        }

        private int SerializedGalaxyShipEnd(ShipHeaderRecord ship)
        {
            if (ship == null) return -1;
            if (ship.IsPlayer && AchievementStats != null &&
                AchievementStats.PlayerEnd > ship.Start) return AchievementStats.PlayerEnd;
            switch (ship.Type)
            {
                case 0: return ship.HasSimpleDerivedTail ? ship.SimpleDerivedTailOffset + 7 : -1;
                case 1: return ship.HasRangerTail ? ship.RangerPostQuestOffset + 64 : -1;
                case 2: return ship.HasSimpleDerivedTail ? ship.SimpleDerivedTailOffset + 1 : -1;
                case 3: return ship.HasSimpleDerivedTail ? ship.SimpleDerivedTailOffset + 9 : -1;
                case 4: return ship.HasSimpleDerivedTail ? ship.SimpleDerivedTailOffset + 1 : -1;
                case 5: return ship.HasTranclucatorTail ? ship.TranclucatorPostArtOffset + 10 : -1;
                default: return ship.HasRuinsTail ? ship.RuinsFinalFlagsOffset + 4 : -1;
            }
        }

        private int LocateExactMissileList(StarHeaderRecord star, out List<MissileRecord> selectedRecords)
        {
            selectedRecords = null;
            int selected = -1, matches = 0;
            for (int candidate = star.HeaderEnd; candidate <= star.TailStart - 2; candidate++)
            {
                int count = MainPayload[candidate] | MainPayload[candidate + 1] << 8;
                if (count > 1024) continue;
                int cursor = candidate + 2;
                List<MissileRecord> records = new List<MissileRecord>(count);
                bool valid = true;
                for (int index = 0; index < count; index++)
                {
                    MissileRecord missile;
                    if (!TryReadMissileRecord(ref cursor, star.TailStart, star.ObjectId, out missile))
                    { valid = false; break; }
                    records.Add(missile);
                }
                if (!valid || cursor != star.TailStart) continue;
                selected = candidate; selectedRecords = records; matches++;
            }
            if (matches != 1)
                throw new SavFormatException("TStar " + star.ObjectId +
                    ": ожидался один точный список ракет, найдено " + matches +
                    ", tail=0x" + star.TailStart.ToString("X") + ", bytes=" +
                    BitConverter.ToString(MainPayload, Math.Max(star.HeaderEnd, star.TailStart - 96),
                        Math.Min(96, star.TailStart - Math.Max(star.HeaderEnd, star.TailStart - 96))) + ".");
            return selected;
        }

        private static bool IsSupportedStarCoordinate(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value >= -4096 && value <= 4096 &&
                value == (float)Math.Truncate(value);
        }

        private static bool IsSupportedStarName(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2 || value.Length > 63) return false;
            foreach (char character in value)
                if (!(character == ' ' || character == '-' || character == '\'' || character == '(' || character == ')' ||
                    character == '.' || character >= '0' && character <= '9' || character >= 'A' && character <= 'Z' ||
                    character >= 'a' && character <= 'z' || character >= '\u0400' && character <= '\u052F'))
                    return false;
            return true;
        }

        private static int ReadBoundedCount(byte[] data, ref int offset, string label)
        {
            int value = ReadUInt16(data, ref offset, label);
            if (value > 10000)
                throw new SavFormatException(label + ": превышен доказанный предел 10000.");
            return value;
        }

        private static void SkipFixedList(byte[] data, ref int offset, int itemSize, string label)
        {
            int count = ReadBoundedCount(data, ref offset, label + " count");
            Skip(data, ref offset, checked(count * itemSize), label);
        }

        private static List<uint> ReadUInt32List(byte[] data, ref int offset, string label)
        {
            int count = ReadBoundedCount(data, ref offset, label + " count");
            List<uint> values = new List<uint>(count);
            for (int index = 0; index < count; index++)
                values.Add(ReadUInt32(data, ref offset, label));
            return values;
        }

        private static List<GalaxyMapLine> ReadMapLineList(byte[] data, ref int offset, string label)
        {
            int count = ReadBoundedCount(data, ref offset, label + " count");
            List<GalaxyMapLine> values = new List<GalaxyMapLine>(count);
            for (int index = 0; index < count; index++)
            {
                GalaxyMapLine line = new GalaxyMapLine();
                line.X1 = ReadSingle(data, ref offset, label + " x1");
                line.Y1 = ReadSingle(data, ref offset, label + " y1");
                line.X2 = ReadSingle(data, ref offset, label + " x2");
                line.Y2 = ReadSingle(data, ref offset, label + " y2");
                values.Add(line);
            }
            return values;
        }

        private static void SkipPolygonList(byte[] data, ref int offset, string label)
        {
            int polygonCount = ReadBoundedCount(data, ref offset, label + " count");
            for (int polygon = 0; polygon < polygonCount; polygon++)
            {
                int pointCount = ReadBoundedCount(data, ref offset, label + " point count");
                Skip(data, ref offset, checked(pointCount * 8 + 24), label + " polygon");
            }
        }

        private byte[] Serialize(uint crc, byte[] encrypted)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                for (int index = 0; index < Header.Length; index++)
                    WriteUtf16Z(stream, Header[index]);
                WriteSizedBlock(stream, PreviewBlock);
                WriteSizedBlock(stream, MapBlock);
                WriteUInt32(stream, crc);
                WriteUInt32(stream, EncryptionKey);
                WriteSizedBlock(stream, encrypted);
                stream.Write(FilmBlock, 0, FilmBlock.Length);
                return stream.ToArray();
            }
        }

        private static SavMetadata ReadMetadata(byte[] payload)
        {
            if (payload.Length < MetadataSize)
                throw new SavFormatException("Основной блок короче 32-байтного metadata-prefix.");
            int[] reserved = { 13, 15, 16, 17, 18 };
            foreach (int offset in reserved)
                if (payload[offset] != 0)
                    throw new SavFormatException("Неизвестная разметка metadata-prefix.");
            if (payload[12] > 1 || payload[14] > 1 || payload[19] > 1)
                throw new SavFormatException("Некорректное логическое поле metadata-prefix.");
            SavMetadata value = new SavMetadata();
            value.CurrentForm = ReadInt32(payload, 0);
            value.CameraX = ReadInt32(payload, 4);
            value.CameraY = ReadInt32(payload, 8);
            value.ShowPanel = payload[12] != 0;
            value.ViewFollow = payload[14] != 0;
            value.CalcHeader = payload[19] != 0;
            value.Tips = ReadUInt32(payload, 20);
            value.PlayerMessageCount = ReadUInt32(payload, 28);
            return value;
        }

        private static Bitmap DecodeImage(byte[] block, string label)
        {
            if (block.Length == 0)
                return null;
            byte[] payload = DecompressZl01(block, label);
            if (payload.Length < 12)
                throw new SavFormatException(label + ": отсутствует заголовок изображения.");
            int width = checked((int)ReadUInt32(payload, 0));
            int height = checked((int)ReadUInt32(payload, 4));
            int rowStride = checked((int)ReadUInt32(payload, 8));
            if (width <= 0 || height <= 0 || width > 4096 || height > 4096 || rowStride < width * 3)
                throw new SavFormatException(label + ": некорректный размер изображения.");
            if (12L + (long)rowStride * height != payload.Length)
                throw new SavFormatException(label + ": размер пиксельного блока не совпадает.");
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < height; y++)
            {
                int sourceRow = height - 1 - y;
                int offset = 12 + sourceRow * rowStride;
                for (int x = 0; x < width; x++)
                {
                    // SAV stores pixels as RGB. Delphi's 24-bit TBitmap scanlines
                    // are BGR, which made the original decoder look as if it
                    // copied the bytes unchanged. System.Drawing expects the
                    // logical RGB components here.
                    byte red = payload[offset + x * 3];
                    byte green = payload[offset + x * 3 + 1];
                    byte blue = payload[offset + x * 3 + 2];
                    bitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return bitmap;
        }

        private static byte[] DecompressZl01(byte[] block, string label)
        {
            if (block.Length < 14 || block[0] != (byte)'Z' || block[1] != (byte)'L' || block[2] != (byte)'0' || block[3] != (byte)'1')
                throw new SavFormatException(label + ": отсутствует ZL01.");
            int expected = checked((int)ReadUInt32(block, 4));
            int zlibOffset = 8;
            if ((block[zlibOffset] & 0x0F) != 8 || (((block[zlibOffset] << 8) | block[zlibOffset + 1]) % 31) != 0)
                throw new SavFormatException(label + ": некорректный zlib-заголовок.");
            if ((block[zlibOffset + 1] & 0x20) != 0)
                throw new SavFormatException(label + ": zlib-словарь не поддерживается.");
            int rawLength = block.Length - zlibOffset - 2 - 4;
            byte[] unpacked;
            using (MemoryStream input = new MemoryStream(block, zlibOffset + 2, rawLength, false))
            using (DeflateStream inflater = new DeflateStream(input, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream(expected))
            {
                inflater.CopyTo(output);
                unpacked = output.ToArray();
            }
            if (unpacked.Length != expected)
                throw new SavFormatException(label + ": распакованный размер не совпадает.");
            uint storedAdler = ((uint)block[block.Length - 4] << 24) | ((uint)block[block.Length - 3] << 16) |
                ((uint)block[block.Length - 2] << 8) | block[block.Length - 1];
            if (Adler32(unpacked) != storedAdler)
                throw new SavFormatException(label + ": Adler32 не совпадает.");
            return unpacked;
        }

        private static byte[] CompressZl01(byte[] payload)
        {
            byte[] raw;
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream deflater = new DeflateStream(output, CompressionLevel.Optimal, true))
                    deflater.Write(payload, 0, payload.Length);
                raw = output.ToArray();
            }
            using (MemoryStream result = new MemoryStream())
            {
                result.WriteByte((byte)'Z'); result.WriteByte((byte)'L'); result.WriteByte((byte)'0'); result.WriteByte((byte)'1');
                WriteUInt32(result, (uint)payload.Length);
                result.WriteByte(0x78); result.WriteByte(0xDA);
                result.Write(raw, 0, raw.Length);
                uint adler = Adler32(payload);
                result.WriteByte((byte)(adler >> 24)); result.WriteByte((byte)(adler >> 16));
                result.WriteByte((byte)(adler >> 8)); result.WriteByte((byte)adler);
                return result.ToArray();
            }
        }

        private static byte[] Crypt(byte[] data, uint key)
        {
            long seed = key;
            byte[] output = (byte[])data.Clone();
            for (int index = 0; index < output.Length; index++)
            {
                seed = (seed % 127773L) * 16807L - (seed / 127773L) * 2836L;
                if (seed < 1)
                    seed += 2147483647L;
                output[index] ^= (byte)((seed - 1) & 0xFF);
            }
            return output;
        }

        private static uint Adler32(byte[] data)
        {
            const uint modulus = 65521;
            uint a = 1, b = 0;
            for (int index = 0; index < data.Length; index++)
            {
                a = (a + data[index]) % modulus;
                b = (b + a) % modulus;
            }
            return (b << 16) | a;
        }

        private static uint[] BuildCrcTable()
        {
            uint[] table = new uint[256];
            for (uint index = 0; index < table.Length; index++)
            {
                uint value = index;
                for (int bit = 0; bit < 8; bit++)
                    value = (value & 1) != 0 ? 0xEDB88320U ^ (value >> 1) : value >> 1;
                table[index] = value;
            }
            return table;
        }

        private static uint Crc32(byte[] data)
        {
            uint crc = 0xFFFFFFFFU;
            for (int index = 0; index < data.Length; index++)
                crc = CrcTable[(crc ^ data[index]) & 0xFF] ^ (crc >> 8);
            return crc ^ 0xFFFFFFFFU;
        }

        private static string ReadUtf16Z(byte[] data, ref int offset, string label)
        {
            int start = offset;
            for (int units = 0; units < 65536; units++)
            {
                if (offset + 2 > data.Length)
                    throw new SavFormatException(label + ": строка обрезана.");
                if (data[offset] == 0 && data[offset + 1] == 0)
                {
                    string value = Encoding.Unicode.GetString(data, start, offset - start);
                    offset += 2;
                    return value;
                }
                offset += 2;
            }
            throw new SavFormatException(label + ": слишком длинная строка.");
        }

        private static string ReadOptionalString(byte[] data, ref int offset, string label)
        {
            return ReadBoolean(data, ref offset, label + " flag")
                ? ReadUtf16Z(data, ref offset, label) : string.Empty;
        }

        private static byte[] ReadSizedBlock(byte[] data, ref int offset, string label)
        {
            uint rawSize = ReadUInt32(data, ref offset, label + " size");
            if (rawSize > int.MaxValue)
                throw new SavFormatException(label + ": блок слишком велик.");
            return Take(data, ref offset, (int)rawSize, label);
        }

        private static byte[] Take(byte[] data, ref int offset, int size, string label)
        {
            if (size < 0 || offset < 0 || offset > data.Length - size)
                throw new SavFormatException(label + ": выход за границы файла.");
            byte[] value = new byte[size];
            Buffer.BlockCopy(data, offset, value, 0, size);
            offset += size;
            return value;
        }

        private static void Skip(byte[] data, ref int offset, int size, string label)
        {
            if (size < 0 || offset < 0 || offset > data.Length - size)
                throw new SavFormatException(label + ": выход за границы файла.");
            offset += size;
        }

        private static uint ReadUInt32(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset > data.Length - 4)
                throw new SavFormatException(label + ": отсутствует UInt32.");
            uint value = ReadUInt32(data, offset);
            offset += 4;
            return value;
        }

        private static int ReadUInt16(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset > data.Length - 2)
                throw new SavFormatException(label + ": отсутствует UInt16.");
            int value = data[offset] | data[offset + 1] << 8;
            offset += 2;
            return value;
        }

        private static byte ReadByte(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset >= data.Length)
                throw new SavFormatException(label + ": отсутствует Byte.");
            return data[offset++];
        }

        private static int ReadInt32(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset > data.Length - 4)
                throw new SavFormatException(label + ": отсутствует Int32.");
            int value = ReadInt32(data, offset);
            offset += 4;
            return value;
        }

        private static float ReadSingle(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset > data.Length - 4)
                throw new SavFormatException(label + ": отсутствует Float32.");
            float value = BitConverter.ToSingle(data, offset);
            offset += 4;
            return value;
        }

        private static double ReadDouble(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset > data.Length - 8)
                throw new SavFormatException(label + ": отсутствует Float64.");
            double value = BitConverter.ToDouble(data, offset);
            offset += 8;
            return value;
        }

        private static bool ReadBoolean(byte[] data, ref int offset, string label)
        {
            if (offset < 0 || offset >= data.Length)
                throw new SavFormatException(label + ": отсутствует Boolean.");
            byte value = data[offset++];
            if (value > 1)
                throw new SavFormatException(label + ": ожидался Boolean 0 или 1.");
            return value != 0;
        }

        private static uint ReadUInt32(byte[] data, int offset)
        {
            return (uint)(data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 | data[offset + 3] << 24);
        }

        private static int ReadInt32(byte[] data, int offset)
        {
            return unchecked((int)ReadUInt32(data, offset));
        }

        private static void WriteInt32(byte[] data, int offset, int value)
        {
            WriteUInt32(data, offset, unchecked((uint)value));
        }

        private static void WriteInt32(Stream stream, int value)
        {
            WriteUInt32(stream, unchecked((uint)value));
        }

        private static void WriteUInt16(Stream stream, ushort value)
        {
            stream.WriteByte((byte)value); stream.WriteByte((byte)(value >> 8));
        }

        private static void WriteBoolean(Stream stream, bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        private static void WriteSingle(Stream stream, float value)
        {
            byte[] raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, raw.Length);
        }

        private static void WriteDouble(Stream stream, double value)
        {
            byte[] raw = BitConverter.GetBytes(value);
            stream.Write(raw, 0, raw.Length);
        }

        private static void WriteUInt32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8);
            data[offset + 2] = (byte)(value >> 16); data[offset + 3] = (byte)(value >> 24);
        }

        private static void WriteUInt32(Stream stream, uint value)
        {
            stream.WriteByte((byte)value); stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16)); stream.WriteByte((byte)(value >> 24));
        }

        private static void WriteUtf16Z(Stream stream, string value)
        {
            byte[] raw = Encoding.Unicode.GetBytes(value);
            stream.Write(raw, 0, raw.Length); stream.WriteByte(0); stream.WriteByte(0);
        }

        private static void WriteOptionalString(Stream stream, string value)
        {
            bool present = !string.IsNullOrEmpty(value);
            WriteBoolean(stream, present);
            if (present) WriteUtf16Z(stream, value);
        }

        private static void WriteSizedBlock(Stream stream, byte[] value)
        {
            WriteUInt32(stream, (uint)value.Length);
            stream.Write(value, 0, value.Length);
        }

        private static bool EqualBytes(byte[] left, byte[] right)
        {
            if (object.ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Length != right.Length) return false;
            for (int index = 0; index < left.Length; index++)
                if (left[index] != right[index]) return false;
            return true;
        }
    }
}
