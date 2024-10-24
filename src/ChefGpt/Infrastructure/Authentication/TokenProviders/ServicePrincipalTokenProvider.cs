using Microsoft.Rest;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ChefGpt.Infrastructure.Authentication.TokenProviders
{
    public class ServicePrincipalTokenProvider : ITokenProvider
    {
        /// <summary>
        ///     The service principals client identifier
        /// </summary>
        private readonly string clientId;

        /// <summary>
        ///     The service principals client secret
        /// </summary>
        private readonly string clientSecret;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServicePrincipalTokenProvider" /> class.
        /// </summary>
        /// <param name="clientId">The client ID of the service principal.</param>
        /// <param name="clientSecret">The client secret of the service principal.</param>
        public ServicePrincipalTokenProvider(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        /// <summary>
        ///     Gets or sets the authority URI for the token request.
        /// </summary>
        public string AuthorityUri { get; set; }

        /// <summary>
        ///     Gets or sets the scope for which the token is requested.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        ///     Asynchronously retrieves an authentication header value containing a bearer token.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the authentication header.</returns>
        /// <exception cref="ArgumentException">Thrown when an empty access token is retrieved.</exception>
        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var authRequest = new Dictionary<string, string>
                                  {
                                      { "grant_type", "client_credentials" },
                                      { "client_id", this.clientId },
                                      { "client_secret", this.clientSecret },
                                      { "scope", this.Scope },
                                  };

            var body = new FormUrlEncodedContent(authRequest);

            // Send a POST request to the authority URI to obtain the token.
            var response = await new HttpClient().PostAsync(this.AuthorityUri, body, cancellationToken);
            response.EnsureSuccessStatusCode(); // Ensure the response is successful.

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var token = JsonConvert.DeserializeAnonymousType(responseContent, new { access_token = string.Empty, token_type = string.Empty });

            // Validate the retrieved access token.
            if (string.IsNullOrWhiteSpace(token.access_token))
            {
                throw new ArgumentException("Empty access token retrieved.");
            }

            // Return the bearer token as an authentication header.
            return new AuthenticationHeaderValue(token.token_type, token.access_token);
        }
    }
}
