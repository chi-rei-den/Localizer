using System;
using System.Reflection;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Package;
using Localizer.Package.Export;
using Localizer.Package.Import;
using Localizer.Package.Load;
using Localizer.Package.Pack;
using Localizer.Package.Save;
using Localizer.Package.Update;
using Ninject;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultPackageModule : NinjectModule
    {
        public override void Load()
        {
            BindLoadService();
            BindManageService();
            BindImportService();
            BindSaveService();
            BindPackService();
            BindExportService();
            BindUpdateService();
        }

        public override void Unload()
        {
            Kernel.Get<AutoImportService>().Dispose();
            Kernel.Get<RefreshLanguageService>().Dispose();
            Kernel.Get<PackageImportService>().Dispose();
        }

        private void BindLoadService()
        {
            Bind<IFileLoadService>().To<JsonFileLoad>().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<SourcePackageLoad<DataModel.Default.Package>>().InSingletonScope();
            Bind<SourcePackageLoad<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<PackedPackageLoad<DataModel.Default.Package>>().InSingletonScope();
            Bind<PackedPackageLoad<DataModel.Default.Package>>().ToSelf().InSingletonScope();
        }

        private void BindManageService()
        {
            Bind<IPackageManageService>().To<PackageManageService>();
        }

        private void BindImportService()
        {
            Bind<IPackageImportService>().To<PackageImportService>().InSingletonScope();

            var packageImportService = Kernel.Get<IPackageImportService>();
            if (Localizer.Config.ImporBasictAfterSetupContent)
            {
                packageImportService.RegisterImporter<BasicItemFile>(typeof(BasicImporterPostContentLoad<BasicItemFile>));
                packageImportService.RegisterImporter<BasicNPCFile>(typeof(BasicImporterPostContentLoad<BasicNPCFile>));
                packageImportService.RegisterImporter<BasicBuffFile>(typeof(BasicImporterPostContentLoad<BasicBuffFile>));
                packageImportService.RegisterImporter<BasicProjectileFile>(typeof(BasicImporterPostContentLoad<BasicProjectileFile>));
                packageImportService.RegisterImporter<BasicPrefixFile>(typeof(BasicImporterPostContentLoad<BasicPrefixFile>));
            }
            else
            {
                packageImportService.RegisterImporter<BasicItemFile>(typeof(BasicImporterBeforeContentLoad<BasicItemFile>));
                packageImportService.RegisterImporter<BasicNPCFile>(typeof(BasicImporterBeforeContentLoad<BasicNPCFile>));
                packageImportService.RegisterImporter<BasicBuffFile>(typeof(BasicImporterBeforeContentLoad<BasicBuffFile>));
                packageImportService.RegisterImporter<BasicProjectileFile>(typeof(BasicImporterBeforeContentLoad<BasicProjectileFile>));
                packageImportService.RegisterImporter<BasicPrefixFile>(typeof(BasicImporterBeforeContentLoad<BasicPrefixFile>));
            }
            packageImportService.RegisterImporter<CustomModTranslationFile>(typeof(CustomModTranslationImporter));
            if (!string.IsNullOrWhiteSpace(Localizer.Config.LdstrImporter))
            {
                packageImportService.RegisterImporter<LdstrFile>(Assembly.GetExecutingAssembly().GetType($"Localizer.Package.Import.{Localizer.Config.LdstrImporter}LdstrImporter"));
            }
            else
            {
                packageImportService.RegisterImporter<LdstrFile>(typeof(CecilLdstrImporter));
            }

            Bind<AutoImportService>().ToSelf().InSingletonScope();
            Bind<RefreshLanguageService>().ToSelf().InSingletonScope();
        }

        private void BindSaveService()
        {
            Bind<IPackageSaveService>().To<PackageSave>().InSingletonScope();
            Bind<IFileSaveService>().To<JsonFileSaveService>().InSingletonScope();
        }

        private void BindPackService()
        {
            Bind<IPackagePackService>().To<ZipPackagePackService<DataModel.Default.Package>>().InSingletonScope();
            Bind<ZipPackagePackService<DataModel.Default.Package>>().ToSelf().InSingletonScope();
        }

        private void BindExportService()
        {
            Bind<IPackageExportService>().To<PackageExport>().InSingletonScope();

            Bind<IFileExportService>().To<BasicFileExport<BasicItemFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicNPCFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicBuffFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicProjectileFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicPrefixFile>>().InSingletonScope();
            Bind<IFileExportService>().To<CustomModTranslationFileExport>().InSingletonScope();
            Bind<IFileExportService>().To<LdstrFileExport>().InSingletonScope();
        }

        private void BindUpdateService()
        {
            void BindUpdate<T>(Type serviceType) where T : IFile
            {
                var updateService = Kernel.Get<IPackageUpdateService>();

                Bind(typeof(FileUpdater), serviceType).To(serviceType).InSingletonScope();

                updateService.RegisterUpdater<T>(Kernel.Get(serviceType) as FileUpdater);
            }

            Bind<IPackageUpdateService>().To<PackageUpdateService>().InSingletonScope();

            BindUpdate<BasicItemFile>(typeof(BasicFileUpdater<BasicItemFile>));
            BindUpdate<BasicNPCFile>(typeof(BasicFileUpdater<BasicNPCFile>));
            BindUpdate<BasicBuffFile>(typeof(BasicFileUpdater<BasicBuffFile>));
            BindUpdate<BasicProjectileFile>(typeof(BasicFileUpdater<BasicProjectileFile>));
            BindUpdate<BasicPrefixFile>(typeof(BasicFileUpdater<BasicPrefixFile>));
            BindUpdate<CustomModTranslationFile>(typeof(CustomModTranslationUpdater));
            BindUpdate<LdstrFile>(typeof(LdstrFileUpdater));

            Bind<IUpdateLogger>().To<PlainUpdateLogger>();
        }
    }
}
