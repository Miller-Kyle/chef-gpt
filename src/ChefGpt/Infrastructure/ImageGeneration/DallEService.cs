// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Azure.AI.OpenAI;

using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Infrastructure.RecipePrompting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Infrastructure.ImageGeneration
{
    public class DallEService : IImageGenerationService
    {
        private readonly AzureAiStudioConfiguration aiStudioConfiguration = new AzureAiStudioConfiguration();

        private readonly OpenAIClient client;

        private readonly ILogger logger;

        public DallEService(OpenAIClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.client = client;
            configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(this.aiStudioConfiguration);
        }

        public async Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Generating Image");
            var imageGenerations = await this.client.GetImageGenerationsAsync(
                                       new ImageGenerationOptions
                                           {
                                               DeploymentName = this.aiStudioConfiguration.DallEModel, Prompt = recipe, Size = ImageSize.Size1024x1024, ImageCount = 1,
                                           },
                                       cancellationToken);

            var imageUri = imageGenerations.Value.Data.First().Url;
            return imageUri;
        }
    }
}