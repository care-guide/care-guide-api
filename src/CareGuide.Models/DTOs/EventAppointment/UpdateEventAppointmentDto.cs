using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventAppointment
{
    public record UpdateEventAppointmentDto(
        string? Doctor,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventAppointmentType Type
    );
}
