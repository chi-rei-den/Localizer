using System;
using System.Reflection;
using Localizer.Helpers;
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
            name = mod.Prop("Name") as string;
            if (name == "ModLoader")
            {
                code = Assembly.GetAssembly(typeof(Main));
            }
            else
            {
                code = mod.Field("assembly") as Assembly;
            }
            var buildProp = mod.Field("properties");
            displayName = buildProp.Field("displayName") as string;
            version = buildProp.Field("version") as Version;
            file = mod.Field("modFile") as TmodFile;
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
        
        public bool Enabled => (bool)typeof(ModLoader).Method("IsEnabled", Name);
    }
}
