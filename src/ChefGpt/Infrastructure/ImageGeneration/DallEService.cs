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
    /// <summary>
    ///     Service for generating images using DALL-E.
    /// </summary>
    public class DallEService : IImageGenerationService
    {
        private readonly AzureAiStudioConfiguration aiStudioConfiguration = new AzureAiStudioConfiguration();

        private readonly OpenAIClient client;

        private readonly ILogger logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DallEService" /> class.
        /// </summary>
        /// <param name="client">The OpenAI client for interacting with the DALL-E service.</param>
        /// <param name="logger">The logger for logging information.</param>
        /// <param name="configuration">The configuration for accessing settings.</param>
        public DallEService(OpenAIClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.client = client;
            configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(this.aiStudioConfiguration);
        }

        /// <summary>
        ///     Generates an image based on the provided recipe instructions.
        /// </summary>
        /// <param name="recipe">The recipe instructions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the URI of the generated image.</returns>
        public async Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Generating Image");
            var imageGenerations = await this.client.GetImageGenerationsAsync(
                                       new ImageGenerationOptions
                                           {
                                               DeploymentName = this.aiStudioConfiguration.DallEModel, 
                                               Prompt = recipe, 
                                               Size = ImageSize.Size1024x1024,
                                               ImageCount = 1,
                                           },
                                       cancellationToken);

            var imageUri = imageGenerations.Value.Data[0].Url;
            return imageUri;
        }
    }
}