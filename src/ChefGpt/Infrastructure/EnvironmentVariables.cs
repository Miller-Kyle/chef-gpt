// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Infrastructure
{
    /// <summary>
    /// Contains constants for environment variable keys used in the ChefGpt application.
    /// </summary>
    internal class EnvironmentVariables
    {
        /// <summary>
        /// The key for the application configuration endpoint environment variable.
        /// </summary>
        public const string AppConfigurationEndpointKey = "APP_CONFIGURATION_ENDPOINT";

        /// <summary>
        /// The key for the managed identity client ID environment variable.
        /// </summary>
        public const string ManagedIdentityClientId = "MANAGED_IDENTITY_CLIENT_ID";
    }
}