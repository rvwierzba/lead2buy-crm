using System.Text.Json.Serialization;

namespace Lead2Buy.API.Dtos.Chatbot
{
    public class OllamaMessageDto
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}