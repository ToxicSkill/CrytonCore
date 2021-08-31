using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrytonCore.ViewModel
{
    public class PdfToImageViewModel : PortableDocumentFormatManager
    {
        public PdfToImageViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
        }
    }
}
