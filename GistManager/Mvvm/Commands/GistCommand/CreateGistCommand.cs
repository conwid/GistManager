using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GistManager.ErrorHandling;
using GistManager.Mvvm.Commands.RelayCommand;

namespace GistManager.Mvvm.Commands.GistCommand
{
    public class CreateGistCommand : RelayCommand<CreateGistCommandArgs>
    {
        public CreateGistCommand(Action<CreateGistCommandArgs> execute, IErrorHandler errorHandler) : this(execute, null, errorHandler)
        {
        }

        public CreateGistCommand(Action<CreateGistCommandArgs> execute, Func<CreateGistCommandArgs, bool> canExecute, IErrorHandler errorHandler) : base(execute, canExecute, errorHandler)
        {

        }
    }
}
