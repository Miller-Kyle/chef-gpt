using ChefGpt.Application.RecipeGeneration.Commands;

namespace ChefGpt.Application.RecipeGeneration.Services
{
    public interface IImageGenerationService
    {
        Task<Uri> GenerateImage(string recipe, CancellationToken cancellationToken);
    }
}
