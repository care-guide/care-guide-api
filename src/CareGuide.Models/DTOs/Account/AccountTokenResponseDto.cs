using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.Account
{
    public record AccountTokenResponseDto(
       Guid Id,
       string Email,
       string Name,
       [property: JsonConverter(typeof(JsonStringEnumConverter))] Gender Gender,
       DateOnly Birthday,
       string SessionToken,
       string RefreshToken
   );
}
