using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace NitroxLinuxLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1) {
                Console.WriteLine("Path to Subnautica game directory must be provided as an argument.");
                return;
            }
            if (!Directory.Exists(args[0]) || !File.Exists(Path.Combine(args[0], "Subnautica.exe"))) {
                Console.WriteLine("Invalid path provided.");
                return;
            }
            string subnauticaPath = args[0];

            // Store path where launcher is in game folder for Nitrox bootstrapper to read
            string nitroxAppData = Path.Combine(subnauticaPath, "Nitrox");
            Directory.CreateDirectory(nitroxAppData);
            File.WriteAllText(Path.Combine(nitroxAppData, "launcherpath.txt"), Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            // TODO: The launcher should override FileRead win32 API for the Subnautica process to give it the modified Assembly-CSharp from memory 
            string bootloaderName = "Nitrox.Bootloader.dll";
            try
            {
                File.Copy(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", bootloaderName), Path.Combine(subnauticaPath, "Subnautica_Data", "Managed", bootloaderName), true);
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to move bootloader dll to Managed folder. Still attempting to launch because it might exist from previous runs.");
            }

            NitroxEntryPatch patch = new NitroxEntryPatch(subnauticaPath);
            patch.Remove();
            patch.Apply();
            if (patch.IsApplied) {
                Console.WriteLine("Patch applied.");
                Process.Start("/usr/bin/steam", "steam://run/264710");
            } else {
                Console.WriteLine("Patch not applied.");
            }
        }
    }
}
