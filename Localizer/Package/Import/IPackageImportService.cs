using System;
using Localizer.DataModel;

namespace Localizer.Package.Import
{
    public interface IPackageImportService
    {
        void RegisterImporter<T>(Type importerType) where T : IFile;
        
        void UnregisterImporter<T>() where T : IFile;
        
        /// <summary>
        ///     Add a package into the internal queue,
        ///     earlier packages have more priorities.
        /// </summary>
        /// <param name="package"></param>
        void Queue(IPackage package);

        /// <summary>
        ///     Start the import process.
        /// </summary>
        void Import(bool preferEarly);

        /// <summary>
        ///     Clear internal queue and ready for next work.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clear internal queue.
        /// </summary>
        void Clear();
    }
}
