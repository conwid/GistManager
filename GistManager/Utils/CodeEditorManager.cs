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
using System.Windows.Media.Imaging;
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
        private BitmapImage saveEnabled;
        private BitmapImage saveDisabled;

        private GistManagerWindowControl mainWindowControl;

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
            // retrieves HasChanges status of the GitFile and updates the Save BUtton if needed
            SetSaveButtonOutline(GistFileVM.HasChanges);

            // get the parent GIst (i.e. technically the Gist not GistFile.
            // This can be itself as first gistfile alphabetically if classified as the Gist
            GistViewModel gistParentFile = _gistFileVM.ParentGist;

            // update the UIControls to the rleevant VM Data
            mainWindowControl.ParentGistName.Text = $"Gist: {gistParentFile.Name}";
            mainWindowControl.ParentGistDescriptionTB.Text = gistParentFile.Description;
            mainWindowControl.GistFilenameTB.Text = $"{_gistFileVM.FileName}";

            // Onto loading the code/contents into the code editor
            // need to create a temp file due to the SyntaxEditor control needing files, not stings
            // first check for any old versions and delete
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

            // Now update to new gistTempFile
            // get rid of the "gist" prefix"
            gistTempFile = _gistFileVM.FileName.Replace("Gist: ", "");

            // replace any illegal chars
            foreach (var c in Path.GetInvalidFileNameChars()) gistTempFile = gistTempFile.Replace(c, '-');

            // construct tempFile's final filename - stored at a clas level so can compare on load of next Gist
            gistTempFile = Path.Combine(Path.GetTempPath(), gistTempFile);

            // write the code to the tmep file
            File.WriteAllText(gistTempFile, _gistFileVM.Content);

            // set the code editor's source to this document
            mainWindowControl.GistCodeEditor.DocumentSource = gistTempFile;

            // once in the editor, no longer needed. Delete. 
            if (File.Exists(gistTempFile)) File.Delete(gistTempFile);

        }

        internal void ToggleOutline()
        {
            mainWindowControl.GistCodeEditor.ShowLineNumber = !mainWindowControl.GistCodeEditor.ShowLineNumber;
            mainWindowControl.GistCodeEditor.EnableOutlining = !mainWindowControl.GistCodeEditor.EnableOutlining;
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
            mainWindowControl.SaveButtonIMG.Source = saveDisabled;
            mainWindowControl.SaveButton.IsEnabled = false;

            // do repo update
            await GistFileVM.UpdateGistAsync();

            // enable Save button
            mainWindowControl.SaveButton.IsEnabled = true;
            mainWindowControl.SaveButtonIMG.Source = saveEnabled;

            // resets gist file has changes indicator
            SetGistFileHasChanges(false);

            return true;
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







    }
}
