using Localizer.DataModel;

namespace Localizer.Services.File
{
    public class JsonFileSaveService : IFileSaveService
    {
        public void Save(IFile file, string path)
        {
            Utils.SerializeJsonAndCreateFile(file, path);
        }
    }
}
