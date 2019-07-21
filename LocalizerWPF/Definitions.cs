using Localizer.DataModel;
using System.Collections.ObjectModel;

namespace LocalizerWPF
{
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
        public ObservableCollection<Package> ModTranslations { get; set; }
        public ModTranslation(string name) { this.Name = name; }
    }
}
