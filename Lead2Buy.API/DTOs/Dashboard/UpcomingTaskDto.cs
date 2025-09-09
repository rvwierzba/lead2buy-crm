namespace Lead2Buy.API.DTOs.Dashboard
{
    public class UpcomingTaskDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime DueDate { get; set; }
        public required string ContactName { get; set; }
    }
}