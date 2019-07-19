using System;

namespace Localizer
{
    public abstract class Plugin : IDisposable
    {
        public abstract void Initialize();

        ~Plugin()
        {
            this.Dispose();
        }

        protected abstract void OnDispose();

        public void Dispose()
        {
            this.OnDispose();
        }
    }
}
