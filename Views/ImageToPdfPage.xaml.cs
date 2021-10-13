using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for ImageToPdfUserControl.xaml
    /// </summary>
    public partial class ImageToPdfPage : Page
    {
        public ImageToPdfPage() => InitializeComponent();

        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            pdfViewGrid.Width = pdfViewGrid.ActualHeight * 2 / 3;
        }
    }
}
