using System;
using System.Collections.Generic;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Package
{
    public class PackageManageService : IPackageManageService
    {
        private readonly string stateSavePath = Localizer.SavePath + "PackageStates.json";

        public ICollection<IPackageGroup> PackageGroups { get; set; }

        private List<PackageGroupState> oldPackageGroupStates = new List<PackageGroupState>();

        /// <summary>
        ///     Add a package into PackageGroups.
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(IPackage package)
        {
            if (package == null)
            {
                return;
            }

            if (PackageGroups.All(pg => pg.Mod.Name != package.Mod.Name))
            {
                PackageGroups.Add(new PackageGroup
                {
                    Mod = package.Mod,
                    Packages = new List<IPackage> { package }
                });
            }
            else
            {
                PackageGroups.FirstOrDefault(
                    pg => pg.Mod.Name == package.Mod.Name &&
                          pg.Packages.All(p => p.Name != package.Name))?.Packages.Add(package);
            }
        }

        public void RemovePackage(IPackage package)
        {
            throw new NotImplementedException();
        }

        public void LoadState()
        {
            if (!System.IO.File.Exists(stateSavePath))
            {
                return;
            }

            oldPackageGroupStates = Utils.ReadFileAndDeserializeJson<List<PackageGroupState>>(stateSavePath) ??
                         new List<PackageGroupState>();

            foreach (var state in oldPackageGroupStates)
            {
                var packageGroup = PackageGroups.FirstOrDefault(pg => pg.Mod.Name == state.ModName);
                if (packageGroup == null)
                {
                    continue;
                }

                foreach (var p in packageGroup.Packages)
                {
                    if (state.Packages.Contains(p.Name))
                    {
                        p.Enabled = (bool)state.Packages[p.Name];
                    }
                }
            }
        }

        public void SaveState()
        {
            foreach (var pg in PackageGroups)
            {
                var state = oldPackageGroupStates.FirstOrDefault(s => s.ModName == pg.Mod.Name);
                if (state != null)
                {
                    oldPackageGroupStates.Remove(state);
                }

                oldPackageGroupStates.Add(new PackageGroupState(pg));
            }

            Utils.SerializeJsonAndCreateFile(oldPackageGroupStates, stateSavePath);
        }

        public void Dispose()
        {
        }
    }
}
