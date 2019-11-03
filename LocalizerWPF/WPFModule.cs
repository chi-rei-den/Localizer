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
            Bind<SourcePackageLoad<Package>>().ToSelf().InSingletonScope();
            Bind<PackedPackageLoad<Package>>().ToSelf().InSingletonScope();
        }
    }
}
