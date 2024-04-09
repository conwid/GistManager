using GistManager.GistService;
using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;
using GistManager.Mvvm.Commands.GistCommand;
using GistManager.Mvvm.Commands.RelayCommand;
using GistManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace GistManager.ViewModels
{
    public class GistManagerWindowViewModel : BindableBase
    {
        private readonly FuncEqualityComparer<GistModel, string> gistEqualityComparer = new FuncEqualityComparer<GistModel, string>(g => g.Id);
        private readonly FuncEqualityComparer<GistFileModel, string> gistFileEqualityComparer = new FuncEqualityComparer<GistFileModel, string>(g => g.Id);

        internal IGistClientService gistClientService;
        public IAsyncOperationStatusManager AsyncOperationStatusManager { get; }
        public IErrorHandler ErrorHandler { get; }

        public CreateGistCommandArgs CreateGistCommandArgs { get; }


        public GistManagerWindowViewModel(IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler)
        {
            this.gistClientService = gistClientService ?? throw new ArgumentNullException(nameof(gistClientService));
            this.AsyncOperationStatusManager = asyncOperationStatusManager ?? throw new ArgumentNullException(nameof(asyncOperationStatusManager));
            this.ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));

            Gists = new ObservableRangeCollection<GistViewModel>();
            PublicFilterCommand = new RelayCommand<FilterEventArgs>(FilterPublicGists, errorHandler);
            PrivateFilterCommand = new RelayCommand<FilterEventArgs>(FilterPrivateGists, errorHandler);
            LoginCommand = new AsyncRelayCommand(LoginAsync, () => !IsAuthenticated, asyncOperationStatusManager, ErrorHandler) { ExecutionInfo = "Logging in", SuppressCompletionCommand = true };
            CreatePublicGistCommand = new CreateGistCommand(e => InitCreate(e.Content, true), errorHandler)  ;
            CreatePrivateGistCommand = new CreateGistCommand(e => InitCreate(e.Content, false), errorHandler);
            CreateGistFileCommand = new CreateGistFileCommand(e => InitCreate(e.Content, e.ParentGist), errorHandler);
            RemoveGistCommand = new RelayCommand<GistViewModel>(g => Gists.Remove(g), errorHandler);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => IsAuthenticated, asyncOperationStatusManager, ErrorHandler) { ExecutionInfo = "Refreshing gists", SuppressCompletionCommand = true };
            LogoutCommand = new AsyncRelayCommand(LogoutAsync, () => IsAuthenticated, asyncOperationStatusManager, ErrorHandler) { ExecutionInfo = "Logging out", SuppressCompletionCommand = true };
           
            AsyncOperationStatusManager.CompletionCommand = new AsyncRelayCommand(this.RefreshAsync, asyncOperationStatusManager, ErrorHandler) { ExecutionInfo = "Refreshing gists", SuppressCompletionCommand = true };

            CreateGistCommandArgs = new CreateGistCommandArgs($"New File created successfully at: {DateTime.Now.ToShortDateString()} - {DateTime.Now.ToShortTimeString()}");
        }


        #region commands
        public ICommand PublicFilterCommand { get; }
        public ICommand PrivateFilterCommand { get; }
        public ICommand CreatePublicGistCommand { get; }
        public ICommand CreatePrivateGistCommand { get; }
        public ICommand CreateGistFileCommand { get; }
        public RelayCommand<GistViewModel> RemoveGistCommand { get; }
        public ICommand LoginCommand { get; }        
        public ICommand RefreshCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region command implementations
        private async Task LoginAsync()
        {            
            if (await gistClientService.AuthenticateAsync())
            {
                IsAuthenticated = true;
                await RefreshAsync();
            }
        }

        private async Task RefreshAsync()
        {
            var gists = await gistClientService.GetGistsAsync();

            Gists.RemoveRange(Gists.Where(g => g.GetType() == typeof(CreateGistViewModel)).ToList());

            var newGists = gists.Except(Gists.Select(g => g.Gist), gistEqualityComparer).ToList();
            var deletedGists = Gists.Where(g => !gists.Contains(g.Gist, gistEqualityComparer)).ToList();
            var existingGists = Gists.Where(g => gists.Contains(g.Gist, gistEqualityComparer)).ToList();

            Gists.RemoveRange(deletedGists);
            Gists.AddRange(newGists.Select(ng => new GistViewModel(ng, gistClientService, AsyncOperationStatusManager, ErrorHandler)));

            foreach (var existingGist in existingGists)
            {
                HandleExistingGist(existingGist, gists.Single(g => g.Id == existingGist.Gist.Id));
            }

        }

        private async Task LogoutAsync()
        {
            this.ErrorHandler.ClearError();
            await gistClientService.LogoutAsync();
            Gists.Clear();
            this.IsAuthenticated = false;
            this.SearchExpression = string.Empty;
        }
        private void FilterPublicGists(FilterEventArgs obj)
        {
            var gistVm = (GistViewModel)obj.Item;
            obj.Accepted = gistVm.Public && (string.IsNullOrWhiteSpace(SearchExpression) || gistVm.Name.ToUpper().Contains(SearchExpression.ToUpper()));
        }
        private void FilterPrivateGists(FilterEventArgs obj)
        {
            var gistVm = (GistViewModel)obj.Item;
            obj.Accepted = !gistVm.Public && (string.IsNullOrWhiteSpace(SearchExpression) || gistVm.Name.ToUpper().Contains(SearchExpression.ToUpper()));
        }
        private void InitCreate(string content, bool isPublic) =>
           Gists.Add(new CreateGistViewModel(isPublic, content, gistClientService, AsyncOperationStatusManager, ErrorHandler));
        private void InitCreate(string content, GistViewModel gistVm) =>
            gistVm.Files.Add(new CreateGistFileViewModel(gistVm, gistClientService, AsyncOperationStatusManager, ErrorHandler) { IsInEditMode = true, Content = content, IsSelected = true });
        #endregion

        #region bound properties   

        // My props
        public bool IsInDarkMode { get; set; } = false;

        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set => SetProperty(ref isAuthenticated, value);
        }



        private string searchExpression;
        public string SearchExpression
        {
            get => searchExpression;
            set => SetProperty(ref searchExpression, value);
        }

        public ObservableRangeCollection<GistViewModel> Gists { get; private set; }
        #endregion               
        private void HandleExistingGist(GistViewModel existingGist, GistModel gistModel)
        {
            if (existingGist.Name != gistModel.Name)
            {
                existingGist.Name = gistModel.Name;
            }

            var inCreate = existingGist.Files.Where(f => f.GetType() == typeof(CreateGistFileViewModel)).ToList();
            existingGist.Files.RemoveRange(inCreate);

            var deletedFiles = existingGist.Files.Where(f => !gistModel.Files.Contains(f.GistFile, gistFileEqualityComparer)).ToList();
            existingGist.Files.RemoveRange(deletedFiles);

            var newHistoryEntries = gistModel.History.Where(g => !existingGist.History.Any(h => h.Version == g.Version)).ToList();
            if (newHistoryEntries.Any())
            {
                foreach (var newHistoryEntry in newHistoryEntries)
                {
                    var newHistoryEntryViewModel = new GistHistoryEntryViewModel(newHistoryEntry, existingGist);
                    existingGist.History.Insert(0, newHistoryEntryViewModel);
                    foreach (var file in existingGist.Files)
                    {
                        file.History.Insert(0, new GistHistoryEntryViewModel(newHistoryEntry, file));
                    }
                }
                existingGist.History.First().IsCheckedOut = true;
            }

            var newFiles = gistModel.Files.Except(existingGist.Files.Select(f => f.GistFile), gistFileEqualityComparer).ToList();
            var newFileViewModels = new List<GistFileViewModel>();
            foreach (var newFile in newFiles)
            {
                var newFileViewModel = new GistFileViewModel(newFile, existingGist, gistClientService, AsyncOperationStatusManager, ErrorHandler);
                newFileViewModel.History.AddRange(existingGist.History.Select(gh => new GistHistoryEntryViewModel(gh.HistoryEntry, newFileViewModel)));
                newFileViewModel.History.First().IsCheckedOut = true;
                existingGist.Files.Add(newFileViewModel);
            }
            existingGist.Files.AddRange(newFileViewModels);
        }

    }
}
