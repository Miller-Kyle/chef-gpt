using ChefGpt.Application.RecipePrompting.Commands;
using ChefGpt.Domain.Models;

namespace ChefGpt.Application.RecipePrompting
{
    public interface IGptService
    {
        Task<string> Send(GetRecipeQuery prompt, CancellationToken cancellationToken);
    }
}
