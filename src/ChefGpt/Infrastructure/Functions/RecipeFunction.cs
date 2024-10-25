// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using System.Net;

using ChefGpt.Application.DTOs;
using ChefGpt.Application.RecipeGeneration.Commands;
using ChefGpt.Domain.Models;

using MediatR;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace ChefGpt.Infrastructure.Functions
{
    public class RecipeFunction
    {
        private readonly ILogger<RecipeFunction> logger;

        private readonly IMediator mediator;

        public RecipeFunction(IMediator mediator, ILogger<RecipeFunction> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function("Prompt")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            this.logger.LogInformation("Recipe Prompt Function triggered");

            var requestBody = await request.ReadAsStringAsync();
            var recipeRequest = ParseRequestBody(requestBody);
            if (string.IsNullOrWhiteSpace(recipeRequest?.Prompt ?? null))
            {
                return await CreateErrorResponse(request, "Invalid request.");
            }

            var recipeResponse = await this.GetRecipeResponseAsync(recipeRequest);
            return await CreateSuccessResponse(request, recipeResponse);
        }

        private static async Task<HttpResponseData> CreateErrorResponse(HttpRequestData request, string errorMessage)
        {
            var errorResponse = request.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync(errorMessage);
            return errorResponse;
        }

        private static async Task<HttpResponseData> CreateSuccessResponse(HttpRequestData request, RecipeResponse recipe)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var responseBody = JsonConvert.SerializeObject(new RecipeResponseDto { Recipe = recipe.Response, SessionId = recipe.SessionId, ImageUri = recipe.ImageUri });
            await response.WriteStringAsync(responseBody);

            return response;
        }

        private static RecipeRequestDto ParseRequestBody(string requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return null;
            }

            try
            {
                var request = JsonConvert.DeserializeObject<RecipeRequestDto>(requestBody);

                if (string.IsNullOrEmpty(request.SessionId))
                {
                    request.SessionId = Guid.NewGuid().ToString();
                }

                return request;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private async Task<RecipeResponse> GetRecipeResponseAsync(RecipeRequestDto recipeRequest)
        {
            var query = new GetRecipeQuery { UserPrompt = recipeRequest.Prompt, SessionId = recipeRequest.SessionId };
            return await this.mediator.Send(query);
        }
    }
}