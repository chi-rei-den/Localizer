using System;
using System.Collections.ObjectModel;
using System.IO;
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

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        private IFileLoadService fileLoadService;
        private IPackageImportService packageImportService;
        private IPackageManageService packageManageService;
        private IPackageLoadService<Package> packedPackageLoadServiceService;
        private IPackageLoadService<Package> sourcePackageLoadServiceService;

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
                packageManageService = Plugin.Kernel.Get<IPackageManageService>();
                sourcePackageLoadServiceService = Plugin.Kernel.Get<SourcePackageLoad<Package>>();
                packedPackageLoadServiceService = Plugin.Kernel.Get<PackedPackageLoad<Package>>();
                packageImportService = Plugin.Kernel.Get<IPackageImportService>();
                fileLoadService = Plugin.Kernel.Get<IFileLoadService>();

                packageManageService.PackageGroups = new ObservableCollection<IPackageGroup>();
            }

            ReloadCommand = new RelayCommand(Reload, () => !loading && !importing);
            SaveStateCommand = new RelayCommand(SaveState, () => !loading && !importing);
            ImportAllCommand = new RelayCommand(ImportAll, () => !loading && !importing);

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
            get => packageManageService?.PackageGroups as ObservableCollection<IPackageGroup>;
            set => packageManageService.PackageGroups = value;
        }

        public RelayCommand ReloadCommand { get; }

        public RelayCommand SaveStateCommand { get; }

        public RelayCommand ImportAllCommand { get; }

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

                packageImportService.Import(false);

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
                    Utils.SafeWrap(() =>
                    {
                        var pack = sourcePackageLoadServiceService.Load(dir.FullName, fileLoadService);
                        if (pack == null)
                        {
                            return;
                        }

                        packageManageService.AddPackage(pack);
                    });
                }

                foreach (var file in new DirectoryInfo(Localizer.Localizer.DownloadPackageDirPath).GetFiles())
                {
                    Utils.SafeWrap(() =>
                    {
                        var pack = packedPackageLoadServiceService.Load(file.FullName, fileLoadService);
                        if (pack == null)
                        {
                            return;
                        }

                        packageManageService.AddPackage(pack);
                    });
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
    }
}
