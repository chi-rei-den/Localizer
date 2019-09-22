namespace Localizer.ServiceInterfaces.Network
{
    public interface IDownloadManagerService
    {
        void Queue(string url, string path);
    }
}
