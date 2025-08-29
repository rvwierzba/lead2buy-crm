using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Dtos.Task
{
    public class UpdateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}