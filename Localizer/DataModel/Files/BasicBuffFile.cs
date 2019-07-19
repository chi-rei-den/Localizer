using System.Collections.Generic;
using System.Linq;

namespace Localizer.DataModel
{
    public class BuffEntry : IEntry
    {
        public BaseEntry Name { get; set; }

        public BaseEntry Description { get; set; }

        public IEntry Clone()
        {
            return new BuffEntry()
            {
                Name = this.Name.Clone() as BaseEntry,
                Description = this.Description.Clone() as BaseEntry,
            };
        }
    }

    public class BasicBuffFile : File
    {
        public Dictionary<string, BuffEntry> Buffs { get; set; }

        public override List<string> GetKeys()
        {
            return this.Buffs.Keys.ToList();
        }

        public override IEntry GetValue(string key)
        {
            return this.Buffs[key];
        }

        public override void AddEntry(string key, IEntry entry)
        {
            this.Buffs.Add(key, entry as BuffEntry);
        }
    }

}
