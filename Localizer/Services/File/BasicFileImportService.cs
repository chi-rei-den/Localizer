using System;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    public sealed class BasicFileImportService<T> : IFileImportService where T : IFile
    {
        public void Import(IFile file, IMod mod)
        {
            if (file.GetType() != typeof(T))
            {
                return;
            }

            foreach (var prop in typeof(T).GetTModLocalizeFieldPropInfos())
            {
                var fieldName = prop.GetTModLocalizeFieldName();

                dynamic field = typeof(Mod).GetFieldDirectly(Utils.GetModByName(mod.Name), fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                dynamic entries = prop.GetValue(file);

                ApplyEntries(entries, field, entryType);
            }
        }

        public IFile Merge(IFile main, IFile addition)
        {
            if (main == null || main.GetType() != addition.GetType() || main.GetType() != typeof(T))
            {
                return null;
            }

            var result = Activator.CreateInstance(main.GetType());

            foreach (var prop in typeof(T).GetTModLocalizeFieldPropInfos())
            {
                dynamic entries = Activator.CreateInstance(prop.PropertyType);

                dynamic mainEntryDict = prop.GetValue(main);
                dynamic additionEntryDict = prop.GetValue(main);

                foreach (var pair in additionEntryDict)
                {
                    if (mainEntryDict.ContainsKey(pair.Key))
                    {
                        entries.Add(pair.Key, Merge(mainEntryDict[pair.Key], pair.Value));
                    }
                    else
                    {
                        entries.Add(pair.Key, pair.Value);
                    }
                }

                prop.SetValue(result, entries);
            }

            return result as IFile;
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

            var props = result.GetType().GetTModLocalizePropPropInfos();

            foreach (var prop in props)
            {
                var merged = (Utils.GetTranslationOfEntry(main, prop) ?? Utils.GetTranslationOfEntry(addition, prop)) ??
                             "";
                var baseEntry = prop.GetValue(result) as BaseEntry;
                baseEntry.Translation = merged;
            }

            return result;
        }

        private void ApplyEntries(dynamic entries, dynamic field, Type entryType)
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

                    modTrans?.Import(mapping.Value.GetValue(ePair.Value) as BaseEntry,
                                     LanguageManager.Instance.ActiveCulture.CultureInfo);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
