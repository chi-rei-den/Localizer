using System;
using System.Net;

namespace Localizer.Network
{
    public class DownloadManager : IDownloadManagerService
    {
        public void Download(string url, string path)
        {
            new WebClient().DownloadFile(new Uri(url), path);
        }

        public void Dispose()
        {
        }
    }
}
