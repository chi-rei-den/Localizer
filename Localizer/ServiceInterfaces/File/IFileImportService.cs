using System.Globalization;
using Localizer.DataModel;

namespace Localizer.Services.File
{
    public interface IFileImportService<T> : IService where T : IFile
    {
        /// <summary>
        ///     Import a file into correspond mod.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mod"></param>
        /// <param name="culture"></param>
        void Import(T file, IMod mod, CultureInfo culture);

        /// <summary>
        ///     Merge an entry into another one.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        T Merge(T main, T addition);

        void Reset();
    }
}
