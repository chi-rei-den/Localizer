using Localizer.DataModel;
using Localizer.Services.File;

namespace Localizer.Package
{
    public interface IPackageLoadService<T> where T : IPackage
    {
        /// <summary>
        ///     Load a package.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileLoadService"></param>
        /// <returns></returns>
        IPackage Load(string path, IFileLoadService fileLoadService);
    }
}
