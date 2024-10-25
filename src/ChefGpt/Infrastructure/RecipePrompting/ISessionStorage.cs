namespace ChefGpt.Infrastructure.RecipePrompting
{
    using ChefGpt.Infrastructure.RecipePrompting.DTOs;

    public interface ISessionStorage
    {
        GptRequestDto Add(string sessionId, Message message);
    }
}
