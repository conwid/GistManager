using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GistManager.GistService.Wpf
{
    public partial class WpfAuthenticationDialog : Window
    {
        private readonly string loginUri;
        public WpfAuthenticationDialog(string uri)
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Height = 800;
            Width = 600;

            loginUri = uri;

            webBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
        }

        private async void WebBrowser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webBrowser.Source.AbsoluteUri.StartsWith(Constants.RedirectUri))
            {
                if (webBrowser.Source.AbsoluteUri.Contains("code="))
                {
                    this.AuthCode = webBrowser.Source.AbsoluteUri.Split(new[] { "code=" }, StringSplitOptions.RemoveEmptyEntries)[1];

                    await Task.Run(() => { Thread.Sleep(2000); });

                    this.DialogResult = true;

                }
                else
                {
                    this.DialogResult = false;
                }
            }
        }

        public string AuthCode { get; private set; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GistManager");
            this.webBrowser.CreationProperties = new Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties { UserDataFolder = path };
            await this.webBrowser.EnsureCoreWebView2Async();
            this.webBrowser.CoreWebView2.Navigate(loginUri);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.webBrowser.NavigationCompleted -= WebBrowser_NavigationCompleted;
            base.OnClosing(e);
        }
    }
}
