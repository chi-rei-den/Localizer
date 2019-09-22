using Localizer.DataModel;

namespace Localizer.Services.Package
{
    public interface IPackageImportService
    {
        /// <summary>
        ///     Add a package into the internal queue,
        ///     earlier packages have more priorities.
        /// </summary>
        /// <param name="package"></param>
        void Queue(IPackage package);

        /// <summary>
        ///     Start the import process.
        /// </summary>
        void Import();

        /// <summary>
        ///     Clear internal queue and ready for next work.
        /// </summary>
        void Reset();
    }
}
