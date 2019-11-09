using System;
using Localizer.DataModel;

namespace Localizer.Package.Update
{
    public abstract class FileUpdater
    {
        /// <summary>
        ///     Compare two packages and merge differences into old one.
        ///     Generate reports with logger.
        /// </summary>
        /// <param name="oldFile"></param>
        /// <param name="newFile"></param>
        /// <param name="logger"></param>
        public abstract void Update(IFile oldFile, IFile newFile, IUpdateLogger logger);

        internal void CheckArgs(IFile oldFile, IFile newFile, IUpdateLogger logger)
        {
            if(oldFile is null)
                throw new ArgumentNullException(nameof(oldFile));
            if(newFile is null)
                throw new ArgumentNullException(nameof(newFile));
            if(logger is null)
                throw new ArgumentNullException(nameof(logger));
            
            if(oldFile.GetType() != newFile.GetType())
                throw new Exception($"Different file type: [{oldFile.GetType().FullName}] and [{newFile.GetType().FullName}]");
        }
    }
}
