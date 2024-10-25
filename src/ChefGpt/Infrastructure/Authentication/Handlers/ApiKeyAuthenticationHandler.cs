// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Infrastructure.Authentication.Handlers
{
    /// <summary>
    /// A message handler that adds an API key to the request headers for authentication purposes.
    /// </summary>
    public class ApiKeyAuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        /// The API key used for authentication.
        /// </summary>
        private readonly string key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="key">The API key.</param>
        public ApiKeyAuthenticationHandler(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler after adding an authorization header
        /// and a custom operation status ID header to the request.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the HTTP response.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("api-key", this.key);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}