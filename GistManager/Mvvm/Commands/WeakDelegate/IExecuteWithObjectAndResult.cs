using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.WeakDelegate
{  
    // https://github.com/lbugnion/mvvmlight 
    public interface IExecuteWithObjectAndResult
    {      
        object ExecuteWithObject(object parameter);
    }
}
