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
using Localizer.ServiceInterfaces;
using Localizer.ServiceInterfaces.Package;
using Localizer.Services.File;
using Localizer.Services.Package;
using LocalizerWPF.Model;
using Microsoft.Win32;
using Ninject;
using static LocalizerWPF.Lang;

namespace LocalizerWPF.ViewModel
{
    public class MakeViewModel : ViewModelBase
    {
        private readonly IFileLoadService fileLoadService;
        private readonly IFileSaveService fileSaveService;
        private readonly IPackageExportService packageExportService;

        private IPackageManageService packageManageService;
        private readonly IPackagePackService packagePackService;
        private readonly IPackageSaveService packageSaveService;
        private readonly IPackageUpdateService packageUpdateService;
        private readonly IPackageLoadService<Package> sourcePackageLoadServiceService;

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

            packageManageService = Localizer.Localizer.Kernel.Get<IPackageManageService>();
            packageExportService = Localizer.Localizer.Kernel.Get<IPackageExportService>();
            packagePackService = Localizer.Localizer.Kernel.Get<IPackagePackService>();
            packageSaveService = Localizer.Localizer.Kernel.Get<IPackageSaveService>();
            packageUpdateService = Localizer.Localizer.Kernel.Get<IPackageUpdateService>();
            fileSaveService = Localizer.Localizer.Kernel.Get<IFileSaveService>();
            sourcePackageLoadServiceService = Localizer.Localizer.Kernel.Get<SourcePackageLoadService<Package>>();
            fileLoadService = Localizer.Localizer.Kernel.Get<IFileLoadService>();
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
                    oldPack = sourcePackageLoadServiceService.Load(dirPath, fileLoadService);
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
                    var updateLogger = Localizer.Localizer.Kernel.Get<IUpdateLogService>();
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
                var dialog = new OpenFileDialog();
                dialog.Title = _("Select-Package-Window-Title");
                dialog.Filter = _("Select-Package-Window-Filter");

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
    }
}
