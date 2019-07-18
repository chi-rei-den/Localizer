using System;
using System.Collections.Generic;
using Localizer.DataModel;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicBuffExporter : Exporter
    {
        public BasicBuffExporter(BasicExportConfig config)
        {
            Config = config;
            logger = new LocalizerLogger(dirPath);
        }

        protected override Type fileType => typeof(BasicBuffFile);
        
        protected override File Extract()
        {
            if(Config.Package?.Mod == null)
                return null;
            
            var buffs = typeof(Mod).GetFieldDirectly<Dictionary<string, ModBuff>>(Config.Package?.Mod, "buffs");

            if (buffs == null)
                 return null;
            
            var file = new BasicBuffFile();
            var config = (BasicExportConfig) Config;
            
            file.Buffs = new Dictionary<string, BuffEntry>();

            foreach (var buff in buffs)
            {
                file.Buffs.Add(buff.Key, new BuffEntry
                {
                    Name = new BaseEntry
                    {
                        Origin = buff.Value.DisplayName?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? buff.Value.DisplayName?.GetTranslation(Config.Package.Language) : ""
                    },
                    Description = new BaseEntry
                    {
                        Origin = buff.Value.Description?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? buff.Value.Description?.GetTranslation(Config.Package.Language) : ""
                    }
                });
            }

            return file;
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            var oldE = oldEntry as BuffEntry;
            var newE = newEntry as BuffEntry;
            
            if(oldE == null || newE == null)
                return;

            if (oldE.Name.Origin != newE.Name.Origin)
            {
                logger.TextUpdateLog($"Buff {key} Name Changed, Old: [{oldE.Name.Origin}], New: [{newE.Name.Origin}]");

                oldE.Name.Origin = newE.Name.Origin;
            }
            
            if (oldE.Description.Origin != newE.Description.Origin)
            {
                logger.TextUpdateLog($"Buff {key} Description Changed, Old: [{oldE.Description.Origin}], New: [{newE.Description.Origin}]");

                oldE.Description.Origin = newE.Description.Origin;
            }
        }
    }
}