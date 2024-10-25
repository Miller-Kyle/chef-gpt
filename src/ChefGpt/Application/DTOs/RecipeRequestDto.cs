// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Application.DTOs
{
    public class RecipeRequestDto
    {
        public string Prompt { get; set; }

        public string SessionId { get; set; }
    }
}