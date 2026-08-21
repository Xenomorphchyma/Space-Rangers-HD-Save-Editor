using System;
using System.Drawing;
using System.IO;

namespace SpaceRangersHdSaveEditor
{
    internal static class IconGenerator
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1) return 2;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[0])));
            using (Icon icon = EditorAssets.AppIcon())
            using (FileStream stream = File.Create(args[0]))
                icon.Save(stream);
            return 0;
        }
    }
}
