using System.Collections.Generic;
using System.Threading.Tasks;
using GistManager.GistService.Model;

namespace GistManager.GistService
{
    public interface IGistClientService
    {
        bool IsAuthenticated { get; }
        Task<bool> AuthenticateAsync();
        Task CreateGistAsync(string gistName, string firstFileContent, bool isPublic);
        Task CreateGistFileAsync(string gistId, string fileName, string fileContent);
        Task DeleteGistAsync(string gistId);
        Task CreateNewGistFileAsync(string gistId, string filename, string comment, string content);
        Task DeleteGistFileAsync(string gistId, string fileName);
        Task<string> GetGistFileContentAsync(string fileUrl);
        Task<IReadOnlyList<GistHistoryEntryModel>> GetGistHistoryEntriesAsync(string gistId);
        Task<IReadOnlyList<GistModel>> GetGistsAsync();
        Task<GistModel> GetGistVersionAsync(string url);
        Task LogoutAsync();
        Task RenameGistFileAsync(string gistId, string originalFileName, string newFileName, string content, string comment);
    }
}