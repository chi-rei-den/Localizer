using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace LocalizerWPF
{
    public class ModTranslationPackage
    {
        public bool Enabled { get; set; }
        public string Name { get; private set; }
        public string Author { get; set; }
        public string Mod { get; private set; }
        public Version Version { get; set; } = new Version(1, 0, 0, 0);
        public CultureInfo Language { get; set; } = CultureInfo.GetCultureInfo("en-US");
        public List<Translation> Translations { get; set; } = new List<Translation>();
        public ModTranslationPackage(string name, string mod)
        {
            this.Name = name;
            this.Mod = mod;
        }
    }

    public class Translation
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Namespace { get; set; } = "*";
        public string Method { get; set; } = "*";
    }
    public class ModTranslation
    {
        public string Name { get; private set; }
        public ObservableCollection<ModTranslationPackage> ModTranslations { get; set; }
        public ModTranslation(string name) { this.Name = name; }
    }
}
