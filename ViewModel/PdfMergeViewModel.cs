using CrytonCore.Infra;
using CrytonCore.Model;
using CrytonCore.Views;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCore.ViewModel
{
    public class PdfMergeViewModel : PDFPageManager
    {
        private SummaryPdfMergePage _summaryPage;

        public PdfMergeViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
            _summaryPage = new SummaryPdfMergePage();
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        private async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new Helpers.DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Pdf),
                Multiselect = true,
                Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["openFiles"]
            });
            var dialogResult = openDialog.RunDialog();
            return dialogResult is not null ? await LoadFile(dialogResult.Select(f => new FileInfo(f)).ToList()) : await Task.Run(() => { return false; });
        }

        private void MoveIndexes(int selectedIndex, int newIndex)
        {
            OrderVector.Move(selectedIndex, newIndex);
            FilesView.Move(selectedIndex, newIndex);
        }

        public RelayCommand SetAsFirst => new(SetAsFirstCommand, true);

        private void SetAsFirstCommand()
        {
            if (SelectedItemIndex == 0)
                return;
            MoveIndexes(SelectedItemIndex, 0);
            UpdateListView();
            SelectedItemIndex = 0;
        }

        public RelayCommand SetAsLast => new(SetAsLastCommand, true);

        private void SetAsLastCommand()
        {
            var lastIndex = FilesView.Count - 1;
            if (SelectedItemIndex == lastIndex)
                return;
            MoveIndexes(SelectedItemIndex, lastIndex);
            UpdateListView();
            SelectedItemIndex = lastIndex;
        }

        public RelayCommand MoveDown => new(MoveDownCommand, true);

        private void MoveDownCommand()
        {
            var newIndex = SelectedItemIndex + 1;
            if (SelectedItemIndex == FilesView.Count - 1)
                return;
            MoveIndexes(SelectedItemIndex, newIndex);
            UpdateListView();
            SelectedItemIndex = newIndex;
        }

        public RelayCommand MoveUp => new(MoveUpCommand, true);

        private void MoveUpCommand()
        {
            var newIndex = SelectedItemIndex - 1;
            if (SelectedItemIndex == 0)
                return;
            MoveIndexes(SelectedItemIndex, newIndex);
            UpdateListView();
            SelectedItemIndex = newIndex;
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
        public RelayCommand MoveNext => new(MoveNextCommand, true);

        private void MoveNextCommand()
        {
            try
            {
                App.GoSummaryPdfMergePage.Invoke(GetSummaryPage());
            }
            catch (Exception)
            {
            }
        }

        private SummaryPdfMergePage GetSummaryPage()
        {
            if (_summaryPage == null) _summaryPage = new SummaryPdfMergePage();
            var result = ((PdfMergeSummaryViewModel)_summaryPage.DataContext).Update(FilesView, PDFCollection,
                OrderVector);
            return result ? _summaryPage : null;
        }
    }
}
