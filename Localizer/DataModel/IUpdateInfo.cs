using System;

namespace Localizer.DataModel
{
    public enum UpdateType
    {
        None,
        Minor,
        Major,
        Critical,
    }

    public interface IUpdateInfo
    {
        UpdateType Type { get; }
        
        Version Version { get; }
    }
}
