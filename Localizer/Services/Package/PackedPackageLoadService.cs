using System.IO;
using System.IO.Compression;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Package;
using Localizer.Services.File;
using Terraria.ModLoader;

namespace Localizer.Services.Package
{
    public sealed class PackedPackageLoadService<T> : IPackageLoadService<T> where T : IPackage
    {
        private readonly string _packageMainFileName = "Package.json";

        public IPackage Load(string path, IFileLoadService fileLoadService)
        {
            Utils.LogDebug($"Loading package from {path}");

            if (!System.IO.File.Exists(path))
            {
                Utils.LogError("Package file doesn't exist!");
                return null;
            }

            using (var zipFileToOpen = new FileStream(path, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Read))
                {
                    var packageStream = archive.GetEntry(_packageMainFileName)?.Open();

                    IPackage package = Utils.ReadFileAndDeserializeJson<T>(packageStream);
                    if (package == null)
                    {
                        Utils.LogError("Package main file deserialization failed!");
                        return null;
                    }

                    var tmod = ModLoader.Mods.FirstOrDefault(m => m.Name == package.ModName);
                    if (tmod == null)
                    {
                        return null;
                    }

                    package.Mod = new ModWrapper(tmod);

                    foreach (var fileTypeName in package.FileList)
                    {
                        Utils.LogDebug($"Loading file [{fileTypeName}]");

                        var fs = archive.GetEntry($"{fileTypeName}.json")?.Open();
                        var file = fileLoadService.Load(fs, fileTypeName);

                        package.AddFile(file);
                    }

                    Utils.LogDebug($"Package [{package.Name} loaded.]");

                    return package;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
