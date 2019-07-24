using Localizer;
using Localizer.DataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LocalizerWPF
{
    /// <summary>
    /// Interaction logic for ModTranslationManager.xaml
    /// </summary>
    public partial class ModTranslationManager : UserControl
    {
        private List<ModTranslation> _modTranslations;
        public List<ModTranslation> ModTranslations
        {
            get => this._modTranslations;
            set => this.DataGrid.ItemsSource = this._modTranslations = value;
        }
        public ModTranslationManager()
        {
            this.InitializeComponent();
            this.DataGrid.MouseLeftButtonUp += (e, a) =>
            {
                var dg = a.OriginalSource as FrameworkElement;
                while (dg != null && !(dg is DataGridRow))
                {
                    dg = VisualTreeHelper.GetParent(dg) as FrameworkElement;
                }
                if ((dg as DataGridRow)?.BindingGroup?.Items[0] == this.DataGrid.SelectedItem)
                {
                    this.DataGrid.RowDetailsVisibilityMode = this.DataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected
                        ? DataGridRowDetailsVisibilityMode.Collapsed
                        : DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                }
            };
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            PackageManager.LoadPackages();
            this.ModTranslations = PackageManager.PackageGroups.Select(i => new ModTranslation(i.Mod.Name) { ModTranslations = new ObservableCollection<Package>(i.Packages) }).ToList();
        }
    }
}
