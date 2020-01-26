using System;
using System.Reflection;
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
            name = mod.ValueOf<string>("Name");
            code = name == "ModLoader" ? Assembly.GetAssembly(typeof(Main)) : mod.ValueOf<Assembly>("assembly");
            var buildProp = mod.ValueOf("properties");
            displayName = buildProp.ValueOf<string>("displayName");
            version = buildProp.ValueOf<Version>("version");
            file = mod.ValueOf<TmodFile>("modFile");
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

        public bool Enabled => (bool)typeof(ModLoader).Invoke("IsEnabled", Name);
    }
}
