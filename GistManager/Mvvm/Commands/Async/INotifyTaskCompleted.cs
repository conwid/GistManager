using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async
{    
    public interface INotifyTaskCompleted : INotifyPropertyChanged
    {
        string ErrorMessage { get; }
        AggregateException Exception { get; }
        Exception InnerException { get; }
        bool IsCanceled { get; }
        bool IsCompleted { get; }
        bool IsFaulted { get; }
        bool IsNotCompleted { get; }
        bool IsSuccessfullyCompleted { get; }
        TaskStatus Status { get; }
        Task Task { get; }
        Task TaskCompletion { get; }
    }
}