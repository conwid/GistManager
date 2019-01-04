using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GistManager.ErrorHandling;
using GistManager.Mvvm.Commands.RelayCommand;

namespace GistManager.Mvvm.Commands.GistCommand
{
    public class CreateGistFileCommand : RelayCommand<CreateGistFileCommandArgs>
    {
        public CreateGistFileCommand(Action<CreateGistFileCommandArgs> execute, IErrorHandler errorHandler) : base(execute, errorHandler)
        {
        }

        public CreateGistFileCommand(Action<CreateGistFileCommandArgs> execute, Func<CreateGistFileCommandArgs, bool> canExecute, IErrorHandler errorHandler) : base(execute, canExecute, errorHandler)
        {
        }
    }
}
