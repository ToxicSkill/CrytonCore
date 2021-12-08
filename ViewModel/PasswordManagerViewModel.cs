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
                new Account() { Name = "Bob", Data = DateTime.Now },
                new Account() { Name = "Alice", Data = DateTime.Now },
                new Account() { Name = "Eva", Data = DateTime.Now }
            };
        }

    }
}
