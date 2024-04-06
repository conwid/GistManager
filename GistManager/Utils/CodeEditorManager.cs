using EnvDTE;
using GistManager.ViewModels;
using Newtonsoft.Json.Linq;
using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Color = System.Windows.Media.Color;

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
            //  mainWindowControl.LanguageSelectorCB.Items = Syncfusion.EditWPFAssembly.
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

            // Check for illegal chars
            foreach (var c in Path.GetInvalidFileNameChars()) gistTempFile = gistTempFile.Replace(c, '-');

            File.WriteAllText(gistTempFile, _gistFileVM.Content);

            mainWindowControl.GistCodeEditor.DocumentSource = gistTempFile;
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

        }

        internal void ToggleOutline()
        {
            mainWindowControl.GistCodeEditor.EnableOutlining = !mainWindowControl.GistCodeEditor.EnableOutlining;
            mainWindowControl.GistCodeEditor.ShowLineNumber = !mainWindowControl.GistCodeEditor.ShowLineNumber;
        }

        internal void UpdateGist()
        {
            GistFileVM.UpdateGist();
        }


        internal void ApplyDarkModeToLanguageSelector()
        {
            Debug.WriteLine(mainWindowControl.GistCodeEditor.GetType());      

        }







    }
}
