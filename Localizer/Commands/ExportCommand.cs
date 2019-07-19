using Localizer.DataExport;
using Localizer.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria.ModLoader;

namespace Localizer.Commands
{
    public class ExportCommand : ModCommand
    {
        public override string Command => "export";
        public override CommandType Type => CommandType.Chat;
        public override string Usage => "export modname packname language";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length != 3)
            {
                caller.Reply("Wrong args!");
                return;
            }

            var mod = ModLoader.Mods.FirstOrDefault(m => m.Name == args[0]);
            if (mod == null)
            {
                caller.Reply("Wrong mod name!");
                return;
            }

            var packname = args[1];
            var lang = CultureInfo.GetCultureInfo(args[2]);


            PackageManager.Export(mod, packname, lang, new Dictionary<Type, ExportConfig>
            {
                [typeof(BasicItemFile)] = new BasicExportConfig
                {
                    MakeBackup = true,
                    WithTranslation = true
                },
                [typeof(BasicNPCFile)] = new BasicExportConfig
                {
                    MakeBackup = true,
                    WithTranslation = true
                },
                [typeof(BasicBuffFile)] = new BasicExportConfig
                {
                    MakeBackup = true,
                    WithTranslation = true
                },
                [typeof(BasicCustomFile)] = new BasicExportConfig
                {
                    MakeBackup = true,
                    WithTranslation = true
                },
                [typeof(LdstrFile)] = new LdstrExportConfig
                {
                    MakeBackup = true,
                },
            });

            caller.Reply("Exported");
        }
    }
}