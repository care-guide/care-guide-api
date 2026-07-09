using CareGuide.Models.Entities.Shared;

namespace CareGuide.Models.Entities
{
    public class Address : Entity
    {
        public required string PostalCode { get; set; }
        public int? Number { get; set; }
        public string? Type { get; set; }
        public required string City { get; set; }
        public string? Complement { get; set; }
        public required string Country { get; set; }
        public string? District { get; set; }
        public string? ReferencePoint { get; set; }
        public required string State { get; set; }
    }
}
