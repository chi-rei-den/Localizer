using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel.Default
{
    public class PrefixEntry : IEntry
    {
        [ModTranslationProp("DisplayName")] public BaseEntry Name { get; set; }

        public IEntry Clone()
        {
            return new PrefixEntry {Name = Name.Clone() as BaseEntry};
        }
    }

    public class BasicPrefixFile : IFile
    {
        [ModTranslationOwnerField("prefixes")]
        public Dictionary<string, PrefixEntry> Prefixes { get; set; } = new Dictionary<string, PrefixEntry>();

        public List<string> GetKeys()
        {
            return Prefixes.Keys.ToList();
        }

        public IEntry GetValue(string key)
        {
            return Prefixes[key];
        }
    }
}
