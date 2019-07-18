using System;
using System.Collections.Generic;
using System.Globalization;
using Terraria.ModLoader;

namespace Localizer.DataModel
{
    public abstract class Batcher
    {
        protected List<File> files;

        protected Dictionary<string, IEntry> entries;

        protected Mod mod;
        protected CultureInfo language;

        public void Add(File file)
        {
            files.Add(file);
        }

        public void Batch()
        {
            try
            {
                if (files.Count == 0)
                    return;

                SortFiles();

                foreach (var f in files)
                {
                    foreach (var key in f.GetKeys())
                    {
                        if (key == null)
                            continue;

                        if (!entries.ContainsKey(key))
                        {
                            var e = f.GetValue(key).Clone();
                            entries.Add(key, e);
                        }
                        else
                        {
                            MergeEntry(key, f.GetValue(key).Clone());
                        }
                    }
                }

                foreach (var pair in entries)
                {
                    ApplyEntry(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error(e);
            }
        }

        public void SetState(Mod mod, CultureInfo language)
        {
            this.mod = mod;
            this.language = language;
        }
        
        public void Reset()
        {
            files.Clear();
            entries.Clear();
        }

        protected abstract void MergeEntry(string key, IEntry toMerge);
        
        protected abstract void ApplyEntry(string key, IEntry entry);

        protected void SortFiles()
        {
            files.Sort((lhs, rhs) => -lhs.Owner.Priority.CompareTo(rhs.Owner.Priority));
        }
    }
}