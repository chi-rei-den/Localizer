using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Localizer.DataModel
{
    public abstract class File
    {
        [JsonIgnore]
        public Package Owner { get; set; }

        public abstract List<string> GetKeys();
        
        public abstract IEntry GetValue(string key);

        public abstract void AddEntry(string key, IEntry entry);
    }
}
