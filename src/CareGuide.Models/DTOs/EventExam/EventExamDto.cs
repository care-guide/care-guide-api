using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventExam
{
    public record EventExamDto(
        Guid Id,
        Guid EventId,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventExamType Type
    );
}
