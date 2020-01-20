using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Terraria.ModLoader;

namespace Localizer.Package.Export
{
    public sealed class BasicFileExport<T> : IFileExportService where T : IFile
    {
        public void Export(IPackage package, IExportConfig config)
        {
            var modName = package.ModName;
            var mod = Utils.GetModByName(modName);

            if (mod == null)
            {
                return;
            }

            var file = Activator.CreateInstance(typeof(T)) as IFile;

            foreach (var prop in typeof(T).ModTranslationOwnerField())
            {
                var fieldName = prop.ModTranslationOwnerFieldName();

                var field = mod.ValueOf<IDictionary>(fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                var entries = CreateEntries(field, entryType, package.Language, config.WithTranslation);

                var entriesOfFile = (IDictionary)prop.GetValue(file);
                foreach (var e in entries)
                {
                    entriesOfFile.Add(e.Key, e.Value);
                }
            }

            package.AddFile(file);
        }

        private Dictionary<string, object> CreateEntries(IDictionary localizeOwners, Type entryType, CultureInfo lang,
                                                         bool withTranslation)
        {
            var entries = new Dictionary<string, object>();

            var mappings = Utils.CreateEntryMappings(entryType);

            foreach (string key in localizeOwners.Keys)
            {
                var entry = Activator.CreateInstance(entryType);

                foreach (var mapping in mappings)
                {
                    object owner = localizeOwners[key];
                    var localizeTrans = owner?.GetType().GetProperty(mapping.Key)?.GetValue(owner) as ModTranslation;

                    mapping.Value.SetValue(entry, new BaseEntry
                    {
                        Origin = localizeTrans.DefaultOrEmpty(),
                        Translation = withTranslation ? localizeTrans?.GetTranslation(lang) : ""
                    });
                }

                entries.Add(key, entry);
            }

            return entries;
        }
    }
}
