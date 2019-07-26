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
    public partial class ManagerView : UserControl
    {
        public ManagerView()
        {
            this.InitializeComponent();
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
    }
}
