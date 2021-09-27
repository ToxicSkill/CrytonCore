//using Microsoft.Win32.SafeHandles;
//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Windows.Media.Imaging;

//namespace CrytonCore.Model
//{
//    public class Image : IDisposable
//    {
//        private bool _disposed = false;
//        // Instantiate a SafeHandle instance.
//        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

//        public string Url { get; set; }

//        public string OutputUrl { get; set; }

//        public string Extension { get; set; }

//        public int Ratio { get; set; } = 0;

//        public int Rotation { get; set; } = 0;

//        public int CurrentNumberOfPage { get; set; } = 0;
        
//        public int MaxNumberOfPages { get; set; } = 1;

//        public bool SwitchPixels { get; set; } = false;

//        public bool MaxQualityFlag { get; set; } = false;

//        public BitmapImage Bitmap { get; set; }

//        public System.Drawing.Size Size { get; set; } = new System.Drawing.Size(600, 900);

//        // Public implementation of Dispose pattern callable by consumers.
//        public virtual void Dispose() => Dispose(true);

//        // Protected implementation of Dispose pattern.
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    // Dispose managed state (managed objects).
//                    _safeHandle?.Dispose();
//                    GC.Collect();
//                }

//                _disposed = true;
//            }
//            GC.SuppressFinalize(this);
//        }
//        ~Image()
//        {
//            Dispose(false);
//        }
//    }

//    public class ImageTool
//    {
//        public int Ratio { get; set; } = 0;

//        public int Rotation { get; set; } = 0;
//        public bool SwitchPixels { get; set; } = false;
//        public List<double> Ratios { get; } = new List<double>();
//        public ImageTool()
//        {
//            Ratios.Add((double)1.414213562373095);
//            Ratios.Add((double)4 / 3);
//            Ratios.Add((double)16 / 9);
//            Ratios.Add(1);
//            Ratios.Add((double)18 / 9);
//        }
//    }
//}
