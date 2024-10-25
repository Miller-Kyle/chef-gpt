using ChefGpt.Infrastructure.RecipePrompting.DTOs;

namespace ChefGpt.Infrastructure.SessionStorage
{

    public interface ISessionStorage
    {
        GptRequestDto Add(string sessionId, Message message);
    }
}
