using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Source { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Endereço
        [MaxLength(9)]
        public string? Cep { get; set; }
        [MaxLength(200)]
        public string? Street { get; set; }
        [MaxLength(20)]
        public string? Number { get; set; }
        [MaxLength(100)]
        public string? Neighborhood { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }
        [MaxLength(2)]
        public string? State { get; set; }

        public string? Observations { get; set; }

        [Required]
        public string Status { get; set; } = "Lead"; // Começa como Lead por padrão

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Interaction> Interactions { get; set; } = new();
        public List<CrmTask> Tasks { get; set; } = new();
    }
}