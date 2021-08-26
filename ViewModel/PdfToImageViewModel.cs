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
            VisibilityChangeDelegate = new VisibilityDelegate(ChangeVisibility);
        }

        private void ChangeVisibility(bool show)
        {
            VisibilityDefaultAsShowed = show ? Visibility.Hidden : Visibility.Visible;
            VisibilityDefaultAsHidden = show ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
