using Localizer.DataModel;

namespace Localizer.Services.File
{
    public interface IFileImportService
    {
        /// <summary>
        ///     Import a file into correspond mod.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mod"></param>
        void Import(IFile file, IMod mod);

        /// <summary>
        ///     Merge an entry into another one.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        IFile Merge(IFile main, IFile addition);
    }
}
