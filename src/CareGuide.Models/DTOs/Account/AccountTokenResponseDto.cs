using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.Account
{
    /// <summary>
    /// Response returned to native (non-browser) clients that cannot rely on HttpOnly cookies.
    /// Carries the same profile fields as <see cref="AccountResponseDto"/> plus the session and
    /// refresh tokens in the JSON body. Only returned when the caller explicitly identifies itself
    /// as a native client (see AuthClientType header). Browsers keep receiving <see cref="AccountResponseDto"/>
    /// with tokens exclusively in HttpOnly cookies.
    /// </summary>
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
