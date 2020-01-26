using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Terraria.ModLoader;

namespace Localizer.Package.Import
{
    [OperationTiming(OperationTiming.PostContentLoad)]
    public sealed class BasicImporter<T> : FileImporter where T : IFile
    {
        public override void Import(IFile file, IMod mod, CultureInfo culture)
        {
            ImportInternal((T)file, mod, culture);
        }

        private void ImportInternal(T file, IMod mod, CultureInfo culture)
        {
            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                var fieldName = prop.ModTranslationOwnerFieldName();

                var field = Utils.GetModByName(mod.Name).ValueOf<IDictionary>(fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                var entries = (IDictionary)prop.GetValue(file);

                ApplyEntries(entries, field, entryType, culture);
            }
        }

        public override IFile Merge(IFile main, IFile addition)
        {
            return MergeInternal((T)main, (T)addition);
        }

        internal T MergeInternal(T main, T addition)
        {
            var result = Activator.CreateInstance(typeof(T));

            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                var entries = (IDictionary)Activator.CreateInstance(prop.PropertyType);

                var mainEntryDict = (IDictionary)prop.GetValue(main);
                var additionEntryDict = (IDictionary)prop.GetValue(addition);

                foreach (string key in mainEntryDict.Keys)
                {
                    entries.Add(key, (mainEntryDict[key] as IEntry)?.Clone());
                }

                foreach (string key in additionEntryDict.Keys)
                {
                    if (entries.Contains(key))
                    {
                        entries[key] = Merge((IEntry)mainEntryDict[key], (IEntry)additionEntryDict[key]);
                    }
                    else
                    {
                        entries.Add(key, additionEntryDict[key]);
                    }
                }

                prop.SetValue(result, entries);
            }

            return (T)result;
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

        private void ApplyEntries(IDictionary entries, IDictionary field, Type entryType, CultureInfo culture)
        {
            var mappings = Utils.CreateEntryMappings(entryType);

            foreach (string eKey in entries.Keys)
            {
                if (!field.Contains(eKey))
                {
                    continue;
                }

                foreach (var mapping in mappings)
                {
                    var localizeOwner = field[eKey];
                    var modTrans =
                        localizeOwner?.GetType().GetProperty(mapping.Key)?.GetValue(localizeOwner) as ModTranslation;

                    modTrans?.Import(mapping.Value.GetValue(entries[eKey]) as BaseEntry, culture);
                }
            }
        }
    }
}
