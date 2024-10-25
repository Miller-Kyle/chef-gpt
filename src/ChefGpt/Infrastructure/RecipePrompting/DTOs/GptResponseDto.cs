namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class GptResponseDto
    {
        public IEnumerable<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public ResponseMessage Message { get; set; }
    }

    public class ResponseMessage
    {
        public string Content { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
    }
}
