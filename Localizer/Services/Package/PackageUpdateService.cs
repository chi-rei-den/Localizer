using System.Linq;
using Localizer.DataModel;
using Localizer.ServiceInterfaces;
using Localizer.ServiceInterfaces.Package;
using Localizer.Services.File;
using Ninject;

namespace Localizer.Services.Package
{
    public class PackageUpdateService : IPackageUpdateService
    {
        public void Update(IPackage oldPackage, IPackage newPackage, IUpdateLogService logger)
        {
            Utils.LogDebug($"Updating package [{oldPackage.Name}]");
            var fileUpdateService = Localizer.Kernel.GetAll<IFileUpdateService>();

            foreach (var service in fileUpdateService)
            {
                foreach (var oldFile in oldPackage.Files)
                {
                    service.Update(oldFile, newPackage.Files.FirstOrDefault(f => f.GetType() == oldFile.GetType()),
                                   logger);
                }
            }

            foreach (var file in newPackage.Files)
            {
                if (oldPackage.Files.All(f => f.GetType() != file.GetType()))
                {
                    oldPackage.AddFile(file);
                }
            }

            Utils.LogDebug($"Package [{oldPackage.Name}] updated.");
        }

        public void Dispose()
        {
        }
    }
}
