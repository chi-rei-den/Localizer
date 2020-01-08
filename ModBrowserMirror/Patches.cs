using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Localizer;
using Localizer.Helpers;
using Terraria.ModLoader;

namespace ModBrowserMirror
{
    public class ReplaceURLs
    {
        private static string populateModBrowserMethodName = "<PopulateModBrowser>";
        private static string fromJsonMethodName = "FromJson";
        private static string onActivateMethodName = "<OnActivate>";

        private const BindingFlags bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        public static void Patch()
        {
            var tmlVersion = ModLoader.version;
            Utils.LogInfo($"Patching ModBrowser, tML version: {tmlVersion}");

            var populateModBrowser = typeof(Mod).Module
                                      .GetType("Terraria.ModLoader.UI.ModBrowser.UIModBrowser")
                                      .GetMethods(bindingFlags)
                                      .FirstOrDefault(m => m.Name.Contains(populateModBrowserMethodName));
            var populateModBrowserTranspiler = typeof(ReplaceURLs).GetMethod(nameof(PopulateModBrowserTranspiler), bindingFlags);
            LocalizerPlugin.HarmonyInstance.Patch(populateModBrowser, null, null, new HarmonyMethod(populateModBrowserTranspiler));

            var fromJson = typeof(Mod).Module
                                       .GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem")
                                       .GetMethod(fromJsonMethodName, bindingFlags);
            var fromJsonTranspiler = typeof(ReplaceURLs).GetMethod(nameof(FromJSONTranspiler), bindingFlags);
            LocalizerPlugin.HarmonyInstance.Patch(fromJson, null, null, new HarmonyMethod(fromJsonTranspiler));

            var onActivate = typeof(Mod).Module
                                       .GetType("Terraria.ModLoader.UI.UIModInfo")
                                      .GetMethods(bindingFlags)
                                      .FirstOrDefault(m => m.Name.Contains(onActivateMethodName));
            var onActivateTranspiler = typeof(ReplaceURLs).GetMethod(nameof(OnActivateTranspiler), bindingFlags);
            LocalizerPlugin.HarmonyInstance.Patch(onActivate, null, null, new HarmonyMethod(onActivateTranspiler));

            Utils.LogInfo("ModBrowser Patched");
        }

        private static IEnumerable<CodeInstruction> PopulateModBrowserTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/listmods.php", "https://k.sgkoi.dev/tModLoader/listmods.php", result);

            return result;
        }

        private static IEnumerable<CodeInstruction> FromJSONTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://k.sgkoi.dev/tModLoader/download.php?Down=mods/", result);

            return result;
        }

        private static IEnumerable<CodeInstruction> OnActivateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            ReplaceLdstr("http://javid.ddns.net/tModLoader/download.php?Down=mods/", "https://k.sgkoi.dev/tModLoader/download.php?Down=mods/", result);
            ReplaceLdstr("http://javid.ddns.net/tModLoader/moddescription.php", "https://k.sgkoi.dev/tModLoader/moddescription.php", result);

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
