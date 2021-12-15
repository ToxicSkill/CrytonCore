using CrytonCore.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCore.Interfaces
{
    public interface IPdfManager
    {
        public Task<PDF> LoadPdf(FileInfo info, PdfPassword pdf = default);
        public Task<PDF> LoadImage(FileInfo info);
        public Task<BitmapImage> GetImageFromPdf(PDF pdf);
        public Task<BitmapImage> ManipulateImage(PDF pdf);

        public Task<bool> SavePdfPagesImages(PDF pdf, string outputPath);
        public Task<bool> ImageToPdf(PDF pdf, BitmapImage bitmap, string outputPath);
        public Task<bool> MergePdf(List<(PdfPassword passwords, FileInfo infos)> files, string outFile);

        public bool SavePdfImage(PDF pdf, string outputPath);
        public bool SavePdfPageImage(string path, BitmapImage bitmapImage);

        public Task SavePdf(string outFile, byte[] bytes);
    }
}