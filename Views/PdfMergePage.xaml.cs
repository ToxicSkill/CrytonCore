using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for _dfMergeUserControl.xaml
    /// </summary>

    public partial class PdfMergePage : Page
    {
        public PdfMergePage() => InitializeComponent();

        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = pdfViewGrid.ActualHeight * 2;
            pdfViewGrid.Width = newSize / 3;
        }
    }
}