using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Localizer.DataModel.Default
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PackageGroupState
    {
        public PackageGroupState()
        { }

        public PackageGroupState(IPackageGroup pg)
        {
            ModName = pg.Mod.Name;
            Packages = new OrderedDictionary();
            foreach (var p in pg.Packages)
            {
                Packages.Add(p.Name, p.Enabled);
            }
        }

        public string ModName { get; set; }

        public OrderedDictionary Packages { get; set; }
    }
}
