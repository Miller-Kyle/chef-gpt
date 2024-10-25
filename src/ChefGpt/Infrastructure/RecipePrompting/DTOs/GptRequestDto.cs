namespace ChefGpt.Infrastructure.RecipePrompting.DTOs
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class GptRequestDto
    {
        public IEnumerable<Message> Messages { get; set; }

        public double Temperature { get; } = 0.7;

        [JsonProperty("top_p")]
        public double TopP { get; } = 0.7;

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; } = 800;

        public bool Stream { get; } = false;
    }

    public class Message
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }

        public IEnumerable<Content> Content { get; set; }
    }

    public enum Role
    {
        system,
        user,
        assistant
    }

    public class Content
    {
        public string Type { get; } = "text";

        public string Text { get; set; }
    }
}
