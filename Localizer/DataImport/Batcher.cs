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
            this.files.Add(file);
        }

        public void Batch()
        {
            try
            {
                if (this.files.Count == 0)
                {
                    return;
                }

                this.SortFiles();

                foreach (var f in this.files)
                {
                    foreach (var key in f.GetKeys())
                    {
                        if (key == null)
                        {
                            continue;
                        }

                        if (!this.entries.ContainsKey(key))
                        {
                            var e = f.GetValue(key).Clone();
                            this.entries.Add(key, e);
                        }
                        else
                        {
                            this.MergeEntry(key, f.GetValue(key).Clone());
                        }
                    }
                }

                foreach (var pair in this.entries)
                {
                    this.ApplyEntry(pair.Key, pair.Value);
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
            Undo();
            this.files.Clear();
            this.entries.Clear();
        }

        protected abstract void MergeEntry(string key, IEntry toMerge);

        protected abstract void ApplyEntry(string key, IEntry entry);

        protected virtual void Undo()
        {
            
        }

        protected void SortFiles()
        {
        }
    }
}