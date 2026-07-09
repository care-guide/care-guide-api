namespace CareGuide.Models.Entities
{
    public class DoctorAddress
    {
        public Guid DoctorId { get; set; }
        public Guid AddressId { get; set; }

        public Doctor Doctor { get; set; } = null!;
        public Address Address { get; set; } = null!;
    }
}
