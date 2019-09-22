using System.Collections.Generic;
using Localizer.DataModel;

namespace Localizer.ServiceInterfaces.Network
{
    public interface IPackageBrowserService
    {
        int GetPageCount();

        List<IPackage> GetListPage(int i);

        string GetDownloadLinkOf();
    }
}
