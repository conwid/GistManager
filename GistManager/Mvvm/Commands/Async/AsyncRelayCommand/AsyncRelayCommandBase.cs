using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GistManager.Mvvm.Commands.Async.AsyncRelayCommand
{
    // https://github.com/StephenCleary/Mvvm.Async
    // https://github.com/lbugnion/mvvmlight
    public abstract class AsyncRelayCommandBase : IAsyncRelayCommand
    {        
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(object parameter);

#pragma warning disable VSTHRD100 // Avoid async void methods; Logically this serves as an event-handler, so can be async void
        public async void Execute(object parameter) => await ExecuteAsync(parameter);
#pragma warning restore VSTHRD100 // Avoid async void methods


        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        protected void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}