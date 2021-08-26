using CrytonCore.Infra;

namespace CrytonCore.ViewModel
{
    public class PdfManagerPageViewModel : NotificationClass
    {
        public RelayCommand SecureItemNavigate { get => new(SecureItemNavigateCommand, true); }

        private void SecureItemNavigateCommand() => (App.Current as App).pdfManagerPage.NavigationService.Navigate((App.Current as App).securityUserControl);

        public RelayCommand MergeItemNavigate { get => new(MergeItemNavigateCommand, true); }

        private void MergeItemNavigateCommand() => (App.Current as App).pdfManagerPage.NavigationService.Navigate((App.Current as App).pdfMergeUserControl);

        public RelayCommand ImageToPdfItemNavigate { get => new(ImageToPdfItemNavigateCommand, true); }

        private void ImageToPdfItemNavigateCommand() => (App.Current as App).pdfManagerPage.NavigationService.Navigate((App.Current as App).imageToPdfUserControl);

        public RelayCommand PdfToImageNavigate { get => new(PdfToImageNavigateCommand, true); }

        private void PdfToImageNavigateCommand() => (App.Current as App).pdfManagerPage.NavigationService.Navigate((App.Current as App).pdfToImageUserControl);

        public int BarsSize { get; set; }

        public void ChangeBarsSize(int actualSize)
        {
            BarsSize = actualSize > 50 ? actualSize - 50 : actualSize;
            OnPropertyChanged(nameof(BarsSize));
        }
    }
}
