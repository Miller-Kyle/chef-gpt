using ChefGpt.Application.RecipePrompting.Commands;
using ChefGpt.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Application.RecipePrompting.CommandHandlers
{
    public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeResponse>
    {
        private readonly IGptService gptService;

        private readonly ILogger<GetRecipeQueryHandler> logger;

        public GetRecipeQueryHandler(IGptService gptService, ILogger<GetRecipeQueryHandler> logger)
        {
            this.gptService = gptService;
            this.logger = logger;
        }

        public async Task<RecipeResponse> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation($"Handling request: {request}", request.UserPrompt);
            var response = await this.gptService.Send(request.UserPrompt, cancellationToken);
            return response;
        }
    }
}
