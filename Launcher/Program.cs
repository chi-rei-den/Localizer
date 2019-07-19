using LocalizerWPF;
using System;
using System.IO;

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
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        // For reading modules from solution directory, need a better and more elegant approach to implement this.
        private static void GenBuildPath()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "localizer_build_path");

            File.WriteAllText(filePath, Path.GetFullPath("../../../Localizer/Plugins/"));
        }
    }
}
