using GistManager.GistService;
using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncCommand;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;
using GistManager.Mvvm.Commands.RelayCommand;
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

        #region constructors
        public GistFileViewModel(IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler)
        {
            GistClientService = gistClientService ?? throw new ArgumentNullException(nameof(gistClientService));
            this.asyncOperationStatusManager = asyncOperationStatusManager ?? throw new ArgumentNullException(nameof(asyncOperationStatusManager));
            FileNameChangedCommand = new AsyncCommand<string>(RenameGistFileAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Renaming gist file" };
            UpdateGistCommand = new AsyncCommand<string>(UpdateGistFilenameAndContentAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Updating gist file" };
            CheckoutCommand = new AsyncCommand<GistHistoryEntryModel>(RefreshGistFileAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Checking out file", SuppressCompletionCommand = true };
        }

        public GistFileViewModel(GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler) : this(gistClientService, commandStatusManager,errorHandler)
        {
            ParentGist = parent;
        }
        public GistFileViewModel(GistFileModel file, GistViewModel parent, IGistClientService gistClientService, IAsyncOperationStatusManager commandStatusManager, IErrorHandler errorHandler) : this(parent, gistClientService, commandStatusManager,errorHandler)
        {
            this.GistFile = file;
            fileName = file.Name;
            History = new ObservableRangeCollection<GistHistoryEntryViewModel>();
            Url = file.Url;
            DeleteGistFileCommand = new AsyncRelayCommand(DeleteGistFileAsync, commandStatusManager, errorHandler) { ExecutionInfo = "Deleting file from gist" };
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
        private AsyncCommand<GistHistoryEntryModel> CheckoutCommand { get; }
        
        // HACK: Command should be readonly
        protected AsyncCommand<string> FileNameChangedCommand { get; set; }

        internal AsyncCommand<string> UpdateGistCommand { get; set; }


        #endregion

        #region command implementation
        private void CopyGistFileUrl() => Clipboard.SetText(Url);

        protected async virtual Task OnFileNameChangedAsync(string originalName, string newName)
        {
            //if (!string.IsNullOrEmpty(originalName) && newName != originalName)
                await FileNameChangedCommand.ExecuteAsync(newName);
        }
        private async Task RenameGistFileAsync(string newName) => await GistClientService.RenameGistFileAsync(ParentGist.Gist.Id, GistFile.Name, newName, Content);

        private async Task UpdateGistFilenameAndContentAsync(string newName) => await GistClientService.RenameGistFileAsync(ParentGist.Gist.Id, GistFile.Name, newName, Content);

        private async Task DeleteGistFileAsync() => await GistClientService.DeleteGistFileAsync(ParentGist.Gist.Id, FileName);

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
