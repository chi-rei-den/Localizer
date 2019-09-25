using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Localizer;
using Terraria.ModLoader;

namespace ModBrowserMirror
{	
    public class ReplaceURLs
    {
        public static void Patch()
        {
            var original = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.ModBrowser.UIModBrowser").GetMethod("<PopulateModBrowser>b__70_0", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            var transpiler = typeof(ReplaceURLs).GetMethod("PopulateModBrowserTranspiler", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            Plugin.HarmonyInstance.Patch(original, null, null, new HarmonyMethod(transpiler));

            var original1 = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem").GetMethod("FromJson", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            var transpiler1 = typeof(ReplaceURLs).GetMethod("FromJSONTranspiler", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            Plugin.HarmonyInstance.Patch(original1, null, null, new HarmonyMethod(transpiler1));

            var original2 = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.UIModInfo").GetMethod("<OnActivate>b__25_0", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            var transpiler2 = typeof(ReplaceURLs).GetMethod("OnActivateTranspiler", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

            Plugin.HarmonyInstance.Patch(original2, null, null, new HarmonyMethod(transpiler2));
        }
        
        static IEnumerable<CodeInstruction> PopulateModBrowserTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/listmods.php", "https://trbbs.cc/trmod/listmods.php", result);

            return result;
        }
        
        static IEnumerable<CodeInstruction> FromJSONTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://trbbs.cc/trmod/", result);

            return result;
        }
        
        static IEnumerable<CodeInstruction> OnActivateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            var ins = result.FirstOrDefault(i => i?.operand?.ToString() == "http://javid.ddns.net/tModLoader/moddescription.php");
            if (ins != null)
            {
                var concat = typeof(string).Module.FindMethod(
                    "System.String System.String::Concat(System.String,System.String,System.String)");
                var thisModName = typeof(Mod)
                                  .Module.GetTypes()
                                  .FirstOrDefault(t => t.FullName == "Terraria.ModLoader.UI.UIModInfo")
                                  .GetField("_modName",
                                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                            BindingFlags.Static);

                ins.operand = "https://trbbs.cc/trmod/";
                var index = result.IndexOf(ins) + 1;
                result.Insert(index, new CodeInstruction(OpCodes.Call, concat));
                result.Insert(index, new CodeInstruction(OpCodes.Ldstr, ".desc"));
                result.Insert(index, new CodeInstruction(OpCodes.Ldfld, thisModName));
                result.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
            }

            return result;
        }
        
        private static void ReplaceLdstr(string o, string n, IEnumerable<CodeInstruction> il)
        {
            var ins = il.FirstOrDefault(i => i?.operand?.ToString() == o);
            if (ins != null)
            {
                ins.operand = n;
            }
        }
    }
}
