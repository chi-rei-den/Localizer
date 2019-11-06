using Localizer.DataModel;

namespace Localizer.Package.Save
{
    public class JsonFileSaveService : IFileSaveService
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
