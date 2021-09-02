using CrytonCore.Helpers;
using CrytonCore.Infra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CrytonCore.Interfaces;
using Image = CrytonCore.Model.Image;

namespace CrytonCore.Model
{
    public abstract class PortableDocumentFormatManager : PageManager
    {
        private class Mode
        {
            public bool SingleSlide { get; set; }

            public bool OnlyPdf { get; set; } = true;
        }

        private class ImageSlider
        {
            public int CurrentIndex { get; set; }

            public int LastIndex { get; set; }

            public int MaxIndex { get; set; }
        }

        private Mode CurrentMode { get; set; }
        private ImageSlider Slider { get; }

        private bool FirstRun { get; set; }

        private PDF _PDF = new();

        protected ObservableCollection<int> OrderVector { get; set; } = new ObservableCollection<int>();
        protected ObservableCollection<PDF> PDFCollection { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }

        protected Dictionary<int, int> SingleSliderDictionary { get; set; }

        protected PortableDocumentFormatManager()
        {
            FilesView = new ObservableCollection<FileListView>();// { new FileListView() { FileName = ":dadaw", FilePath="dadw", Order = 1 } };

            PDFCollection = new ObservableCollection<PDF>();
            Slider = new ImageSlider() { CurrentIndex = 0 };
        }

        protected void SetCurrentMode(bool pdfOnly, bool singleSlider)
        {
            CurrentMode = new Mode() { OnlyPdf = pdfOnly, SingleSlide = singleSlider };
        }
        protected void SetPdfHighQuality(bool highQuality)
        {
            _PDF.SetHighQuality(highQuality);
        }

        public override async Task<bool> LoadFile(IEnumerable<string> fileNames)
        {
            var tasks = fileNames.Select(async url => await AddFileToList(url)).ToList();
            var res = await Task.WhenAll(tasks);
            foreach (var (i, task) in tasks.Select((value, index) => (index, value)))
                if (task.Result)
                    OrderVector.Add(i);
            if (!res.Any(x => x)) return false;
            await UpdateListViewImages();
            UpdateSlider();
            ChangeVisibility(true);
            return true;
        }

        private async Task<bool> AddFileToList(string url)
        {
            PDF newPDF = new(_PDF);
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

        public void RemoveIndexes(int selectedIndex)
        {
            FilesView.RemoveAt(selectedIndex);
            PDFCollection.RemoveAt(OrderVector[selectedIndex]);
            var orderValue = OrderVector[selectedIndex];
            OrderVector.RemoveAt(selectedIndex);
            for (var i = 0; i < OrderVector.Count; i++)
            {
                if (OrderVector[i] > orderValue)
                    OrderVector[i]--;
            }
        }

        public RelayCommand Clear => new(ClearCommand, true);

        private void ClearCommand()
        {
            ClearIndexes();
            UpdateListView();
            SelectedItemIndex = 0;
            ChangeVisibility(false);
        }

        private void ClearIndexes()
        {
            FilesView.Clear();
            OrderVector.Clear();
            PDFCollection.Clear();
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
                if (_selectedItemIndex == value) return;
                if (value == -1 && _selectedItemIndex != value) return;
                _selectedItemIndex = value;
                _ = UpdateImageSourceAsync();
                UpdateSlider();
                OnPropertyChanged(nameof(SelectedItemIndex));
            }
        }
    }
}