using Localizer;
using System.Threading;
using System.Windows;

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
                this._thread = new Thread(() =>
                {
                    var a = new App();
                    a.InitializeComponent();
                    this.app = a;
                    this.app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    this.app.Run();
                });
                this._thread.SetApartmentState(ApartmentState.STA);
                this._thread.IsBackground = true;

                this._thread.Start();
            }
            else
            {
                this.app = Application.Current;
                this.app.Dispatcher.Invoke(() =>
                {
                    this.app.MainWindow = new MainWindow();
                    this.app.MainWindow.Show();
                });
            }
        }

        protected override void OnDispose()
        {
            this.app.Dispatcher.Invoke(() =>
            {
                this.app.MainWindow?.Close();
            });
        }
    }
}
