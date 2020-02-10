using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Harmony;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Localizer.Network;
using Localizer.Package.Import;
using Localizer.UIs;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using Ninject;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using static Localizer.Lang;
using File = System.IO.File;

namespace Localizer
{
    public sealed class Localizer : Mod
    {
        public static string SavePath;
        public static string SourcePackageDirPath;
        public static string DownloadPackageDirPath;
        public static string ConfigPath;
        public static Localizer Instance { get; private set; }
        public static ILog Log { get; private set; }
        public static TmodFile TmodFile { get; private set; }
        public static Configuration Config { get; set; }
        public static OperationTiming State { get; internal set; }
        internal static LocalizerKernel Kernel { get; private set; }
        internal static HarmonyInstance Harmony { get; set; }
        internal static MainWindow PackageUI { get; set; }
        internal static LoadedModWrapper LoadedLocalizer;

        private static Dictionary<int, GameCulture> _gameCultures;

        private static bool _initiated = false;

        public Localizer()
        {
            Instance = this;
            LoadedLocalizer = new LoadedModWrapper("Terraria.ModLoader.Core.AssemblyManager".Type().ValueOf("loadedMods").Invoke("get_Item", "!Localizer"));
            this.SetField("<File>k__BackingField", LoadedLocalizer.File);
            this.SetField("<Code>k__BackingField", LoadedLocalizer.Code);
            Log = LogManager.GetLogger(nameof(Localizer));

            Harmony = HarmonyInstance.Create(nameof(Localizer));
            Harmony.Patch("Terraria.ModLoader.Core.AssemblyManager".Type().Method("Instantiate"), new HarmonyMethod(NoroHelper.MethodInfo(() => AfterLocalizerCtorHook(null))));

            State = OperationTiming.BeforeModCtor;
            TmodFile = Instance.ValueOf<TmodFile>("File");
            Init();
            _initiated = true;
        }

        private static void AfterLocalizerCtorHook(object mod)
        {
            Hooks.InvokeBeforeModCtor(mod);
        }

        private static void Init()
        {
            _gameCultures = typeof(GameCulture).ValueOf<Dictionary<int, GameCulture>>("_legacyCultures");

            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            SavePath = "./Localizer/";
            SourcePackageDirPath = SavePath + "/Source/";
            DownloadPackageDirPath = SavePath + "/Download/";
            ConfigPath = SavePath + "/Config.json";

            Utils.EnsureDir(SavePath);
            Utils.EnsureDir(SourcePackageDirPath);
            Utils.EnsureDir(DownloadPackageDirPath);

            LoadConfig();
            AddModTranslations(Instance);
            Kernel = new LocalizerKernel();
            Kernel.Init();

            var autoImportService = Kernel.Get<AutoImportService>();
        }

        public override void Load()
        {
            if (!_initiated)
            {
                throw new Exception("Localizer not initialized.");
            }

            State = OperationTiming.BeforeModLoad;
            Hooks.InvokeBeforeLoad();
            Kernel.Get<RefreshLanguageService>();

            if (LanguageManager.Instance.ActiveCulture == GameCulture.Chinese)
            {
                ModBrowser.Patches.Patch();
            }

            var onInit = "Terraria.ModLoader.UI.UIModItem".Type().Method("OnInitialize");
            Harmony.Patch(onInit, postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => UIModItemPostfix(null))));

