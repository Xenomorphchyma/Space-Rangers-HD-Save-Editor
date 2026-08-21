using System;
using System.Collections.Generic;
using System.IO;
using SpaceRangersHdSaveEditor;

internal static class NativeSavSelfTest
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 2 && args[0] == "--summary")
            {
                SavContainer save = SavContainer.Load(args[1]);
                int planetCount = 0;
                foreach (StarHeaderRecord star in save.GalaxyStars) planetCount += star.PlanetCount;
                int typeZeroItems = 0;
                uint maximumItemId = 0;
                foreach (ItemHeaderRecord item in save.GalaxyItems)
                {
                    if (item.Type == 0) typeZeroItems++;
                    if (item.ObjectId > maximumItemId) maximumItemId = item.ObjectId;
                }
                Console.WriteLine("player_id={0} ships={1} stations={2} items={3} type0={4} max_item_id={5} holes={14} asteroids={15} missiles={16} scripts={6} script_list=0x{7:X} turn_offset=0x{8:X} planets={9} rangers={10} difficulty={11} cheats_test={12} next_object_id={13}",
                    save.GalaxySummary.PlayerObjectId,
                    save.ShipCount,
                    save.StationCount,
                    save.ItemCount,
                    typeZeroItems,
                    maximumItemId,
                    save.ActiveScripts.Count,
                    save.ActiveScriptListOffset,
                    save.GalaxySummary.TurnOffset,
                    planetCount,
                    save.GalaxySummary.RangerCount,
                    save.GalaxySummary.DifficultyPercent,
                    save.GalaxySummary.CheatsTest,
                    save.GalaxySummary.NextObjectId,
                    save.GalaxyHoles.Count,
                    save.GalaxyAsteroids.Count,
                    save.GalaxyMissiles.Count);
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    if (ship.IsPlayer && ship.ObjectId == save.GalaxySummary.PlayerObjectId)
                    {
                        Console.WriteLine("player_start=0x{0:X} fixed_end=0x{1:X} type={2} owner={3} pilot_race={4} money={5} rnd={6} rnd_out={7} day={8} face={9}",
                            ship.Start, ship.FixedPrefixEnd, ship.Type, ship.Owner, ship.PilotRace,
                            ship.Money, ship.Rnd, ship.RndOut, ship.Day, ship.Face);
                        Console.WriteLine("player_normal=0x{0:X} rank={1}/{2} pirate_rank={3}/{4} liberation_planet={5} last_planet={6} kills={7},{8},{9},{10},{11},{12},{13}",
                            ship.NormalShipTailOffset, ship.CoalitionRank, ship.CoalitionRankPoints,
                            ship.PirateRank, ship.PirateRankPoints, ship.LiberationPlanetId,
                            ship.LastPlanetId, ship.KillAllShips, ship.KillPirates, ship.KillDominators,
                            ship.LiberationSystems, ship.KillPacifics, ship.KillWarriors, ship.KillRangers);
                    }
                SortedDictionary<byte, int> shipTypes = new SortedDictionary<byte, int>();
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    shipTypes[ship.Type] = shipTypes.ContainsKey(ship.Type) ? shipTypes[ship.Type] + 1 : 1;
                foreach (KeyValuePair<byte, int> pair in shipTypes)
                    Console.WriteLine("ship_type={0} count={1}", pair.Key, pair.Value);
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    if ((ship.Type == 0 && ship.ObjectId <= 128) || !ship.HasCommonTail)
                        Console.WriteLine("ship_probe id={0} type={1} start=0x{2:X} name={3} common={4} graph={5}",
                            ship.ObjectId, ship.Type, ship.Start, ship.Name, ship.HasCommonTail, ship.GraphName ?? "");
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    if (ship.HasRuinsTail)
                    {
                        Console.WriteLine("ruins_probe id={0} type={1} tail=0x{2:X} items={3} energy={4} star={5} date={6} flags={7}/{8}/{9}/{10} goods1={11}/{12}/{13}",
                            ship.ObjectId, ship.Type, ship.RuinsShopTailOffset, ship.RuinsEquipmentItemCount,
                            ship.RuinsEnergy, ship.RuinsFlyToStarId, ship.RuinsFlyDate,
                            ship.RuinsSponsor, ship.RuinsSpecialShip, ship.RuinsNoLanding,
                            ship.RuinsNoShopUpdate, ship.RuinsShopGoods[0, 0],
                            ship.RuinsShopGoods[0, 1], ship.RuinsShopGoods[0, 2]);
                        break;
                    }
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    if (ship.HasNormalShipTail && (ship.CoalitionRank > 7 || ship.PirateRank > 7 ||
                        ship.LiberationPlanetId > 100000 || ship.LastPlanetId > 100000))
                        Console.WriteLine("normal_outlier id={0} type={1} tail=0x{2:X} rank={3} pirate_rank={4} liberation={5} last={6}",
                            ship.ObjectId, ship.Type, ship.NormalShipTailOffset, ship.CoalitionRank,
                            ship.PirateRank, ship.LiberationPlanetId, ship.LastPlanetId);
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                    if (ship.Type == 1 && !ship.HasRangerTail)
                    {
                        int candidate = ship.NormalShipTailOffset + 60;
                        int length = Math.Min(192, save.MainPayload.Length - candidate);
                        Console.WriteLine("ranger_unparsed id={0} offset=0x{1:X} bytes={2}", ship.ObjectId,
                            candidate, BitConverter.ToString(save.MainPayload, candidate, length));
                        break;
                    }
                if (save.GalaxyPlanets.Count > 0)
                {
                    PlanetHeaderRecord planet = save.GalaxyPlanets[0];
                    Console.WriteLine("planet_start=0x{0:X} id={1} name={2} polar_angle={3:R} polar_radius={4:R} angle={5:R} radius={6} people={7} money={8} economy={9} owner={10} race={11} government={12}",
                        planet.Start, planet.ObjectId, planet.Name, planet.PolarAngle, planet.PolarRadius,
                        planet.Angle, planet.Radius, planet.PeopleCount, planet.Money, planet.Economy,
                        planet.Owner, planet.Race, planet.Government);
                    Console.WriteLine("planet_late_first graph={0} graph_radius={1} ranger={2} transport={3} quest={4} satellites={5} flags={6}",
                        planet.GraphName ?? "", planet.GraphRadius, planet.RangerCount,
                        planet.TransportCount, planet.QuestNumber, planet.SatelliteCount, planet.HasFlags);
                    Console.WriteLine("planet_relations_first count=0x{0:X}:{1} end=0x{2:X} first={3}",
                        planet.RelationCountOffset, planet.RelationCount, planet.RelationEndOffset,
                        planet.RelationToRangers.Length == 0 ? -1 : planet.RelationToRangers[0]);
                    Console.WriteLine("planet_relations_tail_first={0}",
                        BitConverter.ToString(save.MainPayload, planet.RelationEndOffset, 48));
                }
                int latePlanets = 0, flagPlanets = 0;
                foreach (PlanetHeaderRecord planet in save.GalaxyPlanets)
                {
                    if (planet.HasLateFields) latePlanets++;
                    if (planet.HasFlags) flagPlanets++;
                }
                Console.WriteLine("planet_late={0}/{1} flags={2}/{1}", latePlanets,
                    save.GalaxyPlanets.Count, flagPlanets);
                if (save.AchievementStats != null)
                    Console.WriteLine("achievement_stats=0x{0:X} values={1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11} received=0x{12:X}:{13}",
                        save.AchievementStats.Start, save.AchievementStats.AsteroidsDestroyed,
                        save.AchievementStats.FriedShips, save.AchievementStats.DefendedSystem,
                        save.AchievementStats.PirateSystems, save.AchievementStats.ScienceProgress,
                        save.AchievementStats.ProgramsUsed, save.AchievementStats.PiratesFreed,
                        save.AchievementStats.HealthDrained, save.AchievementStats.FuelGottenFromSun,
                        save.AchievementStats.FuelTankLastId, save.AchievementStats.PlanetsVisited,
                        save.AchievementStats.ReceivedListStart, save.AchievementStats.Received.Count);
                int[] interfaceCounts = new int[5];
                foreach (InterfaceOverrideRecord record in save.GalaxySummary.InterfaceOverrides)
                    interfaceCounts[(int)record.Kind]++;
                Console.WriteLine("interface_overrides={0}/{1}/{2}/{3}/{4}", interfaceCounts[0],
                    interfaceCounts[1], interfaceCounts[2], interfaceCounts[3], interfaceCounts[4]);
                Console.WriteLine("stored_items=" + save.StoredItems.Count);
                return 0;
            }
            if (args.Length < 2 || args[0] != "--list")
                throw new ArgumentException("usage: native-selftest --list paths.txt [roundtrip.sav] [patched.sav] | --summary save.sav");
            string[] paths = File.ReadAllLines(args[1]);
            List<SavContainer> loaded = new List<SavContainer>();
            foreach (string raw in paths)
            {
                string path = raw.Trim();
                if (path.Length == 0) continue;
                try { loaded.Add(SavContainer.Load(path)); }
                catch (Exception error) { throw new InvalidOperationException("failed SAV: " + path, error); }
                if (loaded.Count % 16 == 0 || loaded.Count == paths.Length)
                    Console.WriteLine("native SAV load: {0}/{1}", loaded.Count, paths.Length);
            }
            if (loaded.Count == 0)
                throw new InvalidOperationException("no SAV paths supplied");
            VerifyShipOrderRules();
            foreach (SavContainer save in loaded)
                if (save.GalaxyStarCount <= 0 || save.GalaxyConstellationCount <= 0 || save.GalaxyStarsOffset <= save.GalaxyPrefix.End)
                    throw new InvalidOperationException("TGalaxy directory was not parsed: " + save.SourcePath);
            foreach (SavContainer save in loaded)
            {
                Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in save.GalaxyItems) itemsByStart[item.Start] = item;
                Dictionary<int, ShipHeaderRecord> shipsByStart = new Dictionary<int, ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in save.GalaxyShips) shipsByStart[ship.Start] = ship;
                if (save.GalaxyConstellations.Count != save.GalaxyConstellationCount)
                    throw new InvalidOperationException("TConstellation records were not parsed: " + save.SourcePath);
                foreach (ConstellationRecord constellation in save.GalaxyConstellations)
                {
                    if (constellation.ObjectId == 0 || constellation.StarObjectIds.Count == 0)
                        throw new InvalidOperationException("TConstellation references are invalid: " + save.SourcePath);
                    ValidateMapLines(constellation.BoundaryLines, "visible boundary", save.SourcePath);
                    ValidateMapLines(constellation.HiddenBoundaryLines, "hidden boundary", save.SourcePath);
                    ValidateMapLines(constellation.MapLines, "map line", save.SourcePath);
                }
                if (save.GalaxyPlanets.Count == 0 || save.GalaxyShips.Count == 0 || save.GalaxyItems.Count == 0)
                    throw new InvalidOperationException("nested TPlanet/TShip/TItem headers were not parsed: " + save.SourcePath);
                if (save.GalaxySummary.RangerObjectIds == null ||
                    save.GalaxySummary.RangerObjectIds.Length != save.GalaxySummary.RangerCount)
                    throw new InvalidOperationException("TGalaxy.Rangers reference order was not parsed: " +
                        save.SourcePath);
                foreach (uint rangerId in save.GalaxySummary.RangerObjectIds)
                    if (rangerId == 0 || rangerId >= save.GalaxySummary.NextObjectId)
                        throw new InvalidOperationException("TGalaxy.Rangers reference is outside the SAV id range: " +
                            save.SourcePath + " / " + rangerId);
                foreach (PlanetHeaderRecord planet in save.GalaxyPlanets)
                {
                    if (planet.RelationToRangers == null ||
                        planet.RelationCount != planet.RelationToRangers.Length ||
                        planet.RelationCountOffset + 2 != planet.FixedPrefixEnd ||
                        planet.RelationEndOffset != planet.FixedPrefixEnd + planet.RelationCount ||
                        planet.RelationEndOffset > planet.End)
                        throw new InvalidOperationException(
                            "TPlanet RelationToRangers list was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    if (planet.EquipmentShopItems == null ||
                        planet.EquipmentShopCount != planet.EquipmentShopItems.Count ||
                        planet.EquipmentShopCountOffset != planet.RelationEndOffset ||
                        planet.EquipmentShopEndOffset != (planet.HasWarriorList ?
                            planet.WarriorCountOffset : planet.LateFieldsOffset - 10))
                        throw new InvalidOperationException(
                            "TPlanet EquipmentShop list was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    int expectedShopStart = planet.EquipmentShopCountOffset + 2;
                    foreach (ShipItemListEntry shopItem in planet.EquipmentShopItems)
                    {
                        ItemHeaderRecord nested;
                        if (shopItem.Start != expectedShopStart ||
                            shopItem.ItemStart <= shopItem.Start || shopItem.End <= shopItem.ItemStart ||
                            shopItem.End > planet.EquipmentShopEndOffset ||
                            !itemsByStart.TryGetValue(shopItem.ItemStart, out nested) ||
                            nested.Type != shopItem.ItemType || nested.ObjectId != shopItem.ItemObjectId)
                            throw new InvalidOperationException(
                                "TPlanet EquipmentShop item boundary was not parsed: " +
                                save.SourcePath + " / " + planet.ObjectId + " / " +
                                shopItem.ItemObjectId);
                        expectedShopStart = shopItem.End;
                    }
                    if (expectedShopStart != planet.EquipmentShopEndOffset)
                        throw new InvalidOperationException(
                            "TPlanet EquipmentShop did not reach its exact boundary: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    if (planet.Warriors == null || planet.WarriorEndOffset != planet.LateFieldsOffset - 10 ||
                        (planet.HasWarriorList && (planet.WarriorCountOffset != planet.EquipmentShopEndOffset ||
                            planet.WarriorCount != planet.Warriors.Count)) ||
                        (!planet.HasWarriorList && (planet.WarriorCountOffset != -1 ||
                            planet.WarriorCount != 0 || planet.Warriors.Count != 0)))
                        throw new InvalidOperationException("TPlanet Warriors list was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    int expectedWarriorStart = planet.HasWarriorList ?
                        planet.WarriorCountOffset + 2 : planet.WarriorEndOffset;
                    foreach (PlanetWarriorRecord warrior in planet.Warriors)
                    {
                        ShipHeaderRecord nestedShip;
                        if (warrior.Start != expectedWarriorStart || warrior.ShipStart != warrior.Start + 1 ||
                            warrior.End <= warrior.ShipStart || warrior.End > planet.WarriorEndOffset ||
                            !shipsByStart.TryGetValue(warrior.ShipStart, out nestedShip) ||
                            nestedShip.Type != warrior.ShipType || nestedShip.ObjectId != warrior.ShipObjectId)
                            throw new InvalidOperationException("TPlanet Warrior boundary was not parsed: " +
                                save.SourcePath + " / " + planet.ObjectId + " / " + warrior.ShipObjectId);
                        expectedWarriorStart = warrior.End;
                    }
                    if (expectedWarriorStart != planet.WarriorEndOffset)
                        throw new InvalidOperationException("TPlanet Warriors did not reach exact boundary: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    if (!planet.HasLateFields || !planet.HasFlags)
                        throw new InvalidOperationException("complete TPlanet scalar stream was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    if (planet.Satellites == null || planet.SatelliteCount != planet.Satellites.Count ||
                        planet.SatelliteEndOffset < planet.SatelliteCountOffset + 2 ||
                        planet.SatelliteEndOffset > planet.FlagsOffset - 2)
                        throw new InvalidOperationException("TPlanet TSputnik list was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    int expectedSputnikStart = planet.SatelliteCountOffset + 2;
                    foreach (PlanetSputnikRecord satellite in planet.Satellites)
                    {
                        if (satellite.Start != expectedSputnikStart || satellite.End <= satellite.Start ||
                            satellite.End > planet.SatelliteEndOffset || satellite.OpaqueData == null ||
                            string.IsNullOrEmpty(satellite.GraphName) || float.IsNaN(satellite.AngleCurrent) ||
                            float.IsInfinity(satellite.AngleCurrent))
                            throw new InvalidOperationException("TSputnik boundary/value was not parsed: " +
                                save.SourcePath + " / " + planet.ObjectId + " / " + satellite.ObjectId);
                        expectedSputnikStart = satellite.End;
                    }
                    if (expectedSputnikStart != planet.SatelliteEndOffset)
                        throw new InvalidOperationException("TSputnik list did not reach its exact boundary: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    if (planet.GoneItems == null || planet.GoneItemCount != planet.GoneItems.Count ||
                        planet.GoneItemCountOffset != planet.SatelliteEndOffset ||
                        planet.GoneItemEndOffset != planet.FlagsOffset)
                        throw new InvalidOperationException("TPlanet GoneItems list was not parsed: " +
                            save.SourcePath + " / " + planet.ObjectId);
                    int expectedGoneItemStart = planet.GoneItemCountOffset + 2;
                    foreach (PlanetGoneItemRecord goneItem in planet.GoneItems)
                    {
                        ItemHeaderRecord nested;
                        if (goneItem.Start != expectedGoneItemStart || goneItem.End <= goneItem.ItemStart ||
                            goneItem.End > planet.GoneItemEndOffset ||
                            goneItem.FactoryDiscriminatorOffset != goneItem.Start + 8 ||
                            goneItem.ItemStart <= goneItem.FactoryDiscriminatorOffset ||
                            !itemsByStart.TryGetValue(goneItem.ItemStart, out nested) ||
                            nested.Type != goneItem.ItemType || nested.ObjectId != goneItem.ItemObjectId)
                            throw new InvalidOperationException("TPlanet GoneItem boundary/value was not parsed: " +
                                save.SourcePath + " / " + planet.ObjectId + " / " + goneItem.ItemObjectId);
                        expectedGoneItemStart = goneItem.End;
                    }
                    if (expectedGoneItemStart != planet.GoneItemEndOffset)
                        throw new InvalidOperationException("TPlanet GoneItems did not reach its exact boundary: " +
                            save.SourcePath + " / " + planet.ObjectId);
                }
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                {
                    if (!ship.HasCommonTail)
                        throw new InvalidOperationException("complete TShip common scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (!ship.HasPreCommonCollections || ship.PreCommonTailEnd != ship.CommonTailOffset)
                        throw new InvalidOperationException("TShip nested pre-common collections were not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (ship.TakeItemReferenceIds == null || ship.RecentlyDroppedItemIds == null ||
                        ship.RecentlyDroppedItemCountOffset != ship.TakeItemReferenceCountOffset + 2 +
                            ship.TakeItemReferenceIds.Count * 4 ||
                        ship.PreCommonTailEnd < ship.RecentlyDroppedItemCountOffset + 2 +
                            ship.RecentlyDroppedItemIds.Count * 4 + 12)
                        throw new InvalidOperationException("TShip TakeItems/RecentlyDroppedItems boundaries were not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (ship.Illnesses == null || ship.Illnesses.Count != 25)
                        throw new InvalidOperationException("TShip illness/stimulator records were not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (ship.RelationToRangers == null ||
                        ship.RelationCount != ship.RelationToRangers.Length ||
                        ship.RelationCountOffset != ship.GraphNameEnd + 6 ||
                        ship.RelationEndOffset != ship.RelationCountOffset + 2 + ship.RelationCount ||
                        ship.RelationEndOffset != ship.RewardListOffset)
                        throw new InvalidOperationException("TShip RelationToRangers boundary was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (ship.Rewards == null || ship.Rewards.Count > byte.MaxValue ||
                        ship.RewardListOffset <= ship.GraphNameEnd ||
                        ship.RewardListEndOffset != ship.CommonScalarOffset ||
                        ship.RewardListEndOffset - ship.RewardListOffset != ship.Rewards.Count + 1 ||
                        save.MainPayload[ship.RewardListOffset] != ship.Rewards.Count)
                        throw new InvalidOperationException("TShip reward-list boundary was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    for (int illnessIndex = 0; illnessIndex < ship.Illnesses.Count; illnessIndex++)
                    {
                        ShipIllnessRecord illness = ship.Illnesses[illnessIndex];
                        bool expectedStimulator = illnessIndex == 24;
                        int expectedIndex = expectedStimulator ? 1 : illnessIndex + 1;
                        int expectedStart = expectedStimulator
                            ? ship.CommonScalarOffset + 439
                            : ship.CommonScalarOffset + 25 + illnessIndex * 16;
                        if (illness == null || illness.Stimulator != expectedStimulator ||
                            illness.Index != expectedIndex || illness.Start != expectedStart ||
                            float.IsNaN(illness.Infection) || float.IsInfinity(illness.Infection))
                            throw new InvalidOperationException("TShip illness/stimulator boundary is invalid: " +
                                save.SourcePath + " / " + ship.ObjectId + " / record " + illnessIndex);
                    }
                    if (ship.Type >= 1 && ship.Type <= 4 && !ship.HasNormalShipTail)
                        throw new InvalidOperationException("TNormalShip derived scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if ((ship.Type == 0 || ship.Type == 2 || ship.Type == 3 || ship.Type == 4) &&
                        !ship.HasSimpleDerivedTail)
                        throw new InvalidOperationException("simple derived TShip scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                    if (ship.Type == 1 && !ship.HasRangerTail)
                        throw new InvalidOperationException("TRanger scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.HasRangerTail)
                    {
                        if (ship.RangerQuests == null ||
                            ship.RangerQuestCount != ship.RangerQuests.Count)
                            throw new InvalidOperationException("TRanger quest list was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                        foreach (RangerQuestRecord quest in ship.RangerQuests)
                            if (quest.Start < ship.RangerTailOffset + 10 || quest.End <= quest.Start ||
                                quest.End > ship.RangerPostQuestOffset)
                                throw new InvalidOperationException("TQuest record boundary was not parsed: " +
                                    save.SourcePath + " / " + ship.ObjectId);
                    }
                    if (ship.HasRuinsTail)
                    {
                        if (ship.RuinsEquipmentItems == null ||
                            ship.RuinsEquipmentItemCount != ship.RuinsEquipmentItems.Count ||
                            ship.RuinsEquipmentCountOffset != ship.CommonTailEnd ||
                            ship.RuinsEquipmentEndOffset != ship.RuinsShopTailOffset ||
                            ship.RuinsSaleSatellite == null ||
                            ship.RuinsSaleSatellite.Start != ship.RuinsShopTailOffset + 140 ||
                            ship.RuinsSaleSatellite.ItemStart != ship.RuinsSaleSatellite.Start ||
                            ship.RuinsSaleSatellite.End != ship.RuinsFinalFlagsOffset ||
                            ship.RuinsSaleSatellite.ItemType != 73)
                            throw new InvalidOperationException("TRuins equipment/sale-satellite boundaries were not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                        int expectedRuinsItemStart = ship.RuinsEquipmentCountOffset + 2;
                        foreach (ShipItemListEntry equipment in ship.RuinsEquipmentItems)
                        {
                            ItemHeaderRecord nested;
                            if (equipment.Start != expectedRuinsItemStart || equipment.End <= equipment.ItemStart ||
                                equipment.End > ship.RuinsEquipmentEndOffset ||
                                !itemsByStart.TryGetValue(equipment.ItemStart, out nested) ||
                                nested.Type != equipment.ItemType || nested.ObjectId != equipment.ItemObjectId)
                                throw new InvalidOperationException("TRuins equipment boundary was not parsed: " +
                                    save.SourcePath + " / " + ship.ObjectId + " / " + equipment.ItemObjectId);
                            expectedRuinsItemStart = equipment.End;
                        }
                        ItemHeaderRecord saleSatellite;
                        if (expectedRuinsItemStart != ship.RuinsEquipmentEndOffset ||
                            !itemsByStart.TryGetValue(ship.RuinsSaleSatellite.ItemStart, out saleSatellite) ||
                            saleSatellite.Type != 73 || saleSatellite.ObjectId != ship.RuinsSaleSatellite.ItemObjectId)
                            throw new InvalidOperationException("TRuins nested lists did not reach exact boundaries: " +
                                save.SourcePath + " / " + ship.ObjectId);
                    }
                    if (ship.IsPlayer && !ship.HasPlayerPrefix)
                        throw new InvalidOperationException("TPlayer fixed prefix was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && !ship.HasPlayerFinancialTail)
                        throw new InvalidOperationException("TPlayer financial/statistical tail was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (!ship.HasPlayerStorageItems ||
                        ship.PlayerStorageItems == null ||
                        ship.PlayerObjectStateCount != ship.PlayerStorageItems.Count ||
                        ship.PlayerStorageItemCountOffset != ship.PlayerPrefixOffset + 46 ||
                        ship.PlayerStorageItemsEndOffset != ship.PlayerFinancialOffset))
                        throw new InvalidOperationException("TPlayer StorageItems stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    int expectedStorageStart = ship.PlayerStorageItemCountOffset + 4;
                    foreach (PlayerStorageItemRecord storageItem in ship.PlayerStorageItems)
                    {
                        ItemHeaderRecord nestedStorageItem;
                        if (storageItem.Start != expectedStorageStart ||
                            storageItem.ItemStart <= storageItem.Start + 9 ||
                            storageItem.End <= storageItem.ItemStart ||
                            storageItem.End > ship.PlayerStorageItemsEndOffset ||
                            !itemsByStart.TryGetValue(storageItem.ItemStart, out nestedStorageItem) ||
                            nestedStorageItem.Type != storageItem.ItemType ||
                            nestedStorageItem.ObjectId != storageItem.ItemObjectId)
                            throw new InvalidOperationException("TPlayer StorageItem boundary was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                        expectedStorageStart = storageItem.End;
                    }
                    if (ship.IsPlayer && expectedStorageStart != ship.PlayerStorageItemsEndOffset)
                        throw new InvalidOperationException(
                            "TPlayer StorageItems list did not reach its boundary: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (ship.PlayerInfectionPlaces == null ||
                        ship.PlayerInfectionPlaces.Length != 24 ||
                        ship.PlayerInfectionPlacesOffset != ship.PlayerFinancialOffset + 96 ||
                        ship.PlayerInfectionPlacesEndOffset != ship.PlayerProgramsOffset - 1))
                        throw new InvalidOperationException("TPlayer infection-place strings were not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (ship.PlayerEquipmentSetCount != 10 ||
                        ship.PlayerEquipmentSetItems == null ||
                        ship.PlayerEquipmentSetItems.GetLength(0) != 10 ||
                        ship.PlayerEquipmentSetItems.GetLength(1) != 12 ||
                        ship.PlayerArtefactSetItems == null ||
                        ship.PlayerArtefactSetItems.GetLength(0) != 10 ||
                        ship.PlayerArtefactSetItems.GetLength(1) != 32 ||
                        ship.PlayerEquipmentSetsOffset != ship.PlayerLateStatsOffset + 24 ||
                        ship.PlayerEquipmentSetsEndOffset != ship.PlayerEquipmentSetsOffset + 1801))
                        throw new InvalidOperationException("TPlayer equipment-set stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (!ship.HasPlayerBridge || ship.PlayerBridgeRuins == null ||
                        !ship.PlayerBridgeRuins.HasRuinsTail ||
                        ship.PlayerBridgeRuins.Start != ship.PlayerExperienceOffset + 17 ||
                        ship.PlayerBridgeRuinsEndOffset != ship.PlayerBridgeRuins.RuinsFinalFlagsOffset + 4 ||
                        ship.PlayerBridgeBackgroundEndOffset != save.AchievementStats.ReceivedListStart ||
                        ship.PlayerBridgeBackgroundOffset < ship.PlayerBridgeRuinsEndOffset ||
                        (ship.PlayerCaptainOnBridge == 0 &&
                            (ship.PlayerBridgeReferenceOffset != -1 ||
                             ship.PlayerBridgeCurrentShipId != 0 || ship.PlayerBridgeCurrentPlanetId != 0 ||
                             ship.PlayerBridgeBackgroundOffset != ship.PlayerBridgeRuinsEndOffset)) ||
                        (ship.PlayerCaptainOnBridge != 0 &&
                            (ship.PlayerBridgeReferenceOffset != ship.PlayerBridgeRuinsEndOffset ||
                             ship.PlayerBridgeBackgroundOffset != ship.PlayerBridgeReferenceOffset + 8))))
                        throw new InvalidOperationException("TPlayer Bridge/TRuins stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (ship.PlayerSatelliteItems == null ||
                        ship.PlayerSatelliteCount != ship.PlayerSatelliteItems.Count ||
                        ship.PlayerSatelliteEndOffset < ship.PlayerSatelliteListOffset + 4))
                        throw new InvalidOperationException("TPlayer satellite stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    int expectedSatelliteStart = ship.PlayerSatelliteListOffset + 4;
                    foreach (ShipItemListEntry satellite in ship.PlayerSatelliteItems)
                    {
                        if (satellite.Start != expectedSatelliteStart ||
                            satellite.ItemStart != satellite.Start || satellite.ItemType != 73 ||
                            satellite.End <= satellite.Start || satellite.End > ship.PlayerSatelliteEndOffset)
                            throw new InvalidOperationException("TPlayer TSatellite boundary was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                        expectedSatelliteStart = satellite.End;
                    }
                    if (ship.IsPlayer && expectedSatelliteStart != ship.PlayerSatelliteEndOffset)
                        throw new InvalidOperationException("TPlayer TSatellite list did not reach its boundary: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (!ship.HasPlayerRobotMaps || ship.PlayerRobotMaps == null ||
                        ship.PlayerRobotMapCount != ship.PlayerRobotMaps.Count ||
                        ship.PlayerRobotMapListOffset < ship.PlayerFinancialOffset ||
                        ship.PlayerRobotMapEndOffset != ship.PlayerLateStatsOffset ||
                        ship.PlayerRobotMapEndOffset != ship.PlayerRobotMapListOffset + 4 +
                            ship.PlayerRobotMaps.Count * 40))
                        throw new InvalidOperationException("TPlayer robot-map stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    int expectedRobotMapStart = ship.PlayerRobotMapListOffset + 4;
                    foreach (PlayerRobotMapRecord robotMap in ship.PlayerRobotMaps)
                    {
                        if (robotMap.Start != expectedRobotMapStart ||
                            robotMap.End != robotMap.Start + 40 ||
                            robotMap.End > ship.PlayerRobotMapEndOffset)
                            throw new InvalidOperationException("TRobotMapStat boundary was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                        expectedRobotMapStart = robotMap.End;
                    }
                    if (ship.IsPlayer && (!ship.HasPlayerJournal || ship.PlayerJournalRecords == null ||
                        ship.PlayerJournalListOffset < ship.PlayerLateStatsOffset ||
                        ship.PlayerJournalEndOffset < ship.PlayerJournalListOffset + 4))
                        throw new InvalidOperationException("TPlayer journal stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    foreach (PlayerJournalRecord journal in ship.PlayerJournalRecords)
                        if (journal.Start < ship.PlayerJournalListOffset + 4 ||
                            journal.End <= journal.Start || journal.End > ship.PlayerJournalEndOffset)
                            throw new InvalidOperationException("TJournalRecord boundary was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                    if (ship.IsPlayer && (!ship.HasPlayerNews || ship.PlayerNewsRecords == null ||
                        ship.PlayerNewsListOffset != ship.PlayerJournalEndOffset ||
                        ship.PlayerNewsEndOffset < ship.PlayerNewsListOffset + 2 ||
                        ship.PlayerNewsEndOffset != ship.PlayerPreAchievementFlagsOffset))
                        throw new InvalidOperationException("TPlayer news stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    foreach (GalaxyNewsRecord news in ship.PlayerNewsRecords)
                        if (news.Start < ship.PlayerNewsListOffset + 2 || news.End <= news.Start ||
                            news.End > ship.PlayerNewsEndOffset || news.Text == null ||
                            news.Text.Length > 32768)
                            throw new InvalidOperationException("TPlayer news boundary was not parsed: " +
                                save.SourcePath + " / " + ship.ObjectId);
                    if (ship.Type == 5 && !ship.HasTranclucatorTail)
                        throw new InvalidOperationException("TTranclucator scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId);
                    if (ship.Type >= 6 && ship.Type <= 13 && !ship.HasRuinsTail)
                        throw new InvalidOperationException("TRuins scalar stream was not parsed: " +
                            save.SourcePath + " / " + ship.ObjectId + " / type " + ship.Type);
                }
                foreach (ItemHeaderRecord item in save.GalaxyItems)
                    if (item.Type < 8 && !item.HasGoodsTail)
                        throw new InvalidOperationException("TGoodsItem tail was not parsed: " +
                            save.SourcePath + " / " + item.ObjectId + " / type " + item.Type);
                if (save.ActiveScriptListOffset < 0)
                    throw new InvalidOperationException("TScript active-list boundary was not parsed: " + save.SourcePath);
                if (save.GalaxySummary.ActiveScriptListOffset != save.ActiveScriptListOffset ||
                    save.GalaxySummary.ActiveScripts.Count != save.ActiveScripts.Count)
                    throw new InvalidOperationException("TScript editable model was not attached to TGalaxy: " +
                        save.SourcePath);
                foreach (ScriptRecord script in save.ActiveScripts)
                    if (script.Start < save.ActiveScriptListOffset + 2 || script.End <= script.Start ||
                        script.OldEthers == null)
                        throw new InvalidOperationException("TScript record was not fully parsed: " + save.SourcePath);
                if (save.CustomWeaponInfos.Count != save.GalaxyPrefix.CustomModWeaponCount)
                    throw new InvalidOperationException("TCustomWeaponInfo directory was not parsed: " + save.SourcePath);
                HashSet<int> uniqueItemStarts = new HashSet<int>();
                HashSet<string> customWeaponNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (CustomWeaponInfoRecord weapon in save.CustomWeaponInfos)
                    customWeaponNames.Add(weapon.SystemName);
                foreach (ItemHeaderRecord item in save.GalaxyItems)
                {
                    if (!uniqueItemStarts.Add(item.Start))
                        throw new InvalidOperationException("duplicate TItem model record: " +
                            save.SourcePath + " / " + item.Type + ":" + item.ObjectId);
                    if (item.DerivedFields != null)
                    {
                        HashSet<string> derivedNames = new HashSet<string>(StringComparer.Ordinal);
                        foreach (ItemDerivedField field in item.DerivedFields)
                            if (!derivedNames.Add(field.ControlName))
                                throw new InvalidOperationException(
                                    "duplicate TItem derived field: " + save.SourcePath + " / " +
                                    item.Type + ":" + item.ObjectId + " / " + field.ControlName);
                    }
                    if (item.Type == 68)
                    {
                        if (!customWeaponNames.Contains(item.CustomWeaponName))
                            throw new InvalidOperationException("TCustomWeapon item has no descriptor: " +
                                save.SourcePath + " / " + item.CustomWeaponName);
                        int ownerCount = CountItemOwners(save, item.Start);
                        if (ownerCount < 1)
                            throw new InvalidOperationException("TCustomWeapon item owner count is " +
                                ownerCount + ": " + save.SourcePath + " / " + item.CustomWeaponName +
                                " / " + item.ObjectId);
                    }
                }
                foreach (StarHeaderRecord star in save.GalaxyStars)
                {
                    if (star.HasExactSpaceShipList)
                    {
                        int shipCursor = star.SpaceShipCountOffset + 2;
                        if (star.SpaceShipCountOffset < star.HeaderEnd ||
                            ReadUInt16(save.MainPayload, star.SpaceShipCountOffset) != star.SpaceShips.Count)
                            throw new InvalidOperationException("TStar.Ships count was not parsed: " +
                                save.SourcePath + " / " + star.ObjectId);
                        foreach (StarShipRecord record in star.SpaceShips)
                        {
                            if (record.Start != shipCursor || record.ShipStart != record.Start + 1 ||
                                record.End <= record.ShipStart)
                                throw new InvalidOperationException("TStar.Ships boundary was not parsed: " +
                                    save.SourcePath + " / " + star.ObjectId);
                            shipCursor = record.End;
                        }
                        if (shipCursor != star.SpaceItemCountOffset)
                            throw new InvalidOperationException("TStar.Ships end was not parsed: " +
                                save.SourcePath + " / " + star.ObjectId);
                    }
                    else if (star.SpaceShips.Count != 0)
                        throw new InvalidOperationException("ambiguous TStar.Ships list leaked records: " +
                            save.SourcePath + " / " + star.ObjectId);
                    int cursor = star.SpaceItemCountOffset + 2;
                    if (star.SpaceItemCountOffset < star.HeaderEnd ||
                        ReadUInt16(save.MainPayload, star.SpaceItemCountOffset) != star.SpaceItems.Count)
                        throw new InvalidOperationException("TStar.ItemsInSpace count was not parsed: " +
                            save.SourcePath + " / " + star.ObjectId);
                    foreach (ShipItemListEntry record in star.SpaceItems)
                    {
                        if (record.Start != cursor || record.End <= record.ItemStart)
                            throw new InvalidOperationException("TStar.ItemsInSpace boundary was not parsed: " +
                                save.SourcePath + " / " + star.ObjectId);
                        cursor = record.End;
                    }
                    if (cursor != star.DropItemCountOffset)
                        throw new InvalidOperationException("TStar.ItemsInSpace end was not parsed: " +
                            save.SourcePath + " / " + star.ObjectId);
                }
                if (save.GalaxySummary.ScriptShopSlotCountOffset < 0 ||
                    ReadUInt16(save.MainPayload, save.GalaxySummary.ScriptShopSlotCountOffset) !=
                        save.GalaxySummary.ScriptShopSlots.Count)
                    throw new InvalidOperationException("TScript shop-slot count was not parsed: " +
                        save.SourcePath);
                int shopCursor = save.GalaxySummary.ScriptShopSlotCountOffset + 2;
                foreach (ScriptShopSlotRecord slot in save.GalaxySummary.ScriptShopSlots)
                {
                    if (slot.Start != shopCursor || slot.End <= slot.Start)
                        throw new InvalidOperationException("TScript shop-slot boundary was not parsed: " +
                            save.SourcePath);
                    shopCursor = slot.End;
                }
                if (shopCursor != save.GalaxySummary.ScriptShopSlotListEndOffset)
                    throw new InvalidOperationException("TScript shop-slot end was not parsed: " +
                        save.SourcePath);
                foreach (InterfaceOverrideRecord record in save.GalaxySummary.InterfaceOverrides)
                    if (record.Start < save.GalaxyStarsOffset || record.End <= record.Start ||
                        (int)record.Kind < 0 || (int)record.Kind > 4)
                        throw new InvalidOperationException("interface override record was not parsed: " + save.SourcePath);
                foreach (StoredItemRecord record in save.StoredItems)
                    if (record.Start < save.GalaxyStarsOffset || record.End <= record.Start ||
                        record.ItemTypeOffset <= record.Start || record.ItemStart <= record.ItemTypeOffset ||
                        string.IsNullOrEmpty(record.ScriptTag))
                        throw new InvalidOperationException("TStoredItem record was not parsed: " + save.SourcePath);
                if (save.HasExactStoredItemList)
                {
                    if (save.StoredItemCountOffset < save.GalaxyStarsOffset ||
                        save.StoredItemListEndOffset < save.StoredItemCountOffset + 2 ||
                        (save.MainPayload[save.StoredItemCountOffset] |
                            save.MainPayload[save.StoredItemCountOffset + 1] << 8) != save.StoredItems.Count ||
                        (save.StoredItems.Count == 0 ?
                            save.StoredItemListEndOffset != save.StoredItemCountOffset + 2 :
                            save.StoredItems[save.StoredItems.Count - 1].End != save.StoredItemListEndOffset))
                        throw new InvalidOperationException("exact TStoredItem list boundary/count failed: " +
                            save.SourcePath);
                }
                if (save.GalaxySummary.DifficultyLevels == null ||
                    save.GalaxySummary.DifficultyLevels.Length != 8 ||
                    save.GalaxySummary.CustomRuleLevels == null ||
                    save.GalaxySummary.CustomRuleLevels.Length != 19 ||
                    save.GalaxySummary.CustomRuleFlags == null ||
                    save.GalaxySummary.CustomRuleFlags.Length != 15 ||
                    save.GalaxySummary.CustomRuleLateFlags == null ||
                    save.GalaxySummary.CustomRuleLateFlags.Length != 8 ||
                    save.GalaxySummary.HullGrowth > 2)
                    throw new InvalidOperationException("TGALAXYFORM scalar arrays were not parsed: " +
                        save.SourcePath);
                if (save.GalaxySummary.CompleteQuests == null ||
                    save.GalaxySummary.CompleteQuests.Count != save.GalaxySummary.CompleteQuestCount)
                    throw new InvalidOperationException("TCompleteQuest list was not parsed: " + save.SourcePath);
                foreach (CompleteQuestRecord record in save.GalaxySummary.CompleteQuests)
                    if (record.Start < save.GalaxyStarsOffset || record.End <= record.Start)
                        throw new InvalidOperationException("TCompleteQuest record boundary was not parsed: " +
                            save.SourcePath);
                if (save.GalaxySummary.GalaxyNews == null ||
                    save.GalaxySummary.GalaxyNews.Count != save.GalaxySummary.GalaxyNewsCount)
                    throw new InvalidOperationException("TGalaxyNews list was not parsed: " + save.SourcePath);
                foreach (GalaxyNewsRecord record in save.GalaxySummary.GalaxyNews)
                    if (record.Start < save.GalaxyStarsOffset || record.End <= record.Start)
                        throw new InvalidOperationException("TGalaxyNews record boundary was not parsed: " +
                            save.SourcePath);
                int expectedKellerOffset = save.GalaxySummary.RangerReferenceListOffset + 2 +
                    save.GalaxySummary.RangerCount * 4 + (save.Version > 132 ? 20 : 0);
                if (save.GalaxySummary.KellerAttackOffset != expectedKellerOffset)
                    throw new InvalidOperationException("TGALAXYFORM Keller-attack block was not parsed: " +
                        save.SourcePath);
                if (save.GalaxySummary.WarOperationListOffset < save.ActiveScriptListOffset + 2 ||
                    save.GalaxySummary.WarOperationListOffset > save.GalaxySummary.TurnOffset - 11)
                    throw new InvalidOperationException("TWarOperation list boundary was not parsed: " +
                        save.SourcePath);
                foreach (WarOperationRecord operation in save.GalaxySummary.WarOperations)
                {
                    if (operation.Start < save.GalaxySummary.WarOperationListOffset ||
                        operation.End <= operation.Start || operation.LegacyZero != 0)
                        throw new InvalidOperationException("TWarOperation record was not parsed: " + save.SourcePath);
                    foreach (WarOperationOrderRecord order in operation.Orders)
                        if (order.Type > 7 || float.IsNaN(order.DestinationX) || float.IsInfinity(order.DestinationX) ||
                            float.IsNaN(order.DestinationY) || float.IsInfinity(order.DestinationY))
                            throw new InvalidOperationException("TWarOperation order was not parsed: " + save.SourcePath);
                }
                if (save.GalaxySummary.GateListOffset < save.GalaxyStarsOffset ||
                    save.GalaxySummary.GateListOffset > save.GalaxySummary.PlanetReferenceListOffset - 2)
                    throw new InvalidOperationException("TGate list boundary was not parsed: " + save.SourcePath);
                foreach (GateRecord gate in save.GalaxySummary.Gates)
                    if (gate.Start < save.GalaxySummary.GateListOffset || gate.End <= gate.Start ||
                        float.IsNaN(gate.X) || float.IsInfinity(gate.X) ||
                        float.IsNaN(gate.Y) || float.IsInfinity(gate.Y))
                        throw new InvalidOperationException("TGate record was not parsed: " + save.SourcePath);
                if (save.AchievementStats == null || save.AchievementStats.End - save.AchievementStats.Start != 41)
                    throw new InvalidOperationException("TAchievementStats was not parsed: " + save.SourcePath);
            }

            if (args.Length >= 4)
            {
                SavContainer source = loaded[0];
                foreach (SavContainer candidate in loaded)
                    if (SupportsWriterCoverage(candidate))
                    { source = candidate; break; }
                if (!SupportsWriterCoverage(source))
                    throw new InvalidOperationException(
                        "no SAV provides the complete structural writer fixture coverage");
                Console.WriteLine("writer fixture: " + source.SourcePath);
                source.WriteCopy(args[2], source.Metadata.Clone());
                if (!Equal(File.ReadAllBytes(source.SourcePath), File.ReadAllBytes(args[2])))
                    throw new InvalidOperationException("no-op output is not byte-identical");
                SavMetadata changed = source.Metadata.Clone();
                changed.CameraX = changed.CameraX == int.MaxValue ? changed.CameraX - 1 : changed.CameraX + 1;
                List<PlayerMessageRecord> messages = new List<PlayerMessageRecord>();
                foreach (PlayerMessageRecord message in source.PlayerMessages)
                    messages.Add(message.Clone());
                if (messages.Count > 0)
                    messages[0].FormattedText += " [SRHD Save Editor self-test]";
                if (messages.Count > 1)
                    messages.RemoveAt(messages.Count - 1);
                GalaxyPrefixData galaxy = source.GalaxyPrefix.Clone();
                galaxy.SaveCount = galaxy.SaveCount == int.MaxValue ? galaxy.SaveCount - 1 : galaxy.SaveCount + 1;
                galaxy.UsedMods += ", SpaceRangersHdSaveEditor\\RoundtripProbe";
                List<ConstellationRecord> constellations = new List<ConstellationRecord>();
                foreach (ConstellationRecord constellation in source.GalaxyConstellations)
                    constellations.Add(constellation.Clone());
                if (constellations.Count == 0)
                    throw new InvalidOperationException("writer self-test requires TConstellation records");
                constellations[0].Visible = !constellations[0].Visible;
                GalaxySummaryData galaxySummary = source.GalaxySummary.Clone();
                galaxySummary.DifficultyLevels[0] = (byte)((galaxySummary.DifficultyLevels[0] + 1) % 10);
                galaxySummary.IronWill = !galaxySummary.IronWill;
                galaxySummary.PlanetBattlesDisabled = !galaxySummary.PlanetBattlesDisabled;
                galaxySummary.BlazerResearch = ShiftCoordinate(galaxySummary.BlazerResearch);
                galaxySummary.BlazerMaterial = ShiftUInt32(galaxySummary.BlazerMaterial);
                galaxySummary.KellerResearch = ShiftCoordinate(galaxySummary.KellerResearch);
                galaxySummary.KellerMaterial = ShiftUInt32(galaxySummary.KellerMaterial);
                galaxySummary.TerronResearch = ShiftCoordinate(galaxySummary.TerronResearch);
                galaxySummary.TerronMaterial = ShiftUInt32(galaxySummary.TerronMaterial);
                galaxySummary.WarDeltaDominators = ShiftInt32(galaxySummary.WarDeltaDominators);
                galaxySummary.WarDeltaPirates = ShiftInt32(galaxySummary.WarDeltaPirates);
                galaxySummary.WarDeltaCoalition = ShiftInt32(galaxySummary.WarDeltaCoalition);
                galaxySummary.CustomRules = !galaxySummary.CustomRules;
                galaxySummary.CustomRuleLevels[0] = ShiftByte(galaxySummary.CustomRuleLevels[0]);
                galaxySummary.CustomRuleFlags[0] = !galaxySummary.CustomRuleFlags[0];
                galaxySummary.HullGrowth = (byte)((galaxySummary.HullGrowth + 1) % 3);
                galaxySummary.CustomRuleLateFlags[0] = !galaxySummary.CustomRuleLateFlags[0];
                galaxySummary.KellerAttackStarObjectId = galaxySummary.KellerAttackStarObjectId == 0 ?
                    source.GalaxyStars[0].ObjectId : 0;
                galaxySummary.KellerAttackState = ShiftInt32(galaxySummary.KellerAttackState);
                bool globalChanged = false;
                foreach (ScriptVariableRecord variable in galaxySummary.GlobalVariables)
                {
                    if (variable.Type == 1 || variable.Type == 2)
                    {
                        variable.IntegerValue = ShiftInt32(variable.IntegerValue);
                        globalChanged = true;
                        break;
                    }
                    if (variable.Type == 3)
                    {
                        variable.DoubleValue = variable.DoubleValue == double.MaxValue ? 0.0 :
                            variable.DoubleValue + 0.5;
                        globalChanged = true;
                        break;
                    }
                    if (variable.Type == 4)
                    {
                        variable.StringValue = (variable.StringValue ?? string.Empty) + " X";
                        globalChanged = true;
                        break;
                    }
                }
                if (!globalChanged)
                {
                    ScriptVariableRecord variable = new ScriptVariableRecord();
                    variable.Name = "SrhdSaveEditorGlobalProbe"; variable.Type = 1; variable.IntegerValue = 17;
                    galaxySummary.GlobalVariables.Add(variable);
                }
                if (galaxySummary.ScriptCache.Count > 0)
                {
                    ScriptCacheRecord cache = galaxySummary.ScriptCache[0];
                    cache.CountUse = ShiftUInt16(cache.CountUse);
                    cache.LastTurn = ShiftInt32(cache.LastTurn);
                    cache.RunScript = ShiftInt32(cache.RunScript);
                }
                else
                {
                    ScriptCacheRecord cache = new ScriptCacheRecord();
                    cache.Name = "SRHD Save Editor cache probe"; cache.CountUse = 1;
                    cache.LastTurn = 2; cache.RunScript = 3;
                    galaxySummary.ScriptCache.Add(cache);
                }
                if (galaxySummary.CompleteQuests.Count > 0)
                {
                    CompleteQuestRecord quest = galaxySummary.CompleteQuests[0];
                    quest.Number = ShiftUInt16(quest.Number);
                    quest.Text = (quest.Text ?? string.Empty) + " X";
                    quest.Successful = !quest.Successful;
                    quest.Rejection = !quest.Rejection;
                    if (galaxySummary.CompleteQuests.Count > 1)
                        galaxySummary.CompleteQuests.RemoveAt(galaxySummary.CompleteQuests.Count - 1);
                }
                if (galaxySummary.GalaxyNews.Count > 0)
                {
                    GalaxyNewsRecord news = galaxySummary.GalaxyNews[0];
                    news.Id = ShiftUInt32(news.Id);
                    news.Turn = ShiftUInt32(news.Turn);
                    news.Type = ShiftByte(news.Type);
                    news.Text = (news.Text ?? string.Empty) + " X";
                    if (galaxySummary.GalaxyNews.Count > 1)
                        galaxySummary.GalaxyNews.RemoveAt(galaxySummary.GalaxyNews.Count - 1);
                }
                if (galaxySummary.GalaxyEvents.Count > 0)
                    galaxySummary.GalaxyEvents.RemoveAt(galaxySummary.GalaxyEvents.Count - 1);
                else
                {
                    GalaxyEventRecord galaxyEvent = new GalaxyEventRecord();
                    galaxyEvent.Type = "SrhdSaveEditorSelfTest";
                    galaxyEvent.Turn = 12345;
                    galaxyEvent.Data.Add(-17);
                    galaxyEvent.Data.Add(42);
                    galaxyEvent.TextData.Add("Синтетическое событие");
                    galaxySummary.GalaxyEvents.Add(galaxyEvent);
                }
                galaxySummary.GalaxyEventCount = galaxySummary.GalaxyEvents.Count;
                if (galaxySummary.WarOperations.Count > 0)
                {
                    WarOperationRecord operation = galaxySummary.WarOperations[0];
                    operation.Turn = ShiftUInt16(operation.Turn);
                    operation.RandomSeed = ShiftUInt32(operation.RandomSeed);
                    operation.RandomOut = ShiftUInt32(operation.RandomOut);
                    if (operation.ShipObjectIds.Count > 0)
                        operation.ShipObjectIds.RemoveAt(operation.ShipObjectIds.Count - 1);
                    if (operation.Orders.Count > 0)
                    {
                        WarOperationOrderRecord order = operation.Orders[0];
                        order.DestinationX = ShiftCoordinate(order.DestinationX);
                        order.DestinationY = ShiftCoordinate(order.DestinationY);
                        order.EndMode = ShiftByte(order.EndMode);
                        order.EndTurn = ShiftInt32(order.EndTurn);
                    }
                    if (galaxySummary.WarOperations.Count > 1)
                        galaxySummary.WarOperations.RemoveAt(galaxySummary.WarOperations.Count - 1);
                }
                else
                {
                    WarOperationRecord operation = new WarOperationRecord();
                    operation.Turn = 1;
                    operation.RandomSeed = 2;
                    operation.RandomOut = 3;
                    operation.LegacyZero = 0;
                    operation.ShipObjectIds.Add(source.GalaxyShips[0].ObjectId);
                    WarOperationOrderRecord order = new WarOperationOrderRecord();
                    order.Type = 0; order.ObjectId = 0; order.DestinationX = 1.25F;
                    order.DestinationY = -2.5F; order.EndMode = 0; order.EndTurn = 4;
                    operation.Orders.Add(order);
                    galaxySummary.WarOperations.Add(operation);
                }
                if (galaxySummary.Gates.Count > 0)
                {
                    GateRecord gate = galaxySummary.Gates[0];
                    gate.X = ShiftCoordinate(gate.X); gate.Y = ShiftCoordinate(gate.Y);
                    gate.Angle = ShiftByte(gate.Angle); gate.Size = ShiftUInt16(gate.Size);
                    gate.Text = (gate.Text ?? string.Empty) + " X";
                    if (galaxySummary.Gates.Count > 1)
                        galaxySummary.Gates.RemoveAt(galaxySummary.Gates.Count - 1);
                }
                else
                {
                    GateRecord gate = new GateRecord();
                    gate.X = 1.5F; gate.Y = -2.25F; gate.Angle = 3; gate.Size = 64;
                    gate.Text = "SRHD Save Editor self-test gate";
                    galaxySummary.Gates.Add(gate);
                }
                if (galaxySummary.ActiveScripts.Count > 0)
                {
                    ScriptRecord script = galaxySummary.ActiveScripts[0];
                    script.Name += " X";
                    if (script.InitVariables.Count > 0)
                        script.InitVariables[0].Name += " X";
                    if (script.ItemBindings.Count > 0)
                    {
                        script.ItemBindings[0].CanSell = !script.ItemBindings[0].CanSell;
                        script.ItemBindings[0].OnUseCode += " X";
                    }
                    if (script.ShipBindings.Count > 0)
                        script.ShipBindings[0].Hit = !script.ShipBindings[0].Hit;
                    script.EtherStrings.Add("SRHD Save Editor self-test ether");
                }
                else
                {
                    ScriptRecord script = new ScriptRecord();
                    script.Name = "SRHD Save Editor self-test script";
                    ScriptOldEtherRecord oldEther = new ScriptOldEtherRecord();
                    oldEther.Name = "SRHD Save Editor legacy ether"; oldEther.Value = 17;
                    script.OldEthers.Add(oldEther);
                    ScriptVariableRecord nullValue = new ScriptVariableRecord();
                    nullValue.Name = "NullValue"; nullValue.Type = 0;
                    script.InitVariables.Add(nullValue);
                    ScriptVariableRecord integerValue = new ScriptVariableRecord();
                    integerValue.Name = "IntegerValue"; integerValue.Type = 1; integerValue.IntegerValue = -123;
                    script.InitVariables.Add(integerValue);
                    ScriptVariableRecord dwordValue = new ScriptVariableRecord();
                    dwordValue.Name = "DwordValue"; dwordValue.Type = 2; dwordValue.IntegerValue = 456;
                    script.InitVariables.Add(dwordValue);
                    ScriptVariableRecord floatValue = new ScriptVariableRecord();
                    floatValue.Name = "FloatValue"; floatValue.Type = 3; floatValue.DoubleValue = 1.25;
                    script.InitVariables.Add(floatValue);
                    ScriptVariableRecord stringValue = new ScriptVariableRecord();
                    stringValue.Name = "StringValue"; stringValue.Type = 4; stringValue.StringValue = "Тест";
                    script.TurnVariables.Add(stringValue);
                    ScriptVariableRecord libraryValue = new ScriptVariableRecord();
                    libraryValue.Name = "LibraryValue"; libraryValue.Type = 6;
                    libraryValue.StringValue = "SrhdSaveEditor.Test.Library";
                    script.TurnVariables.Add(libraryValue);
                    ScriptVariableRecord arrayValue = new ScriptVariableRecord();
                    arrayValue.Name = "ArrayValue"; arrayValue.Type = 9;
                    ScriptVariableRecord nestedValue = new ScriptVariableRecord();
                    nestedValue.Name = "Nested"; nestedValue.Type = 1; nestedValue.IntegerValue = 789;
                    arrayValue.ArrayValue.Add(nestedValue);
                    script.TurnVariables.Add(arrayValue);
                    ScriptStarBindingRecord starBinding = new ScriptStarBindingRecord();
                    starBinding.Name = "TestStar";
                    starBinding.StarObjectId = source.GalaxyStars[0].ObjectId;
                    starBinding.LegacyZero = 0;
                    ScriptPlanetBindingRecord planetBinding = new ScriptPlanetBindingRecord();
                    planetBinding.Name = "TestPlanet";
                    planetBinding.PlanetObjectId = source.GalaxyPlanets[0].ObjectId;
                    starBinding.Planets.Add(planetBinding);
                    script.StarBindings.Add(starBinding);
                    ScriptItemRecord itemBinding = new ScriptItemRecord();
                    itemBinding.Name = "TestItem"; itemBinding.CanSell = true;
                    itemBinding.Data1 = 1; itemBinding.Data2 = -2; itemBinding.Data3 = 3;
                    itemBinding.TextData1 = "Text1"; itemBinding.TextData2 = "Text2";
                    itemBinding.TextData3 = "Text3"; itemBinding.OnUseCode = "OnUse";
                    itemBinding.OnActCode = "OnAct";
                    itemBinding.ItemObjectId = source.GalaxyItems[0].ObjectId;
                    script.ItemBindings.Add(itemBinding);
                    ScriptShipRecord shipBinding = new ScriptShipRecord();
                    shipBinding.Group = -4; shipBinding.ShipObjectId = source.GalaxyShips[0].ObjectId;
                    shipBinding.Data0 = 5; shipBinding.Data1 = 6; shipBinding.Data2 = 7;
                    shipBinding.Data3 = 8; shipBinding.StateNum = -9;
                    shipBinding.CustomFaction = "SrhdSaveEditorFaction";
                    shipBinding.Hit = true; shipBinding.HitPlayer = false;
                    script.ShipBindings.Add(shipBinding);
                    script.EtherStrings.Add("SRHD Save Editor self-test ether");
                    galaxySummary.ActiveScripts.Add(script);
                }
                List<StarHeaderRecord> stars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord star in source.GalaxyStars) stars.Add(star.Clone());
                stars[0].Name += " X";
                stars[0].X = stars[0].X >= 4096 ? stars[0].X - 1 : stars[0].X + 1;
                stars[0].Raw1C = stars[0].Raw1C >= 300 ? (ushort)299 : (ushort)(stars[0].Raw1C + 1);
                stars[0].Raw78 = ShiftByte(stars[0].Raw78);
                stars[0].Battle = !stars[0].Battle;
                stars[0].Safety = stars[0].Safety >= 100 ? (byte)99 : (byte)(stars[0].Safety + 1);
                stars[0].Owners = (byte)((stars[0].Owners + 1) % 3);
                stars[0].LastOwners = (byte)((stars[0].LastOwners + 1) % 3);
                stars[0].DominatorSeries = (byte)((stars[0].DominatorSeries + 1) % 3);
                stars[0].CustomFaction = (stars[0].CustomFaction ?? string.Empty) + ".SRHD Save Editor";
                stars[0].SafeRadius = ShiftCoordinate(stars[0].SafeRadius);
                stars[0].DamageRadius = ShiftCoordinate(stars[0].DamageRadius);
                stars[0].GraphRadius = stars[0].GraphRadius == ushort.MaxValue
                    ? (ushort)(ushort.MaxValue - 1) : (ushort)(stars[0].GraphRadius + 1);
                stars[0].MapLabel = (stars[0].MapLabel ?? string.Empty) + ".SRHD Save Editor";
                stars[0].NoComeKling = !stars[0].NoComeKling;
                stars[0].DayBeforeOccupy = ShiftByte(stars[0].DayBeforeOccupy);
                stars[0].DayWithoutPlayer = ShiftInt32(stars[0].DayWithoutPlayer);
                stars[0].DayWithoutCreateShip = ShiftInt32(stars[0].DayWithoutCreateShip);
                stars[0].LastDominatorDate = ShiftInt32(stars[0].LastDominatorDate);
                stars[0].LastPirateDate = ShiftInt32(stars[0].LastPirateDate);
                stars[0].LiberationDate = ShiftInt32(stars[0].LiberationDate);
                stars[0].DayInvadeInertia = ShiftInt32(stars[0].DayInvadeInertia);
                if (stars[0].CustomSystemInfos.Count == 0)
                {
                    CustomSystemInfoRecord customInfo = new CustomSystemInfoRecord();
                    customInfo.Name = "SRHD Save Editor"; customInfo.Icon = "SRHD Save Editor.Icon";
                    customInfo.Info = "TStar writer self-test"; customInfo.Type = "test";
                    customInfo.Distance = 42; stars[0].CustomSystemInfos.Add(customInfo);
                }
                else
                {
                    stars[0].CustomSystemInfos[0].Info += " [SRHD Save Editor]";
                    stars[0].CustomSystemInfos[0].Distance =
                        ShiftInt32(stars[0].CustomSystemInfos[0].Distance);
                }
                int dropStarIndex = -1;
                for (int starIndex = 0; starIndex < stars.Count; starIndex++)
                {
                    if (stars[starIndex].DropItems.Count == 0) continue;
                    dropStarIndex = starIndex;
                    StarDropItemRecord drop = stars[starIndex].DropItems[0];
                    drop.X = ShiftCoordinate(drop.X);
                    drop.Y = ShiftCoordinate(drop.Y);
                    drop.InUse = !drop.InUse;
                    if (stars[starIndex].DropItems.Count > 1)
                        stars[starIndex].DropItems.RemoveAt(stars[starIndex].DropItems.Count - 1);
                    break;
                }
                List<PlanetHeaderRecord> planets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord planet in source.GalaxyPlanets) planets.Add(planet.Clone());
                planets[0].Name += " P";
                planets[0].Raw08 = ShiftInt32(planets[0].Raw08);
                planets[0].Raw0C = ShiftUInt32(planets[0].Raw0C);
                planets[0].PolarAngle = ShiftCoordinate(planets[0].PolarAngle);
                planets[0].PolarRadius = ShiftCoordinate(planets[0].PolarRadius);
                planets[0].Angle = ShiftCoordinate(planets[0].Angle);
                planets[0].Mass = ShiftInt32(planets[0].Mass);
                planets[0].Radius = ShiftInt32(planets[0].Radius);
                planets[0].WaterSpace = ShiftInt32(planets[0].WaterSpace);
                planets[0].WaterSpaceDone = ShiftInt32(planets[0].WaterSpaceDone);
                planets[0].LandSpace = ShiftInt32(planets[0].LandSpace);
                planets[0].LandSpaceDone = ShiftInt32(planets[0].LandSpaceDone);
                planets[0].HillSpace = ShiftInt32(planets[0].HillSpace);
                planets[0].HillSpaceDone = ShiftInt32(planets[0].HillSpaceDone);
                planets[0].OrbitCount = ShiftByte(planets[0].OrbitCount);
                planets[0].VisitedByPlayer = !planets[0].VisitedByPlayer;
                planets[0].OpenInventions[0] = planets[0].OpenInventions[0] >= 16
                    ? (byte)15 : (byte)(planets[0].OpenInventions[0] + 1);
                planets[0].CurrentInvention = ShiftByte(planets[0].CurrentInvention);
                planets[0].OpenPointsInvention = ShiftCoordinate(planets[0].OpenPointsInvention);
                planets[0].NecessaryPercent = ShiftByte(planets[0].NecessaryPercent);
                planets[0].NecessaryPercentK = ShiftByte(planets[0].NecessaryPercentK);
                planets[0].PeopleCount = ShiftUInt32(planets[0].PeopleCount);
                planets[0].Economy = ShiftByte(planets[0].Economy);
                planets[0].Money = ShiftUInt32(planets[0].Money);
                planets[0].Owner = ShiftByte(planets[0].Owner);
                planets[0].Race = ShiftByte(planets[0].Race);
                planets[0].Government = ShiftByte(planets[0].Government);
                planets[0].ShopGoods[0, 0] = ShiftUInt32(planets[0].ShopGoods[0, 0]);
                planets[0].ShopGoods[0, 1] = ShiftUInt32(planets[0].ShopGoods[0, 1]);
                planets[0].ShopGoods[0, 2] = ShiftUInt32(planets[0].ShopGoods[0, 2]);
                planets[0].ShopDeficit[0] = ShiftByte(planets[0].ShopDeficit[0]);
                planets[0].ShopSale[0] = ShiftByte(planets[0].ShopSale[0]);
                if (planets[0].RelationToRangers == null ||
                    planets[0].RelationToRangers.Length == 0)
                    throw new InvalidOperationException(
                        "writer self-test requires TPlanet RelationToRangers values");
                planets[0].RelationToRangers[0] =
                    (byte)((planets[0].RelationToRangers[0] + 1) % 101);
                if (!planets[0].HasLateFields || !planets[0].HasFlags)
                    throw new InvalidOperationException("first TPlanet has no proven late fields");
                planets[0].RangerCount = ShiftUInt16(planets[0].RangerCount);
                planets[0].TransportCount = ShiftUInt16(planets[0].TransportCount);
                planets[0].GraphRadius = ShiftUInt16(planets[0].GraphRadius);
                planets[0].GraphName += ".SrhdSaveEditorTest";
                planets[0].GraphSpeedRotate = ShiftUInt16(planets[0].GraphSpeedRotate);
                planets[0].GraphStepRotate = ShiftInt32(planets[0].GraphStepRotate);
                planets[0].GraphRing = ShiftByte(planets[0].GraphRing);
                planets[0].QuestNumber = ShiftInt32(planets[0].QuestNumber);
                planets[0].NoLanding = !planets[0].NoLanding;
                planets[0].NoPlanetShopUpdate = (byte)((planets[0].NoPlanetShopUpdate + 1) & 3);
                planets[0].NoBuyShips = !planets[0].NoBuyShips;
                planets[0].NoRandomEvents = !planets[0].NoRandomEvents;
                planets[0].IsRogeria = !planets[0].IsRogeria;
                planets[0].CustomFaction = string.IsNullOrEmpty(planets[0].CustomFaction)
                    ? "SRHD Save Editor test" : planets[0].CustomFaction + " test";
                PlanetHeaderRecord editedSputnikPlanet = null;
                PlanetHeaderRecord deletedSputnikPlanet = null;
                foreach (PlanetHeaderRecord planet in planets)
                {
                    if (planet.Satellites.Count == 0) continue;
                    if (editedSputnikPlanet == null)
                    {
                        editedSputnikPlanet = planet;
                        PlanetSputnikRecord satellite = planet.Satellites[0];
                        satellite.GraphName += ".SrhdSaveEditorTest";
                        satellite.AngleCurrent = ShiftSerializedFloat(satellite.AngleCurrent);
                        continue;
                    }
                    deletedSputnikPlanet = planet;
                    planet.Satellites.RemoveAt(planet.Satellites.Count - 1);
                    planet.SatelliteCount = checked((ushort)planet.Satellites.Count);
                    break;
                }
                if (editedSputnikPlanet == null || deletedSputnikPlanet == null)
                    throw new InvalidOperationException("writer self-test requires two TPlanet TSputnik lists");
                PlanetHeaderRecord editedGoneItemPlanet = null;
                PlanetHeaderRecord deletedGoneItemPlanet = null;
                int deletedGoneItemStart = -1;
                foreach (PlanetHeaderRecord planet in planets)
                {
                    if (planet.GoneItems.Count == 0) continue;
                    if (editedGoneItemPlanet == null)
                    {
                        editedGoneItemPlanet = planet;
                        PlanetGoneItemRecord goneItem = planet.GoneItems[0];
                        goneItem.PosX = ShiftByte(goneItem.PosX);
                        goneItem.PosY = ShiftByte(goneItem.PosY);
                        goneItem.LandType = ShiftByte(goneItem.LandType);
                        goneItem.Region = ShiftInt32(goneItem.Region);
                        goneItem.Miss = !goneItem.Miss;
                        continue;
                    }
                    deletedGoneItemPlanet = planet;
                    PlanetGoneItemRecord deleted = planet.GoneItems[planet.GoneItems.Count - 1];
                    deletedGoneItemStart = deleted.ItemStart;
                    planet.GoneItems.RemoveAt(planet.GoneItems.Count - 1);
                    planet.GoneItemCount = checked((ushort)planet.GoneItems.Count);
                    break;
                }
                if (editedGoneItemPlanet == null || deletedGoneItemPlanet == null)
                    throw new InvalidOperationException("writer self-test requires two TPlanet GoneItems lists");
                PlanetHeaderRecord editedShopPlanet = null;
                PlanetHeaderRecord deletedShopPlanet = null;
                int editedShopItemStart = -1;
                int deletedShopItemStart = -1;
                foreach (PlanetHeaderRecord planet in planets)
                {
                    if (planet.EquipmentShopItems.Count == 0) continue;
                    if (editedShopPlanet == null)
                    {
                        editedShopPlanet = planet;
                        editedShopItemStart = planet.EquipmentShopItems[0].ItemStart;
                        continue;
                    }
                    deletedShopPlanet = planet;
                    ShipItemListEntry deleted =
                        planet.EquipmentShopItems[planet.EquipmentShopItems.Count - 1];
                    deletedShopItemStart = deleted.ItemStart;
                    planet.EquipmentShopItems.RemoveAt(planet.EquipmentShopItems.Count - 1);
                    planet.EquipmentShopCount = checked((ushort)planet.EquipmentShopItems.Count);
                    break;
                }
                if (editedShopPlanet == null || deletedShopPlanet == null)
                    throw new InvalidOperationException(
                        "writer self-test requires two TPlanet EquipmentShop lists");
                int editedPlanetWarriorShipStart = -1;
                foreach (PlanetHeaderRecord planet in planets)
                    if (planet.Warriors.Count > 0)
                    {
                        editedPlanetWarriorShipStart = planet.Warriors[0].ShipStart;
                        break;
                    }
                if (editedPlanetWarriorShipStart < 0)
                    throw new InvalidOperationException("writer self-test requires TPlanet Warriors");
                List<ShipHeaderRecord> ships = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in source.GalaxyShips) ships.Add(ship.Clone());
                ShipHeaderRecord editedPlanetWarriorShip = null;
                foreach (ShipHeaderRecord ship in ships)
                    if (ship.Start == editedPlanetWarriorShipStart)
                    {
                        editedPlanetWarriorShip = ship;
                        break;
                    }
                if (editedPlanetWarriorShip == null)
                    throw new InvalidOperationException("writer self-test did not resolve TPlanet Warrior TShip");
                editedPlanetWarriorShip.Money = ShiftUInt32(editedPlanetWarriorShip.Money);
                HashSet<int> deletedShipItemStarts = new HashSet<int>();
                deletedShipItemStarts.Add(deletedGoneItemStart);
                deletedShipItemStarts.Add(deletedShopItemStart);
                HashSet<uint> collectionMutatedShipIds = new HashSet<uint>();
                int normalShipIndex = FindNormalShipIndex(ships);
                int[] simpleShipIndices = FindSimpleDerivedShipIndices(ships);
                int rangerShipIndex = FindRangerShipIndex(ships);
                int playerShipIndex = FindPlayerShipIndex(ships);
                int ruinsShipIndex = FindRuinsShipIndex(ships);
                ships[0].Name += " S";
                ships[0].ScriptName = string.IsNullOrEmpty(ships[0].ScriptName)
                    ? "SRHD Save Editor test" : ships[0].ScriptName + " S";
                ships[0].X = ShiftCoordinate(ships[0].X);
                ships[0].Y = ShiftCoordinate(ships[0].Y);
                ships[0].Owner = (byte)((ships[0].Owner + 1) % 8);
                ships[0].PilotRace = (byte)((ships[0].PilotRace + 1) % 5);
                ships[0].Money = ShiftUInt32(ships[0].Money);
                ships[0].Rnd = ShiftUInt32(ships[0].Rnd);
                ships[0].RndOut = ShiftUInt32(ships[0].RndOut);
                ships[0].Day = ShiftUInt32(ships[0].Day);
                ships[0].Face = ShiftInt32(ships[0].Face);
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 4; field++)
                        ships[0].Goods[good, field] = ShiftUInt32(ships[0].Goods[good, field]);
                if (!ships[0].HasCommonTail)
                    throw new InvalidOperationException("first TShip has no proven common tail");
                ships[0].Forsage = !ships[0].Forsage;
                ships[0].Angle = ShiftCoordinate(ships[0].Angle);
                ships[0].OrderType = (byte)((ships[0].OrderType + 1) % 8);
                ships[0].OrderData = ShiftUInt32(ships[0].OrderData);
                ships[0].OrderDestinationX = ShiftCoordinate(ships[0].OrderDestinationX);
                ships[0].OrderDestinationY = ShiftCoordinate(ships[0].OrderDestinationY);
                ships[0].OrderAbsolute = !ships[0].OrderAbsolute;
                ships[0].Abducted = !ships[0].Abducted;
                ships[0].DaysLanded = ShiftInt32(ships[0].DaysLanded);
                ships[0].ScriptOrderAbsolute = ShiftByte(ships[0].ScriptOrderAbsolute);
                ships[0].GraphDominator = !ships[0].GraphDominator;
                ships[0].GraphName = ships[0].GraphName.Length <= 108
                    ? ships[0].GraphName + ".SrhdSaveEditorTest" : "Ship.SrhdSaveEditorTest";
                ships[0].GraphShipTransparency = ShiftByte(ships[0].GraphShipTransparency);
                ships[0].InHyperSpace = !ships[0].InHyperSpace;
                ships[0].RadiusStop = ShiftCoordinate(ships[0].RadiusStop);
                ships[0].ShipDestroy = !ships[0].ShipDestroy;
                for (int skill = 0; skill < ships[0].Skills.Length; skill++)
                    ships[0].Skills[skill] = ShiftByte(ships[0].Skills[skill]);
                ships[0].Protoplasm = ShiftUInt16(ships[0].Protoplasm);
                ships[0].Points = ShiftUInt32(ships[0].Points);
                ships[0].FreePoints = ShiftUInt32(ships[0].FreePoints);
                ships[0].DayWithoutPlayer = ShiftUInt16(ships[0].DayWithoutPlayer);
                ships[0].GroupOrder = ShiftUInt16(ships[0].GroupOrder);
                ShipIllnessRecord firstIllness = ships[0].Illnesses[0];
                firstIllness.Infection = ShiftSerializedFloat(firstIllness.Infection);
                firstIllness.InfectionDay = ShiftInt32(firstIllness.InfectionDay);
                firstIllness.InfectionEndDay = ShiftInt32(firstIllness.InfectionEndDay);
                firstIllness.InfectionCount = ShiftInt32(firstIllness.InfectionCount);
                ShipIllnessRecord stimulator = ships[0].Illnesses[24];
                stimulator.Infection = ShiftSerializedFloat(stimulator.Infection);
                stimulator.InfectionDay = ShiftInt32(stimulator.InfectionDay);
                stimulator.InfectionEndDay = ShiftInt32(stimulator.InfectionEndDay);
                stimulator.InfectionCount = ShiftInt32(stimulator.InfectionCount);
                if (ships[0].Rewards.Count >= byte.MaxValue)
                    throw new InvalidOperationException("first TShip reward list is full");
                if (ships[0].Rewards.Count != 0)
                    ships[0].Rewards[0] = ShiftByte(ships[0].Rewards[0]);
                ships[0].Rewards.Add(47);
                ships[0].LastNextDay = ShiftInt32(ships[0].LastNextDay);
                ships[0].ChameleonEnabled = !ships[0].ChameleonEnabled;
                ships[0].ChameleonSeries = (byte)((ships[0].ChameleonSeries + 1) % 3);
                ships[0].BlazerChameleonDetect = ships[0].BlazerChameleonDetect == 0 ? (byte)1 : (byte)0;
                ships[0].KellerChameleonDetect = ships[0].KellerChameleonDetect == 0 ? (byte)1 : (byte)0;
                ships[0].TerronChameleonDetect = ships[0].TerronChameleonDetect == 0 ? (byte)1 : (byte)0;
                ships[0].BlazerChameleonCharge = ShiftInt32(ships[0].BlazerChameleonCharge);
                ships[0].KellerChameleonCharge = ShiftInt32(ships[0].KellerChameleonCharge);
                ships[0].TerronChameleonCharge = ShiftInt32(ships[0].TerronChameleonCharge);
                ships[0].TechLevelKnowledge = ShiftByte(ships[0].TechLevelKnowledge);
                ships[0].TradePenalty = ShiftInt32(ships[0].TradePenalty);
                ships[0].TradePoints = ShiftInt32(ships[0].TradePoints);
                ships[0].ContrabandPoints = ShiftInt32(ships[0].ContrabandPoints);
                ships[0].RewardViewCount = ShiftInt32(ships[0].RewardViewCount);
                ships[0].NoDrop = !ships[0].NoDrop;
                ships[0].NoTarget = (byte)((ships[0].NoTarget + 1) % 7);
                ships[0].NoTalk = !ships[0].NoTalk;
                ships[0].NoScan = !ships[0].NoScan;
                ships[0].ScriptChameleon = !ships[0].ScriptChameleon;
                ships[0].RobbedByPlayer = !ships[0].RobbedByPlayer;
                ships[0].CountOfDeflectedPlayerShots = ShiftUInt16(ships[0].CountOfDeflectedPlayerShots);
                if (ships[0].Swarmed > 0)
                {
                    ships[0].Swarmed = 0;
                    ships[0].SwarmAnimation = string.Empty;
                }
                else
                {
                    ships[0].Swarmed = 1;
                    ships[0].SwarmAnimation = "SrhdSaveEditorTest";
                }
                ships[0].CurrentStanding = (byte)((ships[0].CurrentStanding + 1) % 10);
                ships[0].AverageSpeed = ShiftInt32(ships[0].AverageSpeed);
                ships[0].AverageEnemySpeed = ShiftInt32(ships[0].AverageEnemySpeed);
                ships[0].AverageEquipmentValue = ShiftCoordinate(ships[0].AverageEquipmentValue);
                ships[0].AverageCapital = ShiftInt32(ships[0].AverageCapital);
                ships[0].AverageMoneyToCapital = ShiftCoordinate(ships[0].AverageMoneyToCapital);
                ships[0].AverageFreeSpaceRatio = ShiftCoordinate(ships[0].AverageFreeSpaceRatio);
                ships[0].RatioOfTooCostlyEquipmentInShop = ShiftCoordinate(ships[0].RatioOfTooCostlyEquipmentInShop);
                ShipHeaderRecord normalShip = ships[normalShipIndex];
                normalShip.KillAllShips = ShiftInt32(normalShip.KillAllShips);
                normalShip.KillPirates = ShiftInt32(normalShip.KillPirates);
                normalShip.KillDominators = ShiftInt32(normalShip.KillDominators);
                normalShip.LiberationSystems = ShiftInt32(normalShip.LiberationSystems);
                normalShip.KillPacifics = ShiftInt32(normalShip.KillPacifics);
                normalShip.KillWarriors = ShiftInt32(normalShip.KillWarriors);
                normalShip.KillRangers = ShiftInt32(normalShip.KillRangers);
                normalShip.KillInCurrentSystemDominators = ShiftUInt16(normalShip.KillInCurrentSystemDominators);
                normalShip.KillInCurrentSystemPirates = ShiftUInt16(normalShip.KillInCurrentSystemPirates);
                normalShip.KillInCurrentSystemNormals = ShiftUInt16(normalShip.KillInCurrentSystemNormals);
                normalShip.KillCustomInCurrentSystem = ShiftUInt16(normalShip.KillCustomInCurrentSystem);
                normalShip.LiberationPlanetId = normalShip.LiberationPlanetId == 0
                    ? planets[0].ObjectId : 0;
                normalShip.LiberationKills = ShiftInt32(normalShip.LiberationKills);
                normalShip.CoalitionRank = (byte)((normalShip.CoalitionRank + 1) % 8);
                normalShip.CoalitionRankPoints = ShiftUInt16(normalShip.CoalitionRankPoints);
                normalShip.PirateRank = (byte)((normalShip.PirateRank + 1) % 8);
                normalShip.PirateRankPoints = ShiftUInt32(normalShip.PirateRankPoints);
                normalShip.LastPlanetId = normalShip.LastPlanetId == 0 ? planets[0].ObjectId : 0;
                normalShip.TurnPlayerMoneyGoods = ShiftInt32(normalShip.TurnPlayerMoneyGoods);
                foreach (int simpleIndex in simpleShipIndices)
                {
                    ShipHeaderRecord simple = ships[simpleIndex];
                    if (simple.Type == 0)
                    {
                        simple.DominatorType = (byte)((simple.DominatorType + 1) % 8);
                        simple.DominatorSeries = (byte)((simple.DominatorSeries + 1) % 3);
                        simple.RunProgramDate = ShiftInt32(simple.RunProgramDate);
                        simple.RunProgramName = (byte)((simple.RunProgramName + 1) % 12);
                    }
                    else if (simple.Type == 2) simple.TransportType = (byte)((simple.TransportType + 1) % 3);
                    else if (simple.Type == 3)
                    {
                        simple.PiratePrison = ShiftUInt32(simple.PiratePrison);
                        simple.PirateType = (byte)((simple.PirateType + 1) % 4);
                        simple.DesireConflict = ShiftCoordinate(simple.DesireConflict);
                    }
                    else if (simple.Type == 4) simple.WarriorType = (byte)((simple.WarriorType + 1) % 2);
                }
                ShipHeaderRecord rangerShip = ships[rangerShipIndex];
                rangerShip.RangerStatusTrader = ShiftByte(rangerShip.RangerStatusTrader);
                rangerShip.RangerStatusPirate = ShiftByte(rangerShip.RangerStatusPirate);
                rangerShip.RangerStatusWarrior = ShiftByte(rangerShip.RangerStatusWarrior);
                rangerShip.EminentPointsTrader = ShiftByte(rangerShip.EminentPointsTrader);
                rangerShip.EminentPointsPirate = ShiftByte(rangerShip.EminentPointsPirate);
                rangerShip.EminentPointsWarrior = ShiftByte(rangerShip.EminentPointsWarrior);
                rangerShip.RangerMoral = (byte)((rangerShip.RangerMoral + 1) % 3);
                rangerShip.Courageous = (byte)((rangerShip.Courageous + 1) % 101);
                rangerShip.StatusChangeWarrior = ShiftByte(rangerShip.StatusChangeWarrior);
                rangerShip.StatusChangePirate = ShiftByte(rangerShip.StatusChangePirate);
                rangerShip.StatusChangeTrader = ShiftByte(rangerShip.StatusChangeTrader);
                rangerShip.RangerPrison = ShiftUInt32(rangerShip.RangerPrison);
                rangerShip.LastShipId = rangerShip.LastShipId == 0 ? ships[0].ObjectId : 0;
                rangerShip.Nods = ShiftInt32(rangerShip.Nods);
                for (int index = 0; index < rangerShip.ProgramCounts.Length; index++)
                    rangerShip.ProgramCounts[index] = ShiftInt32(rangerShip.ProgramCounts[index]);
                rangerShip.ExcludedFromRating = !rangerShip.ExcludedFromRating;
                RangerQuestRecord addedQuest = new RangerQuestRecord();
                addedQuest.Type = 1;
                addedQuest.Number = 65534;
                addedQuest.PlanetObjectId = planets[0].ObjectId;
                addedQuest.Turn = 123456789;
                addedQuest.Reward = -123456789;
                addedQuest.ObjectId = ships[0].ObjectId;
                addedQuest.Successful = true;
                addedQuest.Text = "SRHD Save Editor <clr=00FF00>quest</clr>\r\nround-trip";
                addedQuest.Congratulations = "SRHD Save Editor congratulations";
                addedQuest.SpecialText = "SRHD Save Editor special\ttext";
                rangerShip.RangerQuests.Add(addedQuest);
                rangerShip.RangerQuestCount = checked((ushort)rangerShip.RangerQuests.Count);
                ShipHeaderRecord playerShip = ships[playerShipIndex];
                playerShip.PlayerPrison = !playerShip.PlayerPrison;
                playerShip.PlayerTalkLocked = !playerShip.PlayerTalkLocked;
                playerShip.PlayerScanLocked = !playerShip.PlayerScanLocked;
                playerShip.KillShipInHyperSpace = ShiftInt32(playerShip.KillShipInHyperSpace);
                playerShip.KillShipInHole = ShiftInt32(playerShip.KillShipInHole);
                for (int index = 0; index < playerShip.KillDominatorsByType.Length; index++)
                    playerShip.KillDominatorsByType[index] = ShiftInt32(
                        playerShip.KillDominatorsByType[index]);
                for (int index = 0; index < playerShip.ChameleonLogic.Length; index++)
                    playerShip.ChameleonLogic[index] = ShiftByte(playerShip.ChameleonLogic[index]);
                playerShip.PlayerDebt = ShiftInt32(playerShip.PlayerDebt);
                playerShip.PlayerDebtDate = ShiftInt32(playerShip.PlayerDebtDate);
                playerShip.PlayerDebtCount = ShiftInt32(playerShip.PlayerDebtCount);
                playerShip.PlayerDeposit = ShiftInt32(playerShip.PlayerDeposit);
                playerShip.PlayerDepositDate = ShiftInt32(playerShip.PlayerDepositDate);
                playerShip.PlayerDepositDay = ShiftInt32(playerShip.PlayerDepositDay);
                playerShip.PlayerDepositPercent = playerShip.PlayerDepositPercent >= 999.0F
                    ? playerShip.PlayerDepositPercent - 1.0F : playerShip.PlayerDepositPercent + 1.0F;
                playerShip.PlayerMedPolicy = ShiftInt32(playerShip.PlayerMedPolicy);
                playerShip.PlayerPirateLicense = ShiftInt32(playerShip.PlayerPirateLicense);
                playerShip.PlayerPiratePoints = ShiftInt32(playerShip.PlayerPiratePoints);
                playerShip.PlayerPirateNewPoints = ShiftInt32(playerShip.PlayerPirateNewPoints);
                playerShip.PlayerFlyToStarId = playerShip.PlayerFlyToStarId == 0 ? stars[0].ObjectId : 0;
                for (int index = 0; index < playerShip.PlayerInvestments.Length; index++)
                    playerShip.PlayerInvestments[index] = ShiftInt32(playerShip.PlayerInvestments[index]);
                playerShip.PlayerImmunity = ShiftByte(playerShip.PlayerImmunity);
                for (int index = 0; index < playerShip.PlayerProgramsInWarBase.Length; index++)
                    playerShip.PlayerProgramsInWarBase[index] = ShiftInt32(
                        playerShip.PlayerProgramsInWarBase[index]);
                playerShip.PlayerDayWarBaseGivePrograms = ShiftInt32(
                    playerShip.PlayerDayWarBaseGivePrograms);
                playerShip.PlayerHitEnemyAfterPrograms = ShiftInt32(
                    playerShip.PlayerHitEnemyAfterPrograms);
                if (playerShip.PlayerRobotMaps.Count == 0)
                {
                    PlayerRobotMapRecord robotMap = new PlayerRobotMapRecord();
                    robotMap.Id = -2000000000;
                    robotMap.Time = 2000000000;
                    robotMap.BuildRobot = 101;
                    robotMap.KillRobot = 202;
                    robotMap.BuildTurret = 303;
                    robotMap.KillTurret = 404;
                    robotMap.KillBuilding = 505;
                    robotMap.Bonus = -606;
                    robotMap.State = 707;
                    robotMap.Turn = 808;
                    playerShip.PlayerRobotMaps.Add(robotMap);
                }
                else
                {
                    PlayerRobotMapRecord robotMap = playerShip.PlayerRobotMaps[0];
                    robotMap.Id = ShiftInt32(robotMap.Id);
                    robotMap.Time = ShiftInt32(robotMap.Time);
                    robotMap.BuildRobot = ShiftInt32(robotMap.BuildRobot);
                    robotMap.KillRobot = ShiftInt32(robotMap.KillRobot);
                    robotMap.BuildTurret = ShiftInt32(robotMap.BuildTurret);
                    robotMap.KillTurret = ShiftInt32(robotMap.KillTurret);
                    robotMap.KillBuilding = ShiftInt32(robotMap.KillBuilding);
                    robotMap.Bonus = ShiftInt32(robotMap.Bonus);
                    robotMap.State = ShiftInt32(robotMap.State);
                    robotMap.Turn = ShiftInt32(robotMap.Turn);
                    playerShip.PlayerRobotMaps.RemoveAt(playerShip.PlayerRobotMaps.Count - 1);
                }
                playerShip.PlayerRobotMapCount = playerShip.PlayerRobotMaps.Count;
                playerShip.PlayerPlanetBattlesWin = ShiftInt32(playerShip.PlayerPlanetBattlesWin);
                playerShip.PlayerLastPlanetBattleDate = ShiftInt32(
                    playerShip.PlayerLastPlanetBattleDate);
                playerShip.PlayerPlanetBattlesRejected = !playerShip.PlayerPlanetBattlesRejected;
                playerShip.PlayerIllnessCount = ShiftUInt16(playerShip.PlayerIllnessCount);
                playerShip.PlayerStimulatorCount = ShiftUInt16(playerShip.PlayerStimulatorCount);
                playerShip.PlayerPrisonCount = ShiftUInt16(playerShip.PlayerPrisonCount);
                playerShip.PlayerUnknownPlanetComplete = ShiftInt32(
                    playerShip.PlayerUnknownPlanetComplete);
                playerShip.PlayerChangeRaceCount = ShiftUInt16(playerShip.PlayerChangeRaceCount);
                playerShip.PlayerChangeSideCount = ShiftUInt16(playerShip.PlayerChangeSideCount);
                playerShip.PlayerHotEquipmentCurrent = (byte)((playerShip.PlayerHotEquipmentCurrent + 1) % 10);
                playerShip.PlayerGoToGovernment = ShiftByte(playerShip.PlayerGoToGovernment);
                playerShip.PlayerNoJump = !playerShip.PlayerNoJump;
                PlayerJournalRecord addedJournal = new PlayerJournalRecord();
                addedJournal.Turn = 987654321;
                addedJournal.Text = "SRHD Save Editor journal\r\nvariable-length round-trip";
                playerShip.PlayerJournalRecords.Add(addedJournal);
                GalaxyNewsRecord addedPlayerNews = new GalaxyNewsRecord();
                addedPlayerNews.Id = 4000000000u;
                addedPlayerNews.Turn = 123456789u;
                addedPlayerNews.Type = 255;
                addedPlayerNews.Text = "SRHD Save Editor <clr=00FF00>player news</clr>\r\nvariable-length round-trip";
                playerShip.PlayerNewsRecords.Add(addedPlayerNews);
                playerShip.PlayerPirateClanReal = !playerShip.PlayerPirateClanReal;
                playerShip.PlayerExperienceDominatorKills = ShiftInt32(
                    playerShip.PlayerExperienceDominatorKills);
                playerShip.PlayerExperiencePirateKills = ShiftInt32(
                    playerShip.PlayerExperiencePirateKills);
                playerShip.PlayerExperienceGoodShipKills = ShiftInt32(
                    playerShip.PlayerExperienceGoodShipKills);
                playerShip.PlayerExperienceTrade = ShiftInt32(playerShip.PlayerExperienceTrade);
                playerShip.PlayerCaptainOnBridge = ShiftByte(playerShip.PlayerCaptainOnBridge);
                playerShip.PlayerBridgeCurrentShipId = ships[0].ObjectId;
                playerShip.PlayerBridgeCurrentPlanetId = planets[0].ObjectId;
                playerShip.PlayerBridgeBackground = (playerShip.PlayerBridgeBackground ?? string.Empty) +
                    "SRHD Save Editor.Bridge.RoundTrip";
                playerShip.PlayerBridgeRuins.RuinsEnergy = ShiftInt32(
                    playerShip.PlayerBridgeRuins.RuinsEnergy);
                ShipHeaderRecord ruinsShip = ships[ruinsShipIndex];
                if (ruinsShip.RuinsEquipmentItems == null || ruinsShip.RuinsEquipmentItems.Count < 2 ||
                    ruinsShip.RuinsSaleSatellite == null)
                    throw new InvalidOperationException("writer self-test requires TRuins equipment and sale satellite");
                int editedRuinsEquipmentStart = ruinsShip.RuinsEquipmentItems[0].ItemStart;
                int editedRuinsSaleSatelliteStart = ruinsShip.RuinsSaleSatellite.ItemStart;
                ShipItemListEntry deletedRuinsEquipment =
                    ruinsShip.RuinsEquipmentItems[ruinsShip.RuinsEquipmentItems.Count - 1];
                ruinsShip.RuinsEquipmentItems.RemoveAt(ruinsShip.RuinsEquipmentItems.Count - 1);
                ruinsShip.RuinsEquipmentItemCount = checked((ushort)ruinsShip.RuinsEquipmentItems.Count);
                deletedShipItemStarts.Add(deletedRuinsEquipment.ItemStart);
                for (int good = 0; good < 8; good++)
                    for (int field = 0; field < 3; field++)
                        ruinsShip.RuinsShopGoods[good, field] = ShiftInt32(ruinsShip.RuinsShopGoods[good, field]);
                ruinsShip.RuinsEnergy = ShiftInt32(ruinsShip.RuinsEnergy);
                ruinsShip.RuinsFlyToStarId = ruinsShip.RuinsFlyToStarId == 0 ? stars[0].ObjectId : 0;
                ruinsShip.RuinsFlyDate = ShiftInt32(ruinsShip.RuinsFlyDate);
                ruinsShip.RuinsSponsor = !ruinsShip.RuinsSponsor;
                ruinsShip.RuinsSpecialShip = !ruinsShip.RuinsSpecialShip;
                ruinsShip.RuinsNoLanding = !ruinsShip.RuinsNoLanding;
                ruinsShip.RuinsNoShopUpdate = ShiftByte(ruinsShip.RuinsNoShopUpdate);

                ShipHeaderRecord collectionShip = ships[0];
                ShipSpecialBonusRecord addedBonus = new ShipSpecialBonusRecord();
                addedBonus.BonusType = collectionShip.SpecialBonuses.Count == 0
                    ? (byte)1 : ShiftByte(collectionShip.SpecialBonuses[0].BonusType);
                addedBonus.Value = collectionShip.SpecialBonuses.Count == 0
                    ? 123456 : ShiftInt32(collectionShip.SpecialBonuses[0].Value);
                collectionShip.SpecialBonuses.Add(addedBonus);
                ShipStatusEffectRecord addedEffect = new ShipStatusEffectRecord();
                addedEffect.EffectType = collectionShip.StatusEffects.Count == 0
                    ? (byte)1 : ShiftByte(collectionShip.StatusEffects[0].EffectType);
                addedEffect.Value = collectionShip.StatusEffects.Count == 0
                    ? 1.25F : ShiftCoordinate(collectionShip.StatusEffects[0].Value);
                addedEffect.LastSourceShipId = ships[0].ObjectId;
                collectionShip.StatusEffects.Add(addedEffect);
                CustomShipInfoRecord addedInfo = new CustomShipInfoRecord();
                addedInfo.Name = "SRHD Save Editor self-test";
                addedInfo.Description = "TCustomShipInfo writer round-trip";
                addedInfo.Data1 = 101; addedInfo.Data2 = -202; addedInfo.Data3 = 303;
                addedInfo.TextData1 = "Text 1"; addedInfo.TextData2 = "Текст 2";
                addedInfo.TextData3 = "Text 3";
                collectionShip.CustomShipInfos.Add(addedInfo);
                collectionShip.TakeItemReferenceIds.Add(source.GalaxyItems[
                    Math.Min(1, source.GalaxyItems.Count - 1)].ObjectId);
                collectionShip.RecentlyDroppedItemIds.Add(source.GalaxyItems[
                    Math.Min(2, source.GalaxyItems.Count - 1)].ObjectId);
                ShipHeaderRecord relationShip = null;
                foreach (ShipHeaderRecord candidate in ships)
                    if (candidate.RelationToRangers != null && candidate.RelationToRangers.Length != 0)
                    {
                        relationShip = candidate; break;
                    }
                if (relationShip == null)
                    throw new InvalidOperationException("writer self-test requires TShip RelationToRangers values");
                relationShip.RelationToRangers[0] =
                    (byte)((relationShip.RelationToRangers[0] + 1) % 101);
                collectionShip.GoodShipId = ships[Math.Min(1, ships.Count - 1)].ObjectId;
                collectionShip.BadShipId = ships[Math.Min(2, ships.Count - 1)].ObjectId;
                collectionShip.PartnerShipId = ships[Math.Min(3, ships.Count - 1)].ObjectId;
                collectionShip.PartnerGood = ShiftInt32(collectionShip.PartnerGood);
                collectionMutatedShipIds.Add(collectionShip.ObjectId);
                collectionMutatedShipIds.Add(relationShip.ObjectId);

                bool deletedEquipment = false, deletedArtefact = false, deletedDrop = false;
                foreach (ShipHeaderRecord ship in ships)
                {
                    if (!deletedEquipment && ship.EquipmentItems.Count > 0)
                    {
                        ShipItemListEntry removed = ship.EquipmentItems[ship.EquipmentItems.Count - 1];
                        ship.EquipmentItems.RemoveAt(ship.EquipmentItems.Count - 1);
                        ship.EquipmentItemCount = checked((ushort)ship.EquipmentItems.Count);
                        deletedShipItemStarts.Add(removed.ItemStart);
                        collectionMutatedShipIds.Add(ship.ObjectId); deletedEquipment = true;
                    }
                    if (!deletedArtefact && ship.ArtefactItems.Count > 0)
                    {
                        ShipItemListEntry removed = ship.ArtefactItems[ship.ArtefactItems.Count - 1];
                        ship.ArtefactItems.RemoveAt(ship.ArtefactItems.Count - 1);
                        deletedShipItemStarts.Add(removed.ItemStart);
                        collectionMutatedShipIds.Add(ship.ObjectId); deletedArtefact = true;
                    }
                    if (!deletedDrop && ship.DropListItems.Count > 0)
                    {
                        ShipItemListEntry removed = ship.DropListItems[ship.DropListItems.Count - 1];
                        ship.DropListItems.RemoveAt(ship.DropListItems.Count - 1);
                        deletedShipItemStarts.Add(removed.ItemStart);
                        collectionMutatedShipIds.Add(ship.ObjectId); deletedDrop = true;
                    }
                    if (deletedEquipment && deletedArtefact && deletedDrop) break;
                }
                List<ItemHeaderRecord> items = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in source.GalaxyItems) items.Add(item.Clone());
                ItemHeaderRecord editedShopItem = null;
                foreach (ItemHeaderRecord item in items)
                    if (item.Start == editedShopItemStart)
                    {
                        editedShopItem = item;
                        break;
                    }
                if (editedShopItem == null)
                    throw new InvalidOperationException(
                        "writer self-test did not resolve nested TPlanet EquipmentShop item");
                editedShopItem.Cost = editedShopItem.Cost == uint.MaxValue ?
                    editedShopItem.Cost - 1 : editedShopItem.Cost + 1;
                ItemHeaderRecord editedRuinsEquipmentItem = null, editedRuinsSaleSatellite = null;
                foreach (ItemHeaderRecord item in items)
                {
                    if (item.Start == editedRuinsEquipmentStart) editedRuinsEquipmentItem = item;
                    if (item.Start == editedRuinsSaleSatelliteStart) editedRuinsSaleSatellite = item;
                }
                if (editedRuinsEquipmentItem == null || editedRuinsSaleSatellite == null)
                    throw new InvalidOperationException("writer self-test did not resolve nested TRuins items");
                editedRuinsEquipmentItem.Cost = editedRuinsEquipmentItem.Cost == uint.MaxValue
                    ? editedRuinsEquipmentItem.Cost - 1 : editedRuinsEquipmentItem.Cost + 1;
                editedRuinsSaleSatellite.Cost = editedRuinsSaleSatellite.Cost == uint.MaxValue
                    ? editedRuinsSaleSatellite.Cost - 1 : editedRuinsSaleSatellite.Cost + 1;
                int nestedTranclucatorItemIndex = -1;
                for (int index = 0; index < items.Count; index++)
                    if (items[index].NestedTranclucator != null &&
                        !deletedShipItemStarts.Contains(items[index].Start))
                    { nestedTranclucatorItemIndex = index; break; }
                if (nestedTranclucatorItemIndex >= 0)
                {
                    ShipHeaderRecord nested = items[nestedTranclucatorItemIndex].NestedTranclucator;
                    nested.Name = (nested.Name ?? string.Empty) + " T";
                    nested.X = ShiftCoordinate(nested.X);
                    nested.Money = ShiftUInt32(nested.Money);
                    nested.GraphName = (nested.GraphName ?? string.Empty) + ".SrhdSaveEditorTest";
                    nested.TranclucatorDocking = !nested.TranclucatorDocking;
                    nested.TranclucatorSeekItems = !nested.TranclucatorSeekItems;
                    nested.TranclucatorAutoArrange = !nested.TranclucatorAutoArrange;
                    nested.TranclucatorArtSize = ShiftInt32(nested.TranclucatorArtSize);
                    nested.TranclucatorArtSystemName = string.IsNullOrEmpty(nested.TranclucatorArtSystemName)
                        ? "SrhdSaveEditorNestedTest" : nested.TranclucatorArtSystemName + ".SrhdSaveEditorTest";
                    for (int index = 0; index < nested.TranclucatorSeekPermits.Length; index++)
                        nested.TranclucatorSeekPermits[index] = !nested.TranclucatorSeekPermits[index];
                    for (int index = 0; index < nested.TranclucatorLandPermits.Length; index++)
                        nested.TranclucatorLandPermits[index] = !nested.TranclucatorLandPermits[index];
                    nested.TranclucatorLandStorage = !nested.TranclucatorLandStorage;
                }
                List<int> derivedItemIndices = new List<int>();
                HashSet<byte> derivedTypes = new HashSet<byte>();
                for (int index = 0; index < items.Count; index++)
                    if (!deletedShipItemStarts.Contains(items[index].Start) &&
                        items[index].HasDerivedTail && items[index].DerivedFields != null &&
                        items[index].DerivedFields.Count > 0 && derivedTypes.Add(items[index].Type))
                        derivedItemIndices.Add(index);
                int customWeaponAmmoItemIndex = FindCustomWeaponAmmoItem(items);
                if (customWeaponAmmoItemIndex >= 0 &&
                    !deletedShipItemStarts.Contains(items[customWeaponAmmoItemIndex].Start) &&
                    !derivedItemIndices.Contains(customWeaponAmmoItemIndex))
                    derivedItemIndices.Add(customWeaponAmmoItemIndex);
                int weaponTargetItemIndex = FindWeaponTargetItem(items, deletedShipItemStarts);
                if (weaponTargetItemIndex >= 0 && !derivedItemIndices.Contains(weaponTargetItemIndex))
                    derivedItemIndices.Add(weaponTargetItemIndex);
                int hullItemIndex = -1;
                for (int index = 0; index < items.Count; index++)
                    if (items[index].Type == 42 && items[index].HasDerivedTail &&
                        !deletedShipItemStarts.Contains(items[index].Start))
                    { hullItemIndex = index; break; }
                if (hullItemIndex < 0)
                    throw new InvalidOperationException(
                        "no THull found for interceptor structural writer self-test");
                ToggleHullInterceptors(items[hullItemIndex], ships[0].ObjectId);
                if (!derivedItemIndices.Contains(hullItemIndex))
                    derivedItemIndices.Add(hullItemIndex);
                foreach (int derivedIndex in derivedItemIndices)
                    foreach (ItemDerivedField field in items[derivedIndex].DerivedFields)
                    {
                        if (field.ControlName == "edWeaponTargetType" ||
                            field.ControlName == "cbWeaponTarget" ||
                            field.ControlName.StartsWith("$", StringComparison.Ordinal)) continue;
                        if (field.Kind == ItemDerivedField.Byte)
                            field.IntegerValue = field.IntegerValue == byte.MaxValue ? byte.MaxValue - 1 : field.IntegerValue + 1;
                        else if (field.Kind == ItemDerivedField.Boolean)
                            field.IntegerValue = field.IntegerValue == 0 ? 1 : 0;
                        else if (field.Kind == ItemDerivedField.UInt16)
                            field.IntegerValue = field.IntegerValue == ushort.MaxValue ? ushort.MaxValue - 1 : field.IntegerValue + 1;
                        else if (field.Kind == ItemDerivedField.Int32)
                            field.IntegerValue = ShiftInt32((int)field.IntegerValue);
                        else if (field.Kind == ItemDerivedField.UInt32)
                            field.IntegerValue = field.ControlName == "cbInterceptorsNextTarget"
                                ? ships[0].ObjectId
                                : field.ControlName.IndexOf("Planet", StringComparison.Ordinal) >= 0
                                ? (field.IntegerValue == 0 ? planets[0].ObjectId : 0)
                                : ShiftUInt32((uint)field.IntegerValue);
                        else if (field.Kind == ItemDerivedField.Float32)
                            field.FloatValue = ShiftCoordinate(field.FloatValue);
                        else if (field.Kind == ItemDerivedField.String)
                            field.StringValue = (field.StringValue ?? string.Empty) + " C";
                    }
                if (weaponTargetItemIndex >= 0)
                {
                    foreach (ItemDerivedField field in items[weaponTargetItemIndex].DerivedFields)
                    {
                        if (field.ControlName == "edWeaponTargetType") field.IntegerValue = 1;
                        if (field.ControlName == "cbWeaponTarget") field.IntegerValue = ships[0].ObjectId;
                    }
                }
                List<HoleRecord> holes = new List<HoleRecord>();
                foreach (HoleRecord hole in source.GalaxyHoles) holes.Add(hole.Clone());
                List<AsteroidRecord> asteroids = new List<AsteroidRecord>();
                foreach (AsteroidRecord asteroid in source.GalaxyAsteroids) asteroids.Add(asteroid.Clone());
                List<MissileRecord> missiles = new List<MissileRecord>();
                foreach (MissileRecord missile in source.GalaxyMissiles) missiles.Add(missile.Clone());
                List<CustomWeaponInfoRecord> customWeapons = new List<CustomWeaponInfoRecord>();
                foreach (CustomWeaponInfoRecord weapon in source.CustomWeaponInfos) customWeapons.Add(weapon.Clone());
                List<InterfaceOverrideRecord> interfaceOverrides = new List<InterfaceOverrideRecord>();
                foreach (InterfaceOverrideRecord record in source.GalaxySummary.InterfaceOverrides)
                    interfaceOverrides.Add(record.Clone());
                List<StoredItemRecord> storedItems = new List<StoredItemRecord>();
                foreach (StoredItemRecord record in source.StoredItems) storedItems.Add(record.Clone());
                int itemIndex = FindEditableItem(items, deletedShipItemStarts);
                int goodsItemIndex = FindGoodsItem(items, deletedShipItemStarts);
                int equipmentItemIndex = FindEquipmentItem(items, deletedShipItemStarts);
                items[itemIndex].Name = string.IsNullOrEmpty(items[itemIndex].Name)
                    ? "SRHD Save Editor test" : items[itemIndex].Name + " I";
                items[itemIndex].Cost = items[itemIndex].Cost == uint.MaxValue
                    ? items[itemIndex].Cost - 1 : items[itemIndex].Cost + 1;
                items[itemIndex].NoDrop = items[itemIndex].NoDrop == 0 ? (byte)1 : (byte)0;
                items[itemIndex].CustomFaction = string.IsNullOrEmpty(items[itemIndex].CustomFaction)
                    ? "SRHD Save Editor test" : items[itemIndex].CustomFaction + " I";
                items[itemIndex].Strength += 1.0F;
                items[itemIndex].Broken = items[itemIndex].Broken == 0 ? (byte)1 : (byte)0;
                items[itemIndex].Exploitable = items[itemIndex].Exploitable == 0 ? (byte)1 : (byte)0;
                items[goodsItemIndex].GoodsItemCount = items[goodsItemIndex].GoodsItemCount >= 10000
                    ? items[goodsItemIndex].GoodsItemCount - 1 : items[goodsItemIndex].GoodsItemCount + 1;
                items[goodsItemIndex].Weight = items[goodsItemIndex].GoodsItemCount;
                items[goodsItemIndex].GoodsItemNatural = !items[goodsItemIndex].GoodsItemNatural;
                ItemHeaderRecord equipmentItem = items[equipmentItemIndex];
                if (equipmentItem.Bonus == 0)
                {
                    equipmentItem.Bonus = 1;
                    equipmentItem.BonusReferenceId = 0xC45A11A1U;
                }
                else
                {
                    equipmentItem.Bonus = 0;
                    equipmentItem.BonusReferenceId = 0;
                }
                if (equipmentItem.Special == 0)
                {
                    equipmentItem.Special = 1;
                    equipmentItem.SpecialReferenceId = 0xC45A11A2U;
                }
                else
                {
                    equipmentItem.Special = 0;
                    equipmentItem.SpecialReferenceId = 0;
                }
                ItemExtraSpecialRecord extraSpecial = new ItemExtraSpecialRecord();
                extraSpecial.Special = 1;
                extraSpecial.ReferenceId = 0xC45A11A3U;
                extraSpecial.Count = 7;
                equipmentItem.ExtraSpecials.Add(extraSpecial);
                AchievementStatsRecord achievements = source.AchievementStats.Clone();
                achievements.AsteroidsDestroyed = achievements.AsteroidsDestroyed == int.MaxValue
                    ? achievements.AsteroidsDestroyed - 1 : achievements.AsteroidsDestroyed + 1;
                achievements.ScienceProgress = achievements.ScienceProgress == byte.MaxValue
                    ? (byte)(byte.MaxValue - 1) : (byte)(achievements.ScienceProgress + 1);
                if (holes.Count > 0)
                {
                    holes[0].GraphName += " H";
                    holes[0].FromX = ShiftCoordinate(holes[0].FromX);
                    holes[0].TurnCreate = holes[0].TurnCreate == int.MaxValue ? holes[0].TurnCreate - 1 : holes[0].TurnCreate + 1;
                }
                if (asteroids.Count > 0)
                {
                    asteroids[0].GraphName += " A";
                    asteroids[0].PositionX = ShiftCoordinate(asteroids[0].PositionX);
                    asteroids[0].Minerals = asteroids[0].Minerals == int.MaxValue
                        ? asteroids[0].Minerals - 1 : asteroids[0].Minerals + 1;
                    StarHeaderRecord relocationTarget = null;
                    foreach (StarHeaderRecord candidate in source.GalaxyStars)
                        if (candidate.ObjectId != asteroids[0].ParentStarId &&
                            candidate.AsteroidCountOffset >= 0)
                        {
                            relocationTarget = candidate;
                            break;
                        }
                    if (relocationTarget == null)
                        throw new InvalidOperationException(
                            "no exact target TStar found for TAsteroid relocation self-test");
                    asteroids[0].ParentStarId = relocationTarget.ObjectId;
                }
                if (missiles.Count == 0)
                    throw new InvalidOperationException("no TMissile record found for writer self-test");
                missiles[0].TechLevel = missiles[0].TechLevel == byte.MaxValue
                    ? (byte)(byte.MaxValue - 1) : (byte)(missiles[0].TechLevel + 1);
                missiles[0].DamageMin = missiles[0].DamageMin == int.MaxValue
                    ? missiles[0].DamageMin - 1 : missiles[0].DamageMin + 1;
                missiles[0].PositionX = ShiftCoordinate(missiles[0].PositionX);
                missiles[0].Live = missiles[0].Live == int.MaxValue
                    ? missiles[0].Live - 1 : missiles[0].Live + 1;
                missiles[0].LastDistanceMin = ShiftCoordinate(missiles[0].LastDistanceMin);
                if (missiles[0].Bonus == 0)
                {
                    missiles[0].Bonus = 1;
                    missiles[0].BonusReferenceId = missiles[0].WeaponId == 0
                        ? missiles[0].ObjectId : missiles[0].WeaponId;
                }
                else
                {
                    missiles[0].Bonus = 0;
                    missiles[0].BonusReferenceId = 0;
                }
                if (missiles[0].TargetType == 0)
                {
                    missiles[0].TargetType = 4;
                    missiles[0].TargetId = missiles[0].ObjectId;
                }
                else
                {
                    missiles[0].TargetType = 0;
                    missiles[0].TargetId = 0;
                }
                if (customWeapons.Count == 0)
                    throw new InvalidOperationException("no TCustomWeaponInfo record found for writer self-test");
                CustomWeaponInfoRecord customWeapon = customWeapons[0];
                string oldCustomWeaponName = customWeapon.SystemName;
                string renamedCustomWeaponName = oldCustomWeaponName + ".SrhdSaveEditorRename";
                int renamedCustomWeaponItems = 0, renamedCustomWeaponMissiles = 0;
                foreach (CustomWeaponInfoRecord candidate in customWeapons)
                    if (!object.ReferenceEquals(candidate, customWeapon) &&
                        string.Equals(candidate.SystemName, renamedCustomWeaponName,
                            StringComparison.Ordinal))
                        throw new InvalidOperationException(
                            "custom weapon rename self-test name already exists");
                customWeapon.SystemName = renamedCustomWeaponName;
                foreach (ItemHeaderRecord item in items)
                    if (item.Type == 68 && string.Equals(item.CustomWeaponName,
                        oldCustomWeaponName, StringComparison.Ordinal))
                    {
                        item.CustomWeaponName = renamedCustomWeaponName;
                        renamedCustomWeaponItems++;
                    }
                foreach (MissileRecord missile in missiles)
                    if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                        oldCustomWeaponName, StringComparison.Ordinal))
                    {
                        missile.CustomWeaponName = renamedCustomWeaponName;
                        renamedCustomWeaponMissiles++;
                    }
                if (renamedCustomWeaponItems + renamedCustomWeaponMissiles == 0)
                    throw new InvalidOperationException(
                        "no linked TCustomWeapon/TCustomMissile found for rename self-test");
                customWeapon.TechLevel = ShiftByte(customWeapon.TechLevel);
                customWeapon.TechRadius = customWeapon.TechRadius >= 19 ? (byte)18 : (byte)(customWeapon.TechRadius + 1);
                customWeapon.ModCost = ShiftCoordinate(customWeapon.ModCost);
                customWeapon.MinDamage = ShiftInt32(customWeapon.MinDamage);
                customWeapon.MaxDamage = ShiftInt32(customWeapon.MaxDamage);
                customWeapon.AverageSize = ShiftInt32(customWeapon.AverageSize);
                customWeapon.AverageRadius = ShiftInt32(customWeapon.AverageRadius);
                customWeapon.Speed = ShiftInt32(customWeapon.Speed);
                customWeapon.MissileRadius = ShiftInt32(customWeapon.MissileRadius);
                customWeapon.MissileMinSpeed = ShiftInt32(customWeapon.MissileMinSpeed);
                customWeapon.MissileMaxSpeed = ShiftInt32(customWeapon.MissileMaxSpeed);
                customWeapon.MissileChanceToBeHit = ShiftByte(customWeapon.MissileChanceToBeHit);
                customWeapon.DamageType ^= 1U;
                customWeapon.ShotType = (byte)((customWeapon.ShotType + 1) % 8);
                customWeapon.ShotCount = ShiftByte(customWeapon.ShotCount);
                customWeapon.AttackCount = ShiftByte(customWeapon.AttackCount);
                customWeapon.SecondaryDamageRadius = ShiftCoordinate(customWeapon.SecondaryDamageRadius);
                customWeapon.MiningFactor = ShiftCoordinate(customWeapon.MiningFactor);
                customWeapon.WeaponDamageSet[0] = ShiftCoordinate(customWeapon.WeaponDamageSet[0]);
                customWeapon.PrimarySE = string.IsNullOrEmpty(customWeapon.PrimarySE)
                    ? "SrhdSaveEditorPrimary" : customWeapon.PrimarySE + ".SRHD Save Editor";
                customWeapon.DefaultPalette = ShiftInt32(customWeapon.DefaultPalette);
                customWeapon.Availability = (byte)((customWeapon.Availability + 1) % 10);
                customWeapon.ABWeaponType = customWeapon.ABWeaponType < 50 || customWeapon.ABWeaponType >= 67
                    ? (byte)50 : (byte)(customWeapon.ABWeaponType + 1);
                if (interfaceOverrides.Count > 0)
                {
                    InterfaceOverrideRecord interfaceRecord = interfaceOverrides[0];
                    interfaceRecord.ModuleName = (interfaceRecord.ModuleName ?? string.Empty) + ".SRHD Save Editor";
                    if (interfaceRecord.Kind == InterfaceOverrideKind.State)
                        interfaceRecord.NewState = (byte)((interfaceRecord.NewState + 1) % 4);
                    else if (interfaceRecord.Kind == InterfaceOverrideKind.Text ||
                        interfaceRecord.Kind == InterfaceOverrideKind.Image)
                        interfaceRecord.NewValue = (interfaceRecord.NewValue ?? string.Empty) + " SRHD Save Editor";
                    else
                        interfaceRecord.NewX = ShiftInt32(interfaceRecord.NewX);
                    interfaceOverrides.RemoveAt(interfaceOverrides.Count - 1);
                }
                else
                {
                    InterfaceOverrideRecord interfaceRecord = new InterfaceOverrideRecord();
                    interfaceRecord.Kind = InterfaceOverrideKind.State;
                    interfaceRecord.ModuleName = "SrhdSaveEditorSelfTest";
                    interfaceRecord.GuiName = "SyntheticControl";
                    interfaceRecord.NewState = 1; interfaceRecord.OldState = 0;
                    interfaceOverrides.Add(interfaceRecord);
                }
                if (storedItems.Count > 0)
                    storedItems[0].ScriptTag += ".SRHD Save Editor";
                source.WriteCopy(args[3], changed, messages, galaxy, stars, planets, ships, items,
                    achievements, holes, asteroids, missiles, customWeapons, interfaceOverrides, storedItems,
                    galaxySummary, constellations);
                SavContainer patched = SavContainer.Load(args[3]);
                if (patched.Metadata.CameraX != changed.CameraX)
                    throw new InvalidOperationException("patched CameraX was not persisted");
                if (messages.Count > 0 && patched.PlayerMessages[0].FormattedText != messages[0].FormattedText)
                    throw new InvalidOperationException("patched player message was not persisted");
                if (patched.PlayerMessages.Count != messages.Count ||
                    patched.Metadata.PlayerMessageCount != (uint)messages.Count)
                    throw new InvalidOperationException("patched TMessagePlayer count was not persisted");
                if (patched.GalaxyPrefix.SaveCount != galaxy.SaveCount || patched.GalaxyPrefix.UsedMods != galaxy.UsedMods)
                    throw new InvalidOperationException("patched TGalaxy prefix was not persisted");
                if (!patched.GalaxyConstellations[0].ContentEquals(constellations[0]))
                    throw new InvalidOperationException("patched TConstellation visibility was not persisted");
                if (!patched.GalaxySummary.EditableContentEquals(galaxySummary))
                    throw new InvalidOperationException("patched TGALAXYFORM scalar fields were not persisted");
                if (patched.GalaxySummary.GalaxyEventCount != galaxySummary.GalaxyEvents.Count ||
                    !GalaxySummaryData.EqualRecords(patched.GalaxySummary.GalaxyEvents,
                        galaxySummary.GalaxyEvents))
                    throw new InvalidOperationException("patched TGalaxyEvent list was not persisted");
                if (!patched.GalaxyStars[0].ContentEquals(stars[0]))
                    throw new InvalidOperationException("patched complete TStar stream was not persisted");
                if (dropStarIndex >= 0 &&
                    !patched.GalaxyStars[dropStarIndex].DropItemsContentEquals(stars[dropStarIndex]))
                    throw new InvalidOperationException("patched TStar.DropItems wrapper was not persisted");
                PlanetHeaderRecord patchedPlanet = FindPlanet(patched, planets[0].ObjectId);
                ShipHeaderRecord patchedShip = FindShip(patched, ships[0].ObjectId);
                ShipHeaderRecord patchedNormalShip = FindShip(patched, ships[normalShipIndex].ObjectId);
                ItemHeaderRecord patchedItem = FindItem(patched, items[itemIndex].Type, items[itemIndex].ObjectId);
                ItemHeaderRecord patchedGoodsItem = FindItem(patched, items[goodsItemIndex].Type,
                    items[goodsItemIndex].ObjectId);
                HoleRecord patchedHole = holes.Count == 0 ? null : FindHole(patched, holes[0].ObjectId);
                AsteroidRecord patchedAsteroid = asteroids.Count == 0 ? null : FindAsteroid(patched, asteroids[0].ObjectId);
                MissileRecord patchedMissile = FindMissile(patched, missiles[0].ObjectId);
                if (!patchedPlanet.ContentEquals(planets[0]))
                    throw new InvalidOperationException("patched TPlanet fixed prefix was not persisted");
                if (!FindPlanet(patched, editedSputnikPlanet.ObjectId).ContentEquals(editedSputnikPlanet))
                    throw new InvalidOperationException("edited TSputnik was not persisted");
                if (!FindPlanet(patched, deletedSputnikPlanet.ObjectId).ContentEquals(deletedSputnikPlanet))
                    throw new InvalidOperationException("deleted TSputnik was not persisted");
                if (!FindPlanet(patched, editedGoneItemPlanet.ObjectId).ContentEquals(editedGoneItemPlanet))
                    throw new InvalidOperationException("edited TPlanet GoneItem was not persisted");
                if (!FindPlanet(patched, deletedGoneItemPlanet.ObjectId).ContentEquals(deletedGoneItemPlanet))
                    throw new InvalidOperationException("deleted TPlanet GoneItem was not persisted");
                if (!FindPlanet(patched, editedShopPlanet.ObjectId).ContentEquals(editedShopPlanet))
                    throw new InvalidOperationException("edited TPlanet EquipmentShop was not persisted");
                if (!FindPlanet(patched, deletedShopPlanet.ObjectId).ContentEquals(deletedShopPlanet))
                    throw new InvalidOperationException("deleted TPlanet EquipmentShop item was not persisted");
                ItemHeaderRecord patchedShopItem = FindItem(patched, editedShopItem.Type,
                    editedShopItem.ObjectId);
                if (patchedShopItem.Cost != editedShopItem.Cost)
                    throw new InvalidOperationException(
                        "nested TPlanet EquipmentShop item edit was not persisted");
                if (!patchedShip.ContentEquals(ships[0]))
                    throw new InvalidOperationException("patched TShip common scalar stream was not persisted");
                if (!FindShip(patched, editedPlanetWarriorShip.ObjectId).ContentEquals(
                    editedPlanetWarriorShip))
                    throw new InvalidOperationException("nested TPlanet Warrior TShip edit was not persisted");
                if (!patchedNormalShip.ContentEquals(ships[normalShipIndex]))
                    throw new InvalidOperationException("patched TNormalShip derived scalar stream was not persisted");
                foreach (int simpleIndex in simpleShipIndices)
                    if (!FindShip(patched, ships[simpleIndex].ObjectId).ContentEquals(ships[simpleIndex]))
                        throw new InvalidOperationException("patched simple TShip derived scalar stream was not persisted");
                if (!FindShip(patched, ships[rangerShipIndex].ObjectId).ContentEquals(
                    ships[rangerShipIndex]))
                    throw new InvalidOperationException("patched TRanger scalar stream was not persisted");
                if (!FindShip(patched, ships[playerShipIndex].ObjectId).ContentEquals(ships[playerShipIndex]))
                    throw new InvalidOperationException("patched TPlayer fixed prefix was not persisted");
                if (!FindShip(patched, ships[ruinsShipIndex].ObjectId).ContentEquals(ships[ruinsShipIndex]))
                    throw new InvalidOperationException("patched TRuins scalar stream was not persisted");
                if (FindItem(patched, editedRuinsEquipmentItem.Type,
                    editedRuinsEquipmentItem.ObjectId).Cost != editedRuinsEquipmentItem.Cost)
                    throw new InvalidOperationException(
                        "nested TRuins equipment item edit was not persisted");
                if (FindItem(patched, editedRuinsSaleSatellite.Type,
                    editedRuinsSaleSatellite.ObjectId).Cost != editedRuinsSaleSatellite.Cost)
                    throw new InvalidOperationException(
                        "nested TRuins sale satellite edit was not persisted");
                foreach (ShipHeaderRecord expectedShip in ships)
                    if (collectionMutatedShipIds.Contains(expectedShip.ObjectId) &&
                        !FindShip(patched, expectedShip.ObjectId).ContentEquals(expectedShip))
                        throw new InvalidOperationException("patched TShip nested collections were not persisted: " +
                            expectedShip.ObjectId);
                if (patchedItem.Name != items[itemIndex].Name || patchedItem.Cost != items[itemIndex].Cost ||
                    patchedItem.NoDrop != items[itemIndex].NoDrop ||
                    patchedItem.CustomFaction != items[itemIndex].CustomFaction ||
                    patchedItem.Strength != items[itemIndex].Strength ||
                    patchedItem.Broken != items[itemIndex].Broken ||
                    patchedItem.Exploitable != items[itemIndex].Exploitable)
                    throw new InvalidOperationException("patched TItem header was not persisted");
                if (!patchedGoodsItem.ContentEquals(items[goodsItemIndex]))
                    throw new InvalidOperationException("patched TGoodsItem tail was not persisted");
                if (!FindItem(patched, equipmentItem.Type, equipmentItem.ObjectId)
                    .ContentEquals(equipmentItem))
                    throw new InvalidOperationException(
                        "patched TEquipment bonus/special/extra-special stream was not persisted");
                foreach (int derivedIndex in derivedItemIndices)
                    if (!FindItem(patched, items[derivedIndex].Type, items[derivedIndex].ObjectId)
                        .ContentEquals(items[derivedIndex]))
                        throw new InvalidOperationException("patched derived TItem tail was not persisted: type " +
                            items[derivedIndex].Type);
                if (nestedTranclucatorItemIndex >= 0 &&
                    !FindItem(patched, items[nestedTranclucatorItemIndex].Type,
                        items[nestedTranclucatorItemIndex].ObjectId).ContentEquals(
                            items[nestedTranclucatorItemIndex]))
                    throw new InvalidOperationException("patched TArtefactTranclucator nested ship was not persisted");
                if (patchedHole != null && (patchedHole.GraphName != holes[0].GraphName ||
                    patchedHole.FromX != holes[0].FromX || patchedHole.TurnCreate != holes[0].TurnCreate))
                    throw new InvalidOperationException("patched THole record was not persisted");
                if (patchedAsteroid != null && (patchedAsteroid.GraphName != asteroids[0].GraphName ||
                    patchedAsteroid.PositionX != asteroids[0].PositionX ||
                    patchedAsteroid.Minerals != asteroids[0].Minerals ||
                    patchedAsteroid.ParentStarId != asteroids[0].ParentStarId))
                    throw new InvalidOperationException("patched TAsteroid record was not persisted");
                if (patchedMissile.TechLevel != missiles[0].TechLevel ||
                    patchedMissile.DamageMin != missiles[0].DamageMin ||
                    patchedMissile.PositionX != missiles[0].PositionX ||
                    patchedMissile.Live != missiles[0].Live ||
                    patchedMissile.LastDistanceMin != missiles[0].LastDistanceMin ||
                    patchedMissile.Bonus != missiles[0].Bonus ||
                    patchedMissile.BonusReferenceId != missiles[0].BonusReferenceId ||
                    patchedMissile.TargetType != missiles[0].TargetType ||
                    patchedMissile.TargetId != missiles[0].TargetId ||
                    patchedMissile.CustomWeaponName != missiles[0].CustomWeaponName)
                    throw new InvalidOperationException("patched TMissile record was not persisted");
                if (patched.CustomWeaponInfos.Count != customWeapons.Count ||
                    !patched.CustomWeaponInfos[0].ContentEquals(customWeapons[0]))
                    throw new InvalidOperationException("patched TCustomWeaponInfo record was not persisted");
                int persistedRenamedItems = 0, persistedRenamedMissiles = 0;
                foreach (ItemHeaderRecord item in patched.GalaxyItems)
                    if (item.Type == 68 && string.Equals(item.CustomWeaponName,
                        renamedCustomWeaponName, StringComparison.Ordinal))
                        persistedRenamedItems++;
                foreach (MissileRecord missile in patched.GalaxyMissiles)
                    if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                        renamedCustomWeaponName, StringComparison.Ordinal))
                        persistedRenamedMissiles++;
                if (persistedRenamedItems != renamedCustomWeaponItems ||
                    persistedRenamedMissiles != renamedCustomWeaponMissiles)
                    throw new InvalidOperationException(
                        "TCustomWeaponInfo system-name cascade was not persisted");
                if (patched.GalaxySummary.InterfaceOverrides.Count != interfaceOverrides.Count)
                    throw new InvalidOperationException("patched interface override count changed");
                for (int interfaceIndex = 0; interfaceIndex < interfaceOverrides.Count; interfaceIndex++)
                    if (!patched.GalaxySummary.InterfaceOverrides[interfaceIndex].ContentEquals(
                        interfaceOverrides[interfaceIndex]))
                        throw new InvalidOperationException("patched interface override was not persisted");
                if (patched.StoredItems.Count != storedItems.Count)
                    throw new InvalidOperationException("patched TStoredItem count changed");
                for (int storedIndex = 0; storedIndex < storedItems.Count; storedIndex++)
                    if (!patched.StoredItems[storedIndex].ContentEquals(storedItems[storedIndex]))
                        throw new InvalidOperationException("patched TStoredItem was not persisted");
                if (patched.AchievementStats.AsteroidsDestroyed != achievements.AsteroidsDestroyed ||
                    patched.AchievementStats.ScienceProgress != achievements.ScienceProgress)
                    throw new InvalidOperationException("patched TAchievementStats was not persisted");
                if (patched.AchievementStats.Received.Count != source.AchievementStats.Received.Count)
                    throw new InvalidOperationException("received achievement list changed");
                for (int achievementIndex = 0; achievementIndex < source.AchievementStats.Received.Count; achievementIndex++)
                    if (patched.AchievementStats.Received[achievementIndex] != source.AchievementStats.Received[achievementIndex])
                        throw new InvalidOperationException("received achievement key changed");
                AssertPlanetMiddleSpansPreserved(source, patched, ships, items,
                    deletedShipItemStarts);
                AssertOpaqueTailPrefix(source.MainPayload, source.GalaxyShips[0].FixedPrefixEnd,
                    patched.MainPayload, patchedShip.FixedPrefixEnd, "TShip");
                int sourceItemOpaque = source.GalaxyItems[itemIndex].HasDerivedTail
                    ? source.GalaxyItems[itemIndex].DerivedTailEnd : source.GalaxyItems[itemIndex].SharedPrefixEnd;
                int patchedItemOpaque = patchedItem.HasDerivedTail
                    ? patchedItem.DerivedTailEnd : patchedItem.SharedPrefixEnd;
                AssertOpaqueTailPrefix(source.MainPayload, sourceItemOpaque,
                    patched.MainPayload, patchedItemOpaque, "TItem");

                SavContainer storageSource = null;
                ShipHeaderRecord storageSourcePlayer = null;
                foreach (SavContainer candidate in loaded)
                    foreach (ShipHeaderRecord ship in candidate.GalaxyShips)
                        if (ship.IsPlayer && ship.PlayerStorageItems.Count >= 2)
                        {
                            storageSource = candidate;
                            storageSourcePlayer = ship;
                            break;
                        }
                if (storageSource == null || storageSourcePlayer == null)
                    throw new InvalidOperationException(
                        "storage writer self-test requires two TPlayer StorageItems");
                List<ShipHeaderRecord> storageShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord ship in storageSource.GalaxyShips)
                    storageShips.Add(ship.Clone());
                ShipHeaderRecord storagePlayer = FindShip(storageShips,
                    storageSourcePlayer.ObjectId);
                PlayerStorageItemRecord editedStorage = storagePlayer.PlayerStorageItems[0];
                editedStorage.Slot = ShiftInt32(editedStorage.Slot);
                if (editedStorage.IsStation)
                {
                    editedStorage.IsStation = false;
                    editedStorage.PlaceObjectId = storageSource.GalaxyPlanets[0].ObjectId;
                }
                else
                {
                    foreach (ShipHeaderRecord station in storageShips)
                        if (station.IsStation)
                        {
                            editedStorage.IsStation = true;
                            editedStorage.PlaceObjectId = station.ObjectId;
                            break;
                        }
                }
                PlayerStorageItemRecord removedStorage = storagePlayer.PlayerStorageItems[
                    storagePlayer.PlayerStorageItems.Count - 1];
                storagePlayer.PlayerStorageItems.RemoveAt(
                    storagePlayer.PlayerStorageItems.Count - 1);
                storagePlayer.PlayerObjectStateCount = storagePlayer.PlayerStorageItems.Count;
                List<ItemHeaderRecord> storageItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord item in storageSource.GalaxyItems)
                    storageItems.Add(item.Clone());
                File.Delete(args[2]);
                storageSource.WriteCopy(args[2], storageSource.Metadata.Clone(), null, null,
                    null, null, storageShips, storageItems);
                SavContainer storagePatched = SavContainer.Load(args[2]);
                ShipHeaderRecord storagePatchedPlayer = FindShip(storagePatched,
                    storageSourcePlayer.ObjectId);
                if (!storagePatchedPlayer.ContentEquals(storagePlayer))
                    throw new InvalidOperationException(
                        "patched TPlayer StorageItems were not persisted");
                foreach (PlayerStorageItemRecord record in storagePatchedPlayer.PlayerStorageItems)
                    if (record.ItemStart == removedStorage.ItemStart)
                        throw new InvalidOperationException(
                            "deleted TPlayer StorageItem was still present");

                SavContainer storedSource = null;
                foreach (SavContainer candidate in loaded)
                    if (candidate.HasExactStoredItemList && candidate.StoredItems.Count >= 2)
                    { storedSource = candidate; break; }
                if (storedSource != null)
                {
                    List<StoredItemRecord> editedStoredItems = new List<StoredItemRecord>();
                    foreach (StoredItemRecord record in storedSource.StoredItems)
                        editedStoredItems.Add(record.Clone());
                    editedStoredItems[0].ScriptTag += ".SRHD Save Editor";
                    StoredItemRecord removedStoredItem = editedStoredItems[editedStoredItems.Count - 1];
                    editedStoredItems.RemoveAt(editedStoredItems.Count - 1);
                    File.Delete(args[2]);
                    storedSource.WriteCopy(args[2], storedSource.Metadata.Clone(), null, null,
                        null, null, null, null, null, null, null, null, null, null,
                        editedStoredItems);
                    SavContainer storedPatched = SavContainer.Load(args[2]);
                    if (!storedPatched.HasExactStoredItemList ||
                        storedPatched.StoredItems.Count != editedStoredItems.Count)
                        throw new InvalidOperationException("patched TStoredItem count was not persisted");
                    for (int index = 0; index < editedStoredItems.Count; index++)
                        if (!storedPatched.StoredItems[index].ContentEquals(editedStoredItems[index]))
                            throw new InvalidOperationException("patched TStoredItem record was not persisted");
                    foreach (StoredItemRecord record in storedPatched.StoredItems)
                        if (record.ItemStart == removedStoredItem.ItemStart)
                            throw new InvalidOperationException("deleted TStoredItem was still present");
                }

                SavContainer deleteSource = null;
                AsteroidRecord deleteAsteroid = null;
                MissileRecord deleteMissile = null;
                foreach (SavContainer candidate in loaded)
                {
                    deleteAsteroid = FindUnreferencedAsteroid(candidate);
                    deleteMissile = FindUnreferencedMissile(candidate);
                    if (deleteAsteroid != null && deleteMissile != null)
                    { deleteSource = candidate; break; }
                }
                if (deleteSource == null)
                    throw new InvalidOperationException(
                        "no unreferenced TAsteroid/TMissile deletion fixture found");
                List<HoleRecord> deletedHoles = new List<HoleRecord>();
                foreach (HoleRecord value in deleteSource.GalaxyHoles) deletedHoles.Add(value.Clone());
                HoleRecord deleteHole = null;
                if (deletedHoles.Count > 0)
                {
                    deleteHole = deletedHoles[deletedHoles.Count - 1];
                    deletedHoles.RemoveAt(deletedHoles.Count - 1);
                }
                List<AsteroidRecord> deletedAsteroids = new List<AsteroidRecord>();
                foreach (AsteroidRecord value in deleteSource.GalaxyAsteroids) deletedAsteroids.Add(value.Clone());
                deletedAsteroids.RemoveAll(delegate(AsteroidRecord value) { return value.Start == deleteAsteroid.Start; });
                List<MissileRecord> deletedMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in deleteSource.GalaxyMissiles) deletedMissiles.Add(value.Clone());
                deletedMissiles.RemoveAll(delegate(MissileRecord value) { return value.Start == deleteMissile.Start; });
                File.Delete(args[2]);
                deleteSource.WriteCopy(args[2], deleteSource.Metadata.Clone(), null, null,
                    null, null, null, null, null, deletedHoles, deletedAsteroids, deletedMissiles);
                SavContainer deletePatched = SavContainer.Load(args[2]);
                if (deleteHole != null &&
                    (deletePatched.GalaxyHoles.Count != deleteSource.GalaxyHoles.Count - 1 ||
                    FindHoleOrNull(deletePatched, deleteHole.ObjectId) != null))
                    throw new InvalidOperationException("structurally deleted THole remained in TGalaxy");
                if (deletePatched.GalaxyAsteroids.Count != deleteSource.GalaxyAsteroids.Count - 1 ||
                    FindAsteroidOrNull(deletePatched, deleteAsteroid.ObjectId) != null)
                    throw new InvalidOperationException("structurally deleted TAsteroid remained in TStar");
                if (deletePatched.GalaxyMissiles.Count != deleteSource.GalaxyMissiles.Count - 1 ||
                    FindMissileOrNull(deletePatched, deleteMissile.ObjectId) != null)
                    throw new InvalidOperationException("structurally deleted TMissile remained in TStar");

                SavContainer itemDeleteSource = null;
                ItemHeaderRecord deleteSpaceItem = null;
                foreach (SavContainer candidate in loaded)
                {
                    if (candidate.GalaxyMissiles.Count == 0) continue;
                    foreach (StarHeaderRecord star in candidate.GalaxyStars)
                    {
                        foreach (ShipItemListEntry entry in star.SpaceItems)
                        {
                            if (CountItemOwners(candidate, entry.ItemStart) != 1) continue;
                            foreach (ItemHeaderRecord item in candidate.GalaxyItems)
                                if (item.Start == entry.ItemStart)
                                {
                                    itemDeleteSource = candidate;
                                    deleteSpaceItem = item;
                                    break;
                                }
                            if (deleteSpaceItem != null) break;
                        }
                        if (deleteSpaceItem != null) break;
                    }
                    if (deleteSpaceItem != null) break;
                }
                if (itemDeleteSource == null)
                    throw new InvalidOperationException(
                        "no singly-owned TStar.ItemsInSpace deletion fixture found");

                List<StarHeaderRecord> itemDeleteStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in itemDeleteSource.GalaxyStars)
                    itemDeleteStars.Add(value.Clone());
                List<ShipHeaderRecord> itemDeleteShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in itemDeleteSource.GalaxyShips)
                    itemDeleteShips.Add(value.Clone());
                List<ItemHeaderRecord> itemDeleteItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord value in itemDeleteSource.GalaxyItems)
                    itemDeleteItems.Add(value.Clone());
                List<MissileRecord> itemDeleteMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in itemDeleteSource.GalaxyMissiles)
                    itemDeleteMissiles.Add(value.Clone());
                HashSet<int> itemDeleteStarts = new HashSet<int>();
                itemDeleteStarts.Add(deleteSpaceItem.Start);
                int itemDeleteCount = itemDeleteSource.DeleteGalaxyItemsCascade(itemDeleteStarts,
                    itemDeleteStars, itemDeleteShips, itemDeleteItems, itemDeleteMissiles);
                if (itemDeleteCount != 1)
                    throw new InvalidOperationException("TItem cascade returned an incorrect count");
                File.Delete(args[2]);
                itemDeleteSource.WriteCopy(args[2], itemDeleteSource.Metadata.Clone(), null, null,
                    itemDeleteStars, null, itemDeleteShips, itemDeleteItems, null, null, null,
                    itemDeleteMissiles);
                SavContainer itemDeletePatched = SavContainer.Load(args[2]);
                if (itemDeletePatched.GalaxyItems.Count != itemDeleteSource.GalaxyItems.Count - 1)
                    throw new InvalidOperationException("structurally deleted TItem count was not persisted");
                foreach (ItemHeaderRecord value in itemDeletePatched.GalaxyItems)
                    if (value.ObjectId == deleteSpaceItem.ObjectId)
                        throw new InvalidOperationException("deleted TStar.ItemsInSpace TItem remained in SAV");
                if (HasItemReference(itemDeletePatched, deleteSpaceItem.ObjectId))
                    throw new InvalidOperationException("deleted TItem still has a parsed reference");

                List<StarHeaderRecord> unsafeItemStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in itemDeleteSource.GalaxyStars)
                    unsafeItemStars.Add(value.Clone());
                foreach (StarHeaderRecord star in unsafeItemStars)
                    star.SpaceItems.RemoveAll(delegate(ShipItemListEntry value)
                        { return value.ItemStart == deleteSpaceItem.Start; });
                List<MissileRecord> unsafeItemMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in itemDeleteSource.GalaxyMissiles)
                    unsafeItemMissiles.Add(value.Clone());
                unsafeItemMissiles[0].TargetType = 2;
                unsafeItemMissiles[0].TargetId = deleteSpaceItem.ObjectId;
                File.Delete(args[2]);
                bool unsafeItemDeleteRejected = false;
                try
                {
                    itemDeleteSource.WriteCopy(args[2], itemDeleteSource.Metadata.Clone(), null, null,
                        unsafeItemStars, null, null, null, null, null, null, unsafeItemMissiles);
                }
                catch (InvalidOperationException)
                {
                    unsafeItemDeleteRejected = true;
                }
                if (!unsafeItemDeleteRejected || File.Exists(args[2]))
                    throw new InvalidOperationException(
                        "unsafe TStar.ItemsInSpace deletion was not rejected before writing");

                SavContainer shipDeleteSource = null;
                ShipHeaderRecord deleteShip = null;
                foreach (SavContainer candidate in loaded)
                {
                    foreach (StarHeaderRecord star in candidate.GalaxyStars)
                    {
                        foreach (StarShipRecord owner in star.SpaceShips)
                        {
                            if (owner.OpaqueTail) continue;
                            ShipHeaderRecord ship = FindShipByStartOrNull(candidate, owner.ShipStart);
                            if (ship == null || ship.IsPlayer || ship.Type == 1 ||
                                ship.ObjectId == candidate.GalaxySummary.BlazerObjectId ||
                                ship.ObjectId == candidate.GalaxySummary.KellerObjectId ||
                                ship.ObjectId == candidate.GalaxySummary.TerronObjectId) continue;
                            shipDeleteSource = candidate;
                            deleteShip = ship;
                            break;
                        }
                        if (deleteShip != null) break;
                    }
                    if (deleteShip != null) break;
                }
                if (shipDeleteSource == null)
                    throw new InvalidOperationException("no deletable TStar.Ships fixture found");
                List<StarHeaderRecord> shipDeleteStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in shipDeleteSource.GalaxyStars)
                    shipDeleteStars.Add(value.Clone());
                List<PlanetHeaderRecord> shipDeletePlanets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord value in shipDeleteSource.GalaxyPlanets)
                    shipDeletePlanets.Add(value.Clone());
                List<ShipHeaderRecord> shipDeleteShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in shipDeleteSource.GalaxyShips)
                    shipDeleteShips.Add(value.Clone());
                List<ItemHeaderRecord> shipDeleteItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord value in shipDeleteSource.GalaxyItems)
                    shipDeleteItems.Add(value.Clone());
                List<MissileRecord> shipDeleteMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in shipDeleteSource.GalaxyMissiles)
                    shipDeleteMissiles.Add(value.Clone());
                GalaxySummaryData shipDeleteSummary = shipDeleteSource.GalaxySummary.Clone();
                HashSet<int> shipDeleteStarts = new HashSet<int>();
                shipDeleteStarts.Add(deleteShip.Start);
                int shipDeleteCount = shipDeleteSource.DeleteGalaxyShipsCascade(shipDeleteStarts,
                    shipDeleteStars, shipDeletePlanets, shipDeleteShips, shipDeleteItems,
                    shipDeleteMissiles, shipDeleteSummary);
                if (shipDeleteCount != 1)
                    throw new InvalidOperationException("TShip cascade returned an incorrect count");
                File.Delete(args[2]);
                shipDeleteSource.WriteCopy(args[2], shipDeleteSource.Metadata.Clone(), null, null,
                    shipDeleteStars, shipDeletePlanets, shipDeleteShips, shipDeleteItems, null,
                    null, null, shipDeleteMissiles, null, null, null, shipDeleteSummary, null);
                SavContainer shipDeletePatched = SavContainer.Load(args[2]);
                if (FindShipByIdOrNull(shipDeletePatched, deleteShip.ObjectId) != null)
                    throw new InvalidOperationException("structurally deleted TShip remained in SAV");
                if (HasShipReference(shipDeletePatched, deleteShip.ObjectId))
                    throw new InvalidOperationException("deleted TShip still has a parsed reference");

                SavContainer warriorDeleteSource = null;
                ShipHeaderRecord deleteWarrior = null;
                foreach (SavContainer candidate in loaded)
                {
                    foreach (PlanetHeaderRecord planet in candidate.GalaxyPlanets)
                    {
                        foreach (PlanetWarriorRecord owner in planet.Warriors)
                        {
                            ShipHeaderRecord ship = FindShipByStartOrNull(candidate, owner.ShipStart);
                            if (ship == null || ship.IsPlayer || ship.Type == 1 ||
                                ship.ObjectId == candidate.GalaxySummary.BlazerObjectId ||
                                ship.ObjectId == candidate.GalaxySummary.KellerObjectId ||
                                ship.ObjectId == candidate.GalaxySummary.TerronObjectId) continue;
                            warriorDeleteSource = candidate; deleteWarrior = ship; break;
                        }
                        if (deleteWarrior != null) break;
                    }
                    if (deleteWarrior != null) break;
                }
                if (warriorDeleteSource == null)
                    throw new InvalidOperationException("no deletable TPlanet.Warriors fixture found");
                List<StarHeaderRecord> warriorStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in warriorDeleteSource.GalaxyStars)
                    warriorStars.Add(value.Clone());
                List<PlanetHeaderRecord> warriorPlanets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord value in warriorDeleteSource.GalaxyPlanets)
                    warriorPlanets.Add(value.Clone());
                List<ShipHeaderRecord> warriorShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in warriorDeleteSource.GalaxyShips)
                    warriorShips.Add(value.Clone());
                List<ItemHeaderRecord> warriorItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord value in warriorDeleteSource.GalaxyItems)
                    warriorItems.Add(value.Clone());
                List<MissileRecord> warriorMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in warriorDeleteSource.GalaxyMissiles)
                    warriorMissiles.Add(value.Clone());
                GalaxySummaryData warriorSummary = warriorDeleteSource.GalaxySummary.Clone();
                HashSet<int> warriorStarts = new HashSet<int>();
                warriorStarts.Add(deleteWarrior.Start);
                warriorDeleteSource.DeleteGalaxyShipsCascade(warriorStarts, warriorStars,
                    warriorPlanets, warriorShips, warriorItems, warriorMissiles, warriorSummary);
                File.Delete(args[2]);
                warriorDeleteSource.WriteCopy(args[2], warriorDeleteSource.Metadata.Clone(), null, null,
                    warriorStars, warriorPlanets, warriorShips, warriorItems, null, null, null,
                    warriorMissiles, null, null, null, warriorSummary, null);
                SavContainer warriorPatched = SavContainer.Load(args[2]);
                if (FindShipByIdOrNull(warriorPatched, deleteWarrior.ObjectId) != null ||
                    HasShipReference(warriorPatched, deleteWarrior.ObjectId))
                    throw new InvalidOperationException(
                        "deleted TPlanet.Warriors TShip or its reference remained in SAV");

                SavContainer rangerDeleteSource = null;
                ShipHeaderRecord deleteRanger = null;
                foreach (SavContainer candidate in loaded)
                {
                    bool exactVanillaShips = true;
                    foreach (StarHeaderRecord star in candidate.GalaxyStars)
                    {
                        if (!star.HasExactSpaceShipList) { exactVanillaShips = false; break; }
                        foreach (StarShipRecord owner in star.SpaceShips)
                            if (owner.OpaqueTail) { exactVanillaShips = false; break; }
                        if (!exactVanillaShips) break;
                    }
                    if (!exactVanillaShips) continue;
                    foreach (StarHeaderRecord star in candidate.GalaxyStars)
                    {
                        foreach (StarShipRecord owner in star.SpaceShips)
                        {
                            ShipHeaderRecord ship = FindShipByStartOrNull(candidate, owner.ShipStart);
                            if (ship == null || ship.IsPlayer || ship.Type != 1) continue;
                            deleteRanger = ship; rangerDeleteSource = candidate; break;
                        }
                        if (deleteRanger != null) break;
                    }
                    if (deleteRanger != null) break;
                }
                if (rangerDeleteSource == null)
                    throw new InvalidOperationException("no exact vanilla TRanger deletion fixture found");
                List<StarHeaderRecord> rangerStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in rangerDeleteSource.GalaxyStars)
                    rangerStars.Add(value.Clone());
                List<PlanetHeaderRecord> rangerPlanets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord value in rangerDeleteSource.GalaxyPlanets)
                    rangerPlanets.Add(value.Clone());
                List<ShipHeaderRecord> rangerShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in rangerDeleteSource.GalaxyShips)
                    rangerShips.Add(value.Clone());
                List<ItemHeaderRecord> rangerItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord value in rangerDeleteSource.GalaxyItems)
                    rangerItems.Add(value.Clone());
                List<MissileRecord> rangerMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in rangerDeleteSource.GalaxyMissiles)
                    rangerMissiles.Add(value.Clone());
                GalaxySummaryData rangerSummary = rangerDeleteSource.GalaxySummary.Clone();
                int originalRangerCount = rangerSummary.RangerCount;
                HashSet<int> rangerStarts = new HashSet<int>(); rangerStarts.Add(deleteRanger.Start);
                rangerDeleteSource.DeleteGalaxyShipsCascade(rangerStarts, rangerStars,
                    rangerPlanets, rangerShips, rangerItems, rangerMissiles, rangerSummary);
                File.Delete(args[2]);
                rangerDeleteSource.WriteCopy(args[2], rangerDeleteSource.Metadata.Clone(), null, null,
                    rangerStars, rangerPlanets, rangerShips, rangerItems, null, null, null,
                    rangerMissiles, null, null, null, rangerSummary, null);
                SavContainer rangerPatched = SavContainer.Load(args[2]);
                if (rangerPatched.GalaxySummary.RangerCount != originalRangerCount - 1 ||
                    FindShipByIdOrNull(rangerPatched, deleteRanger.ObjectId) != null ||
                    HasShipReference(rangerPatched, deleteRanger.ObjectId))
                    throw new InvalidOperationException(
                        "deleted TRanger or its TGalaxy index remained in SAV");
                foreach (PlanetHeaderRecord planet in rangerPatched.GalaxyPlanets)
                    if (planet.RelationToRangers.Length != originalRangerCount - 1)
                        throw new InvalidOperationException(
                            "TRanger deletion did not shrink TPlanet.RelationToRangers");

                List<StarHeaderRecord> unsafeShipStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in shipDeleteSource.GalaxyStars)
                    unsafeShipStars.Add(value.Clone());
                foreach (StarHeaderRecord star in unsafeShipStars)
                    star.SpaceShips.RemoveAll(delegate(StarShipRecord value)
                        { return value.ShipStart == deleteShip.Start; });
                List<ShipHeaderRecord> unsafeShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in shipDeleteSource.GalaxyShips)
                    if (value.Start != deleteShip.Start) unsafeShips.Add(value.Clone());
                List<MissileRecord> unsafeShipMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in shipDeleteSource.GalaxyMissiles)
                    unsafeShipMissiles.Add(value.Clone());
                unsafeShipMissiles[0].TargetType = 1;
                unsafeShipMissiles[0].TargetId = deleteShip.ObjectId;
                File.Delete(args[2]);
                bool unsafeShipDeleteRejected = false;
                try
                {
                    shipDeleteSource.WriteCopy(args[2], shipDeleteSource.Metadata.Clone(), null, null,
                        unsafeShipStars, null, unsafeShips, null, null, null, null,
                        unsafeShipMissiles);
                }
                catch (InvalidOperationException)
                {
                    unsafeShipDeleteRejected = true;
                }
                if (!unsafeShipDeleteRejected || File.Exists(args[2]))
                    throw new InvalidOperationException(
                        "unsafe TStar.Ships deletion was not rejected before writing");

                SavContainer weaponSource = null;
                string deletedWeaponName = null;
                int bestSpecialOwnerCount = -1;
                foreach (SavContainer candidate in loaded)
                {
                    foreach (CustomWeaponInfoRecord descriptor in candidate.CustomWeaponInfos)
                    {
                        bool hasInstance = false;
                        int specialOwnerCount = 0;
                        foreach (ItemHeaderRecord item in candidate.GalaxyItems)
                            if (item.Type == 68 && string.Equals(item.CustomWeaponName,
                                descriptor.SystemName, StringComparison.Ordinal))
                            {
                                hasInstance = true;
                                specialOwnerCount += CountSpecialItemOwners(candidate, item.Start);
                            }
                        if (!hasInstance)
                            foreach (MissileRecord missile in candidate.GalaxyMissiles)
                                if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                                    descriptor.SystemName, StringComparison.Ordinal))
                                { hasInstance = true; break; }
                        if (hasInstance && specialOwnerCount > bestSpecialOwnerCount)
                        {
                            weaponSource = candidate;
                            deletedWeaponName = descriptor.SystemName;
                            bestSpecialOwnerCount = specialOwnerCount;
                        }
                    }
                }
                if (weaponSource == null)
                    throw new InvalidOperationException("no used TCustomWeaponInfo deletion fixture found");
                if (bestSpecialOwnerCount <= 0)
                    throw new InvalidOperationException(
                        "no TCustomWeapon fixture in TStar.ItemsInSpace/TScript shop slots found");
                Console.WriteLine("TCustomWeapon cascade fixture: {0} / {1} / special owners={2}",
                    weaponSource.SourcePath, deletedWeaponName, bestSpecialOwnerCount);

                List<StarHeaderRecord> weaponStars = new List<StarHeaderRecord>();
                foreach (StarHeaderRecord value in weaponSource.GalaxyStars) weaponStars.Add(value.Clone());
                List<PlanetHeaderRecord> weaponPlanets = new List<PlanetHeaderRecord>();
                foreach (PlanetHeaderRecord value in weaponSource.GalaxyPlanets) weaponPlanets.Add(value.Clone());
                List<ShipHeaderRecord> weaponShips = new List<ShipHeaderRecord>();
                foreach (ShipHeaderRecord value in weaponSource.GalaxyShips) weaponShips.Add(value.Clone());
                List<ItemHeaderRecord> weaponItems = new List<ItemHeaderRecord>();
                foreach (ItemHeaderRecord value in weaponSource.GalaxyItems) weaponItems.Add(value.Clone());
                List<MissileRecord> weaponMissiles = new List<MissileRecord>();
                foreach (MissileRecord value in weaponSource.GalaxyMissiles) weaponMissiles.Add(value.Clone());
                List<CustomWeaponInfoRecord> weaponDescriptors = new List<CustomWeaponInfoRecord>();
                foreach (CustomWeaponInfoRecord value in weaponSource.CustomWeaponInfos)
                    weaponDescriptors.Add(value.Clone());
                List<StoredItemRecord> weaponStoredItems = new List<StoredItemRecord>();
                foreach (StoredItemRecord value in weaponSource.StoredItems) weaponStoredItems.Add(value.Clone());
                GalaxySummaryData weaponSummary = weaponSource.GalaxySummary.Clone();
                GalaxyPrefixData weaponGalaxy = weaponSource.GalaxyPrefix.Clone();
                CustomWeaponDeleteResult cascade = weaponSource.DeleteCustomWeaponCascade(
                    deletedWeaponName, weaponDescriptors, weaponStars, weaponPlanets, weaponShips,
                    weaponItems, weaponMissiles, weaponStoredItems, weaponSummary);
                if (cascade.RemovedItemStarts.Count + cascade.RemovedMissileIds.Count == 0)
                    throw new InvalidOperationException("TCustomWeapon cascade removed no used instances");
                weaponGalaxy.CustomModWeaponCount = weaponDescriptors.Count;
                File.Delete(args[2]);
                weaponSource.WriteCopy(args[2], weaponSource.Metadata.Clone(), null, weaponGalaxy,
                    weaponStars, weaponPlanets, weaponShips, weaponItems, null, null, null,
                    weaponMissiles, weaponDescriptors, null, weaponStoredItems, weaponSummary, null);
                SavContainer weaponPatched = SavContainer.Load(args[2]);
                if (weaponPatched.CustomWeaponInfos.Count != weaponSource.CustomWeaponInfos.Count - 1)
                    throw new InvalidOperationException("deleted TCustomWeaponInfo count was not persisted");
                foreach (CustomWeaponInfoRecord descriptor in weaponPatched.CustomWeaponInfos)
                    if (string.Equals(descriptor.SystemName, deletedWeaponName, StringComparison.Ordinal))
                        throw new InvalidOperationException("deleted TCustomWeaponInfo remained in TGalaxy");
                foreach (ItemHeaderRecord item in weaponPatched.GalaxyItems)
                    if (item.Type == 68 && string.Equals(item.CustomWeaponName,
                        deletedWeaponName, StringComparison.Ordinal))
                        throw new InvalidOperationException("deleted TCustomWeapon item remained in SAV");
                foreach (MissileRecord missile in weaponPatched.GalaxyMissiles)
                    if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                        deletedWeaponName, StringComparison.Ordinal))
                        throw new InvalidOperationException("deleted TCustomWeapon missile remained in SAV");

                List<CustomWeaponInfoRecord> unsafeDescriptors = new List<CustomWeaponInfoRecord>();
                foreach (CustomWeaponInfoRecord value in weaponSource.CustomWeaponInfos)
                    if (!string.Equals(value.SystemName, deletedWeaponName, StringComparison.Ordinal))
                        unsafeDescriptors.Add(value.Clone());
                GalaxyPrefixData unsafeGalaxy = weaponSource.GalaxyPrefix.Clone();
                unsafeGalaxy.CustomModWeaponCount = unsafeDescriptors.Count;
                File.Delete(args[2]);
                bool unsafeDeleteRejected = false;
                try
                {
                    weaponSource.WriteCopy(args[2], weaponSource.Metadata.Clone(), null, unsafeGalaxy,
                        null, null, null, null, null, null, null, null, unsafeDescriptors);
                }
                catch (InvalidOperationException)
                {
                    unsafeDeleteRejected = true;
                }
                if (!unsafeDeleteRejected || File.Exists(args[2]))
                    throw new InvalidOperationException(
                        "unsafe TCustomWeaponInfo deletion was not rejected before writing");

            }

            int minimumAsteroids = int.MaxValue, maximumAsteroids = 0, totalMissiles = 0, customMissiles = 0,
                totalWarOperations = 0, totalGates = 0, totalScripts = 0, totalShipEquipment = 0,
                totalShipArtefacts = 0, totalShipDrops = 0, totalSpecialBonuses = 0,
                totalStatusEffects = 0, totalCustomShipInfos = 0, totalRangerQuests = 0,
                totalPlayerStorageItems = 0, totalStoredItems = 0, totalPlayerSatellites = 0,
                totalRobotMaps = 0, totalJournalRecords = 0,
                totalIllnessRecords = 0, totalRewards = 0,
                minimumRewards = int.MaxValue, maximumRewards = 0, totalPlayerNews = 0,
                totalPlanetRelations = 0, planetRelationCountMismatches = 0,
                totalPlanetShopItems = 0, totalPlanetWarriors = 0,
                planetMissingWarriorLists = 0,
                totalPlanetSputniks = 0, totalPlanetGoneItems = 0,
                totalShipTakeItems = 0, totalShipRecentlyDropped = 0,
                totalShipRelations = 0, shipRelationCountMismatches = 0,
                totalRuinsEquipmentItems = 0, totalRuinsSaleSatellites = 0,
                nonEmptyInfectionPlaces = 0, nonEmptyEquipmentSets = 0,
                equipmentSetItemReferences = 0, artefactSetItemReferences = 0,
                activePlayerBridges = 0, nonEmptyPlayerBridgeBackgrounds = 0,
                playerBridgeEquipmentItems = 0;
            foreach (SavContainer save in loaded)
            {
                minimumAsteroids = Math.Min(minimumAsteroids, save.GalaxyAsteroids.Count);
                maximumAsteroids = Math.Max(maximumAsteroids, save.GalaxyAsteroids.Count);
                totalMissiles += save.GalaxyMissiles.Count;
                totalWarOperations += save.GalaxySummary.WarOperations.Count;
                totalGates += save.GalaxySummary.Gates.Count;
                totalScripts += save.ActiveScripts.Count;
                totalStoredItems += save.StoredItems.Count;
                foreach (PlanetHeaderRecord planet in save.GalaxyPlanets)
                {
                    totalPlanetRelations += planet.RelationToRangers.Length;
                    if (planet.RelationCount != save.GalaxySummary.RangerCount)
                        planetRelationCountMismatches++;
                    totalPlanetShopItems += planet.EquipmentShopItems.Count;
                    totalPlanetWarriors += planet.Warriors.Count;
                    if (!planet.HasWarriorList) planetMissingWarriorLists++;
                    totalPlanetSputniks += planet.Satellites.Count;
                    totalPlanetGoneItems += planet.GoneItems.Count;
                }
                foreach (ShipHeaderRecord ship in save.GalaxyShips)
                {
                    totalShipEquipment += ship.EquipmentItems.Count;
                    totalShipArtefacts += ship.ArtefactItems.Count;
                    totalShipDrops += ship.DropListItems.Count;
                    totalSpecialBonuses += ship.SpecialBonuses.Count;
                    totalStatusEffects += ship.StatusEffects.Count;
                    totalCustomShipInfos += ship.CustomShipInfos.Count;
                    totalShipTakeItems += ship.TakeItemReferenceIds.Count;
                    totalShipRecentlyDropped += ship.RecentlyDroppedItemIds.Count;
                    totalShipRelations += ship.RelationToRangers.Length;
                    if (ship.RelationCount != save.GalaxySummary.RangerCount)
                        shipRelationCountMismatches++;
                    totalRuinsEquipmentItems += ship.RuinsEquipmentItems.Count;
                    if (ship.RuinsSaleSatellite != null) totalRuinsSaleSatellites++;
                    totalRangerQuests += ship.RangerQuests.Count;
                    totalPlayerStorageItems += ship.PlayerStorageItems.Count;
                    totalPlayerSatellites += ship.PlayerSatelliteItems.Count;
                    totalRobotMaps += ship.PlayerRobotMaps.Count;
                    totalJournalRecords += ship.PlayerJournalRecords.Count;
                    totalPlayerNews += ship.PlayerNewsRecords.Count;
                    if (ship.IsPlayer)
                    {
                        for (int infection = 0; infection < ship.PlayerInfectionPlaces.Length; infection++)
                            if (!string.IsNullOrEmpty(ship.PlayerInfectionPlaces[infection]))
                                nonEmptyInfectionPlaces++;
                        for (int set = 0; set < 10; set++)
                        {
                            bool nonEmptySet = false;
                            for (int slot = 0; slot < 12; slot++)
                                if (ship.PlayerEquipmentSetItems[set, slot] != 0)
                                { equipmentSetItemReferences++; nonEmptySet = true; }
                            for (int slot = 0; slot < 32; slot++)
                                if (ship.PlayerArtefactSetItems[set, slot] != 0)
                                { artefactSetItemReferences++; nonEmptySet = true; }
                            if (nonEmptySet) nonEmptyEquipmentSets++;
                        }
                        if (ship.PlayerCaptainOnBridge != 0) activePlayerBridges++;
                        if (!string.IsNullOrEmpty(ship.PlayerBridgeBackground))
                            nonEmptyPlayerBridgeBackgrounds++;
                        playerBridgeEquipmentItems += ship.PlayerBridgeRuins.RuinsEquipmentItems.Count;
                    }
                    totalIllnessRecords += ship.Illnesses.Count;
                    totalRewards += ship.Rewards.Count;
                    minimumRewards = Math.Min(minimumRewards, ship.Rewards.Count);
                    maximumRewards = Math.Max(maximumRewards, ship.Rewards.Count);
                }
                foreach (MissileRecord missile in save.GalaxyMissiles)
                    if (missile.IsCustom) customMissiles++;
            }
            Console.WriteLine("native SAV self-test: {0} files verified; asteroids={1}..{2}; missiles={3} (custom={4}); war operations={5}; gates={6}; active scripts={7}; ship lists equipment={8}, artefacts={9}, drops={10}, bonuses={11}, effects={12}, custom-info={13}, ranger-quests={14}, player-storage-items={43}, player-satellites={15}, robot-maps={16}, journal={17}, illness/stimulator={18}, rewards={19} ({20}..{21}/ship), player-news={22}, planet-relations={23} (count-mismatch={24}), planet-shop-items={25}, planet-warriors={26} (missing-list={27}), planet-sputniks={28}, planet-gone-items={29}, ship-take-items={30}, ship-recently-dropped={31}, ship-relations={32} (count-mismatch={33}), ruins-equipment={34}, ruins-sale-satellites={35}, infection-place strings={36}, nonempty equipment sets={37} (equipment refs={38}, artefact refs={39}), active player bridges={40}, bridge backgrounds={41}, bridge equipment={42}, stored-items={44}",
                loaded.Count, minimumAsteroids, maximumAsteroids, totalMissiles, customMissiles,
                totalWarOperations, totalGates, totalScripts, totalShipEquipment, totalShipArtefacts,
                totalShipDrops, totalSpecialBonuses, totalStatusEffects, totalCustomShipInfos,
                totalRangerQuests, totalPlayerSatellites, totalRobotMaps, totalJournalRecords,
                totalIllnessRecords, totalRewards,
                minimumRewards, maximumRewards, totalPlayerNews, totalPlanetRelations,
                planetRelationCountMismatches, totalPlanetShopItems, totalPlanetWarriors,
                planetMissingWarriorLists, totalPlanetSputniks, totalPlanetGoneItems,
                totalShipTakeItems, totalShipRecentlyDropped, totalShipRelations,
                shipRelationCountMismatches, totalRuinsEquipmentItems,
                totalRuinsSaleSatellites, nonEmptyInfectionPlaces, nonEmptyEquipmentSets,
                equipmentSetItemReferences, artefactSetItemReferences, activePlayerBridges,
                nonEmptyPlayerBridgeBackgrounds, playerBridgeEquipmentItems,
                totalPlayerStorageItems, totalStoredItems);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.ToString());
            return 1;
        }
    }

    private static void ValidateMapLines(IList<GalaxyMapLine> lines, string label, string path)
    {
        if (lines == null)
            throw new InvalidOperationException("TConstellation " + label + " list is missing: " + path);
        foreach (GalaxyMapLine line in lines)
            if (float.IsNaN(line.X1) || float.IsNaN(line.Y1) ||
                float.IsNaN(line.X2) || float.IsNaN(line.Y2) ||
                float.IsInfinity(line.X1) || float.IsInfinity(line.Y1) ||
                float.IsInfinity(line.X2) || float.IsInfinity(line.Y2))
                throw new InvalidOperationException("TConstellation " + label + " is invalid: " + path);
    }

    private static void VerifyShipOrderRules()
    {
        StarHeaderRecord from = new StarHeaderRecord {
            ObjectId = 1, Raw08 = 877126563, X = 122.0F, Y = 79.0F
        };
        StarHeaderRecord to = new StarHeaderRecord {
            ObjectId = 37, Raw08 = 1890295475, X = 135.0F, Y = 86.0F, GraphRadius = 200
        };
        ShipHeaderRecord ship = new ShipHeaderRecord { Rnd = 422879693 };
        PlanetHeaderRecord last = new PlanetHeaderRecord { PolarRadius = 3030.0F, Radius = 60 };
        if (ShipOrderRules.JumpData(from, to) != 2)
            throw new InvalidOperationException("TShip jump OrderData formula is incompatible");
        float x, y;
        ShipOrderRules.JumpDestination(ship, from, to, last, out x, out y);
        if (x != 3358.0F || y != 1962.0F)
            throw new InvalidOperationException("TShip jump destination formula is incompatible: " + x + "/" + y);
        StarHeaderRecord cardinalFrom = new StarHeaderRecord {
            Raw08 = 1019274343, X = 121.0F, Y = 38.0F, GraphRadius = 250
        };
        StarHeaderRecord cardinalTo = new StarHeaderRecord {
            Raw08 = 60618940, X = 102.0F, Y = 37.0F
        };
        ShipHeaderRecord cardinalShip = new ShipHeaderRecord { Rnd = 1414448671 };
        PlanetHeaderRecord cardinalLast = new PlanetHeaderRecord { PolarRadius = 2680.0F, Radius = 60 };
        ShipOrderRules.JumpDestination(cardinalShip, cardinalFrom, cardinalTo,
            cardinalLast, out x, out y);
        if (x != -3540.0F || y != 0.0F)
            throw new InvalidOperationException("TShip cardinal jump destination is incompatible: " +
                x + "/" + y);
        HoleRecord hole = new HoleRecord {
            FromStarId = 1, ToStarId = 37, FromX = 12.5F, FromY = -3.5F,
            ToX = 44.0F, ToY = 55.0F
        };
        uint data;
        ShipOrderRules.HoleDestination(hole, 1, out data, out x, out y);
        if (data != 2 || x != 12.5F || y != -3.5F)
            throw new InvalidOperationException("TShip hole-from order formula is invalid");
        ShipOrderRules.HoleDestination(hole, 37, out data, out x, out y);
        if (data != 0x10002U || x != 44.0F || y != 55.0F)
            throw new InvalidOperationException("TShip hole-to order formula is invalid");
        if (ShipOrderRules.DeterministicRandom(-5, 5, 17U) != 1)
            throw new InvalidOperationException("TShip deterministic order random is incompatible");
        float evenAngle = ShipOrderRules.PlanetTakeoffAngle(2U, 0.0F, 0.0F, 0.0F, -400.0F, 3);
        float oddAngle = ShipOrderRules.PlanetTakeoffAngle(3U, 0.0F, 0.0F, 0.0F, -400.0F, 3);
        float carrierAngle = ShipOrderRules.CarrierTakeoffAngle(0.0F, 0.0F, 0.0F, -400.0F, -2);
        if (evenAngle != 273.0F || oddAngle != 93.0F || carrierAngle != 178.0F)
            throw new InvalidOperationException("TShip take-off angle formula is incompatible: " +
                evenAngle + "/" + oddAngle + "/" + carrierAngle);
        if (ShipOrderRules.AngleDifference(355.0F, 5.0F) != 10.0F)
            throw new InvalidOperationException("TShip take-off collision angle wrapping is invalid");
    }

    private static void DumpAchievementCandidates(SavContainer save)
    {
        ShipHeaderRecord player = null;
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            if (ship.ObjectId == save.GalaxySummary.PlayerObjectId) player = ship;
        if (player == null) return;
        byte[] data = save.MainPayload;
        int limit = save.GalaxySummary.PlanetReferenceListOffset;
        int nextStar = limit;
        foreach (StarHeaderRecord star in save.GalaxyStars)
            if (star.Start > player.Start) { nextStar = Math.Min(nextStar, star.Start); }
        int nextShip = limit;
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            if (ship.Start > player.Start && ship.Start < nextShip) nextShip = ship.Start;
        if (nextShip < limit) limit = nextShip - 1;
        Console.WriteLine("player_candidate_limit=0x{0:X} next_star=0x{1:X} next_ship=0x{2:X}", limit, nextStar, nextShip);
        int probe = player.Start + 0x96C;
        if (probe >= 0 && probe <= data.Length - 96)
            Console.WriteLine("probe_0x{0:X}={1}", probe, BitConverter.ToString(data, probe, 96));
        DumpStringListCandidates(data, player.FixedPrefixEnd, limit);
        DumpAchievementKeyLists(data, player.FixedPrefixEnd, limit);
        DumpAchievementStructureCandidates(data, player.FixedPrefixEnd, limit);
        for (int offset = player.FixedPrefixEnd; offset <= limit - 58; offset++)
        {
            if (data[offset - 3] > 7 || data[offset - 2] > 1 || data[offset - 1] > 1 ||
                data[offset + 16] > 100 || data[offset + 57] > 1) continue;
            int asteroids = BitConverter.ToInt32(data, offset);
            int fried = BitConverter.ToInt32(data, offset + 4);
            int defended = BitConverter.ToInt32(data, offset + 8);
            int pirateSystems = BitConverter.ToInt32(data, offset + 12);
            int programs = BitConverter.ToInt32(data, offset + 17);
            int pirates = BitConverter.ToInt32(data, offset + 21);
            int health = BitConverter.ToInt32(data, offset + 25);
            int fuel = BitConverter.ToInt32(data, offset + 29);
            int lastTank = BitConverter.ToInt32(data, offset + 33);
            int visited = BitConverter.ToInt32(data, offset + 37);
            int[] bounded = { asteroids, fried, defended, pirateSystems, programs, pirates, health, fuel };
            bool plausible = true;
            foreach (int value in bounded)
                if (value < 0 || value > 1000000000) { plausible = false; break; }
            if (!plausible || lastTank < 0 || lastTank >= save.GalaxySummary.NextObjectId ||
                visited < 0 || visited > save.GalaxyPlanets.Count) continue;
            int nonzero = 0;
            foreach (int value in bounded) if (value != 0) nonzero++;
            if (lastTank != 0) nonzero++;
            if (visited != 0) nonzero++;
            if (nonzero < 2) continue;
            int[] starts = { 41, 45, 49, 53 };
            foreach (int start in starts)
            {
                int value = BitConverter.ToInt32(data, offset + start);
                if (value < 0 || value > 1000000000) { plausible = false; break; }
            }
            if (!plausible) continue;
            int ruinProbeLength = Math.Min(256, limit - offset - 58);
            int ruinNonzero = 0;
            for (int probeIndex = 0; probeIndex < ruinProbeLength; probeIndex++)
                if (data[offset + 58 + probeIndex] != 0) ruinNonzero++;
            if (ruinNonzero > 24) continue;
            Console.WriteLine("ach_candidate=0x{0:X} pre={1}/{2}/{3} values={4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14} exp={15},{16},{17},{18} captain={19}",
                offset, data[offset - 3], data[offset - 2], data[offset - 1],
                BitConverter.ToInt32(data, offset), BitConverter.ToInt32(data, offset + 4),
                BitConverter.ToInt32(data, offset + 8), BitConverter.ToInt32(data, offset + 12), data[offset + 16],
                BitConverter.ToInt32(data, offset + 17), BitConverter.ToInt32(data, offset + 21),
                BitConverter.ToInt32(data, offset + 25), BitConverter.ToInt32(data, offset + 29),
                BitConverter.ToInt32(data, offset + 33), BitConverter.ToInt32(data, offset + 37),
                BitConverter.ToInt32(data, offset + 41), BitConverter.ToInt32(data, offset + 45),
                BitConverter.ToInt32(data, offset + 49), BitConverter.ToInt32(data, offset + 53), data[offset + 57]);
            Console.WriteLine("ruin_probe_nonzero={0}/{1}", ruinNonzero, ruinProbeLength);
        }
    }

    private static void DumpAchievementKeyLists(byte[] data, int start, int end)
    {
        for (int offset = start; offset <= end - 4; offset++)
        {
            int count = BitConverter.ToInt32(data, offset);
            if (count <= 0 || count > 100) continue;
            int cursor = offset + 4;
            List<string> values = new List<string>();
            bool valid = true;
            for (int index = 0; index < count; index++)
            {
                string value;
                if (!TryReadDiagnosticString(data, ref cursor, 256, false, out value)) { valid = false; break; }
                foreach (char character in value)
                    if (!((character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') || character == '_'))
                    { valid = false; break; }
                if (!valid) break;
                values.Add(value);
            }
            if (valid)
                Console.WriteLine("achievement_key_list=0x{0:X}..0x{1:X} count={2} remaining={3} values={4}",
                    offset, cursor, count, end - cursor, string.Join(" | ", values.ToArray()));
        }
    }

    private static void DumpAchievementStructureCandidates(byte[] data, int start, int end)
    {
        for (int marker = start; marker <= end - 1801; marker++)
        {
            if (data[marker] != 10) continue;
            int cursor = marker + 1;
            int blocks = 0;
            while (blocks < 10 && cursor <= end - 180)
            {
                if (BitConverter.ToUInt16(data, cursor) != 12) break;
                cursor += 2 + 12 * 4;
                if (BitConverter.ToUInt16(data, cursor) != 32) break;
                cursor += 2 + 32 * 4;
                blocks++;
            }
            if (blocks >= 1)
            {
                Console.WriteLine("ach_structure=0x{0:X} blocks={1} cursor=0x{2:X} next={3}",
                    marker, blocks, cursor, data[cursor]);
                if (blocks == 10)
                    Console.WriteLine("ach_structure_tail={0}", BitConverter.ToString(data, cursor,
                        Math.Min(512, end - cursor)));
            }
        }
    }

    private static void DumpStringListCandidates(byte[] data, int start, int end)
    {
        for (int offset = start; offset <= end - 4; offset++)
        {
            int count = BitConverter.ToInt32(data, offset);
            if (count <= 0 || count > 100) continue;
            int cursor = offset + 4;
            List<string> values = new List<string>();
            bool valid = true;
            for (int index = 0; index < count; index++)
            {
                string value;
                if (!TryReadDiagnosticString(data, ref cursor, 256, false, out value))
                {
                    valid = false;
                    break;
                }
                values.Add(value);
            }
            if (valid && cursor == end)
                Console.WriteLine("tail_string_list=0x{0:X} count={1} values={2}",
                    offset, count, string.Join(" | ", values.ToArray()));
        }
        Console.WriteLine("tail_i32=0x{0:X}:{1}", end - 4, BitConverter.ToInt32(data, end - 4));
    }

    private static bool TryReadEmbeddedShipHeader(byte[] data, int start, uint nextObjectId,
        out uint objectId, out string name, out byte type)
    {
        objectId = 0; name = string.Empty; type = 0;
        if (start < 0 || start > data.Length - 32) return false;
        objectId = BitConverter.ToUInt32(data, start);
        if (objectId >= nextObjectId) return false;
        int offset = start + 4;
        if (!TryReadDiagnosticString(data, ref offset, 80, false, out name)) return false;
        string script;
        if (!TryReadDiagnosticString(data, ref offset, 128, true, out script) || offset > data.Length - 2)
            return false;
        type = data[offset];
        return type >= 1 && type <= 13;
    }

    private static bool TryReadDiagnosticString(byte[] data, ref int offset, int maximumLength,
        bool allowEmpty, out string value)
    {
        value = string.Empty;
        System.Text.StringBuilder text = new System.Text.StringBuilder();
        for (int index = 0; index <= maximumLength; index++)
        {
            if (offset > data.Length - 2) return false;
            char character = (char)(data[offset] | data[offset + 1] << 8);
            offset += 2;
            if (character == '\0')
            {
                value = text.ToString();
                return allowEmpty || value.Length > 0;
            }
            if (char.IsControl(character)) return false;
            text.Append(character);
        }
        return false;
    }

    private static float ShiftCoordinate(float value)
    {
        return value >= 1000000.0F ? value - 1.0F : value + 1.0F;
    }

    private static float ShiftSerializedFloat(float value)
    {
        return value == 0.0F ? 1.0F : value * 0.5F;
    }

    private static int ShiftInt32(int value)
    {
        return value == int.MaxValue ? value - 1 : value + 1;
    }

    private static uint ShiftUInt32(uint value)
    {
        return value == uint.MaxValue ? value - 1 : value + 1;
    }

    private static ushort ShiftUInt16(ushort value)
    {
        return value == ushort.MaxValue ? (ushort)(value - 1) : (ushort)(value + 1);
    }

    private static byte ShiftByte(byte value)
    {
        return value == byte.MaxValue ? (byte)(value - 1) : (byte)(value + 1);
    }

    private static int FindEditableItem(List<ItemHeaderRecord> items, HashSet<int> excludedStarts)
    {
        for (int index = 0; index < items.Count; index++)
            if (!excludedStarts.Contains(items[index].Start) &&
                items[index].Type >= 8 && items[index].Name.Length < 500)
                return index;
        throw new InvalidOperationException("no editable equipment item found");
    }

    private static int FindGoodsItem(List<ItemHeaderRecord> items, HashSet<int> excludedStarts)
    {
        for (int index = 0; index < items.Count; index++)
            if (!excludedStarts.Contains(items[index].Start) && items[index].HasGoodsTail) return index;
        throw new InvalidOperationException("no TGoodsItem record found for writer self-test");
    }

    private static PlanetHeaderRecord FindPlanet(SavContainer save, uint objectId)
    {
        foreach (PlanetHeaderRecord value in save.GalaxyPlanets)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TPlanet was not found");
    }

    private static ShipHeaderRecord FindShip(SavContainer save, uint objectId)
    {
        foreach (ShipHeaderRecord value in save.GalaxyShips)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TShip was not found");
    }

    private static ShipHeaderRecord FindShipByStartOrNull(SavContainer save, int start)
    {
        foreach (ShipHeaderRecord value in save.GalaxyShips)
            if (value.Start == start) return value;
        return null;
    }

    private static ShipHeaderRecord FindShipByIdOrNull(SavContainer save, uint objectId)
    {
        foreach (ShipHeaderRecord value in save.GalaxyShips)
            if (value.ObjectId == objectId) return value;
        return null;
    }

    private static bool HasShipReference(SavContainer save, uint shipObjectId)
    {
        GalaxySummaryData summary = save.GalaxySummary;
        if (summary.PlayerObjectId == shipObjectId || summary.AutoBattleShipObjectId == shipObjectId ||
            summary.BlazerObjectId == shipObjectId || summary.KellerObjectId == shipObjectId ||
            summary.TerronObjectId == shipObjectId) return true;
        if (UIntArrayContains(summary.RangerObjectIds, shipObjectId))
            return true;
        if (UIntArrayContains(summary.EminentRangerObjectIds, shipObjectId)) return true;
        foreach (StarHeaderRecord star in save.GalaxyStars)
            foreach (StarDropItemRecord drop in star.DropItems)
                if (drop.ShipObjectId == shipObjectId) return true;
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            if (ship.CurrentShipId == shipObjectId || ship.OrderObjectId == shipObjectId ||
                (ship.OrderType == 2 && ship.OrderObjectId == (shipObjectId | 0x80000000U)) ||
                ship.PlayerBridgeCurrentShipId == shipObjectId) return true;
        foreach (MissileRecord missile in save.GalaxyMissiles)
            if (missile.ShipId == shipObjectId ||
                missile.TargetType == 1 && missile.TargetId == shipObjectId ||
                missile.TargetLostType == 1 && missile.TargetLostId == shipObjectId) return true;
        foreach (ItemHeaderRecord item in save.GalaxyItems)
        {
            if (item.DerivedFields == null) continue;
            byte targetType = 0; uint targetId = 0;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                if (field.ControlName == "edWeaponTargetType")
                    targetType = checked((byte)field.IntegerValue);
                else if (field.ControlName == "cbWeaponTarget")
                    targetId = checked((uint)field.IntegerValue);
            }
            if (targetType == 1 && targetId == shipObjectId) return true;
        }
        foreach (ScriptRecord script in summary.ActiveScripts)
            foreach (ScriptShipRecord binding in script.ShipBindings)
                if (binding.ShipObjectId == shipObjectId) return true;
        foreach (WarOperationRecord operation in summary.WarOperations)
            if (operation.ShipObjectIds.Contains(shipObjectId)) return true;
        return false;
    }

    private static bool UIntArrayContains(uint[] values, uint objectId)
    {
        if (values == null) return false;
        foreach (uint value in values) if (value == objectId) return true;
        return false;
    }

    private static ShipHeaderRecord FindShip(IList<ShipHeaderRecord> ships, uint objectId)
    {
        foreach (ShipHeaderRecord value in ships)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TShip was not found");
    }

    private static int FindNormalShipIndex(List<ShipHeaderRecord> ships)
    {
        for (int index = 0; index < ships.Count; index++)
            if (ships[index].HasNormalShipTail) return index;
        throw new InvalidOperationException("no TNormalShip record found for writer self-test");
    }

    private static bool SupportsWriterCoverage(SavContainer save)
    {
        if (save == null) return false;
        bool normal = false, ranger = false, player = false, ruins = false;
        bool[] simple = new bool[4];
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
        {
            normal |= ship.HasNormalShipTail;
            ranger |= ship.HasRangerTail;
            player |= ship.IsPlayer && ship.HasPlayerPrefix;
            ruins |= ship.HasRuinsTail;
            if (ship.HasSimpleDerivedTail)
            {
                if (ship.Type == 0) simple[0] = true;
                else if (ship.Type == 2) simple[1] = true;
                else if (ship.Type == 3) simple[2] = true;
                else if (ship.Type == 4) simple[3] = true;
            }
        }
        return normal && ranger && player && ruins &&
            simple[0] && simple[1] && simple[2] && simple[3];
    }

    private static int[] FindSimpleDerivedShipIndices(List<ShipHeaderRecord> ships)
    {
        int[] result = { -1, -1, -1, -1 };
        byte[] types = { 0, 2, 3, 4 };
        for (int index = 0; index < ships.Count; index++)
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
                if (result[typeIndex] < 0 && ships[index].Type == types[typeIndex] &&
                    ships[index].HasSimpleDerivedTail) result[typeIndex] = index;
        for (int index = 0; index < result.Length; index++)
            if (result[index] < 0) throw new InvalidOperationException(
                "no simple derived TShip type " + types[index] + " found for writer self-test");
        return result;
    }

    private static int FindRangerShipIndex(List<ShipHeaderRecord> ships)
    {
        for (int index = 0; index < ships.Count; index++)
            if (ships[index].HasRangerTail) return index;
        throw new InvalidOperationException("no TRanger record found for writer self-test");
    }

    private static int FindPlayerShipIndex(List<ShipHeaderRecord> ships)
    {
        for (int index = 0; index < ships.Count; index++)
            if (ships[index].IsPlayer && ships[index].HasPlayerPrefix) return index;
        throw new InvalidOperationException("no TPlayer fixed prefix found for writer self-test");
    }

    private static int FindRuinsShipIndex(List<ShipHeaderRecord> ships)
    {
        for (int index = 0; index < ships.Count; index++)
            if (ships[index].HasRuinsTail) return index;
        throw new InvalidOperationException("no TRuins record found for writer self-test");
    }

    private static int ReadUInt16(byte[] data, int offset)
    {
        return data[offset] | data[offset + 1] << 8;
    }

    private static int CountItemOwners(SavContainer save, int itemStart)
    {
        int count = 0;
        foreach (StarHeaderRecord star in save.GalaxyStars)
        {
            count += CountShipItemOwners(star.SpaceItems, itemStart);
            foreach (StarDropItemRecord record in star.DropItems)
                if (record.ItemStart == itemStart) count++;
        }
        foreach (PlanetHeaderRecord planet in save.GalaxyPlanets)
        {
            count += CountShipItemOwners(planet.EquipmentShopItems, itemStart);
            foreach (PlanetGoneItemRecord record in planet.GoneItems)
                if (record.ItemStart == itemStart) count++;
        }
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            count += CountShipOwners(ship, itemStart);
        foreach (ItemHeaderRecord item in save.GalaxyItems)
            if (item.NestedTranclucator != null)
                count += CountShipOwners(item.NestedTranclucator, itemStart);
        foreach (StoredItemRecord record in save.StoredItems)
            if (record.ItemStart == itemStart) count++;
        foreach (ScriptShopSlotRecord record in save.GalaxySummary.ScriptShopSlots)
            if (record.HasEquipment && record.ItemStart == itemStart) count++;
        return count;
    }

    private static bool HasItemReference(SavContainer save, uint itemObjectId)
    {
        foreach (MissileRecord missile in save.GalaxyMissiles)
            if (missile.TargetType == 2 && missile.TargetId == itemObjectId ||
                missile.TargetLostType == 2 && missile.TargetLostId == itemObjectId)
                return true;
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            if (ShipHasItemReference(ship, itemObjectId)) return true;
        foreach (ItemHeaderRecord item in save.GalaxyItems)
        {
            if (item.DerivedFields == null) continue;
            byte targetType = 0;
            uint targetId = 0;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                if (field.ControlName == "edWeaponTargetType")
                    targetType = checked((byte)field.IntegerValue);
                else if (field.ControlName == "cbWeaponTarget")
                    targetId = checked((uint)field.IntegerValue);
            }
            if (targetType == 2 && targetId == itemObjectId) return true;
        }
        return false;
    }

    private static bool ShipHasItemReference(ShipHeaderRecord ship, uint itemObjectId)
    {
        if (ship == null) return false;
        if (ship.OrderObjectId == itemObjectId) return true;
        if (ship.TakeItemReferenceIds != null && ship.TakeItemReferenceIds.Contains(itemObjectId))
            return true;
        if (ship.RecentlyDroppedItemIds != null &&
            ship.RecentlyDroppedItemIds.Contains(itemObjectId)) return true;
        if (MatrixContains(ship.PlayerEquipmentSetItems, itemObjectId) ||
            MatrixContains(ship.PlayerArtefactSetItems, itemObjectId)) return true;
        return ship.PlayerBridgeRuins != null &&
            ShipHasItemReference(ship.PlayerBridgeRuins, itemObjectId);
    }

    private static bool MatrixContains(uint[,] values, uint objectId)
    {
        if (values == null) return false;
        for (int row = 0; row < values.GetLength(0); row++)
            for (int column = 0; column < values.GetLength(1); column++)
                if (values[row, column] == objectId) return true;
        return false;
    }

    private static int CountSpecialItemOwners(SavContainer save, int itemStart)
    {
        int count = 0;
        foreach (StarHeaderRecord star in save.GalaxyStars)
            count += CountShipItemOwners(star.SpaceItems, itemStart);
        foreach (ScriptShopSlotRecord record in save.GalaxySummary.ScriptShopSlots)
            if (record.HasEquipment && record.ItemStart == itemStart) count++;
        return count;
    }

    private static int CountShipOwners(ShipHeaderRecord ship, int itemStart)
    {
        if (ship == null) return 0;
        int count = CountShipItemOwners(ship.EquipmentItems, itemStart) +
            CountShipItemOwners(ship.ArtefactItems, itemStart) +
            CountShipItemOwners(ship.DropListItems, itemStart) +
            CountShipItemOwners(ship.RuinsEquipmentItems, itemStart);
        if (ship.PlayerStorageItems != null)
            foreach (PlayerStorageItemRecord record in ship.PlayerStorageItems)
                if (record.ItemStart == itemStart) count++;
        if (ship.PlayerBridgeRuins != null)
            count += CountShipOwners(ship.PlayerBridgeRuins, itemStart);
        return count;
    }

    private static int CountShipItemOwners(IList<ShipItemListEntry> records, int itemStart)
    {
        int count = 0;
        if (records != null)
            foreach (ShipItemListEntry record in records)
                if (record.ItemStart == itemStart) count++;
        return count;
    }

    private static ItemHeaderRecord FindItem(SavContainer save, byte type, uint objectId)
    {
        foreach (ItemHeaderRecord value in save.GalaxyItems)
            if (value.Type == type && value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TItem was not found");
    }

    private static int FindEquipmentItem(List<ItemHeaderRecord> items, HashSet<int> excludedStarts)
    {
        for (int index = 0; index < items.Count; index++)
            if (items[index].Type >= 8 && !excludedStarts.Contains(items[index].Start)) return index;
        throw new InvalidOperationException("no TEquipment record found for writer self-test");
    }

    private static int FindCustomWeaponAmmoItem(List<ItemHeaderRecord> items)
    {
        for (int index = 0; index < items.Count; index++)
        {
            ItemHeaderRecord item = items[index];
            if (item.Type != 68 || item.DerivedFields == null) continue;
            bool ammunition = false, maxAmmunition = false;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                ammunition |= field.ControlName == "edAmmunition";
                maxAmmunition |= field.ControlName == "edMaxAmmunition";
            }
            if (ammunition && maxAmmunition) return index;
        }
        return -1;
    }

    private static int FindWeaponTargetItem(List<ItemHeaderRecord> items, HashSet<int> excludedStarts)
    {
        for (int index = 0; index < items.Count; index++)
        {
            ItemHeaderRecord item = items[index];
            if (excludedStarts.Contains(item.Start) || item.DerivedFields == null) continue;
            bool targetType = false, target = false;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                targetType |= field.ControlName == "edWeaponTargetType";
                target |= field.ControlName == "cbWeaponTarget";
            }
            if (targetType && target) return index;
        }
        return -1;
    }

    private static HoleRecord FindHole(SavContainer save, uint objectId)
    {
        foreach (HoleRecord value in save.GalaxyHoles)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched THole was not found");
    }

    private static void ToggleHullInterceptors(ItemHeaderRecord item, uint targetShipId)
    {
        ItemDerivedField flag = null, energyMax = null;
        foreach (ItemDerivedField field in item.DerivedFields)
        {
            if (field.ControlName == "$HullHasInterceptors") flag = field;
            else if (field.ControlName == "edEnergyMax") energyMax = field;
        }
        if (flag == null || energyMax == null)
            throw new InvalidOperationException("THull interceptor fields were not parsed");
        item.DerivedFields.RemoveAll(delegate(ItemDerivedField field)
        {
            return field.ControlName == "cbInterceptorsNextTarget" ||
                field.ControlName == "cbInterceptorsStrategy" ||
                field.ControlName == "edInterceptorsDuration";
        });
        if (flag.IntegerValue != 0)
        {
            flag.IntegerValue = 0;
            return;
        }
        flag.IntegerValue = 1;
        int insertion = item.DerivedFields.IndexOf(energyMax) + 1;
        ItemDerivedField target = new ItemDerivedField();
        target.ControlName = "cbInterceptorsNextTarget";
        target.Kind = ItemDerivedField.UInt32;
        target.IntegerValue = targetShipId;
        item.DerivedFields.Insert(insertion++, target);
        ItemDerivedField strategy = new ItemDerivedField();
        strategy.ControlName = "cbInterceptorsStrategy";
        strategy.Kind = ItemDerivedField.Byte;
        strategy.IntegerValue = 1;
        item.DerivedFields.Insert(insertion++, strategy);
        ItemDerivedField duration = new ItemDerivedField();
        duration.ControlName = "edInterceptorsDuration";
        duration.Kind = ItemDerivedField.Byte;
        duration.IntegerValue = 2;
        item.DerivedFields.Insert(insertion, duration);
    }

    private static AsteroidRecord FindAsteroid(SavContainer save, uint objectId)
    {
        foreach (AsteroidRecord value in save.GalaxyAsteroids)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TAsteroid was not found");
    }

    private static MissileRecord FindMissile(SavContainer save, uint objectId)
    {
        foreach (MissileRecord value in save.GalaxyMissiles)
            if (value.ObjectId == objectId) return value;
        throw new InvalidOperationException("patched TMissile was not found");
    }

    private static AsteroidRecord FindUnreferencedAsteroid(SavContainer save)
    {
        foreach (AsteroidRecord candidate in save.GalaxyAsteroids)
            if (!ObjectTargetIsReferenced(save, 3, candidate.ObjectId, null)) return candidate;
        return null;
    }

    private static MissileRecord FindUnreferencedMissile(SavContainer save)
    {
        foreach (MissileRecord candidate in save.GalaxyMissiles)
            if (!ObjectTargetIsReferenced(save, 4, candidate.ObjectId, candidate)) return candidate;
        return null;
    }

    private static bool ObjectTargetIsReferenced(SavContainer save, byte type, uint objectId,
        MissileRecord ignoredMissile)
    {
        foreach (MissileRecord value in save.GalaxyMissiles)
        {
            if (object.ReferenceEquals(value, ignoredMissile)) continue;
            if (value.TargetType == type && value.TargetId == objectId ||
                value.TargetLostType == type && value.TargetLostId == objectId) return true;
        }
        foreach (ShipHeaderRecord ship in save.GalaxyShips)
            if (ship.OrderObjectId == objectId) return true;
        foreach (ItemHeaderRecord item in save.GalaxyItems)
        {
            if (item.DerivedFields == null) continue;
            byte targetType = 0; uint targetId = 0;
            foreach (ItemDerivedField field in item.DerivedFields)
            {
                if (field.ControlName == "edWeaponTargetType") targetType = checked((byte)field.IntegerValue);
                else if (field.ControlName == "cbWeaponTarget") targetId = checked((uint)field.IntegerValue);
            }
            if (targetType == type && targetId == objectId) return true;
        }
        return false;
    }

    private static HoleRecord FindHoleOrNull(SavContainer save, uint objectId)
    {
        foreach (HoleRecord value in save.GalaxyHoles) if (value.ObjectId == objectId) return value;
        return null;
    }

    private static AsteroidRecord FindAsteroidOrNull(SavContainer save, uint objectId)
    {
        foreach (AsteroidRecord value in save.GalaxyAsteroids) if (value.ObjectId == objectId) return value;
        return null;
    }

    private static MissileRecord FindMissileOrNull(SavContainer save, uint objectId)
    {
        foreach (MissileRecord value in save.GalaxyMissiles) if (value.ObjectId == objectId) return value;
        return null;
    }


    private static void AssertOpaqueTailPrefix(byte[] source, int sourceOffset, byte[] patched,
        int patchedOffset, string label)
    {
        const int length = 48;
        if (!EqualSlice(source, sourceOffset, sourceOffset + length,
            patched, patchedOffset, patchedOffset + length))
            throw new InvalidOperationException(label + " derived tail prefix changed");
    }

    private static void AssertPlanetMiddleSpansPreserved(SavContainer source, SavContainer patched,
        IList<ShipHeaderRecord> updatedShips, IList<ItemHeaderRecord> updatedItems,
        IEnumerable<int> removedItemStarts)
    {
        HashSet<int> changedStarts = new HashSet<int>();
        foreach (int start in removedItemStarts) changedStarts.Add(start);
        Dictionary<int, ShipHeaderRecord> shipsByStart = new Dictionary<int, ShipHeaderRecord>();
        foreach (ShipHeaderRecord candidate in updatedShips) shipsByStart[candidate.Start] = candidate;
        foreach (ShipHeaderRecord original in source.GalaxyShips)
        {
            ShipHeaderRecord updated;
            shipsByStart.TryGetValue(original.Start, out updated);
            if (updated == null || !original.ContentEquals(updated)) changedStarts.Add(original.Start);
        }
        Dictionary<int, ItemHeaderRecord> itemsByStart = new Dictionary<int, ItemHeaderRecord>();
        foreach (ItemHeaderRecord candidate in updatedItems) itemsByStart[candidate.Start] = candidate;
        foreach (ItemHeaderRecord original in source.GalaxyItems)
        {
            ItemHeaderRecord updated;
            itemsByStart.TryGetValue(original.Start, out updated);
            if (updated == null || !original.ContentEquals(updated)) changedStarts.Add(original.Start);
        }

        int verified = 0;
        foreach (PlanetHeaderRecord original in source.GalaxyPlanets)
        {
            int sourceStart = original.RelationEndOffset;
            int sourceEnd = original.LateFieldsOffset - 10;
            if (sourceStart <= 0 || sourceEnd <= sourceStart) continue;
            bool containsChangedObject = false;
            foreach (int start in changedStarts)
                if (start >= sourceStart && start < sourceEnd)
                {
                    containsChangedObject = true;
                    break;
                }
            if (containsChangedObject) continue;

            PlanetHeaderRecord result = FindPlanet(patched, original.ObjectId);
            int patchedStart = result.RelationEndOffset;
            int patchedEnd = result.LateFieldsOffset - 10;
            if (sourceEnd - sourceStart != patchedEnd - patchedStart ||
                !EqualSlice(source.MainPayload, sourceStart, sourceEnd,
                    patched.MainPayload, patchedStart, patchedEnd))
                throw new InvalidOperationException("TPlanet shop/warrior opaque span changed: " +
                    original.ObjectId);
            verified++;
        }
        if (verified == 0)
            throw new InvalidOperationException("no unchanged TPlanet shop/warrior span was verified");
    }

    private static bool Equal(byte[] left, byte[] right)
    {
        if (left.Length != right.Length) return false;
        for (int index = 0; index < left.Length; index++)
            if (left[index] != right[index]) return false;
        return true;
    }

    private static void AssertStarGapsPreserved(SavContainer source, SavContainer patched)
    {
        int sourceCursor = source.GalaxyPrefix.End;
        int patchedCursor = patched.GalaxyPrefix.End;
        for (int index = 0; index < source.GalaxyStars.Count; index++)
        {
            StarHeaderRecord sourceStar = source.GalaxyStars[index];
            StarHeaderRecord patchedStar = patched.GalaxyStars[index];
            if (!EqualSlice(source.MainPayload, sourceCursor, sourceStar.Start,
                patched.MainPayload, patchedCursor, patchedStar.Start))
                throw new InvalidOperationException("bytes before TStar " + sourceStar.ObjectId + " changed");
            sourceCursor = sourceStar.HeaderEnd;
            patchedCursor = patchedStar.HeaderEnd;
        }
        if (!EqualSlice(source.MainPayload, sourceCursor, source.MainPayload.Length,
            patched.MainPayload, patchedCursor, patched.MainPayload.Length))
            throw new InvalidOperationException("bytes after final TStar header changed");
    }

    private static bool EqualSlice(byte[] left, int leftStart, int leftEnd, byte[] right, int rightStart, int rightEnd)
    {
        if (leftEnd - leftStart != rightEnd - rightStart) return false;
        for (int index = 0; index < leftEnd - leftStart; index++)
            if (left[leftStart + index] != right[rightStart + index]) return false;
        return true;
    }
}
