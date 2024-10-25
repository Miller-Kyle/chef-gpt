// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Application.RecipeGeneration.Commands;

namespace ChefGpt.Application.RecipeGeneration.Services
{
    public interface IGptService
    {
        Task<string> Send(GetRecipeQuery prompt, CancellationToken cancellationToken);
    }
}