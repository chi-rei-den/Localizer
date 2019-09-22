using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel
{
    public class ItemEntry : IEntry
    {
        [TModLocalizeTextProp("DisplayName")] public BaseEntry Name { get; set; }

        [TModLocalizeTextProp("Tooltip")] public BaseEntry Tooltip { get; set; }

        public IEntry Clone()
        {
            return new ItemEntry
            {
                Name = Name.Clone() as BaseEntry,
                Tooltip = Tooltip.Clone() as BaseEntry
            };
        }
    }

    public class BasicItemFile : IFile
    {
        [TModLocalizeField("items")]
        public Dictionary<string, ItemEntry> Items { get; set; } = new Dictionary<string, ItemEntry>();

        public List<string> GetKeys()
        {
            return Items.Keys.ToList();
        }

        public IEntry GetValue(string key)
        {
            return Items[key];
        }
    }
}
