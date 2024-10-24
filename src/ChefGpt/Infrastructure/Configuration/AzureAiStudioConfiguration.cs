namespace ChefGpt.Infrastructure.Configuration
{
    internal class AzureAiStudioConfiguration
    {
        public string AuthorityUri { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Uri GptEndpoint { get; set; }

        public Uri DallEEndpoint { get; set; }

        public string Scope { get; set; }
    }
}
