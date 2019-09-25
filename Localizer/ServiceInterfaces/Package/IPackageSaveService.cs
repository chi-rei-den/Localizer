using Localizer.DataModel;
using Localizer.Services;
using Localizer.Services.File;

namespace Localizer.ServiceInterfaces.Package
{
    public interface IPackageSaveService : IService
    {
        /// <summary>
        ///     Save a package to filesystem.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="path"></param>
        /// <param name="fileSaveService"></param>
        void Save(IPackage package, string path, IFileSaveService fileSaveService);
    }
}
