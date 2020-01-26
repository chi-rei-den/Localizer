namespace Localizer
{
    public class Configuration
    {
        public bool AutoImport { get; set; } = true;

        public AutoImportType AutoImportType { get; set; } = AutoImportType.All;

        public bool ShowUI { get; set; } = true;

        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        public bool RebuildTooltips { get; set; } = true;

        public bool RebuildTooltipsOnce { get; set; } = true;
    }
}
