using Docnet.Core;
using Docnet.Core.Models;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCore.Model
{
    public class PortableDocumentFormat
    {
        private readonly int pixelsX = 600;
        private readonly int pixelsY = 900;

        public Image ImagePDF = new Image();
        private ImageTool ImageTools = new ImageTool();
        private byte[] _pdfBytes;

        public Task LoadPdf()
        {
            _pdfBytes = System.IO.File.ReadAllBytes(ImagePDF.Url);
            return Task.CompletedTask;
        }
        public BitmapImage GetBitmapImage() => ImagePDF.Extension == "pdf" ? RenderPage() : ImageToBitmap();

        //public Task<BitmapImage> LoadBitmapImage() => Task.FromResult(RenderPage());
        public Bitmap PrepareBitmapImageToSave(BitmapImage img) => BitmapImage2Bitmap(img);

        //private System.Drawing.Image GetPageImage(PdfiumViewer.PdfDocument document, int dpi) =>
        //    document.Render(ImagePDF.CurrentNumberOfPage, ImagePDF.Size.Width, ImagePDF.Size.Height, dpi, dpi, PdfRenderFlags.Annotations);

        private BitmapImage Convert(System.Drawing.Image img)
        {
            var memory = new MemoryStream();
            img.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.None;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private BitmapImage RenderPage()
        {
            return Convert(ExtractAllImages());
        }

        public System.Drawing.Image ExtractAllImages()
        {
            using (var doc = Patagames.Pdf.Net.PdfDocument.Load(ImagePDF.Url)) // C# Read PDF Document
            {
                var page = doc.Pages[ImagePDF.CurrentNumberOfPage];
                int width = (int)(page.Width / 72.0 * 96);
                int height = (int)(page.Height / 72.0 * 96);
                var bitmap = new PdfBitmap(width, height, true);
                
                bitmap.FillRect(0, 0, width, height, Patagames.Pdf.FS_COLOR.White);
                page.Render(bitmap, 0, 0, width, height, PageRotate.Normal, Patagames.Pdf.Enums.RenderFlags.FPDF_RENDER_LIMITEDIMAGECACHE);
                return bitmap.Image;
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        public void PdfToImage(Bitmap img, string outFileName) => img.Save(outFileName, System.Drawing.Imaging.ImageFormat.Png);


        public BitmapImage ImageToBitmap()
        {
            FileStream fileStream = new FileStream(ImagePDF.Url, FileMode.Open, FileAccess.Read);

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            var ratioMulitpler = ImageTools.Ratios[ImagePDF.Ratio];
            if (ImagePDF.MaxQualityFlag)
            {
                if (ImagePDF.SwitchPixels)
                {
                    img.DecodePixelHeight = pixelsY;
                    img.DecodePixelWidth = (int)(pixelsY * ratioMulitpler);
                }
                else
                {
                    img.DecodePixelWidth = pixelsY;
                    img.DecodePixelHeight = (int)(pixelsY * ratioMulitpler);
                }
            }
            else
            {
                if (ImagePDF.SwitchPixels)
                {
                    img.DecodePixelHeight = pixelsX;
                    img.DecodePixelWidth = (int)(pixelsX * ratioMulitpler);
                }
                else
                {
                    img.DecodePixelWidth = pixelsX;
                    img.DecodePixelHeight = (int)(pixelsX * ratioMulitpler);
                }
            }
            switch (ImagePDF.Rotation)
            {
                case 0:
                    img.Rotation = Rotation.Rotate0;
                    break;
                case 1:
                    img.Rotation = Rotation.Rotate90;
                    break;
                case 2:
                    img.Rotation = Rotation.Rotate180;
                    break;
                case 3:
                    img.Rotation = Rotation.Rotate270;
                    break;

                default:
                    img.Rotation = Rotation.Rotate0;
                    break;
            }
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            img.Freeze();
            return img;
        }
        private iTextSharp.text.Image ImageToPage()
        {
            var imageRes = ImageToBitmap();
            var bitmapImage = BitmapImage2Bitmap(imageRes);
            return iTextSharp.text.Image.GetInstance(bitmapImage as System.Drawing.Image, new BaseColor(0, 0, 0, 0));
        }
        public void ImageToPdf()
        {
            ImagePDF.MaxQualityFlag = true;
            var imageRes = ImageToBitmap();
            var bitmapImage = BitmapImage2Bitmap(imageRes);
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bitmapImage as System.Drawing.Image, new BaseColor(0, 0, 0, 0));
            using (FileStream fs = new FileStream(ImagePDF.OutputUrl, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                iTextSharp.text.Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                image.SetAbsolutePosition(0, 0);
                writer.DirectContent.AddImage(image);
                doc.Close();
            }
        }
        public void MergePdf(List<Model.Image> InFiles, String OutFile)
        {
            //Define a new output document and its size, type
            Document document = new Document(PageSize.A4, 0, 0, 0, 0);
            //Create blank output pdf file and get the stream to write on it.
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(OutFile, FileMode.Create));
            document.Open();

            InFiles.ForEach(file =>
            {
                if (file.Extension == "pdf")
                {
                    PdfReader pdfReader = new PdfReader(file.Url);
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(pdfReader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                else
                {
                    file.MaxQualityFlag = true;
                    ImagePDF = file;
                    iTextSharp.text.Image image = ImageToPage();
                    image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                    document.Add(PageSize.A4);
                    document.Add(image);
                    //writer.DirectContent.AddImage(image);
                }
            });

            document.Close();
        }
    }
}