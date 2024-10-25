// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Domain.Models;
using MediatR;

namespace ChefGpt.Application.RecipeGeneration.Commands
{
    /// <summary>
    /// Query to get a recipe based on user input.
    /// </summary>
    public class GetRecipeQuery : IRequest<RecipeResponse>
    {
        /// <summary>
        /// Gets or sets the session ID associated with the recipe request.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the user prompt for generating the recipe.
        /// </summary>
        public string UserPrompt { get; set; }
    }
}