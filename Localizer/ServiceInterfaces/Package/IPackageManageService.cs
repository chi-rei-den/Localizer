using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.Services;

namespace Localizer
{
    public interface IPackageManageService : IService
    {
        /// <summary>
        ///     All added packages sorted by mod.
        /// </summary>
        ICollection<IPackageGroup> PackageGroups { get; set; }

        /// <summary>
        ///     Add a package.
        /// </summary>
        /// <param name="package"></param>
        void AddPackage(IPackage package);

        /// <summary>
        ///     Remove a package.
        /// </summary>
        /// <param name="package"></param>
        void RemovePackage(IPackage package);

        /// <summary>
        ///     Load saved package state.
        /// </summary>
        void LoadState();

        /// <summary>
        ///     Save the package state.
        /// </summary>
        void SaveState();
    }
}
