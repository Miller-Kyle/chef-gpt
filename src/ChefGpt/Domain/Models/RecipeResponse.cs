// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Domain.Models
{
    /// <summary>
    ///     Represents the response containing the generated recipe and associated metadata.
    /// </summary>
    public class RecipeResponse
    {
        /// <summary>
        ///     Gets or sets the URI of the image associated with the recipe.
        /// </summary>
        public Uri ImageUri { get; set; }

        /// <summary>
        ///     Gets or sets the generated recipe instructions.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        ///     Gets or sets the session ID associated with the recipe request.
        /// </summary>
        public string SessionId { get; set; }
    }
}