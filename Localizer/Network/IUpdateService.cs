using System;
using System.Collections.Generic;
using Localizer.DataModel;

namespace Localizer.Network
{
    public interface IUpdateService
    {
        bool CheckUpdate(Version curVersion, out IUpdateInfo updateInfo);
        
        Dictionary<Version, string> GetChangeLog(Version from, Version to);

        string GetDownloadURL();
    }
}
