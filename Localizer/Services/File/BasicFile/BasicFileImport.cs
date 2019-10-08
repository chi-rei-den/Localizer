using System;
using System.Globalization;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    public sealed class BasicFileImport<T> : IFileImportService<T> where T : IFile, new()
    {
        public void Import(T file, IMod mod, CultureInfo culture)
        {
            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                var fieldName = prop.ModTranslationOwnerFieldName();

                dynamic field = typeof(Mod).GetFieldDirectly(Utils.GetModByName(mod.Name), fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                dynamic entries = prop.GetValue(file);

                ApplyEntries(entries, field, entryType, culture);
            }
        }

        public T Merge(T main, T addition)
        {
            var result = new T();
            
            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                dynamic entries = Activator.CreateInstance(prop.PropertyType);

                dynamic mainEntryDict = prop.GetValue(main);
                dynamic additionEntryDict = prop.GetValue(addition);

                foreach (var t in mainEntryDict)
                {
                    entries.Add(t.Key, t.Value.Clone());
                }
                
                foreach (var pair in additionEntryDict)
                {
                    if (entries.ContainsKey(pair.Key))
                    {
                        entries[pair.Key] = Merge(mainEntryDict[pair.Key], pair.Value);
                    }
                    else
                    {
                        entries.Add(pair.Key, pair.Value);
                    }
                }

                prop.SetValue(result, entries);
            }

            return result;
        }

        public void Reset()
        {
            return;
        }

        public IEntry Merge(IEntry main, IEntry addition)
        {
            if (main.GetType() != addition.GetType())
            {
                return main;
            }

            var result = Activator.CreateInstance(main.GetType()) as IEntry;

            var props = result.GetType().ModTranslationProp();

            foreach (var prop in props)
            {
                var merged = Utils.GetTranslationOfEntry(main, prop);
                if (string.IsNullOrEmpty(merged))
                {
                    merged = Utils.GetTranslationOfEntry(addition, prop) ?? "";
                }
                var baseEntry = Activator.CreateInstance(prop.PropertyType) as BaseEntry;
                baseEntry.Translation = merged;
                prop.SetValue(result, baseEntry);
            }

            return result;
        }

        private void ApplyEntries(dynamic entries, dynamic field, Type entryType, CultureInfo culture)
        {
            var mappings = Utils.CreateEntryMappings(entryType);

            foreach (var ePair in entries)
            {
                if (!field.ContainsKey(ePair.Key))
                {
                    continue;
                }

                foreach (var mapping in mappings)
                {
                    var localizeOwner = field[ePair.Key];
                    var modTrans =
                        localizeOwner?.GetType().GetProperty(mapping.Key)?.GetValue(localizeOwner) as ModTranslation;

                    modTrans?.Import(mapping.Value.GetValue(ePair.Value) as BaseEntry, culture);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
