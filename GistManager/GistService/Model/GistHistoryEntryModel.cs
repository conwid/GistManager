using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService.Model
{
    public class GistHistoryEntryModel
    {
        private readonly GistHistory gistHistory;
        public GistHistoryEntryModel(GistHistory gistHistory)
        {
            this.gistHistory = gistHistory;
        }
        public string Url => gistHistory.Url;
        public string Version => gistHistory.Version;
        public DateTimeOffset CommittedAt => gistHistory.CommittedAt;
    }
}
