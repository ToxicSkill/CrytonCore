using System.IO;
using System.Windows.Media.Imaging;

namespace CrytonCore.Model
{
    public class PDF
    {
        public byte[] Bytes { get; set; }

        public MemoryStream BytesStream { get; set; }

        public string Password { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public bool HighQuality { get; set; }

        public double Dimensions { get; set; }

        public FileInfo Info { get; set; }

        public string Name { get; set; }

        public double Ratio { get; set; }

        public int Rotation { get; set; }

        public bool SwitchPixels { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int CurrentWidth { get; set; }

        public int CurrentHeight { get; set; }

        public int RatioIndex{ get; set; }

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
    }
}
