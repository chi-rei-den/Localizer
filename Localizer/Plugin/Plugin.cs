using System;

namespace Localizer
{
    public abstract class Plugin : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
