using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;

namespace Localizer.Package.Update
{
    public sealed class BasicFileUpdater<T> : FileUpdater where T : IFile
    {
        public override void Update(IFile oldFile, IFile newFile, IUpdateLogger logger)
        {
            CheckArgs(oldFile, newFile, logger);
            UpdateInternal((T)oldFile, (T)newFile, logger);
        }

        public void UpdateInternal(T oldFile, T newFile, IUpdateLogger logger)
        {
            if (oldFile.GetType() != typeof(T) || newFile.GetType() != typeof(T))
            {
                return;
            }

            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                dynamic oldEntries = prop.GetValue(oldFile);
                dynamic newEntries = prop.GetValue(newFile);

                foreach (var newEntryKey in newEntries.Keys)
                {
                    if (oldEntries.ContainsKey(newEntryKey))
                    {
                        UpdateEntry(newEntryKey, oldFile.GetValue(newEntryKey), newFile.GetValue(newEntryKey), logger);
                    }
                    else
                    {
                        logger.Add($"[{newEntryKey}]");
                        dynamic entry = newEntries[newEntryKey].Clone();
                        oldEntries.Add(newEntryKey, entry);
                    }
                }

                var removed = new List<string>();
                foreach (var k in oldEntries.Keys)
                {
                    if (!newEntries.ContainsKey(k))
                    {
                        removed.Add(k);
                    }
                }

                foreach (var r in removed)
                {
                    logger.Remove($"[{r}]");
                }
                
                prop.SetValue(oldFile, oldEntries);
            }
        }

        internal void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry, IUpdateLogger logger)
        {
            foreach (var prop in oldEntry.GetType().ModTranslationProp())
            {
                var o = prop.GetValue(oldEntry) as BaseEntry;
                var n = prop.GetValue(newEntry) as BaseEntry;

                if (o.Origin != n.Origin)
                {
                    logger.Change($"{key}'s {prop.Name}\r\n[Old: \"{o.Origin}\"]\r\n => \r\n[New: \"{n.Origin}\"]\r\n");

                    o.Origin = n.Origin;
                }
            }
        }
    }
}
