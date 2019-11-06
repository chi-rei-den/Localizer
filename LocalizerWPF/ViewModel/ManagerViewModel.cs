using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.Package;
using Localizer.Package.Import;
using Localizer.Package.Load;
using LocalizerWPF.Model;
using Ninject;

namespace LocalizerWPF.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        private IFileLoadService fileLoadService;
        private IPackageImportService packageImportService;
        private IPackageManageService packageManageDisposable;
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
                packageManageDisposable = Plugin.Kernel.Get<IPackageManageService>();
                sourcePackageLoadServiceService = Plugin.Kernel.Get<SourcePackageLoad<Package>>();
                packedPackageLoadServiceService = Plugin.Kernel.Get<PackedPackageLoad<Package>>();
                packageImportService = Plugin.Kernel.Get<IPackageImportService>();
                fileLoadService = Plugin.Kernel.Get<IFileLoadService>();
                packageManageDisposable.PackageGroups = new ObservableCollection<IPackageGroup>();
            }

            ReloadCommand = new RelayCommand(Reload, () => !loading && !importing);
            SaveStateCommand = new RelayCommand(SaveState, () => !loading && !importing);
            ImportAllCommand = new RelayCommand(ImportAll, () => !loading && !importing);

            packageManageDisposable.PackageGroups.Clear();
            LoadPackages();
        }

        public ObservableCollection<IPackageGroup> PackageGroups
        {
            get => packageManageDisposable.PackageGroups as ObservableCollection<IPackageGroup>;
            set => packageManageDisposable.PackageGroups = value;
        }

        public RelayCommand ReloadCommand { get; }

        public RelayCommand SaveStateCommand { get; }

        public RelayCommand ImportAllCommand { get; }

        private void ImportAll()
        {
            importing = true;

            try
            {
                var ss = Plugin.Kernel.GetAll(typeof(FileImporter));
                foreach (var s in ss)
                {
                    s.GetType();
                }
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
            packageManageDisposable.PackageGroups.Clear();
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

                        packageManageDisposable.AddPackage(pack);
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

                        packageManageDisposable.AddPackage(pack);
                    });
                }

                foreach (var pg in PackageGroups)
                {
                    pg.Packages = new ObservableCollection<IPackage>(pg.Packages);
                }

                packageManageDisposable.LoadState();
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
            packageManageDisposable.SaveState();
        }
    }
}
