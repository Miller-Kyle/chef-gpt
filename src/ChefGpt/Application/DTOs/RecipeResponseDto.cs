// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Application.DTOs
{
    /// <summary>
    ///     Data Transfer Object for Recipe Response.
    /// </summary>
    public class RecipeResponseDto
    {
        /// <summary>
        ///     Gets or sets the URI of the image associated with the recipe.
        /// </summary>
        public Uri ImageUri { get; set; }

        /// <summary>
        ///     Gets or sets the recipe details.
        /// </summary>
        public string Recipe { get; set; }

        /// <summary>
        ///     Gets or sets the session ID associated with the recipe request.
        /// </summary>
        public string SessionId { get; set; }
    }
}