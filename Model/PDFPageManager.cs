using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.ViewModel;
using CrytonCore.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CrytonCore.Model
{
    public abstract class PDFPageManager : PageManager, IFileDragDropTarget
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

        protected ObservableCollection<int> OrderVector { get; set; } = new();
        protected ObservableCollection<PDF> PDFCollection { get; }
        protected ObservableCollection<BitmapImage> ImagesCollection { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }

        protected Dictionary<int, int> SingleSliderDictionary { get; set; }

        public delegate void RatioComboBoxDelegate();
        public RatioComboBoxDelegate RatioDelegate;
        public List<PdfPassword> IncorrectPdfList = new();

        protected PDFPageManager()
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

        protected PDF GetCurrentPDF()
        {
            return PDFCollection[OrderVector[SelectedItemIndex]];
        }

        protected void UpdatePdfImageDimensions()
        {
            PDFCollection[OrderVector[SelectedItemIndex]].CurrentHeight = (int)_imageBitmap.Height;
            PDFCollection[OrderVector[SelectedItemIndex]].CurrentWidth = (int)_imageBitmap.Width;
        }


        async void IFileDragDropTarget.OnFileDropAsync(string[] filePaths)
        {
            var fileInfos = filePaths.Select(f => new PdfPassword() { Name = new(f)});
            while (true)
            {
                if (await LoadPdfFile(fileInfos.ToList()))
                    break;
                if (CreatePasswordProviderInstantion().ShowDialog() == false)
                {
                    IncorrectPdfList = IncorrectPdfList.Where(f => f.Password == String.Empty).ToList();
                    break;
                }
                fileInfos = IncorrectPdfList.Where(x => x.Password.Length > 0).ToList();
                IncorrectPdfList.Clear();
            }
        }

        private PasswordProviderWindow CreatePasswordProviderInstantion()
        {
            var dataContextPasswordProvider = new PasswordProviderViewModel(IncorrectPdfList);
            PasswordProviderWindow passwordProvider = new(dataContextPasswordProvider);
            return passwordProvider;
        }

        protected override async Task<bool> LoadPdfFile(IEnumerable<PdfPassword> pdfFiles)
        {
            var tasks = pdfFiles.Select(async url => await AddFileToList(url)).ToList();
            var res = await Task.WhenAll(tasks);
            var allPdfsCorrect = true;
            foreach (var task in tasks.Select((value, i) => new { i, value }))
            {
                if (task.value.Result)
                {
                    if (OrderVector.Count == 0)
                        OrderVector.Add(0);
                    else
                        OrderVector.Add(OrderVector.Max() + 1);
                }
                else
                {
                    IncorrectPdfList.Add(pdfFiles.ToList()[task.i]);
                    allPdfsCorrect = false;
                }
            }
            
            if (!res.Any(x => x)) return false;
            await UpdateListViewImages();
            UpdateSlider();
            ChangeVisibility(true);
            SetSelectedItemIndex(OrderVector.Max());
            return allPdfsCorrect;
        }

        private async Task<bool> AddFileToList(PdfPassword pdfFile)
        {
            var countBeforeAdd = PDFCollection.Count;
            try
            {
                if (CurrentMode.OnlyPdf)
                {
                    var newPdf = await PDFManager.LoadPdf(pdfFile);
                    if (newPdf != null)
                        PDFCollection.Add(newPdf);
                    else
                        return false;
                }
                else
                {
                    PDFCollection.Add(await PDFManager.LoadImage(pdfFile.Name));
                }
            }
            catch (Exception)
            {
                return false;
            }
            return PDFCollection.Count - countBeforeAdd > 0;
        }

        protected void RemoveIndexes(int selectedIndex)
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

        public RelayCommand Delete => new(DeleteCommand, true);

        private void DeleteCommand()
        {
            RemoveIndexes(SelectedItemIndex);
            UpdateListView();
            if (SelectedItemIndex != 0)
                SelectedItemIndex--;
            if (FilesView.Count == 0)
                ChangeVisibility(false);
        }

        public RelayCommand Clear => new(ClearCommand, true);

        private void ClearCommand()
        {
            ClearIndexes();
            UpdateListView();
            ChangeVisibility(false);
        }

        protected void SetSelectedItemIndex(int index) => SelectedItemIndex = index;

        protected void SetSliderIndex(int index) => SliderValue = index;

        private void ClearIndexes()
        {
            SetSelectedItemIndex(0);
            SetSliderIndex(0);
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
            _PDF = PDFCollection[OrderVector[SelectedItemIndex]];
            _PDF.CurrentPage = Slider.CurrentIndex;

            if (!CurrentMode.OnlyPdf)
            {
                RatioDelegate.Invoke();
                BitmapSource = await PDFManager.ManipulateImage(_PDF);
                return;
            }
            if (!CurrentMode.SingleSlide)
            {
                BitmapSource = await PDFManager.GetImageFromPdf(_PDF);
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
                        BitmapSource = await PDFManager.GetImageFromPdf(_PDF);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        protected bool SavePdfPageImage(string path)
        {
            return PDFManager.SavePdfPageImage(path, BitmapSource);
        }

        protected async Task<bool> SavePdfPagesImages(string path)
        {
            return await PDFManager.SavePdfPagesImages(_PDF, path);
        }

        protected static async Task<bool> MergePdf(List<string> files, string outFile)
        {
            return await PDFManager.MergePdf(files, outFile);
        }

        private BitmapImage _imageBitmap;

        public BitmapImage BitmapSource
        {
            get => _imageBitmap;
            set
            {
                if (value == null)
                    return;
                _imageBitmap = value;
                OnPropertyChanged(nameof(BitmapSource));
            }
        }

        private void UpdateSlider()
        {
            if (!CurrentMode.SingleSlide)
            {
                SliderMaximum = _PDF.TotalPages - 1;
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
                if (value == -1 || _selectedItemIndex == value) return;
                _selectedItemIndex = value;
                _ = UpdateImageSourceAsync();
                UpdateSlider();
                OnPropertyChanged(nameof(SelectedItemIndex));
            }
        }
    }
}