namespace CareGuide.Models.DTOs.EventAddress
{
    public record EventAddressDto(
        Guid EventId,
        Guid AddressId,
        string PostalCode,
        int? Number,
        string? Type,
        string City,
        string? Complement,
        string Country,
        string? District,
        string? ReferencePoint,
        string State
    );
}
