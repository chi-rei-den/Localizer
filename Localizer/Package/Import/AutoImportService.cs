using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Localizer.Package.Load;
using Localizer.Package.Pack;
using Localizer.UIs;
using Ninject;

using PackageModel = Localizer.DataModel.Default.Package;

namespace Localizer.Package.Import
{
    public sealed class AutoImportService : Disposable
    {
        public bool Imported => _imported;

        private readonly IPackageManageService _packageManage;
        private readonly SourcePackageLoad<PackageModel> _sourcePackageLoad;
        private readonly PackedPackageLoad<PackageModel> _packedPackageLoad;
        private readonly IPackageImportService _packageImport;
        private readonly IPackagePackService _packagePack;
        private readonly IFileLoadService _fileLoad;

        private bool _imported = false;

        [Inject]
        public AutoImportService(IPackageManageService packageManage,
                                 SourcePackageLoad<PackageModel> sourcePackageLoad,
                                 PackedPackageLoad<PackageModel> packedPackageLoad,
                                 IPackageImportService packageImport,
                                 IPackagePackService packagePack,
                                 IFileLoadService fileLoad)
        {
            _packageManage = packageManage ?? throw new ArgumentNullException(nameof(packageManage));
            _sourcePackageLoad = sourcePackageLoad ?? throw new ArgumentNullException(nameof(sourcePackageLoad));
            _packedPackageLoad = packedPackageLoad ?? throw new ArgumentNullException(nameof(packedPackageLoad));
            _packageImport = packageImport ?? throw new ArgumentNullException(nameof(packageImport));
            _packagePack = packagePack ?? throw new ArgumentNullException(nameof(packagePack));
            _fileLoad = fileLoad ?? throw new ArgumentNullException(nameof(fileLoad));
            LoadPackages();

            Hooks.BeforeModCtor += OnBeforeModCtor;
            Hooks.PostSetupContent += OnPostSetupContent;
        }

        private void OnBeforeModCtor(object mod)
        {
            if (_imported)
            {
                return;
            }

            try
            {
                if (Localizer.Config.AutoImport)
                {
                    var wrapped = new LoadedModWrapper(mod);
                    Utils.LogInfo($"Early auto import for mod: [{wrapped.Name}]");
                    Import(wrapped);
                }
            }
            catch
            {
            }
        }

        private void OnPostSetupContent()
        {
            if (_imported)
            {
                return;
            }

            try
            {
                if (Localizer.Config.AutoImport)
                {
                    Utils.LogInfo($"Late auto import begin.");
                    Import();
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
            finally
            {
                _imported = true;
            }
        }

        private void LoadPackages()
        {
            try
            {
                _packageManage.PackageGroups = new List<IPackageGroup>();

                var type = Localizer.Config.AutoImportType;

                if (type != AutoImportType.DownloadedOnly)
                {
                    LoadSourcePackages();
                }

                if (type != AutoImportType.SourceOnly)
                {
                    LoadPackedPackages();
                }

                _packageManage.LoadState();

                UIModsPatch.ModsExtraInfo = _packageManage.PackageGroups.ToDictionary(group => group.Mod.Name,
                     group =>
                     {
                         var localizedModName = group.Packages.FirstOrDefault(pack => !string.IsNullOrWhiteSpace(pack.LocalizedModName))?.LocalizedModName;
                         return $"{localizedModName}{Environment.NewLine}{string.Join(Environment.NewLine, group.Packages.Select(UI.GetPkgLabelText))}";
                     });
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
        }

        private void LoadSourcePackages()
        {
            foreach (var dir in new DirectoryInfo(Localizer.SourcePackageDirPath).GetDirectories())
            {
                try
                {
                    var pack = _sourcePackageLoad.Load(dir.FullName, _fileLoad);
                    if (pack != null)
                    {
                        _packageManage.AddPackage(pack);
                        _packagePack.Pack(Path.Combine(dir.FullName, "Package.json"));
                    }
                }
                catch
                {
                }
            }
        }

        private void LoadPackedPackages()
        {
            var list = Directory.GetFiles(Localizer.DownloadPackageDirPath).ToList();
            list.AddRange(Directory.GetFiles(Path.Combine(Terraria.Main.SavePath, "Mods"), "*.locpack"));
            foreach (var file in list)
            {
                try
                {
                    var pack = _packedPackageLoad.Load(file, _fileLoad);
                    if (pack != null)
                    {
                        _packageManage.AddPackage(pack);
                    }
                }
                catch
                {
                }
            }
        }

        private void Import(IMod mod = null)
        {
            try
            {
                _packageImport.Clear();

                if (mod is null)
                {
                    foreach (var pg in _packageManage.PackageGroups)
                    {
                        QueuePackageGroup(pg);
                    }
                }
                else
                {
                    QueuePackageGroup(_packageManage.PackageGroups.FirstOrDefault(pg => pg.Mod.Name == mod.Name));
                }

                _packageImport.Import(true);

                Localizer.RefreshLanguages();

                Utils.LogDebug("Auto import end.");
            }
            catch
            {
            }
        }

        private void QueuePackageGroup(IPackageGroup packageGroup)
        {
            if (packageGroup is null || !packageGroup.Mod.Enabled)
            {
                return;
            }

            foreach (var p in packageGroup.Packages)
            {
                if (p.Enabled)
                {
                    _packageImport.Queue(p);
                }
            }
        }

        protected override void DisposeUnmanaged()
        {
            Hooks.PostSetupContent -= OnPostSetupContent;
            Hooks.BeforeModCtor -= OnBeforeModCtor;
        }
    }
}
