using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.ViewModel;
using System;
using System.Windows;

namespace CrytonCore.Views
{
    /// <summary>
    /// Interaction logic for PopUpPage.xaml
    /// </summary>
    public partial class PopupWindow : Window, IDialog
    {
        public PopupWindow(Logger logger)
        {
            InitializeComponent();
            SetLocation();
            var vm = new PopupWindowViewModel(logger);
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }

        private void SetLocation()
        {
            Owner = (Application.Current as App).MainWindow;
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = Owner.Left + Owner.Width / 2 - this.Width / 2;
            this.Top = Owner.Top + Owner.Height /2 - this.Height / 2;
            //WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
