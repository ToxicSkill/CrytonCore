using CrytonCore.Infra;
using CrytonCore.Model;
using CrytonCore.Views;
using System;
using System.Threading.Tasks;

namespace CrytonCore.ViewModel
{
    public class PdfMergeViewModel : PortableDocumentFormatManager
    {
        private SummaryPdfMergePage _summaryPage;

        public PdfMergeViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
            _summaryPage = new SummaryPdfMergePage();
        }

        public async Task<bool> LoadFileViaDialog()
        {
            WindowDialogs.OpenDialog openDialog = new(new Helpers.DialogHelper()
            {
                Filters = Enums.EDialogFilters.EnumToString(Enums.EDialogFilters.DialogFilters.Pdf),
                Multiselect = true,
                Title = "Open files"
            });
            var dialogResult = openDialog.RunDialog();
            return dialogResult is not null ? await LoadFile(dialogResult) : await Task.Run(() => { return false; });
        }

        private void MoveIndexes(int selectedIndex, int newIndex)
        {
            FilesView.Move(selectedIndex, newIndex);
            OrderVector.Move(selectedIndex, newIndex);
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public SummaryPdfMergePage GetSummaryPage()
        {
            if (_summaryPage == null) _summaryPage = new SummaryPdfMergePage();
            var result = ((PdfMergeSummaryViewModel)_summaryPage.DataContext).Update(FilesView, PDFCollection,
                OrderVector);
            return result ? _summaryPage : null;
        }
    }
}
