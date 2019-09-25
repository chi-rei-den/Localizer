using Terraria.Localization;

namespace LocalizerWPF.Model
{
    public class Configuration
    {
        public string ThemeBase { get; set; } = "Light";
        public string Language { get; set; }

        public Configuration()
        {
            var activeCulture = LanguageManager.Instance.ActiveCulture;
            if (activeCulture == GameCulture.Chinese)
            {
                Language = "zh";
            }
            else
            {
                Language = "en";
            }
        }
    }
}
