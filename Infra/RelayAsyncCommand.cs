using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCore.Infra
{
    public class RelayAsyncCommand<T> : ICommand where T : class
    {
        /// <inheritdoc />
        public RelayAsyncCommand(Func<T, Task> execute)
        {
            this.execute = execute;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return !isExecuting;
        }

        /// <inheritdoc />
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter as T);
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        private readonly Func<T, Task> execute;
        private bool isExecuting;

        public async Task ExecuteAsync(T parameter)
        {
            try
            {
                isExecuting = true;
                InvokeCanExecuteChanged();
                await execute(parameter);
            }
            finally
            {
                isExecuting = false;
                InvokeCanExecuteChanged();
            }
        }

        private void InvokeCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
