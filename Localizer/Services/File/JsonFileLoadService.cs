using System;
using System.Collections.Generic;
using System.IO;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Services.File
{
    public class JsonFileLoadService : IFileLoadService
    {
        private static Dictionary<string, Type> fileTypes = new Dictionary<string, Type>
        {
            {typeof(BasicItemFile).Name, typeof(BasicItemFile)},
            {typeof(BasicBuffFile).Name, typeof(BasicBuffFile)},
            {typeof(CustomModTranslationFile).Name, typeof(CustomModTranslationFile)},
            {typeof(BasicNPCFile).Name, typeof(BasicNPCFile)},
            {typeof(LdstrFile).Name, typeof(LdstrFile)}
        };

        public IFile Load(Stream stream, string typeName)
        {
            if (!fileTypes.ContainsKey(typeName))
            {
                Localizer.Log.Error(string.Format("Not supported file type: {0}", typeName));
                return null;
            }

            var type = fileTypes[typeName];

            var file = Utils.ReadFileAndDeserializeJson(type, stream) as IFile;

            if (file == null)
            {
                Localizer.Log.Error(string.Format("File deserialization failed!, FileType: {0}", type.FullName));
            }

            return file;
        }

        public void RegisterType(string typeName, Type type)
        {
            if (fileTypes == null)
            {
                fileTypes = new Dictionary<string, Type>();
            }

            if (fileTypes.ContainsKey(typeName))
            {
                fileTypes[typeName] = type;
            }
            else
            {
                fileTypes.Add(typeName, type);
            }
        }

        public void Dispose()
        {
        }
    }
}
