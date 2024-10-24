using ChefGpt.Domain.Models;

namespace ChefGpt.Application.RecipePrompting
{
    public interface IGptService
    {
        Task<RecipeResponse> Send(string prompt, CancellationToken cancellationToken);
    }
}
