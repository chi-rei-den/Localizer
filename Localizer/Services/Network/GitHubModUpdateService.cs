using System;
using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.ServiceInterfaces.Network;
using Newtonsoft.Json.Linq;

namespace Localizer.Services.Network
{
    public sealed class GitHubModUpdateService : IUpdateService
    {
        private JObject lastCheckResult;
        
        public bool CheckUpdate(Version curVersion, out IUpdateInfo updateInfo)
        {
            var url = "https://api.github.com/repos/chi-rei-den/Localizer/releases/latest";

            var response = Utils.GET(url);
            lastCheckResult = JObject.Parse(Utils.GetResponseBody(response));

            updateInfo = GetUpdateInfo(lastCheckResult);

            if (updateInfo.Type == UpdateType.None)
                return false;

            return updateInfo.Version > curVersion;
        }

        internal GitHubUpdateInfo GetUpdateInfo(JObject jObject)
        {
            return new GitHubUpdateInfo(jObject["tag_name"].ToObject<string>());
        }

        public Dictionary<Version, string> GetChangeLog(Version from, Version to)
        {
            throw new NotImplementedException();
        }

        public string GetDownloadURL()
        {
            return GetDownloadURLInternal(lastCheckResult);
        }

        internal string GetDownloadURLInternal(JObject jObject)
        {
            var assets = jObject["assets"] as JArray;
            
            if(assets == null || assets.Count == 0)
                throw new Exception("Release has no assets!");

            return assets[0]["browser_download_url"].ToObject<string>();
        }
        
        public void Dispose()
        {
        }
    }
}
