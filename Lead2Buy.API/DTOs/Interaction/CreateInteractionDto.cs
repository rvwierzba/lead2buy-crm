using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Dtos.Interaction
{
    public class CreateInteractionDto
    {
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Ex: "Ligação", "E-mail"

        [Required]
        public string Notes { get; set; } = string.Empty;
    }
}