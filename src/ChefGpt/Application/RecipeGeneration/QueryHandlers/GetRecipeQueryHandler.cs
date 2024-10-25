// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using ChefGpt.Application.RecipeGeneration.Commands;
using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Application.RecipePrompting.CommandHandlers
{
    /// <summary>
    /// Handles the GetRecipeQuery to generate a recipe and optionally an image.
    /// </summary>
    public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeResponse>
    {
        private readonly IGptService gptService;
        private readonly IImageGenerationService imageGenerationService;
        private readonly ILogger<GetRecipeQueryHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRecipeQueryHandler"/> class.
        /// </summary>
        /// <param name="gptService">The GPT service for generating recipe instructions.</param>
        /// <param name="imageGenerationService">The image generation service for creating recipe images.</param>
        /// <param name="logger">The logger for logging information.</param>
        public GetRecipeQueryHandler(IGptService gptService, IImageGenerationService imageGenerationService, ILogger<GetRecipeQueryHandler> logger)
        {
            this.gptService = gptService;
            this.imageGenerationService = imageGenerationService;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the GetRecipeQuery request.
        /// </summary>
        /// <param name="request">The recipe query request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recipe response.</returns>
        public async Task<RecipeResponse> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Handling request: {request}", request.UserPrompt);
            var instructions = await this.gptService.Send(request, cancellationToken);

            var recipe = new RecipeResponse { Response = instructions, SessionId = request.SessionId };

            if (GeneratedFinalRecipe(instructions))
            {
                var imageUri = await this.imageGenerationService.GenerateImage(instructions, cancellationToken);
                recipe.ImageUri = imageUri;
            }

            return recipe;
        }

        /// <summary>
        /// Determines whether the generated recipe is the final version.
        /// </summary>
        /// <param name="recipe">The generated recipe instructions.</param>
        /// <returns><c>true</c> if the recipe is the final version; otherwise, <c>false</c>.</returns>
        private static bool GeneratedFinalRecipe(string recipe)
        {
            // Regex format should be read from config as the instructions may change
            return Regex.IsMatch(recipe, @"Here is your .+ recipe");
        }
    }
}