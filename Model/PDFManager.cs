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
    public class PDFManager
    {
        public static async Task<PDF> LoadPdf(FileInfo info)
        {
            return await Task.Run(() =>
            {
                PdfReader reader = new(info.FullName);
                PDF pdf = new()
                {
                    Info = info,
                    Name = info.Name,
                    Bytes = System.IO.File.ReadAllBytes(info.FullName),
                    TotalPages = reader.NumberOfPages,
                    CurrentPage = 0
                };
                return pdf;
            });
        }

        public static async Task<PDF> LoadImage(FileInfo info)
        {
            return await Task.Run(() =>
            {
                FileStream fileStream = new(info.FullName, FileMode.Open, FileAccess.Read);
                BitmapImage img = new();
                img.BeginInit();
                img.StreamSource = fileStream;
                img.EndInit();
                fileStream.Close();
                (int Width, int Height) = ((int)img.Width, (int)img.Height);
                return
                new PDF()
                {
                    Info = info,
                    HighQuality = true,
                    Ratio = 0,
                    Rotation = 0,
                    Name = info.Name,
                    SwitchPixels = false,
                    Bytes = System.IO.File.ReadAllBytes(info.FullName),
                    Width = Width,
                    Height = Height
                };
            });
        }

        public static Task<BitmapImage> GetImageFromPdf(PDF pdf)
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = pdf.CurrentPage; // first page is 0
            var bitmap = new BitmapImage();

            try
            {
                using (IDocLib pdfLibrary = DocLib.Instance)
                {
                    using var docReader = pdfLibrary.GetDocReader(pdf.Bytes, new PageDimensions(pdf.Dimensions));
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

        public static bool SavePdfImage(PDF pdf, string outputPath)
        {
            try
            {
                var bytesLength = pdf.BytesStream.Length;
                if (outputPath is null && bytesLength <= 0)
                    return false;
                using FileStream file = new(outputPath, FileMode.Create, FileAccess.Write);
                byte[] bytes = new byte[bytesLength];
                _ = pdf.BytesStream.Read(bytes, 0, (int)bytesLength);
                file.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                pdf.BytesStream.Close();
            }
            return true;
        }

        public static async Task<bool> ImageToPdf(PDF pdf, BitmapImage bitmap, string outputPath)
        {
            return await Task.Run(async () =>
            {
                Document doc = new(PageSize.A4);
                pdf.HighQuality = true;
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

        public static bool SavePdfPageImage(string path, BitmapImage bitmapImage)
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

        public static async Task<bool> SavePdfPagesImages(PDF pdf, string outputPath)
        {
            try
            {
                List<Task<BitmapImage>> tasks = new();
                var oldPage = pdf.CurrentPage;
                var newPath = outputPath + "//" + pdf.Name + "_";
                for (int i = 0; i < pdf.TotalPages; i++)
                {
                    pdf.CurrentPage = i;
                    tasks.Add(GetImageFromPdf(pdf));
                }
                var results = await Task.WhenAll(tasks);
                foreach (var result in results.Select((value, i) => (i, value)))
                {
                    var bitmap = BitmapImage2Bitmap(result.value);
                    bitmap.Save(newPath + result.i + "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                pdf.CurrentPage = oldPage;
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

        public static Task<BitmapImage> ManipulateImage(PDF pdf)
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
                MemoryStream byteStream = new(pdf.Bytes);
                img.BeginInit();
                img.StreamSource = byteStream;

                if (pdf.SwitchPixels)
                {
                    img.DecodePixelHeight = pdf.Width;
                    img.DecodePixelWidth = pdf.Height;
                }
                else
                {
                    img.DecodePixelHeight = pdf.Height;
                    img.DecodePixelWidth = pdf.Width;
                }

                if (pdf.Ratio != 0)
                    img.DecodePixelWidth = (int)(img.DecodePixelHeight * pdf.Ratio);

                img.Rotation = pdf.Rotation switch
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

        //private iTextSharp.text.Image ImageToPage()
        //{
        //    var imageRes = ImageToBitmap();
        //    var bitmapImage = BitmapImage2Bitmap(imageRes);
        //    return iTextSharp.text.Image.GetInstance(bitmapImage as System.Drawing.Image, new BaseColor(0, 0, 0, 0));
        //}

        public static async Task<bool> MergePdf(List<string> InFiles, String OutFile)
        {
            Document document = new(PageSize.A4, 0, 0, 0, 0);
            try
            {
                //Define a new output document and its size, type
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(OutFile, FileMode.Create));
                document.Open();

                InFiles.ForEach(file =>
                {
                    PdfReader pdfReader = new(file);
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(pdfReader, i);
                        _ = document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                //else
                //{
                //    file.MaxQualityFlag = true;
                //    ImagePDF = file;
                //    iTextSharp.text.Image image = ImageToPage();
                //    image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                //    _ = document.Add(PageSize.A4);
                //    _ = document.Add(image);
                //    //writer.DirectContent.AddImage(image);
                //}
            });
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
