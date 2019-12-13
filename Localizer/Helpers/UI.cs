using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Noro;
using Noro.Access;
using Terraria.ModLoader;
using Terraria.UI;

namespace Localizer.Helpers
{
    public static class UI
    {
        public static void ShowInfoMessage(string message, int gotoMenu, UIState state = null, string altButtonText = "", Action altButtonAction = null)
        {
            var infoMsgUI = GetModLoaderUI("infoMessage") ?? throw new Exception("Cannot Find infoMessage field");

            infoMsgUI.M("Show", message, gotoMenu, state, altButtonText, altButtonAction);
        }

        public static object GetModLoaderUI(string uiName)
        {
            var ui = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.Interface");
            return ui?.F(uiName);
        }

        
        public static void SafeBegin(this SpriteBatch sb)
        {
            try
            {
                sb.Begin();
            }
            catch(InvalidOperationException)
            {
            }
        }        
        public static void SafeBegin(this SpriteBatch sb, SamplerState sampler, RasterizerState rasterizer)
        {
            try
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, sampler, null, rasterizer);
            }
            catch(InvalidOperationException)
            {
            }
        }
        
        public static void SafeEnd(this SpriteBatch sb)
        {
            try
            {
                sb.End();
            }
            catch(InvalidOperationException)
            {
            }
        }
    }
}
