using Localizer.DataModel;
using Localizer.ServiceInterfaces;

namespace Localizer.Services.File
{
    public interface IFileUpdateService
    {
        /// <summary>
        ///     Compare two packages and merge differences into old one.
        ///     Generate reports with logger.
        /// </summary>
        /// <param name="oldFile"></param>
        /// <param name="newFile"></param>
        /// <param name="logger"></param>
        void Update(IFile oldFile, IFile newFile, IUpdateLogService logger);
    }
}
