namespace ChefGpt.Application.DTOs
{
    public class RecipeResponseDto
    {
        public Uri ImageUri { get; set; }

        public string Recipe { get; set; }

        public string SessionId { get; set; }
    }
}
