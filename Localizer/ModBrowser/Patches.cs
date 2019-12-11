using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using Noro;
using Terraria.ModLoader;

namespace Localizer.ModBrowser
{
    public class Patches
    {
        public static void Patch()
        {
            Utils.LogInfo($"Patching ModBrowser, tML version: {ModLoader.version}");
            
            Patch_PopulateModBrowser();
            Patch_FromJson();
            Patch_OnActivate();
            Patch_UIModDownloadItemCtor();
            
            Utils.LogInfo("ModBrowser Patched");
        }

        private static void Patch_PopulateModBrowser()
        {
            var populateModBrowser = Utils.TR().GetType("Terraria.ModLoader.UI.ModBrowser.UIModBrowser")
                                          .AllMethods()
                                          .FirstOrDefault(m => m.Name.Contains("<PopulateModBrowser>"));
            
            Localizer.Harmony.Transpile<Patches>(nameof(PopulateModBrowserTranspiler))
                     .Detour(populateModBrowser);
        }

        private static void Patch_FromJson()
        {            
            var fromJson = Utils.TR().GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem")
                                .FindMethod("FromJson");
            
            Localizer.Harmony.Transpile<Patches>(nameof(FromJSONTranspiler))
                     .Detour(fromJson);
        }

        private static void Patch_OnActivate()
        {
            var onActivate = Utils.TR().GetType("Terraria.ModLoader.UI.UIModInfo")
                                  .AllMethods()
                                  .FirstOrDefault(m => m.Name.Contains("<OnActivate>"));
            
            Localizer.Harmony.Transpile<Patches>(nameof(OnActivateTranspiler))
                     .Detour(onActivate);

        }

        private static void Patch_UIModDownloadItemCtor()
        {
            var uiModDownloadItemCtor = Utils.TR().GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem")
                                             .AllDeclaredConstructors()[0];
            
            Localizer.Harmony.Transpile<Patches>(nameof(ModIconTranspiler))
                     .Detour(uiModDownloadItemCtor);
        }
        
        private static IEnumerable<CodeInstruction> PopulateModBrowserTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/listmods.php", "https://trbbs.cc/trmod/listmods.php", result);

            return result;
        }

        private static IEnumerable<CodeInstruction> FromJSONTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://trbbs.cc/trmod/", result);
            ReplaceLdstr("&tls12=y", "", result);

            return result;
        }

        private static IEnumerable<CodeInstruction> OnActivateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://trbbs.cc/trmod/", result);
            var ins = result.FirstOrDefault(i => i?.operand?.ToString() == "http://javid.ddns.net/tModLoader/moddescription.php");
            if (ins != null)
            {
                var concat = typeof(string).FindMethod("Concat", typeof(string), typeof(string), typeof(string));
                var thisModName = Utils.TR().GetType("Terraria.ModLoader.UI.UIModInfo")
                                  .FindField("_modName");

                ins.operand = "https://trbbs.cc/trmod/";
                var index = result.IndexOf(ins) + 1;
                result.Insert(index, new CodeInstruction(OpCodes.Call, concat));
                result.Insert(index, new CodeInstruction(OpCodes.Ldstr, ".desc"));
                result.Insert(index, new CodeInstruction(OpCodes.Ldfld, thisModName));
                result.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
            }

            return result;
        }

        private static IEnumerable<CodeInstruction> ModIconTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://trbbs.cc/trmod/", result);
            var ins = result.FirstOrDefault(i => i?.operand?.ToString()?.Contains("_modIconUrl") ?? false);
            if (ins != null)
            {
                var index = result.IndexOf(ins) - 1;
                result[index] = new CodeInstruction(OpCodes.Ldnull);
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
