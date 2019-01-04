using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.GistCommand
{
    public class CreateGistCommandArgs
    {
        public CreateGistCommandArgs(string content)
        {
            this.Content = content;
        }
        public string Content { get; }
    }
}
