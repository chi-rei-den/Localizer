using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    public class CustomModTranslationFileImportService : IFileImportService
    {
        public void Import(IFile file, IMod mod)
        {
            if (file.GetType() != typeof(CustomModTranslationFile))
            {
                return;
            }

            var entryDict = (file as CustomModTranslationFile).Translations;

            var translations =
                typeof(Mod).GetFieldDirectly(Utils.GetModByName(mod.Name), "translations") as
                    IDictionary<string, ModTranslation>;

            foreach (var pair in entryDict)
            {
                if(!translations.ContainsKey(pair.Key))
                    continue;
                
                var translation = translations[pair.Key];
                translation?.Import(pair.Value, LanguageManager.Instance.ActiveCulture.CultureInfo);
                translations[pair.Key] = translation;
            }
            
            
        }

        public IFile Merge(IFile main, IFile addition)
        {
            if (main == null || main.GetType() != addition.GetType() ||
                main.GetType() != typeof(CustomModTranslationFile))
            {
                return null;
            }

            var mainFile = main as CustomModTranslationFile;
            var additionFile = addition as CustomModTranslationFile;

            var result = new CustomModTranslationFile();
            result.Translations = new Dictionary<string, BaseEntry>();
            foreach (var pair in additionFile.Translations)
            {
                if (mainFile.Translations.ContainsKey(pair.Key))
                {
                    result.Translations.Add(pair.Key, Merge(mainFile.Translations[pair.Key], pair.Value));
                }
                else
                {
                    result.Translations.Add(pair.Key, pair.Value);
                }
            }

            return result;
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
    }
}
