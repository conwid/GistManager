using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService.Wpf
{
    public class WpfAuthenticationHandler : AuthenticationHandlerBase
    {
        protected override AuthenticationResult GetAuthenticationCode()
        {
            try
            {
                var url = new Uri(string.Format("https://github.com/login/oauth/authorize?client_id={0}&scope={1}", Constants.ClientId, Constants.Scope));
                var authDialog = new WpfAuthenticationDialog(url);                                
                authDialog.ShowDialog();
                if (authDialog.DialogResult ?? false)
                {
                    return new AuthenticationResult(true, authDialog.AuthCode);
                }

                if (!authDialog.DialogResult ?? true)
                {
                    return new AuthenticationResult(false);
                }
                throw new AuthenticationException("Could not authenticate user");
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Could not authenticate user", ex);
            }
        }
    }
}
