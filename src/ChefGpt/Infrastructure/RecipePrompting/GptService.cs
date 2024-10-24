using ChefGpt.Application.Configuration;
using ChefGpt.Application.RecipePrompting;
using ChefGpt.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Infrastructure.RecipePrompting
{
    public class GptService : IGptService
    {
        private readonly HttpClient client;

        private readonly ILogger logger;

        private readonly GptConfiguration configuration = new GptConfiguration();

        public GptService(HttpClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.client = client;
            configuration.GetSection(ApplicationConfigurationKeys.GptConfiguration).Bind(this.configuration);
        }

        public Task<RecipeResponse> Send(string prompt, CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"Sending GPT Request {prompt}", prompt);
            throw new NotImplementedException();
        }
    }
}
