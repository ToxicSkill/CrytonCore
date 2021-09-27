using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCore.ViewModel
{
    public class ImageToPdfViewModel : PDFPageManager
    {
        public ObservableCollection<string> Ratios { get; set; }
        private Collection<double> ValueRatios;
        private string _selectedRatio;
        private int _ratioIndex;
        public string SelectedRatio
        {
            get => _selectedRatio;
            set
            {
                if (value == _selectedRatio)
                    return;
                _selectedRatio = value;
                _ratioIndex = Ratios.IndexOf(_selectedRatio);
                OnPropertyChanged(nameof(SelectedRatio));
            }
        }

        public ImageToPdfViewModel()
        {
            InitializeRatios();
            SetCurrentMode(pdfOnly: false, singleSlider: false);
            SetPdfHighQuality(highQuality: false);
        }

        private void InitializeRatios()
        {
            Ratios = new()
            {
                "Original",
                "1.4142 : 1 (A4)",
                "4:3",
                "16:9",
                "1:1",
                "18:9"
            };
            ValueRatios = new()
            {
                0,
                1.414213562373095,
                1.333333333333333,
                1.777777777777777,
                1,
                2
            };
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        private async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Images),
                Multiselect = true,
                Title = (string)(System.Windows.Application.Current as App).Resources.MergedDictionaries[0]["openFile"]
            }) ; ;
            var dialogResult = openDialog.RunDialog();
            if (dialogResult is not null)
            {
                //return await LoadFile(new[] { dialogResult.First() });
                await LoadFile(dialogResult.Select(f => new FileInfo(f)).ToList());
                return true;
            }
            return await Task.Run(() => false);
        }

        protected override async Task<bool> LoadFileViaDragDrop(IEnumerable<FileInfo> fileNames)
        {
            List<FileInfo> filesInfo = new();
            foreach (var file in fileNames)
            {
                if (file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.png))
                    filesInfo.Add(file);
            }
            return await LoadFile(filesInfo);
        }

        public RelayCommand MoveBack => new(MoveBackCommand, true);

        private void MoveBackCommand()
        {
            try
            {
                App.GoPdfManagerPage.Invoke();
            }
            catch (Exception)
            {
            }
        }

        public RelayAsyncCommand<object> RotateImage => new(RotateImageCommand);

        private async Task RotateImageCommand(object o)
        {
            var currentPDF = GetCurrentPDF();
            currentPDF.Rotation++;
            if (currentPDF.Rotation == 4)
                currentPDF.Rotation = 0;
            BitmapSource = await PDFManager.ManipulateImage(currentPDF);
        }

        public RelayAsyncCommand<object> SwitchImage => new(SwitchImageCommand);

        private async Task SwitchImageCommand(object o)
        {
            var currentPDF = GetCurrentPDF();
            currentPDF.SwitchPixels = !currentPDF.SwitchPixels;
            BitmapSource = await PDFManager.ManipulateImage(currentPDF);
        }

        public RelayAsyncCommand<object> RatioImage => new(RatioImageCommand);

        private async Task RatioImageCommand(object o)
        {
            var currentPDF = GetCurrentPDF();
            currentPDF.Ratio = ValueRatios[_ratioIndex];
            BitmapSource = await PDFManager.ManipulateImage(currentPDF);
        }
    }
}
