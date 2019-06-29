using Terraria.ModLoader;
using Harmony;
using System;

namespace Localizer
{
    class yuyutql : Mod
    {
        private static HarmonyInstance harmony = HarmonyInstance.Create("yuyu");
        public static Mod instance;
        public yuyutql()
        {
            instance = this;
            harmony.PatchAll();
        }
    }
}