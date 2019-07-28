using Localizer.DataModel;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using File = Localizer.DataModel.File;

namespace Localizer.DataExport
{
    public class BasicItemExporter : Exporter
    {
        public BasicItemExporter(ExportConfig config)
        {
            this.Config = config;
            this.logger = new LocalizerLogger(this.dirPath);
        }

        protected override Type fileType => typeof(BasicItemFile);

        protected override File Extract()
        {
            if (this.Config.Package?.Mod == null)
            {
                return null;
            }

            var items = typeof(Mod).GetFieldDirectly<Dictionary<string, ModItem>>(this.Config.Package?.Mod, "items");

            if (items == null)
            {
                return null;
            }

            var file = new BasicItemFile();
            var config = (BasicExportConfig) this.Config;

            file.Items = new Dictionary<string, ItemEntry>();

            foreach (var item in items)
            {
                file.Items.Add(item.Key, new ItemEntry
                {
                    Name = new BaseEntry
                    {
                        Origin = item.Value.DisplayName?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? item.Value.DisplayName?.GetTranslation(this.Config.Package.Language) : ""
                    },
                    Tooltip = new BaseEntry
                    {
                        Origin = item.Value.Tooltip?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? item.Value.Tooltip?.GetTranslation(this.Config.Package.Language) : ""
                    }
                });
            }

            return file;
        }

        protected override void AddEntry(string key, IEntry entry)
        {
            if (!(entry is ItemEntry e))
            {
                return;
            }

            this.logger.TextUpdateLog($"New Item [{key}], Name: [{e.Name.Origin}, Tooltip: [{e.Tooltip.Origin}]]");
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            if (!(oldEntry is ItemEntry oldE) || !(newEntry is ItemEntry newE))
            {
                return;
            }

            if (oldE.Name.Origin != newE.Name.Origin)
            {
                this.logger.TextUpdateLog($"Item {key} Name Changed, Old: [{oldE.Name.Origin}], New: [{newE.Name.Origin}]");

                oldE.Name.Origin = newE.Name.Origin;
            }

            if (oldE.Tooltip.Origin != newE.Tooltip.Origin)
            {
                this.logger.TextUpdateLog($"Item {key} Tooltip Changed, Old: [{oldE.Tooltip.Origin}], New: [{newE.Tooltip.Origin}]");

                oldE.Tooltip.Origin = newE.Tooltip.Origin;
            }
        }
    }
}