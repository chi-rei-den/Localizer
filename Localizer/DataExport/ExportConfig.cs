using Localizer.DataModel;

namespace Localizer.DataExport
{
    public abstract class ExportConfig
    {
        public Package Package { get; internal set; }

        /// <summary>
        /// Create backups before exportation.
        /// </summary>
        public bool MakeBackup { get; set; } = true;

        /// <summary>
        /// Overwrite or not if the file is already exists.
        /// </summary>
        public bool ForceOverride { get; set; } = false;
    }
}