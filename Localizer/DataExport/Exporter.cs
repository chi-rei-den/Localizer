using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localizer.DataModel;
using Terraria.ModLoader;
using File = System.IO.File;

namespace Localizer.DataExport
{
    public abstract class Exporter
    {
        public ExportConfig Config { get; protected set; }

        protected LocalizerLogger logger;

        protected abstract Type fileType { get; }

        protected string dirPath => Path.Combine(PackageManager.ExportPath, Config.Package.Name);
        protected string filePath => Path.Combine(dirPath, fileType.Name + ".json");

        public DataModel.File Export()
        {
            try
            {
                var file = Extract();
                
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                if (Config.MakeBackup)
                {
                    var backupDirPath = Path.Combine(dirPath, "Backups/");
                    if (!Directory.Exists(backupDirPath))
                        Directory.CreateDirectory(backupDirPath);

                    var backupFilePath = Path.Combine(backupDirPath, 
                        $"{fileType.Name}-{Utils.DateTimeEscapeForPath(DateTime.Now)}");

                    if (File.Exists(filePath))
                        File.Copy(filePath, backupFilePath);
                }

                if (!Config.ForceOverride)
                {
                    var oldFile = PackageManager.ReadFile(filePath, fileType);

                    if (oldFile != null)
                    {
                        Update(oldFile, file);
                    }

                    file = oldFile;
                }

                Utils.SerializeJsonAndCreateFile(file, filePath);
                
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
                    logger.TextUpdateLog($"[{key}] changed.");
                    UpdateEntry(key, oldFile.GetValue(key), newFile.GetValue(key));
                }
                else
                {
                    logger.TextUpdateLog($"New content: [{key}]");
                    var entry = newFile.GetValue(key);
                    oldFile.AddEntry(key, entry);
                    AddEntry(key, entry);
                }

                oldKeys.Remove(key);
            }

            foreach (var key in oldKeys)
            {
                logger.TextUpdateLog($"Obsolete: [{key}]");
                RemoveEntry(key, oldFile.GetValue(key));
            }
        }

        protected abstract DataModel.File Extract();

        protected virtual void RemoveEntry(string key, IEntry entry){}

        protected virtual void AddEntry(string key, IEntry entry){}

        protected abstract void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry);
    }
}