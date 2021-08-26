using CrytonCore.Infra;
using CrytonCore.Model;
using System.Threading.Tasks;
using System.Windows;
using CrytonCore.Views;
using CrytonCore.Helpers;

namespace CrytonCore.ViewModel
{
    public class PdfMergeViewModel : PortableDocumentFormatManager
    {
        private SummaryPdfMergeUserControl _summaryUserControl;
        public PdfMergeViewModel()
        {
            SetCurrentMode(pdfOnly: true,singleSlider: false);
            VisibilityChangeDelegate = new VisibilityDelegate(ChangeVisibility);
            _summaryUserControl = new SummaryPdfMergeUserControl();
        }

        private void ChangeVisibility(bool show)
        {
            VisibilityDefaultAsShowed = show ? Visibility.Hidden : Visibility.Visible;
            VisibilityDefaultAsHidden = show ? Visibility.Visible : Visibility.Hidden;
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

        public async Task<SummaryPdfMergeUserControl> GetSummaryPage()
        {
            if (_summaryUserControl == null) _summaryUserControl = new SummaryPdfMergeUserControl();
            var result = ((PdfMergeSummaryViewModel) _summaryUserControl.DataContext).Update(FilesView, PDFCollection,
                OrderVector);
            return result ? _summaryUserControl : null;
        }
    }
}
