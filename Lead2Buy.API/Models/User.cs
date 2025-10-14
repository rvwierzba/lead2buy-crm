using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<CrmTask> Tasks { get; set; } = new();
        public List<ChatJob> ChatJobs { get; set; } = new();
    }
}