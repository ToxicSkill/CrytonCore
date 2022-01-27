using CrytonCore.Interfaces;

namespace CrytonCore.PdfService
{
    public record Mode(bool OnlyPdf, bool SingleSlide) : IMode
    {
        public bool GetCurrentPdfMode()
        {
            return OnlyPdf;
        }

        public bool GetCurrentSlideMode()
        {
            return SingleSlide;
        }
    }
}
