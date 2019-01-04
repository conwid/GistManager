using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async
{
    public interface IAsyncOperationStatusManager
    {
        IAsyncOperation CurrentOperation { get; }
        AsyncRelayCommand.AsyncRelayCommand CompletionCommand { get; set; }
        void ClearOperation(IAsyncOperation command);
        void AddOperation(IAsyncOperation command);
    }
}
