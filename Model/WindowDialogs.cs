using CrytonCore.Helpers;
using CrytonCore.Interfaces;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;

namespace CrytonCore.Model
{
    public class WindowDialogs
    {

        public class OpenDialog : IWindowDialog
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

        public class SaveDialog : IWindowDialog
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


        public class FolderDialog : IWindowDialog
        {            
            private readonly DialogHelper _dialogArgs;

            public FolderDialog(DialogHelper dialogArgs)
            {
                _dialogArgs = dialogArgs;
            }

            public List<string> RunDialog()
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new List<string>() { dialog.FileName } : null;
            }
        }
    }
}
