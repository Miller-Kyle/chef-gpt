using Microsoft.Rest;

namespace ChefGpt.Infrastructure.Authentication.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        ///     The token provider that's used to generate the token
        /// </summary>
        private readonly ITokenProvider tokenProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticationHandler" /> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider.</param>
        public AuthenticationHandler(ITokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        /// <summary>
        ///     Sends an HTTP request to the inner handler after adding an authorization header
        ///     and a custom operation status ID header to the request.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the HTTP response.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authenticationHeader = await this.tokenProvider.GetAuthenticationHeaderAsync(cancellationToken);
            request.Headers.Authorization = authenticationHeader;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
