using System;
using System.IO;

namespace SpaceRangersHdSaveEditor
{
    internal static class CatalogSelfTest
    {
        private static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.Error.WriteLine("Usage: catalog_selftest <game-path> [used-mods-file]");
                return 2;
            }
            try
            {
                string usedMods = args.Length == 2 ? File.ReadAllText(args[1]).Trim() : string.Empty;
                if (usedMods.StartsWith("CurrentMod=", StringComparison.OrdinalIgnoreCase))
                    usedMods = usedMods.Substring("CurrentMod=".Length).Trim();
                GameDataCatalog catalog = GameDataCatalog.Load(args[0], usedMods);
                Console.WriteLine("sources=" + catalog.SourceCount);
                Console.WriteLine("micro_modules=" + catalog.MicroModules.Count);
                Console.WriteLine("hull_series=" + catalog.HullSeries.Count);
                Console.WriteLine("reward_names=" + catalog.RewardNames.Count);
                Console.WriteLine("sputnik_graphs=" + catalog.SputnikGraphs.Count);
                Console.WriteLine("ship_graphs=" + catalog.ShipGraphs.Count);
                Console.WriteLine("planet_graphs=" + catalog.PlanetGraphs.Count);
                Console.WriteLine("asteroid_graphs=" + catalog.AsteroidGraphs.Count);
                Console.WriteLine("constellation_names=" + catalog.ConstellationNames.Count);
                if (catalog.ShipGraphs.Count == 0 ||
                    !catalog.ShipGraphs.Contains("Ship.Tranclucator"))
                    throw new InvalidDataException("Data.SE.Ship graph catalog was not decoded");
                if (catalog.PlanetGraphs.Count == 0 ||
                    !catalog.PlanetGraphs.Contains("Planet.000"))
                    throw new InvalidDataException("Data.SE.Planet graph catalog was not decoded");
                if (catalog.AsteroidGraphs.Count == 0 ||
                    !catalog.AsteroidGraphs.Contains("Asteroid.Blue00"))
                    throw new InvalidDataException("Data.SE.Asteroid graph catalog was not decoded");
                string firstReward;
                if (args.Length == 1 && (catalog.RewardNames.Count != 48 ||
                    !catalog.RewardNames.TryGetValue(0, out firstReward) ||
                    firstReward != "Орден Быка"))
                    throw new InvalidDataException("stock Reward catalog was not decoded exactly");
                if (args.Length == 1 && (catalog.PlanetGraphs.Count != 207 ||
                    catalog.AsteroidGraphs.Count != 71))
                    throw new InvalidDataException("stock planet/asteroid graph catalog count mismatch");
                string firstConstellation;
                if (args.Length == 1 && (catalog.ConstellationNames.Count < 20 ||
                    !catalog.ConstellationNames.TryGetValue(1, out firstConstellation) ||
                    firstConstellation != "Гурт"))
                    throw new InvalidDataException("stock constellation names were not decoded exactly");
                int[] expectedWeaponGroups = { 0, 1, 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 2, 1, 0, 2 };
                for (int weapon = 1; weapon <= expectedWeaponGroups.Length; weapon++)
                {
                    int actual;
                    if (!catalog.WeaponDamageGroups.TryGetValue(weapon, out actual) || actual < 0 || actual > 2 ||
                        args.Length == 1 && actual != expectedWeaponGroups[weapon - 1])
                        throw new InvalidDataException("weapon damage group mismatch for W" + weapon +
                            ": expected " + expectedWeaponGroups[weapon - 1] + ", got " + actual);
                }
                string[] actualWeaponGroups = new string[18];
                for (int weapon = 1; weapon <= actualWeaponGroups.Length; weapon++)
                    actualWeaponGroups[weapon - 1] = catalog.WeaponDamageGroups[weapon].ToString();
                Console.WriteLine("weapon_damage_groups=" + string.Join(",", actualWeaponGroups));
                foreach (string diagnostic in catalog.Diagnostics)
                    Console.WriteLine("diagnostic=" + diagnostic.Replace('\r', ' ').Replace('\n', ' '));
                if (catalog.MicroModules.Count > 0)
                {
                    MicroModuleCatalogEntry first = catalog.MicroModules[0];
                    Console.WriteLine("micro_first=" + first.Index + "|" + first.BlockName + "|" +
                        first.ReferenceId.ToString("X8") + "|" + first.Name);
                }
                if (catalog.HullSeries.Count > 0)
                {
                    HullSeriesCatalogEntry first = catalog.HullSeries[0];
                    Console.WriteLine("hull_first=" + first.Index + "|" + first.BlockName + "|" +
                        first.ReferenceId.ToString("X8") + "|" + first.Name);
                }
                return catalog.IsAvailable ? 0 : 1;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error.ToString());
                return 1;
            }
        }
    }
}
