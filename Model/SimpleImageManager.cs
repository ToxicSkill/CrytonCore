using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCore.Tools
{
    public class SimpleImageManager : IDisposable
    {
        private bool _disposed = false;
        // Instantiate a SafeHandle instance.
        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public string Url { get; set; }

        public string OutputUrl { get; set; }

        public string Extension { get; set; }

        public int Ratio { get; set; } = 0;

        public int Rotation { get; set; } = 0;

        public int MaxNumberOfPages { get; set; } = 0;

        public bool SwitchPixels { get; set; } = false;

        public bool MaxQualityFlag { get; set; } = true;

        public System.Drawing.Size Size { get; set; } = new System.Drawing.Size(600, 900);

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
        ~SimpleImageManager()
        {
            Dispose(false);
        }
    }
}
