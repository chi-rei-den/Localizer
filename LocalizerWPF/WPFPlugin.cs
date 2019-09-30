using System.Threading;
using System.Windows;
using Localizer;
using LocalizerWPF.ViewModel;

namespace LocalizerWPF
{
    public class WPFPlugin : Plugin
    {
        public override void Initialize()
        {
            Localizer.Localizer.Kernel.Load(new[] { new WPFModule() });

            if (Application.Current == null)
            {
                var thread = new Thread(() =>
                {
                    var a = new App();
                    a.InitializeComponent();
                    a.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    a.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;

                thread.Start();
            }
            else
            {
                var app = Application.Current;
                app.Dispatcher.Invoke(() =>
                {
                    var locator = app.TryFindResource("Locator");
                    if (locator != null)
                    {
                        app.Resources.Remove("Locator");
                    }
                    app.Resources.Add("Locator", new ViewModelLocator());
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
            var app = Application.Current;
            app?.Dispatcher.Invoke(() =>
            {
                app.MainWindow?.Close();
            });
            app?.Dispatcher.Thread.Join(1000);

        }
    }
}
