using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService.Model
{
    public class GistModel
    {
        private readonly Gist gist;
        public GistModel(Gist gist)
        {
            this.gist = gist;
            files = new SortedList<string, GistFileModel>();
            History = new List<GistHistoryEntryModel>();
            foreach (var file in gist.Files)
            {
                files.Add(file.Key, new GistFileModel(file.Value));
            }
        }

        public string Id => gist.Id;
        public bool IsPublic => gist.Public;
        public string Description => gist.Description;
        public string Name => GetGistName();
        private string GetGistName()
        {
            if (gist.Files.Count == 0)
                return $"gist:{Guid.NewGuid().ToString("N").ToLower()}";
            return gist.Files.FirstOrDefault().Value.Filename;
        }
        public string Url => gist.HtmlUrl;

        private readonly SortedList<string, GistFileModel> files;
        public IList<GistFileModel> Files => files.Values;

        public GistFileModel GetFileByName(string name)
        {
            files.TryGetValue(name, out var result);
            return result;
        }
        public GistFileModel GetFileById(string id) => files.Values.SingleOrDefault(f => f.Id == id);

        public IReadOnlyList<GistHistoryEntryModel> History { get; }

    }
}
