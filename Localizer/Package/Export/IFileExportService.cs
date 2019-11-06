using Localizer.DataModel;

namespace Localizer.Package.Export
{
    public interface IFileExportService
    {
        /// <summary>
        ///     Extract texts from mod and add them into the package.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="config"></param>
        void Export(IPackage package, IExportConfig config);
    }
}
