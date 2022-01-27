using CrytonCore.Infra;
using CrytonCore.Interfaces;
using System.Windows;

namespace CrytonCore.Abstract
{
    public abstract class PageManager : NotificationClass, IVisibility
    {
        public void ChangeVisibility(bool visibile)
        {
            VisibilityDefaultAsShowed = visibile ? Visibility.Hidden : Visibility.Visible;
            VisibilityDefaultAsHidden = visibile ? Visibility.Visible : Visibility.Hidden;
        }
        
        private Visibility _visibilityShowed = Visibility.Visible;
        private Visibility _visibilityHidden = Visibility.Hidden;

        public Visibility VisibilityDefaultAsShowed
        {
            get => _visibilityShowed;
            set
            {
                _visibilityShowed = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsShowed));
            }
        }
        public Visibility VisibilityDefaultAsHidden
        {
            get => _visibilityHidden;
            set
            {
                _visibilityHidden = value;
                OnPropertyChanged(nameof(VisibilityDefaultAsHidden));
            }
        }
    }
}
