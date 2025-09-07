using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Dtos.Chatbot
{
    public class ChatRequestDto
    {
        [Required]
        public string Prompt { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }
}