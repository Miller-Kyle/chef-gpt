// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration settings for Azure AI Studio integration.
    /// </summary>
    internal class AzureAiStudioConfiguration
    {
        /// <summary>
        /// Gets or sets the API key for accessing Azure AI Studio services.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URI for DALL-E service.
        /// </summary>
        public Uri DallEEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the model name for DALL-E service.
        /// </summary>
        public string DallEModel { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URI for GPT service.
        /// </summary>
        public Uri GptEndpoint { get; set; }
    }
}