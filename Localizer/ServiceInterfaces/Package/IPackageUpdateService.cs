using Localizer.DataModel;

namespace Localizer.ServiceInterfaces.Package
{
    public interface IPackageUpdateService
    {
        /// <summary>
        ///     Compare two packages and merge differences into old one.
        ///     Generate reports with logger.
        /// </summary>
        /// <param name="oldPackage"></param>
        /// <param name="newPackage"></param>
        /// <param name="logger"></param>
        void Update(IPackage oldPackage, IPackage newPackage, IUpdateLogService logger);
    }
}
