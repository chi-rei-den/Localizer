using System;
using System.Collections.Generic;
using System.Globalization;

namespace Localizer.DataModel
{
    public interface IPackage
    {
        /// <summary>
        ///     Name of the package.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Author of the package.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        ///     The name of the mod this package target for.
        /// </summary>
        string ModName { get; set; }

        /// <summary>
        ///     The localized name of the mod.
        /// </summary>
        string LocalizedModName { get; set; }

        /// <summary>
        ///     Description of the package.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     Version of the package.
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        ///     Version of the targeted mod.
        /// </summary>
        Version ModVersion { get; set; }

        /// <summary>
        ///     Culture of the package.
        /// </summary>
        CultureInfo Language { get; set; }

        /// <summary>
        ///     File types this package including.
        /// </summary>
        ICollection<string> FileList { get; set; }

        /// <summary>
        ///     Files this package including.
        /// </summary>
        ICollection<IFile> Files { get; set; }

        int Count { get; }

        /// <summary>
        ///     The enable status of the package.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        ///     The mod this package target for.
        /// </summary>
        IMod Mod { get; set; }

        /// <summary>
        ///     Add a file into Files and FileList
        /// </summary>
        /// <param name="file"></param>
        void AddFile(IFile file);

        /// <summary>
        ///     Remove a file from Files and FileList
        /// </summary>
        /// <param name="file"></param>
        void RemoveFile(IFile file);
    }
}
