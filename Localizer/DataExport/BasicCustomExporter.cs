using Localizer.DataModel;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicCustomExporter : Exporter
    {
        public BasicCustomExporter(ExportConfig config)
        {
            this.Config = config;
            this.logger = new LocalizerLogger(this.dirPath);
        }

        protected override Type fileType => typeof(BasicCustomFile);

        protected override File Extract()
        {
            if (this.Config.Package?.Mod == null)
            {
                return null;
            }

            var translations = typeof(Mod).GetFieldDirectly<Dictionary<string, ModTranslation>>(this.Config.Package?.Mod, "translations");

            if (translations == null)
            {
                return null;
            }

            var file = new BasicCustomFile();
            var config = (BasicExportConfig) this.Config;

            file.Entries = new Dictionary<string, BaseEntry>();

            foreach (var translation in translations)
            {
                file.Entries.Add(translation.Key, new BaseEntry
                {
                    Origin = translation.Value.DefaultOrEmpty(),
                    Translation = config.WithTranslation ? translation.Value?.GetTranslation(this.Config.Package.Language) : ""
                });
            }

            return file;
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            if (!(oldEntry is BaseEntry oldE) || !(newEntry is BaseEntry newE))
            {
                return;
            }

            if (oldE.Origin != newE.Origin)
            {
                this.logger.TextUpdateLog($"Custom {key} Changed, Old: [{oldE.Origin}], New: [{newE.Origin}]");

                oldE.Origin = newE.Origin;
            }
        }
    }
}