using Localizer.Package;
using Localizer.ServiceInterfaces.Package;
using Localizer.Services.File;
using Localizer.Services.Package;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultPackageModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPackageManageService>().To<PackageManage>().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<SourcePackageLoad<DataModel.Default.Package>>().InSingletonScope();
            Bind<SourcePackageLoad<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<PackedPackageLoad<DataModel.Default.Package>>().InSingletonScope();
            Bind<PackedPackageLoad<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IPackageSaveService>().To<PackageSave>().InSingletonScope();

            Bind<IPackageExportService>().To<PackageExport>().InSingletonScope();

            Bind<IPackageImportService>().To<PackageImport>().InSingletonScope();

            Bind<IPackageUpdateService>().To<PackageUpdate>().InSingletonScope();

            Bind<IPackagePackService>().To<ZipPackagePack<DataModel.Default.Package>>().InSingletonScope();
            Bind<ZipPackagePack<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IFileLoadService>().To<JsonFileLoad>().InSingletonScope();
            Bind<IFileSaveService>().To<JsonFileSave>().InSingletonScope();
        }
    }
}
