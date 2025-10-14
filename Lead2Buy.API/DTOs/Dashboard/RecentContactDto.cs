namespace Lead2Buy.API.DTOs.Dashboard
{
    public class RecentContactDto
    {
        public Guid Id { get; set; } = new Guid();
        public required string Name { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}