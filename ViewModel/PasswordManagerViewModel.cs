using CrytonCore.Model;
using System;
using System.Collections.ObjectModel;

namespace CrytonCore.ViewModel
{
    public class PasswordManagerViewModel
    {
        public ObservableCollection<Account> AccountView { get; set; }

        public PasswordManagerViewModel()
        {
            AccountView = new ObservableCollection<Account>()
            {
                new Account() { Name = "Bob", Date = DateTime.Now },
                new Account() { Name = "Alice", Date = DateTime.Now },
                new Account() { Name = "Eva", Date = DateTime.Now }
            };
        }

    }
}
