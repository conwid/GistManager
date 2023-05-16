using GistManager.GistService.Model;
using GistManager.GistService.Wpf;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService
{
    public class GistClientService : IGistClientService
    {
        private readonly TimeSpan downloadTimeOut = TimeSpan.FromMinutes(1);
        private readonly GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(Constants.ProductHeaderValue));
        private readonly Func<IAuthenticationHandler> authenticationHandlerFactory;

        public GistClientService(Func<IAuthenticationHandler> authenticationHandlerFactory)
        {
            this.authenticationHandlerFactory = authenticationHandlerFactory ?? throw new ArgumentNullException(nameof(authenticationHandlerFactory));
        }

        private IGistsClient gistClient => gitHubClient.Gist;
        private IConnection gitHubConnection => gitHubClient.Connection;
        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GistManagerForVisualStudio");
            return client;
        }
        private async Task<T> DownloadAsync<T>(string url)
        {
            return (await gitHubConnection.Get<T>(new Uri(url), downloadTimeOut)).Body;
        }

        public async Task CreateGistFileAsync(string gistId, string fileName, string fileContent)
        {
            var update = new GistUpdate();
            update.Files.Add(fileName, new GistFileUpdate { Content = fileContent, NewFileName = fileName });
            await gistClient.Edit(gistId, update);
        }
        public async Task CreateGistAsync(string gistName, string firstFileContent, bool isPublic)
        {
            var newGist = new NewGist
            {
                Public = isPublic,
                Description = "Gist created from visual studio extension",
            };
            newGist.Files.Add(gistName, firstFileContent);
            await gistClient.Create(newGist);
        }
        public async Task RenameGistFileAsync(string gistId, string originalFileName, string newFileName, string content)
        {
            var update = new GistUpdate();
            update.Files.Add(originalFileName, new GistFileUpdate { Content = content, NewFileName = newFileName });
            await gistClient.Edit(gistId, update);
        }
        public async Task DeleteGistFileAsync(string gistId, string fileName)
        {
            var update = new GistUpdate();
            update.Files.Add(fileName, new GistFileUpdate { Content = null, NewFileName = null });
            await gistClient.Edit(gistId, update);
        }
        public async Task DeleteGistAsync(string gistId)
        {
            await gistClient.Delete(gistId);
        }
        public async Task<string> GetGistFileContentAsync(string fileUrl)
        {
            return await DownloadAsync<string>(fileUrl);
        }
        public async Task<bool> AuthenticateAsync()
        {
            IsAuthenticated = false;
            var tokenResult = await authenticationHandlerFactory().GetTokenAsync();
            if (!tokenResult.IsTokenGenerationSuccessful)
                return false;
            gitHubClient.Connection.Credentials = new Credentials(tokenResult.Token);
            this.IsAuthenticated = true;
            return true;
        }

        public async Task<IReadOnlyList<GistModel>> GetGistsAsync()
        {
            var gists = (await gistClient.GetAll()).Select(g => new GistModel(g)).ToList();
            foreach (var gist in gists)
            {
                // HACK: History is readonly!!!!
                ((List<GistHistoryEntryModel>)gist.History).AddRange(await GetGistHistoryEntriesAsync(gist.Id));
            }
            return gists;
        }
        public async Task<IReadOnlyList<GistHistoryEntryModel>> GetGistHistoryEntriesAsync(string gistId)
        {
            return (await gistClient.GetAllCommits(gistId)).Select(gh => new GistHistoryEntryModel(gh)).ToList();
        }
        public async Task<GistModel> GetGistVersionAsync(string url)
        {
            return new GistModel(await DownloadAsync<Gist>(url));
        }
        public Task LogoutAsync()
        {
            this.IsAuthenticated = false;
            return Task.CompletedTask;
        }
        public bool IsAuthenticated { get; private set; }
    }
}
