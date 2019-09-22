using Localizer.DataModel;

namespace LocalizerWPF.Model
{
    public class ExportConfig : IExportConfig
    {
        public bool MakeBackup { get; set; }
        public bool ForceOverride { get; set; }
        public bool WithTranslation { get; set; }
    }
}
