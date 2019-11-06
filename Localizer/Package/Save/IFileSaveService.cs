using Localizer.DataModel;

namespace Localizer.Package.Save
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