            var drawSelf = "Terraria.ModLoader.UI.UIModItem".Type().Method("DrawSelf");
            Harmony.Patch(drawSelf, postfix: new HarmonyMethod(NoroHelper.MethodInfo(() => DrawSelfPostfix(null))));
        }

        private static int frameCounter;
        private static void DrawSelfPostfix(object __instance)
        {
            var modName = __instance.ValueOf("_mod")?.ValueOf("Name")?.ToString();
            if (modName == "!Localizer")
            {
                frameCounter++;
                var current = __instance as UIPanel;
                if (current.ValueOf<UIImage>("_configButton")?.IsMouseHovering ?? false)
                {
                    current.SetField("_tooltip", "Localizer UI");
                }

                if (frameCounter % 2 == 0)
                {
                    var name = "Localizer";
                    var rainbowText = "";
                    for (var i = 0; i < name.Length; i++)
                    {
                        var colorHue = frameCounter / 300f;
                        colorHue += i * 0.5f / name.Length;
                        colorHue %= 1;
                        colorHue *= (float)Math.PI / 2;
                        var color = Main.hslToRgb(colorHue, 1, 0.5f);
                        rainbowText += $"[c/{color.R:X2}{color.G:X2}{color.B:X2}:{name[i]}]";
                    }
                    current.ValueOf<UIText>("_modName").SetField("_text", $"{rainbowText} v{current.ValueOf("_mod").ValueOf("modFile").ValueOf("version")}");
                }
            }
        }

        private static void UIModItemPostfix(object __instance)
        {
            var modName = __instance.ValueOf("_mod")?.ValueOf("Name")?.ToString();
            if (modName == "!Localizer")
            {
                var current = __instance as UIPanel;
                var configButton = new UIImage(typeof(UICommon).ValueOf<Texture2D>("ButtonModConfigTexture"))
                {
                    Width =
                    {
                        Pixels = 36f
                    },
                    Height =
                    {
                        Pixels = 36f
                    },
                    Left =
                    {
                        Pixels = -108f - 10f,
                        Precent = 1f
                    },
                    Top =
                    {
                        Pixels = 40f
                    }
                };
                configButton.OnClick += (evt, element) =>
                {
                    if (PackageUI != null)
                    {
                        PackageUI.Visible = true;
                    }
                    else
                    {
                        PackageUI = new MainWindow();
                        Instance.UIHost.Desktop.AddWindow(PackageUI);
                    }
                };
                current.SetField("_configButton", configButton);
                current.Append(configButton);
            }
        }

        public override void PostSetupContent()
        {
            State = OperationTiming.BeforeContentLoad;
            Hooks.InvokeBeforeSetupContent();
            CheckUpdate();
            AddPostDrawHook();
        }

        public UIHost UIHost { get; private set; }
        private void AddPostDrawHook()
        {
            UIHost = new UIHost();

            Main.OnPostDraw += OnPostDraw;
        }

        private void OnPostDraw(GameTime time)
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.spriteBatch.SafeBegin();
            Hooks.InvokeOnPostDraw(time);
            UIHost.Update(time);
            UIHost.Draw(time);
            Main.DrawCursor(Main.DrawThickCursor(false), false);
            Main.spriteBatch.SafeEnd();
        }

        public override void PostAddRecipes()
        {
            State = OperationTiming.PostContentLoad;

            Hooks.InvokePostSetupContent();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Hooks.InvokeOnGameUpdate(gameTime);
        }

        public void CheckUpdate()
        {
            Task.Run(() =>
            {
                var curVersion = Version;
                if (Kernel.Get<IUpdateService>().CheckUpdate(curVersion, out var updateInfo))
                {
                    var msg = _("NewVersion", updateInfo.Version);
                    if (Main.gameMenu)
                    {
                        UI.ShowInfoMessage(msg, 0);
                    }
                    else
                    {
                        Main.NewText(msg, Color.Red);
                    }
                }
            });
        }

        public override void Unload()
        {
            try
            {
                SaveConfig();

                PackageUI?.Close();
                UIHost.Dispose();
                Main.OnPostDraw -= OnPostDraw;

                HookEndpointManager.RemoveAllOwnedBy(this);
                Harmony.UnpatchAll(nameof(Localizer));
                Kernel.Dispose();

                PackageUI = null;
                Harmony = null;
                Kernel = null;
                _gameCultures = null;
                Config = null;
                Instance = null;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                _initiated = false;
                Log = null;
            }

            base.Unload();
        }

        public static void LoadConfig()
        {
            Log.Info("Loading config");
            if (File.Exists(ConfigPath))
            {
                Config = Utils.ReadFileAndDeserializeJson<Configuration>(ConfigPath);
                if (Config is null)
                {
                    throw new Exception("Config read failed!");
                }
            }
            else
            {
                Log.Info("No config file, creating...");
                Config = new Configuration();
            }

            Utils.SerializeJsonAndCreateFile(Config, ConfigPath);
            Log.Info("Config loaded");
        }

        public static void SaveConfig()
        {
            Log.Info("Saving config...");
            Utils.SerializeJsonAndCreateFile(Config, ConfigPath);
            Log.Info("Config saved");
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

        public static void RefreshLanguages()
        {
            Kernel.Get<RefreshLanguageService>().Refresh();
        }

        public static IMod GetWrappedMod(string name)
        {
            if (State < OperationTiming.PostContentLoad)
            {
                var loadedMods = "Terraria.ModLoader.Core.AssemblyManager".Type().ValueOf("loadedMods");
                return (bool)loadedMods.Invoke("ContainsKey", name)
                    ? new LoadedModWrapper(loadedMods.Invoke("get_Item", name))
                    : null;
            }

            var mod = Utils.GetModByName(name);
            if (mod is null)
            {
                return null;
            }

            return new ModWrapper(mod);
        }

        public static bool CanDoOperationNow(Type t)
        {
            var attribute = t.GetCustomAttribute<OperationTimingAttribute>();
            return attribute == null || CanDoOperationNow(attribute.Timing);
        }

        public static bool CanDoOperationNow(OperationTiming t)
        {
            return (t & State) != 0;
        }
    }
}
