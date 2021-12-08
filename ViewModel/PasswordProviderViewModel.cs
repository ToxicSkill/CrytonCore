using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCore.ViewModel
{
    public class PasswordProviderViewModel : NotificationClass
    {
        public ObservableCollection<PdfPassword> Passwords { get; set; }

        public PasswordProviderViewModel(List<PdfPassword> passwords)
        {
            Passwords = new ObservableCollection<PdfPassword>();
            foreach (PdfPassword password in passwords)
                Passwords.Add(password);
        }

        public PasswordProviderViewModel() { }

    }
}
