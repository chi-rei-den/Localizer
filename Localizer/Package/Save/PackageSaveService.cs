using System.IO;
using Localizer.DataModel;

namespace Localizer.Package.Save
{
    public class PackageSave : IPackageSaveService
    {
        public void Save(IPackage package, string path, IFileSaveService fileSaveDisposable)
        {
            Utils.SerializeJsonAndCreateFile(package, Path.Combine(path, "Package.json"));

            foreach (var file in package.Files)
            {
                fileSaveDisposable.Save(file, Path.Combine(path, file.GetType().Name + ".json"));
            }
        }

        public void Dispose()
        {
        }
    }
}
