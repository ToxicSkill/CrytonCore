using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCore
{
    public interface IAsyncCommand<in T> : ICommand
    {
        IEnumerable<Task<bool>> RunngingTasks { get; }
        bool CanExecute();
        Task<bool> ExecuteAsync(T parameter);
    }
}
