using System;
using SpaceRangersHdSaveEditor;

internal static class CatalogReferenceSelftest
{
    private static int Main(string[] args)
    {
        if (args.Length != 2)
            throw new ArgumentException("usage: catalog-reference-selftest <game-path> <sav>");

        SavContainer save = SavContainer.Load(args[1]);
        GameDataCatalog catalog = GameDataCatalog.Load(args[0], save.GalaxyPrefix.UsedMods);
        int checkedReferences = 0;
        int shifted = 0;
        int missing = 0;
        int printed = 0;

        foreach (ItemHeaderRecord item in save.GalaxyItems)
        {
            VerifyMicro(catalog, "TItem bonus", item.ObjectId, item.Bonus,
                item.BonusReferenceId, ref checkedReferences, ref shifted, ref missing, ref printed);
            VerifyMicro(catalog, "TItem special", item.ObjectId, item.Special,
                item.SpecialReferenceId, ref checkedReferences, ref shifted, ref missing, ref printed);
            foreach (ItemExtraSpecialRecord extra in item.ExtraSpecials)
                VerifyMicro(catalog, "TItem extra", item.ObjectId, extra.Special,
                    extra.ReferenceId, ref checkedReferences, ref shifted, ref missing, ref printed);

            ItemDerivedField series = Find(item, "edSeriesNum");
            ItemDerivedField crc = Find(item, "edSeriesCRC");
            if (series != null && series.IntegerValue >= 0)
            {
                checkedReferences++;
                uint referenceId = crc == null ? 0u : checked((uint)crc.IntegerValue);
                HullSeriesCatalogEntry entry = catalog.FindHullSeries(
                    checked((int)series.IntegerValue), referenceId);
                if (entry == null || entry.Index != series.IntegerValue ||
                    entry.ReferenceId != referenceId)
                {
                    if (entry != null && entry.ReferenceId == referenceId)
                        shifted++;
                    else
                    {
                        missing++;
                        if (printed++ < 20)
                            Console.WriteLine("MISSING TItem series id={0} index={1} crc={2:X8}",
                                item.ObjectId, series.IntegerValue, referenceId);
                    }
                }
            }
        }

        foreach (MissileRecord missile in save.GalaxyMissiles)
        {
            VerifyMicro(catalog, "TMissile bonus", missile.ObjectId, missile.Bonus,
                missile.BonusReferenceId, ref checkedReferences, ref shifted, ref missing, ref printed);
            VerifyMicro(catalog, "TMissile special", missile.ObjectId, missile.Special,
                missile.SpecialReferenceId, ref checkedReferences, ref shifted, ref missing, ref printed);
        }

        Console.WriteLine("sources={0}", catalog.SourceCount);
        Console.WriteLine("micro_modules={0}", catalog.MicroModules.Count);
        Console.WriteLine("hull_series={0}", catalog.HullSeries.Count);
        Console.WriteLine("checked={0}", checkedReferences);
        Console.WriteLine("shifted_index_resolved_by_crc={0}", shifted);
        Console.WriteLine("missing_crc={0}", missing);
        foreach (string diagnostic in catalog.Diagnostics)
            Console.WriteLine("diagnostic=" + diagnostic);
        return missing == 0 ? 0 : 2;
    }

    private static void VerifyMicro(GameDataCatalog catalog, string kind, uint objectId,
        int index, uint referenceId, ref int checkedReferences, ref int shifted,
        ref int missing, ref int printed)
    {
        if (index <= 0) return;
        checkedReferences++;
        MicroModuleCatalogEntry entry = catalog.FindMicroModule(index, referenceId);
        if (entry != null && entry.ReferenceId == referenceId)
        {
            if (entry.Index != index) shifted++;
            return;
        }
        missing++;
        if (printed++ < 20)
            Console.WriteLine("MISSING {0} id={1} index={2} crc={3:X8}",
                kind, objectId, index, referenceId);
    }

    private static ItemDerivedField Find(ItemHeaderRecord item, string controlName)
    {
        if (item.DerivedFields == null) return null;
        foreach (ItemDerivedField field in item.DerivedFields)
            if (field.ControlName == controlName) return field;
        return null;
    }
}
