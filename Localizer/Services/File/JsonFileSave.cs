using Localizer.DataModel;

namespace Localizer.Services.File
{
    public class JsonFileSave : IFileSaveService
    {
        public void Save(IFile file, string path)
        {
            Utils.SerializeJsonAndCreateFile(file, path);
        }

        public void Dispose()
        {
        }
    }
}
