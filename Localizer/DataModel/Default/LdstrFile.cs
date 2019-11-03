using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel.Default
{
    public class LdstrEntry : IEntry
    {
        public List<BaseEntry> Instructions { get; set; }

        public IEntry Clone()
        {
            var entry = new LdstrEntry { Instructions = new List<BaseEntry>() };

            foreach (var ins in Instructions)
            {
                entry.Instructions.Add(ins.Clone() as BaseEntry);
            }

            return entry;
        }
    }

    [OperationTiming]
    public class LdstrFile : IFile
    {
        public Dictionary<string, LdstrEntry> LdstrEntries { get; set; } = new Dictionary<string, LdstrEntry>();

        public List<string> GetKeys()
        {
            return LdstrEntries.Keys.ToList();
        }

        public IEntry GetValue(string key)
        {
            return LdstrEntries[key];
        }
    }
}
