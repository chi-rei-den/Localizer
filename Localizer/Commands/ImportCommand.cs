using System.Linq;
using Terraria.ModLoader;

namespace Localizer.Commands
{
    public class ImportCommand : ModCommand
    {
        public override string Command => "import";
        public override CommandType Type => CommandType.Chat;
        public override string Usage => "import mod";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length != 1)
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
            PackageManager.Import(mod);

            caller.Reply("Imported");
        }
    }
}