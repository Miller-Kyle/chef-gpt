// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using System.Text;

using ChefGpt.Application.RecipeGeneration.Query;
using ChefGpt.Application.RecipeGeneration.Services;
using ChefGpt.Infrastructure.Configuration;
using ChefGpt.Infrastructure.RecipePrompting.DTOs;
using ChefGpt.Infrastructure.SessionStorage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChefGpt.Infrastructure.RecipePrompting
{
    /// <summary>
    ///     Service for interacting with GPT to generate recipe instructions.
    /// </summary>
    public class GptService : IGptService
    {
        private readonly AzureAiStudioConfiguration aiStudioConfiguration = new AzureAiStudioConfiguration();

        private readonly HttpClient client;

        private readonly ILogger logger;

        private readonly ISessionStorage sessionStorage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GptService" /> class.
        /// </summary>
        /// <param name="sessionStorage">The session storage for storing and retrieving session data.</param>
        /// <param name="client">The HTTP client for sending requests to GPT.</param>
        /// <param name="logger">The logger for logging information.</param>
        /// <param name="configuration">The configuration for accessing settings.</param>
        public GptService(ISessionStorage sessionStorage, HttpClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.sessionStorage = sessionStorage;
            this.logger = logger;
            this.client = client;
            configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(this.aiStudioConfiguration);
        }

        /// <summary>
        ///     Sends a recipe query to GPT and returns the generated recipe instructions.
        /// </summary>
        /// <param name="recipeQuery">The recipe query containing the user prompt and session ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated recipe instructions.</returns>
        public async Task<string> Send(GetRecipeQuery recipeQuery, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Sending GPT Request: {Prompt}", recipeQuery.UserPrompt);

            var userMessage = this.CreateMessage(Role.user, recipeQuery.UserPrompt);
            var gptRequest = this.sessionStorage.Add(recipeQuery.SessionId, userMessage);
            var requestBody = this.SerializeRequestBody(gptRequest);

            var gptResponse = await this.SendGptRequestAsync(requestBody, cancellationToken);
            var recipeResponse = this.ParseGptResponse(gptResponse);

            this.SaveResponseToSession(recipeQuery.SessionId, recipeResponse);

            this.logger.LogInformation("Received Response: {Response}", gptResponse);

            return recipeResponse.Choices.First().Message.Content;
        }

        /// <summary>
        ///     Creates a message with the specified role and prompt.
        /// </summary>
        /// <param name="role">The role of the message sender.</param>
        /// <param name="prompt">The prompt content.</param>
        /// <returns>The created message.</returns>
        private Message CreateMessage(Role role, string prompt)
        {
            return new Message { Role = role, Content = new List<Content> { new Content { Text = prompt } } };
        }

        /// <summary>
        ///     Parses the GPT response data into a <see cref="GptResponseDto" /> object.
        /// </summary>
        /// <param name="responseData">The GPT response data as a string.</param>
        /// <returns>The parsed <see cref="GptResponseDto" /> object.</returns>
        private GptResponseDto ParseGptResponse(string responseData)
        {
            return JsonConvert.DeserializeObject<GptResponseDto>(responseData) ?? throw new InvalidOperationException("Failed to parse GPT response.");
        }

        /// <summary>
        ///     Saves the GPT response to the session storage.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <param name="gptResponse">The GPT response data.</param>
        private void SaveResponseToSession(string sessionId, GptResponseDto gptResponse)
        {
            var message = gptResponse.Choices.First().Message;
            if (message != null)
            {
                this.sessionStorage.Add(sessionId, this.CreateMessage(Role.assistant, message.Content));
            }
        }

        /// <summary>
        ///     Sends a GPT request asynchronously and returns the response as a string.
        /// </summary>
        /// <param name="requestBody">The request body as a JSON string.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the GPT response as a string.</returns>
        private async Task<string> SendGptRequestAsync(string requestBody, CancellationToken cancellationToken)
        {
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await this.client.PostAsync(this.aiStudioConfiguration.GptEndpoint.ToString(), requestContent, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <summary>
        ///     Serializes the GPT request object to a JSON string.
        /// </summary>
        /// <param name="request">The GPT request object.</param>
        /// <returns>The serialized JSON string.</returns>
        private string SerializeRequestBody(GptRequestDto request)
        {
            var serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(request, serializerSettings);
        }
    }
}