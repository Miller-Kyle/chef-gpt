// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    public class GptRequestDto
    {
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; } = 800;

        public IEnumerable<Message> Messages { get; set; }

        public bool Stream { get; } = false;

        public double Temperature { get; } = 0.7;

        [JsonProperty("top_p")]
        public double TopP { get; } = 0.7;
    }

    public class Message
    {
        public IEnumerable<Content> Content { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
    }

    public enum Role
    {
        system,

        user,

        assistant
    }

    public class Content
    {
        public string Text { get; set; }

        public string Type { get; } = "text";
    }
}