using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager
{
    public static class Constants
    {
        public const string ClientId = "7b2c75d657cb7c0f74fd";
        public const string ClientSecret = "3e9cae678b3fa92ddca819c98c9e613d07f1c6c9";
        
        public const string Scope = "gist";
        public const string ProductHeaderValue = "VsGistIntegration";
        public const string UserAgentHeaderValue = "VsGistIntegration";
        public const string RedirectUri = "http://vsgistintegration/authResult";
    }
}
