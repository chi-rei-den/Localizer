using System;
using System.Collections.Generic;
using Localizer.DataModel;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicCustomExporter : Exporter
    {
        public BasicCustomExporter(BasicExportConfig config)
        {
            Config = config;
            logger = new LocalizerLogger(dirPath);
        }
        
        protected override Type fileType => typeof(BasicCustomFile);

        protected override File Extract()
        {
            if(Config.Package?.Mod == null)
                return null;
            
            var translations = typeof(Mod).GetFieldDirectly<Dictionary<string, ModTranslation>>(Config.Package?.Mod, "translations");

            if (translations == null)
                return null;
            
            var file = new BasicCustomFile();
            var config = (BasicExportConfig) Config;
            
            file.Entries = new Dictionary<string, BaseEntry>();

            foreach (var translation in translations)
            {
                file.Entries.Add(translation.Key, new BaseEntry
                {
                    Origin = translation.Value.DefaultOrEmpty(),
                    Translation = config.WithTranslation ? translation.Value?.GetTranslation(Config.Package.Language) : ""
                });
            }

            return file;
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            var oldE = oldEntry as BaseEntry;
            var newE = newEntry as BaseEntry;
            
            if(oldE == null || newE == null)
                return;

            if (oldE.Origin != newE.Origin)
            {
                logger.TextUpdateLog($"Custom {key} Changed, Old: [{oldE.Origin}], New: [{newE.Origin}]");

                oldE.Origin = newE.Origin;
            }
        }
    }
}