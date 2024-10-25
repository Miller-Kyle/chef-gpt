using ChefGpt.Domain.Models;
using MediatR;

namespace ChefGpt.Application.RecipeGeneration.Commands
{
    public class GetRecipeQuery : IRequest<RecipeResponse>
    {
        public string UserPrompt { get; set; }

        public string SessionId { get; set; }
    }
}
