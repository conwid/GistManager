using GistManager.GistService;
using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;
using GistManager.Mvvm.Commands.RelayCommand;
using GistManager.Utils;
using Microsoft.VisualStudio;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GistManager.ViewModels
{
    public class GistViewModel : BindableBase, IViewModelWithHistory
    {
        private readonly IAsyncOperationStatusManager asyncOperationStatusManager;
        protected IGistClientService GistClientService { get; }
        public GistModel Gist { get; }

        private IErrorHandler errorHandler;

        

        #region constructors
        protected GistViewModel(string name, IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler)
        {
            this.asyncOperationStatusManager = asyncOperationStatusManager ?? throw new ArgumentNullException(nameof(asyncOperationStatusManager));
            GistClientService = gistClientService ?? throw new ArgumentNullException(nameof(gistClientService));

            Name = name;
            Files = new ObservableRangeCollection<GistFileViewModel>();
            History = new ObservableRangeCollection<GistHistoryEntryViewModel>();
            DeleteGistCommand = new AsyncRelayCommand(DeleteGistAsync, asyncOperationStatusManager, errorHandler) { ExecutionInfo = "Deleting gist" };
            CopyGistUrlCommand = new RelayCommand(CopyGistUrl, errorHandler);
            CreateNewGistCommand = new AsyncRelayCommand(CreateNewGistAsync, errorHandler);
            this.errorHandler = errorHandler;
        }

        public GistViewModel(IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler) : this((string)null, gistClientService, asyncOperationStatusManager, errorHandler)
        {
        }
        public GistViewModel(GistModel gist, IGistClientService gistClientService, IAsyncOperationStatusManager asyncOperationStatusManager, IErrorHandler errorHandler) : this(gist.Name, gistClientService, asyncOperationStatusManager, errorHandler)
        {
            Gist = gist;
            Description = gist.Description;
            Public = gist.IsPublic;
            Url = gist.Url;
            var files = new List<GistFileViewModel>(gist.Files.Count);
            foreach (var file in gist.Files)
            {
                var gistFileViewModel = new GistFileViewModel(file, this, gistClientService, asyncOperationStatusManager, errorHandler);
                gistFileViewModel.History.AddRange(gist.History.Select(h => new GistHistoryEntryViewModel(h, gistFileViewModel)));
                files.Add(gistFileViewModel);
            }
            Files.AddRange(files);
            History.AddRange(gist.History.Select(h => new GistHistoryEntryViewModel(h, this)));
            History.First().IsCheckedOut = true;
        }
        #endregion

        #region commands
        public ICommand DeleteGistCommand { get; }
        public ICommand CopyGistUrlCommand { get; }
        public ICommand CreateNewGistCommand { get; }
        #endregion

        #region command implementations
        private async Task DeleteGistAsync() => await GistClientService.DeleteGistAsync(Gist.Id);
        private void CopyGistUrl() => Clipboard.SetText(this.Url);
        private async Task CreateNewGistAsync() => await CreateNewGistUniqueFilenameAsync();

        #endregion

        private async Task CreateNewGistUniqueFilenameAsync()
        {
            string filenameRoot = "NewGistFile";
            int count = 0;

            while (Files.Any(x => x.FileName == $"{filenameRoot} ({count})"))
            {
                count += 1;
            }
            string finalFilename = $"{filenameRoot} ({count})";
            string fileContent = $"New Gist File created at {DateTime.Now}: {finalFilename}";


            var result = await GistClientService.CreateGistFileAsync(Gist.Id, finalFilename, fileContent);

            GistFile gistFile = result.Files[finalFilename];
            
            GistFileModel gfm = new GistFileModel(gistFile);            

            GistFileViewModel newGistFileViewModel = new GistFileViewModel(gfm, this, GistClientService, asyncOperationStatusManager, errorHandler);

            Files.Add(newGistFileViewModel);

        }

        #region bound properties

        public ObservableRangeCollection<GistFileViewModel> Files { get; set; }
        public ObservableRangeCollection<GistHistoryEntryViewModel> History { get; }

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Public { get; set; }
        #endregion

        public Task OnHistoryCheckoutAsync(GistHistoryEntryViewModel version)
        {
            foreach (var historyEntry in History)
            {
                if (historyEntry.Version != version.Version)
                    historyEntry.IsCheckedOut = false;
            }
            foreach (var file in Files)
            {
                file.History.Single(h => h.Version == version.Version).IsCheckedOut = true;
            }
            return Task.CompletedTask;
        }
    }
}
