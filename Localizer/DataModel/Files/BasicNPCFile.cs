using System.Collections.Generic;
using System.Linq;

namespace Localizer.DataModel
{
    public class BasicNPCFile : File
    {
        public Dictionary<string, BaseEntry> Names { get; set; }

        public override List<string> GetKeys()
        {
            return this.Names.Keys.ToList();
        }

        public override IEntry GetValue(string key)
        {
            return this.Names[key];
        }

        public override void AddEntry(string key, IEntry entry)
        {
            this.Names.Add(key, entry as BaseEntry);
        }
    }


}
