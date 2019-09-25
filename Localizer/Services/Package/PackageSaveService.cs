using System.IO;
using Localizer.DataModel;
using Localizer.ServiceInterfaces.Package;
using Localizer.Services.File;

namespace Localizer.Services.Package
{
    public class PackageSaveService : IPackageSaveService
    {
        public void Save(IPackage package, string path, IFileSaveService fileSaveService)
        {
            Utils.SerializeJsonAndCreateFile(package, Path.Combine(path, "Package.json"));

            foreach (var file in package.Files)
            {
                fileSaveService.Save(file, Path.Combine(path, file.GetType().Name + ".json"));
            }
        }

        public void Dispose()
        {
        }
    }
}
