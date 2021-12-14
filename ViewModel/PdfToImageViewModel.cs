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
    public class PdfToImageViewModel : PDFPageManager
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

        protected async Task<bool> LoadPdfFileViaDragDrop(IEnumerable<FileInfo> pdfsInfos)
        {
            Clear.Execute(null);
            return await LoadPdfFile(pdfsInfos.ToList());
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        private async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Pdf),
                Multiselect = false,
                Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["openFile"]
            }); ;
            var dialogResult = openDialog.RunDialog();
            if (dialogResult != null)
            {
                Clear.Execute(null);
                return await LoadPdfFile(new() { new FileInfo(dialogResult.First()) });
            }
            return false;
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
            WindowDialogs.SaveDialog saveDialog = new(new DialogHelper()
            {
                DefaultExtension = Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg),
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Images),
                Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["saveFile"]
            });
            var dialogResult = saveDialog.RunDialog();
            if (dialogResult is not null)
                _ = SavePdfPageImage(dialogResult.First());
        }

        public RelayAsyncCommand<object> SaveAll => new(SaveAllCommand);

        private async Task<bool> SaveAllCommand(object o)
        {
            WindowDialogs.FolderDialog folderDialog = new(new DialogHelper()
            {
                Title = "Chose folder"
            }); ;
            var dialogResult = folderDialog.RunDialog();
            return dialogResult is not null
                && await SavePdfPagesImages(dialogResult.First());
        }
    }
}
