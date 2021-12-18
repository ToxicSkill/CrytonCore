using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrytonCore.Helpers;
using CrytonCore.Infra;
using CrytonCore.Model;

namespace CrytonCore.ViewModel
{
    public class PdfMergeSummaryViewModel : PDFPageManager
    {
        private readonly string _defaultTemporaryPath = @"temp.pdf";

        public PdfMergeSummaryViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: true);
        }

        public async Task<bool> Update(
            ObservableCollection<PDF> images,
            ObservableCollection<int> orderVector)
        {
            ClearCollections();

            List<FileListView> views = new();

            for (var i = 0; i < images.Count; i++)
            {
                PdfCollection.Add(images[orderVector[i]]);
                views.Add(new() { FileName = PdfCollection.Last().Info.Name, Order = i + 1});
            }

            var mergeList = (from pdf 
                             in PdfCollection 
                             select (new Tuple<PdfPassword, FileInfo>(pdf.Password, pdf.Info))
                             .ToValueTuple())
                             .ToList();

            if (await MergePdf(mergeList, _defaultTemporaryPath))
            {
                ClearCollections();
                if (await LoadPdfFile(new() { new(_defaultTemporaryPath) }))
                {
                    FilesView = new(views);
                    OnPropertyChanged(nameof(FilesView));
                    return true;
                }
            }

            return false;
        }

        private void ClearCollections()
        {
            PdfCollection.Clear();
            OrderVector.Clear();
            FilesView.Clear();
        }

        public RelayCommand Cancel => new(CancelCommand, true);

        private void CancelCommand()
        {
            App.GoPdfMergePage.Invoke();
        }

        public RelayAsyncCommand<object> SaveMerged => new(SaveMergedCommand);

        private async Task<bool> SaveMergedCommand(object o)
        {
            WindowDialogs.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Pdf),
                Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["saveFiles"]
            }); ;
            var dialogResult = saveDialog.RunDialog();
            if (dialogResult is not null)
            {
                await SavePdf(dialogResult.First(), PdfCollection.First().Bytes);
            }
            return false;
        }
    }
}
