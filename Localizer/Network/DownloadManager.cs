using System;
using System.Net;

namespace Localizer.Network
{
    public class DownloadManager : IDownloadManagerService
    {
        public void Download(string url, string path)
        {
           var wc= new WebClient();
            wc.Headers[HttpRequestHeader.UserAgent] = Utils.UserAgent();
            wc.Headers[HttpRequestHeader.AcceptLanguage] = Terraria.Localization.LanguageManager.Instance.ActiveCulture.CultureInfo.ToString();
            wc.DownloadFile(new Uri(url), path);
        }

        public void Dispose()
        {
        }
    }
}
