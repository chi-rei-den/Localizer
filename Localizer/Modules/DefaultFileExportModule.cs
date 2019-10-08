using Localizer.DataModel.Default;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileExportModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileExportService>().To<BasicFileExport<BasicItemFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicNPCFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicBuffFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicProjectileFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExport<BasicPrefixFile>>().InSingletonScope();
            Bind<IFileExportService>().To<CustomModTranslationFileExport>().InSingletonScope();
            Bind<IFileExportService>().To<LdstrFileExport>().InSingletonScope();
        }
    }
}
