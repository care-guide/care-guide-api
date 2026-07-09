namespace CareGuide.Models.Entities
{
    public class EventAddress
    {
        public Guid EventId { get; set; }
        public Guid AddressId { get; set; }

        public Event Event { get; set; } = null!;
        public Address Address { get; set; } = null!;
    }
}
