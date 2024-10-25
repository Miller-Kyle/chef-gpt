using ChefGpt.Application.RecipeGeneration.Commands;
using ChefGpt.Domain.Models;

namespace ChefGpt.Application.RecipeGeneration.Services
{
    public interface IGptService
    {
        Task<string> Send(GetRecipeQuery prompt, CancellationToken cancellationToken);
    }
}
