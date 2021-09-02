using CrytonCore.Infra;
using CrytonCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CrytonCore.ViewModel
{
    public class PdfToImageViewModel : PortableDocumentFormatManager
    {
        public delegate void LoadFileDelegate(object sender, EventArgs e);// (object sender, EventArgs e);
        public LoadFileDelegate DoLoadFile;

        public PdfToImageViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
            SetPdfHighQuality(highQuality: true);
        }

        public RelayCommand LoadAnother => new(LoadAnotherCommand, true);
        
        private void LoadAnotherCommand()
        {
            Clear.Execute(null);
            DoLoadFile.Invoke(null, null);
            SelectedItemIndex = -1;
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

    }
}
