using System.Linq;
using Localizer.DataModel;
using Localizer.ServiceInterfaces;
using Localizer.ServiceInterfaces.Package;
using Localizer.Services.File;
using Ninject;

namespace Localizer.Services.Package
{
    public class PackageUpdate : IPackageUpdateService
    {
        public void Update(IPackage oldPackage, IPackage newPackage, IUpdateLogService logger)
        {
            Utils.LogDebug($"Updating package [{oldPackage.Name}]");

            foreach (var oldFile in oldPackage.Files)
            {
                dynamic temp = oldFile;
                var s = GetUpdateService(temp);
                s.Update(oldFile, newPackage.Files.FirstOrDefault(f => f.GetType() == oldFile.GetType()),
                               logger);
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

        private static IFileUpdateService<T> GetUpdateService<T>(T file) where T : IFile
        {
            return Localizer.Kernel.Get<IFileUpdateService<T>>();
        }
        
        public void Dispose()
        {
        }
    }
}
