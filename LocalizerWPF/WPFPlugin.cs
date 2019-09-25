using System.Threading;
using System.Windows;
using Localizer;
using Terraria;
using PlayerInput = Terraria.GameInput.PlayerInput;

namespace LocalizerWPF
{
    public class WPFPlugin : Plugin
    {
        private Thread _thread;
        private Application app;

        public override void Initialize()
        {
            Localizer.Localizer.Kernel.Load(new[] {new WPFModule()});

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
                    if (Localizer.Localizer.Config.ShowUI)
                    {
                        app.MainWindow.Show();
                        Localizer.Localizer.Log.Info("Window showed.");
                    }
                });
            }
        }

        protected override void OnDispose()
        {
            app.Dispatcher.Invoke(() =>
            {
                app.MainWindow?.Close();
                Localizer.Localizer.Log.Info("Window closed.");
            });
        }
    }
}
