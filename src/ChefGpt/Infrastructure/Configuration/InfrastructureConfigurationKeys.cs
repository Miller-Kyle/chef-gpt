// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Infrastructure.Configuration
{
    /// <summary>
    ///     Contains keys for accessing various configuration settings in the ChefGpt application.
    /// </summary>
    internal class InfrastructureConfigurationKeys
    {
        /// <summary>
        ///     Key for accessing Azure AI Studio configuration settings.
        /// </summary>
        public const string AzureAiStudioConfiguration = "ChefGpt:AzureAiStudioConfiguration";

        /// <summary>
        ///     Key for accessing the general label configuration setting.
        /// </summary>
        public const string Label = "ChefGpt";

        /// <summary>
        ///     Key for accessing the sentinel configuration setting.
        /// </summary>
        public const string Sentinel = "ChefGpt:Sentinel";
    }
}