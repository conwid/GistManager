using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GistManager.GistService
{
    public abstract class AuthenticationHandlerBase : IAuthenticationHandler
    {
        private const string acceptHeader = "application/json";
        private const string accessTokenKey = "access_token";

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.ParseAdd(acceptHeader);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgentHeaderValue);
            return client;
        }
        protected abstract AuthenticationResult GetAuthenticationCode();
        public async Task<TokenResult> GetTokenAsync()
        {
            var authenticationResult = GetAuthenticationCode();
            if (!authenticationResult.IsAuthenticationSuccessful)
            {
                return new TokenResult(false);
            }
            var requestUri = new Uri($"https://github.com/login/oauth/access_token?client_id={ClientInfo.ClientId}&client_secret={ClientInfo.ClientSecret}&code={authenticationResult.AuthenticationCode}");
            using (var httpClient = this.CreateHttpClient())
            using (var response = await httpClient.PostAsync(requestUri, null))
            {
                string responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var jsonObject = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(responseString);
                var accessToken = jsonObject[accessTokenKey];
                return new TokenResult(true, accessToken);
            }
        }
    }
}
