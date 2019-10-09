using System.Collections.Generic;
using Localizer.ServiceInterfaces;

namespace LocalizerTest.NonTest
{
    public sealed class UpdateLogger : IUpdateLogService
    {
        public List<string> Added { get; private set; }
        public List<string> Removed { get; private set; }
        public List<string> Changed { get; private set; }

        public UpdateLogger()
        {
            Added = new List<string>();
            Removed = new List<string>();
            Changed = new List<string>();
        }
        
        public void Dispose()
        {
        }

        public void Init(string name)
        {
        }

        public void Add(object content)
        {
            Added.Add(content.ToString());
        }

        public void Remove(object content)
        {
            Removed.Add(content.ToString());
        }

        public void Change(object content)
        {
            Changed.Add(content.ToString());
        }
    }
}
