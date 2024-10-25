using Azure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChefGpt.Application.Configuration;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Application.RecipeGeneration.Commands;
using ChefGpt.Infrastructure.RecipePrompting;
using ChefGpt.Infrastructure.Authentication.Handlers;
using ChefGpt.Infrastructure.SessionStorage;
using ChefGpt.Application.RecipeGeneration.Services;
using System.Configuration;
using OpenAI;
using ChefGpt.Infrastructure.ImageGeneration;
using System.ClientModel;

namespace ChefGpt.Infrastructure
{
    public class Program
    {
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
            services.AddHttpClient<IGptService, GptService>()
                    .AddHttpMessageHandler(_ => GetApiKeyAuthenticationHandler(aiStudioConfiguration));
        }

        private static OpenAIClient BuildOpenAiClient(AzureAiStudioConfiguration configuration)
        {
            var credential = new ApiKeyCredential(configuration.ApiKey);
            var options = new OpenAIClientOptions
            {
                Endpoint = configuration.DallEEndpoint
            };
            return new OpenAIClient(credential, options);
        }


        private static DefaultAzureCredential GetManagedIdentity()
        {
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = Environment.GetEnvironmentVariable(EnvironmentVariables.ManagedIdentityClientId)!
            });
        }

        private static DelegatingHandler GetApiKeyAuthenticationHandler(AzureAiStudioConfiguration configuration)
        {
            return new ApiKeyAuthenticationHandler(configuration.ApiKey);
        }
    }
}