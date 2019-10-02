using System;
using System.Collections.Generic;
using Localizer.ServiceInterfaces.Network;

namespace Localizer.Services.Network
{
    public sealed class GitHubModUpdateService : IUpdateService
    {
        
        public UpdateType DetectUpdate(Version curVersion)
        {
            var url = "https://api.github.com/repos/chi-rei-den/Localizer/releases/latest";
            
            throw new NotImplementedException();
            
        }

        internal void PopulateReleaseResponseJson(string response)
        {
            Utils.SafeWrap(GetDownloadURL, out var ex);
        }

        public Dictionary<Version, string> GetChangeLog(Version @from, Version to)
        {
            throw new NotImplementedException();
        }

        public string GetDownloadURL()
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
        }
    }
}
