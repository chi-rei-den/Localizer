using Localizer.DataModel;

namespace Localizer.Package.Update
{
    public interface IPackageUpdateService
    {
        void RegisterUpdater<T>(FileUpdater updater) where T : IFile;
        
        void UnregisterUpdater<T>() where T : IFile;
        
        /// <summary>
        ///     Compare two packages and merge differences into old one.
        ///     Generate reports with logger.
        /// </summary>
        /// <param name="oldPackage"></param>
        /// <param name="newPackage"></param>
        /// <param name="logger"></param>
        void Update(IPackage oldPackage, IPackage newPackage, IUpdateLogger logger);
    }
}
