using System.Collections.Generic;

namespace Localizer.DataModel
{
    public class BasicNPCBatcher : Batcher
    {
        public static BasicNPCBatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BasicNPCBatcher();
                }

                return _instance;
            }
        }

        protected static BasicNPCBatcher _instance;

        protected BasicNPCBatcher()
        {
            this.files = new List<File>();

            this.entries = new Dictionary<string, IEntry>();
        }

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = this.entries[key] as BaseEntry;
            if (string.IsNullOrWhiteSpace(e.Translation))
            {
                e.Translation = (toMerge as BaseEntry).Translation;
            }
        }

        protected override void ApplyEntry(string key, IEntry entry)
        {
            var npc = this.mod.GetNPC(key);
            if (npc == null)
            {
                return;
            }

            var e = entry as BaseEntry;
            npc.DisplayName?.Import(e, this.language);
        }
    }
}