using System.Collections.Generic;

namespace Localizer.DataModel
{

    public class BasicItemBatcher : Batcher
    {
        public static BasicItemBatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BasicItemBatcher();
                }

                return _instance;
            }
        }

        protected static BasicItemBatcher _instance;
        protected BasicItemBatcher()
        {
            this.files = new List<File>();

            this.entries = new Dictionary<string, IEntry>();
        }

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = this.entries[key] as ItemEntry;
            if (string.IsNullOrWhiteSpace(e?.Name.Translation))
            {
                e.Name.Translation = (toMerge as ItemEntry).Name.Translation;
            }
            if (string.IsNullOrWhiteSpace(e?.Tooltip.Translation))
            {
                e.Tooltip.Translation = (toMerge as ItemEntry).Tooltip.Translation;
            }
        }

        protected override void ApplyEntry(string key, IEntry entry)
        {
            var item = this.mod.GetItem(key);
            if (item == null)
            {
                return;
            }

            var e = entry as ItemEntry;
            item.DisplayName?.Import(e.Name, this.language);
            item.Tooltip?.Import(e.Tooltip, this.language);
            item.item.RebuildTooltip();
        }
    }
}