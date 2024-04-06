using EnvDTE;
using GistManager.GistService;
using GistManager.ViewModels;
using Newtonsoft.Json.Linq;
using Octokit;
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

        public bool GistEdited { get; set; } = false;

        private GistFileViewModel _gistFileVM = null;
        private string gistTempFile = null;
        private bool gistIsDirty = false;

        private GistManagerWindowControl mainWindowControl;

        public CodeEditorManager(GistManagerWindowControl mainWindowControl)
        {
            this.mainWindowControl = mainWindowControl;
            //  mainWindowControl.LanguageSelectorCB.Items = Syncfusion.EditWPFAssembly.
        }

        private async void OnGistFileChanged()
        {
            // Checks to see if the gist has been edited. If so, saves it. 
            if (GistEdited)
            {
                mainWindowControl.MidPanel.IsEnabled = false;
                var reposnse = await mainWindowControl.ViewModel.gistClientService.RenameGistFileAsync(GistFileVM.ParentGist.Gist.Id, GistFileVM.FileName, GistFileVM.FileName, GistFileVM.Content, GistFileVM.ParentGist.Description);
                await UpdateGist();
            }

            GistViewModel gistParentFile = _gistFileVM.ParentGist;

            mainWindowControl.ParentGistName.Text = $"Gist: {gistParentFile.Name}";
            mainWindowControl.ParentGistDescriptionTB.Text = gistParentFile.Description;
            mainWindowControl.GistFilenameTB.Text = $"{_gistFileVM.FileName}";

            // Now load editor - need to create a temp file
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

            // Now update to new gistTempFile
            // get rid of the "gist" prefix"
            gistTempFile = _gistFileVM.FileName.Replace("Gist: ", "");

            // replace any illegal chars
            foreach (var c in Path.GetInvalidFileNameChars()) gistTempFile = gistTempFile.Replace(c, '-');

            gistTempFile = Path.Combine(Path.GetTempPath(), gistTempFile);

            File.WriteAllText(gistTempFile, _gistFileVM.Content);

            mainWindowControl.GistCodeEditor.DocumentSource = gistTempFile;
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

            mainWindowControl.MidPanel.IsEnabled = true;
            GistEdited = false;
        }

        internal void ToggleOutline()
        {
            mainWindowControl.GistCodeEditor.EnableOutlining = !mainWindowControl.GistCodeEditor.EnableOutlining;
            mainWindowControl.GistCodeEditor.ShowLineNumber = !mainWindowControl.GistCodeEditor.ShowLineNumber;
        }

        internal async Task UpdateGist()
        {
            GistFileVM.Content = mainWindowControl.GistCodeEditor.Text;
            GistFileVM.ParentGist.Description = mainWindowControl.ParentGistDescriptionTB.Text;

            // Changing the filename causes update to Gists API
            GistFileVM.FileName = mainWindowControl.GistFilenameTB.Text;
        }

        internal void ApplyDarkModeToLanguageSelector()
        {
            Debug.WriteLine(mainWindowControl.GistCodeEditor.GetType());

        }







    }
}
