using System.IO;

namespace CrytonCore.Model
{
    public interface IPdf
    {
        public void SetCurrentPage(int currentPage);
        public void SetQuality(bool highQuality);
        public void SetDimensions(double dimensions = 2.0);
        public void SetDimensions();
        public void SetRatio(Ratio ratio);
        public void IncrementRotation();
        public void SetRotation(int rotation);
        public void SetPixelsSwitch();
        public void SetWidth(int width);
        public void SetHeight(int height);
        public void SetCurrentWidth(int currentWidth);
        public void SetCurrentHeight(int currentHeight);
        public void SetRatioIndex(int ratioIndex);

        public int GetTotalPages();
        public int GetCurrentPage();
        public bool GetQuality();
        public double GetDimensions();
        public int GetRotation();
        public bool GetPixelsSwitch();
        public int GetWidth();
        public int GetHeight();
        public int GetCurrentWidth();
        public int GetCurrentHeight();
        public int GetRatioIndex();
        public PageSlider GetSlider();
        public PdfPassword GetPassword();
        public MemoryStream GetBytesStream();
        public Ratio GetRatio();
        public FileInfo GetInfo();
        public byte[] GetBytes();
    }
}
