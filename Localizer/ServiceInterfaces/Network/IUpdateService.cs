using System;
using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.Services;

namespace Localizer.ServiceInterfaces.Network
{
    public interface IUpdateService : IService
    {
        bool CheckUpdate(Version curVersion, out IUpdateInfo updateInfo);
        
        Dictionary<Version, string> GetChangeLog(Version from, Version to);

        string GetDownloadURL();
    }
}
