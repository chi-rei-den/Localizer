using System.Linq;
using Localizer.DataModel;
using Localizer.Services.File;
using Ninject;
using Terraria.ModLoader;

namespace Localizer.Services.Package
{
    public class PackageExportService : IPackageExportService
    {
        public void Export(IPackage package, IExportConfig config)
        {
            var fileExportServices = Localizer.Kernel.GetAll<IFileExportService>();

            if (package.Mod == null || package.Mod.GetType() != typeof(Mod))
            {
                package.Mod = new ModWrapper(ModLoader.Mods.FirstOrDefault(m => m.Name == package.ModName));
            }

            foreach (var service in fileExportServices)
            {
                service.Export(package, config);
            }
        }
    }
}
