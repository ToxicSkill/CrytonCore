using Docnet.Core;
using Docnet.Core.Models;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCore.Model
{
    public class PDF
    {
        private readonly int pixelsX = 600;
        private readonly int pixelsY = 900;

        public byte[] Bytes { get; private set; }

        private MemoryStream BytesStream { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool HighQuality { get; set; }

        private double Dimensions { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public double Ratio { get; set; }

        public int Rotation { get; set; }

        public bool SwitchPixels { get; set; }

        public PDF()
        {
            SetHighQuality(false);
        }

        public PDF(PDF pdf)
        {
            SetHighQuality(pdf.HighQuality);
        }

        public void SetHighQuality(bool highQuality)
        {
            HighQuality = highQuality;
            SetDimensions();
        }

        private void SetDimensions()
        {
            Dimensions = HighQuality ? 2.0d : 1.0d;
        }

        public async Task<bool> LoadPdf(string path)
        {
            return await Task.Run(() =>
            {
                Url = path;
                Name = path.Split('\\').Last();
                Bytes = System.IO.File.ReadAllBytes(path);
                PdfReader reader = new(path);
                TotalPages = reader.NumberOfPages;
                CurrentPage = 0;
                return true;
            });
        }
        public Task<BitmapImage> GetImageFromPdf()
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = CurrentPage; // first page is 0
            var bitmap = new BitmapImage();

            try
            {
                using (IDocLib pdfLibrary = DocLib.Instance)
                {
                    using var docReader = pdfLibrary.GetDocReader(Bytes, new PageDimensions(Dimensions));
                    using var pageReader = docReader.GetPageReader(pdfPageNum);
                    var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
                    rawBytes = RearrangeBytesToRGBA(rawBytes);
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();

                    // specify that we are reading a byte array of colors in R-G-B-A order.
                    PixelReadSettings pixelReadSettings = new(width, height, StorageType.Char, PixelMapping.RGBA);
                    using MagickImage imgPdfOverlay = new(rawBytes, pixelReadSettings);
                    imgBackdrop = new MagickImage(backdropColor, width, height);
                    imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);
                }

                imgBackdrop.Write(memoryStream, MagickFormat.Png);
                imgBackdrop.Dispose();
                memoryStream.Position = 0;

                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.CacheOption = BitmapCacheOption.Default;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            catch (Exception)
            {
                throw;
            }
            return Task.FromResult(bitmap);
        }
        private static byte[] RearrangeBytesToRGBA(byte[] BGRABytes)
        {
            var max = BGRABytes.Length;
            var RGBABytes = new byte[max];
            var idx = 0;
            byte r;
            byte g;
            byte b;
            byte a;
            while (idx < max)
            {
                // get colors in original order: B G R A
                b = BGRABytes[idx];
                g = BGRABytes[idx + 1];
                r = BGRABytes[idx + 2];
                a = BGRABytes[idx + 3];

                // re-arrange to be in new order: R G B A
                RGBABytes[idx] = r;
                RGBABytes[idx + 1] = g;
                RGBABytes[idx + 2] = b;
                RGBABytes[idx + 3] = a;

                idx += 4;
            }
            return RGBABytes;
        }

        public bool SavePdfImage(string path)
        {
            try
            {
                if (path is null && BytesStream.Length <= 0)
                    return false;
                using FileStream file = new(path, FileMode.Create, FileAccess.Write);
                byte[] bytes = new byte[BytesStream.Length];
                _ = BytesStream.Read(bytes, 0, (int)BytesStream.Length);
                file.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                BytesStream.Close();
            }
            return true;
        }

        public bool ImageToPdf(string outputUrl)
        {
            Document doc = new(PageSize.A4);
            HighQuality = true;
            try
            {
                var imageRes = ImageToBitmap();
                var bitmapImage = BitmapImage2Bitmap(imageRes);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bitmapImage as System.Drawing.Image, new BaseColor(0, 0, 0, 0));
                using FileStream fs = new(outputUrl, FileMode.Create, FileAccess.Write, FileShare.None);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                image.SetAbsolutePosition(0, 0);
                writer.DirectContent.AddImage(image);
                doc.Close();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                doc.Close();
            }
            return true;
        }

        public bool SavePdfPageImage(string path, BitmapImage bitmapImage)
        {
            try
            {
                var bitmap = BitmapImage2Bitmap(bitmapImage);
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public async Task<bool> SavePdfPagesImages(string path)
        {
            try
            {
                List<Task<BitmapImage>> tasks = new();
                var oldPage = CurrentPage;
                var newPath = path + "//" + Name + "_";
                for (int i = 0; i < TotalPages; i++)
                {
                    CurrentPage = i;
                    tasks.Add(GetImageFromPdf());
                }
                var results = await Task.WhenAll(tasks);
                foreach (var result in results.Select((value, i) => (i, value)))
                {
                    var bitmap = BitmapImage2Bitmap(result.value);
                    bitmap.Save(newPath + result.i + "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                CurrentPage = oldPage;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            Bitmap bitmap = new(outStream);

            return new Bitmap(bitmap);
        }
        public BitmapImage ImageToBitmap()
        {
            FileStream fileStream = new(Url, FileMode.Open, FileAccess.Read);

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            if (HighQuality)
            {
                if (SwitchPixels)
                {
                    img.DecodePixelHeight = pixelsY;
                    img.DecodePixelWidth = (int)(pixelsY * Ratio);
                }
                else
                {
                    img.DecodePixelWidth = pixelsY;
                    img.DecodePixelHeight = (int)(pixelsY * Ratio);
                }
            }
            else
            {
                if (SwitchPixels)
                {
                    img.DecodePixelHeight = pixelsX;
                    img.DecodePixelWidth = (int)(pixelsX * Ratio);
                }
                else
                {
                    img.DecodePixelWidth = pixelsX;
                    img.DecodePixelHeight = (int)(pixelsX * Ratio);
                }
            }
            img.Rotation = Rotation switch
            {
                0 => System.Windows.Media.Imaging.Rotation.Rotate0,
                1 => System.Windows.Media.Imaging.Rotation.Rotate90,
                2 => System.Windows.Media.Imaging.Rotation.Rotate180,
                3 => System.Windows.Media.Imaging.Rotation.Rotate270,
                _ => System.Windows.Media.Imaging.Rotation.Rotate0,
            };
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
    }
}
