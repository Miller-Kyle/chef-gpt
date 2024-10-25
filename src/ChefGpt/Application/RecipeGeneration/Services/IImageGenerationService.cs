// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Application.RecipeGeneration.Services
{
    public interface IImageGenerationService
    {
        Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken);
    }
}