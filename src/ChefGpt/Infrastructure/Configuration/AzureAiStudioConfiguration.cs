namespace ChefGpt.Infrastructure.Configuration
{
    internal class AzureAiStudioConfiguration
    {
        public string AuthorityUri { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Uri Endpoint { get; set; }

        public string TenantId { get; set; }

        public string Scope { get; set; }
    }
}
