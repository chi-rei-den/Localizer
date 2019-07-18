using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.DataModel
{
    public class BasicNPCFile : File
    {
        public Dictionary<string, BaseEntry> Names { get; set; }

        public override List<string> GetKeys() => Names.Keys.ToList();
        public override IEntry GetValue(string key) => Names[key];
        
        public override void AddEntry(string key, IEntry entry)
        {
            Names.Add(key, entry as BaseEntry);
        }
    }
    
    
}
