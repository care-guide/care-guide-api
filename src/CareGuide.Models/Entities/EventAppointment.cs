using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;

namespace CareGuide.Models.Entities
{
    public class EventAppointment : Entity
    {
        public required Guid EventId { get; set; }
        public string? Doctor { get; set; }
        public Guid? DoctorId { get; set; }
        public required EventAppointmentType Type { get; set; }

        public Event Event { get; set; } = null!;
        public Doctor? DoctorRef { get; set; }
    }
}
