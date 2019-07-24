using System.Collections.Generic;
using Terraria.ModLoader;

namespace Localizer.DataModel
{
    public class PackageGroup
    {
        public Mod Mod { get; set; }
        
        public List<Package> Packages { get; set; }
    }
}