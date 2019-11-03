using System;

namespace Localizer
{
    public abstract class LocalizerPlugin : IDisposable
    {
        public abstract string Name { get; }
        public abstract string Author { get; }
        public virtual Version Version => new Version();
        public abstract string Description { get; }
        
        protected bool disposed = false;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LocalizerPlugin()
        {
            Dispose(false);
        }

        public abstract void Initialize(LocalizerKernel kernel);

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
