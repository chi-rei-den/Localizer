using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Localizer;

using Config = LocalizerWPF.Model.Configuration;
using Configuration = Localizer.Configuration;

namespace LocalizerWPF.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public Config Config { get; set; }

        public Configuration LocalizerConfig
        {
            get => Localizer.Localizer.Config;
            set
            {
                Localizer.Localizer.Config = value;
                RaisePropertyChanged("LocalizerConfig");
                Localizer.Localizer.SaveConfig();
            }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                RaisePropertyChanged("SelectedLanguage");
                Config.Language = LanguageMappings[_selectedLanguage];
                Lang.SwitchLang(Config.Language);
                SaveConfig();
            }
        }

        public string[] Languages { get; set; } = new[] { "English", "中文(简体)" };
        public Dictionary<string, string> LanguageMappings { get; set; } = new Dictionary<string, string>()
        {
            {"English", "en"},
            {"中文(简体)", "zh"},
        };

        public string[] ThemeBases { get; set; } = new[] { "Light", "Dark" };

        private string configPath = "./Localizer/UIConfig.json";
        private string _selectedLanguage;

        public SettingViewModel()
        {
            LoadConfig();
            SelectedLanguage = LanguageMappings.Keys.FirstOrDefault(k => LanguageMappings[k] == Config.Language);
        }

        public void LoadConfig()
        {
            if (File.Exists(configPath))
            {
                Config = Utils.ReadFileAndDeserializeJson<Config>(configPath);
            }
            else
            {
                Config = new Config();
                Utils.SerializeJsonAndCreateFile(Config, configPath);
            }
        }

        public void SaveConfig()
        {
            Utils.SerializeJsonAndCreateFile(Config, configPath);
        }
    }
}
