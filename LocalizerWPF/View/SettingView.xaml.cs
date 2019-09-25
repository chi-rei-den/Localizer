using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Ioc;
using LocalizerWPF.ViewModel;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;

namespace LocalizerWPF.View
{
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
        }
        
        private void AutoImport_OnClick(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            
            var setting = ServiceLocator.Current.GetInstance<SettingViewModel>();

            setting.LocalizerConfig.AutoImport = toggle.IsChecked ?? true;
            
            Localizer.Localizer.SaveConfig();
        }
    }
}

