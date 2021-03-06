using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCore.Infra
{
    public class RelayCommand : ICommand
    {
        private Action localAction;
        private bool _localCanExecute;
        public RelayCommand(Action action, bool canExecute)
        {
            localAction = action;
            _localCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _localCanExecute;
        }

        public void Execute(object parameter)
        {
            localAction();
        }
    }
    public class RelayCommandAsync : ICommand
    {
        private Task localAction;
        private bool _localCanExecute;
        public RelayCommandAsync(Task action, bool canExecute)
        {
            localAction = action;
            _localCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _localCanExecute;
        }

        public void Execute(object parameter)
        {
            _ = Task.Run(() => localAction);
        }
    }
}
