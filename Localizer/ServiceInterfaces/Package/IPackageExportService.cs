using Localizer.DataModel;

namespace Localizer.Services.Package
{
    public interface IPackageExportService : IService
    {
        /// <summary>
        ///     Extract texts from the mod and fill up the package's file list.
        /// </summary>
        /// <param name="package">Package with necessary information.</param>
        /// <param name="config"></param>
        void Export(IPackage package, IExportConfig config);
    }
}
