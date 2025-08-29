using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    // Enum para controlar o status do nosso job
    public enum JobStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }

    public class ChatJob
    {
        public int Id { get; set; }

        [Required]
        public string UserPrompt { get; set; } = string.Empty; // A pergunta do usuário

        public string? FilePath { get; set; } // O caminho para o arquivo salvo no servidor

        public JobStatus Status { get; set; } = JobStatus.Pending; // Começa como pendente

        public string? AiResponse { get; set; } // A resposta final da IA

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Chave Estrangeira para o Usuário que solicitou
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}