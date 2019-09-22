using Localizer.DataModel;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileExportModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileExportService>().To<BasicFileExportService<BasicItemFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExportService<BasicNPCFile>>().InSingletonScope();
            Bind<IFileExportService>().To<BasicFileExportService<BasicBuffFile>>().InSingletonScope();
            Bind<IFileExportService>().To<CustomModTranslationFileExportService>().InSingletonScope();
            Bind<IFileExportService>().To<LdstrFileExportService>().InSingletonScope();
        }
    }
}
