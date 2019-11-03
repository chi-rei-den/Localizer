using System.Collections.Generic;
using System.Globalization;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    [OperationTiming(OperationTiming.PostContentLoad)]
    public class CustomModTranslationFileImport : IFileImportService<CustomModTranslationFile>
    {
        public void Import(CustomModTranslationFile file, IMod mod, CultureInfo culture)
        {
            var entryDict = file.Translations;

            var translations = Utils.GetModByName(mod.Name).Field("translations") as
                    IDictionary<string, ModTranslation>;

            if (translations == null)
            {
                return;
            }
            
            foreach (var pair in entryDict)
            {
                if (!translations.ContainsKey(pair.Key))
                {
                    continue;
                }

                var translation = translations[pair.Key];
                translation?.Import(pair.Value, culture);
                translations[pair.Key] = translation;
            }


        }

        public CustomModTranslationFile Merge(CustomModTranslationFile main, CustomModTranslationFile addition)
        {
            var result = new CustomModTranslationFile
            {
                Translations = new Dictionary<string, BaseEntry>()
            };

            foreach (var t in main.Translations)
            {
                result.Translations.Add(t.Key, t.Value.Clone() as BaseEntry);
            }
            
            foreach (var pair in addition.Translations)
            {
                if (result.Translations.ContainsKey(pair.Key))
                {
                    result.Translations[pair.Key] = Merge(main.Translations[pair.Key], pair.Value);
                }
                else
                {
                    result.Translations.Add(pair.Key, pair.Value.Clone() as BaseEntry);
                }
            }

            return result;
        }

        public void Reset()
        {

            return;
        }

        public BaseEntry Merge(BaseEntry main, BaseEntry addition)
        {
            var e = main.Clone() as BaseEntry;

            if (string.IsNullOrWhiteSpace(main.Translation))
            {
                e.Translation = addition.Translation;
            }

            return e;
        }

        public void Dispose()
        {
        }
    }
}
