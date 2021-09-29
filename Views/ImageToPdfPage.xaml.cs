using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for ImageToPdfUserControl.xaml
    /// </summary>
    public partial class ImageToPdfPage : Page
    {
        public ImageToPdfPage()
        {
            InitializeComponent();
        }


        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e) => pdfViewGrid.Width = (pdfViewGrid.ActualHeight * 2) / 3;

        //private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (filesListView.SelectedIndex == -1)
        //        return;
        //    try
        //    {
        //        LastSelectedIndex = this.filesListView.SelectedIndex;
        //        this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        //        this.ratioComboBox.SelectedIndex = this._images[LastSelectedIndex].Ratio;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private async void SaveSelectedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog saveFileDialog = new()
        //    {
        //        Title = "Save pdf file",
        //        Filter = "Pdf file (.pdf)|*.pdf" // Filter files by extension
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            //await Application.Current.Dispatcher.InvokeAsync(ImageToPdf);
        //            await SaveImageAsPdf(saveFileDialog);
        //        }
        //        catch (Exception ex)
        //        {
        //            var what = ex.Message;
        //            throw;
        //        }
        //    }
        //}

        //private async Task SaveImageAsPdf(SaveFileDialog saveFileDialog)
        //{
        //    _images[LastSelectedIndex].OutputUrl = saveFileDialog.FileName;
        //    _pdf.ImagePDF = _images[LastSelectedIndex];
        //    await Application.Current.Dispatcher.InvokeAsync(_pdf.ImageToPdf);
        //}

        //private async Task<BitmapImage> GetImage()
        //{
        //    _pdf.ImagePDF = this._images[LastSelectedIndex];
        //    return await Application.Current.Dispatcher.InvokeAsync(_pdf.ImageToBitmap);
        //}

        //private void ClearButton_Click(object sender, MouseButtonEventArgs e)
        //{
        //    if (filesListView.SelectedIndex == -1)
        //        return;

        //    if (_images.Count != 0)
        //    {
        //        _images[LastSelectedIndex].Dispose();
        //        _images.RemoveAt(LastSelectedIndex);
        //    }
        //    if (_images.Count != 0)
        //    {
        //        AddListViewItem(true);
        //        filesListView.SelectedIndex = 0;
        //    }
        //    else
        //    {
        //        filesListView.Items.Clear();
        //        filesListView.Items.Refresh();
        //        pdfImage.Source = null;

        //        ChangeVisibility();
        //    }
        //}

        //private async void RotateButton_Click(object sender, MouseButtonEventArgs e)
        //{
        //    this._images[LastSelectedIndex].Rotation = _images[LastSelectedIndex].Rotation >= 3 ?
        //        _images[LastSelectedIndex].Rotation = 0 :
        //        ++_images[LastSelectedIndex].Rotation;
        //    this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
        //    this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        //}

        //private async void SwitchPixelsButton_Click(object sender, MouseButtonEventArgs e)
        //{
        //    this._images[LastSelectedIndex].SwitchPixels = !this._images[LastSelectedIndex].SwitchPixels;
        //    this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
        //    this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        //}

        //private async void RatioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (filesListView.Items.Count != 0)
        //    {
        //        this._images[LastSelectedIndex].Ratio = ratioComboBox.SelectedIndex;
        //        this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
        //        this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        //    }
        //}
        //private async void MergeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (await (App.Current as App).pdfMergeUserControl.AddForeignImages(this._images))
        //    //{
        //    //    _navigation = System.Windows.Navigation.NavigationService.GetNavigationService(this);
        //    //    _navigation.Navigate((App.Current as App).pdfMergeUserControl);
        //    //}
        //}
    }
}
