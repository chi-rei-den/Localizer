using System.Collections.Generic;
using System.Linq;

namespace Localizer.DataModel
{
    public class BasicCustomFile : File
    {
        public Dictionary<string, BaseEntry> Entries { get; set; }

        public override List<string> GetKeys() => Entries.Keys.ToList();
        public override IEntry GetValue(string key) => Entries[key];
        
        public override void AddEntry(string key, IEntry entry)
        {
            Entries.Add(key, entry as BaseEntry);
        }
    }
}