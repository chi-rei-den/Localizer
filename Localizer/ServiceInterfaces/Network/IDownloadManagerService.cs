using Localizer.Services;

namespace Localizer.ServiceInterfaces.Network
{
    public interface IDownloadManagerService : IService
    {
        void Download(string url, string path);
    }
}
