using System.Collections.Generic;

namespace Localizer.DataModel
{
 
    public class BasicItemBatcher : Batcher
    {
        public static BasicItemBatcher Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new BasicItemBatcher();

                return _instance;
            }
        }
        
        protected static BasicItemBatcher _instance;
        protected BasicItemBatcher()
        {
            files = new List<File>();
            
            entries = new Dictionary<string, IEntry>();
        }

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = entries[key] as ItemEntry;
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
            var item = mod.GetItem(key);
            if (item == null)
                return;

            var e = entry as ItemEntry;
            item.DisplayName?.Import(e.Name, language);
            item.Tooltip?.Import(e.Tooltip, language);
            item.item.RebuildTooltip();
        }
    }
}