using CrytonCore.Model;
using CrytonCore.ViewModel;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for ImageToPdfUserControl.xaml
    /// </summary>
    public partial class PdfToImageUserControl : UserControl
    {
        private System.Windows.Navigation.NavigationService _navigationService;
        public PdfToImageUserControl()
        {
            InitializeComponent();
        }

        private async Task LoadFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _ = await (DataContext as PdfToImageViewModel)?.LoadFile(openFileDialog.FileNames);
            }
        }
        private async void LoadFileButton_Click(object sender, EventArgs e) => await LoadFile();


        //private void AddFileToList(string url)
        //{
        //    Model.Image img = new()
        //    {
        //        Url = url,
        //        Ratio = 0,
        //        Rotation = 0,
        //        SwitchPixels = false,
        //        Extension = "pdf"
        //    };
        //    try
        //    {
        //        var reader = new PdfReader(img.Url);
        //        img.MaxNumberOfPages = reader.NumberOfPages;
        //        _pdf.ImagePDF = img;
        //        img.Bitmap = _pdf.GetBitmapImage();
        //        _images.Add(img);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = pdfViewGrid.ActualHeight * 2;
            pdfViewGrid.Width = newSize / 3;
        }

        //private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (filesListView.SelectedIndex == -1)
        //        return;
        //    try
        //    {
        //        LastSelectedIndex = this.filesListView.SelectedIndex;
        //        this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        //        // this.ratioComboBox.SelectedIndex = this._images[LastSelectedIndex].Ratio;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private void SaveSelectedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog saveFileDialog = new()
        //    {
        //        Title = "Save image file",
        //        Filter = "JPG (.jpg)|*.jpg" // Filter files by extension
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            var currentIndex = filesListView.SelectedIndex;
        //            //await Application.Current.Dispatcher.InvokeAsync(ImageToPdf);
        //            _images[currentIndex].Size = new System.Drawing.Size(1200, 1800);
        //            _images[currentIndex].MaxQualityFlag = true;
        //            _pdf.ImagePDF = _images[currentIndex];

        //            var image = _pdf.GetBitmapImage();
        //            var preparedImage = _pdf.PrepareBitmapImageToSave(image);
        //            preparedImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
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
        //        // this._images[LastSelectedIndex].Ratio = ratioComboBox.SelectedIndex;
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
