using CrytonCore.Interfaces;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCore.ViewModel
{
    public class DialogViewModel : IDialogRequestClose
    {
        public DialogViewModel(string message, string title)
        {
            Message = message;
            Title = title;
            OkCommand =
                new ActionCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestEventArgs(true)));
        }

        public event EventHandler<DialogCloseRequestEventArgs> CloseRequested;
        public string Message { get; }
        public string Title { get; }

        public ICommand OkCommand { get; }
    }
}
