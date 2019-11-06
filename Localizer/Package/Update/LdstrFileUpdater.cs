using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Package.Update
{
    public class LdstrFileUpdater : FileUpdater
    {
        public override void Update(IFile oldFile, IFile newFile, IUpdateLogger logger)
        {
            CheckArgs(oldFile, newFile, logger);
            UpdateInternal(oldFile as LdstrFile, newFile as LdstrFile, logger);
        }
        
        public void UpdateInternal(LdstrFile oldFile, LdstrFile newFile, IUpdateLogger logger)
        {
            var oldEntries = oldFile.LdstrEntries;
            var newEntries = newFile.LdstrEntries;

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

        public void Dispose()
        {
        }
    }
}
