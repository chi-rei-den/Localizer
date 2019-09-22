using System.Collections.Generic;

namespace Localizer.DataModel
{
    public class PackageGroup : IPackageGroup
    {
        public IMod Mod { get; set; }

        public ICollection<IPackage> Packages { get; set; }
    }
}
