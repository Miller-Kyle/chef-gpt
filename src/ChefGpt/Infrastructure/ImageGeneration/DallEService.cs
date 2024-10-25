using ChefGpt.Application.RecipeGeneration.Commands;
using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Infrastructure.RecipePrompting;
using ChefGpt.Infrastructure.SessionStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Images;
using Azure.Identity;
using OpenAI.Images;
using OpenAI;

namespace ChefGpt.Infrastructure.ImageGeneration
{
    public class DallEService : IImageGenerationService
    {
        private readonly OpenAIClient client;

        private readonly ILogger logger;

        public DallEService(OpenAIClient client, ILogger<GptService> logger)
        {
            this.logger = logger;
            this.client = client;
        }

        public async Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken)
        {
            // read dall-e-3 from config
            var imageClient = this.client.GetImageClient("dall-e-3");
            var image = await imageClient.GenerateImageAsync(recipe, cancellationToken: cancellationToken);
            var imageUri = image.Value.ImageUri;
            return imageUri;
        }
    }
}
