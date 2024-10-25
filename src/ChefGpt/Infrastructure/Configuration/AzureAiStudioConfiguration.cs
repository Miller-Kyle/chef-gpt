// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Infrastructure.Configuration
{
    internal class AzureAiStudioConfiguration
    {
        public string ApiKey { get; set; }

        public Uri DallEEndpoint { get; set; }

        public string DallEModel { get; set; }

        public Uri GptEndpoint { get; set; }
    }
}