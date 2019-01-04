using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm
{
    public interface IErrorHandler
    {
        string ErrorInfo { get; }
        Exception Error { get; }
        bool HasError { get; }
        void ClearError();
        void SetError(string error);
        void SetError(Exception ex);
        void SetError(string error, Exception ex);
    }
}
