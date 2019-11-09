using Localizer.Package;
using Localizer.Package.Load;
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
