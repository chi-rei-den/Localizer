using Localizer.DataExport;
using Localizer.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ModLoader;
using File = System.IO.File;

namespace Localizer
{
    public class PackageManager
    {
        public static readonly string PackageFileName = "Package.json";

        public static string SavePath => Localizer.SavePath;
        public static string PackagePath => SavePath + "Packages/";
        public static string ExportPath => SavePath + "Exported/";
        
        public static string PackageGroupStatesPath => SavePath + "PackageStates.json";

        public static List<PackageGroup> PackageGroups { get; set; }
        
        public static bool Loading { get; private set; }
        
        public static bool Importing { get; private set; }

        public static Dictionary<Type, Batcher> Batchers = new Dictionary<Type, Batcher>
        {
            [typeof(BasicItemFile)] = BasicItemBatcher.Instance,
            [typeof(BasicBuffFile)] = BasicBuffBatcher.Instance,
            [typeof(BasicCustomFile)] = BasicCustomBatcher.Instance,
            [typeof(BasicNPCFile)] = BasicNPCBatcher.Instance,
            [typeof(LdstrFile)] = LdstrBatcher.Instance,
        };

        public static List<Type> FileTypes = new List<Type>()
        {
            typeof(BasicItemFile),
            typeof(BasicBuffFile),
            typeof(BasicCustomFile),
            typeof(BasicNPCFile),
            typeof(LdstrFile),
        };

        public static void Init()
        {
            PackageGroups = new List<PackageGroup>();
        }

        public static void Unload()
        {
            foreach (var batcher in Batchers)
            {
                batcher.Value.Reset();
                batcher.Value.Revert();
            }
            
            SavePackageGroupStates();
        }

        public static void Revert()
        {
            foreach (var batcher in Batchers)
            {
                batcher.Value.Revert();
            }
        }

        public static void LoadPackages(bool reload=true)
        {
            Loading = true;
            try
            {
                PackageGroups = new List<PackageGroup>();

                LoadZippedPackage();
                LoadExportedPackages();
                
                LoadPackageGroupStates();

                if (Localizer.Config.AutoImport && !reload)
                {
                    ImportAll();
                }
            }
            finally
            {
                Loading = false;
            }
        }

