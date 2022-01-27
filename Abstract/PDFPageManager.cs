using CrytonCore.Design;
using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.PdfService;
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

namespace CrytonCore.Abstract
{
    public abstract class PDFPageManager : PageManager, IFileDragDropTarget
    {

        private bool FirstRun { get; set; }

        private IMode CurrentMode;
        private readonly IPdfManager _pdfManager;
        private PDF _currentPdf;


        protected ObservableCollection<int> OrderVector { get; set; } = new();
        protected ObservableCollection<PDF> PdfCollection { get; }
        protected ObservableCollection<BitmapImage> ImagesCollection { get; }
        public ObservableCollection<FileListView> FilesView { get; set; }
        public PageSlider Slider = new();
        public delegate void RatioComboBoxDelegate();
        public RatioComboBoxDelegate RatioDelegate;
        public List<PdfPasswordBase> IncorrectPdfList = new();

        protected PDFPageManager()
        {
            _currentPdf = new PDF();
            _pdfManager = new PDFManager();
            FilesView = new ObservableCollection<FileListView>();// { new FileListView() { FileName = ":dadaw", FilePath="dadw", Order = 1 } };
            PdfCollection = new ObservableCollection<PDF>();
            //Slider = new ImageSlider() { CurrentIndex = 0 };
        }

        protected void SetCurrentMode(bool pdfOnly, bool singleSlider)
        {
            CurrentMode = new Mode(pdfOnly, singleSlider);
        }

        protected void SetPdfHighQuality(bool highQuality)
        {
            _currentPdf.SetQuality(highQuality);
        }

        protected PDF GetCurrentPDF()
        {
            return PdfCollection[OrderVector[SelectedItemIndex]];
        }

        protected void UpdatePdfImageDimensions()
        {
            PdfCollection[OrderVector[SelectedItemIndex]].SetCurrentHeight((int)_imageBitmap.Height);
            PdfCollection[OrderVector[SelectedItemIndex]].SetCurrentWidth((int)_imageBitmap.Width);
        }

        private async Task<IPdf> LoadPDF(FileInfo pdfInfo, PdfPassword pdfPassword)
        {
            return await _pdfManager.LoadPdf(pdfInfo, pdfPassword);
        }

        private  async Task<IPdf> LoadImagePDF(FileInfo pdfInfo)
        {
            return await _pdfManager.LoadImage(pdfInfo);
        }

        private async Task<bool> AccumulatePDF(List<(FileInfo info, PdfPassword password)> pdfs)
        {
            foreach (var pdf in pdfs)
            {
                var newPdf = CurrentMode.GetCurrentPdfMode() ?
                    await LoadPDF(pdf.info, pdf.password) :
                    await LoadImagePDF(pdf.info);
                if (newPdf == null)
                {
                    IncorrectPdfList.Add(new(pdf.info));
                    continue;
                }
                PdfCollection.Add((PDF)newPdf);
                FillOrderVector();
            }
            return pdfs.Count - IncorrectPdfList.Count != 0;
        }

        private async Task<bool> AccumulatePDF(List<FileInfo> pdfInfos)
        {
            foreach (var pdf in pdfInfos)
            {
                var newPdf = CurrentMode.GetCurrentPdfMode() ?
                    await LoadPDF(pdf, default) :
                    await LoadImagePDF(pdf);
                if (newPdf == null)
                {
                    IncorrectPdfList.Add(new(pdf));
                    continue;
                }
                PdfCollection.Add((PDF)newPdf);
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

        public virtual async void OnFileDropAsync(string[] filePaths)
        {
            await LoadPdfFile(filePaths.Select(item => new FileInfo(item)).ToList());
        }

        private PasswordProviderWindow CreatePasswordProviderInstantion()
        {
            return new(new PasswordProviderViewModel(IncorrectPdfList));
        }

        protected async Task<bool> LoadPdfFile(List<FileInfo> pdfsFiles)
        {
            if(await AccumulatePDF(pdfsFiles.ToList()))
                await UpdateUI();
            await CheckForIncorrectPDF();
            return true;
        }

        public async Task UpdateImage()
        {
            BitmapSource = await _pdfManager.ManipulateImage(GetCurrentPDF());
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
            PdfCollection.RemoveAt(OrderVector[selectedIndex]);
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
            PdfCollection.Clear();
        }

        private async Task UpdateListViewImages()
        {
            if (FilesView.Count == 0)
                FirstRun = true;
            FilesView.Clear();
            var orderIndex = 0;

            foreach (var pdf in PdfCollection)
            {
                orderIndex++;
                var file = new FileListView()
                {
                    Order = orderIndex,
                    FileName = pdf.Info.Name
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
            return _pdfManager.SavePdfPageImage(path, BitmapSource);
        }

        protected async Task<bool> SavePdfPagesImages(string path)
        {
            return await _pdfManager.SavePdfPagesImages(_currentPdf, path);
        }

        protected async Task<bool> MergePdf(List<(PdfPassword passwords, FileInfo infos)> files, string outFile)
        {
            return await _pdfManager.MergePdf(files, outFile);
        }

        protected async Task<bool> SavePdf(string outFile, byte[] bytes)
        {
            return await _pdfManager.SavePdf(outFile, bytes);
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
            if (!CurrentMode.GetCurrentPdfMode())
            {
                //RatioDelegate.Invoke();
                BitmapSource = await _pdfManager.ManipulateImage(_currentPdf);
                return;
            }
            if (!CurrentMode.GetCurrentSlideMode())
            {
                BitmapSource = await _pdfManager.GetImageFromPdf(_currentPdf);
            }
            else
            {
                try
                {
                    Slider = _currentPdf.GetSlider();
                    var currentPage = Slider.CurrentIndex;
                    _currentPdf.SetCurrentPage(Slider.CurrentIndex > 0 ?
                        currentPage - Slider.MaxIndex - 1 :
                        currentPage);
                    BitmapSource = await _pdfManager.GetImageFromPdf(_currentPdf);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void UpdateCurrentSlider()
        {
            Slider = _currentPdf.GetSlider();

            if (Slider is null)
                return;

            SliderVisibility = Slider.MaxIndex > 0 ? 
                Visibility.Visible : 
                Visibility.Hidden;

            OnPropertyChanged(nameof(SliderValue));
            OnPropertyChanged(nameof(SliderMaximum));
            OnPropertyChanged(nameof(SliderVisibility));
        }

        private async Task UpdateCurrentPDF()
        {
            _currentPdf = PdfCollection[OrderVector[SelectedItemIndex]];
            UpdateCurrentSlider();
            await UpdateImageSourceAsync();
        }

        public int SliderValue
        {
            get => Slider.CurrentIndex;
            set
            {
                if (Slider is null)
                    return;
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
                OnItemChanges();
            }
        }

        public virtual void OnItemChanges()
        {
            return;
        }
    }
}