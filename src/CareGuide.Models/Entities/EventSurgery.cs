using CareGuide.Models.Entities.Shared;

namespace CareGuide.Models.Entities
{
    public class EventSurgery : Entity
    {
        public required Guid EventId { get; set; }
        public string? Type { get; set; }

        public Event Event { get; set; } = null!;
    }
}
