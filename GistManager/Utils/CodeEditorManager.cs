using GistManager.ViewModels;
using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;

namespace GistManager.Utils
{
    internal class CodeEditorManager
    {
        public GistFileViewModel GistFileVM
        {
            get { return gistFileVM; }
            set
            {
                //ensure any residual changes not trigge
                CheckForChangesBeforeGistFileVMChange();

                gistFileVM = value;
                OnGistFileChanged();
            }
        }

        private GistFileViewModel gistFileVM = null;
        private string gistTempFile = null;
        private BitmapImage saveEnabled;
        private BitmapImage saveDisabled;

        private GistManagerWindowControl mainWindowControl;

        private Dictionary<List<String>, Languages> codeLanguageMappings = new Dictionary<List<string>, Languages>()
            {
            {new List<string>() {"c" }, Languages.C },
            {new List<string>() {"cs" }, Languages.CSharp },
            {new List<string>() {"dpr", "pas", "dfm" }, Languages.Delphi },
            {new List<string>() {"html", "htm" }, Languages.HTML },
            {new List<string>() {"java" }, Languages.Java },
            {new List<string>() {"js", "cjs", "mjs" }, Languages.JScript },
            {new List<string>() {"ps1" }, Languages.PowerShell },
            {new List<string>() {"sql" }, Languages.SQL },
            {new List<string>() {"txt" }, Languages.Text },
            {new List<string>() {"vbs" }, Languages.VBScript },
            {new List<string>() {"vb" }, Languages.VisualBasic },
            {new List<string>() {"xaml" }, Languages.XAML },
            {new List<string>() {"xml" }, Languages.XML },
        };

        public CodeEditorManager(GistManagerWindowControl mainWindowControl)
        {
            this.mainWindowControl = mainWindowControl;

            // below used to indicate user cannot save for any async/await events
            saveEnabled = new BitmapImage(new Uri("pack://application:,,,/GistManager;component/Resources/Save.png"));
            saveDisabled = new BitmapImage(new Uri("pack://application:,,,/GistManager;component/Resources/SaveInactive.png"));

        }

        private void SetSaveButtonOutline(bool visible)
        {
            if (visible)
                mainWindowControl.SaveButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            else
                mainWindowControl.SaveButton.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        internal void SetGistFileHasChanges(bool isChanged)
        {
            // this provide visual cue to user that gist has changes
            SetSaveButtonOutline(isChanged);
            GistFileVM.HasChanges = isChanged;
        }
        private void OnGistFileChanged()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // first delete temporary file of last GistFileView
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

            // retrieves HasChanges status of the GitFile and updates the Save BUtton if needed
            SetSaveButtonOutline(GistFileVM.HasChanges);

            // get the parent GIst (i.e. technically the Gist not GistFile.
            // This can be itself as first gistfile alphabetically if classified as the Gist
            GistViewModel gistParentFile = gistFileVM.ParentGist;

            // update the UIControls to the rleevant VM Data
            mainWindowControl.ParentGistName.Text = $"Gist: {gistParentFile.Name}";
            mainWindowControl.ParentGistDescriptionTB.Text = gistParentFile.Description;
            mainWindowControl.GistFilenameTB.Text = $"{gistFileVM.FileName}";

            // Onto loading the code/contents into the code editor
            // need to create a temp file due to the SyntaxEditor control needing files, not stings
            // first check for any old versions and delete
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

            // Now update to new gistTempFile
            // get rid of the "gist" prefix"
            gistTempFile = gistFileVM.FileName.Replace("Gist: ", "");

            // replace any illegal chars
            foreach (var c in Path.GetInvalidFileNameChars()) gistTempFile = gistTempFile.Replace(c, '-');

            // construct tempFile's final filename - stored at a clas level so can compare on load of next Gist
            gistTempFile = Path.Combine(Path.GetTempPath(), gistTempFile);

            // write the code to the tmep file
            File.WriteAllText(gistTempFile, gistFileVM.Content);

            // Reset the language
            mainWindowControl.GistCodeEditor.DocumentLanguage = Languages.Text;

            // set the code editor's source to this document
            mainWindowControl.GistCodeEditor.DocumentSource = gistTempFile;

            // now try and auto-math the language form any extension
            string ext = Path.GetExtension(gistTempFile).Replace(".","");

            if (ext != null)
            {
                var languageKvp = codeLanguageMappings.Where(x => x.Key.Contains(ext)).FirstOrDefault();
                if (!languageKvp.Equals(default(KeyValuePair<List<string>, Languages>)))
                {
                    ChangeEditorLanguage(languageKvp.Value.ToString());
                    mainWindowControl.LanguageSelectorCB.Text = languageKvp.Value.ToString();
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        internal void ToggleOutline(bool? state)
        {
            mainWindowControl.GistCodeEditor.ShowLineNumber = (bool)state;
            mainWindowControl.GistCodeEditor.EnableOutlining = (bool)state;
        }

        internal void ToggleIntellisense(bool? state)
        {
            mainWindowControl.GistCodeEditor.EnableIntellisense = (bool)state;
        }

        internal void ToggleAutoIndent(bool? state)
        {
            mainWindowControl.GistCodeEditor.IsAutoIndentationEnabled = (bool)state;
        }

        /// <summary>
        /// Updates the Gist on the Gist Repository
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> UpdateGistOnRepositoryAsync()
        {
            // return save button border to normal (aesthetics) 
            SetSaveButtonOutline(false);

            // update GistFileViewModel for edge cases where changes not caught by UI changes
            UpdateGistViewModel();

            // disable Save button
           // mainWindowControl.SaveButtonIMG.Source = saveDisabled;
            mainWindowControl.SaveButton.IsEnabled = false;

            // do repo update
            await GistFileVM.UpdateGistAsync();

            // enable Save button
            mainWindowControl.SaveButton.IsEnabled = true;
           // mainWindowControl.SaveButtonIMG.Source = saveEnabled;

            // resets gist file has changes indicator
            SetGistFileHasChanges(false);

            mainWindowControl.GistCodeEditor.SaveFile(gistTempFile);

            return true;
        }

        private void CheckForChangesBeforeGistFileVMChange()
        {
            CheckUiWithGistVmForChanges();
        }

        private void UpdateGistViewModel()
        {
            // update the Gist's viewmodel form the UIElements
            GistFileVM.ParentGist.Description = mainWindowControl.ParentGistDescriptionTB.Text;
            GistFileVM.Content = mainWindowControl.GistCodeEditor.Text;
            GistFileVM.FileName = mainWindowControl.GistFilenameTB.Text;
        }

        internal void CheckUiWithGistVmForChanges()
        {
            if (GistFileVM == null) return;

            // checks the UIElement values against the view model for changes
            if (mainWindowControl.GistCodeEditor.Text != GistFileVM.Content ||
                mainWindowControl.GistFilenameTB.Text != GistFileVM.FileName ||
               mainWindowControl.ParentGistDescriptionTB.Text != GistFileVM.ParentGist.Description)
            {
                // changes found - do Gist ViewModel update  
                UpdateGistViewModel();

                // set gist file has changes indicator to true
                SetGistFileHasChanges(true);
            }
        }

        internal void ChangeEditorLanguage(string languageString)
        {
            Languages language = (Languages)Enum.Parse(typeof(Languages), languageString);
            mainWindowControl.GistCodeEditor.DocumentLanguage = language;
        }





    }
}
