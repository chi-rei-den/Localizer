using System;
using System.Collections.Generic;
using Localizer.DataModel;
using Terraria.ModLoader;

namespace Localizer.DataExport
{
    public class BasicNPCExporter : Exporter
    {
        public BasicNPCExporter(BasicExportConfig config)
        {
            Config = config;
            logger = new LocalizerLogger(dirPath);
        }

        protected override Type fileType => typeof(BasicNPCFile);

        protected override File Extract()
        {
            if(Config.Package?.Mod == null)
                return null;
            
            var npcs = typeof(Mod).GetFieldDirectly<Dictionary<string, ModNPC>>(Config.Package?.Mod, "npcs");

            if (npcs == null)
                 return null;
            
            var file = new BasicNPCFile();
            var config = (BasicExportConfig) Config;
            
            file.Names = new Dictionary<string, BaseEntry>();

            foreach (var npc in npcs)
            {
                file.Names.Add(npc.Key, new BaseEntry
                {
                    Origin = npc.Value.DisplayName?.DefaultOrEmpty(),
                    Translation = config.WithTranslation ? npc.Value.DisplayName?.GetTranslation(Config.Package.Language) : ""
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
                logger.TextUpdateLog($"NPC {key} Name Changed, Old: [{oldE.Origin}], New: [{newE.Origin}]");

                oldE.Origin = newE.Origin;
            }
        }
    }
}