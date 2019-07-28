using System;
using log4net;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LocalizerWPF.Model;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer
{
    public class Localizer : Mod
    {
        public static ILog Log { get; private set; }
        public static Localizer Instance { get; private set; }

        public static Configuration Config { get; set; }
        
        public static string SavePath => Terraria.Main.SavePath + "/Localizer/";

        public static string ConfigPath = Terraria.Main.SavePath + "/Localizer/Config.json";

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

            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            
            LoadConfig();
            
        }

        public override void PostSetupContent()
        {
            PackageManager.LoadPackages(false);
            PluginManager.LoadPlugins();
        }

        public override void Unload()
        {
            base.Unload();
            
            SaveConfig();

            PluginManager.UnloadPlugins();
            PackageManager.Unload();
        }

        public static void LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                Config = Utils.ReadFileAndDeserializeJson<Configuration>(ConfigPath);
            }
            else
            {
                Config = new Configuration();
                Utils.SerializeJsonAndCreateFile(Config, ConfigPath);
            }
        }

        public static void SaveConfig()
        {
            Utils.SerializeJsonAndCreateFile(Config, ConfigPath);
        }

        public static GameCulture AddGameCulture(CultureInfo culture)
        {
            return GameCulture.FromName(culture.Name) != null ? null : new GameCulture(culture.Name, _gameCultures.Count);
        }

        public static GameCulture CultureInfoToGameCulture(CultureInfo culture)
        {
            var gc = GameCulture.FromName(culture.Name);
            return gc ?? AddGameCulture(culture);
        }

        public static void RefreshLanguages(CultureInfo lang)
        {
            ModContent.RefreshModLanguage(CultureInfoToGameCulture(lang));
        }
    }
}
