using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        private ObservableCollection<PackageGroup> _packageGroups;
        
        public ObservableCollection<PackageGroup> PackageGroups
        {
            get => _packageGroups = new ObservableCollection<PackageGroup>(PackageManager.PackageGroups);
            set => PackageManager.PackageGroups = (_packageGroups = value).ToList();
        }
        
        public RelayCommand ReloadCommand { get; private set; }
        
        public RelayCommand ImportAllCommand { get; private set; }
        
        public RelayCommand RevertCommand { get; private set; }
        
        public ManagerViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                _packageGroups = new ObservableCollection<PackageGroup>();
                
                PackageManager.LoadPackages();
            }
            
            ReloadCommand = new RelayCommand(Reload, () => !PackageManager.Loading);
            ImportAllCommand = new RelayCommand(ImportAll, () => !PackageManager.Importing);
            RevertCommand = new RelayCommand(Revert);
            
            
        }
        
        private void ImportAll()
        {
            PackageManager.ImportAll();
            Localizer.Localizer.Log.Debug("All Packages Imported");
        }

        private void Reload()
        {
            PackageManager.LoadPackages();
            _packageGroups.Clear();
            PackageManager.PackageGroups.ForEach(pg => _packageGroups.Add(pg));
            Localizer.Localizer.Log.Debug("Packages Reloaded");
        }

        private void Revert()
        {
            PackageManager.Revert();
            Localizer.Localizer.Log.Debug("Translation Reverted");
        }
    }
}