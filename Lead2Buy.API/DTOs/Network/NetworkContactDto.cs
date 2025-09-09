using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.DTOs.Network
{
    public class NetworkContactDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? OtherSocialUrl { get; set; }
        public string? Notes { get; set; }
        // --- NOVOS CAMPOS AQUI ---
        public string? FacebookUrl { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? YouTubeChannelUrl { get; set; }
    }

    public class CreateNetworkContactDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [MaxLength(150)]
        public string? Email { get; set; }
        
        // --- NOVOS CAMPOS AQUI ---
        [Url(ErrorMessage = "URL do Facebook inválida.")]
        public string? FacebookUrl { get; set; }

        [Phone(ErrorMessage = "Número de WhatsApp inválido.")]
        [MaxLength(20)]
        public string? WhatsAppNumber { get; set; }

        [Url(ErrorMessage = "URL do YouTube inválida.")]
        public string? YouTubeChannelUrl { get; set; }
        // --- FIM DOS NOVOS CAMPOS ---

        [Url(ErrorMessage = "URL do LinkedIn inválida.")]
        public string? LinkedInUrl { get; set; }
        
        [Url(ErrorMessage = "URL do Instagram inválida.")]
        public string? InstagramUrl { get; set; }
        
        [Url(ErrorMessage = "URL de outra rede social inválida.")]
        public string? OtherSocialUrl { get; set; }

        public string? Notes { get; set; }
    }
}