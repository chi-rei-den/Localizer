using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Localizer
{
    public class Localizer : Mod
    {
        public override string Name => Assembly.GetExecutingAssembly().GetName().Name;
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public override void Load()
        {

        }
    }
}
