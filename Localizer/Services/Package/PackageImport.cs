using System.Collections.Generic;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using Ninject;

namespace Localizer.Services.Package
{
    public class PackageImport : IPackageImportService
    {
        private readonly List<IPackageGroup> packageGroups = new List<IPackageGroup>();

        public void Queue(IPackage package)
        {
            if (packageGroups.All(pg => pg.Mod != package.Mod))
            {
                packageGroups.Add(new PackageGroup
                {
                    Mod = package.Mod,
                    Packages = new List<IPackage> { package }
                });
            }
            else
            {
                packageGroups.FirstOrDefault(pg => pg.Mod == package.Mod)?.Packages.Add(package);
            }

            Utils.LogDebug($"Queued [{package.Name}]");
        }

        public void Import(bool early = false)
        {
            Utils.LogDebug($"Begin importing");

            foreach (var group in packageGroups)
            {
                Utils.SafeWrap(() =>
                {
                    var merged = Merge(group);
                    foreach (dynamic f in merged.Files)
                    {
                        var s = GetImportService(f);
                        Utils.LogDebug($"Importing [{f.GetType()}] using [{s.GetType()}]");
                        s.Import(f, group.Mod, group.Packages.ToList()[0].Language);
                        Utils.LogDebug($"Imported");
                    }
                });
            }
            
            Utils.LogDebug($"Imported");
        }

        public void Reset()
        {
            foreach (dynamic s in Localizer.Kernel.GetAll(typeof(IFileImportService<>)))
            {
                s.Reset();
            }
            packageGroups.Clear();
        }

        private static IFileImportService<T> GetImportService<T>(T file) where T : IFile
        {
            return Localizer.Kernel.Get<IFileImportService<T>>();
        }

        internal static DataModel.Default.Package Merge(IPackageGroup group)
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
                Utils.SafeWrap(() =>
                {
                    foreach (dynamic file in package.Files)
                    {
                        if (result.Files.All(f => f.GetType() != file.GetType()))
                        {
                            result.Files.Add(file);
                        }
                        else
                        {
                            var main = package.Files.FirstOrDefault(f => f.GetType() == file.GetType());
                            var s = GetImportService(file);
                            var merged = s.Merge(main, file);
                            result.Files.Remove(main);
                            result.Files.Add(merged);
                        }
                    }
                });
            }

            return result;
        }

        public void Dispose()
        {
        }
    }
}
