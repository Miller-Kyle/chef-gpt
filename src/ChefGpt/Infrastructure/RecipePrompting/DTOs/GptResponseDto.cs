// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    /// <summary>
    ///     Data Transfer Object for GPT response.
    /// </summary>
    public class GptResponseDto
    {
        /// <summary>
        ///     Gets or sets the collection of choices returned by GPT.
        /// </summary>
        public IEnumerable<Choice> Choices { get; set; }
    }

    /// <summary>
    ///     Represents a choice in the GPT response.
    /// </summary>
    public class Choice
    {
        /// <summary>
        ///     Gets or sets the message content of the choice.
        /// </summary>
        public ResponseMessage Message { get; set; }
    }

    /// <summary>
    ///     Represents a message in the GPT response.
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        ///     Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the role of the message sender.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
    }
}