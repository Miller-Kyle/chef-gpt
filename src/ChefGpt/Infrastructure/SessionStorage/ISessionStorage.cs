// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using ChefGpt.Infrastructure.RecipePrompting.DTOs;

namespace ChefGpt.Infrastructure.SessionStorage
{
    public interface ISessionStorage
    {
        GptRequestDto Add(string sessionId, Message message);
    }
}