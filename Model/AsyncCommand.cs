using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCore.Model
{
    public abstract class AsyncCommand<T> : IAsyncCommand<T>
    {
        private readonly ObservableCollection<Task<bool>> runningTasks;
        protected AsyncCommand()
        {
            runningTasks = new ObservableCollection<Task<bool>>();
            runningTasks.CollectionChanged += OnRuningTasksChanged;
        }

        private void OnRuningTasksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public IEnumerable<Task<bool>> RunngingTasks => runningTasks;

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }
        async void ICommand.Execute(object parameter)
        {
            Task<bool> runningTask = ExecuteAsync((T)parameter);
            runningTasks.Add(runningTask);

            try
            {
                await runningTask;
            }
            finally
            {
                runningTasks.Remove(runningTask);
            }
        }
        public abstract bool CanExecute();
        public abstract Task<bool> ExecuteAsync(T parameter);
    }
}
