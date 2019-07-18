using System.Collections.Generic;

namespace Localizer.DataModel
{
    public class BasicCustomBatcher : Batcher
    {
        public static BasicCustomBatcher Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new BasicCustomBatcher();

                return _instance;
            }
        }
        
        protected static BasicCustomBatcher _instance;

        protected BasicCustomBatcher()
        {
            files = new List<File>();
            
            entries = new Dictionary<string, IEntry>();
        }

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = entries[key] as BaseEntry;
            if (string.IsNullOrWhiteSpace(e.Translation))
            {
                e.Translation = (toMerge as BaseEntry).Translation;
            }
        }

        protected override void ApplyEntry(string key, IEntry entry)
        {
            var translation = mod.CreateTranslation(key.Replace(string.Format("Mods.{0}.", mod.Name), ""));
            var e = entry as BaseEntry;
            translation.Import(e, language);
            mod.AddTranslation(translation);
        }
    }
}