using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventAppointment
{
    public record CreateEventAppointmentDto(
        string? Doctor,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventAppointmentType Type
    );
}
