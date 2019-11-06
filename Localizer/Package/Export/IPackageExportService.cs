using Localizer.DataModel;

namespace Localizer.Package.Export
{
    public interface IPackageExportService
    {
        /// <summary>
        ///     Extract texts from the mod and fill up the package's file list.
        /// </summary>
        /// <param name="package">Package with necessary information.</param>
        /// <param name="config"></param>
        void Export(IPackage package, IExportConfig config);
    }
}
