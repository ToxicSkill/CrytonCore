using CrytonCore.Interfaces;
using System.Collections.ObjectModel;

namespace CrytonCore.Helpers
{
    public class RSAHelper : IHelpersInterface
    {
        public ObservableCollection<string> BitsRSAComboBoxSource { 
            get {
                return new ObservableCollection<string> {
                    "512",
                    "1024",
                    "2048",
                    "4096"
                };
            }
        }

        public RSAHelper()
        {
            SetSelectedItemFirst(0);
        }
        public int ActualIndex { get; set; }
        public string ActualItem { get; set; }
        public int ActualBits { get; set; }
        public string MethodName { get { return "RSA"; } }

        public ObservableCollection<string> GetFirst() => BitsRSAComboBoxSource;

        public string GetSelectedItemFirst() => ActualItem;

        public int GetSelectedItemIndexFirst() => ActualIndex;
        public int GetSelectedItemAsIntValueFirst() => ActualBits;

        public void SetSelectedItemFirst(string item)
        {
            ActualItem = item;
            ActualIndex = BitsRSAComboBoxSource.IndexOf(ActualItem);
            SetBits();
        }
        public void SetSelectedItemFirst(int itemIndex)
        {
            ActualIndex = itemIndex;
            ActualItem = BitsRSAComboBoxSource[ActualIndex];
            SetBits();
        }
        private void SetBits()
        {
            if (int.TryParse(ActualItem, out int bits))
            {
                ActualBits = bits;
            }
        }
    }
}
