using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CrytonCore.Infra;
using CrytonCore.Model;
using Image = CrytonCore.Tools.Image;

namespace CrytonCore.ViewModel
{
    public class PdfMergeSummaryViewModel : PortableDocumentFormatManager
    {
        public PdfMergeSummaryViewModel()
        {
            SetCurrentMode(true, true);
            FirstRun = true;
        }

        public bool Update(
            ObservableCollection<FileListView> filesView,
            ObservableCollection<Image> images,
            ObservableCollection<int> orderVector)
        {
            FilesView.Clear();
            Images.Clear();
            OrderVector = orderVector;

            for (var i = 0; i < OrderVector.Count; i++)
            {
                FilesView.Add(filesView[i]);
                Images.Add(images[orderVector[i]]);
            }

            SingleSliderDictionary = new Dictionary<int, int>();

            var pastPages = 0;
            for (var j = 0; j < OrderVector.Count; j++)
            {
                for (var i = pastPages; i < Images[j].MaxNumberOfPages + pastPages; i++)
                {
                    SingleSliderDictionary.Add(i, j);
                }
                pastPages += Images[j].MaxNumberOfPages;
            }
            SliderMaximum = SingleSliderDictionary.Count - 1;
            return SliderMaximum > 0;
            //await UpdateImageSourceAsync();
        }

        private bool FirstRun;
        public RelayCommand MouseEnterEvent => new RelayCommand(MouseEnterCommand, true);

        private void MouseEnterCommand()
        {
            if (FirstRun)
            {
                SliderValue = 0;
                SelectedItemIndex = 0;
                FirstRun = false;
            }
        }
    }
}
