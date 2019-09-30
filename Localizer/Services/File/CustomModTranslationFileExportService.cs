using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    public class CustomModTranslationFileExportService : IFileExportService
    {
        public void Export(IPackage package, IExportConfig config)
        {
            if (package?.Mod == null)
            {
                return;
            }

            var translations =
                typeof(Mod).GetFieldDirectly<Dictionary<string, ModTranslation>>(
                    Utils.GetModByName(package.ModName), "translations");

            if (translations == null)
            {
                return;
            }

            var file = new CustomModTranslationFile
            {
                Translations = new Dictionary<string, BaseEntry>()
            };

            foreach (var translation in translations)
            {
                file.Translations.Add(translation.Key, new BaseEntry
                {
                    Origin = translation.Value.DefaultOrEmpty(),
                    Translation = config.WithTranslation ? translation.Value?.GetTranslation(package.Language) : ""
                });
            }

            package.AddFile(file);
        }

        public void Dispose()
        {
        }
    }
}
