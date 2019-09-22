using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.Package;
using Localizer.Services.File;
using Localizer.Services.Package;
using LocalizerWPF.Model;
using Ninject;

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        private readonly IFileLoadService fileLoadService;
        private readonly IPackageImportService packageImportService;
        private readonly IPackageManageService packageManageService;
        private readonly IPackageLoadService<Package> packedPackageLoadServiceService;
        private readonly IPackageLoadService<Package> sourcePackageLoadServiceService;

        public ManagerViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                packageManageService = Localizer.Localizer.Kernel.Get<IPackageManageService>();
                sourcePackageLoadServiceService = Localizer.Localizer.Kernel.Get<SourcePackageLoadService<Package>>();
                packedPackageLoadServiceService = Localizer.Localizer.Kernel.Get<PackedPackageLoadService<Package>>();
                packageImportService = Localizer.Localizer.Kernel.Get<IPackageImportService>();
                fileLoadService = Localizer.Localizer.Kernel.Get<IFileLoadService>();

                packageManageService.PackageGroups = new ObservableCollection<IPackageGroup>();
            }

            ReloadCommand = new RelayCommand(Reload);
            SaveStateCommand = new RelayCommand(SaveState);
            ImportAllCommand = new RelayCommand(ImportAll);
            RevertCommand = new RelayCommand(Revert);

            LoadPackages();
        }

        public ObservableCollection<IPackageGroup> PackageGroups
        {
            get => packageManageService.PackageGroups as ObservableCollection<IPackageGroup>;
            set => packageManageService.PackageGroups = value;
        }

        public RelayCommand ReloadCommand { get; }

        public RelayCommand SaveStateCommand { get; }

        public RelayCommand ImportAllCommand { get; }

        public RelayCommand RevertCommand { get; }

        private void ImportAll()
        {
            packageImportService.Reset();

            foreach (var pg in PackageGroups)
            {
                foreach (var p in pg.Packages)
                {
                    if (p.Enabled)
                    {
                        packageImportService.Queue(p);
                    }
                }
            }

            packageImportService.Import();

            Utils.LogDebug("All Packages Imported");
        }

        private void Reload()
        {
            packageManageService.PackageGroups.Clear();
            LoadPackages();
            Utils.LogDebug("Packages Reloaded");
        }

        private void LoadPackages()
        {
            foreach (var dir in new DirectoryInfo(Localizer.Localizer.SourcePackageDirPath).GetDirectories())
            {
                packageManageService.AddPackage(sourcePackageLoadServiceService.Load(dir.FullName, fileLoadService));
            }

            foreach (var file in new DirectoryInfo(Localizer.Localizer.DownloadPackageDirPath).GetFiles())
            {
                packageManageService.AddPackage(packedPackageLoadServiceService.Load(file.FullName, fileLoadService));
            }

            foreach (var pg in PackageGroups)
            {
                pg.Packages = new ObservableCollection<IPackage>(pg.Packages);
            }

            packageManageService.LoadState();
        }

        private void SaveState()
        {
            packageManageService.SaveState();
        }

        private void Revert()
        {
            Utils.LogDebug("Translation Reverted");
        }
    }
}
