

namespace Lead2Buy.API.Models
{
    public class UserActivity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ContactId { get; set; }
        public DateTime OccurredAt { get; set; }
        public string ActionType { get; set; } = string.Empty; // ex: "StageChanged", "InteractionAdded", "TaskCompleted"
        public string? Details { get; set; }

        public User User { get; set; }
        public Contact Contact { get; set; }
    }
}