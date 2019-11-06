using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Terraria.ModLoader;

namespace Localizer.Package.Export
{
    public class CustomModTranslationFileExport : IFileExportService
    {
        public void Export(IPackage package, IExportConfig config)
        {
            if (package?.Mod == null)
            {
                return;
            }

            var translations = Utils.GetModByName(package.ModName).Field("translations") 
                as Dictionary<string, ModTranslation>;

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
