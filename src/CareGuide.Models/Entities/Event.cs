using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;

namespace CareGuide.Models.Entities
{
    public class Event : Entity, IPersonOwnedEntity
    {
        public required Guid? PersonId { get; set; }
        public required string Name { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public required EventType Type { get; set; }
        public bool IsAllDay { get; set; } = false;
        public RecurrenceType? RecurrenceType { get; set; }
        public RecurrenceFrequency? RecurrenceFrequency { get; set; }
        public string? Details { get; set; }

        public Person Person { get; set; } = null!;
        public ICollection<EventFile> EventFiles { get; set; } = new List<EventFile>();
        public ICollection<EventAddress> EventAddresses { get; set; } = new List<EventAddress>();
    }
}
