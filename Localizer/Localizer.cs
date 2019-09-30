using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Localizer.Modules;
using log4net;
using MonoMod.RuntimeDetour.HookGen;
using Ninject.Modules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer
{
    public sealed class Localizer : Mod
    {
        public static string SavePath;
        public static string SourcePackageDirPath;
        public static string DownloadPackageDirPath;
        public static string ConfigPath;

        private static Dictionary<int, GameCulture> _gameCultures;

        public Localizer()
        {
            _gameCultures =
                typeof(GameCulture).GetFieldDirectly(null, "_legacyCultures") as Dictionary<int, GameCulture>;
        }

        public static ILog Log { get; private set; }
        public static Localizer Instance { get; private set; }

        public static LocalizerKernel Kernel { get; private set; }

        public static Configuration Config { get; set; }

        public override void Load()
        {
            Log = Logger;
            Instance = this;

            PluginManager.Init();

            SavePath = "./Localizer/";
            SourcePackageDirPath = SavePath + "/Source/";
            DownloadPackageDirPath = SavePath + "/Download/";
            ConfigPath = SavePath + "/Config.json";

            Utils.CreateDirectory(SavePath);
            Utils.CreateDirectory(SourcePackageDirPath);
            Utils.CreateDirectory(DownloadPackageDirPath);

            LoadConfig();

            Kernel = new LocalizerKernel();
            Kernel.Load(new NinjectModule[]
            {
                new DefaultPackageModule(), new DefaultFileExportModule(),
                new DefaultFileUpdateModule(), new DefaultFileImportModule(),
                new DefaultNetworkModule(),
            });
        }

        public override void PostSetupContent()
        {
            PluginManager.LoadPlugins();
        }

        public override void Unload()
        {
            try
            {
                SaveConfig();

                PluginManager.UnloadPlugins();

                HookEndpointManager.RemoveAllOwnedBy(this);

                Kernel.Dispose();

                Kernel = null;
                _gameCultures = null;
                Instance = null;
                Config = null;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                Log = null;
            }

            base.Unload();
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
            return GameCulture.FromName(culture.Name) != null
                ? null
                : new GameCulture(culture.Name, _gameCultures.Count);
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

        public static void RefreshLanguages()
        {
            ModContent.RefreshModLanguage(LanguageManager.Instance.ActiveCulture);
        }
    }
}
