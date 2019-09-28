using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModBrowserMirror
{
    public class Plugin : Localizer.Plugin
    {
        public static HarmonyInstance HarmonyInstance { get; set; }
        
        public override void Initialize()
        {
            if(LanguageManager.Instance.ActiveCulture != GameCulture.Chinese)
                return;

            HarmonyInstance = HarmonyInstance.Create("ModBrowserMirror");
            
            ReplaceURLs.Patch();
        }

        protected override void OnDispose()
        {
            HarmonyInstance.UnpatchAll("ModBrowserMirror");
            HarmonyInstance = null;
        }
    }
}
