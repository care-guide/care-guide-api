using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventAppointment
{
    public record EventAppointmentDto(
        Guid Id,
        Guid EventId,
        string? Doctor,
        Guid? DoctorId,
        string? DoctorName,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventAppointmentType Type
    );
}
