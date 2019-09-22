using System;

namespace Localizer
{
    public abstract class Plugin : IDisposable
    {
        public void Dispose()
        {
            OnDispose();
        }

        public abstract void Initialize();

        ~Plugin()
        {
            Dispose();
        }

        protected abstract void OnDispose();
    }
}
