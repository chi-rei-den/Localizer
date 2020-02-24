using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Terraria.ModLoader;

namespace Localizer.ModBrowser
{
    public static class Patches
    {
        public static void Patch()
        {
            Utils.LogInfo($"Patching ModBrowser, tML version: {ModLoader.version}");

            if (!string.IsNullOrEmpty(GetModListURL()))
            {
                var populateModBrowser = "Terraria.ModLoader.UI.ModBrowser.UIModBrowser".Type()
                                          .GetMethods(NoroHelper.Any)
                                          .FirstOrDefault(m => m.Name.Contains("<PopulateModBrowser>"));
                Localizer.Harmony.Patch(populateModBrowser, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => PopulateModBrowserTranspiler(null))));
            }

            if (!string.IsNullOrEmpty(GetModDownloadURL()))
            {
                var fromJson = "Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem".Type().Method("FromJson");
                Localizer.Harmony.Patch(fromJson, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => FromJSONTranspiler(null))));

                if (!string.IsNullOrEmpty(GetModDescURL()))
                {
                    var onActivate = "Terraria.ModLoader.UI.UIModInfo".Type()
                                              .GetMethods(NoroHelper.Any)
                                              .FirstOrDefault(m => m.Name.Contains("<OnActivate>"));
                    Localizer.Harmony.Patch(onActivate, null, null, new HarmonyMethod(NoroHelper.MethodInfo(() => OnActivateTranspiler(null))));
                }
            }

            Utils.LogInfo("ModBrowser Patched");
        }

        private static string GetModListURL()
        {
            var mirror = Localizer.Config.ModListMirror[0];
            switch (mirror)
            {
                case "mirror.sgkoi.dev":
                    return "https://mirror.sgkoi.dev/tModLoader/listmods.php";
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/listmods.php";
                case "mirror3.sgkoi.dev":
                    return "https://trbbs.cc/trmod/listmods.php";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/listmods.php";
                default:
                    return mirror;
            }
        }

        private static string GetModDownloadURL()
        {
            var mirror = Localizer.Config.ModDownloadMirror[0];
            switch (mirror)
            {
                case "mirror.sgkoi.dev":
                    return "https://mirror.sgkoi.dev/tModLoader/download.php?Down=mods/";
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/download.php?Down=mods/";
                case "mirror3.sgkoi.dev":
                    return "https://trbbs.cc/trmod/";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/download.php?Down=mods/";
                default:
                    return mirror;
            }
        }

        private static string GetModDescURL()
        {
            var mirror = Localizer.Config.ModDescMirror[0];
            switch (mirror)
            {
                case "mirror.sgkoi.dev":
                    return "https://mirror.sgkoi.dev/tModLoader/moddescription.php";
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/moddescription.php";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/moddescription.php";
                default:
                    return mirror;
            }
        }

        private static IEnumerable<CodeInstruction> PopulateModBrowserTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/listmods.php", GetModListURL(), result);

            return result;
        }

        private static IEnumerable<CodeInstruction> FromJSONTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", GetModDownloadURL(), result);
            ReplaceLdstr("&tls12=y", "", result);

            return result;
        }

        private static IEnumerable<CodeInstruction> OnActivateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", GetModDownloadURL(), result);
            ReplaceLdstr("http://javid.ddns.net/tModLoader/moddescription.php", GetModDescURL(), result);
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
