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
    public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeResponse>
    {
        private readonly IGptService gptService;

        private readonly IImageGenerationService imageGenerationService;

        private readonly ILogger<GetRecipeQueryHandler> logger;

        public GetRecipeQueryHandler(IGptService gptService, IImageGenerationService imageGenerationService, ILogger<GetRecipeQueryHandler> logger)
        {
            this.gptService = gptService;
            this.imageGenerationService = imageGenerationService;
            this.logger = logger;
        }

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

        private static bool GeneratedFinalRecipe(string recipe)
        {
            // Regex format should be read from config as the instructions may change
            return Regex.IsMatch(recipe, @"Here is your .+ recipe");
        }
    }
}