using CareGuide.Models.Entities.Shared;

namespace CareGuide.Models.Entities
{
    public class Doctor : Entity, IPersonOwnedEntity
    {
        public required Guid? PersonId { get; set; }
        public required string Name { get; set; }
        public string? Details { get; set; }

        public Person Person { get; set; } = null!;
        public ICollection<DoctorPhone> DoctorPhones { get; set; } = new List<DoctorPhone>();
        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
        public ICollection<DoctorAddress> DoctorAddresses { get; set; } = new List<DoctorAddress>();
    }
}
