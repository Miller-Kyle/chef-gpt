// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Application.RecipeGeneration.Commands;

namespace ChefGpt.Application.RecipeGeneration.Services
{
    /// <summary>
    /// Interface for GPT service to generate recipe instructions.
    /// </summary>
    public interface IGptService
    {
        /// <summary>
        /// Sends a recipe query prompt to the GPT service and returns the generated recipe instructions.
        /// </summary>
        /// <param name="prompt">The recipe query prompt.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated recipe instructions.</returns>
        Task<string> Send(GetRecipeQuery prompt, CancellationToken cancellationToken);
    }
}