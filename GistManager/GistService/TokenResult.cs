using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.GistService
{
    public class TokenResult
    {
        public string Token { get; }
        public bool IsTokenGenerationSuccessful { get; }

        public TokenResult(bool isTokenGenerationSuccessful, string token = default)
        {
            if (isTokenGenerationSuccessful && string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("A valid token must be provided if authentication is successful");

            IsTokenGenerationSuccessful = isTokenGenerationSuccessful;
            Token = token;
        }
    }
}
