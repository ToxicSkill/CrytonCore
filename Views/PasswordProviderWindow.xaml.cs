using CrytonCore.Interfaces;
using CrytonCore.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PasswordProviderWindow.xaml
    /// </summary>
    public partial class PasswordProviderWindow : Window, IClose
    {
        public PasswordProviderWindow(PasswordProviderViewModel dataContext)
        {
            InitializeComponent();
            this.DataContext = new PasswordProviderViewModel();
            (DataContext as PasswordProviderViewModel).Passwords = new();
            (DataContext as PasswordProviderViewModel).Passwords = dataContext.Passwords;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
