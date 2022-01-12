using CrytonCore.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for SecurityPage.xaml
    /// </summary>
    public partial class SecurityPage : Page
    {
        public SecurityPage()
        {
            InitializeComponent();
        }

        private void OwnerPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((SecurityPageViewModel)DataContext).OwnerSecurePassword = ((PasswordBox)sender).SecurePassword; }
        }

        private void UserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((SecurityPageViewModel)DataContext).UserSecurePassword = ((PasswordBox)sender).SecurePassword; }
        }
    }
}

