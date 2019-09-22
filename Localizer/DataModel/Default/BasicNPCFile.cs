using System.Collections.Generic;
using System.Linq;
using Localizer.Attributes;

namespace Localizer.DataModel
{
    public class NPCEntry : IEntry
    {
        [TModLocalizeTextProp("DisplayName")] public BaseEntry Name { get; set; }

        public IEntry Clone()
        {
            return new NPCEntry {Name = Name.Clone() as BaseEntry};
        }
    }

    public class BasicNPCFile : IFile
    {
        [TModLocalizeField("npcs")]
        public Dictionary<string, NPCEntry> NPCs { get; set; } = new Dictionary<string, NPCEntry>();

        public List<string> GetKeys()
        {
            return NPCs.Keys.ToList();
        }

        public IEntry GetValue(string key)
        {
            return NPCs[key];
        }
    }
}
