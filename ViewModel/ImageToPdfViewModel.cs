using CrytonCore.Abstract;
using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.PdfService;
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

        private readonly IRatios _valueRatios;
        private readonly IPdfManager _pdfManager;

        private string _selectedRatio;
        private int _ratioIndex;

        public ImageToPdfViewModel()
        {
            _valueRatios = new Ratios();
            _pdfManager = new PDFManager();
            Ratios = new ObservableCollection<string>(_valueRatios.GetRatiosNames());
            //RatioDelegate = new(ChangeRatioComboBoxItemCurrentImage);
            SetCurrentMode(pdfOnly: false, singleSlider: false);
            SetPdfHighQuality(highQuality: false);
        }

        public override void OnItemChanges()
        {
            SelectedRatio = _valueRatios.GetRatioNameByValue(GetCurrentPDF().GetRatio().CurrentValue);
        }


        public string SelectedRatio
        {
            get => _selectedRatio;
            set
            {
                if (value == _selectedRatio)
                    return;
                _selectedRatio = value;
                _valueRatios.SetCurrentRatioByName(value);
                _ = ChangeRatioComboBoxItemCurrentImage();
                OnPropertyChanged(nameof(SelectedRatio));
            }
        }

        private async Task ChangeRatioComboBoxItemCurrentImage()
        {
            PdfCollection[OrderVector[SelectedItemIndex]].SetRatio(_valueRatios.GetCurrentRatio());
            await UpdateImage();
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
                await LoadPdfFile(dialogResult.Select(f => new FileInfo(f)).ToList());
                return true;
            }
            return await Task.Run(() => false);
        }

        protected async Task<bool> LoadFileViaDragDrop(IEnumerable<FileInfo> fileNames)
        {
            List<FileInfo> filesInfo = new();
            foreach (var file in fileNames)
            {
                if (file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.png))
                    filesInfo.Add(file);
            }
            return await LoadPdfFile(filesInfo);
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
            _ = await _pdfManager.ImageToPdf(currnetPdf, await _pdfManager.ManipulateImage(GetCurrentPDF()), await GetSavePath());
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
            PdfCollection[OrderVector[SelectedItemIndex]].IncrementRotation();
        }

        public RelayAsyncCommand<object> RotateImage => new(RotateImageCommand);

        private async Task RotateImageCommand(object o)
        {
            RotateCurrentImage90Degrees();
            await UpdateImage();
        }

        private void SwitchPixelsCurrentImage()
        {
            PdfCollection[OrderVector[SelectedItemIndex]].SetPixelsSwitch(); 
        }

        public RelayAsyncCommand<object> SwitchImage => new(SwitchImageCommand);

        private async Task SwitchImageCommand(object o)
        {
            SwitchPixelsCurrentImage();
            await UpdateImage();
        }

        private void ChangeRatioCurrentImage()
        {
            PdfCollection[OrderVector[SelectedItemIndex]].SetRatio(_valueRatios.GetCurrentRatio());
            PdfCollection[OrderVector[SelectedItemIndex]].SetRatioIndex(_ratioIndex);
        }

        public RelayAsyncCommand<object> RatioImage => new(RatioImageCommand);

        private async Task RatioImageCommand(object o)
        {
            ChangeRatioCurrentImage();
            await UpdateImage();
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
