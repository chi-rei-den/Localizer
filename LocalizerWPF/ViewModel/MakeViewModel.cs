using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.Package;
using Localizer.Package.Export;
using Localizer.Package.Load;
using Localizer.Package.Pack;
using Localizer.Package.Save;
using Localizer.Package.Update;
using LocalizerWPF.Model;
using Microsoft.Win32;
using Ninject;
using static LocalizerWPF.Lang;

namespace LocalizerWPF.ViewModel
{
    public class MakeViewModel : ViewModelBase
    {
        private IFileLoadService fileLoadService;
        private IFileSaveService fileSaveService;
        private IPackageExportService packageExportService;

        private IPackageManageService packageManageService;
        private IPackagePackService packagePackService;
        private IPackageSaveService packageSaveService;
        private IPackageUpdateService packageUpdateService;
        private IPackageLoadService<Package> sourcePackageLoadService;

        public ObservableCollection<IMod> Mods
        {
            get
            {
                var mods = Utils.GetLoadedMods();
                if (mods == null)
                {
                    return new ObservableCollection<IMod>();
                }

                return new ObservableCollection<IMod>(mods);
            }
        }

        public IMod SelectedMod { get; set; }

        public string PackageName { get; set; }

        public CultureInfo SelectedLanguage { get; set; }

        public ObservableCollection<CultureInfo> Languages { get; set; }

        public bool MakeBackup { get; set; }

        public bool ForceOverride { get; set; }

        public bool WithTranslation { get; set; }

        public RelayCommand ExportCommand { get; }
        public RelayCommand PackUpCommand { get; }
        public RelayCommand RefreshCommand { get; }


        public MakeViewModel()
        {
            PackageName = _("Enter-Package-Name-Placeholder");

            Languages = new ObservableCollection<CultureInfo>(
                CultureInfo.GetCultures(CultureTypes.AllCultures));

            SelectedLanguage = CultureInfo.CurrentCulture;

            MakeBackup = true;
            ForceOverride = false;
            WithTranslation = true;

            ExportCommand = new RelayCommand(Export);
            PackUpCommand = new RelayCommand(PackUp);
            RefreshCommand = new RelayCommand(Refresh);

            packageManageService = Plugin.Kernel.Get<IPackageManageService>();
            packageExportService = Plugin.Kernel.Get<IPackageExportService>();
            packagePackService = Plugin.Kernel.Get<IPackagePackService>();
            packageSaveService = Plugin.Kernel.Get<IPackageSaveService>();
            packageUpdateService = Plugin.Kernel.Get<IPackageUpdateService>();
            fileSaveService = Plugin.Kernel.Get<IFileSaveService>();
            sourcePackageLoadService = Plugin.Kernel.Get<SourcePackageLoad<Package>>();
            fileLoadService = Plugin.Kernel.Get<IFileLoadService>();
        }

        private void Export()
        {
            try
            {
                if (SelectedMod == null)
                {
                    MessageBox.Show(_("No-Selected-Mod-Error-Message"));
                    return;
                }

                if (string.IsNullOrWhiteSpace(PackageName))
                {
                    MessageBox.Show(_("No-Package-Name-Error-Message"));
                    return;
                }

                var package = new Package
                {
                    Name = PackageName,
                    Language = SelectedLanguage,
                    ModName = SelectedMod.Name,
                    Files = new ObservableCollection<IFile>()
                };

                var dirPath = Utils.EscapePath(Path.Combine(Localizer.Localizer.SourcePackageDirPath, PackageName));

                packageExportService.Export(package,
                                            new ExportConfig
                                            {
                                                ForceOverride = ForceOverride,
                                                MakeBackup = MakeBackup,
                                                WithTranslation = WithTranslation
                                            });

                IPackage oldPack;
                if (Directory.Exists(dirPath))
                {
                    oldPack = sourcePackageLoadService.Load(dirPath, fileLoadService);
                }
                else
                {
                    Utils.CreateDirectory(dirPath);
                    oldPack = null;
                }

                if (MakeBackup && oldPack != null)
                {
                    var backupDirPath = Utils.EscapePath(Path.Combine(dirPath, "Backups", Utils.DateTimeToFileName(DateTime.Now)));
                    Utils.CreateDirectory(backupDirPath);
                    packageSaveService.Save(oldPack, backupDirPath, fileSaveService);

                    Utils.LogDebug($"[{PackageName}'s backup created.]");
                }

                if (!ForceOverride && oldPack != null)
                {
                    var updateLogger = Plugin.Kernel.Get<IUpdateLogger>();
                    updateLogger.Init($"{package.Name}-{Utils.DateTimeToFileName(DateTime.Now)}");

                    packageUpdateService.Update(oldPack, package, updateLogger);

                    packageSaveService.Save(oldPack, dirPath, fileSaveService);
                }
                else
                {
                    packageSaveService.Save(package, dirPath, fileSaveService);
                }

                MessageBox.Show(string.Format(_("Package-Exported-Success-Message"), PackageName));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Localizer.Localizer.Log.Error(e);
            }
        }

        private void PackUp()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = _("Select-Package-Window-Title"),
                    Filter = _("Select-Package-Window-Filter")
                };

                if (dialog.ShowDialog() ?? false)
                {
                    var file = dialog.FileName;

                    var fileInfo = new FileInfo(file);

                    packagePackService.Pack(fileInfo.FullName);

                    MessageBox.Show(_("Package-Created-Success-Message"));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Refresh()
        {
            RaisePropertyChanged("Mods");
        }
    }
}
