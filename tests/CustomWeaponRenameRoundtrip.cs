using System;
using System.Collections.Generic;
using SpaceRangersHdSaveEditor;

internal static class CustomWeaponRenameRoundtrip
{
    private static int Main(string[] args)
    {
        if (args.Length != 2)
            throw new ArgumentException("usage: custom-weapon-rename-roundtrip <source.sav> <output.sav>");
        SavContainer source = SavContainer.Load(args[0]);
        CustomWeaponInfoRecord descriptor = null;
        foreach (CustomWeaponInfoRecord candidate in source.CustomWeaponInfos)
        {
            bool used = false;
            foreach (ItemHeaderRecord item in source.GalaxyItems)
                if (item.Type == 68 && string.Equals(item.CustomWeaponName,
                    candidate.SystemName, StringComparison.Ordinal)) { used = true; break; }
            if (!used)
                foreach (MissileRecord missile in source.GalaxyMissiles)
                    if (missile.IsCustom && string.Equals(missile.CustomWeaponName,
                        candidate.SystemName, StringComparison.Ordinal)) { used = true; break; }
            if (used) { descriptor = candidate; break; }
        }
        if (descriptor == null)
            throw new InvalidOperationException("no used TCustomWeaponInfo descriptor found");

        List<PlayerMessageRecord> messages = new List<PlayerMessageRecord>();
        foreach (PlayerMessageRecord value in source.PlayerMessages) messages.Add(value.Clone());
        List<ConstellationRecord> constellations = new List<ConstellationRecord>();
        foreach (ConstellationRecord value in source.GalaxyConstellations) constellations.Add(value.Clone());
        List<StarHeaderRecord> stars = new List<StarHeaderRecord>();
        foreach (StarHeaderRecord value in source.GalaxyStars) stars.Add(value.Clone());
        List<PlanetHeaderRecord> planets = new List<PlanetHeaderRecord>();
        foreach (PlanetHeaderRecord value in source.GalaxyPlanets) planets.Add(value.Clone());
        List<ShipHeaderRecord> ships = new List<ShipHeaderRecord>();
        foreach (ShipHeaderRecord value in source.GalaxyShips) ships.Add(value.Clone());
        List<ItemHeaderRecord> items = new List<ItemHeaderRecord>();
        foreach (ItemHeaderRecord value in source.GalaxyItems) items.Add(value.Clone());
        List<HoleRecord> holes = new List<HoleRecord>();
        foreach (HoleRecord value in source.GalaxyHoles) holes.Add(value.Clone());
        List<AsteroidRecord> asteroids = new List<AsteroidRecord>();
        foreach (AsteroidRecord value in source.GalaxyAsteroids) asteroids.Add(value.Clone());
        List<MissileRecord> missiles = new List<MissileRecord>();
        foreach (MissileRecord value in source.GalaxyMissiles) missiles.Add(value.Clone());
        List<CustomWeaponInfoRecord> weapons = new List<CustomWeaponInfoRecord>();
        foreach (CustomWeaponInfoRecord value in source.CustomWeaponInfos) weapons.Add(value.Clone());
        List<InterfaceOverrideRecord> interfaces = new List<InterfaceOverrideRecord>();
        foreach (InterfaceOverrideRecord value in source.GalaxySummary.InterfaceOverrides)
            interfaces.Add(value.Clone());
        List<StoredItemRecord> stored = new List<StoredItemRecord>();
        foreach (StoredItemRecord value in source.StoredItems) stored.Add(value.Clone());

        string oldName = descriptor.SystemName;
        string newName = oldName + ".SrhdSaveEditorRenameTest";
        foreach (CustomWeaponInfoRecord value in weapons)
            if (value.Start == descriptor.Start) value.SystemName = newName;
            else if (string.Equals(value.SystemName, newName, StringComparison.Ordinal))
                throw new InvalidOperationException("rename test name already exists");
        int itemCount = 0, missileCount = 0;
        foreach (ItemHeaderRecord value in items)
            if (value.Type == 68 && string.Equals(value.CustomWeaponName, oldName,
                StringComparison.Ordinal))
            { value.CustomWeaponName = newName; itemCount++; }
        foreach (MissileRecord value in missiles)
            if (value.IsCustom && string.Equals(value.CustomWeaponName, oldName,
                StringComparison.Ordinal))
            { value.CustomWeaponName = newName; missileCount++; }
        if (itemCount + missileCount == 0)
            throw new InvalidOperationException("rename test descriptor has no references");

        source.WriteCopy(args[1], source.Metadata.Clone(), messages, source.GalaxyPrefix.Clone(),
            stars, planets, ships, items, source.AchievementStats.Clone(), holes, asteroids,
            missiles, weapons, interfaces, stored, source.GalaxySummary.Clone(), constellations);
        SavContainer changed = SavContainer.Load(args[1]);
        int persistedItems = 0, persistedMissiles = 0;
        foreach (ItemHeaderRecord value in changed.GalaxyItems)
            if (value.Type == 68 && string.Equals(value.CustomWeaponName, newName,
                StringComparison.Ordinal)) persistedItems++;
        foreach (MissileRecord value in changed.GalaxyMissiles)
            if (value.IsCustom && string.Equals(value.CustomWeaponName, newName,
                StringComparison.Ordinal)) persistedMissiles++;
        bool descriptorRenamed = false;
        foreach (CustomWeaponInfoRecord value in changed.CustomWeaponInfos)
            if (value.Start == descriptor.Start && string.Equals(value.SystemName, newName,
                StringComparison.Ordinal)) descriptorRenamed = true;
        if (!descriptorRenamed || persistedItems != itemCount || persistedMissiles != missileCount)
            throw new InvalidOperationException("TCustomWeaponInfo rename cascade round-trip differs");
        Console.WriteLine("TCustomWeapon rename round-trip: {0} -> {1}; items={2}; missiles={3}",
            oldName, newName, itemCount, missileCount);
        return 0;
    }
}
