using GistManager.ErrorHandling;
using GistManager.Mvvm.Commands.WeakDelegate;
using System;
using System.Windows.Input;

namespace GistManager.Mvvm.Commands.RelayCommand
{
    // https://github.com/lbugnion/mvvmlight
    public class RelayCommand : ICommand
    {
        private readonly WeakAction execute;

        private readonly WeakFunc<bool> canExecute;

        private readonly IErrorHandler errorHandler;

        public RelayCommand(Action execute, IErrorHandler errorHandler) : this(execute, null, errorHandler)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute, IErrorHandler errorHandler)
        {
            this.execute = new WeakAction(execute ?? throw new ArgumentNullException(nameof(execute)));
            this.errorHandler = errorHandler;
            if (canExecute != null)
            {
                this.canExecute = new WeakFunc<bool>(canExecute);
            }
        }


        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
        public bool CanExecute(object parameter) => canExecute == null || (canExecute.IsStatic || canExecute.IsAlive) && canExecute.Execute();

        public virtual void Execute(object parameter)
        {
            try
            {
                if (CanExecute(parameter) && this.execute != null && (this.execute.IsStatic || this.execute.IsAlive))
                {
                    this.execute.Execute();
                }
            }
            catch (Exception ex)
            {
                errorHandler.SetError("Could not complete operation", ex);
            }
        }
    }
}
