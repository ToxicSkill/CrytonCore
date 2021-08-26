using CrytonCore.Helpers;
using CrytonCore.Infra;
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
    public abstract class PortableDocumentFormatManager : NotificationClass, IFileDragDropTarget
    {
        protected delegate void VisibilityDelegate(bool show);

        protected VisibilityDelegate VisibilityChangeDelegate;

        protected Dictionary<int, int> SingleSliderDictionary { get; set; }
        private Mode CurrentMode { get; set; }

        private class Mode
        {
            public bool SingleSlide { get; set; }

            public bool OnlyPdf { get; set; } = true;
        }

        protected ObservableCollection<int> OrderVector { get; set; } = new ObservableCollection<int>();

        private PDF _PDF = new();
        private ImageSlider Slider { get; }
        private bool FirstRun { get; set; }
        protected ObservableCollection<PDF> PDFCollection { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }

        protected PortableDocumentFormatManager()
        {
            FilesView = new ObservableCollection<FileListView>();
            PDFCollection = new ObservableCollection<PDF>();
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
                if (task.Result)
                    OrderVector.Add(i);
            if (!res.Any(x => x)) return false;
            await UpdateListViewImages();
            UpdateSlider();
            await Task.Run(() => VisibilityChangeDelegate(true));
            return true;
        }

        private async Task<bool> AddFileToList(string url)
        {
            PDF newPDF = new();
            try
            {
                await newPDF.LoadPdf(url);
                PDFCollection.Add(newPDF);
            }
            catch (Exception)
            {
                return false;
            }
            return newPDF.Bytes.Length > 0;
        }
        private async Task UpdateListViewImages()
        {
            if (FilesView.Count == 0)
                FirstRun = true;
            FilesView.Clear();
            var orderIndex = 0;
            foreach (var pdf in PDFCollection)
            {
                orderIndex++;
                var file = new FileListView()
                {
                    Order = orderIndex,
                    FileName = pdf.Name
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
                _PDF = PDFCollection[OrderVector[SelectedItemIndex]];
                _PDF.CurrentPage = Slider.CurrentIndex;
                BitmapSource = await _PDF.GetImageFromPdf();
            }
            else
            {
                if (SingleSliderDictionary.Count > 0)
                {
                    try
                    {
                        var currentIndex = SingleSliderDictionary[Slider.CurrentIndex];
                        _PDF = PDFCollection[currentIndex];
                        var max = 0;
                        if (currentIndex > 0)
                        {
                            var keys = SingleSliderDictionary.Where(x => x.Value == currentIndex - 1).Select(x => x.Key);
                            max = keys.Max();
                        }
                        var currentPage = Slider.CurrentIndex;
                        _PDF.CurrentPage =
                        currentIndex > 0 ?
                        currentPage - max - 1 :
                        currentPage;
                        BitmapSource = await _PDF.GetImageFromPdf();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
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
                OnPropertyChanged(nameof(BitmapSource));
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
                SliderMaximum =  _PDF.TotalPages - 1;
                SliderVisibility = SliderMaximum == 0 ? Visibility.Hidden : Visibility.Visible;
            }
            if (CurrentMode.SingleSlide)
            {
                var keys = SingleSliderDictionary.Where(x => x.Value == SelectedItemIndex).Select(x => x.Key);
                if (keys.Any())
                    SliderValue = keys.Min();
            }
        }


        async void IFileDragDropTarget.OnFileDropAsync(string[] filePaths)
        {
            _ = await LoadFile(filePaths);
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
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        public int SliderMaximum
        {
            get => Slider.MaxIndex;
            protected set
            {
                Slider.MaxIndex = value;
                OnPropertyChanged(nameof(SliderMaximum));
            }
        }

        private Visibility _sliderVisibility = Visibility.Visible;

        public Visibility SliderVisibility
        {
            get => _sliderVisibility;
            set
            {
                _sliderVisibility = value;
                OnPropertyChanged(nameof(SliderVisibility));
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
                OnPropertyChanged(nameof(SelectedItemIndex));
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
                OnPropertyChanged(nameof(VisibilityDefaultAsShowed));
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => _visibilityHidden;
            set
            {
                _visibilityHidden = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsHidden));
            }
        }
    }
}