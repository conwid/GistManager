using System.Collections.Concurrent;
using System.Linq;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;

namespace GistManager.Mvvm.Commands.Async
{
    public class AsyncOperationStatusManager : BindableBase, IAsyncOperationStatusManager
    {
        private readonly ConcurrentQueue<IAsyncOperation> operations = new ConcurrentQueue<IAsyncOperation>();

        public IAsyncOperation CurrentOperation => operations.LastOrDefault();
        public AsyncRelayCommand.AsyncRelayCommand CompletionCommand { get; set; }
        public void AddOperation(IAsyncOperation operation)
        {
            operations.Enqueue(operation);
            RaisePropertyChanged(nameof(CurrentOperation));
        }

        public void ClearOperation(IAsyncOperation operation)
        {
            var done = operations.TryDequeue(out var lastOperation);
            RaisePropertyChanged(nameof(CurrentOperation));
            if (operations.Count == 0 && done && !lastOperation.SuppressCompletionCommand)
            {
                if (CompletionCommand?.CanExecute(null) ?? false)
                    CompletionCommand.Execute(null);
            }
        }
    }
}