        public static void SavePackageGroupStates()
        {
            try
            {
                var states = new List<PackageGroupState>();

                foreach (var pg in PackageGroups)
                {
                    states.Add(new PackageGroupState(pg));
                }
                
                Utils.SerializeJsonAndCreateFile(states, PackageGroupStatesPath);
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        public static void LoadPackageGroupStates()
        {
            if(!File.Exists(PackageGroupStatesPath))
                return;

            try
            {
                var states = Utils.ReadFileAndDeserializeJson<List<PackageGroupState>>(PackageGroupStatesPath);

                foreach (var state in states)
                {
                    var packageGroup = PackageGroups.FirstOrDefault(pg => pg.Mod.Name == state.ModName);
                    if (packageGroup == null)
                        continue;

                    foreach (var p in packageGroup.Packages)
                    {
                        if (state.Packages.Contains(p.Name))
                        {
                            p.Enabled = (bool) state.Packages[p.Name];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }
        
        public static void LoadZippedPackage()
        {
            if (!Directory.Exists(PackagePath))
            {
                Directory.CreateDirectory(PackagePath);
            }

            var packs = new DirectoryInfo(PackagePath).GetFiles("*.locpack", SearchOption.TopDirectoryOnly);

            foreach (var pack in packs)
            {
                TryLoadZippedPackage(pack);
            }
        }
        
        public static void LoadExportedPackages()
        {
            if (!Directory.Exists(ExportPath))
            {
                Directory.CreateDirectory(ExportPath);
            }

            var packDirs = new DirectoryInfo(ExportPath).GetDirectories();

            foreach (var packDir in packDirs)
            {
                TryLoadExportedPackage(packDir);
            }
        }

        public static void TryLoadZippedPackage(FileInfo packFile)
        {
            try
            {
                var packageFilePath = packFile.FullName;

                if (!File.Exists(packageFilePath))
                {
                    return;
                }

                using (FileStream zipFileToOpen = new FileStream(packageFilePath, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Read))
                    {
                        var packageStream = archive.GetEntry("Package.json")?.Open();
                        
                        var package = Utils.ReadFileAndDeserializeJson<Package>(packageStream);
                        package.Init();
                        if(package.Mod == null)
                            return;

                        foreach (var filename in package.FileList)
                        {
                            var fileStream = archive.GetEntry($"{filename}.json")?.Open();
                            var file = Utils.ReadFileAndDeserializeJson(GetFileTypeByTypeName(filename), fileStream) as DataModel.File;

                            package.AddFile(file);
                        }

                        AddPackage(package);
                    }
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        public static void TryLoadExportedPackage(DirectoryInfo packDir)
        {
            try
            {
                var packageFilePath = Path.Combine(packDir.FullName, PackageFileName);

                if (!File.Exists(packageFilePath))
                {
                    return;
                }

                var package = Utils.ReadFileAndDeserializeJson<Package>(packageFilePath);
                package.Init();
                if(package.Mod == null)
                    return;

                foreach (var filename in package.FileList)
                {
                    var filePath = Path.Combine(packDir.FullName, filename + ".json");
                    var file = ReadFile(filePath, filename);

                    package.AddFile(file);
                }

                AddPackage(package);
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        public static void ZipPackage(DirectoryInfo packDir)
        {
            try
            {
                var packageFilePath = Path.Combine(packDir.FullName, PackageFileName);

                if (!File.Exists(packageFilePath))
                {
                    return;
                }

                var package = Utils.ReadFileAndDeserializeJson<Package>(packageFilePath);
                using (FileStream zipFileToOpen = new FileStream($"{packDir.FullName}/{package.Name}.locpack", FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))
                    {
                        Utils.WriteZipArchiveEntry(archive, "Package.json");
                        
                        foreach (var filename in package.FileList)
                        {
                            var filePath = Path.Combine(packDir.FullName, filename + ".json");           
                            if (!File.Exists(filePath))
                                continue;
                            Utils.WriteZipArchiveEntry(archive, filename + ".json");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        public static DataModel.File ReadFile(string path, string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
            {
                return null;
            }

            return ReadFile(path, GetFileTypeByTypeName(fileType));
        }

        public static DataModel.File ReadFile(string path, Type fileType)
        {
            if (File.Exists(path))
            {
                var file = Utils.ReadFileAndDeserializeJson(fileType, path) as DataModel.File;

                return file;
            }

            return null;
        }

        public static void Export(Mod mod, string packName, CultureInfo lang, Dictionary<Type, ExportConfig> toExports)
        {
            try
            {
                var package = new Package(packName, mod, lang);

                foreach (var pair in toExports)
                {
                    pair.Value.Package = package;
                    var exporter = ExporterFactory.CreateExporter(pair.Key, pair.Value);
                    var file = exporter?.Export();
                    if (file == null)
                    {
                        continue;
                    }

                    package.AddFile(file);
                    package.FileList.Add(pair.Key.Name);
                }

                AddPackage(package);

                Utils.SerializeJsonAndCreateFile(package, Path.Combine(ExportPath, package.Name, "Package.json"));
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        /// <summary>
        /// Import all enabled packages.
        /// </summary>
        public static void ImportAll()
        {
            foreach (var pg in PackageGroups)
            {
                Import(pg.Mod);
            }
        }
        
        /// <summary>
        /// Import a mod's enabled packages.
        /// </summary>
        /// <param name="mod"></param>
        public static void Import(Mod mod)
        {
            Importing = true;
            try
            {
                var languages = FindLanguages(mod);

                foreach (var lang in languages)
                {
                    DoBatch(mod, lang);
                    Localizer.RefreshLanguages(lang);
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
            finally
            {
                Importing = false;
            }
        }

        /// <summary>
        /// Add a package into PackageGroups.
        /// </summary>
        /// <param name="package"></param>
        public static void AddPackage(Package package)
        {
            if (!PackageGroups.Exists(pg => pg.Mod == package.Mod))
            {
                PackageGroups.Add(new PackageGroup()
                {
                    Mod = package.Mod,
                    Packages = new List<Package>() {package}
                });
            }
            else
            {
                PackageGroups.FirstOrDefault(pg => pg.Mod == package.Mod)?.Packages.Add(package);
            }
        }

        private static Type GetFileTypeByTypeName(string typeName)
        {
            foreach (var type in FileTypes)
            {
                if (type.Name == typeName)
                {
                    return type;
                }
            }

            return null;
        }

        private static void DoBatch(Mod mod, CultureInfo language)
        {
            var packages = PackageGroups.FirstOrDefault(pg => pg.Mod == mod)?.Packages;
            if (packages == null)
                return;

            foreach (var package in packages.FindAll(p => p.Language == language))
            {
                foreach (var file in package.Files)
                {
                    if (Batchers.TryGetValue(file.GetType(), out var batcher))
                    {
                        batcher.Add(file);
                    }
                }
            }

            foreach (var batcher in Batchers.Values)
            {
                batcher.SetState(mod, language);

                batcher.Batch();

                batcher.Reset();
            }

        }

        private static List<CultureInfo> FindLanguages(Mod mod)
        {
            var results = new List<CultureInfo>();
            
            var packages = PackageGroups.FirstOrDefault(pg => pg.Mod == mod)?.Packages;
            if (packages == null)
                return results;

            foreach (var package in packages)
            {
                if (!results.Contains(package.Language))
                {
                    results.Add(package.Language);
                }
            }

            return results;
        }
    }
}
