using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.Address
{
    public record CreateAddressDto(
        string PostalCode,
        int? Number,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] AddressType Type,
        string City,
        string? Complement,
        string Country,
        string? District,
        string? ReferencePoint,
        string State
    );
}
