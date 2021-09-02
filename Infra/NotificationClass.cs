using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrytonCore.Infra
{
    public class NotificationClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
