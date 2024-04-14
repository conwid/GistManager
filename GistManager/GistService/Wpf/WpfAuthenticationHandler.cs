using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GistManager.GistService.Wpf
{
    public class WpfAuthenticationHandler : AuthenticationHandlerBase
    {
        public WpfAuthenticationHandler(HttpClient httpClient) : base(httpClient)
        {

        }

        protected override AuthenticationResult GetAuthenticationCode()
        {
            try
            {
                var url = string.Format("https://github.com/login/oauth/authorize?client_id={0}&scope={1}", Properties.Settings.Default.ClientId, Constants.Scope);
                var authDialog = new WpfAuthenticationDialog(url);
                authDialog.Background = new SolidColorBrush(Color.FromArgb(255, 13, 17, 23));
                authDialog.ShowDialog();
                if (authDialog.DialogResult ?? false)
                    return new AuthenticationResult(true, authDialog.AuthCode);

                if (!authDialog.DialogResult ?? true)
                    return new AuthenticationResult(false);
                
                throw new AuthenticationException("Could not authenticate user");
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Could not authenticate user", ex);
            }
        }
    }
}
