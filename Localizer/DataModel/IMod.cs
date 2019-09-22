using System;
using System.Reflection;

namespace Localizer
{
    public interface IMod
    {
        /// <summary>
        ///     Name of the mod.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Code of the mod.
        /// </summary>
        Assembly Code { get; }

        /// <summary>
        ///     DisplayName of the mod.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///     Version of the mod.
        /// </summary>
        Version Version { get; }
    }
}
