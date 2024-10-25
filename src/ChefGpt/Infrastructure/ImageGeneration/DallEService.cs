using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Infrastructure.RecipePrompting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Azure.AI.OpenAI;
using Azure;

namespace ChefGpt.Infrastructure.ImageGeneration
{
    public class DallEService : IImageGenerationService
    {
        private readonly OpenAIClient client;

        private readonly ILogger logger;

        private readonly AzureAiStudioConfiguration aiStudioConfiguration = new AzureAiStudioConfiguration();

        public DallEService(OpenAIClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.client = client;
            configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(this.aiStudioConfiguration);
        }

        public async Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken)
        {
            var imageGenerations = await client.GetImageGenerationsAsync(
                new ImageGenerationOptions()
                {
                    DeploymentName = this.aiStudioConfiguration.DallEModel,
                    Prompt = recipe,
                    Size = ImageSize.Size1024x1024,
                    ImageCount = 1,
                    
                });

            var imageUri = imageGenerations.Value.Data.First().Url;
            return imageUri;
        }
    }
}
