using System.IO;
using System.IO.Compression;
using Localizer.DataModel;

namespace Localizer.Services.Package
{
    public class ZipPackagePack<T> : IPackagePackService where T : IPackage
    {
        private readonly string _packageMainFileName = "Package.json";

        public void Pack(string path)
        {
            Utils.LogDebug($"Packing package from {path}");

            if (!System.IO.File.Exists(path))
            {
                Utils.LogError("Cannot find the package file!");
                return;
            }

            var packDir = new FileInfo(path).Directory;

            var package = Utils.ReadFileAndDeserializeJson<T>(path);
            using (var zipFileToOpen = new FileStream($"{packDir.FullName}/{package.Name}.locpack", FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))
                {
                    Utils.WriteZipArchiveEntry(archive, path, _packageMainFileName);

                    foreach (var filename in package.FileList)
                    {
                        Utils.LogDebug($"Writing {filename}");

                        var filePath = Path.Combine(packDir.FullName, filename + ".json");

                        if (!System.IO.File.Exists(filePath))
                        {
                            Utils.LogError($"Cannot find file: {filePath}!");
                            continue;
                        }

                        Utils.WriteZipArchiveEntry(archive, filePath, filename + ".json");
                    }
                }
            }

            Utils.LogDebug("Packed");
        }

        public void Dispose()
        {
        }
    }
}
