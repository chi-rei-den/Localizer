using Localizer.DataModel;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicBuffExporter : Exporter
    {
        public BasicBuffExporter(ExportConfig config)
        {
            this.Config = config;
            this.logger = new LocalizerLogger(this.dirPath);
        }

        protected override Type fileType => typeof(BasicBuffFile);

        protected override File Extract()
        {
            if (this.Config.Package?.Mod == null)
            {
                return null;
            }

            var buffs = typeof(Mod).GetFieldDirectly<Dictionary<string, ModBuff>>(this.Config.Package?.Mod, "buffs");

            if (buffs == null)
            {
                return null;
            }

            var file = new BasicBuffFile();
            var config = (BasicExportConfig) this.Config;

            file.Buffs = new Dictionary<string, BuffEntry>();

            foreach (var buff in buffs)
            {
                file.Buffs.Add(buff.Key, new BuffEntry
                {
                    Name = new BaseEntry
                    {
                        Origin = buff.Value.DisplayName?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? buff.Value.DisplayName?.GetTranslation(this.Config.Package.Language) : ""
                    },
                    Description = new BaseEntry
                    {
                        Origin = buff.Value.Description?.DefaultOrEmpty(),
                        Translation = config.WithTranslation ? buff.Value.Description?.GetTranslation(this.Config.Package.Language) : ""
                    }
                });
            }

            return file;
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            if (!(oldEntry is BuffEntry oldE) || !(newEntry is BuffEntry newE))
            {
                return;
            }

            if (oldE.Name.Origin != newE.Name.Origin)
            {
                this.logger.TextUpdateLog($"Buff {key} Name Changed, Old: [{oldE.Name.Origin}], New: [{newE.Name.Origin}]");

                oldE.Name.Origin = newE.Name.Origin;
            }

            if (oldE.Description.Origin != newE.Description.Origin)
            {
                this.logger.TextUpdateLog($"Buff {key} Description Changed, Old: [{oldE.Description.Origin}], New: [{newE.Description.Origin}]");

                oldE.Description.Origin = newE.Description.Origin;
            }
        }
    }
}