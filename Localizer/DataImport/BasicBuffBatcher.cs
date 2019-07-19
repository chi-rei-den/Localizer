using System.Collections.Generic;

namespace Localizer.DataModel
{
    public class BasicBuffBatcher : Batcher
    {
        public static BasicBuffBatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BasicBuffBatcher();
                }

                return _instance;
            }
        }

        protected static BasicBuffBatcher _instance;

        protected BasicBuffBatcher()
        {
            this.files = new List<File>();

            this.entries = new Dictionary<string, IEntry>();
        }

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = this.entries[key] as BuffEntry;
            if (string.IsNullOrWhiteSpace(e.Name.Translation))
            {
                e.Name.Translation = (toMerge as BuffEntry).Name.Translation;
            }
            if (string.IsNullOrWhiteSpace(e.Description.Translation))
            {
                e.Description.Translation = (toMerge as BuffEntry).Description.Translation;
            }
        }

        protected override void ApplyEntry(string key, IEntry entry)
        {
            var item = this.mod.GetBuff(key);
            if (item == null)
            {
                return;
            }

            var e = entry as BuffEntry;
            item.DisplayName?.Import(e.Name, this.language);
            item.Description?.Import(e.Description, this.language);
        }
    }
}