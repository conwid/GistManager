using GistManager.GistService;
using GistManager.GistService.Model;
using GistManager.Mvvm;
using GistManager.Mvvm.Commands.Async;
using GistManager.Mvvm.Commands.Async.AsyncRelayCommand;
using GistManager.Mvvm.Commands.RelayCommand;
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
            CreateGistFileCommand = new AsyncRelayCommand(CreateGistFileAsync, errorHandler);
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
        public ICommand CreateGistFileCommand { get; }
        #endregion

        #region command implementations
        private async Task DeleteGistAsync() => await GistClientService.DeleteGistAsync(Gist.Id);
        private void CopyGistUrl() => Clipboard.SetText(this.Url);
        private async Task CreateNewGistAsync() => await GistClientService.CreateGistAsync("NewGist.txt", "New Gist File - consider renaming", true);
        private async Task CreateGistFileAsync() => await CreateGistWithUniqueFilenameAsync();


        #endregion


        private async Task<Gist> CreateGistWithUniqueFilenameAsync()
        {
            string filenameBase = "NewGistFile";
            int count = 0;            
                
            while (Gist.Files.Any( gfm => gfm.Filename == $"{filenameBase}({count}).txt"))
            {
                count += 1;
            }

            string filename = $"{filenameBase}({count}).txt";
            string content = $"New Gist file created on {DateTime.Now}";

            // WARNING: conwid's original implementation has the Gist.ID set to the filename, not the Parent Gist.Id
            // If you try with the filename above GistClientService.CreateGistFileAsync - you get an error. It seems to need
            // the correct parent ID to insert the file against the parent. Thoughts: not sure why he's using filename as the ID??
            // NOT SURE THIS BREAKS ANYTHING YET, BUT MIGHT DO!!
            // TODO: check the RefreshGists routine as I guess this is where Ids are being set, as when you start anew, any checked in 
            // gists have the filename as ID. Could try altering the read in routine?
            Gist response =  await GistClientService.CreateGistFileAsync(Gist.Id, filename,content);

            // Difference in GistFileModel.ID between newly added and those rad at startup explained above. 
            Gist.AddGistFileModelToFiles(new GistFileModel(new GistFile(0, filename, null, null, content, response.HtmlUrl)));

            return response;
        }


        #region bound properties

        public ObservableRangeCollection<GistFileViewModel> Files { get; }
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
