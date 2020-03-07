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

        public string[] ModListMirror { get; set; } = new[]
        {
            "mirror.sgkoi.dev",
            "mirror4.sgkoi.dev",
            "mirror2.sgkoi.dev",
            "mirror3.sgkoi.dev",
            "mirror5.sgkoi.dev",
        };

        public string[] ModDownloadMirror { get; set; } = new[]
        {
            "mirror.sgkoi.dev",
            "mirror4.sgkoi.dev",
            "mirror3.sgkoi.dev",
            "mirror2.sgkoi.dev",
            "mirror5.sgkoi.dev",
        };

        public string[] ModDescMirror { get; set; } = new[]
        {
            "mirror.sgkoi.dev",
            "mirror4.sgkoi.dev",
            "mirror2.sgkoi.dev",
            "mirror5.sgkoi.dev",
        };
    }
}
