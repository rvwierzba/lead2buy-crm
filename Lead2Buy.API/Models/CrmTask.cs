using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class CrmTask
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; } // Data de Vencimento

        public bool IsCompleted { get; set; } = false; // Começa como não concluída

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Relacionamentos (Chaves Estrangeiras) ---

        // Tarefa associada a um Contato
        public int ContactId { get; set; }
        public Contact? Contact { get; set; }

        // Tarefa associada a um Usuário (quem deve executá-la)
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}