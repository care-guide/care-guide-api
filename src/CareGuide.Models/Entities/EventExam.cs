using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;

namespace CareGuide.Models.Entities
{
    public class EventExam : Entity
    {
        public required Guid EventId { get; set; }
        public required EventExamType Type { get; set; }

        public Event Event { get; set; } = null!;
    }
}
