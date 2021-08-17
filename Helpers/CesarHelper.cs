using CrytonCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrytonCore.Helpers
{
    public class CesarHelper : IHelpersInterface
    {
        public ObservableCollection<string> RenameCesarComboBoxSource
        {
            get
            {
                return new ObservableCollection<string> {
                    "3",
                    "4",
                    "5",
                    "6"
                };
            }
        }

        public CesarHelper()
        {
            SetSelectedItemFirst(0);
        }
        public int ActualIndex { get; set; }
        public string ActualItem { get; set; }
        public int ActualShift { get; set; }
        public string MethodName { get { return "CESAR"; } }

        public ObservableCollection<string> GetFirst() => RenameCesarComboBoxSource;

        public string GetSelectedItemFirst() => ActualItem;

        public int GetSelectedItemIndexFirst() => ActualIndex;
        public int GetSelectedItemAsIntValueFirst() => ActualShift;

        public void SetSelectedItemFirst(string item)
        {
            ActualItem = item;
            ActualIndex = RenameCesarComboBoxSource.IndexOf(ActualItem);
            SetShift();
        }
        public void SetSelectedItemFirst(int itemIndex)
        {
            ActualIndex = itemIndex;
            ActualItem = RenameCesarComboBoxSource[ActualIndex];
            SetShift();
        }
        private void SetShift()
        {
            if (int.TryParse(ActualItem, out int shift))
            {
                ActualShift = shift;
            }
        }
    }
}
