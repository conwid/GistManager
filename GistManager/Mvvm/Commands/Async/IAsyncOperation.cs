using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async
{ 
    public interface IAsyncOperation
    {
        INotifyTaskCompleted Execution { get; }
        string ExecutionInfo { get; }
        bool SuppressCompletionCommand { get; set; }
    }   
}
