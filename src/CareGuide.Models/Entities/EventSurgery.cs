using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;

namespace CareGuide.Models.Entities
{
    public class EventSurgery : Entity
    {
        public required Guid EventId { get; set; }
        public required EventSurgeryType Type { get; set; }

        public Event Event { get; set; } = null!;
    }
}
