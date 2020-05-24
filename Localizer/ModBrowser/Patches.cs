using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Harmony;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.ModBrowser
{
    public static class Patches
    {
        public static HarmonyInstance HarmonyInstance { get; set; }

        public static void Patch()
        {
            Utils.LogInfo($"Patching ModBrowser, tML version: {ModLoader.version}");

            HarmonyInstance = HarmonyInstance.Create(nameof(Patches));

            if (LanguageManager.Instance.ActiveCulture == GameCulture.Chinese)
            {
                try
                {
                    #region Download Mod List
                    if (!string.IsNullOrEmpty(GetModListURL()))
                    {
                        HarmonyInstance.Patch("Terraria.ModLoader.UI.ModBrowser.UIModBrowser", "<PopulateModBrowser>",
                            exactMatch: false,
                            transpiler: NoroHelper.HarmonyMethod(() => PopulateModBrowserTranspiler(null)));
                        Utils.LogInfo("PopulateModBrowser Patched");
                    }
                    #endregion

                    #region Get Mod Download URL
                    if (!string.IsNullOrEmpty(GetModDownloadURL()))
                    {
                        HarmonyInstance.Patch("Terraria.ModLoader.UI.ModBrowser.UIModDownloadItem", "FromJson",
                            transpiler: NoroHelper.HarmonyMethod(() => FromJSONTranspiler(null)));
                        Utils.LogInfo("FromJson Patched");

                        if (!string.IsNullOrEmpty(GetModDescURL()))
                        {
                            HarmonyInstance.Patch("Terraria.ModLoader.UI.UIModInfo", "<OnActivate>",
                                exactMatch: false,
                                transpiler: NoroHelper.HarmonyMethod(() => OnActivateTranspiler(null)));
                            Utils.LogInfo("OnActivate Patched");
                        }
                    }
                    #endregion

                    #region List My Mods
                    // Terraria.ModLoader.UI.UIManagePublished.OnActivate
                    // "http://javid.ddns.net/tModLoader/listmymods.php"
                    #endregion

                    #region Publish Mod
                    // Terraria.ModLoader.UI.UIModSourceItem.PublishMod
                    // "http://javid.ddns.net/tModLoader/publishmod.php"
                    #endregion

                    #region Unpublish Mod
                    // Terraria.ModLoader.UI.UIModManageItem.UnpublishMod
                    // "http://javid.ddns.net/tModLoader/unpublishmymod.php"
                    #endregion

                    #region Register Link
                    // Terraria.ModLoader.UI.UIEnterPassphraseMenu.VisitRegisterWebpage
                    // Terraria.ModLoader.UI.UIEnterSteamIDMenu.VisitRegisterWebpage
                    // "http://javid.ddns.net/tModLoader/register.php"
                    #endregion

                    #region Direct Mod Listing
                    // Terraria.ModLoader.UI.ModBrowser.UIModBrowser.<>c.<ShowOfflineTroubleshootingMessage>
                    // "http://javid.ddns.net/tModLoader/DirectModDownloadListing.php"
                    #endregion

                    #region Query Mod Download URL
                    // Terraria.ModLoader.Interface.ServerModBrowserMenu
                    // "http://javid.ddns.net/tModLoader/tools/querymoddownloadurl.php?modname="
                    #endregion

                    #region Error Report
                    // No plan
                    #endregion

                    #region ModCompile
                    HarmonyInstance.Patch("Terraria.ModLoader.UI.UIDeveloperModeHelp", "DownloadModCompile",
                        transpiler: NoroHelper.HarmonyMethod(() => ModCompileTranspiler(null)));
                    Utils.LogInfo("DownloadModCompile Patched");
                    #endregion

                    Utils.LogInfo("ModBrowser Patched");
                }
                catch (Exception e)
                {
                    Utils.LogInfo($"ModBrowser Patch exception: {e}");
                }
            }

            try
            {
                HarmonyInstance.Patch("Terraria.ModLoader.UI.DownloadManager.DownloadFile", "SetupDownloadRequest",
                    postfix: NoroHelper.HarmonyMethod(() => PostSetupDownloadRequest(null)));
                Utils.LogInfo("SetupDownloadRequest Patched");
            }
            catch (Exception e)
            {
                Utils.LogInfo($"ModBrowser Patch exception: {e}");
            }
        }

        private static readonly Regex _defaultMirror = new Regex(@"mirror(?:\d*)?\.sgkoi\.dev");

        private static string GetModListURL()
        {
            var mirror = Localizer.Config.ModListMirror[0];
            switch (mirror)
            {
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/listmods.php";
                case "mirror3.sgkoi.dev":
                    return "https://trbbs.cc/trmod/listmods.php";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/listmods.php";
                default:
                    return _defaultMirror.IsMatch(mirror) ? $"https://{mirror}/tModLoader/listmods.php" : mirror;
            }
        }

        private static string GetModDownloadURL()
        {
            var mirror = Localizer.Config.ModDownloadMirror[0];
            switch (mirror)
            {
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/download.php?Down=mods/";
                case "mirror3.sgkoi.dev":
                    return "https://trbbs.cc/trmod/";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/download.php?Down=mods/";
                default:
                    return _defaultMirror.IsMatch(mirror) ? $"https://{mirror}/tModLoader/download.php?Down=mods/" : mirror;
            }
        }

        private static string GetModDescURL()
        {
            var mirror = Localizer.Config.ModDescMirror[0];
            switch (mirror)
            {
                case "mirror2.sgkoi.dev":
                    return "http://www.mb.axeel.moe/tModLoader/moddescription.php";
                case "mirror4.sgkoi.dev":
                    return "http://www.mb2.axeel.moe:25555/tModLoader/moddescription.php";
                default:
                    return _defaultMirror.IsMatch(mirror) ? $"https://{mirror}/tModLoader/moddescription.php" : mirror;
            }
        }

        private static void PostSetupDownloadRequest(object __instance)
        {
            var request = __instance.ValueOf<HttpWebRequest>("<Request>k__BackingField");
            request.Headers[HttpRequestHeader.UserAgent] = Utils.UserAgent(false);
            request.Headers[HttpRequestHeader.AcceptLanguage] = LanguageManager.Instance.ActiveCulture.CultureInfo.ToString();
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

        private static IEnumerable<CodeInstruction> ModCompileTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].opcode == OpCodes.Ldstr && $"{result[i].operand}" == "https://github.com/tModLoader/tModLoader/releases/download/")
                {
                    result[i].operand = "https://mirror.sgkoi.dev/direct";
                    result[i + 1] = new CodeInstruction(OpCodes.Ldstr, "");
                }
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
