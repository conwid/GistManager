using System.Threading.Tasks;
using System.Windows.Input;

namespace GistManager.Mvvm.Commands.Async.AsyncRelayCommand
{
    public interface IAsyncRelayCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}