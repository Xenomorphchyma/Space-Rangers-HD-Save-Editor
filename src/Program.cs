using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: AssemblyTitle("Space Rangers HD Save Editor")]
[assembly: AssemblyDescription("Open-source save editor for Space Rangers HD")]
[assembly: AssemblyCompany("Xenomorphchyma")]
[assembly: AssemblyProduct("Space Rangers HD Save Editor")]
[assembly: AssemblyCopyright("Copyright © 2026 Xenomorphchyma")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-rc.1")]
[assembly: ComVisible(false)]

namespace SpaceRangersHdSaveEditor
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm form = new MainForm();
            if (args.Length == 1)
                form.OpenAtStartup(args[0]);
            Application.Run(form);
        }
    }
}
