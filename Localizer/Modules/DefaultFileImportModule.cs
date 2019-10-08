using System;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileImportModule : NinjectModule
    {
        public override void Load()
        {
            BindImport<BasicItemFile>(typeof(BasicFileImport<BasicItemFile>));
            BindImport<BasicNPCFile>(typeof(BasicFileImport<BasicNPCFile>));
            BindImport<BasicBuffFile>(typeof(BasicFileImport<BasicBuffFile>));
            BindImport<BasicProjectileFile>(typeof(BasicFileImport<BasicProjectileFile>));
            BindImport<BasicPrefixFile>(typeof(BasicFileImport<BasicPrefixFile>));
            BindImport<CustomModTranslationFile>(typeof(CustomModTranslationFileImport));
            BindImport<LdstrFile>(typeof(HarmonyLdstrFileImport));
        }

        private void BindImport<T>(Type serviceType) where T : IFile
        {
            Bind(typeof(IFileImportService<>), typeof(IFileImportService<T>))
                .To(serviceType).InSingletonScope();
        }
    }
}
