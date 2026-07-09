namespace CareGuide.Models.DTOs.Address
{
    public record AddressDto(
        Guid Id,
        string PostalCode,
        int? Number,
        string? Type,
        string City,
        string? Complement,
        string Country,
        string? District,
        string? ReferencePoint,
        string State,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
