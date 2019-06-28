using System;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Localizer
{

    [HarmonyPatch(typeof(ChatManager), "DrawColorCodedStringWithShadow", new Type[]
    { typeof(SpriteBatch), typeof(DynamicSpriteFont),typeof(string), typeof(Vector2),typeof(Color),
      typeof(float), typeof(Vector2), typeof(Vector2), typeof(float), typeof(float)})]
    public class InternalDrawHandler
    {
        private static List<string> textlist = new List<string>();
        public static bool Prefix(SpriteBatch spriteBatch, DynamicSpriteFont font, ref string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            if (DataContainer.dict.ContainsKey(text))
                text = DataContainer.dict[text];
            return true;
        }
    }

}