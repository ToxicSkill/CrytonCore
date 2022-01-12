using CrytonCore.Infra;
using CrytonCore.Model;
using CrytonCore.Views;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.ViewModel
{
    public class MainWindowViewModel : PageManager
    {
        private readonly SettingsPage _settingsPage = new();
        private readonly WelcomePage _welcomePage = new();
        private readonly CryptPage _cryptingPage = new();
        private readonly PdfManagerPage _pdfManagerPage = new();
        private readonly PdfMergePage _pdfMergePage = new();
        private readonly SecurityPage _pdfSecurityPage = new();
        private readonly PdfToImagePage _pdfToImagePage = new();
        private readonly ImageToPdfPage _imageToPdfPage = new();
        private readonly PasswordManagerPage _passwordManagerPage = new();

        public MainWindowViewModel()
        {
            App.GoStartPage = new App.WelcomePageDelegate(ToWelcomePage);
            App.GoCryptingPage = new App.CryptingPageDelegate(ToCryptingPage);
            App.GoPdfManagerPage = new App.PdfManagerPageDelegate(ToPdfManagerPage);
            App.GoPdfMergePage = new App.PdfMergePageDelegate(ToPdfMergePage);
            App.GoPdfSecurityPage = new App.PdfSecurityPageDelegate(ToPdfSecurityPage);
            App.GoPdfToImagePage = new App.PdfToImagePageDelegate(ToPdfToImagePage);
            App.GoImageToPdfPage = new App.ImageToPdfPageDelegate(ToImageToPdfPage);
            App.GoSummaryPdfMergePage = new App.SummaryPdfMergePageDelegate(ToPdfSummaryMergePage);
            App.GoPasswordManagerPage = new App.PasswordManagerPageDelegate(ToPasswordManagerPage);

            DisplayPage = _welcomePage;
            ToggleButtonCheck = false;
            App.AppIsLoaded = true;
        }

        private void ToWelcomePage() => DisplayPage = _welcomePage;
        private void ToCryptingPage() => DisplayPage = _cryptingPage;
        private void ToPdfManagerPage() => DisplayPage = _pdfManagerPage;
        private void ToSettingsPage() => DisplayPage = _settingsPage;
        private void ToPdfMergePage() => DisplayPage = _pdfMergePage;
        private void ToPdfSecurityPage() => DisplayPage = _pdfSecurityPage;
        private void ToPdfToImagePage() => DisplayPage = _pdfToImagePage;
        private void ToImageToPdfPage() => DisplayPage = _imageToPdfPage;
        private void ToPasswordManagerPage() => DisplayPage = _passwordManagerPage;
        private void ToPdfSummaryMergePage(SummaryPdfMergePage summaryPdfMergePage) => DisplayPage = summaryPdfMergePage;

        private bool _toggleButtonCheck = true;
        public bool ToggleButtonCheck
        {
            get => _toggleButtonCheck;
            set
            {
                if (value is true)
                    DisplayPage = _welcomePage;
                _toggleButtonCheck = value;
                ChangeVisibility(!_toggleButtonCheck);
                OnPropertyChanged(nameof(ToggleButtonCheck));
            }
        }
        public RelayCommand GoCrypting => new(GoCryptingCommand, App.AppIsLoaded);
        private void GoCryptingCommand() => ToCryptingPage();

        public RelayCommand GoPdfManager => new(GoPdfManagerCommand, App.AppIsLoaded);
        private void GoPdfManagerCommand() => ToPdfManagerPage();

        public RelayCommand GoSettings => new(GoSettingsCommand, App.AppIsLoaded);
        private void GoSettingsCommand() => ToSettingsPage();

        public RelayCommand GoPdfMerge => new(GoPdfMergeCommand, App.AppIsLoaded);
        private void GoPdfMergeCommand() => ToPdfMergePage();

        public RelayCommand GoPdfSecurity => new(GoPdfSecurityCommand, App.AppIsLoaded);
        private void GoPdfSecurityCommand() => ToPdfSecurityPage();

        public RelayCommand GoPdfToImage => new(GoPdfToImageCommand, App.AppIsLoaded);
        private void GoPdfToImageCommand() => ToPdfToImagePage();

        public RelayCommand GoImageToPdf => new(GoImageToPdfCommand, App.AppIsLoaded);
        private void GoImageToPdfCommand() => ToImageToPdfPage();

        public RelayCommand GoPasswordManager => new(GoPasswordManagerCommand, App.AppIsLoaded);
        private void GoPasswordManagerCommand() => ToPasswordManagerPage();

        private Page _displayPage;

        public Page DisplayPage
        {
            get => _displayPage;
            set
            {
                if (Equals(_displayPage, value))
                {
                    return;
                }
                if (!Equals(_welcomePage, value))
                {
                    ToggleButtonCheck = false;
                }

                _displayPage = value;
                OnPropertyChanged(nameof(DisplayPage));
            }
        }

        public RelayCommand CloseMainWindow => new(CloseMainWindowCommand, true);

        private void CloseMainWindowCommand() => System.Environment.Exit(0);

        public RelayCommand MinimizeMainWindow => new(MinimizeMainWindowCommand, true);

        public RelayCommand MaximizeOrNormalizeMainWindow => new(MaximizeOrNormalizeMainWindowCommand, true);

        public void MaximizeOrNormalizeMainWindowCommand()
        {
            if (Application.Current.MainWindow is {WindowState: WindowState.Normal})
                MaximizeMainWindowCommand();
            else if (Application.Current.MainWindow != null && Application.Current.MainWindow.WindowState != WindowState.Normal)
                NormalizeMainWindowCommand();
        }

        private void MinimizeMainWindowCommand()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeMainWindowCommand()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private void NormalizeMainWindowCommand()
        {
            if (Application.Current.MainWindow != null) 
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}
