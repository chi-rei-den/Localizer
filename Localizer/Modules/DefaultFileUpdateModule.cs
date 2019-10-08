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
            Bind<IFileUpdateService>().To<BasicFileUpdate<BasicItemFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdate<BasicNPCFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdate<BasicBuffFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdate<BasicProjectileFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<BasicFileUpdate<BasicPrefixFile>>().InSingletonScope();
            Bind<IFileUpdateService>().To<CustomModTranslationFileUpdate>().InSingletonScope();
            Bind<IFileUpdateService>().To<LdstrFileUpdate>().InSingletonScope();

            Bind<IUpdateLogService>().To<UpdateLog>();
        }
    }
}
