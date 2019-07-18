using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizerWPF;

namespace Launcher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            GenBuildPath();
#endif
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        // For reading modules from solution directory, need a better and more elegant approach to implement this.
        static void GenBuildPath()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "localizer_build_path");

            File.WriteAllText(filePath, Path.GetFullPath("../../../Localizer/Plugins/"));
        }
    }
}
