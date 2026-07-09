namespace CareGuide.Models.DTOs.Address
{
    public record CreateAddressDto(
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
