using GistManager.ViewModels;
using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace GistManager.Utils
{
    internal class CodeEditorManager
    {

 
        public GistFileViewModel GistFileVM
        {
            get { return _gistFileVM; }
            set
            {
                _gistFileVM = value;
                OnGistFileChanged();
            }            
        }

        private GistFileViewModel _gistFileVM = null;
        private string gistTempFile = null;


        private GistManagerWindowControl mainWindowControl;

        public CodeEditorManager(GistManagerWindowControl mainWindowControl)
        {
            this.mainWindowControl = mainWindowControl;  
        }

        private void OnGistFileChanged()
        {
            GistViewModel gistParentFile = _gistFileVM.ParentGist;
            
            mainWindowControl.ParentGistName.Text = $"Gist: {gistParentFile.Name}";
            mainWindowControl.ParentGistDescription.Text = gistParentFile.Description;
            mainWindowControl.GistFilename.Text = $"File: {_gistFileVM.FileName}";

            // Now load editor - need to create a temp file
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);
            gistTempFile = Path.Combine(Path.GetTempPath(), _gistFileVM.FileName.Replace("Gist: ", ""));

            File.WriteAllText(gistTempFile, _gistFileVM.Content);

            mainWindowControl.GistCodeEditor.DocumentSource = gistTempFile;
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

        }








    }
}
