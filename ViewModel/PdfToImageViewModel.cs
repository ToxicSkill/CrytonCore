using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCore.ViewModel
{
    public class PdfToImageViewModel : PortableDocumentFormatManager
    {
        public PdfToImageViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
            SetPdfHighQuality(highQuality: true);
            InitializeEmptyCollection();
        }

        private void InitializeEmptyCollection()
        {
            FilesView = new
                System.Collections.ObjectModel.ObservableCollection<FileListView>(){
                new FileListView(){
                    FileName=string.Empty,
                    FilePath=string.Empty,
                    Order=0
                }
            };
        }

        public async Task<bool> LoadFileViaDialog()
        {
            WindowDialogs.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.EnumToString(Enums.EDialogFilters.DialogFilters.Pdf),
                Multiselect = false,
                Title = "Open file"
            }); ;
            var dialogResult = openDialog.RunDialog();
            if (dialogResult is not null)
            {
                Clear.Execute(null);
                return await LoadFile(new[] { dialogResult.First() });
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public RelayCommand SaveCurrent => new(SaveCurrentCommand, true);

        private void SaveCurrentCommand()
        {
            WindowDialogs.SaveDialog saveDialog = new(new Helpers.DialogHelper()
            {
                DefaultExtension = Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg),
                Filters = Enums.EDialogFilters.EnumToString(Enums.EDialogFilters.DialogFilters.Jpeg),
                Title = "Save file"
            });
            var dialogResult = saveDialog.RunDialog();
            if (dialogResult is not null)
                _ = SavePdfPageImage(dialogResult.First());
        }
    }
}
