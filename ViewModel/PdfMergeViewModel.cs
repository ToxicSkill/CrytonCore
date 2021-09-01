using CrytonCore.Infra;
using CrytonCore.Model;
using System.Threading.Tasks;
using CrytonCore.Views;

namespace CrytonCore.ViewModel
{
    public class PdfMergeViewModel : PortableDocumentFormatManager
    {
        private SummaryPdfMergePage _summaryPage;

        public PdfMergeViewModel()
        {
            SetCurrentMode(pdfOnly: true,singleSlider: false);
            _summaryPage = new SummaryPdfMergePage();
        }

        private void MoveIndexes(int selectedIndex, int newIndex)
        {
            FilesView.Move(selectedIndex, newIndex);
            OrderVector.Move(selectedIndex, newIndex);
        }

        private void RemoveIndexes(int selectedIndex)
        {
            FilesView.RemoveAt(selectedIndex);
            PDFCollection.RemoveAt(OrderVector[selectedIndex]);
            var orderValue = OrderVector[selectedIndex];
            OrderVector.RemoveAt(selectedIndex);
            for (var i = 0; i < OrderVector.Count; i++)
            {
                if (OrderVector[i] > orderValue)
                    OrderVector[i]--;
            }
        }
        private void ClearIndexes()
        {
            FilesView.Clear();
            OrderVector.Clear();
            PDFCollection.Clear();
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
        public RelayCommand Clear => new(ClearCommand, true);

        private void ClearCommand()
        {
            ClearIndexes();
            UpdateListView();
            SelectedItemIndex = 0;
            ChangeVisibility(false);
        }

        public async Task<SummaryPdfMergePage> GetSummaryPage()
        {
            if (_summaryPage == null) _summaryPage = new SummaryPdfMergePage();
            var result = ((PdfMergeSummaryViewModel)_summaryPage.DataContext).Update(FilesView, PDFCollection,
                OrderVector);
            return result ? _summaryPage : null;
        }
    }
}
