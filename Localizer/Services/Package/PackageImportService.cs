using System.Collections.Generic;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using Ninject;

namespace Localizer.Services.Package
{
    public class PackageImportService : IPackageImportService
    {
        private readonly List<IPackageGroup> packageGroups = new List<IPackageGroup>();

        public void Queue(IPackage package)
        {
            if (packageGroups.All(pg => pg.Mod != package.Mod))
            {
                packageGroups.Add(new PackageGroup
                {
                    Mod = package.Mod,
                    Packages = new List<IPackage> {package}
                });
            }
            else
            {
                packageGroups.FirstOrDefault(pg => pg.Mod == package.Mod)?.Packages.Add(package);
            }
            
            Utils.LogDebug($"Queued [{package.Name}]");
        }

        public void Import()
        {
            Utils.LogDebug($"Begin importing");
            var services = Localizer.Kernel.GetAll<IFileImportService>();

            foreach (var group in packageGroups)
            {
                var merged = Merge(group, services);
                foreach (var f in merged.Files)
                {
                    foreach (var s in services)
                    {
                        Utils.LogDebug($"Importing [{f.GetType()}] using [{s.GetType()}]");
                        s.Import(f, group.Mod);
                        Utils.LogDebug($"Imported");
                    }
                }
            }
            Utils.LogDebug($"Imported");
        }

        public void Reset()
        {
            packageGroups.Clear();
        }

        private static DataModel.Default.Package Merge(IPackageGroup group, IEnumerable<IFileImportService> services)
        {
            var result = new DataModel.Default.Package
            {
                Mod = group.Mod,
                ModName = group.Mod.Name,
                Files = new List<IFile>(),
                FileList = new List<string>()
            };
            foreach (var package in group.Packages)
            {
                foreach (var file in package.Files)
                {
                    if (result.Files.All(f => f.GetType() != file.GetType()))
                    {
                        result.Files.Add(file);
                    }
                    else
                    {
                        var main = package.Files.FirstOrDefault(f => f.GetType() == file.GetType());
                        foreach (var service in services)
                        {
                            var merged = service.Merge(main, file);
                            result.Files.Remove(main);
                            result.Files.Add(merged);
                        }
                    }
                }
            }

            return result;
        }
    }
}
