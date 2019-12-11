using System;
using System.Reflection;
using Localizer.Helpers;
using Noro.Access;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Localizer.DataModel.Default
{
    public class LoadedModWrapper : IMod
    {
        private readonly WeakReference<object> wrapped;

        public LoadedModWrapper(object mod)
        {
            wrapped = new WeakReference<object>(mod);
            name = mod.P("Name") as string;
            if (name == "ModLoader")
            {
                code = Assembly.GetAssembly(typeof(Main));
            }
            else
            {
                code = mod.F("assembly") as Assembly;
            }
            var buildProp = mod.F("properties").A();
            displayName = buildProp["displayName"] as string;
            version = buildProp["version"] as Version;
            file = mod.F("modFile") as TmodFile;
        }

        public string Name => name ?? "";
        private string name;

        public Assembly Code => code;
        private Assembly code;
        
        public string DisplayName => displayName ?? "";
        private string displayName;

        public Version Version => version ?? new Version();
        private Version version;

        public TmodFile File => file;
        private TmodFile file;
        
        public bool Enabled => (bool)typeof(ModLoader).M("IsEnabled", Name);
    }
}
