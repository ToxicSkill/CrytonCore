using System;
using CrytonCore.ViewModel;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CrytonCore.Helpers;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for _dfMergeUserControl.xaml
    /// </summary>

    public partial class PdfMergeUserControl : UserControl
    {
        private System.Windows.Navigation.NavigationService _navigationService;
        public PdfMergeUserControl()
        {
            InitializeComponent();
        }

        private async Task LoadFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
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

        private void MoveNext(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _navigationService = System.Windows.Navigation.NavigationService.GetNavigationService(this);
                var view = (DataContext as PdfMergeViewModel)?.GetSummaryPage().Result;
                if(view != null)
                    _navigationService?.Navigate(view);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MoveBack(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _navigationService = System.Windows.Navigation.NavigationService.GetNavigationService(this);
                _navigationService?.Navigate((App.Current as App).pdfManagerPage);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}