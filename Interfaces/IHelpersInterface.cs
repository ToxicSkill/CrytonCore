using System.Collections.ObjectModel;

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
