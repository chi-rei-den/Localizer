using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight.Ioc;
using LocalizerWPF.Model;
using LocalizerWPF.ViewModel;
using MahApps.Metro.Controls;

namespace LocalizerWPF
{
    /// <summary>
    ///     Interaction logic for ModTranslationManager.xaml
    /// </summary>
    public partial class ManagerView : UserControl
    {
        public ManagerView()
        {
            InitializeComponent();
            PackageDataGrid.MouseLeftButtonDown += (e, a) =>
            {
                var dg = a.OriginalSource as FrameworkElement;
                while (dg != null && !(dg is DataGridRow))
                {
                    dg = VisualTreeHelper.GetParent(dg) as FrameworkElement;
                }

                if (dg is DataGridRow row)
                {
                    row.IsSelected = !row.IsSelected;
                }
            };
        }

        private void ToggleSwitch_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var toggle = sender as ToggleSwitch;
                (toggle.Tag as Package).Enabled = toggle.IsChecked ?? true;
                SimpleIoc.Default.GetInstance<ManagerViewModel>().SaveStateCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Localizer.Utils.LogError(ex);
            }
        }

        private void OnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ManagerViewModel>().SaveStateCommand.Execute(null);
        }
    }
}
