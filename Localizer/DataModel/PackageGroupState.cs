using System.Collections.Generic;
using System.Collections.Specialized;

namespace Localizer.DataModel
{
    public class PackageGroupState
    {
        public string ModName { get; set; }

        public OrderedDictionary Packages { get; set; }
        
        public PackageGroupState() { }

        public PackageGroupState(PackageGroup pg)
        {
            ModName = pg.Mod.Name;
            Packages = new OrderedDictionary();
            for (int i = 0; i < pg.Packages.Count; i++)
            {
                Packages.Add(pg.Packages[i].Name, pg.Packages[i].Enabled);
            }
        }
    }
}