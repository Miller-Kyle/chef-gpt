// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

namespace ChefGpt.Domain.Models
{
    public class RecipeResponse
    {
        public Uri ImageUri { get; set; }

        public string Response { get; set; }

        public string SessionId { get; set; }
    }
}