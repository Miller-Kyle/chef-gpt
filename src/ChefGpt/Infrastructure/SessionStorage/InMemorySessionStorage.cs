using ChefGpt.Application.Configuration;
using ChefGpt.Infrastructure.RecipePrompting.DTOs;
using Microsoft.Extensions.Configuration;

namespace ChefGpt.Infrastructure.SessionStorage
{
    public class InMemorySessionStorage : ISessionStorage
    {
        private Dictionary<string, GptRequestDto> history = new Dictionary<string, GptRequestDto>();
        private readonly GptConfiguration gptConfiguration = new GptConfiguration();

        public InMemorySessionStorage(IConfiguration configuration)
        {
            configuration.GetSection(ApplicationConfigurationKeys.GptConfiguration).Bind(this.gptConfiguration);
        }

        public GptRequestDto Add(string sessionId, Message message)
        {
            if (!this.history.ContainsKey(sessionId))
            {
                this.history[sessionId] = new GptRequestDto
                {
                    Messages = new List<Message>
                    {
                        new Message
                                {
                                    Role = Role.system,
                                    Content = new List<Content>
                                    {
                                        new Content
                                        {
                                            Text = this.gptConfiguration.SystemPrompt
                                        }
                                    }
                                }
                    }
                };
            }

            var session = this.history[sessionId];
            session.Messages = session.Messages.Append(message);

            return session;
        }

    }
}
