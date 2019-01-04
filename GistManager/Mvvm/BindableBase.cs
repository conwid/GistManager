using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm
{
    // https://github.com/lbugnion/mvvmlight
    public abstract class BindableBase : INotifyPropertyChanged
    {
        private const string exceptionMessage = "Property name cannot be empty";
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = default)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(exceptionMessage, nameof(propertyName));

            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(exceptionMessage, nameof(propertyName));

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    }
}
