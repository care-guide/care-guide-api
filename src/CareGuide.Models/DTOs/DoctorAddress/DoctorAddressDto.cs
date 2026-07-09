namespace CareGuide.Models.DTOs.DoctorAddress
{
    public record DoctorAddressDto(
        Guid DoctorId,
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
