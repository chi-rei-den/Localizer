using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.ServiceInterfaces;

namespace Localizer.Services.File
{
    public class CustomModTranslationFileUpdateService : IFileUpdateService
    {
        public void Update(IFile oldFile, IFile newFile, IUpdateLogService logger)
        {
            if (oldFile.GetType() != typeof(CustomModTranslationFile) ||
                newFile.GetType() != typeof(CustomModTranslationFile))
            {
                return;
            }

            var oldEntries = (oldFile as CustomModTranslationFile).Translations;
            var newEntries = (newFile as CustomModTranslationFile).Translations;

            foreach (var newEntryKey in newEntries.Keys)
            {
                if (oldEntries.Keys.Contains(newEntryKey))
                {
                    var o = oldEntries[newEntryKey];
                    var n = newEntries[newEntryKey];
                    if (o.Origin != n.Origin)
                    {
                        logger.Change($"{newEntryKey}, [Old: \"{o.Origin}\"] => [New: \"{n.Origin}\"]");

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
    }
}
