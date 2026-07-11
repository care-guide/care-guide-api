using System.Text.Json.Serialization;
using CareGuide.Models.Enums;

namespace CareGuide.Models.DTOs.Event
{
    public record UpdateEventDto(
        Guid Id,
        string Name,
        DateTime StartDate,
        DateTime? EndDate,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventType Type,
        bool IsAllDay,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] RecurrenceType? RecurrenceType,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] RecurrenceFrequency? RecurrenceFrequency,
        string? Details
    );
}
