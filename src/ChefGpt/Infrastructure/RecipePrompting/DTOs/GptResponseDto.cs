// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    public class GptResponseDto
    {
        public IEnumerable<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public ResponseMessage Message { get; set; }
    }

    public class ResponseMessage
    {
        public string Content { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
    }
}