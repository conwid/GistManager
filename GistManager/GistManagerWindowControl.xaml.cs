using GistManager.Utils;
using GistManager.ViewModels;
using Microsoft.VisualStudio.Shell.Interop;
using Octokit;
using Syncfusion.SfSkinManager;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using System.Drawing;
using Color = System.Windows.Media.Color;
using Brush = System.Windows.Media.Brush;
using System.Management.Instrumentation;
using System.Threading.Tasks;

namespace GistManager
{
    /// <summary>
    /// Interaction logic for GistManagerWindowControl.
    /// </summary>
    public partial class GistManagerWindowControl : UserControl
    {
        private readonly GistManagerWindowViewModel viewModel;

        internal CodeEditorManager CodeEditorManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GistManagerWindowControl"/> class.
        /// </summary>
        public GistManagerWindowControl(GistManagerWindowViewModel gistManagerWindowViewModel)
        {
            this.InitializeComponent();
            viewModel = gistManagerWindowViewModel;
            DataContext = viewModel;

            CodeEditorManager = new CodeEditorManager(this);

            GridLengthConverter converter = new GridLengthConverter();
            GistTreeRow.Height = (GridLength)converter.ConvertFromString(Properties.Settings.Default.GistTreeGridLength);

            ApplyTheme();
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        private void ApplyTheme()
        {
            UpdateTheme(Helpers.IsDarkMode());
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            bool themeChangedToDarkMode = Helpers.IsDarkMode();

            if (Properties.Settings.Default.DarkMode != themeChangedToDarkMode)
            {
                Properties.Settings.Default.DarkMode = Helpers.IsDarkMode();
                Properties.Settings.Default.Save();

                ApplyTheme(); // this ensures the right text color

                viewModel.IsAuthenticated = false;
                TopToolbar.Visibility = Visibility.Hidden;
                LoginPromptTB.Text = "Visual Studio Restart needed to reset theme.";
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "GistManagerWindow");
        }



        #region MyCode =========================================================================================
        // MyCode ==============================================================================================

        /// <summary>
        /// <!-- May need to form a spearate Theme class if get any more colros to pass in -->
        /// </summary>
        /// <param name="darkMode"></param>
        private void UpdateTheme(bool darkMode)
        {
            // DARK MODE
            SolidColorBrush globalTextColorBrushDark = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            SolidColorBrush backgroundTextBoxTextDark = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));


            // LIGHT MODE
            SolidColorBrush globalTextColorBrushLight = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));
            SolidColorBrush backgroundTextBoxTextLight = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));


            if (darkMode)
            {
                SfSkinManager.SetTheme(this, new Theme("MaterialDark"));
                UpdateThemeElements(globalTextColorBrushDark, backgroundTextBoxTextDark);
            }
            else
            {
                SfSkinManager.SetTheme(this, new Theme("MaterialLight"));
                UpdateThemeElements(globalTextColorBrushLight, backgroundTextBoxTextLight);
            }
        }

        private void UpdateThemeElements(Brush globalTextColorBrush, SolidColorBrush backgroundTextBoxText)
        {
            PublicGistGTVD.Expander.Foreground = globalTextColorBrush;
            PublicGistGTVD.TreeView.Foreground = globalTextColorBrush;

            PrivateGistGTVD.Expander.Foreground = globalTextColorBrush;
            PrivateGistGTVD.TreeView.Foreground = globalTextColorBrush;

            statusBar.Foreground = globalTextColorBrush;

            GistCodeEditor.Foreground = globalTextColorBrush;
            GistCodeEditor.LineNumberTextForeground = globalTextColorBrush;
            GistCodeEditor.CaretBrush = globalTextColorBrush;

            LanguageSelectorCB.Foreground = globalTextColorBrush;

            searchBox.Foreground = globalTextColorBrush;

            GistCodeEditor.InvalidateVisual();

        }


        //private void searchLabel_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (SystemConfiguraiton.DarkModeSelected())
        //    {
        //        ((System.Windows.Controls.Label)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        //    }
        //}


        private void ToggleErrorWordwrap_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorMessageDetailsTB.TextWrapping == TextWrapping.NoWrap)
                ErrorMessageDetailsTB.TextWrapping = TextWrapping.Wrap;
            else
                ErrorMessageDetailsTB.TextWrapping = TextWrapping.NoWrap;
        }
        private void CopyErrorText_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ErrorMessageDetailsTB.Text);
        }

        private void CollapseErrorDialog_Click(object sender, RoutedEventArgs e)
        {
            errorPanel.Visibility = Visibility.Collapsed;
        }

        private void OutlineButton_Click(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.ToggleOutline();
        }


        #endregion End: MyCode =================================================================================

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.UpdateGist();
        }



        private void GistTreeScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridLengthConverter converter = new GridLengthConverter();
            Properties.Settings.Default.GistTreeGridLength = converter.ConvertToString(GistTreeRow.Height);
            Properties.Settings.Default.Save();
        }

        private void ParentGistDescriptionTB_LostFocus(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.UpdateGist();
        }

        private void ParentGistDescriptionTB_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                CodeEditorManager.UpdateGist();
        }

        private void GistFilenameTB_LostFocus(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.UpdateGist();
        }

        private void GistFilenameTB_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                CodeEditorManager.UpdateGist();
        }

        private void AddNewGistBT_ClickAsync(object sender, RoutedEventArgs e)
        {
            viewModel.gistClientService.CreateGistAsync(" New Gist", "Gist File created in Visual Studio", true);
        }
    }
}