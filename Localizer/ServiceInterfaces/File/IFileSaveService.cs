using Localizer.DataModel;

namespace Localizer.Services.File
{
    public interface IFileSaveService
    {
        /// <summary>
        ///     Save a file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        void Save(IFile file, string path);
    }
}
