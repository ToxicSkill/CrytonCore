using CrytonCore.Infra;
using CrytonCore.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Image = CrytonCore.Model.Image;

namespace CrytonCore.Model
{
    public abstract class PortableDocumentFormatManager : NotificationClass
    {
        protected delegate void VisibilityDelegate(bool show);

        protected VisibilityDelegate VisibilityChangeDelegate;

        protected Dictionary<int, int> SingleSliderDictionary { get; set; }
        private Mode CurrentMode { get; set; }

        private class Mode
        {
            public bool SingleSlide { get; set; } = false;

            public bool OnlyPdf { get; set; } = true;
        }

        protected ObservableCollection<int> OrderVector { get; set; } = new ObservableCollection<int>();

        private readonly PortableDocumentFormat _pdf = new PortableDocumentFormat();
        private ImageSlider Slider { get; }
        private bool FirstRun { get; set; }
        protected ObservableCollection<Image> Images { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }

        protected PortableDocumentFormatManager()
        {
            FilesView = new ObservableCollection<FileListView>();
            Images = new ObservableCollection<Image>();
            Slider = new ImageSlider() { CurrentIndex = 0 };
        }

        protected void SetCurrentMode(bool pdfOnly, bool singleSlider)
        {
            CurrentMode = new Mode() { OnlyPdf = pdfOnly, SingleSlide = singleSlider };
        }

