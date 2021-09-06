using System;
using System.Windows.Controls;
using CrytonCore.ViewModel;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for Crypt.xaml
    /// </summary>
    public partial class CryptPage : Page
    {
        public CryptPage() => InitializeComponent();

        private async void LoadFile_Clicked(object sender, EventArgs e) => await (DataContext as CryptingPageViewModel)?.LoadFileViaDialog();
    }
}