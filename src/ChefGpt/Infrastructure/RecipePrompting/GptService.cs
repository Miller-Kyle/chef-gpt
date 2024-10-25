// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using System.Text;

using ChefGpt.Application.RecipeGeneration.Commands;
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
    public class GptService : IGptService
    {
        private readonly AzureAiStudioConfiguration aiStudioConfiguration = new AzureAiStudioConfiguration();

        private readonly HttpClient client;

        private readonly ILogger logger;

        private readonly ISessionStorage sessionStorage;

        public GptService(ISessionStorage sessionStorage, HttpClient client, ILogger<GptService> logger, IConfiguration configuration)
        {
            this.sessionStorage = sessionStorage;
            this.logger = logger;
            this.client = client;
            configuration.GetSection(InfrastructureConfigurationKeys.AzureAiStudioConfiguration).Bind(this.aiStudioConfiguration);
        }

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

        private Message CreateMessage(Role role, string prompt)
        {
            return new Message { Role = role, Content = new List<Content> { new Content { Text = prompt } } };
        }

        private GptResponseDto ParseGptResponse(string responseData)
        {
            return JsonConvert.DeserializeObject<GptResponseDto>(responseData) ?? throw new InvalidOperationException("Failed to parse GPT response.");
        }

        private void SaveResponseToSession(string sessionId, GptResponseDto gptResponse)
        {
            var message = gptResponse.Choices.First().Message;
            if (message != null)
            {
                this.sessionStorage.Add(sessionId, this.CreateMessage(Role.assistant, message.Content));
            }
        }

        private async Task<string> SendGptRequestAsync(string requestBody, CancellationToken cancellationToken)
        {
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await this.client.PostAsync(this.aiStudioConfiguration.GptEndpoint.ToString(), requestContent, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        private string SerializeRequestBody(GptRequestDto request)
        {
            var serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(request, serializerSettings);
        }
    }
}