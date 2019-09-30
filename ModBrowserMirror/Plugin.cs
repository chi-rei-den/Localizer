using Harmony;
using Terraria.Localization;

namespace ModBrowserMirror
{
    public class Plugin : Localizer.Plugin
    {
        public static HarmonyInstance HarmonyInstance { get; set; }

        public override void Initialize()
        {
            if (LanguageManager.Instance.ActiveCulture != GameCulture.Chinese)
            {
                return;
            }

            HarmonyInstance = HarmonyInstance.Create("ModBrowserMirror");

            ReplaceURLs.Patch();
        }

        protected override void OnDispose()
        {
            HarmonyInstance.UnpatchAll("ModBrowserMirror");
            HarmonyInstance = null;
        }
    }
}
