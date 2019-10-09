using System;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.ServiceInterfaces;
using Localizer.Services;
using Localizer.Services.File;
using Ninject.Modules;

namespace Localizer.Modules
{
    public class DefaultFileUpdateModule : NinjectModule
    {
        public override void Load()
        {
            BindUpdate<BasicItemFile>(typeof(BasicFileUpdate<BasicItemFile>));
            BindUpdate<BasicNPCFile>(typeof(BasicFileUpdate<BasicNPCFile>));
            BindUpdate<BasicBuffFile>(typeof(BasicFileUpdate<BasicBuffFile>));
            BindUpdate<BasicProjectileFile>(typeof(BasicFileUpdate<BasicProjectileFile>));
            BindUpdate<BasicPrefixFile>(typeof(BasicFileUpdate<BasicPrefixFile>));
            BindUpdate<CustomModTranslationFile>(typeof(CustomModTranslationFileUpdate));
            BindUpdate<LdstrFile>(typeof(LdstrFileUpdate));

            Bind<IUpdateLogService>().To<UpdateLog>();
        }
        
        private void BindUpdate<T>(Type serviceType) where T : IFile
        {
            Bind(typeof(IFileUpdateService<>), typeof(IFileUpdateService<T>))
                .To(serviceType).InSingletonScope();
        }
    }
}
