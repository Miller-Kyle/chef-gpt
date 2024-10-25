// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Application.RecipeGeneration.Services
{
    /// <summary>
    /// Interface for image generation service to create images based on recipe instructions.
    /// </summary>
    public interface IImageGenerationService
    {
        /// <summary>
        /// Generates an image based on the provided recipe instructions.
        /// </summary>
        /// <param name="recipe">The recipe instructions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the URI of the generated image.</returns>
        Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken);
    }
}