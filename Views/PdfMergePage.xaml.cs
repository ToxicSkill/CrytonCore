using System;
using CrytonCore.ViewModel;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for _dfMergeUserControl.xaml
    /// </summary>

    public partial class PdfMergePage : Page
    {
        public PdfMergePage() => InitializeComponent();

        private async Task LoadFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Pdf files (*.pdf)|*.pdf",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _ = await (DataContext as PdfMergeViewModel)?.LoadFile(openFileDialog.FileNames);
            }
        }
        private async void LoadFileButton_Click(object sender, EventArgs e) => await LoadFile();

        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = pdfViewGrid.ActualHeight * 2;
            pdfViewGrid.Width = newSize / 3;
        }
    }
}