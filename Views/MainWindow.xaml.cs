using System;
using System.Windows;
using System.Windows.Input;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;
            Welcome_Clicked();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != TopBar) return; //it clicked is smtg else than grid bar

            if (e.ClickCount == 2 && Application.Current.MainWindow.WindowState == WindowState.Normal)
                MaxBtn_Click(sender, e);

            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set tooltip visibility

            if (Tg_Btn.IsChecked == true)
            {
                tt_crypt.Visibility = Visibility.Collapsed;
                tt_security.Visibility = Visibility.Collapsed;
                tt_settings.Visibility = Visibility.Collapsed;
                tt_statistics.Visibility = Visibility.Collapsed;
                tt_informations.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_crypt.Visibility = Visibility.Visible;
                tt_security.Visibility = Visibility.Visible;
                tt_settings.Visibility = Visibility.Visible;
                tt_statistics.Visibility = Visibility.Visible;
                tt_informations.Visibility = Visibility.Visible;
            }
        }
        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Tg_Btn.IsChecked = false;   // closing menu after choosing option from menu
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();
        private void CloseBtn_Up(object sender, RoutedEventArgs e) => CloseBtn.Background.Opacity = 0.5;
        private void MinBtn_Click(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
        private void Tg_Btn_Click(object sender, RoutedEventArgs e) => Pages.NavigationService.Navigate((App.Current as App).welcomePage);

        private void Welcome_Clicked()
        {
            Tg_Btn.IsChecked = true; // closing menu after choosing option from menu
            Pages.NavigationService.Navigate((App.Current as App).welcomePage);
        }

        private void Crypt_clicked(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false; // closing menu after choosing option from menu
            Pages.NavigationService.Navigate((App.Current as App).cryptWindow);
        }

        private void Security_clicked(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false; // closing menu after choosing option from menu
            Pages.NavigationService.Navigate((App.Current as App).pdfManagerPage);
        }

        private void Settings_clicked(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false; // closing menu after choosing option from menu
            Pages.NavigationService.Navigate((App.Current as App).settingsPage);
        }
        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            int ScreenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight,
                WindowHeight = (int)Application.Current.MainWindow.ActualHeight;

            int PlusSize = (int)Math.Round((ScreenHeight - WindowHeight) * 0.1052);

            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                CryptBtn.Margin = new Thickness(0, 45, 0, 55);
                SecurityBtn.Margin = new Thickness(0, 0, 0, 55);
                SettingsBtn.Margin = new Thickness(0, 0, 0, 55);
                StatisticsBtn.Margin = new Thickness(0, 0, 0, 55);
                InfoBtn.Margin = new Thickness(0, 0, 0, 55);
            }
            else// if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                CryptBtn.Margin = new Thickness(0, 45 + PlusSize, 0, 55 + PlusSize);
                SecurityBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
                SettingsBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
                StatisticsBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
                InfoBtn.Margin = new Thickness(0, 0, 0, 55 + PlusSize);
            }
        }
    }
}