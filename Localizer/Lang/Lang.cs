using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

// ReSharper disable once CheckNamespace
namespace Localizer
{
    public static class Lang
    {
        private static List<string> _keys = new List<string>()
        {
            "NewVersion",
        };

        private static Dictionary<string, string> _en = new Dictionary<string, string>()
        {
            { _keys[0], "Found new version of Localizer: {0} \nPlease go update it." },
        };

        private static Dictionary<string, string> _zh = new Dictionary<string, string>()
        {
            { _keys[0], "发现汉化者Mod有新版本: {0} \n请前往更新。" },
        };

        public static void AddModTranslations(Mod mod)
        {
            foreach (var k in _keys)
            {
                var translation = mod.CreateTranslation(k);
                translation.SetDefault(_en[k]);
                translation.AddTranslation(GameCulture.Chinese, _zh[k]);
                mod.AddTranslation(translation);
            }
        }

        public static string _(string key)
        {
            return Language.GetTextValue($"Mods.Localizer.{key}");
        }

        public static string _(string key, object arg0)
        {
            return Language.GetTextValue($"Mods.Localizer.{key}", arg0);
        }
    }
}
