using System.Collections.Generic;
using System.Collections.ObjectModel;
using CrytonCore.Infra;
using CrytonCore.Model;

namespace CrytonCore.ViewModel
{
    public class PdfMergeSummaryViewModel : PortableDocumentFormatManager
    {
        public PdfMergeSummaryViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: true);
            FirstRun = true;
        }

        public bool Update(
            ObservableCollection<FileListView> filesView,
            ObservableCollection<PDF> images,
            ObservableCollection<int> orderVector)
        {
            FilesView.Clear();
            PDFCollection.Clear();
            OrderVector = orderVector;

            for (var i = 0; i < OrderVector.Count; i++)
            {
                FilesView.Add(filesView[i]);
                PDFCollection.Add(images[orderVector[i]]);
            }

            SingleSliderDictionary = new Dictionary<int, int>();

            var pastPages = 0;
            for (var j = 0; j < OrderVector.Count; j++)
            {
                for (var i = pastPages; i < PDFCollection[j].TotalPages + pastPages; i++)
                {
                    SingleSliderDictionary.Add(i, j);
                }
                pastPages += PDFCollection[j].TotalPages;
            }
            SliderMaximum = SingleSliderDictionary.Count - 1;
            return SliderMaximum > 0;
            //await UpdateImageSourceAsync();
        }

        private bool FirstRun;
        public RelayCommand MouseEnterEvent => new(MouseEnterCommand, true);

        private void MouseEnterCommand()
        {
            if (!FirstRun) return;
            SliderValue = 0;
            SelectedItemIndex = 0;
            FirstRun = false;
        }
    }
}
