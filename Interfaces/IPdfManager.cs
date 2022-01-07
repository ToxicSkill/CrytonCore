using CrytonCore.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCore.Interfaces
{
    public interface IPdfManager
    {
        public Task<IPdf> LoadPdf(FileInfo info, PdfPassword pdf = default);
        public Task<IPdf> LoadImage(FileInfo info);
        public Task<BitmapImage> GetImageFromPdf(IPdf pdf);
        public Task<BitmapImage> ManipulateImage(IPdf pdf);

        public Task<bool> SavePdfPagesImages(IPdf pdf, string outputPath);
        public Task<bool> ImageToPdf(IPdf pdf, BitmapImage bitmap, string outputPath);
        public Task<bool> MergePdf(List<(PdfPassword passwords, FileInfo infos)> files, string outFile);
        public Task<bool> SavePdf(string outFile, byte[] bytes);

        public bool SavePdfImage(IPdf pdf, string outputPath);
        public bool SavePdfPageImage(string path, BitmapImage bitmapImage);

    }
}