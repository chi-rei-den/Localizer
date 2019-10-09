using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.ServiceInterfaces;

namespace Localizer.Services.File
{
    public class CustomModTranslationFileUpdate : IFileUpdateService<CustomModTranslationFile>
    {
        public void Update(CustomModTranslationFile oldFile, CustomModTranslationFile newFile, IUpdateLogService logger)
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
