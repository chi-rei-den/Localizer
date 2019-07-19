using System.Collections.Generic;
using System.Linq;

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

        public override List<string> GetKeys()
        {
            return this.Items.Keys.ToList();
        }

        public override IEntry GetValue(string key)
        {
            return this.Items[key];
        }

        public override void AddEntry(string key, IEntry entry)
        {
            this.Items.Add(key, entry as ItemEntry);
        }
    }

}
