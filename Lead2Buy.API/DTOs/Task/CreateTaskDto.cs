using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Dtos.Task
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public Guid ContactId { get; set; } = new Guid();
    }
}