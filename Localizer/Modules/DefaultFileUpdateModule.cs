using Localizer.DataModel.Default;
using Localizer.ServiceInterfaces;
using Localizer.Services;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileUpdateModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileUpdateService>().To<BasicFileUpdateService<BasicItemFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdateService<BasicNPCFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdateService<BasicBuffFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdateService<BasicProjectileFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdateService<BasicPrefixFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<CustomModTranslationFileUpdateService>().InSingletonScope();
            Bind<IFileUpdateService>().To<LdstrFileUpdateService>().InSingletonScope();

            Bind<IUpdateLogService>().To<UpdateLogService>();
        }
    }
}
