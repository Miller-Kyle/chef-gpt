namespace ChefGpt.Infrastructure.Configuration
{
    internal class AzureAiStudioConfiguration
    {
        public string ApiKey { get; set; }

        public Uri GptEndpoint { get; set; }

        public Uri DallEEndpoint { get; set; }
    }
}
