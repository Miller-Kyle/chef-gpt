// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;

using ChefGpt.Application.Configuration;
using ChefGpt.Application.RecipeGeneration.Query;
using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Infrastructure.Authentication.Handlers;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Infrastructure.ImageGeneration;
using ChefGpt.Infrastructure.RecipePrompting;
using ChefGpt.Infrastructure.SessionStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChefGpt.Infrastructure
{
    /// <summary>
    ///     The main entry point for the ChefGpt application.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The main method, which is the entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            var appConfigEndpoint = new Uri(Environment.GetEnvironmentVariable(EnvironmentVariables.AppConfigurationEndpointKey)!);
            IConfigurationRefresher configurationRefresher = null;

            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath)
                          .AddJsonFile("local.settings.json", true, true)
                          .AddEnvironmentVariables()
                          .AddAzureAppConfiguration(options =>
                          {
                              options.Connect(appConfigEndpoint, GetManagedIdentity())
                                     .Select(KeyFilter.Any, InfrastructureConfigurationKeys.Label)
                                     .ConfigureRefresh(refresh =>
                                     {
                                         refresh.Register(InfrastructureConfigurationKeys.Sentinel, InfrastructureConfigurationKeys.Label, true);
                                     })
                                     .UseFeatureFlags(featureFlagConfiguration =>
                                     {
                                         featureFlagConfiguration.Select(KeyFilter.Any, InfrastructureConfigurationKeys.Label);
                                     })
                                     .ConfigureKeyVault(kv =>
                                     {
                                         kv.SetCredential(GetManagedIdentity());
                                     });
                              configurationRefresher = options.GetRefresher()!;
                          });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(hostContext, services, configurationRefresher!);
                })
                .Build();

            host.Run();
        }

        /// <summary>
        ///     Builds the OpenAI client using the specified configuration.
        /// </summary>
        /// <param name="configuration">The Azure AI Studio configuration.</param>
        /// <returns>The configured OpenAI client.</returns>
        private static OpenAIClient BuildOpenAiClient(AzureAiStudioConfiguration configuration)
        {
            return new OpenAIClient(configuration.DallEEndpoint, new AzureKeyCredential(configuration.ApiKey));
        }

        /// <summary>
        ///     Configures the services for the application.
        /// </summary>
        /// <param name="hostContext">The host builder context.</param>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configurationRefresher">The configuration refresher for Azure App Configuration.</param>
        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services, IConfigurationRefresher configurationRefresher)
        {
            var gptConfiguration = new GptConfiguration();
            var aiStudioConfiguration = new AzureAiStudioConfiguration();

            hostContext.Configuration.GetSection(ApplicationConfigurationKeys.GptConfiguration).Bind(gptConfiguration);
            hostContext.Configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(aiStudioConfiguration);

            services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(GetRecipeQuery).Assembly));

            services.AddSingleton(configurationRefresher);
            services.AddSingleton(BuildOpenAiClient(aiStudioConfiguration));
            services.AddSingleton<ISessionStorage, InMemorySessionStorage>();
            services.AddSingleton<IGptService, GptService>();
            services.AddSingleton<IImageGenerationService, DallEService>();
            services.AddHttpClient<IGptService, GptService>().AddHttpMessageHandler(_ => GetApiKeyAuthenticationHandler(aiStudioConfiguration));
        }

        /// <summary>
        ///     Creates an API key authentication handler.
        /// </summary>
        /// <param name="configuration">The Azure AI Studio configuration containing the API key.</param>
        /// <returns>A delegating handler that adds the API key to the request headers.</returns>
        private static DelegatingHandler GetApiKeyAuthenticationHandler(AzureAiStudioConfiguration configuration)
        {
            return new ApiKeyAuthenticationHandler(configuration.ApiKey);
        }

        /// <summary>
        ///     Gets the managed identity credential for Azure services.
        /// </summary>
        /// <returns>The managed identity credential.</returns>
        private static DefaultAzureCredential GetManagedIdentity()
        {
            return new DefaultAzureCredential(
                new DefaultAzureCredentialOptions { ManagedIdentityClientId = Environment.GetEnvironmentVariable(EnvironmentVariables.ManagedIdentityClientId)! });
        }
    }
}