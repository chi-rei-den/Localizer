using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Localizer;
using Localizer.DataModel;

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        public ObservableCollection<PackageGroup> PackageGroups
        {
            get => new ObservableCollection<PackageGroup>(PackageManager.PackageGroups);
            set => PackageManager.PackageGroups = value.ToList();
        }
        
        public ManagerViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
//                PackageGroups = new ObservableCollection<PackageGroup>();
//                
//                PackageManager.LoadPackages();
                Localizer.Localizer.Log.Info("ajkhsgbdljauhwgdfliy");
            }
        }
    }
}