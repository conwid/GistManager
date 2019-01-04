using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.Async
{
    // https://github.com/StephenCleary/Mvvm.Async
    public sealed class NotifyTaskCompleted : INotifyTaskCompleted
    {
        private readonly IErrorHandler errorHandler;
        private readonly string errorInfo;
        public NotifyTaskCompleted(Task task, IErrorHandler errorHandler, string errorInfo)
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
            if (propertyChanged == null)
                return;
            RaisePropertyChanged(propertyChanged, nameof(Status));
            RaisePropertyChanged(propertyChanged, nameof(IsCompleted));
            RaisePropertyChanged(propertyChanged, nameof(IsNotCompleted));
            RaisePropertyChanged(propertyChanged, nameof(IsCanceled));
            RaisePropertyChanged(propertyChanged, nameof(IsFaulted));
            RaisePropertyChanged(propertyChanged, nameof(Exception));
            RaisePropertyChanged(propertyChanged, nameof(InnerException));
            RaisePropertyChanged(propertyChanged, nameof(ErrorMessage));
            RaisePropertyChanged(propertyChanged, nameof(IsSuccessfullyCompleted));
        }

        private void RaisePropertyChanged(PropertyChangedEventHandler originalEventReference, string propertyName)
            => originalEventReference(this, new PropertyChangedEventArgs(propertyName));

        public Task Task { get; }
        public Task TaskCompletion { get; }
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