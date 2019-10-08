using Localizer.Package;
using Localizer.Services.Package;
using LocalizerWPF.Model;
using Ninject.Modules;

namespace LocalizerWPF
{
    public class WPFModule : NinjectModule
    {
        public override void Load()
        {
            Localizer.Localizer.Kernel.Bind<SourcePackageLoad<Package>>().ToSelf().InSingletonScope();
            Localizer.Localizer.Kernel.Bind<PackedPackageLoad<Package>>().ToSelf().InSingletonScope();
        }
    }
}
