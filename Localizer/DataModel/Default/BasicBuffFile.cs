using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel
{
    public class BuffEntry : IEntry
    {
        [TModLocalizeTextProp("DisplayName")] public BaseEntry Name { get; set; }

        [TModLocalizeTextProp("Description")] public BaseEntry Description { get; set; }

        public IEntry Clone()
        {
            return new BuffEntry
            {
                Name = Name.Clone() as BaseEntry,
                Description = Description.Clone() as BaseEntry
            };
        }
    }

    public class BasicBuffFile : IFile
    {
        [TModLocalizeField("buffs")]
        public Dictionary<string, BuffEntry> Buffs { get; set; } = new Dictionary<string, BuffEntry>();

        public List<string> GetKeys()
        {
            return Buffs.Keys.ToList();
        }

        public IEntry GetValue(string key)
        {
            return Buffs[key];
        }
    }
}
