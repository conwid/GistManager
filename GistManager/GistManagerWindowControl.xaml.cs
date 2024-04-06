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

namespace GistManager
{
    /// <summary>
    /// Interaction logic for GistManagerWindowControl.
    /// </summary>
    public partial class GistManagerWindowControl : UserControl
    {
        private readonly GistManagerWindowViewModel viewModel;



        private bool instantiated = false;

        internal CodeEditorManager CodeEditorManager;



        /// <summary>
        /// Initializes a new instance of the <see cref="GistManagerWindowControl"/> class.
        /// </summary>
        public GistManagerWindowControl(GistManagerWindowViewModel gistManagerWindowViewModel)
        {
            SfSkinManager.SetTheme(this, new Theme("MaterialDark", new string[] { "GistCodeEditor" }));

            this.InitializeComponent();
            viewModel = gistManagerWindowViewModel;
            DataContext = viewModel;

            CodeEditorManager = new CodeEditorManager(this);

            ApplyTheme();

            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            instantiated = true;
        }

        private void ApplyTheme()
        {

            bool darkMode = Helpers.IsDarkMode();
            UpdateTheme(darkMode);

        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            if (!instantiated) return;
            MessageBox.Show("Theme changed event args message: " + e.Message);

            if (this.IsLoaded)
            {
                Properties.Settings.Default.DarkMode = Helpers.IsDarkMode();
                Properties.Settings.Default.Save();

                viewModel.IsAuthenticated = false;
                TopToolbar.Visibility = Visibility.Hidden;
                StatusBarLabal.Text = "Restart needed to reset theme.";
                StatusBarImage.Width = 24;
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

        private void UpdateTheme(bool darkMode)
        {
            SolidColorBrush globalTextColorBrushDark = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));

            SolidColorBrush globalTextColorBrushLight = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10));


            if (darkMode)
            {
                SfSkinManager.SetTheme(this, new Theme("MaterialDark", new string[] { "GistCodeEditor" }));
                UpdateThemeElements(globalTextColorBrushDark);
            }
            else
            {
                SfSkinManager.SetTheme(this, new Theme("MaterialLight", new string[] { "GistCodeEditor" }));
                UpdateThemeElements(globalTextColorBrushLight);
            }

            DarkModeToggleButton.IsChecked = darkMode;
        }

        private void UpdateThemeElements(Brush globalTextColorBrush)
        {
            PublicGistGTVD.Expander.Foreground = globalTextColorBrush;
            PublicGistGTVD.TreeView.Foreground = globalTextColorBrush;

            PrivateGistGTVD.Expander.Foreground = globalTextColorBrush;
            PrivateGistGTVD.TreeView.Foreground = globalTextColorBrush;

            statusBar.Foreground = globalTextColorBrush;
            GistCodeEditor.Foreground = globalTextColorBrush;
            GistCodeEditor.LineNumberTextForeground = globalTextColorBrush;
            LanguageSelectorCB.Foreground = globalTextColorBrush;

            GistCodeEditor.InvalidateVisual();

        }


        private void searchLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (SystemConfiguraiton.DarkModeSelected())
            {
                ((System.Windows.Controls.Label)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDarkModeSettings();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDarkModeSettings();
        }

        private void UpdateDarkModeSettings()
        {
            Properties.Settings.Default.DarkMode = (bool)DarkModeToggleButton.IsChecked;
            Properties.Settings.Default.Save();
            UpdateTheme((bool)DarkModeToggleButton.IsChecked);

            StatusBarLabal.Text = "Theme will refresh on restart";
            StatusBarImage.Width = 24;
        }
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

        private void GistManager_Loaded(object sender, RoutedEventArgs e)
        {



        }

        private void LineNumbersButton_Click(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.ToggleLineNumbers();
        }

        private void OutlineButton_Click(object sender, RoutedEventArgs e)
        {
            CodeEditorManager.ToggleOutline();
        }

        #endregion End: MyCode =================================================================================


    }
}