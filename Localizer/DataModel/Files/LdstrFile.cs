using System.Collections.Generic;
using System.Linq;

namespace Localizer.DataModel
{
    public class LdstrEntry : IEntry
    {
        public List<BaseEntry> Instructions { get; set; }

        public IEntry Clone()
        {
            var entry = new LdstrEntry
            {
                Instructions = new List<BaseEntry>()
            };

            foreach (var ins in this.Instructions)
            {
                entry.Instructions.Add(ins.Clone() as BaseEntry);
            }

            return entry;
        }
    }

    public class LdstrFile : File
    {
        public Dictionary<string, LdstrEntry> LdstrEntries { get; set; }

        public override List<string> GetKeys()
        {
            return this.LdstrEntries.Keys.ToList();
        }

        public override IEntry GetValue(string key)
        {
            return this.LdstrEntries[key];
        }

        public override void AddEntry(string key, IEntry entry)
        {
            this.LdstrEntries.Add(key, entry as LdstrEntry);
        }
    }
}