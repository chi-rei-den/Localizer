using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
        
        public RelayCommand ReloadCommand { get; private set; }
        
        public RelayCommand ImportAllCommand { get; private set; }
        
        public ManagerViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                PackageGroups = new ObservableCollection<PackageGroup>();
                
                PackageManager.LoadPackages();
            }
            
            ReloadCommand = new RelayCommand(Reload, () => !PackageManager.Loading);
            ImportAllCommand = new RelayCommand(ImportAll, () => !PackageManager.Importing);
        }

        private void ImportAll()
        {
            PackageManager.ImportAll();
            Localizer.Localizer.Log.Debug("All Packages Imported");
        }

        private void Reload()
        {
            PackageManager.LoadPackages();
            Localizer.Localizer.Log.Debug("Packages Reloaded");
        }
    }
}