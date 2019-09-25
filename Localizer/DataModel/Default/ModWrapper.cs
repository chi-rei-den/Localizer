using System;
using System.Reflection;
using Terraria.ModLoader;

namespace Localizer.DataModel.Default
{
    public class ModWrapper : IMod
    {
        private readonly Mod wrapped;

        public ModWrapper(Mod mod)
        {
            wrapped = mod;
        }

        public string Name => wrapped?.Name ?? "";

        public Assembly Code => wrapped?.Code;
        public string DisplayName => wrapped.DisplayName ?? "";
        public Version Version => wrapped?.Version;
    }
}
