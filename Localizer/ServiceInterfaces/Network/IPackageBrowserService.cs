using System.Collections.Generic;
using Localizer.DataModel;
using Localizer.Services;

namespace Localizer.ServiceInterfaces.Network
{
    public interface IPackageBrowserService : IService
    {
        ICollection<IPackage> GetList();

        int GetPageCount();

        ICollection<IPackage> GetListByPage(int i);

        string GetDownloadLinkOf(IPackage package);
    }
}
