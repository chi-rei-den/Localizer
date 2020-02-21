using System;

namespace Localizer
{
    public abstract class Disposable : IDisposable
    {
        protected bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (!disposed)
            {
                if (disposeManaged)
                {
                    DisposeManaged();
                }

                DisposeUnmanaged();

                disposed = true;
            }
        }

        protected virtual void DisposeManaged() { }

        protected virtual void DisposeUnmanaged() { }
    }
}
