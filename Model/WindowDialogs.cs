using CrytonCore.Helpers;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCore.Model
{
    public class WindowDialogs
    {

        public class OpenDialog
        {
            private readonly DialogHelper _dialogArgs;

            public OpenDialog(DialogHelper dialogArgs)
            {
                _dialogArgs = dialogArgs;
            }

            public List<string> RunDialog()
            {
                if (_dialogArgs == null) return null;
                OpenFileDialog openFileDialog = new()
                {
                    Title = _dialogArgs.Title,
                    DefaultExt = _dialogArgs.DefaultExtension,
                    Filter = _dialogArgs.Filters,
                    Multiselect = _dialogArgs.Multiselect
                };
                if (openFileDialog.ShowDialog() == true)
                    return openFileDialog.FileNames.ToList();
                else
                    return null;
            }
        }

        public class SaveDialog
        {
            private readonly DialogHelper _dialogArgs;

            public SaveDialog(DialogHelper dialogArgs)
            {
                _dialogArgs = dialogArgs;
            }

            public List<string> RunDialog()
            {
                if (_dialogArgs == null) return null;
                SaveFileDialog saveFileDialog = new()
                {
                    Title = _dialogArgs.Title,
                    DefaultExt = _dialogArgs.DefaultExtension,
                    Filter = _dialogArgs.Filters
                };
                if (saveFileDialog.ShowDialog() == true)
                    return saveFileDialog.FileNames.ToList();
                else
                    return null;
            }
        }
    }
}
