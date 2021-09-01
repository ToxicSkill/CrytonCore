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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != TopBar) return;

            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();

            if (e.LeftButton == MouseButtonState.Pressed &&
                Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Top = 0;
                (DataContext as MainWindowViewModel).MaximizeOrNormalizeMainWindowCommand();
                DragMove();
            }
            if (e.ClickCount == 2)
            {
                (DataContext as MainWindowViewModel).MaximizeOrNormalizeMainWindowCommand();
            }
        }
    }
}