using GistManager.GistService;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncCommand;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GistManager.ViewModels
{
    public class CreateGistFileViewModel : GistFileViewModel
    {
        #region constructors        
        public CreateGistFileViewModel(GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler) : base(parent, gistClientService, asyncOperationStatusManager, errorHandler)
        {
            //FileNameChangedCommand = new AsyncCommand<string>(CreateGistFileAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Creating gist file" };
        }
        #endregion

        #region command implementation
        private async Task CreateGistFileAsync(string newFileName) =>
            await GistClientService.CreateGistFileAsync(ParentGist.Gist.Id, newFileName, Content);

        protected async override Task OnFileNameChangedAsync(string originalName, string newName)=>
            await FileNameChangedCommand.ExecuteAsync(newName);

        #endregion

        
    }
}
