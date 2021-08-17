using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrytonCore.Interfaces
{
    public interface IHelpersInterface
    {
        ObservableCollection<string> GetFirst();
        string GetSelectedItemFirst();
        int GetSelectedItemIndexFirst();

        void SetSelectedItemFirst(string item);
        void SetSelectedItemFirst(int itemIndex);

        int GetSelectedItemAsIntValueFirst();
    }
}
