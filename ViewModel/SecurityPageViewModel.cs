using CrytonCore.Abstract;
using CrytonCore.Infra;
using CrytonCore.Interfaces;
using CrytonCore.Model;
using CrytonCore.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Visibility;

namespace CrytonCore.ViewModel
{
    internal class SecurityPageViewModel : PDFPageManager, ISecurity
    {
        public SecureString OwnerSecurePassword { private get; set; }
        public SecureString UserSecurePassword { private get; set; }

        public Visibility RandomPasswordsVisibility { get; set; } = Hidden;
        public Visibility PasswordsVisibility { get; set; } = Visible;
        public Visibility UserVisibility { get; set; } = Hidden;

        public string RandomOwnerPassword { get; set; }
        public string RandomUserPassword { get; set; }

        private bool _useRandomPassword;
        public bool UseRandomPassowrd 
        { 
            private get
            {
                return _useRandomPassword;
            }
            set
            {
                if (value == _useRandomPassword)
                    return;
                _useRandomPassword = value;
                if (_useRandomPassword)
                    SetRandomPassword();
                else
                    ChangePasswordsVisibility(true);
            } 
        }

        public bool UseHighestEncryption { private get; set; } = true;

        private bool _setUserAndOwnerPassord;
        public bool SetUserAndOwnerPassord 
        { 
            private get => _setUserAndOwnerPassord; 
            set
            {
                if (value == _setUserAndOwnerPassord)
                    return;
                _setUserAndOwnerPassord = value;
                SetUserVisibility(_setUserAndOwnerPassord);
            }
        }

        public SecurityPageViewModel()
        {
            SetCurrentMode(pdfOnly: true, singleSlider: false);
        }


        private void SetUserVisibility(bool setUserAndOwnerPassord)
        {
            UserVisibility = setUserAndOwnerPassord ? Visible : Hidden;

            OnPropertyChanged(nameof(UserVisibility));
        }

        private void ChangePasswordsVisibility(bool defaultVisibility)
        {
            RandomPasswordsVisibility = defaultVisibility ? Hidden : Visible;
            PasswordsVisibility = defaultVisibility ? Visible : Hidden;

            OnPropertyChanged(nameof(PasswordsVisibility));
            OnPropertyChanged(nameof(RandomPasswordsVisibility));
        }

        public void SetOwnerPassword(SecureString secString)
        {
            OwnerSecurePassword = secString;
        }

        public void SetUserPassword(SecureString secString)
        {
            UserSecurePassword = secString;
        }

        public void SetRandomPassword(int length = 12)
        {
            for (int i = 0; i < 2; i++)
            {
                var password = Password.Generate(length, length/2);
                if (i % 2 == 0)
                    RandomOwnerPassword = password;
                else
                    RandomUserPassword = password;
            }

            RandomPasswordsVisibility = Visible;
            PasswordsVisibility = Hidden;

            OnPropertyChanged(nameof(RandomOwnerPassword));
            OnPropertyChanged(nameof(RandomUserPassword));
            OnPropertyChanged(nameof(PasswordsVisibility));
            OnPropertyChanged(nameof(RandomPasswordsVisibility));
        }

        protected async Task<bool> LoadFileViaDragDrop(IEnumerable<FileInfo> fileNames)
        {
            List<FileInfo> filesInfo = new();
            foreach (var file in fileNames)
            {
                if (file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpeg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.jpg) ||
                    file.Extension == "." + Enums.EExtensions.EnumToString(Enums.EExtensions.Extensions.png))
                    filesInfo.Add(file);
            }
            return await LoadPdfFile(filesInfo);
        }

        public RelayAsyncCommand<object> LoadFileViaDialog => new(LoadFileViaDialogCommand);

        private async Task<bool> LoadFileViaDialogCommand(object o)
        {
            WindowDialogs.OpenDialog openDialog = new(new Helpers.DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.Pdf),
                Multiselect = true,
                Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["openFiles"]
            });
            var dialogResult = openDialog.RunDialog();
            return dialogResult != null && await LoadPdfFile(dialogResult.Select(f => new FileInfo(f)).ToList());
        }
    }
}
