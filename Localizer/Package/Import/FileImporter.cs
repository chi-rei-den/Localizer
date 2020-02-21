using System.Globalization;
using Localizer.DataModel;

namespace Localizer.Package.Import
{
    public abstract class FileImporter : Disposable
    {
        /// <summary>
        ///     Import a file into correspond mod.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mod"></param>
        /// <param name="culture"></param>
        public abstract void Import(IFile file, IMod mod, CultureInfo culture);

        /// <summary>
        ///     Merge an entry into another one.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        public abstract IFile Merge(IFile main, IFile addition);

        public virtual void Reset() { }
    }
}
