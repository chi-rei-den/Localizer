using System.ComponentModel;
using LocalizerWPF.ViewModel;

namespace LocalizerWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
        }

        public void OnClosing(object sender, CancelEventArgs args)
        {
            ViewModelLocator.Cleanup();
        }
    }
}
