using Localizer.DataModel;
using Localizer.ServiceInterfaces;

namespace Localizer.Services.File
{
    public interface IFileUpdateService<T> : IService where T : IFile
    {
        /// <summary>
        ///     Compare two packages and merge differences into old one.
        ///     Generate reports with logger.
        /// </summary>
        /// <param name="oldFile"></param>
        /// <param name="newFile"></param>
        /// <param name="logger"></param>
        void Update(T oldFile, T newFile, IUpdateLogService logger);
    }
}
