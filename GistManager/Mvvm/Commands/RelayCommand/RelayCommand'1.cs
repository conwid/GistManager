using GistManager.ErrorHandling;
using GistManager.Mvvm.Commands.WeakDelegate;
using System;
using System.Windows.Input;

namespace GistManager.Mvvm.Commands.RelayCommand
{
    // https://github.com/lbugnion/mvvmlight
    public class RelayCommand<T> : ICommand
    {
        private readonly WeakAction<T> execute;
        private readonly WeakFunc<T, bool> canExecute;
        private readonly IErrorHandler errorHandler;

        public RelayCommand(Action<T> execute, IErrorHandler errorHandler) : this(execute, null, errorHandler)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, IErrorHandler errorHandler)
        {
            this.execute = new WeakAction<T>(execute ?? throw new ArgumentNullException(nameof(execute)));
            this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            if (canExecute != null)
            {
                this.canExecute = new WeakFunc<T, bool>(canExecute);
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

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            if (canExecute.IsStatic || canExecute.IsAlive)
            {
                if (parameter == null && typeof(T).IsValueType)
                {
                    return canExecute.Execute(default);
                }

                return canExecute.Execute((T)parameter);
            }
            return false;
        }

        public virtual void Execute(object parameter)
        {
            try
            {
                var val = parameter;
                if (parameter != null && parameter.GetType() != typeof(T))
                {
                    if (parameter is IConvertible)
                    {
                        val = Convert.ChangeType(parameter, typeof(T), null);
                    }
                }

                if (CanExecute(val) && execute != null && (execute.IsStatic || execute.IsAlive))
                {
                    if (val == null)
                    {
                        if (typeof(T).IsValueType)
                        {
                            execute.Execute(default);
                        }
                        else
                        {
                            execute.Execute((T)val);
                        }
                    }
                    else
                    {
                        execute.Execute((T)val);
                    }
                }
            }
            catch (Exception ex)
            {
                errorHandler.SetError("Could not complete operation", ex);
            }
        }
    }
}
