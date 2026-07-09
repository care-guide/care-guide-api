using System.Text.Json.Serialization;
using CareGuide.Models.Enums;

namespace CareGuide.Models.DTOs.Event
{
    public record EventDto(
        Guid Id,
        Guid PersonId,
        string Name,
        DateTime StartDate,
        DateTime? EndDate,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventType Type,
        bool IsAllDay,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] RecurrenceType? RecurrenceType,
        int? RecurrenceFrequency,
        string? Details,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
