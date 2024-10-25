// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Application.DTOs
{
    /// <summary>
    /// Data transfer object for recipe requests.
    /// </summary>
    public class RecipeRequestDto
    {
        /// <summary>
        /// Gets or sets the prompt for the recipe request.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the session ID for the recipe request.
        /// </summary>
        public string SessionId { get; set; }
    }
}