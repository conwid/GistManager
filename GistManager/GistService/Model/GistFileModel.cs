using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService.Model
{
    public class GistFileModel
    {
        private readonly GistFile gistFile;
        public GistFileModel(GistFile gistFile)
        {
            this.gistFile = gistFile;
        }

        public string Url => gistFile.RawUrl;
        public string Name => gistFile.Filename;
        public string Content => gistFile.Content;

        public string Id
        {
            get
            {
                var data = Url.Split('/');
                return data[data.Length - 2];
            }
        }
    }
}
