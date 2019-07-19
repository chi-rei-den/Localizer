using Localizer.DataModel;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicNPCExporter : Exporter
    {
        public BasicNPCExporter(BasicExportConfig config)
        {
            this.Config = config;
            this.logger = new LocalizerLogger(this.dirPath);
        }

        protected override Type fileType => typeof(BasicNPCFile);

        protected override File Extract()
        {
            if (this.Config.Package?.Mod == null)
            {
                return null;
            }

            var npcs = typeof(Mod).GetFieldDirectly<Dictionary<string, ModNPC>>(this.Config.Package?.Mod, "npcs");

            if (npcs == null)
            {
                return null;
            }

            var file = new BasicNPCFile();
            var config = (BasicExportConfig) this.Config;

            file.Names = new Dictionary<string, BaseEntry>();

            foreach (var npc in npcs)
            {
                file.Names.Add(npc.Key, new BaseEntry
                {
                    Origin = npc.Value.DisplayName?.DefaultOrEmpty(),
                    Translation = config.WithTranslation ? npc.Value.DisplayName?.GetTranslation(this.Config.Package.Language) : ""
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
                this.logger.TextUpdateLog($"NPC {key} Name Changed, Old: [{oldE.Origin}], New: [{newE.Origin}]");

                oldE.Origin = newE.Origin;
            }
        }
    }
}