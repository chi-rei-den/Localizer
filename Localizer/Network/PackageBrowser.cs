using System;
using System.Collections.Generic;
using System.Globalization;
using Localizer.DataModel;
using Newtonsoft.Json.Linq;

namespace Localizer.Network
{
    public class PackageBrowser : IPackageBrowserService
    {
#if DEBUG
        public string serverURL = "http://127.0.0.1:8000/api/";
#else
        public string serverURL = "http://www.localizer.axeel.moe/api/";
#endif

        private Dictionary<IPackage, int> packages = new Dictionary<IPackage, int>();

        public ICollection<IPackage> GetList()
        {
            packages.Clear();

            var result = new List<IPackage>();

            var response = Utils.GET($"{serverURL}pkg/list");
            var jArray = JArray.Parse(Utils.GetResponseBody(response));

            foreach (JObject jo in jArray)
            {
                Utils.SafeWrap(() =>
                {
                    var pack = new DataModel.Default.Package()
                    {
                        Name = jo["name"].ToObject<string>(),
                        Author = jo["author"].ToObject<string>(),
                        Version = jo["version"].ToObject<Version>(),
                        ModName = jo["mod"].ToObject<string>(),
                        Language = jo["language"].ToObject<CultureInfo>(),
                        Description = jo["description"].ToObject<string>(),
                    };

                    packages.Add(pack, jo["id"].Value<int>());
                    result.Add(pack);
                });
            }

            return result;
        }

        public int GetPageCount()
        {
            throw new NotImplementedException();
        }

        public ICollection<IPackage> GetListByPage(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDownloadLinkOf(IPackage package)
        {
            if (!packages.ContainsKey(package))
            {
                return null;
            }

            return $"{serverURL}pkg/download/{packages[package]}";
        }

        public void Dispose()
        {
        }
    }
}
