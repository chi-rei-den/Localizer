using System;
using Terraria.ModLoader;
using Harmony;

namespace Localizer
{

    [HarmonyPatch(typeof(TooltipLine), MethodType.Constructor, new Type[]{typeof(Mod), typeof(string), typeof(string)})]
    public class TooltipHandler
    {
        public static bool Prefix(Mod mod, string name, ref string text)
        {
            //text = "hacked";
            return true;
        }
    }
}