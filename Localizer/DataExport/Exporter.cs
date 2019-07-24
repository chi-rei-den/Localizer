using Localizer.DataModel;
using System;
using System.IO;
using System.Linq;
using File = System.IO.File;

namespace Localizer.DataExport
{
    public abstract class Exporter
    {
        public ExportConfig Config { get; protected set; }

        protected LocalizerLogger logger;

        protected abstract Type fileType { get; }

        protected string dirPath => Path.Combine(PackageManager.ExportPath, this.Config.Package.Name);
        protected string filePath => Path.Combine(this.dirPath, this.fileType.Name + ".json");

        public DataModel.File Export()
        {
            try
            {
                var file = this.Extract();
                
                if (!Directory.Exists(this.dirPath))
                {
                    Directory.CreateDirectory(this.dirPath);
                }

                if (this.Config.MakeBackup)
                {
                    var backupDirPath = Path.Combine(this.dirPath, "Backups/");
                    if (!Directory.Exists(backupDirPath))
                    {
                        Directory.CreateDirectory(backupDirPath);
                    }

                    var backupFilePath = Path.Combine(backupDirPath,
                        $"{this.fileType.Name}-{Utils.DateTimeEscapeForPath(DateTime.Now)}");

                    if (File.Exists(this.filePath))
                    {
                        File.Copy(this.filePath, backupFilePath);
                    }
                }

                if (!this.Config.ForceOverride)
                {
                    var oldFile = PackageManager.ReadFile(this.filePath, this.fileType);

                    if (oldFile != null)
                    {
                        this.Update(oldFile, file);

                        file = oldFile;
                    }
                }

                Utils.SerializeJsonAndCreateFile(file, this.filePath);

                return file;
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }

            return null;
        }

        protected void Update(DataModel.File oldFile, DataModel.File newFile)
        {
            var temp = new string[oldFile.GetKeys().Count];
            oldFile.GetKeys().CopyTo(temp);
            var oldKeys = temp.ToList();
            foreach (var key in newFile.GetKeys())
            {
                if (oldKeys.Contains(key))
                {
                    this.logger.TextUpdateLog($"[{key}] changed.");
                    this.UpdateEntry(key, oldFile.GetValue(key), newFile.GetValue(key));
                }
                else
                {
                    this.logger.TextUpdateLog($"New content: [{key}]");
                    var entry = newFile.GetValue(key);
                    oldFile.AddEntry(key, entry);
                    this.AddEntry(key, entry);
                }

                oldKeys.Remove(key);
            }

            foreach (var key in oldKeys)
            {
                this.logger.TextUpdateLog($"Obsolete: [{key}]");
                this.RemoveEntry(key, oldFile.GetValue(key));
            }
        }

        protected abstract DataModel.File Extract();

        protected virtual void RemoveEntry(string key, IEntry entry) { }

        protected virtual void AddEntry(string key, IEntry entry) { }

        protected abstract void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry);
    }
}