using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Terraria.ModLoader;

namespace Localizer.ModBrowser
{
    public class Patches
    {
        public static void Patch()
        {
            Utils.LogInfo($"Patching ModBrowser, tML version: {ModLoader.version}");

            var populateModBrowser = "Terraria.ModLoader.UI.ModBrowser.UIModBrowser".Type()
                                      .GetMethods(NoroHelper.Any)
                                      .FirstOrDefault(m => m.Name.Contains("<PopulateModBrowser>"));
            Localizer.Harmony.Patch(populateModBrowser, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => PopulateModBrowserTranspiler(null))));

            var fromJson = "Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem".Type().Method("FromJson");
            Localizer.Harmony.Patch(fromJson, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => FromJSONTranspiler(null))));

            var onActivate = "Terraria.ModLoader.UI.UIModInfo".Type()
                                      .GetMethods(NoroHelper.Any)
                                      .FirstOrDefault(m => m.Name.Contains("<OnActivate>"));
            Localizer.Harmony.Patch(onActivate, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => OnActivateTranspiler(null))));

            Utils.LogInfo("ModBrowser Patched");
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
                var concat = NoroHelper.MethodInfo(() => string.Concat("", "", ""));
                var thisModName = "Terraria.ModLoader.UI.UIModInfo".Type().Field("_modName");

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
