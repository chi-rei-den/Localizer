using System;
using System.Reflection;
using Localizer.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Localizer.DataModel.Default
{
    public class ModWrapper : IMod
    {
        private readonly WeakReference<Mod> wrapped;

        public ModWrapper(Mod mod)
        {
            if(mod is null)
                throw new ArgumentNullException(nameof(mod));
            wrapped = new WeakReference<Mod>(mod);
        }

        public Mod Mod
        {
            get
            {
                Mod mod = null;
                wrapped?.TryGetTarget(out mod);
                return mod;
            }
        }

        public string Name => Mod?.Name ?? "";

        public Assembly Code
        {
            get
            {
                
                if (Name == "ModLoader")
                {
                    return Assembly.GetAssembly(typeof(Main));
                }
                else
                {
                    return Mod?.Code;
                }
            }
        }

        public string DisplayName => Mod.DisplayName ?? "";
        public Version Version => Mod?.Version;
        public TmodFile File => Mod?.Prop("File") as TmodFile;
        public bool Enabled => (bool)typeof(ModLoader).Method("IsEnabled", Name);
    }
}
