using System;
using System.Net;
using Localizer.ServiceInterfaces.Network;

namespace Localizer.Services.Network
{
    public class DownloadManagerService : IDownloadManagerService
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
