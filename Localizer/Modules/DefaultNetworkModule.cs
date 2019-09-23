using Localizer.ServiceInterfaces.Network;
using Localizer.Services.Network;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultNetworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPackageBrowserService>().To<PackageBrowserService>().InSingletonScope();
            Bind<IDownloadManagerService>().To<DownloadManagerService>().InSingletonScope();
        }
    }
}
