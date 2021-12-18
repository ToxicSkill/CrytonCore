using CrytonCore.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.exceptions;
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
    public class PDFManager : IPdfManager
    {
        public async Task<IPdf> LoadPdf(FileInfo info, PdfPassword pdf = default)
        {
            return await Task.Run(() =>
            {
                try
                {
                    PdfReader reader;
                    reader = pdf != default ?
                    !string.Equals(pdf.Password, string.Empty) ?
                    reader = new(info.FullName, pdf.GetBytesPassword()) :
                    reader = new(info.FullName) :
                    reader = new(info.FullName);

                    return InitializePdf(pdf, info, reader);
                }
                catch (BadPasswordException)
                {
                    Console.WriteLine("Bad password provided");
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        private static IPdf InitializePdf(PdfPassword pdfPassword, FileInfo pdfInfo, PdfReader reader)
        {
            PDF pdf = new()
            {
                Info = pdfInfo,
                Bytes = System.IO.File.ReadAllBytes(pdfInfo.FullName),
                TotalPages = reader.NumberOfPages,
                Slider = new() { CurrentIndex = 0, LastIndex = 0, MaxIndex = reader.NumberOfPages - 1 }
            };
            pdf.SetCurrentPage(0);
            _ = pdf.Password.SetPassword(pdfPassword?.Password);
            return pdf;
        }

        public async Task<IPdf> LoadImage(FileInfo info)
        {
            return await Task.Run(() =>
            {
                try
                {
                    FileStream fileStream = new(info.FullName, FileMode.Open, FileAccess.Read);
                    BitmapImage img = new();
                    img.BeginInit();
                    img.StreamSource = fileStream;
                    img.EndInit();
                    fileStream.Close();
                    (int Width, int Height) = ((int)img.Width, (int)img.Height);
                    return
                    new PDF(Width, Height)
                    {
                        Info = info,
                        Bytes = System.IO.File.ReadAllBytes(info.FullName)
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public Task<BitmapImage> GetImageFromPdf(IPdf pdf)
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = pdf.GetSlider().CurrentIndex; // first page is 0
            var bitmap = new BitmapImage();

            try
            {
                using (IDocLib pdfLibrary = DocLib.Instance)
                {
                    var password = pdf.GetPassword();
                    var bytes = pdf.GetBytes();
                    var dimensions = pdf.GetDimensions();
                    var reader = string.Equals(password, default) ?
                        pdfLibrary.GetDocReader(bytes, new PageDimensions(dimensions)) :
                        pdfLibrary.GetDocReader(bytes, pdf.GetPassword().Password, new PageDimensions(dimensions));
                    using var docReader = reader;
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

        public bool SavePdfImage(IPdf pdf, string outputPath)
        {
            var bytesStream = pdf.GetBytesStream();
            try
            {
                var bytesLength = bytesStream.Length;
                if (outputPath is null && bytesLength <= 0)
                    return false;
                using FileStream file = new(outputPath, FileMode.Create, FileAccess.Write);
                byte[] bytes = new byte[bytesLength];
                _ = pdf.GetBytesStream().Read(bytes, 0, (int)bytesLength);
                file.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                bytesStream.Close();
            }
            return true;
        }

        public async Task<bool> ImageToPdf(IPdf pdf, BitmapImage bitmap, string outputPath)
        {
            return await Task.Run(async () =>
            {
                Document doc = new(PageSize.A4);
                pdf.SetQuality(true);
                try
                {
                    //var imageRes = await ManipulateImage(pdf);
                    var bitmapImage = BitmapImage2Bitmap(bitmap);
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bitmapImage, new BaseColor(0, 0, 0, 0));
                    using FileStream fs = new(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
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
            });
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

        public async Task<bool> SavePdfPagesImages(IPdf pdf, string outputPath)
        {
            try
            {
                List<Task<BitmapImage>> tasks = new();
                var oldPage = pdf.GetCurrentPage();
                var newPath = outputPath + "//" + pdf.GetInfo().Name + "_";
                for (int i = 0; i < pdf.GetTotalPages(); i++)
                {
                    pdf.SetCurrentPage(i);
                    tasks.Add(GetImageFromPdf(pdf));
                }
                var results = await Task.WhenAll(tasks);
                foreach (var result in results.Select((value, i) => (i, value)))
                {
                    var bitmap = BitmapImage2Bitmap(result.value);
                    bitmap.Save(newPath + result.i + "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                pdf.SetCurrentPage(oldPage);
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

        public Task<BitmapImage> ManipulateImage(IPdf pdf)
        {
            return Task.Run(() =>
            {
                BitmapImage img = new();
                //if (bitmapImage is null)
                //{
                //    FileStream fileStream = new(pdf.Info.FullName, FileMode.Open, FileAccess.Read);
                //    img.BeginInit();
                //    img.StreamSource = fileStream;
                //    fileStream.Close();
                //}
                MemoryStream byteStream = new(pdf.GetBytes());
                img.BeginInit();
                img.StreamSource = byteStream;

                if (pdf.GetPixelsSwitch())
                {
                    img.DecodePixelHeight = pdf.GetWidth();
                    img.DecodePixelWidth = pdf.GetHeight();
                }
                else
                {
                    img.DecodePixelHeight = pdf.GetHeight();
                    img.DecodePixelWidth = pdf.GetWidth();
                }

                if (pdf.GetRatio() != 0)
                    img.DecodePixelWidth = (int)(img.DecodePixelHeight * pdf.GetRatio());

                img.Rotation = pdf.GetRotation() switch
                {
                    0 => Rotation.Rotate0,
                    1 => Rotation.Rotate90,
                    2 => Rotation.Rotate180,
                    3 => Rotation.Rotate270,
                    _ => Rotation.Rotate0,
                };
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                byteStream.Close();
                return img;
            });
        }

        private static async Task<IElement> GetImagePage(PdfImportedPage page)
        {
            return await Task.Run(() =>
            {
                return iTextSharp.text.Image.GetInstance(page);
            });
        }

        //public async Task<bool> Merge20(List<(PdfPassword passwords, FileInfo infos)> files, String OutFile)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var firstFile = files[0];
        //        using (PdfReader readerFirst = new(firstFile.infos.FullName, firstFile.passwords.GetBytesPassword()))
        //        using (PdfStamper stamper = new(readerFirst, new FileStream(OutFile, FileMode.Create)))
        //        {
        //            foreach (var file in files.Skip(1))
        //            {
        //                using (PdfReader reader = new(file.infos.FullName, file.passwords.GetBytesPassword()))
        //                {
        //                    for (var readerPage = 1; readerPage < reader.NumberOfPages; ++readerPage)
        //                    {
        //                        PdfImportedPage singlePage = stamper.GetImportedPage(reader, readerPage);
        //                        iTextSharp.text.Rectangle pageRect = reader.GetPageSizeWithRotation(readerPage);
        //                        for (int page = readerFirst.NumberOfPages + 1; page > 1; page--)
        //                        {
        //                            stamper.InsertPage(page, pageRect);
        //                            stamper.GetOverContent(page).AddTemplate(singlePage, pageRect.Left, pageRect.Bottom);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return true;
        //    }
        //    );
        //}

        public async Task SavePdf(string outFile, byte[] bytes)
        {
            await System.IO.File.WriteAllBytesAsync(outFile, bytes); // Requires System.IO
        }

        public  async Task<bool> MergePdf(List<(PdfPassword passwords, FileInfo infos)> files, string outFile)
        {
            Document document = new(PageSize.A4, 0, 0, 0, 0);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outFile, FileMode.Create));
                document.Open();

                List<Task<IElement>> tasks = new();

                PdfReader.unethicalreading = true;

                foreach (var file in files)
                {
                    PdfReader pdfReader = new(file.infos.FullName, file.passwords.GetBytesPassword());
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(pdfReader, i);
                        tasks.Add(GetImagePage(page));
                    }
                }

                var pages = await Task.WhenAll(tasks);
                foreach (var page in pages)
                {
                    document.Add(page);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                document.Close();
            }
        }
    }
}
