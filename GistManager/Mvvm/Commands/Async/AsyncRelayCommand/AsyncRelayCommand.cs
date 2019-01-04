using GistManager.Mvvm.Commands.WeakDelegate;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GistManager.Mvvm.Commands.Async.AsyncRelayCommand
{
    // https://github.com/StephenCleary/Mvvm.Async
    // https://github.com/lbugnion/mvvmlight
    public class AsyncRelayCommand : AsyncRelayCommandBase, INotifyPropertyChanged, IAsyncOperation
    {
        private readonly WeakFunc<Task> command;
        private readonly WeakFunc<bool> canExecute;
        private INotifyTaskCompleted execution;
        private readonly IAsyncOperationStatusManager asyncOperationStatusManager;
        private readonly IErrorHandler errorHandler;

        public string ExecutionInfo { get; set; }
        public AsyncRelayCommand(Func<Task> command, IErrorHandler errorHandler) : this(command, null, null,errorHandler)
        {
        }

        public AsyncRelayCommand(Func<Task> command, Func<bool> canExecute, IErrorHandler errorHandler) : this(command, canExecute, null, errorHandler)
        {
        }

        public AsyncRelayCommand(Func<Task> command, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler) : this(command, null, asyncOperationStatusManager, errorHandler)
        {
        }


        public AsyncRelayCommand(Func<Task> command, Func<bool> canExecute, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler)
        {
            this.command = new WeakFunc<Task>(command) ?? throw new ArgumentNullException(nameof(command));
            this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            if (canExecute != null)
                this.canExecute = new WeakFunc<bool>(canExecute);
            if (commandStatusManager != null)
                this.asyncOperationStatusManager = commandStatusManager;           
        }

        public override bool CanExecute(object parameter)
        {
            return (canExecute == null || (canExecute.IsStatic || canExecute.IsAlive) && canExecute.Execute())
                    && (Execution == null || Execution.IsCompleted);
        }
        public override async Task ExecuteAsync(object parameter)
        {
            asyncOperationStatusManager?.AddOperation(this);
            Execution = new NotifyTaskCompleted(command.Execute(), errorHandler, ExecutionInfo + " failed");
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            RaiseCanExecuteChanged();
            asyncOperationStatusManager?.ClearOperation(this);
        }
        public INotifyTaskCompleted Execution
        {
            get => execution;
            private set
            {
                execution = value;
                OnPropertyChanged(nameof(Execution));
            }
        }

        public bool SuppressCompletionCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}