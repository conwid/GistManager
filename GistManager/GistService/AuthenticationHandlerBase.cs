using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService
{
    public abstract class AuthenticationHandlerBase : IAuthenticationHandler
    {
        private const string accessTokenKey = "access_token";
        private readonly HttpClient httpClient;

        protected AuthenticationHandlerBase(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        protected abstract AuthenticationResult GetAuthenticationCode();
        public async Task<TokenResult> GetTokenAsync()
        {
            var authenticationResult = GetAuthenticationCode();
            if (!authenticationResult.IsAuthenticationSuccessful)
                return new TokenResult(false);
            
            var requestUri = new Uri($"https://github.com/login/oauth/access_token?client_id={Properties.Settings.Default.ClientId}" +
                $"&client_secret={Properties.Settings.Default.ClientSecret}" +
                $"&code={authenticationResult.AuthenticationCode}");
            using (var response = await httpClient.PostAsync(requestUri, null))
            {
                string responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                var accessToken = jsonObject[accessTokenKey];
                return new TokenResult(true, accessToken);
            }
        }
    }
}
