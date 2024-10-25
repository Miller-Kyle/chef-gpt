// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Infrastructure.RecipePrompting.DTOs;

namespace ChefGpt.Infrastructure.SessionStorage
{
    /// <summary>
    /// Interface for session storage to manage GPT request sessions.
    /// </summary>
    public interface ISessionStorage
    {
        /// <summary>
        /// Adds a message to the session history. If the session does not exist, it creates a new session.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <param name="message">The message to add to the session.</param>
        /// <returns>The updated <see cref="GptRequestDto"/> object for the session.</returns>
        GptRequestDto Add(string sessionId, Message message);
    }
}