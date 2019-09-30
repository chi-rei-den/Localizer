using System;
using System.Linq;
using System.Windows;

namespace LocalizerWPF
{
    public static class Lang
    {
        public static string[] Languages { get; set; } = new[] { "en", "zh" };

        private static ResourceDictionary CurrentLangDict { get; set; } = new ResourceDictionary();

        public static void SwitchLang(string lang)
        {
            if (!Languages.Contains(lang))
            {
                lang = "en";
            }

            var merged = Application.Current.Resources.MergedDictionaries;
            if (CurrentLangDict != null && merged.Contains(CurrentLangDict))
            {
                merged.Remove(CurrentLangDict);
            }

            var assemblyName = typeof(Lang).Assembly.GetName().Name;
            CurrentLangDict = new ResourceDictionary()
            {
                Source = new Uri($"pack://application:,,,/{assemblyName};component/Resources/Langs/{lang}.xaml")
            };
            merged.Add(CurrentLangDict);
            CurrentLangDict.BeginInit();
        }

        public static string _(string key)
        {
            if (!CurrentLangDict.Contains(key))
            {
                return key;
            }

            return CurrentLangDict[key].ToString();
        }

        public static void Dispose()
        {
            Languages = null;
            CurrentLangDict = null;
        }
    }
}
