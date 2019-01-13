using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace GistManager.GistService.Wpf
{
    public partial class WpfAuthenticationDialog : Window
    {
        private readonly Uri loginUri;
        public WpfAuthenticationDialog(Uri uri)
        {
            InitializeComponent();
            loginUri = uri;
            webBrowser.NavigationCompleted += webBrowser_NavigationCompleted;
        }

        public string AuthCode { get; private set; }

        private void webBrowser_NavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e)
        {
            if (e.Uri.AbsoluteUri.StartsWith(Constants.RedirectUri))
            {
                if (e.Uri.AbsoluteUri.Contains("code="))
                {
                    this.AuthCode = e.Uri.AbsoluteUri.Split(new[] { "code=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.webBrowser.Navigate(loginUri);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.webBrowser.NavigationCompleted -= webBrowser_NavigationCompleted;
            base.OnClosing(e);
        }
    }
}
