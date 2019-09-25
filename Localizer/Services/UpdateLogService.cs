using System.IO;
using Localizer.ServiceInterfaces;

namespace Localizer.Services
{
    public class UpdateLogService : IUpdateLogService
    {
        private static readonly string logDirPath = Path.Combine(Localizer.SavePath, "UpdateLogs");

        private string path;

        public void Init(string name)
        {
            Utils.CreateDirectory(logDirPath);
            path = Path.Combine(logDirPath, name + ".txt");
        }

        public void Add(object content)
        {
            System.IO.File.AppendAllText(path, $"[Content Added]: {content} \r\n");
        }

        public void Remove(object content)
        {
            System.IO.File.WriteAllText(path, $"[Content Removed]: {content} \r\n");
        }

        public void Change(object content)
        {
            System.IO.File.WriteAllText(path, $"[Content Changed]: {content} \r\n");
        }

        public void Dispose()
        {
        }
    }
}
