using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

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
            RatioDelegate = new(ChangeRatioComboBoxItemCurrentImage);
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

        private void ChangeRatioComboBoxItemCurrentImage()
        {
            SelectedRatio = Ratios[GetCurrentPDF().RatioIndex];
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

        private static async Task<string> GetSavePath()
        {
            WindowDialogs.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Pdf),
                Title = (string)((App)System.Windows.Application.Current).Resources.MergedDictionaries[0]["saveFiles"]
            }); ;
            var dialogResult = saveDialog.RunDialog();
            return await Task.Run(() => dialogResult is not null ? dialogResult.First() : string.Empty);
        }

        public RelayAsyncCommand<object> MoveNext => new(MoveNextCommand);

        private async Task MoveNextCommand(object o)
        {
            var currnetPdf = GetCurrentPDF();
            _ = await PDFManager.ImageToPdf(currnetPdf, await PDFManager.ManipulateImage(GetCurrentPDF()), await GetSavePath());
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

        private void RotateCurrentImage90Degrees()
        {
            if (++PDFCollection[OrderVector[SelectedItemIndex]].Rotation == 4)
                PDFCollection[OrderVector[SelectedItemIndex]].Rotation = 0;
        }

        public RelayAsyncCommand<object> RotateImage => new(RotateImageCommand);

        private async Task RotateImageCommand(object o)
        {
            RotateCurrentImage90Degrees();
            BitmapSource = await PDFManager.ManipulateImage(GetCurrentPDF());
        }

        private void SwitchPixelsCurrentImage()
        {
            PDFCollection[OrderVector[SelectedItemIndex]].SwitchPixels ^= true; 
        }

        public RelayAsyncCommand<object> SwitchImage => new(SwitchImageCommand);

        private async Task SwitchImageCommand(object o)
        {
            SwitchPixelsCurrentImage();
            BitmapSource = await PDFManager.ManipulateImage(GetCurrentPDF());
        }

        private void ChangeRatioCurrentImage()
        {
            PDFCollection[OrderVector[SelectedItemIndex]].Ratio = ValueRatios[_ratioIndex];
            PDFCollection[OrderVector[SelectedItemIndex]].RatioIndex = _ratioIndex;
        }

        public RelayAsyncCommand<object> RatioImage => new(RatioImageCommand);

        private async Task RatioImageCommand(object o)
        {
            ChangeRatioCurrentImage();
            BitmapSource = await PDFManager.ManipulateImage(GetCurrentPDF());
        }
        private BlurEffect _effect;
        private BlurEffect _effectCombo;

        public BlurEffect Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                OnPropertyChanged(nameof(Effect));
            }
        }
        public BlurEffect EffectCombo
        {
            get => _effectCombo;
            set
            {
                _effectCombo = value;
                OnPropertyChanged(nameof(EffectCombo));
            }
        }

        public RelayCommand EffectComboFocusLost => new(EffectComboFocusLostCommand, true);

        private void EffectComboFocusLostCommand()
        {
            Effect = null;
            EffectCombo = null;
        }

        public RelayCommand EffectComboClick => new(EffectComboClickCommand, true);

        private void EffectComboClickCommand()
        {
            BlurEffect newEffect = new()
            {
                Radius = 15
            };
            Effect = newEffect;
            EffectCombo = null;
        }
    }
}
