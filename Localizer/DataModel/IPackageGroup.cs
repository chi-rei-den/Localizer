using System.Collections.Generic;

namespace Localizer.DataModel
{
    public interface IPackageGroup
    {
        /// <summary>
        ///     Mod of the group, every package in this group is for this mod.
        /// </summary>
        IMod Mod { get; set; }

        ICollection<IPackage> Packages { get; set; }
    }
}
