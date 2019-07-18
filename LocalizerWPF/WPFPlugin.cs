using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Localizer;

namespace LocalizerWPF
{
    public class WPFPlugin : Plugin
    {
        private Thread _thread;
        private Application app;

        public override void Initialize()
        {
            if (Application.Current == null)
            {
                _thread = new Thread(() =>
                {
                    var a = new App();
                    a.InitializeComponent();
                    app = a;
                    app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    app.Run();
                });
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.IsBackground = true;

                _thread.Start();
            }
            else
            {
                app = Application.Current;
                app.Dispatcher.Invoke(() =>
                {
                    app.MainWindow = new MainWindow();
                    app.MainWindow.Show();
                });
            }
        }

        protected override void OnDispose()
        {
            app.Dispatcher.Invoke(() =>
            {
                app.MainWindow?.Close();
            });
        }
    }
}
