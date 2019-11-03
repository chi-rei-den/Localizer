using Harmony;
using Localizer;
using Terraria.Localization;

namespace ModBrowserMirror
{
    public class LocalizerPlugin : Localizer.LocalizerPlugin
    {
        public override string Name => "ModBrowserMirror";
        public override string Author => "Chireiden";
        public override string Description => "Provide the mod browser mirror service for chinese players.";
        
        public static HarmonyInstance HarmonyInstance { get; set; }

        public override void Initialize(LocalizerKernel kernel)
        {
            if (LanguageManager.Instance.ActiveCulture != GameCulture.Chinese)
            {
                return;
            }

            HarmonyInstance = HarmonyInstance.Create("ModBrowserMirror");

            ReplaceURLs.Patch();
        }

        protected override void DisposeUnmanaged()
        {
            HarmonyInstance.UnpatchAll("ModBrowserMirror");
            HarmonyInstance = null;
        }
    }
}
