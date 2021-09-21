using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCore.ViewModel
{
    public class ImageToPdfViewModel : PortableDocumentFormatManager
    {
        public ImageToPdfViewModel()
        {
            SetCurrentMode(pdfOnly: false, singleSlider: false);
            SetPdfHighQuality(highQuality: false);
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        private async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Images),
                Multiselect = false,
                Title = (string)(System.Windows.Application.Current as App).Resources.MergedDictionaries[0]["openFile"]
            }) ; ;
            var dialogResult = openDialog.RunDialog();
            if (dialogResult is not null)
            {
                //return await LoadFile(new[] { dialogResult.First() });
                await LoadImage(new FileInfo(dialogResult.First()));
                return true;
            }
            return await Task.Run(() => false);
        }

        protected override async Task<bool> LoadFileViaDragDrop(IEnumerable<FileInfo> fileNames)
        {
            var first = fileNames.First();
            if (first.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg) ||
                first.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpg) ||
                first.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.png))
            {
                await LoadImage(new FileInfo(first.FullName));
                return await Task.Run(() => true);
            }
            return await Task.Run(() => false);
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

        public RelayCommand RotateImage => new(RotateImageCommand, true);

        private void RotateImageCommand()
        {

        }

        public RelayCommand SwitchImage => new(SwitchImageCommand, true);

        private void SwitchImageCommand()
        {

        }

        public RelayCommand RatioImage => new(RatioImageCommand, true);

        private void RatioImageCommand()
        {

        }
    }
}
