using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer
{
    public abstract class Plugin : IDisposable
    {
        public abstract void Initialize();

        ~Plugin()
        {
            Dispose();
        }

        protected abstract void OnDispose();

        public void Dispose()
        {
            OnDispose();
        }
    }
}
