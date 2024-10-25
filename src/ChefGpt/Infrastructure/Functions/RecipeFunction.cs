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
    /// <summary>
    /// Azure Function to handle recipe generation requests.
    /// </summary>
    public class RecipeFunction
    {
        private readonly ILogger<RecipeFunction> logger;
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeFunction"/> class.
        /// </summary>
        /// <param name="mediator">The mediator for sending commands and queries.</param>
        /// <param name="logger">The logger for logging information.</param>
        public RecipeFunction(IMediator mediator, ILogger<RecipeFunction> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <summary>
        /// Handles HTTP POST requests to generate a recipe based on user input.
        /// </summary>
        /// <param name="request">The HTTP request data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response data.</returns>
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

        /// <summary>
        /// Creates an error response with the specified error message.
        /// </summary>
        /// <param name="request">The HTTP request data.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response data.</returns>
        private static async Task<HttpResponseData> CreateErrorResponse(HttpRequestData request, string errorMessage)
        {
            var errorResponse = request.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync(errorMessage);
            return errorResponse;
        }

        /// <summary>
        /// Creates a success response with the specified recipe data.
        /// </summary>
        /// <param name="request">The HTTP request data.</param>
        /// <param name="recipe">The recipe response data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response data.</returns>
        private static async Task<HttpResponseData> CreateSuccessResponse(HttpRequestData request, RecipeResponse recipe)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var responseBody = JsonConvert.SerializeObject(new RecipeResponseDto { Recipe = recipe.Response, SessionId = recipe.SessionId, ImageUri = recipe.ImageUri });
            await response.WriteStringAsync(responseBody);

            return response;
        }

        /// <summary>
        /// Parses the request body to extract the recipe request data.
        /// </summary>
        /// <param name="requestBody">The request body as a string.</param>
        /// <returns>The parsed <see cref="RecipeRequestDto"/> object, or <c>null</c> if the request body is invalid.</returns>
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
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a recipe query to the mediator and returns the generated recipe response.
        /// </summary>
        /// <param name="recipeRequest">The recipe request data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="RecipeResponse"/>.</returns>
        private async Task<RecipeResponse> GetRecipeResponseAsync(RecipeRequestDto recipeRequest)
        {
            var query = new GetRecipeQuery { UserPrompt = recipeRequest.Prompt, SessionId = recipeRequest.SessionId };
            return await this.mediator.Send(query);
        }
    }
}