using System;
using System.Collections.Generic;
using SpaceRangersHdSaveEditor;

internal static class HullInterceptorRoundtrip
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("usage: hull-interceptor-roundtrip input.sav output.sav");
            SavContainer source = SavContainer.Load(args[0]);
            List<ItemHeaderRecord> items = new List<ItemHeaderRecord>();
            foreach (ItemHeaderRecord item in source.GalaxyItems) items.Add(item.Clone());
            ItemHeaderRecord selected = null;
            foreach (ItemHeaderRecord item in items)
                if (item.Type == 42 && item.HasDerivedTail)
                { selected = item; break; }
            if (selected == null) throw new InvalidOperationException("no THull fixture found");
            bool expected = Toggle(selected, source.GalaxyShips[0].ObjectId);
            source.WriteCopy(args[1], source.Metadata.Clone(), null, null, null, null, null, items);
            SavContainer patched = SavContainer.Load(args[1]);
            ItemHeaderRecord actual = null;
            foreach (ItemHeaderRecord item in patched.GalaxyItems)
                if (item.Type == selected.Type && item.ObjectId == selected.ObjectId)
                { actual = item; break; }
            if (actual == null || !actual.ContentEquals(selected))
                throw new InvalidOperationException("structurally toggled THull did not round-trip");
            ItemDerivedField flag = Find(actual, "$HullHasInterceptors");
            bool hasOptional = Find(actual, "cbInterceptorsNextTarget") != null &&
                Find(actual, "cbInterceptorsStrategy") != null &&
                Find(actual, "edInterceptorsDuration") != null;
            if (flag == null || (flag.IntegerValue != 0) != expected || hasOptional != expected)
                throw new InvalidOperationException("THull conditional field layout is inconsistent");
            Console.WriteLine("THull interceptor round-trip: ID={0}, enabled={1}", actual.ObjectId, expected);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static bool Toggle(ItemHeaderRecord item, uint targetShipId)
    {
        ItemDerivedField flag = Find(item, "$HullHasInterceptors");
        ItemDerivedField energyMax = Find(item, "edEnergyMax");
        if (flag == null || energyMax == null) throw new InvalidOperationException("THull core fields missing");
        item.DerivedFields.RemoveAll(delegate(ItemDerivedField field)
        {
            return field.ControlName == "cbInterceptorsNextTarget" ||
                field.ControlName == "cbInterceptorsStrategy" ||
                field.ControlName == "edInterceptorsDuration";
        });
        bool enabled = flag.IntegerValue == 0;
        flag.IntegerValue = enabled ? 1 : 0;
        if (!enabled) return false;
        int insertion = item.DerivedFields.IndexOf(energyMax) + 1;
        item.DerivedFields.Insert(insertion++, Field("cbInterceptorsNextTarget", ItemDerivedField.UInt32, targetShipId));
        item.DerivedFields.Insert(insertion++, Field("cbInterceptorsStrategy", ItemDerivedField.Byte, 1));
        item.DerivedFields.Insert(insertion, Field("edInterceptorsDuration", ItemDerivedField.Byte, 2));
        return true;
    }

    private static ItemDerivedField Find(ItemHeaderRecord item, string name)
    {
        foreach (ItemDerivedField field in item.DerivedFields)
            if (field.ControlName == name) return field;
        return null;
    }

    private static ItemDerivedField Field(string name, byte kind, long value)
    {
        ItemDerivedField field = new ItemDerivedField();
        field.ControlName = name; field.Kind = kind; field.IntegerValue = value;
        return field;
    }
}
