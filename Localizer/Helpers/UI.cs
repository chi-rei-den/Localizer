using System;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.UI;

namespace Localizer
{
    public static class UI
    {
        public static void ShowInfoMessage(string message, int gotoMenu, UIState state = null, string altButtonText = "", Action altButtonAction = null)
        {
            var infoMsgUI = GetModLoaderUI("infoMessage") ?? throw new Exception("Cannot Find infoMessage field");

            infoMsgUI.GetType().GetMethod("Show", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(infoMsgUI, new object[] { message, gotoMenu, state, altButtonText, altButtonAction });
        }

        public static object GetModLoaderUI(string uiName)
        {
            var ui = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.Interface");
            return ui?.GetField(uiName, BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
        }
    }
}
