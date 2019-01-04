using GistManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.GistCommand
{
    public class CreateGistFileCommandArgs
    {
        public CreateGistFileCommandArgs(string content, GistViewModel parentGist)
        {
            Content = content;
            ParentGist = parentGist;
        }
        public string Content { get; }
        public GistViewModel ParentGist { get; }
    }
}
