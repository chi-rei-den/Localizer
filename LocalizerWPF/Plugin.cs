using System.Threading;
using System.Windows;
using Localizer;
using LocalizerWPF.ViewModel;

namespace LocalizerWPF
{
    public class Plugin : LocalizerPlugin
    {
        public static LocalizerKernel Kernel { get; private set; }
        
        public override string Name => "LocalizerUI-WPF";
        public override string Author => "Chireiden";
        public override string Description => "An official UI component of Localizer.";

        public override void Initialize(LocalizerKernel kernel)
        {
            Kernel = kernel;
            kernel.Load(new[] { new WPFModule() });

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

        protected override void DisposeUnmanaged()
        {
            var app = Application.Current;
            app?.Dispatcher.Invoke(() =>
            {
                app.MainWindow?.Close();
            });
        }
    }
}
