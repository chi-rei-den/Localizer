using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Services.File
{
    public abstract class LdstrFileImportBase
    {
        internal bool HaveTranslation(LdstrEntry entry)
        {
            foreach (var ins in entry.Instructions)
            {
                if (!string.IsNullOrEmpty(ins.Translation))
                {
                    return true;
                }
            }

            return false;
        }
        
        public LdstrFile Merge(LdstrFile main, LdstrFile addition)
        {
            var result = new LdstrFile
            {
                LdstrEntries = new Dictionary<string, LdstrEntry>()
            };
            
            foreach (var e in main.LdstrEntries)
            {
                result.LdstrEntries.Add(e.Key, e.Value.Clone() as LdstrEntry);
            }
            
            foreach (var pair in addition.LdstrEntries)
            {
                if (result.LdstrEntries.ContainsKey(pair.Key))
                {
                    result.LdstrEntries[pair.Key] = Merge(main.LdstrEntries[pair.Key], pair.Value);
                }
                else
                {
                    result.LdstrEntries.Add(pair.Key, pair.Value);
                }
            }

            return result;
        }
        
        internal LdstrEntry Merge(LdstrEntry main, LdstrEntry addition)
        {
            var result = new LdstrEntry { Instructions = new List<BaseEntry>() };

            foreach (var ins in main.Instructions)
            {
                if (!result.Instructions.Exists(i => i.Origin == ins.Origin))
                {
                    result.Instructions.Add(ins.Clone() as BaseEntry);
                }
            }

            foreach (var ins in addition.Instructions)
            {
                if (!result.Instructions.Exists(i => i.Origin == ins.Origin))
                {
                    result.Instructions.Add(ins.Clone() as BaseEntry);
                }
            }

            return result;
        }
    }
}
