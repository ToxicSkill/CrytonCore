using CrytonCore.ViewModel;
using System;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PdfManagerPage.xaml
    /// </summary>
    public partial class PdfManagerPage : Page
    {
        public PdfManagerPage()
        {
            InitializeComponent();
            (DataContext as PdfManagerPageViewModel)?.ChangeBarsSize((int)ActualWidth);
        }

        private void Page_LayoutUpdated(object sender, EventArgs e)
        {
            if ((int)ActualWidth > 50)
                (DataContext as PdfManagerPageViewModel)?.ChangeBarsSize((int)ActualWidth);
        }
    }
}
