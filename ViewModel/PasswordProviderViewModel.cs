using CrytonCore.Infra;
using CrytonCore.Model;
using CrytonCore.PdfService;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCore.ViewModel
{
    public class PasswordProviderViewModel : NotificationClass
    {
        public ObservableCollection<PdfPasswordBase> Passwords { get; set; }

        public PasswordProviderViewModel(List<PdfPasswordBase> passwords)
        {
            Passwords = new ObservableCollection<PdfPasswordBase>();
            foreach (var password in passwords)
                Passwords.Add(password);
        }

        public PasswordProviderViewModel() { }

    }
}