        public async Task<bool> LoadFile(IEnumerable<string> fileNames)
        {
            var tasks = fileNames.Select(async url => await AddFileToList(url)).ToList();
            var res = await Task.WhenAll(tasks);
            foreach (var (i, task) in tasks.Select((value, index) => (index, value)))
            {
                if (task.Result)
                    OrderVector.Add(i);
            }

            if (!res.Any(x => x)) return false;
            await UpdateListViewImages();
            UpdateSlider(); 
            await Task.Run(() => VisibilityChangeDelegate(true));
            return true;
        }
        private async Task<bool> AddFileToList(string url, bool pdfFormat = true)
        {
            var extension = url.Split('.').Last().ToLower();

            if (extension == "pdf")
                pdfFormat = true;

            if (Enums.Enumerates.ImagesExtensions.gif.ToString("g").Equals(extension) ||
                Enums.Enumerates.ImagesExtensions.jpg.ToString("g").Equals(extension) ||
                Enums.Enumerates.ImagesExtensions.jpeg.ToString("g").Equals(extension))
                pdfFormat = false;

            var img = new Image
            {
                Url = url,
                Extension = url.Split('.').Last().ToLower()
            };
            try
            {
                if (pdfFormat)
                {
                    var reader = new iTextSharp.text.pdf.PdfReader(img.Url);
                    img.MaxNumberOfPages = reader.NumberOfPages;

                }
                else
                {
                    img.MaxNumberOfPages = 1;
                }
                _pdf.ImagePDF = img;
                await _pdf.LoadPdf();
                Images.Add(img);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
            
        }

        private async Task UpdateListViewImages()
        {
            if (FilesView.Count == 0)
                FirstRun = true;
            FilesView.Clear();
            var orderIndex = 0;
            foreach (var image in Images)
            {
                orderIndex++;
                var file = new FileListView()
                {
                    Order = orderIndex,
                    FileName = image.Url.Split('\\').Last()
                };

                FilesView.Add(file);
            }
            if (FirstRun)
                await UpdateImageSourceAsync();
            FirstRun = false;
        }

        protected void UpdateListView()
        {
            var filesViewCopy = new ObservableCollection<FileListView>(FilesView);
            FilesView.Clear();
            var orderIndex = 0;
            foreach (var item in filesViewCopy)
            {
                orderIndex++;
                var file = new FileListView()
                {
                    Order = orderIndex,
                    FileName = item.FileName
                };

                FilesView.Add(file);
            }
        }

        protected async Task UpdateImageSourceAsync()
        {
            if (!CurrentMode.SingleSlide)
            {
                var currentIndex = OrderVector[SelectedItemIndex];
                _pdf.ImagePDF = Images[currentIndex];
                _pdf.ImagePDF.CurrentNumberOfPage = Slider.CurrentIndex;
                //_pdf.ImagePDF.Bitmap = await Application.Current.Dispatcher.InvokeAsync(_pdf.GetBitmapImage);
                _pdf.ImagePDF.Bitmap = await _pdf.GetPdfPageImage();
                BitmapSource = _pdf.ImagePDF.Bitmap;
            }
            else
            {
                if (SingleSliderDictionary.Count > 0)
                {
                    try
                    {
                        var currentIndex = SingleSliderDictionary[Slider.CurrentIndex];
                        _pdf.ImagePDF = Images[currentIndex];
                        var max = 0;
                        if (currentIndex > 0)
                        {
                            var keys = SingleSliderDictionary.Where(x => x.Value == currentIndex - 1).Select(x => x.Key);
                            max = keys.Max();
                        }
                        var currentPage = Slider.CurrentIndex;
                        _pdf.ImagePDF.CurrentNumberOfPage =
                        currentIndex > 0 ?
                        currentPage - max - 1 :
                        currentPage;

                        //_pdf.ImagePDF.Bitmap = await Application.Current.Dispatcher.InvokeAsync(_pdf.GetPdfPageImage());
                        _pdf.ImagePDF.Bitmap = await _pdf.GetPdfPageImage();
                        BitmapSource = _pdf.ImagePDF.Bitmap;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    //Images[currentIndex].Bitmap = await Application.Current.Dispatcher.InvokeAsync(PDF.GetBitmapImage);
                    //BitmapSource = Images[currentIndex].Bitmap;
                }
            }
        }

        private BitmapImage _imageBitmap;
        public BitmapImage BitmapSource
        {
            get => _imageBitmap;
            private set
            {
                _imageBitmap = value;
                OnPropertyChanged("BitmapSource");
            }
        }

        private class ImageSlider
        {
            public int CurrentIndex { get; set; }

            public int LastIndex { get; set; }

            public int MaxIndex { get; set; }
        }
        private void UpdateSlider()
        {
            if (!CurrentMode.SingleSlide)
            {
                SliderMaximum = _pdf.ImagePDF.MaxNumberOfPages - 1;
                SliderVisibility = SliderMaximum == 0 ? Visibility.Hidden : Visibility.Visible;
            }
            if (CurrentMode.SingleSlide)
            {
                var keys = SingleSliderDictionary.Where(x => x.Value == SelectedItemIndex).Select(x => x.Key);
                if (keys.Count() > 0)
                    SliderValue = keys.Min();
            }
        }

        public int SliderValue
        {
            get => Slider.CurrentIndex;
            set
            {
                if (value == Slider.CurrentIndex)
                    return;
                Slider.LastIndex = Slider.CurrentIndex;
                Slider.CurrentIndex = value;
                _ = UpdateImageSourceAsync();
                OnPropertyChanged("SliderValue");
            }
        }

        public int SliderMaximum
        {
            get => Slider.MaxIndex;
            protected set
            {
                Slider.MaxIndex = value;
                OnPropertyChanged("SliderMaximum");
            }
        }

        private Visibility _sliderVisibility = Visibility.Visible;

        public Visibility SliderVisibility
        {
            get => _sliderVisibility;
            set
            {
                _sliderVisibility = value;
                OnPropertyChanged("SliderVisibility");
            }
        }

        private int _selectedItemIndex;
        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (value == -1 && _selectedItemIndex != value) return;
                _selectedItemIndex = value;
                _ = UpdateImageSourceAsync();
                UpdateSlider();
                OnPropertyChanged("SelectedItemIndex");
            }
        }

        private Visibility _visibilityShowed = Visibility.Visible;
        private Visibility _visibilityHidden = Visibility.Hidden;

        public Visibility VisibilityDefaultAsShowed
        {
            get => _visibilityShowed;
            set
            {
                _visibilityShowed = value;
                OnPropertyChanged("VisibilityDefaultAsShowed");
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => _visibilityHidden;
            set
            {
                _visibilityHidden = value;
                OnPropertyChanged("VisibilityDefaultAsHidden");
            }
        }
    }
}