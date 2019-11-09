using Localizer.DataModel;

namespace Localizer.Package.Save
{
    public interface IPackageSaveService
    {
        /// <summary>
        ///     Save a package to filesystem.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="path"></param>
        /// <param name="fileSaveDisposable"></param>
        void Save(IPackage package, string path, IFileSaveService fileSaveDisposable);
    }
}
