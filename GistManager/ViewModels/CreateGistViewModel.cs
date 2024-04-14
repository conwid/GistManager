using GistManager.GistService;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncCommand;
using Octokit;
using System.Threading.Tasks;

namespace GistManager.ViewModels
{
    public class CreateGistViewModel : GistViewModel
    {
        private class CreateGistFileInnerViewModel : CreateGistFileViewModel
        {
            public CreateGistFileInnerViewModel(GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler) : base(parent, gistClientService, commandStatusManager, errorHandler)
            {
                FileNameChangedCommand = null;
            }
        }
        private readonly CreateGistFileInnerViewModel createGistFileViewModel;

        private AsyncCommand CreateGistCommand { get; }

        public CreateGistViewModel(bool isPublic, string content, IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager,
            IErrorHandler errorHandler) : base(gistClientService, asyncOperationStatusManager, errorHandler)
        {
            Public = isPublic;
            IsExpanded = true;
            Name = "New Gist";

            CreateGistCommand = new AsyncCommand(CreateGistAsync, asyncOperationStatusManager, errorHandler)
            { ExecutionInfo = "Creating gist" };

            createGistFileViewModel = new CreateGistFileInnerViewModel(this, gistClientService, asyncOperationStatusManager,
                errorHandler) { Content = content, FileName = "New Gist", IsInEditMode = false, IsSelected = true };

            createGistFileViewModel.PropertyChanged += PropertyChangedAsync;
            Files.Add(createGistFileViewModel);

            CreateGistCommand.ExecuteAsync();

        }
        private async void PropertyChangedAsync(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CreateGistFileViewModel.FileName))
            {
                if (!string.IsNullOrWhiteSpace(createGistFileViewModel.FileName))
                    
                    await CreateGistCommand.ExecuteAsync();
            }
        }
        private async Task CreateGistAsync() =>
            await CreateGistAndUpdateCollectionAsync();

        private async Task CreateGistAndUpdateCollectionAsync()
        {
            Gist createdGist = await GistClientService.CreateGistAsync(createGistFileViewModel.FileName, createGistFileViewModel.Content, Public);
        }




    }
}
