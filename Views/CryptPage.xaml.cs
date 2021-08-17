using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;
using CrytonCore.ViewModel;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for Crypt.xaml
    /// </summary>
    public partial class CryptPage : Page
    {
        public CryptPage()
        {
            InitializeComponent();
        }
        private async Task LoadFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _ = await (DataContext as CryptingPageViewModel)?.LoadFile(openFileDialog.FileName);
            }
        }
        /// <summary>
        /// overload method for button and label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadFile_Clicked(object sender, EventArgs e) => await LoadFile();

        // public void GetError(Exception exception) => (Application.Current as App).GetCurrentErrorMessage = exception.Message;
    }
}