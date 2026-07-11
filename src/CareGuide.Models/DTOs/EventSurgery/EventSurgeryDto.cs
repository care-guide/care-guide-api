using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventSurgery
{
    public record EventSurgeryDto(
        Guid Id,
        Guid EventId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventSurgeryType Type
    );
}
