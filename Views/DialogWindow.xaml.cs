using CrytonCore.Interfaces;
using System.Windows;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IDialog
    {
        public DialogWindow()
        {
            InitializeComponent();
            Owner = (Application.Current as App).MainWindow;
        }
    }
}
