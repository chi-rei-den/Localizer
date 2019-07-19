using System.Collections.Generic;
using System.Linq;

namespace Localizer.DataModel
{
    public class BasicCustomFile : File
    {
        public Dictionary<string, BaseEntry> Entries { get; set; }

        public override List<string> GetKeys()
        {
            return this.Entries.Keys.ToList();
        }

        public override IEntry GetValue(string key)
        {
            return this.Entries[key];
        }

        public override void AddEntry(string key, IEntry entry)
        {
            this.Entries.Add(key, entry as BaseEntry);
        }
    }
}