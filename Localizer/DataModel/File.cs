using Newtonsoft.Json;
using System.Collections.Generic;

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
