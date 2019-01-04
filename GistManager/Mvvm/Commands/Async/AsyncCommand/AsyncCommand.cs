using GistManager.Mvvm.Commands.WeakDelegate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async.AsyncCommand
{
    // https://github.com/StephenCleary/Mvvm.Async
    // https://github.com/lbugnion/mvvmlight
    public class AsyncCommand : IAsyncOperation
    {
        private readonly WeakFunc<Task> command;
        private INotifyTaskCompleted execution;
        private IAsyncOperationStatusManager asyncOperationStatusManager;
        private readonly IErrorHandler errorHandler;
        public string ExecutionInfo { get; set; }

        public AsyncCommand(Func<Task> command, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
            this.asyncOperationStatusManager = asyncOperationStatusManager ?? throw new ArgumentNullException(nameof(asyncOperationStatusManager));
            this.command = new WeakFunc<Task>(command ?? throw new ArgumentNullException(nameof(command)));
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

        public async Task ExecuteAsync()
        {
            asyncOperationStatusManager.AddOperation(this);
            Execution = new NotifyTaskCompleted(command.Execute(), errorHandler, ExecutionInfo + " failed");
            await Execution.TaskCompletion;
            asyncOperationStatusManager.ClearOperation(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
