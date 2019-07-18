using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer
{
    public class Localizer : Mod
    {
        public static ILog Log { get; private set; }
        public static Localizer Instance { get; private set; }

        private static Dictionary<int, GameCulture> _gameCultures;

        public Localizer()
        {
            _gameCultures = typeof(GameCulture).GetFieldDirectly(null, "_legacyCultures") as Dictionary<int, GameCulture>;

            PluginManager.Init();
            PackageManager.Init();
        }

        public override void Load()
        {
            Log = this.Logger;
            Instance = this;
            
            PackageManager.LoadPackages();
            PluginManager.LoadPlugins();
        }

        public override void Unload()
        {
            base.Unload();

            PluginManager.UnloadPlugins();
        }

        public static GameCulture AddGameCulture(CultureInfo culture)
        {
            if (GameCulture.FromName(culture.Name) != null)
                return null;

            return new GameCulture(culture.Name, _gameCultures.Count);
        }

        public static GameCulture CultureInfoToGameCulture(CultureInfo culture)
        {
            var gc = GameCulture.FromName(culture.Name);
            return gc == null ? AddGameCulture(culture) : gc;
        }

        public static void RefreshLanguages(CultureInfo lang)
        {
            ModContent.RefreshModLanguage(CultureInfoToGameCulture(lang));
        }
    }
}
