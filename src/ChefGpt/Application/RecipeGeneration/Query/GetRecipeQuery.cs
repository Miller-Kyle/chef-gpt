// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Domain.Models;

using MediatR;

namespace ChefGpt.Application.RecipeGeneration.Commands
{
    public class GetRecipeQuery : IRequest<RecipeResponse>
    {
        public string SessionId { get; set; }

        public string UserPrompt { get; set; }
    }
}