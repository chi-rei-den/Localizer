namespace Localizer.DataExport
{
    public class BasicExportConfig : ExportConfig
    {
        /// <summary>
        /// If the ModTranslation already has a translation of current culture,
        /// then export it.
        /// </summary>
        public bool WithTranslation { get; set; }
    }
}