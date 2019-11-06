using Localizer.DataModel;
using Ninject;
using Terraria.ModLoader;

namespace Localizer.Package.Export
{
    public class PackageExport : IPackageExportService
    {
        public void Export(IPackage package, IExportConfig config)
        {
            var fileExportServices = Localizer.Kernel.GetAll<IFileExportService>();

            if (package.Mod == null || package.Mod.GetType() != typeof(Mod))
            {
                package.Mod = Localizer.GetWrappedMod(package.ModName);
            }

            foreach (var service in fileExportServices)
            {
                service.Export(package, config);
            }
        }
    }
}
