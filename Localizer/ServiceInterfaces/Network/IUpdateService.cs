using System;
using System.Collections.Generic;
using Localizer.Services;

namespace Localizer.ServiceInterfaces.Network
{
    public enum UpdateType
    {
        None,
        Minor,
        Major,
        Critical,
    }
    
    public interface IUpdateService : IService
    {
        UpdateType DetectUpdate(Version curVersion);
        
        Dictionary<Version, string> GetChangeLog(Version from, Version to);

        string GetDownloadURL();
    }
}
