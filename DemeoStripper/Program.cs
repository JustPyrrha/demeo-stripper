using System;
using System.IO;
using System.Linq;

namespace DemeoStripper
{
    internal static class Program
    {
        internal static string InstallDirectory;

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && args[0] != null)
                {
                    InstallDirectory = Path.GetDirectoryName(args[0]);
                    if (!File.Exists(Path.Combine(InstallDirectory, InstallDir.DemeoExe)))
                    {
                        throw new Exception(Path.Combine(InstallDirectory, InstallDir.DemeoExe));
                    }
                }
                else
                {
                    Logger.Log("Resolving Demeo install directory");
                    InstallDirectory = InstallDir.GetInstallDir();
                    if (InstallDirectory == null)
                    {
                        throw new Exception();
                    }
                }

                var monoDir = Path.Combine(InstallDirectory, @"MelonLoader\Dependencies\MonoBleedingEdge.x64");
                var supportDir = Path.Combine(InstallDirectory, @"MelonLoader\Dependencies\SupportModules");
                var managedDir = Path.Combine(InstallDirectory, @"demeo_Data\Managed");

                Logger.Log("Resolving Beat Saber version");
                var version = VersionFinder.FindVersion(InstallDirectory);

                var outDir = Path.Combine(Directory.GetCurrentDirectory(), "stripped", version);
                Logger.Log("Creating output directory");
                Directory.CreateDirectory(outDir);

                var whitelist = new[]
                {
                    "IPA.",
                    "TextMeshPro",
                    "UnityEngine.",
                    "Assembly-CSharp",
                    "0Harmony",
                    "Newtonsoft.Json",
                    "MainAssembly",
                    "Cinemachine",
                    "DynamicBone",
                    "FinalIK",
                    "OculusPlatform",
                    "HMLib",
                    "HMUI",
                    "VRUI",
                };

                foreach (var f in ResolveDLLs(managedDir, whitelist))
                {
                    StripDLL(f, outDir, managedDir, monoDir, supportDir);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        private static string[] ResolveDLLs(string managedDir, string[] whitelist)
        {
            var files = Directory.GetFiles(managedDir).Where(path =>
            {
                FileInfo info = new FileInfo(path);
                if (info.Extension != ".dll") return false;

                foreach (string substr in whitelist)
                {
                    if (info.Name.Contains(substr)) return true;
                }

                return false;
            });

            return files.ToArray();
        }

        internal static void StripDLL(string f, string outDir, params string[] resolverDirs)
        {
            if (File.Exists(f) == false) return;
            var file = new FileInfo(f);
            Logger.Log($"Stripping {file.Name}");

            var mod = ModuleProcessor.Load(file.FullName, resolverDirs);
            mod.Virtualize();
            mod.Strip();

            string outFile = Path.Combine(outDir, file.Name);
            mod.Write(outFile);
        }
    }
}
