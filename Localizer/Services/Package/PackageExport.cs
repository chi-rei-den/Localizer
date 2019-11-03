using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using Ninject;
using Terraria.ModLoader;

namespace Localizer.Services.Package
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

        public void Dispose()
        {
        }
    }
}
