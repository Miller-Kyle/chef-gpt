using ChefGpt.Domain.Models;
using MediatR;

namespace ChefGpt.Application.RecipePrompting.Commands
{
    public class GetRecipeQuery : IRequest<RecipeResponse>
    {
        public string UserPrompt { get; set; }

        public string SessionId { get; set; }
    }
}
