using GistManager.GistService;
using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncCommand;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;
using GistManager.Mvvm.Commands.RelayCommand;
using GistManager.Utils;
using Microsoft.VisualStudio;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GistManager.ViewModels
{
    public class GistFileViewModel : BindableBase, IViewModelWithHistory
    {
        private readonly IAsyncOperationStatusManager asyncOperationStatusManager;
        protected IGistClientService GistClientService { get; }
        public GistFileModel GistFile { get; }
        public GistViewModel ParentGist { get; }

        public bool Refreshing { get; set; }

        /// <summary>
        /// Stores whether the code, filename or code has been changed from load
        /// </summary>
        public bool HasChanges { get; set; }

        #region constructors
        public GistFileViewModel(IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler)
        {
            GistClientService = gistClientService ?? throw new ArgumentNullException(nameof(gistClientService));
            this.asyncOperationStatusManager = asyncOperationStatusManager ?? throw new ArgumentNullException(nameof(asyncOperationStatusManager));
            FileNameChangedCommand = new AsyncCommand<string>(RenameGistFileAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Renaming gist file" };
            UpdateGistCommand = new AsyncCommand<string>(UpdateGistFilenameAndContentAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Updating gist file", SuppressCompletionCommand = true };
            CheckoutCommand = new AsyncCommand<GistHistoryEntryModel>(RefreshGistFileAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Checking out file", SuppressCompletionCommand = true };
        }

        public GistFileViewModel(GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler) : this(gistClientService, commandStatusManager, errorHandler)
        {
            ParentGist = parent;
        }
        public GistFileViewModel(GistFileModel file, GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler) : this(parent, gistClientService, commandStatusManager, errorHandler)
        {
            this.GistFile = file;
            fileName = file.Name;
            History = new ObservableRangeCollection<GistHistoryEntryViewModel>();
            Url = file.Url;
            Content = file.Content;
            DeleteGistFileCommand = new AsyncRelayCommand(DeleteGistFileAsync, commandStatusManager, errorHandler) { ExecutionInfo = "Deleting file from gist", SuppressCompletionCommand = true };
            CopyGistFileUrlCommand = new RelayCommand(CopyGistFileUrl, errorHandler);
        }
        #endregion           

        #region bound properties
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                var originalFileName = fileName;
                SetProperty(ref fileName, value);
                // Below removed as don't want autosave on filename change
                return;
                if (!Refreshing && !string.IsNullOrWhiteSpace(originalFileName))
                {
                    OnFileNameChangedAsync(originalFileName, fileName);
                }
            }
        }

        private string content;
        public string Content
        {
            get => content;
            set
            {
                SetProperty(ref content, value);
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
        public string Url { get; }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }


        private bool isInEditMode;
        public bool IsInEditMode
        {
            get => isInEditMode;
            set => SetProperty(ref isInEditMode, value);
        }
        public ObservableRangeCollection<GistHistoryEntryViewModel> History { get; set; }
        #endregion

        public string Id
        {
            get
            {
                var data = Url.Split('/');
                return data[data.Length - 1];
            }
        }

        #region commands
        public ICommand DeleteGistFileCommand { get; }
        public ICommand CopyGistFileUrlCommand { get; }
        public ICommand CreateNewGist { get; }
        private AsyncCommand<GistHistoryEntryModel> CheckoutCommand { get; }

        // HACK: Command should be readonly
        protected AsyncCommand<string> FileNameChangedCommand { get; set; }

        internal AsyncCommand<string> UpdateGistCommand { get; set; }

        #endregion

        #region command implementation
        private void CopyGistFileUrl() => Clipboard.SetText(Url);

        /// <summary>
        /// <!-- RETIRED as wanting -->
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        protected async virtual Task OnFileNameChangedAsync(string originalName, string newName)
        {
            if (!string.IsNullOrEmpty(originalName) && newName != originalName)
                await FileNameChangedCommand.ExecuteAsync(newName);
        }

        internal async virtual Task UpdateGistAsync()
        {
           // await GistClientService.


            await UpdateGistCommand.ExecuteAsync(this.fileName);
        }
        private async Task RenameGistFileAsync(string newName) => await GistClientService.RenameGistFileAsync(ParentGist.Gist.Id, GistFile.Name, newName, Content, ParentGist.Description);

        private async Task UpdateGistFilenameAndContentAsync(string newName) => await GistClientService.RenameGistFileAsync(ParentGist.Gist.Id, GistFile.Name, newName, Content, ParentGist.Description);

        private async Task DeleteGistFileAsync() => await DeleteGistFileUpdateUiAsync();


        private async Task DeleteGistFileUpdateUiAsync()
        {

            await GistClientService.DeleteGistFileAsync(ParentGist.Gist.Id, FileName);
            ParentGist.Files.Remove(this);
        }


        private async Task RefreshGistFileAsync(GistHistoryEntryModel historyEntry)
        {
            var gistVersion = await GistClientService.GetGistVersionAsync(historyEntry.Url);
            var gistFile = gistVersion.GetFileById(Id);
            if (gistFile != null)
            {
                Content = await GistClientService.GetGistFileContentAsync(gistFile.Url);
                IsEnabled = true;
                Refreshing = true;
                FileName = gistFile.Name;
                Refreshing = false;
            }
            else
            {
                IsEnabled = false;
            }
        }
        #endregion
        public async Task OnHistoryCheckoutAsync(GistHistoryEntryViewModel version)
        {
            foreach (var historyEntry in History)
            {
                if (historyEntry.Version != version.Version)
                {
                    historyEntry.IsCheckedOut = false;
                }
            }
            await CheckoutCommand.ExecuteAsync(version.HistoryEntry);
        }
    }
}
