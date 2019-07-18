using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Localizer.DataModel;
using Terraria.ModLoader;
using File = Localizer.DataModel.File;

namespace Localizer.DataExport
{
    public class BasicItemExporter : Exporter
    {
        public BasicItemExporter(BasicExportConfig config)
        {
            Config = config;
            logger = new LocalizerLogger(dirPath);
        }

        protected override Type fileType => typeof(BasicItemFile);
        
        protected override File Extract()
        {
            if(Config.Package?.Mod == null)
                return null;
            
            var items = typeof(Mod).GetFieldDirectly<Dictionary<string, ModItem>>(Config.Package?.Mod, "items");

            if (items == null)
                 return null;
            
            var file = new BasicItemFile();
            var config = (BasicExportConfig) Config;
            
            file.Items = new Dictionary<string, ItemEntry>();

            foreach (var item in items)
            {
                file.Items.Add(item.Key, new ItemEntry
                {
                    Name = new BaseEntry
                    {
                        Origin = item.Value.DisplayName?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? item.Value.DisplayName?.GetTranslation(Config.Package.Language) : ""
                    },
                    Tooltip = new BaseEntry
                    {
                        Origin = item.Value.Tooltip?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? item.Value.Tooltip?.GetTranslation(Config.Package.Language) : ""
                    }
                });
            }

            return file;
        }

        protected override void AddEntry(string key, IEntry entry)
        {
            var e = entry as ItemEntry;
            if(e == null)
                return;
            
            logger.TextUpdateLog($"New Item [{key}], Name: [{e.Name.Origin}, Tooltip: [{e.Tooltip.Origin}]]");
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            var oldE = oldEntry as ItemEntry;
            var newE = newEntry as ItemEntry;
            
            if(oldE == null || newE == null)
                return;

            if (oldE.Name.Origin != newE.Name.Origin)
            {
                logger.TextUpdateLog($"Item {key} Name Changed, Old: [{oldE.Name.Origin}], New: [{newE.Name.Origin}]");

                oldE.Name.Origin = newE.Name.Origin;
            }
            
            if (oldE.Tooltip.Origin != newE.Tooltip.Origin)
            {
                logger.TextUpdateLog($"Item {key} Tooltip Changed, Old: [{oldE.Tooltip.Origin}], New: [{newE.Tooltip.Origin}]");

                oldE.Tooltip.Origin = newE.Tooltip.Origin;
            }
        }
    }
}