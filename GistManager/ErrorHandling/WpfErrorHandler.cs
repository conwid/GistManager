using GistManager.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.ErrorHandling
{
    public class WpfErrorHandler : BindableBase, IErrorHandler
    {       

        private bool hasError;
        public bool HasError
        {
            get => hasError;
            private set => SetProperty(ref hasError, value); 
        }

        private string errorInfo;
        public string ErrorInfo
        {
            get => errorInfo;
            private set => SetProperty(ref errorInfo, value); 
        }

        private Exception error;
        public Exception Error
        {
            get  => error;
            private set => SetProperty(ref error, value); 
        }

        public void ClearError()
        {
            ErrorInfo = null;
            Error = null;
            HasError = false;
        }

        public void SetError(string error) => SetError(error, null);
        public void SetError(Exception ex) => SetError(null, ex);
        public void SetError(string error, Exception ex)
        {
            Error = ex;
            ErrorInfo = error;
            HasError = true;
        }
    }
}
