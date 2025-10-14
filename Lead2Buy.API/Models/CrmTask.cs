using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class CrmTask
    {
        public Guid Id { get; set; }  

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; } // Data de vencimento

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Relacionamentos ---
        public Guid ContactId { get; set; }
        public Contact? Contact { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}