using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;

namespace Localizer.DataModel
{
    public class ItemEntry : IEntry
    {
        public BaseEntry Name { get; set; }
     
        public BaseEntry Tooltip { get; set; }
        
        public IEntry Clone()
        {
            return new ItemEntry()
            {
                Name = this.Name.Clone() as BaseEntry,
                Tooltip = this.Tooltip.Clone() as BaseEntry,
            };
        }
    }
    
    public class BasicItemFile : File
    {
        public Dictionary<string, ItemEntry> Items { get; set; }

        public override List<string> GetKeys() => Items.Keys.ToList();
        public override IEntry GetValue(string key) => Items[key];
        
        public override void AddEntry(string key, IEntry entry)
        {
            Items.Add(key, entry as ItemEntry);
        }
    }

}
