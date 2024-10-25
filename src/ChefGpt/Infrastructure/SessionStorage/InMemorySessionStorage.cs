// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Application.Configuration;
using ChefGpt.Infrastructure.RecipePrompting.DTOs;
using Microsoft.Extensions.Configuration;

namespace ChefGpt.Infrastructure.SessionStorage
{
    /// <summary>
    /// In-memory implementation of session storage for GPT requests.
    /// </summary>
    public class InMemorySessionStorage : ISessionStorage
    {
        /// <summary>
        /// Configuration settings for GPT.
        /// </summary>
        private readonly GptConfiguration gptConfiguration = new GptConfiguration();

        /// <summary>
        /// Dictionary to store session history with session ID as the key.
        /// </summary>
        private readonly Dictionary<string, GptRequestDto> history = new Dictionary<string, GptRequestDto>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySessionStorage"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for accessing settings.</param>
        public InMemorySessionStorage(IConfiguration configuration)
        {
            configuration.GetSection(ApplicationConfigurationKeys.GptConfiguration).Bind(this.gptConfiguration);
        }

        /// <summary>
        /// Adds a message to the session history. If the session does not exist, it creates a new session with a system prompt.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <param name="message">The message to add to the session.</param>
        /// <returns>The updated <see cref="GptRequestDto"/> object for the session.</returns>
        public GptRequestDto Add(string sessionId, Message message)
        {
            if (!this.history.ContainsKey(sessionId))
            {
                this.history[sessionId] = new GptRequestDto
                {
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Role = Role.system,
                            Content = new List<Content> { new Content { Text = this.gptConfiguration.SystemPrompt } }
                        }
                    }
                };
            }

            var session = this.history[sessionId];
            session.Messages = session.Messages.Append(message);

            return session;
        }
    }
}