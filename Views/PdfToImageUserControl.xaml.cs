using CrytonCore.Model;
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
        private int counter = 0;
        private int LastSelectedIndex = 0;
        const int pixels = 600;
        const int pixelsHQ = 900;

        private PortableDocumentFormat _pdf = new PortableDocumentFormat();

        private readonly List<Model.Image> _images = new List<Model.Image>();
        private readonly Model.ImageTool _image = new Model.ImageTool();

        private System.Windows.Navigation.NavigationService _navigation;

        public PdfToImageUserControl()
        {
            InitializeComponent();

            ChangeVisibility();
            (App.Current as App).themesUpdaters.Add(new App.UpdateThemes(UpdateTheme));
        }
        private void UpdateTheme()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml") });

            //switch ((App.Current as App).GetThemeDictionary())
            //{
            //    case 1:
            //        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Orange.xaml") });
            //        break;
            //    //case 2:
            //    //    this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml") });
            //    //    break;
            //    default:
            //        this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml") });
            //        break;
            //}
        }

        private void ChangeVisibility(bool visible = false)
        {
            if (visible == true)
            {
                this.dragDropGrid.Visibility = Visibility.Hidden;
                this.filesListView.Visibility = Visibility.Visible;
                this.controlsGrid.Visibility = Visibility.Visible;
                this.imageGrid.Visibility = Visibility.Visible;
                this.saveSelectedButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.dragDropGrid.Visibility = Visibility.Visible;
                this.filesListView.Visibility = Visibility.Hidden;
                this.controlsGrid.Visibility = Visibility.Hidden;
                this.imageGrid.Visibility = Visibility.Hidden;
                this.saveSelectedButton.Visibility = Visibility.Hidden;
            }
        }

        private class Column
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        private void AddListViewItem(bool resetCounter = false)
        {
            if (resetCounter == true)
                counter = 0;

            this.filesListView.Items.Clear();
            this.filesListView.Items.Refresh();

            GridView gridView = new GridView();
            this.filesListView.View = gridView;

            List<dynamic> myItems = new List<dynamic>();
            dynamic myItem;

            IDictionary<string, object> myItemValues;

            foreach (var fileString in _images)
            {
                myItem = new System.Dynamic.ExpandoObject();

                myItemValues = (IDictionary<string, object>)myItem;
                myItemValues["ID"] = ++counter;
                myItemValues = (IDictionary<string, object>)myItem;
                myItemValues["File name"] = fileString.Url.Split('\\').Last();
                myItemValues = (IDictionary<string, object>)myItem;
                myItemValues["File path"] = fileString;//.PadLeft(fileString.Length-2);

                myItems.Add(myItem);
            }

            // Assuming that all objects have same columns - using first item to determine the columns
            List<Column> columns = new List<Column>();

            myItemValues = (IDictionary<string, object>)myItems[0];

            // Key is the column, value is the value
            foreach (var pair in myItemValues)
            {
                Column column = new Column
                {
                    FileName = pair.Key,
                    FilePath = pair.Key
                };

                columns.Add(column);
            }

            // Add the column definitions to the list view
            gridView.Columns.Clear();

            foreach (var column in columns)
            {
                var binding = new Binding(column.FileName);

                gridView.Columns.Add(new GridViewColumn
                {
                    Header = column.FileName,
                    DisplayMemberBinding = binding,
                    HeaderContainerStyle = (Style)Application.Current.Resources["GridViewColumnHeaderStyle"]
                });
            }

            // Add all items to the list
            foreach (dynamic item in myItems)
            {
                this.filesListView.Items.Add(item);
            }

            if (this.filesListView.Items.Count != 0)
                this.filesListView.Visibility = Visibility.Visible;
        }
        private void FilesListView_Drop(object sender, DragEventArgs e)
        {
            if (null != e.Data && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (var url in e.Data.GetData(DataFormats.FileDrop) as string[])
                {
                    if (url.Split('.').Last().ToLower() == "pdf")
                        AddFileToList(url);
                }
                if (_images.Count != 0)
                {
                    AddListViewItem(true);
                    ChangeVisibility(true);
                    this.filesListView.SelectedIndex = 0;
                }
                else return;
            }
        }
        private void LoadFileButton_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pdf Files|*.pdf";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var url in openFileDialog.FileNames)
                    AddFileToList(url);

                if (_images.Count != 0)
                {
                    AddListViewItem(true);
                    ChangeVisibility(true);
                    this.filesListView.SelectedIndex = 0;
                }
                else return;
            }
        }

        private void AddFileToList(string url)
        {
            Model.Image img = new Model.Image
            {
                Url = url,
                Ratio = 0,
                Rotation = 0,
                SwitchPixels = false,
                Extension = "pdf"
            };
            try
            {
                var reader = new PdfReader(img.Url);
                img.MaxNumberOfPages = reader.NumberOfPages;
                _pdf.ImagePDF = img;
                img.Bitmap = _pdf.GetBitmapImage();
                _images.Add(img);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void LoadFileButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => this.LabelDropInfo2.Opacity = 1;
        private void LoadFileButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => this.LabelDropInfo2.Opacity = 0.6;
        private void PdfViewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = pdfViewGrid.ActualHeight * 2;
            pdfViewGrid.Width = newSize / 3;
        }

        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filesListView.SelectedIndex == -1)
                return;
            try
            {
                LastSelectedIndex = this.filesListView.SelectedIndex;
                this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
                // this.ratioComboBox.SelectedIndex = this._images[LastSelectedIndex].Ratio;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save image file",
                Filter = "JPG (.jpg)|*.jpg" // Filter files by extension
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var currentIndex = filesListView.SelectedIndex;
                    //await Application.Current.Dispatcher.InvokeAsync(ImageToPdf);
                    _images[currentIndex].Size = new System.Drawing.Size(1200, 1800);
                    _images[currentIndex].MaxQualityFlag = true;
                    _pdf.ImagePDF = _images[currentIndex];

                    var image = _pdf.GetBitmapImage();
                    var preparedImage = _pdf.PrepareBitmapImageToSave(image);
                    preparedImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    var what = ex.Message;
                    throw;
                }
            }
        }

        private async Task SaveImageAsPdf(SaveFileDialog saveFileDialog)
        {
            _images[LastSelectedIndex].OutputUrl = saveFileDialog.FileName;
            _pdf.ImagePDF = _images[LastSelectedIndex];
            await Application.Current.Dispatcher.InvokeAsync(_pdf.ImageToPdf);
        }

        private async Task<BitmapImage> GetImage()
        {
            _pdf.ImagePDF = this._images[LastSelectedIndex];
            return await Application.Current.Dispatcher.InvokeAsync(_pdf.ImageToBitmap);
        }

        private void ClearButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (filesListView.SelectedIndex == -1)
                return;

            if (_images.Count != 0)
            {
                _images[LastSelectedIndex].Dispose();
                _images.RemoveAt(LastSelectedIndex);
            }
            if (_images.Count != 0)
            {
                AddListViewItem(true);
                filesListView.SelectedIndex = 0;
            }
            else
            {
                filesListView.Items.Clear();
                filesListView.Items.Refresh();
                pdfImage.Source = null;

                ChangeVisibility();
            }
        }

        private async void RotateButton_Click(object sender, MouseButtonEventArgs e)
        {
            this._images[LastSelectedIndex].Rotation = _images[LastSelectedIndex].Rotation >= 3 ?
                _images[LastSelectedIndex].Rotation = 0 :
                ++_images[LastSelectedIndex].Rotation;
            this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
            this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        }

        private async void SwitchPixelsButton_Click(object sender, MouseButtonEventArgs e)
        {
            this._images[LastSelectedIndex].SwitchPixels = !this._images[LastSelectedIndex].SwitchPixels;
            this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
            this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
        }

        private async void RatioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filesListView.Items.Count != 0)
            {
                // this._images[LastSelectedIndex].Ratio = ratioComboBox.SelectedIndex;
                this._images[LastSelectedIndex].Bitmap = await Task.Run(() => GetImage());
                this.pdfImage.Source = this._images[LastSelectedIndex].Bitmap;
            }
        }
        private async void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            //if (await (App.Current as App).pdfMergeUserControl.AddForeignImages(this._images))
            //{
            //    _navigation = System.Windows.Navigation.NavigationService.GetNavigationService(this);
            //    _navigation.Navigate((App.Current as App).pdfMergeUserControl);
            //}
        }

        private void saveAllButton_MouseEnter(object sender, MouseEventArgs e) => this.saveAllButton.Opacity = 0.8;
        private void saveAllButton_MouseLeave(object sender, MouseEventArgs e) => this.saveAllButton.Opacity = 1;
        private void saveSelectedButton_MouseEnter(object sender, MouseEventArgs e) => this.saveSelectedButton.Opacity = 0.8;
        private void saveSelectedButton_MouseLeave(object sender, MouseEventArgs e) => this.saveAllButton.Opacity = 1;

    }
}
