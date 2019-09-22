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
            Bind<IPackageManageService>().To<PackageManageService>().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<SourcePackageLoadService<DataModel.Default.Package>>().InSingletonScope();
            Bind<SourcePackageLoadService<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IPackageLoadService<DataModel.Default.Package>>()
                .To<PackedPackageLoadService<DataModel.Default.Package>>().InSingletonScope();
            Bind<PackedPackageLoadService<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IPackageSaveService>().To<PackageSaveService>().InSingletonScope();

            Bind<IPackageExportService>().To<PackageExportService>().InSingletonScope();

            Bind<IPackageImportService>().To<PackageImportService>().InSingletonScope();

            Bind<IPackageUpdateService>().To<PackageUpdateService>().InSingletonScope();

            Bind<IPackagePackService>().To<ZipPackagePackService<DataModel.Default.Package>>().InSingletonScope();
            Bind<ZipPackagePackService<DataModel.Default.Package>>().ToSelf().InSingletonScope();

            Bind<IFileLoadService>().To<JsonFileLoadService>().InSingletonScope();
            Bind<IFileSaveService>().To<JsonFileSaveService>().InSingletonScope();
        }
    }
}
