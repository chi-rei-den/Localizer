using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Package.Update
{
    public class CustomModTranslationUpdater : FileUpdater
    {
        public override void Update(IFile oldFile, IFile newFile, IUpdateLogger logger)
        {
            CheckArgs(oldFile, newFile, logger);
            UpdateInternal(oldFile as CustomModTranslationFile, newFile as CustomModTranslationFile, logger);
        }
        
        public void UpdateInternal(CustomModTranslationFile oldFile, CustomModTranslationFile newFile, IUpdateLogger logger)
        {
            var oldEntries = oldFile.Translations;
            var newEntries = newFile.Translations;

            foreach (var newEntryKey in newEntries.Keys)
            {
                if (oldEntries.Keys.Contains(newEntryKey))
                {
                    var o = oldEntries[newEntryKey];
                    var n = newEntries[newEntryKey];
                    if (o.Origin != n.Origin)
                    {
                        logger.Change($"{newEntryKey}\r\n[Old: \"{o.Origin}\"]\r\n => \r\n[New: \"{n.Origin}\"]\r\n");

                        o.Origin = n.Origin;
                    }
                }
                else
                {
                    logger.Add($"[{newEntryKey}]");
                    var entry = newEntries[newEntryKey];
                    oldEntries.Add(newEntryKey, entry);
                }
            }

            var removed = oldEntries.Keys.Where(k => !newEntries.ContainsKey(k));
            
            foreach (var r in removed)
            {
                logger.Remove($"[{r}]");
            }
        }

        public void Dispose()
        {
        }
    }
}
