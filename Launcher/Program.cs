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
    }
}
