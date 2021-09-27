using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCore.Model
{
    public class Image
    {
        public string Url { get; set; }
        public string OutputImage { get; set; }
        public int Order { get; set; }
        public int RatioIndex { get; set; }
        public bool SwitchedPixels { get; set; }
        public BitmapImage Bitmap { get; set; }
    }

}
