using System;
using System.Globalization;
using System.Linq;
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

                dynamic field = Utils.GetModByName(mod.Name).Field(fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                dynamic entries = prop.GetValue(file);

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
    }
}
