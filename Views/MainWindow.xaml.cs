using CrytonCore.ViewModel;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            (DataContext as MainWindowViewModel).ToggleButtonCheck = true;
        }

        private bool ClickCount = false;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != TopBar) return; //it clicked is smtg else than grid bar

            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();

            if (e.ClickCount == 2)
            {
                if (!ClickCount)
                {
                    ClickCount = true;
                    return;
                }
                else
                {
                    (DataContext as MainWindowViewModel).MaximizeOrNormalizeMainWindowCommand();
                    ClickCount = false;
                }
            }
        }


        //private void MaxBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    int ScreenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight,
        //        WindowHeight = (int)Application.Current.MainWindow.ActualHeight;

        //    int PlusSize = (int)Math.Round((ScreenHeight - WindowHeight) * 0.1052);

        //    if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
        //    {
        //        Application.Current.MainWindow.WindowState = WindowState.Normal;
        //        CryptBtn.Margin = new Thickness(0, 45, 0, 55);
        //        SecurityBtn.Margin = new Thickness(0, 0, 0, 55);
        //        SettingsBtn.Margin = new Thickness(0, 0, 0, 55);
        //        StatisticsBtn.Margin = new Thickness(0, 0, 0, 55);
        //        InfoBtn.Margin = new Thickness(0, 0, 0, 55);
        //    }
        //    else// if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
        //    {
        //        Application.Current.MainWindow.WindowState = WindowState.Maximized;
        //        CryptBtn.Margin = new Thickness(0, 45 + PlusSize, 0, 55 + PlusSize);
        //        SecurityBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
        //        SettingsBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
        //        StatisticsBtn.Margin = new Thickness(0, 40, 0, 55 + PlusSize);
        //        InfoBtn.Margin = new Thickness(0, 0, 0, 55 + PlusSize);
        //    }
        //}
    }
}