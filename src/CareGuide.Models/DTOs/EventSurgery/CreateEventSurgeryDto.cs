using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventSurgery
{
    public record CreateEventSurgeryDto(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventSurgeryType Type
    );
}
