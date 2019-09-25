using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.Package;
using Localizer.Services.File;
using Localizer.Services.Package;
using LocalizerWPF.Model;
using Ninject;
using Terraria.Localization;

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        private readonly IFileLoadService fileLoadService;
        private readonly IPackageImportService packageImportService;
        private readonly IPackageManageService packageManageService;
        private readonly IPackageLoadService<Package> packedPackageLoadServiceService;
        private readonly IPackageLoadService<Package> sourcePackageLoadServiceService;

        private bool loading = false;
        private bool importing = false;

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

            ReloadCommand = new RelayCommand(Reload, () => !loading && !importing);
            SaveStateCommand = new RelayCommand(SaveState, () => !loading && !importing);
            ImportAllCommand = new RelayCommand(ImportAll, () => !loading && !importing);
            RevertCommand = new RelayCommand(Revert);
            
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                LoadPackages();
                if (Localizer.Localizer.Config.AutoImport)
                {
                    ImportAll();
                }
            });
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
            importing = true;

            try
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

                Localizer.Localizer.RefreshLanguages();

                Utils.LogDebug("All Packages Imported");
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
            finally
            {
                importing = false;
            }
        }

        private void Reload()
        {
            packageManageService.PackageGroups.Clear();
            LoadPackages();
            Utils.LogDebug("Packages Reloaded");
        }

        private void LoadPackages()
        {
            loading = true;

            try
            {
                foreach (var dir in new DirectoryInfo(Localizer.Localizer.SourcePackageDirPath).GetDirectories())
                {
                    var pack = sourcePackageLoadServiceService.Load(dir.FullName, fileLoadService);
                    if(pack == null)
                        continue;
                    packageManageService.AddPackage(pack);
                }

                foreach (var file in new DirectoryInfo(Localizer.Localizer.DownloadPackageDirPath).GetFiles())
                {
                    var pack = packedPackageLoadServiceService.Load(file.FullName, fileLoadService);
                    if(pack == null)
                        continue;
                    packageManageService.AddPackage(pack);
                }

                foreach (var pg in PackageGroups)
                {
                    pg.Packages = new ObservableCollection<IPackage>(pg.Packages);
                }

                packageManageService.LoadState();
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
            finally
            {
                loading = false;
            }
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
