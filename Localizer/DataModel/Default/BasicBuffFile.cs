using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel.Default
{
    public class BuffEntry : IEntry
    {
        [ModTranslationProp("DisplayName")] public BaseEntry Name { get; set; }

        [ModTranslationProp("Description")] public BaseEntry Description { get; set; }

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
        [ModTranslationOwnerField("buffs")]
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
