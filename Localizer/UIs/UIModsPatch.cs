using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI.Chat;
using static Localizer.Lang;

namespace Localizer.UIs
{
    internal static class UIModsPatch
    {
        internal static bool ReloadRequired;
        private static int frameCounter;
        internal static Dictionary<string, string> ModsExtraInfo;

        public static void Patch()
        {
            ReloadRequired = false;
            frameCounter = 0;
            ModsExtraInfo = new Dictionary<string, string>();

            var refHolder = false;
            var reloadRequired = "Terraria.ModLoader.Config.ConfigManager".Type().Method("ModNeedsReload");
            Localizer.Harmony.Patch(reloadRequired, postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => ModNeedsReloadPostfix(ref refHolder))));

            var onInit = "Terraria.ModLoader.UI.UIModItem".Type().Method("OnInitialize");
            Localizer.Harmony.Patch(onInit, postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => UIModItemPostfix(null))));

            var drawSelf = "Terraria.ModLoader.UI.UIModItem".Type().Method("DrawSelf");
            Localizer.Harmony.Patch(drawSelf, postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => DrawSelfPostfix(null, null))));

            var populateFromJson = "Terraria.ModLoader.UI.ModBrowser.UIModBrowser".Type().Method("PopulateFromJson");
            Localizer.Harmony.Patch(populateFromJson, prefix: new HarmonyMethod(NoroHelper.MethodInfo(() => PopulateFromJsonPrefix())),
                postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => PopulateFromJsonPostfix())));
        }

        private static void PopulateFromJsonPrefix()
        {
            Localizer.LoadedLocalizer.File.SetField("<name>k__BackingField", "Localizer");
            Localizer.LoadedLocalizer.SetField("name", "Localizer");
        }

        private static void PopulateFromJsonPostfix()
        {
            Localizer.LoadedLocalizer.File.SetField("<name>k__BackingField", "!Localizer");
            Localizer.LoadedLocalizer.SetField("name", "!Localizer");
        }

        private static void ModNeedsReloadPostfix(ref bool __result)
        {
            if (ReloadRequired)
            {
                __result = true;
            }
        }

        private static void DrawSelfPostfix(object __instance, SpriteBatch spriteBatch)
        {
            var current = __instance as UIPanel;
            var modName = __instance.ValueOf("_mod")?.ValueOf("Name")?.ToString();
            var modNameHovering = current.ValueOf<UIText>("_modName")?.IsMouseHovering ?? false;
            if (modName == "!Localizer")
            {
                frameCounter++;

                current.ValueOf<UIText>("_modName")
                       .SetField("_text",
                                 $"{Utils.AsRainbow("Localizer", frameCounter)} v{current.ValueOf("_mod").ValueOf("modFile").ValueOf("version")}");

                var tooltip = "";
                if (modNameHovering)
                {
                    var modAuthor = current.ValueOf("_mod")?.ValueOf("properties").ValueOf<string>("author");
                    if (modAuthor.Length > 0)
                    {
                        tooltip = _("OpenUI",
                            Language.GetTextValue("tModLoader.ModsByline", Utils.AsRainbow(modAuthor, frameCounter + 150, 9)),
                            $"{Utils.AsRainbow("Localizer", frameCounter)}");
                    }
                }

                if (!string.IsNullOrEmpty(tooltip))
                {
                    current.SetField("_tooltip", "");
                    var snippets = ChatManager.ParseMessage(tooltip, Color.White).ToArray();
                    var x = ChatManager.GetStringSize(Main.fontMouseText, snippets, Vector2.One).X;
                    var pos = Main.MouseScreen + new Vector2(16f);
                    pos.X = Math.Min(pos.X, Main.screenWidth - x - 16f);
                    pos.Y = Math.Min(pos.Y, Main.screenHeight - 30);
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText,
                      snippets, pos, 0, Vector2.Zero, Vector2.One, out var _);
                }
            }
            else if (modNameHovering && ModsExtraInfo.ContainsKey(modName))
            {
                current.SetField("_tooltip", current.ValueOf<string>("_tooltip") + Environment.NewLine + ModsExtraInfo[modName]);
            }
        }

        private static void UIModItemPostfix(object __instance)
        {
            var modName = __instance.ValueOf("_mod")?.ValueOf("Name")?.ToString();
            if (modName == "!Localizer")
            {
                __instance.ValueOf<UIText>("_modName").OnClick += (evt, element) =>
                {
                    if (Localizer.PackageUI != null)
                    {
                        Localizer.PackageUI.Visible = true;
                    }
                    else
                    {
                        Localizer.PackageUI = new MainWindow();
                        Localizer.Instance.UIHost.Desktop.AddWindow(Localizer.PackageUI);
                    }
                };
            }
        }
    }
}
