using Localizer.DataModel;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileImportModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileImportService>().To<BasicFileImportService<BasicItemFile>>().InSingletonScope();
            Bind<IFileImportService>().To<BasicFileImportService<BasicNPCFile>>().InSingletonScope();
            Bind<IFileImportService>().To<BasicFileImportService<BasicBuffFile>>().InSingletonScope();
            Bind<IFileImportService>().To<CustomModTranslationFileImportService>().InSingletonScope();
            Bind<IFileImportService>().To<LdstrFileImportService>().InSingletonScope();
        }
    }
}
