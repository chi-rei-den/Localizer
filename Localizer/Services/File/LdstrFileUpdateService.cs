using System.Linq;
using Localizer.DataModel;
using Localizer.ServiceInterfaces;

namespace Localizer.Services.File
{
    public class LdstrFileUpdateService : IFileUpdateService
    {
        public void Update(IFile oldFile, IFile newFile, IUpdateLogService logger)
        {
            if (oldFile.GetType() != typeof(LdstrFile) || newFile.GetType() != typeof(LdstrFile))
            {
                return;
            }

            var oldEntries = (oldFile as LdstrFile).LdstrEntries;
            var newEntries = (newFile as LdstrFile).LdstrEntries;

            foreach (var newEntryKey in newEntries.Keys)
            {
                if (oldEntries.Keys.Contains(newEntryKey))
                {
                    var o = oldEntries[newEntryKey];
                    var n = newEntries[newEntryKey];
                    foreach (var newIns in n.Instructions)
                    {
                        if (o.Instructions.Exists(oi => oi.Origin == newIns.Origin))
                        {
                            continue;
                        }

                        o.Instructions.Add(newIns);
                        logger.Change($"New instruction of {newEntryKey}: [{newIns}]");
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
