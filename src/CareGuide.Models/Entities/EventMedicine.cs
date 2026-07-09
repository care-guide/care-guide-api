using CareGuide.Models.Entities.Shared;

namespace CareGuide.Models.Entities
{
    public class EventMedicine : Entity
    {
        public required Guid EventId { get; set; }
        public string? Dosage { get; set; }
        public string? Laboratory { get; set; }

        public Event Event { get; set; } = null!;
    }
}
