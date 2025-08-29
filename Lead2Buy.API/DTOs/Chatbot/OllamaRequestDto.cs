using System.Text.Json.Serialization;

namespace Lead2Buy.API.Dtos.Chatbot
{
    public class OllamaRequestDto
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "phi4-mini";

        [JsonPropertyName("messages")]
        public List<OllamaMessageDto> Messages { get; set; } = new();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }
}