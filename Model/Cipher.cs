using CrytonCore.Interfaces;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CrytonCore.Ciphers
{
    public abstract class Cipher : IDisposable
    {
        private bool _disposed = false;
        // Instantiate a SafeHandle instance.
        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public virtual async Task<bool> Encrypt(IProgress<int> progress, CancellationToken cancellation) { return (bool)await Task.FromResult(default(object)); }
        public virtual async Task<bool> Decrypt(IProgress<int> progress, CancellationToken cancellation) { return (bool)await Task.FromResult(default(object)); }
        public virtual string Name { get; set; }
        public IHelpersInterface Helper { get; set; }
        public virtual T GetItem<T>() => GetType() == typeof(T) ? (T)Convert.ChangeType(this, typeof(T)) : default;

        // Public implementation of Dispose pattern callable by consumers.
        public virtual void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    _safeHandle?.Dispose();
                    GC.Collect();
                }

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        ~Cipher()
        {
            Dispose(false);
        }
    }
}
