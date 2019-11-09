using Localizer.DataModel;

namespace Localizer.Package.Load
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
