// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    /// <summary>
    ///     Data Transfer Object for GPT request.
    /// </summary>
    public class GptRequestDto
    {
        /// <summary>
        ///     Gets the maximum number of tokens for the GPT response.
        /// </summary>
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; } = 800;

        /// <summary>
        ///     Gets or sets the collection of messages to be sent to GPT.
        /// </summary>
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the response should be streamed.
        /// </summary>
        public bool Stream { get; } = false;

        /// <summary>
        ///     Gets the temperature setting for the GPT response, controlling the randomness.
        /// </summary>
        public double Temperature { get; } = 0.7;

        /// <summary>
        ///     Gets the top_p setting for the GPT response, controlling the diversity.
        /// </summary>
        [JsonProperty("top_p")]
        public double TopP { get; } = 0.7;
    }

    /// <summary>
    ///     Represents a message to be sent to GPT.
    /// </summary>
    public class Message
    {
        /// <summary>
        ///     Gets or sets the collection of content within the message.
        /// </summary>
        public IEnumerable<Content> Content { get; set; }

        /// <summary>
        ///     Gets or sets the role of the message sender.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
    }

    /// <summary>
    ///     Enum representing the role of the message sender.
    /// </summary>
    public enum Role
    {
        /// <summary>
        ///     The system role.
        /// </summary>
        system,

        /// <summary>
        ///     The user role.
        /// </summary>
        user,

        /// <summary>
        ///     The assistant role.
        /// </summary>
        assistant
    }

    /// <summary>
    ///     Represents the content of a message.
    /// </summary>
    public class Content
    {
        /// <summary>
        ///     Gets or sets the text content of the message.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Gets the type of the content, which is always "text".
        /// </summary>
        public string Type { get; } = "text";
    }
}