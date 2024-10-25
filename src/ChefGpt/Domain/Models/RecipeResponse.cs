namespace ChefGpt.Domain.Models
{
    public class RecipeResponse
    {
        public string Response { get; set; }

        public string SessionId { get; set; }

        public Uri ImageUri { get; set; }
    }
}
