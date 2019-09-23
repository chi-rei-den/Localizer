namespace Localizer.ServiceInterfaces.Network
{
    public interface IDownloadManagerService
    {
        void Download(string url, string path);
    }
}
