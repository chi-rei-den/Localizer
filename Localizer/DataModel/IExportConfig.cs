namespace Localizer.DataModel
{
    public interface IExportConfig
    {
        /// <summary>
        ///     Create backups before exportation.
        /// </summary>
        bool MakeBackup { get; set; }

        /// <summary>
        ///     Overwrite or not if the file is already exists.
        /// </summary>
        bool ForceOverride { get; set; }

        /// <summary>
        ///     If the ModTranslation already has a translation of current culture,
        ///     then export it.
        /// </summary>
        bool WithTranslation { get; set; }
    }
}
