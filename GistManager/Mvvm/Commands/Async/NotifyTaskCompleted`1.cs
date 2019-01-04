using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async
{    
    public sealed class NotifyTaskCompleted<TResult> : INotifyTaskCompleted
    {
        private readonly IErrorHandler errorHandler;
        private readonly string errorInfo;
        public NotifyTaskCompleted(Task<TResult> task, IErrorHandler errorHandler, string errorInfo)
        {
            this.errorHandler = errorHandler;
            this.errorInfo = errorInfo;
            Task = task;
            TaskCompletion = WatchTaskAsync(task);
        }
        private async Task WatchTaskAsync(Task task)
        {
            RaisePropertiesChanged();
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                errorHandler.SetError(errorInfo, ex);
            }
            RaisePropertiesChanged();
        }

        private void RaisePropertiesChanged()
        {
            var propertyChanged = PropertyChanged;
            if (null == propertyChanged)
                return;
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));


            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));

            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));

            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));

        }

        public Task<TResult> Task { get; }
        Task INotifyTaskCompleted.Task => Task;
        public Task TaskCompletion { get; }
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
#pragma warning disable VSTHRD104 // Offer async methods
        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
#pragma warning restore VSTHRD104 // Offer async methods
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => InnerException?.InnerException;
        public string ErrorMessage => InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}