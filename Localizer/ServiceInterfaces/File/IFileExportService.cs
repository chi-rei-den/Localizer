using Localizer.DataModel;

namespace Localizer.Services.File
{
    public interface IFileExportService : IService
    {
        /// <summary>
        ///     Extract texts from mod and add them into the package.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="config"></param>
        void Export(IPackage package, IExportConfig config);
    }
}
