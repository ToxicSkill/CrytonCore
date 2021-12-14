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

        private Mode CurrentMode { get; set; }

        private bool FirstRun { get; set; }

        private PDF _PDF = new();

        protected ObservableCollection<int> OrderVector { get; set; } = new();
        protected ObservableCollection<PDF> PDFCollection { get; }
        protected ObservableCollection<BitmapImage> ImagesCollection { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }
        public ImageSlider Slider = new();
        public delegate void RatioComboBoxDelegate();
        public RatioComboBoxDelegate RatioDelegate;
        public List<PdfPasswordBase> IncorrectPdfList = new();

        protected PDFPageManager()
        {
            FilesView = new ObservableCollection<FileListView>();// { new FileListView() { FileName = ":dadaw", FilePath="dadw", Order = 1 } };
            PDFCollection = new ObservableCollection<PDF>();
            //Slider = new ImageSlider() { CurrentIndex = 0 };
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

        private static async Task<PDF> LoadPDF(FileInfo pdfInfo, PdfPassword pdfPassword)
        {
            return await PDFManager.LoadPdf(pdfInfo, pdfPassword);
        }

        private static async Task<PDF> LoadImagePDF(FileInfo pdfInfo)
        {
            return await PDFManager.LoadImage(pdfInfo);
        }

        private async Task<bool> AccumulatePDF(string[] paths)
        {
            foreach (var path in paths)
            {
                var pdf = CurrentMode.OnlyPdf ?
                    await LoadPDF(new(path), new()) :
                    await LoadImagePDF(new(path));
                if (pdf == null)
                {
                    IncorrectPdfList.Add(new(path));
                    continue;
                }
                PDFCollection.Add(pdf);
                FillOrderVector();
            }
            return paths.Length - IncorrectPdfList.Count != 0;
        }

        private async Task<bool> AccumulatePDF(List<(FileInfo info, PdfPassword password)> pdfs)
        {
            foreach (var pdf in pdfs)
            {
                var newPdf = CurrentMode.OnlyPdf ?
                    await LoadPDF(pdf.info, pdf.password) :
                    await LoadImagePDF(pdf.info);
                if (newPdf == null)
                {
                    IncorrectPdfList.Add(new(pdf.info));
                    continue;
                }
                PDFCollection.Add(newPdf);
                FillOrderVector();
            }
            return pdfs.Count - IncorrectPdfList.Count != 0;
        }

        private async Task<bool> AccumulatePDF(List<FileInfo> pdfInfos)
        {
            foreach (var pdf in pdfInfos)
            {
                var newPdf = CurrentMode.OnlyPdf ?
                    await LoadPDF(pdf, default) :
                    await LoadImagePDF(pdf);
                if (newPdf == null)
                {
                    IncorrectPdfList.Add(new(pdf));
                    continue;
                }
                PDFCollection.Add(newPdf);
                FillOrderVector();
            }
            return pdfInfos.Count - IncorrectPdfList.Count != 0;
        }

        private void FillOrderVector()
        {
            if (OrderVector.Count == 0)
                OrderVector.Add(0);
            else
                OrderVector.Add(OrderVector.Max() + 1);
        }

        private async Task CheckForIncorrectPDF()
        {
            if (IncorrectPdfList.Count == 0)
                return;
            while (true)
            {
                if (CreatePasswordProviderInstantion().ShowDialog() == false)
                {
                    IncorrectPdfList.Clear();
                    break;
                }
                else
                {
                    List<(FileInfo info, PdfPassword password)> pdfCollection = new();
                    foreach (var item in IncorrectPdfList)
                    {
                        var pdfPassword = new PdfPassword();
                        pdfPassword.SetPassword(item.Password);
                        pdfCollection.Add(new(item.Name, pdfPassword));
                    }
                    var pdfCountBeforeUpdate = IncorrectPdfList.Count;
                    IncorrectPdfList.Clear();
                    await AccumulatePDF(pdfCollection);
                    if(IncorrectPdfList.Count - pdfCountBeforeUpdate != 0)
                    {
                        await UpdateUI();
                        if (IncorrectPdfList.Count == 0)
                            break;
                    }
                }
            }
        }

        async void IFileDragDropTarget.OnFileDropAsync(string[] filePaths)
        {
            await LoadPdfFile(filePaths.Select(item => new FileInfo(item)).ToList());
        }

        private PasswordProviderWindow CreatePasswordProviderInstantion()
        {
            var dataContextPasswordProvider = new PasswordProviderViewModel(IncorrectPdfList);
            return new(dataContextPasswordProvider);
        }

        protected async Task<bool> LoadPdfFile(List<FileInfo> pdfsFiles)
        {
            if(await AccumulatePDF(pdfsFiles.ToList()))
                await UpdateUI();
            await CheckForIncorrectPDF();
            return true;
        }

        private async Task UpdateUI()
        {
            await UpdateListViewImages();
            UpdateCurrentSlider();
            ChangeVisibility(true);
            SetSelectedItemIndex(OrderVector.Max());
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
                await UpdateCurrentPDF();
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

        protected bool SavePdfPageImage(string path)
        {
            return PDFManager.SavePdfPageImage(path, BitmapSource);
        }

        protected async Task<bool> SavePdfPagesImages(string path)
        {
            return await PDFManager.SavePdfPagesImages(_PDF, path);
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

        protected async Task UpdateImageSourceAsync()
        {
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
                try
                {
                    Slider = _PDF.Slider;
                    var currentPage = Slider.CurrentIndex;
                    _PDF.CurrentPage =
                    Slider.CurrentIndex > 0 ?
                    currentPage - Slider.MaxIndex - 1 :
                    currentPage;
                    BitmapSource = await PDFManager.GetImageFromPdf(_PDF);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void UpdateCurrentSlider()
        {
            Slider = _PDF.Slider;

            if (Slider is null)
                return;

            SliderVisibility = Slider.MaxIndex > 0 ? Visibility.Visible : Visibility.Hidden;

            OnPropertyChanged(nameof(SliderValue));
            OnPropertyChanged(nameof(SliderMaximum));
            OnPropertyChanged(nameof(SliderVisibility));
        }

        private async Task UpdateCurrentPDF()
        {
            _PDF = PDFCollection[OrderVector[SelectedItemIndex]];
            UpdateCurrentSlider();
            await UpdateImageSourceAsync();
        }

        public int SliderValue
        {
            get => Slider.CurrentIndex;
            set
            {
                if (Slider.CurrentIndex == value)
                    return;
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
                _ = UpdateCurrentPDF();
                OnPropertyChanged(nameof(SelectedItemIndex));
            }
        }
    }
}