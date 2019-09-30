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
            Plugin.HarmonyInstance.Patch(populateModBrowser, null, null, new HarmonyMethod(populateModBrowserTranspiler));

            var fromJson = typeof(Mod).Module
                                       .GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem")
                                       .GetMethod(fromJsonMethodName, bindingFlags);
            var fromJsonTranspiler = typeof(ReplaceURLs).GetMethod(nameof(FromJSONTranspiler), bindingFlags);
            Plugin.HarmonyInstance.Patch(fromJson, null, null, new HarmonyMethod(fromJsonTranspiler));

            var onActivate = typeof(Mod).Module
                                       .GetType("Terraria.ModLoader.UI.UIModInfo")
                                      .GetMethods(bindingFlags)
                                      .FirstOrDefault(m => m.Name.Contains(onActivateMethodName));
            var onActivateTranspiler = typeof(ReplaceURLs).GetMethod(nameof(OnActivateTranspiler), bindingFlags);
            Plugin.HarmonyInstance.Patch(onActivate, null, null, new HarmonyMethod(onActivateTranspiler));

            var uiModDownloadItemCtor = typeof(Mod).Module
                                        .GetType("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem")
                                        .GetConstructors(bindingFlags)[0];
            var removeIconTranspiler = typeof(ReplaceURLs).GetMethod(nameof(RemoveIconTranspiler), bindingFlags);
            Plugin.HarmonyInstance.Patch(uiModDownloadItemCtor, null, null, new HarmonyMethod(removeIconTranspiler));

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
                var concat = typeof(string).Module.FindMethod(
                    "System.String System.String::Concat(System.String,System.String,System.String)");
                var thisModName = typeof(Mod)
                                  .Module.GetTypes()
                                  .FirstOrDefault(t => t.FullName == "Terraria.ModLoader.UI.UIModInfo")
                                  .GetField("_modName", bindingFlags);

                ins.operand = "https://trbbs.cc/trmod/";
                var index = result.IndexOf(ins) + 1;
                result.Insert(index, new CodeInstruction(OpCodes.Call, concat));
                result.Insert(index, new CodeInstruction(OpCodes.Ldstr, ".desc"));
                result.Insert(index, new CodeInstruction(OpCodes.Ldfld, thisModName));
                result.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
            }

            return result;
        }

        private static IEnumerable<CodeInstruction> RemoveIconTranspiler(IEnumerable<CodeInstruction> instructions)
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
