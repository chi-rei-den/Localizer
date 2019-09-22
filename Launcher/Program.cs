using System;
using System.IO;
using LocalizerWPF;

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
