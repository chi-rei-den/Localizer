namespace Localizer.Services.Package
{
    public interface IPackagePackService
    {
        /// <summary>
        ///     Pack up a package.
        /// </summary>
        /// <param name="path">Path of the package main file.</param>
        void Pack(string path);
    }
}
