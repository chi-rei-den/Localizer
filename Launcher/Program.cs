using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Localizer;
using LocalizerWPF;
using Terraria.ModLoader;

namespace Launcher
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG
            GenBuildPath();
#endif
            try
            {
                var localizer = new Localizer.Localizer();
                localizer.Load();

                
                var ui = typeof(Mod).Module.GetType("Terraria.ModLoader.UI.Interface");

                var infoField = ui.GetField("infoMessage", BindingFlags.Static | BindingFlags.NonPublic);
                var info = infoField.GetValue(null);
                
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // For reading modules from solution directory, need a better and more elegant approach to implement this.
        private static void GenBuildPath()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        "localizer_build_path");

            File.WriteAllText(filePath, Path.GetFullPath("../../../../Localizer/Plugins/"));
        }
    }
}
