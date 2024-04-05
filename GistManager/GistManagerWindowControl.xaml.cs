using GistManager.Utils;
using GistManager.ViewModels;
using Octokit;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GistManager
{
    /// <summary>
    /// Interaction logic for GistManagerWindowControl.
    /// </summary>
    public partial class GistManagerWindowControl : UserControl
    {
        private bool darkModeButtonSetupCleared = false;
        private readonly GistManagerWindowViewModel viewModel;
        /// <summary>
        /// Initializes a new instance of the <see cref="GistManagerWindowControl"/> class.
        /// </summary>
        public GistManagerWindowControl(GistManagerWindowViewModel gistManagerWindowViewModel)
        {
            this.InitializeComponent();
            viewModel = gistManagerWindowViewModel;
            DataContext = viewModel;

            DarkModeToggleButton.IsChecked = Properties.Settings.Default.DarkMode;
            if (SystemConfiguraiton.DarkModeSelected())
            {
                searchBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 246, 246, 246));
                searchBox.Background = new SolidColorBrush(Color.FromArgb(255, 24, 24, 24));
                statusBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 246, 246, 246));
                statusBar.Background = new SolidColorBrush(Color.FromArgb(255, 40, 40, 40));
            }

            darkModeButtonSetupCleared = true; // soz - super hacky!

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
            if (!darkModeButtonSetupCleared) return;
            Properties.Settings.Default.DarkMode = (bool)DarkModeToggleButton.IsChecked;
            Properties.Settings.Default.Save();
            StatusBarLabal.Text = "Theme will refresh on restart";
            StatusBarImage.Width = 24;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (errorPanel.Visibility == Visibility.Collapsed || errorPanel.Visibility == Visibility.Hidden)
                errorPanel.Visibility = Visibility.Visible;
            else errorPanel.Visibility = Visibility.Collapsed;
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
    }
}