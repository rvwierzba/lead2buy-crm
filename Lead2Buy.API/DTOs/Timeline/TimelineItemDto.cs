
namespace Lead2Buy.API.DTOs.Timeline
{
    public class TimelineItemDto
    {
        public string Kind { get; set; } = string.Empty;   // "interaction", "task", "activity"
        public DateTime When { get; set; }
        public string? Title { get; set; }
        public string? Details { get; set; }
    }
}