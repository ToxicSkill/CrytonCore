using System.IO;
using System.Linq;

namespace CrytonCore.Model
{
    public class PDF : IPdf
    {
        public PageSlider Slider { get; set; }

        public MemoryStream BytesStream { get; private set; }

        public PdfPassword Password { get; private set; }

        public FileInfo Info { get; init; }

        public Ratio Ratio { get; set; }

        public int TotalPages { get; init; }

        public byte[] Bytes { get; init; }

        public int CurrentPage { get; private set; }

        public bool HighQuality { get; private set; }

        public double Dimensions { get; private set; }

        public int Rotation { get; private set; }

        public bool SwitchPixels { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int CurrentWidth { get; private set; }

        public int CurrentHeight { get; private set; }

               
        public PDF()
        {
            Password = new();
            SetQuality(false);
        }

        public PDF(PDF pdf)
        {
            Password = new();
            SetQuality(pdf.HighQuality);
        }

        public PDF(int width, int height)
        {
            Password = new();
            HighQuality = true;
            Ratio = new();
            Rotation = 0;
            SwitchPixels = false;
            Width = width;
            Height = height;
        }

        public void SetCurrentPage(int currentPage)
        {
            CurrentHeight = currentPage < 0 ? 0 : currentPage;
        }

        public void SetQuality(bool highQuality)
        {
            HighQuality = highQuality;
            SetDimensions();
        }

        public void SetDimensions(double dimensions = 2.0)
        {
            Dimensions = dimensions;
        }

        public void SetDimensions()
        {
            Dimensions = HighQuality ? 2.0d : 1.0d;
        }

        public void SetRatio(Ratio ratio)
        {
            Ratio = new(ratio);
        }

        public void SetRotation(int rotation)
        {
            if (Enumerable.Range(0, 3).Contains(rotation))
                Rotation = rotation;
        }

        public void IncrementRotation()
        {
            if (++Rotation == 4)
                Rotation = 0;
        }

        public void SetPixelsSwitch()
        {
            SwitchPixels ^= true;
        }

        public void SetWidth(int width)
        {
            Width = width < 0 ? 0 : width;
        }

        public void SetHeight(int height)
        {
            Height = height < 0 ? 0 : height;
        }

        public void SetCurrentWidth(int currentWidth)
        {
            CurrentWidth = currentWidth < 0 ? 0 : currentWidth;
        }

        public void SetCurrentHeight(int currentHeight)
        {
            CurrentHeight = currentHeight < 0 ? 0 : currentHeight;
        }

        public void SetRatioIndex(int ratioIndex)
        {
            Ratio.CurrentIndex = Ratios.GetCount() < ratioIndex  || ratioIndex < 0 ? -1 : ratioIndex;
        }

        public int GetCurrentPage()
        {
            return CurrentPage;
        }

        public bool GetQuality()
        {
            return HighQuality;
        }

        public double GetDimensions()
        {
            return Dimensions;
        }

        public Ratio GetRatio()
        {
            return Ratio;
        }

        public int GetRotation()
        {
            return Rotation;
        }

        public bool GetPixelsSwitch()
        {
            return SwitchPixels;
        }

        public int GetWidth()
        {
            return Width;
        }

        public int GetHeight()
        {
             return Height; 
        }

        public int GetCurrentWidth()
        {
            return CurrentWidth;
        }

        public int GetCurrentHeight()
        {
            return CurrentHeight;
        }

        public int GetRatioIndex()
        {
            return Ratio.CurrentIndex;
        }

        public PageSlider GetSlider()
        {
            return Slider;
        }

        public PdfPassword GetPassword()
        {
            return Password;
        }

        public MemoryStream GetBytesStream()
        {
            return BytesStream;
        }

        public byte[] GetBytes()
        {
            return Bytes;
        }

        public FileInfo GetInfo()
        {
            return Info;
        }

        public int GetTotalPages()
        {
            return TotalPages;
        }
    }
}
