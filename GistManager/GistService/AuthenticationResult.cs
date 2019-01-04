using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService
{
    public class AuthenticationResult
    {
        public string AuthenticationCode { get; }
        public bool IsAuthenticationSuccessful { get; }

        public AuthenticationResult(bool isAuthenticationSuccessful, string authenticationCode = default)
        {
            if (isAuthenticationSuccessful && string.IsNullOrWhiteSpace(authenticationCode))
                throw new ArgumentException("A valid authentication code must be provided if authentication is successful");

            IsAuthenticationSuccessful = isAuthenticationSuccessful;
            AuthenticationCode = authenticationCode;
        }
    }
}
