using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class Interaction
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Ex: "Ligação", "E-mail", "Reunião"

        public string Notes { get; set; } = string.Empty; // Anotações sobre a interação

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chave Estrangeira para o Contato ---
        public int ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}